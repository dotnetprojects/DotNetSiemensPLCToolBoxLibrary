using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5
{
    class Step5ProjectFolder : ProjectFolder, IProgrammFolder
    {
        public ISymbolTable SymbolTable
        {
            get { return null; }
            set {  }
        }

        public IBlocksFolder BlocksFolder
        {
            get { return (IBlocksFolder)SubItems[0]; }
            set { SubItems[0] = (ProjectFolder)value; }
        }
    }
}
