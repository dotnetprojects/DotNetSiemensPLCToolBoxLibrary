#if !IPHONE
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public static class SelectProjectPart
    {
        public static Step7ProjectFolder SelectBlocksOfflineFolder()
        {
            return SelectBlocksOfflineFolder("");
        }

        public static Step7ProjectFolder SelectBlocksOfflineFolder(string FileAndProjectInternalFolder)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder);
            myFrm.SelectPart = SelectPartType.BlocksOfflineFolder;
            myFrm.ShowDialog();
            return (Step7ProjectFolder)myFrm.retVal;
        }

        public static S7ProgrammFolder SelectS7ProgrammFolder()
        {
            return SelectS7ProgrammFolder("");
        }

        public static S7ProgrammFolder SelectS7ProgrammFolder(string FileAndProjectInternalFolder)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder);
            myFrm.SelectPart = SelectPartType.S7ProgrammFolder;
            myFrm.ShowDialog();
            return (S7ProgrammFolder)myFrm.retVal;
        }

        public static SymbolTable SelectSymbolTable()
        {
            return SelectSymbolTable("");
        }

        public static SymbolTable SelectSymbolTable(string FileAndProjectInternalFolder)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder);
            myFrm.SelectPart = SelectPartType.SymbolTable;
            myFrm.ShowDialog();
            return (SymbolTable)myFrm.retVal;
        }

        public static S7DataBlock SelectDataBlock(string FileAndProjectInternalFolder)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder);
            myFrm.SelectPart = SelectPartType.DataBlock;
            myFrm.ShowDialog();
            return (S7DataBlock)myFrm.retVal;
        }

        public static S7FunctionBlock SelectFunctionBlock(string FileAndProjectInternalFolder)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder);
            myFrm.SelectPart = SelectPartType.DataBlock;
            myFrm.ShowDialog();
            return (S7FunctionBlock)myFrm.retVal;
        }


        public static S7VATBlock SelectVAT()
        {
            return SelectVAT("");
        }

        public static S7VATBlock SelectVAT(string FileAndProjectInternalFolder)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder);
            myFrm.SelectPart = SelectPartType.VariableTable;
            myFrm.ShowDialog();
            return (S7VATBlock)myFrm.retVal;
        }

        public static S7DataBlock SelectUDT()
        {
            return SelectUDT("");
        }

        public static S7DataBlock SelectUDT(string FileAndProjectInternalFolder)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder);
            myFrm.SelectPart = SelectPartType.DataType;
            myFrm.ShowDialog();
            return (S7DataBlock)myFrm.retVal;
        }

        public static PLCTag SelectTAG()
        {
            return SelectTAG("");
        }

        //This selects a Tag From a Step 7 Project
        public static PLCTag SelectTAG(string FileAndProjectInternalFolder)
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm(FileAndProjectInternalFolder);
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