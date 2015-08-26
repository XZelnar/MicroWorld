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
    public partial class ChartDebug : Form
    {
        static List<ChartDebug> DoNotDestroyMeList = new List<ChartDebug>();

        public ChartDebug()
        {
            InitializeComponent();

            DoNotDestroyMeList.Add(this);
        }

        bool IsRunning = false;
        long LastTick = 0;
        int CurX = 0;

        ScripterNet.ScripterVM vm = new ScripterNet.ScripterVM();

        private void ChartDebug_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < Modding.ModdingLogics.registeredMods.Count; i++)
                vm.RegisterAssembly(Modding.ModdingLogics.registeredMods[i].GetType().Assembly);
        }

        Color[] colors = new Color[] { Color.Lime, Color.Orange, Color.Red, Color.Yellow, Color.Cyan };

        private void nv_SeriesCount_ValueChanged(object sender, EventArgs e)
        {
            while (nv_SeriesCount.Value < chart.Series.Count)
                chart.Series.RemoveAt(chart.Series.Count - 1);
            while (nv_SeriesCount.Value > chart.Series.Count)
                chart.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series(chart.Series.Count.ToString()) 
                { ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line, 
                    Color = colors[chart.Series.Count], BorderWidth = 4 });

            if (nv_SeriesCount.Value >= 2)
            {
                gb_s2.Visible = true;
                b_s2c.BackColor = chart.Series[1].Color;
                nv_s2w.Value = chart.Series[1].BorderWidth;
            }
            else
                gb_s2.Visible = false;
            if (nv_SeriesCount.Value >= 3)
            {
                gb_s3.Visible = true;
                b_s3c.BackColor = chart.Series[2].Color;
                nv_s3w.Value = chart.Series[2].BorderWidth;
            }
            else
                gb_s3.Visible = false;
            if (nv_SeriesCount.Value >= 4)
            {
                gb_s4.Visible = true;
                b_s4c.BackColor = chart.Series[3].Color;
                nv_s4w.Value = chart.Series[3].BorderWidth;
            }
            else
                gb_s4.Visible = false;
            if (nv_SeriesCount.Value >= 5)
            {
                gb_s5.Visible = true;
                b_s5c.BackColor = chart.Series[4].Color;
                nv_s5w.Value = chart.Series[4].BorderWidth;
            }
            else
                gb_s5.Visible = false;
        }

        private void nv_ValuesCount_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < chart.Series.Count; i++)
                while (chart.Series[i].Points.Count > nv_ValuesCount.Value)
                    chart.Series[i].Points.RemoveAt(0);
        }

        private void b_start_Click(object sender, EventArgs e)
        {
            if (!IsRunning)
            {
                Start();
            }
            else
                Stop();
        }

        public void Start()
        {
            //timer.Start();
            IsRunning = true;
            b_start.BackColor = Color.Red;
            b_start.Text = "Stop";

            try
            {
                vm.Execute(tb_code.Text);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Stop()
        {
            //timer.Stop();
            vm.Terminate();
            IsRunning = false;
            b_start.BackColor = Color.Lime;
            b_start.Text = "Start";
            vm.Reset();
        }

        public static void Step()
        {
            for (int i = 0; i < DoNotDestroyMeList.Count; i++)
                DoNotDestroyMeList[i].timer_Tick(null, null);
        }

        public static void StepGame()
        {
            for (int i = 0; i < DoNotDestroyMeList.Count; i++)
                DoNotDestroyMeList[i].timer_Tick(null, null, true);
        }

        private void timer_Tick(object sender, EventArgs e, bool game = false)
        {
            if (!IsRunning)
                return;
            if (game != cb_GameTicks.Checked)
                return;

            if (Main.Ticks - LastTick < nv_UpdateFreq.Value)
                return;
            LastTick = Main.Ticks;

            double[] a = null;
            try
            {
                a = vm.InvokeFunction("GetPoints", null) as double[];
            }
            catch (Exception ex)
            {
                Stop();
                MessageBox.Show(ex.Message);
                return;
            }
            for (int i = 0; i < Math.Min(a.Length, chart.Series.Count); i++)
            {
                chart.Series[i].Points.AddXY(CurX, a[i]);
                if (chart.Series[i].Points.Count > nv_ValuesCount.Value)
                    chart.Series[i].Points.RemoveAt(0);
            }
            chart.ChartAreas[0].AxisX.Minimum = CurX - (double)nv_ValuesCount.Value;
            chart.ChartAreas[0].AxisX.Maximum = CurX;
            //chart.ChartAreas[0].AxisY.
            CurX++;
        }

        private void tb_code_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsRunning)
                Stop();
        }

        private void b_Clear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chart.Series.Count; i++)
                chart.Series[i].Points.Clear();
        }

        private void ChartDebug_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoNotDestroyMeList.Remove(this);
        }

        private void nv_ymax_ValueChanged(object sender, EventArgs e)
        {
            nv_ymin.Maximum = (decimal)((double)nv_ymax.Value - 1);
            nv_ymax.Minimum = (decimal)((double)nv_ymin.Value + 1);

            chart.ChartAreas[0].AxisY.Minimum = (int)nv_ymin.Value;
            chart.ChartAreas[0].AxisY.Maximum = (int)nv_ymax.Value;
        }

        private void nv_s5w_ValueChanged(object sender, EventArgs e)
        {
            int i = (sender as NumericUpDown).Name[4] - '1';
            chart.Series[i].BorderWidth = (int)(sender as NumericUpDown).Value;
        }

        private void b_s5c_Click(object sender, EventArgs e)
        {
            int i = (sender as Button).Name[3] - '1';
            colorDialog.Color = (sender as Button).BackColor;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                (sender as Button).BackColor = colorDialog.Color;
                chart.Series[i].Color = colorDialog.Color;
            }
        }
    }
}
