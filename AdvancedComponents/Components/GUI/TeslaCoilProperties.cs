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
    public class TeslaCoilProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox voltage;
        public TextBox capacitance;
        public TextBox range;
        public Label curCharge;

        public override void Initialize()
        {

            WasInitialized = true;

            size = new Vector2(230, 180);

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

            Label l = new Label(5, 80, "Discharge voltage:");
            l.foreground = Color.White;
            controls.Add(l);

            voltage = new TextBox(175, 80, (int)size.X - 180, 20);
            voltage.MaxLength = 5;
            voltage.Multiline = false;
            voltage.Mask = TextBox.InputMask.Numbers;
            voltage.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(voltage);

            l = new Label(5, 105, "Capacitance:");
            l.foreground = Color.White;
            controls.Add(l);

            capacitance = new TextBox(175, 105, (int)size.X - 180, 20);
            capacitance.MaxLength = 6;
            capacitance.Multiline = false;
            capacitance.Mask = TextBox.InputMask.Numbers;
            capacitance.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(capacitance);

            l = new Label(5, 130, "Range:");
            l.foreground = Color.White;
            controls.Add(l);

            range = new TextBox(175, 130, (int)size.X - 180, 20);
            range.MaxLength = 3;
            range.Multiline = false;
            range.Mask = TextBox.InputMask.Numbers;
            range.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(range);

            curCharge = new Label(5, 155, "");
            curCharge.foreground = Color.White;
            curCharge.TextAlignment = Renderer.TextAlignment.Right;
            controls.Add(curCharge);

            base.Initialize();
        }

        public override void Update()
        {
            curCharge.text = ((int)((AssociatedComponent.Logics as Logics.TeslaCoilLogics).ChargingProcess * 100)).ToString() + "% charged";
            curCharge.Size = new Vector2(size.X - 10, 20);

            base.Update();
        }

        public override void Load()
        {
            var p = (AssociatedComponent as TeslaCoil);
            var l = (AssociatedComponent.Logics as Logics.TeslaCoilLogics);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                voltage.Editable = p.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                voltage.Editable = true;
            }
            voltage.Editable = p.IsRemovable;

            String s = l.DischargeVoltage.ToString();
            if (s.Length > voltage.MaxLength) s = s.Substring(0, voltage.MaxLength);
            voltage.Text = s;

            s = l.Capacitance.ToString();
            if (s.Length > capacitance.MaxLength) s = s.Substring(0, capacitance.MaxLength);
            capacitance.Text = s;

            s = l.Range.ToString();
            if (s.Length > range.MaxLength) s = s.Substring(0, range.MaxLength);
            range.Text = s;
        }

        public override void Save()
        {
            double t;
            if (AssociatedComponent != null)
            {
                var l = (AssociatedComponent.Logics as Logics.TeslaCoilLogics);

                if (Double.TryParse(voltage.Text, out t))
                {
                    if (t < 20) t = 20;
                    if (t > Settings.MAX_VOLTAGE) t = Settings.MAX_VOLTAGE;
                    l.DischargeVoltage = (float)t;
                }
                if (Double.TryParse(range.Text, out t))
                {
                    if (t < 32) t = 32;
                    if (t > 256) t = 256;
                    l.Range = (float)t;
                }
                if (Double.TryParse(capacitance.Text, out t))
                {
                    if (t < l.DischargeVoltage * 10) t = l.DischargeVoltage * 10;
                    if (t > 10000) t = 10000;
                    l.Capacitance = (float)t;
                }

                Load();
            }
        }
    }
}
