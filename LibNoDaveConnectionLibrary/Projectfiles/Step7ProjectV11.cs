using System.IO;

namespace LibNoDaveConnectionLibrary.Projectfiles
{
    public class Step7ProjectV11 : Project
    {
        
        public Step7ProjectV11(string projectfile)
        {
            ProjectFile = projectfile;
            ProjectFolder = projectfile.Substring(0, projectfile.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar;            
            LoadProject();
        }

        internal override void LoadProject()
        {
            //XmlReader 
        }
    }
}
