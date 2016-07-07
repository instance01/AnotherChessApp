using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameObjects
{
    class GameObject
    {
        ChessGame game;
        public Model model;
        public Vector3 previousPosition;
        public Vector3 position;
        public BoundingBox boundingBox;
        public Texture2D texture;

        public Matrix worldMatrix;

        public GameObject(ChessGame game, Model model, Vector3 position, BoundingBox boundingBox, Texture2D texture)
        {
            this.game = game;
            this.model = model;
            this.position = position;
            this.boundingBox = boundingBox;
            this.texture = texture;
        }

        public GameObject init()
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    worldMatrix = Matrix.CreateTranslation(position);
                    effect.World = worldMatrix;
                }
            }
            return this;
        }

        public void draw()
        {
            // Update bounding box whenever object has moved
            if (previousPosition != position)
            {
                boundingBox = game.UpdateBoundingBox(model, worldMatrix);
                previousPosition = position;
            }
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    if(texture != null)
                    {
                        effect.TextureEnabled = true;
                        effect.Texture = texture;
                    }

                    effect.World = worldMatrix;
                    effect.View = game.viewMatrix;
                    effect.Projection = game.projectionMatrix;
                }
                mesh.Draw();
            }
        }

        public void moveDelta(float dx, float dy, float dz)
        {
            position = new Vector3(position.X + dx, position.Y + dy, position.Z + dz);
            worldMatrix = Matrix.CreateTranslation(position);
        }
    }
}
