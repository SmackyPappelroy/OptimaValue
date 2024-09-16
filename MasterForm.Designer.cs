namespace OptimaValue
{
    partial class MasterForm
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
            components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Optima");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MasterForm));
            addPlcMenu = new System.Windows.Forms.ContextMenuStrip(components);
            addPlc = new System.Windows.Forms.ToolStripMenuItem();
            treeView = new System.Windows.Forms.TreeView();
            imageList = new System.Windows.Forms.ImageList(components);
            contentPanel = new System.Windows.Forms.Panel();
            statusPanel = new System.Windows.Forms.Panel();
            serviceImage = new System.Windows.Forms.PictureBox();
            databaseImage = new System.Windows.Forms.PictureBox();
            errorImage = new System.Windows.Forms.PictureBox();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            txtStatus = new System.Windows.Forms.Label();
            lblStatus = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            btnStart = new System.Windows.Forms.Button();
            btnStop = new System.Windows.Forms.Button();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            menuStrip = new System.Windows.Forms.MenuStrip();
            menuSettings = new System.Windows.Forms.ToolStripMenuItem();
            databasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            debugMenu = new System.Windows.Forms.ToolStripMenuItem();
            notifyMenu = new System.Windows.Forms.ToolStripMenuItem();
            autoStartTool = new System.Windows.Forms.ToolStripMenuItem();
            serviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            menuQuestion = new System.Windows.Forms.ToolStripMenuItem();
            comboTrend = new System.Windows.Forms.ToolStripComboBox();
            btnStartTrend = new System.Windows.Forms.ToolStripMenuItem();
            notifyIcon = new System.Windows.Forms.NotifyIcon(components);
            databaseImageList = new System.Windows.Forms.ImageList(components);
            toolTip = new System.Windows.Forms.ToolTip(components);
            addPlcMenu.SuspendLayout();
            statusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)serviceImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)databaseImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // addPlcMenu
            // 
            addPlcMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            addPlcMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { addPlc });
            addPlcMenu.Name = "addPlcMenu";
            addPlcMenu.Size = new System.Drawing.Size(141, 26);
            // 
            // addPlc
            // 
            addPlc.Name = "addPlc";
            addPlc.Size = new System.Drawing.Size(140, 22);
            addPlc.Text = "Lägg till PLC";
            addPlc.Click += addPlc_Click;
            // 
            // treeView
            // 
            treeView.BackColor = System.Drawing.Color.WhiteSmoke;
            treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            treeView.Font = new System.Drawing.Font("Arial", 12F);
            treeView.ForeColor = System.Drawing.Color.Black;
            treeView.ImageIndex = 0;
            treeView.ImageList = imageList;
            treeView.Location = new System.Drawing.Point(4, 105);
            treeView.Margin = new System.Windows.Forms.Padding(4);
            treeView.Name = "treeView";
            treeNode1.ContextMenuStrip = addPlcMenu;
            treeNode1.Name = "Optima";
            treeNode1.Text = "Optima";
            treeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { treeNode1 });
            treeView.SelectedImageIndex = 0;
            treeView.Size = new System.Drawing.Size(342, 567);
            treeView.TabIndex = 0;
            treeView.MouseDoubleClick += treeView_MouseDoubleClick;
            // 
            // imageList
            // 
            imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            imageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList.ImageStream");
            imageList.TransparentColor = System.Drawing.Color.Transparent;
            imageList.Images.SetKeyName(0, "optima");
            imageList.Images.SetKeyName(1, "stop_circled_16px.png");
            imageList.Images.SetKeyName(2, "play");
            imageList.Images.SetKeyName(3, "warning");
            imageList.Images.SetKeyName(4, "job_16px.png");
            imageList.Images.SetKeyName(5, "status");
            imageList.Images.SetKeyName(6, "tags_16px.png");
            imageList.Images.SetKeyName(7, "icons8_gas_running_16.png");
            imageList.Images.SetKeyName(8, "cipStation.png");
            // 
            // contentPanel
            // 
            contentPanel.BackColor = System.Drawing.Color.FromArgb(38, 38, 38);
            contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            contentPanel.Location = new System.Drawing.Point(4, 240);
            contentPanel.Margin = new System.Windows.Forms.Padding(4);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new System.Drawing.Size(852, 431);
            contentPanel.TabIndex = 1;
            // 
            // statusPanel
            // 
            statusPanel.BackColor = System.Drawing.Color.FromArgb(38, 38, 38);
            statusPanel.Controls.Add(serviceImage);
            statusPanel.Controls.Add(databaseImage);
            statusPanel.Controls.Add(errorImage);
            statusPanel.Controls.Add(pictureBox1);
            statusPanel.Controls.Add(txtStatus);
            statusPanel.Controls.Add(lblStatus);
            statusPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            statusPanel.Location = new System.Drawing.Point(4, 4);
            statusPanel.Margin = new System.Windows.Forms.Padding(4);
            statusPanel.Name = "statusPanel";
            statusPanel.Size = new System.Drawing.Size(852, 228);
            statusPanel.TabIndex = 2;
            // 
            // serviceImage
            // 
            serviceImage.BackColor = System.Drawing.Color.Transparent;
            serviceImage.Image = (System.Drawing.Image)resources.GetObject("serviceImage.Image");
            serviceImage.Location = new System.Drawing.Point(805, 76);
            serviceImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            serviceImage.Name = "serviceImage";
            serviceImage.Size = new System.Drawing.Size(48, 48);
            serviceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            serviceImage.TabIndex = 16;
            serviceImage.TabStop = false;
            toolTip.SetToolTip(serviceImage, "Service installerad");
            // 
            // databaseImage
            // 
            databaseImage.BackColor = System.Drawing.Color.Transparent;
            databaseImage.Location = new System.Drawing.Point(821, 35);
            databaseImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            databaseImage.Name = "databaseImage";
            databaseImage.Size = new System.Drawing.Size(28, 28);
            databaseImage.TabIndex = 15;
            databaseImage.TabStop = false;
            // 
            // errorImage
            // 
            errorImage.BackColor = System.Drawing.Color.Transparent;
            errorImage.Image = (System.Drawing.Image)resources.GetObject("errorImage.Image");
            errorImage.Location = new System.Drawing.Point(778, 4);
            errorImage.Margin = new System.Windows.Forms.Padding(4);
            errorImage.Name = "errorImage";
            errorImage.Size = new System.Drawing.Size(38, 37);
            errorImage.TabIndex = 14;
            errorImage.TabStop = false;
            errorImage.Visible = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new System.Drawing.Point(826, 7);
            pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(18, 19);
            pictureBox1.TabIndex = 13;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // txtStatus
            // 
            txtStatus.AutoSize = true;
            txtStatus.Dock = System.Windows.Forms.DockStyle.Top;
            txtStatus.Font = new System.Drawing.Font("Century Gothic", 12F);
            txtStatus.ForeColor = System.Drawing.Color.FromArgb(230, 230, 230);
            txtStatus.Location = new System.Drawing.Point(0, 28);
            txtStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtStatus.MaximumSize = new System.Drawing.Size(501, 0);
            txtStatus.Name = "txtStatus";
            txtStatus.Padding = new System.Windows.Forms.Padding(10, 9, 0, 0);
            txtStatus.Size = new System.Drawing.Size(69, 30);
            txtStatus.TabIndex = 12;
            txtStatus.Text = "Status";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Dock = System.Windows.Forms.DockStyle.Top;
            lblStatus.Font = new System.Drawing.Font("Century Gothic", 15.75F);
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            lblStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            lblStatus.Location = new System.Drawing.Point(0, 0);
            lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new System.Windows.Forms.Padding(24, 4, 24, 0);
            lblStatus.Size = new System.Drawing.Size(117, 28);
            lblStatus.TabIndex = 11;
            lblStatus.Text = "Status";
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.FromArgb(38, 38, 38);
            panel1.Controls.Add(btnStart);
            panel1.Controls.Add(btnStop);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(4, 4);
            panel1.Margin = new System.Windows.Forms.Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(342, 93);
            panel1.TabIndex = 3;
            // 
            // btnStart
            // 
            btnStart.BackColor = System.Drawing.Color.FromArgb(38, 38, 38);
            btnStart.BackgroundImage = (System.Drawing.Image)resources.GetObject("btnStart.BackgroundImage");
            btnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            btnStart.Dock = System.Windows.Forms.DockStyle.Left;
            btnStart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(38, 38, 38);
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnStart.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold);
            btnStart.Location = new System.Drawing.Point(0, 0);
            btnStart.Margin = new System.Windows.Forms.Padding(4);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(109, 93);
            btnStart.TabIndex = 9;
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.BackgroundImage = (System.Drawing.Image)resources.GetObject("btnStop.BackgroundImage");
            btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            btnStop.Dock = System.Windows.Forms.DockStyle.Right;
            btnStop.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(38, 38, 38);
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnStop.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold);
            btnStop.Location = new System.Drawing.Point(220, 0);
            btnStop.Margin = new System.Windows.Forms.Padding(4);
            btnStop.Name = "btnStop";
            btnStop.Size = new System.Drawing.Size(122, 93);
            btnStop.TabIndex = 8;
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Controls.Add(treeView, 0, 1);
            tableLayoutPanel1.Location = new System.Drawing.Point(14, 34);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85F));
            tableLayoutPanel1.Size = new System.Drawing.Size(350, 676);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.Controls.Add(statusPanel, 0, 0);
            tableLayoutPanel2.Controls.Add(contentPanel, 0, 1);
            tableLayoutPanel2.Location = new System.Drawing.Point(368, 34);
            tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.Size = new System.Drawing.Size(860, 676);
            tableLayoutPanel2.TabIndex = 5;
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { menuSettings, menuQuestion, comboTrend, btnStartTrend });
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            menuStrip.Size = new System.Drawing.Size(1228, 32);
            menuStrip.TabIndex = 6;
            menuStrip.Text = "menuStrip1";
            // 
            // menuSettings
            // 
            menuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { databasToolStripMenuItem, debugMenu, notifyMenu, autoStartTool, serviceToolStripMenuItem });
            menuSettings.Name = "menuSettings";
            menuSettings.Size = new System.Drawing.Size(84, 28);
            menuSettings.Text = "Inställningar";
            // 
            // databasToolStripMenuItem
            // 
            databasToolStripMenuItem.Name = "databasToolStripMenuItem";
            databasToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            databasToolStripMenuItem.Text = "Databas...";
            databasToolStripMenuItem.Click += databasToolStripMenuItem_Click;
            // 
            // debugMenu
            // 
            debugMenu.CheckOnClick = true;
            debugMenu.Name = "debugMenu";
            debugMenu.Size = new System.Drawing.Size(133, 22);
            debugMenu.Text = "Debug";
            debugMenu.CheckedChanged += debugMeny_CheckedChanged;
            // 
            // notifyMenu
            // 
            notifyMenu.CheckOnClick = true;
            notifyMenu.Name = "notifyMenu";
            notifyMenu.Size = new System.Drawing.Size(133, 22);
            notifyMenu.Text = "Notify Ikon";
            notifyMenu.CheckedChanged += notifyMenu_CheckedChanged;
            // 
            // autoStartTool
            // 
            autoStartTool.CheckOnClick = true;
            autoStartTool.Name = "autoStartTool";
            autoStartTool.Size = new System.Drawing.Size(133, 22);
            autoStartTool.Text = "Autostart";
            autoStartTool.CheckedChanged += autoStartTool_CheckedChanged;
            // 
            // serviceToolStripMenuItem
            // 
            serviceToolStripMenuItem.Name = "serviceToolStripMenuItem";
            serviceToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            serviceToolStripMenuItem.Text = "Service";
            serviceToolStripMenuItem.Click += serviceToolStripMenuItem_Click;
            // 
            // menuQuestion
            // 
            menuQuestion.Font = new System.Drawing.Font("Arial Narrow", 9.75F);
            menuQuestion.Name = "menuQuestion";
            menuQuestion.Size = new System.Drawing.Size(25, 28);
            menuQuestion.Text = "?";
            menuQuestion.Click += toolStripMenuItem1_Click;
            // 
            // comboTrend
            // 
            comboTrend.Name = "comboTrend";
            comboTrend.Size = new System.Drawing.Size(300, 28);
            comboTrend.TextChanged += comboTrend_TextChanged;
            // 
            // btnStartTrend
            // 
            btnStartTrend.Image = (System.Drawing.Image)resources.GetObject("btnStartTrend.Image");
            btnStartTrend.Name = "btnStartTrend";
            btnStartTrend.Size = new System.Drawing.Size(95, 28);
            btnStartTrend.Text = "Visa trend";
            btnStartTrend.Click += btnStartTrend_Click;
            // 
            // notifyIcon
            // 
            notifyIcon.Text = "notifyIcon1";
            notifyIcon.Visible = true;
            notifyIcon.Click += notifyIcon_Click;
            // 
            // databaseImageList
            // 
            databaseImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            databaseImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("databaseImageList.ImageStream");
            databaseImageList.TransparentColor = System.Drawing.Color.Transparent;
            databaseImageList.Images.SetKeyName(0, "no_database_26px.png");
            databaseImageList.Images.SetKeyName(1, "database_view_26px.png");
            // 
            // MasterForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(1228, 715);
            Controls.Add(menuStrip);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(1249, 879);
            Name = "MasterForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Optima Value";
            FormClosing += MasterForm_FormClosing;
            Load += MasterForm_Load;
            addPlcMenu.ResumeLayout(false);
            statusPanel.ResumeLayout(false);
            statusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)serviceImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)databaseImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.Label txtStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolStripMenuItem addPlc;
        private System.Windows.Forms.ContextMenuStrip addPlcMenu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuSettings;
        private System.Windows.Forms.ToolStripMenuItem databasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugMenu;
        private System.Windows.Forms.ToolStripMenuItem menuQuestion;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PictureBox errorImage;
        private System.Windows.Forms.ToolStripMenuItem notifyMenu;
        private System.Windows.Forms.PictureBox databaseImage;
        private System.Windows.Forms.ImageList databaseImageList;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem autoStartTool;
        private System.Windows.Forms.ToolStripComboBox comboTrend;
        private System.Windows.Forms.ToolStripMenuItem btnStartTrend;
        private System.Windows.Forms.ToolStripMenuItem serviceToolStripMenuItem;
        private System.Windows.Forms.PictureBox serviceImage;
    }
}