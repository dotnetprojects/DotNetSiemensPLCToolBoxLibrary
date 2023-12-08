using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    public interface IProjectFolder
    {
        string Name { get; set; }
        Project Project { get; set; }
        ProjectFolder Parent { get; set; }
        List<ProjectFolder> SubItems { get; set; }

        string ProjectAndStructuredFolderName { get; }
    }
}