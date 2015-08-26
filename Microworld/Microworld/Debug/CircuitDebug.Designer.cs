namespace MicroWorld
{
    partial class CircuitDebug
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.lv = new System.Windows.Forms.ListView();
            this.ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Voltage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Sending = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.ch)).BeginInit();
            this.SuspendLayout();
            // 
            // lv
            // 
            this.lv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ID,
            this.Voltage,
            this.Sending});
            this.lv.GridLines = true;
            this.lv.Location = new System.Drawing.Point(1102, 567);
            this.lv.Name = "lv";
            this.lv.Size = new System.Drawing.Size(57, 59);
            this.lv.TabIndex = 0;
            this.lv.UseCompatibleStateImageBehavior = false;
            this.lv.View = System.Windows.Forms.View.Details;
            this.lv.Visible = false;
            // 
            // ID
            // 
            this.ID.Text = "ID";
            // 
            // Voltage
            // 
            this.Voltage.Text = "Voltage";
            this.Voltage.Width = 103;
            // 
            // Sending
            // 
            this.Sending.Text = "Sending";
            this.Sending.Width = 117;
            // 
            // ch
            // 
            chartArea1.Name = "ChartArea1";
            this.ch.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.ch.Legends.Add(legend1);
            this.ch.Location = new System.Drawing.Point(592, 12);
            this.ch.Name = "ch";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.ch.Series.Add(series1);
            this.ch.Size = new System.Drawing.Size(633, 300);
            this.ch.TabIndex = 1;
            this.ch.Text = "chart1";
            // 
            // CircuitDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1237, 736);
            this.Controls.Add(this.ch);
            this.Controls.Add(this.lv);
            this.DoubleBuffered = true;
            this.Name = "CircuitDebug";
            this.Text = "CircuitDebug";
            this.Load += new System.EventHandler(this.CircuitDebug_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CircuitDebug_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CircuitDebug_MouseClick);
            ((System.ComponentModel.ISupportInitialize)(this.ch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListView lv;
        private System.Windows.Forms.ColumnHeader ID;
        private System.Windows.Forms.ColumnHeader Voltage;
        private System.Windows.Forms.ColumnHeader Sending;
        private System.Windows.Forms.DataVisualization.Charting.Chart ch;
    }
}