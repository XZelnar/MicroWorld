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

namespace MicroWorld.Graphics.GUI.Scene
{
    class SandboxMenu : Scene
    {
        Elements.MenuAnimatedButton bnew, bload, back;
        public new void Initialize()
        {
            ShouldBeScaled = false;

            bnew = new Elements.MenuAnimatedButton(100, 160, 400, 45, "New");
            bnew.InitAnimation("GUI/Menus/New", "GUI/Menus/NewSelected", 100, 7, 0);
            bnew.onClicked += new Elements.Button.ClickedEventHandler(newClick);
            controls.Add(bnew);

            bload = new Elements.MenuAnimatedButton(100, 225, 400, 45, "Load");
            bload.InitAnimation("GUI/Menus/Load", "GUI/Menus/LoadSelected", 100, 7, 0);
            bload.onClicked += new Elements.Button.ClickedEventHandler(loadClick);
            controls.Add(bload);

            back = new Elements.MenuAnimatedButton(100, 290, 400, 45, "Back");
            back.InitAnimation("GUI/Menus/Back", "GUI/Menus/BackSelected", 100, 7, 0);
            back.onClicked += new Elements.Button.ClickedEventHandler(backClick);
            controls.Add(back);

            base.Initialize();

            background = GUIEngine.s_mainMenu.background;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (e.key == Keys.Escape.GetHashCode())
            {
                backClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.N.GetHashCode())
            {
                newClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.L.GetHashCode())
            {
                loadClick(null, null);
                e.Handled = true;
                return;
            }
            base.onKeyPressed(e);
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            bnew.position = new Vector2(100f / 800f * w, 160f / 480f * h);
            bload.position = new Vector2(100f / 800f * w, 225f / 480f * h);
            back.position = new Vector2(100f / 800f * w, 290f / 480f * h);

            bnew.Size = new Vector2(3f * w / 4, 45f / 480f * h);
            bload.Size = bnew.Size;
            back.Size = bnew.Size;
        }

        public void newClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();

            //Logics.CircuitManager.Clear();
            //Components.ComponentsManager.Clear();
            //Logics.LevelEngine.Stop();
            //Logics.GameInputHandler.PlacableAreas.Clear();
            //Settings.ResetInGameSettings();
            Logics.GameLogicsHelper.InitForGame();

            //Main.curState = "GAMESandbox";
            newGame = true;
            GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_game, "GAMESandbox");
            GUIEngine.s_componentSelector.ClearCount();
            //Graphics.GUI.GUIEngine.s_componentSelector.SelectedIndex = 0;
            //GUIEngine.curScene = null;
        }
        bool newGame = false;
        public override void PostFadeOut()
        {
            if (newGame)
            {
                Logics.GameLogicsHelper.InitScenesForGame();
                //Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_componentSelector);
                //Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_runControl);
                newGame = false;
            }
            base.PostFadeOut();
        }

        public void loadClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_sandboxLoadMenu, "GUISandboxLoad");
            //Main.curState = "GUISandboxLoad";
            //GUIEngine.curScene = GUIEngine.s_sandboxLoadMenu;
        }

        public void backClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_mainMenu, "GUIMainMenu");
            //Main.curState = "GUIMainMenu";
            //GUIEngine.curScene = GUIEngine.s_mainMenu;
        }

        public override void FadeIn()
        {
            SetButtonsVisibility(false);
            Sound.SoundPlayer.ItemFadeIn();
            bnew.FadeIn();
            bload.FadeIn();
            back.FadeIn();
            SetButtonsVisibility(true);
            while (true)
            {
                if (!back.IsFading) break;
                System.Threading.Thread.Sleep(1);
            }
            base.FadeIn();
        }

        public void SetButtonsVisibility(bool t)
        {
            bnew.isVisible = t;
            bload.isVisible = t;
            back.isVisible = t;
        }

        public override void FadeOut()
        {
            Sound.SoundPlayer.ItemFadeOut();
            bnew.FadeOut();
            bload.FadeOut();
            back.FadeOut();
            while (true)
            {
                if (!back.IsFading)
                    break;
                System.Threading.Thread.Sleep(1);
            }
        }

    }
}
