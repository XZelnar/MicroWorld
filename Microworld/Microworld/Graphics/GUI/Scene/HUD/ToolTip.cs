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
    //Not where the arrow is pointing, but where it is pointing from
    public enum ArrowLineDirection
    {
        RightUp = 0,
        RightDown = 1,
        LeftUp = 2,
        LeftDown = 3,
        None = 4
    }
    class ToolTip : HUDScene
    {

        private int CurLength = 0;
        private String text = "";
        private SpriteFont font;
        private Texture2D bgtexture, arrowRU, arrowRD, arrowLU, arrowLD;
        private ArrowLineDirection textDirection = ArrowLineDirection.RightUp;
        private Vector2 size = new Vector2();
        private Vector2 charSize;

        public String Text
        {
            get { return text; }
            set
            {
                text = value;
                CurLength = 0;
                var a = font.MeasureString(text);
                Size = new Vector2(arrowRU.Width + a.X, Math.Max(arrowRU.Height, a.Y));
                if (ForcedDirection != ArrowLineDirection.None)
                {
                    switch (ForcedDirection)
                    {
                        case ArrowLineDirection.RightUp:
                            break;
                        case ArrowLineDirection.RightDown:
                            position.Y += arrowRU.Height;
                            break;
                        case ArrowLineDirection.LeftUp:
                            position.X -= size.X;
                            break;
                        case ArrowLineDirection.LeftDown:
                            position.X -= size.X;
                            position.Y += arrowRU.Height;
                            break;
                        case ArrowLineDirection.None:
                            break;
                        default:
                            break;
                    }
                    textDirection = ForcedDirection;
                }
                else
                {
                    if (position.X + size.X > Main.WindowWidth)
                    {
                        if (position.Y > 0)
                        {
                            textDirection = ArrowLineDirection.LeftUp;
                            position.X -= size.X;
                        }
                        else
                        {
                            textDirection = ArrowLineDirection.LeftDown;
                            position.X -= size.X;
                            position.Y += arrowRU.Height;
                        }
                    }
                    else
                    {
                        if (position.Y > 0)
                        {
                            textDirection = ArrowLineDirection.RightUp;
                        }
                        else
                        {
                            textDirection = ArrowLineDirection.RightDown;
                            position.Y += arrowRU.Height;
                        }
                    }
                }
            }
        }
        public float opacity = 1f;
        public ArrowLineDirection ForcedDirection = ArrowLineDirection.None;

        private Vector2 position = new Vector2();
        /// <summary>
        /// Tip of the arrow position
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                if (arrowRU != null)
                {
                    position.Y -= arrowRU.Height;
                }
            }
        }
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        public new void Initialize()
        {
            ShouldBeScaled = false;
            CanOffset = false;
            Layer = 560;

            font = GUIEngine.font;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            bgtexture = ResourceManager.Load<Texture2D>("GUI/ToolTip/bg");
            arrowRU = ResourceManager.Load<Texture2D>("GUI/ToolTip/ToolTipArrowRU");
            arrowRD = ResourceManager.Load<Texture2D>("GUI/ToolTip/ToolTipArrowRD");
            arrowLU = ResourceManager.Load<Texture2D>("GUI/ToolTip/ToolTipArrowLU");
            arrowLD = ResourceManager.Load<Texture2D>("GUI/ToolTip/ToolTipArrowLD");

            font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_9");
            charSize = font.MeasureString("w");
        }

        public override void onShow()
        {
            CurLength = 0;

            base.onShow();
        }

        public override void onClose()
        {
            ForcedDirection = ArrowLineDirection.None;
            isVisible = false;
            CurLength = 0;

            base.onClose();
        }

        public override void Update()
        {
            if (isVisible)
            {
                if (CurLength < Text.Length)
                    CurLength++;
                if (CurLength > Text.Length)
                    CurLength = Text.Length;
            }
            else
                CurLength = 0;

            base.Update();
        }

        public void SkipAnimation()
        {
            CurLength = text.Length;
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;

            renderer.End();
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None,
                RasterizerState.CullNone);
            var a = font.MeasureString(Text.Substring(0, CurLength));

            //arrow
            //bg
            //text
            switch (textDirection)
            {
                case ArrowLineDirection.RightUp:
                    renderer.Draw(arrowRU, position, Color.White * opacity);
                    renderer.Draw(GraphicsEngine.pixel,
                        new Rectangle((int)(position.X + arrowRU.Width + 2), (int)position.Y, (int)size.X - arrowRU.Width + 2, (int)a.Y),
                        Settings.BGCOLOR * 0.7f);
                    renderer.DrawString(font, Text.Substring(0, CurLength), new Vector2(position.X + arrowRU.Width + 2, position.Y),
                        Color.White * opacity, Renderer.TextAlignment.Left);
                    break;
                case ArrowLineDirection.RightDown:
                    renderer.Draw(arrowRD, position, Color.White * opacity);
                    renderer.Draw(GraphicsEngine.pixel,
                        new Rectangle((int)(position.X + arrowRD.Width + 2), (int)(position.Y + arrowRD.Height - charSize.Y), (int)a.X + 2, (int)a.Y),
                        Settings.BGCOLOR * 0.7f);
                    renderer.DrawString(font, Text.Substring(0, CurLength), 
                        new Vector2(position.X + arrowRD.Width + 2, position.Y + arrowRD.Height - charSize.Y),
                        Color.White * opacity, Renderer.TextAlignment.Left);
                    break;
                case ArrowLineDirection.LeftUp:
                    renderer.Draw(arrowLU, position + new Vector2(size.X - arrowLU.Width, 0), Color.White * opacity);
                    renderer.Draw(GraphicsEngine.pixel,
                        new Rectangle((int)(position.X + size.X - arrowLU.Width - a.X) - 2, (int)position.Y, (int)a.X + 4, (int)a.Y + 2),
                        Settings.BGCOLOR * 0.7f);
                    renderer.DrawString(font, Text.Substring(0, CurLength), 
                        new Rectangle((int)position.X, (int)position.Y, (int)size.X - arrowLU.Width, (int)size.Y),
                        Color.White * opacity, Renderer.TextAlignment.Right);
                    break;
                case ArrowLineDirection.LeftDown:
                    renderer.Draw(arrowLD, position + new Vector2(size.X - arrowLD.Width, 0), Color.White * opacity);
                    renderer.Draw(GraphicsEngine.pixel,
                        new Rectangle((int)(position.X + size.X - arrowLD.Width - a.X) - 2, (int)(position.Y + arrowLD.Height - charSize.Y), (int)a.X + 4, (int)a.Y + 2),
                        Settings.BGCOLOR * 0.7f);
                    renderer.DrawString(font, Text.Substring(0, CurLength),
                        new Rectangle((int)position.X, (int)(position.Y + arrowLD.Height - charSize.Y),
                            (int)size.X - arrowLD.Width, (int)size.Y),
                        Color.White * opacity, Renderer.TextAlignment.Right);
                    break;
                default:
                    break;
            }

            renderer.End();
            renderer.BeginUnscaled();
        }

        public override bool IsIn(int x, int y)
        {
            return false;
        }
    }
}
