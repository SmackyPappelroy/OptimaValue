namespace OptimaValue.ML
{
    partial class MLForm
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
            btnPredict = new System.Windows.Forms.Button();
            clbTags = new System.Windows.Forms.CheckedListBox();
            dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            dgvPredictions = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvPredictions).BeginInit();
            SuspendLayout();
            // 
            // btnPredict
            // 
            btnPredict.Location = new System.Drawing.Point(3, 104);
            btnPredict.Name = "btnPredict";
            btnPredict.Size = new System.Drawing.Size(145, 52);
            btnPredict.TabIndex = 1;
            btnPredict.Text = "Förutspå";
            btnPredict.UseVisualStyleBackColor = true;
            btnPredict.Click += btnPredict_Click;
            // 
            // clbTags
            // 
            clbTags.FormattingEnabled = true;
            clbTags.Location = new System.Drawing.Point(204, 104);
            clbTags.Name = "clbTags";
            clbTags.Size = new System.Drawing.Size(502, 180);
            clbTags.TabIndex = 2;
            // 
            // dateTimePickerStart
            // 
            dateTimePickerStart.Location = new System.Drawing.Point(3, 16);
            dateTimePickerStart.Name = "dateTimePickerStart";
            dateTimePickerStart.Size = new System.Drawing.Size(224, 27);
            dateTimePickerStart.TabIndex = 3;
            // 
            // dateTimePickerEnd
            // 
            dateTimePickerEnd.Location = new System.Drawing.Point(482, 16);
            dateTimePickerEnd.Name = "dateTimePickerEnd";
            dateTimePickerEnd.Size = new System.Drawing.Size(224, 27);
            dateTimePickerEnd.TabIndex = 4;
            // 
            // dgvPredictions
            // 
            dgvPredictions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPredictions.Location = new System.Drawing.Point(3, 300);
            dgvPredictions.Name = "dgvPredictions";
            dgvPredictions.RowTemplate.Height = 25;
            dgvPredictions.Size = new System.Drawing.Size(717, 451);
            dgvPredictions.TabIndex = 5;
            // 
            // MLForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(67, 62, 71);
            ClientSize = new System.Drawing.Size(732, 763);
            Controls.Add(dgvPredictions);
            Controls.Add(dateTimePickerEnd);
            Controls.Add(dateTimePickerStart);
            Controls.Add(clbTags);
            Controls.Add(btnPredict);
            Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "MLForm";
            Text = "MLForm";
            ((System.ComponentModel.ISupportInitialize)dgvPredictions).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnPredict;
        private System.Windows.Forms.CheckedListBox clbTags;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.DataGridView dgvPredictions;
    }
}