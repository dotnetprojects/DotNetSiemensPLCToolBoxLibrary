namespace TCPForwarder
{
    partial class ServiceConfig
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
            this.cmdServiceStart = new System.Windows.Forms.Button();
            this.cmdServiceInstall = new System.Windows.Forms.Button();
            this.cmdServiceStop = new System.Windows.Forms.Button();
            this.cmdServiceUninstall = new System.Windows.Forms.Button();
            this.serviceState = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.servicezustand = new System.Windows.Forms.Timer(this.components);
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmdServiceStart
            // 
            this.cmdServiceStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdServiceStart.Location = new System.Drawing.Point(12, 48);
            this.cmdServiceStart.Name = "cmdServiceStart";
            this.cmdServiceStart.Size = new System.Drawing.Size(57, 25);
            this.cmdServiceStart.TabIndex = 13;
            this.cmdServiceStart.Text = "start";
            this.cmdServiceStart.UseVisualStyleBackColor = true;
            this.cmdServiceStart.Click += new System.EventHandler(this.cmdServiceStart_Click);
            // 
            // cmdServiceInstall
            // 
            this.cmdServiceInstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdServiceInstall.Location = new System.Drawing.Point(12, 129);
            this.cmdServiceInstall.Name = "cmdServiceInstall";
            this.cmdServiceInstall.Size = new System.Drawing.Size(57, 25);
            this.cmdServiceInstall.TabIndex = 14;
            this.cmdServiceInstall.Text = "install";
            this.cmdServiceInstall.UseVisualStyleBackColor = true;
            this.cmdServiceInstall.Click += new System.EventHandler(this.cmdServiceInstall_Click);
            // 
            // cmdServiceStop
            // 
            this.cmdServiceStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdServiceStop.Location = new System.Drawing.Point(97, 48);
            this.cmdServiceStop.Name = "cmdServiceStop";
            this.cmdServiceStop.Size = new System.Drawing.Size(57, 25);
            this.cmdServiceStop.TabIndex = 15;
            this.cmdServiceStop.Text = "stop";
            this.cmdServiceStop.UseVisualStyleBackColor = true;
            this.cmdServiceStop.Click += new System.EventHandler(this.cmdServiceStop_Click);
            // 
            // cmdServiceUninstall
            // 
            this.cmdServiceUninstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdServiceUninstall.Location = new System.Drawing.Point(97, 129);
            this.cmdServiceUninstall.Name = "cmdServiceUninstall";
            this.cmdServiceUninstall.Size = new System.Drawing.Size(57, 25);
            this.cmdServiceUninstall.TabIndex = 16;
            this.cmdServiceUninstall.Text = "uninstall";
            this.cmdServiceUninstall.UseVisualStyleBackColor = true;
            this.cmdServiceUninstall.Click += new System.EventHandler(this.cmdServiceUninstall_Click);
            // 
            // serviceState
            // 
            this.serviceState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serviceState.Location = new System.Drawing.Point(12, 29);
            this.serviceState.Name = "serviceState";
            this.serviceState.Size = new System.Drawing.Size(142, 16);
            this.serviceState.TabIndex = 12;
            this.serviceState.Text = "ServiceState";
            this.serviceState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(12, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 20);
            this.label10.TabIndex = 17;
            this.label10.Text = "Service:";
            // 
            // cmdClose
            // 
            this.cmdClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdClose.Location = new System.Drawing.Point(223, 9);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(88, 82);
            this.cmdClose.TabIndex = 18;
            this.cmdClose.Text = "Ok";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // servicezustand
            // 
            this.servicezustand.Enabled = true;
            this.servicezustand.Interval = 1000;
            this.servicezustand.Tick += new System.EventHandler(this.servicezustand_Tick);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Location = new System.Drawing.Point(12, 232);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(304, 196);
            this.propertyGrid.TabIndex = 19;
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(228, 198);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 28);
            this.btnSave.TabIndex = 20;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 206);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Config:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // ServiceConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 440);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cmdServiceStart);
            this.Controls.Add(this.cmdServiceInstall);
            this.Controls.Add(this.cmdServiceStop);
            this.Controls.Add(this.cmdServiceUninstall);
            this.Controls.Add(this.serviceState);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ServiceConfig";
            this.Text = "ServiceConfig";
            this.Load += new System.EventHandler(this.ServiceConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdServiceStart;
        private System.Windows.Forms.Button cmdServiceInstall;
        private System.Windows.Forms.Button cmdServiceStop;
        private System.Windows.Forms.Button cmdServiceUninstall;
        private System.Windows.Forms.Label serviceState;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Timer servicezustand;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
    }
}