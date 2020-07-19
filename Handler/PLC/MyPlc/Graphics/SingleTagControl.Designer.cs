namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    partial class SingleTagControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleTagControl));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeTagMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTagMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.txtName = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.statusPicture = new System.Windows.Forms.PictureBox();
            this.statsImage = new System.Windows.Forms.PictureBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statsImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 20);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeTagMenu,
            this.removeTagMenu});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(111, 48);
            // 
            // changeTagMenu
            // 
            this.changeTagMenu.Name = "changeTagMenu";
            this.changeTagMenu.Size = new System.Drawing.Size(110, 22);
            this.changeTagMenu.Text = "Ändra";
            this.changeTagMenu.Click += new System.EventHandler(this.changeTagMenu_Click);
            // 
            // removeTagMenu
            // 
            this.removeTagMenu.Name = "removeTagMenu";
            this.removeTagMenu.Size = new System.Drawing.Size(110, 22);
            this.removeTagMenu.Text = "Ta Bort";
            this.removeTagMenu.Click += new System.EventHandler(this.removeTagMenu_Click);
            // 
            // txtName
            // 
            this.txtName.AutoSize = true;
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(69, 0);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(330, 26);
            this.txtName.TabIndex = 1;
            this.txtName.Text = "txtName";
            this.txtName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.statusPicture, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.statsImage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtName, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(402, 26);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // statusPicture
            // 
            this.statusPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusPicture.Image = ((System.Drawing.Image)(resources.GetObject("statusPicture.Image")));
            this.statusPicture.InitialImage = ((System.Drawing.Image)(resources.GetObject("statusPicture.InitialImage")));
            this.statusPicture.Location = new System.Drawing.Point(25, 3);
            this.statusPicture.Name = "statusPicture";
            this.statusPicture.Size = new System.Drawing.Size(16, 20);
            this.statusPicture.TabIndex = 3;
            this.statusPicture.TabStop = false;
            // 
            // statsImage
            // 
            this.statsImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsImage.Image = ((System.Drawing.Image)(resources.GetObject("statsImage.Image")));
            this.statsImage.InitialImage = ((System.Drawing.Image)(resources.GetObject("statsImage.InitialImage")));
            this.statsImage.Location = new System.Drawing.Point(47, 3);
            this.statsImage.Name = "statsImage";
            this.statsImage.Size = new System.Drawing.Size(16, 20);
            this.statsImage.TabIndex = 2;
            this.statsImage.TabStop = false;
            this.statsImage.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDoubleClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "icons8_ok_16.png");
            this.imageList.Images.SetKeyName(1, "icons8_error_16.png");
            // 
            // SingleTagControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SingleTagControl";
            this.Size = new System.Drawing.Size(402, 26);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statsImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label txtName;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem changeTagMenu;
        private System.Windows.Forms.ToolStripMenuItem removeTagMenu;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox statsImage;
        private System.Windows.Forms.PictureBox statusPicture;
        private System.Windows.Forms.ImageList imageList;
    }
}
