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
    public class CapacitorProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox capacitance, voltage;
        public Label charge;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(170, 155);

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

            Label l = new Label(5, 80, "Capasitance:");
            l.foreground = Color.White;
            controls.Add(l);

            capacitance = new TextBox(115, 80, (int)size.X - 120, 20);
            capacitance.MaxLength = 5;
            capacitance.Multiline = false;
            capacitance.Mask = TextBox.InputMask.Numbers;
            capacitance.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(capacitance);

            l = new Label(5, 105, "Voltage:");
            l.foreground = Color.White;
            controls.Add(l);

            voltage = new TextBox(115, 105, (int)size.X - 120, 20);
            voltage.MaxLength = 5;
            voltage.Multiline = false;
            voltage.Mask = TextBox.InputMask.Numbers;
            voltage.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(voltage);

            l = new Label(5, 130, "Current charge: ");
            l.foreground = Color.White;
            controls.Add(l);

            charge = new Label(5, 130, "100%");
            charge.foreground = Color.White;
            charge.TextAlignment = Renderer.TextAlignment.Right;
            charge.Size = new Vector2(Size.X - 10, charge.Size.Y);
            controls.Add(charge);

            base.Initialize();
        }

        public override void Update()
        {
            var l = AssociatedComponent.Logics as Logics.CapacitorLogics;
            charge.text = ((int)Math.Abs(l.CurCharge / l.Capacitance * 100)).ToString() + "%";
            charge.Size = new Vector2(Size.X - 10, charge.Size.Y);

            base.Update();
        }

        public override void Load()
        {
            var w = (AssociatedComponent.Logics as Logics.CapacitorLogics);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                capacitance.Editable = AssociatedComponent.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                capacitance.Editable = true;
            }
            capacitance.Editable = AssociatedComponent.IsRemovable;
            voltage.Editable = capacitance.Editable;

            String s = w.Capacitance.ToString();
            if (s.Length > capacitance.MaxLength) 
                s = s.Substring(0, capacitance.MaxLength);
            capacitance.Text = s;

            s = w.MaxOutputVoltage.ToString();
            if (s.Length > voltage.MaxLength) 
                s = s.Substring(0, voltage.MaxLength);
            voltage.Text = s;
        }

        public override void Save()
        {
            double t;
            if (AssociatedComponent != null)
            {
                if (Double.TryParse(capacitance.Text, out t))
                {
                    if (t < 1) t = 1;
                    if (t > Settings.MAX_CAPACITANCE) t = Settings.MAX_CAPACITANCE;
                    (AssociatedComponent.Logics as Logics.CapacitorLogics).Capacitance = (float)t;
                }
                if (Double.TryParse(voltage.Text, out t))
                {
                    if (t < 5) t = 5;
                    if (t > Settings.MAX_VOLTAGE) t = Settings.MAX_VOLTAGE;
                    (AssociatedComponent.Logics as Logics.CapacitorLogics).MaxOutputVoltage = t;
                }
            }
        }
    }
}
