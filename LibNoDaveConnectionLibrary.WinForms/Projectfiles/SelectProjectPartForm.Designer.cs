namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    partial class SelectProjectPartForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectProjectPartForm));
            this.cmdOpenProject = new System.Windows.Forms.Button();
            this.lblProjectInfo = new System.Windows.Forms.Label();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.treeStep7Project = new System.Windows.Forms.TreeView();
            this.imglstIconsForTreeview = new System.Windows.Forms.ImageList(this.components);
            this.chkShowDeleted = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.lstProjectFolder = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // cmdOpenProject
            // 
            this.cmdOpenProject.Location = new System.Drawing.Point(21, 80);
            this.cmdOpenProject.Name = "cmdOpenProject";
            this.cmdOpenProject.Size = new System.Drawing.Size(90, 41);
            this.cmdOpenProject.TabIndex = 2;
            this.cmdOpenProject.Text = "...";
            this.cmdOpenProject.UseVisualStyleBackColor = true;
            this.cmdOpenProject.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblProjectInfo
            // 
            this.lblProjectInfo.Location = new System.Drawing.Point(114, 35);
            this.lblProjectInfo.Name = "lblProjectInfo";
            this.lblProjectInfo.Size = new System.Drawing.Size(310, 15);
            this.lblProjectInfo.TabIndex = 3;
            // 
            // lblProjectName
            // 
            this.lblProjectName.Location = new System.Drawing.Point(114, 8);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(310, 15);
            this.lblProjectName.TabIndex = 3;
            // 
            // treeStep7Project
            // 
            this.treeStep7Project.ImageIndex = 0;
            this.treeStep7Project.ImageList = this.imglstIconsForTreeview;
            this.treeStep7Project.Location = new System.Drawing.Point(21, 127);
            this.treeStep7Project.Name = "treeStep7Project";
            this.treeStep7Project.SelectedImageIndex = 0;
            this.treeStep7Project.Size = new System.Drawing.Size(403, 364);
            this.treeStep7Project.TabIndex = 4;
            this.treeStep7Project.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeStep7Project_BeforeCollapse);
            this.treeStep7Project.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeStep7Project_BeforeExpand);
            this.treeStep7Project.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeStep7Project.DoubleClick += new System.EventHandler(this.treeStep7Project_DoubleClick);
            // 
            // imglstIconsForTreeview
            // 
            this.imglstIconsForTreeview.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglstIconsForTreeview.ImageStream")));
            this.imglstIconsForTreeview.TransparentColor = System.Drawing.Color.Transparent;
            this.imglstIconsForTreeview.Images.SetKeyName(0, "Generic Folder   Closed.ico");
            this.imglstIconsForTreeview.Images.SetKeyName(1, "Generic Folder   Open.ico");
            this.imglstIconsForTreeview.Images.SetKeyName(2, "300er.png");
            this.imglstIconsForTreeview.Images.SetKeyName(3, "400er.png");
            this.imglstIconsForTreeview.Images.SetKeyName(4, "Rack300.png");
            this.imglstIconsForTreeview.Images.SetKeyName(5, "rack400.png");
            this.imglstIconsForTreeview.Images.SetKeyName(6, "Folder.png");
            this.imglstIconsForTreeview.Images.SetKeyName(7, "im151.png");
            this.imglstIconsForTreeview.Images.SetKeyName(8, "im151cpu.png");
            this.imglstIconsForTreeview.Images.SetKeyName(9, "cp.png");
            this.imglstIconsForTreeview.Images.SetKeyName(10, "s5.png");
            // 
            // chkShowDeleted
            // 
            this.chkShowDeleted.AutoSize = true;
            this.chkShowDeleted.Location = new System.Drawing.Point(21, 497);
            this.chkShowDeleted.Name = "chkShowDeleted";
            this.chkShowDeleted.Size = new System.Drawing.Size(110, 19);
            this.chkShowDeleted.TabIndex = 8;
            this.chkShowDeleted.Text = "Show deleted";
            this.chkShowDeleted.UseVisualStyleBackColor = true;
            this.chkShowDeleted.Visible = false;
            this.chkShowDeleted.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Project Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "Project Info:";
            // 
            // cmdOk
            // 
            this.cmdOk.Location = new System.Drawing.Point(794, 8);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(62, 32);
            this.cmdOk.TabIndex = 9;
            this.cmdOk.Text = "OK";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(794, 46);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(62, 32);
            this.cmdCancel.TabIndex = 10;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // lstProjectFolder
            // 
            this.lstProjectFolder.FormattingEnabled = true;
            this.lstProjectFolder.ItemHeight = 15;
            this.lstProjectFolder.Location = new System.Drawing.Point(438, 127);
            this.lstProjectFolder.Name = "lstProjectFolder";
            this.lstProjectFolder.Size = new System.Drawing.Size(418, 364);
            this.lstProjectFolder.TabIndex = 11;
            this.lstProjectFolder.DoubleClick += new System.EventHandler(this.lstProjectFolder_DoubleClick);
            // 
            // SelectProjectPartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 532);
            this.Controls.Add(this.lstProjectFolder);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.Controls.Add(this.chkShowDeleted);
            this.Controls.Add(this.treeStep7Project);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblProjectInfo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblProjectName);
            this.Controls.Add(this.cmdOpenProject);
            this.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SelectProjectPartForm";
            this.Text = "Select Project/Block";
            this.Load += new System.EventHandler(this.SelectProjectPartForm_Load);
            this.Shown += new System.EventHandler(this.SelectProjectPartForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOpenProject;
        private System.Windows.Forms.Label lblProjectInfo;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.TreeView treeStep7Project;
        private System.Windows.Forms.CheckBox chkShowDeleted;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.ListBox lstProjectFolder;
        private System.Windows.Forms.ImageList imglstIconsForTreeview;

    }
}

