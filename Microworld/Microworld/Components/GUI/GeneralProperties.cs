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
using MicroWorld.Graphics.GUI.Scene;
using MicroWorld.Graphics.GUI;
using MicroWorld.Graphics;

namespace MicroWorld.Components.GUI
{
    public class GeneralProperties : HUDScene
    {
        public enum FadeState
        {
            None = 0,
            FadeIn = 1,
            FadeOut = 2
        }

        public static readonly Color bgColor = new Color(39, 49, 88);
        public static SpriteFont TitleFont = null;

        public RenderTarget2D fbo;
        public Vector2 position = new Vector2(), size = new Vector2();
        public Component AssociatedComponent = null;
        public FadeState fadeState = FadeState.None;
        public float Opacity = 0f;
        public Vector2 GameOffset = new Vector2();

        public Vector2 Size
        {
            get { return size; }
            set
            {
                size = value;
                ReCreateFBO();
            }
        }
        public bool StayOpened
        {
            get
            {
                return GUIEngine.ContainsHUDScene(GUIEngine.s_subComponentButtons) && GUIEngine.s_subComponentButtons.isVisible &&
                    GUIEngine.s_subComponentButtons.SelectedComponent == AssociatedComponent;
            }
            set
            {
                if (value == false)
                {
                    Close();
                }
            }
        }

        public bool WasInitialized = false;
        private bool WasJustShown = false;

        private bool dimOpacity = false;
        protected virtual bool DimOpacity
        {
            get { return InputEngine.Control; }
        }

        public Label ID;


        public MenuButton close, remove, help;


        public void ReCreateFBO()
        {
            if (fbo != null)
                fbo.Dispose();

            fbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, (int)size.X, (int)size.Y);
        }

        public override void Initialize()
        {
            if (this.GetType() == typeof(GeneralProperties))
            {
                Label title;
                CheckBox removable;

                WasInitialized = true;

                size = new Vector2(120, 80);

                title = new Label(0, 5, AssociatedComponent.Graphics.GetCSToolTip());
                title.font = TitleFont;
                title.UpdateSizeToTextSize();
                title.TextAlignment = Renderer.TextAlignment.Center;
                title.foreground = Color.White;
                controls.Add(title);

                size.X = Math.Max(title.font.MeasureString(title.text).X + 80, size.X);

                title.size.X = size.X - 20;

                removable = new CheckBox(5, 55, (int)size.X - 10, 20, "Removable: ", false);
                removable.foreground = Color.White;
                removable.onCheckedChanged += new CheckBox.CheckBoxCheckedHandler(removable_onCheckedChanged);
                controls.Add(removable);
            }

            Label l = new Label(5, 30, "ID:");
            l.foreground = Color.White;
            controls.Add(l);

            ID = new Label(5, 30, "0");
            ID.foreground = Color.White;
            ID.TextAlignment = Renderer.TextAlignment.Right;
            controls.Add(ID);

            remove = new MenuButton(5, 5, 20, 20, "");
            remove.LeftTexture = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/Remove");
            remove.onClicked += new Button.ClickedEventHandler(remove_onClicked);
            controls.Add(remove);

            close = new MenuButton((int)size.X - 25, 5, 20, 20, "");
            close.LeftTexture = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/Close");
            close.onClicked += new Button.ClickedEventHandler(close_onClicked);
            controls.Add(close);

            help = new MenuButton((int)size.X - 45, 5, 20, 20, "");
            help.LeftTexture = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/Help");
            help.onClicked += new Button.ClickedEventHandler(help_onClicked);
            controls.Add(help);

            base.Initialize();

            ReCreateFBO();
        }

        void help_onClicked(object sender, InputEngine.MouseArgs e)
        {
            String link = AssociatedComponent.Graphics.GetHandbookFile();

            if (!Main.curState.StartsWith("GAME") || link == null) return;
            if (link != null && link != "")
            {
                GUIEngine.AddHUDScene(GUIEngine.s_mainMenu);
                GUIEngine.s_mainMenu.Show();
                GUIEngine.s_mainMenu.InitForHandbook(true);
                GUIEngine.s_handbook.InitForFolder("Content/Encyclopedia/" + link.Substring(0, link.IndexOf("/")));
                GUIEngine.s_handbook.OpenPage("Content/Encyclopedia/" + link);
                InputEngine.eventHandled = true;
            }
        }

        void remove_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible &&
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent == AssociatedComponent)
            {
                MicroWorld.Graphics.GUI.GUIEngine.RemoveHUDScene(MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons);
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible = false;
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent = null;
                InputEngine.blockClick = true;
                AssociatedComponent.OnButtonClickedRemove();
                //Close();
            }
        }

        void close_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible &&
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent == AssociatedComponent)
            {
                MicroWorld.Graphics.GUI.GUIEngine.RemoveHUDScene(MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons);
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible = false;
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent = null;
                InputEngine.blockClick = true;
                Close();
            }
        }

        public void removable_onCheckedChanged(object sender, bool IsChecked)
        {
            AssociatedComponent.IsRemovable = IsChecked;
        }

        public virtual void tb_onLooseFocus()
        {
            Save();
        }

        public override void OnGraphicsDeviceReset()
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] is MicroWorld.Graphics.GUI.Elements.MenuButton)
                    (controls[i] as MicroWorld.Graphics.GUI.Elements.MenuButton).WasInitiallyDrawn = false;
            }

            base.OnGraphicsDeviceReset();
        }

        #region Events
        public override void onShow()
        {
            WasJustShown = true;
            isVisible = true;

            fadeState = FadeState.FadeIn;
            Opacity = -1f;

            ID.text = AssociatedComponent.ID.ToString();
            ID.Size = new Vector2((int)size.X - 10, 20);

            Load();

            base.onShow();
        }

        public override void Close()
        {
            fadeState = FadeState.FadeOut;
        }

        public override void onClose()
        {
            isVisible = false;
            InputEngine.blockClick = true;
            Save();
            base.onClose();
        }
        #endregion

        public virtual void Save()
        {
        }

        public virtual void Load()
        {
            (controls[1] as CheckBox).Checked = AssociatedComponent.IsRemovable;
            if (Main.curState == "GAMELevels")
            {
                (controls[1] as CheckBox).Enabled = false;
            }
            else
            {
                (controls[1] as CheckBox).Enabled = true;
            }
        }

        public override void Update()
        {
            if (!WasInitialized)
            {
                Initialize();
            }

            if (!isVisible)
                return;

            WasJustShown = false;

            base.Update();

            var p = AssociatedComponent.Graphics.Position + GameOffset;

            float px = p.X, py = p.Y;
            Vector2 s = AssociatedComponent.Graphics.GetSize() * Settings.GameScale;

            Utilities.Tools.GameToScreenCoords(ref px, ref py);
            position.X = px + s.X;
            position.Y = py + (s.Y - size.Y) / 2;



            remove.isVisible = AssociatedComponent.IsRemovable;
            remove.isEnabled = remove.isVisible;

            if (StayOpened)
            {
                if (Layer != 1)
                {
                    Layer = 1;
                    MicroWorld.Graphics.GUI.GUIEngine.SortScenes();
                }
            }
            else
                if (Layer != 0)
                {
                    Layer = 0;
                    MicroWorld.Graphics.GUI.GUIEngine.SortScenes();
                }


            /*
            #region EnabledFromState
            bool en = Settings.GameState == Settings.GameStates.Stopped;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] is Button && controls[i] != close)
                    (controls[i] as Button).isEnabled = en;
                if (controls[i] is TextBox)
                    (controls[i] as TextBox).Editable = en;
                if (controls[i] is CheckBox)
                    (controls[i] as CheckBox).Enabled = en;
            }

            #endregion
            //*/


            GraphicsEngine.Renderer.EnableFBO(fbo);
            GraphicsEngine.Renderer.GraphicsDevice.Clear(Color.Transparent);

            GraphicsEngine.Renderer.BeginUnscaled();
            DrawFBO(GraphicsEngine.Renderer);
            GraphicsEngine.Renderer.End();

            GraphicsEngine.Renderer.DisableFBO();

            #region FadeControl
            if (fadeState == FadeState.FadeIn)
            {
                Opacity += 0.05f;
                if (Opacity >= 1f)
                {
                    Opacity = 1f;
                    fadeState = FadeState.None;
                }
            }
            if (fadeState == FadeState.FadeOut)
            {
                Opacity -= 0.05f;
                if (Opacity <= 0f)
                {
                    Opacity = 0f;
                    fadeState = FadeState.None;
                    base.Close();
                }
            }
            #endregion
        }

        public override void Draw(Renderer renderer)
        {
            if (Opacity > 0)
            {
                float o = DimOpacity ? Opacity / 4f : Opacity;
                //bg
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), bgColor * o);
                //borger
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)position.Y, 1, (int)size.Y), Color.White * o);
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)position.Y, (int)size.X, 1), Color.White * o);
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)(position.X + size.X - 1), (int)position.Y, 1, (int)size.Y), Color.White * o);
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)(position.Y + size.Y - 1), (int)size.X, 1), Color.White * o);

                renderer.End();
                GraphicsEngine.Renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
                    RasterizerState.CullNone);
                renderer.Draw(fbo, position, Color.White * o);
                renderer.End();
                renderer.BeginUnscaled();
            }
        }

        public virtual void DrawFBO(Renderer renderer)
        {
            base.Draw(renderer);
        }




        #region IO
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            if (WasJustShown || !StayOpened)
                return;

            if (DimOpacity)
                return;

            InputEngine.MouseArgs ee = new InputEngine.MouseArgs()
            {
                button = e.button,
                curState = new MouseState(
                    e.curState.X - (int)position.X,
                    e.curState.Y - (int)position.Y,
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton,
                    e.curState.MiddleButton,
                    e.curState.RightButton,
                    e.curState.XButton1,
                    e.curState.XButton2)
            };

            base.onButtonDown(ee);
            if (IsIn(e.curState.X, e.curState.Y) && StayOpened)
                e.Handled = true;
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            if (WasJustShown || !StayOpened)
                return;

            if (DimOpacity)
                return;

            InputEngine.MouseArgs ee = new InputEngine.MouseArgs()
            {
                button = e.button,
                curState = new MouseState(
                    e.curState.X - (int)position.X,
                    e.curState.Y - (int)position.Y,
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton,
                    e.curState.MiddleButton,
                    e.curState.RightButton,
                    e.curState.XButton1,
                    e.curState.XButton2)
            };

            base.onButtonUp(ee);
            if (IsIn(e.curState.X, e.curState.Y))
                e.Handled = true;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (WasJustShown || !StayOpened)
                return;

            int gx = e.curState.X, gy = e.curState.Y;
            Utilities.Tools.ScreenToGameCoords(ref gx, ref gy);

            if (DimOpacity)
            {
                if (!AssociatedComponent.isIn(gx, gy))
                    Close();
                return;
            }

            InputEngine.MouseArgs ee = new InputEngine.MouseArgs()
            {
                button = e.button,
                curState = new MouseState(
                    e.curState.X - (int)position.X,
                    e.curState.Y - (int)position.Y,
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton,
                    e.curState.MiddleButton,
                    e.curState.RightButton,
                    e.curState.XButton1,
                    e.curState.XButton2)
            };

            base.onButtonClick(ee);

            if (IsIn(e.curState.X, e.curState.Y) && StayOpened)
                e.Handled = true;

            if (!IsIn(e.curState.X, e.curState.Y) && !AssociatedComponent.isIn(gx, gy))
                Close();
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            InputEngine.MouseMoveArgs ee = new InputEngine.MouseMoveArgs()
            {
                dx = e.dx,
                dy = e.dy,
                curState = new MouseState(
                    e.curState.X - (int)position.X,
                    e.curState.Y - (int)position.Y,
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton,
                    e.curState.MiddleButton,
                    e.curState.RightButton,
                    e.curState.XButton1,
                    e.curState.XButton2)
            };

            base.onMouseMove(ee);

            int px = e.curState.X, py = e.curState.Y;
            Utilities.Tools.ScreenToGameCoords(ref px, ref py);

            if (!StayOpened && !AssociatedComponent.isIn(px, py))
                Close();
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            InputEngine.MouseWheelMoveArgs ee = new InputEngine.MouseWheelMoveArgs()
            {
                delta = e.delta,
                curState = new MouseState(
                    e.curState.X - (int)position.X,
                    e.curState.Y - (int)position.Y,
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton,
                    e.curState.MiddleButton,
                    e.curState.RightButton,
                    e.curState.XButton1,
                    e.curState.XButton2)
            };

            base.onMouseWheelMove(ee);
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (e.key == Keys.Enter.GetHashCode())
            {
                Save();
            }
            base.onKeyPressed(e);
            e.Handled = true;
        }

        public override bool IsIn(int x, int y)
        {
            return x >= position.X && x < position.X + size.X
                && y >= position.Y && y < position.Y + size.Y;
        }
        #endregion
    }
}
