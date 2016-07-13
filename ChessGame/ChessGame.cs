using ChessGame.GameObjects;
using ChessGame.Scenes;
using ChessGame.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UCIWrapper;

namespace ChessGame
{
    public class ChessGame : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        Wrapper uciwrapper;

        public GameCamera camera;
        public GameContent content;
        public GameScenes scenes;
        public ChessUtil chessUtil;
        public GameUtil util;

        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        Ray raycast;
        public ChessPieceObject raycastChessPieceObject;
        Vector3 raycastPreviousLocation;

        public List<BoardTileObject> boardTiles;
        public List<ChessPieceObject> chessPieces;
        public List<EnvironmentBlock> envBlocks;

        Boolean debug = false;

        public string[] xCoords = new string[8] { "1", "2", "3", "4", "5", "6", "7", "8" };
        public string[] zCoords = new string[8] { "a", "b", "c", "d", "e", "f", "g", "h" };
        public string currentMove = "";
        public string currentCoord = "";
        public bool currentSideWhite = true;
        public bool currentTurn = true;
        public string allMoves = "";

        public ChessPieceObject kingW;
        public ChessPieceObject kingB;

        public ChessGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            uciwrapper = new Wrapper("stockfish 7 x64.exe");
            camera = new GameCamera(this);
            content = new GameContent(this);
            scenes = new GameScenes(this);
            chessUtil = new ChessUtil(this);
            util = new GameUtil(this);
            boardTiles = new List<BoardTileObject>();
            chessPieces = new List<ChessPieceObject>();
            envBlocks = new List<EnvironmentBlock>();
            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            content.loadContent();

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
            viewMatrix = Matrix.CreateLookAt(camera.cameraPosition, Vector3.Zero, Vector3.UnitY);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.PreferredBackBufferWidth / (float) graphics.PreferredBackBufferHeight, 1, 200);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 != 0)
                    {
                        util.CreateBoardTile(true, new Vector3(i * 2 - 7, 0, j * 2 - 7));
                    }
                    else
                    {
                        util.CreateBoardTile(false, new Vector3(i * 2 - 7, 0, j * 2 - 7));
                    }
                }
            }

            // TODO test
            util.CreateEnvironmentBlock(new Vector3(-9, 0, -9));

            util.CreateChessPiece(true, content.TowerW, new Vector3(-7, 1, -7), "T");
            util.CreateChessPiece(true, content.HorseW1, new Vector3(-7, 1, -5), "H");
            util.CreateChessPiece(true, content.BishopW, new Vector3(-7, 1, -3), "B");
            util.CreateChessPiece(true, content.QueenW, new Vector3(-7, 1, -1), "Q");
            kingW = util.CreateChessPiece(true, content.KingW, new Vector3(-7, 1, +1), "K");
            util.CreateChessPiece(true, content.BishopW, new Vector3(-7, 1, +3), "B");
            util.CreateChessPiece(true, content.HorseW2, new Vector3(-7, 1, +5), "H");
            util.CreateChessPiece(true, content.TowerW, new Vector3(-7, 1, +7), "T");
            for(int i = 0; i < 8; i++)
            {
                util.CreateChessPiece(true, content.PawnW, new Vector3(-5, 1, i * 2 - 7), "P");
            }
            util.CreateChessPiece(false, content.TowerB, new Vector3(+7, 1, -7), "T");
            util.CreateChessPiece(false, content.HorseB1, new Vector3(+7, 1, -5), "H");
            util.CreateChessPiece(false, content.BishopB, new Vector3(+7, 1, -3), "B");
            util.CreateChessPiece(false, content.QueenB, new Vector3(+7, 1, -1), "Q");
            kingB = util.CreateChessPiece(false, content.KingB, new Vector3(+7, 1, +1), "K");
            util.CreateChessPiece(false, content.BishopB, new Vector3(+7, 1, +3), "B");
            util.CreateChessPiece(false, content.HorseB2, new Vector3(+7, 1, +5), "H");
            util.CreateChessPiece(false, content.TowerB, new Vector3(+7, 1, +7), "T");
            for (int i = 0; i < 8; i++)
            {
                util.CreateChessPiece(false, content.PawnB, new Vector3(+5, 1, i * 2 - 7), "P");
            }

            scenes.init();
            scenes.mainMenuScene.enabled = true;
            scenes.ingameScene.enabled = true;
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
                    string coord = zCoords[((int)z + 1) / 2 + 3] + xCoords[((int)x + 1) / 2 + 3];
                    currentMove = currentCoord + coord;
                    string temp = currentCoord;
                    currentCoord = coord;
                    if(!chessUtil.isValidMove(raycastChessPieceObject, new Vector3(x, y, z), currentMove))
                    {
                        raycastChessPieceObject.moveTo(raycastPreviousLocation);
                        currentCoord = temp;
                    } else
                    {
                        currentSideWhite = !currentSideWhite;
                        allMoves = allMoves.Length < 1 ? currentMove : (allMoves + " " + currentMove);
                        uciwrapper.sendUCICommand("position startpos moves " + allMoves);
                        uciwrapper.sendUCICommand("go");
                    }
                }
                raycastChessPieceObject = null;
            }
            if (clickHold && ingame)
            {
                if(count % 2 == 1)
                {
                    raycast = util.FindWhereClicked(currentMouseState);
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

            foreach(EnvironmentBlock envBlock in envBlocks)
            {
                envBlock.update();
            }

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
            uciwrapper.startEngine();
            uciwrapper.OnNewMoveEvent += new Wrapper.NewMoveEventHandler(newMoveEvent);
            uciwrapper.OnNewEvent += new Wrapper.NewEventHandler(newEvent);
            uciwrapper.sendUCICommand("uci");
            uciwrapper.sendUCICommand("setoption name Skill Level value 1"); // 2 is 1420 elo according to LiChess, setting purposely so low to test out whether that works
            if(!currentSideWhite && !currentTurn)
            {
                uciwrapper.sendUCICommand("go");
            }
        }

        private void newMoveEvent(object sender, Wrapper.MoveEventArgs e)
        {
            string move = e.Move.Substring(0, 4);
            allMoves = allMoves.Length < 1 ? move : (allMoves + " " + move);

            string originChar = move.Substring(0, 1);
            string originNum = move.Substring(1, 1);
            string destinationChar = move.Substring(2, 1);
            string destinationNum = move.Substring(3, 1);
            int originX = Array.IndexOf(xCoords, originNum);
            int destinationX = Array.IndexOf(xCoords, destinationNum);
            int originY = Array.IndexOf(zCoords, originChar);
            int destinationY = Array.IndexOf(zCoords, destinationChar);

            ChessPieceObject obj_ = chessUtil.getFigure(destinationY, destinationX);
            if (obj_ != null)
            {
                chessPieces.Remove(obj_);
            }

            ChessPieceObject obj = chessUtil.getFigure(originY, originX);
            if(obj != null)
            {
                if (obj.id == "K")
                {
                    bool castle = chessUtil.castle(originX, originY, destinationX, destinationY, obj.white);
                    Console.WriteLine(" > " + castle);
                }
                int x = 2 * (destinationX - 3) - 1;
                int z = 2 * (destinationY - 3) - 1;
                obj.moveTo(new Vector3(x, 1, z));

            } else
            {
                Console.WriteLine("Something just broke");
            }

            currentSideWhite = !currentSideWhite;
        }

        private void newEvent(object sender, Wrapper.NewEventArgs e)
        {
            // Console.WriteLine(e.Line);
        }

    }
}
