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
    public class CheckBox : Control
    {
        static Texture2D yes, no;

        Label ltext = new Label(0, 0, "");
        Button bcheck = new Button(20, 0, 20, 20, "");

        private bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                bcheck.isEnabled = value;
            }
        }

        public Color foreground
        {
            get { return ltext.foreground; }
            set { ltext.foreground = value; }
        }

        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;
                ltext.position = position;
                bcheck.position.X = position.X + size.X - 20;
                bcheck.position.Y = position.Y;
            }
        }
        public override Vector2 Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
                ltext.size = size - new Vector2(20, 0);
                bcheck.position.X = position.X + size.X - 20;
            }
        }

        private bool isChecked = false;
        public bool Checked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                bcheck.Text = value ? "x" : "";
                if (onCheckedChanged != null)
                    onCheckedChanged.Invoke(this, isChecked);
            }
        }

        public delegate void CheckBoxCheckedHandler(object sender, bool IsChecked);
        public event CheckBoxCheckedHandler onCheckedChanged;

        public CheckBox()
        {
        }

        public CheckBox(int x, int y, int w, int h, String txt, bool c)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(w, h);
            ltext.text = txt;
            Checked = c;
        }

        public override void Initialize()
        {
            base.Initialize();
            ltext.Initialize();
            bcheck.Initialize();
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            if (enabled && e.button == 0 && IsIn(e.curState.X,e.curState.Y))
            {
                Checked = !Checked;
            }
        }

        public override void Draw(Renderer renderer)
        {
            if (yes == null)
            {
                yes = ResourceManager.Load<Texture2D>("GUI/Elements/Check");
                no = ResourceManager.Load<Texture2D>("GUI/Elements/Cross");
            }

            Main.renderer.Draw(bcheck.Text == "x" ? yes : no, new Rectangle(
                (int)bcheck.position.X, (int)bcheck.position.Y, (int)bcheck.size.X, (int)bcheck.size.Y), Color.White);
            base.Draw(renderer);
            if (!enabled)
                Main.renderer.Draw(GraphicsEngine.pixel,
                    new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)bcheck.size.Y), Color.Gray * 0.7f);
            ltext.Draw(renderer);
            //bcheck.Draw(renderer);
        }

        public override bool IsIn(int x, int y)
        {
            return x >= position.X && x < position.X + size.X
                && y >= position.Y && y < position.Y + size.Y;
        }
    }
}
