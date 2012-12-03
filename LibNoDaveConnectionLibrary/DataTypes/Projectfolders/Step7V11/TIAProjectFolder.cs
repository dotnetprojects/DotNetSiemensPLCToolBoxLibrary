using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    /// <summary>
    /// Base Abstract Class for every Project Folder.
    /// </summary>
    public class TIAProjectFolder : ProjectFolder
    {
        public TIAProjectFolder(Step7ProjectV11 Project)
        {
            this.Project = Project;
        }

    }
}
