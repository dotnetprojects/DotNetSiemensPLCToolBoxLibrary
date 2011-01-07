using System.Collections.Generic;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;

namespace LibNoDaveConnectionLibrary.DataTypes.Projects
{
    /// <summary>
    /// Base Abstract Class for every Project Folder.
    /// </summary>
    public class ProjectFolder
    {
        public string Name { get; set; }

        public List<ProjectFolder> SubItems { get; set; }
        public ProjectFolder Parent { get; set; }

        public LibNoDaveConnectionLibrary.Projectfiles.Project Project { get; set; }

        //This is the ID of the Folder in the Database (not everyone has one)
        //Only for internal use.
        internal int ID;

        
        public ProjectFolder()
        {
            SubItems = new List<ProjectFolder>();
        }

        public override string ToString()
        {
            if (Parent != null)
                return Parent + "\\" + Name;
            return Name;
        }
    }
}
