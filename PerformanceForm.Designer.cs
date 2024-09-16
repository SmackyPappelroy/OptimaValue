namespace OptimaValue
{
    partial class PerformanceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerformanceForm));
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            txtThread = new System.Windows.Forms.Label();
            txtRam = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            txtCpu = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            txtSqlWrite = new System.Windows.Forms.Label();
            txtSqlRead = new System.Windows.Forms.Label();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            lblMinCycle = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            lblLastCycle = new System.Windows.Forms.Label();
            lblAvgCycle = new System.Windows.Forms.Label();
            lblMaxCycle = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
            cpuCartesianChart = new LiveCharts.WinForms.CartesianChart();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(txtThread, 0, 5);
            tableLayoutPanel1.Controls.Add(txtRam, 0, 3);
            tableLayoutPanel1.Controls.Add(label4, 0, 0);
            tableLayoutPanel1.Controls.Add(txtCpu, 0, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Controls.Add(label1, 0, 4);
            tableLayoutPanel1.Controls.Add(label10, 0, 6);
            tableLayoutPanel1.Controls.Add(label11, 0, 8);
            tableLayoutPanel1.Controls.Add(txtSqlWrite, 0, 7);
            tableLayoutPanel1.Controls.Add(txtSqlRead, 0, 9);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 12;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(239, 639);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // txtThread
            // 
            txtThread.Anchor = System.Windows.Forms.AnchorStyles.Top;
            txtThread.AutoSize = true;
            txtThread.Font = new System.Drawing.Font("Century Gothic", 18F);
            txtThread.ForeColor = System.Drawing.Color.White;
            txtThread.Location = new System.Drawing.Point(106, 281);
            txtThread.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtThread.Name = "txtThread";
            txtThread.Size = new System.Drawing.Size(26, 30);
            txtThread.TabIndex = 27;
            txtThread.Text = "0";
            // 
            // txtRam
            // 
            txtRam.Anchor = System.Windows.Forms.AnchorStyles.Top;
            txtRam.AutoSize = true;
            txtRam.Font = new System.Drawing.Font("Century Gothic", 18F);
            txtRam.ForeColor = System.Drawing.Color.White;
            txtRam.Location = new System.Drawing.Point(63, 205);
            txtRam.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtRam.MaximumSize = new System.Drawing.Size(502, 0);
            txtRam.Name = "txtRam";
            txtRam.Padding = new System.Windows.Forms.Padding(9, 9, 0, 0);
            txtRam.Size = new System.Drawing.Size(112, 38);
            txtRam.TabIndex = 24;
            txtRam.Text = "0,00 MB";
            txtRam.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Century Gothic", 12F);
            label4.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label4.Location = new System.Drawing.Point(54, 108);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(130, 21);
            label4.TabIndex = 22;
            label4.Text = "CPU-belastning";
            label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // txtCpu
            // 
            txtCpu.Anchor = System.Windows.Forms.AnchorStyles.Top;
            txtCpu.AutoSize = true;
            txtCpu.Font = new System.Drawing.Font("Century Gothic", 18F);
            txtCpu.ForeColor = System.Drawing.Color.White;
            txtCpu.Location = new System.Drawing.Point(72, 129);
            txtCpu.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtCpu.MaximumSize = new System.Drawing.Size(502, 0);
            txtCpu.Name = "txtCpu";
            txtCpu.Padding = new System.Windows.Forms.Padding(9, 9, 0, 0);
            txtCpu.Size = new System.Drawing.Size(94, 38);
            txtCpu.TabIndex = 23;
            txtCpu.Text = "0,00 %";
            txtCpu.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            txtCpu.Leave += txtCpu_Leave;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Century Gothic", 12F);
            label2.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label2.Location = new System.Drawing.Point(69, 184);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(101, 21);
            label2.TabIndex = 25;
            label2.Text = "RAM-minne";
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Century Gothic", 12F);
            label1.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label1.Location = new System.Drawing.Point(49, 260);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(140, 21);
            label1.TabIndex = 26;
            label1.Text = "Använda trådar";
            // 
            // label10
            // 
            label10.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Century Gothic", 12F);
            label10.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label10.Location = new System.Drawing.Point(26, 336);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(186, 21);
            label10.TabIndex = 28;
            label10.Text = "Skrivningar till SQL / sek";
            // 
            // label11
            // 
            label11.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Century Gothic", 12F);
            label11.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label11.Location = new System.Drawing.Point(24, 412);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(191, 21);
            label11.TabIndex = 29;
            label11.Text = "Läsningar från SQL / sek";
            // 
            // txtSqlWrite
            // 
            txtSqlWrite.Anchor = System.Windows.Forms.AnchorStyles.Top;
            txtSqlWrite.AutoSize = true;
            txtSqlWrite.Font = new System.Drawing.Font("Century Gothic", 18F);
            txtSqlWrite.ForeColor = System.Drawing.Color.White;
            txtSqlWrite.Location = new System.Drawing.Point(106, 357);
            txtSqlWrite.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtSqlWrite.Name = "txtSqlWrite";
            txtSqlWrite.Size = new System.Drawing.Size(26, 30);
            txtSqlWrite.TabIndex = 30;
            txtSqlWrite.Text = "0";
            // 
            // txtSqlRead
            // 
            txtSqlRead.Anchor = System.Windows.Forms.AnchorStyles.Top;
            txtSqlRead.AutoSize = true;
            txtSqlRead.Font = new System.Drawing.Font("Century Gothic", 18F);
            txtSqlRead.ForeColor = System.Drawing.Color.White;
            txtSqlRead.Location = new System.Drawing.Point(106, 433);
            txtSqlRead.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtSqlRead.Name = "txtSqlRead";
            txtSqlRead.Size = new System.Drawing.Size(26, 30);
            txtSqlRead.TabIndex = 31;
            txtSqlRead.Text = "0";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(lblMinCycle, 0, 7);
            tableLayoutPanel2.Controls.Add(label3, 0, 0);
            tableLayoutPanel2.Controls.Add(label5, 0, 2);
            tableLayoutPanel2.Controls.Add(label6, 0, 4);
            tableLayoutPanel2.Controls.Add(label7, 0, 6);
            tableLayoutPanel2.Controls.Add(lblLastCycle, 0, 1);
            tableLayoutPanel2.Controls.Add(lblAvgCycle, 0, 3);
            tableLayoutPanel2.Controls.Add(lblMaxCycle, 0, 5);
            tableLayoutPanel2.Controls.Add(label8, 0, 8);
            tableLayoutPanel2.Controls.Add(label9, 0, 10);
            tableLayoutPanel2.Controls.Add(cartesianChart1, 0, 9);
            tableLayoutPanel2.Controls.Add(cpuCartesianChart, 0, 11);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(239, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 12;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.41219139F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.412193F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.412193F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.412193F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.412193F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.412193F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.412193F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.412192F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.403533F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.3806725F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.403533F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.51472F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel2.Size = new System.Drawing.Size(664, 639);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // lblMinCycle
            // 
            lblMinCycle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            lblMinCycle.AutoSize = true;
            lblMinCycle.Font = new System.Drawing.Font("Century Gothic", 18F);
            lblMinCycle.ForeColor = System.Drawing.Color.White;
            lblMinCycle.Location = new System.Drawing.Point(287, 238);
            lblMinCycle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMinCycle.MaximumSize = new System.Drawing.Size(502, 0);
            lblMinCycle.Name = "lblMinCycle";
            lblMinCycle.Padding = new System.Windows.Forms.Padding(9, 9, 0, 0);
            lblMinCycle.Size = new System.Drawing.Size(89, 34);
            lblMinCycle.TabIndex = 30;
            lblMinCycle.Text = "10 ms";
            lblMinCycle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Century Gothic", 12F);
            label3.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label3.Location = new System.Drawing.Point(275, 13);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(113, 21);
            label3.TabIndex = 23;
            label3.Text = "Förra cykeltid";
            label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Century Gothic", 12F);
            label5.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label5.Location = new System.Drawing.Point(270, 81);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(123, 21);
            label5.TabIndex = 24;
            label5.Text = "Medel cykeltid";
            label5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Century Gothic", 12F);
            label6.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label6.Location = new System.Drawing.Point(278, 149);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(108, 21);
            label6.TabIndex = 25;
            label6.Text = "Max cykeltid";
            label6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label7
            // 
            label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Century Gothic", 12F);
            label7.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label7.Location = new System.Drawing.Point(281, 217);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(102, 21);
            label7.TabIndex = 26;
            label7.Text = "Min cykeltid";
            label7.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lblLastCycle
            // 
            lblLastCycle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            lblLastCycle.AutoSize = true;
            lblLastCycle.Font = new System.Drawing.Font("Century Gothic", 18F);
            lblLastCycle.ForeColor = System.Drawing.Color.White;
            lblLastCycle.Location = new System.Drawing.Point(287, 34);
            lblLastCycle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblLastCycle.MaximumSize = new System.Drawing.Size(502, 0);
            lblLastCycle.Name = "lblLastCycle";
            lblLastCycle.Padding = new System.Windows.Forms.Padding(9, 9, 0, 0);
            lblLastCycle.Size = new System.Drawing.Size(89, 34);
            lblLastCycle.TabIndex = 27;
            lblLastCycle.Text = "10 ms";
            lblLastCycle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblAvgCycle
            // 
            lblAvgCycle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            lblAvgCycle.AutoSize = true;
            lblAvgCycle.Font = new System.Drawing.Font("Century Gothic", 18F);
            lblAvgCycle.ForeColor = System.Drawing.Color.White;
            lblAvgCycle.Location = new System.Drawing.Point(287, 102);
            lblAvgCycle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAvgCycle.MaximumSize = new System.Drawing.Size(502, 0);
            lblAvgCycle.Name = "lblAvgCycle";
            lblAvgCycle.Padding = new System.Windows.Forms.Padding(9, 9, 0, 0);
            lblAvgCycle.Size = new System.Drawing.Size(89, 34);
            lblAvgCycle.TabIndex = 28;
            lblAvgCycle.Text = "10 ms";
            lblAvgCycle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblMaxCycle
            // 
            lblMaxCycle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            lblMaxCycle.AutoSize = true;
            lblMaxCycle.Font = new System.Drawing.Font("Century Gothic", 18F);
            lblMaxCycle.ForeColor = System.Drawing.Color.White;
            lblMaxCycle.Location = new System.Drawing.Point(287, 170);
            lblMaxCycle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMaxCycle.MaximumSize = new System.Drawing.Size(502, 0);
            lblMaxCycle.Name = "lblMaxCycle";
            lblMaxCycle.Padding = new System.Windows.Forms.Padding(9, 9, 0, 0);
            lblMaxCycle.Size = new System.Drawing.Size(89, 34);
            lblMaxCycle.TabIndex = 29;
            lblMaxCycle.Text = "10 ms";
            lblMaxCycle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label8
            // 
            label8.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Century Gothic", 12F);
            label8.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label8.Location = new System.Drawing.Point(277, 285);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(110, 21);
            label8.TabIndex = 31;
            label8.Text = "Cykeltid (ms)";
            label8.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label9
            // 
            label9.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("Century Gothic", 12F);
            label9.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            label9.Location = new System.Drawing.Point(245, 468);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(173, 21);
            label9.TabIndex = 32;
            label9.Text = "CPU-användning (%)";
            label9.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // cartesianChart1
            // 
            cartesianChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            cartesianChart1.Location = new System.Drawing.Point(3, 309);
            cartesianChart1.Name = "cartesianChart1";
            cartesianChart1.Size = new System.Drawing.Size(658, 143);
            cartesianChart1.TabIndex = 33;
            cartesianChart1.Text = "cartesianChart1";
            // 
            // cpuCartesianChart
            // 
            cpuCartesianChart.Dock = System.Windows.Forms.DockStyle.Fill;
            cpuCartesianChart.Location = new System.Drawing.Point(3, 492);
            cpuCartesianChart.Name = "cpuCartesianChart";
            cpuCartesianChart.Size = new System.Drawing.Size(658, 144);
            cpuCartesianChart.TabIndex = 34;
            cpuCartesianChart.Text = "cartesianChart2";
            // 
            // PerformanceForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(38, 38, 38);
            ClientSize = new System.Drawing.Size(903, 639);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PerformanceForm";
            Opacity = 0.9D;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "PerformanceForm";
            TopMost = true;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label txtCpu;
        private System.Windows.Forms.Label txtRam;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label txtThread;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblLastCycle;
        private System.Windows.Forms.Label lblMinCycle;
        private System.Windows.Forms.Label lblAvgCycle;
        private System.Windows.Forms.Label lblMaxCycle;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private LiveCharts.WinForms.CartesianChart cartesianChart1;
        private LiveCharts.WinForms.CartesianChart cpuCartesianChart;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label txtSqlWrite;
        private System.Windows.Forms.Label txtSqlRead;
    }
}