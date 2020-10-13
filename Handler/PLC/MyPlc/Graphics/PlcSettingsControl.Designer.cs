namespace OptimaValue.Handler.PLC.Graphics
{
    partial class PlcSettingsControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlcSettingsControl));
            this.lblName = new System.Windows.Forms.Label();
            this.lblIpAddress = new System.Windows.Forms.Label();
            this.lblCpu = new System.Windows.Forms.Label();
            this.lblRack = new System.Windows.Forms.Label();
            this.lblSlot = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.comboCpu = new System.Windows.Forms.ComboBox();
            this.txtRack = new System.Windows.Forms.TextBox();
            this.txtSlot = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.checkActive = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnDelete = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSyncTime = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblName.Location = new System.Drawing.Point(49, 86);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.lblName.Size = new System.Drawing.Size(114, 48);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Namn";
            // 
            // lblIpAddress
            // 
            this.lblIpAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblIpAddress.AutoSize = true;
            this.lblIpAddress.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIpAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblIpAddress.Location = new System.Drawing.Point(49, 220);
            this.lblIpAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIpAddress.Name = "lblIpAddress";
            this.lblIpAddress.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.lblIpAddress.Size = new System.Drawing.Size(152, 48);
            this.lblIpAddress.TabIndex = 1;
            this.lblIpAddress.Text = "IP-adress";
            // 
            // lblCpu
            // 
            this.lblCpu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCpu.AutoSize = true;
            this.lblCpu.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCpu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblCpu.Location = new System.Drawing.Point(337, 86);
            this.lblCpu.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCpu.Name = "lblCpu";
            this.lblCpu.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.lblCpu.Size = new System.Drawing.Size(143, 48);
            this.lblCpu.TabIndex = 2;
            this.lblCpu.Text = "CPU-typ";
            // 
            // lblRack
            // 
            this.lblRack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRack.AutoSize = true;
            this.lblRack.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblRack.Location = new System.Drawing.Point(49, 354);
            this.lblRack.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRack.Name = "lblRack";
            this.lblRack.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.lblRack.Size = new System.Drawing.Size(99, 48);
            this.lblRack.TabIndex = 3;
            this.lblRack.Text = "Rack";
            // 
            // lblSlot
            // 
            this.lblSlot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSlot.AutoSize = true;
            this.lblSlot.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSlot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblSlot.Location = new System.Drawing.Point(337, 354);
            this.lblSlot.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSlot.Name = "lblSlot";
            this.lblSlot.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.lblSlot.Size = new System.Drawing.Size(76, 48);
            this.lblSlot.TabIndex = 4;
            this.lblSlot.Text = "Slot";
            // 
            // txtName
            // 
            this.txtName.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.txtName.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(51, 140);
            this.txtName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(274, 37);
            this.txtName.TabIndex = 5;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // txtIp
            // 
            this.txtIp.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIp.Location = new System.Drawing.Point(51, 274);
            this.txtIp.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(270, 37);
            this.txtIp.TabIndex = 6;
            this.txtIp.Validating += new System.ComponentModel.CancelEventHandler(this.txtIp_Validating);
            // 
            // comboCpu
            // 
            this.comboCpu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCpu.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboCpu.FormattingEnabled = true;
            this.comboCpu.Items.AddRange(new object[] {
            "S7200",
            "Logo0BA8",
            "S7300",
            "S7400",
            "S71200",
            "S71500"});
            this.comboCpu.Location = new System.Drawing.Point(339, 140);
            this.comboCpu.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboCpu.Name = "comboCpu";
            this.comboCpu.Size = new System.Drawing.Size(180, 36);
            this.comboCpu.TabIndex = 7;
            this.comboCpu.Validating += new System.ComponentModel.CancelEventHandler(this.comboCpu_Validating);
            // 
            // txtRack
            // 
            this.txtRack.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRack.Location = new System.Drawing.Point(51, 408);
            this.txtRack.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtRack.Name = "txtRack";
            this.txtRack.Size = new System.Drawing.Size(103, 37);
            this.txtRack.TabIndex = 8;
            this.txtRack.Validating += new System.ComponentModel.CancelEventHandler(this.txtRack_Validating);
            // 
            // txtSlot
            // 
            this.txtSlot.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSlot.Location = new System.Drawing.Point(339, 408);
            this.txtSlot.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtSlot.Name = "txtSlot";
            this.txtSlot.Size = new System.Drawing.Size(103, 37);
            this.txtSlot.TabIndex = 9;
            this.txtSlot.Validating += new System.ComponentModel.CancelEventHandler(this.txtSlot_Validating);
            // 
            // btnSave
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.btnSave, 2);
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(337, 541);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.tableLayoutPanel2.SetRowSpan(this.btnSave, 3);
            this.btnSave.Size = new System.Drawing.Size(325, 163);
            this.btnSave.TabIndex = 10;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // checkActive
            // 
            this.checkActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkActive.AutoSize = true;
            this.checkActive.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.checkActive.Location = new System.Drawing.Point(49, 22);
            this.checkActive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkActive.Name = "checkActive";
            this.checkActive.Size = new System.Drawing.Size(111, 40);
            this.checkActive.TabIndex = 11;
            this.checkActive.Text = "Aktiv";
            this.checkActive.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // btnDelete
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.btnDelete, 2);
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.Location = new System.Drawing.Point(4, 541);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDelete.Name = "btnDelete";
            this.tableLayoutPanel2.SetRowSpan(this.btnDelete, 3);
            this.btnDelete.Size = new System.Drawing.Size(325, 163);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel2.Controls.Add(this.btnSyncTime, 2, 7);
            this.tableLayoutPanel2.Controls.Add(this.btnSave, 2, 8);
            this.tableLayoutPanel2.Controls.Add(this.btnDelete, 0, 8);
            this.tableLayoutPanel2.Controls.Add(this.checkActive, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblName, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.comboCpu, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblCpu, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtName, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtRack, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.lblIpAddress, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.txtIp, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.lblRack, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.lblSlot, 2, 5);
            this.tableLayoutPanel2.Controls.Add(this.txtSlot, 2, 6);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 10;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(666, 709);
            this.tableLayoutPanel2.TabIndex = 14;
            // 
            // btnSyncTime
            // 
            this.btnSyncTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSyncTime.FlatAppearance.BorderSize = 0;
            this.btnSyncTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSyncTime.Font = new System.Drawing.Font("Century Gothic", 14.25F);
            this.btnSyncTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.btnSyncTime.Location = new System.Drawing.Point(336, 472);
            this.btnSyncTime.Name = "btnSyncTime";
            this.btnSyncTime.Size = new System.Drawing.Size(282, 61);
            this.btnSyncTime.TabIndex = 15;
            this.btnSyncTime.Text = "Synka PLC-klocka";
            this.btnSyncTime.UseVisualStyleBackColor = true;
            this.btnSyncTime.Click += new System.EventHandler(this.btnSyncTime_Click);
            // 
            // PlcSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.Controls.Add(this.tableLayoutPanel2);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PlcSettingsControl";
            this.Size = new System.Drawing.Size(666, 709);
            this.Load += new System.EventHandler(this.PlcSettingsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblIpAddress;
        private System.Windows.Forms.Label lblCpu;
        private System.Windows.Forms.Label lblRack;
        private System.Windows.Forms.Label lblSlot;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.ComboBox comboCpu;
        private System.Windows.Forms.TextBox txtRack;
        private System.Windows.Forms.TextBox txtSlot;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.CheckBox checkActive;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnSyncTime;
    }
}
