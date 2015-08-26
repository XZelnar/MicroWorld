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
using MicroWorld.Graphics.GUI.Elements;

namespace MicroWorld.Graphics.GUI.Scene
{
    class InGameMenu : HUDScene
    {
        public Vector2 position = new Vector2(), size = new Vector2();

        MenuAnimatedButton resume, save, restart, encyclopedia, options, mainmenu, exit;
        float opacity = 0f;

        #region Events
        public class ButtonClickedArgs
        {
            public int button;//yes == 1, no == 0
        }
        public delegate void ButtonClickedEventHandler(object sender, ButtonClickedArgs e);
        public event ButtonClickedEventHandler onButtonClicked;
        #endregion

        public InGameMenu()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Initialize()
        {
            Layer = 950;

            resume = new MenuAnimatedButton(100, 10, 600, 45, "Resume");
            resume.InitAnimation("GUI/HUD/InGameMenu/Resume", "GUI/HUD/InGameMenu/ResumeSelected", 100, 7, 0);
            resume.onClicked += new Button.ClickedEventHandler(resume_onClicked);
            controls.Add(resume);

            save = new MenuAnimatedButton(100, 105, 600, 45, "Save");
            save.InitAnimation("GUI/HUD/InGameMenu/Save", "GUI/HUD/InGameMenu/SaveSelected", 100, 7, 0);
            save.onClicked += new Button.ClickedEventHandler(save_onClicked);
            controls.Add(save);

            restart = new MenuAnimatedButton(100, 105, 600, 45, "Save");
            restart.InitAnimation("GUI/HUD/InGameMenu/Restart", "GUI/HUD/InGameMenu/RestartSelected", 100, 7, 0);
            restart.onClicked += new Button.ClickedEventHandler(restart_onClicked);
            controls.Add(restart);

            encyclopedia = new MenuAnimatedButton(100, 105, 600, 45, "Encyclopedia");
            encyclopedia.InitAnimation("GUI/HUD/InGameMenu/Handbook", "GUI/HUD/InGameMenu/HandbookSelected", 100, 7, 130);
            encyclopedia.onClicked += new Button.ClickedEventHandler(encyclopedia_onClicked);
            controls.Add(encyclopedia);

            options = new MenuAnimatedButton(100, 200, 600, 45, "Options");
            options.InitAnimation("GUI/HUD/InGameMenu/Options", "GUI/HUD/InGameMenu/OptionsSelected", 100, 7, 70);
            options.onClicked += new Button.ClickedEventHandler(options_onClicked);
            //options.isEnabled = false;
            controls.Add(options);

            mainmenu = new MenuAnimatedButton(100, 295, 600, 45, "Main Menu");
            mainmenu.InitAnimation("GUI/HUD/InGameMenu/MainMenu", "GUI/HUD/InGameMenu/MainMenuSelected", 100, 7, 0);
            mainmenu.onClicked += new Button.ClickedEventHandler(mainmenu_onClicked);
            controls.Add(mainmenu);

            exit = new MenuAnimatedButton(100, 390, 600, 45, "Exit");
            exit.InitAnimation("GUI/HUD/InGameMenu/Exit", "GUI/HUD/InGameMenu/ExitSelected", 100, 7, 105);
            exit.onClicked += new Button.ClickedEventHandler(exit_onClicked);
            controls.Add(exit);

            base.Initialize();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);

            resume.position = new Vector2(100f / 800f * w, 38f / 480f * h);
            save.position = new Vector2(100f / 800f * w, 98f / 480f * h);
            restart.position = new Vector2(100f / 800f * w, 158f / 480f * h);
            encyclopedia.position = new Vector2(100f / 800f * w, 218f / 480f * h);
            options.position = new Vector2(100f / 800f * w, 278f / 480f * h);
            mainmenu.position = new Vector2(100f / 800f * w, 338f / 480f * h);
            exit.position = new Vector2(100f / 800f * w, 398f / 480f * h);

            resume.Size = new Vector2(600f / 800f * w, 45f / 480f * h);
            save.Size = resume.Size;
            restart.Size = resume.Size;
            encyclopedia.Size = resume.Size;
            options.Size = resume.Size;
            mainmenu.Size = resume.Size;
            exit.Size = resume.Size;
        }

        void exit_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            Main.Close();
        }

        void mainmenu_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_mainMenu, "GUIMainMenu");
            //Main.curState = "GUIMainMenu";
            //GUIEngine.curScene = GUIEngine.s_mainMenu;
            GUIEngine.ClearHUDs();
            Logics.LevelEngine.Stop();
            Graphics.GraphicsEngine.camera.Center = new Vector2();
        }

        void options_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.s_options.CameFromState = Main.curState;
            GUIEngine.s_options.CameFromScene = GUIEngine.curScene;
            GUIEngine.ChangeScene(GUIEngine.s_options, "GUIOptions");
            //Main.curState = "GUIOptions";
            //GUIEngine.curScene = GUIEngine.s_options;
        }

        void restart_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (Main.curState == "GAMELevels")
            {
                GUIEngine.s_levelSelection.StartLevel();
                GUIEngine.RemoveHUDScene(this);
            }
            else
            {
                Components.ComponentsManager.Clear();
                Logics.CircuitManager.Clear();
                GUIEngine.RemoveHUDScene(this);
            }
        }

        void save_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            //Main.curState = "GUISandbox";
            if (Main.curState == "GAMESandbox")
            {
                GUIEngine.ChangeScene(GUIEngine.s_sandboxSaveMenu, null);
                //GUIEngine.curScene = GUIEngine.s_sandboxSaveMenu;
            }
            else if (Main.curState == "GAMELevels")
            {
                GUIEngine.ChangeScene(GUIEngine.s_levelsSaveMenu, null);
                //GUIEngine.curScene = GUIEngine.s_levelsSaveMenu;
            }
            else if (Main.curState == "GAMElvlDesign")
            {
                GUIEngine.ChangeScene(GUIEngine.s_lvlDesignerSave, null);
            }
        }

        void encyclopedia_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.s_encyclopediaBrowser.lastScene = GUIEngine.curScene;
            GUIEngine.s_encyclopediaBrowser.lastState = Main.curState;
            GUIEngine.s_encyclopediaBrowser.ResetState();
            GUIEngine.ChangeScene(GUIEngine.s_encyclopediaBrowser, "GUIEncyclopediaBrowser");
        }

        void resume_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            Close();
        }


        public override void Draw(Renderer renderer)
        {
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Color.Black * opacity);
            base.Draw(renderer);
        }


        System.Threading.Thread fadethread;
        public override void onShow()
        {
            if (fadethread != null && fadethread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                try
                {
                    fadethread.Abort();
                }
                catch { }
            }
            fadethread = new System.Threading.Thread(new System.Threading.ThreadStart(FadeIn));
            fadethread.IsBackground = true;
            fadethread.Start();

            base.onShow();
        }

        public override void Close()
        {
            if (fadethread != null && fadethread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                try
                {
                    fadethread.Abort();
                }
                catch { }
            }
            fadethread = new System.Threading.Thread(new System.Threading.ThreadStart(FadeOut));
            fadethread.IsBackground = true;
            fadethread.Start();
        }

        public override void onClose()
        {
            isVisible = false;
            opacity = 0f;
            base.onClose();
        }

        public override void FadeIn()
        {
            SetButtonsVisibility(false);
            resume.FadeIn();
            save.FadeIn();
            restart.FadeIn();
            encyclopedia.FadeIn();
            options.FadeIn();
            mainmenu.FadeIn();
            exit.FadeIn();
            SetButtonsVisibility(true);
            while (opacity < 0.5f)
            {
                opacity += 0.0005f;
                System.Threading.Thread.Sleep(1);
            }
            while (true)
            {
                if (!exit.IsFading) break;
                System.Threading.Thread.Sleep(1);
            }
            base.FadeIn();
        }

        public void SetButtonsVisibility(bool t)
        {
            resume.isVisible = t;
            save.isVisible = t;
            restart.isVisible = t;
            encyclopedia.isVisible = t;
            options.isVisible = t;
            mainmenu.isVisible = t;
            exit.isVisible = t;
        }

        public override void FadeOut()
        {
            resume.FadeOut();
            save.FadeOut();
            restart.FadeOut();
            encyclopedia.FadeOut();
            options.FadeOut();
            mainmenu.FadeOut();
            exit.FadeOut();
            System.Threading.Thread.Sleep(1);
            while (opacity > 0f)
            {
                opacity -= 0.0005f;
                System.Threading.Thread.Sleep(1);
            }
            while (true)
            {
                if (!exit.IsFading)
                    break;
                System.Threading.Thread.Sleep(1);
            }
            GUIEngine.RemoveHUDScene(this);
        }

        #region InputOverrides
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            base.onButtonDown(e);
            e.Handled = true;
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            base.onButtonUp(e);
            e.Handled = true;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            e.Handled = true;
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            e.Handled = true;
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            base.onMouseWheelMove(e);
            e.Handled = true;
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
            e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            base.onKeyPressed(e);
            e.Handled = true;
            if (e.key == Keys.Escape.GetHashCode())
            {
                Close();
            }
        }

        public override bool IsIn(int x, int y)
        {
            return x >= position.X && x < position.X + size.X
                && y >= position.Y && y < position.Y + size.Y;
        }
        #endregion

    }
}
