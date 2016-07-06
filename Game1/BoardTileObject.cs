using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class BoardTileObject
    {
        public Model model;
        public Vector3 previousPos;
        public Vector3 pos;

        public Boolean white;

        public BoundingBox boundingBox;

        public BoardTileObject(Model model, Vector3 position, Boolean white, BoundingBox box)
        {
            this.model = model;
            this.previousPos = position;
            this.pos = position;
            this.white = white;
            this.boundingBox = box;
        }

        public BoardTileObject init()
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.CreateTranslation(pos);
                }
                //mesh.Draw();
            }
            return this;
        }

        public void draw(Game1 game)
        {
            // Update bounding box whenever object has moved
            if (previousPos != pos)
            {
                boundingBox = game.UpdateBoundingBox(model, Matrix.CreateTranslation(pos));
                previousPos = pos;
            }
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.World = Matrix.CreateTranslation(pos);
                    effect.View = game.viewMatrix;
                    effect.Projection = game.projectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
