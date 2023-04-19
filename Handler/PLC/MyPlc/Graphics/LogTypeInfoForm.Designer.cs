namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    partial class LogTypeInfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogTypeInfoForm));
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(0, 9);
            label1.MaximumSize = new System.Drawing.Size(650, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(648, 693);
            label1.TabIndex = 0;
            label1.Text = resources.GetString("label1.Text");
            // 
            // LogTypeInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(67, 62, 71);
            ClientSize = new System.Drawing.Size(654, 731);
            Controls.Add(label1);
            Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ForeColor = System.Drawing.Color.FromArgb(224, 224, 224);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "LogTypeInfoForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Log-typ info";
            TopMost = true;
            FormClosing += LogTypeInfoForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
    }
}