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
    class OptionsGraphics : MenuFrameScene
    {
        int maxw, maxh;

        Elements.ComboBox co_aspectRation, co_resolution;
        Elements.Label l_aspectRatio, l_resolution;
        Elements.CheckBox cb_fullscreen, cb_drawgrig, cb_warning;
        Elements.MenuButton b_apply;

        internal String origRes = "";
        internal String origAR = "";
        internal bool origFS = false;

        public override void Initialize()
        {
            ButtonsCount = FrameButtonsCount.One;

            #region MaxRes
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

            #region Resoulutions
            l_aspectRatio = new Elements.Label((int)Position.X + 5, (int)Position.Y + 5, "Aspect ratio:");
            l_aspectRatio.foreground = Color.White;
            controls.Add(l_aspectRatio);

            co_aspectRation = new Elements.ComboBox((int)Position.X + 150, (int)Position.Y + 7, 120, 24);
            co_aspectRation.onElementSelected += new Elements.ComboBox.ElementSelectedHandler(ar_onElementSelected);
            controls.Add(co_aspectRation);
            co_aspectRation.ItemsAdd("4*3");
            co_aspectRation.ItemsAdd("5*3");
            if (maxw >= 1280 && maxh >= 720) co_aspectRation.ItemsAdd("16*9");
            if (maxw >= 1280 && maxh >= 800) co_aspectRation.ItemsAdd("16*10");

            l_resolution = new Elements.Label((int)Position.X + 5, (int)Position.Y + 35, "Resolution:");
            l_resolution.foreground = Color.White;
            controls.Add(l_resolution);

            co_resolution = new Elements.ComboBox((int)Position.X + 150, (int)Position.Y + 37, 120, 24);
            co_resolution.onElementSelected += new Elements.ComboBox.ElementSelectedHandler(res_onElementSelected);
            controls.Add(co_resolution);

            cb_fullscreen = new Elements.CheckBox((int)Position.X + 5, (int)Position.Y + 65, 200, 20, "FullScreen", false);
            cb_fullscreen.foreground = Color.White;
            cb_fullscreen.onCheckedChanged += new Elements.CheckBox.CheckBoxCheckedHandler(cb_fullscreen_onCheckedChanged);
            controls.Add(cb_fullscreen);

            b_apply = new Elements.MenuButton((int)(Position.X + Size.X) - 120, (int)(Position.Y + Size.Y) - 23, 120, 23, 
                "Apply");
            b_apply.Font = ButtonFont;
            b_apply.onClicked += new Elements.Button.ClickedEventHandler(apply_onClicked);
            controls.Add(b_apply);
            #endregion

            cb_drawgrig = new Elements.CheckBox((int)Position.X + 5, (int)Position.Y + 95, 200, 20, "Draw grid", false);
            cb_drawgrig.foreground = Color.White;
            cb_drawgrig.onCheckedChanged += new Elements.CheckBox.CheckBoxCheckedHandler(cb_drawgrig_onCheckedChanged);
            controls.Add(cb_drawgrig);

            cb_warning = new Elements.CheckBox((int)Position.X + 5, (int)Position.Y + 125, 200, 20, "Show intro warning", false);
            cb_warning.foreground = Color.White;
            cb_warning.onCheckedChanged += new Elements.CheckBox.CheckBoxCheckedHandler(cb_warning_onCheckedChanged);
            controls.Add(cb_warning);

            base.Initialize();
        }

        void cb_warning_onCheckedChanged(object sender, bool IsChecked)
        {
            Settings.IntroWarningShow = IsChecked;
        }

        void cb_fullscreen_onCheckedChanged(object sender, bool IsChecked)
        {
            Settings.IsFullscreen = IsChecked;
        }

        void cb_drawgrig_onCheckedChanged(object sender, bool IsChecked)
        {
            GridDraw.ShouldDrawGrid = IsChecked;
        }

        YesNoMessageBox resChangeMb;
        int TicksSinceResChange = 0;
        void apply_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Settings.ChangeResolution();
            resChangeMb = YesNoMessageBox.Show("Do you want to keep this resolution?\r\nChanges will be reverted in 15 second(s)");
            resChangeMb.onButtonClicked += new YesNoMessageBox.ButtonClickedEventHandler(mb_onButtonClicked);
            TicksSinceResChange = 0;
        }

        void mb_onButtonClicked(object sender, YesNoMessageBox.ButtonClickedArgs e)
        {
            if (e.button == 0)
            {
                Settings.Resolution = origRes;
                Settings.AspectRatio = origAR;
                Settings.IsFullscreen = origFS;
                co_resolution.text = origRes;
                co_aspectRation.text = origAR;
                cb_fullscreen.Checked = origFS;
                Settings.ChangeResolution();
            }
            else
            {
                origAR = Settings.AspectRatio;
                origRes = Settings.Resolution;
                origFS = Settings.IsFullscreen;
            }
            resChangeMb.Close();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            Vector2 p = GetPosForWH(w, h);
            l_aspectRatio.Position = new Vector2((int)p.X + 5, (int)p.Y + 5);
            co_aspectRation.Position = new Vector2((int)p.X + 150, (int)p.Y + 7);
            l_resolution.Position = new Vector2((int)p.X + 5, (int)p.Y + 35);
            co_resolution.Position = new Vector2((int)p.X + 150, (int)p.Y + 37);
            cb_fullscreen.Position = new Vector2((int)p.X + 5, (int)p.Y + 65);
            cb_drawgrig.Position = new Vector2((int)p.X + 5, (int)p.Y + 95);
            cb_warning.Position = new Vector2((int)p.X + 5, (int)p.Y + 125);
            b_apply.Position = new Vector2((int)(p.X + GetSizeForWH(w, h).X) - 120, 
                (int)(p.Y + GetSizeForWH(w, h).Y) - 23);

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void OnGraphicsDeviceReset()
        {
            b_apply.WasInitiallyDrawn = false;
            base.OnGraphicsDeviceReset();
        }

        public override void onShow()
        {
            base.onShow();
            co_aspectRation.text = Settings.AspectRatio;
            InitResForAR();
            co_resolution.text = Settings.Resolution;
            cb_fullscreen.Checked = Settings.IsFullscreen;
            cb_drawgrig.Checked = GridDraw.ShouldDrawGrid;
            cb_warning.Checked = Settings.IntroWarningShow;
        }

        void ar_onElementSelected(object sender, string text)
        {
            InitResForAR();
            Settings.AspectRatio = text;
            Settings.Resolution = co_resolution.text;
        }

        void res_onElementSelected(object sender, string text)
        {
            Settings.Resolution = text;
        }

        public override void Update()
        {
            if (resChangeMb != null && resChangeMb.isVisible)
            {
                TicksSinceResChange++;
                if (TicksSinceResChange >= 15 * 60)
                {
                    TicksSinceResChange = 0;
                    resChangeMb.Close();
                    mb_onButtonClicked(null, new YesNoMessageBox.ButtonClickedArgs() { button = 0 });
                }
                else
                {
                    resChangeMb.Text = "Do you want to keep this resolution?\r\nChanges will be reverted in " +
                        ((int)((15 * 60 - TicksSinceResChange) / 60)).ToString() + " second(s)";
                }
            }

            base.Update();
        }

        public void InitResForAR()
        {
            co_resolution.ItemsClear();
            if (co_aspectRation.text == "4*3")
            {
                //if (maxw >= 800 && maxh >= 600) co_resolution.ItemsAdd("800*600");
                //if (maxw >= 1024 && maxh >= 768) co_resolution.ItemsAdd("1024*768");
                if (maxw >= 1280 && maxh >= 960) co_resolution.ItemsAdd("1280*960");
                if (maxw >= 1600 && maxh >= 1200) co_resolution.ItemsAdd("1600*1200");
                co_resolution.text = "1280*960";
            }
            if (co_aspectRation.text == "5*3")
            {
                //if (maxw >= 800 && maxh >= 480) co_resolution.ItemsAdd("800*480");
                if (maxw >= 1280 && maxh >= 768) co_resolution.ItemsAdd("1280*768");
                if (maxw >= 1600 && maxh >= 960) co_resolution.ItemsAdd("1600*960");
                co_resolution.text = "1280*768";
            }
            if (co_aspectRation.text == "16*9")
            {
                if (maxw >= 1280 && maxh >= 720) co_resolution.ItemsAdd("1280*720");
                if (maxw >= 1360 && maxh >= 768) co_resolution.ItemsAdd("1360*768");
                if (maxw >= 1600 && maxh >= 900) co_resolution.ItemsAdd("1600*900");
                if (maxw >= 1920 && maxh >= 1080) co_resolution.ItemsAdd("1920*1080");
                co_resolution.text = "1280*720";
            }
            if (co_aspectRation.text == "16*10")
            {
                if (maxw >= 1280 && maxh >= 800) co_resolution.ItemsAdd("1280*800");
                if (maxw >= 1440 && maxh >= 900) co_resolution.ItemsAdd("1440*900");
                if (maxw >= 1680 && maxh >= 1050) co_resolution.ItemsAdd("1680*1050");
                if (maxw >= 1920 && maxh >= 1200) co_resolution.ItemsAdd("1920*1200");
                co_resolution.text = "1280*800";
            }
        }
    }
}
