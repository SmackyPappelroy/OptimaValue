namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    partial class StatusControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusControl));
            panel1 = new System.Windows.Forms.Panel();
            errorImage = new System.Windows.Forms.PictureBox();
            txtStatus = new System.Windows.Forms.Label();
            lblPlcStatus = new System.Windows.Forms.Label();
            panel2 = new System.Windows.Forms.Panel();
            txtMisc = new System.Windows.Forms.Label();
            txtPlc = new System.Windows.Forms.Label();
            panelStatus = new System.Windows.Forms.Panel();
            txtUpTime = new System.Windows.Forms.Label();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorImage).BeginInit();
            panel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.FromArgb(67, 62, 71);
            panel1.Controls.Add(errorImage);
            panel1.Controls.Add(txtStatus);
            panel1.Controls.Add(lblPlcStatus);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(3, 163);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(512, 365);
            panel1.TabIndex = 12;
            // 
            // errorImage
            // 
            errorImage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            errorImage.BackColor = System.Drawing.Color.Transparent;
            errorImage.Image = (System.Drawing.Image)resources.GetObject("errorImage.Image");
            errorImage.Location = new System.Drawing.Point(474, 4);
            errorImage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            errorImage.Name = "errorImage";
            errorImage.Size = new System.Drawing.Size(37, 37);
            errorImage.TabIndex = 15;
            errorImage.TabStop = false;
            errorImage.Visible = false;
            // 
            // txtStatus
            // 
            txtStatus.AutoSize = true;
            txtStatus.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtStatus.ForeColor = System.Drawing.Color.FromArgb(230, 230, 230);
            txtStatus.Location = new System.Drawing.Point(0, 33);
            txtStatus.MaximumSize = new System.Drawing.Size(432, 0);
            txtStatus.Name = "txtStatus";
            txtStatus.Padding = new System.Windows.Forms.Padding(9, 9, 0, 0);
            txtStatus.Size = new System.Drawing.Size(68, 30);
            txtStatus.TabIndex = 10;
            txtStatus.Text = "Status";
            // 
            // lblPlcStatus
            // 
            lblPlcStatus.AutoSize = true;
            lblPlcStatus.Font = new System.Drawing.Font("Century Gothic", 15.75F);
            lblPlcStatus.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            lblPlcStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            lblPlcStatus.Location = new System.Drawing.Point(0, 0);
            lblPlcStatus.Name = "lblPlcStatus";
            lblPlcStatus.Padding = new System.Windows.Forms.Padding(25, 9, 25, 0);
            lblPlcStatus.Size = new System.Drawing.Size(164, 33);
            lblPlcStatus.TabIndex = 9;
            lblPlcStatus.Text = "PLC Status";
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.Color.FromArgb(67, 62, 71);
            panel2.Controls.Add(txtMisc);
            panel2.Controls.Add(txtPlc);
            panel2.Controls.Add(panelStatus);
            panel2.Controls.Add(txtUpTime);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(3, 4);
            panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(512, 151);
            panel2.TabIndex = 13;
            // 
            // txtMisc
            // 
            txtMisc.AutoSize = true;
            txtMisc.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtMisc.ForeColor = System.Drawing.Color.FromArgb(230, 230, 230);
            txtMisc.Location = new System.Drawing.Point(49, 53);
            txtMisc.Name = "txtMisc";
            txtMisc.Size = new System.Drawing.Size(44, 21);
            txtMisc.TabIndex = 16;
            txtMisc.Text = "Misc";
            txtMisc.Visible = false;
            // 
            // txtPlc
            // 
            txtPlc.AutoSize = true;
            txtPlc.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtPlc.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            txtPlc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            txtPlc.Location = new System.Drawing.Point(12, 0);
            txtPlc.Name = "txtPlc";
            txtPlc.Size = new System.Drawing.Size(49, 24);
            txtPlc.TabIndex = 15;
            txtPlc.Text = "PLC";
            // 
            // panelStatus
            // 
            panelStatus.Dock = System.Windows.Forms.DockStyle.Left;
            panelStatus.Location = new System.Drawing.Point(0, 0);
            panelStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panelStatus.Name = "panelStatus";
            panelStatus.Size = new System.Drawing.Size(12, 151);
            panelStatus.TabIndex = 14;
            // 
            // txtUpTime
            // 
            txtUpTime.AutoSize = true;
            txtUpTime.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtUpTime.ForeColor = System.Drawing.Color.FromArgb(230, 230, 230);
            txtUpTime.Location = new System.Drawing.Point(49, 30);
            txtUpTime.Name = "txtUpTime";
            txtUpTime.Size = new System.Drawing.Size(68, 21);
            txtUpTime.TabIndex = 11;
            txtUpTime.Text = "UpTime";
            txtUpTime.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel2, 0, 0);
            tableLayoutPanel1.Controls.Add(panel1, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            tableLayoutPanel1.Size = new System.Drawing.Size(518, 532);
            tableLayoutPanel1.TabIndex = 14;
            // 
            // StatusControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(38, 38, 38);
            Controls.Add(tableLayoutPanel1);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "StatusControl";
            Size = new System.Drawing.Size(518, 532);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorImage).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label txtUpTime;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label txtStatus;
        private System.Windows.Forms.Label lblPlcStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox errorImage;
        private System.Windows.Forms.Label txtPlc;
        private System.Windows.Forms.Label txtFailedConnectAttempts;
        private System.Windows.Forms.Label txtMisc;
        private System.Windows.Forms.Label txtReconnectTime;
    }
}
