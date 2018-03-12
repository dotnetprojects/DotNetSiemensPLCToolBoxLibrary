namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    partial class NckTagEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NckTagEditor));
            this.pGridNCK = new System.Windows.Forms.PropertyGrid();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.oKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abbruchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pGridNCK
            // 
            resources.ApplyResources(this.pGridNCK, "pGridNCK");
            this.pGridNCK.Name = "pGridNCK";
            this.pGridNCK.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oKToolStripMenuItem,
            this.abbruchToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // oKToolStripMenuItem
            // 
            resources.ApplyResources(this.oKToolStripMenuItem, "oKToolStripMenuItem");
            this.oKToolStripMenuItem.Name = "oKToolStripMenuItem";
            this.oKToolStripMenuItem.Click += new System.EventHandler(this.OK_Click);
            // 
            // abbruchToolStripMenuItem
            // 
            resources.ApplyResources(this.abbruchToolStripMenuItem, "abbruchToolStripMenuItem");
            this.abbruchToolStripMenuItem.Name = "abbruchToolStripMenuItem";
            this.abbruchToolStripMenuItem.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // NckTagEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.pGridNCK);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "NckTagEditor";
            this.Load += new System.EventHandler(this.LibNoDaveValueEditor_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pGridNCK;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem oKToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abbruchToolStripMenuItem;

    }
}