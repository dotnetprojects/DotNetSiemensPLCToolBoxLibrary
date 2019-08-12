namespace JFK_VarTab
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblProjectInfo = new System.Windows.Forms.Label();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.treeStep7Project = new System.Windows.Forms.TreeView();
            this.imglstIconsForTreeview = new System.Windows.Forms.ImageList(this.components);
            this.dtaSymbolTable = new System.Windows.Forms.DataGridView();
            this.symbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.datatype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operandiec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tia_key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lstListBox = new System.Windows.Forms.ListBox();
            this.txtTextBox = new System.Windows.Forms.TextBox();
            this.chkShowDeleted = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmdSetKnowHow = new System.Windows.Forms.Button();
            this.cmdRemoveKnowHow = new System.Windows.Forms.Button();
            this.cmdUndeleteBlock = new System.Windows.Forms.Button();
            this.txtUndeleteName = new System.Windows.Forms.TextBox();
            this.grpVisu = new System.Windows.Forms.GroupBox();
            this.cmdWebfactoryTags = new System.Windows.Forms.Button();
            this.chkCombineStructComments = new System.Windows.Forms.CheckBox();
            this.cmdCreateWinCCFlexibleTags = new System.Windows.Forms.Button();
            this.chkExpandArrays = new System.Windows.Forms.CheckBox();
            this.chkUseErrPrefix = new System.Windows.Forms.CheckBox();
            this.chkFixedErrorNumber = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTagsPrefix = new System.Windows.Forms.TextBox();
            this.txtErrPrefix = new System.Windows.Forms.TextBox();
            this.txtStartErrorNumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtConnectionName = new System.Windows.Forms.TextBox();
            this.cmdCreateWinCCTags = new System.Windows.Forms.Button();
            this.cmdCreateWinCCErrorMessages = new System.Windows.Forms.Button();
            this.cmdCreateFlexibleErrorMessages = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.fetchPLCData = new System.Windows.Forms.Timer(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblToolStripFileSystemFolder = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblConnInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblConnected = new System.Windows.Forms.ToolStripStatusLabel();
            this.lstProjects = new System.Windows.Forms.ListBox();
            this.cmdProjectsBrowser = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reachablePLCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lstConnections = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.watchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unwatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.downloadOnlineBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.dBStructResizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataBlockValueSaveRestoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.searchPasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertCallsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExpandDatablockArrays = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.createDokumentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parseAllBlocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createAWLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dependenciesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.callHirachyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.featuresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.viewBlockList = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanelVisu = new System.Windows.Forms.TableLayoutPanel();
            this.datablockView = new System.Windows.Forms.Integration.ElementHost();
            this.dataBlockViewControl = new TestProjectFileFunctions.DataBlockViewControl();
            this.dtaPnPbList = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.exportMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.export = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.hexBox = new Be.Windows.Forms.HexBox();
            ((System.ComponentModel.ISupportInitialize)(this.dtaSymbolTable)).BeginInit();
            this.grpVisu.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.viewBlockList.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelVisu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtaPnPbList)).BeginInit();
            this.exportMenu.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblProjectInfo
            // 
            this.lblProjectInfo.Location = new System.Drawing.Point(200, 82);
            this.lblProjectInfo.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblProjectInfo.Name = "lblProjectInfo";
            this.lblProjectInfo.Size = new System.Drawing.Size(1380, 26);
            this.lblProjectInfo.TabIndex = 3;
            this.lblProjectInfo.Click += new System.EventHandler(this.label2_Click);
            // 
            // lblProjectName
            // 
            this.lblProjectName.Location = new System.Drawing.Point(200, 56);
            this.lblProjectName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(1286, 26);
            this.lblProjectName.TabIndex = 3;
            // 
            // treeStep7Project
            // 
            this.treeStep7Project.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeStep7Project.ImageIndex = 0;
            this.treeStep7Project.ImageList = this.imglstIconsForTreeview;
            this.treeStep7Project.Location = new System.Drawing.Point(6, 6);
            this.treeStep7Project.Margin = new System.Windows.Forms.Padding(6);
            this.treeStep7Project.Name = "treeStep7Project";
            this.treeStep7Project.SelectedImageIndex = 0;
            this.treeStep7Project.Size = new System.Drawing.Size(614, 426);
            this.treeStep7Project.TabIndex = 4;
            this.treeStep7Project.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeStep7Project_BeforeCollapse);
            this.treeStep7Project.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeStep7Project_BeforeExpand);
            this.treeStep7Project.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeStep7Project.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeStep7Project_NodeMouseClick);
            this.treeStep7Project.Click += new System.EventHandler(this.treeView1_Click);
            this.treeStep7Project.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeStep7Project_MouseClick);
            // 
            // imglstIconsForTreeview
            // 
            this.imglstIconsForTreeview.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglstIconsForTreeview.ImageStream")));
            this.imglstIconsForTreeview.TransparentColor = System.Drawing.Color.Transparent;
            this.imglstIconsForTreeview.Images.SetKeyName(0, "Generic Folder   Closed.ico");
            this.imglstIconsForTreeview.Images.SetKeyName(1, "Generic Folder   Open.ico");
            this.imglstIconsForTreeview.Images.SetKeyName(2, "300er.png");
            this.imglstIconsForTreeview.Images.SetKeyName(3, "400er.png");
            this.imglstIconsForTreeview.Images.SetKeyName(4, "Rack300.png");
            this.imglstIconsForTreeview.Images.SetKeyName(5, "rack400.png");
            this.imglstIconsForTreeview.Images.SetKeyName(6, "Folder.png");
            this.imglstIconsForTreeview.Images.SetKeyName(7, "im151.png");
            this.imglstIconsForTreeview.Images.SetKeyName(8, "im151cpu.png");
            this.imglstIconsForTreeview.Images.SetKeyName(9, "cp.png");
            this.imglstIconsForTreeview.Images.SetKeyName(10, "s5.png");
            // 
            // dtaSymbolTable
            // 
            this.dtaSymbolTable.AllowUserToAddRows = false;
            this.dtaSymbolTable.AllowUserToDeleteRows = false;
            this.dtaSymbolTable.AllowUserToResizeRows = false;
            this.dtaSymbolTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtaSymbolTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.symbol,
            this.datatype,
            this.operand,
            this.operandiec,
            this.comment,
            this.tia_key});
            this.dtaSymbolTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtaSymbolTable.Location = new System.Drawing.Point(0, 0);
            this.dtaSymbolTable.Margin = new System.Windows.Forms.Padding(6);
            this.dtaSymbolTable.Name = "dtaSymbolTable";
            this.dtaSymbolTable.RowHeadersWidth = 82;
            this.dtaSymbolTable.Size = new System.Drawing.Size(1220, 700);
            this.dtaSymbolTable.TabIndex = 5;
            this.dtaSymbolTable.Visible = false;
            // 
            // symbol
            // 
            this.symbol.HeaderText = "symbol";
            this.symbol.MinimumWidth = 10;
            this.symbol.Name = "symbol";
            this.symbol.Width = 200;
            // 
            // datatype
            // 
            this.datatype.HeaderText = "datatype";
            this.datatype.MinimumWidth = 10;
            this.datatype.Name = "datatype";
            this.datatype.Width = 200;
            // 
            // operand
            // 
            this.operand.HeaderText = "operand";
            this.operand.MinimumWidth = 10;
            this.operand.Name = "operand";
            this.operand.Width = 200;
            // 
            // operandiec
            // 
            this.operandiec.HeaderText = "operandiec";
            this.operandiec.MinimumWidth = 10;
            this.operandiec.Name = "operandiec";
            this.operandiec.Width = 200;
            // 
            // comment
            // 
            this.comment.HeaderText = "comment";
            this.comment.MinimumWidth = 10;
            this.comment.Name = "comment";
            this.comment.Width = 200;
            // 
            // tia_key
            // 
            this.tia_key.HeaderText = "tia_key";
            this.tia_key.MinimumWidth = 10;
            this.tia_key.Name = "tia_key";
            this.tia_key.Width = 200;
            // 
            // lstListBox
            // 
            this.lstListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstListBox.FormattingEnabled = true;
            this.lstListBox.ItemHeight = 29;
            this.lstListBox.Location = new System.Drawing.Point(6, 6);
            this.lstListBox.Margin = new System.Windows.Forms.Padding(6);
            this.lstListBox.Name = "lstListBox";
            this.lstListBox.Size = new System.Drawing.Size(708, 566);
            this.lstListBox.TabIndex = 6;
            this.lstListBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.lstListBox.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            this.lstListBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstListBox_KeyPress);
            // 
            // txtTextBox
            // 
            this.txtTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTextBox.Location = new System.Drawing.Point(0, 0);
            this.txtTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.txtTextBox.Multiline = true;
            this.txtTextBox.Name = "txtTextBox";
            this.txtTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtTextBox.Size = new System.Drawing.Size(1220, 700);
            this.txtTextBox.TabIndex = 7;
            this.txtTextBox.Visible = false;
            this.txtTextBox.WordWrap = false;
            // 
            // chkShowDeleted
            // 
            this.chkShowDeleted.AutoSize = true;
            this.chkShowDeleted.Location = new System.Drawing.Point(6, 658);
            this.chkShowDeleted.Margin = new System.Windows.Forms.Padding(6);
            this.chkShowDeleted.Name = "chkShowDeleted";
            this.chkShowDeleted.Size = new System.Drawing.Size(237, 34);
            this.chkShowDeleted.TabIndex = 8;
            this.chkShowDeleted.Text = "Show deleted";
            this.chkShowDeleted.UseVisualStyleBackColor = true;
            this.chkShowDeleted.CheckedChanged += new System.EventHandler(this.chkShowDeleted_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 56);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(221, 30);
            this.label3.TabIndex = 3;
            this.label3.Text = "Project Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 82);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(221, 30);
            this.label4.TabIndex = 3;
            this.label4.Text = "Project Info:";
            // 
            // cmdSetKnowHow
            // 
            this.cmdSetKnowHow.Location = new System.Drawing.Point(6, 6);
            this.cmdSetKnowHow.Margin = new System.Windows.Forms.Padding(6);
            this.cmdSetKnowHow.Name = "cmdSetKnowHow";
            this.cmdSetKnowHow.Size = new System.Drawing.Size(332, 48);
            this.cmdSetKnowHow.TabIndex = 10;
            this.cmdSetKnowHow.Text = "Set Know How Protection";
            this.cmdSetKnowHow.UseVisualStyleBackColor = true;
            this.cmdSetKnowHow.Click += new System.EventHandler(this.button2_Click);
            // 
            // cmdRemoveKnowHow
            // 
            this.cmdRemoveKnowHow.Location = new System.Drawing.Point(350, 6);
            this.cmdRemoveKnowHow.Margin = new System.Windows.Forms.Padding(6);
            this.cmdRemoveKnowHow.Name = "cmdRemoveKnowHow";
            this.cmdRemoveKnowHow.Size = new System.Drawing.Size(368, 48);
            this.cmdRemoveKnowHow.TabIndex = 11;
            this.cmdRemoveKnowHow.Text = "Remove Know How Protection";
            this.cmdRemoveKnowHow.UseVisualStyleBackColor = true;
            this.cmdRemoveKnowHow.Click += new System.EventHandler(this.button3_Click);
            // 
            // cmdUndeleteBlock
            // 
            this.cmdUndeleteBlock.Location = new System.Drawing.Point(730, 6);
            this.cmdUndeleteBlock.Margin = new System.Windows.Forms.Padding(6);
            this.cmdUndeleteBlock.Name = "cmdUndeleteBlock";
            this.cmdUndeleteBlock.Size = new System.Drawing.Size(328, 48);
            this.cmdUndeleteBlock.TabIndex = 12;
            this.cmdUndeleteBlock.Text = "Undelete Block";
            this.cmdUndeleteBlock.UseVisualStyleBackColor = true;
            this.cmdUndeleteBlock.Visible = false;
            this.cmdUndeleteBlock.Click += new System.EventHandler(this.button4_Click);
            // 
            // txtUndeleteName
            // 
            this.txtUndeleteName.Location = new System.Drawing.Point(6, 66);
            this.txtUndeleteName.Margin = new System.Windows.Forms.Padding(6);
            this.txtUndeleteName.Name = "txtUndeleteName";
            this.txtUndeleteName.Size = new System.Drawing.Size(132, 37);
            this.txtUndeleteName.TabIndex = 13;
            this.txtUndeleteName.Text = "9999";
            this.txtUndeleteName.Visible = false;
            // 
            // grpVisu
            // 
            this.grpVisu.Controls.Add(this.cmdWebfactoryTags);
            this.grpVisu.Controls.Add(this.chkCombineStructComments);
            this.grpVisu.Controls.Add(this.cmdCreateWinCCFlexibleTags);
            this.grpVisu.Controls.Add(this.chkExpandArrays);
            this.grpVisu.Controls.Add(this.chkUseErrPrefix);
            this.grpVisu.Controls.Add(this.chkFixedErrorNumber);
            this.grpVisu.Controls.Add(this.label10);
            this.grpVisu.Controls.Add(this.label2);
            this.grpVisu.Controls.Add(this.label1);
            this.grpVisu.Controls.Add(this.txtTagsPrefix);
            this.grpVisu.Controls.Add(this.txtErrPrefix);
            this.grpVisu.Controls.Add(this.txtStartErrorNumber);
            this.grpVisu.Controls.Add(this.label9);
            this.grpVisu.Controls.Add(this.txtConnectionName);
            this.grpVisu.Controls.Add(this.cmdCreateWinCCTags);
            this.grpVisu.Controls.Add(this.cmdCreateWinCCErrorMessages);
            this.grpVisu.Controls.Add(this.cmdCreateFlexibleErrorMessages);
            this.grpVisu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpVisu.Location = new System.Drawing.Point(726, 6);
            this.grpVisu.Margin = new System.Windows.Forms.Padding(6);
            this.grpVisu.Name = "grpVisu";
            this.grpVisu.Padding = new System.Windows.Forms.Padding(6);
            this.grpVisu.Size = new System.Drawing.Size(464, 566);
            this.grpVisu.TabIndex = 16;
            this.grpVisu.TabStop = false;
            this.grpVisu.Text = "Visualization Toolbox";
            // 
            // cmdWebfactoryTags
            // 
            this.cmdWebfactoryTags.Location = new System.Drawing.Point(38, 730);
            this.cmdWebfactoryTags.Margin = new System.Windows.Forms.Padding(6);
            this.cmdWebfactoryTags.Name = "cmdWebfactoryTags";
            this.cmdWebfactoryTags.Size = new System.Drawing.Size(396, 46);
            this.cmdWebfactoryTags.TabIndex = 7;
            this.cmdWebfactoryTags.Text = "Create Webfactory Tags";
            this.cmdWebfactoryTags.UseVisualStyleBackColor = true;
            this.cmdWebfactoryTags.Click += new System.EventHandler(this.cmdCreateWEBfactoryTags_Click);
            // 
            // chkCombineStructComments
            // 
            this.chkCombineStructComments.Checked = true;
            this.chkCombineStructComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCombineStructComments.Location = new System.Drawing.Point(38, 182);
            this.chkCombineStructComments.Margin = new System.Windows.Forms.Padding(6);
            this.chkCombineStructComments.Name = "chkCombineStructComments";
            this.chkCombineStructComments.Size = new System.Drawing.Size(416, 32);
            this.chkCombineStructComments.TabIndex = 6;
            this.chkCombineStructComments.Text = "Combine Struct/UDT Comments";
            this.chkCombineStructComments.UseVisualStyleBackColor = true;
            // 
            // cmdCreateWinCCFlexibleTags
            // 
            this.cmdCreateWinCCFlexibleTags.Location = new System.Drawing.Point(38, 690);
            this.cmdCreateWinCCFlexibleTags.Margin = new System.Windows.Forms.Padding(6);
            this.cmdCreateWinCCFlexibleTags.Name = "cmdCreateWinCCFlexibleTags";
            this.cmdCreateWinCCFlexibleTags.Size = new System.Drawing.Size(396, 46);
            this.cmdCreateWinCCFlexibleTags.TabIndex = 5;
            this.cmdCreateWinCCFlexibleTags.Text = "Create WinCC-Flexible Tags";
            this.cmdCreateWinCCFlexibleTags.UseVisualStyleBackColor = true;
            this.cmdCreateWinCCFlexibleTags.Click += new System.EventHandler(this.cmdCreateWinCCFlexibleTags_Click);
            // 
            // chkExpandArrays
            // 
            this.chkExpandArrays.AutoSize = true;
            this.chkExpandArrays.Location = new System.Drawing.Point(46, 560);
            this.chkExpandArrays.Margin = new System.Windows.Forms.Padding(6);
            this.chkExpandArrays.Name = "chkExpandArrays";
            this.chkExpandArrays.Size = new System.Drawing.Size(253, 34);
            this.chkExpandArrays.TabIndex = 4;
            this.chkExpandArrays.Text = "Expand Arrays";
            this.chkExpandArrays.UseVisualStyleBackColor = true;
            // 
            // chkUseErrPrefix
            // 
            this.chkUseErrPrefix.Location = new System.Drawing.Point(38, 214);
            this.chkUseErrPrefix.Margin = new System.Windows.Forms.Padding(6);
            this.chkUseErrPrefix.Name = "chkUseErrPrefix";
            this.chkUseErrPrefix.Size = new System.Drawing.Size(308, 32);
            this.chkUseErrPrefix.TabIndex = 4;
            this.chkUseErrPrefix.Text = "Errors Start with: ";
            this.chkUseErrPrefix.UseVisualStyleBackColor = true;
            // 
            // chkFixedErrorNumber
            // 
            this.chkFixedErrorNumber.AutoSize = true;
            this.chkFixedErrorNumber.Location = new System.Drawing.Point(38, 150);
            this.chkFixedErrorNumber.Margin = new System.Windows.Forms.Padding(6);
            this.chkFixedErrorNumber.Name = "chkFixedErrorNumber";
            this.chkFixedErrorNumber.Size = new System.Drawing.Size(333, 34);
            this.chkFixedErrorNumber.TabIndex = 4;
            this.chkFixedErrorNumber.Text = "Fixed Error Number";
            this.chkFixedErrorNumber.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(40, 446);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(477, 120);
            this.label10.TabIndex = 3;
            this.label10.Text = "Style of comments\r\nin error message DB:\r\n\r\n[Error Place];[Error Message]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 602);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 30);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tags Prefix";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 118);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(301, 30);
            this.label1.TabIndex = 2;
            this.label1.Text = "Start Error Number";
            // 
            // txtTagsPrefix
            // 
            this.txtTagsPrefix.Location = new System.Drawing.Point(200, 596);
            this.txtTagsPrefix.Margin = new System.Windows.Forms.Padding(6);
            this.txtTagsPrefix.Name = "txtTagsPrefix";
            this.txtTagsPrefix.Size = new System.Drawing.Size(222, 37);
            this.txtTagsPrefix.TabIndex = 1;
            // 
            // txtErrPrefix
            // 
            this.txtErrPrefix.Location = new System.Drawing.Point(346, 210);
            this.txtErrPrefix.Margin = new System.Windows.Forms.Padding(6);
            this.txtErrPrefix.Name = "txtErrPrefix";
            this.txtErrPrefix.Size = new System.Drawing.Size(76, 37);
            this.txtErrPrefix.TabIndex = 1;
            this.txtErrPrefix.Text = "$";
            // 
            // txtStartErrorNumber
            // 
            this.txtStartErrorNumber.Location = new System.Drawing.Point(292, 114);
            this.txtStartErrorNumber.Margin = new System.Windows.Forms.Padding(6);
            this.txtStartErrorNumber.Name = "txtStartErrorNumber";
            this.txtStartErrorNumber.Size = new System.Drawing.Size(132, 37);
            this.txtStartErrorNumber.TabIndex = 1;
            this.txtStartErrorNumber.Text = "1";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 42);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(269, 30);
            this.label9.TabIndex = 2;
            this.label9.Text = "Connection Name:";
            // 
            // txtConnectionName
            // 
            this.txtConnectionName.Location = new System.Drawing.Point(30, 74);
            this.txtConnectionName.Margin = new System.Windows.Forms.Padding(6);
            this.txtConnectionName.Name = "txtConnectionName";
            this.txtConnectionName.Size = new System.Drawing.Size(392, 37);
            this.txtConnectionName.TabIndex = 1;
            this.txtConnectionName.Text = "Verbindung_1";
            // 
            // cmdCreateWinCCTags
            // 
            this.cmdCreateWinCCTags.Location = new System.Drawing.Point(38, 638);
            this.cmdCreateWinCCTags.Margin = new System.Windows.Forms.Padding(6);
            this.cmdCreateWinCCTags.Name = "cmdCreateWinCCTags";
            this.cmdCreateWinCCTags.Size = new System.Drawing.Size(396, 50);
            this.cmdCreateWinCCTags.TabIndex = 0;
            this.cmdCreateWinCCTags.Text = "Create WinCC Tags";
            this.cmdCreateWinCCTags.UseVisualStyleBackColor = true;
            this.cmdCreateWinCCTags.Click += new System.EventHandler(this.cmdCreateWinCCTags_Click);
            // 
            // cmdCreateWinCCErrorMessages
            // 
            this.cmdCreateWinCCErrorMessages.Location = new System.Drawing.Point(38, 342);
            this.cmdCreateWinCCErrorMessages.Margin = new System.Windows.Forms.Padding(6);
            this.cmdCreateWinCCErrorMessages.Name = "cmdCreateWinCCErrorMessages";
            this.cmdCreateWinCCErrorMessages.Size = new System.Drawing.Size(396, 88);
            this.cmdCreateWinCCErrorMessages.TabIndex = 0;
            this.cmdCreateWinCCErrorMessages.Text = "Create WinCC Error Messages && Tags";
            this.cmdCreateWinCCErrorMessages.UseVisualStyleBackColor = true;
            this.cmdCreateWinCCErrorMessages.Click += new System.EventHandler(this.cmdCreateWinCCErrorMessages_Click);
            // 
            // cmdCreateFlexibleErrorMessages
            // 
            this.cmdCreateFlexibleErrorMessages.Location = new System.Drawing.Point(38, 256);
            this.cmdCreateFlexibleErrorMessages.Margin = new System.Windows.Forms.Padding(6);
            this.cmdCreateFlexibleErrorMessages.Name = "cmdCreateFlexibleErrorMessages";
            this.cmdCreateFlexibleErrorMessages.Size = new System.Drawing.Size(396, 88);
            this.cmdCreateFlexibleErrorMessages.TabIndex = 0;
            this.cmdCreateFlexibleErrorMessages.Text = "Create WinCC-Flexible Error Messages && Tags";
            this.cmdCreateFlexibleErrorMessages.UseVisualStyleBackColor = true;
            this.cmdCreateFlexibleErrorMessages.Click += new System.EventHandler(this.cmdCreateFlexibleErrorMessages_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(1037, 30);
            this.label5.TabIndex = 15;
            this.label5.Text = "Step7, Simatic, WinCC && WinCC Flexible are trademarks of SIEMENS";
            // 
            // fetchPLCData
            // 
            this.fetchPLCData.Interval = 300;
            this.fetchPLCData.Tick += new System.EventHandler(this.fetchPLCData_Tick);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblToolStripFileSystemFolder,
            this.lblStatus,
            this.lblConnInfo,
            this.toolStripStatusLabel1,
            this.lblConnected});
            this.statusStrip.Location = new System.Drawing.Point(0, 892);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(2, 0, 26, 0);
            this.statusStrip.Size = new System.Drawing.Size(1866, 42);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 22;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblToolStripFileSystemFolder
            // 
            this.lblToolStripFileSystemFolder.AutoSize = false;
            this.lblToolStripFileSystemFolder.Name = "lblToolStripFileSystemFolder";
            this.lblToolStripFileSystemFolder.Size = new System.Drawing.Size(600, 32);
            this.lblToolStripFileSystemFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(936, 32);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "i.o.";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblConnInfo
            // 
            this.lblConnInfo.AutoSize = false;
            this.lblConnInfo.Name = "lblConnInfo";
            this.lblConnInfo.Size = new System.Drawing.Size(230, 32);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(29, 32);
            this.toolStripStatusLabel1.Text = "  ";
            // 
            // lblConnected
            // 
            this.lblConnected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblConnected.Name = "lblConnected";
            this.lblConnected.Size = new System.Drawing.Size(43, 32);
            this.lblConnected.Text = "    ";
            // 
            // lstProjects
            // 
            this.lstProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstProjects.FormattingEnabled = true;
            this.lstProjects.ItemHeight = 29;
            this.lstProjects.Location = new System.Drawing.Point(6, 6);
            this.lstProjects.Margin = new System.Windows.Forms.Padding(6);
            this.lstProjects.Name = "lstProjects";
            this.lstProjects.Size = new System.Drawing.Size(434, 190);
            this.lstProjects.TabIndex = 24;
            this.lstProjects.DoubleClick += new System.EventHandler(this.lstProjects_DoubleClick);
            // 
            // cmdProjectsBrowser
            // 
            this.cmdProjectsBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdProjectsBrowser.Font = new System.Drawing.Font("Courier New", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdProjectsBrowser.Location = new System.Drawing.Point(452, 6);
            this.cmdProjectsBrowser.Margin = new System.Windows.Forms.Padding(6);
            this.cmdProjectsBrowser.Name = "cmdProjectsBrowser";
            this.cmdProjectsBrowser.Size = new System.Drawing.Size(156, 190);
            this.cmdProjectsBrowser.TabIndex = 25;
            this.cmdProjectsBrowser.Text = "Projects\r\nBrowser";
            this.cmdProjectsBrowser.UseVisualStyleBackColor = true;
            this.cmdProjectsBrowser.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolsToolStripMenuItem1,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(12, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(1866, 44);
            this.menuStrip1.TabIndex = 26;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(72, 36);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(208, 44);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(208, 44);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configConnectionToolStripMenuItem,
            this.reachablePLCsToolStripMenuItem,
            this.lstConnections,
            this.toolStripSeparator1,
            this.watchToolStripMenuItem,
            this.unwatchToolStripMenuItem,
            this.toolStripSeparator2,
            this.downloadOnlineBlockToolStripMenuItem,
            this.toolStripSeparator3,
            this.dBStructResizerToolStripMenuItem,
            this.dataBlockValueSaveRestoreToolStripMenuItem,
            this.toolStripSeparator4,
            this.searchPasswordToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(106, 36);
            this.toolsToolStripMenuItem.Text = "Online";
            // 
            // configConnectionToolStripMenuItem
            // 
            this.configConnectionToolStripMenuItem.Name = "configConnectionToolStripMenuItem";
            this.configConnectionToolStripMenuItem.Size = new System.Drawing.Size(472, 44);
            this.configConnectionToolStripMenuItem.Text = "Config Connection";
            this.configConnectionToolStripMenuItem.Click += new System.EventHandler(this.configConnectionToolStripMenuItem_Click);
            // 
            // reachablePLCsToolStripMenuItem
            // 
            this.reachablePLCsToolStripMenuItem.Name = "reachablePLCsToolStripMenuItem";
            this.reachablePLCsToolStripMenuItem.Size = new System.Drawing.Size(472, 44);
            this.reachablePLCsToolStripMenuItem.Text = "Reachable PLC\'s";
            this.reachablePLCsToolStripMenuItem.Click += new System.EventHandler(this.reachablePLCsToolStripMenuItem_Click);
            // 
            // lstConnections
            // 
            this.lstConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstConnections.Name = "lstConnections";
            this.lstConnections.Size = new System.Drawing.Size(121, 40);
            this.lstConnections.SelectedIndexChanged += new System.EventHandler(this.lstConnections_SelectedIndexChanged_1);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(469, 6);
            // 
            // watchToolStripMenuItem
            // 
            this.watchToolStripMenuItem.Name = "watchToolStripMenuItem";
            this.watchToolStripMenuItem.Size = new System.Drawing.Size(472, 44);
            this.watchToolStripMenuItem.Text = "Watch Datablock";
            this.watchToolStripMenuItem.Click += new System.EventHandler(this.watchToolStripMenuItem_Click);
            // 
            // unwatchToolStripMenuItem
            // 
            this.unwatchToolStripMenuItem.Name = "unwatchToolStripMenuItem";
            this.unwatchToolStripMenuItem.Size = new System.Drawing.Size(472, 44);
            this.unwatchToolStripMenuItem.Text = "Unwatch";
            this.unwatchToolStripMenuItem.Click += new System.EventHandler(this.unwatchToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(469, 6);
            // 
            // downloadOnlineBlockToolStripMenuItem
            // 
            this.downloadOnlineBlockToolStripMenuItem.Name = "downloadOnlineBlockToolStripMenuItem";
            this.downloadOnlineBlockToolStripMenuItem.Size = new System.Drawing.Size(472, 44);
            this.downloadOnlineBlockToolStripMenuItem.Text = "Download Online Block";
            this.downloadOnlineBlockToolStripMenuItem.Click += new System.EventHandler(this.downloadOnlineBlockToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(469, 6);
            // 
            // dBStructResizerToolStripMenuItem
            // 
            this.dBStructResizerToolStripMenuItem.Name = "dBStructResizerToolStripMenuItem";
            this.dBStructResizerToolStripMenuItem.Size = new System.Drawing.Size(472, 44);
            this.dBStructResizerToolStripMenuItem.Text = "UDT-Array-Resizer";
            this.dBStructResizerToolStripMenuItem.Click += new System.EventHandler(this.dBStructResizerToolStripMenuItem_Click);
            // 
            // dataBlockValueSaveRestoreToolStripMenuItem
            // 
            this.dataBlockValueSaveRestoreToolStripMenuItem.Name = "dataBlockValueSaveRestoreToolStripMenuItem";
            this.dataBlockValueSaveRestoreToolStripMenuItem.Size = new System.Drawing.Size(472, 44);
            this.dataBlockValueSaveRestoreToolStripMenuItem.Text = "Data Block Value Save/Restore";
            this.dataBlockValueSaveRestoreToolStripMenuItem.Click += new System.EventHandler(this.dataBlockValueSaveRestoreToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(469, 6);
            // 
            // searchPasswordToolStripMenuItem
            // 
            this.searchPasswordToolStripMenuItem.Name = "searchPasswordToolStripMenuItem";
            this.searchPasswordToolStripMenuItem.Size = new System.Drawing.Size(472, 44);
            this.searchPasswordToolStripMenuItem.Text = "Search Password";
            this.searchPasswordToolStripMenuItem.Click += new System.EventHandler(this.searchPasswordToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convertCallsToolStripMenuItem,
            this.mnuExpandDatablockArrays});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(119, 36);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // convertCallsToolStripMenuItem
            // 
            this.convertCallsToolStripMenuItem.Checked = true;
            this.convertCallsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.convertCallsToolStripMenuItem.Name = "convertCallsToolStripMenuItem";
            this.convertCallsToolStripMenuItem.Size = new System.Drawing.Size(411, 44);
            this.convertCallsToolStripMenuItem.Text = "Convert UCs to Calls";
            this.convertCallsToolStripMenuItem.Click += new System.EventHandler(this.convertCallsToolStripMenuItem_Click);
            // 
            // mnuExpandDatablockArrays
            // 
            this.mnuExpandDatablockArrays.Name = "mnuExpandDatablockArrays";
            this.mnuExpandDatablockArrays.Size = new System.Drawing.Size(411, 44);
            this.mnuExpandDatablockArrays.Text = "Expand Datablock Arrays";
            this.mnuExpandDatablockArrays.Click += new System.EventHandler(this.mnuExpandDatablockArrays_Click);
            // 
            // toolsToolStripMenuItem1
            // 
            this.toolsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createDokumentationToolStripMenuItem,
            this.parseAllBlocksToolStripMenuItem,
            this.createAWLToolStripMenuItem,
            this.dependenciesToolStripMenuItem,
            this.callHirachyToolStripMenuItem});
            this.toolsToolStripMenuItem1.Name = "toolsToolStripMenuItem1";
            this.toolsToolStripMenuItem1.Size = new System.Drawing.Size(90, 36);
            this.toolsToolStripMenuItem1.Text = "Tools";
            // 
            // createDokumentationToolStripMenuItem
            // 
            this.createDokumentationToolStripMenuItem.Name = "createDokumentationToolStripMenuItem";
            this.createDokumentationToolStripMenuItem.Size = new System.Drawing.Size(392, 44);
            this.createDokumentationToolStripMenuItem.Text = "Create Dokumentation";
            this.createDokumentationToolStripMenuItem.Click += new System.EventHandler(this.createDokumentationToolStripMenuItem_Click);
            // 
            // parseAllBlocksToolStripMenuItem
            // 
            this.parseAllBlocksToolStripMenuItem.Name = "parseAllBlocksToolStripMenuItem";
            this.parseAllBlocksToolStripMenuItem.Size = new System.Drawing.Size(392, 44);
            this.parseAllBlocksToolStripMenuItem.Text = "Parse all Blocks";
            this.parseAllBlocksToolStripMenuItem.Click += new System.EventHandler(this.parseAllBlocksToolStripMenuItem_Click);
            // 
            // createAWLToolStripMenuItem
            // 
            this.createAWLToolStripMenuItem.Name = "createAWLToolStripMenuItem";
            this.createAWLToolStripMenuItem.Size = new System.Drawing.Size(392, 44);
            this.createAWLToolStripMenuItem.Text = "Create AWL";
            this.createAWLToolStripMenuItem.Click += new System.EventHandler(this.createAWLToolStripMenuItem_Click);
            // 
            // dependenciesToolStripMenuItem
            // 
            this.dependenciesToolStripMenuItem.Name = "dependenciesToolStripMenuItem";
            this.dependenciesToolStripMenuItem.Size = new System.Drawing.Size(392, 44);
            this.dependenciesToolStripMenuItem.Text = "Dependencies";
            this.dependenciesToolStripMenuItem.Click += new System.EventHandler(this.dependenciesToolStripMenuItem_Click);
            // 
            // callHirachyToolStripMenuItem
            // 
            this.callHirachyToolStripMenuItem.Name = "callHirachyToolStripMenuItem";
            this.callHirachyToolStripMenuItem.Size = new System.Drawing.Size(392, 44);
            this.callHirachyToolStripMenuItem.Text = "Call-Hierarchy";
            this.callHirachyToolStripMenuItem.Click += new System.EventHandler(this.callHirachyToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.featuresToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(85, 36);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // featuresToolStripMenuItem
            // 
            this.featuresToolStripMenuItem.Name = "featuresToolStripMenuItem";
            this.featuresToolStripMenuItem.Size = new System.Drawing.Size(239, 44);
            this.featuresToolStripMenuItem.Text = "Features";
            this.featuresToolStripMenuItem.Click += new System.EventHandler(this.featuresToolStripMenuItem_Click);
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(6, 142);
            this.mainSplitContainer.Margin = new System.Windows.Forms.Padding(6);
            this.mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.dtaSymbolTable);
            this.mainSplitContainer.Panel2.Controls.Add(this.viewBlockList);
            this.mainSplitContainer.Panel2.Controls.Add(this.datablockView);
            this.mainSplitContainer.Panel2.Controls.Add(this.hexBox);
            this.mainSplitContainer.Panel2.Controls.Add(this.txtTextBox);
            this.mainSplitContainer.Panel2.Controls.Add(this.dtaPnPbList);
            this.mainSplitContainer.Size = new System.Drawing.Size(1854, 700);
            this.mainSplitContainer.SplitterDistance = 626;
            this.mainSplitContainer.SplitterWidth = 8;
            this.mainSplitContainer.TabIndex = 26;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.treeStep7Project, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkShowDeleted, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 214F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(626, 700);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 168F));
            this.tableLayoutPanel2.Controls.Add(this.lstProjects, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmdProjectsBrowser, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 444);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(614, 202);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // viewBlockList
            // 
            this.viewBlockList.Controls.Add(this.tableLayoutPanel3);
            this.viewBlockList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewBlockList.Location = new System.Drawing.Point(0, 0);
            this.viewBlockList.Margin = new System.Windows.Forms.Padding(6);
            this.viewBlockList.Name = "viewBlockList";
            this.viewBlockList.Padding = new System.Windows.Forms.Padding(6);
            this.viewBlockList.Size = new System.Drawing.Size(1220, 700);
            this.viewBlockList.TabIndex = 0;
            this.viewBlockList.TabStop = false;
            this.viewBlockList.Visible = false;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanelVisu, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 36);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1208, 658);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.cmdSetKnowHow);
            this.flowLayoutPanel1.Controls.Add(this.cmdRemoveKnowHow);
            this.flowLayoutPanel1.Controls.Add(this.cmdUndeleteBlock);
            this.flowLayoutPanel1.Controls.Add(this.txtUndeleteName);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(6, 596);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1196, 56);
            this.flowLayoutPanel1.TabIndex = 11;
            // 
            // tableLayoutPanelVisu
            // 
            this.tableLayoutPanelVisu.ColumnCount = 2;
            this.tableLayoutPanelVisu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelVisu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 476F));
            this.tableLayoutPanelVisu.Controls.Add(this.grpVisu, 1, 0);
            this.tableLayoutPanelVisu.Controls.Add(this.lstListBox, 0, 0);
            this.tableLayoutPanelVisu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelVisu.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanelVisu.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanelVisu.Name = "tableLayoutPanelVisu";
            this.tableLayoutPanelVisu.RowCount = 1;
            this.tableLayoutPanelVisu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelVisu.Size = new System.Drawing.Size(1196, 578);
            this.tableLayoutPanelVisu.TabIndex = 12;
            // 
            // datablockView
            // 
            this.datablockView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.datablockView.Location = new System.Drawing.Point(0, 0);
            this.datablockView.Margin = new System.Windows.Forms.Padding(6);
            this.datablockView.Name = "datablockView";
            this.datablockView.Size = new System.Drawing.Size(1220, 700);
            this.datablockView.TabIndex = 17;
            this.datablockView.Text = "wpfElementHost";
            this.datablockView.Visible = false;
            this.datablockView.ChildChanged += new System.EventHandler<System.Windows.Forms.Integration.ChildChangedEventArgs>(this.elementHost1_ChildChanged);            
            this.datablockView.Child = this.dataBlockViewControl;
            // 
            // dtaPnPbList
            // 
            this.dtaPnPbList.AllowUserToAddRows = false;
            this.dtaPnPbList.AllowUserToDeleteRows = false;
            this.dtaPnPbList.AllowUserToResizeRows = false;
            this.dtaPnPbList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtaPnPbList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dtaPnPbList.ContextMenuStrip = this.exportMenu;
            this.dtaPnPbList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtaPnPbList.Location = new System.Drawing.Point(0, 0);
            this.dtaPnPbList.Margin = new System.Windows.Forms.Padding(6);
            this.dtaPnPbList.Name = "dtaPnPbList";
            this.dtaPnPbList.RowHeadersWidth = 82;
            this.dtaPnPbList.Size = new System.Drawing.Size(1220, 700);
            this.dtaPnPbList.TabIndex = 19;
            this.dtaPnPbList.Visible = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "id";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 10;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 200;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "name";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 10;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 200;
            // 
            // exportMenu
            // 
            this.exportMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.exportMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.export});
            this.exportMenu.Name = "exportMenu";
            this.exportMenu.Size = new System.Drawing.Size(158, 42);
            // 
            // export
            // 
            this.export.Name = "export";
            this.export.Size = new System.Drawing.Size(157, 38);
            this.export.Text = "Export";
            this.export.Click += new System.EventHandler(this.export_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.mainSplitContainer, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 44);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 136F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1866, 848);
            this.tableLayoutPanel5.TabIndex = 28;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lblProjectName);
            this.panel1.Controls.Add(this.lblProjectInfo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(6, 6);
            this.panel1.Margin = new System.Windows.Forms.Padding(6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1854, 124);
            this.panel1.TabIndex = 0;
            // 
            // hexBox
            // 
            this.hexBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox.InfoForeColor = System.Drawing.Color.Empty;
            this.hexBox.Location = new System.Drawing.Point(0, 0);
            this.hexBox.Margin = new System.Windows.Forms.Padding(6);
            this.hexBox.Name = "hexBox";
            this.hexBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox.Size = new System.Drawing.Size(1220, 700);
            this.hexBox.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1866, 934);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Courier New", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Toolbox for Siemens PLCs (c) 2010/2011 Jochen Kühner";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtaSymbolTable)).EndInit();
            this.grpVisu.ResumeLayout(false);
            this.grpVisu.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            this.mainSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.viewBlockList.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanelVisu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtaPnPbList)).EndInit();
            this.exportMenu.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProjectInfo;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.TreeView treeStep7Project;
        private System.Windows.Forms.DataGridView dtaSymbolTable;
        private System.Windows.Forms.ListBox lstListBox;
        private System.Windows.Forms.TextBox txtTextBox;
        private System.Windows.Forms.CheckBox chkShowDeleted;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cmdSetKnowHow;
        private System.Windows.Forms.Button cmdRemoveKnowHow;
        private System.Windows.Forms.Button cmdUndeleteBlock;
        private System.Windows.Forms.TextBox txtUndeleteName;
        private System.Windows.Forms.GroupBox grpVisu;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtConnectionName;
        private System.Windows.Forms.Button cmdCreateWinCCTags;
        private System.Windows.Forms.Button cmdCreateWinCCErrorMessages;
        private System.Windows.Forms.Button cmdCreateFlexibleErrorMessages;
        private System.Windows.Forms.CheckBox chkFixedErrorNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtStartErrorNumber;
        private System.Windows.Forms.ImageList imglstIconsForTreeview;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTagsPrefix;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkExpandArrays;
        private System.Windows.Forms.Integration.ElementHost datablockView;
        private TestProjectFileFunctions.DataBlockViewControl dataBlockViewControl;
        private System.Windows.Forms.Timer fetchPLCData;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblConnInfo;
        private System.Windows.Forms.ListBox lstProjects;
        private System.Windows.Forms.Button cmdProjectsBrowser;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem featuresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox viewBlockList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelVisu;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem configConnectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox lstConnections;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem watchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unwatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lblConnected;
        private System.Windows.Forms.ToolStripStatusLabel lblToolStripFileSystemFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem downloadOnlineBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertCallsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem dBStructResizerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuExpandDatablockArrays;
        private System.Windows.Forms.CheckBox chkUseErrPrefix;
        private System.Windows.Forms.TextBox txtErrPrefix;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem searchPasswordToolStripMenuItem;
        private System.Windows.Forms.Button cmdCreateWinCCFlexibleTags;
        private System.Windows.Forms.CheckBox chkCombineStructComments;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem createDokumentationToolStripMenuItem;
        private Be.Windows.Forms.HexBox hexBox;
        private System.Windows.Forms.ToolStripMenuItem parseAllBlocksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createAWLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dependenciesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem callHirachyToolStripMenuItem;
        private System.Windows.Forms.Button cmdWebfactoryTags;
        private System.Windows.Forms.ToolStripMenuItem dataBlockValueSaveRestoreToolStripMenuItem;
        private System.Windows.Forms.DataGridView dtaPnPbList;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.ContextMenuStrip exportMenu;
        private System.Windows.Forms.ToolStripMenuItem export;
        private System.Windows.Forms.DataGridViewTextBoxColumn symbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn datatype;
        private System.Windows.Forms.DataGridViewTextBoxColumn operand;
        private System.Windows.Forms.DataGridViewTextBoxColumn operandiec;
        private System.Windows.Forms.DataGridViewTextBoxColumn comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn tia_key;
        private System.Windows.Forms.ToolStripMenuItem reachablePLCsToolStripMenuItem;
    }
}

