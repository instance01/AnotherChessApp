using ChessGame.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Util
{
    public class GameUtil
    {
        ChessGame game;
        public GameUtil(ChessGame game)
        {
            this.game = game;
        }

        public void CreateBoardTile(Boolean white, Vector3 position)
        {
            Model model = white ? game.content.whiteTile : game.content.blackTile;
            game.boardTiles.Add((BoardTileObject)new BoardTileObject(game, model, position, white, UpdateBoundingBox(model, Matrix.CreateTranslation(position))).init());
        }

        public ChessPieceObject CreateChessPiece(Boolean white, Model model, Vector3 position, String id)
        {
            ChessPieceObject obj = (ChessPieceObject)new ChessPieceObject(game, model, position, white, UpdateBoundingBox(model, Matrix.CreateTranslation(position)), id).init();
            game.chessPieces.Add(obj);
            return obj;
        }

        public Ray FindWhereClicked(MouseState ms)
        {
            Vector3 nearScreenPoint = new Vector3(ms.X, ms.Y, 0);
            Vector3 farScreenPoint = new Vector3(ms.X, ms.Y, 1);
            Vector3 nearWorldPoint = game.GraphicsDevice.Viewport.Unproject(nearScreenPoint, game.projectionMatrix, game.viewMatrix, Matrix.Identity);
            Vector3 farWorldPoint = game.GraphicsDevice.Viewport.Unproject(farScreenPoint, game.projectionMatrix, game.viewMatrix, Matrix.Identity);

            Vector3 direction = farWorldPoint - nearWorldPoint;
            direction.Normalize();

            return new Ray(nearWorldPoint, direction);
        }

        // http://gamedev.stackexchange.com/questions/2438/how-do-i-create-bounding-boxes-with-xna-4-0
        public BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);
                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            return new BoundingBox(min, max);
        }


    }
}
