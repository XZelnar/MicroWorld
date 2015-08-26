namespace MicroWorld.Debug
{
    partial class ChartDebug
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.b_start = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nv_UpdateFreq = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nv_ValuesCount = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nv_SeriesCount = new System.Windows.Forms.NumericUpDown();
            this.tb_code = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.b_Clear = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.nv_ymin = new System.Windows.Forms.NumericUpDown();
            this.nv_ymax = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.gb_s1 = new System.Windows.Forms.GroupBox();
            this.nv_s1w = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.b_s1c = new System.Windows.Forms.Button();
            this.gb_s2 = new System.Windows.Forms.GroupBox();
            this.nv_s2w = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.b_s2c = new System.Windows.Forms.Button();
            this.gb_s3 = new System.Windows.Forms.GroupBox();
            this.nv_s3w = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.b_s3c = new System.Windows.Forms.Button();
            this.gb_s4 = new System.Windows.Forms.GroupBox();
            this.nv_s4w = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.b_s4c = new System.Windows.Forms.Button();
            this.gb_s5 = new System.Windows.Forms.GroupBox();
            this.nv_s5w = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.b_s5c = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.cb_GameTicks = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nv_UpdateFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nv_ValuesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nv_SeriesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nv_ymin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nv_ymax)).BeginInit();
            this.gb_s1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nv_s1w)).BeginInit();
            this.gb_s2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nv_s2w)).BeginInit();
            this.gb_s3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nv_s3w)).BeginInit();
            this.gb_s4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nv_s4w)).BeginInit();
            this.gb_s5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nv_s5w)).BeginInit();
            this.SuspendLayout();
            // 
            // chart
            // 
            this.chart.BackColor = System.Drawing.Color.Black;
            chartArea1.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea1.AxisY.IsStartedFromZero = false;
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorTickMark.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.Maximum = 1D;
            chartArea1.AxisY.Minimum = -1D;
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.White;
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.BorderColor = System.Drawing.Color.White;
            chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Location = new System.Drawing.Point(13, 13);
            this.chart.Name = "chart";
            this.chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series1.BorderWidth = 4;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Lime;
            series1.Name = "Series1";
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(804, 233);
            this.chart.TabIndex = 0;
            this.chart.Text = "chart1";
            // 
            // b_start
            // 
            this.b_start.BackColor = System.Drawing.Color.Lime;
            this.b_start.Location = new System.Drawing.Point(742, 252);
            this.b_start.Name = "b_start";
            this.b_start.Size = new System.Drawing.Size(75, 23);
            this.b_start.TabIndex = 1;
            this.b_start.Text = "Start";
            this.b_start.UseVisualStyleBackColor = false;
            this.b_start.Click += new System.EventHandler(this.b_start_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 254);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Update frequency (ticks):";
            // 
            // nv_UpdateFreq
            // 
            this.nv_UpdateFreq.Location = new System.Drawing.Point(162, 252);
            this.nv_UpdateFreq.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.nv_UpdateFreq.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nv_UpdateFreq.Name = "nv_UpdateFreq";
            this.nv_UpdateFreq.Size = new System.Drawing.Size(36, 22);
            this.nv_UpdateFreq.TabIndex = 3;
            this.nv_UpdateFreq.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(204, 254);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Values count:";
            // 
            // nv_ValuesCount
            // 
            this.nv_ValuesCount.Location = new System.Drawing.Point(289, 252);
            this.nv_ValuesCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nv_ValuesCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nv_ValuesCount.Name = "nv_ValuesCount";
            this.nv_ValuesCount.Size = new System.Drawing.Size(46, 22);
            this.nv_ValuesCount.TabIndex = 5;
            this.nv_ValuesCount.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nv_ValuesCount.ValueChanged += new System.EventHandler(this.nv_ValuesCount_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(341, 254);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Series count:";
            // 
            // nv_SeriesCount
            // 
            this.nv_SeriesCount.Location = new System.Drawing.Point(423, 252);
            this.nv_SeriesCount.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nv_SeriesCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nv_SeriesCount.Name = "nv_SeriesCount";
            this.nv_SeriesCount.Size = new System.Drawing.Size(36, 22);
            this.nv_SeriesCount.TabIndex = 7;
            this.nv_SeriesCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nv_SeriesCount.ValueChanged += new System.EventHandler(this.nv_SeriesCount_ValueChanged);
            // 
            // tb_code
            // 
            this.tb_code.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_code.Location = new System.Drawing.Point(12, 281);
            this.tb_code.Multiline = true;
            this.tb_code.Name = "tb_code";
            this.tb_code.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_code.Size = new System.Drawing.Size(805, 216);
            this.tb_code.TabIndex = 9;
            this.tb_code.Text = "function double[] GetPoints()\r\n{\r\n  double[] r = new double[1];\r\n  r[0] = System." +
    "Math.Sin((double)MicroWorld.Main.Ticks / 56);\r\n  return r;\r\n}";
            this.tb_code.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_code_KeyDown);
            // 
            // timer
            // 
            this.timer.Interval = 3;
            // 
            // b_Clear
            // 
            this.b_Clear.Location = new System.Drawing.Point(661, 252);
            this.b_Clear.Name = "b_Clear";
            this.b_Clear.Size = new System.Drawing.Size(75, 23);
            this.b_Clear.TabIndex = 11;
            this.b_Clear.Text = "Clear";
            this.b_Clear.UseVisualStyleBackColor = true;
            this.b_Clear.Click += new System.EventHandler(this.b_Clear_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(823, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "Ymin";
            // 
            // nv_ymin
            // 
            this.nv_ymin.Location = new System.Drawing.Point(871, 40);
            this.nv_ymin.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nv_ymin.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nv_ymin.Name = "nv_ymin";
            this.nv_ymin.Size = new System.Drawing.Size(75, 22);
            this.nv_ymin.TabIndex = 13;
            this.nv_ymin.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.nv_ymin.ValueChanged += new System.EventHandler(this.nv_ymax_ValueChanged);
            // 
            // nv_ymax
            // 
            this.nv_ymax.Location = new System.Drawing.Point(871, 12);
            this.nv_ymax.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nv_ymax.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.nv_ymax.Name = "nv_ymax";
            this.nv_ymax.Size = new System.Drawing.Size(75, 22);
            this.nv_ymax.TabIndex = 14;
            this.nv_ymax.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nv_ymax.ValueChanged += new System.EventHandler(this.nv_ymax_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(823, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 17);
            this.label5.TabIndex = 15;
            this.label5.Text = "Ymax";
            // 
            // gb_s1
            // 
            this.gb_s1.Controls.Add(this.nv_s1w);
            this.gb_s1.Controls.Add(this.label6);
            this.gb_s1.Controls.Add(this.b_s1c);
            this.gb_s1.Location = new System.Drawing.Point(826, 68);
            this.gb_s1.Name = "gb_s1";
            this.gb_s1.Size = new System.Drawing.Size(120, 81);
            this.gb_s1.TabIndex = 16;
            this.gb_s1.TabStop = false;
            this.gb_s1.Text = "Series1";
            // 
            // nv_s1w
            // 
            this.nv_s1w.Location = new System.Drawing.Point(57, 52);
            this.nv_s1w.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nv_s1w.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nv_s1w.Name = "nv_s1w";
            this.nv_s1w.Size = new System.Drawing.Size(57, 22);
            this.nv_s1w.TabIndex = 2;
            this.nv_s1w.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nv_s1w.ValueChanged += new System.EventHandler(this.nv_s5w_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "Width";
            // 
            // b_s1c
            // 
            this.b_s1c.BackColor = System.Drawing.Color.Lime;
            this.b_s1c.Location = new System.Drawing.Point(7, 22);
            this.b_s1c.Name = "b_s1c";
            this.b_s1c.Size = new System.Drawing.Size(107, 23);
            this.b_s1c.TabIndex = 0;
            this.b_s1c.Text = "Color";
            this.b_s1c.UseVisualStyleBackColor = false;
            this.b_s1c.Click += new System.EventHandler(this.b_s5c_Click);
            // 
            // gb_s2
            // 
            this.gb_s2.Controls.Add(this.nv_s2w);
            this.gb_s2.Controls.Add(this.label7);
            this.gb_s2.Controls.Add(this.b_s2c);
            this.gb_s2.Location = new System.Drawing.Point(826, 155);
            this.gb_s2.Name = "gb_s2";
            this.gb_s2.Size = new System.Drawing.Size(120, 81);
            this.gb_s2.TabIndex = 17;
            this.gb_s2.TabStop = false;
            this.gb_s2.Text = "Series2";
            this.gb_s2.Visible = false;
            // 
            // nv_s2w
            // 
            this.nv_s2w.Location = new System.Drawing.Point(57, 52);
            this.nv_s2w.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nv_s2w.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nv_s2w.Name = "nv_s2w";
            this.nv_s2w.Size = new System.Drawing.Size(57, 22);
            this.nv_s2w.TabIndex = 2;
            this.nv_s2w.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nv_s2w.ValueChanged += new System.EventHandler(this.nv_s5w_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 17);
            this.label7.TabIndex = 1;
            this.label7.Text = "Width";
            // 
            // b_s2c
            // 
            this.b_s2c.BackColor = System.Drawing.Color.Lime;
            this.b_s2c.Location = new System.Drawing.Point(7, 22);
            this.b_s2c.Name = "b_s2c";
            this.b_s2c.Size = new System.Drawing.Size(107, 23);
            this.b_s2c.TabIndex = 0;
            this.b_s2c.Text = "Color";
            this.b_s2c.UseVisualStyleBackColor = false;
            this.b_s2c.Click += new System.EventHandler(this.b_s5c_Click);
            // 
            // gb_s3
            // 
            this.gb_s3.Controls.Add(this.nv_s3w);
            this.gb_s3.Controls.Add(this.label8);
            this.gb_s3.Controls.Add(this.b_s3c);
            this.gb_s3.Location = new System.Drawing.Point(826, 242);
            this.gb_s3.Name = "gb_s3";
            this.gb_s3.Size = new System.Drawing.Size(120, 81);
            this.gb_s3.TabIndex = 17;
            this.gb_s3.TabStop = false;
            this.gb_s3.Text = "Series3";
            this.gb_s3.Visible = false;
            // 
            // nv_s3w
            // 
            this.nv_s3w.Location = new System.Drawing.Point(57, 52);
            this.nv_s3w.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nv_s3w.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nv_s3w.Name = "nv_s3w";
            this.nv_s3w.Size = new System.Drawing.Size(57, 22);
            this.nv_s3w.TabIndex = 2;
            this.nv_s3w.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nv_s3w.ValueChanged += new System.EventHandler(this.nv_s5w_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 17);
            this.label8.TabIndex = 1;
            this.label8.Text = "Width";
            // 
            // b_s3c
            // 
            this.b_s3c.BackColor = System.Drawing.Color.Lime;
            this.b_s3c.Location = new System.Drawing.Point(7, 22);
            this.b_s3c.Name = "b_s3c";
            this.b_s3c.Size = new System.Drawing.Size(107, 23);
            this.b_s3c.TabIndex = 0;
            this.b_s3c.Text = "Color";
            this.b_s3c.UseVisualStyleBackColor = false;
            this.b_s3c.Click += new System.EventHandler(this.b_s5c_Click);
            // 
            // gb_s4
            // 
            this.gb_s4.Controls.Add(this.nv_s4w);
            this.gb_s4.Controls.Add(this.label9);
            this.gb_s4.Controls.Add(this.b_s4c);
            this.gb_s4.Location = new System.Drawing.Point(826, 329);
            this.gb_s4.Name = "gb_s4";
            this.gb_s4.Size = new System.Drawing.Size(120, 81);
            this.gb_s4.TabIndex = 17;
            this.gb_s4.TabStop = false;
            this.gb_s4.Text = "Series4";
            this.gb_s4.Visible = false;
            // 
            // nv_s4w
            // 
            this.nv_s4w.Location = new System.Drawing.Point(57, 52);
            this.nv_s4w.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nv_s4w.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nv_s4w.Name = "nv_s4w";
            this.nv_s4w.Size = new System.Drawing.Size(57, 22);
            this.nv_s4w.TabIndex = 2;
            this.nv_s4w.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nv_s4w.ValueChanged += new System.EventHandler(this.nv_s5w_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 17);
            this.label9.TabIndex = 1;
            this.label9.Text = "Width";
            // 
            // b_s4c
            // 
            this.b_s4c.BackColor = System.Drawing.Color.Lime;
            this.b_s4c.Location = new System.Drawing.Point(7, 22);
            this.b_s4c.Name = "b_s4c";
            this.b_s4c.Size = new System.Drawing.Size(107, 23);
            this.b_s4c.TabIndex = 0;
            this.b_s4c.Text = "Color";
            this.b_s4c.UseVisualStyleBackColor = false;
            this.b_s4c.Click += new System.EventHandler(this.b_s5c_Click);
            // 
            // gb_s5
            // 
            this.gb_s5.Controls.Add(this.nv_s5w);
            this.gb_s5.Controls.Add(this.label10);
            this.gb_s5.Controls.Add(this.b_s5c);
            this.gb_s5.Location = new System.Drawing.Point(826, 416);
            this.gb_s5.Name = "gb_s5";
            this.gb_s5.Size = new System.Drawing.Size(120, 81);
            this.gb_s5.TabIndex = 17;
            this.gb_s5.TabStop = false;
            this.gb_s5.Text = "Series5";
            this.gb_s5.Visible = false;
            // 
            // nv_s5w
            // 
            this.nv_s5w.Location = new System.Drawing.Point(57, 52);
            this.nv_s5w.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nv_s5w.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nv_s5w.Name = "nv_s5w";
            this.nv_s5w.Size = new System.Drawing.Size(57, 22);
            this.nv_s5w.TabIndex = 2;
            this.nv_s5w.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nv_s5w.ValueChanged += new System.EventHandler(this.nv_s5w_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 17);
            this.label10.TabIndex = 1;
            this.label10.Text = "Width";
            // 
            // b_s5c
            // 
            this.b_s5c.BackColor = System.Drawing.Color.Lime;
            this.b_s5c.Location = new System.Drawing.Point(7, 22);
            this.b_s5c.Name = "b_s5c";
            this.b_s5c.Size = new System.Drawing.Size(107, 23);
            this.b_s5c.TabIndex = 0;
            this.b_s5c.Text = "Color";
            this.b_s5c.UseVisualStyleBackColor = false;
            this.b_s5c.Click += new System.EventHandler(this.b_s5c_Click);
            // 
            // cb_GameTicks
            // 
            this.cb_GameTicks.AutoSize = true;
            this.cb_GameTicks.Checked = true;
            this.cb_GameTicks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_GameTicks.Location = new System.Drawing.Point(465, 253);
            this.cb_GameTicks.Name = "cb_GameTicks";
            this.cb_GameTicks.Size = new System.Drawing.Size(100, 21);
            this.cb_GameTicks.TabIndex = 18;
            this.cb_GameTicks.Text = "Game ticks";
            this.cb_GameTicks.UseVisualStyleBackColor = true;
            // 
            // ChartDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 509);
            this.Controls.Add(this.cb_GameTicks);
            this.Controls.Add(this.gb_s5);
            this.Controls.Add(this.gb_s4);
            this.Controls.Add(this.gb_s3);
            this.Controls.Add(this.gb_s2);
            this.Controls.Add(this.gb_s1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nv_ymax);
            this.Controls.Add(this.nv_ymin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.b_Clear);
            this.Controls.Add(this.tb_code);
            this.Controls.Add(this.nv_SeriesCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nv_ValuesCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nv_UpdateFreq);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.b_start);
            this.Controls.Add(this.chart);
            this.Name = "ChartDebug";
            this.Text = "ChartDebug";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChartDebug_FormClosing);
            this.Load += new System.EventHandler(this.ChartDebug_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nv_UpdateFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nv_ValuesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nv_SeriesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nv_ymin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nv_ymax)).EndInit();
            this.gb_s1.ResumeLayout(false);
            this.gb_s1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nv_s1w)).EndInit();
            this.gb_s2.ResumeLayout(false);
            this.gb_s2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nv_s2w)).EndInit();
            this.gb_s3.ResumeLayout(false);
            this.gb_s3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nv_s3w)).EndInit();
            this.gb_s4.ResumeLayout(false);
            this.gb_s4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nv_s4w)).EndInit();
            this.gb_s5.ResumeLayout(false);
            this.gb_s5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nv_s5w)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.Button b_start;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nv_UpdateFreq;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nv_ValuesCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nv_SeriesCount;
        private System.Windows.Forms.TextBox tb_code;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button b_Clear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nv_ymin;
        private System.Windows.Forms.NumericUpDown nv_ymax;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox gb_s1;
        private System.Windows.Forms.NumericUpDown nv_s1w;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button b_s1c;
        private System.Windows.Forms.GroupBox gb_s2;
        private System.Windows.Forms.NumericUpDown nv_s2w;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button b_s2c;
        private System.Windows.Forms.GroupBox gb_s3;
        private System.Windows.Forms.NumericUpDown nv_s3w;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button b_s3c;
        private System.Windows.Forms.GroupBox gb_s4;
        private System.Windows.Forms.NumericUpDown nv_s4w;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button b_s4c;
        private System.Windows.Forms.GroupBox gb_s5;
        private System.Windows.Forms.NumericUpDown nv_s5w;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button b_s5c;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.CheckBox cb_GameTicks;
    }
}