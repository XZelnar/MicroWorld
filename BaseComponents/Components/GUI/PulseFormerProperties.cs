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
    public class PulseFormerProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public TextBox resistance;
        public MicroWorld.Graphics.GUI.Elements.Button loadSeq;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(220, 130);

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

            Label l = new Label(5, 80, "Max resistance:");
            l.foreground = Color.White;
            controls.Add(l);

            resistance = new TextBox(145, 80, (int)size.X - 150, 20);
            resistance.MaxLength = 7;
            resistance.Multiline = false;
            resistance.Mask = TextBox.InputMask.Numbers;
            resistance.onLooseFocus += new TextBox.FocusEventHandler(tb_onLooseFocus);
            controls.Add(resistance);

            loadSeq = new MicroWorld.Graphics.GUI.Elements.Button(5, 105, (int)size.X - 10, 20, "Load sequence");
            loadSeq.onClicked += new MicroWorld.Graphics.GUI.Elements.Button.ClickedEventHandler(loadSeq_onClicked);
            controls.Add(loadSeq);

            base.Initialize();
        }

        void loadSeq_onClicked(object sender, InputEngine.MouseArgs e)
        {
            System.Windows.Forms.OpenFileDialog d = new System.Windows.Forms.OpenFileDialog();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(d.FileName);
                List<float> t = new List<float>();
                while (sr.Peek() > -1)
                {
                    try
                    {
                        String s = sr.ReadLine();
                        s = s.Replace('.', ',');
                        t.Add((float)Convert.ToDouble(s));
                    }
                    catch
                    {
                        t.Add(0);
                    }
                }
                sr.Close();
                (AssociatedComponent.Logics as Logics.PulseFormerLogics).pulses = t.ToArray();
            }
        }

        public override void Load()
        {
            var w = (AssociatedComponent as PulseFormer);
            removable.Checked = AssociatedComponent.IsRemovable;
            if (Main.CurState == "GAMELevels")
            {
                removable.Enabled = false;
                resistance.Editable = w.IsRemovable;
                loadSeq.isEnabled = w.IsRemovable;
            }
            else
            {
                removable.Enabled = true;
                resistance.Editable = true;
                loadSeq.isEnabled = true;
            }
            resistance.Editable = w.IsRemovable;
            loadSeq.isEnabled = w.IsRemovable;

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
                    if (t < 10) t = 10;
                    if (t > Settings.MAX_RESISTANCE) t = Settings.MAX_RESISTANCE;
                    (AssociatedComponent as PulseFormer).MaxResistance = (float)t;
                }
            }
        }
    }
}
