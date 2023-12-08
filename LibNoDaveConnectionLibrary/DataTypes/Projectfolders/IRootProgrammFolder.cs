using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    //Root Folder for a PLC Program
    //in S7 it is the Programs Folder!
    //in TIA it's the CPU Folder
    public interface IRootProgrammFolder
    {
        Block GetBlockRecursive(string name);
    }
}