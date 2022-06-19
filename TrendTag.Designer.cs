namespace OptimaValue
{
    partial class TrendTag
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrendTag));
            this.topPanel = new System.Windows.Forms.Panel();
            this.chart = new LiveCharts.WinForms.CartesianChart();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtStartTime = new System.Windows.Forms.Label();
            this.txtStopTime = new System.Windows.Forms.Label();
            this.lblTimeSpan = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.txtTimeSpan = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPlay = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.txtMin = new System.Windows.Forms.TextBox();
            this.txtSetpoint = new System.Windows.Forms.TextBox();
            this.txtMax = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.topPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.topPanel, 3);
            this.topPanel.Controls.Add(this.chart);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.Location = new System.Drawing.Point(4, 52);
            this.topPanel.Margin = new System.Windows.Forms.Padding(4);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1056, 428);
            this.topPanel.TabIndex = 0;
            // 
            // chart
            // 
            this.chart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.Location = new System.Drawing.Point(0, 0);
            this.chart.Margin = new System.Windows.Forms.Padding(4);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(1056, 428);
            this.chart.TabIndex = 600;
            this.chart.Text = "cartesianChart1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.topPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtStartTime, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtStopTime, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblTimeSpan, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1064, 532);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // txtStartTime
            // 
            this.txtStartTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtStartTime.AutoSize = true;
            this.txtStartTime.ForeColor = System.Drawing.Color.White;
            this.txtStartTime.Location = new System.Drawing.Point(4, 499);
            this.txtStartTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtStartTime.Name = "txtStartTime";
            this.txtStartTime.Size = new System.Drawing.Size(50, 18);
            this.txtStartTime.TabIndex = 2;
            this.txtStartTime.Text = "label2";
            // 
            // txtStopTime
            // 
            this.txtStopTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.txtStopTime.AutoSize = true;
            this.txtStopTime.ForeColor = System.Drawing.Color.White;
            this.txtStopTime.Location = new System.Drawing.Point(1011, 499);
            this.txtStopTime.Name = "txtStopTime";
            this.txtStopTime.Size = new System.Drawing.Size(50, 18);
            this.txtStopTime.TabIndex = 3;
            this.txtStopTime.Text = "label2";
            // 
            // lblTimeSpan
            // 
            this.lblTimeSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimeSpan.AutoSize = true;
            this.lblTimeSpan.ForeColor = System.Drawing.Color.White;
            this.lblTimeSpan.Location = new System.Drawing.Point(357, 499);
            this.lblTimeSpan.Name = "lblTimeSpan";
            this.lblTimeSpan.Size = new System.Drawing.Size(348, 18);
            this.lblTimeSpan.TabIndex = 4;
            this.lblTimeSpan.Text = "00:00:30";
            this.lblTimeSpan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.txtTimeSpan, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(357, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(348, 42);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // txtTimeSpan
            // 
            this.txtTimeSpan.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtTimeSpan.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtTimeSpan.Location = new System.Drawing.Point(178, 5);
            this.txtTimeSpan.Margin = new System.Windows.Forms.Padding(4);
            this.txtTimeSpan.Name = "txtTimeSpan";
            this.txtTimeSpan.Size = new System.Drawing.Size(127, 32);
            this.txtTimeSpan.TabIndex = 4;
            this.txtTimeSpan.Text = "1";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(83, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 19);
            this.label1.TabIndex = 500;
            this.label1.Text = "Tidspann:";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.btnPlay, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(711, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(350, 42);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.ImageIndex = 0;
            this.btnPlay.ImageList = this.imageList1;
            this.btnPlay.Location = new System.Drawing.Point(3, 3);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 36);
            this.btnPlay.TabIndex = 5;
            this.btnPlay.UseVisualStyleBackColor = true;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Stop Squared_64px.png");
            this.imageList1.Images.SetKeyName(1, "Play Button Circled_64px.png");
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Controls.Add(this.txtMin, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtSetpoint, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtMax, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(348, 42);
            this.tableLayoutPanel4.TabIndex = 8;
            // 
            // txtMin
            // 
            this.txtMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMin.Location = new System.Drawing.Point(3, 8);
            this.txtMin.Name = "txtMin";
            this.txtMin.PlaceholderText = "Min";
            this.txtMin.Size = new System.Drawing.Size(110, 26);
            this.txtMin.TabIndex = 1;
            // 
            // txtSetpoint
            // 
            this.txtSetpoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSetpoint.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtSetpoint.Location = new System.Drawing.Point(235, 8);
            this.txtSetpoint.Name = "txtSetpoint";
            this.txtSetpoint.PlaceholderText = "Setpoint";
            this.txtSetpoint.Size = new System.Drawing.Size(110, 26);
            this.txtSetpoint.TabIndex = 3;
            // 
            // txtMax
            // 
            this.txtMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMax.Location = new System.Drawing.Point(119, 8);
            this.txtMax.Name = "txtMax";
            this.txtMax.PlaceholderText = "Max";
            this.txtMax.Size = new System.Drawing.Size(110, 26);
            this.txtMax.TabIndex = 2;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // TrendTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 532);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TrendTag";
            this.Text = "jjhlkkjlhg";
            this.topPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox txtTimeSpan;
        private System.Windows.Forms.Label lblTimeSpan;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        public System.Windows.Forms.ErrorProvider errorProvider1;
        public System.Windows.Forms.Label txtStartTime;
        public System.Windows.Forms.Label txtStopTime;
        private System.Windows.Forms.ImageList imageList1;
        internal System.Windows.Forms.Button btnPlay;
        internal LiveCharts.WinForms.CartesianChart chart;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        internal System.Windows.Forms.TextBox txtSetpoint;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        internal System.Windows.Forms.TextBox txtMin;
        internal System.Windows.Forms.TextBox txtMax;
    }
}