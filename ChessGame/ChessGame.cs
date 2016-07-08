﻿using ChessGame.GameObjects;
using ChessGame.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChessGame
{
    public class ChessGame : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        GameCamera camera;
        public GameContent content;
        public GameScenes scenes;

        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        Ray raycast;
        ChessPieceObject raycastChessPieceObject;

        public List<BoardTileObject> boardTiles;
        public List<ChessPieceObject> chessPieces;

        Boolean debug = false;

        public ChessGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            camera = new GameCamera(this);
            content = new GameContent(this);
            scenes = new GameScenes(this);
            boardTiles = new List<BoardTileObject>();
            chessPieces = new List<ChessPieceObject>();
            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            content.loadContent();

            viewMatrix = Matrix.CreateLookAt(camera.cameraPosition, Vector3.Zero, Vector3.UnitY);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.PreferredBackBufferWidth / (float) graphics.PreferredBackBufferHeight, 1, 200);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 != 0)
                    {
                        CreateBoardTile(true, new Vector3(i * 2 - 7, 0, j * 2 - 7));
                    }
                    else
                    {
                        CreateBoardTile(false, new Vector3(i * 2 - 7, 0, j * 2 - 7));
                    }
                }
            }

            CreateChessPiece(true, content.TowerW, new Vector3(-7, 1, -7));
            CreateChessPiece(true, content.HorseW1, new Vector3(-7, 1, -5));
            CreateChessPiece(true, content.BishopW, new Vector3(-7, 1, -3));
            CreateChessPiece(true, content.QueenW, new Vector3(-7, 1, -1));
            CreateChessPiece(true, content.KingW, new Vector3(-7, 1, +1));
            CreateChessPiece(true, content.BishopW, new Vector3(-7, 1, +3));
            CreateChessPiece(true, content.HorseW2, new Vector3(-7, 1, +5));
            CreateChessPiece(true, content.TowerW, new Vector3(-7, 1, +7));
            for(int i = 0; i < 8; i++)
            {
                CreateChessPiece(true, content.PawnW, new Vector3(-5, 1, i * 2 - 7));
            }
            CreateChessPiece(false, content.TowerB, new Vector3(+7, 1, -7));
            CreateChessPiece(false, content.HorseB1, new Vector3(+7, 1, -5));
            CreateChessPiece(false, content.BishopB, new Vector3(+7, 1, -3));
            CreateChessPiece(false, content.QueenB, new Vector3(+7, 1, -1));
            CreateChessPiece(false, content.KingB, new Vector3(+7, 1, +1));
            CreateChessPiece(false, content.BishopB, new Vector3(+7, 1, +3));
            CreateChessPiece(false, content.HorseB2, new Vector3(+7, 1, +5));
            CreateChessPiece(false, content.TowerB, new Vector3(+7, 1, +7));
            for (int i = 0; i < 8; i++)
            {
                CreateChessPiece(false, content.PawnB, new Vector3(+5, 1, i * 2 - 7));
            }

            scenes.init();
            scenes.mainMenuScene.enabled = true;
            scenes.ingameScene.enabled = true; // TODO remove
        }

        protected override void UnloadContent()
        {

        }

        int count = 0;
        MouseState previousMouseState = new MouseState();
        KeyboardState previousKeyboardState = new KeyboardState();
        protected override void Update(GameTime gameTime)
        {
            count++;
            // Keyboard input
            KeyboardState currentKeyboardState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (previousKeyboardState.IsKeyUp(Keys.F3) && currentKeyboardState.IsKeyDown(Keys.F3))
            {
                debug = !debug;
            }
            previousKeyboardState = currentKeyboardState;

            // Mouse input
            MouseState currentMouseState = Mouse.GetState();
            bool click = currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
            bool clickHold = currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed;
            bool ingame = true;
            if(scenes.mainMenuScene.enabled || scenes.setupVsAIScene.enabled)
            {
                ingame = false;
                camera.rotateDelta(0.001F);
            }
            if (currentMouseState.LeftButton == ButtonState.Released && ingame && raycastChessPieceObject != null)
            {
                raycastChessPieceObject.position.Y = 1;
                raycastChessPieceObject.position.X = (float) (2D * Math.Round((raycastChessPieceObject.position.X + 1) / 2) - 1);
                raycastChessPieceObject.position.Z = (float) (2D * Math.Round((raycastChessPieceObject.position.Z + 1) / 2) - 1);
                raycastChessPieceObject.moveTo(raycastChessPieceObject.position);
                raycastChessPieceObject = null;
            }
            if (clickHold && ingame)
            {
                if(count % 2 == 1)
                {
                    raycast = FindWhereClicked(currentMouseState);
                    if(raycastChessPieceObject == null)
                    {
                        float dist = 100F;
                        ChessPieceObject tempB = null;
                        foreach (ChessPieceObject b in chessPieces)
                        {
                            BoundingBox box = b.boundingBox;
                            float? f = raycast.Intersects(b.boundingBox);
                            if (f != null)
                            {
                                if (f < dist)
                                {
                                    dist = (float)f;
                                    tempB = b;
                                }
                            }
                        }
                        if (tempB != null)
                        {
                            raycastChessPieceObject = tempB;
                            
                        }
                    } else
                    {
                        Vector3 position = raycast.Position;
                        Vector3 direction = raycast.Direction;
                        float zFactor = -position.Y / direction.Y;
                        Vector3 zeroWorldPoint = position + direction * zFactor;
                        zeroWorldPoint.Y += 1;
                        raycastChessPieceObject.moveTo(zeroWorldPoint);
                    }
                }
            }
            if (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed && ingame)
            {
                // move camera while holding middle mouse button
                float dx = previousMouseState.Position.X - currentMouseState.Position.X;
                camera.rotateDelta(dx / 100F);
            }
            scenes.setupVsAIScene.mouseHover(currentMouseState.Position.X, currentMouseState.Position.Y, click);
            scenes.mainMenuScene.mouseHover(currentMouseState.Position.X, currentMouseState.Position.Y, click);
            previousMouseState = currentMouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Reset from 2D so 3D stuff works
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // Draw 3D stuff
            scenes.drawScenes3D();
            base.Draw(gameTime);

            // Draw 2D stuff
            spriteBatch.Begin();
            if (debug)
            {
                if (raycastChessPieceObject != null)
                {
                    spriteBatch.DrawString(content.font, "Raycast Object: X~" + raycastChessPieceObject.position.X + " Y~" + raycastChessPieceObject.position.Y + " Z~" + raycastChessPieceObject.position.Z, new Vector2(10, 30), Color.Black);
                }
                spriteBatch.DrawString(content.font, "" + gameTime.ElapsedGameTime.TotalSeconds + " Raycast: X~" + raycast.Direction.X + " Y~" + raycast.Direction.Y + " Z~" + raycast.Direction.Z, new Vector2(10, 10), Color.Black);
            }
            scenes.drawScenes2D();
            spriteBatch.End();
        }

        protected void CreateBoardTile(Boolean white, Vector3 position)
        {
            Model model = white ? content.whiteTile : content.blackTile;
            boardTiles.Add((BoardTileObject) new BoardTileObject(this, model, position, white, UpdateBoundingBox(model, Matrix.CreateTranslation(position))).init());
        }

        protected void CreateChessPiece(Boolean white, Model model, Vector3 position)
        {
            chessPieces.Add((ChessPieceObject) new ChessPieceObject(this, model, position, white, UpdateBoundingBox(model, Matrix.CreateTranslation(position))).init());
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
