using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.Gui;
using Microsoft.Xna.Framework;

namespace ChessGame.Scenes
{
    public class MainMenu : Scene
    {
        GuiButton humanBtn;
        GuiButton aiBtn;

        public MainMenu(ChessGame game) : base(game)
        {
            humanBtn = new GuiButton(game, new Vector2(game.graphics.PreferredBackBufferWidth / 2 - 90, 150), game.content.buttonHuman_dark, new Vector2(180, 30));
            aiBtn = new GuiButton(game, new Vector2(game.graphics.PreferredBackBufferWidth / 2 - 90, 190), game.content.buttonAI_dark, new Vector2(180, 30));
        }

        public override void draw2D()
        {
            int width = game.graphics.PreferredBackBufferWidth;
            int height = game.graphics.PreferredBackBufferHeight;
            game.spriteBatch.Draw(game.content.transparentRectangle, new Rectangle(0, 0, width, height), Color.Black);
            game.spriteBatch.DrawString(game.content.font, "YOU", new Vector2(width / 2 - game.content.font.MeasureString("YOU").X / 2, 90), Color.White);
            game.spriteBatch.DrawString(game.content.font, "vs", new Vector2(width / 2 - game.content.font.MeasureString("vs").X / 2, 110), Color.White);
            humanBtn.draw();
            aiBtn.draw();
        }

        public void mouseHover(float x, float y, bool click)
        {
            if (!enabled)
            {
                return;
            }
            if(humanBtn.isCoordinateInObject(x, y))
            {
                humanBtn.texture = game.content.buttonHuman_light;
                if (click)
                {
                    // TODO multiplayer
                }
            } else
            {
                humanBtn.texture = game.content.buttonHuman_dark;
            }

            if (aiBtn.isCoordinateInObject(x, y))
            {
                aiBtn.texture = game.content.buttonAI_light;
                if (click)
                {
                    game.scenes.mainMenuScene.enabled = false;
                    game.scenes.setupVsAIScene.enabled = true;
                }
            }
            else
            {
                aiBtn.texture = game.content.buttonAI_dark;
            }
        }
    }
}
