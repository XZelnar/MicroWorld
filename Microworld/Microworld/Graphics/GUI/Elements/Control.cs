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
    public abstract class Control
    {
        internal Vector2 position = new Vector2();
        public virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        internal Vector2 size = new Vector2();
        public virtual Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }
        private bool _isFocused = false;
        public virtual bool isFocused
        {
            get { return _isFocused; }
            set { _isFocused = value; }
        }
        public bool isVisible = true;
        public object tag = null;

        public virtual void Initialize() { }

        public virtual void Update() { }

        public virtual void Draw(Renderer renderer) { }

        public virtual void PostDraw() { }

        public virtual void Dispose() { }

        public virtual void onButtonDown(InputEngine.MouseArgs e) { }

        public virtual void onButtonUp(InputEngine.MouseArgs e) { }

        public virtual void onButtonClick(InputEngine.MouseArgs e) { }

        public virtual void onMouseMove(InputEngine.MouseMoveArgs e) { }

        public virtual void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e) { }

        public virtual void onKeyDown(InputEngine.KeyboardArgs e) { }

        public virtual void onKeyUp(InputEngine.KeyboardArgs e) { }

        public virtual void onKeyPressed(InputEngine.KeyboardArgs e) { }

        public virtual void OnGraphicsDeviceReset() { }

        public virtual void FadeIn() { }

        public virtual void FadeOut() { }

        public virtual bool IsIn(int x, int y)
        {
            return x >= position.X && x < position.X + size.X
                && y >= position.Y && y < position.Y + size.Y;
        }

    }
}
