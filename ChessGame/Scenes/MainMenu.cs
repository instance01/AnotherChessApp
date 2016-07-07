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
            humanBtn = new GuiButton(game, new Vector2(game.graphics.PreferredBackBufferWidth / 2 - 90, 50), game.content.buttonHuman_dark, new Vector2(180, 30));
            aiBtn = new GuiButton(game, new Vector2(game.graphics.PreferredBackBufferWidth / 2 - 90, 100), game.content.buttonAI_dark, new Vector2(180, 30));
        }

        public override void draw2D()
        {
            humanBtn.draw();
            aiBtn.draw();
        }
    }
}
