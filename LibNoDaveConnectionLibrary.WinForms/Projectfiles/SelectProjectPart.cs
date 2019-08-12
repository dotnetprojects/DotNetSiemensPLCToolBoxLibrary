#if !IPHONE
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public static class SelectProjectPart
    {
        public static BlocksOfflineFolder SelectBlocksOfflineFolder()
        {
            return SelectBlocksOfflineFolder("");
        }

        public static BlocksOfflineFolder SelectBlocksOfflineFolder(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.BlocksOfflineFolder;
            myFrm.ShowDialog();
            return (BlocksOfflineFolder)myFrm.retVal;
        }

        public static S7ProgrammFolder SelectS7ProgrammFolder()
        {
            return SelectS7ProgrammFolder("");
        }

        public static S7ProgrammFolder SelectS7ProgrammFolder(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.S7ProgrammFolder;
            myFrm.ShowDialog();
            return (S7ProgrammFolder)myFrm.retVal;
        }

        public static IRootProgrammFolder SelectRootProgrammFolder()
        {
            return SelectRootProgrammFolder("");
        }

        public static IRootProgrammFolder SelectRootProgrammFolder(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.RootProgrammFolder;
            myFrm.ShowDialog();
            return (IRootProgrammFolder)myFrm.retVal;
        }

        public static ISymbolTable SelectSymbolTable()
        {
            return SelectSymbolTable("");
        }

        public static ISymbolTable SelectSymbolTable(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.SymbolTable;
            myFrm.ShowDialog();
            return (ISymbolTable)myFrm.retVal;
        }

        public static S7DataBlock SelectDataBlock(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.DataBlock;
            myFrm.ShowDialog();
            return (S7DataBlock)myFrm.retVal;
        }

        public static IDataBlock SelectIDataBlock(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.IDataBlock;
            myFrm.ShowDialog();
            return (IDataBlock)myFrm.retVal;
        }

        public static List<S7DataBlock> SelectDataBlocks(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.DataBlocks;
            myFrm.ShowDialog();
            return (List<S7DataBlock>)myFrm.retVal;
        }

        public static List<IDataBlock> SelectIDataBlocks(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.IDataBlocks;
            myFrm.ShowDialog();
            return (List<IDataBlock>)myFrm.retVal;
        }

        public static S7FunctionBlock SelectFunctionBlock(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.DataBlock;
            myFrm.ShowDialog();
            return (S7FunctionBlock)myFrm.retVal;
        }

        public static S7VATBlock SelectVAT()
        {
            return SelectVAT("");
        }

        public static S7VATBlock SelectVAT(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.VariableTable;
            myFrm.ShowDialog();
            return (S7VATBlock)myFrm.retVal;
        }

        public static object SelectVATorSymbolTable()
        {
            return SelectVATorSymbolTable("");
        }

        public static object SelectVATorSymbolTable(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.VariableTableOrSymbolTable;
            myFrm.ShowDialog();
            return myFrm.retVal;
        }

        public static S7DataBlock SelectUDT()
        {
            return SelectUDT("");
        }

        public static S7DataBlock SelectUDT(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.DataType;
            myFrm.ShowDialog();
            return (S7DataBlock)myFrm.retVal;
        }

        public static PLCTag SelectTAG()
        {
            return SelectTAG("");
        }

        //This selects a Tag From a Step 7 Project
        public static PLCTag SelectTAG(string FileAndProjectInternalFolder, bool hideOpenProjectButton = true)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder, hideOpenProjectButton);
            myFrm.SelectPart = SelectPartType.Tag;
            myFrm.ShowDialog();
            PLCTag retVal = (PLCTag) myFrm.retVal;
            myFrm.Close();
            myFrm.Dispose();
            return retVal;
        }

        //This is used to select multiple Tags from a Step 7 Project
        public static List<PLCTag> SelectTAGs(string FileAndProjectInternalFolder)
        {
            SelectProjectPartMultiForm myFrm = new SelectProjectPartMultiForm(FileAndProjectInternalFolder);
            myFrm.SelectPart = SelectPartType.Tag;
            myFrm.ShowDialog();
            var tags = myFrm.SelectedTags;
            myFrm.Dispose();
            return tags;
        }
    }
}
#endif