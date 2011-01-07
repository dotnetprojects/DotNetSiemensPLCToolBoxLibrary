namespace JFK_VarTab
{
    partial class Vartab
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
            this.dataGridViewVarTab = new System.Windows.Forms.DataGridView();
            this.Addresse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Typ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DisplayTyp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Wert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Steuerwert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.symbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cmdConfigConnection = new System.Windows.Forms.Button();
            this.lstConnections = new System.Windows.Forms.ComboBox();
            this.cmdConnect = new System.Windows.Forms.Button();
            this.cmdDisconnect = new System.Windows.Forms.Button();
            this.cmdView = new System.Windows.Forms.Button();
            this.cmdStopView = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cmdControlValues = new System.Windows.Forms.Button();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdLoad = new System.Windows.Forms.Button();
            this.cmdLoadVat = new System.Windows.Forms.Button();
            this.cmdLoadSymboltable = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.conninfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.errtxt = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVarTab)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewVarTab
            // 
            this.dataGridViewVarTab.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewVarTab.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Addresse,
            this.Typ,
            this.DisplayTyp,
            this.Wert,
            this.Steuerwert,
            this.symbol,
            this.comments});
            this.dataGridViewVarTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewVarTab.Location = new System.Drawing.Point(3, 43);
            this.dataGridViewVarTab.Name = "dataGridViewVarTab";
            this.dataGridViewVarTab.Size = new System.Drawing.Size(1055, 470);
            this.dataGridViewVarTab.TabIndex = 0;
            this.dataGridViewVarTab.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewVarTab_CellContentClick);
            this.dataGridViewVarTab.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridViewVarTab.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView1_RowsRemoved);
            // 
            // Addresse
            // 
            this.Addresse.HeaderText = "address";
            this.Addresse.Name = "Addresse";
            // 
            // Typ
            // 
            this.Typ.HeaderText = "data-type";
            this.Typ.Name = "Typ";
            // 
            // DisplayTyp
            // 
            this.DisplayTyp.HeaderText = "view-type";
            this.DisplayTyp.Name = "DisplayTyp";
            // 
            // Wert
            // 
            this.Wert.HeaderText = "value";
            this.Wert.Name = "Wert";
            // 
            // Steuerwert
            // 
            this.Steuerwert.HeaderText = "control-value";
            this.Steuerwert.Name = "Steuerwert";
            // 
            // symbol
            // 
            this.symbol.HeaderText = "symbol";
            this.symbol.Name = "symbol";
            // 
            // comments
            // 
            this.comments.HeaderText = "comments";
            this.comments.Name = "comments";
            // 
            // timer1
            // 
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewVarTab, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1061, 516);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.cmdConfigConnection);
            this.flowLayoutPanel1.Controls.Add(this.lstConnections);
            this.flowLayoutPanel1.Controls.Add(this.cmdConnect);
            this.flowLayoutPanel1.Controls.Add(this.cmdDisconnect);
            this.flowLayoutPanel1.Controls.Add(this.cmdView);
            this.flowLayoutPanel1.Controls.Add(this.cmdStopView);
            this.flowLayoutPanel1.Controls.Add(this.progressBar1);
            this.flowLayoutPanel1.Controls.Add(this.cmdControlValues);
            this.flowLayoutPanel1.Controls.Add(this.cmdSave);
            this.flowLayoutPanel1.Controls.Add(this.cmdLoad);
            this.flowLayoutPanel1.Controls.Add(this.cmdLoadVat);
            this.flowLayoutPanel1.Controls.Add(this.cmdLoadSymboltable);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1055, 34);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // cmdConfigConnection
            // 
            this.cmdConfigConnection.Location = new System.Drawing.Point(3, 3);
            this.cmdConfigConnection.Name = "cmdConfigConnection";
            this.cmdConfigConnection.Size = new System.Drawing.Size(64, 31);
            this.cmdConfigConnection.TabIndex = 3;
            this.cmdConfigConnection.Text = "config";
            this.cmdConfigConnection.UseVisualStyleBackColor = true;
            this.cmdConfigConnection.Click += new System.EventHandler(this.cmdConfig_Click);
            // 
            // lstConnections
            // 
            this.lstConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstConnections.FormattingEnabled = true;
            this.lstConnections.ItemHeight = 13;
            this.lstConnections.Location = new System.Drawing.Point(73, 7);
            this.lstConnections.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.lstConnections.Name = "lstConnections";
            this.lstConnections.Size = new System.Drawing.Size(121, 21);
            this.lstConnections.TabIndex = 11;
            this.lstConnections.SelectedIndexChanged += new System.EventHandler(this.lstConnections_SelectedIndexChanged);
            // 
            // cmdConnect
            // 
            this.cmdConnect.Location = new System.Drawing.Point(200, 3);
            this.cmdConnect.Name = "cmdConnect";
            this.cmdConnect.Size = new System.Drawing.Size(75, 31);
            this.cmdConnect.TabIndex = 4;
            this.cmdConnect.Text = "connect";
            this.cmdConnect.UseVisualStyleBackColor = true;
            this.cmdConnect.Click += new System.EventHandler(this.cmdConnect_Click);
            // 
            // cmdDisconnect
            // 
            this.cmdDisconnect.Location = new System.Drawing.Point(281, 3);
            this.cmdDisconnect.Name = "cmdDisconnect";
            this.cmdDisconnect.Size = new System.Drawing.Size(75, 31);
            this.cmdDisconnect.TabIndex = 4;
            this.cmdDisconnect.Text = "disconnect";
            this.cmdDisconnect.UseVisualStyleBackColor = true;
            this.cmdDisconnect.Click += new System.EventHandler(this.cmdDisconnect_Click);
            // 
            // cmdView
            // 
            this.cmdView.Location = new System.Drawing.Point(362, 3);
            this.cmdView.Name = "cmdView";
            this.cmdView.Size = new System.Drawing.Size(72, 31);
            this.cmdView.TabIndex = 2;
            this.cmdView.Text = "view";
            this.cmdView.UseVisualStyleBackColor = true;
            this.cmdView.Click += new System.EventHandler(this.cmdView_Click);
            // 
            // cmdStopView
            // 
            this.cmdStopView.Location = new System.Drawing.Point(440, 3);
            this.cmdStopView.Name = "cmdStopView";
            this.cmdStopView.Size = new System.Drawing.Size(72, 31);
            this.cmdStopView.TabIndex = 5;
            this.cmdStopView.Text = "stop view";
            this.cmdStopView.UseVisualStyleBackColor = true;
            this.cmdStopView.Click += new System.EventHandler(this.cmdStopView_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(518, 3);
            this.progressBar1.MarqueeAnimationSpeed = 10;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 31);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 8;
            // 
            // cmdControlValues
            // 
            this.cmdControlValues.Location = new System.Drawing.Point(624, 3);
            this.cmdControlValues.Name = "cmdControlValues";
            this.cmdControlValues.Size = new System.Drawing.Size(72, 31);
            this.cmdControlValues.TabIndex = 2;
            this.cmdControlValues.Text = "control";
            this.cmdControlValues.UseVisualStyleBackColor = true;
            this.cmdControlValues.Click += new System.EventHandler(this.cmdControlValues_Click);
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(702, 3);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(72, 31);
            this.cmdSave.TabIndex = 6;
            this.cmdSave.Text = "save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdLoad
            // 
            this.cmdLoad.Location = new System.Drawing.Point(780, 3);
            this.cmdLoad.Name = "cmdLoad";
            this.cmdLoad.Size = new System.Drawing.Size(72, 31);
            this.cmdLoad.TabIndex = 7;
            this.cmdLoad.Text = "load";
            this.cmdLoad.UseVisualStyleBackColor = true;
            this.cmdLoad.Click += new System.EventHandler(this.cmdLoad_Click);
            // 
            // cmdLoadVat
            // 
            this.cmdLoadVat.Location = new System.Drawing.Point(858, 3);
            this.cmdLoadVat.Name = "cmdLoadVat";
            this.cmdLoadVat.Size = new System.Drawing.Size(75, 31);
            this.cmdLoadVat.TabIndex = 9;
            this.cmdLoadVat.Text = "load vat";
            this.cmdLoadVat.UseVisualStyleBackColor = true;
            this.cmdLoadVat.Click += new System.EventHandler(this.cmdLoadVat_Click);
            // 
            // cmdLoadSymboltable
            // 
            this.cmdLoadSymboltable.Location = new System.Drawing.Point(939, 3);
            this.cmdLoadSymboltable.Name = "cmdLoadSymboltable";
            this.cmdLoadSymboltable.Size = new System.Drawing.Size(75, 31);
            this.cmdLoadSymboltable.TabIndex = 12;
            this.cmdLoadSymboltable.Text = "load symtab";
            this.cmdLoadSymboltable.UseVisualStyleBackColor = true;
            this.cmdLoadSymboltable.Click += new System.EventHandler(this.cmdLoadSymboltable_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.conninfo,
            this.errtxt});
            this.statusStrip1.Location = new System.Drawing.Point(0, 494);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1061, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.statusStrip1_ItemClicked);
            // 
            // conninfo
            // 
            this.conninfo.Name = "conninfo";
            this.conninfo.Size = new System.Drawing.Size(24, 17);
            this.conninfo.Text = "cnt";
            // 
            // errtxt
            // 
            this.errtxt.Name = "errtxt";
            this.errtxt.Size = new System.Drawing.Size(22, 17);
            this.errtxt.Text = "---";
            // 
            // Vartab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 516);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Vartab";
            this.Text = "JFK-VarTab";
            this.Load += new System.EventHandler(this.Vartab_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVarTab)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewVarTab;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button cmdConfigConnection;
        private System.Windows.Forms.Button cmdConnect;
        private System.Windows.Forms.Button cmdView;
        private System.Windows.Forms.Button cmdStopView;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdLoad;
        private System.Windows.Forms.Button cmdControlValues;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button cmdDisconnect;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel errtxt;
        private System.Windows.Forms.ToolStripStatusLabel conninfo;
        private System.Windows.Forms.Button cmdLoadVat;
        private System.Windows.Forms.ComboBox lstConnections;
        private System.Windows.Forms.Button cmdLoadSymboltable;
        private System.Windows.Forms.DataGridViewTextBoxColumn Addresse;
        private System.Windows.Forms.DataGridViewTextBoxColumn Typ;
        private System.Windows.Forms.DataGridViewTextBoxColumn DisplayTyp;
        private System.Windows.Forms.DataGridViewTextBoxColumn Wert;
        private System.Windows.Forms.DataGridViewTextBoxColumn Steuerwert;
        private System.Windows.Forms.DataGridViewTextBoxColumn symbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn comments;
    }
}