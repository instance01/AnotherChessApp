using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Animations
{
    public class Animation
    {
        ChessGame game;
        public bool enabled = false;
        // Dictionary<int, Vector3> keys = new Dictionary<int, Vector3>();

        public Animation(ChessGame game)
        {
            this.game = game;
        }

    }
}
