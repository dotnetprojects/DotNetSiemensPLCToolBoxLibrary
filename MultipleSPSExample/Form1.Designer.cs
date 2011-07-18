namespace MultipleSPSExample
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connection1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connection2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connection3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connection4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connection5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connection6ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.systemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.read_timer_plc1 = new System.Windows.Forms.Timer(this.components);
            this.read_timer_plc2 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem,
            this.systemToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip";
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connection1ToolStripMenuItem,
            this.connection2ToolStripMenuItem,
            this.connection3ToolStripMenuItem,
            this.connection4ToolStripMenuItem,
            this.connection5ToolStripMenuItem,
            this.connection6ToolStripMenuItem});
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.configurationToolStripMenuItem.Text = "Configuration";
            // 
            // connection1ToolStripMenuItem
            // 
            this.connection1ToolStripMenuItem.Name = "connection1ToolStripMenuItem";
            this.connection1ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.connection1ToolStripMenuItem.Text = "Connection 1";
            this.connection1ToolStripMenuItem.Click += new System.EventHandler(this.connection1ToolStripMenuItem_Click);
            // 
            // connection2ToolStripMenuItem
            // 
            this.connection2ToolStripMenuItem.Name = "connection2ToolStripMenuItem";
            this.connection2ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.connection2ToolStripMenuItem.Text = "Connection 2";
            this.connection2ToolStripMenuItem.Click += new System.EventHandler(this.connection2ToolStripMenuItem_Click);
            // 
            // connection3ToolStripMenuItem
            // 
            this.connection3ToolStripMenuItem.Name = "connection3ToolStripMenuItem";
            this.connection3ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.connection3ToolStripMenuItem.Text = "Connection 3";
            this.connection3ToolStripMenuItem.Click += new System.EventHandler(this.connection3ToolStripMenuItem_Click);
            // 
            // connection4ToolStripMenuItem
            // 
            this.connection4ToolStripMenuItem.Name = "connection4ToolStripMenuItem";
            this.connection4ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.connection4ToolStripMenuItem.Text = "Connection 4";
            this.connection4ToolStripMenuItem.Click += new System.EventHandler(this.connection4ToolStripMenuItem_Click);
            // 
            // connection5ToolStripMenuItem
            // 
            this.connection5ToolStripMenuItem.Name = "connection5ToolStripMenuItem";
            this.connection5ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.connection5ToolStripMenuItem.Text = "Connection 5";
            this.connection5ToolStripMenuItem.Click += new System.EventHandler(this.connection5ToolStripMenuItem_Click);
            // 
            // connection6ToolStripMenuItem
            // 
            this.connection6ToolStripMenuItem.Name = "connection6ToolStripMenuItem";
            this.connection6ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.connection6ToolStripMenuItem.Text = "Connection 6";
            this.connection6ToolStripMenuItem.Click += new System.EventHandler(this.connection6ToolStripMenuItem_Click);
            // 
            // systemToolStripMenuItem
            // 
            this.systemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem});
            this.systemToolStripMenuItem.Name = "systemToolStripMenuItem";
            this.systemToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.systemToolStripMenuItem.Text = "System";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // read_timer_plc1
            // 
            this.read_timer_plc1.Interval = 1000;
            this.read_timer_plc1.Tick += new System.EventHandler(this.read_timer_plc1_Tick);
            // 
            // read_timer_plc2
            // 
            this.read_timer_plc2.Interval = 1000;
            this.read_timer_plc2.Tick += new System.EventHandler(this.read_timer_plc2_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "MultipleSPSExample";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connection1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connection2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connection3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connection4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connection5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connection6ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem systemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.Timer read_timer_plc1;
        private System.Windows.Forms.Timer read_timer_plc2;
    }
}

