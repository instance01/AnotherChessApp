using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.Gui;

namespace ChessGame.Scenes
{
    public class SetupVsAI : Scene
    {
        GuiButton selectAiBtn;
        GuiButton startBtn;
        GuiButton selectColorBtn;

        public SetupVsAI(ChessGame game) : base(game)
        {
            selectAiBtn = new GuiButton(game, new Vector2(game.graphics.PreferredBackBufferWidth / 2 - 55, 100), game.content.select_ai, new Vector2(110, 30));
            startBtn = new GuiButton(game, new Vector2(game.graphics.PreferredBackBufferWidth / 2 - 90, 220), game.content.buttonAI_dark, new Vector2(180, 30));
            selectColorBtn = new GuiButton(game, new Vector2(game.graphics.PreferredBackBufferWidth / 2 - 55, 180), game.content.buttonSelectWhite_dark, new Vector2(110, 30));
        }

        public override void draw2D()
        {
            int width = game.graphics.PreferredBackBufferWidth;
            int height = game.graphics.PreferredBackBufferHeight;
            game.spriteBatch.Draw(game.content.transparentRectangle, new Rectangle(0, 0, width, height), Color.Black);
            game.spriteBatch.DrawString(game.content.font, "Setup vs AI", new Vector2(width / 2 - game.content.font.MeasureString("Setup vs AI").X / 2, 50), Color.White);
            game.spriteBatch.DrawString(game.content.font, "Elo", new Vector2(width / 2 - 200, 150), Color.White);
            game.spriteBatch.DrawString(game.content.font, "You're playing as ", new Vector2(width / 2 - 200, 180), Color.White);
            selectColorBtn.draw();
            selectAiBtn.draw();
            startBtn.draw();
        }

        public void mouseHover(float x, float y, bool click)
        {
            if (!enabled)
            {
                return;
            }
            if (startBtn.isCoordinateInObject(x, y))
            {
                startBtn.texture = game.content.buttonStart_light;
                if (click)
                {
                    game.startGame();
                }
            }
            else
            {
                startBtn.texture = game.content.buttonStart_dark;
                
            }

            if(selectColorBtn.isCoordinateInObject(x, y))
            {
                if (click)
                {
                    game.currentSideWhite = !game.currentSideWhite;
                    game.currentTurn = !game.currentTurn;
                }
                if (game.currentSideWhite)
                {
                    selectColorBtn.texture = game.content.buttonSelectWhite_light;
                }
                else
                {
                    selectColorBtn.texture = game.content.buttonSelectBlack_light;
                }
            } else
            {
                if (game.currentSideWhite)
                {
                    selectColorBtn.texture = game.content.buttonSelectWhite_dark;
                }
                else
                {
                    selectColorBtn.texture = game.content.buttonSelectBlack_dark;
                }
            }
        }
    }
}
