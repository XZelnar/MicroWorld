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
    class LevelsMenu : Scene
    {
        Elements.MenuAnimatedButton bnew, bload, bcustom, ldesign, back;
        public new void Initialize()
        {
            ShouldBeScaled = false;

            bnew = new Elements.MenuAnimatedButton(100, 83, 600, 45, "New");
            bnew.InitAnimation("GUI/Menus/LevelMenu/Campaign", "GUI/Menus/LevelMenu/CampaignSelected", 100, 7, 0);
            bnew.onClicked += new Elements.Button.ClickedEventHandler(newClick);
            controls.Add(bnew);

            bload = new Elements.MenuAnimatedButton(100, 150, 600, 45, "Load");
            bload.InitAnimation("GUI/Menus/Load", "GUI/Menus/LoadSelected", 100, 7, 0);
            bload.onClicked += new Elements.Button.ClickedEventHandler(loadClick);
            controls.Add(bload);

            bcustom = new Elements.MenuAnimatedButton(100, 217, 600, 45, "Custom levels");
            bcustom.InitAnimation("GUI/Menus/LevelMenu/CustomLevels", "GUI/Menus/LevelMenu/CustomLevelsSelected", 100, 7, 0);
            bcustom.isEnabled = false;
            bcustom.onClicked += new Elements.Button.ClickedEventHandler(customClick);
            controls.Add(bcustom);

            ldesign = new Elements.MenuAnimatedButton(100, 284, 600, 45, "LevelDesigner");
            ldesign.InitAnimation("GUI/Menus/LevelMenu/LevelDesigner", "GUI/Menus/LevelMenu/LevelDesignerSelected", 100, 7, 0);
            ldesign.onClicked += new Elements.Button.ClickedEventHandler(ldesign_onClicked);
            controls.Add(ldesign);

            back = new Elements.MenuAnimatedButton(100, 351, 600, 45, "Back");
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
            if (e.key == Keys.C.GetHashCode())
            {
                customClick(null, null);
                e.Handled = true;
                return;
            }
            base.onKeyPressed(e);
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            bnew.position = new Vector2(100f / 800f * w, 83f / 480f * h);
            bload.position = new Vector2(100f / 800f * w, 150f / 480f * h);
            bcustom.position = new Vector2(100f / 800f * w, 217f / 480f * h);
            ldesign.position = new Vector2(100f / 800f * w, 284f / 480f * h);
            back.position = new Vector2(100f / 800f * w, 351f / 480f * h);

            bnew.Size = new Vector2(3 * w / 4, 45f / 480f * h);
            bload.Size = bnew.Size;
            bcustom.Size = bnew.Size;
            ldesign.Size = bnew.Size;
            back.Size = bnew.Size;
        }

        public void newClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_levelSelection, "GUILevelSelector");
            //Main.curState = "GUILevelSelector";
            //GUIEngine.curScene = GUIEngine.s_levelSelector;
        }

        public void loadClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_levelsLoadMenu, "GUILevelsLoad");
            //Main.curState = "GUILevelsLoad";
            //GUIEngine.curScene = GUIEngine.s_levelsLoadMenu;
        }

        public void customClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
        }

        void ldesign_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_lvlDesignerMenu, "GUIlvlDesignerMenu");
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
            bcustom.FadeIn();
            ldesign.FadeIn();
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
            bcustom.isVisible = t;
            ldesign.isVisible = t;
            back.isVisible = t;
        }

        public override void FadeOut()
        {
            Sound.SoundPlayer.ItemFadeOut();
            bnew.FadeOut();
            bload.FadeOut();
            bcustom.FadeOut();
            ldesign.FadeOut();
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
