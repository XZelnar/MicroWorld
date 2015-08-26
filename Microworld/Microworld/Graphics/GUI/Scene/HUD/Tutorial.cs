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
    class Tutorial : HUDScene
    {
        Texture2D bg;
        public Color background = Color.White;
        public Color linkOverlay = new Color(0, 255, 255);

        public Vector2 position = new Vector2(), size = new Vector2();

        Label text;
        String _txt;
        String _link;

        /// <summary>
        /// Scene timeout in ticks
        /// </summary>
        public int Timeout = 200;
        private int _ticks = 0;
        private float opacity = 0f;
        public float colorLinkOverlay = 0f;

        #region Events
        public class TutorialClickedArgs
        {
            public int button;//left == 0, right == 1, middle = 2
        }
        public delegate void TutorialClickedEventHandler(object sender, TutorialClickedArgs e);
        public static event TutorialClickedEventHandler OnTutorialClicked;
        #endregion

        public Tutorial()
        {
            String txt = "";
            _txt = txt;
            //position = new Vector2(ComponentSelector.ElementSize.X, 0);
            //size = new Vector2(Main.WindowWidth - ComponentSelector.ElementSize.X, 0);

            text = new Label(0, 0, txt);
            text.position = position;
            Vector2 v = new Vector2();
            FitText(ref txt, out v);
            size.Y = v.Y;
            text.text = txt;
            text.size = size;
            text.TextAlignment = Graphics.Renderer.TextAlignment.Center;
            text.foreground = Color.White;

            controls.Add(text);

            OnTutorialClicked += new TutorialClickedEventHandler(Tutorial_OnTutorialClicked);
        }

        public override void Initialize()
        {
            Layer = 900;
            base.Initialize();
        }

        public override void LoadContent()
        {
            bg = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");
            text.font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_12");
            base.LoadContent();
        }

        void Tutorial_OnTutorialClicked(object sender, Tutorial.TutorialClickedArgs e)
        {
            if (e.button == 1)
            {
                if (!Main.curState.StartsWith("GAME")) return;
                if (_link != null && _link != "")
                {
                    //GUIEngine.s_encyclopediaPage.LastState = Main.curState;
                    //GUIEngine.s_encyclopediaPage.LastScene = GUIEngine.curScene;
                    //Main.curState = "GUIEncyclopedia";
                    //GUIEngine.curScene = GUIEngine.s_encyclopediaPage;
                    //GUIEngine.s_encyclopediaPage.OpenPage("Content/Encyclopedia/" + _link);

                    GUIEngine.AddHUDScene(GUIEngine.s_mainMenu);
                    GUIEngine.s_mainMenu.Show();
                    GUIEngine.s_mainMenu.InitForHandbook(true);
                    GUIEngine.s_handbook.InitForFolder("Content/Encyclopedia/" + _link.Substring(0, _link.IndexOf("/")));
                    GUIEngine.s_handbook.OpenPage("Content/Encyclopedia/" + _link);
                }
            }
            else if (e.button == 0)
            {
                if (Timeout != Logics.LevelEngine.NO_TIMEOUT)
                    ForceClose();
            }
        }

        public void ForceClose()
        {
            if (Timeout == Logics.LevelEngine.NO_TIMEOUT || _ticks < Timeout - 20)
                Timeout = _ticks + 20;
        }

        private void FitText(ref String s, out Vector2 textsize)
        {
            var a = s.Split(' ');
            String r = "";
            for (int i = 0; i < a.Length; i++)
            {
                if (text.font.MeasureString(r + a[i] + " ").X < size.X - 4)
                {
                    r += a[i] + " ";
                }
                else
                {
                    r += "\r\n" + a[i] + " ";
                }
            }
            s = r;
            textsize = text.font.MeasureString(r);
        }

        public void SetText(String txt)
        {
            _txt = txt;

            String s = (String)_txt.Clone();
            Vector2 v = new Vector2();
            FitText(ref s, out v);
            v.Y += 16;
            size.Y = v.Y + 4;
            text.text = s;
            text.size = size;
            position.Y = Main.WindowHeight - 100 - size.Y;
            text.position = position;
        }

        public void SetLink(String l)
        {
            colorLinkOverlay = 0;
            _link = "";
            if (l != null && l != "" && System.IO.File.Exists("Content/Encyclopedia/" + l))
                _link = l;
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            position.X = 150;
            position.Y = h - 100;
            size.X = w - 150 - position.X;

            //size = new Vector2(w - ComponentSelector.ElementSize.X, 0);
            String s = (String)_txt.Clone();
            Vector2 v = new Vector2();
            FitText(ref s, out v);
            size.Y = v.Y;
            text.text = s;
            text.size = size;
            text.position = position;
            text.position.X += 2;

            base.OnResolutionChanged(w, h, oldw, oldh);

            SetText(_txt);
        }

        public override void onShow()
        {
            _ticks = 0;
            opacity = 0f;
            text.foreground = Color.White * opacity;
            base.onShow();
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            if (IsIn(e.curState.X, e.curState.Y))
            {
                Sound.SoundPlayer.PlayButtonClick();
                if (OnTutorialClicked != null)
                {
                    e.Handled = true;
                    OnTutorialClicked.Invoke(this, new TutorialClickedArgs() { button = e.button });
                }
            }
        }

        public override void Dispose()
        {
            text.Dispose();
            text = null;
            base.Dispose();
        }

        float d;
        public override void Update()
        {
            if (!Graphics.GUI.GUIEngine.ContainsHUDScene(Graphics.GUI.GUIEngine.s_mainMenu))
            {
                Vector2 p = new Vector2(InputEngine.curMouse.X, InputEngine.curMouse.Y);
                Rectangle r = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
                d = Math.Abs(Utilities.Tools.DistancePointToRectangle(p, r));
                if (d > 100)
                {
                    opacity = 0.8333333333f;
                }
                else
                {
                    opacity = (float)(1f - 0.1666666666f * d / 100f);
                }

                _ticks++;
                if (_ticks <= 20) 
                    opacity = (float)_ticks / 24f;
                if (Timeout > 0)
                {
                    if (_ticks >= Timeout)
                    {
                        _ticks = 0;
                        isVisible = false;
                    }
                    if (_ticks >= Timeout - 20) 
                        opacity = (float)(Timeout - _ticks) / 24f;
                }
                text.foreground = Color.White * opacity;

                if (_link != "")
                {
                    colorLinkOverlay += 0.005f;
                    if (colorLinkOverlay >= 0.6f) colorLinkOverlay -= 0.6f;
                }
                if (Timeout != Logics.LevelEngine.NO_TIMEOUT && _ticks >= Timeout)
                {
                    Graphics.GUI.GUIEngine.RemoveHUDScene(this);
                }
            }
            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            RenderHelper.SmartDrawRectangle(bg, 6, (int)position.X, (int)position.Y, (int)size.X, (int)size.Y,
                background * opacity, renderer);
            //Main.renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
            //    background * opacity * 0.8f);
            Main.renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                linkOverlay * opacity * (colorLinkOverlay > 0.3f ? 0.6f - colorLinkOverlay : colorLinkOverlay) * 0.5f);
            if (Timeout != Logics.LevelEngine.NO_TIMEOUT)
                Shortcuts.renderer.DrawString(GUIEngine.font, "[Left-click to dismiss]",
                    new Rectangle((int)position.X, (int)(position.Y + size.Y - 17), (int)size.X, 12), Color.Gray * opacity * 0.8f, Renderer.TextAlignment.Left);
            if (_link != "")
                Shortcuts.renderer.DrawString(GUIEngine.font, "[Right-click to open the link]",
                    new Rectangle((int)position.X, (int)(position.Y + size.Y - 17), (int)size.X, 12), Color.Gray * opacity * 0.8f, Renderer.TextAlignment.Right);
            base.Draw(renderer);
        }

        public override Vector2 GetPosition()
        {
            return position;
        }

        public override Vector2 GetSize()
        {
            return size + new Vector2(8, 4);
        }

        public override bool IsIn(int x, int y)
        {
            return x >= position.X && x < position.X + size.X
                && y >= position.Y && y < position.Y + size.Y;
        }

    }
}
