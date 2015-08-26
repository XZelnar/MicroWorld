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
    public partial class ViewVisMap : Form
    {
        public ViewVisMap()
        {
            InitializeComponent();
        }

        private void ViewVisMap_Load(object sender, EventArgs e)
        {
            //Location = Screen.AllScreens[1].WorkingArea.Location;
        }

        private unsafe void timer1_Tick(object sender, EventArgs e)
        {
            //System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(Draw));
            //t.Start();
            if (!Visible)
                return;
            Draw();
            BackgroundImage = bmp;
        }
        Bitmap bmp = new Bitmap(1024, 1024);
        const int ofs = 128;
        public unsafe void Draw()
        {
            lock (bmp)
            {
                bmp = new Bitmap(1024, 1024);
                //xpyp
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (!Components.ComponentsManager.VisibilityMap.Exists(i * 64 * 8, j * 64 * 8)) continue;
                        var c = Components.ComponentsManager.VisibilityMap.GetChunk(i * 64 * 8, j * 64 * 8);
                        for (int x = 0; x < 64; x++)
                        {
                            for (int y = 0; y < 64; y++)
                            {
                                if (x == 0 || y == 0)
                                    bmp.SetPixel(ofs + x + i * 64, ofs + y + j * 64, Color.Yellow);
                                else
                                    bmp.SetPixel(ofs + x + i * 64, ofs + y + j * 64, c.data[x, y] == 0 ? Color.Red : c.data[x, y] == 1 ? Color.Lime : Color.Blue);
                            }
                        }
                    }
                }
                //xpyn
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (!Components.ComponentsManager.VisibilityMap.Exists(i * 64 * 8, -j * 64 * 8 - 1)) continue;
                        var c = Components.ComponentsManager.VisibilityMap.GetChunk(i * 64 * 8, -j * 64 * 8 - 1);
                        for (int x = 0; x < 64; x++)
                        {
                            for (int y = 0; y < 64; y++)
                            {
                                if (x == 0 || y == 0)
                                    bmp.SetPixel(ofs + x + i * 64, ofs - y - j * 64, Color.Yellow);
                                else
                                bmp.SetPixel(ofs + x + i * 64, ofs - y - j * 64, c.data[x, y] == 0 ? Color.Red : c.data[x, y] == 1 ? Color.Lime : Color.Blue);
                            }
                        }
                    }
                }
                //xnyn
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (!Components.ComponentsManager.VisibilityMap.Exists(-i * 64 * 8 - 1, -j * 64 * 8 - 1)) continue;
                        var c = Components.ComponentsManager.VisibilityMap.GetChunk(-i * 64 * 8 - 1, -j * 64 * 8 - 1);
                        for (int x = 0; x < 64; x++)
                        {
                            for (int y = 0; y < 64; y++)
                            {
                                if (x == 0 || y == 0)
                                    bmp.SetPixel(ofs - x - i * 64, ofs - y - j * 64, Color.Yellow);
                                else
                                bmp.SetPixel(ofs - x - i * 64, ofs - y - j * 64, c.data[x, y] == 0 ? Color.Red : c.data[x, y] == 1 ? Color.Lime : Color.Blue);
                            }
                        }
                    }
                }
                //xnyp
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (!Components.ComponentsManager.VisibilityMap.Exists(-i * 64 * 8 - 1, j * 64 * 8)) continue;
                        var c = Components.ComponentsManager.VisibilityMap.GetChunk(-i * 64 * 8 - 1, j * 64 * 8);
                        for (int x = 0; x < 64; x++)
                        {
                            for (int y = 0; y < 64; y++)
                            {
                                if (x == 0 || y == 0)
                                    bmp.SetPixel(ofs - x - i * 64, ofs + y + j * 64, Color.Yellow);
                                else
                                    bmp.SetPixel(ofs - x - i * 64, ofs + y + j * 64, c.data[x, y] == 0 ? Color.Red : c.data[x, y] == 1 ? Color.Lime : Color.Blue);
                            }
                        }
                    }
                }




                bmp.SetPixel(512, 512, Color.Lime);
            }
        }

    }
}
