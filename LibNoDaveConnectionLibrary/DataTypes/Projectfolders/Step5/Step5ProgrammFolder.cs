using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5
{
    public class Step5ProgrammFolder : ProjectFolder, IProgrammFolder
    {
        public ISymbolTable SymbolTable
        {
            get
            {
                if (SubItems.Count > 1 && SubItems[1] is SymbolTable)
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

        public ReferenceData ReferenceDataFolder
        {
            get
            {
                if (SubItems.Count > 2)
                    return (ReferenceData) SubItems[2];
                else
                    return (ReferenceData) SubItems[1];                
            }
            set {  }
        }

        public OnlineBlocksFolder OnlineBlocksFolder { get { return null; } set { } }

    }
}
