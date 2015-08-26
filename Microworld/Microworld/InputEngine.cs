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
using System.Runtime.InteropServices;

namespace MicroWorld
{
    public unsafe static class InputEngine
    {
        private const bool IOFromLog = false;
        private const bool IOFastForward = true;


        public const int MAX_CLICK_DISTANCE = 5;



        private static int lastKeyPressed = 0;
        private static int ticksSincePressed = 0;

        private static int clickStartX = 0, clickStartY = 0;
        private static int clickButton = 0;
        private static bool canClick = false;

        private static int midKeyX, midKeyY;
        private static int timeSinceMidKey;


        internal static System.IO.BinaryWriter logger;
        private static uint ticks = 0;






        public static bool eventHandled = false;
        public static bool blockClick = false;
        //==================================================Mouse
        public static MouseState curMouse, lastMouse;

        private static Vector2? leftMouseButtonDownPos = null, rightMouseButtonDownPos = null;
        public static Vector2? LeftMouseButtonDownPos
        {
            get { return InputEngine.leftMouseButtonDownPos; }
        }
        public static Vector2? RightMouseButtonDownPos
        {
            get { return InputEngine.rightMouseButtonDownPos; }
        }

        public class MouseArgs
        {
            public MouseState curState;
            public int button;//left == 0, right == 1, middle = 2

            public bool Handled
            {
                get { return InputEngine.eventHandled; }
                set { InputEngine.eventHandled = (value ? true : InputEngine.eventHandled); }
            }

            public void SetHandled()
            {
                InputEngine.eventHandled = true;
            }
        }
        public delegate void MouseEventHandler(MouseArgs e);
        public static event MouseEventHandler onButtonDown;
        public static event MouseEventHandler onButtonUp;
        public static event MouseEventHandler onButtonClick;
        public static event MouseEventHandler onMouseWheelClick;
        public class MouseMoveArgs
        {
            public MouseState curState;
            public int dx, dy;

            public bool Handled
            {
                get { return InputEngine.eventHandled; }
                set { InputEngine.eventHandled = (value ? true : InputEngine.eventHandled); }
            }

            public void SetHandled()
            {
                InputEngine.eventHandled = true;
            }
        }
        public delegate void MouseMoveEventHandler(MouseMoveArgs e);
        public static event MouseMoveEventHandler onMouseMove;
        public class MouseWheelMoveArgs
        {
            public MouseState curState;
            public int delta;

            public bool Handled
            {
                get { return InputEngine.eventHandled; }
                set { InputEngine.eventHandled = (value ? true : InputEngine.eventHandled); }
            }

            public void SetHandled()
            {
                InputEngine.eventHandled = true;
            }
        }
        public delegate void MouseWheelMoveEventHandler(MouseWheelMoveArgs e);
        public static event MouseWheelMoveEventHandler onMouseWheelMove;


        //==================================================KB
        public static KeyboardState curKeyboard, lastKeyboard;
        public class KeyboardArgs
        {
            public KeyboardState curState;
            public int key;

            public bool Handled
            {
                get { return InputEngine.eventHandled; }
                set { InputEngine.eventHandled = (value ? true : InputEngine.eventHandled); }
            }

            public void SetHandled()
            {
                InputEngine.eventHandled = true;
            }
        }
        public delegate void KeyboardEventHandler(KeyboardArgs e);
        public static event KeyboardEventHandler onKeyDown;
        public static event KeyboardEventHandler onKeyUp;
        public static event KeyboardEventHandler onKeyPressed;


        public static bool Shift = false;
        public static bool Control = false;
        public static bool Alt = false;
        public static bool CapsLock = false;
        public static bool NumLock = false;

        public static Vector2 GetCursorGameCoords()
        {
            return new Vector2((int)(curMouse.X / Settings.GameScale - Settings.GameOffset.X),
                    (int)(curMouse.Y / Settings.GameScale - Settings.GameOffset.Y));
        }

        internal static bool WasActive = false;
        public static void Update()
        {
            ticks++;
            if (!Main.isActive)
            {
                WasActive = false;
                return;
            }
            lastMouse = curMouse;
            lastKeyboard = curKeyboard;
            if (IOFromLog && !Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                d = 0;
                ReadFromLog();
            }
            //==================================================Mouse
            if (!IOFromLog || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                curMouse = Mouse.GetState();
            if (lastMouse == null) return;
            //move
            if (curMouse.X != lastMouse.X || curMouse.Y != lastMouse.Y)
                if (onMouseMove != null)
                {
                    onMouseMove(new MouseMoveArgs()
                    {
                        curState = curMouse,
                        dx = curMouse.X - lastMouse.X,
                        dy = curMouse.Y - lastMouse.Y
                    });
                    eventHandled = false;
                }
            //left
            if (curMouse.LeftButton == ButtonState.Pressed && lastMouse.LeftButton == ButtonState.Released)
            {
                leftMouseButtonDownPos = new Vector2(curMouse.X, curMouse.Y);
                if (onButtonDown != null && WasActive)
                {
                    if (curMouse.X >= 0 && curMouse.Y >= 0 && curMouse.X <= Main.WindowWidth && curMouse.Y < Main.WindowHeight)
                    onButtonDown(new MouseArgs() { curState = curMouse, button = 0 });
                    eventHandled = false;
                }
                canClick = true;
                clickButton = 0;
                clickStartX = curMouse.X;
                clickStartY = curMouse.Y;
            }
            if (curMouse.LeftButton == ButtonState.Released && lastMouse.LeftButton == ButtonState.Pressed)
            {
                if (onButtonUp != null && WasActive)
                {
                    onButtonUp(new MouseArgs() { curState = curMouse, button = 0 });
                }
                if (!blockClick && canClick && clickButton == 0 &&
                    Math.Abs(clickStartX - curMouse.X) < MAX_CLICK_DISTANCE &&
                    Math.Abs(clickStartY - curMouse.Y) < MAX_CLICK_DISTANCE)
                {
                    if (onButtonClick != null && WasActive)
                    {
                        eventHandled = false;
                        if (curMouse.X >= 0 && curMouse.Y >= 0 && curMouse.X <= Main.WindowWidth && curMouse.Y < Main.WindowHeight)
                        onButtonClick(new MouseArgs() { curState = curMouse, button = 0 });
                    }
                }
                leftMouseButtonDownPos = null;
                eventHandled = false;
            }
            blockClick = false;
            //right
            if (curMouse.RightButton == ButtonState.Pressed && lastMouse.RightButton == ButtonState.Released)
            {
                rightMouseButtonDownPos = new Vector2(curMouse.X, curMouse.Y);
                if (onButtonDown != null && WasActive)
                {
                    if (curMouse.X >= 0 && curMouse.Y >= 0 && curMouse.X <= Main.WindowWidth && curMouse.Y < Main.WindowHeight)
                    onButtonDown(new MouseArgs() { curState = curMouse, button = 1 });
                    eventHandled = false;
                }
                canClick = true;
                clickButton = 1;
                clickStartX = curMouse.X;
                clickStartY = curMouse.Y;
            }
            if (curMouse.RightButton == ButtonState.Released && lastMouse.RightButton == ButtonState.Pressed)
            {
                if (onButtonUp != null && WasActive)
                {
                    onButtonUp(new MouseArgs() { curState = curMouse, button = 1 });
                }
                if (!blockClick && canClick && clickButton == 1 && !eventHandled &&
                    Math.Abs(clickStartX - curMouse.X) < MAX_CLICK_DISTANCE && 
                    Math.Abs(clickStartY - curMouse.Y) < MAX_CLICK_DISTANCE)
                {
                    if (onButtonClick != null && WasActive)
                    {
                        if (curMouse.X >= 0 && curMouse.Y >= 0 && curMouse.X <= Main.WindowWidth && curMouse.Y < Main.WindowHeight)
                        onButtonClick(new MouseArgs() { curState = curMouse, button = 1 });
                    }
                }
                leftMouseButtonDownPos = null;
                eventHandled = false;
            }
            blockClick = false;
            //wheel
            if (curMouse.ScrollWheelValue != lastMouse.ScrollWheelValue)
                if (onMouseWheelMove != null && WasActive)
                {
                    onMouseWheelMove(new MouseWheelMoveArgs()
                    {
                        curState = curMouse,
                        delta = curMouse.ScrollWheelValue - lastMouse.ScrollWheelValue
                    });
                    eventHandled = false;
                }
            if (curMouse.MiddleButton != lastMouse.MiddleButton)
            {
                if (curMouse.MiddleButton == ButtonState.Pressed)
                {
                    timeSinceMidKey = 0;
                    midKeyX = curMouse.X;
                    midKeyY = curMouse.Y;
                }
                else
                {
                    if (timeSinceMidKey < 20 && 
                        Math.Abs(curMouse.X - midKeyX) < MAX_CLICK_DISTANCE &&
                        Math.Abs(curMouse.Y - midKeyY) < MAX_CLICK_DISTANCE)
                    {
                        if (onMouseWheelClick != null && WasActive)
                        {
                            onMouseWheelClick(new MouseArgs() { curState = curMouse, button = 2 });
                        }
                        eventHandled = false;
                    }
                }
            }
            //==================================================KB
            if (!IOFromLog || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                curKeyboard = Keyboard.GetState();

            Shift = curKeyboard.IsKeyDown(Keys.LeftShift) || curKeyboard.IsKeyDown(Keys.RightShift);
            Control = curKeyboard.IsKeyDown(Keys.LeftControl) || curKeyboard.IsKeyDown(Keys.RightControl);
            Alt = curKeyboard.IsKeyDown(Keys.LeftAlt) || curKeyboard.IsKeyDown(Keys.RightAlt);
            CapsLock = System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock);
            NumLock = System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.NumLock);

            if (lastKeyboard == null) return;
            for (int i = 0; i < 256; i++)
            {
                if (curKeyboard.IsKeyDown((Keys)i) != lastKeyboard.IsKeyDown((Keys)i))
                {
                    if (curKeyboard.IsKeyDown((Keys)i))
                    {
                        if (onKeyDown != null && Main.isActive)
                        {
                            onKeyDown(new KeyboardArgs() { curState = curKeyboard, key = i });
                            eventHandled = false;
                        }
                        if (onKeyPressed != null && Main.isActive)
                        {
                            if (lastKeyPressed != 0) ticksSincePressed = 0;
                            lastKeyPressed = i;
                            onKeyPressed(new KeyboardArgs() { curState = curKeyboard, key = i });
                            eventHandled = false;
                        }
                    }
                    else
                    {
                        if (lastKeyPressed == i)
                        {
                            lastKeyPressed = 0; 
                            ticksSincePressed = 0;
                        }
                        if (onKeyUp != null && Main.isActive)
                        {
                            onKeyUp(new KeyboardArgs() { curState = curKeyboard, key = i });
                            eventHandled = false;
                        }
                    }
                }
            }
            //support for pressed key
            if (lastKeyPressed != 0)
            {
                ticksSincePressed++;
                if (ticksSincePressed > 20 && ticksSincePressed % 2 == 0)
                {
                    if (onKeyDown != null)
                    {
                        onKeyPressed(new KeyboardArgs() { curState = curKeyboard, key = lastKeyPressed });
                        eventHandled = false;
                    }
                }
            }
            else
            {
                ticksSincePressed = 0;
            }
            //support for pressed middle button
            if (curMouse.MiddleButton == ButtonState.Pressed)
            {
                timeSinceMidKey++;
            }

            if (Settings.LogInput)
                Log();

            WasActive = true;
        }

        public static bool IsKeyDown(Keys k)
        {
            return curKeyboard.IsKeyDown(k) && !lastKeyboard.IsKeyDown(k);
        }

        public static bool IsKeyPressed(Keys k)
        {
            return curKeyboard.IsKeyDown(k);
        }

        public static bool IsKeyUp(Keys k)
        {
            return !curKeyboard.IsKeyDown(k) && lastKeyboard.IsKeyDown(k);
        }

        public static void Log()
        {
            String s = ticks.ToString() + ";";
            //mouse
            s += curMouse.X.ToString() + ";" +
                 curMouse.Y.ToString() + ";" +
                 curMouse.ScrollWheelValue.ToString() + ";" +
                 (curMouse.LeftButton == ButtonState.Pressed ? "1" : "0") + ";" +
                 (curMouse.MiddleButton == ButtonState.Pressed ? "1" : "0") + ";" +
                 (curMouse.RightButton == ButtonState.Pressed ? "1" : "0") + ";";
            //kb
            for (int i = 0; i < 256; i++)
            {
                if (curKeyboard.IsKeyDown((Keys)i))
                {
                    s += ((char)i).ToString();
                }
            }
            

            if (logger == null)
                logger = new System.IO.BinaryWriter(new System.IO.FileStream("debug/io.log", System.IO.FileMode.Append));
            logger.Write(s + "\r\n");
        }

        private static System.IO.StreamReader sr = null;
        static int d = 0;
        internal static int LogLength = -1;
        public static void ReadFromLog()
        {
            d++;
            String t = "-1";
            try
            {
                if (sr == null)
                {
                    if (System.IO.File.Exists("io.log"))
                        sr = new System.IO.StreamReader("io.log");
                    if (sr != null && LogLength == -1)
                    {
                        LogLength = sr.ReadToEnd().Split('\n').Length;
                        sr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                    }
                }
                else
                {
                    Main.window.Title = "[" + d + "/" + LogLength.ToString() + "]";
                    if (sr.Peek() == -1) return;
                    String s = sr.ReadLine();
                    var a = s.Split(';');
                    t = a[0];
                    curMouse = new MouseState(Convert.ToInt32(a[1]), Convert.ToInt32(a[2]), Convert.ToInt32(a[3]),
                        (a[4] == "1" ? ButtonState.Pressed : ButtonState.Released),
                        (a[5] == "1" ? ButtonState.Pressed : ButtonState.Released),
                        (a[6] == "1" ? ButtonState.Pressed : ButtonState.Released),
                        ButtonState.Released, ButtonState.Released);
                    if (IOFastForward && Keyboard.GetState().IsKeyDown(Keys.F) && d < 50)
                    {
                        if (curMouse.LeftButton == lastMouse.LeftButton &&
                            curMouse.RightButton == lastMouse.RightButton &&
                            curMouse.MiddleButton == lastMouse.MiddleButton &&
                            curMouse.ScrollWheelValue == lastMouse.ScrollWheelValue &&
                            curMouse.X == lastMouse.X &&
                            curMouse.Y == lastMouse.Y)
                        {
                            ReadFromLog();
                            return;
                        }
                    }
                    Keys[] k = new Keys[a[7].Length];
                    for (int i = 0; i < a[7].Length; i++)
                    {
                        k[i] = (Keys)a[7][i];
                    }
                    curKeyboard = new KeyboardState(k);
                }
            }
            catch (Exception e)
            {
                IO.Log.Write(IO.Log.State.ERROR, e.Message + "\r\n\r\n" + t + "\r\n");
            }
        }

        public static bool WereBothMouseButtonsClicked()
        {
            return curMouse.RightButton == ButtonState.Released && lastMouse.RightButton == ButtonState.Pressed &&
                curMouse.LeftButton == ButtonState.Released && lastMouse.LeftButton == ButtonState.Pressed;
        }



        public static String GetClipboardText()
        {
            if (System.Windows.Forms.Clipboard.ContainsText())
            {
                return System.Windows.Forms.Clipboard.GetText();
            }
            return "";
        }

        public static void SetClipboardText(String replacementText)
        {
            if (replacementText == "") replacementText = " ";
            System.Windows.Forms.Clipboard.SetText(replacementText);
        }


    }
}
