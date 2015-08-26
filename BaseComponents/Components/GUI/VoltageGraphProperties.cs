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
    public class VoltageGraphProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox max, min;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(250, 130);

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

            Label l = new Label(5, 80, "Min voltage:");
            l.foreground = Color.White;
            controls.Add(l);

            min = new TextBox(185, 80, (int)size.X - 190, 20);
            min.MaxLength = 3;
            min.Multiline = false;
            min.Mask = TextBox.InputMask.Numbers;
            min.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(min);

            l = new Label(5, 105, "Max voltage:");
            l.foreground = Color.White;
            controls.Add(l);

            max = new TextBox(185, 105, (int)size.X - 190, 20);
            max.MaxLength = 3;
            max.Multiline = false;
            max.Mask = TextBox.InputMask.Numbers;
            max.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(max);

            base.Initialize();
        }

        public override void Load()
        {
            var p = (AssociatedComponent as VoltageGraph);
            var l = (AssociatedComponent.Logics as Logics.VoltageGraphLogics);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
            }
            else
            {
                removable.Enabled = true;
            }
            max.Editable = p.IsRemovable;
            min.Editable = p.IsRemovable;

            String s = l.max.ToString();
            if (s.Length > max.MaxLength) s = s.Substring(0, max.MaxLength);
            max.Text = s;

            s = l.min.ToString();
            if (s.Length > min.MaxLength) s = s.Substring(0, min.MaxLength);
            min.Text = s;
        }

        public override void Save()
        {
            int t;
            if (AssociatedComponent != null)
            {
                if (Int32.TryParse(max.Text, out t))
                    (AssociatedComponent.Logics as Logics.VoltageGraphLogics).max = t;
                if (Int32.TryParse(min.Text, out t))
                    (AssociatedComponent.Logics as Logics.VoltageGraphLogics).min = t;
                if ((AssociatedComponent.Logics as Logics.VoltageGraphLogics).max < (AssociatedComponent.Logics as Logics.VoltageGraphLogics).min)
                {
                    t = (AssociatedComponent.Logics as Logics.VoltageGraphLogics).max;
                    (AssociatedComponent.Logics as Logics.VoltageGraphLogics).max = (AssociatedComponent.Logics as Logics.VoltageGraphLogics).min;
                    (AssociatedComponent.Logics as Logics.VoltageGraphLogics).min = t;
                    Load();
                }
            }
        }
    }
}
