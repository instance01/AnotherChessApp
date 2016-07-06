﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameObjects
{
    class ChessPieceObject : GameObject
    {
        public Boolean white;

        public ChessPieceObject(ChessGame game, Model model, Vector3 position, Boolean white, BoundingBox box) : base(game, model, position, box, white ? game.whiteMatteTexture : game.blackMatteTexture)
        {
            this.white = white;
        }
    }
}