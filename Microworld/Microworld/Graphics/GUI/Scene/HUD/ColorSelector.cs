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
    public class ColorSelector : HUDScene
    {
        public static Texture2D colormap = null;

        public Vector2 position = new Vector2(), size = new Vector2();

        #region Events
        public class ColorSelectedArgs
        {
            public Color color;
        }
        public delegate void ColorSelectedEventHandler(object sender, ColorSelectedArgs e);
        public event ColorSelectedEventHandler onColorSelected;
        #endregion

        private ColorSelector(Vector2 pos)
        {
            position = pos;
            size = new Vector2(258, 258);
        }

        public static ColorSelector Show(Vector2 pos)
        {
            var a = new ColorSelector(pos);
            a.Initialize();
            a.isVisible = true;
            GUIEngine.AddHUDScene(a);
            a.wasJustOpened = true;
            return a;
        }

        public override void Initialize()
        {
            Layer = 800;
            base.Initialize();
        }

        internal bool wasJustOpened = false;
        public override void Update()
        {
            wasJustOpened = false;
            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            if (colormap == null)
                colormap = ResourceManager.Load<Texture2D>("GUI/HUD/ColorSelect");

            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Color.Black * 0.5f);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), Color.Black);
            renderer.Draw(colormap, new Rectangle(1 + (int)position.X, 1 + (int)position.Y, (int)size.X - 2, (int)size.Y - 2), 
                Color.White);

            int x = (int)(InputEngine.curMouse.X - position.X);
            int y = (int)(InputEngine.curMouse.Y - position.Y);
            if (x > 0 && x < size.X - 2)
            {
                renderer.Draw(GraphicsEngine.pixel, new Rectangle(x + (int)position.X, (int)position.Y, 1, (int)size.Y), 
                    new Color(0, 50, 50) * 0.7f);
            }
            if (y > 0 && y < size.Y - 2)
            {
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, y + (int)position.Y, (int)size.X, 1), 
                    new Color(0, 50, 50) * 0.7f);
            }
            base.Draw(renderer);
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            base.onButtonDown(e);
            e.Handled = true;
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            base.onButtonUp(e);
            e.Handled = true;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (wasJustOpened) return;
            base.onButtonClick(e);
            e.Handled = true;
            if (IsIn(e.curState.X, e.curState.Y))
            {
                int x = (int)(e.curState.X - position.X);
                int y = (int)(e.curState.Y - position.Y);
                Color[] c = new Color[1];
                colormap.GetData<Color>(0, new Rectangle(x, y, 1, 1), c, 0, 1);
                if (onColorSelected != null)
                    onColorSelected.Invoke(this, new ColorSelectedArgs() { color = c[0] });
                Close();
            }
            else
            {
                Close();
            }
        }

        bool wasThisTooltip = false;
        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            e.Handled = true;
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            base.onMouseWheelMove(e);
            e.Handled = true;
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
            e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (e.key == Keys.Escape.GetHashCode())
            {
                Close();
                e.Handled = true;
                return;
            }
            base.onKeyPressed(e);
            e.Handled = true;
        }

        public override bool IsIn(int x, int y)
        {
            return x >= position.X && x < position.X + size.X
                && y >= position.Y && y < position.Y + size.Y;
        }

    }
}
