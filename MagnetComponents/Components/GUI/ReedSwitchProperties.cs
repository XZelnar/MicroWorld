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
    public class ReedSwitchProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox reqField;

        public override void Initialize()
        {

            WasInitialized = true;

            size = new Vector2(220, 105);

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

            Label l = new Label(5, 80, "Required field:");
            l.foreground = Color.White;
            controls.Add(l);

            reqField = new TextBox(165, 80, (int)size.X - 170, 20);
            reqField.MaxLength = 5;
            reqField.Multiline = false;
            reqField.Mask = TextBox.InputMask.Numbers;
            reqField.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(reqField);

            base.Initialize();
        }

        public override void Load()
        {
            var w = (AssociatedComponent as ReedSwitch);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                reqField.Editable = w.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                reqField.Editable = true;
            }
            reqField.Editable = w.IsRemovable;

            String s = (AssociatedComponent.Logics as Logics.ReedSwitchLogics).RequiredField.ToString();
            if (s.Length > reqField.MaxLength) s = s.Substring(0, reqField.MaxLength);
            reqField.Text = s;
        }

        public override void Save()
        {
            double t;
            if (AssociatedComponent != null)
            {
                if (Double.TryParse(reqField.Text, out t))
                {
                    if (t < 0) t = 0;
                    if (t > Settings.MAX_MAGNETIC_FIELD) t = Settings.MAX_MAGNETIC_FIELD;
                    (AssociatedComponent.Logics as Logics.ReedSwitchLogics).RequiredField = (float)t;
                }
            }
        }
    }
}
