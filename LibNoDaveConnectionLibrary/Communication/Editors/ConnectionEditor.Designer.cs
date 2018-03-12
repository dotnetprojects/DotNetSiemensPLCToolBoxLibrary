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
            this.txtWritePort = new System.Windows.Forms.TextBox();
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
            this.label4 = new System.Windows.Forms.Label();
            this.lstConnTypeRouting = new System.Windows.Forms.ComboBox();
            this.lblS7OnlineDevice = new System.Windows.Forms.Label();
            this.txtLIBNODAVECPUPort = new System.Windows.Forms.TextBox();
            this.lblLIBNODAVECPUPort = new System.Windows.Forms.Label();
            this.lblTimeoutDescr = new System.Windows.Forms.Label();
            this.lblTimeoutIPConnectDescr = new System.Windows.Forms.Label();
            this.cmdTest = new System.Windows.Forms.Button();
            this.lblState = new System.Windows.Forms.Label();
            this.lstConnType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkUseShortRequest = new System.Windows.Forms.CheckBox();
            this.tryConnect = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstConnectionList
            // 
            this.lstConnectionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstConnectionList.FormattingEnabled = true;
            resources.ApplyResources(this.lstConnectionList, "lstConnectionList");
            this.lstConnectionList.Name = "lstConnectionList";
            this.lstConnectionList.SelectedIndexChanged += new System.EventHandler(this.lstConnectionList_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cmdConnectionAdd
            // 
            resources.ApplyResources(this.cmdConnectionAdd, "cmdConnectionAdd");
            this.cmdConnectionAdd.Name = "cmdConnectionAdd";
            this.cmdConnectionAdd.UseVisualStyleBackColor = true;
            this.cmdConnectionAdd.Click += new System.EventHandler(this.cmdConnectionAdd_Click);
            // 
            // cmdConnectionDelete
            // 
            resources.ApplyResources(this.cmdConnectionDelete, "cmdConnectionDelete");
            this.cmdConnectionDelete.Name = "cmdConnectionDelete";
            this.cmdConnectionDelete.UseVisualStyleBackColor = true;
            this.cmdConnectionDelete.Click += new System.EventHandler(this.cmdConnectionDelete_Click);
            // 
            // lstLIBNODAVEConnectionType
            // 
            this.lstLIBNODAVEConnectionType.BackColor = System.Drawing.Color.White;
            this.lstLIBNODAVEConnectionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.lstLIBNODAVEConnectionType, "lstLIBNODAVEConnectionType");
            this.lstLIBNODAVEConnectionType.FormattingEnabled = true;
            this.lstLIBNODAVEConnectionType.Name = "lstLIBNODAVEConnectionType";
            this.lstLIBNODAVEConnectionType.SelectedIndexChanged += new System.EventHandler(this.lstLIBNODAVEConnectionType_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lblLIBNODAVEEntryPoint
            // 
            resources.ApplyResources(this.lblLIBNODAVEEntryPoint, "lblLIBNODAVEEntryPoint");
            this.lblLIBNODAVEEntryPoint.Name = "lblLIBNODAVEEntryPoint";
            // 
            // txtLIBNODAVECPURack
            // 
            resources.ApplyResources(this.txtLIBNODAVECPURack, "txtLIBNODAVECPURack");
            this.txtLIBNODAVECPURack.Name = "txtLIBNODAVECPURack";
            this.txtLIBNODAVECPURack.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPURack
            // 
            resources.ApplyResources(this.lblLIBNODAVECPURack, "lblLIBNODAVECPURack");
            this.lblLIBNODAVECPURack.Name = "lblLIBNODAVECPURack";
            // 
            // txtLIBNODAVECPUSlot
            // 
            resources.ApplyResources(this.txtLIBNODAVECPUSlot, "txtLIBNODAVECPUSlot");
            this.txtLIBNODAVECPUSlot.Name = "txtLIBNODAVECPUSlot";
            this.txtLIBNODAVECPUSlot.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUSlot
            // 
            resources.ApplyResources(this.lblLIBNODAVECPUSlot, "lblLIBNODAVECPUSlot");
            this.lblLIBNODAVECPUSlot.Name = "lblLIBNODAVECPUSlot";
            // 
            // txtLIBNODAVECPUIP
            // 
            resources.ApplyResources(this.txtLIBNODAVECPUIP, "txtLIBNODAVECPUIP");
            this.txtLIBNODAVECPUIP.Name = "txtLIBNODAVECPUIP";
            this.txtLIBNODAVECPUIP.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUIP
            // 
            resources.ApplyResources(this.lblLIBNODAVECPUIP, "lblLIBNODAVECPUIP");
            this.lblLIBNODAVECPUIP.Name = "lblLIBNODAVECPUIP";
            // 
            // lblLIBNODAVELokalCOMPort
            // 
            resources.ApplyResources(this.lblLIBNODAVELokalCOMPort, "lblLIBNODAVELokalCOMPort");
            this.lblLIBNODAVELokalCOMPort.Name = "lblLIBNODAVELokalCOMPort";
            this.lblLIBNODAVELokalCOMPort.Click += new System.EventHandler(this.lblLIBNODAVELokalCOMPort_Click);
            // 
            // txtLIBNODAVELokalMPI
            // 
            resources.ApplyResources(this.txtLIBNODAVELokalMPI, "txtLIBNODAVELokalMPI");
            this.txtLIBNODAVELokalMPI.Name = "txtLIBNODAVELokalMPI";
            this.txtLIBNODAVELokalMPI.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVELokalMPI
            // 
            resources.ApplyResources(this.lblLIBNODAVELokalMPI, "lblLIBNODAVELokalMPI");
            this.lblLIBNODAVELokalMPI.Name = "lblLIBNODAVELokalMPI";
            // 
            // txtLIBNODAVECPUMPI
            // 
            resources.ApplyResources(this.txtLIBNODAVECPUMPI, "txtLIBNODAVECPUMPI");
            this.txtLIBNODAVECPUMPI.Name = "txtLIBNODAVECPUMPI";
            this.txtLIBNODAVECPUMPI.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUMPI
            // 
            resources.ApplyResources(this.lblLIBNODAVECPUMPI, "lblLIBNODAVECPUMPI");
            this.lblLIBNODAVECPUMPI.Name = "lblLIBNODAVECPUMPI";
            // 
            // lstLIBNODAVEBusSpeed
            // 
            this.lstLIBNODAVEBusSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.lstLIBNODAVEBusSpeed, "lstLIBNODAVEBusSpeed");
            this.lstLIBNODAVEBusSpeed.FormattingEnabled = true;
            this.lstLIBNODAVEBusSpeed.Name = "lstLIBNODAVEBusSpeed";
            this.lstLIBNODAVEBusSpeed.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmdOK
            // 
            resources.ApplyResources(this.cmdOK, "cmdOK");
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdSave
            // 
            resources.ApplyResources(this.cmdSave, "cmdSave");
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // lblConnectionName
            // 
            this.lblConnectionName.BackColor = System.Drawing.Color.White;
            this.lblConnectionName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblConnectionName, "lblConnectionName");
            this.lblConnectionName.Name = "lblConnectionName";
            this.lblConnectionName.Click += new System.EventHandler(this.lblConnectionName_Click);
            // 
            // lstLIBNODAVELokalCOMPort
            // 
            this.lstLIBNODAVELokalCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.lstLIBNODAVELokalCOMPort, "lstLIBNODAVELokalCOMPort");
            this.lstLIBNODAVELokalCOMPort.FormattingEnabled = true;
            this.lstLIBNODAVELokalCOMPort.Name = "lstLIBNODAVELokalCOMPort";
            this.lstLIBNODAVELokalCOMPort.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVEBusSpeed
            // 
            resources.ApplyResources(this.lblLIBNODAVEBusSpeed, "lblLIBNODAVEBusSpeed");
            this.lblLIBNODAVEBusSpeed.Name = "lblLIBNODAVEBusSpeed";
            // 
            // cmdUndo
            // 
            resources.ApplyResources(this.cmdUndo, "cmdUndo");
            this.cmdUndo.Name = "cmdUndo";
            this.cmdUndo.UseVisualStyleBackColor = true;
            this.cmdUndo.Click += new System.EventHandler(this.cmdUndo_Click);
            // 
            // chkNetlinkReset
            // 
            resources.ApplyResources(this.chkNetlinkReset, "chkNetlinkReset");
            this.chkNetlinkReset.Name = "chkNetlinkReset";
            this.toolTip.SetToolTip(this.chkNetlinkReset, resources.GetString("chkNetlinkReset.ToolTip"));
            this.chkNetlinkReset.UseVisualStyleBackColor = true;
            this.chkNetlinkReset.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblTimeout
            // 
            resources.ApplyResources(this.lblTimeout, "lblTimeout");
            this.lblTimeout.Name = "lblTimeout";
            this.toolTip.SetToolTip(this.lblTimeout, resources.GetString("lblTimeout.ToolTip"));
            // 
            // txtTimeout
            // 
            resources.ApplyResources(this.txtTimeout, "txtTimeout");
            this.txtTimeout.Name = "txtTimeout";
            this.toolTip.SetToolTip(this.txtTimeout, resources.GetString("txtTimeout.ToolTip"));
            this.txtTimeout.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // txtTimeoutIPConnect
            // 
            resources.ApplyResources(this.txtTimeoutIPConnect, "txtTimeoutIPConnect");
            this.txtTimeoutIPConnect.Name = "txtTimeoutIPConnect";
            this.toolTip.SetToolTip(this.txtTimeoutIPConnect, resources.GetString("txtTimeoutIPConnect.ToolTip"));
            this.txtTimeoutIPConnect.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblTimeoutIPConnect
            // 
            resources.ApplyResources(this.lblTimeoutIPConnect, "lblTimeoutIPConnect");
            this.lblTimeoutIPConnect.Name = "lblTimeoutIPConnect";
            this.toolTip.SetToolTip(this.lblTimeoutIPConnect, resources.GetString("lblTimeoutIPConnect.ToolTip"));
            // 
            // txtWritePort
            // 
            resources.ApplyResources(this.txtWritePort, "txtWritePort");
            this.txtWritePort.Name = "txtWritePort";
            this.toolTip.SetToolTip(this.txtWritePort, resources.GetString("txtWritePort.ToolTip"));
            this.txtWritePort.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // btnConfigEntryPoint
            // 
            resources.ApplyResources(this.btnConfigEntryPoint, "btnConfigEntryPoint");
            this.btnConfigEntryPoint.Name = "btnConfigEntryPoint";
            this.btnConfigEntryPoint.UseVisualStyleBackColor = true;
            this.btnConfigEntryPoint.Click += new System.EventHandler(this.btnConfigEntryPoint_Click);
            // 
            // lstListEntryPoints
            // 
            this.lstListEntryPoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.lstListEntryPoints, "lstListEntryPoints");
            this.lstListEntryPoints.FormattingEnabled = true;
            this.lstListEntryPoints.Name = "lstListEntryPoints";
            this.lstListEntryPoints.SelectedIndexChanged += new System.EventHandler(this.lstListEntryPoints_SelectedIndexChanged);
            // 
            // lblLIBNODAVELokalComSpeed
            // 
            resources.ApplyResources(this.lblLIBNODAVELokalComSpeed, "lblLIBNODAVELokalComSpeed");
            this.lblLIBNODAVELokalComSpeed.Name = "lblLIBNODAVELokalComSpeed";
            // 
            // lstLIBNODAVELokalComSpeed
            // 
            this.lstLIBNODAVELokalComSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.lstLIBNODAVELokalComSpeed, "lstLIBNODAVELokalComSpeed");
            this.lstLIBNODAVELokalComSpeed.FormattingEnabled = true;
            this.lstLIBNODAVELokalComSpeed.Items.AddRange(new object[] {
            resources.GetString("lstLIBNODAVELokalComSpeed.Items"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items1"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items2"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items3"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items4"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items5"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items6"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items7"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items8"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items9"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items10"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items11"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items12"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items13"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items14"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items15"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items16"),
            resources.GetString("lstLIBNODAVELokalComSpeed.Items17")});
            this.lstLIBNODAVELokalComSpeed.Name = "lstLIBNODAVELokalComSpeed";
            this.lstLIBNODAVELokalComSpeed.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVELokalComParity
            // 
            resources.ApplyResources(this.lblLIBNODAVELokalComParity, "lblLIBNODAVELokalComParity");
            this.lblLIBNODAVELokalComParity.Name = "lblLIBNODAVELokalComParity";
            // 
            // lstLIBNODAVELokalComParity
            // 
            this.lstLIBNODAVELokalComParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.lstLIBNODAVELokalComParity, "lstLIBNODAVELokalComParity");
            this.lstLIBNODAVELokalComParity.FormattingEnabled = true;
            this.lstLIBNODAVELokalComParity.Items.AddRange(new object[] {
            resources.GetString("lstLIBNODAVELokalComParity.Items"),
            resources.GetString("lstLIBNODAVELokalComParity.Items1"),
            resources.GetString("lstLIBNODAVELokalComParity.Items2")});
            this.lstLIBNODAVELokalComParity.Name = "lstLIBNODAVELokalComParity";
            this.lstLIBNODAVELokalComParity.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // txtRoutingDestination
            // 
            resources.ApplyResources(this.txtRoutingDestination, "txtRoutingDestination");
            this.txtRoutingDestination.Name = "txtRoutingDestination";
            this.txtRoutingDestination.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingDestination
            // 
            resources.ApplyResources(this.lblRoutingDestination, "lblRoutingDestination");
            this.lblRoutingDestination.Name = "lblRoutingDestination";
            // 
            // chkRouting
            // 
            resources.ApplyResources(this.chkRouting, "chkRouting");
            this.chkRouting.Name = "chkRouting";
            this.chkRouting.UseVisualStyleBackColor = true;
            this.chkRouting.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // txtRoutingSubnetFirst
            // 
            resources.ApplyResources(this.txtRoutingSubnetFirst, "txtRoutingSubnetFirst");
            this.txtRoutingSubnetFirst.Name = "txtRoutingSubnetFirst";
            this.txtRoutingSubnetFirst.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingSubnet
            // 
            resources.ApplyResources(this.lblRoutingSubnet, "lblRoutingSubnet");
            this.lblRoutingSubnet.Name = "lblRoutingSubnet";
            // 
            // txtRoutingSubnetSecond
            // 
            resources.ApplyResources(this.txtRoutingSubnetSecond, "txtRoutingSubnetSecond");
            this.txtRoutingSubnetSecond.Name = "txtRoutingSubnetSecond";
            this.txtRoutingSubnetSecond.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingMinus
            // 
            resources.ApplyResources(this.lblRoutingMinus, "lblRoutingMinus");
            this.lblRoutingMinus.Name = "lblRoutingMinus";
            // 
            // txtRoutingRack
            // 
            resources.ApplyResources(this.txtRoutingRack, "txtRoutingRack");
            this.txtRoutingRack.Name = "txtRoutingRack";
            this.txtRoutingRack.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingRack
            // 
            resources.ApplyResources(this.lblRoutingRack, "lblRoutingRack");
            this.lblRoutingRack.Name = "lblRoutingRack";
            // 
            // txtRoutingSlot
            // 
            resources.ApplyResources(this.txtRoutingSlot, "txtRoutingSlot");
            this.txtRoutingSlot.Name = "txtRoutingSlot";
            this.txtRoutingSlot.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingSlot
            // 
            resources.ApplyResources(this.lblRoutingSlot, "lblRoutingSlot");
            this.lblRoutingSlot.Name = "lblRoutingSlot";
            this.lblRoutingSlot.Click += new System.EventHandler(this.lblRoutingSlot_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lstConnTypeRouting);
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
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lstConnTypeRouting
            // 
            this.lstConnTypeRouting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.lstConnTypeRouting, "lstConnTypeRouting");
            this.lstConnTypeRouting.FormattingEnabled = true;
            this.lstConnTypeRouting.Items.AddRange(new object[] {
            resources.GetString("lstConnTypeRouting.Items"),
            resources.GetString("lstConnTypeRouting.Items1"),
            resources.GetString("lstConnTypeRouting.Items2")});
            this.lstConnTypeRouting.Name = "lstConnTypeRouting";
            this.lstConnTypeRouting.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblS7OnlineDevice
            // 
            resources.ApplyResources(this.lblS7OnlineDevice, "lblS7OnlineDevice");
            this.lblS7OnlineDevice.Name = "lblS7OnlineDevice";
            // 
            // txtLIBNODAVECPUPort
            // 
            resources.ApplyResources(this.txtLIBNODAVECPUPort, "txtLIBNODAVECPUPort");
            this.txtLIBNODAVECPUPort.Name = "txtLIBNODAVECPUPort";
            this.txtLIBNODAVECPUPort.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUPort
            // 
            resources.ApplyResources(this.lblLIBNODAVECPUPort, "lblLIBNODAVECPUPort");
            this.lblLIBNODAVECPUPort.Name = "lblLIBNODAVECPUPort";
            // 
            // lblTimeoutDescr
            // 
            resources.ApplyResources(this.lblTimeoutDescr, "lblTimeoutDescr");
            this.lblTimeoutDescr.Name = "lblTimeoutDescr";
            // 
            // lblTimeoutIPConnectDescr
            // 
            resources.ApplyResources(this.lblTimeoutIPConnectDescr, "lblTimeoutIPConnectDescr");
            this.lblTimeoutIPConnectDescr.Name = "lblTimeoutIPConnectDescr";
            // 
            // cmdTest
            // 
            resources.ApplyResources(this.cmdTest, "cmdTest");
            this.cmdTest.Name = "cmdTest";
            this.cmdTest.UseVisualStyleBackColor = true;
            this.cmdTest.Click += new System.EventHandler(this.cmdTest_Click);
            // 
            // lblState
            // 
            this.lblState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblState, "lblState");
            this.lblState.Name = "lblState";
            // 
            // lstConnType
            // 
            this.lstConnType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.lstConnType, "lstConnType");
            this.lstConnType.FormattingEnabled = true;
            this.lstConnType.Items.AddRange(new object[] {
            resources.GetString("lstConnType.Items"),
            resources.GetString("lstConnType.Items1"),
            resources.GetString("lstConnType.Items2")});
            this.lstConnType.Name = "lstConnType";
            this.lstConnType.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // chkUseShortRequest
            // 
            resources.ApplyResources(this.chkUseShortRequest, "chkUseShortRequest");
            this.chkUseShortRequest.Name = "chkUseShortRequest";
            this.chkUseShortRequest.UseVisualStyleBackColor = true;
            this.chkUseShortRequest.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // tryConnect
            // 
            this.tryConnect.DoWork += new System.ComponentModel.DoWorkEventHandler(this.tryConnect_DoWork);
            // 
            // ConnectionEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.txtWritePort);
            this.Controls.Add(this.chkUseShortRequest);
            this.Controls.Add(this.lstConnType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblState);
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
            this.Controls.Add(this.cmdTest);
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
        private System.Windows.Forms.Button cmdTest;
        private System.Windows.Forms.Label lblState;
        private System.ComponentModel.BackgroundWorker tryConnect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox lstConnTypeRouting;
        private System.Windows.Forms.ComboBox lstConnType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkUseShortRequest;
        private System.Windows.Forms.TextBox txtWritePort;
    }
}