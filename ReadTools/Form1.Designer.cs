namespace ToolReader
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
            this.cmdConnect = new System.Windows.Forms.Button();
            this.cmdConfig = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnReadTools = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblActualParts = new System.Windows.Forms.Label();
            this.lblTotalParts = new System.Windows.Forms.Label();
            this.lblCounter = new System.Windows.Forms.Label();
            this.lblCycleTime = new System.Windows.Forms.Label();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolIdentNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.duploDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.edgesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.internalToolNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.depotDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.placeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lockedDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.lastRestDurabilityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restDurabilityTotalMinutesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maxTimeTotalMinutesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maxTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restDurabilityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblOldProgNetTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolDataBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdConnect
            // 
            this.cmdConnect.Location = new System.Drawing.Point(251, 12);
            this.cmdConnect.Name = "cmdConnect";
            this.cmdConnect.Size = new System.Drawing.Size(227, 27);
            this.cmdConnect.TabIndex = 2;
            this.cmdConnect.Text = "Connect";
            this.cmdConnect.UseVisualStyleBackColor = true;
            this.cmdConnect.Click += new System.EventHandler(this.cmdConnect_Click);
            // 
            // cmdConfig
            // 
            this.cmdConfig.Location = new System.Drawing.Point(12, 12);
            this.cmdConfig.Name = "cmdConfig";
            this.cmdConfig.Size = new System.Drawing.Size(227, 27);
            this.cmdConfig.TabIndex = 3;
            this.cmdConfig.Text = "Config";
            this.cmdConfig.UseVisualStyleBackColor = true;
            this.cmdConfig.Click += new System.EventHandler(this.cmdConfig_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.Location = new System.Drawing.Point(12, 42);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(466, 36);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnReadTools
            // 
            this.btnReadTools.Location = new System.Drawing.Point(12, 81);
            this.btnReadTools.Name = "btnReadTools";
            this.btnReadTools.Size = new System.Drawing.Size(466, 27);
            this.btnReadTools.TabIndex = 5;
            this.btnReadTools.Text = "Read";
            this.btnReadTools.UseVisualStyleBackColor = true;
            this.btnReadTools.Click += new System.EventHandler(this.btnReadTools_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.toolIdentNumberDataGridViewTextBoxColumn,
            this.duploDataGridViewTextBoxColumn,
            this.edgesDataGridViewTextBoxColumn,
            this.internalToolNumberDataGridViewTextBoxColumn,
            this.depotDataGridViewTextBoxColumn,
            this.placeDataGridViewTextBoxColumn,
            this.lockedDataGridViewCheckBoxColumn,
            this.lastRestDurabilityDataGridViewTextBoxColumn,
            this.restDurabilityTotalMinutesDataGridViewTextBoxColumn,
            this.maxTimeTotalMinutesDataGridViewTextBoxColumn,
            this.statusDataGridViewTextBoxColumn,
            this.maxTimeDataGridViewTextBoxColumn,
            this.currTimeDataGridViewTextBoxColumn,
            this.restDurabilityDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.toolDataBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(12, 126);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(1206, 330);
            this.dataGridView1.TabIndex = 6;
            // 
            // lblActualParts
            // 
            this.lblActualParts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblActualParts.Location = new System.Drawing.Point(641, 7);
            this.lblActualParts.Name = "lblActualParts";
            this.lblActualParts.Size = new System.Drawing.Size(180, 36);
            this.lblActualParts.TabIndex = 7;
            this.lblActualParts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalParts
            // 
            this.lblTotalParts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalParts.Location = new System.Drawing.Point(641, 72);
            this.lblTotalParts.Name = "lblTotalParts";
            this.lblTotalParts.Size = new System.Drawing.Size(180, 36);
            this.lblTotalParts.TabIndex = 8;
            this.lblTotalParts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCounter
            // 
            this.lblCounter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCounter.Location = new System.Drawing.Point(858, 7);
            this.lblCounter.Name = "lblCounter";
            this.lblCounter.Size = new System.Drawing.Size(180, 36);
            this.lblCounter.TabIndex = 9;
            this.lblCounter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCycleTime
            // 
            this.lblCycleTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCycleTime.Location = new System.Drawing.Point(858, 72);
            this.lblCycleTime.Name = "lblCycleTime";
            this.lblCycleTime.Size = new System.Drawing.Size(180, 36);
            this.lblCycleTime.TabIndex = 10;
            this.lblCycleTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // toolIdentNumberDataGridViewTextBoxColumn
            // 
            this.toolIdentNumberDataGridViewTextBoxColumn.DataPropertyName = "ToolIdentNumber";
            this.toolIdentNumberDataGridViewTextBoxColumn.HeaderText = "ToolIdentNumber";
            this.toolIdentNumberDataGridViewTextBoxColumn.Name = "toolIdentNumberDataGridViewTextBoxColumn";
            this.toolIdentNumberDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // duploDataGridViewTextBoxColumn
            // 
            this.duploDataGridViewTextBoxColumn.DataPropertyName = "Duplo";
            this.duploDataGridViewTextBoxColumn.HeaderText = "Duplo";
            this.duploDataGridViewTextBoxColumn.Name = "duploDataGridViewTextBoxColumn";
            this.duploDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // edgesDataGridViewTextBoxColumn
            // 
            this.edgesDataGridViewTextBoxColumn.DataPropertyName = "Edges";
            this.edgesDataGridViewTextBoxColumn.HeaderText = "Edges";
            this.edgesDataGridViewTextBoxColumn.Name = "edgesDataGridViewTextBoxColumn";
            this.edgesDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // internalToolNumberDataGridViewTextBoxColumn
            // 
            this.internalToolNumberDataGridViewTextBoxColumn.DataPropertyName = "InternalToolNumber";
            this.internalToolNumberDataGridViewTextBoxColumn.HeaderText = "InternalToolNumber";
            this.internalToolNumberDataGridViewTextBoxColumn.Name = "internalToolNumberDataGridViewTextBoxColumn";
            this.internalToolNumberDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // depotDataGridViewTextBoxColumn
            // 
            this.depotDataGridViewTextBoxColumn.DataPropertyName = "Depot";
            this.depotDataGridViewTextBoxColumn.HeaderText = "Depot";
            this.depotDataGridViewTextBoxColumn.Name = "depotDataGridViewTextBoxColumn";
            this.depotDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // placeDataGridViewTextBoxColumn
            // 
            this.placeDataGridViewTextBoxColumn.DataPropertyName = "Place";
            this.placeDataGridViewTextBoxColumn.HeaderText = "Place";
            this.placeDataGridViewTextBoxColumn.Name = "placeDataGridViewTextBoxColumn";
            this.placeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // lockedDataGridViewCheckBoxColumn
            // 
            this.lockedDataGridViewCheckBoxColumn.DataPropertyName = "Locked";
            this.lockedDataGridViewCheckBoxColumn.HeaderText = "Locked";
            this.lockedDataGridViewCheckBoxColumn.Name = "lockedDataGridViewCheckBoxColumn";
            this.lockedDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // lastRestDurabilityDataGridViewTextBoxColumn
            // 
            this.lastRestDurabilityDataGridViewTextBoxColumn.DataPropertyName = "LastRestDurability";
            this.lastRestDurabilityDataGridViewTextBoxColumn.HeaderText = "LastRestDurability";
            this.lastRestDurabilityDataGridViewTextBoxColumn.Name = "lastRestDurabilityDataGridViewTextBoxColumn";
            this.lastRestDurabilityDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // restDurabilityTotalMinutesDataGridViewTextBoxColumn
            // 
            this.restDurabilityTotalMinutesDataGridViewTextBoxColumn.DataPropertyName = "RestDurabilityTotalMinutes";
            this.restDurabilityTotalMinutesDataGridViewTextBoxColumn.HeaderText = "RestDurabilityTotalMinutes";
            this.restDurabilityTotalMinutesDataGridViewTextBoxColumn.Name = "restDurabilityTotalMinutesDataGridViewTextBoxColumn";
            this.restDurabilityTotalMinutesDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // maxTimeTotalMinutesDataGridViewTextBoxColumn
            // 
            this.maxTimeTotalMinutesDataGridViewTextBoxColumn.DataPropertyName = "MaxTimeTotalMinutes";
            this.maxTimeTotalMinutesDataGridViewTextBoxColumn.HeaderText = "MaxTimeTotalMinutes";
            this.maxTimeTotalMinutesDataGridViewTextBoxColumn.Name = "maxTimeTotalMinutesDataGridViewTextBoxColumn";
            this.maxTimeTotalMinutesDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // maxTimeDataGridViewTextBoxColumn
            // 
            this.maxTimeDataGridViewTextBoxColumn.DataPropertyName = "MaxTime";
            this.maxTimeDataGridViewTextBoxColumn.HeaderText = "MaxTime";
            this.maxTimeDataGridViewTextBoxColumn.Name = "maxTimeDataGridViewTextBoxColumn";
            this.maxTimeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // currTimeDataGridViewTextBoxColumn
            // 
            this.currTimeDataGridViewTextBoxColumn.DataPropertyName = "CurrTime";
            this.currTimeDataGridViewTextBoxColumn.HeaderText = "CurrTime";
            this.currTimeDataGridViewTextBoxColumn.Name = "currTimeDataGridViewTextBoxColumn";
            this.currTimeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // restDurabilityDataGridViewTextBoxColumn
            // 
            this.restDurabilityDataGridViewTextBoxColumn.DataPropertyName = "RestDurability";
            this.restDurabilityDataGridViewTextBoxColumn.HeaderText = "RestDurability";
            this.restDurabilityDataGridViewTextBoxColumn.Name = "restDurabilityDataGridViewTextBoxColumn";
            this.restDurabilityDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // toolDataBindingSource
            // 
            this.toolDataBindingSource.DataSource = typeof(ToolReader.ToolData);
            // 
            // lblOldProgNetTime
            // 
            this.lblOldProgNetTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOldProgNetTime.Location = new System.Drawing.Point(1044, 72);
            this.lblOldProgNetTime.Name = "lblOldProgNetTime";
            this.lblOldProgNetTime.Size = new System.Drawing.Size(180, 36);
            this.lblOldProgNetTime.TabIndex = 11;
            this.lblOldProgNetTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1220, 523);
            this.Controls.Add(this.lblOldProgNetTime);
            this.Controls.Add(this.lblCycleTime);
            this.Controls.Add(this.lblCounter);
            this.Controls.Add(this.lblTotalParts);
            this.Controls.Add(this.lblActualParts);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnReadTools);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.cmdConnect);
            this.Controls.Add(this.cmdConfig);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolDataBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdConnect;
        private System.Windows.Forms.Button cmdConfig;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnReadTools;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn machineIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn newAddedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn toolIdentNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn duploDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn edgesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn alternativeToolIdentNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn internalToolNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn depotDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn placeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn lockedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastRestDurabilityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn restDurabilityTotalMinutesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxTimeTotalMinutesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn currTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn restDurabilityDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource toolDataBindingSource;
        private System.Windows.Forms.Label lblActualParts;
        private System.Windows.Forms.Label lblTotalParts;
        private System.Windows.Forms.Label lblCounter;
        private System.Windows.Forms.Label lblCycleTime;
        private System.Windows.Forms.Label lblOldProgNetTime;
    }
}

