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
    public class WireProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox maxCurrent;
        public Label current;
        public Label voltage;

        protected override bool DimOpacity
        {
            get
            {
                return base.DimOpacity || (AssociatedComponent as Wire).dnd;
            }
        }

        public override void Initialize()
        {

            WasInitialized = true;

            size = new Vector2(220, 155);

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

            Label l = new Label(5, 80, "Max current:");
            l.foreground = Color.White;
            controls.Add(l);

            maxCurrent = new TextBox(165, 80, (int)size.X - 170, 20);
            maxCurrent.MaxLength = 8;
            maxCurrent.Multiline = false;
            maxCurrent.Mask = TextBox.InputMask.Numbers;
            maxCurrent.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(maxCurrent);

            l = new Label(5, 105, "Current:");
            l.foreground = Color.White;
            controls.Add(l);

            current = new Label(5, 105, "");
            current.foreground = Color.White;
            current.TextAlignment = Renderer.TextAlignment.Right;
            controls.Add(current);

            l = new Label(5, 130, "Voltage drop:");
            l.foreground = Color.White;
            controls.Add(l);

            voltage = new Label(5, 130, "");
            voltage.foreground = Color.White;
            voltage.TextAlignment = Renderer.TextAlignment.Right;
            controls.Add(voltage);

            base.Initialize();
        }

        public override void Update()
        {
            if (Double.IsNaN((AssociatedComponent as Wire).Current))
                current.text = "0 A";
            else
                current.text = ((float)((int)((AssociatedComponent as Wire).Current * 100)) / 100).ToString() + " A";
            if (Double.IsNaN((AssociatedComponent as Wire).VoltageDropAbs))
                voltage.text = "0 A";
            else
                voltage.text = ((float)((int)((AssociatedComponent as Wire).VoltageDropAbs * 100)) / 100).ToString() + " V";

            current.Size = new Vector2((int)size.X - 10, 20);
            voltage.Size = new Vector2((int)size.X - 10, 20);

            base.Update();
        }

        public override void Load()
        {
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.curState == "GAMELevels")
            {
                removable.Enabled = false;
                maxCurrent.Editable = false;
            }
            else
            {
                removable.Enabled = true;
                maxCurrent.Editable = true;
            }
            String s = (AssociatedComponent as Wire).MaxWithstandingCurrent.ToString();
            if (s.Length > maxCurrent.MaxLength) s = s.Substring(0, maxCurrent.MaxLength);
            maxCurrent.Text = s;
        }

        public override void Save()
        {
            double t;
            if (AssociatedComponent != null && Double.TryParse(maxCurrent.Text, out t))
            {
                if (t < 0) t = -1;
                (AssociatedComponent as Wire).MaxWithstandingCurrent = t;
            }
        }
    }
}
