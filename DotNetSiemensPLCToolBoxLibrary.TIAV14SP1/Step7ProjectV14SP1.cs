using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA;
using Microsoft.Win32;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.V14SP1
{
    public partial class Step7ProjectV14SP1 : Project, IDisposable
    {
        public enum TiaVersionTypes
        {
            V11 = 11,
            V12 = 12,
            V13 = 13,
            V14 = 14,
        }

        public static TiaVersionTypes TiaVersion { get; private set; }

        private string DataFile = null;

        private XmlDocument tiaProject;

        internal ZipHelper _ziphelper = new ZipHelper(null);

        public CultureInfo Culture { get; set; }

        public Step7ProjectV14SP1(string projectfile, CultureInfo culture = null)
        {
            if (culture == null)
                Culture = CultureInfo.CurrentCulture;
            else
                Culture = culture;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

            ProjectFile = projectfile;

            if (ProjectFile.ToLower().EndsWith("zip") || ProjectFile.ToLower().EndsWith("zap14"))
            {
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".ap14");
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".al14");

                if (string.IsNullOrEmpty(projectfile))
                    throw new Exception("Zip-File contains no valid TIA Project !");
                this._ziphelper = new ZipHelper(projectfile);
            }

           
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(_ziphelper.GetReadStream(projectfile));

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("x", "http://www.siemens.com/2007/07/Automation/CommonServices/DataInfoValueData");

                var nd = xmlDoc.SelectSingleNode("x:Data", nsmgr);
                this.ProjectName = nd.Attributes["Name"].Value;
            }
            catch (Exception) 
            { }

            DataFile = Path.GetDirectoryName(projectfile) + "\\System\\PEData.plf";
            ProjectFolder = projectfile.Substring(0, projectfile.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar;

            //BinaryParseTIAFile();
            //LoadProject();
            LoadViaOpennessDlls();

            currentDomain.AssemblyResolve -= currentDomain_AssemblyResolve;            
        }        

        internal XmlDocument xmlDoc;
        
        Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            int index = args.Name.IndexOf(',');
            if (index == -1)
            {
                return null;
            }
            var name = args.Name.Substring(0, index) + ".dll";

            var filePathReg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Siemens\\Automation\\_InstalledSW\\TIAP14\\Global") ??
                              Registry.LocalMachine.OpenSubKey("SOFTWARE\\Siemens\\Automation\\_InstalledSW\\TIAP14\\Global");

            if (filePathReg != null)
            {
                string filePath = Path.Combine(filePathReg.GetValue("Path").ToString(), "PublicAPI\\V14 SP1");
                var path = Path.Combine(filePath, name);
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    return Assembly.LoadFrom(fullPath);
                }

                filePath = Path.Combine(filePathReg.GetValue("Path").ToString(), "PublicAPI\\V14");
                path = Path.Combine(filePath, name);
                fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    return Assembly.LoadFrom(fullPath);
                }
            }

            return null;
        }

        private object tiaExport;
        private Type tiaExportType;
        

        internal Dictionary<TiaObjectId, TiaFileObject> TiaObjects = new Dictionary<TiaObjectId, TiaFileObject>();

        protected override void LoadProject()
        {
            _projectLoaded = true;
        }
    }
}
