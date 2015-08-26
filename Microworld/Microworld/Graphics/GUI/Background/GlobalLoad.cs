using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroWorld.Graphics.GUI.Background
{
    unsafe class GlobalLoad : Background
    {
        private Texture2D title, dots, gradient, pixel;
        private SpriteFont font, fontSmall;
        private int gradStart = 1920;

        RenderTarget2D fbo;
        int warningFadeState = 0;
        int warningTimeout = 0;
        String warningText = 
            "WARNING!!!\r\n" + 
            "This game is still in development!\r\n" + 
            "It contains pre-release content that in no way represents the final product.\r\n" + 
            "Some features may be placeholders, not work as intended or not work at all.\r\n" + 
            "\r\n" + 
            "Build version: " + Settings.VERSION;

        public override void Initialize()
        {
            dots = ResourceManager.Load<Texture2D>("GUI/Loading/DotsBG");
            gradient = ResourceManager.Load<Texture2D>("GUI/Loading/Gradient");
            title = ResourceManager.Load<Texture2D>("GUI/Loading/Title");
            pixel = ResourceManager.Load<Texture2D>("pixel");
            font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_18");
            fontSmall = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_14");
        }

        public override void Update()
        {
            if (warningFadeState < 100)
            {
                if (fbo == null || fbo.Width != Main.WindowWidth)
                {
                    fbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, Main.WindowWidth, Main.WindowHeight);
                }
                GraphicsEngine.Renderer.EnableFBO(fbo);
                GraphicsEngine.Renderer.BeginUnscaled();

                #region ActualDraw
                GraphicsEngine.Renderer.Draw(title, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Color.White);
                GraphicsEngine.Renderer.Draw(dots, new Rectangle(
                    439 * Main.WindowWidth / 1920, 872 * Main.WindowHeight / 1080,
                    1003 * Main.WindowWidth / 1920, 14 * Main.WindowHeight / 1080),
                    Color.White);
                GraphicsEngine.Renderer.Draw(gradient, new Rectangle(
                    gradStart, 872 * Main.WindowHeight / 1080,
                    480 * Main.WindowWidth / 1920, 14 * Main.WindowHeight / 1080),
                    Color.White);
                #endregion

                GraphicsEngine.Renderer.End();
                GraphicsEngine.Renderer.DisableFBO();
            }

            if (warningFadeState != 0 && warningFadeState < 200)
                warningFadeState += 4;
            else if (warningFadeState == 200)
            {
                warningTimeout++;
                InputEngine.Update();
                if (InputEngine.curMouse.LeftButton != ButtonState.Released || InputEngine.curMouse.RightButton != ButtonState.Released || 
                    InputEngine.curKeyboard.GetPressedKeys().Length != 0)
                    warningFadeState += 4;
                if (warningTimeout >= 800)
                    warningFadeState += 4;
            }
            else if (warningFadeState > 200)
            {
                warningFadeState += 4;
                if (warningFadeState >= 300)
                    Graphics.GUI.GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_mainMenu, "GUIMain");
            }
        }

        public void OnContentLoaded()
        {
            if (warningFadeState == 0)
                if (!Settings.IntroWarningShow || Settings.IntroWarningSkip_File)
                    Graphics.GUI.GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_mainMenu, "GUIMain");
                else
                    warningFadeState += 4;
                    //warningFadeState += 300;
        }

        Random r = new Random();
        public override void Draw(Renderer renderer)
        {
            if (warningFadeState < 100)
            {
                gradStart += 20 * Main.WindowWidth / 1920;
                if (gradStart >= (439 + 1003) * Main.WindowWidth / 1920)
                    gradStart = (439 - 1003) * Main.WindowWidth / 1920;

                renderer.Draw(fbo, new Vector2(), Color.White);
                renderer.Draw(pixel, new Rectangle(0, 0, Main.windowWidth, Main.windowHeight), Color.Black * ((float)warningFadeState / 100f));
            }
            else
            {
                renderer.Draw(pixel, new Rectangle(0, 0, Main.windowWidth, Main.windowHeight), Color.Black);
                renderer.DrawStringLeft(font, warningText, new Vector2(), Color.White);
                renderer.DrawStringRight(fontSmall, "[Press any key to skip]", new Rectangle(0, Main.windowHeight - 23, Main.windowWidth, 23), Color.Gray);
                if (warningFadeState < 200)
                    renderer.Draw(pixel, new Rectangle(0, 0, Main.windowWidth, Main.windowHeight), Color.Black * ((float)(200 - warningFadeState) / 100f));
                else
                    renderer.Draw(pixel, new Rectangle(0, 0, Main.windowWidth, Main.windowHeight), Color.Black * ((float)(warningFadeState - 200) / 100f));
            }
        }

    }
}
