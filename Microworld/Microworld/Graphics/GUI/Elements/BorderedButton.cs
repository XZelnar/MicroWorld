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
    class BorderedButton : Button
    {
        public float opacity = 1f;

        public BorderedButton(int x, int y, int w, int h, String txt) : base(x, y, w, h, txt)
        {
            mouseOverColor = Color.White;
            background = Color.White * 0.9f;
            pressedColor = Color.White * 0.95f;
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;
            if (texture == null) texture = textureGlobal;
            if (font == null) font = GUIEngine.font;
            Main.renderer.Draw(texture,
                new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                new Rectangle(4, 4, (int)1, (int)1),
                (isEnabled ? (isPressed ? pressedColor : (isMouseOver ? mouseOverColor : background)) : disabledColor) * opacity);

            RenderHelper.SmartDrawRectangle(texture, 3, (int)position.X, (int)position.Y, (int)size.X, (int)size.Y,
                (isEnabled ? (isPressed ? pressedColor : (isMouseOver ? mouseOverColor : background)) : disabledColor) * opacity, renderer);

            if (textOld != text)
            {
                textOld = text;
                stringSize = GUIEngine.font.MeasureString(text);
            }

            Main.renderer.DrawString(font, text, new Rectangle((int)position.X,
                (int)position.Y, (int)size.X, (int)size.Y), foreground * opacity, textAlignment);
        }
    }
}
