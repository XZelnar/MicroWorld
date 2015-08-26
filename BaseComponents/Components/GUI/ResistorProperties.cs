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
    public class ResistorProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox resistance;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(170, 105);

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

            Label l = new Label(5, 80, "Resistance:");
            l.foreground = Color.White;
            controls.Add(l);

            resistance = new TextBox(115, 80, (int)size.X - 120, 20);
            resistance.MaxLength = 5;
            resistance.Multiline = false;
            resistance.Mask = TextBox.InputMask.Numbers;
            resistance.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(resistance);

            base.Initialize();
        }

        public override void Load()
        {
            var w = (AssociatedComponent as Resistor);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                resistance.Editable = w.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                resistance.Editable = true;
            }
            resistance.Editable = w.IsRemovable;

            String s = w.Resistance.ToString();
            if (s.Length > resistance.MaxLength) s = s.Substring(0, resistance.MaxLength);
            resistance.Text = s;
        }

        public override void Save()
        {
            double t;
            if (AssociatedComponent != null)
            {
                if (Double.TryParse(resistance.Text, out t))
                {
                    if (t < 1) t = 1;
                    if (t > Settings.MAX_RESISTANCE) t = Settings.MAX_RESISTANCE;
                    (AssociatedComponent as Resistor).Resistance = (float)t;
                }
            }
        }
    }
}
