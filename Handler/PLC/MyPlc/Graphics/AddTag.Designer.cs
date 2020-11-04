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
            this.txtName = new System.Windows.Forms.TextBox();
            this.comboLogType = new System.Windows.Forms.ComboBox();
            this.txtTimeOfDay = new System.Windows.Forms.TextBox();
            this.txtDeadband = new System.Windows.Forms.TextBox();
            this.comboVarType = new System.Windows.Forms.ComboBox();
            this.txtBlockNr = new System.Windows.Forms.TextBox();
            this.txtScaleOffset = new System.Windows.Forms.TextBox();
            this.txtScaleMin = new System.Windows.Forms.TextBox();
            this.txtScaleMax = new System.Windows.Forms.TextBox();
            this.comboDataType = new System.Windows.Forms.ComboBox();
            this.txtStartByte = new System.Windows.Forms.TextBox();
            this.txtNrOfElements = new System.Windows.Forms.TextBox();
            this.txtBitAddress = new System.Windows.Forms.TextBox();
            this.comboLogFreq = new System.Windows.Forms.ComboBox();
            this.txtUnit = new System.Windows.Forms.TextBox();
            this.checkActive = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblLogType = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblDeadband = new System.Windows.Forms.Label();
            this.lblVarType = new System.Windows.Forms.Label();
            this.lblBlock = new System.Windows.Forms.Label();
            this.lblScaleOffset = new System.Windows.Forms.Label();
            this.lblScaleMin = new System.Windows.Forms.Label();
            this.lblScaleMax = new System.Windows.Forms.Label();
            this.lblDataType = new System.Windows.Forms.Label();
            this.lblStartAddress = new System.Windows.Forms.Label();
            this.lblNrOfValues = new System.Windows.Forms.Label();
            this.lblBitAddress = new System.Windows.Forms.Label();
            this.lblFreq = new System.Windows.Forms.Label();
            this.lblUnit = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 487);
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
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtName.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(3, 26);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(400, 23);
            this.txtName.TabIndex = 2;
            this.toolTip.SetToolTip(this.txtName, "Taggens unika namn");
            // 
            // comboLogType
            // 
            this.comboLogType.Dock = System.Windows.Forms.DockStyle.Left;
            this.comboLogType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLogType.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.comboLogType.FormattingEnabled = true;
            this.comboLogType.Items.AddRange(new object[] {
            "Cyclic",
            "Delta",
            "TimeOfDay",
            "Event"});
            this.comboLogType.Location = new System.Drawing.Point(3, 78);
            this.comboLogType.Name = "comboLogType";
            this.comboLogType.Size = new System.Drawing.Size(400, 24);
            this.comboLogType.TabIndex = 3;
            this.toolTip.SetToolTip(this.comboLogType, "Hur ska taggen loggas");
            this.comboLogType.SelectedIndexChanged += new System.EventHandler(this.comboLogType_SelectedIndexChanged);
            // 
            // txtTimeOfDay
            // 
            this.txtTimeOfDay.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtTimeOfDay.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtTimeOfDay.Location = new System.Drawing.Point(3, 131);
            this.txtTimeOfDay.Name = "txtTimeOfDay";
            this.txtTimeOfDay.Size = new System.Drawing.Size(400, 23);
            this.txtTimeOfDay.TabIndex = 4;
            this.toolTip.SetToolTip(this.txtTimeOfDay, "Tid på dagen taggen ska loggas vid TimeOfDay [hh:mm:ss]");
            // 
            // txtDeadband
            // 
            this.txtDeadband.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtDeadband.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtDeadband.Location = new System.Drawing.Point(3, 183);
            this.txtDeadband.Name = "txtDeadband";
            this.txtDeadband.Size = new System.Drawing.Size(400, 23);
            this.txtDeadband.TabIndex = 5;
            this.toolTip.SetToolTip(this.txtDeadband, "Dödband vid Delta-loggning");
            // 
            // comboVarType
            // 
            this.comboVarType.Dock = System.Windows.Forms.DockStyle.Left;
            this.comboVarType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboVarType.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.comboVarType.FormattingEnabled = true;
            this.comboVarType.Items.AddRange(new object[] {
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
            "DateTimeLong"});
            this.comboVarType.Location = new System.Drawing.Point(3, 235);
            this.comboVarType.Name = "comboVarType";
            this.comboVarType.Size = new System.Drawing.Size(400, 24);
            this.comboVarType.TabIndex = 6;
            this.toolTip.SetToolTip(this.comboVarType, "Taggens variabeltyp");
            this.comboVarType.SelectedIndexChanged += new System.EventHandler(this.comboVarType_SelectedIndexChanged);
            // 
            // txtBlockNr
            // 
            this.txtBlockNr.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtBlockNr.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtBlockNr.Location = new System.Drawing.Point(3, 288);
            this.txtBlockNr.Name = "txtBlockNr";
            this.txtBlockNr.Size = new System.Drawing.Size(400, 23);
            this.txtBlockNr.TabIndex = 7;
            this.toolTip.SetToolTip(this.txtBlockNr, "DB-nummer");
            // 
            // txtScaleOffset
            // 
            this.txtScaleOffset.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtScaleOffset.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtScaleOffset.Location = new System.Drawing.Point(3, 340);
            this.txtScaleOffset.Name = "txtScaleOffset";
            this.txtScaleOffset.Size = new System.Drawing.Size(400, 23);
            this.txtScaleOffset.TabIndex = 8;
            this.toolTip.SetToolTip(this.txtScaleOffset, "DB-nummer");
            // 
            // txtScaleMin
            // 
            this.txtScaleMin.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtScaleMin.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtScaleMin.Location = new System.Drawing.Point(3, 392);
            this.txtScaleMin.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.txtScaleMin.Name = "txtScaleMin";
            this.txtScaleMin.Size = new System.Drawing.Size(400, 23);
            this.txtScaleMin.TabIndex = 9;
            this.toolTip.SetToolTip(this.txtScaleMin, "Enhet t.ex. Liter, %...");
            // 
            // txtScaleMax
            // 
            this.txtScaleMax.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtScaleMax.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtScaleMax.Location = new System.Drawing.Point(3, 444);
            this.txtScaleMax.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.txtScaleMax.Name = "txtScaleMax";
            this.txtScaleMax.Size = new System.Drawing.Size(400, 23);
            this.txtScaleMax.TabIndex = 10;
            this.toolTip.SetToolTip(this.txtScaleMax, "Enhet t.ex. Liter, %...");
            // 
            // comboDataType
            // 
            this.comboDataType.Dock = System.Windows.Forms.DockStyle.Right;
            this.comboDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDataType.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.comboDataType.FormattingEnabled = true;
            this.comboDataType.Items.AddRange(new object[] {
            "Counter",
            "Timer",
            "Input",
            "Output",
            "Memory",
            "DataBlock"});
            this.comboDataType.Location = new System.Drawing.Point(426, 26);
            this.comboDataType.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.comboDataType.Name = "comboDataType";
            this.comboDataType.Size = new System.Drawing.Size(400, 24);
            this.comboDataType.TabIndex = 11;
            this.toolTip.SetToolTip(this.comboDataType, "Vilken data-area ska taggen loggas från");
            // 
            // txtStartByte
            // 
            this.txtStartByte.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtStartByte.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtStartByte.Location = new System.Drawing.Point(426, 79);
            this.txtStartByte.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.txtStartByte.Name = "txtStartByte";
            this.txtStartByte.Size = new System.Drawing.Size(400, 23);
            this.txtStartByte.TabIndex = 12;
            this.toolTip.SetToolTip(this.txtStartByte, "Vilken address-offset (byte gällande DB)");
            // 
            // txtNrOfElements
            // 
            this.txtNrOfElements.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtNrOfElements.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtNrOfElements.Location = new System.Drawing.Point(426, 131);
            this.txtNrOfElements.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.txtNrOfElements.Name = "txtNrOfElements";
            this.txtNrOfElements.Size = new System.Drawing.Size(400, 23);
            this.txtNrOfElements.TabIndex = 13;
            this.toolTip.SetToolTip(this.txtNrOfElements, "Hur många värden framåt ska loggas");
            // 
            // txtBitAddress
            // 
            this.txtBitAddress.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtBitAddress.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtBitAddress.Location = new System.Drawing.Point(426, 183);
            this.txtBitAddress.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.txtBitAddress.Name = "txtBitAddress";
            this.txtBitAddress.Size = new System.Drawing.Size(400, 23);
            this.txtBitAddress.TabIndex = 14;
            this.toolTip.SetToolTip(this.txtBitAddress, "Vilken bit när man loggar boolean");
            // 
            // comboLogFreq
            // 
            this.comboLogFreq.Dock = System.Windows.Forms.DockStyle.Right;
            this.comboLogFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLogFreq.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.comboLogFreq.FormattingEnabled = true;
            this.comboLogFreq.ImeMode = System.Windows.Forms.ImeMode.On;
            this.comboLogFreq.ItemHeight = 16;
            this.comboLogFreq.Items.AddRange(new object[] {
            "Never",
            "_50ms",
            "_100ms",
            "_250ms",
            "_500ms",
            "_1s",
            "_2s",
            "_10s",
            "_30s",
            "_1m"});
            this.comboLogFreq.Location = new System.Drawing.Point(426, 235);
            this.comboLogFreq.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.comboLogFreq.Name = "comboLogFreq";
            this.comboLogFreq.Size = new System.Drawing.Size(400, 24);
            this.comboLogFreq.TabIndex = 49;
            this.toolTip.SetToolTip(this.comboLogFreq, "Hur ofta taggen läses från PLC");
            // 
            // txtUnit
            // 
            this.txtUnit.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtUnit.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtUnit.Location = new System.Drawing.Point(426, 288);
            this.txtUnit.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(400, 23);
            this.txtUnit.TabIndex = 16;
            this.toolTip.SetToolTip(this.txtUnit, "Enhet t.ex. Liter, %...");
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
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.txtName);
            this.flowLayoutPanel1.Controls.Add(this.lblLogType);
            this.flowLayoutPanel1.Controls.Add(this.comboLogType);
            this.flowLayoutPanel1.Controls.Add(this.lblTime);
            this.flowLayoutPanel1.Controls.Add(this.txtTimeOfDay);
            this.flowLayoutPanel1.Controls.Add(this.lblDeadband);
            this.flowLayoutPanel1.Controls.Add(this.txtDeadband);
            this.flowLayoutPanel1.Controls.Add(this.lblVarType);
            this.flowLayoutPanel1.Controls.Add(this.comboVarType);
            this.flowLayoutPanel1.Controls.Add(this.lblBlock);
            this.flowLayoutPanel1.Controls.Add(this.txtBlockNr);
            this.flowLayoutPanel1.Controls.Add(this.lblScaleOffset);
            this.flowLayoutPanel1.Controls.Add(this.txtScaleOffset);
            this.flowLayoutPanel1.Controls.Add(this.lblScaleMin);
            this.flowLayoutPanel1.Controls.Add(this.txtScaleMin);
            this.flowLayoutPanel1.Controls.Add(this.lblScaleMax);
            this.flowLayoutPanel1.Controls.Add(this.txtScaleMax);
            this.flowLayoutPanel1.Controls.Add(this.lblDataType);
            this.flowLayoutPanel1.Controls.Add(this.comboDataType);
            this.flowLayoutPanel1.Controls.Add(this.lblStartAddress);
            this.flowLayoutPanel1.Controls.Add(this.txtStartByte);
            this.flowLayoutPanel1.Controls.Add(this.lblNrOfValues);
            this.flowLayoutPanel1.Controls.Add(this.txtNrOfElements);
            this.flowLayoutPanel1.Controls.Add(this.lblBitAddress);
            this.flowLayoutPanel1.Controls.Add(this.txtBitAddress);
            this.flowLayoutPanel1.Controls.Add(this.lblFreq);
            this.flowLayoutPanel1.Controls.Add(this.comboLogFreq);
            this.flowLayoutPanel1.Controls.Add(this.lblUnit);
            this.flowLayoutPanel1.Controls.Add(this.txtUnit);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(838, 478);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.label1.Size = new System.Drawing.Size(68, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "Namn";
            // 
            // lblLogType
            // 
            this.lblLogType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLogType.AutoSize = true;
            this.lblLogType.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogType.Location = new System.Drawing.Point(3, 52);
            this.lblLogType.Name = "lblLogType";
            this.lblLogType.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblLogType.Size = new System.Drawing.Size(79, 23);
            this.lblLogType.TabIndex = 5;
            this.lblLogType.Text = "Log-typ";
            // 
            // lblTime
            // 
            this.lblTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.Location = new System.Drawing.Point(3, 105);
            this.lblTime.Name = "lblTime";
            this.lblTime.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblTime.Size = new System.Drawing.Size(42, 23);
            this.lblTime.TabIndex = 7;
            this.lblTime.Text = "Tid";
            // 
            // lblDeadband
            // 
            this.lblDeadband.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDeadband.AutoSize = true;
            this.lblDeadband.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeadband.Location = new System.Drawing.Point(3, 157);
            this.lblDeadband.Name = "lblDeadband";
            this.lblDeadband.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblDeadband.Size = new System.Drawing.Size(96, 23);
            this.lblDeadband.TabIndex = 9;
            this.lblDeadband.Text = "Dödband";
            // 
            // lblVarType
            // 
            this.lblVarType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVarType.AutoSize = true;
            this.lblVarType.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVarType.Location = new System.Drawing.Point(3, 209);
            this.lblVarType.Name = "lblVarType";
            this.lblVarType.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblVarType.Size = new System.Drawing.Size(111, 23);
            this.lblVarType.TabIndex = 11;
            this.lblVarType.Text = "Variabeltyp";
            // 
            // lblBlock
            // 
            this.lblBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBlock.AutoSize = true;
            this.lblBlock.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBlock.Location = new System.Drawing.Point(3, 262);
            this.lblBlock.Name = "lblBlock";
            this.lblBlock.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblBlock.Size = new System.Drawing.Size(130, 23);
            this.lblBlock.TabIndex = 13;
            this.lblBlock.Text = "Block-nummer";
            // 
            // lblScaleOffset
            // 
            this.lblScaleOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblScaleOffset.AutoSize = true;
            this.lblScaleOffset.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScaleOffset.Location = new System.Drawing.Point(3, 314);
            this.lblScaleOffset.Name = "lblScaleOffset";
            this.lblScaleOffset.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblScaleOffset.Size = new System.Drawing.Size(140, 23);
            this.lblScaleOffset.TabIndex = 34;
            this.lblScaleOffset.Text = "Skalering Offset";
            // 
            // lblScaleMin
            // 
            this.lblScaleMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblScaleMin.AutoSize = true;
            this.lblScaleMin.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.lblScaleMin.Location = new System.Drawing.Point(3, 366);
            this.lblScaleMin.Name = "lblScaleMin";
            this.lblScaleMin.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblScaleMin.Size = new System.Drawing.Size(121, 23);
            this.lblScaleMin.TabIndex = 36;
            this.lblScaleMin.Text = "Skalering Min";
            // 
            // lblScaleMax
            // 
            this.lblScaleMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblScaleMax.AutoSize = true;
            this.lblScaleMax.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.lblScaleMax.Location = new System.Drawing.Point(3, 418);
            this.lblScaleMax.Name = "lblScaleMax";
            this.lblScaleMax.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblScaleMax.Size = new System.Drawing.Size(127, 23);
            this.lblScaleMax.TabIndex = 38;
            this.lblScaleMax.Text = "Skalering Max";
            // 
            // lblDataType
            // 
            this.lblDataType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDataType.AutoSize = true;
            this.lblDataType.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDataType.Location = new System.Drawing.Point(426, 0);
            this.lblDataType.Name = "lblDataType";
            this.lblDataType.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblDataType.Size = new System.Drawing.Size(87, 23);
            this.lblDataType.TabIndex = 40;
            this.lblDataType.Text = "Datatyp";
            // 
            // lblStartAddress
            // 
            this.lblStartAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStartAddress.AutoSize = true;
            this.lblStartAddress.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStartAddress.Location = new System.Drawing.Point(426, 53);
            this.lblStartAddress.Name = "lblStartAddress";
            this.lblStartAddress.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblStartAddress.Size = new System.Drawing.Size(112, 23);
            this.lblStartAddress.TabIndex = 42;
            this.lblStartAddress.Text = "Start-adress";
            // 
            // lblNrOfValues
            // 
            this.lblNrOfValues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNrOfValues.AutoSize = true;
            this.lblNrOfValues.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNrOfValues.Location = new System.Drawing.Point(426, 105);
            this.lblNrOfValues.Name = "lblNrOfValues";
            this.lblNrOfValues.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblNrOfValues.Size = new System.Drawing.Size(125, 23);
            this.lblNrOfValues.TabIndex = 44;
            this.lblNrOfValues.Text = "Antal värden";
            // 
            // lblBitAddress
            // 
            this.lblBitAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBitAddress.AutoSize = true;
            this.lblBitAddress.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBitAddress.Location = new System.Drawing.Point(426, 157);
            this.lblBitAddress.Name = "lblBitAddress";
            this.lblBitAddress.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblBitAddress.Size = new System.Drawing.Size(93, 23);
            this.lblBitAddress.TabIndex = 46;
            this.lblBitAddress.Text = "Bit-adress";
            // 
            // lblFreq
            // 
            this.lblFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFreq.AutoSize = true;
            this.lblFreq.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFreq.Location = new System.Drawing.Point(426, 209);
            this.lblFreq.Name = "lblFreq";
            this.lblFreq.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblFreq.Size = new System.Drawing.Size(113, 23);
            this.lblFreq.TabIndex = 48;
            this.lblFreq.Text = "Läs-frekvens";
            // 
            // lblUnit
            // 
            this.lblUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUnit.AutoSize = true;
            this.lblUnit.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.lblUnit.Location = new System.Drawing.Point(426, 262);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Padding = new System.Windows.Forms.Padding(10, 0, 0, 2);
            this.lblUnit.Size = new System.Drawing.Size(120, 23);
            this.lblUnit.TabIndex = 50;
            this.lblUnit.Text = "Värde-enhet";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel2.Controls.Add(this.tableLayoutPanel1);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(1, 40);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(841, 600);
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
            // AddTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(62)))), ((int)(((byte)(71)))));
            this.ClientSize = new System.Drawing.Size(842, 629);
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
            this.flowLayoutPanel1.PerformLayout();
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Label lblLogType;
        private System.Windows.Forms.ComboBox comboLogType;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.TextBox txtTimeOfDay;
        private System.Windows.Forms.Label lblDeadband;
        private System.Windows.Forms.TextBox txtDeadband;
        private System.Windows.Forms.Label lblVarType;
        private System.Windows.Forms.ComboBox comboVarType;
        private System.Windows.Forms.Label lblBlock;
        private System.Windows.Forms.TextBox txtBlockNr;
        private System.Windows.Forms.Label lblScaleOffset;
        private System.Windows.Forms.TextBox txtScaleOffset;
        private System.Windows.Forms.Label lblScaleMin;
        private System.Windows.Forms.TextBox txtScaleMin;
        private System.Windows.Forms.Label lblScaleMax;
        private System.Windows.Forms.TextBox txtScaleMax;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.ComboBox comboDataType;
        private System.Windows.Forms.Label lblStartAddress;
        private System.Windows.Forms.TextBox txtStartByte;
        private System.Windows.Forms.Label lblNrOfValues;
        private System.Windows.Forms.TextBox txtNrOfElements;
        private System.Windows.Forms.Label lblBitAddress;
        private System.Windows.Forms.TextBox txtBitAddress;
        private System.Windows.Forms.Label lblFreq;
        private System.Windows.Forms.ComboBox comboLogFreq;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.TextBox txtUnit;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.CheckBox checkActive;
    }
}