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
    public class LEDProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox luminosity;
        public MicroWorld.Graphics.GUI.Elements.Button color;
        public Label brightness;

        MicroWorld.Graphics.GUI.Scene.ColorSelector colsel;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(180, 150);

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

            Label l = new Label(5, 80, "Luminosity:");
            l.foreground = Color.White;
            controls.Add(l);

            luminosity = new TextBox(125, 80, (int)size.X - 130, 20);
            luminosity.MaxLength = 5;
            luminosity.Multiline = false;
            luminosity.Mask = TextBox.InputMask.Numbers;
            luminosity.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(luminosity);

            l = new Label(5, 105, "Color: ");
            l.foreground = Color.White;
            controls.Add(l);

            color = new MicroWorld.Graphics.GUI.Elements.Button(125, 105, (int)size.X - 130, 20, "");
            color.onClicked += new MicroWorld.Graphics.GUI.Elements.Button.ClickedEventHandler(color_onClicked);
            controls.Add(color);

            l = new Label(5, 130, "Brightness: ");
            l.foreground = Color.White;
            controls.Add(l);

            brightness = new Label(5, 130, "");
            brightness.foreground = Color.White;
            brightness.TextAlignment = Renderer.TextAlignment.Right;
            controls.Add(brightness);

            base.Initialize();
        }

        void color_onClicked(object sender, InputEngine.MouseArgs e)
        {
            colsel = ColorSelector.Show(position + color.Position + new Vector2(color.Size.X, 0));
            colsel.onColorSelected += new ColorSelector.ColorSelectedEventHandler(colsel_onColorSelected);
        }

        void colsel_onColorSelected(object sender, ColorSelector.ColorSelectedArgs e)
        {
            color.background = e.color;
            color.disabledColor = color.background;
            color.mouseOverColor = color.background;
            color.pressedColor = color.background;
        }

        public override void Update()
        {
            if (colsel != null && colsel.isVisible)
            {
                colsel.position = position + color.Position + new Vector2(color.Size.X, 0);
            }

            brightness.text = ((int)((AssociatedComponent.Logics as Logics.LEDLogics).Brightness * 100)).ToString() + " %";
            brightness.Size = new Vector2(size.X - 10, 20);

            base.Update();
        }

        public override void Load()
        {
            var w = (AssociatedComponent as LED);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                color.isEnabled = w.IsRemovable;
                luminosity.Editable = w.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                color.isEnabled = true;
                luminosity.Editable = true;
            }
            luminosity.Editable = w.IsRemovable;
            luminosity.Editable = w.IsRemovable;

            String s = w.Luminosity.ToString();
            if (s.Length > luminosity.MaxLength) s = s.Substring(0, luminosity.MaxLength);
            luminosity.Text = s;

            color.background = (w.Graphics as Graphics.LEDGraphics).LEDColor;
            color.disabledColor = color.background;
            color.mouseOverColor = color.background;
            color.pressedColor = color.background;
        }

        public override void Save()
        {
            double t;
            if (AssociatedComponent != null)
            {
                if (Double.TryParse(luminosity.Text, out t))
                {
                    if (t < 1) t = 1;
                    if (t > Settings.MAX_LUMINOSITY) t = Settings.MAX_LUMINOSITY;
                    (AssociatedComponent as LED).Luminosity = (float)t;
                }

                (AssociatedComponent.Graphics as Graphics.LEDGraphics).LEDColor = color.background;
            }
        }
    }
}
