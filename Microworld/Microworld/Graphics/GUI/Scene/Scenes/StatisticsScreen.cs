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
    class StatisticsScreen : Scene
    {
        Texture2D bg;

        public String CameFromState = "";
        public Scene CameFromScene = null;

        Elements.Label[] tl = new Elements.Label[8];
        Elements.Label[] l = new Elements.Label[8];
        Elements.EncyclopediaBrowserButton sb, res;
        public new void Initialize()
        {
            ShouldBeScaled = false;

            #region ControlButtons
            sb = new Elements.EncyclopediaBrowserButton(540, 440, 120, 30, "Back");
            (sb as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            sb.foreground = Color.White;
            sb.onClicked += new Elements.Button.ClickedEventHandler(sbClick);
            //sb.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            controls.Add(sb);

            res = new Elements.EncyclopediaBrowserButton(670, 440, 120, 30, "Reset");
            (res as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            res.foreground = Color.White;
            res.onClicked += new Elements.Button.ClickedEventHandler(resClick);
            //res.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            controls.Add(res);
            #endregion

            for (int i = 0; i < l.Length; i++)
            {
                tl[i] = new Elements.Label(10, 10 + i * 30, "");
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

            int mw = 0;
            for (int i = 0; i < l.Length; i++)
            {
                if (tl[i].size.X > mw) mw = (int)tl[i].size.X;
            }

            mw += 35;
            for (int i = 0; i < l.Length; i++)
            {
                l[i] = new Elements.Label(mw, 10 + i * 30, "");
                l[i].font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_14");
                l[i].foreground = Color.White;
                controls.Add(l[i]);
            }

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
                e.Handled = true;
                return;
            }
            base.onKeyPressed(e);
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            sb.position.X += w - oldw;
            sb.position.Y += h - oldh;
            res.position.X += w - oldw;
            res.position.Y += h - oldh;
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

        public void sbClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(CameFromScene, CameFromState);
            //Main.curState = CameFromState;
            //GUIEngine.curScene = CameFromScene;
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

    }
}
