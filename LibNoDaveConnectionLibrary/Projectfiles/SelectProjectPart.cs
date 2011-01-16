#if !IPHONE
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public static class SelectProjectPart
    {
        public static Step7ProjectFolder SelectBlocksOfflineFolder()
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm();
            myFrm.SelectPart = SelectPartType.BlocksOfflineFolder;
            myFrm.ShowDialog();
            return (Step7ProjectFolder)myFrm.retVal;
        }

        public static S7ProgrammFolder SelectS7ProgrammFolder()
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm();
            myFrm.SelectPart = SelectPartType.S7ProgrammFolder;
            myFrm.ShowDialog();
            return (S7ProgrammFolder)myFrm.retVal;
        }

        public static SymbolTable SelectSymbolTable()
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm();
            myFrm.SelectPart = SelectPartType.SymbolTable;
            myFrm.ShowDialog();
            return (SymbolTable)myFrm.retVal;
        }
        
        public static VATBlock SelectVAT()
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm();
            myFrm.SelectPart = SelectPartType.VariableTable;
            myFrm.ShowDialog();
            return (VATBlock)myFrm.retVal;
        }

        //This selects a Tag From a Step 7 Project
        public static PLCTag SelectTAG()
        {
            SelectProjectPartForm myFrm = new SelectProjectPartForm();
            myFrm.SelectPart = SelectPartType.LibNoDaveValue;
            myFrm.ShowDialog();
            return (PLCTag)myFrm.retVal;
        }
    }
}
#endif