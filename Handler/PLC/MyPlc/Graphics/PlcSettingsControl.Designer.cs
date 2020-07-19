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
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblName.Location = new System.Drawing.Point(57, 65);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(66, 23);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Namn";
            // 
            // lblIpAddress
            // 
            this.lblIpAddress.AutoSize = true;
            this.lblIpAddress.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIpAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblIpAddress.Location = new System.Drawing.Point(57, 131);
            this.lblIpAddress.Name = "lblIpAddress";
            this.lblIpAddress.Size = new System.Drawing.Size(94, 23);
            this.lblIpAddress.TabIndex = 1;
            this.lblIpAddress.Text = "IP-adress";
            // 
            // lblCpu
            // 
            this.lblCpu.AutoSize = true;
            this.lblCpu.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCpu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblCpu.Location = new System.Drawing.Point(249, 194);
            this.lblCpu.Name = "lblCpu";
            this.lblCpu.Size = new System.Drawing.Size(87, 23);
            this.lblCpu.TabIndex = 2;
            this.lblCpu.Text = "CPU-typ";
            // 
            // lblRack
            // 
            this.lblRack.AutoSize = true;
            this.lblRack.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblRack.Location = new System.Drawing.Point(57, 194);
            this.lblRack.Name = "lblRack";
            this.lblRack.Size = new System.Drawing.Size(57, 23);
            this.lblRack.TabIndex = 3;
            this.lblRack.Text = "Rack";
            // 
            // lblSlot
            // 
            this.lblSlot.AutoSize = true;
            this.lblSlot.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSlot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblSlot.Location = new System.Drawing.Point(57, 254);
            this.lblSlot.Name = "lblSlot";
            this.lblSlot.Size = new System.Drawing.Size(43, 23);
            this.lblSlot.TabIndex = 4;
            this.lblSlot.Text = "Slot";
            // 
            // txtName
            // 
            this.txtName.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.txtName.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(61, 91);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(249, 27);
            this.txtName.TabIndex = 5;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // txtIp
            // 
            this.txtIp.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIp.Location = new System.Drawing.Point(61, 157);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(181, 27);
            this.txtIp.TabIndex = 6;
            this.txtIp.Validating += new System.ComponentModel.CancelEventHandler(this.txtIp_Validating);
            // 
            // comboCpu
            // 
            this.comboCpu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCpu.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboCpu.FormattingEnabled = true;
            this.comboCpu.Items.AddRange(new object[] {
            "S7200",
            "Logo0BA8",
            "S7300",
            "S7400",
            "S71200",
            "S71500"});
            this.comboCpu.Location = new System.Drawing.Point(253, 220);
            this.comboCpu.Name = "comboCpu";
            this.comboCpu.Size = new System.Drawing.Size(121, 29);
            this.comboCpu.TabIndex = 7;
            this.comboCpu.Validating += new System.ComponentModel.CancelEventHandler(this.comboCpu_Validating);
            // 
            // txtRack
            // 
            this.txtRack.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRack.Location = new System.Drawing.Point(61, 220);
            this.txtRack.Name = "txtRack";
            this.txtRack.Size = new System.Drawing.Size(70, 27);
            this.txtRack.TabIndex = 8;
            this.txtRack.Validating += new System.ComponentModel.CancelEventHandler(this.txtRack_Validating);
            // 
            // txtSlot
            // 
            this.txtSlot.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSlot.Location = new System.Drawing.Point(61, 280);
            this.txtSlot.Name = "txtSlot";
            this.txtSlot.Size = new System.Drawing.Size(70, 27);
            this.txtSlot.TabIndex = 9;
            this.txtSlot.Validating += new System.ComponentModel.CancelEventHandler(this.txtSlot_Validating);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.btnSave.Location = new System.Drawing.Point(234, 332);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(140, 68);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "SPARA";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // checkActive
            // 
            this.checkActive.AutoSize = true;
            this.checkActive.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.checkActive.Location = new System.Drawing.Point(61, 22);
            this.checkActive.Name = "checkActive";
            this.checkActive.Size = new System.Drawing.Size(76, 27);
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
            this.btnDelete.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.btnDelete.Location = new System.Drawing.Point(61, 332);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(140, 68);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "TA BORT";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // PlcSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.checkActive);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtSlot);
            this.Controls.Add(this.txtRack);
            this.Controls.Add(this.comboCpu);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblSlot);
            this.Controls.Add(this.lblRack);
            this.Controls.Add(this.lblCpu);
            this.Controls.Add(this.lblIpAddress);
            this.Controls.Add(this.lblName);
            this.Name = "PlcSettingsControl";
            this.Size = new System.Drawing.Size(444, 461);
            this.Load += new System.EventHandler(this.PlcSettingsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}
