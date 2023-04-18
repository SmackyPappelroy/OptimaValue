namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    partial class CalculationForm
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
            btnAddOperatorComboBox = new System.Windows.Forms.Button();
            btnGenerate = new System.Windows.Forms.Button();
            lblCalculation = new System.Windows.Forms.Label();
            pnlComboBoxes = new System.Windows.Forms.Panel();
            btnRemoveComboBoxes = new System.Windows.Forms.Button();
            lblDisplayCalculation = new System.Windows.Forms.Label();
            btnAddTagComboBox = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            SuspendLayout();
            // 
            // btnAddOperatorComboBox
            // 
            btnAddOperatorComboBox.Location = new System.Drawing.Point(13, 249);
            btnAddOperatorComboBox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            btnAddOperatorComboBox.Name = "btnAddOperatorComboBox";
            btnAddOperatorComboBox.Size = new System.Drawing.Size(153, 66);
            btnAddOperatorComboBox.TabIndex = 0;
            btnAddOperatorComboBox.Text = "Lägg till operator";
            btnAddOperatorComboBox.UseVisualStyleBackColor = true;
            btnAddOperatorComboBox.Click += btnAddOperatorComboBox_Click;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new System.Drawing.Point(12, 504);
            btnGenerate.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new System.Drawing.Size(153, 66);
            btnGenerate.TabIndex = 1;
            btnGenerate.Text = "Generera ekvation";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // lblCalculation
            // 
            lblCalculation.ForeColor = System.Drawing.Color.White;
            lblCalculation.Location = new System.Drawing.Point(12, 9);
            lblCalculation.MaximumSize = new System.Drawing.Size(556, 154);
            lblCalculation.Name = "lblCalculation";
            lblCalculation.Size = new System.Drawing.Size(520, 101);
            lblCalculation.TabIndex = 2;
            lblCalculation.Text = "___";
            toolTip1.SetToolTip(lblCalculation, "TagId ekvation");
            // 
            // pnlComboBoxes
            // 
            pnlComboBoxes.AutoScroll = true;
            pnlComboBoxes.Location = new System.Drawing.Point(172, 249);
            pnlComboBoxes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlComboBoxes.Name = "pnlComboBoxes";
            pnlComboBoxes.Size = new System.Drawing.Size(361, 492);
            pnlComboBoxes.TabIndex = 3;
            // 
            // btnRemoveComboBoxes
            // 
            btnRemoveComboBoxes.Location = new System.Drawing.Point(12, 419);
            btnRemoveComboBoxes.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            btnRemoveComboBoxes.Name = "btnRemoveComboBoxes";
            btnRemoveComboBoxes.Size = new System.Drawing.Size(153, 66);
            btnRemoveComboBoxes.TabIndex = 4;
            btnRemoveComboBoxes.Text = "Ta bort";
            btnRemoveComboBoxes.UseVisualStyleBackColor = true;
            btnRemoveComboBoxes.Click += btnRemoveComboBoxes_Click;
            // 
            // lblDisplayCalculation
            // 
            lblDisplayCalculation.ForeColor = System.Drawing.Color.White;
            lblDisplayCalculation.Location = new System.Drawing.Point(12, 119);
            lblDisplayCalculation.MaximumSize = new System.Drawing.Size(556, 154);
            lblDisplayCalculation.Name = "lblDisplayCalculation";
            lblDisplayCalculation.Size = new System.Drawing.Size(520, 101);
            lblDisplayCalculation.TabIndex = 5;
            lblDisplayCalculation.Text = "___";
            toolTip1.SetToolTip(lblDisplayCalculation, "Tagnamn ekvation");
            // 
            // btnAddTagComboBox
            // 
            btnAddTagComboBox.Location = new System.Drawing.Point(13, 334);
            btnAddTagComboBox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            btnAddTagComboBox.Name = "btnAddTagComboBox";
            btnAddTagComboBox.Size = new System.Drawing.Size(153, 66);
            btnAddTagComboBox.TabIndex = 6;
            btnAddTagComboBox.Text = "Lägg till tag";
            btnAddTagComboBox.UseVisualStyleBackColor = true;
            btnAddTagComboBox.Click += btnAddTagComboBox_Click;
            // 
            // CalculationForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(67, 62, 71);
            ClientSize = new System.Drawing.Size(545, 754);
            Controls.Add(btnAddTagComboBox);
            Controls.Add(lblDisplayCalculation);
            Controls.Add(btnRemoveComboBoxes);
            Controls.Add(pnlComboBoxes);
            Controls.Add(lblCalculation);
            Controls.Add(btnGenerate);
            Controls.Add(btnAddOperatorComboBox);
            Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            Name = "CalculationForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Kalkylering";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnAddOperatorComboBox;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblCalculation;
        private System.Windows.Forms.Panel pnlComboBoxes;
        private System.Windows.Forms.Button btnRemoveComboBoxes;
        private System.Windows.Forms.Label lblDisplayCalculation;
        private System.Windows.Forms.Button btnAddTagComboBox;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}