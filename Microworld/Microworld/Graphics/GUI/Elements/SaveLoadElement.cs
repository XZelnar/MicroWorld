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
    public class SaveLoadElement : Control
    {
        private static Texture2D file;
        private static SpriteFont font, fontsmall;

        private RenderTarget2D fbo;
        private float lerpState = 0;
        private bool isMouseOver = false;
        private Vector2 offset = new Vector2();
        private DateTime saveDateTime = new DateTime();
        private String SaveDateString = "01.02.2014 18:57";

        internal int index = 0;
        internal int lastClick = 0;

        public String SaveName = "";
        public DateTime SaveDateTime
        {
            get { return saveDateTime; }
            set
            {
                saveDateTime = value;
                SaveDateString = saveDateTime.ToShortDateString() + " " + saveDateTime.ToShortTimeString();
            }
        }
        public bool StaySelected = false;
        public Vector2 Offset
        {
            get { return offset; }
            set
            {
                offset = value;
                isMouseOver = IsIn(InputEngine.curMouse.X + (int)offset.X, InputEngine.curMouse.Y + (int)offset.Y);
            }
        }
        public override Vector2 Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                if (fbo != null)
                {
                    fbo.Dispose();
                    fbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, (int)Size.X, (int)Size.Y);
                }
            }
        }


        public SaveLoadElement(int x, int y)
        {
            position = new Vector2(x, y);
            size = new Vector2(1084 * Main.WindowWidth / 1920, 81);
        }

        public override void Initialize()
        {
            if (file == null)
            {
                file = ResourceManager.Load<Texture2D>("GUI/Menus/SaveLoad/File");
                font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_19");
                fontsmall = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_16");
            }
            fbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, (int)Size.X, (int)Size.Y);
        }

        public override void Update()
        {
            DrawToFBO();
            if (StaySelected)
            {
                lerpState = 1;
            }
            else
            {
                if (isMouseOver)
                {
                    if (!IsIn(InputEngine.curMouse.X - (int)offset.X, InputEngine.curMouse.Y - (int)offset.Y))
                        isMouseOver = false;
                    lerpState += 0.07f;
                    if (lerpState > 1)
                        lerpState = 1;
                }
                else
                {
                    lerpState -= 0.07f;
                    if (lerpState < 0)
                        lerpState = 0;
                }
            }
        }

        public void DrawToFBO()
        {
            if (file == null)
            {
                return;
            }

            Renderer renderer = GraphicsEngine.Renderer;

            renderer.EnableFBO(fbo);
            renderer.GraphicsDevice.PresentationParameters.MultiSampleCount = 1;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                size.X, size.Y, 0, 0, 1);
            Effects.Effects.colorLerp.Parameters["MatrixTransform"].SetValue(projection);
            Effects.Effects.colorLerp.Parameters["state"].SetValue(lerpState);
            Effects.Effects.colorLerp.Parameters["halfpixel"].SetValue(new float[]{0.5f / size.X, 0.5f / size.Y});
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, 
                RasterizerState.CullNone, Effects.Effects.colorLerp);

            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, (int)size.X, (int)size.Y), Shortcuts.BG_COLOR);
            renderer.Draw(file, new Rectangle(16, 13, 41, 55), Color.White);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(73, 5, 2, 70), Color.White);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(73, 39, (int)size.X - 74 - 2, 2), Color.White);
            renderer.DrawStringLeft(font, SaveName, new Vector2(83, 7), Color.White);
            renderer.DrawString(fontsmall, SaveDateString, new Rectangle(83, 41, 200, 30), Color.White, Renderer.TextAlignment.Left);
            //renderer.DrawString(font, SaveDateString, new Rectangle(0, 41, (int)size.X, 30), Color.White * 0.8f, Renderer.TextAlignment.Right);
            int x = (int)(size.X - 76) / 2 + 76;
            int w = (int)(size.X - x - 2);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(x, 2, w, (int)(size.Y - 4)), Color.White * lerpState);
            renderer.Draw(GraphicsEngine.dottedPatternBig,
                new Rectangle(x + 2, 4, w - 4, (int)(size.Y - 8)),
                new Rectangle(x + 2, 4, w - 4, (int)(size.Y - 8)), 
                Color.White * lerpState);

            renderer.End();
            renderer.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
            renderer.DisableFBO();
        }

        public override void Draw(Renderer renderer)
        {
            if (fbo != null)
                renderer.Draw(fbo, position + Offset, Color.White);
        }

        #region IO
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            StaySelected = IsIn(e.curState.X - (int)offset.X, e.curState.Y - (int)offset.Y);
            if (StaySelected)
                lastClick = (int)Main.Ticks;
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            isMouseOver = IsIn(e.curState.X - (int)offset.X, e.curState.Y - (int)offset.Y);
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            isMouseOver = IsIn(e.curState.X - (int)offset.X, e.curState.Y - (int)offset.Y);
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
        }
        #endregion
    }
}
