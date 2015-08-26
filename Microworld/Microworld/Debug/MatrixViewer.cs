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
    public partial class MatrixViewer : Form
    {
        public MatrixViewer()
        {
            InitializeComponent();
        }

        private void MatrixViewer_Load(object sender, EventArgs e)
        {
            this.Location = Screen.AllScreens[1].WorkingArea.Location;
        }
    }
}
