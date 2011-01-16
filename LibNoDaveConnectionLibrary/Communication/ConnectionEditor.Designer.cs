namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    partial class ConnectionEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionEditor));
            this.lstConnectionList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdConnectionAdd = new System.Windows.Forms.Button();
            this.cmdConnectionDelete = new System.Windows.Forms.Button();
            this.lstLIBNODAVEConnectionType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblLIBNODAVEEntryPoint = new System.Windows.Forms.Label();
            this.txtLIBNODAVECPURack = new System.Windows.Forms.TextBox();
            this.lblLIBNODAVECPURack = new System.Windows.Forms.Label();
            this.txtLIBNODAVECPUSlot = new System.Windows.Forms.TextBox();
            this.lblLIBNODAVECPUSlot = new System.Windows.Forms.Label();
            this.txtLIBNODAVECPUIP = new System.Windows.Forms.TextBox();
            this.lblLIBNODAVECPUIP = new System.Windows.Forms.Label();
            this.lblLIBNODAVELokalCOMPort = new System.Windows.Forms.Label();
            this.txtLIBNODAVELokalMPI = new System.Windows.Forms.TextBox();
            this.lblLIBNODAVELokalMPI = new System.Windows.Forms.Label();
            this.txtLIBNODAVECPUMPI = new System.Windows.Forms.TextBox();
            this.lblLIBNODAVECPUMPI = new System.Windows.Forms.Label();
            this.lstLIBNODAVEBusSpeed = new System.Windows.Forms.ComboBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdSave = new System.Windows.Forms.Button();
            this.lblConnectionName = new System.Windows.Forms.Label();
            this.lstLIBNODAVELokalCOMPort = new System.Windows.Forms.ComboBox();
            this.lblLIBNODAVEBusSpeed = new System.Windows.Forms.Label();
            this.cmdUndo = new System.Windows.Forms.Button();
            this.chkNetlinkReset = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblTimeout = new System.Windows.Forms.Label();
            this.txtTimeout = new System.Windows.Forms.TextBox();
            this.txtTimeoutIPConnect = new System.Windows.Forms.TextBox();
            this.lblTimeoutIPConnect = new System.Windows.Forms.Label();
            this.btnConfigEntryPoint = new System.Windows.Forms.Button();
            this.lstListEntryPoints = new System.Windows.Forms.ComboBox();
            this.lblLIBNODAVELokalComSpeed = new System.Windows.Forms.Label();
            this.lstLIBNODAVELokalComSpeed = new System.Windows.Forms.ComboBox();
            this.lblLIBNODAVELokalComParity = new System.Windows.Forms.Label();
            this.lstLIBNODAVELokalComParity = new System.Windows.Forms.ComboBox();
            this.txtRoutingDestination = new System.Windows.Forms.TextBox();
            this.lblRoutingDestination = new System.Windows.Forms.Label();
            this.chkRouting = new System.Windows.Forms.CheckBox();
            this.txtRoutingSubnetFirst = new System.Windows.Forms.TextBox();
            this.lblRoutingSubnet = new System.Windows.Forms.Label();
            this.txtRoutingSubnetSecond = new System.Windows.Forms.TextBox();
            this.lblRoutingMinus = new System.Windows.Forms.Label();
            this.txtRoutingRack = new System.Windows.Forms.TextBox();
            this.lblRoutingRack = new System.Windows.Forms.Label();
            this.txtRoutingSlot = new System.Windows.Forms.TextBox();
            this.lblRoutingSlot = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblS7OnlineDevice = new System.Windows.Forms.Label();
            this.txtLIBNODAVECPUPort = new System.Windows.Forms.TextBox();
            this.lblLIBNODAVECPUPort = new System.Windows.Forms.Label();
            this.lblTimeoutDescr = new System.Windows.Forms.Label();
            this.lblTimeoutIPConnectDescr = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstConnectionList
            // 
            this.lstConnectionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstConnectionList.FormattingEnabled = true;
            this.lstConnectionList.Location = new System.Drawing.Point(14, 27);
            this.lstConnectionList.Name = "lstConnectionList";
            this.lstConnectionList.Size = new System.Drawing.Size(277, 21);
            this.lstConnectionList.TabIndex = 0;
            this.lstConnectionList.SelectedIndexChanged += new System.EventHandler(this.lstConnectionList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Verbindungen";
            // 
            // cmdConnectionAdd
            // 
            this.cmdConnectionAdd.Location = new System.Drawing.Point(297, 18);
            this.cmdConnectionAdd.Name = "cmdConnectionAdd";
            this.cmdConnectionAdd.Size = new System.Drawing.Size(76, 19);
            this.cmdConnectionAdd.TabIndex = 26;
            this.cmdConnectionAdd.Text = "hinzufügen";
            this.cmdConnectionAdd.UseVisualStyleBackColor = true;
            this.cmdConnectionAdd.Click += new System.EventHandler(this.cmdConnectionAdd_Click);
            // 
            // cmdConnectionDelete
            // 
            this.cmdConnectionDelete.Location = new System.Drawing.Point(297, 43);
            this.cmdConnectionDelete.Name = "cmdConnectionDelete";
            this.cmdConnectionDelete.Size = new System.Drawing.Size(76, 19);
            this.cmdConnectionDelete.TabIndex = 27;
            this.cmdConnectionDelete.Text = "löschen";
            this.cmdConnectionDelete.UseVisualStyleBackColor = true;
            this.cmdConnectionDelete.Click += new System.EventHandler(this.cmdConnectionDelete_Click);
            // 
            // lstLIBNODAVEConnectionType
            // 
            this.lstLIBNODAVEConnectionType.BackColor = System.Drawing.Color.White;
            this.lstLIBNODAVEConnectionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstLIBNODAVEConnectionType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lstLIBNODAVEConnectionType.FormattingEnabled = true;
            this.lstLIBNODAVEConnectionType.Location = new System.Drawing.Point(14, 101);
            this.lstLIBNODAVEConnectionType.Name = "lstLIBNODAVEConnectionType";
            this.lstLIBNODAVEConnectionType.Size = new System.Drawing.Size(333, 21);
            this.lstLIBNODAVEConnectionType.TabIndex = 0;
            this.lstLIBNODAVEConnectionType.SelectedIndexChanged += new System.EventHandler(this.lstLIBNODAVEConnectionType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Protokoll";
            // 
            // lblLIBNODAVEEntryPoint
            // 
            this.lblLIBNODAVEEntryPoint.AutoSize = true;
            this.lblLIBNODAVEEntryPoint.Location = new System.Drawing.Point(44, 148);
            this.lblLIBNODAVEEntryPoint.Name = "lblLIBNODAVEEntryPoint";
            this.lblLIBNODAVEEntryPoint.Size = new System.Drawing.Size(76, 13);
            this.lblLIBNODAVEEntryPoint.TabIndex = 4;
            this.lblLIBNODAVEEntryPoint.Text = "Zugangspunkt";
            // 
            // txtLIBNODAVECPURack
            // 
            this.txtLIBNODAVECPURack.Location = new System.Drawing.Point(130, 185);
            this.txtLIBNODAVECPURack.Name = "txtLIBNODAVECPURack";
            this.txtLIBNODAVECPURack.Size = new System.Drawing.Size(49, 20);
            this.txtLIBNODAVECPURack.TabIndex = 4;
            this.txtLIBNODAVECPURack.Text = "2";
            this.txtLIBNODAVECPURack.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPURack
            // 
            this.lblLIBNODAVECPURack.AutoSize = true;
            this.lblLIBNODAVECPURack.Location = new System.Drawing.Point(48, 188);
            this.lblLIBNODAVECPURack.Name = "lblLIBNODAVECPURack";
            this.lblLIBNODAVECPURack.Size = new System.Drawing.Size(58, 13);
            this.lblLIBNODAVECPURack.TabIndex = 4;
            this.lblLIBNODAVECPURack.Text = "CPU-Rack";
            // 
            // txtLIBNODAVECPUSlot
            // 
            this.txtLIBNODAVECPUSlot.Location = new System.Drawing.Point(130, 211);
            this.txtLIBNODAVECPUSlot.Name = "txtLIBNODAVECPUSlot";
            this.txtLIBNODAVECPUSlot.Size = new System.Drawing.Size(49, 20);
            this.txtLIBNODAVECPUSlot.TabIndex = 5;
            this.txtLIBNODAVECPUSlot.Text = "3";
            this.txtLIBNODAVECPUSlot.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUSlot
            // 
            this.lblLIBNODAVECPUSlot.AutoSize = true;
            this.lblLIBNODAVECPUSlot.Location = new System.Drawing.Point(48, 214);
            this.lblLIBNODAVECPUSlot.Name = "lblLIBNODAVECPUSlot";
            this.lblLIBNODAVECPUSlot.Size = new System.Drawing.Size(50, 13);
            this.lblLIBNODAVECPUSlot.TabIndex = 4;
            this.lblLIBNODAVECPUSlot.Text = "CPU-Slot";
            // 
            // txtLIBNODAVECPUIP
            // 
            this.txtLIBNODAVECPUIP.Location = new System.Drawing.Point(130, 262);
            this.txtLIBNODAVECPUIP.Name = "txtLIBNODAVECPUIP";
            this.txtLIBNODAVECPUIP.Size = new System.Drawing.Size(97, 20);
            this.txtLIBNODAVECPUIP.TabIndex = 7;
            this.txtLIBNODAVECPUIP.Text = "192.168.1.1";
            this.txtLIBNODAVECPUIP.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUIP
            // 
            this.lblLIBNODAVECPUIP.AutoSize = true;
            this.lblLIBNODAVECPUIP.Location = new System.Drawing.Point(48, 265);
            this.lblLIBNODAVECPUIP.Name = "lblLIBNODAVECPUIP";
            this.lblLIBNODAVECPUIP.Size = new System.Drawing.Size(58, 13);
            this.lblLIBNODAVECPUIP.TabIndex = 4;
            this.lblLIBNODAVECPUIP.Text = "IP-Adresse";
            // 
            // lblLIBNODAVELokalCOMPort
            // 
            this.lblLIBNODAVELokalCOMPort.AutoSize = true;
            this.lblLIBNODAVELokalCOMPort.Location = new System.Drawing.Point(308, 235);
            this.lblLIBNODAVELokalCOMPort.Name = "lblLIBNODAVELokalCOMPort";
            this.lblLIBNODAVELokalCOMPort.Size = new System.Drawing.Size(53, 13);
            this.lblLIBNODAVELokalCOMPort.TabIndex = 4;
            this.lblLIBNODAVELokalCOMPort.Text = "COM-Port";
            this.lblLIBNODAVELokalCOMPort.Click += new System.EventHandler(this.lblLIBNODAVELokalCOMPort_Click);
            // 
            // txtLIBNODAVELokalMPI
            // 
            this.txtLIBNODAVELokalMPI.Location = new System.Drawing.Point(387, 206);
            this.txtLIBNODAVELokalMPI.Name = "txtLIBNODAVELokalMPI";
            this.txtLIBNODAVELokalMPI.Size = new System.Drawing.Size(49, 20);
            this.txtLIBNODAVELokalMPI.TabIndex = 12;
            this.txtLIBNODAVELokalMPI.Text = "0";
            this.txtLIBNODAVELokalMPI.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVELokalMPI
            // 
            this.lblLIBNODAVELokalMPI.AutoSize = true;
            this.lblLIBNODAVELokalMPI.Location = new System.Drawing.Point(308, 209);
            this.lblLIBNODAVELokalMPI.Name = "lblLIBNODAVELokalMPI";
            this.lblLIBNODAVELokalMPI.Size = new System.Drawing.Size(55, 13);
            this.lblLIBNODAVELokalMPI.TabIndex = 4;
            this.lblLIBNODAVELokalMPI.Text = "Lokal-MPI";
            // 
            // txtLIBNODAVECPUMPI
            // 
            this.txtLIBNODAVECPUMPI.Location = new System.Drawing.Point(130, 236);
            this.txtLIBNODAVECPUMPI.Name = "txtLIBNODAVECPUMPI";
            this.txtLIBNODAVECPUMPI.Size = new System.Drawing.Size(49, 20);
            this.txtLIBNODAVECPUMPI.TabIndex = 6;
            this.txtLIBNODAVECPUMPI.Text = "2";
            this.txtLIBNODAVECPUMPI.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUMPI
            // 
            this.lblLIBNODAVECPUMPI.AutoSize = true;
            this.lblLIBNODAVECPUMPI.Location = new System.Drawing.Point(48, 239);
            this.lblLIBNODAVECPUMPI.Name = "lblLIBNODAVECPUMPI";
            this.lblLIBNODAVECPUMPI.Size = new System.Drawing.Size(51, 13);
            this.lblLIBNODAVECPUMPI.TabIndex = 4;
            this.lblLIBNODAVECPUMPI.Text = "CPU-MPI";
            // 
            // lstLIBNODAVEBusSpeed
            // 
            this.lstLIBNODAVEBusSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstLIBNODAVEBusSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lstLIBNODAVEBusSpeed.FormattingEnabled = true;
            this.lstLIBNODAVEBusSpeed.Location = new System.Drawing.Point(130, 314);
            this.lstLIBNODAVEBusSpeed.Name = "lstLIBNODAVEBusSpeed";
            this.lstLIBNODAVEBusSpeed.Size = new System.Drawing.Size(131, 21);
            this.lstLIBNODAVEBusSpeed.TabIndex = 9;
            this.lstLIBNODAVEBusSpeed.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(452, 10);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(89, 38);
            this.cmdOK.TabIndex = 25;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(362, 101);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(73, 21);
            this.cmdSave.TabIndex = 23;
            this.cmdSave.Text = "speichern";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Visible = false;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // lblConnectionName
            // 
            this.lblConnectionName.BackColor = System.Drawing.Color.White;
            this.lblConnectionName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblConnectionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionName.Location = new System.Drawing.Point(14, 27);
            this.lblConnectionName.Name = "lblConnectionName";
            this.lblConnectionName.Size = new System.Drawing.Size(241, 21);
            this.lblConnectionName.TabIndex = 7;
            this.lblConnectionName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblConnectionName.Visible = false;
            this.lblConnectionName.Click += new System.EventHandler(this.lblConnectionName_Click);
            // 
            // lstLIBNODAVELokalCOMPort
            // 
            this.lstLIBNODAVELokalCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstLIBNODAVELokalCOMPort.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lstLIBNODAVELokalCOMPort.FormattingEnabled = true;
            this.lstLIBNODAVELokalCOMPort.Location = new System.Drawing.Point(387, 231);
            this.lstLIBNODAVELokalCOMPort.Name = "lstLIBNODAVELokalCOMPort";
            this.lstLIBNODAVELokalCOMPort.Size = new System.Drawing.Size(124, 21);
            this.lstLIBNODAVELokalCOMPort.TabIndex = 13;
            this.lstLIBNODAVELokalCOMPort.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVEBusSpeed
            // 
            this.lblLIBNODAVEBusSpeed.AutoSize = true;
            this.lblLIBNODAVEBusSpeed.Location = new System.Drawing.Point(47, 317);
            this.lblLIBNODAVEBusSpeed.Name = "lblLIBNODAVEBusSpeed";
            this.lblLIBNODAVEBusSpeed.Size = new System.Drawing.Size(80, 13);
            this.lblLIBNODAVEBusSpeed.TabIndex = 4;
            this.lblLIBNODAVEBusSpeed.Text = "MPI/DP Speed";
            // 
            // cmdUndo
            // 
            this.cmdUndo.Location = new System.Drawing.Point(441, 101);
            this.cmdUndo.Name = "cmdUndo";
            this.cmdUndo.Size = new System.Drawing.Size(73, 21);
            this.cmdUndo.TabIndex = 24;
            this.cmdUndo.Text = "rückgänig";
            this.cmdUndo.UseVisualStyleBackColor = true;
            this.cmdUndo.Visible = false;
            this.cmdUndo.Click += new System.EventHandler(this.cmdUndo_Click);
            // 
            // chkNetlinkReset
            // 
            this.chkNetlinkReset.AutoSize = true;
            this.chkNetlinkReset.Location = new System.Drawing.Point(311, 317);
            this.chkNetlinkReset.Name = "chkNetlinkReset";
            this.chkNetlinkReset.Size = new System.Drawing.Size(188, 17);
            this.chkNetlinkReset.TabIndex = 16;
            this.chkNetlinkReset.Text = "Netlink beim aufbau immer reseten";
            this.toolTip.SetToolTip(this.chkNetlinkReset, resources.GetString("chkNetlinkReset.ToolTip"));
            this.chkNetlinkReset.UseVisualStyleBackColor = true;
            this.chkNetlinkReset.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(308, 175);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(45, 13);
            this.lblTimeout.TabIndex = 13;
            this.lblTimeout.Text = "Timeout";
            this.toolTip.SetToolTip(this.lblTimeout, "Timeout zum warten auf Daten von der CPU");
            // 
            // txtTimeout
            // 
            this.txtTimeout.Location = new System.Drawing.Point(387, 172);
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.Size = new System.Drawing.Size(90, 20);
            this.txtTimeout.TabIndex = 11;
            this.txtTimeout.Text = "5000000";
            this.toolTip.SetToolTip(this.txtTimeout, "Timeout zum warten auf Daten von der CPU");
            this.txtTimeout.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // txtTimeoutIPConnect
            // 
            this.txtTimeoutIPConnect.Location = new System.Drawing.Point(427, 148);
            this.txtTimeoutIPConnect.Name = "txtTimeoutIPConnect";
            this.txtTimeoutIPConnect.Size = new System.Drawing.Size(50, 20);
            this.txtTimeoutIPConnect.TabIndex = 10;
            this.txtTimeoutIPConnect.Text = "5000";
            this.toolTip.SetToolTip(this.txtTimeoutIPConnect, "Timeout welcher für den Connect zu einer IP benutzt wird");
            this.txtTimeoutIPConnect.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblTimeoutIPConnect
            // 
            this.lblTimeoutIPConnect.AutoSize = true;
            this.lblTimeoutIPConnect.Location = new System.Drawing.Point(308, 151);
            this.lblTimeoutIPConnect.Name = "lblTimeoutIPConnect";
            this.lblTimeoutIPConnect.Size = new System.Drawing.Size(101, 13);
            this.lblTimeoutIPConnect.TabIndex = 13;
            this.lblTimeoutIPConnect.Text = "Timeout IP Connect";
            this.toolTip.SetToolTip(this.lblTimeoutIPConnect, "Timeout welcher für den Connect zu einer IP benutzt wird");
            // 
            // btnConfigEntryPoint
            // 
            this.btnConfigEntryPoint.Location = new System.Drawing.Point(265, 145);
            this.btnConfigEntryPoint.Name = "btnConfigEntryPoint";
            this.btnConfigEntryPoint.Size = new System.Drawing.Size(26, 20);
            this.btnConfigEntryPoint.TabIndex = 3;
            this.btnConfigEntryPoint.Text = "...";
            this.btnConfigEntryPoint.UseVisualStyleBackColor = true;
            this.btnConfigEntryPoint.Click += new System.EventHandler(this.btnConfigEntryPoint_Click);
            // 
            // lstListEntryPoints
            // 
            this.lstListEntryPoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstListEntryPoints.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lstListEntryPoints.FormattingEnabled = true;
            this.lstListEntryPoints.Location = new System.Drawing.Point(130, 144);
            this.lstListEntryPoints.Name = "lstListEntryPoints";
            this.lstListEntryPoints.Size = new System.Drawing.Size(131, 21);
            this.lstListEntryPoints.TabIndex = 1;
            this.lstListEntryPoints.SelectedIndexChanged += new System.EventHandler(this.lstListEntryPoints_SelectedIndexChanged);
            // 
            // lblLIBNODAVELokalComSpeed
            // 
            this.lblLIBNODAVELokalComSpeed.AutoSize = true;
            this.lblLIBNODAVELokalComSpeed.Location = new System.Drawing.Point(308, 260);
            this.lblLIBNODAVELokalComSpeed.Name = "lblLIBNODAVELokalComSpeed";
            this.lblLIBNODAVELokalComSpeed.Size = new System.Drawing.Size(65, 13);
            this.lblLIBNODAVELokalComSpeed.TabIndex = 4;
            this.lblLIBNODAVELokalComSpeed.Text = "COM-Speed";
            // 
            // lstLIBNODAVELokalComSpeed
            // 
            this.lstLIBNODAVELokalComSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstLIBNODAVELokalComSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lstLIBNODAVELokalComSpeed.FormattingEnabled = true;
            this.lstLIBNODAVELokalComSpeed.Items.AddRange(new object[] {
            "75",
            "110",
            "134",
            "150",
            "300",
            "600",
            "1200",
            "1800",
            "2400",
            "4800",
            "7200",
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115200",
            "128000"});
            this.lstLIBNODAVELokalComSpeed.Location = new System.Drawing.Point(387, 256);
            this.lstLIBNODAVELokalComSpeed.Name = "lstLIBNODAVELokalComSpeed";
            this.lstLIBNODAVELokalComSpeed.Size = new System.Drawing.Size(112, 21);
            this.lstLIBNODAVELokalComSpeed.TabIndex = 14;
            this.lstLIBNODAVELokalComSpeed.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVELokalComParity
            // 
            this.lblLIBNODAVELokalComParity.AutoSize = true;
            this.lblLIBNODAVELokalComParity.Location = new System.Drawing.Point(308, 286);
            this.lblLIBNODAVELokalComParity.Name = "lblLIBNODAVELokalComParity";
            this.lblLIBNODAVELokalComParity.Size = new System.Drawing.Size(60, 13);
            this.lblLIBNODAVELokalComParity.TabIndex = 4;
            this.lblLIBNODAVELokalComParity.Text = "COM-Parity";
            // 
            // lstLIBNODAVELokalComParity
            // 
            this.lstLIBNODAVELokalComParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstLIBNODAVELokalComParity.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lstLIBNODAVELokalComParity.FormattingEnabled = true;
            this.lstLIBNODAVELokalComParity.Items.AddRange(new object[] {
            "none",
            "even",
            "odd"});
            this.lstLIBNODAVELokalComParity.Location = new System.Drawing.Point(387, 282);
            this.lstLIBNODAVELokalComParity.Name = "lstLIBNODAVELokalComParity";
            this.lstLIBNODAVELokalComParity.Size = new System.Drawing.Size(112, 21);
            this.lstLIBNODAVELokalComParity.TabIndex = 15;
            this.lstLIBNODAVELokalComParity.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // txtRoutingDestination
            // 
            this.txtRoutingDestination.Location = new System.Drawing.Point(155, 42);
            this.txtRoutingDestination.Name = "txtRoutingDestination";
            this.txtRoutingDestination.Size = new System.Drawing.Size(84, 20);
            this.txtRoutingDestination.TabIndex = 18;
            this.txtRoutingDestination.Text = "192.168.1.1";
            this.txtRoutingDestination.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingDestination
            // 
            this.lblRoutingDestination.AutoSize = true;
            this.lblRoutingDestination.Location = new System.Drawing.Point(37, 45);
            this.lblRoutingDestination.Name = "lblRoutingDestination";
            this.lblRoutingDestination.Size = new System.Drawing.Size(112, 13);
            this.lblRoutingDestination.TabIndex = 4;
            this.lblRoutingDestination.Text = "Ziel CPU (MPI/DP/IP)";
            // 
            // chkRouting
            // 
            this.chkRouting.AutoSize = true;
            this.chkRouting.Location = new System.Drawing.Point(14, 19);
            this.chkRouting.Name = "chkRouting";
            this.chkRouting.Size = new System.Drawing.Size(63, 17);
            this.chkRouting.TabIndex = 17;
            this.chkRouting.Text = "Routing";
            this.chkRouting.UseVisualStyleBackColor = true;
            this.chkRouting.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // txtRoutingSubnetFirst
            // 
            this.txtRoutingSubnetFirst.Location = new System.Drawing.Point(371, 42);
            this.txtRoutingSubnetFirst.MaxLength = 4;
            this.txtRoutingSubnetFirst.Name = "txtRoutingSubnetFirst";
            this.txtRoutingSubnetFirst.Size = new System.Drawing.Size(48, 20);
            this.txtRoutingSubnetFirst.TabIndex = 21;
            this.txtRoutingSubnetFirst.Text = "00A1";
            this.txtRoutingSubnetFirst.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtRoutingSubnetFirst.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingSubnet
            // 
            this.lblRoutingSubnet.AutoSize = true;
            this.lblRoutingSubnet.Location = new System.Drawing.Point(289, 45);
            this.lblRoutingSubnet.Name = "lblRoutingSubnet";
            this.lblRoutingSubnet.Size = new System.Drawing.Size(76, 13);
            this.lblRoutingSubnet.TabIndex = 4;
            this.lblRoutingSubnet.Text = "S7-Subnetz-ID";
            // 
            // txtRoutingSubnetSecond
            // 
            this.txtRoutingSubnetSecond.Location = new System.Drawing.Point(434, 42);
            this.txtRoutingSubnetSecond.MaxLength = 4;
            this.txtRoutingSubnetSecond.Name = "txtRoutingSubnetSecond";
            this.txtRoutingSubnetSecond.Size = new System.Drawing.Size(48, 20);
            this.txtRoutingSubnetSecond.TabIndex = 22;
            this.txtRoutingSubnetSecond.Text = "000A";
            this.txtRoutingSubnetSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtRoutingSubnetSecond.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingMinus
            // 
            this.lblRoutingMinus.AutoSize = true;
            this.lblRoutingMinus.Location = new System.Drawing.Point(388, 45);
            this.lblRoutingMinus.Name = "lblRoutingMinus";
            this.lblRoutingMinus.Size = new System.Drawing.Size(10, 13);
            this.lblRoutingMinus.TabIndex = 4;
            this.lblRoutingMinus.Text = "-";
            // 
            // txtRoutingRack
            // 
            this.txtRoutingRack.Location = new System.Drawing.Point(155, 68);
            this.txtRoutingRack.Name = "txtRoutingRack";
            this.txtRoutingRack.Size = new System.Drawing.Size(49, 20);
            this.txtRoutingRack.TabIndex = 19;
            this.txtRoutingRack.Text = "2";
            this.txtRoutingRack.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingRack
            // 
            this.lblRoutingRack.AutoSize = true;
            this.lblRoutingRack.Location = new System.Drawing.Point(116, 71);
            this.lblRoutingRack.Name = "lblRoutingRack";
            this.lblRoutingRack.Size = new System.Drawing.Size(33, 13);
            this.lblRoutingRack.TabIndex = 4;
            this.lblRoutingRack.Text = "Rack";
            // 
            // txtRoutingSlot
            // 
            this.txtRoutingSlot.Location = new System.Drawing.Point(155, 94);
            this.txtRoutingSlot.Name = "txtRoutingSlot";
            this.txtRoutingSlot.Size = new System.Drawing.Size(49, 20);
            this.txtRoutingSlot.TabIndex = 20;
            this.txtRoutingSlot.Text = "2";
            this.txtRoutingSlot.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingSlot
            // 
            this.lblRoutingSlot.AutoSize = true;
            this.lblRoutingSlot.Location = new System.Drawing.Point(116, 97);
            this.lblRoutingSlot.Name = "lblRoutingSlot";
            this.lblRoutingSlot.Size = new System.Drawing.Size(25, 13);
            this.lblRoutingSlot.TabIndex = 4;
            this.lblRoutingSlot.Text = "Slot";
            this.lblRoutingSlot.Click += new System.EventHandler(this.lblRoutingSlot_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkRouting);
            this.groupBox1.Controls.Add(this.txtRoutingDestination);
            this.groupBox1.Controls.Add(this.txtRoutingRack);
            this.groupBox1.Controls.Add(this.txtRoutingSlot);
            this.groupBox1.Controls.Add(this.txtRoutingSubnetFirst);
            this.groupBox1.Controls.Add(this.txtRoutingSubnetSecond);
            this.groupBox1.Controls.Add(this.lblRoutingDestination);
            this.groupBox1.Controls.Add(this.lblRoutingRack);
            this.groupBox1.Controls.Add(this.lblRoutingSlot);
            this.groupBox1.Controls.Add(this.lblRoutingSubnet);
            this.groupBox1.Controls.Add(this.lblRoutingMinus);
            this.groupBox1.Location = new System.Drawing.Point(14, 343);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(528, 123);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // lblS7OnlineDevice
            // 
            this.lblS7OnlineDevice.AutoSize = true;
            this.lblS7OnlineDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblS7OnlineDevice.Location = new System.Drawing.Point(134, 168);
            this.lblS7OnlineDevice.Name = "lblS7OnlineDevice";
            this.lblS7OnlineDevice.Size = new System.Drawing.Size(27, 9);
            this.lblS7OnlineDevice.TabIndex = 4;
            this.lblS7OnlineDevice.Text = "(none)";
            // 
            // txtLIBNODAVECPUPort
            // 
            this.txtLIBNODAVECPUPort.Location = new System.Drawing.Point(130, 288);
            this.txtLIBNODAVECPUPort.Name = "txtLIBNODAVECPUPort";
            this.txtLIBNODAVECPUPort.Size = new System.Drawing.Size(76, 20);
            this.txtLIBNODAVECPUPort.TabIndex = 8;
            this.txtLIBNODAVECPUPort.Text = "102";
            this.txtLIBNODAVECPUPort.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUPort
            // 
            this.lblLIBNODAVECPUPort.AutoSize = true;
            this.lblLIBNODAVECPUPort.Location = new System.Drawing.Point(48, 291);
            this.lblLIBNODAVECPUPort.Name = "lblLIBNODAVECPUPort";
            this.lblLIBNODAVECPUPort.Size = new System.Drawing.Size(26, 13);
            this.lblLIBNODAVECPUPort.TabIndex = 4;
            this.lblLIBNODAVECPUPort.Text = "Port";
            // 
            // lblTimeoutDescr
            // 
            this.lblTimeoutDescr.AutoSize = true;
            this.lblTimeoutDescr.Location = new System.Drawing.Point(483, 175);
            this.lblTimeoutDescr.Name = "lblTimeoutDescr";
            this.lblTimeoutDescr.Size = new System.Drawing.Size(20, 13);
            this.lblTimeoutDescr.TabIndex = 13;
            this.lblTimeoutDescr.Text = "µS";
            // 
            // lblTimeoutIPConnectDescr
            // 
            this.lblTimeoutIPConnectDescr.AutoSize = true;
            this.lblTimeoutIPConnectDescr.Location = new System.Drawing.Point(483, 151);
            this.lblTimeoutIPConnectDescr.Name = "lblTimeoutIPConnectDescr";
            this.lblTimeoutIPConnectDescr.Size = new System.Drawing.Size(22, 13);
            this.lblTimeoutIPConnectDescr.TabIndex = 13;
            this.lblTimeoutIPConnectDescr.Text = "mS";
            // 
            // ConnectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 478);
            this.ControlBox = false;
            this.Controls.Add(this.lblTimeoutIPConnectDescr);
            this.Controls.Add(this.lblTimeoutDescr);
            this.Controls.Add(this.lblTimeoutIPConnect);
            this.Controls.Add(this.lblTimeout);
            this.Controls.Add(this.txtTimeoutIPConnect);
            this.Controls.Add(this.txtTimeout);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lstListEntryPoints);
            this.Controls.Add(this.btnConfigEntryPoint);
            this.Controls.Add(this.chkNetlinkReset);
            this.Controls.Add(this.lblConnectionName);
            this.Controls.Add(this.cmdUndo);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.lstLIBNODAVELokalComParity);
            this.Controls.Add(this.lstLIBNODAVELokalComSpeed);
            this.Controls.Add(this.lstLIBNODAVELokalCOMPort);
            this.Controls.Add(this.lstLIBNODAVEBusSpeed);
            this.Controls.Add(this.lblLIBNODAVECPUSlot);
            this.Controls.Add(this.lblLIBNODAVECPURack);
            this.Controls.Add(this.lblLIBNODAVECPUMPI);
            this.Controls.Add(this.lblLIBNODAVELokalComParity);
            this.Controls.Add(this.lblLIBNODAVELokalMPI);
            this.Controls.Add(this.lblLIBNODAVELokalComSpeed);
            this.Controls.Add(this.lblLIBNODAVEBusSpeed);
            this.Controls.Add(this.lblLIBNODAVELokalCOMPort);
            this.Controls.Add(this.lblLIBNODAVECPUPort);
            this.Controls.Add(this.lblLIBNODAVECPUIP);
            this.Controls.Add(this.lblS7OnlineDevice);
            this.Controls.Add(this.lblLIBNODAVEEntryPoint);
            this.Controls.Add(this.txtLIBNODAVECPUSlot);
            this.Controls.Add(this.txtLIBNODAVECPURack);
            this.Controls.Add(this.txtLIBNODAVECPUMPI);
            this.Controls.Add(this.txtLIBNODAVELokalMPI);
            this.Controls.Add(this.txtLIBNODAVECPUPort);
            this.Controls.Add(this.txtLIBNODAVECPUIP);
            this.Controls.Add(this.cmdConnectionDelete);
            this.Controls.Add(this.cmdConnectionAdd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstLIBNODAVEConnectionType);
            this.Controls.Add(this.lstConnectionList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionEditor";
            this.Text = "PLC Connection Library - Connection Editor";
            this.Load += new System.EventHandler(this.ConnectionEditor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox lstConnectionList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdConnectionAdd;
        private System.Windows.Forms.Button cmdConnectionDelete;
        private System.Windows.Forms.ComboBox lstLIBNODAVEConnectionType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblLIBNODAVEEntryPoint;
        private System.Windows.Forms.TextBox txtLIBNODAVECPURack;
        private System.Windows.Forms.Label lblLIBNODAVECPURack;
        private System.Windows.Forms.TextBox txtLIBNODAVECPUSlot;
        private System.Windows.Forms.Label lblLIBNODAVECPUSlot;
        private System.Windows.Forms.TextBox txtLIBNODAVECPUIP;
        private System.Windows.Forms.Label lblLIBNODAVECPUIP;
        private System.Windows.Forms.Label lblLIBNODAVELokalCOMPort;
        private System.Windows.Forms.TextBox txtLIBNODAVELokalMPI;
        private System.Windows.Forms.Label lblLIBNODAVELokalMPI;
        private System.Windows.Forms.TextBox txtLIBNODAVECPUMPI;
        private System.Windows.Forms.Label lblLIBNODAVECPUMPI;
        private System.Windows.Forms.ComboBox lstLIBNODAVEBusSpeed;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Label lblConnectionName;
        private System.Windows.Forms.ComboBox lstLIBNODAVELokalCOMPort;
        private System.Windows.Forms.Label lblLIBNODAVEBusSpeed;
        private System.Windows.Forms.Button cmdUndo;
        private System.Windows.Forms.CheckBox chkNetlinkReset;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnConfigEntryPoint;
        private System.Windows.Forms.ComboBox lstListEntryPoints;
        private System.Windows.Forms.Label lblLIBNODAVELokalComSpeed;
        private System.Windows.Forms.ComboBox lstLIBNODAVELokalComSpeed;
        private System.Windows.Forms.Label lblLIBNODAVELokalComParity;
        private System.Windows.Forms.ComboBox lstLIBNODAVELokalComParity;
        private System.Windows.Forms.TextBox txtRoutingDestination;
        private System.Windows.Forms.Label lblRoutingDestination;
        private System.Windows.Forms.CheckBox chkRouting;
        private System.Windows.Forms.TextBox txtRoutingSubnetFirst;
        private System.Windows.Forms.Label lblRoutingSubnet;
        private System.Windows.Forms.TextBox txtRoutingSubnetSecond;
        private System.Windows.Forms.Label lblRoutingMinus;
        private System.Windows.Forms.TextBox txtRoutingRack;
        private System.Windows.Forms.Label lblRoutingRack;
        private System.Windows.Forms.TextBox txtRoutingSlot;
        private System.Windows.Forms.Label lblRoutingSlot;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblS7OnlineDevice;
        private System.Windows.Forms.TextBox txtLIBNODAVECPUPort;
        private System.Windows.Forms.Label lblLIBNODAVECPUPort;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.TextBox txtTimeout;
        private System.Windows.Forms.Label lblTimeoutDescr;
        private System.Windows.Forms.TextBox txtTimeoutIPConnect;
        private System.Windows.Forms.Label lblTimeoutIPConnect;
        private System.Windows.Forms.Label lblTimeoutIPConnectDescr;
    }
}