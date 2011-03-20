namespace TestProjectFileFunctions
{
    partial class DBStructresizer
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
            this.lstConnections = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtStartByte = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBytes = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtNewSize = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lblState = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Verbindung:";
            // 
            // lstConnections
            // 
            this.lstConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstConnections.FormattingEnabled = true;
            this.lstConnections.Location = new System.Drawing.Point(112, 6);
            this.lstConnections.Name = "lstConnections";
            this.lstConnections.Size = new System.Drawing.Size(135, 21);
            this.lstConnections.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "DB:";
            // 
            // txtDB
            // 
            this.txtDB.Location = new System.Drawing.Point(112, 36);
            this.txtDB.Name = "txtDB";
            this.txtDB.Size = new System.Drawing.Size(127, 20);
            this.txtDB.TabIndex = 2;
            this.txtDB.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Startadresse:";
            // 
            // txtStartByte
            // 
            this.txtStartByte.Location = new System.Drawing.Point(112, 62);
            this.txtStartByte.Name = "txtStartByte";
            this.txtStartByte.Size = new System.Drawing.Size(127, 20);
            this.txtStartByte.TabIndex = 2;
            this.txtStartByte.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Anzahl Bytes:";
            // 
            // txtBytes
            // 
            this.txtBytes.Location = new System.Drawing.Point(112, 88);
            this.txtBytes.Name = "txtBytes";
            this.txtBytes.Size = new System.Drawing.Size(127, 20);
            this.txtBytes.TabIndex = 2;
            this.txtBytes.Text = "20";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Struct größe:";
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(112, 114);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(127, 20);
            this.txtSize.TabIndex = 2;
            this.txtSize.Text = "10";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Struct Neue Größe";
            // 
            // txtNewSize
            // 
            this.txtNewSize.Location = new System.Drawing.Point(112, 140);
            this.txtNewSize.Name = "txtNewSize";
            this.txtNewSize.Size = new System.Drawing.Size(127, 20);
            this.txtNewSize.TabIndex = 2;
            this.txtNewSize.Text = "16";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 233);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(237, 36);
            this.button1.TabIndex = 3;
            this.button1.Text = "Lesen...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(15, 275);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(237, 36);
            this.button2.TabIndex = 3;
            this.button2.Text = "Schreiben...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblState
            // 
            this.lblState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblState.Location = new System.Drawing.Point(15, 174);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(237, 56);
            this.lblState.TabIndex = 4;
            // 
            // DBStructresizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 330);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtNewSize);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.txtBytes);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtStartByte);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lstConnections);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DBStructresizer";
            this.Text = "UDT-Array-Resizer";
            this.Load += new System.EventHandler(this.DBStructresizer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox lstConnections;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtStartByte;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBytes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNewSize;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblState;
    }
}