using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using System.IO;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Openness
{
    public interface ITIAOpennessProjectFolder : IProjectFolder
    {
        string Name { get; set; }

        void ImportFile(FileInfo file, bool overwrite, bool importFromSource);

        void CompileBlocks();
    }
}