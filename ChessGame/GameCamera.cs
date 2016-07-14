using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    public class GameCamera
    {
        ChessGame game;
        public Vector3 cameraPosition;
        float dA = 0F;

        public GameCamera(ChessGame game)
        {
            cameraPosition = new Vector3(-15, 24, 0);
            this.game = game;
        }

        public void moveDelta(float dx, float dy, float dz)
        {
            cameraPosition = new Vector3(cameraPosition.X + dx, cameraPosition.Y + dy, cameraPosition.Z + dz);
            game.viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
        }

        public void rotateDelta(float angle)
        {
            dA += angle;
            cameraPosition = Vector3.Transform(cameraPosition - Vector3.Zero, Matrix.CreateFromAxisAngle(Vector3.UnitY, angle));
            game.viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
        }

        public void resetRotation(float angle)
        {
            cameraPosition = Vector3.Transform(cameraPosition - Vector3.Zero, Matrix.CreateFromAxisAngle(Vector3.UnitY, -dA - angle));
            game.viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
        }

        public void moveToPositionSmooth(float x, float y, float z)
        {
            // TODO
        }
    }
}
