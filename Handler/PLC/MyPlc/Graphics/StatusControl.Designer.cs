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
            this.panel1 = new System.Windows.Forms.Panel();
            this.errorImage = new System.Windows.Forms.PictureBox();
            this.txtStatus = new System.Windows.Forms.Label();
            this.lblPlcStatus = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtPlc = new System.Windows.Forms.Label();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.txtUpTime = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorImage)).BeginInit();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.panel1.Controls.Add(this.errorImage);
            this.panel1.Controls.Add(this.txtStatus);
            this.panel1.Controls.Add(this.lblPlcStatus);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 182);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(658, 522);
            this.panel1.TabIndex = 12;
            // 
            // errorImage
            // 
            this.errorImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.errorImage.BackColor = System.Drawing.Color.Transparent;
            this.errorImage.Image = ((System.Drawing.Image)(resources.GetObject("errorImage.Image")));
            this.errorImage.Location = new System.Drawing.Point(610, 5);
            this.errorImage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.errorImage.Name = "errorImage";
            this.errorImage.Size = new System.Drawing.Size(48, 49);
            this.errorImage.TabIndex = 15;
            this.errorImage.TabStop = false;
            this.errorImage.Visible = false;
            // 
            // txtStatus
            // 
            this.txtStatus.AutoSize = true;
            this.txtStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtStatus.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.txtStatus.Location = new System.Drawing.Point(0, 51);
            this.txtStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtStatus.MaximumSize = new System.Drawing.Size(555, 0);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Padding = new System.Windows.Forms.Padding(12, 12, 0, 0);
            this.txtStatus.Size = new System.Drawing.Size(94, 42);
            this.txtStatus.TabIndex = 10;
            this.txtStatus.Text = "Status";
            // 
            // lblPlcStatus
            // 
            this.lblPlcStatus.AutoSize = true;
            this.lblPlcStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPlcStatus.Font = new System.Drawing.Font("Century Gothic", 15.75F);
            this.lblPlcStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblPlcStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblPlcStatus.Location = new System.Drawing.Point(0, 0);
            this.lblPlcStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPlcStatus.Name = "lblPlcStatus";
            this.lblPlcStatus.Padding = new System.Windows.Forms.Padding(32, 12, 32, 0);
            this.lblPlcStatus.Size = new System.Drawing.Size(235, 51);
            this.lblPlcStatus.TabIndex = 9;
            this.lblPlcStatus.Text = "PLC status";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.panel2.Controls.Add(this.txtPlc);
            this.panel2.Controls.Add(this.panelStatus);
            this.panel2.Controls.Add(this.txtUpTime);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(4, 5);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(658, 167);
            this.panel2.TabIndex = 13;
            // 
            // txtPlc
            // 
            this.txtPlc.AutoSize = true;
            this.txtPlc.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtPlc.Font = new System.Drawing.Font("Century Gothic", 15.75F);
            this.txtPlc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.txtPlc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtPlc.Location = new System.Drawing.Point(15, 0);
            this.txtPlc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtPlc.Name = "txtPlc";
            this.txtPlc.Padding = new System.Windows.Forms.Padding(21, 12, 32, 0);
            this.txtPlc.Size = new System.Drawing.Size(130, 51);
            this.txtPlc.TabIndex = 15;
            this.txtPlc.Text = "PLC";
            // 
            // panelStatus
            // 
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelStatus.Location = new System.Drawing.Point(0, 0);
            this.panelStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(15, 167);
            this.panelStatus.TabIndex = 14;
            // 
            // txtUpTime
            // 
            this.txtUpTime.AutoSize = true;
            this.txtUpTime.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUpTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.txtUpTime.Location = new System.Drawing.Point(63, 75);
            this.txtUpTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtUpTime.Name = "txtUpTime";
            this.txtUpTime.Size = new System.Drawing.Size(122, 36);
            this.txtUpTime.TabIndex = 11;
            this.txtUpTime.Text = "UpTime";
            this.txtUpTime.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(666, 709);
            this.tableLayoutPanel1.TabIndex = 14;
            // 
            // StatusControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "StatusControl";
            this.Size = new System.Drawing.Size(666, 709);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorImage)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

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
    }
}
