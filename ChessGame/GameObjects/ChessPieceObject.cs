using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameObjects
{
    public class ChessPieceObject : GameObject
    {
        public Boolean white;
        public String id = "";

        public ChessPieceObject(ChessGame game, Model model, Vector3 position, Boolean white, BoundingBox box, String id) : base(game, model, position, box, white ? game.content.whiteMatteTexture : game.content.blackMatteTexture)
        {
            this.white = white;
            this.id = id;
        }
    }
}
