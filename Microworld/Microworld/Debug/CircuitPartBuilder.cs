using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MicroWorld.Debug
{
    public partial class CircuitPartBuilder : Form
    {
        Logics.CircuitPart cp = null;
        Point offset = new Point();

        public CircuitPartBuilder()
        {
            InitializeComponent();
        }

        public void LoadList()
        {
            lb_iterations.Items.Clear();
            lb_parts.Items.Clear();
            cp = null;
            offset = new Point(panel1.Width / 2, panel1.Height / 2);

            var a = System.IO.Directory.GetDirectories("debug/CircuitParts");
            for (int i = 0; i < a.Length; i++)
			{
                lb_parts.Items.Add(i.ToString());
			}

            ReDraw();
        }

        public void ReDraw()
        {
            var g = this.CreateGraphics();
            g.Clear(BackColor);
            if (lb_parts.SelectedIndex == -1 || lb_iterations.SelectedIndex == -1)
                return;

            g.FillRectangle(new SolidBrush(Color.Black), panel1.Location.X, panel1.Location.Y, panel1.Width, panel1.Height);
            g.DrawLine(new Pen(Color.Yellow), panel1.Location.X, offset.Y + panel1.Location.Y, panel1.Location.X + panel1.Width, offset.Y + panel1.Location.Y);
            g.DrawLine(new Pen(Color.Yellow), offset.X + panel1.Location.X, panel1.Location.Y, offset.X + panel1.Location.X, panel1.Location.Y + panel1.Height);

            Microsoft.Xna.Framework.Vector2 p;
            for (int i = 0; i < cp.jointspos.Count; i++)
            {
                p = (Microsoft.Xna.Framework.Vector2)cp.jointspos[i];
                Color c = Color.White;
                for (int j = 0; j < cp.potentialGrounds.Count; j++)
                {
                    if (cp.potentialGrounds[j] == i)
                    {
                        c = Color.Blue;
                    }
                }
                for (int j = 0; j < cp.potentialPowerSources.Count; j++)
                {
                    if (cp.potentialPowerSources[j] == i)
                    {
                        if (c == Color.Blue)
                            c = Color.Purple;
                        else
                            c = Color.Red;
                    }
                }

                g.FillRectangle(new SolidBrush(c), p.X + offset.X + panel1.Location.X, p.Y + offset.Y + panel1.Location.Y, 4, 4);
            }

            List<Microsoft.Xna.Framework.Vector2> t = new List<Microsoft.Xna.Framework.Vector2>();
            for (int i = 0; i < cp.wirespos.Count; i++)
            {
                t = (List<Microsoft.Xna.Framework.Vector2>)cp.wirespos[i];
                for (int j = 0; j < t.Count - 1; j++)
                {
                    g.DrawLine(new Pen(Color.Lime), t[j].X + panel1.Location.X + offset.X, t[j].Y + panel1.Location.Y + offset.Y,
                        t[j + 1].X + panel1.Location.X + offset.X, t[j + 1].Y + panel1.Location.Y + offset.Y);
                }
            }
        }

        private void b_refresh_Click(object sender, EventArgs e)
        {
            LoadList();
        }

        private void CircuitPartBuilder_Load(object sender, EventArgs e)
        {
            LoadList();
        }

        private void lb_parts_SelectedIndexChanged(object sender, EventArgs e)
        {
            lb_iterations.Items.Clear();
            ReDraw();

            if (lb_parts.SelectedIndex != -1)
            {
                var a = System.IO.Directory.GetFiles("debug/CircuitParts/" + lb_parts.SelectedIndex.ToString());
                for (int i = 0; i < a.Length; i++)
                {
                    lb_iterations.Items.Add(i.ToString());
                }
            }
        }

        private void lb_iterations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lb_iterations.SelectedIndex != -1)
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                System.IO.Stream s = new System.IO.FileStream("debug/CircuitParts/" + lb_parts.SelectedIndex.ToString() + "/" + lb_iterations.SelectedIndex.ToString() + ".bin", System.IO.FileMode.Open);
                cp = f.Deserialize(s) as Logics.CircuitPart;
                s.Close();
            }

            ReDraw();
        }

        bool dnd = false;
        Point oldloc = new Point();
        private void CircuitPartBuilder_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                dnd = true;
                oldloc = Cursor.Position;
            }
        }

        private void CircuitPartBuilder_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                dnd = false;
        }

        private void CircuitPartBuilder_MouseMove(object sender, MouseEventArgs e)
        {
            if (dnd)
            {
                offset = new Point(Cursor.Position.X - oldloc.X + offset.X, Cursor.Position.Y - oldloc.Y + offset.Y);
                oldloc = Cursor.Position;
                ReDraw();
            }
        }

        private void b_nextIt_Click(object sender, EventArgs e)
        {
            if (lb_iterations.SelectedIndex < lb_iterations.Items.Count - 1 && lb_iterations.SelectedIndex != -1)
                lb_iterations.SelectedIndex++;
        }

        private void b_prevIt_Click(object sender, EventArgs e)
        {
            if (lb_iterations.SelectedIndex > 0)
                lb_iterations.SelectedIndex--;
        }
    }
}
