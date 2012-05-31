using IPAddressControlLib;

namespace Kopplungstester
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.faTabStrip1 = new System.Windows.Forms.TabControl();
            this.faTabStripItemInfo = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.faTabStripItemSend = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cmdClearSend = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmdSend = new System.Windows.Forms.Button();
            this.lblTeleLength = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblLenEmpf = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.lstStoredSenddata = new System.Windows.Forms.ListBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cmdRemoveSendeTele = new System.Windows.Forms.Button();
            this.cmdAddSendeTele = new System.Windows.Forms.Button();
            this.dtaSendTabelle = new System.Windows.Forms.DataGridView();
            this.Bezeichnung = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Laenge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Wert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.dtaSendSendTable = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dtaSendQuittTable = new System.Windows.Forms.DataGridView();
            this.colSendQuittText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.faTabStripItemRecieve = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.kryptonButton2 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grdEmpfang = new System.Windows.Forms.DataGridView();
            this.colEmpf = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dtaEmpfangstelegrammAufgeschluesselt = new System.Windows.Forms.DataGridView();
            this.colEmpfBezeichnung = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmpfLaenge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmpfWert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.faTabStripItemSettings = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmdDelTele = new System.Windows.Forms.Button();
            this.cmdEditQuittFields = new System.Windows.Forms.Button();
            this.lstQuitt = new System.Windows.Forms.ListBox();
            this.chkAutoQuitt = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numSequenceNumberLength = new System.Windows.Forms.NumericUpDown();
            this.numSequenceNumberPosition = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkChanel2active = new System.Windows.Forms.CheckBox();
            this.numPort2 = new System.Windows.Forms.NumericUpDown();
            this.numPort1 = new System.Windows.Forms.NumericUpDown();
            this.lblKanal2Port = new System.Windows.Forms.Label();
            this.chkChanel1active = new System.Windows.Forms.CheckBox();
            this.lblKanal2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.optTwoChannel = new System.Windows.Forms.RadioButton();
            this.optOneChannel = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ipAddressControl = new IPAddressControlLib.IPAddressControl();
            this.cmdSelectStep7UDT = new System.Windows.Forms.Button();
            this.cmdConnect = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdSettExport = new System.Windows.Forms.Button();
            this.cmdSettImport = new System.Windows.Forms.Button();
            this.cmdSettingsSave = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.picConnection1 = new System.Windows.Forms.PictureBox();
            this.picConnection2 = new System.Windows.Forms.PictureBox();
            this.cmdDisconnect = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.faTabStrip1.SuspendLayout();
            this.faTabStripItemInfo.SuspendLayout();
            this.faTabStripItemSend.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtaSendTabelle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtaSendSendTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaSendQuittTable)).BeginInit();
            this.faTabStripItemRecieve.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdEmpfang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaEmpfangstelegrammAufgeschluesselt)).BeginInit();
            this.faTabStripItemSettings.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSequenceNumberLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSequenceNumberPosition)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picConnection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picConnection2)).BeginInit();
            this.SuspendLayout();
            // 
            // faTabStrip1
            // 
            this.faTabStrip1.Controls.Add(this.faTabStripItemInfo);
            this.faTabStrip1.Controls.Add(this.faTabStripItemSend);
            this.faTabStrip1.Controls.Add(this.faTabStripItemRecieve);
            this.faTabStrip1.Controls.Add(this.faTabStripItemSettings);
            this.faTabStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.faTabStrip1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.faTabStrip1.Location = new System.Drawing.Point(3, 53);
            this.faTabStrip1.Name = "faTabStrip1";
            this.faTabStrip1.SelectedIndex = 0;
            this.faTabStrip1.Size = new System.Drawing.Size(896, 569);
            this.faTabStrip1.TabIndex = 2;
            this.faTabStrip1.Text = "Sende Tabelle";
            // 
            // faTabStripItemInfo
            // 
            this.faTabStripItemInfo.Controls.Add(this.label10);
            this.faTabStripItemInfo.Controls.Add(this.label9);
            this.faTabStripItemInfo.Location = new System.Drawing.Point(4, 22);
            this.faTabStripItemInfo.Name = "faTabStripItemInfo";
            this.faTabStripItemInfo.Size = new System.Drawing.Size(888, 543);
            this.faTabStripItemInfo.TabIndex = 3;
            this.faTabStripItemInfo.Text = "Info";
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(74, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(726, 81);
            this.label10.TabIndex = 0;
            this.label10.Text = resources.GetString("label10.Text");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(39, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Info:";
            // 
            // faTabStripItemSend
            // 
            this.faTabStripItemSend.Controls.Add(this.tableLayoutPanel1);
            this.faTabStripItemSend.Location = new System.Drawing.Point(4, 22);
            this.faTabStripItemSend.Name = "faTabStripItemSend";
            this.faTabStripItemSend.Size = new System.Drawing.Size(888, 543);
            this.faTabStripItemSend.TabIndex = 0;
            this.faTabStripItemSend.Text = "Sende Tabelle";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.Controls.Add(this.cmdClearSend, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(888, 543);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // cmdClearSend
            // 
            this.cmdClearSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClearSend.Location = new System.Drawing.Point(741, 514);
            this.cmdClearSend.Name = "cmdClearSend";
            this.cmdClearSend.Size = new System.Drawing.Size(144, 25);
            this.cmdClearSend.TabIndex = 5;
            this.cmdClearSend.Text = "CLR";
            this.cmdClearSend.UseVisualStyleBackColor = true;
            this.cmdClearSend.Click += new System.EventHandler(this.cmdClearSend_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.lblStatus, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.cmdSend, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblTeleLength, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.panel4, 0, 4);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(741, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 99F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(144, 505);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Location = new System.Drawing.Point(3, 90);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(138, 99);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmdSend
            // 
            this.cmdSend.Location = new System.Drawing.Point(3, 3);
            this.cmdSend.Name = "cmdSend";
            this.cmdSend.Size = new System.Drawing.Size(138, 24);
            this.cmdSend.TabIndex = 6;
            this.cmdSend.Text = "Senden";
            this.cmdSend.UseVisualStyleBackColor = true;
            this.cmdSend.Click += new System.EventHandler(this.cmdSend_Click);
            // 
            // lblTeleLength
            // 
            this.lblTeleLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTeleLength.Location = new System.Drawing.Point(3, 189);
            this.lblTeleLength.Name = "lblTeleLength";
            this.lblTeleLength.Size = new System.Drawing.Size(138, 64);
            this.lblTeleLength.TabIndex = 9;
            this.lblTeleLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.numericUpDown1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 33);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(138, 54);
            this.panel2.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Laufnummer";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Kopplungstester.Properties.Settings.Default, "Laufnummer", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown1.Location = new System.Drawing.Point(18, 24);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(102, 21);
            this.numericUpDown1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblLenEmpf);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 256);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(138, 246);
            this.panel4.TabIndex = 11;
            // 
            // lblLenEmpf
            // 
            this.lblLenEmpf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLenEmpf.Location = new System.Drawing.Point(15, 159);
            this.lblLenEmpf.Name = "lblLenEmpf";
            this.lblLenEmpf.Size = new System.Drawing.Size(40, 13);
            this.lblLenEmpf.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 143);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Länge:";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer4);
            this.tableLayoutPanel1.SetRowSpan(this.splitContainer2, 2);
            this.splitContainer2.Size = new System.Drawing.Size(732, 537);
            this.splitContainer2.SplitterDistance = 305;
            this.splitContainer2.TabIndex = 4;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.tableLayoutPanel5);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.dtaSendTabelle);
            this.splitContainer3.Size = new System.Drawing.Size(732, 305);
            this.splitContainer3.SplitterDistance = 76;
            this.splitContainer3.TabIndex = 0;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel5.Controls.Add(this.lstStoredSenddata, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(732, 76);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // lstStoredSenddata
            // 
            this.lstStoredSenddata.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStoredSenddata.FormattingEnabled = true;
            this.lstStoredSenddata.Location = new System.Drawing.Point(3, 3);
            this.lstStoredSenddata.Name = "lstStoredSenddata";
            this.lstStoredSenddata.Size = new System.Drawing.Size(626, 70);
            this.lstStoredSenddata.TabIndex = 0;
            this.lstStoredSenddata.DoubleClick += new System.EventHandler(this.lstStoredSenddata_DoubleClick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.cmdRemoveSendeTele);
            this.panel3.Controls.Add(this.cmdAddSendeTele);
            this.panel3.Location = new System.Drawing.Point(635, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(94, 70);
            this.panel3.TabIndex = 1;
            // 
            // cmdRemoveSendeTele
            // 
            this.cmdRemoveSendeTele.Location = new System.Drawing.Point(8, 41);
            this.cmdRemoveSendeTele.Name = "cmdRemoveSendeTele";
            this.cmdRemoveSendeTele.Size = new System.Drawing.Size(76, 22);
            this.cmdRemoveSendeTele.TabIndex = 0;
            this.cmdRemoveSendeTele.Text = "Remove";
            this.cmdRemoveSendeTele.UseVisualStyleBackColor = true;
            this.cmdRemoveSendeTele.Click += new System.EventHandler(this.cmdRemoveSendeTele_Click);
            // 
            // cmdAddSendeTele
            // 
            this.cmdAddSendeTele.Location = new System.Drawing.Point(8, 13);
            this.cmdAddSendeTele.Name = "cmdAddSendeTele";
            this.cmdAddSendeTele.Size = new System.Drawing.Size(76, 22);
            this.cmdAddSendeTele.TabIndex = 0;
            this.cmdAddSendeTele.Text = "Add";
            this.cmdAddSendeTele.UseVisualStyleBackColor = true;
            this.cmdAddSendeTele.Click += new System.EventHandler(this.cmdAddSendeTele_Click);
            // 
            // dtaSendTabelle
            // 
            this.dtaSendTabelle.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtaSendTabelle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtaSendTabelle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Bezeichnung,
            this.Laenge,
            this.Wert});
            this.dtaSendTabelle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtaSendTabelle.Location = new System.Drawing.Point(0, 0);
            this.dtaSendTabelle.Name = "dtaSendTabelle";
            this.dtaSendTabelle.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtaSendTabelle.Size = new System.Drawing.Size(732, 225);
            this.dtaSendTabelle.TabIndex = 4;
            this.dtaSendTabelle.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtaSendTabelle_CellEndEdit);
            this.dtaSendTabelle.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dtaSendTabelle_RowsAdded);
            // 
            // Bezeichnung
            // 
            this.Bezeichnung.HeaderText = "Bezeichnung";
            this.Bezeichnung.Name = "Bezeichnung";
            // 
            // Laenge
            // 
            this.Laenge.HeaderText = "Länge";
            this.Laenge.Name = "Laenge";
            // 
            // Wert
            // 
            this.Wert.HeaderText = "Wert";
            this.Wert.Name = "Wert";
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.dtaSendSendTable);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.dtaSendQuittTable);
            this.splitContainer4.Size = new System.Drawing.Size(732, 228);
            this.splitContainer4.SplitterDistance = 109;
            this.splitContainer4.TabIndex = 0;
            // 
            // dtaSendSendTable
            // 
            this.dtaSendSendTable.AllowUserToAddRows = false;
            this.dtaSendSendTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtaSendSendTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtaSendSendTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            this.dtaSendSendTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtaSendSendTable.Location = new System.Drawing.Point(0, 0);
            this.dtaSendSendTable.Name = "dtaSendSendTable";
            this.dtaSendSendTable.ReadOnly = true;
            this.dtaSendSendTable.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtaSendSendTable.Size = new System.Drawing.Size(732, 109);
            this.dtaSendSendTable.TabIndex = 7;
            this.dtaSendSendTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtaSendSendTable_CellContentClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Gesendete Telegramme";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dtaSendQuittTable
            // 
            this.dtaSendQuittTable.AllowUserToAddRows = false;
            this.dtaSendQuittTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtaSendQuittTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtaSendQuittTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSendQuittText});
            this.dtaSendQuittTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtaSendQuittTable.Location = new System.Drawing.Point(0, 0);
            this.dtaSendQuittTable.Name = "dtaSendQuittTable";
            this.dtaSendQuittTable.ReadOnly = true;
            this.dtaSendQuittTable.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtaSendQuittTable.Size = new System.Drawing.Size(732, 115);
            this.dtaSendQuittTable.TabIndex = 6;
            // 
            // colSendQuittText
            // 
            this.colSendQuittText.HeaderText = "Empfangene Quittungen";
            this.colSendQuittText.Name = "colSendQuittText";
            this.colSendQuittText.ReadOnly = true;
            // 
            // faTabStripItemRecieve
            // 
            this.faTabStripItemRecieve.Controls.Add(this.tableLayoutPanel2);
            this.faTabStripItemRecieve.Location = new System.Drawing.Point(4, 22);
            this.faTabStripItemRecieve.Name = "faTabStripItemRecieve";
            this.faTabStripItemRecieve.Size = new System.Drawing.Size(888, 543);
            this.faTabStripItemRecieve.TabIndex = 1;
            this.faTabStripItemRecieve.Text = "Empfangs FIFO";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel2.Controls.Add(this.kryptonButton2, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.splitContainer1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(888, 543);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // kryptonButton2
            // 
            this.kryptonButton2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonButton2.Location = new System.Drawing.Point(741, 3);
            this.kryptonButton2.Name = "kryptonButton2";
            this.kryptonButton2.Size = new System.Drawing.Size(144, 40);
            this.kryptonButton2.TabIndex = 4;
            this.kryptonButton2.Text = "CLR";
            this.kryptonButton2.UseVisualStyleBackColor = true;
            this.kryptonButton2.Click += new System.EventHandler(this.kryptonButton2_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grdEmpfang);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dtaEmpfangstelegrammAufgeschluesselt);
            this.splitContainer1.Size = new System.Drawing.Size(732, 537);
            this.splitContainer1.SplitterDistance = 212;
            this.splitContainer1.TabIndex = 5;
            // 
            // grdEmpfang
            // 
            this.grdEmpfang.AllowUserToAddRows = false;
            this.grdEmpfang.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdEmpfang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdEmpfang.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEmpf});
            this.grdEmpfang.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdEmpfang.Location = new System.Drawing.Point(0, 0);
            this.grdEmpfang.Name = "grdEmpfang";
            this.grdEmpfang.ReadOnly = true;
            this.grdEmpfang.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdEmpfang.Size = new System.Drawing.Size(732, 212);
            this.grdEmpfang.TabIndex = 4;
            this.grdEmpfang.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdEmpfang_CellClick);
            this.grdEmpfang.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdEmpfang_CellContentClick);
            // 
            // colEmpf
            // 
            this.colEmpf.HeaderText = "Empfangstelegramme";
            this.colEmpf.Name = "colEmpf";
            this.colEmpf.ReadOnly = true;
            // 
            // dtaEmpfangstelegrammAufgeschluesselt
            // 
            this.dtaEmpfangstelegrammAufgeschluesselt.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtaEmpfangstelegrammAufgeschluesselt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtaEmpfangstelegrammAufgeschluesselt.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEmpfBezeichnung,
            this.colEmpfLaenge,
            this.colEmpfWert});
            this.dtaEmpfangstelegrammAufgeschluesselt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtaEmpfangstelegrammAufgeschluesselt.Location = new System.Drawing.Point(0, 0);
            this.dtaEmpfangstelegrammAufgeschluesselt.Name = "dtaEmpfangstelegrammAufgeschluesselt";
            this.dtaEmpfangstelegrammAufgeschluesselt.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtaEmpfangstelegrammAufgeschluesselt.Size = new System.Drawing.Size(732, 321);
            this.dtaEmpfangstelegrammAufgeschluesselt.TabIndex = 5;
            // 
            // colEmpfBezeichnung
            // 
            this.colEmpfBezeichnung.HeaderText = "Bezeichnung";
            this.colEmpfBezeichnung.Name = "colEmpfBezeichnung";
            // 
            // colEmpfLaenge
            // 
            this.colEmpfLaenge.HeaderText = "Länge";
            this.colEmpfLaenge.Name = "colEmpfLaenge";
            // 
            // colEmpfWert
            // 
            this.colEmpfWert.HeaderText = "Wert";
            this.colEmpfWert.Name = "colEmpfWert";
            // 
            // faTabStripItemSettings
            // 
            this.faTabStripItemSettings.Controls.Add(this.groupBox4);
            this.faTabStripItemSettings.Controls.Add(this.groupBox3);
            this.faTabStripItemSettings.Controls.Add(this.groupBox2);
            this.faTabStripItemSettings.Controls.Add(this.groupBox1);
            this.faTabStripItemSettings.Location = new System.Drawing.Point(4, 22);
            this.faTabStripItemSettings.Name = "faTabStripItemSettings";
            this.faTabStripItemSettings.Size = new System.Drawing.Size(888, 543);
            this.faTabStripItemSettings.TabIndex = 2;
            this.faTabStripItemSettings.Text = "Einstellungen";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.cmdDelTele);
            this.groupBox4.Controls.Add(this.cmdEditQuittFields);
            this.groupBox4.Controls.Add(this.lstQuitt);
            this.groupBox4.Controls.Add(this.chkAutoQuitt);
            this.groupBox4.Location = new System.Drawing.Point(409, 18);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(176, 232);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Quittungs Parameter";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 54);
            this.label5.TabIndex = 16;
            this.label5.Text = "Wenn automatisch quittieren eingeschaltet ist werden im Telegramm die Positionen " +
    "durch den Text ersetzt";
            // 
            // cmdDelTele
            // 
            this.cmdDelTele.Location = new System.Drawing.Point(6, 139);
            this.cmdDelTele.Name = "cmdDelTele";
            this.cmdDelTele.Size = new System.Drawing.Size(58, 22);
            this.cmdDelTele.TabIndex = 12;
            this.cmdDelTele.Text = "Del";
            this.cmdDelTele.UseVisualStyleBackColor = true;
            this.cmdDelTele.Click += new System.EventHandler(this.cmdDelTele_Click);
            // 
            // cmdEditQuittFields
            // 
            this.cmdEditQuittFields.Location = new System.Drawing.Point(112, 139);
            this.cmdEditQuittFields.Name = "cmdEditQuittFields";
            this.cmdEditQuittFields.Size = new System.Drawing.Size(58, 22);
            this.cmdEditQuittFields.TabIndex = 12;
            this.cmdEditQuittFields.Text = "Add";
            this.cmdEditQuittFields.UseVisualStyleBackColor = true;
            this.cmdEditQuittFields.Click += new System.EventHandler(this.cmdEditQuittFields_Click);
            // 
            // lstQuitt
            // 
            this.lstQuitt.FormattingEnabled = true;
            this.lstQuitt.Location = new System.Drawing.Point(6, 38);
            this.lstQuitt.Name = "lstQuitt";
            this.lstQuitt.Size = new System.Drawing.Size(164, 95);
            this.lstQuitt.TabIndex = 12;
            // 
            // chkAutoQuitt
            // 
            this.chkAutoQuitt.AutoSize = true;
            this.chkAutoQuitt.Checked = true;
            this.chkAutoQuitt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoQuitt.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Kopplungstester.Properties.Settings.Default, "AutomaticQuitt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkAutoQuitt.Location = new System.Drawing.Point(6, 20);
            this.chkAutoQuitt.Name = "chkAutoQuitt";
            this.chkAutoQuitt.Size = new System.Drawing.Size(134, 17);
            this.chkAutoQuitt.TabIndex = 15;
            this.chkAutoQuitt.Text = "Automatisch quittieren";
            this.chkAutoQuitt.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numSequenceNumberLength);
            this.groupBox3.Controls.Add(this.numSequenceNumberPosition);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(11, 103);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(176, 107);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Telegramm Parameter";
            // 
            // numSequenceNumberLength
            // 
            this.numSequenceNumberLength.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Kopplungstester.Properties.Settings.Default, "SequenceNumberLength", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numSequenceNumberLength.Location = new System.Drawing.Point(12, 75);
            this.numSequenceNumberLength.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numSequenceNumberLength.Name = "numSequenceNumberLength";
            this.numSequenceNumberLength.Size = new System.Drawing.Size(141, 21);
            this.numSequenceNumberLength.TabIndex = 12;
            // 
            // numSequenceNumberPosition
            // 
            this.numSequenceNumberPosition.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Kopplungstester.Properties.Settings.Default, "SequenceNumberPosition", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numSequenceNumberPosition.Location = new System.Drawing.Point(12, 34);
            this.numSequenceNumberPosition.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numSequenceNumberPosition.Name = "numSequenceNumberPosition";
            this.numSequenceNumberPosition.Size = new System.Drawing.Size(141, 21);
            this.numSequenceNumberPosition.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Laufnummer Länge";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Laufnummer Position";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkChanel2active);
            this.groupBox2.Controls.Add(this.numPort2);
            this.groupBox2.Controls.Add(this.numPort1);
            this.groupBox2.Controls.Add(this.lblKanal2Port);
            this.groupBox2.Controls.Add(this.chkChanel1active);
            this.groupBox2.Controls.Add(this.lblKanal2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.optTwoChannel);
            this.groupBox2.Controls.Add(this.optOneChannel);
            this.groupBox2.Location = new System.Drawing.Point(216, 18);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(187, 232);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Verbindungs Parameter";
            // 
            // chkChanel2active
            // 
            this.chkChanel2active.AutoSize = true;
            this.chkChanel2active.Checked = true;
            this.chkChanel2active.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkChanel2active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Kopplungstester.Properties.Settings.Default, "RecieveConnectionActive", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkChanel2active.Enabled = false;
            this.chkChanel2active.Location = new System.Drawing.Point(121, 171);
            this.chkChanel2active.Name = "chkChanel2active";
            this.chkChanel2active.Size = new System.Drawing.Size(49, 17);
            this.chkChanel2active.TabIndex = 15;
            this.chkChanel2active.Text = "aktiv";
            this.chkChanel2active.UseVisualStyleBackColor = true;
            // 
            // numPort2
            // 
            this.numPort2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Kopplungstester.Properties.Settings.Default, "RecievePort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numPort2.Enabled = false;
            this.numPort2.Location = new System.Drawing.Point(27, 188);
            this.numPort2.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numPort2.Name = "numPort2";
            this.numPort2.Size = new System.Drawing.Size(141, 21);
            this.numPort2.TabIndex = 12;
            // 
            // numPort1
            // 
            this.numPort1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Kopplungstester.Properties.Settings.Default, "SendPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numPort1.Location = new System.Drawing.Point(27, 120);
            this.numPort1.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numPort1.Name = "numPort1";
            this.numPort1.Size = new System.Drawing.Size(141, 21);
            this.numPort1.TabIndex = 12;
            // 
            // lblKanal2Port
            // 
            this.lblKanal2Port.AutoSize = true;
            this.lblKanal2Port.Enabled = false;
            this.lblKanal2Port.Location = new System.Drawing.Point(24, 172);
            this.lblKanal2Port.Name = "lblKanal2Port";
            this.lblKanal2Port.Size = new System.Drawing.Size(27, 13);
            this.lblKanal2Port.TabIndex = 14;
            this.lblKanal2Port.Text = "Port";
            // 
            // chkChanel1active
            // 
            this.chkChanel1active.AutoSize = true;
            this.chkChanel1active.Checked = true;
            this.chkChanel1active.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkChanel1active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Kopplungstester.Properties.Settings.Default, "SendConnectionActive", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkChanel1active.Location = new System.Drawing.Point(121, 103);
            this.chkChanel1active.Name = "chkChanel1active";
            this.chkChanel1active.Size = new System.Drawing.Size(49, 17);
            this.chkChanel1active.TabIndex = 15;
            this.chkChanel1active.Text = "aktiv";
            this.chkChanel1active.UseVisualStyleBackColor = true;
            // 
            // lblKanal2
            // 
            this.lblKanal2.AutoSize = true;
            this.lblKanal2.Enabled = false;
            this.lblKanal2.Location = new System.Drawing.Point(9, 150);
            this.lblKanal2.Name = "lblKanal2";
            this.lblKanal2.Size = new System.Drawing.Size(88, 13);
            this.lblKanal2.TabIndex = 14;
            this.lblKanal2.Text = "Kanal 2 (recieve)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Kanal 1 (send / recieve)";
            // 
            // optTwoChannel
            // 
            this.optTwoChannel.AutoSize = true;
            this.optTwoChannel.Checked = true;
            this.optTwoChannel.Location = new System.Drawing.Point(6, 43);
            this.optTwoChannel.Name = "optTwoChannel";
            this.optTwoChannel.Size = new System.Drawing.Size(60, 17);
            this.optTwoChannel.TabIndex = 14;
            this.optTwoChannel.TabStop = true;
            this.optTwoChannel.Text = "2 Kanal";
            this.optTwoChannel.UseVisualStyleBackColor = true;
            this.optTwoChannel.CheckedChanged += new System.EventHandler(this.optTwoChannel_CheckedChanged);
            // 
            // optOneChannel
            // 
            this.optOneChannel.AutoSize = true;
            this.optOneChannel.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Kopplungstester.Properties.Settings.Default, "UseOnlyOneConnection", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.optOneChannel.Location = new System.Drawing.Point(6, 20);
            this.optOneChannel.Name = "optOneChannel";
            this.optOneChannel.Size = new System.Drawing.Size(60, 17);
            this.optOneChannel.TabIndex = 14;
            this.optOneChannel.Text = "1 Kanal";
            this.optOneChannel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ipAddressControl);
            this.groupBox1.Location = new System.Drawing.Point(11, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(176, 79);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Verbindungs Parameter";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "IP - Adresse SPS";
            // 
            // ipAddressControl
            // 
            this.ipAddressControl.AllowInternalTab = false;
            this.ipAddressControl.AutoHeight = true;
            this.ipAddressControl.BackColor = System.Drawing.SystemColors.Window;
            this.ipAddressControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ipAddressControl.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipAddressControl.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Kopplungstester.Properties.Settings.Default, "IPAddress", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ipAddressControl.Location = new System.Drawing.Point(12, 52);
            this.ipAddressControl.MinimumSize = new System.Drawing.Size(87, 21);
            this.ipAddressControl.Name = "ipAddressControl";
            this.ipAddressControl.ReadOnly = false;
            this.ipAddressControl.Size = new System.Drawing.Size(141, 21);
            this.ipAddressControl.TabIndex = 13;
            this.ipAddressControl.Text = global::Kopplungstester.Properties.Settings.Default.IPAddress;
            // 
            // cmdSelectStep7UDT
            // 
            this.cmdSelectStep7UDT.Location = new System.Drawing.Point(281, 3);
            this.cmdSelectStep7UDT.Name = "cmdSelectStep7UDT";
            this.cmdSelectStep7UDT.Size = new System.Drawing.Size(136, 40);
            this.cmdSelectStep7UDT.TabIndex = 6;
            this.cmdSelectStep7UDT.Text = "Send/Recieve Struktur\r\naus Step7 UDT";
            this.cmdSelectStep7UDT.UseVisualStyleBackColor = true;
            this.cmdSelectStep7UDT.Click += new System.EventHandler(this.cmdSelectStep7UDT_Click);
            // 
            // cmdConnect
            // 
            this.cmdConnect.Location = new System.Drawing.Point(3, 3);
            this.cmdConnect.Name = "cmdConnect";
            this.cmdConnect.Size = new System.Drawing.Size(107, 40);
            this.cmdConnect.TabIndex = 11;
            this.cmdConnect.Text = "Connect";
            this.cmdConnect.UseVisualStyleBackColor = true;
            this.cmdConnect.Click += new System.EventHandler(this.cmdConnect_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.faTabStrip1, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(902, 625);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdSettExport);
            this.panel1.Controls.Add(this.cmdSettImport);
            this.panel1.Controls.Add(this.cmdSettingsSave);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(896, 44);
            this.panel1.TabIndex = 3;
            // 
            // cmdSettExport
            // 
            this.cmdSettExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSettExport.Location = new System.Drawing.Point(636, 1);
            this.cmdSettExport.Name = "cmdSettExport";
            this.cmdSettExport.Size = new System.Drawing.Size(69, 40);
            this.cmdSettExport.TabIndex = 12;
            this.cmdSettExport.Text = "Settings export";
            this.cmdSettExport.UseVisualStyleBackColor = true;
            this.cmdSettExport.Click += new System.EventHandler(this.cmdSettExport_Click);
            // 
            // cmdSettImport
            // 
            this.cmdSettImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSettImport.Location = new System.Drawing.Point(711, 1);
            this.cmdSettImport.Name = "cmdSettImport";
            this.cmdSettImport.Size = new System.Drawing.Size(69, 40);
            this.cmdSettImport.TabIndex = 12;
            this.cmdSettImport.Text = "Settings import";
            this.cmdSettImport.UseVisualStyleBackColor = true;
            this.cmdSettImport.Click += new System.EventHandler(this.cmdSettImport_Click);
            // 
            // cmdSettingsSave
            // 
            this.cmdSettingsSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSettingsSave.Location = new System.Drawing.Point(786, 1);
            this.cmdSettingsSave.Name = "cmdSettingsSave";
            this.cmdSettingsSave.Size = new System.Drawing.Size(107, 40);
            this.cmdSettingsSave.TabIndex = 12;
            this.cmdSettingsSave.Text = "Änderungen Speichern";
            this.cmdSettingsSave.UseVisualStyleBackColor = true;
            this.cmdSettingsSave.Click += new System.EventHandler(this.cmdSettingsSave_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.cmdConnect);
            this.flowLayoutPanel1.Controls.Add(this.picConnection1);
            this.flowLayoutPanel1.Controls.Add(this.picConnection2);
            this.flowLayoutPanel1.Controls.Add(this.cmdDisconnect);
            this.flowLayoutPanel1.Controls.Add(this.cmdSelectStep7UDT);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, -3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(550, 44);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // picConnection1
            // 
            this.picConnection1.BackColor = System.Drawing.Color.Red;
            this.picConnection1.Location = new System.Drawing.Point(116, 3);
            this.picConnection1.Name = "picConnection1";
            this.picConnection1.Size = new System.Drawing.Size(20, 41);
            this.picConnection1.TabIndex = 12;
            this.picConnection1.TabStop = false;
            this.toolTip.SetToolTip(this.picConnection1, "Sende Verbindung aufgebaut");
            // 
            // picConnection2
            // 
            this.picConnection2.BackColor = System.Drawing.Color.Red;
            this.picConnection2.Location = new System.Drawing.Point(142, 3);
            this.picConnection2.Name = "picConnection2";
            this.picConnection2.Size = new System.Drawing.Size(20, 41);
            this.picConnection2.TabIndex = 12;
            this.picConnection2.TabStop = false;
            this.toolTip.SetToolTip(this.picConnection2, "Recieve Verbindung aufgebaut");
            // 
            // cmdDisconnect
            // 
            this.cmdDisconnect.Location = new System.Drawing.Point(168, 3);
            this.cmdDisconnect.Name = "cmdDisconnect";
            this.cmdDisconnect.Size = new System.Drawing.Size(107, 40);
            this.cmdDisconnect.TabIndex = 11;
            this.cmdDisconnect.Text = "Disconnect";
            this.cmdDisconnect.UseVisualStyleBackColor = true;
            this.cmdDisconnect.Click += new System.EventHandler(this.cmdDisconnect_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 625);
            this.Controls.Add(this.tableLayoutPanel4);
            this.Name = "MainForm";
            this.Text = "Kopplungstester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.faTabStrip1.ResumeLayout(false);
            this.faTabStripItemInfo.ResumeLayout(false);
            this.faTabStripItemInfo.PerformLayout();
            this.faTabStripItemSend.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtaSendTabelle)).EndInit();
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtaSendSendTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaSendQuittTable)).EndInit();
            this.faTabStripItemRecieve.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdEmpfang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaEmpfangstelegrammAufgeschluesselt)).EndInit();
            this.faTabStripItemSettings.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSequenceNumberLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSequenceNumberPosition)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picConnection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picConnection2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl faTabStrip1;
        private System.Windows.Forms.TabPage faTabStripItemSend;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button cmdSend;
        private System.Windows.Forms.Button cmdSelectStep7UDT;
        private System.Windows.Forms.DataGridView dtaSendTabelle;
        private System.Windows.Forms.TabPage faTabStripItemRecieve;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button kryptonButton2;
        private System.Windows.Forms.TabPage faTabStripItemSettings;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkChanel2active;
        private System.Windows.Forms.Label lblKanal2Port;
        private System.Windows.Forms.CheckBox chkChanel1active;
        private System.Windows.Forms.Label lblKanal2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton optTwoChannel;
        private System.Windows.Forms.RadioButton optOneChannel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bezeichnung;
        private System.Windows.Forms.DataGridViewTextBoxColumn Laenge;
        private System.Windows.Forms.DataGridViewTextBoxColumn Wert;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView grdEmpfang;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmpf;
        private System.Windows.Forms.DataGridView dtaEmpfangstelegrammAufgeschluesselt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmpfBezeichnung;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmpfLaenge;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmpfWert;
        private System.Windows.Forms.Button cmdConnect;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numSequenceNumberPosition;
        private IPAddressControlLib.IPAddressControl ipAddressControl;
        private System.Windows.Forms.NumericUpDown numSequenceNumberLength;
        private System.Windows.Forms.NumericUpDown numPort1;
        private System.Windows.Forms.NumericUpDown numPort2;
        private System.Windows.Forms.Button cmdDisconnect;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button cmdEditQuittFields;
        private System.Windows.Forms.ListBox lstQuitt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkAutoQuitt;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblTeleLength;
        private System.Windows.Forms.PictureBox picConnection1;
        private System.Windows.Forms.PictureBox picConnection2;
        private System.Windows.Forms.Button cmdSettingsSave;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.ListBox lstStoredSenddata;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button cmdRemoveSendeTele;
        private System.Windows.Forms.Button cmdAddSendeTele;
        private System.Windows.Forms.Button cmdSettExport;
        private System.Windows.Forms.Button cmdSettImport;
        private System.Windows.Forms.Button cmdDelTele;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblLenEmpf;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabPage faTabStripItemInfo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.DataGridView dtaSendSendTable;
        private System.Windows.Forms.DataGridView dtaSendQuittTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSendQuittText;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.Button cmdClearSend;
    }
}