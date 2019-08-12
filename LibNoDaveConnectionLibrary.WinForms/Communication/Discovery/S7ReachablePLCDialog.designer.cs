namespace DotNetSiemensPLCToolBoxLibrary.Communication.Discovery
{
    partial class S7ReachablePLCDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(S7ReachablePLCDialog));
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonOK = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAbort = new System.Windows.Forms.ToolStripButton();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.listViewFoundPlcs = new System.Windows.Forms.ListView();
            this.columnHeaderAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderManufacturer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSlot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageListPlc = new System.Windows.Forms.ImageList(this.components);
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMain
            // 
            resources.ApplyResources(this.toolStripMain, "toolStripMain");
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOK,
            this.toolStripButtonCancel,
            this.toolStripSeparator1,
            this.toolStripButtonRefresh,
            this.toolStripButtonAbort,
            this.toolStripProgressBar});
            this.toolStripMain.Name = "toolStripMain";
            // 
            // toolStripButtonOK
            // 
            resources.ApplyResources(this.toolStripButtonOK, "toolStripButtonOK");
            this.toolStripButtonOK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonOK.Name = "toolStripButtonOK";
            this.toolStripButtonOK.Click += new System.EventHandler(this.ToolStripButtonOK_Click);
            // 
            // toolStripButtonCancel
            // 
            resources.ApplyResources(this.toolStripButtonCancel, "toolStripButtonCancel");
            this.toolStripButtonCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonCancel.Name = "toolStripButtonCancel";
            this.toolStripButtonCancel.Click += new System.EventHandler(this.ToolStripButtonCancel_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // toolStripButtonRefresh
            // 
            resources.ApplyResources(this.toolStripButtonRefresh, "toolStripButtonRefresh");
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.ToolStripButtonRefresh_Click);
            // 
            // toolStripButtonAbort
            // 
            resources.ApplyResources(this.toolStripButtonAbort, "toolStripButtonAbort");
            this.toolStripButtonAbort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAbort.Name = "toolStripButtonAbort";
            this.toolStripButtonAbort.Click += new System.EventHandler(this.ToolStripButtonAbort_Click);
            // 
            // toolStripProgressBar
            // 
            resources.ApplyResources(this.toolStripProgressBar, "toolStripProgressBar");
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            // 
            // listViewFoundPlcs
            // 
            resources.ApplyResources(this.listViewFoundPlcs, "listViewFoundPlcs");
            this.listViewFoundPlcs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderAddress,
            this.columnHeaderName,
            this.columnHeaderType,
            this.columnHeaderManufacturer,
            this.columnHeaderSlot});
            this.listViewFoundPlcs.MultiSelect = false;
            this.listViewFoundPlcs.Name = "listViewFoundPlcs";
            this.listViewFoundPlcs.UseCompatibleStateImageBehavior = false;
            this.listViewFoundPlcs.View = System.Windows.Forms.View.Details;
            this.listViewFoundPlcs.SelectedIndexChanged += new System.EventHandler(this.listViewFoundPlcs_SelectedIndexChanged);
            // 
            // columnHeaderAddress
            // 
            resources.ApplyResources(this.columnHeaderAddress, "columnHeaderAddress");
            // 
            // columnHeaderName
            // 
            resources.ApplyResources(this.columnHeaderName, "columnHeaderName");
            // 
            // columnHeaderType
            // 
            resources.ApplyResources(this.columnHeaderType, "columnHeaderType");
            // 
            // columnHeaderManufacturer
            // 
            resources.ApplyResources(this.columnHeaderManufacturer, "columnHeaderManufacturer");
            // 
            // columnHeaderSlot
            // 
            resources.ApplyResources(this.columnHeaderSlot, "columnHeaderSlot");
            // 
            // imageListPlc
            // 
            this.imageListPlc.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListPlc.ImageStream")));
            this.imageListPlc.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListPlc.Images.SetKeyName(0, "S7-300");
            this.imageListPlc.Images.SetKeyName(1, "S7-400");
            // 
            // S7ReachablePLCDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.listViewFoundPlcs);
            this.Controls.Add(this.toolStripMain);
            this.Name = "S7ReachablePLCDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReachablePlcDialog_FormClosing);
            this.Shown += new System.EventHandler(this.ReachablePlcDialog_Shown);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ListView listViewFoundPlcs;
        private System.Windows.Forms.ToolStripButton toolStripButtonOK;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripButton toolStripButtonAbort;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ColumnHeader columnHeaderAddress;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.ColumnHeader columnHeaderManufacturer;
        private System.Windows.Forms.ColumnHeader columnHeaderSlot;
        private System.Windows.Forms.ImageList imageListPlc;
    }
}

