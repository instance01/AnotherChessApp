using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessGame.GameObjects
{
    public class BoardTileObject : GameObject
    {
        public Boolean white;

        public BoardTileObject(ChessGame game, Model model, Vector3 position, Boolean white, BoundingBox box) : base(game, model, position, box, null)
        {
            this.white = white;
        }
    }
}
