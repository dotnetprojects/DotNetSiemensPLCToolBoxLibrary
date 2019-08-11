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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PLCTagEditor));
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
            resources.ApplyResources(this.lblLen, "lblLen");
            this.lblLen.Name = "lblLen";
            // 
            // txtByte
            // 
            resources.ApplyResources(this.txtByte, "txtByte");
            this.txtByte.Name = "txtByte";
            this.txtByte.TextChanged += new System.EventHandler(this.txtByte_TextChanged);
            // 
            // txtBit
            // 
            resources.ApplyResources(this.txtBit, "txtBit");
            this.txtBit.Name = "txtBit";
            this.txtBit.TextChanged += new System.EventHandler(this.txtBit_TextChanged);
            // 
            // txtLen
            // 
            resources.ApplyResources(this.txtLen, "txtLen");
            this.txtLen.Name = "txtLen";
            this.txtLen.TextChanged += new System.EventHandler(this.txtLen_TextChanged);
            // 
            // txtDB
            // 
            resources.ApplyResources(this.txtDB, "txtDB");
            this.txtDB.Name = "txtDB";
            this.txtDB.TextChanged += new System.EventHandler(this.txtDB_TextChanged);
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            resources.ApplyResources(this.cmbType, "cmbType");
            this.cmbType.Name = "cmbType";
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // cmbSource
            // 
            this.cmbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSource.FormattingEnabled = true;
            resources.ApplyResources(this.cmbSource, "cmbSource");
            this.cmbSource.Name = "cmbSource";
            this.cmbSource.SelectedIndexChanged += new System.EventHandler(this.cmbSource_SelectedIndexChanged);
            // 
            // cmdOK
            // 
            resources.ApplyResources(this.cmdOK, "cmdOK");
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // lblPT1
            // 
            resources.ApplyResources(this.lblPT1, "lblPT1");
            this.lblPT1.Name = "lblPT1";
            // 
            // txtValueInS7
            // 
            resources.ApplyResources(this.txtValueInS7, "txtValueInS7");
            this.txtValueInS7.Name = "txtValueInS7";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.txtDB);
            this.flowLayoutPanel1.Controls.Add(this.lblPT1);
            this.flowLayoutPanel1.Controls.Add(this.txtByte);
            this.flowLayoutPanel1.Controls.Add(this.lblPT2);
            this.flowLayoutPanel1.Controls.Add(this.txtBit);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // lblPT2
            // 
            resources.ApplyResources(this.lblPT2, "lblPT2");
            this.lblPT2.Name = "lblPT2";
            // 
            // PLCTagEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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