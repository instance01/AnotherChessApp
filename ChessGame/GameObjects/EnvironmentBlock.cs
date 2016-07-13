using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameObjects
{
    public class EnvironmentBlock : GameObject
    {

        public EnvironmentBlock(ChessGame game, Model model, Vector3 position, BoundingBox box, Texture2D texture) : base(game, model, position, box, texture)
        {
            i = new Random().Next(150);
        }

        int i = 0;
        float dYMax = 0F;
        float dY = 0F;
        public void update()
        { 
            if(i > 200)
            {
                dY = (float)(new Random().NextDouble() - 0.5D) / 100;
                i = new Random().Next(100);
                dYMax = (float)(new Random().NextDouble() - 0.5D);
            }

            if(position.Y < (1.5F + dYMax) && position.Y > (-1.5F + dYMax) && dY != 0F)
            {
                moveDelta(0F, dY, 0F);
            }

            i++;
        }

    }
}
