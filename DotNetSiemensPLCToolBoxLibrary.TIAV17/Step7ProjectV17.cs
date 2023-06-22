using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA;
using DotNetSiemensPLCToolBoxLibrary.TIAV17;
using Microsoft.Win32;
using Siemens.Engineering;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.V17
{
    public partial class Step7ProjectV17 : Project, IDisposable
    {
        private readonly Credentials _credentials;

        private string DataFile = null;

        private XmlDocument tiaProject;

        internal ZipHelper _ziphelper = new ZipHelper(null);

        public CultureInfo Culture { get; set; }

        public Step7ProjectV17()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

            AksForInstance();

            LoadViaOpennessDlls();

            currentDomain.AssemblyResolve -= currentDomain_AssemblyResolve;
        }

        public static Step7ProjectV17 AttachToInstanceWithFilename(string filename)
        {
            var inst = new Step7ProjectV17("");

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += inst.currentDomain_AssemblyResolve;
            inst.AksForInstanceWithFilename(filename);
            inst.LoadViaOpennessDlls();
            currentDomain.AssemblyResolve -= inst.currentDomain_AssemblyResolve;

            return inst;
        }

        private Step7ProjectV17(string notUsed)
        { }


        private void AksForInstanceWithFilename(string file)
        {

            tiaPortal = new Siemens.Engineering.TiaPortal(Siemens.Engineering.TiaPortalMode.WithoutUserInterface);

            var processes = TiaPortal.GetProcesses().ToArray();
            var process = processes.First(x => x.ProjectPath != null && x.ProjectPath.FullName == file);
            tiaPortal = process.Attach();
            tiapProject = tiaPortal.Projects[0];
            this.ProjectFile = process.ProjectPath.ToString();
        }

        private void AksForInstance()
        {

            tiaPortal = new Siemens.Engineering.TiaPortal(Siemens.Engineering.TiaPortalMode.WithoutUserInterface);

            var processes = TiaPortal.GetProcesses().ToArray();
            var sLst = processes.Select(x => "Projekt : " + (x.ProjectPath != null ? x.ProjectPath.ToString() : "-")).ToArray();
            AppDomain domain = AppDomain.CreateDomain("another domain");
            CrossAppDomainDelegate action = () =>
            {
                var app = new Application();
                var ask = new SelectPortalInstance();
                var p = AppDomain.CurrentDomain.GetData("processes") as string[];
                ask.lstInstances.ItemsSource = p;
                app.Run(ask);
                AppDomain.CurrentDomain.SetData("idx", ask.lstInstances.SelectedIndex);
            };
            domain.SetData("processes", sLst);
            domain.DoCallBack(action);
            var idx = (int)domain.GetData("idx");

            tiaPortal = processes[idx].Attach();
            tiapProject = tiaPortal.Projects[0];
            this.ProjectFile = processes[idx].ProjectPath.ToString();
        }

        public Step7ProjectV17(string projectfile, CultureInfo culture = null) : this(projectfile, culture, null)
        {
        }

        public Step7ProjectV17(string projectfile, CultureInfo culture = null, Credentials credentials = null)
        {
            _credentials = credentials;
            if (culture == null)
                Culture = CultureInfo.CurrentCulture;
            else
                Culture = culture;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

            ProjectFile = projectfile;

            if (ProjectFile.ToLower().EndsWith("zip") || ProjectFile.ToLower().EndsWith("zap17"))
            {
                this._ziphelper = new ZipHelper(projectfile);
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = _ziphelper.GetFirstZipEntryWithEnding(".ap17");
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = _ziphelper.GetFirstZipEntryWithEnding(".al17");
                if (string.IsNullOrEmpty(projectfile))
                    throw new Exception("Zip-File contains no valid TIA Project !");
            }

            try
            {
                using (var stream = _ziphelper.GetReadStream(projectfile))
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(stream);

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("x", "http://www.siemens.com/2007/07/Automation/CommonServices/DataInfoValueData");

                    var nd = xmlDoc.SelectSingleNode("x:Data", nsmgr);
                    this.ProjectName = nd.Attributes["Name"].Value;
                }
            }
            catch (Exception)
            { }

            DataFile = Path.GetDirectoryName(projectfile) + "\\System\\PEData.plf";
            ProjectFolder = projectfile.Substring(0, projectfile.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar;

            //BinaryParseTIAFile();
            //LoadProject();
            OpenViaOpennessDlls(credentials);

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

            var filePathReg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Siemens\\Automation\\_InstalledSW\\TIAP17\\Global") ??
                              Registry.LocalMachine.OpenSubKey("SOFTWARE\\Siemens\\Automation\\_InstalledSW\\TIAP17\\Global");

            if (filePathReg != null)
            {
                string filePath = Path.Combine(filePathReg.GetValue("Path").ToString(), "PublicAPI\\V17");
                var path = Path.Combine(filePath, name);
                var fullPath = Path.GetFullPath(path);
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

        public override ProjectType ProjectType
        {
            get { return ProjectType.Tia17; }
        }

        protected override void LoadProject()
        {
            _projectLoaded = true;
        }
    }
}
