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
    class OptionsAudio : MenuFrameScene
    {
        Elements.ScrollBar mv, ev, mzv;
        Elements.Label lmv, lev, lmzv;

        public override void Initialize()
        {
            #region MasterVolume
            lmv = new Elements.Label((int)Position.X + 5, (int)Position.Y + 15, "Master Volume");
            lmv.foreground = Color.White;
            controls.Add(lmv);

            mv = new Elements.ScrollBar((int)Position.X + 165, (int)Position.Y + 19, 200, 20);
            mv.onValueChanged += new Elements.ScrollBar.ValueChangedEventHandler(mv_onValueChanged);
            mv.MinValue = 0;
            mv.MaxValue = 100;
            mv.Value = (int)(Settings.MasterVolume * 100);
            controls.Add(mv);
            #endregion

            #region EffectVolume
            lev = new Elements.Label((int)Position.X + 5, (int)Position.Y + 45, "Effects Volume");
            lev.foreground = Color.White;
            controls.Add(lev);

            ev = new Elements.ScrollBar((int)Position.X + 165, (int)Position.Y + 49, 200, 20);
            ev.onValueChanged += new Elements.ScrollBar.ValueChangedEventHandler(ev_onValueChanged);
            ev.MinValue = 0;
            ev.MaxValue = 100;
            ev.Value = (int)(Settings.EffectsVolume * 100);
            controls.Add(ev);
            #endregion

            #region MusicVolume
            lmzv = new Elements.Label((int)Position.X + 5, (int)Position.Y + 75, "Music Volume");
            lmzv.foreground = Color.White;
            controls.Add(lmzv);

            mzv = new Elements.ScrollBar((int)Position.X + 165, (int)Position.Y + 79, 200, 20);
            mzv.onValueChanged += new Elements.ScrollBar.ValueChangedEventHandler(mzv_onValueChanged);
            mzv.MinValue = 0;
            mzv.MaxValue = 100;
            mzv.Value = (int)(Settings.MusicVolume * 100);
            controls.Add(mzv);
            #endregion

            base.Initialize();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            lmv.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 15);
            mv.Position = new Vector2((int)GetPosForWH(w, h).X + 165, (int)GetPosForWH(w, h).Y + 19);

            lev.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 45);
            ev.Position = new Vector2((int)GetPosForWH(w, h).X + 165, (int)GetPosForWH(w, h).Y + 49);

            lmzv.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 75);
            mzv.Position = new Vector2((int)GetPosForWH(w, h).X + 165, (int)GetPosForWH(w, h).Y + 79);

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void onShow()
        {
            base.onShow();
            mv.Value = (int)(Settings.MasterVolume * 100);
            ev.Value = (int)(Settings.EffectsVolume * 100);
            mzv.Value = (int)(Settings.MusicVolume * 100);
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
    }
}
