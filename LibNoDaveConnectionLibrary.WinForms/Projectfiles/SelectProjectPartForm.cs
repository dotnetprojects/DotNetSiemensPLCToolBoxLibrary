using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    internal partial class SelectProjectPartForm : Form
    {
        public SelectProjectPartForm(string projectFile, bool hideOpenProjectButton = true)
        {
            InitializeComponent();

            fnm = projectFile;
            if (!string.IsNullOrEmpty(fnm))
            {
                if (hideOpenProjectButton)
                    cmdOpenProject.Visible = false;
                loadPrj();
            }
        }

        Project tmpPrj;


        #region TreeNodes

        class myTreeNode : TreeNode
        {
            public object myObject;
        }

        class DBValueTreeNode : TreeNode
        {
            public S7DataRow s7datarow;
        }

        class DBTreeNode : DBValueTreeNode
        {
            public ProjectPlcBlockInfo PLCBlockInfo;
        }

        class FakeNode : TreeNode
        { }

        #endregion

        private void AddDBValueNodes(DBTreeNode nd)
        {
            if (nd.Nodes.Count > 0 && nd.Nodes[0] is FakeNode)
            {
                nd.Nodes.RemoveAt(0);
                S7DataBlock blk = (S7DataBlock)nd.PLCBlockInfo.GetBlock();
                nd.s7datarow = (S7DataRow)blk.Structure;
                AddDBValueSubNodes(nd, (S7DataRow)blk.Structure);
            }
        }

        private void AddDBValueSubNodes(TreeNode nd, S7DataRow row)
        {
            if (row.Children != null)
                foreach (S7DataRow s7DataRow in row.Children)
                {
                    if (s7DataRow.Children.Count > 0)
                    {
                        DBValueTreeNode newNd = new DBValueTreeNode();
                        newNd.Text = s7DataRow.Name;
                        newNd.s7datarow = s7DataRow;
                        nd.Nodes.Add(newNd);
                        AddDBValueSubNodes(newNd, s7DataRow);
                    }
                }
        }

        private void AddNodes(TreeNode nd, List<ProjectFolder> lst)
        {
            foreach (var subitem in lst)
            {
                myTreeNode tmpNode = new myTreeNode();
                tmpNode.Text = subitem.Name;
                tmpNode.myObject = subitem;
                tmpNode.ImageIndex = 0;
                //nd.ImageKey
                //Set the Image for the Folders...
                if (subitem.GetType() == typeof(StationConfigurationFolder))
                {
                    if (((StationConfigurationFolder)subitem).StationType == PLCType.Simatic300)
                        tmpNode.ImageIndex = 4;
                    else if (((StationConfigurationFolder)subitem).StationType == PLCType.Simatic400 || ((StationConfigurationFolder)subitem).StationType == PLCType.Simatic400H)
                        tmpNode.ImageIndex = 5;
                }
                else if (subitem.GetType() == typeof(CPUFolder))
                {
                    if (((CPUFolder)subitem).CpuType == PLCType.Simatic300)
                        tmpNode.ImageIndex = 2;
                    else if (((CPUFolder)subitem).CpuType == PLCType.Simatic400 || ((CPUFolder)subitem).CpuType == PLCType.Simatic400H)
                        tmpNode.ImageIndex = 3;
                }

                nd.Nodes.Add(tmpNode);

                if (subitem.SubItems != null)
                    AddNodes(tmpNode, subitem.SubItems);

                if (subitem is IBlocksFolder && this.SelectPart == SelectPartType.Tag)
                {
                    IBlocksFolder blkFld = (IBlocksFolder)subitem;
                    foreach (ProjectPlcBlockInfo projectBlockInfo in blkFld.readPlcBlocksList())
                    {
                        if (projectBlockInfo.BlockType == PLCBlockType.DB || projectBlockInfo.BlockType == PLCBlockType.S5_DB || projectBlockInfo.BlockType == PLCBlockType.S5_DX)
                        {
                            string nm = projectBlockInfo.BlockName;
                            if (projectBlockInfo.SymbolTabelEntry != null)
                                nm += " (" + projectBlockInfo.SymbolTabelEntry.Symbol + ")";
                            DBTreeNode trnd = new DBTreeNode() { Text = nm, PLCBlockInfo = projectBlockInfo };
                            trnd.Nodes.Add(new FakeNode());
                            tmpNode.Nodes.Add(trnd);
                        }
                    }

                }
            }
        }

        private string fnm = "";

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "All supported types (*.zip, *.s7p, *.s5d, *.ap11, *.ap12, *.ap13, *.ap14, *.al11, *.al12, *.al13, *.al14)|*.s7p;*.zip;*.s5d;*.s7l;*.ap11;*.ap12;*.ap13;*.ap14;*.al11;*.al12;*.al13;*.al14|Step5 Project|*.s5d|Step7 V5.5 Project|*.s7p;*.s7l|Zipped Step5/Step7 Project|*.zip|TIA-Portal Project|*.ap11;*.ap12;*.ap13;*.ap14;*.al11;*.al12;*.al13;*.al14";
            var ret = op.ShowDialog();
            if (ret == DialogResult.OK)
            {
                fnm = op.FileName;
                loadPrj();
            }
        }

        void loadPrj()
        {
            if (fnm != "")
            {
                treeStep7Project.Nodes.Clear();

                tmpPrj = Projects.LoadProject(fnm.Split('|')[0], chkShowDeleted.Checked);

                lblProjectName.Text = tmpPrj.ProjectName;
                lblProjectInfo.Text = tmpPrj.ProjectDescription;

                myTreeNode trnd = new myTreeNode();
                trnd.myObject = tmpPrj.ProjectStructure;
                trnd.Text = tmpPrj.ProjectStructure.Name;
                trnd.ImageIndex = 0;
                AddNodes(trnd, tmpPrj.ProjectStructure.SubItems);
                treeStep7Project.Nodes.Add(trnd);
            }
        }

        private class DBRowValue
        {
            public S7DataRow myRow;
            public override string ToString()
            {
                return myRow.Name + " (" + myRow.DataType.ToString() + ")";
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            lstProjectFolder.Items.Clear();

            if (treeStep7Project.SelectedNode is myTreeNode)
            {
                var tmp = (myTreeNode)treeStep7Project.SelectedNode;
                if (tmp.myObject is IBlocksFolder)
                {
                    IBlocksFolder blkFld = (IBlocksFolder)tmp.myObject;
                    if ((int)SelectPart > 1000)
                    {
                        List<ProjectBlockInfo> blocks = blkFld.readPlcBlocksList();
                        foreach (ProjectBlockInfo step7ProjectBlockInfo in blocks)
                        {
                            if (step7ProjectBlockInfo.BlockType == PLCBlockType.VAT && SelectPart == SelectPartType.VariableTable)
                                lstProjectFolder.Items.Add(step7ProjectBlockInfo);
                            if (step7ProjectBlockInfo.BlockType == PLCBlockType.DB && (SelectPart == SelectPartType.DataBlock || SelectPart == SelectPartType.DataBlocks || SelectPart == SelectPartType.IDataBlock || SelectPart == SelectPartType.IDataBlocks))
                                lstProjectFolder.Items.Add(step7ProjectBlockInfo);
                            if (step7ProjectBlockInfo.BlockType == PLCBlockType.UDT && SelectPart == SelectPartType.DataType)
                                lstProjectFolder.Items.Add(step7ProjectBlockInfo);
                            if ((step7ProjectBlockInfo.BlockType == PLCBlockType.FB || step7ProjectBlockInfo.BlockType == PLCBlockType.FC) && SelectPart == SelectPartType.FunctionBlock)
                                lstProjectFolder.Items.Add(step7ProjectBlockInfo);
                        }

                    }
                }
            }
            else if (treeStep7Project.SelectedNode is DBValueTreeNode)
            {
                //Maybe a DBTreeNode is not yet Expanded, then it need to be filled after select!
                if (treeStep7Project.SelectedNode is DBTreeNode)
                    AddDBValueNodes((DBTreeNode)treeStep7Project.SelectedNode);

                DBValueTreeNode tmp = (DBValueTreeNode)treeStep7Project.SelectedNode;

                foreach (S7DataRow s7DataRow in tmp.s7datarow.Children)
                {
                    if (s7DataRow.DataType != S7DataRowType.STRUCT && s7DataRow.DataType != S7DataRowType.UDT)
                    {
                        lstProjectFolder.Items.Add(new DBRowValue() { myRow = s7DataRow });
                    }
                }
            }

        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            loadPrj();
        }

        public object retVal;

        private void cmdOk_Click(object sender, EventArgs e)
        {
            if (SelectPart == SelectPartType.BlocksOfflineFolder)
            {
                if (treeStep7Project.SelectedNode != null)
                {
                    var tmp = (myTreeNode)treeStep7Project.SelectedNode;
                    if (typeof(IBlocksFolder).IsAssignableFrom(tmp.myObject.GetType()))
                        retVal = (IBlocksFolder)tmp.myObject;
                    else
                        retVal = null;
                }
            }
            else if (SelectPart == SelectPartType.S7ProgrammFolder)
            {
                if (treeStep7Project.SelectedNode != null)
                {
                    var tmp = (myTreeNode)treeStep7Project.SelectedNode;
                    if (typeof(S7ProgrammFolder).IsAssignableFrom(tmp.myObject.GetType()))
                        retVal = (S7ProgrammFolder)tmp.myObject;
                    else
                        retVal = null;
                }
            }
            else if (SelectPart == SelectPartType.RootProgrammFolder)
            {
                if (treeStep7Project.SelectedNode != null)
                {
                    var tmp = (myTreeNode)treeStep7Project.SelectedNode;
                    if (typeof(IRootProgrammFolder).IsAssignableFrom(tmp.myObject.GetType()))
                        retVal = (IRootProgrammFolder)tmp.myObject;
                    else
                        retVal = null;
                }
            }
            else if (SelectPart == SelectPartType.VariableTable)
            {
                if (lstProjectFolder.SelectedItem != null)
                {
                    this.Hide();
                    S7ProjectBlockInfo tmp = (S7ProjectBlockInfo)lstProjectFolder.SelectedItem;
                    if (tmp.BlockType == PLCBlockType.VAT)
                    {
                        retVal = ((IBlocksFolder)tmp.ParentFolder).GetBlock(tmp);
                        ((Block)retVal).ParentFolder = tmp.ParentFolder;
                    }
                }
            }
            else if (SelectPart == SelectPartType.VariableTableOrSymbolTable)
            {
                if (lstProjectFolder.SelectedItem != null)
                {
                    this.Hide();
                    S7ProjectBlockInfo tmp = (S7ProjectBlockInfo)lstProjectFolder.SelectedItem;
                    if (tmp.BlockType == PLCBlockType.VAT)
                    {
                        retVal = ((IBlocksFolder)tmp.ParentFolder).GetBlock(tmp);
                        ((Block)retVal).ParentFolder = tmp.ParentFolder;
                    }
                }
                else if (treeStep7Project.SelectedNode != null)
                {
                    var tmp = (myTreeNode)treeStep7Project.SelectedNode;
                    if (tmp.myObject is ISymbolTable)
                        retVal = tmp.myObject as ISymbolTable;
                }
            }
            else if (SelectPart == SelectPartType.DataBlock)
            {
                if (lstProjectFolder.SelectedItem != null)
                {
                    this.Hide();
                    S7ProjectBlockInfo tmp = (S7ProjectBlockInfo)lstProjectFolder.SelectedItem;
                    if (tmp.BlockType == PLCBlockType.DB)
                    {
                        retVal = ((IBlocksFolder)tmp.ParentFolder).GetBlock(tmp);
                        ((Block)retVal).ParentFolder = tmp.ParentFolder;
                    }
                }
            }
            else if (SelectPart == SelectPartType.IDataBlock)
            {
                if (lstProjectFolder.SelectedItem != null)
                {
                    this.Hide();
                    ProjectBlockInfo tmp = (ProjectBlockInfo)lstProjectFolder.SelectedItem;
                    if (tmp.BlockType == PLCBlockType.DB)
                    {
                        retVal = ((IBlocksFolder)tmp.ParentFolder).GetBlock(tmp);
                        ((Block)retVal).ParentFolder = tmp.ParentFolder;
                    }
                }
            }
            else if (SelectPart == SelectPartType.DataBlocks)
            {
                if (lstProjectFolder.SelectedItems.Count > 0)
                {
                    this.Hide();

                    var blocks = new List<S7DataBlock>();

                    foreach (S7ProjectBlockInfo s7ProjectBlockInfo in lstProjectFolder.SelectedItems)
                    {
                        if (s7ProjectBlockInfo.BlockType == PLCBlockType.DB)
                        {
                            var block = ((IBlocksFolder)s7ProjectBlockInfo.ParentFolder).GetBlock(s7ProjectBlockInfo);
                            block.ParentFolder = s7ProjectBlockInfo.ParentFolder;

                            blocks.Add((S7DataBlock)block);
                        }
                    }

                    retVal = blocks.Count > 0 ? blocks : null;
                }
            }
            else if (SelectPart == SelectPartType.IDataBlocks)
            {
                if (lstProjectFolder.SelectedItems.Count > 0)
                {
                    this.Hide();

                    var blocks = new List<IDataBlock>();

                    foreach (ProjectBlockInfo s7ProjectBlockInfo in lstProjectFolder.SelectedItems)
                    {
                        if (s7ProjectBlockInfo.BlockType == PLCBlockType.DB)
                        {
                            var block = ((IBlocksFolder)s7ProjectBlockInfo.ParentFolder).GetBlock(s7ProjectBlockInfo);
                            block.ParentFolder = s7ProjectBlockInfo.ParentFolder;

                            blocks.Add((IDataBlock)block);
                        }
                    }

                    retVal = blocks.Count > 0 ? blocks : null;
                }
            }
            else if (SelectPart == SelectPartType.FunctionBlock)
            {
                if (lstProjectFolder.SelectedItem != null)
                {
                    this.Hide();
                    S7ProjectBlockInfo tmp = (S7ProjectBlockInfo)lstProjectFolder.SelectedItem;
                    if (tmp.BlockType == PLCBlockType.FC || tmp.BlockType == PLCBlockType.FB || tmp.BlockType == PLCBlockType.OB)
                    {
                        retVal = ((IBlocksFolder)tmp.ParentFolder).GetBlock(tmp);
                        ((Block)retVal).ParentFolder = tmp.ParentFolder;
                    }
                }
            }
            else if (SelectPart == SelectPartType.DataType)
            {
                if (lstProjectFolder.SelectedItem != null)
                {
                    this.Hide();
                    S7ProjectBlockInfo tmp = (S7ProjectBlockInfo)lstProjectFolder.SelectedItem;
                    if (tmp.BlockType == PLCBlockType.UDT)
                    {
                        retVal = ((IBlocksFolder)tmp.ParentFolder).GetBlock(tmp);
                        ((Block)retVal).ParentFolder = tmp.ParentFolder;
                    }
                }
            }
            else if (SelectPart == SelectPartType.SymbolTable)
            {
                if (treeStep7Project.SelectedNode != null)
                {
                    var tmp = (myTreeNode)treeStep7Project.SelectedNode;
                    if (tmp.myObject is ISymbolTable)
                        retVal = tmp.myObject as ISymbolTable;
                    else
                        retVal = null;
                }
            }
            else if (SelectPart == SelectPartType.Tag)
            {
                if (lstProjectFolder.SelectedItem != null)
                {
                    this.Hide();
                    DBRowValue tmp = (DBRowValue)lstProjectFolder.SelectedItem;
                    retVal = tmp.myRow.PlcTag;
                }
            }
            if (retVal != null)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select the right Project Part, or press cancel to close this window.");
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void SelectProjectPartForm_Load(object sender, EventArgs e)
        {

        }

        private void SelectProjectPartForm_Shown(object sender, EventArgs e)
        {
            if ((int)_selectPart < 1000)
            {
                this.Width = 454;
                cmdOk.Left = 362;
                cmdCancel.Left = 362;
                lstProjectFolder.Visible = false;
            }
        }

        private SelectPartType _selectPart = SelectPartType.BlocksOfflinePart;
        public SelectPartType SelectPart
        {
            get { return _selectPart; }
            set
            {
                _selectPart = value;

                if (_selectPart == SelectPartType.DataBlocks || _selectPart == SelectPartType.IDataBlocks)
                    lstProjectFolder.SelectionMode = SelectionMode.MultiExtended;
                else
                    lstProjectFolder.SelectionMode = SelectionMode.One;
            }
        }

        private void treeStep7Project_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is DBTreeNode)
                AddDBValueNodes((DBTreeNode)e.Node);
            if (e.Node.ImageIndex == 0)
                e.Node.ImageIndex = 1;
        }

        private void treeStep7Project_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageIndex == 1)
                e.Node.ImageIndex = 0;
        }

        private void lstProjectFolder_DoubleClick(object sender, EventArgs e)
        {
            if (lstProjectFolder.SelectedItem != null)
                cmdOk_Click(sender, e);
        }

        private void treeStep7Project_DoubleClick(object sender, EventArgs e)
        {
            if (lstProjectFolder.Visible == false && treeStep7Project.SelectedNode != null)
                cmdOk_Click(sender, e);
        }
    }
}
