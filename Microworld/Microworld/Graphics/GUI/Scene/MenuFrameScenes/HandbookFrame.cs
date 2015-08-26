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
    class HandbookFrame : MenuFrameScene//folders menu, tutorial message
    {
        String curFolder = "";
        List<Elements.MenuButton> buttons = new List<Elements.MenuButton>();
        Elements.MenuButton goToLink;
        Elements.Label lFolder;
        Elements.ScrollBar sbFileList, sbArticle;
        Encyclopedia.Article article;
        SpriteFont font;
        Texture2D file, folder;
        Vector2 FileListOffset = new Vector2();
        Vector2 ArticleOffset = new Vector2();

        public override void Initialize()
        {
            ButtonsCount = FrameButtonsCount.One;

            article = new Encyclopedia.Article();
            article.position = Position + new Vector2(260, 23 + 96);
            article.Size = Size - new Vector2(260, 23 + 96) - new Vector2(26, 28);
            article.view.OnPageLoaded += new Elements.HTMLViewer.PageLoaded(view_OnPageLoaded);

            lFolder = new Elements.Label((int)Position.X + 92, (int)Position.Y + 31, "");
            lFolder.foreground = Color.White;
            controls.Add(lFolder);

            sbFileList = new Elements.ScrollBar((int)Position.X + 278 - 7, (int)Position.Y + 100, 16, (int)Size.Y - 100 - 7);
            sbFileList.MaxValue = 10000;
            sbFileList.MinValue = 0;
            sbFileList.Value = 0;
            sbFileList.IsVertical = true;
            sbFileList.onValueChanged += new Elements.ScrollBar.ValueChangedEventHandler(sbFileList_onValueChanged);
            controls.Add(sbFileList);

            sbArticle = new Elements.ScrollBar((int)(Position.X + Size.X) - 8, (int)Position.Y + 100, 16, (int)Size.Y - 89 - 100 - 7);
            sbArticle.MaxValue = 1;
            sbArticle.MinValue = 0;
            sbArticle.Value = 0;
            sbArticle.IsVertical = true;
            sbArticle.onValueChanged += new Elements.ScrollBar.ValueChangedEventHandler(sbArticle_onValueChanged);
            controls.Add(sbArticle);

            goToLink = new Elements.MenuButton((int)(Position.X + Size.X - 120), (int)(Position.Y + Size.Y - 23), 120, 23, "Link");
            goToLink.isEnabled = false;
            goToLink.Font = ButtonFont;
            goToLink.onClicked += new Elements.Button.ClickedEventHandler(gotolink_onClicked);
            controls.Add(goToLink);

            base.Initialize();
        }

        public override void LoadContent()
        {
            font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_14");
            file = ResourceManager.Load<Texture2D>("GUI/Encyclopedia/file");
            folder = ResourceManager.Load<Texture2D>("GUI/Encyclopedia/folder");
            lFolder.font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_22");
            base.LoadContent();
        }

        void view_OnPageLoaded()
        {
            ArticleOffset = new Vector2();
            sbArticle.MaxValue = (int)Math.Max(1, article.view.ActualSize.Y - article.Size.Y);
            sbArticle.Value = 0;
        }

        void sbArticle_onValueChanged(object sender, Elements.ScrollBar.ValueChangedArgs e)
        {
            ArticleOffset.Y = -e.value;
            article.view.SetOffset(ArticleOffset);
        }

        void sbFileList_onValueChanged(object sender, Elements.ScrollBar.ValueChangedArgs e)
        {
            if (buttons.Count == 0)
                return;
            float h = -(buttons[buttons.Count - 1].Position.Y - Position.Y + buttons[buttons.Count - 1].Size.Y - Size.Y);
            FileListOffset.Y = sbFileList.Value * h / sbFileList.MaxValue;
        }

        void gotolink_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            article.GoToLink();
        }

        public override void onClose()
        {
            article.Clear();
            sbArticle.MaxValue = 1;
            sbArticle.Value = 0;
            base.onClose();
        }

        public void InitForFolder(String fol)
        {
            if (fol == "")
            {
                article.Clear();
                curFolder = "";
                lFolder.text = "";
                while (buttons.Count != 0)
                {
                    controls.Remove(buttons[0]);
                    buttons[0].Dispose();
                    buttons.RemoveAt(0);
                }
                sbFileList.MaxValue = 1;
                sbFileList.Value = 0;
                FileListOffset = new Vector2();
                return;
            }

            article.Clear();

            curFolder = fol + "/";
            String s = fol;
            s = s.Substring(0, s.Length - 1);
            s = s.Substring(s.LastIndexOf('/') + 1);
            lFolder.text = s;

            while (buttons.Count != 0)
            {
                controls.Remove(buttons[0]);
                buttons[0].Dispose();
                buttons.RemoveAt(0);
            }

            var a = System.IO.Directory.GetFiles(fol, "*.edf");
            for (int i = 0; i < a.Length; i++)
            {
                buttons.Add(new Elements.MenuButton((int)Position.X + 23, (int)Position.Y + 23 + buttons.Count * 48 + 96, 230, 36, 
                    GetFilenameFromPath(a[i])));
                buttons[i].onClicked += new Elements.Button.ClickedEventHandler(HandbookFrame_onClicked);
                buttons[i].Font = font;
                buttons[i].DrawBottomLine = false;
                buttons[i].TextOffset = new Vector2(20, 10);
                buttons[i].LeftTexture = file;
                buttons[i].TextureOffset = new Vector2(0, 1);
                buttons[i].Initialize();
            }

            float h = buttons[buttons.Count - 1].Position.Y - Position.Y + buttons[buttons.Count - 1].Size.Y - Size.Y;
            sbFileList.MaxValue = (int)h;
            sbFileList.Value = 0;
            FileListOffset = new Vector2();
        }

        private String GetFilenameFromPath(String s)
        {
            s = s.Substring(0, s.Length - 4);
            s = s.Substring(s.LastIndexOf('\\') + 1);
            return s;
        }

        void HandbookFrame_onClicked(object sender, InputEngine.MouseArgs e)
        {
            OpenPage(curFolder + (sender as Elements.MenuButton).Text + ".edf");
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].StaySelected = (buttons[i] == sender);
            }
        }

        public void OpenPage(String page)
        {
            article.Load(page);
            goToLink.isEnabled = article.HasLink;
            goToLink.WasInitiallyDrawn = false;
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            Vector2 pos = GetPosForWH(w, h);
            Vector2 s = GetSizeForWH(w, h);

            article.position = pos + new Vector2(279 + 28, 96 + 28);
            article.Size = s - new Vector2(279 + 28, 96 + 28);
            lFolder.Position = new Vector2((int)pos.X + 92, (int)pos.Y + 31);
            goToLink.Position = new Vector2((int)pos.X + (int)s.X - 120, (int)pos.Y + (int)s.Y - 23);
            sbFileList.Position = new Vector2((int)Position.X + 278 - 7, (int)Position.Y + 100);
            sbFileList.Size = new Vector2(16, (int)Size.Y - 100 - 7);
            sbArticle.Position = new Vector2((int)(Position.X + Size.X) - 8, (int)Position.Y + 100);
            sbArticle.Size = new Vector2(16, (int)Size.Y - 89 - 100 - 7);

            sbArticle.MaxValue = (int)Math.Max(0, article.view.ActualSize.Y - article.Size.Y);
            sbArticle.Value = 0;
            ArticleOffset = new Vector2();

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void OnGraphicsDeviceReset()
        {
            article.view.Refresh();
            goToLink.WasInitiallyDrawn = false;
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].WasInitiallyDrawn = false;
            }
            base.OnGraphicsDeviceReset();
        }

        public override void Update()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Update();
            }
            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            if (article != null)
                article.Draw(renderer);

            renderer.Draw(folder, Position + new Vector2(26, 26), Color.White);

            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)Position.X - 9, (int)Position.Y + 96, (int)Size.X + 9, 2), Color.White);//top bar
            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)(Position.X + Size.X) - 1, (int)Position.Y + 96, 1, 6), Color.White);//small thing near right sb
            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)Position.X + 278, (int)Position.Y + 96, 2, 6), Color.White);//small thing near left sb
            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)Position.X + 150, (int)Position.Y + (int)Size.Y - 2, 130, 2), Color.White);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)Position.X + 278, (int)Position.Y + (int)Size.Y - 7, 2, 6), Color.White);

            base.Draw(renderer);

            renderer.SetScissorRectangle(Position.X, Position.Y + 98, Size.X, Size.Y - 100, false);
            renderer.End();
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                        null, Graphics.GraphicsEngine.s_ScissorsOn, null,
                        Matrix.CreateTranslation(MainMenu.FrameOffset + FileListOffset.X, FileListOffset.Y, 0));

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Draw(renderer);
            }

            renderer.End();
            renderer.ResetScissorRectangle();
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                        null, RasterizerState.CullNone, null,
                        Matrix.CreateTranslation(MainMenu.FrameOffset, 0, 0));
        }

        #region IO
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            InputEngine.MouseArgs en = new InputEngine.MouseArgs()
            {
                button = e.button,
                curState = new MouseState(e.curState.X, e.curState.Y - (int)FileListOffset.Y, e.curState.ScrollWheelValue,
                    e.curState.LeftButton, e.curState.MiddleButton, e.curState.RightButton, e.curState.XButton1,
                    e.curState.XButton2)
            };
            if (IsIn(e.curState.X, e.curState.Y))
            {
                foreach (Elements.Control c in buttons)
                {
                    if (c.isVisible) 
                        c.onButtonClick(en);
                    if (InputEngine.eventHandled) 
                        break;
                }
                if (InputEngine.eventHandled)
                    return;

                base.onButtonClick(e);
            }
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            InputEngine.MouseArgs en = new InputEngine.MouseArgs()
            {
                button = e.button,
                curState = new MouseState(e.curState.X, e.curState.Y - (int)FileListOffset.Y, e.curState.ScrollWheelValue,
                    e.curState.LeftButton, e.curState.MiddleButton, e.curState.RightButton, e.curState.XButton1,
                    e.curState.XButton2)
            };
            if (IsIn(e.curState.X, e.curState.Y))
            {
                foreach (Elements.Control c in buttons)
                {
                    if (c.isVisible) c.onButtonDown(en);
                    if (InputEngine.eventHandled) break;
                }
            if (InputEngine.eventHandled)
                return;

            base.onButtonDown(e);
                }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            InputEngine.MouseArgs en = new InputEngine.MouseArgs()
            {
                button = e.button,
                curState = new MouseState(e.curState.X, e.curState.Y - (int)FileListOffset.Y, e.curState.ScrollWheelValue,
                    e.curState.LeftButton, e.curState.MiddleButton, e.curState.RightButton, e.curState.XButton1,
                    e.curState.XButton2)
            };
            foreach (Elements.Control c in buttons)
            {
                if (c.isVisible) c.onButtonUp(en);
                if (InputEngine.eventHandled) break;
            }
            if (InputEngine.eventHandled)
                return;

            base.onButtonUp(e);
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            InputEngine.MouseMoveArgs en = new InputEngine.MouseMoveArgs()
            {
                dx = e.dx,
                dy = e.dy,
                curState = new MouseState(e.curState.X, e.curState.Y - (int)FileListOffset.Y, e.curState.ScrollWheelValue,
                    e.curState.LeftButton, e.curState.MiddleButton, e.curState.RightButton, e.curState.XButton1,
                    e.curState.XButton2)
            };
            foreach (Elements.Control c in buttons)
            {
                if (c.isVisible) c.onMouseMove(en);
                if (InputEngine.eventHandled) break;
            }
            if (InputEngine.eventHandled)
                return;

            base.onMouseMove(e);
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            if (buttons.Count > 0 &&
                e.curState.X > Position.X && e.curState.Y > Position.Y + 96 &&
                e.curState.X < Position.X + 280 && e.curState.Y < Position.Y + Size.Y)
            {
                FileListOffset.Y += e.delta / 3;
                if (FileListOffset.Y > 0)
                    FileListOffset.Y = 0;
                float h = -(buttons[buttons.Count - 1].Position.Y - Position.Y + buttons[buttons.Count - 1].Size.Y - Size.Y);
                if (FileListOffset.Y < h)
                    FileListOffset.Y = h;

                sbFileList.Value = (int)(FileListOffset.Y / h * sbFileList.MaxValue);
            }

            onMouseMove(new InputEngine.MouseMoveArgs() { curState = e.curState });

            base.onMouseWheelMove(e);
        }
        #endregion
    }
}
