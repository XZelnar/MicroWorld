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

namespace MicroWorld.IO
{
    public class InputSequence
    {
        #region Variables
        private bool WasLastPressed = false;

        public List<Keys> keys = new List<Keys>();
        public short WheelDelta = 0;
        public bool LeftMKey = false;
        public bool RightMKey = false;
        public bool MiddleMKey = false;
        public bool X1MKey = false;
        public bool X2MKey = false;
        public bool Control = false;
        public bool Shift = false;
        public bool Alt = false;
        public bool Continious = false;
        #endregion

        #region Constructors
        public InputSequence() { }

        public InputSequence(bool continious, params Keys[] nkeys)
        {
            Continious = continious;
            for (int i = 0; i < nkeys.Length; i++)
                keys.Add(nkeys[i]);
        }

        public InputSequence(bool continious, bool ctrl, bool shift, bool alt, params Keys[] nkeys)
        {
            Continious = continious;
            Control = ctrl;
            this.Shift = shift;
            this.Alt = alt;
            for (int i = 0; i < nkeys.Length; i++)
                keys.Add(nkeys[i]);
        }

        public InputSequence(bool continious, bool ctrl, bool shift, bool alt, Boolean leftmouse, bool rightmouse, bool middlemouse, 
            short wheel, params Keys[] nkeys)
        {
            Continious = continious;
            Control = ctrl;
            this.Shift = shift;
            this.Alt = alt;
            LeftMKey = leftmouse;
            RightMKey = rightmouse;
            MiddleMKey = middlemouse;
            WheelDelta = wheel;
            for (int i = 0; i < nkeys.Length; i++)
                keys.Add(nkeys[i]);
        }
        #endregion

        #region Private
        private void Clean()
        {
            for (int i = 0; i < keys.Count - 1; i++)
                for (int j = i + 1; j < keys.Count; j++)
                    if (keys[i] == keys[j])
                    {
                        keys.RemoveAt(j);
                        j--;
                    }
        }

        private String KeyToString(Keys k)
        {
            if (k == Keys.OemBackslash)
                return "\\";
            if (k == Keys.OemCloseBrackets)
                return "]";
            if (k == Keys.OemComma)
                return ",";
            if (k == Keys.OemMinus)
                return "-";
            if (k == Keys.OemOpenBrackets)
                return "[";
            if (k == Keys.OemPeriod)
                return ".";
            if (k == Keys.OemPipe)
                return "\\";
            if (k == Keys.OemPlus)
                return "+";
            if (k == Keys.OemQuestion)
                return "/";
            if (k == Keys.OemQuotes)
                return "'";
            if (k == Keys.OemSemicolon)
                return ";";
            if (k == Keys.OemTilde)
                return "`";
            return k.ToString();
        }
        #endregion

        #region Public
        public void AddRecord()
        {
            if (InputEngine.curMouse.LeftButton == ButtonState.Pressed && InputEngine.lastMouse.LeftButton == ButtonState.Released)
                LeftMKey = true;
            if (InputEngine.curMouse.RightButton == ButtonState.Pressed && InputEngine.lastMouse.RightButton == ButtonState.Released)
                RightMKey = true;
            if (InputEngine.curMouse.MiddleButton == ButtonState.Pressed && InputEngine.lastMouse.MiddleButton == ButtonState.Released)
                MiddleMKey = true;
            if (InputEngine.curMouse.XButton1 == ButtonState.Pressed && InputEngine.lastMouse.XButton1 == ButtonState.Released)
                X1MKey = true;
            if (InputEngine.curMouse.XButton2 == ButtonState.Pressed && InputEngine.lastMouse.XButton2 == ButtonState.Released)
                X2MKey = true;

            if (!Control)
                Control = InputEngine.Control;
            if (!Shift)
                Shift = InputEngine.Shift;
            if (!Alt)
                Alt = InputEngine.Alt;

            if (InputEngine.curMouse.ScrollWheelValue != InputEngine.lastMouse.ScrollWheelValue)
            {
                WheelDelta = (short)Math.Sign(InputEngine.curMouse.ScrollWheelValue - InputEngine.lastMouse.ScrollWheelValue);
            }

            for (int i = 0; i < 256; i++)
            {
                if (InputEngine.curKeyboard.IsKeyDown((Keys)i) && 
                    i != Keys.LeftControl.GetHashCode() && i != Keys.RightControl.GetHashCode() &&
                    i != Keys.LeftShift.GetHashCode() && i != Keys.RightShift.GetHashCode() &&
                    i != Keys.LeftAlt.GetHashCode() && i != Keys.RightAlt.GetHashCode())
                    keys.Add((Keys)i);
            }
            Clean();
        }

        public bool IsMatched()
        {
            bool a = WasLastPressed;
            WasLastPressed = false;

            if (keys.Count == 0 && !Control && !Shift && !Alt && !LeftMKey && !RightMKey && !MiddleMKey && !X1MKey && !X2MKey && WheelDelta == 0)
                return false;

            for (int i = 0; i < keys.Count; i++)
            {
                if (!InputEngine.curKeyboard.IsKeyDown(keys[i]))
                    return false;
            }

            if (Control && !InputEngine.Control)
                return false;
            if (Shift && !InputEngine.Shift)
                return false;
            if (Alt && !InputEngine.Alt)
                return false;

            if (Math.Sign(InputEngine.curMouse.ScrollWheelValue - InputEngine.lastMouse.ScrollWheelValue) != WheelDelta)
                return false;

            if ((InputEngine.curMouse.LeftButton == ButtonState.Pressed) != LeftMKey)
                return false;
            if ((InputEngine.curMouse.RightButton == ButtonState.Pressed) != RightMKey)
                return false;
            if ((InputEngine.curMouse.MiddleButton == ButtonState.Pressed) != MiddleMKey)
                return false;
            if ((InputEngine.curMouse.XButton1 == ButtonState.Pressed) != X1MKey)
                return false;
            if ((InputEngine.curMouse.XButton2 == ButtonState.Pressed) != X2MKey)
                return false;

            WasLastPressed = a;
            if (!Continious)
                if (WasLastPressed)
                    return false;
            WasLastPressed = true;
            return true;
        }

        public void Clear()
        {
            keys.Clear();
            WheelDelta = 0;
            LeftMKey = false;
            RightMKey = false;
            MiddleMKey = false;
            X1MKey = false;
            X2MKey = false;
            Control = false;
            Shift = false;
            Alt = false;
        }

        public String Save()
        {
            String r = "";
            for (int i = 0; i < keys.Count; i++)
                r += keys[i].GetHashCode().ToString() + ";";
            r += (Continious ? "1" : "0") + ";";
            r += (Control ? "1" : "0") + ";";
            r += (Shift ? "1" : "0") + ";";
            r += (Alt ? "1" : "0") + ";";
            r += (LeftMKey ? "1" : "0") + ";";
            r += (RightMKey ? "1" : "0") + ";";
            r += (MiddleMKey ? "1" : "0") + ";";
            r += (X1MKey ? "1" : "0") + ";";
            r += (X2MKey ? "1" : "0") + ";";
            r += WheelDelta.ToString();
            return r;
        }

        public void Load(String s)
        {
            if (s == null || s == "") return;
            var a = s.Split(';');
            if (s.Length < 10) return;
            Clear();

            for (int i = 0; i < a.Length - 10; i++)
                keys.Add((Keys)Convert.ToInt32(a[i]));
            Continious = a[a.Length - 10] == "1";
            Control = a[a.Length - 9] == "1";
            Shift = a[a.Length - 8] == "1";
            Alt = a[a.Length - 7] == "1";
            LeftMKey = a[a.Length - 6] == "1";
            RightMKey = a[a.Length - 5] == "1";
            MiddleMKey = a[a.Length - 4] == "1";
            X1MKey = a[a.Length - 3] == "1";
            X2MKey = a[a.Length - 2] == "1";
            WheelDelta = Convert.ToInt16(a[a.Length - 1]);
        }

        public override String ToString()
        {
            String r = "";

            if (Control)
                r += "<Control> & ";
            if (Shift)
                r += "<Shift> & ";
            if (Alt)
                r += "<Alt> & ";

            for (int i = 0; i < keys.Count; i++)
                r += "<" + KeyToString(keys[i]) + "> & ";

            if (LeftMKey)
                r += "<Left mouse key> & ";
            if (RightMKey)
                r += "<Right mouse key> & ";
            if (MiddleMKey)
                r += "<Middle mouse key> & ";
            if (X1MKey)
                r += "<X1 mouse key> & ";
            if (X2MKey)
                r += "<X2 mouse key> & ";

            if (WheelDelta > 0)
                r += "<Mouse wheel up>";
            if (WheelDelta < 0)
                r += "<Mouse wheel down>";

            if (r.EndsWith(" & "))
                r = r.Substring(0, r.Length - 3);
            if (r.Length == 0)
                r = "???";

            return r;
        }

        public String ToString(SpriteFont font, int maxWidth, bool multiline)
        {
            String r = ToString();
            if (multiline)
            {
                var a = r.Split('&');
                r = "";

                for (int i = 0; i < a.Length; i++)
                {
                    if (font.MeasureString(r + a[i] + "&").X > maxWidth)
                        r += "\r\n";
                    r += a[i] + "&";
                }
                if (r.EndsWith("&"))
                    r = r.Substring(0, r.Length - 1);
                return r;
            }
            else
            {
                if (font.MeasureString(r).X <= maxWidth)
                    return r;
                String t = "";
                for (int i = 0; i < r.Length; i++)
                {
                    if (font.MeasureString(t + r[i] + "...").X > maxWidth)
                        return t + "...";
                    t += r[i];
                }
            }
            return "";
        }

        public void CopyFrom(InputSequence s)
        {
            keys = s.keys;
            Control = s.Control;
            Shift = s.Shift;
            Alt = s.Alt;

            LeftMKey = s.LeftMKey;
            RightMKey = s.RightMKey;
            MiddleMKey = s.MiddleMKey;
            X1MKey = s.X1MKey;
            X2MKey = s.X2MKey;
            WheelDelta = s.WheelDelta;
        }
        #endregion
    }
}
