using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    internal partial class SelectProjectPartForm : Form
    {
        public SelectProjectPartForm()
        {
            InitializeComponent();            
        }        

        Step7ProjectV5 tmpPrj;        

        class myTreeNode : TreeNode
        {
            public object myObject;
        }

        public void AddNodes(TreeNode nd, List<ProjectFolder> lst)
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
                    if (((StationConfigurationFolder)subitem).StationType==PLCType.Simatic300)
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
            }
        }

        private string fnm = "";
       
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog op=new OpenFileDialog();
            op.Filter = "*.s7p|*.s7p";
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

                tmpPrj = new Step7ProjectV5(fnm, chkShowDeleted.Checked);

                //listBox1.Items.AddRange(tmp.PrgProjectFolders.ToArray());
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

      
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
            lstProjectFolder.Items.Clear();
            var tmp = (myTreeNode) treeStep7Project.SelectedNode;
            if (tmp.myObject.GetType() == typeof(BlocksOfflineFolder))
            {
                BlocksOfflineFolder blkFld = (BlocksOfflineFolder) tmp.myObject;
                if ((int)SelectPart>1000) 
                {
                    List<ProjectBlockInfo> blocks = blkFld.readPlcBlocksList();
                    foreach (ProjectBlockInfo step7ProjectBlockInfo in blocks)
                    {
                        if (step7ProjectBlockInfo.BlockType == PLCBlockType.VAT && SelectPart==SelectPartType.VariableTable)
                            lstProjectFolder.Items.Add(step7ProjectBlockInfo);
                        if (step7ProjectBlockInfo.BlockType == PLCBlockType.DB && SelectPart == SelectPartType.DataBlock)
                            lstProjectFolder.Items.Add(step7ProjectBlockInfo);
                        if (step7ProjectBlockInfo.BlockType == PLCBlockType.UDT && SelectPart == SelectPartType.DataType)
                            lstProjectFolder.Items.Add(step7ProjectBlockInfo);
                        if ((step7ProjectBlockInfo.BlockType == PLCBlockType.FB || step7ProjectBlockInfo.BlockType == PLCBlockType.FC) && SelectPart == SelectPartType.FunctionBlock)
                            lstProjectFolder.Items.Add(step7ProjectBlockInfo);
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
                    var tmp = (myTreeNode) treeStep7Project.SelectedNode;
                    if (tmp.myObject.GetType() == typeof(BlocksOfflineFolder))
                        retVal = (BlocksOfflineFolder) tmp.myObject;
                    else
                        retVal = null;                    
                }
            }
            else if (SelectPart==SelectPartType.S7ProgrammFolder)
            {
                if (treeStep7Project.SelectedNode != null)
                {
                    var tmp = (myTreeNode)treeStep7Project.SelectedNode;
                    if (tmp.myObject.GetType() == typeof(S7ProgrammFolder))
                        retVal = (S7ProgrammFolder)tmp.myObject;
                    else
                        retVal = null;
                }
            }
            else if (SelectPart == SelectPartType.VariableTable)
            {
                if (lstProjectFolder.SelectedItem != null)
                {
                    this.Hide();
                    S7ProjectBlockInfo tmp = (S7ProjectBlockInfo) lstProjectFolder.SelectedItem;
                    if (tmp.BlockType == PLCBlockType.UDT)
                    {
                        retVal = ((IBlocksFolder) tmp.ParentFolder).GetBlock(tmp);
                        ((Block) retVal).ParentFolder = tmp.ParentFolder;
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
                        retVal = ((IBlocksFolder) tmp.ParentFolder).GetBlock(tmp);
                        ((Block) retVal).ParentFolder = tmp.ParentFolder;
                    }
                }
            }
            else if (SelectPart == SelectPartType.SymbolTable)
            {
                if (treeStep7Project.SelectedNode != null)
                {                    
                    var tmp = (myTreeNode)treeStep7Project.SelectedNode;
                    if (tmp.myObject.GetType() == typeof(SymbolTable))
                        retVal = (SymbolTable)tmp.myObject;
                    else
                        retVal = null;                    
                }
            }
            if (retVal!=null)
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
            set { _selectPart = value; }
        }

        private void treeStep7Project_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
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
