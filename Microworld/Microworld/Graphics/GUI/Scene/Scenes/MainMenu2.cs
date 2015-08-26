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
    class MainMenu2 : Scene
    {
        Elements.MenuAnimatedButton lvl, sb, hb, op, st, ex;
        Elements.EncyclopediaBrowserButton cr, intr;
        public new void Initialize()
        {
            ShouldBeScaled = false;
            background = new Background.MainMenu();

            lvl = new Elements.MenuAnimatedButton(100, 50, 600, 45, "Levels");
            lvl.InitAnimation("GUI/Menus/MainMenu/Levels", "GUI/Menus/MainMenu/LevelsSelected", 100, 7, 160);
            lvl.onClicked += new Elements.Button.ClickedEventHandler(lvlClick);
            controls.Add(lvl);

            sb = new Elements.MenuAnimatedButton(100, 117, 600, 45, "Sandbox");
            sb.InitAnimation("GUI/Menus/MainMenu/Sandbox", "GUI/Menus/MainMenu/SandboxSelected", 100, 7, 0);
            sb.onClicked +=new Elements.Button.ClickedEventHandler(sbClick);
            controls.Add(sb);

            hb = new Elements.MenuAnimatedButton(100, 184, 600, 45, "Handbook");
            hb.InitAnimation("GUI/Menus/MainMenu/Handbook", "GUI/Menus/MainMenu/HandbookSelected", 100, 7, 130);
            hb.onClicked += new Elements.Button.ClickedEventHandler(hbClick);
            controls.Add(hb);

            op = new Elements.MenuAnimatedButton(100, 251, 600, 45, "Options");
            op.InitAnimation("GUI/Menus/MainMenu/Options", "GUI/Menus/MainMenu/OptionsSelected", 100, 7, 70);
            op.onClicked += new Elements.Button.ClickedEventHandler(opClick);
            controls.Add(op);

            st = new Elements.MenuAnimatedButton(100, 318, 600, 45, "Statistics");
            st.InitAnimation("GUI/Menus/MainMenu/Statistics", "GUI/Menus/MainMenu/StatisticsSelected", 100, 7, 35);
            st.onClicked += new Elements.Button.ClickedEventHandler(stClick);
            controls.Add(st);

            ex = new Elements.MenuAnimatedButton(100, 385, 600, 45, "Exit");
            ex.InitAnimation("GUI/Menus/MainMenu/Exit", "GUI/Menus/MainMenu/ExitSelected", 100, 7, 105);
            ex.onClicked += new Elements.Button.ClickedEventHandler(exClick);
            controls.Add(ex);

            cr = new Elements.EncyclopediaBrowserButton(Main.WindowWidth - 150, Main.WindowHeight - 35, 150, 35, "Credits");
            cr.OverrideTexture("GUI/Menus/MainMenu/ButtonBackgroundMM");
            cr.Font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_15");
            cr.foreground = Color.White;
            cr.onClicked += new Elements.Button.ClickedEventHandler(cr_onClicked);
            cr.IdleOpacity = 0.8f;
            controls.Add(cr);

            intr = new Elements.EncyclopediaBrowserButton(0, Main.WindowHeight - 35, 150, 35, "Intro");
            intr.OverrideTexture("GUI/Menus/MainMenu/ButtonBackgroundMM");
            intr.Font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_15");
            intr.foreground = Color.White;
            intr.onClicked += new Elements.Button.ClickedEventHandler(intr_onClicked);
            intr.IdleOpacity = 0.8f;
            controls.Add(intr);

            Main.LoadingDetails = "Initializing scenes...";

            base.Initialize();
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (ex.IsFading)
            {
                e.Handled = true;
                return;
            }
            if (e.key == Keys.L.GetHashCode())
            {
                lvlClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.S.GetHashCode())
            {
                sbClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.B.GetHashCode())
            {
                hbClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.T.GetHashCode())
            {
                stClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.O.GetHashCode())
            {
                opClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.E.GetHashCode())
            {
                exClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.I.GetHashCode())
            {
                intr_onClicked(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.C.GetHashCode())
            {
                cr_onClicked(null, null);
                e.Handled = true;
                return;
            }
            base.onKeyPressed(e);
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);

            lvl.position = new Vector2(100f / 800f * w, 50f / 480f * h);
            sb.position = new Vector2(100f / 800f * w, 117f / 480f * h);
            hb.position = new Vector2(100f / 800f * w, 184f / 480f * h);
            op.position = new Vector2(100f / 800f * w, 251f / 480f * h);
            st.position = new Vector2(100f / 800f * w, 318f / 480f * h);
            ex.position = new Vector2(100f / 800f * w, 385f / 480f * h);

            intr.position = new Vector2(0, h - intr.size.Y);
            cr.position = new Vector2(w - cr.size.X, h - cr.size.Y);

            sb.Size = new Vector2(600f / 800f * w, 45f / 480f * h);
            lvl.Size = sb.Size;
            hb.Size = sb.Size;
            op.Size = sb.Size;
            st.Size = sb.Size;
            ex.Size = sb.Size;


        }

        void intr_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            Graphics.GUI.GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_intro, "GUIIntro");
        }

        void cr_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            Graphics.GUI.GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_credits2, "GUICredits");
        }

        public void lvlClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_levelsMenu, "GUILevels");
            //Main.curState = "GUILevels";
            //GUIEngine.curScene = GUIEngine.s_levelsMenu;
        }

        public void sbClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_sandboxMenu, "GUISandbox");
            //Main.curState = "GUISandbox";
            //GUIEngine.curScene = GUIEngine.s_sandboxMenu;
        }

        public void hbClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.s_encyclopediaBrowser.lastScene = GUIEngine.curScene;
            GUIEngine.s_encyclopediaBrowser.lastState = Main.curState;
            GUIEngine.s_encyclopediaBrowser.ResetState();
            GUIEngine.ChangeScene(GUIEngine.s_encyclopediaBrowser, "GUIEncyclopediaBrowser");
        }

        public void opClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.s_options.CameFromState = Main.curState;
            GUIEngine.s_options.CameFromScene = GUIEngine.curScene;
            GUIEngine.ChangeScene(GUIEngine.s_options, "GUIOptions");
            //Main.curState = "GUIOptions";
            //GUIEngine.curScene = GUIEngine.s_options;
        }

        public void stClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.s_statistics2.CameFromState = Main.curState;
            GUIEngine.s_statistics2.CameFromScene = GUIEngine.curScene;
            GUIEngine.ChangeScene(GUIEngine.s_statistics2, "GUIStatistics");
            //Main.curState = "GUIStatistics";
            //GUIEngine.curScene = GUIEngine.s_statistics;
        }

        public void exClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            Main.Close();
        }

        public override void FadeIn()
        {
            SetButtonsVisibility(false);
            Sound.SoundPlayer.ItemFadeIn();
            lvl.FadeIn();
            sb.FadeIn();
            hb.FadeIn(); 
            op.FadeIn(); 
            st.FadeIn();
            ex.FadeIn();

            cr.position.X = Main.WindowWidth;
            intr.position.X = -intr.size.X;
            SetButtonsVisibility(true);

            for (int i = 0; i < intr.size.X; i++)
            {
                intr.position.X = i - intr.size.X;
                cr.position.X = Main.WindowWidth - i;
                System.Threading.Thread.Sleep(3);
            }

            while (true)
            {
                if (!ex.IsFading) break;
                System.Threading.Thread.Sleep(1);
            }
            base.FadeIn();
        }

        public void SetButtonsVisibility(bool t)
        {
            lvl.isVisible = t;
            sb.isVisible = t;
            hb.isVisible = t;
            op.isVisible = t;
            st.isVisible = t;
            ex.isVisible = t;

            cr.isVisible = t;
            intr.isVisible = t;
        }

        public override void FadeOut()
        {
            Sound.SoundPlayer.ItemFadeOut();
            lvl.FadeOut();
            sb.FadeOut();
            hb.FadeOut();
            op.FadeOut();
            st.FadeOut();
            ex.FadeOut();

            intr.position = new Vector2(0, Main.WindowHeight - intr.size.Y);
            cr.position = new Vector2(Main.WindowWidth - cr.size.X, Main.WindowHeight - cr.size.Y);

            for (int i = (int)intr.size.X; i >= 0; i--)
            {
                intr.position.X = i - intr.size.X;
                cr.position.X = Main.WindowWidth - i;
                System.Threading.Thread.Sleep(3);
            }

            while (true)
            {
                if (!ex.IsFading) 
                    break;
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}
