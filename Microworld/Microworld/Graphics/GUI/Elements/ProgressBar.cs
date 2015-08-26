using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MicroWorld.Graphics.GUI.Elements
{
    public class ProgressBar : Control
    {
        private static Texture2D defaultBackground, defaultForeground;

        public Texture2D Background, Foreground;
        public Color BackgroungColor = Color.White, ForegroundColor = Color.White;
        public bool ClientEditable = true;
        public bool Enabled = true;

        private int value = 0, maxValue = 100, minValue = 0;
        private int foregroundTextureWidth = 0;
        private int backgroundBorder = 3;

        public int Value
        {
            get { return this.value; }
            set
            {
                value = value > maxValue ? maxValue : value < minValue ? minValue : value;
                if (this.value != value)
                {
                    int old = this.value;
                    this.value = value;
                    updateWidth();

                    if (onValueChanged != null)
                        onValueChanged.Invoke(this, value, old);
                }
            }
        }
        public int MinValue
        {
            get { return minValue; }
            set
            {
                minValue = value;
                Value = Value;
                updateWidth();
            }
        }
        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                Value = Value;
                updateWidth();
            }
        }
        public int BackgroundBorder
        {
            get { return backgroundBorder; }
            set
            {
                backgroundBorder = value;
                Value = Value;
            }
        }

        #region Events
        public delegate void ValueChangedEventHandler(object sender, int newValue, int oldValue);
        public event ValueChangedEventHandler onValueChanged;
        #endregion



        public ProgressBar()
        {
        }

        public ProgressBar(int x, int y, int w, int h, bool editable = true)
        {
            position = new Vector2(x, y);
            size = new Vector2(w, h);
            Value = Value;
            updateWidth();
            ClientEditable = editable;
        }

        private void updateWidth()
        {
            foregroundTextureWidth = (int)((float)(value - minValue) / (maxValue - minValue) * (size.X - backgroundBorder * 2));
        }

        public override void Initialize()
        {
            base.Initialize();

            if (defaultForeground == null)
            {
                defaultBackground = ResourceManager.Load<Texture2D>("GUI/bg");
                defaultForeground = ResourceManager.Load<Texture2D>("LimeYellowRedGradient");
            }

            if (Background == null)
                Background = defaultBackground;
            if (Foreground == null)
                Foreground = defaultForeground;
        }

        public override void Draw(Renderer renderer)
        {
            if (Background != null)
            {
                RenderHelper.SmartDrawRectangle(Background, BackgroundBorder, (int)position.X, (int)position.Y, (int)size.X, (int)size.Y, BackgroungColor, renderer);
                //renderer.Draw(Background, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), BackgroungColor);
            }
            if (Foreground != null)
                renderer.Draw(Foreground, new Rectangle((int)position.X + BackgroundBorder, (int)position.Y + BackgroundBorder, 
                    foregroundTextureWidth, (int)size.Y - BackgroundBorder * 2), 
                    new Rectangle(0, 0, (int)(foregroundTextureWidth * Foreground.Width / size.X), Foreground.Height), Enabled ? ForegroundColor : Color.Gray);
        }

        bool dndValue = false;
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            base.onButtonDown(e);

            if (ClientEditable && Enabled && e.button == 0 && IsIn(e.curState.X, e.curState.Y))
            {
                dndValue = true;
                Value = (int)((e.curState.X - position.X - backgroundBorder) / (size.X - backgroundBorder * 2) * (maxValue - minValue) + minValue);
                e.Handled = true;
            }
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);

            if (dndValue && Enabled)
            {
                Value = (int)((e.curState.X - position.X - backgroundBorder) / (size.X - backgroundBorder * 2) * (maxValue - minValue) + minValue);
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            base.onButtonUp(e);

            if (dndValue && e.button == 0)
                dndValue = false;
        }
    }
}
