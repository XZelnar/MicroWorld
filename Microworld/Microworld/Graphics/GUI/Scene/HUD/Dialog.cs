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
    class Dialog : HUDScene
    {
        public class Character
        {
            public String ID = "";
            public String Name = "";
            public Texture2D Image = null;

            public Character()
            {
            }

            public Character(String id, String name, Texture2D img = null)//TODO rm null
            {
                ID = id;
                Name = name;
                Image = img;
            }
        }

        Texture2D bg;
        SpriteFont dialogFont, hintFont, characterFont;
        RenderTarget2D fbo;

        public Vector2 position = new Vector2(), size = new Vector2(1, 1), textSize = new Vector2();

        String _txt = "";
        String text = "";

        Character character = null;
        public static List<Character> characters = new List<Character>();

        private int _ticks = 0;
        private float opacity = 0f;
        public float colorLinkOverlay = 0f;
        private bool isClosing = false;
        private int ClosingTick = 0;

        private BorderedButton skip;

        #region Events
        public class DialogClickedArgs
        {
            public int button;//left == 0, right == 1, middle = 2, skip = 3
        }
        public delegate void DialogClickedEventHandler(object sender, DialogClickedArgs e);
        public static event DialogClickedEventHandler OnDialogClicked;
        #endregion

        public Dialog()
        {
            OnDialogClicked += new DialogClickedEventHandler(Dialog_OnDialogClicked);
        }

        public void ReGenFBO()
        {
            if (fbo != null)
                fbo.Dispose();
            fbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, (int)size.X, (int)size.Y);
        }

        public override void Initialize()
        {
            Layer = 900;

            skip = new BorderedButton((int)position.X + 78, (int)position.Y + 78, 80, 16, "Skip dialog");
            skip.onClicked += new Button.ClickedEventHandler(skip_onClicked);
            skip.background = Color.White * 0.7f;
            controls.Add(skip);

            base.Initialize();
        }

        void skip_onClicked(object sender, InputEngine.MouseArgs e)
        {
            OnDialogClicked(this, new DialogClickedArgs() { button = 3 });
        }

        public override void LoadContent()
        {
            ReGenFBO();
            bg = ResourceManager.Load<Texture2D>("GUI/bgbw");
            dialogFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_14");
            hintFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_9_b");
            characterFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_10");
            skip.Font = hintFont;
            skip.texture = bg;

            characters.Add(new Character("%Player%", "Max"));
            characters.Add(new Character("%Engineer%", "Jake"));
            characters.Add(new Character("%Scientist%", "Alise"));

            base.LoadContent();
        }

        void Dialog_OnDialogClicked(object sender, DialogClickedArgs e)
        {
            if ((e.button == 0 || e.button == 1) && !isClosing)
            {
                ForceClose();
            }
        }

        public void ForceClose()
        {
            isClosing = true;
            ClosingTick = _ticks + 20;
        }

        private void FitText(ref String s, out Vector2 textsize)
        {
            var a = s.Split(' ');
            String r = "";
            for (int i = 0; i < a.Length; i++)
            {
                if (dialogFont.MeasureString(r + a[i] + " ").X < size.X - 4)
                {
                    r += a[i] + " ";
                }
                else
                {
                    r += "\r\n" + a[i] + " ";
                }
            }
            s = r;
            textsize = dialogFont.MeasureString(r);
        }

        public void SetText(String character, String txt)
        {
            ReplaceCharactersInText(ref txt);
            _txt = txt;
            this.character = GetCharacterForID(character);

            text = (String)_txt.Clone();
            Vector2 v = new Vector2();
            FitText(ref text, out v);
            size.Y = v.Y + 18 < 96 ? 96 : v.Y + 18;
            textSize = v;
            ReGenFBO();
            position.Y = Main.WindowHeight - 100 - size.Y;
        }

        public void ReplaceCharactersInText(ref String s)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                s = s.Replace(characters[i].ID, characters[i].Name);
            }
        }

        public Character GetCharacterForID(String id)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].ID == id)
                    return characters[i];
            }
            return new Character(id, id);
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            position.X = 150;
            position.Y = h - 100;
            size.X = w - 150 - position.X;

            ReGenFBO();

            base.OnResolutionChanged(w, h, oldw, oldh);

            SetText(character == null ? "" : character.ID, _txt);//TODO
        }

        public override void onShow()
        {
            isClosing = false;
            ClosingTick = 0;
            _ticks = 0;
            base.onShow();
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            if (IsIn(e.curState.X, e.curState.Y))
            {
                Sound.SoundPlayer.PlayButtonClick();
                if (OnDialogClicked != null)
                {
                    e.Handled = true;
                    OnDialogClicked.Invoke(this, new DialogClickedArgs() { button = e.button });
                }
            }
        }

        float d;
        public override void Update()
        {
            if (!Graphics.GUI.GUIEngine.ContainsHUDScene(Graphics.GUI.GUIEngine.s_mainMenu))
            {
                Vector2 p = new Vector2(InputEngine.curMouse.X, InputEngine.curMouse.Y);
                Rectangle r = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
                d = Math.Abs(Utilities.Tools.DistancePointToRectangle(p, r));
                if (d > 200)
                {
                    opacity = 0.6f;
                }
                else
                {
                    opacity = (float)(1f - 0.4 * d / 200f);
                }

                _ticks++;
                if (_ticks <= 20) opacity *= (float)_ticks / 24f;
                if (isClosing && _ticks >= ClosingTick - 20) opacity *= (float)(ClosingTick - _ticks) / 24f;
                if (isClosing && _ticks >= ClosingTick)
                {
                    _ticks = 0;
                    //isVisible = false;
                    Graphics.GUI.GUIEngine.RemoveHUDScene(this);
                }
            }

            var ren = GraphicsEngine.Renderer;
            ren.EnableFBO(fbo);
            ren.GraphicsDevice.Clear(Color.Transparent);
            ren.BeginUnscaled();
            DrawToFBO(ren);
            ren.End();
            ren.DisableFBO();

            skip.Position = new Vector2((int)position.X + 80, (int)(position.Y + size.Y) - skip.Size.Y - 2);
            skip.opacity = opacity;

            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            renderer.Draw(fbo, new Vector2((int)position.X, (int)position.Y), Color.White * opacity);

            base.Draw(renderer);
        }

        public void DrawToFBO(Renderer renderer)
        {
            RenderHelper.SmartDrawRectangle(bg, 3, 0, 0, (int)size.X, (int)size.Y, Color.White, renderer);
            RenderHelper.SmartDrawRectangle(bg, 3, 0, 0, 80, (int)size.Y, Color.White, renderer);
            RenderHelper.SmartDrawRectangle(bg, 3, 0, 0, 80, 96, Color.White, renderer);
            RenderHelper.SmartDrawRectangle(bg, 3, 0, 0, 80, 80, Color.White, renderer);

            renderer.DrawStringRight(hintFont, "Click this message to continue...", new Rectangle(0, (int)size.Y - 18, (int)size.X - 8, 12), Color.Gray);

            renderer.DrawStringCentered(characterFont, character.Name, new Rectangle(0, 78, 80, 16), Color.White);
            renderer.DrawStringCentered(dialogFont, text, new Rectangle(80, (int)(size.Y - textSize.Y) / 2 - 2, (int)size.X - 80, (int)textSize.Y), Color.White);

            if (character.Image != null)
                renderer.Draw(character.Image, new Rectangle(2, 2, 76, 76), Color.White);
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
