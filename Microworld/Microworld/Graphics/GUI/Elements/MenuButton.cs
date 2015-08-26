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

namespace MicroWorld.Graphics.GUI.Elements
{
    public class MenuButton : Button
    {
        public MenuButton[] Children = new MenuButton[0];
        public Scene.MenuFrameScene frameScene = null;

        public RenderTarget2D fboin, fboout;
        public Texture2D LeftTexture = null;
        public Vector2 TextureOffset = new Vector2();
        public Color LeftTextureColor = Color.White;

        public bool WasInitiallyDrawn = false;
        public Vector2 TextOffset = new Vector2();

        public bool StaySelected = false;
        public bool ShouldStaySelectedAfterClick = false;
        public bool DrawBottomLine = true;
        public bool MoveOnMouseOver = true;
        public bool StretchLeftTextureToSize = false;

        public override Vector2 Size
        {
            get
            {
                return size;
            }
            set
            {
                if (size != value)
                {
                    size = value;
                    CreateFBO();
                }
            }
        }
        public override String Text
        {
            get { return base.Text; }
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    CreateFBO();
                }
            }
        }
        public override SpriteFont Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                if (size.X != 0)
                    CreateFBO();
            }
        }

        public MenuButton()
            : base(0, 0, 0, 0, "")
        {
            this.onClicked += new ClickedEventHandler(MenuButton_onClicked);
        }

        public MenuButton(String text)
            : base(0, 0, 0, 0, text)
        {
            this.onClicked += new ClickedEventHandler(MenuButton_onClicked);
        }

        public MenuButton(int x, int y, int w, int h, String text)
            : base(x, y, w, h, text)
        {
            this.onClicked += new ClickedEventHandler(MenuButton_onClicked);
        }

        void MenuButton_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (ShouldStaySelectedAfterClick)
                StaySelected = true;
        }

        public override void Initialize()
        {
            MouseOverOffset = 0;
            MouseOverTicks = 0;

            background = new Color(45, 57, 107);
            if (Font == null)
                Font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_22");
            mouseOverColor = background;
            pressedColor = background;
            textAlignment = Renderer.TextAlignment.Left;

            if (fboin == null)
                CreateFBO();

            base.Initialize();
        }

        public void CreateFBO()
        {
            if (fboin != null)
            {
                fboin.Dispose();
                fboout.Dispose();
            }

            fboin = Main.renderer.CreateFBO((int)size.X, (int)size.Y);
            fboout = Main.renderer.CreateFBO((int)size.X, (int)size.Y);

            WasInitiallyDrawn = false;
        }

        float MouseOverTicks = 0;
        protected int MouseOverOffset = 0;
        public override void Update()
        {
            float oldticks = MouseOverTicks;
            if (isMouseOver && isEnabled)
            {
                if (MouseOverTicks < size.X + size.Y - 1)
                {
                    if (MouseOverOffset < 20)
                        MouseOverOffset += 2;
                    MouseOverTicks += (size.X + size.Y) / 20;
                    var fbo = Main.renderer.CurFBO;
                    Main.renderer.EnableFBO(fboin);
                    DrawToFBO(Main.renderer);
                    Main.renderer.EnableFBO(fbo);
                }
            }
            else
            {
                if (MouseOverTicks > 0)
                {
                    if (MouseOverOffset > 0)
                        MouseOverOffset -= 2;
                    MouseOverTicks -= (size.X + size.Y) / 20;
                    if (MouseOverTicks < 0)
                        MouseOverTicks = 0;
                    var fbo = Main.renderer.CurFBO;
                    Main.renderer.EnableFBO(fboin);
                    Main.renderer.GraphicsDevice.Clear(Color.Transparent);
                    DrawToFBO(Main.renderer);
                    Main.renderer.EnableFBO(fbo);
                }
            }

            if (StaySelected)
            {
                MouseOverTicks = size.X + size.Y;
                MouseOverOffset = 20;
            }

            if (!WasInitiallyDrawn && Graphics.GraphicsEngine.pixel != null)
            {
                WasInitiallyDrawn = true;
                var fbo = Main.renderer.CurFBO;
                Main.renderer.EnableFBO(fboin);
                Main.renderer.GraphicsDevice.Clear(Color.Transparent);
                DrawToFBO(Main.renderer);
                Main.renderer.EnableFBO(fbo);
                Utilities.Graphical.InvertGradient(fboin, ref fboout, MouseOverTicks / (size.X + size.Y), 
                    Color.White, new Color(45, 57, 107));
            }
            else if (MouseOverTicks != oldticks)
            {
                Utilities.Graphical.InvertGradient(fboin, ref fboout, MouseOverTicks / (size.X + size.Y), 
                    Color.White, new Color(45, 57, 107));
            }

            base.Update();
        }

        public override void OnGraphicsDeviceReset()
        {
            WasInitiallyDrawn = false;

            base.OnGraphicsDeviceReset();
        }

        public void ResetMouseOverAnimation()
        {
            StaySelected = false;
            MouseOverTicks = 0;
            MouseOverOffset = 0;
            Utilities.Graphical.InvertGradient(fboin, ref fboout, MouseOverTicks / (size.X + size.Y),
                Color.White, new Color(45, 57, 107));
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;
            renderer.Draw(fboout, position, Color.White);
            if (DrawBottomLine)
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)(position.Y + size.Y - 1), (int)size.X, 1), 
                    isEnabled ? Color.White : Color.Gray);
        }

        public virtual void DrawToFBO(Renderer renderer)
        {
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default,
                RasterizerState.CullNone);
            //Main.renderer.Draw(GraphicsEngine.pixel,
            //    new Rectangle(0, 0, (int)size.X, (int)size.Y),
            //    isEnabled ? (isPressed ? pressedColor : (isMouseOver ? mouseOverColor : background)) : disabledColor);
            Main.renderer.Draw(GraphicsEngine.pixel,
                new Rectangle(0, 0, (int)size.X, (int)size.Y),
                background);
            if (!isEnabled)
                Main.renderer.Draw(GraphicsEngine.dottedPattern,
                    new Rectangle(0, 0, (int)size.X, (int)size.Y),
                    new Rectangle(0, 0, (int)size.X, (int)size.Y),
                    Color.White);

            if (textOld != Text)
            {
                textOld = Text;
                stringSize = GUIEngine.font.MeasureString(Text);
            }

            Main.renderer.DrawString(Font, Text, new Rectangle((MoveOnMouseOver ? MouseOverOffset : 0) + 10 + (int)TextOffset.X,
                -4 + (int)TextOffset.Y, (int)size.X, (int)size.Y), foreground, textAlignment);

            if (LeftTexture != null)
                if (StretchLeftTextureToSize)
                {
                    renderer.Draw(LeftTexture, new Rectangle(0, 0, (int)Size.X, (int)Size.Y), MouseOverTicks == 0 ? LeftTextureColor : Color.White);
                }
                else
                {
                    renderer.Draw(LeftTexture, TextureOffset, MouseOverTicks == 0 ? LeftTextureColor : Color.White);
                }
        }
    }
}
