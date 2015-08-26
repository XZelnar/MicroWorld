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
    public class AccumulatorProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox maxCharge;
        public ProgressBar pb_charge, pb_startCharge;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(250, 155);

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

            Label l = new Label(5, 80, "Max charge:");
            l.foreground = Color.White;
            controls.Add(l);

            maxCharge = new TextBox(185, 80, (int)size.X - 190, 20);
            maxCharge.MaxLength = 5;
            maxCharge.Multiline = false;
            maxCharge.Mask = TextBox.InputMask.Numbers;
            maxCharge.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(maxCharge);

            l = new Label(5, 105, "Initial charge:");
            l.foreground = Color.White;
            controls.Add(l);

            pb_startCharge = new ProgressBar(140, 105, (int)size.X - 145, 20, true);
            pb_startCharge.Foreground = ResourceManager.Load<Texture2D>("RedYellowLimeGradient");
            pb_startCharge.onValueChanged += new ProgressBar.ValueChangedEventHandler(pb_startCharge_onValueChanged);
            controls.Add(pb_startCharge);

            l = new Label(5, 130, "Charge:");
            l.foreground = Color.White;
            controls.Add(l);

            pb_charge = new ProgressBar(140, 130, (int)size.X - 145, 20, false);
            pb_charge.Foreground = ResourceManager.Load<Texture2D>("RedYellowLimeGradient");
            controls.Add(pb_charge);

            base.Initialize();
        }

        void pb_startCharge_onValueChanged(object sender, int newValue, int oldValue)
        {
            (AssociatedComponent.Logics as Logics.AccumulatorLogics).StartCharge = newValue;
        }

        public override void Update()
        {
            base.Update();

            pb_charge.MaxValue = (int)(AssociatedComponent.Logics as Logics.AccumulatorLogics).MaxCharge;
            pb_charge.Value = (int)(AssociatedComponent.Logics as Logics.AccumulatorLogics).Charge;
            pb_startCharge.MaxValue = (int)(AssociatedComponent.Logics as Logics.AccumulatorLogics).MaxCharge;
        }

        public override void Load()
        {
            var p = (AssociatedComponent as Accumulator);
            var l = (AssociatedComponent.Logics as Logics.AccumulatorLogics);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                maxCharge.Editable = p.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                maxCharge.Editable = true;
            }
            maxCharge.Editable = p.IsRemovable;
            pb_startCharge.Enabled = p.IsRemovable;

            pb_charge.MaxValue = (int)l.MaxCharge;
            pb_startCharge.MaxValue = (int)l.StartCharge;
            pb_charge.Value = (int)l.Charge;
            String s = l.MaxCharge.ToString();
            if (s.Length > maxCharge.MaxLength) s = s.Substring(0, maxCharge.MaxLength);
            maxCharge.Text = s;
        }

        public override void Save()
        {
            int t;
            if (AssociatedComponent != null)
            {
                if (Int32.TryParse(maxCharge.Text, out t))
                {
                    if (t < 0) t = 0;
                    if (t > 10000) t = 10000;
                    (AssociatedComponent.Logics as Logics.AccumulatorLogics).MaxCharge = (int)t;
                }
            }
        }
    }
}
