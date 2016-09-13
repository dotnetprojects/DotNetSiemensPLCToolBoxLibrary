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
            resources.ApplyResources(this.lstConnectionList, "lstConnectionList");
            this.lstConnectionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstConnectionList.FormattingEnabled = true;
            this.lstConnectionList.Name = "lstConnectionList";
            this.toolTip.SetToolTip(this.lstConnectionList, resources.GetString("lstConnectionList.ToolTip"));
            this.lstConnectionList.SelectedIndexChanged += new System.EventHandler(this.lstConnectionList_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // cmdConnectionAdd
            // 
            resources.ApplyResources(this.cmdConnectionAdd, "cmdConnectionAdd");
            this.cmdConnectionAdd.Name = "cmdConnectionAdd";
            this.toolTip.SetToolTip(this.cmdConnectionAdd, resources.GetString("cmdConnectionAdd.ToolTip"));
            this.cmdConnectionAdd.UseVisualStyleBackColor = true;
            this.cmdConnectionAdd.Click += new System.EventHandler(this.cmdConnectionAdd_Click);
            // 
            // cmdConnectionDelete
            // 
            resources.ApplyResources(this.cmdConnectionDelete, "cmdConnectionDelete");
            this.cmdConnectionDelete.Name = "cmdConnectionDelete";
            this.toolTip.SetToolTip(this.cmdConnectionDelete, resources.GetString("cmdConnectionDelete.ToolTip"));
            this.cmdConnectionDelete.UseVisualStyleBackColor = true;
            this.cmdConnectionDelete.Click += new System.EventHandler(this.cmdConnectionDelete_Click);
            // 
            // lstLIBNODAVEConnectionType
            // 
            resources.ApplyResources(this.lstLIBNODAVEConnectionType, "lstLIBNODAVEConnectionType");
            this.lstLIBNODAVEConnectionType.BackColor = System.Drawing.Color.White;
            this.lstLIBNODAVEConnectionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstLIBNODAVEConnectionType.FormattingEnabled = true;
            this.lstLIBNODAVEConnectionType.Name = "lstLIBNODAVEConnectionType";
            this.toolTip.SetToolTip(this.lstLIBNODAVEConnectionType, resources.GetString("lstLIBNODAVEConnectionType.ToolTip"));
            this.lstLIBNODAVEConnectionType.SelectedIndexChanged += new System.EventHandler(this.lstLIBNODAVEConnectionType_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // lblLIBNODAVEEntryPoint
            // 
            resources.ApplyResources(this.lblLIBNODAVEEntryPoint, "lblLIBNODAVEEntryPoint");
            this.lblLIBNODAVEEntryPoint.Name = "lblLIBNODAVEEntryPoint";
            this.toolTip.SetToolTip(this.lblLIBNODAVEEntryPoint, resources.GetString("lblLIBNODAVEEntryPoint.ToolTip"));
            // 
            // txtLIBNODAVECPURack
            // 
            resources.ApplyResources(this.txtLIBNODAVECPURack, "txtLIBNODAVECPURack");
            this.txtLIBNODAVECPURack.Name = "txtLIBNODAVECPURack";
            this.toolTip.SetToolTip(this.txtLIBNODAVECPURack, resources.GetString("txtLIBNODAVECPURack.ToolTip"));
            this.txtLIBNODAVECPURack.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPURack
            // 
            resources.ApplyResources(this.lblLIBNODAVECPURack, "lblLIBNODAVECPURack");
            this.lblLIBNODAVECPURack.Name = "lblLIBNODAVECPURack";
            this.toolTip.SetToolTip(this.lblLIBNODAVECPURack, resources.GetString("lblLIBNODAVECPURack.ToolTip"));
            // 
            // txtLIBNODAVECPUSlot
            // 
            resources.ApplyResources(this.txtLIBNODAVECPUSlot, "txtLIBNODAVECPUSlot");
            this.txtLIBNODAVECPUSlot.Name = "txtLIBNODAVECPUSlot";
            this.toolTip.SetToolTip(this.txtLIBNODAVECPUSlot, resources.GetString("txtLIBNODAVECPUSlot.ToolTip"));
            this.txtLIBNODAVECPUSlot.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUSlot
            // 
            resources.ApplyResources(this.lblLIBNODAVECPUSlot, "lblLIBNODAVECPUSlot");
            this.lblLIBNODAVECPUSlot.Name = "lblLIBNODAVECPUSlot";
            this.toolTip.SetToolTip(this.lblLIBNODAVECPUSlot, resources.GetString("lblLIBNODAVECPUSlot.ToolTip"));
            // 
            // txtLIBNODAVECPUIP
            // 
            resources.ApplyResources(this.txtLIBNODAVECPUIP, "txtLIBNODAVECPUIP");
            this.txtLIBNODAVECPUIP.Name = "txtLIBNODAVECPUIP";
            this.toolTip.SetToolTip(this.txtLIBNODAVECPUIP, resources.GetString("txtLIBNODAVECPUIP.ToolTip"));
            this.txtLIBNODAVECPUIP.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUIP
            // 
            resources.ApplyResources(this.lblLIBNODAVECPUIP, "lblLIBNODAVECPUIP");
            this.lblLIBNODAVECPUIP.Name = "lblLIBNODAVECPUIP";
            this.toolTip.SetToolTip(this.lblLIBNODAVECPUIP, resources.GetString("lblLIBNODAVECPUIP.ToolTip"));
            // 
            // lblLIBNODAVELokalCOMPort
            // 
            resources.ApplyResources(this.lblLIBNODAVELokalCOMPort, "lblLIBNODAVELokalCOMPort");
            this.lblLIBNODAVELokalCOMPort.Name = "lblLIBNODAVELokalCOMPort";
            this.toolTip.SetToolTip(this.lblLIBNODAVELokalCOMPort, resources.GetString("lblLIBNODAVELokalCOMPort.ToolTip"));
            this.lblLIBNODAVELokalCOMPort.Click += new System.EventHandler(this.lblLIBNODAVELokalCOMPort_Click);
            // 
            // txtLIBNODAVELokalMPI
            // 
            resources.ApplyResources(this.txtLIBNODAVELokalMPI, "txtLIBNODAVELokalMPI");
            this.txtLIBNODAVELokalMPI.Name = "txtLIBNODAVELokalMPI";
            this.toolTip.SetToolTip(this.txtLIBNODAVELokalMPI, resources.GetString("txtLIBNODAVELokalMPI.ToolTip"));
            this.txtLIBNODAVELokalMPI.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVELokalMPI
            // 
            resources.ApplyResources(this.lblLIBNODAVELokalMPI, "lblLIBNODAVELokalMPI");
            this.lblLIBNODAVELokalMPI.Name = "lblLIBNODAVELokalMPI";
            this.toolTip.SetToolTip(this.lblLIBNODAVELokalMPI, resources.GetString("lblLIBNODAVELokalMPI.ToolTip"));
            // 
            // txtLIBNODAVECPUMPI
            // 
            resources.ApplyResources(this.txtLIBNODAVECPUMPI, "txtLIBNODAVECPUMPI");
            this.txtLIBNODAVECPUMPI.Name = "txtLIBNODAVECPUMPI";
            this.toolTip.SetToolTip(this.txtLIBNODAVECPUMPI, resources.GetString("txtLIBNODAVECPUMPI.ToolTip"));
            this.txtLIBNODAVECPUMPI.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUMPI
            // 
            resources.ApplyResources(this.lblLIBNODAVECPUMPI, "lblLIBNODAVECPUMPI");
            this.lblLIBNODAVECPUMPI.Name = "lblLIBNODAVECPUMPI";
            this.toolTip.SetToolTip(this.lblLIBNODAVECPUMPI, resources.GetString("lblLIBNODAVECPUMPI.ToolTip"));
            // 
            // lstLIBNODAVEBusSpeed
            // 
            resources.ApplyResources(this.lstLIBNODAVEBusSpeed, "lstLIBNODAVEBusSpeed");
            this.lstLIBNODAVEBusSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstLIBNODAVEBusSpeed.FormattingEnabled = true;
            this.lstLIBNODAVEBusSpeed.Name = "lstLIBNODAVEBusSpeed";
            this.toolTip.SetToolTip(this.lstLIBNODAVEBusSpeed, resources.GetString("lstLIBNODAVEBusSpeed.ToolTip"));
            this.lstLIBNODAVEBusSpeed.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmdOK
            // 
            resources.ApplyResources(this.cmdOK, "cmdOK");
            this.cmdOK.Name = "cmdOK";
            this.toolTip.SetToolTip(this.cmdOK, resources.GetString("cmdOK.ToolTip"));
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdSave
            // 
            resources.ApplyResources(this.cmdSave, "cmdSave");
            this.cmdSave.Name = "cmdSave";
            this.toolTip.SetToolTip(this.cmdSave, resources.GetString("cmdSave.ToolTip"));
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // lblConnectionName
            // 
            resources.ApplyResources(this.lblConnectionName, "lblConnectionName");
            this.lblConnectionName.BackColor = System.Drawing.Color.White;
            this.lblConnectionName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblConnectionName.Name = "lblConnectionName";
            this.toolTip.SetToolTip(this.lblConnectionName, resources.GetString("lblConnectionName.ToolTip"));
            this.lblConnectionName.Click += new System.EventHandler(this.lblConnectionName_Click);
            // 
            // lstLIBNODAVELokalCOMPort
            // 
            resources.ApplyResources(this.lstLIBNODAVELokalCOMPort, "lstLIBNODAVELokalCOMPort");
            this.lstLIBNODAVELokalCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstLIBNODAVELokalCOMPort.FormattingEnabled = true;
            this.lstLIBNODAVELokalCOMPort.Name = "lstLIBNODAVELokalCOMPort";
            this.toolTip.SetToolTip(this.lstLIBNODAVELokalCOMPort, resources.GetString("lstLIBNODAVELokalCOMPort.ToolTip"));
            this.lstLIBNODAVELokalCOMPort.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVEBusSpeed
            // 
            resources.ApplyResources(this.lblLIBNODAVEBusSpeed, "lblLIBNODAVEBusSpeed");
            this.lblLIBNODAVEBusSpeed.Name = "lblLIBNODAVEBusSpeed";
            this.toolTip.SetToolTip(this.lblLIBNODAVEBusSpeed, resources.GetString("lblLIBNODAVEBusSpeed.ToolTip"));
            // 
            // cmdUndo
            // 
            resources.ApplyResources(this.cmdUndo, "cmdUndo");
            this.cmdUndo.Name = "cmdUndo";
            this.toolTip.SetToolTip(this.cmdUndo, resources.GetString("cmdUndo.ToolTip"));
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
            this.toolTip.SetToolTip(this.btnConfigEntryPoint, resources.GetString("btnConfigEntryPoint.ToolTip"));
            this.btnConfigEntryPoint.UseVisualStyleBackColor = true;
            this.btnConfigEntryPoint.Click += new System.EventHandler(this.btnConfigEntryPoint_Click);
            // 
            // lstListEntryPoints
            // 
            resources.ApplyResources(this.lstListEntryPoints, "lstListEntryPoints");
            this.lstListEntryPoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstListEntryPoints.FormattingEnabled = true;
            this.lstListEntryPoints.Name = "lstListEntryPoints";
            this.toolTip.SetToolTip(this.lstListEntryPoints, resources.GetString("lstListEntryPoints.ToolTip"));
            this.lstListEntryPoints.SelectedIndexChanged += new System.EventHandler(this.lstListEntryPoints_SelectedIndexChanged);
            // 
            // lblLIBNODAVELokalComSpeed
            // 
            resources.ApplyResources(this.lblLIBNODAVELokalComSpeed, "lblLIBNODAVELokalComSpeed");
            this.lblLIBNODAVELokalComSpeed.Name = "lblLIBNODAVELokalComSpeed";
            this.toolTip.SetToolTip(this.lblLIBNODAVELokalComSpeed, resources.GetString("lblLIBNODAVELokalComSpeed.ToolTip"));
            // 
            // lstLIBNODAVELokalComSpeed
            // 
            resources.ApplyResources(this.lstLIBNODAVELokalComSpeed, "lstLIBNODAVELokalComSpeed");
            this.lstLIBNODAVELokalComSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            this.toolTip.SetToolTip(this.lstLIBNODAVELokalComSpeed, resources.GetString("lstLIBNODAVELokalComSpeed.ToolTip"));
            this.lstLIBNODAVELokalComSpeed.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVELokalComParity
            // 
            resources.ApplyResources(this.lblLIBNODAVELokalComParity, "lblLIBNODAVELokalComParity");
            this.lblLIBNODAVELokalComParity.Name = "lblLIBNODAVELokalComParity";
            this.toolTip.SetToolTip(this.lblLIBNODAVELokalComParity, resources.GetString("lblLIBNODAVELokalComParity.ToolTip"));
            // 
            // lstLIBNODAVELokalComParity
            // 
            resources.ApplyResources(this.lstLIBNODAVELokalComParity, "lstLIBNODAVELokalComParity");
            this.lstLIBNODAVELokalComParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstLIBNODAVELokalComParity.FormattingEnabled = true;
            this.lstLIBNODAVELokalComParity.Items.AddRange(new object[] {
            resources.GetString("lstLIBNODAVELokalComParity.Items"),
            resources.GetString("lstLIBNODAVELokalComParity.Items1"),
            resources.GetString("lstLIBNODAVELokalComParity.Items2")});
            this.lstLIBNODAVELokalComParity.Name = "lstLIBNODAVELokalComParity";
            this.toolTip.SetToolTip(this.lstLIBNODAVELokalComParity, resources.GetString("lstLIBNODAVELokalComParity.ToolTip"));
            this.lstLIBNODAVELokalComParity.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // txtRoutingDestination
            // 
            resources.ApplyResources(this.txtRoutingDestination, "txtRoutingDestination");
            this.txtRoutingDestination.Name = "txtRoutingDestination";
            this.toolTip.SetToolTip(this.txtRoutingDestination, resources.GetString("txtRoutingDestination.ToolTip"));
            this.txtRoutingDestination.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingDestination
            // 
            resources.ApplyResources(this.lblRoutingDestination, "lblRoutingDestination");
            this.lblRoutingDestination.Name = "lblRoutingDestination";
            this.toolTip.SetToolTip(this.lblRoutingDestination, resources.GetString("lblRoutingDestination.ToolTip"));
            // 
            // chkRouting
            // 
            resources.ApplyResources(this.chkRouting, "chkRouting");
            this.chkRouting.Name = "chkRouting";
            this.toolTip.SetToolTip(this.chkRouting, resources.GetString("chkRouting.ToolTip"));
            this.chkRouting.UseVisualStyleBackColor = true;
            this.chkRouting.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // txtRoutingSubnetFirst
            // 
            resources.ApplyResources(this.txtRoutingSubnetFirst, "txtRoutingSubnetFirst");
            this.txtRoutingSubnetFirst.Name = "txtRoutingSubnetFirst";
            this.toolTip.SetToolTip(this.txtRoutingSubnetFirst, resources.GetString("txtRoutingSubnetFirst.ToolTip"));
            this.txtRoutingSubnetFirst.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingSubnet
            // 
            resources.ApplyResources(this.lblRoutingSubnet, "lblRoutingSubnet");
            this.lblRoutingSubnet.Name = "lblRoutingSubnet";
            this.toolTip.SetToolTip(this.lblRoutingSubnet, resources.GetString("lblRoutingSubnet.ToolTip"));
            // 
            // txtRoutingSubnetSecond
            // 
            resources.ApplyResources(this.txtRoutingSubnetSecond, "txtRoutingSubnetSecond");
            this.txtRoutingSubnetSecond.Name = "txtRoutingSubnetSecond";
            this.toolTip.SetToolTip(this.txtRoutingSubnetSecond, resources.GetString("txtRoutingSubnetSecond.ToolTip"));
            this.txtRoutingSubnetSecond.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingMinus
            // 
            resources.ApplyResources(this.lblRoutingMinus, "lblRoutingMinus");
            this.lblRoutingMinus.Name = "lblRoutingMinus";
            this.toolTip.SetToolTip(this.lblRoutingMinus, resources.GetString("lblRoutingMinus.ToolTip"));
            // 
            // txtRoutingRack
            // 
            resources.ApplyResources(this.txtRoutingRack, "txtRoutingRack");
            this.txtRoutingRack.Name = "txtRoutingRack";
            this.toolTip.SetToolTip(this.txtRoutingRack, resources.GetString("txtRoutingRack.ToolTip"));
            this.txtRoutingRack.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingRack
            // 
            resources.ApplyResources(this.lblRoutingRack, "lblRoutingRack");
            this.lblRoutingRack.Name = "lblRoutingRack";
            this.toolTip.SetToolTip(this.lblRoutingRack, resources.GetString("lblRoutingRack.ToolTip"));
            // 
            // txtRoutingSlot
            // 
            resources.ApplyResources(this.txtRoutingSlot, "txtRoutingSlot");
            this.txtRoutingSlot.Name = "txtRoutingSlot";
            this.toolTip.SetToolTip(this.txtRoutingSlot, resources.GetString("txtRoutingSlot.ToolTip"));
            this.txtRoutingSlot.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblRoutingSlot
            // 
            resources.ApplyResources(this.lblRoutingSlot, "lblRoutingSlot");
            this.lblRoutingSlot.Name = "lblRoutingSlot";
            this.toolTip.SetToolTip(this.lblRoutingSlot, resources.GetString("lblRoutingSlot.ToolTip"));
            this.lblRoutingSlot.Click += new System.EventHandler(this.lblRoutingSlot_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
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
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // lstConnTypeRouting
            // 
            resources.ApplyResources(this.lstConnTypeRouting, "lstConnTypeRouting");
            this.lstConnTypeRouting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstConnTypeRouting.FormattingEnabled = true;
            this.lstConnTypeRouting.Items.AddRange(new object[] {
            resources.GetString("lstConnTypeRouting.Items"),
            resources.GetString("lstConnTypeRouting.Items1"),
            resources.GetString("lstConnTypeRouting.Items2")});
            this.lstConnTypeRouting.Name = "lstConnTypeRouting";
            this.toolTip.SetToolTip(this.lstConnTypeRouting, resources.GetString("lstConnTypeRouting.ToolTip"));
            this.lstConnTypeRouting.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblS7OnlineDevice
            // 
            resources.ApplyResources(this.lblS7OnlineDevice, "lblS7OnlineDevice");
            this.lblS7OnlineDevice.Name = "lblS7OnlineDevice";
            this.toolTip.SetToolTip(this.lblS7OnlineDevice, resources.GetString("lblS7OnlineDevice.ToolTip"));
            // 
            // txtLIBNODAVECPUPort
            // 
            resources.ApplyResources(this.txtLIBNODAVECPUPort, "txtLIBNODAVECPUPort");
            this.txtLIBNODAVECPUPort.Name = "txtLIBNODAVECPUPort";
            this.toolTip.SetToolTip(this.txtLIBNODAVECPUPort, resources.GetString("txtLIBNODAVECPUPort.ToolTip"));
            this.txtLIBNODAVECPUPort.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblLIBNODAVECPUPort
            // 
            resources.ApplyResources(this.lblLIBNODAVECPUPort, "lblLIBNODAVECPUPort");
            this.lblLIBNODAVECPUPort.Name = "lblLIBNODAVECPUPort";
            this.toolTip.SetToolTip(this.lblLIBNODAVECPUPort, resources.GetString("lblLIBNODAVECPUPort.ToolTip"));
            // 
            // lblTimeoutDescr
            // 
            resources.ApplyResources(this.lblTimeoutDescr, "lblTimeoutDescr");
            this.lblTimeoutDescr.Name = "lblTimeoutDescr";
            this.toolTip.SetToolTip(this.lblTimeoutDescr, resources.GetString("lblTimeoutDescr.ToolTip"));
            // 
            // lblTimeoutIPConnectDescr
            // 
            resources.ApplyResources(this.lblTimeoutIPConnectDescr, "lblTimeoutIPConnectDescr");
            this.lblTimeoutIPConnectDescr.Name = "lblTimeoutIPConnectDescr";
            this.toolTip.SetToolTip(this.lblTimeoutIPConnectDescr, resources.GetString("lblTimeoutIPConnectDescr.ToolTip"));
            // 
            // cmdTest
            // 
            resources.ApplyResources(this.cmdTest, "cmdTest");
            this.cmdTest.Name = "cmdTest";
            this.toolTip.SetToolTip(this.cmdTest, resources.GetString("cmdTest.ToolTip"));
            this.cmdTest.UseVisualStyleBackColor = true;
            this.cmdTest.Click += new System.EventHandler(this.cmdTest_Click);
            // 
            // lblState
            // 
            resources.ApplyResources(this.lblState, "lblState");
            this.lblState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblState.Name = "lblState";
            this.toolTip.SetToolTip(this.lblState, resources.GetString("lblState.ToolTip"));
            // 
            // lstConnType
            // 
            resources.ApplyResources(this.lstConnType, "lstConnType");
            this.lstConnType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstConnType.FormattingEnabled = true;
            this.lstConnType.Items.AddRange(new object[] {
            resources.GetString("lstConnType.Items"),
            resources.GetString("lstConnType.Items1"),
            resources.GetString("lstConnType.Items2")});
            this.lstConnType.Name = "lstConnType";
            this.toolTip.SetToolTip(this.lstConnType, resources.GetString("lstConnType.ToolTip"));
            this.lstConnType.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // chkUseShortRequest
            // 
            resources.ApplyResources(this.chkUseShortRequest, "chkUseShortRequest");
            this.chkUseShortRequest.Name = "chkUseShortRequest";
            this.toolTip.SetToolTip(this.chkUseShortRequest, resources.GetString("chkUseShortRequest.ToolTip"));
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
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
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