using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    class GameContent
    {
        ChessGame game;

        public Texture2D whiteMatteTexture, blackMatteTexture;

        public Model whiteTile;
        public Model blackTile;
        public Model TowerW, HorseW1, HorseW2, BishopW, QueenW, KingW, PawnW, TowerB, HorseB1, HorseB2, BishopB, QueenB, KingB, PawnB;
        public SpriteFont font;

        public GameContent(ChessGame game)
        {
            this.game = game;
        }

        public void loadContent()
        {
            whiteMatteTexture = game.Content.Load<Texture2D>("white_matte");
            blackMatteTexture = game.Content.Load<Texture2D>("black_matte");

            whiteTile = game.Content.Load<Model>("whiteTile");
            blackTile = game.Content.Load<Model>("blackTile");
            TowerW = game.Content.Load<Model>("TowerW_bauhaus");
            HorseW1 = game.Content.Load<Model>("HorseW1_bauhaus");
            HorseW2 = game.Content.Load<Model>("HorseW2_bauhaus");
            BishopW = game.Content.Load<Model>("BishopW_bauhaus");
            QueenW = game.Content.Load<Model>("QueenW_bauhaus");
            KingW = game.Content.Load<Model>("KingW_bauhaus");
            PawnW = game.Content.Load<Model>("PawnW_bauhaus");
            TowerB = game.Content.Load<Model>("TowerB_bauhaus");
            HorseB1 = game.Content.Load<Model>("HorseB1_bauhaus");
            HorseB2 = game.Content.Load<Model>("HorseB2_bauhaus");
            BishopB = game.Content.Load<Model>("BishopB_bauhaus");
            QueenB = game.Content.Load<Model>("QueenB_bauhaus");
            KingB = game.Content.Load<Model>("KingB_bauhaus");
            PawnB = game.Content.Load<Model>("PawnB_bauhaus");
            font = game.Content.Load<SpriteFont>("Font");
        }


    }
}
