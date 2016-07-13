using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    class KeyFrame
    {

        public int index;
        public int time;
        public Vector3 pos;

        public KeyFrame(int index, int time, Vector3 pos)
        {
            this.index = index;
            this.time = time;
            this.pos = pos;
        }

    }
}
