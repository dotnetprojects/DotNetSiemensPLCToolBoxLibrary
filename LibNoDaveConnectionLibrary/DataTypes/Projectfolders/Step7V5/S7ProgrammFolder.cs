namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class S7ProgrammFolder : Step7ProjectFolder
    {
        internal int _linkfileoffset;
        public SymbolTable SymbolTable { get; set; }
        public BlocksOfflineFolder BlocksOfflineFolder { get; set; }
    }
}
