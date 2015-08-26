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
    public class Button : Control
    {
        public static Texture2D textureGlobal, textureSelected, border;

        public Texture2D texture;
        protected SpriteFont font;
        public virtual SpriteFont Font
        {
            get { return font; }
            set
            {
                font = value;
                stringSize = GUIEngine.font.MeasureString(text);
            }
        }

        protected String text = "";
        public virtual String Text
        {
            get { return text; }
            set { text = value; }
        }
        public Renderer.TextAlignment textAlignment = Renderer.TextAlignment.Center;
        public Color foreground = Color.White, background = Color.Black, pressedColor = new Color(200, 200, 200),
            disabledColor = new Color(128, 128, 128), mouseOverColor = new Color(90, 90, 90);
        private bool _isPressed = false;
        public virtual bool isPressed
        {
            get { return _isPressed; }
            set { _isPressed = value; }
        }
        public bool isEnabled = true;
        private bool _isMouseOver = false;
        public virtual bool isMouseOver
        {
            get { return _isMouseOver; }
            set { _isMouseOver = value; }
        }

        #region Events
        public delegate void ClickedEventHandler(object sender, InputEngine.MouseArgs e);
        public event ClickedEventHandler onClicked;
        #endregion

        protected Vector2 stringSize;
        protected String textOld = "";

        public Button(int x, int y, int w, int h, String txt)
        {
            position = new Vector2(x, y);
            size = new Vector2(w, h);
            text = txt;
            textOld = txt;
            stringSize = GUIEngine.font.MeasureString(text);
        }

        public override void Initialize()
        {
            if (font == null) font = GUIEngine.font;
        }

        public override void Update()
        {
        }

        protected void drawBorder(ref Texture2D t, Color c, Vector2 pos, Vector2 s, int corners, bool lines)
        {
            if (lines)//Only draws contour
            {
                Main.renderer.Draw(t,
                    new Rectangle((int)pos.X, (int)pos.Y, (int)corners, (int)s.Y - corners),
                    new Rectangle(0, 0, (int)corners, (int)s.Y - corners),
                    c);
                Main.renderer.Draw(t,
                    new Rectangle((int)(pos.X + corners), (int)pos.Y, (int)s.X - corners, (int)corners),
                    new Rectangle(corners, 0, (int)s.X - corners, (int)corners),
                    c);
                Main.renderer.Draw(t,
                    new Rectangle((int)pos.X, (int)(pos.Y + s.Y - corners), (int)s.X - corners, (int)corners),
                    new Rectangle(0, 32 - corners, (int)s.X - corners, (int)corners),
                    c);
                Main.renderer.Draw(t,
                    new Rectangle((int)(pos.X + s.X - corners), (int)(pos.Y), corners, (int)s.Y - corners),
                    new Rectangle(32 - corners, 0, corners, (int)s.Y - corners),
                    c);
                Main.renderer.Draw(t,
                    new Rectangle((int)(pos.X + s.X - corners), (int)(pos.Y + s.Y - corners), corners, corners),
                    new Rectangle(32 - corners, 32 - corners, corners, corners),
                    c);
            }
            else//Fully draws background
            {
                Main.renderer.Draw(t,
                    new Rectangle((int)pos.X, (int)pos.Y, (int)corners, (int)corners),
                    new Rectangle(0, 0, (int)corners, (int)corners),
                    c);
                Main.renderer.Draw(t,
                    new Rectangle((int)(pos.X + s.X - corners), (int)pos.Y, (int)corners, (int)corners),
                    new Rectangle(32 - corners, 0, (int)corners, (int)corners),
                    c);
                Main.renderer.Draw(t,
                    new Rectangle((int)pos.X, (int)(pos.Y + s.Y - corners), (int)corners, (int)corners),
                    new Rectangle(0, 32 - corners, (int)corners, (int)corners),
                    c);
                Main.renderer.Draw(t,
                    new Rectangle((int)(pos.X + s.X - corners), (int)(pos.Y + s.Y - corners), corners, corners),
                    new Rectangle(32 - corners, 32 - corners, corners, corners),
                    c);
            }
        }

        protected void drawCorners(ref Texture2D t, Color c, Vector2 pos, Vector2 s, int corners)
        {
            Main.renderer.Draw(t,
                new Rectangle((int)pos.X, (int)pos.Y, corners, corners),
                new Rectangle(0, 0, (int)corners, (int)corners),
                c);
            Main.renderer.Draw(t,
                new Rectangle((int)(pos.X + size.X - corners), (int)pos.Y, corners, corners),
                new Rectangle(32 - corners, 0, (int)corners, (int)corners),
                c);
            Main.renderer.Draw(t,
                new Rectangle((int)pos.X, (int)(pos.Y + size.Y - corners), corners, corners),
                new Rectangle(0, 32 - corners, (int)corners, (int)corners),
                c);
            Main.renderer.Draw(t,
                new Rectangle((int)(pos.X + size.X - corners), (int)(pos.Y + size.Y - corners), corners, corners),
                new Rectangle(32 - corners, 32 - corners, (int)corners, (int)corners),
                c);
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;
            if (texture == null) texture = textureGlobal;
            if (font == null) font = GUIEngine.font;
            Main.renderer.Draw(texture,
                new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                new Rectangle(4, 4, (int)1, (int)1),
                isEnabled ? (isPressed ? pressedColor : (isMouseOver ? mouseOverColor : background)) : disabledColor);

            drawBorder(ref texture, 
                isEnabled ? (isPressed ? pressedColor : (isMouseOver ? mouseOverColor : background)) : disabledColor,
                position, size, 4, false);
            drawCorners(ref border, Color.White, position, size, 4);
            //drawBorder(ref textureSelected, Color.White,
            //    position, size, 8, true);
            /*
            MicroWorld.spriteBatch.Draw(texture,
                new Rectangle((int)position.X, (int)position.Y, (int)4, (int)4),
                new Rectangle(0, 0, (int)4, (int)4),
                isEnabled ? isPressed ? pressedColor : background : disabledColor);
            MicroWorld.spriteBatch.Draw(texture,
                new Rectangle((int)(position.X + size.X - 4), (int)position.Y, (int)4, (int)4),
                new Rectangle(252, 0, (int)4, (int)4),
                isEnabled ? isPressed ? pressedColor : background : disabledColor);
            MicroWorld.spriteBatch.Draw(texture,
                new Rectangle((int)position.X, (int)(position.Y + size.Y - 4), (int)4, (int)4),
                new Rectangle(0, 252, (int)4, (int)4),
                isEnabled ? isPressed ? pressedColor : background : disabledColor);
            MicroWorld.spriteBatch.Draw(texture,
                new Rectangle((int)(position.X + size.X - 4), (int)(position.Y + size.Y - 4), 4, 4),
                new Rectangle(252, 252, 4, 4),
                isEnabled ? isPressed ? pressedColor : background : disabledColor);//*/

            if (textOld != text)
            {
                textOld = text;
                stringSize = GUIEngine.font.MeasureString(text);
            }

            Main.renderer.DrawString(font, text, new Rectangle((int)position.X,
                (int)position.Y, (int)size.X, (int)size.Y), foreground, textAlignment);

        }

        public void onButtonDown(Vector2 e)
        {
            if (isEnabled && IsIn((int)e.X, (int)e.Y))
            {
                isPressed = true;
                InputEngine.eventHandled = true;
            }
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            onButtonDown(new Vector2(e.curState.X, e.curState.Y));
        }

        public void onButtonUp(Vector2 e)
        {
            if (isEnabled && isPressed)
            {
                isPressed = false;
                if (onClicked != null && IsIn((int)e.X, (int)e.Y))
                {
                    Statistics.ButtonsClicked++;
                    onClicked(this, null);
                    InputEngine.eventHandled = true;
                }
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            onButtonUp(new Vector2(e.curState.X, e.curState.Y));
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y)) InputEngine.eventHandled = true;
        }

        public void onMouseMove(Vector2 e)
        {
            if (isEnabled &&
                (InputEngine.curMouse.LeftButton == ButtonState.Pressed ||
                InputEngine.curMouse.RightButton == ButtonState.Pressed) &&
                IsIn((int)e.X, (int)e.Y))
            {
                isPressed = true;
            }
            else
            {
                isPressed = false;
            }
            if (!isPressed && IsIn((int)e.X, (int)e.Y))
            {
                isMouseOver = true;
            }
            else
            {
                isMouseOver = false;
            }
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            onMouseMove(new Vector2(e.curState.X, e.curState.Y));
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
