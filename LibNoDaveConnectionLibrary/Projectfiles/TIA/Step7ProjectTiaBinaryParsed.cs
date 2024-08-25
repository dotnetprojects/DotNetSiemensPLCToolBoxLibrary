using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Structs;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA
{
    public class Step7ProjectTiaBinaryParsed : Project
    {
        private readonly Credentials _credentials;

        private string DataFile = null;

        private XmlDocument tiaProject;

        internal ZipHelper _ziphelper = new ZipHelper(null);

        public CultureInfo Culture { get; set; }

        //will be removed later...
        public List<TiaObjectHeader> TiaObjectsList;

        public void BinaryParseTIAFile()
        {
            using (var sourceStream = _ziphelper.GetReadStream(DataFile))
            {
                var rd = new BinaryReader(sourceStream);
                var header = TiaFileHeader.Deserialize(rd);

                TiaObjectsList = new List<TiaObjectHeader>();
                try
                {
                    while (true)
                    {
                        var hd = TiaObjectHeader.Deserialize(rd);
                        TiaObjectsList.Add(hd);
                    }
                }
                catch (EndOfStreamException) // Zip File Stream has no length
                { }

                /*var rootId = new TiaObjectId(TiaFixedRootObjectInstanceIds.RootObjectCollectionId);
                var rootObjects = new TiaRootObjectList(TiaObjects[rootId]);
                var projectid = rootObjects.TiaRootObjectEntrys.FirstOrDefault(x => x.ObjectId.TypeId == (int)TiaTypeIds.Siemens_Automation_DomainModel_ProjectData).ObjectId;
                var projectobj = TiaObjects[projectid];*/
            }
        }

        public Step7ProjectTiaBinaryParsed(string projectfile, CultureInfo culture = null) : this(projectfile, culture, null)
        {
        }

        public Step7ProjectTiaBinaryParsed(string projectfile, CultureInfo culture = null, Credentials credentials = null)
        {
            _credentials = credentials;
            if (culture == null)
                Culture = CultureInfo.CurrentCulture;
            else
                Culture = culture;

            ProjectFile = projectfile;

            if (ProjectFile.ToLower().EndsWith("zip") || Path.GetExtension(ProjectFile.ToLower()).Contains("zap"))
            {
                this._ziphelper = new ZipHelper(projectfile);
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = _ziphelper.GetFirstZipEntryWithMatch("\\.ap.*");
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = _ziphelper.GetFirstZipEntryWithMatch("\\.al.*");
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

            if (_ziphelper.IsZipFile)
                DataFile = "System\\PEData.plf";
            else
                DataFile = Path.GetDirectoryName(projectfile) + "\\System\\PEData.plf";
            ProjectFolder = projectfile.Substring(0, projectfile.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar;

            BinaryParseTIAFile();
        }

        internal XmlDocument xmlDoc;

        private object tiaExport;
        private Type tiaExportType;


        public override ProjectType ProjectType
        {
            get { return ProjectType.TiaBinary; }
        }

        protected override void LoadProject()
        {
            _projectLoaded = true;
        }
    }
}
