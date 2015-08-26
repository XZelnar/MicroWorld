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
    class Options : Scene
    {
        Texture2D bg;

        public String CameFromState = "";
        public Scene CameFromScene = null;

        int maxw, maxh;

        Elements.ScrollBar mv, ev, mzv;
        Elements.ComboBox ar, res;
        Elements.EncyclopediaBrowserButton sb, sac, ap;
        Elements.Label lar, lres;
        Elements.CheckBox fs;

        Elements.HotkeyControl hcSimStart;
        Elements.HotkeyControl hcSimStop;
        Elements.HotkeyControl hcSimPause;
        Elements.HotkeyControl hcUndo;
        Elements.HotkeyControl hcCompRem;
        Elements.HotkeyControl hcCompProp;
        Elements.HotkeyControl hcEraser;
        Elements.HotkeyControl hcZoomIn;
        Elements.HotkeyControl hcZoomOut;

        public new void Initialize()
        {
            #region Basic
            Layer = 100000;
            ShouldBeScaled = false;

            var a = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.GetEnumerator();
            a.MoveNext();
            maxw = a.Current.Width;
            maxh = a.Current.Height;
            a.MoveNext();
            while (true)//DO NOT TOUCH THIS CYCLE OR ELSE MONO GETS SCARED!!!
            {
                try
                {
                    if (a.Current == null)
                        break;
                }
                catch { System.Threading.Thread.Sleep(1); }
                try
                {
                    if (a.Current == null)
                        break;
                }
                catch { break; }
                try
                {
                    if (a.Current.Width > maxw) maxw = a.Current.Width;
                    if (a.Current.Height > maxh) maxh = a.Current.Height;
                }
                catch { }
                a.MoveNext();
            }
            #endregion

            #region ControlButtons
            sb = new Elements.EncyclopediaBrowserButton(410, 440, 120, 30, "Back");
            (sb as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            sb.foreground = Color.White;
            //sb.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            sb.onClicked +=new Elements.Button.ClickedEventHandler(sbClick);
            controls.Add(sb);

            sac = new Elements.EncyclopediaBrowserButton(540, 440, 120, 30, "Save");
            (sac as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            sac.foreground = Color.White;
            //sac.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            sac.onClicked += new Elements.Button.ClickedEventHandler(sacClick);
            controls.Add(sac);

            ap = new Elements.EncyclopediaBrowserButton(670, 440, 120, 30, "Apply");
            (ap as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            ap.foreground = Color.White;
            //ap.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            ap.onClicked += new Elements.Button.ClickedEventHandler(apClick);
            controls.Add(ap);
            #endregion

            #region MasterVolume
            Elements.Label l1 = new Elements.Label(10, 20, "Master Volume");
            l1.foreground = Color.White;
            controls.Add(l1);

            mv = new Elements.ScrollBar(170, 24, 200, 20);
            mv.onValueChanged += new Elements.ScrollBar.ValueChangedEventHandler(mv_onValueChanged);
            mv.MinValue = 0;
            mv.MaxValue = 100;
            mv.Value = (int)(Settings.MasterVolume * 100);
            controls.Add(mv);
            #endregion

            #region EffectVolume
            Elements.Label l11 = new Elements.Label(10, 50, "Effects Volume");
            l11.foreground = Color.White;
            controls.Add(l11);

            ev = new Elements.ScrollBar(170, 54, 200, 20);
            ev.onValueChanged += new Elements.ScrollBar.ValueChangedEventHandler(ev_onValueChanged);
            ev.MinValue = 0;
            ev.MaxValue = 100;
            ev.Value = (int)(Settings.EffectsVolume * 100);
            controls.Add(ev);
            #endregion

            #region MusicVolume
            Elements.Label l111 = new Elements.Label(10, 80, "Music Volume");
            l111.foreground = Color.White;
            controls.Add(l111);

            mzv = new Elements.ScrollBar(170, 84, 200, 20);
            mzv.onValueChanged += new Elements.ScrollBar.ValueChangedEventHandler(mzv_onValueChanged);
            mzv.MinValue = 0;
            mzv.MaxValue = 100;
            mzv.Value = (int)(Settings.MusicVolume * 100);
            controls.Add(mzv);
            #endregion

            #region Resoulutions
            lar = new Elements.Label(400, 20, "Aspect ratio:");
            lar.foreground = Color.White;
            controls.Add(lar);

            ar = new Elements.ComboBox(550, 22, 120, 24);
            ar.onElementSelected += new Elements.ComboBox.ElementSelectedHandler(ar_onElementSelected);
            controls.Add(ar);
            ar.ItemsAdd("4*3");
            ar.ItemsAdd("5*3");
            if (maxw >= 1280 && maxh >= 720) ar.ItemsAdd("16*9");
            if (maxw >= 1280 && maxh >= 800) ar.ItemsAdd("16*10");

            lres = new Elements.Label(400, 50, "Resolution:");
            lres.foreground = Color.White;
            controls.Add(lres);

            res = new Elements.ComboBox(550, 52, 120, 24);
            res.onElementSelected += new Elements.ComboBox.ElementSelectedHandler(res_onElementSelected);
            controls.Add(res);

            fs = new Elements.CheckBox(400, 90, 170, 20, "FullScreen", false);
            fs.foreground = Color.White;
            controls.Add(fs);
            #endregion

            #region Hotkeys
            hcSimStart = new Elements.HotkeyControl(5, 125, 250, "Simulation start", Settings.k_SimulationStart);
            controls.Add(hcSimStart);
            hcSimStop = new Elements.HotkeyControl(5, 150, 250,  "Simulation stop", Settings.k_SimulationStop);
            controls.Add(hcSimStop);
            hcSimPause = new Elements.HotkeyControl(5, 175, 250, "Simulation pause", Settings.k_SimulationPause);
            controls.Add(hcSimPause);
            hcUndo = new Elements.HotkeyControl(5,    200, 250, "Undo", Settings.k_Undo);
            controls.Add(hcUndo);
            hcCompRem = new Elements.HotkeyControl(5, 250, 250, "Remove single component", Settings.k_ComponentRemove);
            controls.Add(hcCompRem);
            hcEraser = new Elements.HotkeyControl(5, 275, 250, "Eraser", Settings.k_Eraser);
            controls.Add(hcEraser);
            hcZoomIn = new Elements.HotkeyControl(5, 300, 250, "Zoom In", Settings.k_ZoomIn);
            controls.Add(hcZoomIn);
            hcZoomOut = new Elements.HotkeyControl(5, 325, 250, "Zoom Out", Settings.k_ZoomOut);
            controls.Add(hcZoomOut);
            #endregion

            base.Initialize();
            background = GUIEngine.s_mainMenu.background;
        }

        public override void LoadContent()
        {
            bg = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");
            base.LoadContent();
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (e.key == Keys.Escape.GetHashCode())
            {
                sbClick(null, null);
                return;
            }
            if (e.key == Keys.S.GetHashCode())
            {
                sacClick(null, null);
                return;
            }
            base.onKeyPressed(e);
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            sb.position.X += w - oldw;
            sb.position.Y += h - oldh;
            sac.position.X += w - oldw;
            sac.position.Y += h - oldh;
            ap.position.X += w - oldw;
            ap.position.Y += h - oldh;

            lar.position.X = (int)(w / 2);
            lres.position.X = (int)(w / 2);
            fs.Position = new Vector2((int)(w / 2), fs.Position.Y);

            ar.position.X = lar.position.X + 150;
            res.position.X = lres.position.X + 150;

            fadeSize = new Vector2(Main.WindowWidth, Main.WindowHeight);
        }

        void mv_onValueChanged(object sender, Elements.ScrollBar.ValueChangedArgs e)
        {
            Settings.MasterVolume = (float)e.value / 100f;
        }

        void ev_onValueChanged(object sender, Elements.ScrollBar.ValueChangedArgs e)
        {
            Settings.EffectsVolume = (float)e.value / 100f;
        }

        void mzv_onValueChanged(object sender, Elements.ScrollBar.ValueChangedArgs e)
        {
            Settings.MusicVolume = (float)e.value / 100f;
        }

        void res_onElementSelected(object sender, string text)
        {
        }

        public void InitResForAR()
        {
            res.ItemsClear();
            if (ar.text == "4*3")
            {
                if (maxw >= 800 && maxh >= 600) res.ItemsAdd("800*600");
                if (maxw >= 1024 && maxh >= 768) res.ItemsAdd("1024*768");
                if (maxw >= 1280 && maxh >= 960) res.ItemsAdd("1280*960");
                if (maxw >= 1600 && maxh >= 1200) res.ItemsAdd("1600*1200");
                res.text = "800*600";
            }
            if (ar.text == "5*3")
            {
                if (maxw >= 800 && maxh >= 480) res.ItemsAdd("800*480");
                if (maxw >= 1280 && maxh >= 768) res.ItemsAdd("1280*768");
                if (maxw >= 1600 && maxh >= 960) res.ItemsAdd("1600*960");
                res.text = "800*480";
            }
            if (ar.text == "16*9")
            {
                if (maxw >= 1280 && maxh >= 720) res.ItemsAdd("1280*720");
                if (maxw >= 1360 && maxh >= 768) res.ItemsAdd("1360*768");
                if (maxw >= 1600 && maxh >= 900) res.ItemsAdd("1600*900");
                if (maxw >= 1920 && maxh >= 1080) res.ItemsAdd("1920*1080");
                res.text = "1280*720";
            }
            if (ar.text == "16*10")
            {
                if (maxw >= 1280 && maxh >= 800) res.ItemsAdd("1280*800");
                if (maxw >= 1440 && maxh >= 900) res.ItemsAdd("1440*900");
                if (maxw >= 1680 && maxh >= 1050) res.ItemsAdd("1680*1050");
                if (maxw >= 1920 && maxh >= 1200) res.ItemsAdd("1920*1200");
                res.text = "1280*800";
            }
        }

        void ar_onElementSelected(object sender, string text)
        {
            InitResForAR();
        }

        public override void onShow()
        {
            fadeSize = new Vector2();
            base.onShow();
            ar.text = Settings.AspectRatio;
            InitResForAR();
            res.text = Settings.Resolution;
            fs.Checked = Settings.IsFullscreen;
            mv.Value = (int)(Settings.MasterVolume * 100);
            ev.Value = (int)(Settings.EffectsVolume * 100);
            mzv.Value = (int)(Settings.MusicVolume * 100);
        }

        public void sbClick(object sender, InputEngine.MouseArgs e)
        {
            Settings.Load();
            Settings.ChangeResolution();
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(CameFromScene, CameFromState);
            //Main.curState = CameFromState;
            //GUIEngine.curScene = CameFromScene;
        }

        public void sacClick(object sender, InputEngine.MouseArgs e)
        {
            //if (Settings.Resolution != res.text)
            //{
            Settings.AspectRatio = ar.text;
            Settings.Resolution = res.text;
            Settings.IsFullscreen = fs.Checked;
            //}
            Settings.ChangeResolution();
            Sound.SoundPlayer.PlayButtonClick();
            Settings.Save();
            GUIEngine.ChangeScene(CameFromScene, CameFromState);

            #region Hotkeys
            //Settings.k_SimulationStart = hcSimStart.Key;
            //Settings.k_SimulationStop = hcSimStop.Key;
            //Settings.k_SimulationPause = hcSimPause.Key;
            #endregion
            //Main.curState = CameFromState;
            //GUIEngine.curScene = CameFromScene;
        }

        public void apClick(object sender, InputEngine.MouseArgs e)
        {
            Settings.ChangeResolution(res.text, fs.Checked);
            Sound.SoundPlayer.PlayButtonClick();
        }


        Vector2 fadeSize = new Vector2();
        public override void Draw(Renderer renderer)
        {
            if (background != null) background.Draw(renderer);
            if (fadeSize.Y != Main.WindowHeight)
                renderer.SetScissorRectangle((int)(Main.WindowWidth - fadeSize.X) / 2, (int)(Main.WindowHeight - fadeSize.Y) / 2,
                    (int)fadeSize.X, (int)fadeSize.Y, false);
            RenderHelper.SmartDrawRectangle(bg, 5, 0, 0, Main.WindowWidth, Main.WindowHeight, Color.White, renderer);
            foreach (Elements.Control c in controls)
            {
                c.Draw(renderer);
            }
            if (fadeSize.X != Main.WindowWidth)
                renderer.ResetScissorRectangle();
        }

        public override void FadeIn()
        {
            fadeSize = new Vector2();
            Vector2 delta = new Vector2((float)Main.WindowWidth / 50f, (float)Main.WindowHeight / 50f);
            fadeSize.Y += delta.Y;
            //x
            for (int i = 0; i < 10; i++)
            {
                fadeSize.X += delta.X / 2f;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 40; i++)
            {
                fadeSize.X += delta.X;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 10; i++)
            {
                fadeSize.X += delta.X / 2f;
                System.Threading.Thread.Sleep(5);
            }
            //y
            for (int i = 0; i < 10; i++)
            {
                fadeSize.Y += delta.Y / 2f;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 39; i++)
            {
                fadeSize.Y += delta.Y;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 10; i++)
            {
                fadeSize.Y += delta.Y / 2f;
                System.Threading.Thread.Sleep(5);
            }
        }

        public override void FadeOut()
        {
            Vector2 delta = new Vector2((float)Main.WindowWidth / 50f, (float)Main.WindowHeight / 50f);
            //y
            for (int i = 0; i < 10; i++)
            {
                fadeSize.Y -= delta.Y / 2f;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 39; i++)
            {
                fadeSize.Y -= delta.Y;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 10; i++)
            {
                fadeSize.Y -= delta.Y / 2f;
                System.Threading.Thread.Sleep(5);
            }
            //x
            for (int i = 0; i < 10; i++)
            {
                fadeSize.X -= delta.X / 2f;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 40; i++)
            {
                fadeSize.X -= delta.X;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 10; i++)
            {
                fadeSize.X -= delta.X / 2f;
                System.Threading.Thread.Sleep(5);
            }
        }

        #region ioblock
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            e.Handled = true;
        }

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

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
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

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            base.onMouseWheelMove(e);
            e.Handled = true;
        }
        #endregion
    }
}
