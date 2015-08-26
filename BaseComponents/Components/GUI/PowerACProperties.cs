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
    public class PowerACProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox voltage, period;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(150, 130);

            title = new Label(0, 5, AssociatedComponent.Graphics.GetCSToolTip());
            title.font = TitleFont;
            title.UpdateSizeToTextSize();
            title.TextAlignment = Renderer.TextAlignment.Center;
            title.foreground = Color.White;
            controls.Add(title);

            Size = new Vector2(Math.Max(title.font.MeasureString(title.text).X + 80, Size.X), Size.Y);

            title.Size = new Vector2(Size.X - 20, title.Size.Y);

            removable = new CheckBox(5, 55, (int)size.X - 10, 20, "Removable: ", false);
            removable.foreground = Color.White;
            removable.onCheckedChanged += new CheckBox.CheckBoxCheckedHandler(removable_onCheckedChanged);
            controls.Add(removable);

            Label l = new Label(5, 80, "Voltage:");
            l.foreground = Color.White;
            controls.Add(l);

            voltage = new TextBox(175, 80, (int)size.X - 180, 20);
            voltage.MaxLength = 5;
            voltage.Multiline = false;
            voltage.Mask = TextBox.InputMask.Numbers;
            voltage.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(voltage);

            l = new Label(5, 105, "Period:");
            l.foreground = Color.White;
            controls.Add(l);

            period = new TextBox(175, 105, (int)size.X - 180, 20);
            period.MaxLength = 4;
            period.Multiline = false;
            period.Mask = TextBox.InputMask.Numbers;
            period.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(period);

            base.Initialize();
        }

        public override void Load()
        {
            var p = (AssociatedComponent.Logics as Logics.PowerACLogics);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                voltage.Editable = AssociatedComponent.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                voltage.Editable = true;
            }
            voltage.Editable = AssociatedComponent.IsRemovable;
            period.Editable = voltage.Editable;

            String s = p.voltage.ToString();
            if (s.Length > voltage.MaxLength) s = s.Substring(0, voltage.MaxLength);
            voltage.Text = s;

            s = p.period.ToString();
            if (s.Length > period.MaxLength) s = s.Substring(0, period.MaxLength);
            period.Text = s;
        }

        public override void Save()
        {
            double t;
            if (AssociatedComponent != null)
            {
                if (Double.TryParse(voltage.Text, out t))
                {
                    if (t < 1) t = 1;
                    if (t > Settings.MAX_VOLTAGE) t = Settings.MAX_VOLTAGE;
                    (AssociatedComponent.Logics as Logics.PowerACLogics).voltage = t;
                }
                if (Double.TryParse(period.Text, out t))
                {
                    if (t < 1) t = 1;
                    //if (t > Settings.MAX_VOLTAGE) t = Settings.MAX_VOLTAGE;
                    (AssociatedComponent.Logics as Logics.PowerACLogics).period = (float)t;
                }
            }
        }
    }
}
