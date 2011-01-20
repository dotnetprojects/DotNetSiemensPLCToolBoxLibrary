using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5
{
    public class Step5ProgrammFolder : ProjectFolder, IProgrammFolder
    {
        public ISymbolTable SymbolTable
        {
            get
            {
                if (SubItems.Count>1)
                    return (ISymbolTable)SubItems[1];
                return null;
            }
            set {  }
        }

        public IBlocksFolder BlocksFolder
        {
            get { return (IBlocksFolder)SubItems[0]; }
            set { SubItems[0] = (ProjectFolder)value; }
        }

        public OnlineBlocksFolder OnlineBlocksFolder { get { return null; } set { } }

    }
}
