using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scenes
{
    public class Scene
    {
        public ChessGame game;
        public Boolean enabled = false;
        public Scene(ChessGame game)
        {
            this.game = game;
        }

        public virtual void draw2D()
        {

        }

        public virtual void draw3D()
        {

        }

    }
}
