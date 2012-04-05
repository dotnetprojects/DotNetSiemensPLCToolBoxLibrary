namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    /// <summary>
    /// This is the Main Folder, it Contains a BlockInfos Folder and a Symbol Table!
    /// </summary>
    public interface IProgrammFolder
    {
        ISymbolTable SymbolTable { get; set; }
        IBlocksFolder BlocksFolder { get; set; }
        OnlineBlocksFolder OnlineBlocksFolder { get; set; }
    }
}
