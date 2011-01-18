namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    /// <summary>
    /// This is the Main Folder, it Contains a Blocks Folder and a Symbol Table!
    /// </summary>
    public interface IProgrammFolder
    {
        ISymbolTable SymbolTable { get; set; }
        IBlocksFolder BlocksFolder { get; set; }
    }
}
