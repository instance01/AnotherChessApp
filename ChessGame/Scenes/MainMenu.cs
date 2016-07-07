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
            game.spriteBatch.DrawString(game.content.font, "YOU", new Vector2(game.graphics.PreferredBackBufferWidth / 2 - game.content.font.MeasureString("YOU").X / 2, 100), Color.Black);
            game.spriteBatch.DrawString(game.content.font, "vs", new Vector2(game.graphics.PreferredBackBufferWidth / 2 - game.content.font.MeasureString("vs").X / 2, 120), Color.Black);
            humanBtn.draw();
            aiBtn.draw();
        }
    }
}
