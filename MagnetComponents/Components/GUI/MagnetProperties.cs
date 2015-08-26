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
    public class MagnetProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public CheckBox north, south;
        public TextBox fieldRange;

        public override void Initialize()
        {

            WasInitialized = true;

            size = new Vector2(180, 130);

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

            north = new CheckBox(5, 80, (int)size.X / 2 - 10, 20, "North: ", false);
            north.foreground = Color.White;
            north.onCheckedChanged += new CheckBox.CheckBoxCheckedHandler(north_onCheckedChanged);
            controls.Add(north);

            south = new CheckBox((int)size.X / 2 + 5, 80, (int)size.X / 2 - 10, 20, "South: ", false);
            south.foreground = Color.White;
            south.onCheckedChanged += new CheckBox.CheckBoxCheckedHandler(south_onCheckedChanged);
            controls.Add(south);

            Label l = new Label(5, 105, "Field range:");
            l.foreground = Color.White;
            controls.Add(l);

            fieldRange = new TextBox(125, 105, (int)size.X - 130, 20);
            fieldRange.MaxLength = 5;
            fieldRange.Multiline = false;
            fieldRange.Mask = TextBox.InputMask.Numbers;
            fieldRange.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(fieldRange);

            base.Initialize();
        }

        bool ignoreCheckEvent = false;
        void south_onCheckedChanged(object sender, bool IsChecked)
        {
            if (ignoreCheckEvent)
                return;
            if (IsChecked)
            {
                ignoreCheckEvent = true;
                north.Checked = false;
                ignoreCheckEvent = false;
                (AssociatedComponent as Magnet).pole = MagnetPole.S;
            }
            else
                north.Checked = true;
        }

        void north_onCheckedChanged(object sender, bool IsChecked)
        {
            if (ignoreCheckEvent)
                return;
            if (IsChecked)
            {
                ignoreCheckEvent = true;
                south.Checked = false;
                ignoreCheckEvent = false;
                (AssociatedComponent as Magnet).pole = MagnetPole.N;
            }
            else
                south.Checked = true;
        }

        public override void Load()
        {
            var w = (AssociatedComponent as Magnet);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                fieldRange.Editable = w.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                fieldRange.Editable = true;
            }
            fieldRange.Editable = w.IsRemovable;
            north.Enabled = w.IsRemovable;
            south.Enabled = w.IsRemovable;

            south.Checked = w.pole == MagnetPole.S;
            north.Checked = w.pole == MagnetPole.N;

            String s = w.FieldRadius.ToString();
            if (s.Length > fieldRange.MaxLength) s = s.Substring(0, fieldRange.MaxLength);
            fieldRange.Text = s;
        }

        public override void Save()
        {
            double t;
            if (AssociatedComponent != null)
            {
                if (Double.TryParse(fieldRange.Text, out t))
                {
                    if (t < 1) t = 1;
                    if (t > Settings.MAX_MAGNETIC_FIELD) t = Settings.MAX_MAGNETIC_FIELD;
                    (AssociatedComponent as Magnet).FieldRadius = (float)t;
                }
            }
            Load();
        }
    }
}
