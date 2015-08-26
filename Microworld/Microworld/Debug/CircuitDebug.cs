using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MicroWorld
{
    public partial class CircuitDebug : Form
    {
        public CircuitDebug()
        {
            InitializeComponent();
        }

        private void CircuitDebug_Load(object sender, EventArgs e)
        {
            this.Location = Screen.AllScreens[1].WorkingArea.Location;
        }

        private void CircuitDebug_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush b = new SolidBrush(Color.Blue);
            SolidBrush yc = new SolidBrush(Color.Lime);
            SolidBrush r = new SolidBrush(Color.Red);

            for (int i = 0; i < lv.Items.Count; i++)
            {
                float x = 2;
                float y = i * 15;
                while (y >= ClientSize.Height-5)
                {
                    y -= ClientSize.Height;
                    x += 150;
                }
                e.Graphics.DrawString(lv.Items[i].Text, lv.Font, b, new PointF(x, y));
                e.Graphics.DrawString(lv.Items[i].SubItems[1].Text, lv.Font, yc, new PointF(x+30, y));
                e.Graphics.DrawString(lv.Items[i].SubItems[2].Text, lv.Font, r, new PointF(x+60, y));
            }

            if (ch.Series[0].Points.Count > 100)
            {
                ch.Series[0].Points.RemoveAt(0);
            }
            try
            {
                ch.Series[0].Points.AddY(Convert.ToDouble(lv.Items[Convert.ToInt32(ch.Tag)].SubItems[1].Text));
            }
            catch { }
            ch.Invalidate();
        }

        private void CircuitDebug_MouseClick(object sender, MouseEventArgs e)
        {
            int x = (int)(e.X / 150);
            int y = (int)(e.Y / 15);
            int id = y + x * 48;
            if (ch.Tag == null || id.ToString() != ch.Tag.ToString())
            {
                ch.Series[0].Points.Clear();
                ch.Tag = id.ToString();
                ch.Series[0].Points.AddY(5);
                //ch.Series[0].Points.RemoveAt(0);
            }
        }
    }
}
