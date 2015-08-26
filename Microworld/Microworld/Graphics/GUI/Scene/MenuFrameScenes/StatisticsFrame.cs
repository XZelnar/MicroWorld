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
    class StatisticsFrame : MenuFrameScene
    {
        Elements.Label[] tl = new Elements.Label[8];
        Elements.Label[] l = new Elements.Label[8];
        Elements.MenuButton res;

            int mw = 0;
        public override void Initialize()
        {
            ButtonsCount = FrameButtonsCount.One;

            #region ControlButtons

            res = new Elements.MenuButton("Reset");
            res.Position = new Vector2((int)(Position.X + Size.X) - 120, (int)(Position.Y + Size.Y) - 23);
            res.Size = new Vector2(120, 23);
            res.Font = ButtonFont;
            res.onClicked += new Elements.Button.ClickedEventHandler(resClick);
            controls.Add(res);
            #endregion

            for (int i = 0; i < l.Length; i++)
            {
                tl[i] = new Elements.Label((int)Position.X + 10, (int)Position.Y + 10 + i * 40, "");
                tl[i].font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_14");
                tl[i].foreground = Color.White;
                controls.Add(tl[i]);
            }
            tl[0].text = "Wires placed: ";
            tl[1].text = "Components placed: ";
            tl[2].text = "Times started: ";
            tl[3].text = "Components Removed: ";
            tl[4].text = "Wires burned: ";
            tl[5].text = "Buttons clicked: ";
            tl[6].text = "Text entered: ";
            tl[7].text = "Game start: ";

            for (int i = 0; i < l.Length; i++)
            {
                if (tl[i].size.X > mw) mw = (int)tl[i].size.X;
            }

            mw += 85;
            for (int i = 0; i < l.Length; i++)
            {
                l[i] = new Elements.Label((int)Position.X + mw, (int)Position.Y + 10 + i * 40, "");
                l[i].font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_14");
                l[i].foreground = Color.White;
                controls.Add(l[i]);
            }

            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            res.position = new Vector2((int)(GetPosForWH(w, h).X + GetSizeForWH(w, h).X) - 120, 
                (int)(GetPosForWH(w, h).Y + GetSizeForWH(w, h).Y) - 23);

            for (int i = 0; i < l.Length; i++)
            {
                tl[i].position = new Vector2((int)Position.X + 30, (int)Position.Y + 30 + i * 40);
                l[i].position = new Vector2((int)Position.X + mw, (int)Position.Y + 30 + i * 40);
            }

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void onShow()
        {
            base.onShow();
        }

        public void resClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            YesNoMessageBox mb = YesNoMessageBox.Show("All statistics will be lost.\r\nContinue?");
            mb.onButtonClicked += new YesNoMessageBox.ButtonClickedEventHandler(mb_onButtonClicked);
        }

        void mb_onButtonClicked(object sender, YesNoMessageBox.ButtonClickedArgs e)
        {
            if (e.button == 1)
            {
                Statistics.Reset();
            }
        }

        public override void Update()
        {
            base.Update();

            l[0].text = Math.Round(Statistics.WireLengthPlaced, 2).ToString() + " mm";
            l[1].text = Statistics.ElementsPlaced.ToString();
            l[2].text = Statistics.TimesSimulationStarted.ToString();
            l[3].text = Statistics.ComponentsRemoved.ToString();
            l[4].text = Statistics.WiresLengthBurned.ToString() + " mm";
            l[5].text = Statistics.ButtonsClicked.ToString();
            l[6].text = Statistics.TextCharsEntered.ToString() + " characters";
            l[7].text = Statistics.GameStarts.ToString() + " times";
        }
    }
}
