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
            buttonUninstallService = new System.Windows.Forms.Button();
            txtServiceExist = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // buttonInstallService
            // 
            buttonInstallService.Location = new System.Drawing.Point(46, 74);
            buttonInstallService.Name = "buttonInstallService";
            buttonInstallService.Size = new System.Drawing.Size(118, 57);
            buttonInstallService.TabIndex = 0;
            buttonInstallService.Text = "Installera service";
            buttonInstallService.UseVisualStyleBackColor = true;
            buttonInstallService.Click += buttonInstallService_Click;
            // 
            // buttonUninstallService
            // 
            buttonUninstallService.Location = new System.Drawing.Point(221, 74);
            buttonUninstallService.Name = "buttonUninstallService";
            buttonUninstallService.Size = new System.Drawing.Size(118, 57);
            buttonUninstallService.TabIndex = 1;
            buttonUninstallService.Text = "Avinstallera service";
            buttonUninstallService.UseVisualStyleBackColor = true;
            buttonUninstallService.Click += buttonUninstallService_Click;
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
            // ServiceInstallerAppForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(464, 257);
            Controls.Add(txtServiceExist);
            Controls.Add(buttonUninstallService);
            Controls.Add(buttonInstallService);
            Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "ServiceInstallerAppForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ServiceInstaller";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button buttonInstallService;
        private System.Windows.Forms.Button buttonUninstallService;
        private System.Windows.Forms.Label txtServiceExist;
    }
}