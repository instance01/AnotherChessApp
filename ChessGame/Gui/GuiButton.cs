using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Gui
{
    class GuiButton : GuiObject
    {
        Boolean enabled = true;
        

        public GuiButton(ChessGame game, Vector2 position, Texture2D texture, Vector2 size) : base(game, position, texture, size)
        {

        }

    }
}
