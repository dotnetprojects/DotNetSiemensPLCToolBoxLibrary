namespace ReadSpeedtest
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
            this.txtTag = new System.Windows.Forms.TextBox();
            this.cmdConfig = new System.Windows.Forms.Button();
            this.cmdConnect = new System.Windows.Forms.Button();
            this.cmdReadWithReadValue = new System.Windows.Forms.Button();
            this.cmdReadWithReadValues = new System.Windows.Forms.Button();
            this.cmdReadWithLibnodave = new System.Windows.Forms.Button();
            this.txtWert = new System.Windows.Forms.TextBox();
            this.txtZeit = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdReadValuesWithvarTab = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtTag
            // 
            this.txtTag.Location = new System.Drawing.Point(12, 117);
            this.txtTag.Name = "txtTag";
            this.txtTag.Size = new System.Drawing.Size(227, 20);
            this.txtTag.TabIndex = 0;
            this.txtTag.Text = "P#DB99.DBX0.0 BYTE 700";
            // 
            // cmdConfig
            // 
            this.cmdConfig.Location = new System.Drawing.Point(12, 12);
            this.cmdConfig.Name = "cmdConfig";
            this.cmdConfig.Size = new System.Drawing.Size(227, 27);
            this.cmdConfig.TabIndex = 1;
            this.cmdConfig.Text = "Config";
            this.toolTip1.SetToolTip(this.cmdConfig, "Click here to Config the Connection");
            this.cmdConfig.UseVisualStyleBackColor = true;
            this.cmdConfig.Click += new System.EventHandler(this.cmdConfig_Click);
            // 
            // cmdConnect
            // 
            this.cmdConnect.Location = new System.Drawing.Point(12, 45);
            this.cmdConnect.Name = "cmdConnect";
            this.cmdConnect.Size = new System.Drawing.Size(227, 27);
            this.cmdConnect.TabIndex = 1;
            this.cmdConnect.Text = "Connect";
            this.toolTip1.SetToolTip(this.cmdConnect, "Click here to Connect to the PLC");
            this.cmdConnect.UseVisualStyleBackColor = true;
            this.cmdConnect.Click += new System.EventHandler(this.cmdConnect_Click);
            // 
            // cmdReadWithReadValue
            // 
            this.cmdReadWithReadValue.Location = new System.Drawing.Point(12, 143);
            this.cmdReadWithReadValue.Name = "cmdReadWithReadValue";
            this.cmdReadWithReadValue.Size = new System.Drawing.Size(227, 27);
            this.cmdReadWithReadValue.TabIndex = 1;
            this.cmdReadWithReadValue.Text = "Read with ReadValue";
            this.toolTip1.SetToolTip(this.cmdReadWithReadValue, "Click here to start reading the TAG with the ReadValue Fuction from the Toolbox L" +
                    "ib (uses readmanybytes from libnodave)");
            this.cmdReadWithReadValue.UseVisualStyleBackColor = true;
            this.cmdReadWithReadValue.Click += new System.EventHandler(this.cmdReadWithReadValue_Click);
            // 
            // cmdReadWithReadValues
            // 
            this.cmdReadWithReadValues.Location = new System.Drawing.Point(12, 176);
            this.cmdReadWithReadValues.Name = "cmdReadWithReadValues";
            this.cmdReadWithReadValues.Size = new System.Drawing.Size(227, 27);
            this.cmdReadWithReadValues.TabIndex = 1;
            this.cmdReadWithReadValues.Text = "Read with ReadValues";
            this.toolTip1.SetToolTip(this.cmdReadWithReadValues, "Click here to start reading the TAG with the ReadValues Fuction from the Toolbox " +
                    "Lib (uses PrepareReadRequest, AddtoReadRequest,...)");
            this.cmdReadWithReadValues.UseVisualStyleBackColor = true;
            this.cmdReadWithReadValues.Click += new System.EventHandler(this.cmdReadWithReadValues_Click);
            // 
            // cmdReadWithLibnodave
            // 
            this.cmdReadWithLibnodave.Location = new System.Drawing.Point(12, 242);
            this.cmdReadWithLibnodave.Name = "cmdReadWithLibnodave";
            this.cmdReadWithLibnodave.Size = new System.Drawing.Size(227, 27);
            this.cmdReadWithLibnodave.TabIndex = 1;
            this.cmdReadWithLibnodave.Text = "Read with libnodave";
            this.toolTip1.SetToolTip(this.cmdReadWithLibnodave, "Click here to start reading using readmanybytes directly");
            this.cmdReadWithLibnodave.UseVisualStyleBackColor = true;
            this.cmdReadWithLibnodave.Click += new System.EventHandler(this.cmdReadWithLibnodave_Click);
            // 
            // txtWert
            // 
            this.txtWert.Location = new System.Drawing.Point(12, 285);
            this.txtWert.Multiline = true;
            this.txtWert.Name = "txtWert";
            this.txtWert.ReadOnly = true;
            this.txtWert.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWert.Size = new System.Drawing.Size(227, 75);
            this.txtWert.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtWert, "The Value wich was Read");
            // 
            // txtZeit
            // 
            this.txtZeit.Location = new System.Drawing.Point(12, 366);
            this.txtZeit.Name = "txtZeit";
            this.txtZeit.ReadOnly = true;
            this.txtZeit.Size = new System.Drawing.Size(227, 20);
            this.txtZeit.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtZeit, "The time the last read has took");
            // 
            // lblStatus
            // 
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.Location = new System.Drawing.Point(12, 78);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(227, 36);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmdReadValuesWithvarTab
            // 
            this.cmdReadValuesWithvarTab.Location = new System.Drawing.Point(12, 209);
            this.cmdReadValuesWithvarTab.Name = "cmdReadValuesWithvarTab";
            this.cmdReadValuesWithvarTab.Size = new System.Drawing.Size(227, 27);
            this.cmdReadValuesWithvarTab.TabIndex = 1;
            this.cmdReadValuesWithvarTab.Text = "Read with ReadValuesWithVarTabFunctions";
            this.cmdReadValuesWithvarTab.UseVisualStyleBackColor = true;
            this.cmdReadValuesWithvarTab.Click += new System.EventHandler(this.cmdReadValuesWithvarTab_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 392);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtWert);
            this.Controls.Add(this.cmdReadWithLibnodave);
            this.Controls.Add(this.cmdReadValuesWithvarTab);
            this.Controls.Add(this.cmdReadWithReadValues);
            this.Controls.Add(this.cmdReadWithReadValue);
            this.Controls.Add(this.cmdConnect);
            this.Controls.Add(this.cmdConfig);
            this.Controls.Add(this.txtZeit);
            this.Controls.Add(this.txtTag);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "ReadSpeedTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTag;
        private System.Windows.Forms.Button cmdConfig;
        private System.Windows.Forms.Button cmdConnect;
        private System.Windows.Forms.Button cmdReadWithReadValue;
        private System.Windows.Forms.Button cmdReadWithReadValues;
        private System.Windows.Forms.Button cmdReadWithLibnodave;
        private System.Windows.Forms.TextBox txtWert;
        private System.Windows.Forms.TextBox txtZeit;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button cmdReadValuesWithvarTab;
    }
}

