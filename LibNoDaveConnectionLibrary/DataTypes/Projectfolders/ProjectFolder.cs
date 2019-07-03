using System;
using System.Collections.Generic;
using System.Drawing;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    /// <summary>
    /// Base Abstract Class for every Project Folder.
    /// </summary>
    public class ProjectFolder : ProjectItem, IProjectFolder
    {
#if !IPHONE	
        public string FolderClosedImageName
        {
            get
            {
                if (this is StationConfigurationFolder)
                {
                    if (((StationConfigurationFolder)this).StationType == PLCType.Simatic300)
                        return "DotNetSiemensPLCToolBoxLibrary.Resources.FolderImages.s7_300_rack.ico"; 
                    else if (((StationConfigurationFolder)this).StationType == PLCType.Simatic400 || ((StationConfigurationFolder)this).StationType == PLCType.Simatic400H)
                        return "DotNetSiemensPLCToolBoxLibrary.Resources.FolderImages.s7_400_rack.ico"; 
                }
                else if (this is CPUFolder)
                {
                    if (((CPUFolder)this).CpuType == PLCType.Simatic300)
                        return "DotNetSiemensPLCToolBoxLibrary.Resources.FolderImages.s7_300.ico"; 
                    else if (((CPUFolder)this).CpuType == PLCType.Simatic400 || ((CPUFolder)this).CpuType == PLCType.Simatic400H)
                        return "DotNetSiemensPLCToolBoxLibrary.Resources.FolderImages.s7_400.ico"; 
                }
                else if (this is Step5ProgrammFolder)
                {
                    return "DotNetSiemensPLCToolBoxLibrary.Resources.FolderImages.s5.ico";
                }
                return "DotNetSiemensPLCToolBoxLibrary.Resources.FolderImages.closed_folder.ico";             
            }
        }

        public Image FolderClosedImage
        {
            get
            {
                System.Reflection.Assembly thisExe;
                thisExe = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream file = thisExe.GetManifestResourceStream(FolderClosedImageName);
                if (file == null)
                    return null;
                return Image.FromStream(file);
            }
        }

        public string FolderOpenedImageName
        {
            get
            {
                return "DotNetSiemensPLCToolBoxLibrary.Resources.FolderImages.opened_folder.ico";
            }
        }
        public Image FolderOpenedImage
        {
            get
            {
                System.Reflection.Assembly thisExe;
                thisExe = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream file = thisExe.GetManifestResourceStream(FolderOpenedImageName);
                if (file == null)
                    return null;
                return Image.FromStream(file);
            }
        }
#endif

        public List<ProjectFolder> SubItems { get; set; }
        public ProjectFolder Parent { get; set; }

        public string StructuredFolderName
        {
            get
            {
                if (Parent != null) return Parent.StructuredFolderName + "\\" + Name;
                return Name;
            }
        }

        public string ProjectAndStructuredFolderName
        {
            get { return this.Project.ProjectFile + "|" + StructuredFolderName; }
        }

        public Project Project { get; set; }

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

        public virtual ProjectFolder CreateFolder(string name)
        {
            throw new NotImplementedException();
        }
    }
}
