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
    public partial class MCInfo : Form
    {
        private int debugTicks = 0;

        public int DebugTicks
        {
            get { return debugTicks; }
            set
            {
                debugTicks = value;
                label2.Text = "Ticks: " + debugTicks.ToString();
            }
        }

        private float updateTime = 0;

        public float UpdateTime
        {
            get { return updateTime; }
            set
            {
                updateTime = value;
                this.Invoke(new MethodInvoker(updatelbl3));
            }
        }

        public void updatelbl3()
        {
                label3.Text = "Update time: " + updateTime.ToString() + "ms";
        }

        public MCInfo()
        {
            InitializeComponent();
        }
        public bool isOpen = false;

        public void show()
        {
            DebugTicks = 0;
            Show();
        }

        public void SetTitle(String s)
        {
            this.Text = s;
        }

        public void colse()
        {
            Close();
        }

        private void MCInfo_Shown(object sender, EventArgs e)
        {
            isOpen = true;
        }

        private void MCInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            isOpen = false;
            e.Cancel = true;
            Hide();
        }
    }
}
