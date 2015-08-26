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
    public class HotkeyControl : Control
    {
        public override Vector2 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                b_Key.position += value - Position;
                b_Text.position += value - Position;
                base.Position = value;
            }
        }
        public override Vector2 Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                if (value.X < buttonOffset + 1)
                    value.X = buttonOffset + 1;
                base.Size = value;
                SetButtonSize((int)value.X - buttonOffset);
            }
        }

        public IO.InputSequence Key = new IO.InputSequence();

        private MenuButton b_Key;
        private Label b_Text;

        private IO.InputSequence key;
        private static Texture2D texture;
        private int buttonOffset = 0;

        public HotkeyControl(int x, int y, int buttonOffset, String txt, IO.InputSequence key)
        {
            this.buttonOffset = buttonOffset;
            this.key = key;

            b_Key = new MenuButton(x + buttonOffset, y + 2, 240, 16, key.ToString());
            b_Key.Text = key.ToString(GUIEngine.font, (int)b_Key.size.X, false);
            b_Key.Font = GUIEngine.font;
            b_Key.TextOffset = new Vector2(0, 4);
            b_Key.DrawBottomLine = false;
            b_Key.onClicked += new Button.ClickedEventHandler(b_Key_onClicked);

            b_Text = new Label(x + 2, y - 1, txt);
            b_Text.foreground = Color.White;

            position = new Vector2(x, y);
            Size = new Vector2(buttonOffset + 240, 20);

            Key = key;
        }

        public void SetButtonSize(int w)
        {
            b_Key.Size = new Vector2(w - 7, b_Key.Size.Y);
            b_Key.Text = key.ToString(GUIEngine.font, (int)b_Key.size.X, false);
        }

        public override void Initialize()
        {
            b_Key.Initialize();
            b_Text.Initialize();

            if (texture == null)
                texture = ResourceManager.Load<Texture2D>("GUI/Elements/ComboBoxBg");

            base.Initialize();
        }

        void b_Key_onClicked(object sender, InputEngine.MouseArgs e)
        {
            var a = Scene.SequenceSelection.ShowNew();
            a.onSequenceSelected += new Scene.SequenceSelection.SequenceSelectedEventHandler(a_onSequenceSelected);
        }

        void a_onSequenceSelected(object sender, IO.InputSequence key)
        {
            Key.CopyFrom(key);
            b_Key.Text = Key.ToString(b_Key.Font, (int)b_Key.size.X, false);
        }

        public override void Update()
        {
            b_Key.Update();

            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            RenderHelper.SmartDrawRectangle(texture, 5, (int)position.X + buttonOffset, (int)position.Y, (int)size.X - buttonOffset - 5, (int)size.Y,
                Color.White * 0.6f, renderer);
            RenderHelper.SmartDrawRectangle(texture, 5, (int)position.X, (int)position.Y, buttonOffset, (int)size.Y,
                Color.White * 0.6f, renderer);

            b_Key.Draw(renderer);
            b_Text.Draw(renderer);
            base.Draw(renderer);
        }

        public String KeyToString(Keys k)
        {
            int khc = k.GetHashCode();

            #region A-Z
            //a-z,A-Z
            if (khc >= Keys.A.GetHashCode() && khc <= Keys.Z.GetHashCode())
                return ((char)k).ToString();
            #endregion

            #region D Keys
            //`
            if (k == Keys.OemTilde)
                return "`";
            //D0-D9
            if (khc >= Keys.D0.GetHashCode() && khc <= Keys.D9.GetHashCode())
                    return ((char)k).ToString();
            //-
            if (k == Keys.OemMinus)
                return "-";
            //+
            if (k == Keys.OemPlus)
                return "=";
            #endregion

            #region NumKeys
            //N0-N9 w/ numlock
            if (InputEngine.NumLock && khc >= Keys.NumPad0.GetHashCode() && khc <= Keys.NumPad9.GetHashCode())
                return "N" + k.ToString().Substring(k.ToString().Length - 1);
            //N/
            if (k == Keys.Divide)
                return "/";
            //N*
            if (k == Keys.Multiply)
                return "*";
            //N-
            if (k == Keys.Subtract)
                return "-";
            //N+
            if (k == Keys.Add)
                return "+";
            //N.
            if (k == Keys.Decimal)
                return ".";
            #endregion

            #region OEM
            // \
            if (k == Keys.OemPipe)
                return "\\";
            //[
            if (k == Keys.OemOpenBrackets)
                return "[";
            //]
            if (k == Keys.OemCloseBrackets)
                return "]";
            //;
            if (k == Keys.OemSemicolon)
                return ";";
            //'
            if (k == Keys.OemQuotes)
                return "'";
            //,
            if (k == Keys.OemComma)
                return ",";
            //.
            if (k == Keys.OemPeriod)
                return ".";
            //?
            if (k == Keys.OemQuestion)
                return "/";
            #endregion

            return k.ToString();
        }

        #region IO
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            b_Key.onButtonDown(e);
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            b_Key.onButtonUp(e);
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            b_Key.onButtonClick(e);
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            b_Key.onMouseMove(e);
        }
        #endregion
    }
}
