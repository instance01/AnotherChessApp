using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    public class GameContent
    {
        ChessGame game;

        public Texture2D whiteMatteTexture, blackMatteTexture;
        public Texture2D buttonHuman_light, buttonHuman_dark, buttonAI_light, buttonAI_dark, buttonStart_dark, buttonStart_light, select_ai, buttonSelectWhite_dark, buttonSelectWhite_light, buttonSelectBlack_dark, buttonSelectBlack_light;
        public Texture2D transparentRectangle;
        public Texture2D darkGrayTexture;

        public Model whiteTile;
        public Model blackTile;
        public Model TowerW, HorseW1, HorseW2, BishopW, QueenW, KingW, PawnW, TowerB, HorseB1, HorseB2, BishopB, QueenB, KingB, PawnB;
        public Model envBlock;
        public SpriteFont font; 

        public GameContent(ChessGame game)
        {
            this.game = game;
        }

        public void loadContent()
        {
            whiteMatteTexture = game.Content.Load<Texture2D>("Textures/white_matte");
            blackMatteTexture = game.Content.Load<Texture2D>("Textures/black_matte");
            buttonHuman_light = game.Content.Load<Texture2D>("Textures/buttonHuman_light");
            buttonHuman_dark = game.Content.Load<Texture2D>("Textures/buttonHuman_dark");
            buttonAI_light = game.Content.Load<Texture2D>("Textures/buttonAI_light");
            buttonAI_dark = game.Content.Load<Texture2D>("Textures/buttonAI_dark");
            buttonStart_dark = game.Content.Load<Texture2D>("Textures/buttonStart_dark");
            buttonStart_light = game.Content.Load<Texture2D>("Textures/buttonStart_light");
            select_ai = game.Content.Load<Texture2D>("Textures/select_ai");
            buttonSelectWhite_dark = game.Content.Load<Texture2D>("Textures/buttonSelectWhite_dark");
            buttonSelectWhite_light = game.Content.Load<Texture2D>("Textures/buttonSelectWhite_light");
            buttonSelectBlack_dark = game.Content.Load<Texture2D>("Textures/buttonSelectBlack_dark");
            buttonSelectBlack_light = game.Content.Load<Texture2D>("Textures/buttonSelectBlack_light");
            darkGrayTexture = game.Content.Load<Texture2D>("Textures/dark-gray-texture");
            transparentRectangle = new Texture2D(game.GraphicsDevice, 1, 1);
            transparentRectangle.SetData(new[] { Color.FromNonPremultiplied(0, 0, 0, 190)});

            whiteTile = game.Content.Load<Model>("whiteTile");
            blackTile = game.Content.Load<Model>("blackTile");
            envBlock = game.Content.Load<Model>("env_block");
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
