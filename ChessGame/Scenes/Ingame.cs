using ChessGame.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scenes
{
    public class Ingame : Scene
    {
        public Ingame(ChessGame game) : base(game)
        {

        }

        public override void draw3D()
        {
            foreach (BoardTileObject obj in game.boardTiles)
            {
                obj.draw();
            }

            foreach(EnvironmentBlock obj in game.envBlocks)
            {
                obj.draw();
            }

            for (int i = 0; i < game.chessPieces.Count; i++)
            {
                if (i < game.chessPieces.Count)
                {
                    ChessPieceObject obj = game.chessPieces[i];
                    if (obj != null)
                    {
                        obj.draw();
                    }
                }
            }
        }
    }
}
