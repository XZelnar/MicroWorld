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
using MicroWorld.Graphics.GUI.Scene;
using MicroWorld.Graphics.GUI;
using MicroWorld.Graphics;

namespace MicroWorld.Components.GUI
{
    public class PulseCoreProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox ticks;

        public override void Initialize()
        {

            WasInitialized = true;

            size = new Vector2(170, 105);

            title = new Label(0, 5, AssociatedComponent.Graphics.GetCSToolTip());
            title.TextAlignment = Renderer.TextAlignment.Center;
            title.foreground = Color.White;
            title.font = TitleFont;
            controls.Add(title);

            size.X = Math.Max(title.font.MeasureString(title.text).X + 80, size.X);

            title.size.X = size.X - 20;

            removable = new CheckBox(5, 55, (int)size.X - 10, 20, "Removable: ", false);
            removable.foreground = Color.White;
            removable.onCheckedChanged += new CheckBox.CheckBoxCheckedHandler(removable_onCheckedChanged);
            controls.Add(removable);

            Label l = new Label(5, 80, "Ticks: ");
            l.foreground = Color.White;
            controls.Add(l);

            ticks = new TextBox(145, 80, (int)size.X - 150, 20, "0");
            ticks.MaxLength = 5;
            ticks.Multiline = false;
            ticks.Mask = TextBox.InputMask.Numbers;
            ticks.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(ticks);

            base.Initialize();
        }

        public override void Load()
        {
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.curState == "GAMELevels")
            {
                removable.Enabled = false;
                ticks.Editable = false;
            }
            else
            {
                removable.Enabled = true;
                ticks.Editable = true;
            }
            String s = (AssociatedComponent.Logics as Logics.PulseCoreLogics).RequiredActivity.ToString();
            if (s.Length > ticks.MaxLength) s = s.Substring(0, ticks.MaxLength);
            ticks.Text = s;
        }

        public override void Save()
        {
            if (AssociatedComponent != null)
            {
                int t;
                if (Int32.TryParse(ticks.Text, out t) && t != (AssociatedComponent.Logics as Logics.PulseCoreLogics).RequiredActivity)
                {
                    t = t < 1 ? 1 : t > 10000 ? 10000 : t;
                    (AssociatedComponent.Logics as Logics.PulseCoreLogics).RequiredActivity = t;
                }
            }
        }
    }
}
