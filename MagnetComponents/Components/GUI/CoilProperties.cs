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
    public class CoilProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox fieldRange;
        public Label power;

        public override void Initialize()
        {

            WasInitialized = true;

            size = new Vector2(170, 170);//130

            title = new Label(0, 5, AssociatedComponent.Graphics.GetCSToolTip());
            title.TextAlignment = Renderer.TextAlignment.Center;
            title.foreground = Color.White;
            title.font = TitleFont;
            controls.Add(title);

            Size = new Vector2(Math.Max(title.font.MeasureString(title.text).X + 80, Size.X), Size.Y);

            title.Size = new Vector2(Size.X - 20, title.Size.Y);

            removable = new CheckBox(5, 55, (int)size.X - 10, 20, "Removable: ", false);
            removable.foreground = Color.White;
            removable.onCheckedChanged += new CheckBox.CheckBoxCheckedHandler(removable_onCheckedChanged);
            controls.Add(removable);

            Label l = new Label(5, 80, "Field range:");
            l.foreground = Color.White;
            controls.Add(l);

            fieldRange = new TextBox(125, 80, (int)size.X - 130, 20);
            fieldRange.MaxLength = 5;
            fieldRange.Multiline = false;
            fieldRange.Mask = TextBox.InputMask.Numbers;
            fieldRange.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(fieldRange);

            l = new Label(5, 105, "Power:");
            l.foreground = Color.White;
            controls.Add(l);

            power = new Label(5, 105, "");
            power.foreground = Color.White;
            power.TextAlignment = Renderer.TextAlignment.Right;
            controls.Add(power);

            base.Initialize();
        }

        public override void Update()
        {
            //power.text = ((int)((AssociatedComponent.Logics as Logics.CoilLogics).chargeState)).ToString() + " %";//TODO
            Coil p = AssociatedComponent as Coil;
            var l = AssociatedComponent.Logics as Logics.CoilLogics;
            power.text = Math.Round(l.chargeState, 4).ToString() + "\r\n" +
                "cur: " + Math.Round(l.curField.force * 10000, 4).ToString() + "   " + l.curField.direction.ToString();
            power.Size = new Vector2(size.X - 10, 20);

            base.Update();
        }

        public override void Load()
        {
            var w = (AssociatedComponent as Coil);
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
            fieldRange.Editable = w.IsRemovable;

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
                    (AssociatedComponent as Coil).FieldRadius = (float)t;
                }
            }
        }
    }
}
