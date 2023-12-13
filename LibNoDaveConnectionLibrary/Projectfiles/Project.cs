using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public abstract class Project
    {

        internal static Logger logger = LogManager.GetCurrentClassLogger();
        public abstract ProjectType ProjectType { get; }

        public string ProjectFile { get; set; }

        public string ProjectFolder { get; set; }

        public String ProjectName { get; set; }

        public String ProjectDescription { get; set; }

        private ProjectFolder _ProjectStructure;

        protected List<ProjectFolder> _allFolders = new List<ProjectFolder>();

        public List<ProjectFolder> AllFolders
        {
            get { return _allFolders; }
        }

        public MnemonicLanguage ProjectLanguage { get; set; }

        public Encoding ProjectEncoding = Encoding.GetEncoding("ISO-8859-1");

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

        protected bool _projectLoaded;

        protected abstract void LoadProject();

        public override string ToString()
        {
            string retVal = ProjectName;

            if (ProjectName == null)
                retVal = ProjectFile;

            return retVal;
        }
    }
}