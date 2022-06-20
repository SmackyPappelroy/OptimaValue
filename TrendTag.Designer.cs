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
            this.lblStartTime = new System.Windows.Forms.Label();
            this.lblStopTime = new System.Windows.Forms.Label();
            this.lblTimeSpan = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.txtTimeSpan = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPlay = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.txtMin = new System.Windows.Forms.TextBox();
            this.txtSetpoint = new System.Windows.Forms.TextBox();
            this.txtMax = new System.Windows.Forms.TextBox();
            this.txtStartDate = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.topPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.topPanel, 3);
            this.topPanel.Controls.Add(this.chart);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.Location = new System.Drawing.Point(4, 80);
            this.topPanel.Margin = new System.Windows.Forms.Padding(4);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1056, 368);
            this.topPanel.TabIndex = 0;
            // 
            // chart
            // 
            this.chart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.Location = new System.Drawing.Point(0, 0);
            this.chart.Margin = new System.Windows.Forms.Padding(4);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(1056, 368);
            this.chart.TabIndex = 600;
            this.chart.Text = "cartesianChart1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.topPanel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblStartTime, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblStopTime, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblTimeSpan, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtStartDate, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtStatus, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1064, 532);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lblStartTime
            // 
            this.lblStartTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.ForeColor = System.Drawing.Color.White;
            this.lblStartTime.Location = new System.Drawing.Point(4, 499);
            this.lblStartTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(50, 18);
            this.lblStartTime.TabIndex = 2;
            this.lblStartTime.Text = "label2";
            // 
            // lblStopTime
            // 
            this.lblStopTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblStopTime.AutoSize = true;
            this.lblStopTime.ForeColor = System.Drawing.Color.White;
            this.lblStopTime.Location = new System.Drawing.Point(1011, 499);
            this.lblStopTime.Name = "lblStopTime";
            this.lblStopTime.Size = new System.Drawing.Size(50, 18);
            this.lblStopTime.TabIndex = 3;
            this.lblStopTime.Text = "label2";
            // 
            // lblTimeSpan
            // 
            this.lblTimeSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimeSpan.AutoSize = true;
            this.lblTimeSpan.ForeColor = System.Drawing.Color.White;
            this.lblTimeSpan.Location = new System.Drawing.Point(358, 499);
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(358, 3);
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
            this.tableLayoutPanel3.Controls.Add(this.trackBar, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(712, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(349, 42);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // btnPlay
            // 
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.ImageIndex = 0;
            this.btnPlay.ImageList = this.imageList1;
            this.btnPlay.Location = new System.Drawing.Point(3, 3);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
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
            this.imageList1.Images.SetKeyName(0, "stop_squared_64px.png");
            this.imageList1.Images.SetKeyName(1, "Play Button Circled_64px.png");
            // 
            // trackBar
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.trackBar, 2);
            this.trackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar.Location = new System.Drawing.Point(119, 3);
            this.trackBar.Maximum = 100;
            this.trackBar.Minimum = 70;
            this.trackBar.Name = "trackBar";
            this.trackBar.Size = new System.Drawing.Size(227, 45);
            this.trackBar.TabIndex = 6;
            this.trackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar.Value = 100;
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
            this.tableLayoutPanel4.Size = new System.Drawing.Size(349, 42);
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
            this.txtSetpoint.Size = new System.Drawing.Size(111, 26);
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
            // txtStartDate
            // 
            this.txtStartDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStartDate.Location = new System.Drawing.Point(3, 455);
            this.txtStartDate.Name = "txtStartDate";
            this.txtStartDate.PlaceholderText = "StartDatum";
            this.txtStartDate.Size = new System.Drawing.Size(349, 26);
            this.txtStartDate.TabIndex = 10;
            this.txtStartDate.Visible = false;
            // 
            // txtStatus
            // 
            this.txtStatus.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.txtStatus, 3);
            this.txtStatus.ForeColor = System.Drawing.Color.White;
            this.txtStatus.Location = new System.Drawing.Point(5, 53);
            this.txtStatus.Margin = new System.Windows.Forms.Padding(5);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(50, 18);
            this.txtStatus.TabIndex = 11;
            this.txtStatus.Text = "label2";
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
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
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
        public System.Windows.Forms.Label lblStartTime;
        public System.Windows.Forms.Label lblStopTime;
        private System.Windows.Forms.ImageList imageList1;
        internal System.Windows.Forms.Button btnPlay;
        internal LiveCharts.WinForms.CartesianChart chart;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        internal System.Windows.Forms.TextBox txtSetpoint;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        internal System.Windows.Forms.TextBox txtMin;
        internal System.Windows.Forms.TextBox txtMax;
        internal System.Windows.Forms.TextBox txtStartDate;
        internal System.Windows.Forms.TrackBar trackBar;
        internal System.Windows.Forms.Label txtStatus;
    }
}