using ChessGame.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Util
{
    class ChessUtil
    {
        ChessGame game;
        public ChessUtil(ChessGame game)
        {
            this.game = game;
        }

        public bool isValidMove(ChessPieceObject obj, Vector3 position, string move, bool white)
        {
            if (isValidMoveInternal(obj.id, move, white))
            {
                obj.moveTo(position);
                if (!isKingAttacked(obj, white))
                {
                    return true;
                }
            }
            return false;
        }

        public bool isValidMoveInternal(string id, string move, bool white)
        {
            string originChar = move.Substring(0, 1);
            string originNum = move.Substring(1, 1);
            string destinationChar = move.Substring(2, 1);
            string destinationNum = move.Substring(3, 1);
            int originX = Array.IndexOf(game.xCoords, originNum);
            int destinationX = Array.IndexOf(game.xCoords, destinationNum);
            int originY = Array.IndexOf(game.zCoords, originChar);
            int destinationY = Array.IndexOf(game.zCoords, destinationChar);

            Console.WriteLine(move + " " + originX + " " + originY + " " + destinationX + " " + destinationY);

            return isValidMoveInternal(id, originX, originY, destinationX, destinationY, white);
        }

        public bool isValidMoveInternal(string id, int originX, int originY, int destinationX, int destinationY, bool white)
        {
            // TODO maybe refactor into Coordinate class?
            if (originX == destinationX && originY == destinationY)
            {
                return false;
            }
            if (hasFigure(destinationY, destinationX, true, !white))
            {
                return false;
            }

            if (id == "T")
            {
                return checkTower(originX, originY, destinationX, destinationY);
            }
            else if (id == "H")
            {
                if (Math.Abs(originY - destinationY) == 2 && Math.Abs(originX - destinationX) == 1)
                {
                    return true;
                }
                if (Math.Abs(originY - destinationY) == 1 && Math.Abs(originX - destinationX) == 2)
                {
                    return true;
                }
            }
            else if (id == "B")
            {
                return checkBishop(originX, originY, destinationX, destinationY);
            }
            else if (id == "Q")
            {
                bool t = checkTower(originX, originY, destinationX, destinationY);
                bool b = checkBishop(originX, originY, destinationX, destinationY);
                return (t || b);
            }
            else if (id == "K")
            {
                int yvec = Math.Abs(originY - destinationY);
                int xvec = Math.Abs(originX - destinationX);
                if (yvec <= 1 && xvec <= 1)
                {
                    return true;
                }
                // TODO castle
            }
            else if (id == "P")
            {
                if (white)
                {
                    if (destinationX < originX)
                    {
                        return false;
                    }
                }
                else
                {
                    if (destinationX > originX)
                    {
                        return false;
                    }
                }
                int xdist = Math.Abs(destinationY - originY);
                int ydist = Math.Abs(destinationX - originX);
                //Console.WriteLine("#####" + white + " " + ydist + " " + xdist);
                if (((white && originX == 1) || (!white && originX == 6)) && ydist == 2)
                {
                    return !isFigureBetween(originX, originY, destinationX, destinationY) && !hasFigure(destinationY, destinationX);
                }
                if (xdist == 0 && ydist == 1 && !hasFigure(destinationY, destinationX))
                {
                    return true;
                }
                if (xdist == 1 && ydist == 1 && hasFigure(destinationY, destinationX, true, white))
                {
                    return true;
                }
            }

            return false;
        }

        // Copied from my other chess program
        // TODO rewrite with better readability
        // oy and ox is origin, cy and cx is destination
        public Boolean isFigureBetween(int oy, int ox, int cy, int cx)
        {
            Boolean isDiagonal = true;
            int xvec = cx - ox;
            int yvec = cy - oy;
            int xyvec = Math.Abs(xvec) - Math.Abs(yvec);
            if ((xvec == 0 && yvec != 0) || (xvec != 0 && yvec == 0))
            {
                isDiagonal = false;
            }

            int xbase = 0;
            int ybase = 0;
            if (ox > cx)
            {
                xbase = cx;
            }
            else
            {
                xbase = ox;
            }
            if (oy > cy)
            {
                ybase = cy;
            }
            else
            {
                ybase = oy;
            }
            for (int i = 0; i <= Math.Abs(ox - cx); i++)
            {
                for (int j = 0; j <= Math.Abs(oy - cy); j++)
                {
                    int ccx = xbase + i;
                    int ccy = ybase + j;
                    if ((ccx == ox && ccy == oy) || (ccx == cx && ccy == cy))
                    {
                        continue;
                    }
                    int xvec_ = cx - ccx;
                    int yvec_ = cy - ccy;
                    int xyvec_ = Math.Abs(xvec_) - Math.Abs(yvec_);
                    if (!isDiagonal)
                    {
                        if (xvec_ == xvec || yvec_ == yvec)
                        {
                            if (hasFigure(ccx, ccy))
                            {
                                return true;
                            }
                        }
                        continue;
                    }
                    if (xyvec == xyvec_)
                    {
                        if (hasFigure(ccx, ccy))
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        public bool hasFigure(int ccX, int ccY, bool checkForColor = false, bool white = false)
        {
            foreach (ChessPieceObject obj in game.chessPieces)
            {
                if (checkForColor)
                {
                    if (obj.white == white)
                    {
                        continue;
                    }
                }
                int x = (int)((obj.position.X + 1) / 2 + 3);
                int y = (int)((obj.position.Z + 1) / 2 + 3);
                //Console.WriteLine(white + " " + obj.position.X + " " + obj.position.Z);
                //Console.WriteLine(">>> " + ccX + " " + ccY + " = " + x + " " + y);
                if (ccY == x && ccX == y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool checkBishop(int originX, int originY, int destinationX, int destinationY)
        {
            int xvec = Math.Abs(destinationX - originX);
            int yvec = Math.Abs(destinationY - originY);
            int xyvec = Math.Abs(xvec) - Math.Abs(yvec);
            if (xyvec == 0)
            {
                return !isFigureBetween(originX, originY, destinationX, destinationY);
            }
            return false;
        }

        public bool checkTower(int originX, int originY, int destinationX, int destinationY)
        {
            if ((originX == destinationX && originY != destinationY) || (originX != destinationX && originY == destinationY))
            {
                return !isFigureBetween(originX, originY, destinationX, destinationY);
            }
            return false;
        }

        public bool isKingAttacked(ChessPieceObject movedObj, bool white)
        {
            ChessPieceObject king = white ? game.kingW : game.kingB;

            int kingY = (int)((king.position.X + 1) / 2 + 3);
            int kingX = (int)((king.position.Z + 1) / 2 + 3);

            foreach (ChessPieceObject obj in game.chessPieces)
            {
                if (obj.white != white)
                {
                    int y = (int)((obj.position.X + 1) / 2 + 3);
                    int x = (int)((obj.position.Z + 1) / 2 + 3);
                    if (isValidMoveInternal(obj.id, y, x, kingY, kingX, !white))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
