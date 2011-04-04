namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    partial class PLCTagEditor
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
            this.lblLen = new System.Windows.Forms.Label();
            this.txtByte = new System.Windows.Forms.TextBox();
            this.txtBit = new System.Windows.Forms.TextBox();
            this.txtLen = new System.Windows.Forms.TextBox();
            this.txtDB = new System.Windows.Forms.TextBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.cmbSource = new System.Windows.Forms.ComboBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblPT1 = new System.Windows.Forms.Label();
            this.txtValueInS7 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblPT2 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLen
            // 
            this.lblLen.AutoSize = true;
            this.lblLen.Location = new System.Drawing.Point(11, 178);
            this.lblLen.Name = "lblLen";
            this.lblLen.Size = new System.Drawing.Size(37, 13);
            this.lblLen.TabIndex = 20;
            this.lblLen.Text = "Länge";
            // 
            // txtByte
            // 
            this.txtByte.Location = new System.Drawing.Point(55, 3);
            this.txtByte.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.txtByte.Name = "txtByte";
            this.txtByte.Size = new System.Drawing.Size(56, 20);
            this.txtByte.TabIndex = 13;
            this.txtByte.Text = "0";
            this.txtByte.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtByte.TextChanged += new System.EventHandler(this.txtByte_TextChanged);
            // 
            // txtBit
            // 
            this.txtBit.Location = new System.Drawing.Point(121, 3);
            this.txtBit.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.txtBit.Name = "txtBit";
            this.txtBit.Size = new System.Drawing.Size(36, 20);
            this.txtBit.TabIndex = 14;
            this.txtBit.Text = "0";
            this.txtBit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtBit.TextChanged += new System.EventHandler(this.txtBit_TextChanged);
            // 
            // txtLen
            // 
            this.txtLen.Location = new System.Drawing.Point(15, 197);
            this.txtLen.Name = "txtLen";
            this.txtLen.Size = new System.Drawing.Size(61, 20);
            this.txtLen.TabIndex = 11;
            this.txtLen.Text = "1";
            this.txtLen.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtLen.TextChanged += new System.EventHandler(this.txtLen_TextChanged);
            // 
            // txtDB
            // 
            this.txtDB.Location = new System.Drawing.Point(0, 3);
            this.txtDB.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.txtDB.Name = "txtDB";
            this.txtDB.Size = new System.Drawing.Size(45, 20);
            this.txtDB.TabIndex = 12;
            this.txtDB.Text = "1";
            this.txtDB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDB.TextChanged += new System.EventHandler(this.txtDB_TextChanged);
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(15, 78);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(161, 21);
            this.cmbType.TabIndex = 16;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // cmbSource
            // 
            this.cmbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSource.FormattingEnabled = true;
            this.cmbSource.Location = new System.Drawing.Point(15, 29);
            this.cmbSource.Name = "cmbSource";
            this.cmbSource.Size = new System.Drawing.Size(121, 21);
            this.cmbSource.TabIndex = 15;
            this.cmbSource.SelectedIndexChanged += new System.EventHandler(this.cmbSource_SelectedIndexChanged);
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(246, 12);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(89, 38);
            this.cmdOK.TabIndex = 21;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(246, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 29);
            this.button1.TabIndex = 21;
            this.button1.Text = "Abbruch";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Datenquelle";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Adresse";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Datentyp";
            // 
            // lblPT1
            // 
            this.lblPT1.Location = new System.Drawing.Point(45, 0);
            this.lblPT1.Margin = new System.Windows.Forms.Padding(0);
            this.lblPT1.Name = "lblPT1";
            this.lblPT1.Size = new System.Drawing.Size(10, 20);
            this.lblPT1.TabIndex = 17;
            this.lblPT1.Text = ".";
            this.lblPT1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // txtValueInS7
            // 
            this.txtValueInS7.Location = new System.Drawing.Point(15, 253);
            this.txtValueInS7.Name = "txtValueInS7";
            this.txtValueInS7.Size = new System.Drawing.Size(166, 20);
            this.txtValueInS7.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 233);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "In S7 Schreibweise";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.txtDB);
            this.flowLayoutPanel1.Controls.Add(this.lblPT1);
            this.flowLayoutPanel1.Controls.Add(this.txtByte);
            this.flowLayoutPanel1.Controls.Add(this.lblPT2);
            this.flowLayoutPanel1.Controls.Add(this.txtBit);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(15, 140);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(191, 26);
            this.flowLayoutPanel1.TabIndex = 22;
            // 
            // lblPT2
            // 
            this.lblPT2.Location = new System.Drawing.Point(111, 0);
            this.lblPT2.Margin = new System.Windows.Forms.Padding(0);
            this.lblPT2.Name = "lblPT2";
            this.lblPT2.Size = new System.Drawing.Size(10, 20);
            this.lblPT2.TabIndex = 17;
            this.lblPT2.Text = ".";
            this.lblPT2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // PLCTagEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 285);
            this.ControlBox = false;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblLen);
            this.Controls.Add(this.txtLen);
            this.Controls.Add(this.txtValueInS7);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.cmbSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PLCTagEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LibNoDaveValueEditor";
            this.Load += new System.EventHandler(this.LibNoDaveValueEditor_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLen;
        private System.Windows.Forms.TextBox txtByte;
        private System.Windows.Forms.TextBox txtBit;
        private System.Windows.Forms.TextBox txtLen;
        private System.Windows.Forms.TextBox txtDB;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.ComboBox cmbSource;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblPT1;
        private System.Windows.Forms.TextBox txtValueInS7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblPT2;
    }
}