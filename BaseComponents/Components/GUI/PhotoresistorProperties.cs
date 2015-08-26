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
    public class PhotoresistorProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox resistance;
        public Label curResistance;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(200, 130);

            title = new Label(0, 5, AssociatedComponent.Graphics.GetCSToolTip());
            title.font = TitleFont;
            title.UpdateSizeToTextSize();
            title.TextAlignment = Renderer.TextAlignment.Center;
            title.foreground = Color.White;
            controls.Add(title);

            Size = new Vector2(Math.Max(title.font.MeasureString(title.text).X + 120, Size.X), Size.Y);

            title.Size = new Vector2(Size.X - 20, title.Size.Y);

            removable = new CheckBox(5, 55, (int)size.X - 10, 20, "Removable: ", false);
            removable.foreground = Color.White;
            removable.onCheckedChanged += new CheckBox.CheckBoxCheckedHandler(removable_onCheckedChanged);
            controls.Add(removable);

            Label l = new Label(5, 80, "Max resistance:");
            l.foreground = Color.White;
            controls.Add(l);

            resistance = new TextBox(205, 80, (int)size.X - 210, 20);
            resistance.MaxLength = 5;
            resistance.Multiline = false;
            resistance.Mask = TextBox.InputMask.Numbers;
            resistance.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(resistance);

            l = new Label(5, 105, "Current resistance:");
            l.foreground = Color.White;
            controls.Add(l);

            curResistance = new Label(5, 105, "");
            curResistance.TextAlignment = Renderer.TextAlignment.Right;
            curResistance.Size = new Vector2(size.X - 10, 20);
            curResistance.foreground = Color.White;
            controls.Add(curResistance);

            base.Initialize();
        }

        public override void Update()
        {
            curResistance.text = ((int)((AssociatedComponent as Photoresistor).W.Resistance)).ToString() + " Ω";
            curResistance.Size = new Vector2(size.X - 10, 20);

            base.Update();
        }

        public override void Load()
        {
            var w = (AssociatedComponent as Photoresistor);
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

            String s = w.MaxResistance.ToString();
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
                    if (t < 0) t = 0;
                    if (t > Settings.MAX_RESISTANCE) t = Settings.MAX_RESISTANCE;
                    (AssociatedComponent as Photoresistor).MaxResistance = (float)t;
                }
            }
        }
    }
}
