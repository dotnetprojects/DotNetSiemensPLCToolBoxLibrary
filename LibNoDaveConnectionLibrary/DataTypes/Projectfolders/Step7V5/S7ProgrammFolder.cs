namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class S7ProgrammFolder : Step7ProjectFolder, IProgrammFolder
    {
        internal int _linkfileoffset;
        public ISymbolTable SymbolTable { get; set; }
        public BlocksOfflineFolder BlocksOfflineFolder { get; set; }
        public OnlineBlocksFolder OnlineBlocksFolder { get; set; }

        public IBlocksFolder BlocksFolder
        {
            get { return BlocksOfflineFolder; }
            set
            {
                BlocksOfflineFolder = (BlocksOfflineFolder)value;
            }
        }
    }
}
