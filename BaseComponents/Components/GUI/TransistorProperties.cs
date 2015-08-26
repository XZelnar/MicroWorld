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
    public class TransistorProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox baseCurrent;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(200, 105);

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

            Label l = new Label(5, 80, "Max base current:");
            l.foreground = Color.White;
            controls.Add(l);

            baseCurrent = new TextBox(145, 80, (int)size.X - 150, 20);
            baseCurrent.MaxLength = 5;
            baseCurrent.Multiline = false;
            baseCurrent.Mask = TextBox.InputMask.Numbers;
            baseCurrent.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(baseCurrent);

            base.Initialize();
        }

        public override void Load()
        {
            var w = (AssociatedComponent.Logics as Logics.TransistorLogics);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                baseCurrent.Editable = AssociatedComponent.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                baseCurrent.Editable = true;
            }
            baseCurrent.Editable = AssociatedComponent.IsRemovable;

            String s = w.BaseControlVotage.ToString();
            if (s.Length > baseCurrent.MaxLength) s = s.Substring(0, baseCurrent.MaxLength);
            baseCurrent.Text = s;
        }

        public override void Save()
        {
            double t;
            if (AssociatedComponent != null)
            {
                if (Double.TryParse(baseCurrent.Text, out t))
                {
                    if (t < 1) t = 1;
                    if (t > Settings.MAX_VOLTAGE) t = Settings.MAX_VOLTAGE;
                    (AssociatedComponent.Logics as Logics.TransistorLogics).BaseControlVotage = t;
                }
            }
        }
    }
}
