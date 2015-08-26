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
    class LevelSelectorItem : MenuButton
    {
        public override Vector2 Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                GenLines();
            }
        }

        VertexPositionColorTexture[] lines = new VertexPositionColorTexture[12];
        SpriteFont bfont;

        public override void Initialize()
        {
            GlobalEvents.onResolutionChanged += new GlobalEvents.ResolutionEventHandler(GlobalEvents_onResolutionChanged);
            Font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_16");
            bfont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_40");
            Size = new Vector2(370 * Main.WindowWidth / 1920, 211 * Main.WindowHeight / 1080);
            Text = "Level 1";

            base.Initialize();
        }

        public override void Dispose()
        {
            GlobalEvents.onResolutionChanged -= new GlobalEvents.ResolutionEventHandler(GlobalEvents_onResolutionChanged);
            base.Dispose();
        }

        public void GenLines()
        {
            lines[0] = new VertexPositionColorTexture(new Vector3(3, 3, 0), Color.White, new Vector2());
            lines[1] = new VertexPositionColorTexture(new Vector3(size.X, 3, 0), Color.White, new Vector2());
            lines[2] = lines[1];
            lines[3] = new VertexPositionColorTexture(new Vector3(size.X, 185 * Main.WindowHeight / 1080, 0),
                Color.White, new Vector2());
            lines[4] = lines[3];
            lines[5] = new VertexPositionColorTexture(new Vector3(3, 185 * Main.WindowHeight / 1080, 0),
                Color.White, new Vector2());
            lines[6] = lines[5];
            lines[7] = lines[0];
            lines[8] = lines[5];
            lines[9] = new VertexPositionColorTexture(new Vector3(3, size.Y, 0), Color.White, new Vector2());
            lines[10] = new VertexPositionColorTexture(new Vector3(0, 92 * Main.WindowHeight / 1080, 0),
                Color.White, new Vector2());
            lines[11] = new VertexPositionColorTexture(new Vector3(3, 92 * Main.WindowHeight / 1080, 0),
                Color.White, new Vector2());
        }

        void GlobalEvents_onResolutionChanged(int w, int h, int oldw, int oldh)
        {
            Size = new Vector2(370 * w / 1920, 211 * h / 1080);
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;
            renderer.Draw(fboout, position, isEnabled ? Color.White : Color.Gray);
        }

        public override void DrawToFBO(Renderer renderer)
        {
            renderer.BeginUnscaled();
            renderer.Draw(GraphicsEngine.pixel,
                new Rectangle(0, 0, (int)size.X, (int)size.Y),
                isEnabled ? (isPressed ? pressedColor : (isMouseOver ? mouseOverColor : background)) : disabledColor);

            if (textOld != Text)
            {
                textOld = Text;
                stringSize = GUIEngine.font.MeasureString(Text);
            }

            renderer.DrawString(Font, Text, new Rectangle(MouseOverOffset + 10,
                (int)size.Y - 22, (int)size.X, (int)size.Y), foreground, textAlignment);

            //inside text
            String t = Text.Substring(Text.Length - 1);
            var a = bfont.MeasureString(t);
            renderer.DrawStringLeft(bfont, t, new Vector2(3 + (size.X - 3 - a.X) / 2, 3 + (lines[3].Position.Y - 3 - a.Y) / 2), Color.White);

            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, 0, 0), Color.White);
            renderer.DrawLinesList(lines);
            renderer.End();
        }
    }
}
