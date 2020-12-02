namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    partial class AddTag
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTag));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.checkActive = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.paraName = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.paraLogType = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.paramaterComboControl();
            this.paraLogTime = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.paraDeadband = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.paraVarType = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.paramaterComboControl();
            this.paraBlockNr = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.paraScaleOffset = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.paraScaleMin = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.paraScaleMax = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.paraDataType = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.paramaterComboControl();
            this.paraStartAddress = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.paraNrOfValues = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.paraBitAddress = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.paraFreq = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.paramaterComboControl();
            this.paraUnit = new OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters.parameterTextControl();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnSave, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnNew, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 559);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 90);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.Black;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(403, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(394, 84);
            this.btnSave.TabIndex = 18;
            this.toolTip.SetToolTip(this.btnSave, "Ändra befintlig tag");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnNew
            // 
            this.btnNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNew.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Century Gothic", 20.25F);
            this.btnNew.ForeColor = System.Drawing.Color.Black;
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.Location = new System.Drawing.Point(3, 3);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(394, 84);
            this.btnNew.TabIndex = 17;
            this.toolTip.SetToolTip(this.btnNew, "Skapa ny tag");
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // checkActive
            // 
            this.checkActive.AutoSize = true;
            this.checkActive.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.checkActive.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.checkActive.Location = new System.Drawing.Point(3, 3);
            this.checkActive.Name = "checkActive";
            this.checkActive.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.checkActive.Size = new System.Drawing.Size(80, 25);
            this.checkActive.TabIndex = 1;
            this.checkActive.Text = "Aktiv";
            this.toolTip.SetToolTip(this.checkActive, "Ska taggen loggas?");
            this.checkActive.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.paraName);
            this.flowLayoutPanel1.Controls.Add(this.paraLogType);
            this.flowLayoutPanel1.Controls.Add(this.paraLogTime);
            this.flowLayoutPanel1.Controls.Add(this.paraDeadband);
            this.flowLayoutPanel1.Controls.Add(this.paraVarType);
            this.flowLayoutPanel1.Controls.Add(this.paraBlockNr);
            this.flowLayoutPanel1.Controls.Add(this.paraScaleOffset);
            this.flowLayoutPanel1.Controls.Add(this.paraScaleMin);
            this.flowLayoutPanel1.Controls.Add(this.paraScaleMax);
            this.flowLayoutPanel1.Controls.Add(this.paraDataType);
            this.flowLayoutPanel1.Controls.Add(this.paraStartAddress);
            this.flowLayoutPanel1.Controls.Add(this.paraNrOfValues);
            this.flowLayoutPanel1.Controls.Add(this.paraBitAddress);
            this.flowLayoutPanel1.Controls.Add(this.paraFreq);
            this.flowLayoutPanel1.Controls.Add(this.paraUnit);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(873, 550);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel2.Controls.Add(this.tableLayoutPanel1);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(1, 40);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(841, 659);
            this.flowLayoutPanel2.TabIndex = 2;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.checkActive);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(1, 2);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(132, 32);
            this.flowLayoutPanel3.TabIndex = 3;
            // 
            // paraName
            // 
            this.paraName.BackColor = System.Drawing.Color.Transparent;
            this.paraName.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraName.Header = "Namn";
            this.paraName.Location = new System.Drawing.Point(5, 5);
            this.paraName.Margin = new System.Windows.Forms.Padding(5);
            this.paraName.Name = "paraName";
            this.paraName.ParameterValue = "";
            this.paraName.Size = new System.Drawing.Size(409, 56);
            this.paraName.TabIndex = 51;
            this.paraName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // paraLogType
            // 
            this.paraLogType.BackColor = System.Drawing.Color.Transparent;
            this.paraLogType.ComboItems = new string[] {
        "Cyclic",
        "Delta",
        "TimeOfDay",
        "Event"};
            this.paraLogType.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraLogType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraLogType.HeaderText = "Log-typ";
            this.paraLogType.Location = new System.Drawing.Point(5, 71);
            this.paraLogType.Margin = new System.Windows.Forms.Padding(5);
            this.paraLogType.Name = "paraLogType";
            this.paraLogType.Size = new System.Drawing.Size(409, 56);
            this.paraLogType.TabIndex = 52;
            // 
            // paraLogTime
            // 
            this.paraLogTime.BackColor = System.Drawing.Color.Transparent;
            this.paraLogTime.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraLogTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraLogTime.Header = "Tid";
            this.paraLogTime.Location = new System.Drawing.Point(5, 137);
            this.paraLogTime.Margin = new System.Windows.Forms.Padding(5);
            this.paraLogTime.Name = "paraLogTime";
            this.paraLogTime.ParameterValue = "";
            this.paraLogTime.Size = new System.Drawing.Size(409, 56);
            this.paraLogTime.TabIndex = 53;
            this.paraLogTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtTimeOfDay_Validating);
            // 
            // paraDeadband
            // 
            this.paraDeadband.BackColor = System.Drawing.Color.Transparent;
            this.paraDeadband.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraDeadband.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraDeadband.Header = "Dödband";
            this.paraDeadband.Location = new System.Drawing.Point(5, 203);
            this.paraDeadband.Margin = new System.Windows.Forms.Padding(5);
            this.paraDeadband.Name = "paraDeadband";
            this.paraDeadband.ParameterValue = "";
            this.paraDeadband.Size = new System.Drawing.Size(409, 56);
            this.paraDeadband.TabIndex = 54;
            this.paraDeadband.Validating += new System.ComponentModel.CancelEventHandler(this.txtDeadband_TextChanged);
            // 
            // paraVarType
            // 
            this.paraVarType.BackColor = System.Drawing.Color.Transparent;
            this.paraVarType.ComboItems = new string[] {
        "Bit",
        "Byte",
        "Word",
        "DWord",
        "Int",
        "DInt",
        "Real",
        "String",
        "StringEx",
        "Timer",
        "Counter",
        "DateTime",
        "DateTimeLong"};
            this.paraVarType.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraVarType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraVarType.HeaderText = "Variabel-typ";
            this.paraVarType.Location = new System.Drawing.Point(5, 269);
            this.paraVarType.Margin = new System.Windows.Forms.Padding(5);
            this.paraVarType.Name = "paraVarType";
            this.paraVarType.Size = new System.Drawing.Size(409, 56);
            this.paraVarType.TabIndex = 66;
            // 
            // paraBlockNr
            // 
            this.paraBlockNr.BackColor = System.Drawing.Color.Transparent;
            this.paraBlockNr.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraBlockNr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraBlockNr.Header = "Block-nummer";
            this.paraBlockNr.Location = new System.Drawing.Point(5, 335);
            this.paraBlockNr.Margin = new System.Windows.Forms.Padding(5);
            this.paraBlockNr.Name = "paraBlockNr";
            this.paraBlockNr.ParameterValue = "";
            this.paraBlockNr.Size = new System.Drawing.Size(409, 56);
            this.paraBlockNr.TabIndex = 56;
            this.paraBlockNr.Validating += new System.ComponentModel.CancelEventHandler(this.txtBlockNr_Validating);
            // 
            // paraScaleOffset
            // 
            this.paraScaleOffset.BackColor = System.Drawing.Color.Transparent;
            this.paraScaleOffset.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraScaleOffset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraScaleOffset.Header = "Skalering offset";
            this.paraScaleOffset.Location = new System.Drawing.Point(5, 401);
            this.paraScaleOffset.Margin = new System.Windows.Forms.Padding(5);
            this.paraScaleOffset.Name = "paraScaleOffset";
            this.paraScaleOffset.ParameterValue = "";
            this.paraScaleOffset.Size = new System.Drawing.Size(409, 56);
            this.paraScaleOffset.TabIndex = 57;
            this.paraScaleOffset.Validating += new System.ComponentModel.CancelEventHandler(this.txtScaleOffset_Validating);
            // 
            // paraScaleMin
            // 
            this.paraScaleMin.BackColor = System.Drawing.Color.Transparent;
            this.paraScaleMin.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraScaleMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraScaleMin.Header = "Skalering Min";
            this.paraScaleMin.Location = new System.Drawing.Point(5, 467);
            this.paraScaleMin.Margin = new System.Windows.Forms.Padding(5);
            this.paraScaleMin.Name = "paraScaleMin";
            this.paraScaleMin.ParameterValue = "";
            this.paraScaleMin.Size = new System.Drawing.Size(409, 56);
            this.paraScaleMin.TabIndex = 58;
            this.paraScaleMin.Validating += new System.ComponentModel.CancelEventHandler(this.txtScaleMin_Validating);
            // 
            // paraScaleMax
            // 
            this.paraScaleMax.BackColor = System.Drawing.Color.Transparent;
            this.paraScaleMax.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraScaleMax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraScaleMax.Header = "Skalering Max";
            this.paraScaleMax.Location = new System.Drawing.Point(424, 5);
            this.paraScaleMax.Margin = new System.Windows.Forms.Padding(5);
            this.paraScaleMax.Name = "paraScaleMax";
            this.paraScaleMax.ParameterValue = "";
            this.paraScaleMax.Size = new System.Drawing.Size(409, 56);
            this.paraScaleMax.TabIndex = 59;
            this.paraScaleMax.Validating += new System.ComponentModel.CancelEventHandler(this.txtScaleMax_Validating);
            // 
            // paraDataType
            // 
            this.paraDataType.BackColor = System.Drawing.Color.Transparent;
            this.paraDataType.ComboItems = new string[] {
        "Counter",
        "Timer",
        "Input",
        "Output",
        "Memory",
        "DataBlock"};
            this.paraDataType.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraDataType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraDataType.HeaderText = "Datatyp";
            this.paraDataType.Location = new System.Drawing.Point(424, 71);
            this.paraDataType.Margin = new System.Windows.Forms.Padding(5);
            this.paraDataType.Name = "paraDataType";
            this.paraDataType.Size = new System.Drawing.Size(409, 56);
            this.paraDataType.TabIndex = 60;
            // 
            // paraStartAddress
            // 
            this.paraStartAddress.BackColor = System.Drawing.Color.Transparent;
            this.paraStartAddress.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraStartAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraStartAddress.Header = "Start-adress";
            this.paraStartAddress.Location = new System.Drawing.Point(424, 137);
            this.paraStartAddress.Margin = new System.Windows.Forms.Padding(5);
            this.paraStartAddress.Name = "paraStartAddress";
            this.paraStartAddress.ParameterValue = "";
            this.paraStartAddress.Size = new System.Drawing.Size(409, 56);
            this.paraStartAddress.TabIndex = 61;
            this.paraStartAddress.Validating += new System.ComponentModel.CancelEventHandler(this.txtStartByte_Validating);
            // 
            // paraNrOfValues
            // 
            this.paraNrOfValues.BackColor = System.Drawing.Color.Transparent;
            this.paraNrOfValues.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraNrOfValues.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraNrOfValues.Header = "Antal värden";
            this.paraNrOfValues.Location = new System.Drawing.Point(424, 203);
            this.paraNrOfValues.Margin = new System.Windows.Forms.Padding(5);
            this.paraNrOfValues.Name = "paraNrOfValues";
            this.paraNrOfValues.ParameterValue = "";
            this.paraNrOfValues.Size = new System.Drawing.Size(409, 56);
            this.paraNrOfValues.TabIndex = 62;
            this.paraNrOfValues.Validating += new System.ComponentModel.CancelEventHandler(this.txtNrOfElements_Validating);
            // 
            // paraBitAddress
            // 
            this.paraBitAddress.BackColor = System.Drawing.Color.Transparent;
            this.paraBitAddress.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraBitAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraBitAddress.Header = "Bit-adress";
            this.paraBitAddress.Location = new System.Drawing.Point(424, 269);
            this.paraBitAddress.Margin = new System.Windows.Forms.Padding(5);
            this.paraBitAddress.Name = "paraBitAddress";
            this.paraBitAddress.ParameterValue = "";
            this.paraBitAddress.Size = new System.Drawing.Size(409, 56);
            this.paraBitAddress.TabIndex = 63;
            this.paraBitAddress.Validating += new System.ComponentModel.CancelEventHandler(this.txtBitAddress_Validating);
            // 
            // paraFreq
            // 
            this.paraFreq.BackColor = System.Drawing.Color.Transparent;
            this.paraFreq.ComboItems = new string[] {
        "Never",
        "_50ms",
        "_100ms",
        "_250ms",
        "_500ms",
        "_1s",
        "_2s",
        "_10s",
        "_30s",
        "_1m",
        "_5m"};
            this.paraFreq.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraFreq.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraFreq.HeaderText = "Läs-frekvens";
            this.paraFreq.Location = new System.Drawing.Point(424, 335);
            this.paraFreq.Margin = new System.Windows.Forms.Padding(5);
            this.paraFreq.Name = "paraFreq";
            this.paraFreq.Size = new System.Drawing.Size(409, 56);
            this.paraFreq.TabIndex = 64;
            // 
            // paraUnit
            // 
            this.paraUnit.BackColor = System.Drawing.Color.Transparent;
            this.paraUnit.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.paraUnit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.paraUnit.Header = "Värde-enhet";
            this.paraUnit.Location = new System.Drawing.Point(424, 401);
            this.paraUnit.Margin = new System.Windows.Forms.Padding(5);
            this.paraUnit.Name = "paraUnit";
            this.paraUnit.ParameterValue = "";
            this.paraUnit.Size = new System.Drawing.Size(409, 56);
            this.paraUnit.TabIndex = 65;
            this.paraUnit.Validating += new System.ComponentModel.CancelEventHandler(this.txtUnit_Validating);
            // 
            // AddTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.ClientSize = new System.Drawing.Size(889, 711);
            this.Controls.Add(this.flowLayoutPanel3);
            this.Controls.Add(this.flowLayoutPanel2);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddTag";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lägg till / Ändra tag...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddTag_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.CheckBox checkActive;
        private Parameters.parameterTextControl paraName;
        private Parameters.paramaterComboControl paraLogType;
        private Parameters.parameterTextControl paraLogTime;
        private Parameters.parameterTextControl paraDeadband;
        private Parameters.parameterTextControl paraBlockNr;
        private Parameters.parameterTextControl paraScaleOffset;
        private Parameters.parameterTextControl paraScaleMin;
        private Parameters.parameterTextControl paraScaleMax;
        private Parameters.paramaterComboControl paraDataType;
        private Parameters.parameterTextControl paraStartAddress;
        private Parameters.parameterTextControl paraNrOfValues;
        private Parameters.parameterTextControl paraBitAddress;
        private Parameters.paramaterComboControl paraFreq;
        private Parameters.parameterTextControl paraUnit;
        private Parameters.paramaterComboControl paraVarType;
    }
}
