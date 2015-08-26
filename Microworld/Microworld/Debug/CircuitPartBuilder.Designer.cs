namespace MicroWorld.Debug
{
    partial class CircuitPartBuilder
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lb_parts = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lb_iterations = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.b_refresh = new System.Windows.Forms.Button();
            this.b_nextIt = new System.Windows.Forms.Button();
            this.b_prevIt = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lb_parts);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(93, 283);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PartInd";
            // 
            // lb_parts
            // 
            this.lb_parts.FormattingEnabled = true;
            this.lb_parts.ItemHeight = 16;
            this.lb_parts.Location = new System.Drawing.Point(7, 22);
            this.lb_parts.Name = "lb_parts";
            this.lb_parts.Size = new System.Drawing.Size(79, 244);
            this.lb_parts.TabIndex = 0;
            this.lb_parts.SelectedIndexChanged += new System.EventHandler(this.lb_parts_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lb_iterations);
            this.groupBox2.Location = new System.Drawing.Point(113, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(98, 283);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Iteration";
            // 
            // lb_iterations
            // 
            this.lb_iterations.FormattingEnabled = true;
            this.lb_iterations.ItemHeight = 16;
            this.lb_iterations.Location = new System.Drawing.Point(6, 22);
            this.lb_iterations.Name = "lb_iterations";
            this.lb_iterations.Size = new System.Drawing.Size(84, 244);
            this.lb_iterations.TabIndex = 0;
            this.lb_iterations.SelectedIndexChanged += new System.EventHandler(this.lb_iterations_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(217, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 284);
            this.panel1.TabIndex = 2;
            this.panel1.Visible = false;
            // 
            // b_refresh
            // 
            this.b_refresh.Location = new System.Drawing.Point(13, 303);
            this.b_refresh.Name = "b_refresh";
            this.b_refresh.Size = new System.Drawing.Size(75, 23);
            this.b_refresh.TabIndex = 3;
            this.b_refresh.Text = "ReLoad";
            this.b_refresh.UseVisualStyleBackColor = true;
            this.b_refresh.Click += new System.EventHandler(this.b_refresh_Click);
            // 
            // b_nextIt
            // 
            this.b_nextIt.Location = new System.Drawing.Point(517, 303);
            this.b_nextIt.Name = "b_nextIt";
            this.b_nextIt.Size = new System.Drawing.Size(75, 23);
            this.b_nextIt.TabIndex = 4;
            this.b_nextIt.Text = ">>";
            this.b_nextIt.UseVisualStyleBackColor = true;
            this.b_nextIt.Click += new System.EventHandler(this.b_nextIt_Click);
            // 
            // b_prevIt
            // 
            this.b_prevIt.Location = new System.Drawing.Point(436, 303);
            this.b_prevIt.Name = "b_prevIt";
            this.b_prevIt.Size = new System.Drawing.Size(75, 23);
            this.b_prevIt.TabIndex = 5;
            this.b_prevIt.Text = "<<";
            this.b_prevIt.UseVisualStyleBackColor = true;
            this.b_prevIt.Click += new System.EventHandler(this.b_prevIt_Click);
            // 
            // CircuitPartBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 335);
            this.Controls.Add(this.b_prevIt);
            this.Controls.Add(this.b_nextIt);
            this.Controls.Add(this.b_refresh);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Name = "CircuitPartBuilder";
            this.Text = "CircuitPartBuilder";
            this.Load += new System.EventHandler(this.CircuitPartBuilder_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CircuitPartBuilder_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CircuitPartBuilder_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CircuitPartBuilder_MouseUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lb_parts;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lb_iterations;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button b_refresh;
        private System.Windows.Forms.Button b_nextIt;
        private System.Windows.Forms.Button b_prevIt;
    }
}