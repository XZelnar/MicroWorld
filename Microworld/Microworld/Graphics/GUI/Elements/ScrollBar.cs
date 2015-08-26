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
    public class ScrollBar : Control
    {
        public static Texture2D defaultTexture;
        public const int MOUSE_SENSITIVITY = 1;//the more the value, the less the mouse sensitivity is
        public const int BG_CLICK_MULTIPLIER = 10;

        public Texture2D texture;
        public Vector2 buttonSize = new Vector2(16, 16);
        public Vector2 barSize = new Vector2(12, 16);
        public Vector2 bgSize = new Vector2(1, 16);

        public bool IsVertical = false;

        public Color ColorButtonPressed = new Color(220, 220, 220);

        public int ButtonPressed = -1;
        private int initElementPressed = -1;//0 = buttons, 1 = bar, 2 = bg

        private int value = 0;
        public int Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                if (this.value > MaxValue) this.value = MaxValue;
                if (this.value < MinValue) this.value = MinValue;
                UpdateBarPosition();
                if (onValueChanged != null)
                {
                    onValueChanged(this, new ValueChangedArgs() { value = this.value });
                }
            }
        }
        public int MaxValue = 100, MinValue = 0;//TODO encapsulate
        public int Step = 1;

        private int ticksSinceButtonPress = -1;
        private Vector2 barPosition = new Vector2();

        #region Events
        public class ValueChangedArgs
        {
            public int value;
        }
        public delegate void ValueChangedEventHandler(object sender, ValueChangedArgs e);
        public event ValueChangedEventHandler onValueChanged;
        #endregion

        public ScrollBar(int x, int y, int w, int h)
        {
            position = new Vector2(x, y);
            size = new Vector2(w, h);
            UpdateBarPosition();
        }

        public override void Initialize()
        {
            UpdateBarPosition();
        }

        public override void Update()
        {
            if (InputEngine.curMouse.LeftButton == ButtonState.Pressed)
            {
                #region Horizontal
                if (!IsVertical)
                {
                    if (initElementPressed == 0)//left arrow
                    {
                        if (InputEngine.curMouse.X >= position.X && InputEngine.curMouse.Y >= position.Y &&
                        InputEngine.curMouse.X < position.X + buttonSize.X &&
                        InputEngine.curMouse.Y < position.Y + buttonSize.Y)
                        {
                            if (ticksSinceButtonPress == -1)
                            {
                                pressButton(-Step);
                                ButtonPressed = 0;
                                initElementPressed = 0;
                            }
                            else if (ticksSinceButtonPress > 20 && ticksSinceButtonPress % MOUSE_SENSITIVITY == 0)
                            {
                                pressButton(-Step);
                                ButtonPressed = 0;
                            }
                            ticksSinceButtonPress++;
                        }
                        else ButtonPressed = -1;
                    }
                    else if (initElementPressed == 1)//right arrow
                    {
                        if (InputEngine.curMouse.X >= position.X + size.X - buttonSize.X &&
                        InputEngine.curMouse.Y >= position.Y &&
                        InputEngine.curMouse.X < position.X + size.X &&
                        InputEngine.curMouse.Y < position.Y + buttonSize.Y)
                        {
                            if (ticksSinceButtonPress == -1)
                            {
                                pressButton(Step);
                                ButtonPressed = 1;
                                initElementPressed = 1;
                            }
                            else if (ticksSinceButtonPress > 20 && ticksSinceButtonPress % MOUSE_SENSITIVITY == 0)
                            {
                                pressButton(Step);
                                ButtonPressed = 1;
                            }
                            ticksSinceButtonPress++;
                        }
                        else ButtonPressed = -1;
                    }
                    else if (initElementPressed == 2)//bar
                    {
                        Value = (int)(((InputEngine.curMouse.X - position.X - buttonSize.X - barSize.X / 2) /
                            (float)(size.X - buttonSize.X * 2 - barSize.X)) * (MaxValue - MinValue));
                        CheckValueOutOfBounds();
                        UpdateBarPosition();
                    }
                    else if (initElementPressed == 3)//bg
                    {
                        if (ticksSinceButtonPress != -1 &&
                            (ticksSinceButtonPress < 20 || ticksSinceButtonPress % MOUSE_SENSITIVITY != 0))
                        {
                            ticksSinceButtonPress++;
                            goto postMouse;
                        }
                        ticksSinceButtonPress++;
                        float mx = InputEngine.curMouse.X - position.X - buttonSize.X - barSize.X / 2;
                        int tp = (int)((mx /
                            (float)(size.X - buttonSize.X * 2 - barSize.X)) * (MaxValue - MinValue));
                        if (tp != Value)
                        {
                            if (tp > Value)
                            {
                                if (tp - Value < Step * BG_CLICK_MULTIPLIER)
                                {
                                    Value = tp;
                                }
                                else
                                {
                                    pressButton(Step * BG_CLICK_MULTIPLIER);
                                }
                                CheckValueOutOfBounds();
                                UpdateBarPosition();
                            }
                            if (tp < Value)
                            {
                                if (Value - tp < Step * BG_CLICK_MULTIPLIER)
                                {
                                    Value = tp;
                                }
                                else
                                {
                                    pressButton(-Step * BG_CLICK_MULTIPLIER);
                                }
                                CheckValueOutOfBounds();
                                UpdateBarPosition();
                            }
                        }
                    }
                    else
                    {
                        ticksSinceButtonPress = -1;
                        ButtonPressed = -1;
                        initElementPressed = -1;
                    }
                    goto postMouse;
                }
                #endregion
                #region Vertical
                if (IsVertical)
                {
                    if (initElementPressed == 0)//left arrow
                    {
                        if (InputEngine.curMouse.X >= position.X && InputEngine.curMouse.Y >= position.Y &&
                        InputEngine.curMouse.X < position.X + buttonSize.X &&
                        InputEngine.curMouse.Y < position.Y + buttonSize.Y)
                        {
                            if (ticksSinceButtonPress == -1)
                            {
                                pressButton(-Step);
                                ButtonPressed = 0;
                                initElementPressed = 0;
                            }
                            else if (ticksSinceButtonPress > 20 && ticksSinceButtonPress % MOUSE_SENSITIVITY == 0)
                            {
                                pressButton(-Step);
                                ButtonPressed = 0;
                            }
                            ticksSinceButtonPress++;
                        }
                        else ButtonPressed = -1;
                    }
                    else if (initElementPressed == 1)//right arrow
                    {
                        if (InputEngine.curMouse.X >= position.X &&
                        InputEngine.curMouse.Y >= position.Y + size.Y - buttonSize.Y &&
                        InputEngine.curMouse.X < position.X + buttonSize.X &&
                        InputEngine.curMouse.Y < position.Y + size.Y)
                        {
                            if (ticksSinceButtonPress == -1)
                            {
                                pressButton(Step);
                                ButtonPressed = 1;
                                initElementPressed = 1;
                            }
                            else if (ticksSinceButtonPress > 20 && ticksSinceButtonPress % MOUSE_SENSITIVITY == 0)
                            {
                                pressButton(Step);
                                ButtonPressed = 1;
                            }
                            ticksSinceButtonPress++;
                        }
                        else ButtonPressed = -1;
                    }
                    else if (initElementPressed == 2)//bar
                    {
                        Value = (int)(((InputEngine.curMouse.Y - position.Y - buttonSize.Y - barSize.X / 2) /
                            (float)(size.Y - buttonSize.Y * 2 - barSize.X)) * (MaxValue - MinValue));
                        CheckValueOutOfBounds();
                        UpdateBarPosition();
                    }
                    else if (initElementPressed == 3)//bg
                    {
                        if (ticksSinceButtonPress != -1 &&
                            (ticksSinceButtonPress < 20 || ticksSinceButtonPress % MOUSE_SENSITIVITY != 0))
                        {
                            ticksSinceButtonPress++;
                            goto postMouse;
                        }
                        ticksSinceButtonPress++;
                        float mx = InputEngine.curMouse.Y - position.Y - buttonSize.Y - barSize.X / 2;
                        int tp = (int)((mx /
                            (float)(size.Y - buttonSize.Y * 2 - barSize.X)) * (MaxValue - MinValue));
                        if (tp != Value)
                        {
                            if (tp > Value)
                            {
                                if (tp - Value < Step * BG_CLICK_MULTIPLIER)
                                {
                                    Value = tp;
                                }
                                else
                                {
                                    pressButton(Step * BG_CLICK_MULTIPLIER);
                                }
                                CheckValueOutOfBounds();
                                UpdateBarPosition();
                            }
                            if (tp < Value)
                            {
                                if (Value - tp < Step * BG_CLICK_MULTIPLIER)
                                {
                                    Value = tp;
                                }
                                else
                                {
                                    pressButton(-Step * BG_CLICK_MULTIPLIER);
                                }
                                CheckValueOutOfBounds();
                                UpdateBarPosition();
                            }
                        }
                    }
                    else
                    {
                        ticksSinceButtonPress = -1;
                        ButtonPressed = -1;
                        initElementPressed = -1;
                    }
                    goto postMouse;
                }
                #endregion
            }
            else
            {
                ticksSinceButtonPress = -1;
                ButtonPressed = -1;
                initElementPressed = -1;
            }
        postMouse: ;
        }

        public void pressButton(int step)
        {
            Value += step;
            CheckValueOutOfBounds();
            UpdateBarPosition();
        }

        public void CheckValueOutOfBounds()
        {
            if (Value < MinValue) Value = MinValue;
            if (Value > MaxValue) Value = MaxValue;
        }

        public void UpdateBarPosition()
        {
            float c = (float)(Value - MinValue) / (float)(MaxValue - MinValue);
            if (!IsVertical)
            {
                barPosition = new Vector2((int)(position.X + buttonSize.X + c * (size.X - buttonSize.X * 2 - barSize.X)),
                        (int)position.Y);
            }
            else
            {
                barPosition = new Vector2((int)(position.X) + 2,
                        (int)(position.Y + buttonSize.Y-2 + c * (size.Y - buttonSize.Y * 2 - barSize.X+2)));
            }
        }

        public override void Draw(Renderer renderer)
        {
            if (texture == null) texture = defaultTexture;
            #region Horizontal
            if (!IsVertical)
            {
                //bg
                Main.renderer.Draw(texture,
                    new Rectangle((int)position.X + 16, (int)position.Y - 2, (int)size.X - 32, (int)size.Y),
                new Rectangle((int)buttonSize.X + 1, 0, 0, (int)bgSize.Y), Color.White);
                //bar
                Main.renderer.Draw(texture,
                    new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)barSize.X, (int)barSize.Y),
                new Rectangle((int)(texture.Bounds.Width - barSize.X), 0, (int)barSize.X, (int)barSize.Y), Color.White);
                //buttonleft
                Main.renderer.Draw(texture,
                    new Rectangle((int)position.X, (int)position.Y, (int)buttonSize.X, (int)buttonSize.Y),
                    new Rectangle(0, 0, (int)buttonSize.X, (int)buttonSize.Y), ButtonPressed == 0 ? ColorButtonPressed : Color.White);
                //buttonright
                Main.renderer.Draw(texture,
                    new Rectangle((int)(position.X + size.X - buttonSize.X), (int)position.Y, (int)buttonSize.X, (int)buttonSize.Y),
                new Rectangle(0, 0, (int)buttonSize.X, (int)buttonSize.Y), ButtonPressed == 1 ? ColorButtonPressed : Color.White,
                0, new Vector2(), SpriteEffects.FlipHorizontally, 0);
            }
            #endregion
            #region Vertical
            if (IsVertical)
            {
                //bg
                Main.renderer.Draw(texture,
                    new Rectangle((int)position.X + 1, (int)position.Y + 16, (int)size.X, (int)size.Y - 32),
                new Rectangle((int)buttonSize.X + 3, 8, (int)bgSize.Y, 0), Color.White);
                //bar
                Main.renderer.Draw(texture,
                    new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)barSize.X, (int)barSize.Y),
                new Rectangle((int)(texture.Bounds.Width - barSize.X), 0, (int)barSize.X, (int)barSize.Y), Color.White);
                //buttonleft
                Main.renderer.Draw(texture,
                    new Rectangle((int)position.X+8, (int)position.Y+8, (int)buttonSize.X, (int)buttonSize.Y),
                    new Rectangle(0, 0, (int)buttonSize.X, (int)buttonSize.Y), ButtonPressed == 0 ? ColorButtonPressed : Color.White, 
                    (float)Math.PI / 2, new Vector2(8,8),SpriteEffects.None,0);
                tas += 0.1f;
                //buttonright
                Main.renderer.Draw(texture,
                    new Rectangle((int)(position.X+8), (int)(position.Y + size.Y - buttonSize.Y+8), (int)buttonSize.X, (int)buttonSize.Y),
                new Rectangle(0, 0, (int)buttonSize.X, (int)buttonSize.Y), ButtonPressed == 1 ? ColorButtonPressed : Color.White,
                (float)Math.PI / 2, new Vector2(8,8), SpriteEffects.FlipHorizontally, 0);
            }
            #endregion
        }
        float tas = 0;
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                InputEngine.eventHandled = true;
                if (e.curState.LeftButton == ButtonState.Pressed)
                {
                    #region Horizontal
                    if (!IsVertical)
                    {
                        if (InputEngine.curMouse.X >= position.X && InputEngine.curMouse.Y >= position.Y &&
                            InputEngine.curMouse.X < position.X + buttonSize.X &&
                            InputEngine.curMouse.Y < position.Y + buttonSize.Y)//left button
                        {
                            initElementPressed = 0;
                        }
                        else if (InputEngine.curMouse.X >= position.X + size.X - buttonSize.X &&
                            InputEngine.curMouse.Y >= position.Y &&
                            InputEngine.curMouse.X < position.X + size.X &&
                            InputEngine.curMouse.Y < position.Y + buttonSize.Y)//right button
                        {
                            initElementPressed = 1;
                        }
                        else if (InputEngine.curMouse.X >= barPosition.X &&
                            InputEngine.curMouse.Y >= barPosition.Y &&
                            InputEngine.curMouse.X < barPosition.X + barSize.X &&
                            InputEngine.curMouse.Y < barPosition.Y + barSize.Y)//bar
                        {
                            initElementPressed = 2;
                        }
                        else
                        {
                            initElementPressed = 3;
                        }
                        return;
                    }
                    #endregion
                    #region Vertical
                    if(IsVertical)
                    {
                        if (InputEngine.curMouse.X >= position.X && InputEngine.curMouse.Y >= position.Y &&
                            InputEngine.curMouse.X < position.X + buttonSize.X &&
                            InputEngine.curMouse.Y < position.Y + buttonSize.Y)//left button
                        {
                            initElementPressed = 0;
                        }
                        else if (InputEngine.curMouse.X >= position.X &&
                            InputEngine.curMouse.Y >= position.Y + size.Y - buttonSize.Y &&
                            InputEngine.curMouse.X < position.X + buttonSize.X &&
                            InputEngine.curMouse.Y < position.Y + size.Y)//right button
                        {
                            initElementPressed = 1;
                        }
                        else if (InputEngine.curMouse.X >= barPosition.X &&
                            InputEngine.curMouse.Y >= barPosition.Y &&
                            InputEngine.curMouse.X < barPosition.X + barSize.X &&
                            InputEngine.curMouse.Y < barPosition.Y + barSize.Y)//bar
                        {
                            initElementPressed = 2;
                        }
                        else
                        {
                            initElementPressed = 3;
                        }
                    }
                    #endregion
                }
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y)) InputEngine.eventHandled = true;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y)) InputEngine.eventHandled = true;
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
