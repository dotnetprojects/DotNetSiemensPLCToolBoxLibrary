using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace LibNoDaveConnectionLibrary.STEP7Projectfiles
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
