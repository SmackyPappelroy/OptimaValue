namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    partial class AddPlcFromFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddPlcFromFile));
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            btnSave = new System.Windows.Forms.Button();
            btnNew = new System.Windows.Forms.Button();
            errorProvider = new System.Windows.Forms.ErrorProvider(components);
            toolTip = new System.Windows.Forms.ToolTip(components);
            checkActive = new System.Windows.Forms.CheckBox();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            paraName = new Parameters.parameterTextControl();
            paraLogType = new Parameters.paramaterComboControl();
            paraDescription = new Parameters.parameterTextControl();
            paraLogTime = new Parameters.parameterTextControl();
            paraFreq = new Parameters.paramaterComboControl();
            paraVarType = new Parameters.paramaterComboControl();
            paraDataType = new Parameters.paramaterComboControl();
            paraBlockNr = new Parameters.parameterTextControl();
            paraStartAddress = new Parameters.parameterTextControl();
            paraBitAddress = new Parameters.parameterTextControl();
            paraDeadband = new Parameters.parameterTextControl();
            paraScaleOffset = new Parameters.parameterTextControl();
            paraScaleMin = new Parameters.parameterTextControl();
            paraScaleMax = new Parameters.parameterTextControl();
            paraRawMin = new Parameters.parameterTextControl();
            paraRawMax = new Parameters.parameterTextControl();
            paraNrOfValues = new Parameters.parameterTextControl();
            paraUnit = new Parameters.parameterTextControl();
            flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            btnOpc = new System.Windows.Forms.Button();
            btnEkvation = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            flowLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(btnSave, 1, 0);
            tableLayoutPanel1.Controls.Add(btnNew, 0, 0);
            tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            tableLayoutPanel1.Location = new System.Drawing.Point(4, 625);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            tableLayoutPanel1.Size = new System.Drawing.Size(895, 108);
            tableLayoutPanel1.TabIndex = 0;
            tableLayoutPanel1.Paint += tableLayoutPanel1_Paint;
            // 
            // btnSave
            // 
            btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(67, 62, 71);
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSave.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnSave.ForeColor = System.Drawing.Color.Black;
            btnSave.Image = (System.Drawing.Image)resources.GetObject("btnSave.Image");
            btnSave.Location = new System.Drawing.Point(451, 4);
            btnSave.Margin = new System.Windows.Forms.Padding(4);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(440, 100);
            btnSave.TabIndex = 18;
            toolTip.SetToolTip(btnSave, "Ändra befintlig tag");
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnUpdate_Click;
            // 
            // btnNew
            // 
            btnNew.Dock = System.Windows.Forms.DockStyle.Fill;
            btnNew.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(67, 62, 71);
            btnNew.FlatAppearance.BorderSize = 0;
            btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnNew.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnNew.ForeColor = System.Drawing.Color.Black;
            btnNew.Image = (System.Drawing.Image)resources.GetObject("btnNew.Image");
            btnNew.Location = new System.Drawing.Point(4, 4);
            btnNew.Margin = new System.Windows.Forms.Padding(4);
            btnNew.Name = "btnNew";
            btnNew.Size = new System.Drawing.Size(439, 100);
            btnNew.TabIndex = 17;
            toolTip.SetToolTip(btnNew, "Skapa ny tag");
            btnNew.UseVisualStyleBackColor = true;
            btnNew.Click += btnAdd_Click;
            // 
            // errorProvider
            // 
            errorProvider.ContainerControl = this;
            // 
            // checkActive
            // 
            checkActive.AutoSize = true;
            checkActive.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            checkActive.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            checkActive.Location = new System.Drawing.Point(4, 4);
            checkActive.Margin = new System.Windows.Forms.Padding(4);
            checkActive.Name = "checkActive";
            checkActive.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            checkActive.Size = new System.Drawing.Size(81, 25);
            checkActive.TabIndex = 1;
            checkActive.Text = "Aktiv";
            toolTip.SetToolTip(checkActive, "Ska taggen loggas?");
            checkActive.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            flowLayoutPanel1.Controls.Add(paraName);
            flowLayoutPanel1.Controls.Add(paraLogType);
            flowLayoutPanel1.Controls.Add(paraDescription);
            flowLayoutPanel1.Controls.Add(paraLogTime);
            flowLayoutPanel1.Controls.Add(paraFreq);
            flowLayoutPanel1.Controls.Add(paraVarType);
            flowLayoutPanel1.Controls.Add(paraDataType);
            flowLayoutPanel1.Controls.Add(paraBlockNr);
            flowLayoutPanel1.Controls.Add(paraStartAddress);
            flowLayoutPanel1.Controls.Add(paraBitAddress);
            flowLayoutPanel1.Controls.Add(paraDeadband);
            flowLayoutPanel1.Controls.Add(paraScaleOffset);
            flowLayoutPanel1.Controls.Add(paraScaleMin);
            flowLayoutPanel1.Controls.Add(paraScaleMax);
            flowLayoutPanel1.Controls.Add(paraRawMin);
            flowLayoutPanel1.Controls.Add(paraRawMax);
            flowLayoutPanel1.Controls.Add(paraNrOfValues);
            flowLayoutPanel1.Controls.Add(paraUnit);
            flowLayoutPanel1.Location = new System.Drawing.Point(4, 2);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 4);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(895, 615);
            flowLayoutPanel1.TabIndex = 1;
            // 
            // paraName
            // 
            paraName.BackColor = System.Drawing.Color.Transparent;
            paraName.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraName.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraName.Header = "Namn";
            paraName.Location = new System.Drawing.Point(6, 6);
            paraName.Margin = new System.Windows.Forms.Padding(6);
            paraName.Name = "paraName";
            paraName.ParameterValue = "";
            paraName.Size = new System.Drawing.Size(433, 56);
            paraName.TabIndex = 51;
            paraName.Validating += ValidateControl;
            // 
            // paraLogType
            // 
            paraLogType.BackColor = System.Drawing.Color.Transparent;
            paraLogType.ComboItems = new string[] { "Cyclic", "Delta", "TimeOfDay", "Event", "WriteWatchDogInt16", "Calculated", "RateOfChange" };
            paraLogType.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraLogType.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraLogType.HeaderText = "Log-typ";
            paraLogType.Location = new System.Drawing.Point(451, 6);
            paraLogType.Margin = new System.Windows.Forms.Padding(6);
            paraLogType.Name = "paraLogType";
            paraLogType.Size = new System.Drawing.Size(433, 56);
            paraLogType.TabIndex = 52;
            // 
            // paraDescription
            // 
            paraDescription.BackColor = System.Drawing.Color.Transparent;
            paraDescription.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraDescription.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraDescription.Header = "Beskrivning";
            paraDescription.Location = new System.Drawing.Point(6, 74);
            paraDescription.Margin = new System.Windows.Forms.Padding(6);
            paraDescription.Name = "paraDescription";
            paraDescription.ParameterValue = "";
            paraDescription.Size = new System.Drawing.Size(433, 56);
            paraDescription.TabIndex = 67;
            // 
            // paraLogTime
            // 
            paraLogTime.BackColor = System.Drawing.Color.Transparent;
            paraLogTime.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraLogTime.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraLogTime.Header = "Tid";
            paraLogTime.Location = new System.Drawing.Point(451, 74);
            paraLogTime.Margin = new System.Windows.Forms.Padding(6);
            paraLogTime.Name = "paraLogTime";
            paraLogTime.ParameterValue = "";
            paraLogTime.Size = new System.Drawing.Size(433, 56);
            paraLogTime.TabIndex = 53;
            paraLogTime.Validating += ValidateControl;
            // 
            // paraFreq
            // 
            paraFreq.BackColor = System.Drawing.Color.Transparent;
            paraFreq.ComboItems = new string[] { "Never", "_50ms", "_100ms", "_250ms", "_500ms", "_1s", "_2s", "_10s", "_30s", "_1m", "_5m", "_20m", "_1h" };
            paraFreq.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraFreq.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraFreq.HeaderText = "Läs-frekvens";
            paraFreq.Location = new System.Drawing.Point(6, 142);
            paraFreq.Margin = new System.Windows.Forms.Padding(6);
            paraFreq.Name = "paraFreq";
            paraFreq.Size = new System.Drawing.Size(433, 56);
            paraFreq.TabIndex = 64;
            // 
            // paraVarType
            // 
            paraVarType.BackColor = System.Drawing.Color.Transparent;
            paraVarType.ComboItems = new string[] { "Bit", "Byte", "Word", "DWord", "Int", "DInt", "Real", "String", "S7String", "Timer", "Counter", "DateTime", "DateTimeLong" };
            paraVarType.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraVarType.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraVarType.HeaderText = "Variabel-typ";
            paraVarType.Location = new System.Drawing.Point(451, 142);
            paraVarType.Margin = new System.Windows.Forms.Padding(6);
            paraVarType.Name = "paraVarType";
            paraVarType.Size = new System.Drawing.Size(433, 50);
            paraVarType.TabIndex = 66;
            // 
            // paraDataType
            // 
            paraDataType.BackColor = System.Drawing.Color.Transparent;
            paraDataType.ComboItems = new string[] { "Counter", "Timer", "Input", "Output", "Memory", "DataBlock" };
            paraDataType.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraDataType.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraDataType.HeaderText = "Datatyp";
            paraDataType.Location = new System.Drawing.Point(6, 210);
            paraDataType.Margin = new System.Windows.Forms.Padding(6);
            paraDataType.Name = "paraDataType";
            paraDataType.Size = new System.Drawing.Size(433, 56);
            paraDataType.TabIndex = 60;
            // 
            // paraBlockNr
            // 
            paraBlockNr.BackColor = System.Drawing.Color.Transparent;
            paraBlockNr.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraBlockNr.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraBlockNr.Header = "Block-nummer";
            paraBlockNr.Location = new System.Drawing.Point(451, 210);
            paraBlockNr.Margin = new System.Windows.Forms.Padding(6);
            paraBlockNr.Name = "paraBlockNr";
            paraBlockNr.ParameterValue = "";
            paraBlockNr.Size = new System.Drawing.Size(433, 50);
            paraBlockNr.TabIndex = 56;
            paraBlockNr.Validating += ValidateControl;
            // 
            // paraStartAddress
            // 
            paraStartAddress.BackColor = System.Drawing.Color.Transparent;
            paraStartAddress.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraStartAddress.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraStartAddress.Header = "Start-adress";
            paraStartAddress.Location = new System.Drawing.Point(6, 278);
            paraStartAddress.Margin = new System.Windows.Forms.Padding(6);
            paraStartAddress.Name = "paraStartAddress";
            paraStartAddress.ParameterValue = "";
            paraStartAddress.Size = new System.Drawing.Size(433, 56);
            paraStartAddress.TabIndex = 61;
            paraStartAddress.Validating += ValidateControl;
            // 
            // paraBitAddress
            // 
            paraBitAddress.BackColor = System.Drawing.Color.Transparent;
            paraBitAddress.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraBitAddress.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraBitAddress.Header = "Bit-adress";
            paraBitAddress.Location = new System.Drawing.Point(451, 278);
            paraBitAddress.Margin = new System.Windows.Forms.Padding(6);
            paraBitAddress.Name = "paraBitAddress";
            paraBitAddress.ParameterValue = "";
            paraBitAddress.Size = new System.Drawing.Size(433, 56);
            paraBitAddress.TabIndex = 63;
            paraBitAddress.Validating += ValidateControl;
            // 
            // paraDeadband
            // 
            paraDeadband.BackColor = System.Drawing.Color.Transparent;
            paraDeadband.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraDeadband.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraDeadband.Header = "Dödband";
            paraDeadband.Location = new System.Drawing.Point(6, 346);
            paraDeadband.Margin = new System.Windows.Forms.Padding(6);
            paraDeadband.Name = "paraDeadband";
            paraDeadband.ParameterValue = "";
            paraDeadband.Size = new System.Drawing.Size(433, 56);
            paraDeadband.TabIndex = 54;
            paraDeadband.Validating += ValidateControl;
            // 
            // paraScaleOffset
            // 
            paraScaleOffset.BackColor = System.Drawing.Color.Transparent;
            paraScaleOffset.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraScaleOffset.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraScaleOffset.Header = "Skalering offset";
            paraScaleOffset.Location = new System.Drawing.Point(451, 346);
            paraScaleOffset.Margin = new System.Windows.Forms.Padding(6);
            paraScaleOffset.Name = "paraScaleOffset";
            paraScaleOffset.ParameterValue = "";
            paraScaleOffset.Size = new System.Drawing.Size(433, 56);
            paraScaleOffset.TabIndex = 57;
            paraScaleOffset.Validating += ValidateControl;
            // 
            // paraScaleMin
            // 
            paraScaleMin.BackColor = System.Drawing.Color.Transparent;
            paraScaleMin.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraScaleMin.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraScaleMin.Header = "Skalering Min";
            paraScaleMin.Location = new System.Drawing.Point(6, 414);
            paraScaleMin.Margin = new System.Windows.Forms.Padding(6);
            paraScaleMin.Name = "paraScaleMin";
            paraScaleMin.ParameterValue = "";
            paraScaleMin.Size = new System.Drawing.Size(433, 56);
            paraScaleMin.TabIndex = 58;
            paraScaleMin.Validating += ValidateControl;
            // 
            // paraScaleMax
            // 
            paraScaleMax.BackColor = System.Drawing.Color.Transparent;
            paraScaleMax.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraScaleMax.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraScaleMax.Header = "Skalering Max";
            paraScaleMax.Location = new System.Drawing.Point(451, 414);
            paraScaleMax.Margin = new System.Windows.Forms.Padding(6);
            paraScaleMax.Name = "paraScaleMax";
            paraScaleMax.ParameterValue = "";
            paraScaleMax.Size = new System.Drawing.Size(433, 56);
            paraScaleMax.TabIndex = 59;
            paraScaleMax.Validating += ValidateControl;
            // 
            // paraRawMin
            // 
            paraRawMin.BackColor = System.Drawing.Color.Transparent;
            paraRawMin.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraRawMin.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraRawMin.Header = "Rå skal min";
            paraRawMin.Location = new System.Drawing.Point(6, 482);
            paraRawMin.Margin = new System.Windows.Forms.Padding(6);
            paraRawMin.Name = "paraRawMin";
            paraRawMin.ParameterValue = "";
            paraRawMin.Size = new System.Drawing.Size(433, 56);
            paraRawMin.TabIndex = 69;
            paraRawMin.Validating += ValidateControl;
            // 
            // paraRawMax
            // 
            paraRawMax.BackColor = System.Drawing.Color.Transparent;
            paraRawMax.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraRawMax.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraRawMax.Header = "Rå skal max";
            paraRawMax.Location = new System.Drawing.Point(450, 481);
            paraRawMax.Margin = new System.Windows.Forms.Padding(5, 5, 10, 5);
            paraRawMax.Name = "paraRawMax";
            paraRawMax.ParameterValue = "";
            paraRawMax.Size = new System.Drawing.Size(433, 56);
            paraRawMax.TabIndex = 68;
            paraRawMax.Validating += ValidateControl;
            // 
            // paraNrOfValues
            // 
            paraNrOfValues.BackColor = System.Drawing.Color.Transparent;
            paraNrOfValues.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraNrOfValues.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraNrOfValues.Header = "Antal värden";
            paraNrOfValues.Location = new System.Drawing.Point(6, 550);
            paraNrOfValues.Margin = new System.Windows.Forms.Padding(6);
            paraNrOfValues.Name = "paraNrOfValues";
            paraNrOfValues.ParameterValue = "";
            paraNrOfValues.Size = new System.Drawing.Size(433, 56);
            paraNrOfValues.TabIndex = 62;
            paraNrOfValues.Validating += ValidateControl;
            // 
            // paraUnit
            // 
            paraUnit.BackColor = System.Drawing.Color.Transparent;
            paraUnit.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            paraUnit.ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            paraUnit.Header = "Värde-enhet";
            paraUnit.Location = new System.Drawing.Point(451, 550);
            paraUnit.Margin = new System.Windows.Forms.Padding(6);
            paraUnit.Name = "paraUnit";
            paraUnit.ParameterValue = "";
            paraUnit.Size = new System.Drawing.Size(433, 56);
            paraUnit.TabIndex = 65;
            paraUnit.Validating += ValidateControl;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(flowLayoutPanel1);
            flowLayoutPanel2.Controls.Add(tableLayoutPanel1);
            flowLayoutPanel2.Location = new System.Drawing.Point(1, 46);
            flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new System.Drawing.Size(895, 743);
            flowLayoutPanel2.TabIndex = 2;
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.Controls.Add(checkActive);
            flowLayoutPanel3.Location = new System.Drawing.Point(1, 2);
            flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(4);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new System.Drawing.Size(154, 37);
            flowLayoutPanel3.TabIndex = 3;
            // 
            // btnOpc
            // 
            btnOpc.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnOpc.ForeColor = System.Drawing.Color.Black;
            btnOpc.Location = new System.Drawing.Point(302, 8);
            btnOpc.Name = "btnOpc";
            btnOpc.Size = new System.Drawing.Size(273, 31);
            btnOpc.TabIndex = 4;
            btnOpc.Text = "Utforska OPC-server...";
            btnOpc.UseVisualStyleBackColor = true;
            btnOpc.Visible = false;
            // 
            // btnEkvation
            // 
            btnEkvation.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnEkvation.ForeColor = System.Drawing.Color.Black;
            btnEkvation.Location = new System.Drawing.Point(620, 8);
            btnEkvation.Name = "btnEkvation";
            btnEkvation.Size = new System.Drawing.Size(200, 31);
            btnEkvation.TabIndex = 5;
            btnEkvation.Text = "Kalkylerade värde...";
            btnEkvation.UseVisualStyleBackColor = true;
            btnEkvation.Click += clickEkvation;
            // 
            // AddPlcFromFile
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            BackColor = System.Drawing.Color.FromArgb(67, 62, 71);
            ClientSize = new System.Drawing.Size(920, 806);
            Controls.Add(btnEkvation);
            Controls.Add(btnOpc);
            Controls.Add(flowLayoutPanel3);
            Controls.Add(flowLayoutPanel2);
            ForeColor = System.Drawing.Color.FromArgb(175, 175, 175);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4);
            Name = "AddPlcFromFile";
            Opacity = 0.95D;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Lägg till / Ändra tag...";
            FormClosing += AddTag_FormClosing;
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel3.ResumeLayout(false);
            flowLayoutPanel3.PerformLayout();
            ResumeLayout(false);
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
        private Parameters.parameterTextControl paraDescription;
        private Parameters.parameterTextControl paraRawMax;
        private Parameters.parameterTextControl paraRawMin;
        private System.Windows.Forms.Button btnOpc;
        private System.Windows.Forms.Button btnEkvation;
    }
}
