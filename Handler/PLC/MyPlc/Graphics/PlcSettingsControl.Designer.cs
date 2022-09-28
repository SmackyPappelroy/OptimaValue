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
            this.imageTest = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.comboOpcTopic = new System.Windows.Forms.ComboBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageTest)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblName.Location = new System.Drawing.Point(38, 86);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.lblName.Size = new System.Drawing.Size(76, 30);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Namn";
            // 
            // lblIpAddress
            // 
            this.lblIpAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblIpAddress.AutoSize = true;
            this.lblIpAddress.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblIpAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblIpAddress.Location = new System.Drawing.Point(38, 182);
            this.lblIpAddress.Name = "lblIpAddress";
            this.lblIpAddress.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.lblIpAddress.Size = new System.Drawing.Size(100, 30);
            this.lblIpAddress.TabIndex = 1;
            this.lblIpAddress.Text = "IP-adress";
            // 
            // lblCpu
            // 
            this.lblCpu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCpu.AutoSize = true;
            this.lblCpu.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCpu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblCpu.Location = new System.Drawing.Point(262, 86);
            this.lblCpu.Name = "lblCpu";
            this.lblCpu.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.lblCpu.Size = new System.Drawing.Size(95, 30);
            this.lblCpu.TabIndex = 2;
            this.lblCpu.Text = "CPU-typ";
            // 
            // lblRack
            // 
            this.lblRack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRack.AutoSize = true;
            this.lblRack.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblRack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblRack.Location = new System.Drawing.Point(38, 278);
            this.lblRack.Name = "lblRack";
            this.lblRack.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.lblRack.Size = new System.Drawing.Size(67, 30);
            this.lblRack.TabIndex = 3;
            this.lblRack.Text = "Rack";
            // 
            // lblSlot
            // 
            this.lblSlot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSlot.AutoSize = true;
            this.lblSlot.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSlot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.lblSlot.Location = new System.Drawing.Point(262, 278);
            this.lblSlot.Name = "lblSlot";
            this.lblSlot.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.lblSlot.Size = new System.Drawing.Size(51, 30);
            this.lblSlot.TabIndex = 4;
            this.lblSlot.Text = "Slot";
            // 
            // txtName
            // 
            this.txtName.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.txtName.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtName.Location = new System.Drawing.Point(40, 120);
            this.txtName.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(214, 27);
            this.txtName.TabIndex = 5;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // txtIp
            // 
            this.txtIp.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtIp.Location = new System.Drawing.Point(40, 216);
            this.txtIp.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(211, 27);
            this.txtIp.TabIndex = 6;
            this.txtIp.Validating += new System.ComponentModel.CancelEventHandler(this.txtIp_Validating);
            // 
            // comboCpu
            // 
            this.comboCpu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCpu.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.comboCpu.FormattingEnabled = true;
            this.comboCpu.Items.AddRange(new object[] {
            "S7200",
            "Logo0BA8",
            "S7300",
            "S7400",
            "S71200",
            "S71500",
            "OPC"});
            this.comboCpu.Location = new System.Drawing.Point(264, 120);
            this.comboCpu.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.comboCpu.Name = "comboCpu";
            this.comboCpu.Size = new System.Drawing.Size(141, 27);
            this.comboCpu.TabIndex = 7;
            this.comboCpu.SelectedValueChanged += new System.EventHandler(this.comboCpu_SelectedValueChanged);
            this.comboCpu.Validating += new System.ComponentModel.CancelEventHandler(this.comboCpu_Validating);
            // 
            // txtRack
            // 
            this.txtRack.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtRack.Location = new System.Drawing.Point(40, 312);
            this.txtRack.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtRack.Name = "txtRack";
            this.txtRack.Size = new System.Drawing.Size(81, 27);
            this.txtRack.TabIndex = 8;
            this.txtRack.Validating += new System.ComponentModel.CancelEventHandler(this.txtRack_Validating);
            // 
            // txtSlot
            // 
            this.txtSlot.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtSlot.Location = new System.Drawing.Point(264, 312);
            this.txtSlot.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtSlot.Name = "txtSlot";
            this.txtSlot.Size = new System.Drawing.Size(81, 27);
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
            this.btnSave.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(262, 408);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.tableLayoutPanel2.SetRowSpan(this.btnSave, 3);
            this.btnSave.Size = new System.Drawing.Size(253, 120);
            this.btnSave.TabIndex = 10;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // checkActive
            // 
            this.checkActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkActive.AutoSize = true;
            this.checkActive.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.checkActive.Location = new System.Drawing.Point(38, 38);
            this.checkActive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkActive.Name = "checkActive";
            this.checkActive.Size = new System.Drawing.Size(77, 26);
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
            this.btnDelete.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.Location = new System.Drawing.Point(3, 408);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDelete.Name = "btnDelete";
            this.tableLayoutPanel2.SetRowSpan(this.btnDelete, 3);
            this.btnDelete.Size = new System.Drawing.Size(253, 120);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
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
            this.tableLayoutPanel2.Controls.Add(this.imageTest, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 10;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.46154F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.615385F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.615385F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.615385F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.615385F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.615385F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.615385F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.615385F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.615385F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.615385F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(518, 532);
            this.tableLayoutPanel2.TabIndex = 14;
            // 
            // btnSyncTime
            // 
            this.btnSyncTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSyncTime.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.btnSyncTime.FlatAppearance.BorderSize = 0;
            this.btnSyncTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSyncTime.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSyncTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.btnSyncTime.Location = new System.Drawing.Point(261, 358);
            this.btnSyncTime.Margin = new System.Windows.Forms.Padding(2);
            this.btnSyncTime.Name = "btnSyncTime";
            this.btnSyncTime.Size = new System.Drawing.Size(220, 44);
            this.btnSyncTime.TabIndex = 15;
            this.btnSyncTime.Text = "Synka PLC-klocka";
            this.btnSyncTime.UseVisualStyleBackColor = true;
            this.btnSyncTime.Click += new System.EventHandler(this.btnSyncTime_Click);
            // 
            // imageTest
            // 
            this.imageTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.imageTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageTest.Location = new System.Drawing.Point(486, 3);
            this.imageTest.Name = "imageTest";
            this.imageTest.Size = new System.Drawing.Size(29, 62);
            this.imageTest.TabIndex = 17;
            this.imageTest.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnConnect, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboOpcTopic, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(262, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(218, 62);
            this.tableLayoutPanel1.TabIndex = 18;
            // 
            // btnConnect
            // 
            this.btnConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnConnect.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnConnect.Location = new System.Drawing.Point(0, 0);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(0);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(218, 31);
            this.btnConnect.TabIndex = 16;
            this.btnConnect.Text = "Testa anslutning";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // comboOpcTopic
            // 
            this.comboOpcTopic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboOpcTopic.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.comboOpcTopic.FormattingEnabled = true;
            this.comboOpcTopic.Location = new System.Drawing.Point(3, 34);
            this.comboOpcTopic.Name = "comboOpcTopic";
            this.comboOpcTopic.Size = new System.Drawing.Size(212, 24);
            this.comboOpcTopic.TabIndex = 17;
            this.comboOpcTopic.Visible = false;
            this.comboOpcTopic.SelectedIndexChanged += new System.EventHandler(this.comboOpcTopic_SelectedIndexChanged);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "disconnected_90px.png");
            this.imageList.Images.SetKeyName(1, "disconnected_90px_red.png");
            this.imageList.Images.SetKeyName(2, "connected_90px.png");
            // 
            // PlcSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.Controls.Add(this.tableLayoutPanel2);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PlcSettingsControl";
            this.Size = new System.Drawing.Size(518, 532);
            this.Load += new System.EventHandler(this.PlcSettingsControl_Load);
            this.Leave += new System.EventHandler(this.PlcSettingsControl_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageTest)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
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
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.PictureBox imageTest;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox comboOpcTopic;
    }
}
