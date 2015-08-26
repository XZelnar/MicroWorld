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
    public class DelayerProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox delay;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(150, 105);

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

            Label l = new Label(5, 80, "Delay:");
            l.foreground = Color.White;
            controls.Add(l);

            delay = new TextBox(115, 80, (int)size.X - 120, 20);
            delay.MaxLength = 5;
            delay.Multiline = false;
            delay.Mask = TextBox.InputMask.Numbers;
            delay.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(delay);

            base.Initialize();
        }

        public override void Load()
        {
            var p = (AssociatedComponent as Delayer);
            var l = (AssociatedComponent.Logics as Logics.DelayerLogics);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                delay.Editable = p.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                delay.Editable = true;
            }
            delay.Editable = p.IsRemovable;

            String s = l.Delay.ToString();
            if (s.Length > delay.MaxLength) s = s.Substring(0, delay.MaxLength);
            delay.Text = s;
        }

        public override void Save()
        {
            int t;
            if (AssociatedComponent != null)
            {
                if (Int32.TryParse(delay.Text, out t))
                {
                    if (t < 0) t = 0;
                    if (t > 1000) t = 1000;
                    (AssociatedComponent.Logics as Logics.DelayerLogics).Delay = (int)t;
                }
            }
        }
    }
}
