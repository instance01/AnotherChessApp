using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Gui
{
    class GuiObject
    {
        ChessGame game;
        public Vector2 position;
        public Texture2D texture;
        public Vector2 size;

        public GuiObject(ChessGame game, Vector2 position, Texture2D texture, Vector2 size)
        {
            this.game = game;
            this.position = position;
            this.texture = texture;
            this.size = size;
        }

        public virtual void draw()
        {
            game.spriteBatch.Draw(texture, new Rectangle((int) position.X, (int) position.Y, (int) size.X, (int) size.Y), Color.White);
        }

        public virtual Boolean isCoordinateInObject(float x, float y)
        {
            return x > position.X && x < position.X + size.X && y > position.Y && y < position.Y + size.Y;
        }

    }
}
