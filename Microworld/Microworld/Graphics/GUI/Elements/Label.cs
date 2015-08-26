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
    public class Label : Control
    {
        public static SpriteFont DefaultFont;
        private SpriteFont _font;
        public SpriteFont font
        {
            get { return _font == null ? Label.DefaultFont : _font; }
            set { _font = value; }
        }

        private String _text = "";
        public String text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (font == null)
                    WasMeasured = false;
                else
                {
                    size = font.MeasureString(text);
                    WasMeasured = true;
                }
            }
        }
        public Color foreground = Color.Black;
        public Graphics.Renderer.TextAlignment TextAlignment = Graphics.Renderer.TextAlignment.Left;

        private bool WasMeasured = false;

        public Label(int x, int y, String txt)
        {
            position = new Vector2(x, y);
            text = txt;
            if (font == null)
                WasMeasured = false;
            else
            {
                size = font.MeasureString(text);
                WasMeasured = true;
            }
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            if (!WasMeasured && font != null)
            {
                size = font.MeasureString(text);
                WasMeasured = true;
            }
        }

        public void UpdateSizeToTextSize()
        {
            if (font == null)
                WasMeasured = false;
            else
            {
                size = font.MeasureString(text);
                WasMeasured = true;
            }
        }

        public override void Draw(Renderer renderer)
        {
            Main.renderer.DrawString(font, text,
                new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), foreground, TextAlignment);
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
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

    }
}
