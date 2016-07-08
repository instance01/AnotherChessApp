using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    class Animation
    {
        bool enabled = false;
        Dictionary<int, Vector3> keys = new Dictionary<int, Vector3>();

        public Animation()
        {

        }

        int elapsedTime = 0;
        public void update(int i)
        {
            if (!enabled)
            {
                return;
            }

            elapsedTime += i;
            foreach(int time in keys.Keys)
            {

            }
        }

    }
}
