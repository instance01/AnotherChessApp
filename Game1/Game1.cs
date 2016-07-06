using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Texture2D whiteMatteTexture, blackMatteTexture;

        Model whiteTile;
        Model blackTile;
        Model TowerW, HorseW1, HorseW2, BishopW, QueenW, KingW;
        SpriteFont font;

        float camdx, camdy, camdz;
        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        Ray raycast;
        BoardTileObject raycastBoardTileObject;

        Vector3 cameraPosition = new Vector3(-15, 20, 5);

        List<BoardTileObject> boardTiles;
        List<ChessPieceObject> chessPieces;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            boardTiles = new List<BoardTileObject>();
            chessPieces = new List<ChessPieceObject>();
            camdx = camdz = 1.1F;
            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            whiteMatteTexture = Content.Load<Texture2D>("white_matte");
            blackMatteTexture = Content.Load<Texture2D>("black_matte");

            whiteTile = Content.Load<Model>("whiteTile");
            blackTile = Content.Load<Model>("blackTile");
            TowerW = Content.Load<Model>("TowerW_bauhaus");
            HorseW1 = Content.Load<Model>("HorseW1_bauhaus");
            HorseW2 = Content.Load<Model>("HorseW2_bauhaus");
            BishopW = Content.Load<Model>("BishopW_bauhaus");
            QueenW = Content.Load<Model>("QueenW_bauhaus");
            KingW = Content.Load<Model>("KingW_bauhaus");
            font = Content.Load<SpriteFont>("Font");

            viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight, 1, 200);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 != 0)
                    {
                        CreateBoardTile(true, new Vector3(i * 2 - 8, 0, j * 2 - 8));
                    }
                    else
                    {
                        CreateBoardTile(false, new Vector3(i * 2 - 8, 0, j * 2 - 8));
                    }

                }
            }

            CreateChessPiece(true, TowerW, new Vector3(-8, 1, -8));
            CreateChessPiece(true, HorseW1, new Vector3(-8, 1, -6));
            CreateChessPiece(true, BishopW, new Vector3(-8, 1, -4));
            CreateChessPiece(true, QueenW, new Vector3(-8, 1, -2));
            CreateChessPiece(true, KingW, new Vector3(-8, 1, 0));
            CreateChessPiece(true, BishopW, new Vector3(-8, 1, +2));
            CreateChessPiece(true, HorseW2, new Vector3(-8, 1, +4));
            CreateChessPiece(true, TowerW, new Vector3(-8, 1, +6));
        }

        protected override void UnloadContent()
        {

        }

        MouseState previousMouseState = new MouseState();
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO remove camera test again
            //camdx = (float) Math.Pow(camdx, 1.1D);
            //camdz = (float) Math.Pow(camdz, 1.1D);
            camdx += 0.1F;
            camdz += 0.1F;
            cameraPosition = new Vector3(-15 + camdx, 20, 5 + camdz);
            viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                raycast = FindWhereClicked(Mouse.GetState());
                float dist = 100F;
                BoardTileObject tempB = null;
                foreach(BoardTileObject b in boardTiles)
                {
                    BoundingBox box = b.boundingBox;
                    float? f = raycast.Intersects(b.boundingBox);
                    if (f != null)
                    {
                        if(f < dist)
                        {
                            dist = (float) f;
                            tempB = b;
                        }
                    }
                }
                if(tempB != null)
                {
                    raycastBoardTileObject = tempB;
                    raycastBoardTileObject.pos.Y += 0.5F;
                }
            }

            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Just some debug text messages, will remove later
            spriteBatch.Begin();
            if (raycastBoardTileObject != null)
            {
                spriteBatch.DrawString(font, "Raycast Object: X~" + raycastBoardTileObject.pos.X + " Y~" + raycastBoardTileObject.pos.Y + " Z~" + raycastBoardTileObject.pos.Z, new Vector2(10, 30), Color.Black);
            }
            spriteBatch.DrawString(font, "" + gameTime.ElapsedGameTime.TotalSeconds + " Raycast: X~" + raycast.Direction.X + " Y~" + raycast.Direction.Y + " Z~" + raycast.Direction.Z, new Vector2(10, 10), Color.Black);
            spriteBatch.End();
            
            // Reset GraphicsDevice so the 3d scene works
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            
            foreach(BoardTileObject obj in boardTiles)
            {
                obj.draw(this);
            }

            foreach(ChessPieceObject obj in chessPieces)
            {
                obj.draw(this);
            }

            base.Draw(gameTime);
        }

        protected void CreateBoardTile(Boolean white, Vector3 position)
        {
            Model model = white ? whiteTile : blackTile;
            boardTiles.Add(new BoardTileObject(model, position, white, UpdateBoundingBox(model, Matrix.CreateTranslation(position))).init());
        }

        protected void CreateChessPiece(Boolean white, Model model, Vector3 position)
        {
            chessPieces.Add(new ChessPieceObject(model, position, white, UpdateBoundingBox(model, Matrix.CreateTranslation(position))).init());
        }

        public Ray FindWhereClicked(MouseState ms)
        {
            Vector3 nearScreenPoint = new Vector3(ms.X, ms.Y, 0);
            Vector3 farScreenPoint = new Vector3(ms.X, ms.Y, 1);
            Vector3 nearWorldPoint = GraphicsDevice.Viewport.Unproject(nearScreenPoint, projectionMatrix, viewMatrix, Matrix.Identity);
            Vector3 farWorldPoint = GraphicsDevice.Viewport.Unproject(farScreenPoint, projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 direction = farWorldPoint - nearWorldPoint;
            direction.Normalize();

            return new Ray(nearWorldPoint, direction);
        }

        // http://gamedev.stackexchange.com/questions/2438/how-do-i-create-bounding-boxes-with-xna-4-0
        public BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);
                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            return new BoundingBox(min, max);
        }
    }
}
