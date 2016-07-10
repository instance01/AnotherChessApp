using ChessGame.GameObjects;
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

        public GameCamera camera;
        public GameContent content;
        public GameScenes scenes;

        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        Ray raycast;
        ChessPieceObject raycastChessPieceObject;
        Vector3 raycastPreviousLocation;

        public List<BoardTileObject> boardTiles;
        public List<ChessPieceObject> chessPieces;

        Boolean debug = false;

        string[] xCoords = new string[8] { "1", "2", "3", "4", "5", "6", "7", "8" };
        string[] zCoords = new string[8] { "A", "B", "C", "D", "E", "F", "G", "H" };
        public string currentMove = "";
        public string currentCoord = "";
        public bool currentSideWhite = true;
        public bool currentTurn = true;

        ChessPieceObject kingW;
        ChessPieceObject kingB;

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

            CreateChessPiece(true, content.TowerW, new Vector3(-7, 1, -7), "T");
            CreateChessPiece(true, content.HorseW1, new Vector3(-7, 1, -5), "H");
            CreateChessPiece(true, content.BishopW, new Vector3(-7, 1, -3), "B");
            CreateChessPiece(true, content.QueenW, new Vector3(-7, 1, -1), "Q");
            kingW = CreateChessPiece(true, content.KingW, new Vector3(-7, 1, +1), "K");
            CreateChessPiece(true, content.BishopW, new Vector3(-7, 1, +3), "B");
            CreateChessPiece(true, content.HorseW2, new Vector3(-7, 1, +5), "H");
            CreateChessPiece(true, content.TowerW, new Vector3(-7, 1, +7), "T");
            for(int i = 0; i < 8; i++)
            {
                CreateChessPiece(true, content.PawnW, new Vector3(-5, 1, i * 2 - 7), "P");
            }
            CreateChessPiece(false, content.TowerB, new Vector3(+7, 1, -7), "T");
            CreateChessPiece(false, content.HorseB1, new Vector3(+7, 1, -5), "H");
            CreateChessPiece(false, content.BishopB, new Vector3(+7, 1, -3), "B");
            CreateChessPiece(false, content.QueenB, new Vector3(+7, 1, -1), "Q");
            kingB = CreateChessPiece(false, content.KingB, new Vector3(+7, 1, +1), "K");
            CreateChessPiece(false, content.BishopB, new Vector3(+7, 1, +3), "B");
            CreateChessPiece(false, content.HorseB2, new Vector3(+7, 1, +5), "H");
            CreateChessPiece(false, content.TowerB, new Vector3(+7, 1, +7), "T");
            for (int i = 0; i < 8; i++)
            {
                CreateChessPiece(false, content.PawnB, new Vector3(+5, 1, i * 2 - 7), "P");
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
                float y = 1;
                float x = (float) (2D * Math.Round((raycastChessPieceObject.position.X + 1) / 2) - 1);
                float z = (float) (2D * Math.Round((raycastChessPieceObject.position.Z + 1) / 2) - 1);
                if(x < -8 || x > 8 || z < -8 || z > 8)
                {
                    raycastChessPieceObject.moveTo(raycastPreviousLocation);
                } else
                {
                    string coord = zCoords[((int)z + 1) / 2 + 3] + xCoords[((int)x + 1) / 2 + 3]; ;
                    currentMove = currentCoord + coord;
                    string temp = currentCoord;
                    currentCoord = coord;
                    if(!isValidMove(raycastChessPieceObject, new Vector3(x, y, z), currentMove, raycastChessPieceObject.white))
                    {
                        raycastChessPieceObject.moveTo(raycastPreviousLocation);
                        currentCoord = temp;
                    }
                }
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
                            if((tempB.white && currentSideWhite) || (!tempB.white && !currentSideWhite) && currentTurn)
                            {
                                raycastPreviousLocation = tempB.position;
                                raycastChessPieceObject = tempB;
                                currentCoord = zCoords[((int)raycastChessPieceObject.position.Z + 1) / 2 + 3] + xCoords[((int)raycastChessPieceObject.position.X + 1) / 2 + 3];
                            }
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
                spriteBatch.DrawString(content.font, currentCoord + " " + currentMove, new Vector2(10, 50), Color.Black);
            }
            scenes.drawScenes2D();
            spriteBatch.End();
        }

        public void startGame()
        {
            scenes.setupVsAIScene.enabled = false;
            scenes.ingameScene.enabled = true;
            camera.resetRotation(currentSideWhite ? 0F : (float)Math.PI);
            // TODO start up UCIWrapper
        }

        protected void CreateBoardTile(Boolean white, Vector3 position)
        {
            Model model = white ? content.whiteTile : content.blackTile;
            boardTiles.Add((BoardTileObject) new BoardTileObject(this, model, position, white, UpdateBoundingBox(model, Matrix.CreateTranslation(position))).init());
        }

        protected ChessPieceObject CreateChessPiece(Boolean white, Model model, Vector3 position, String id)
        {
            ChessPieceObject obj = (ChessPieceObject) new ChessPieceObject(this, model, position, white, UpdateBoundingBox(model, Matrix.CreateTranslation(position)), id).init();
            chessPieces.Add(obj);
            return obj;
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

        public bool isValidMove(ChessPieceObject obj, Vector3 position, string move, bool white)
        {
            if (isValidMoveInternal(obj.id, move, white))
            {
                obj.moveTo(position);
                if (!isKingAttacked(obj, white))
                {
                    return true;
                }
            }
            return false;
        }

        public bool isValidMoveInternal(string id, string move, bool white)
        {
            string originChar = move.Substring(0, 1);
            string originNum = move.Substring(1, 1);
            string destinationChar = move.Substring(2, 1);
            string destinationNum = move.Substring(3, 1);
            int originX = Array.IndexOf(xCoords, originNum);
            int destinationX = Array.IndexOf(xCoords, destinationNum);
            int originY = Array.IndexOf(zCoords, originChar);
            int destinationY = Array.IndexOf(zCoords, destinationChar);

            Console.WriteLine(move + " " + originX + " " + originY + " " + destinationX + " " + destinationY);

            return isValidMoveInternal(id, originX, originY, destinationX, destinationY, white);
        }

        public bool isValidMoveInternal(string id, int originX, int originY, int destinationX, int destinationY, bool white)
        {
            // TODO maybe refactor into Coordinate class?
            if(originX == destinationX && originY == destinationY)
            {
                return false;
            }
            if (hasFigure(destinationY, destinationX, true, !white))
            {
                return false;
            }

            if (id == "T")
            {
                return checkTower(originX, originY, destinationX, destinationY);
            }
            else if (id == "H")
            {
                if (Math.Abs(originY - destinationY) == 2 && Math.Abs(originX - destinationX) == 1)
                {
                    return true;
                }
                if (Math.Abs(originY - destinationY) == 1 && Math.Abs(originX - destinationX) == 2)
                {
                    return true;
                }
            }
            else if (id == "B")
            {
                return checkBishop(originX, originY, destinationX, destinationY);
            }
            else if (id == "Q")
            {
                bool t = checkTower(originX, originY, destinationX, destinationY);
                bool b = checkBishop(originX, originY, destinationX, destinationY);
                return (t || b);
            }
            else if (id == "K")
            {
                int yvec = Math.Abs(originY - destinationY);
                int xvec = Math.Abs(originX - destinationX);
                if (yvec <= 1 && xvec <= 1)
                {
                    // TODO isKingAttacked() at destination?
                    return true;
                }
                // TODO castle
            }
            else if (id == "P")
            {
                if (white)
                {
                    if (destinationX < originX)
                    {
                        return false;
                    }
                }
                else
                {
                    if (destinationX > originX)
                    {
                        return false;
                    }
                }
                int xdist = Math.Abs(destinationY - originY);
                int ydist = Math.Abs(destinationX - originX);
                // Console.WriteLine("#####" + ydist + " " + xdist);
                if (((white && originX == 1) || (!white && originX == 6)) && ydist == 2)
                {
                    return !isFigureBetween(originX, originY, destinationX, destinationY);
                }
                if (xdist == 0 && ydist == 1 && !hasFigure(destinationX, destinationY))
                {
                    return true;
                }
                if (xdist == 1 && ydist == 1 && hasFigure(destinationX, destinationY, true, white))
                {
                    return true;
                }
            }

            return false;
        }

        // Copied from my other chess program
        // TODO rewrite with better readability
        // oy and ox is origin, cy and cx is destination
        public Boolean isFigureBetween(int oy, int ox, int cy, int cx)
        {
            Boolean isDiagonal = true;
            int xvec = cx - ox;
            int yvec = cy - oy;
            int xyvec = Math.Abs(xvec) - Math.Abs(yvec);
            if ((xvec == 0 && yvec != 0) || (xvec != 0 && yvec == 0))
            {
                isDiagonal = false;
            }

            int xbase = 0;
            int ybase = 0;
            if (ox > cx)
            {
                xbase = cx;
            }
            else
            {
                xbase = ox;
            }
            if (oy > cy)
            {
                ybase = cy;
            }
            else
            {
                ybase = oy;
            }
            for (int i = 0; i <= Math.Abs(ox - cx); i++)
            {
                for (int j = 0; j <= Math.Abs(oy - cy); j++)
                {
                    int ccx = xbase + i;
                    int ccy = ybase + j;
                    if ((ccx == ox && ccy == oy) || (ccx == cx && ccy == cy))
                    {
                        continue;
                    }
                    int xvec_ = cx - ccx;
                    int yvec_ = cy - ccy;
                    int xyvec_ = Math.Abs(xvec_) - Math.Abs(yvec_);
                    // Console.WriteLine(xyvec_ + " ##");
                    if (!isDiagonal)
                    {
                        if (xvec_ == xvec || yvec_ == yvec)
                        {
                            if (hasFigure(ccx, ccy))
                            {
                                return true;
                            }
                        }
                        continue;
                    }
                    if (xyvec == xyvec_)
                    {
                        if (hasFigure(ccx, ccy))
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        public bool hasFigure(int ccX, int ccY, bool checkForColor = false, bool white = false)
        {
            foreach(ChessPieceObject obj in chessPieces)
            {
                if(obj == raycastChessPieceObject)
                {
                    continue;
                }
                if (checkForColor)
                {
                    if(obj.white == white)
                    {
                        return false;
                    }
                }
                int y = (int)((obj.position.X + 1) / 2 + 3);
                int x = (int)((obj.position.Z + 1) / 2 + 3);
                //Console.WriteLine(obj.position.X + " " + obj.position.Z);
                //Console.WriteLine(">>> " + ccX + " " + ccY + " = " + x + " " + y);
                if(ccX == x && ccY == y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool checkBishop(int originX, int originY, int destinationX, int destinationY)
        {
            int xvec = Math.Abs(destinationX - originX);
            int yvec = Math.Abs(destinationY - originY);
            int xyvec = Math.Abs(xvec) - Math.Abs(yvec);
            if (xyvec == 0)
            {
                return !isFigureBetween(originX, originY, destinationX, destinationY);
            }
            return false;
        }

        public bool checkTower(int originX, int originY, int destinationX, int destinationY)
        {
            if ((originX == destinationX && originY != destinationY) || (originX != destinationX && originY == destinationY))
            {
                return !isFigureBetween(originX, originY, destinationX, destinationY);
            }
            return false;
        }

        public bool isKingAttacked(ChessPieceObject movedObj, bool white)
        {
            ChessPieceObject king = white ? kingW : kingB;

            int kingY = (int)((king.position.X + 1) / 2 + 3);
            int kingX = (int)((king.position.Z + 1) / 2 + 3);

            foreach (ChessPieceObject obj in chessPieces)
            {
                if(obj.white != white)
                {
                    int y = (int)((obj.position.X + 1) / 2 + 3);
                    int x = (int)((obj.position.Z + 1) / 2 + 3);
                    if (isValidMoveInternal(obj.id, x, y, kingX, kingY, white))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
