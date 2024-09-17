namespace OptimaValue
{
    partial class ServiceInstallerAppForm
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
            buttonInstallService = new System.Windows.Forms.Button();
            txtServiceExist = new System.Windows.Forms.Label();
            statusPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)statusPictureBox).BeginInit();
            SuspendLayout();
            // 
            // buttonInstallService
            // 
            buttonInstallService.Location = new System.Drawing.Point(142, 74);
            buttonInstallService.Name = "buttonInstallService";
            buttonInstallService.Size = new System.Drawing.Size(118, 57);
            buttonInstallService.TabIndex = 0;
            buttonInstallService.Text = "Installera service";
            buttonInstallService.UseVisualStyleBackColor = true;
            // 
            // txtServiceExist
            // 
            txtServiceExist.AutoSize = true;
            txtServiceExist.Location = new System.Drawing.Point(43, 18);
            txtServiceExist.Name = "txtServiceExist";
            txtServiceExist.Size = new System.Drawing.Size(153, 21);
            txtServiceExist.TabIndex = 2;
            txtServiceExist.Text = "Does service exist?";
            // 
            // statusPictureBox
            // 
            statusPictureBox.Location = new System.Drawing.Point(17, 19);
            statusPictureBox.Name = "statusPictureBox";
            statusPictureBox.Size = new System.Drawing.Size(20, 20);
            statusPictureBox.TabIndex = 3;
            statusPictureBox.TabStop = false;
            // 
            // ServiceInstallerAppForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(417, 182);
            Controls.Add(statusPictureBox);
            Controls.Add(txtServiceExist);
            Controls.Add(buttonInstallService);
            Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "ServiceInstallerAppForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ServiceInstaller";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)statusPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button buttonInstallService;
        private System.Windows.Forms.Label txtServiceExist;
        private System.Windows.Forms.PictureBox statusPictureBox;
    }
}