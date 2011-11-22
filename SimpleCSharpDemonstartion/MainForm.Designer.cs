namespace SimpleCSharpDemonstration
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
            this.components = new System.ComponentModel.Container();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.lblString = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.cmdReadStruct = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(157, 40);
            this.button2.TabIndex = 3;
            this.button2.Text = "ReadString\r\nTimer Starten";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(249, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(157, 40);
            this.button1.TabIndex = 2;
            this.button1.Text = "Config Connection";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(239, 39);
            this.label1.TabIndex = 4;
            this.label1.Text = "Dieses BeispielPRg liest einen 10 Zeichen String \r\nvon DB1.DBX0.0 (also 12 Bytes)" +
    "\r\nund zeigt Ihn an!";
            // 
            // lblString
            // 
            this.lblString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblString.Location = new System.Drawing.Point(94, 160);
            this.lblString.Name = "lblString";
            this.lblString.Size = new System.Drawing.Size(185, 30);
            this.lblString.TabIndex = 5;
            this.lblString.Text = "String";
            this.lblString.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(249, 219);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(157, 40);
            this.button3.TabIndex = 2;
            this.button3.Text = "Select Symbol Table";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // cmdReadStruct
            // 
            this.cmdReadStruct.Location = new System.Drawing.Point(12, 219);
            this.cmdReadStruct.Name = "cmdReadStruct";
            this.cmdReadStruct.Size = new System.Drawing.Size(157, 40);
            this.cmdReadStruct.TabIndex = 6;
            this.cmdReadStruct.Text = "Read Struct";
            this.cmdReadStruct.UseVisualStyleBackColor = true;
            this.cmdReadStruct.Click += new System.EventHandler(this.cmdReadStruct_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(298, 122);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(108, 35);
            this.button4.TabIndex = 7;
            this.button4.Text = "Read";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(249, 58);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(157, 40);
            this.button5.TabIndex = 8;
            this.button5.Text = "Config Connection via Code";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 262);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.cmdReadStruct);
            this.Controls.Add(this.lblString);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblString;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button cmdReadStruct;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}