namespace SimpleTcpSocket
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdConnect = new System.Windows.Forms.Button();
            this.tabMainTab = new System.Windows.Forms.TabControl();
            this.tabPageLogger = new System.Windows.Forms.TabPage();
            this.tabPageSender = new System.Windows.Forms.TabPage();
            this.cmdSave = new System.Windows.Forms.Button();
            this.txtRecieve = new System.Windows.Forms.TextBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cmdSend = new System.Windows.Forms.Button();
            this.txtTelegramm = new System.Windows.Forms.TextBox();
            this.txtSended = new System.Windows.Forms.TextBox();
            this.txtRecieve2 = new System.Windows.Forms.TextBox();
            this.tabMainTab.SuspendLayout();
            this.tabPageLogger.SuspendLayout();
            this.tabPageSender.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port:";
            // 
            // cmdConnect
            // 
            this.cmdConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdConnect.Location = new System.Drawing.Point(297, 6);
            this.cmdConnect.Name = "cmdConnect";
            this.cmdConnect.Size = new System.Drawing.Size(145, 33);
            this.cmdConnect.TabIndex = 5;
            this.cmdConnect.Text = "Connect";
            this.cmdConnect.UseVisualStyleBackColor = false;
            this.cmdConnect.Click += new System.EventHandler(this.cmdConnect_Click);
            // 
            // tabMainTab
            // 
            this.tabMainTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMainTab.Controls.Add(this.tabPageLogger);
            this.tabMainTab.Controls.Add(this.tabPageSender);
            this.tabMainTab.Location = new System.Drawing.Point(12, 45);
            this.tabMainTab.Name = "tabMainTab";
            this.tabMainTab.SelectedIndex = 0;
            this.tabMainTab.Size = new System.Drawing.Size(676, 373);
            this.tabMainTab.TabIndex = 6;
            // 
            // tabPageLogger
            // 
            this.tabPageLogger.Controls.Add(this.txtRecieve);
            this.tabPageLogger.Location = new System.Drawing.Point(4, 22);
            this.tabPageLogger.Name = "tabPageLogger";
            this.tabPageLogger.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLogger.Size = new System.Drawing.Size(668, 347);
            this.tabPageLogger.TabIndex = 0;
            this.tabPageLogger.Text = "Logger";
            this.tabPageLogger.UseVisualStyleBackColor = true;
            // 
            // tabPageSender
            // 
            this.tabPageSender.Controls.Add(this.tableLayoutPanel1);
            this.tabPageSender.Location = new System.Drawing.Point(4, 22);
            this.tabPageSender.Name = "tabPageSender";
            this.tabPageSender.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSender.Size = new System.Drawing.Size(668, 347);
            this.tabPageSender.TabIndex = 1;
            this.tabPageSender.Text = "Sender";
            this.tabPageSender.UseVisualStyleBackColor = true;
            // 
            // cmdSave
            // 
            this.cmdSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdSave.Location = new System.Drawing.Point(594, 6);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(90, 33);
            this.cmdSave.TabIndex = 7;
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = false;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // txtRecieve
            // 
            this.txtRecieve.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecieve.Location = new System.Drawing.Point(3, 3);
            this.txtRecieve.Multiline = true;
            this.txtRecieve.Name = "txtRecieve";
            this.txtRecieve.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRecieve.Size = new System.Drawing.Size(662, 341);
            this.txtRecieve.TabIndex = 0;
            this.txtRecieve.WordWrap = false;
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.Checked = global::SimpleTcpSocket.Properties.Settings.Default.Active;
            this.chkActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActive.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::SimpleTcpSocket.Properties.Settings.Default, "Active", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkActive.Location = new System.Drawing.Point(240, 8);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(56, 17);
            this.chkActive.TabIndex = 4;
            this.chkActive.Text = "Active";
            this.chkActive.UseVisualStyleBackColor = true;
            // 
            // txtPort
            // 
            this.txtPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::SimpleTcpSocket.Properties.Settings.Default, "Port", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtPort.Location = new System.Drawing.Point(182, 6);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(52, 20);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = global::SimpleTcpSocket.Properties.Settings.Default.Port;
            // 
            // txtIP
            // 
            this.txtIP.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::SimpleTcpSocket.Properties.Settings.Default, "IP", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtIP.Location = new System.Drawing.Point(38, 6);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(99, 20);
            this.txtIP.TabIndex = 1;
            this.txtIP.Text = global::SimpleTcpSocket.Properties.Settings.Default.IP;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.55719F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.44282F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(662, 341);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 39);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtSended);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtRecieve2);
            this.splitContainer1.Size = new System.Drawing.Size(656, 299);
            this.splitContainer1.SplitterDistance = 156;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 81.4262F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.5738F));
            this.tableLayoutPanel2.Controls.Add(this.cmdSend, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtTelegramm, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(656, 30);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // cmdSend
            // 
            this.cmdSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdSend.Location = new System.Drawing.Point(537, 3);
            this.cmdSend.Name = "cmdSend";
            this.cmdSend.Size = new System.Drawing.Size(116, 24);
            this.cmdSend.TabIndex = 8;
            this.cmdSend.Text = "Send";
            this.cmdSend.UseVisualStyleBackColor = false;
            this.cmdSend.Click += new System.EventHandler(this.cmdSend_Click);
            // 
            // txtTelegramm
            // 
            this.txtTelegramm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTelegramm.Location = new System.Drawing.Point(3, 5);
            this.txtTelegramm.Name = "txtTelegramm";
            this.txtTelegramm.Size = new System.Drawing.Size(528, 20);
            this.txtTelegramm.TabIndex = 9;
            // 
            // txtSended
            // 
            this.txtSended.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSended.Location = new System.Drawing.Point(0, 0);
            this.txtSended.Multiline = true;
            this.txtSended.Name = "txtSended";
            this.txtSended.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSended.Size = new System.Drawing.Size(656, 156);
            this.txtSended.TabIndex = 0;
            this.txtSended.WordWrap = false;
            // 
            // txtRecieve2
            // 
            this.txtRecieve2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecieve2.Location = new System.Drawing.Point(0, 0);
            this.txtRecieve2.Multiline = true;
            this.txtRecieve2.Name = "txtRecieve2";
            this.txtRecieve2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRecieve2.Size = new System.Drawing.Size(656, 139);
            this.txtRecieve2.TabIndex = 1;
            this.txtRecieve2.WordWrap = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 430);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.tabMainTab);
            this.Controls.Add(this.cmdConnect);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "TCP-Test";
            this.tabMainTab.ResumeLayout(false);
            this.tabPageLogger.ResumeLayout(false);
            this.tabPageLogger.PerformLayout();
            this.tabPageSender.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Button cmdConnect;
        private System.Windows.Forms.TabControl tabMainTab;
        private System.Windows.Forms.TabPage tabPageLogger;
        private System.Windows.Forms.TabPage tabPageSender;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.TextBox txtRecieve;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtSended;
        private System.Windows.Forms.TextBox txtRecieve2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button cmdSend;
        private System.Windows.Forms.TextBox txtTelegramm;
    }
}

