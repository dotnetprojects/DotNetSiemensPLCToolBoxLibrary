using System;
using System.Collections.Generic;
using System.Text;
using LibNoDaveConnectionLibrary.DataTypes.Projects;

namespace LibNoDaveConnectionLibrary.Projectfiles
{
    public abstract class Project
    {
        public string ProjectFile{ get; set;}
        public string ProjectFolder{ get; set;}

        public String ProjectName { get; set; }
        public String ProjectDescription { get; set; }

        private ProjectFolder _ProjectStructure;
        public ProjectFolder ProjectStructure
        {
            get
            {
                if (!_projectLoaded)
                    LoadProject();
                return _ProjectStructure;
            }
            set { _ProjectStructure = value; }
        }
        internal bool _projectLoaded;
        internal abstract void LoadProject();

        public override string ToString()
        {
            string retVal = ProjectName;
            
            if (ProjectName == null)
                retVal= ProjectFile;
            
            return retVal;
        }

    }
}
