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
            this.lblBit = new System.Windows.Forms.Label();
            this.lblByte = new System.Windows.Forms.Label();
            this.lblLen = new System.Windows.Forms.Label();
            this.lblDB = new System.Windows.Forms.Label();
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
            this.lblPT2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblPT1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblBit
            // 
            this.lblBit.AutoSize = true;
            this.lblBit.Location = new System.Drawing.Point(126, 129);
            this.lblBit.Name = "lblBit";
            this.lblBit.Size = new System.Drawing.Size(19, 13);
            this.lblBit.TabIndex = 17;
            this.lblBit.Text = "Bit";
            // 
            // lblByte
            // 
            this.lblByte.AutoSize = true;
            this.lblByte.Location = new System.Drawing.Point(65, 129);
            this.lblByte.Name = "lblByte";
            this.lblByte.Size = new System.Drawing.Size(28, 13);
            this.lblByte.TabIndex = 18;
            this.lblByte.Text = "Byte";
            // 
            // lblLen
            // 
            this.lblLen.AutoSize = true;
            this.lblLen.Location = new System.Drawing.Point(17, 181);
            this.lblLen.Name = "lblLen";
            this.lblLen.Size = new System.Drawing.Size(37, 13);
            this.lblLen.TabIndex = 20;
            this.lblLen.Text = "Länge";
            // 
            // lblDB
            // 
            this.lblDB.AutoSize = true;
            this.lblDB.Location = new System.Drawing.Point(14, 128);
            this.lblDB.Name = "lblDB";
            this.lblDB.Size = new System.Drawing.Size(22, 13);
            this.lblDB.TabIndex = 19;
            this.lblDB.Text = "DB";
            // 
            // txtByte
            // 
            this.txtByte.Location = new System.Drawing.Point(68, 145);
            this.txtByte.Name = "txtByte";
            this.txtByte.Size = new System.Drawing.Size(56, 20);
            this.txtByte.TabIndex = 13;
            this.txtByte.Text = "0";
            // 
            // txtBit
            // 
            this.txtBit.Location = new System.Drawing.Point(129, 145);
            this.txtBit.Name = "txtBit";
            this.txtBit.Size = new System.Drawing.Size(36, 20);
            this.txtBit.TabIndex = 14;
            this.txtBit.Text = "0";
            // 
            // txtLen
            // 
            this.txtLen.Location = new System.Drawing.Point(17, 197);
            this.txtLen.Name = "txtLen";
            this.txtLen.Size = new System.Drawing.Size(61, 20);
            this.txtLen.TabIndex = 11;
            this.txtLen.Text = "1";
            // 
            // txtDB
            // 
            this.txtDB.Location = new System.Drawing.Point(18, 145);
            this.txtDB.Name = "txtDB";
            this.txtDB.Size = new System.Drawing.Size(45, 20);
            this.txtDB.TabIndex = 12;
            this.txtDB.Text = "1";
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(20, 74);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(161, 21);
            this.cmbType.TabIndex = 16;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // cmbSource
            // 
            this.cmbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSource.FormattingEnabled = true;
            this.cmbSource.Location = new System.Drawing.Point(20, 30);
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
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Datenquelle";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Adresse";
            // 
            // lblPT2
            // 
            this.lblPT2.AutoSize = true;
            this.lblPT2.Location = new System.Drawing.Point(122, 152);
            this.lblPT2.Name = "lblPT2";
            this.lblPT2.Size = new System.Drawing.Size(10, 13);
            this.lblPT2.TabIndex = 17;
            this.lblPT2.Text = ".";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Datentyp";
            // 
            // lblPT1
            // 
            this.lblPT1.AutoSize = true;
            this.lblPT1.Location = new System.Drawing.Point(61, 152);
            this.lblPT1.Name = "lblPT1";
            this.lblPT1.Size = new System.Drawing.Size(10, 13);
            this.lblPT1.TabIndex = 17;
            this.lblPT1.Text = ".";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 253);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(166, 20);
            this.textBox1.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 237);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "In S7 Schreibweise";
            // 
            // PLCTagEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 285);
            this.ControlBox = false;
            this.Controls.Add(this.txtByte);
            this.Controls.Add(this.txtBit);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.lblPT2);
            this.Controls.Add(this.lblBit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblByte);
            this.Controls.Add(this.lblLen);
            this.Controls.Add(this.lblDB);
            this.Controls.Add(this.txtLen);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.txtDB);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.cmbSource);
            this.Controls.Add(this.lblPT1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PLCTagEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LibNoDaveValueEditor";
            this.Load += new System.EventHandler(this.LibNoDaveValueEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBit;
        private System.Windows.Forms.Label lblByte;
        private System.Windows.Forms.Label lblLen;
        private System.Windows.Forms.Label lblDB;
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
        private System.Windows.Forms.Label lblPT2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblPT1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
    }
}