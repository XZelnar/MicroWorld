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
    public class CoreProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public CheckBox record;
        public TextBox ticks, accuracy;

        public override void Initialize()
        {

            WasInitialized = true;

            size = new Vector2(190, 155);

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

            ticks = new TextBox(135, 80, (int)size.X - 140, 20, "0");
            ticks.MaxLength = 5;
            ticks.Multiline = false;
            ticks.Mask = TextBox.InputMask.Numbers;
            ticks.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(ticks);

            l = new Label(5, 105, "Accuracy (%): ");
            l.foreground = Color.White;
            controls.Add(l);

            accuracy = new TextBox(135, 105, (int)size.X - 140, 20, "0");
            accuracy.MaxLength = 3;
            accuracy.Multiline = false;
            accuracy.Mask = TextBox.InputMask.Numbers;
            accuracy.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(accuracy);

            record = new CheckBox(5, 130, (int)size.X - 10, 20, "Record: ", false);
            record.foreground = Color.White;
            controls.Add(record);

            base.Initialize();
        }

        public override void Load()
        {
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.curState == "GAMELevels")
            {
                removable.Enabled = false;
                record.Enabled = false;
                ticks.Editable = false;
                accuracy.Editable = false;
                (AssociatedComponent.Logics as Logics.CoreLogics).record = false;
            }
            else
            {
                removable.Enabled = true;
                record.Enabled = true;
                ticks.Editable = true;
                accuracy.Editable = true;
            }
            String s = (AssociatedComponent.Logics as Logics.CoreLogics).target.Length.ToString();
            if (s.Length > ticks.MaxLength) s = s.Substring(0, ticks.MaxLength);
            ticks.Text = s;
            record.Checked = (AssociatedComponent.Logics as Logics.CoreLogics).record;
            accuracy.Text = Math.Round((AssociatedComponent.Logics as Logics.CoreLogics).RequiredAccuracy * 100).ToString();
        }

        public override void Save()
        {
            if (AssociatedComponent != null)
            {
                (AssociatedComponent.Logics as Logics.CoreLogics).record = record.Checked;
                int t;
                if (Int32.TryParse(ticks.Text, out t) && t != (AssociatedComponent.Logics as Logics.CoreLogics).target.Length)
                {
                    t = t < 1 ? 1 : t > 10000 ? 10000 : t;
                    (AssociatedComponent.Logics as Logics.CoreLogics).target = new bool[t];
                    (AssociatedComponent.Logics as Logics.CoreLogics).result = new bool[t];
                }
                double d;
                if (Double.TryParse(accuracy.Text, out d))
                {
                    d = d < 0 ? 0 : d > 100 ? 100 : d;
                    (AssociatedComponent.Logics as Logics.CoreLogics).RequiredAccuracy = (float)d / 100f;
                }
            }
        }
    }
}
