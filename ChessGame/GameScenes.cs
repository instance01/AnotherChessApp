using ChessGame.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    public class GameScenes
    {
        ChessGame game;
        public Ingame ingameScene;
        public SetupVsAI setupVsAIScene;
        public MainMenu mainMenuScene;
        public GameScenes(ChessGame game)
        {
            this.game = game;
        }

        public void init()
        {
            ingameScene = new Ingame(game);
            setupVsAIScene = new SetupVsAI(game);
            mainMenuScene = new MainMenu(game);
        }

        public void drawScenes2D()
        {
            if (ingameScene.enabled)
            {
                ingameScene.draw2D();
            }
            if (setupVsAIScene.enabled)
            {
                setupVsAIScene.draw2D();
            }
            if (mainMenuScene.enabled)
            {
                mainMenuScene.draw2D();
            }
        }

        public void drawScenes3D()
        {
            if (ingameScene.enabled)
            {
                ingameScene.draw3D();
            }
            if (setupVsAIScene.enabled)
            {
                setupVsAIScene.draw3D();
            }
            if (mainMenuScene.enabled)
            {
                mainMenuScene.draw3D();
            }
        }

    }
}
