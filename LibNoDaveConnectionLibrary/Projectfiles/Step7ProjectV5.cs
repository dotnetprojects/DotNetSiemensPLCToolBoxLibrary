using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Hardware.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S5.MC5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Network;
using System.Diagnostics;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public class Step7ProjectV5 : Project, IDisposable
    {
        //types object
        const int objectType_Simatic300 = 1314969;
        const int objectType_Simatic400 = 1314970;
        const int objectType_Simatic400H = 1315650;
        const int objectType_EternetInCPU3xx = 2364796;
        const int objectType_EternetInCPU4xx = 2364763;
        const int objectType_MpiDPinCPU = 1314972;
        const int objectType_MpiDP400 = 1315038;
        const int objectType_MpiDP300 = 1315016;
        
        private string _offlineblockdb;

        internal bool _showDeleted = false;

        //Zipfile is used as Object, because SharpZipLib is not available on every platform!
        internal ZipHelper _ziphelper = new ZipHelper(null);

        //When a Zip File is used, here is the s7p name!
        internal string _projectfilename;

        internal char _DirSeperator
        {
            get
            {
                if (!_ziphelper.IsZipped())
                    return Path.DirectorySeparatorChar;
                else
                    return '/';
            }
        }

        public Step7ProjectV5(string projectfile, bool showDeleted)
            : this(projectfile, showDeleted, null)
        { }

        public Step7ProjectV5(string projectfile, bool showDeleted, Encoding prEn)
        {
            _projectfilename = projectfile;

            if (projectfile.ToLower().EndsWith("zip"))
            {
                _projectfilename = ZipHelper.GetFirstZipEntryWithEnding(projectfile, ".s7p");

                if (string.IsNullOrEmpty(_projectfilename))
                    _projectfilename = ZipHelper.GetFirstZipEntryWithEnding(projectfile, ".s7l");

                if (string.IsNullOrEmpty(_projectfilename))
                    throw new Exception("Zip-File contains no valid Step7 Project !");
                this._ziphelper = new ZipHelper(projectfile);

            }

            ProjectFile = projectfile;
            ProjectFolder = _projectfilename.Substring(0, _projectfilename.LastIndexOf(_DirSeperator)) + _DirSeperator;

            ProjectEncoding = (prEn ?? Encoding.GetEncoding("ISO-8859-1"));
            var lngFile = _ziphelper.GetReadStream(ProjectFolder + "Global" + _DirSeperator + "Language");
            if (prEn == null && lngFile != null)
            {
                var rd = new StreamReader(lngFile);
                string line;
                while ((line = rd.ReadLine()) != null)
                {
                    if (line != "0")
                    {
                        int code;
                        if (int.TryParse(line, out code))
                        {
                            var enc = Encoding.GetEncodings().FirstOrDefault(x => x.CodePage == code);
                            if (enc != null)
                            {
                                ProjectEncoding = enc.GetEncoding();
                                break;
                            }
                        }
                    }
                }
            }

            LoadProjectHeader(projectfile, showDeleted);
        }

        private void LoadProjectHeader(string projectfile, bool showDeleted)
        {
            _showDeleted = showDeleted;

            //Projekt Infos auslesen
            //FileStream fsProject = new FileStream(ProjectFile, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite);
            Stream fsProject = _ziphelper.GetReadStream(_projectfilename);

            //Anzahl der Bytes auslesen..
            byte[] projectFile = new byte[_ziphelper.GetStreamLength(_projectfilename, fsProject)];
            fsProject.Read(projectFile, 0, projectFile.Length);//Convert.ToInt32(fsProject.Length));
            fsProject.Close();

            ProjectName = System.Text.Encoding.UTF7.GetString(projectFile, 5, projectFile[4]);
            ProjectDescription = System.Text.Encoding.UTF7.GetString(projectFile, 5 + projectFile[4] + 2, projectFile[projectFile[4] + 6]);
            //Fertig

            _offlineblockdb = ProjectFolder + "ombstx" + _DirSeperator + "offline" + _DirSeperator + "BSTCNTOF.DBF";
        }

        public override string ToString()
        {
            string retVal = base.ToString();
            if (_ziphelper.IsZipped())
                retVal += "(zipped)";
            if (_showDeleted == true)
                retVal += " (show deleted)";
            return retVal;
        }

        internal bool hasChanges;

        ~Step7ProjectV5()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (hasChanges)
            {
                hasChanges = false;
                //ZipHelper.SaveZip(_zipfile);
            }
            _ziphelper.Close();
        }

        /*
        private Step7ProjectFolder _step7ProjectStructure;

        public Step7ProjectFolder Step7ProjectStructure
        {
            get
            {
                if (!_projectLoaded)
                    LoadProject();
                return _step7ProjectStructure;
            }
            set { _step7ProjectStructure = value; }
        }
        */

        private List<CPUFolder> _cpuFolders;
        public List<CPUFolder> CPUFolders
        {
            get
            {
                if (!_projectLoaded)
                    LoadProject();
                return _cpuFolders;
            }
            set { _cpuFolders = value; }
        }

        private List<CPFolder> _cpFolders;
        public List<CPFolder> CPFolders
        {
            get
            {
                if (!_projectLoaded)
                    LoadProject();
                return _cpFolders;
            }
            set { _cpFolders = value; }
        }

        private List<S7ProgrammFolder> _s7ProgrammFolders;
        public List<S7ProgrammFolder> S7ProgrammFolders
        {
            get
            {
                if (!_projectLoaded)
                    LoadProject();
                return _s7ProgrammFolders;
            }
            set { _s7ProgrammFolders = value; }
        }

        private List<BlocksOfflineFolder> _blocksOfflineFolders;
        public List<BlocksOfflineFolder> BlocksOfflineFolders
        {
            get
            {
                if (!_projectLoaded)
                    LoadProject();
                return _blocksOfflineFolders;
            }
            set { _blocksOfflineFolders = value; }
        }

        public override ProjectType ProjectType
        {
            get { return ProjectType.Step7; }
        }

        override protected void LoadProject()
        {
            _projectLoaded = true;

            ProjectStructure = new Step7ProjectFolder() { Project = this };
            CPUFolders = new List<CPUFolder>();
            CPFolders = new List<CPFolder>();
            S7ProgrammFolders = new List<S7ProgrammFolder>();
            BlocksOfflineFolders = new List<BlocksOfflineFolder>();

            ProjectStructure.Name = this.ToString();

            var stations = new List<StationConfigurationFolder>();

            List<CPFolder> DPFolders = new List<CPFolder>();//ProfiBusDP and MPI
            //Get The Project Stations...
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hstatx" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hstatx" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        if ((int)row["OBJTYP"] == objectType_Simatic300 || (int)row["OBJTYP"] == objectType_Simatic400 || (int)row["OBJTYP"] == objectType_Simatic400H)
                        {
                            var x = new StationConfigurationFolder() { Project = this, Parent = ProjectStructure };
                            x.Name = ((string)row["Name"]).Replace("\0", "");
                            if ((bool)row["DELETED_FLAG"]) x.Name = "$$_" + x.Name;
                            x.ID = (int)row["ID"];
                            x.UnitID = (int)row["UNITID"];
                            x.ObjTyp = (int)row["OBJTYP"];
                            switch ((int)row["OBJTYP"])
                            {
                                case objectType_Simatic300:
                                    x.StationType = PLCType.Simatic300;
                                    break;
                                case objectType_Simatic400:
                                    x.StationType = PLCType.Simatic400;
                                    break;
                                case objectType_Simatic400H:
                                    x.StationType = PLCType.Simatic400H;
                                    break;
                            }
                            x.Parent = ProjectStructure;
                            ProjectStructure.SubItems.Add(x);
                            stations.Add(x);
                            _allFolders.Add(x);
                        }
                        else if ( Convert.ToInt32(row["OBJTYP"])==objectType_MpiDPinCPU)
                        {
                            var dp = new CPFolder();
                            dp.UnitID = Convert.ToInt32(row["UNITID"]);//is UNITID in CPUFolder
                            dp.ID = Convert.ToInt32(row["ID"]);
                            DPFolders.Add(dp);
                            _allFolders.Add(dp);
                        }
                    }
                }
            }
            
            //Get The HW Folder for the Station...
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hstatx" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hstatx" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        if (Convert.ToInt32(row["RELID"]) == 1315820)
                        {
                            int TobjType = Convert.ToInt32(row["TOBJTYP"]);

                            if (TobjType == objectType_MpiDP400 || TobjType == objectType_MpiDP300)
                            {
                                var dp = DPFolders.FirstOrDefault(x => x.ID == Convert.ToInt32(row["SOBJID"]));
                                if (dp != null)
                                {
                                    if (dp.IdTobjId == null) dp.IdTobjId = new List<int>();
                                    dp.IdTobjId.Add(Convert.ToInt32(row["TOBJID"]));
                                }
                            }
                        }
                    }
                }
            }
            
            /*
            //Get The HW Folder for the Station...
            if (ZipHelper.FileExists(_zipfile,ProjectFolder + "hOmSave7" + _DirSeperator + "s7hstatx" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hstatx" + _DirSeperator + "HRELATI1.DBF", _zipfile, _DirSeperator);
                foreach (var y in Step7ProjectStructure.SubItems)
                {
                    if (y.GetType() == typeof (StationConfigurationFolder))
                    {
                        var z = (StationConfigurationFolder) y;
                        foreach (DataRow row in dbfTbl.Rows)
                        {
                            if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                            {
                                if ((int)row["SOBJID"] == z.ID && (int)row["RELID"] == 1315838)
                                {
                                    var x = new CPUFolder() {Project = this};
                                    x.UnitID = Convert.ToInt32(row["TUNITID"]);
                                    x.TobjTyp = Convert.ToInt32(row["TOBJTYP"]);
                                    x.CpuType = z.StationType;
                                    bool add = true;
                                    foreach (Step7ProjectFolder tmp in z.SubItems)
                                    {
                                        if (tmp.GetType() == typeof (CPUFolder) && ((CPUFolder) tmp).UnitID == x.UnitID)
                                            add = false;
                                    }
                                    if (add)
                                    {
                                        x.Parent = z;
                                        z.SubItems.Add(x);
                                        CPUFolders.Add(x);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            */


            //Get The CPs...
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7wb53ax" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7wb53ax" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        var cp = new CPFolder() { Project = this };
                        cp.ID = Convert.ToInt32(row["ID"]);
                        cp.UnitID = Convert.ToInt32(row["UNITID"]);
                        cp.TobjTyp = Convert.ToInt32(row["OBJTYP"]);
                        cp.Name = ((string)row["Name"]).Replace("\0", "");
                        if (Convert.ToBoolean(row["DELETED_FLAG"])) cp.Name = "$$_" + cp.Name;
                        cp.Rack = Convert.ToInt32(row["SUBSTATN"]);
                        cp.Slot = Convert.ToInt32(row["MODULN"]);
                        cp.SubModulNumber = Convert.ToInt32(row["SUBMODN"]);
                        CPFolders.Add(cp);
                        _allFolders.Add(cp);
                    }
                }
            }
            //add subitem to parent
            foreach(var cp in CPFolders.Where(x=>x.SubModulNumber > 0))
            {
                var parent = CPFolders.FirstOrDefault(x => x.ID == cp.UnitID);
                if (parent != null) parent.SubModul = cp;
            }

            //Get The CP Folders
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7wb53ax" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7wb53ax" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);
                foreach(DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        int relID = Convert.ToInt32(row["RELID"]);
                        if (relID == 1315827)
                        {
                            var cpu = (StationConfigurationFolder)ProjectStructure.SubItems.FirstOrDefault(x => x.ID == Convert.ToInt32(row["TUNITID"]));
                            var cp = CPFolders.FirstOrDefault(x => x.ID == Convert.ToInt32(row["SOBJID"]));
                            if (cpu != null && cp != null) cpu.SubItems.Add(cp);
                        }
                        else if (relID == 64)
                        {
                            var cp = CPFolders.FirstOrDefault(x => x.ID == Convert.ToInt32(row["SOBJID"]));
                            if (cp != null)
                            {
                                if (cp.TobjId == null) cp.TobjId = new List<int>();
                                cp.TobjId.Add(Convert.ToInt32(row["TOBJID"]));
                            }
                        }
                    }
                }
            }

            //Get The CPU 300 Folders
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk31ax" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk31ax" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);
                foreach (var y in ProjectStructure.SubItems)
                {
                    if (y.GetType() == typeof(StationConfigurationFolder))
                    {
                        var z = (StationConfigurationFolder)y;
                        foreach (DataRow row in dbfTbl.Rows)
                        {
                            if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                            {
                                if ((int)row["TUNITID"] == z.ID && (int)row["TOBJTYP"] == 1314972)
                                //((int)row["TUNITTYP"] == 1314969 || (int)row["TUNITTYP"] == 1314969 || (int)row["TUNITTYP"] == 1314969))
                                {
                                    var x = new CPUFolder() { Project = this };
                                    x.UnitID = Convert.ToInt32(row["TUNITID"]);
                                    x.TobjTyp = Convert.ToInt32(row["TOBJTYP"]);
                                    x.CpuType = z.StationType;
                                    x.ID = Convert.ToInt32(row["SOBJID"]);
                                    x.Parent = z;
                                    z.SubItems.Add(x);
                                    CPUFolders.Add(x);
                                    _allFolders.Add(x);
                                }
                            }
                        }
                    }
                }
            }

            //Get The CPU 300 ET200s Folders
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hkcomx" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hkcomx" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);
                foreach (var y in ProjectStructure.SubItems)
                {
                    if (y.GetType() == typeof(StationConfigurationFolder))
                    {
                        var z = (StationConfigurationFolder)y;
                        foreach (DataRow row in dbfTbl.Rows)
                        {
                            if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                            {
                                if ((int)row["TUNITID"] == z.ID && (int)row["TOBJTYP"] == 1314972)
                                //((int)row["TUNITTYP"] == 1314969 || (int)row["TUNITTYP"] == 1314969 || (int)row["TUNITTYP"] == 1314969))
                                {
                                    var x = new CPUFolder() { Project = this };
                                    x.UnitID = Convert.ToInt32(row["TUNITID"]);
                                    x.TobjTyp = Convert.ToInt32(row["TOBJTYP"]);
                                    x.CpuType = z.StationType;
                                    x.ID = Convert.ToInt32(row["SOBJID"]);
                                    x.CpuType = PLCType.SimaticET200;
                                    x.Parent = z;
                                    z.SubItems.Add(x);
                                    CPUFolders.Add(x);
                                    _allFolders.Add(x);
                                }
                            }
                        }
                    }
                }
            }
            //Get The CPU 400 Folders
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk41ax" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk41ax" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);

                foreach (var y in ProjectStructure.SubItems)
                {
                    if (y.GetType() == typeof(StationConfigurationFolder))
                    {
                        var z = (StationConfigurationFolder)y;
                        foreach (DataRow row in dbfTbl.Rows)
                        {
                            if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                            {
                                if ((int)row["TUNITID"] == z.ID && ((int)row["TOBJTYP"] == 1314972 || (int)row["TOBJTYP"] == 1315656 /* BackupCPU bei H Sys */))
                                //((int)row["TUNITTYP"] == 1314969 || (int)row["TUNITTYP"] == 1314969 || (int)row["TUNITTYP"] == 1314969))
                                {
                                    var x = new CPUFolder() { Project = this };
                                    x.UnitID = Convert.ToInt32(row["TUNITID"]);
                                    x.TobjTyp = Convert.ToInt32(row["TOBJTYP"]);
                                    x.CpuType = z.StationType;
                                    x.ID = Convert.ToInt32(row["SOBJID"]);
                                    x.Parent = z;
                                    z.SubItems.Add(x);
                                    CPUFolders.Add(x);
                                    _allFolders.Add(x);
                                }
                            }
                        }
                    }
                }
            }

            //Get The CPU(ET200S)...
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hkcomx" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hkcomx" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);

                foreach (var y in CPUFolders)
                {
                    foreach (DataRow row in dbfTbl.Rows)
                    {
                        if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                        {
                            if ((int)row["ID"] == y.ID && y.CpuType == PLCType.SimaticET200)
                            //if ((int)row["UNITID"] == y.UnitID && y.CpuType == PLCType.SimaticET200)
                            {
                                y.Name = ((string)row["Name"]).Replace("\0", "");
                                if ((bool)row["DELETED_FLAG"]) y.Name = "$$_" + y.Name;
                                y.ID = (int)row["ID"];

                                y.Rack = (int)row["SUBSTATN"];
                                y.Slot = (int)row["MODULN"];
                            }
                        }
                    }
                }
            }

            //Get The CPU(300)...
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk31ax" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk31ax" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);

                foreach (var y in CPUFolders)
                {
                    foreach (DataRow row in dbfTbl.Rows)
                    {
                        if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                        {
                            if ((int)row["ID"] == y.ID && y.CpuType == PLCType.Simatic300)
                            //if ((int)row["UNITID"] == y.UnitID && y.CpuType == PLCType.Simatic300)
                            {
                                y.Name = ((string)row["Name"]).Replace("\0", "");
                                if ((bool)row["DELETED_FLAG"]) y.Name = "$$_" + y.Name;
                                y.ID = (int)row["ID"];

                                y.Rack = (int)row["SUBSTATN"];
                                y.Slot = (int)row["MODULN"];
                            }
                        }
                    }
                }
            }

            //Get The CPU(300) password
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk31ax" + _DirSeperator + "HATTRME1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk31ax" + _DirSeperator + "HATTRME1.DBF", _ziphelper, _DirSeperator);
                byte[] memoarray = null;

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"])
                    {
                        if ((int)row["ATTRIIDM"] == 111142)
                        {
                            if (row["MEMOARRAYM"] != DBNull.Value)
                                memoarray = (byte[])row["MEMOARRAYM"];

                            if (memoarray.Length >= 12)
                            {
                                // memoarray[3] : level password (1-3)
                                byte[] mempass = new byte[8];
                                for (int i = 0; i < 8; i++)
                                {
                                    if (i < 2) mempass[i] = (byte)(memoarray[i + 4] ^ 0xAA);
                                    else mempass[i] = (byte)(memoarray[i + 2] ^ memoarray[i + 4] ^ 0xAA);
                                }
                                string res = ProjectEncoding.GetString(mempass);
                                foreach (var y in CPUFolders)
                                {
                                    if ((int)row["IDM"] == y.ID)
                                    {
                                        y.PasswdHard = res.Trim();
                                    }
                                }
                            }
                        }
                    }
                }
            }


            //Get The CPU(400)...
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk41ax" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk41ax" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);

                foreach (var y in CPUFolders)
                {
                    foreach (DataRow row in dbfTbl.Rows)
                    {
                        if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                        {
                            if ((int)row["ID"] == y.ID && (y.CpuType == PLCType.Simatic400 || y.CpuType == PLCType.Simatic400H))
                            //if ((int)row["UNITID"] == y.UnitID && (y.CpuType == PLCType.Simatic400 || y.CpuType == PLCType.Simatic400H) )
                            {
                                y.Name = ((string)row["Name"]).Replace("\0", "");
                                if ((bool)row["DELETED_FLAG"]) y.Name = "$$_" + y.Name;
                                y.ID = (int)row["ID"];

                                y.Rack = (int)row["SUBSTATN"];
                                y.Slot = (int)row["MODULN"];
                            }
                        }
                    }
                }
            }

            //Get The CPU(400) password
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk41ax" + _DirSeperator + "HATTRME1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk41ax" + _DirSeperator + "HATTRME1.DBF", _ziphelper, _DirSeperator);
                byte[] memoarray = null;

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"])
                    {
                        if ((int)row["ATTRIIDM"] == 111142)
                        {
                            if (row["MEMOARRAYM"] != DBNull.Value)
                                memoarray = (byte[])row["MEMOARRAYM"];
                            if (memoarray.Length >= 12)
                            {
                                // memoarray[3] : level password (1-3)
                                byte[] mempass = new byte[8];
                                for (int i = 0; i < 8; i++)
                                {
                                    if (i < 2) mempass[i] = (byte)(memoarray[i + 4] ^ 0xAA);
                                    else mempass[i] = (byte)(memoarray[i + 2] ^ memoarray[i + 4] ^ 0xAA);
                                }
                                string res = ProjectEncoding.GetString(mempass);
                                foreach (var y in CPUFolders)
                                {
                                    if ((int)row["IDM"] == y.ID)
                                    {
                                        y.PasswdHard = res.Trim();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var tmpS7ProgrammFolders = new List<S7ProgrammFolder>();
            //Get all Program Folders
            if (_ziphelper.FileExists(ProjectFolder + "hrs" + _DirSeperator + "S7RESOFF.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hrs" + _DirSeperator + "S7RESOFF.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        var x = new S7ProgrammFolder() { Project = this };
                        x.Name = ((string)row["Name"]).Replace("\0", "");
                        if ((bool)row["DELETED_FLAG"]) x.Name = "$$_" + x.Name;
                        x.ID = (int)row["ID"];
                        x._linkfileoffset = (int)row["RSRVD4_L"];
                        S7ProgrammFolders.Add(x);
                        tmpS7ProgrammFolders.Add(x);
                        _allFolders.Add(x);
                    }
                }
            }

            //Combine Folder and CPU (300)
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk31ax" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk31ax" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {

                        if ((int)row["RELID"] == 16)
                        {
                            int cpuid = (int)row["SOBJID"];
                            int fldid = (int)row["TOBJID"];
                            foreach (var y in CPUFolders)
                            {
                                if (y.ID == cpuid && y.CpuType == PLCType.Simatic300)
                                {
                                    foreach (var z in S7ProgrammFolders)
                                    {
                                        if (z.ID == fldid)
                                        {
                                            z.Parent = y;
                                            y.SubItems.Add(z);
                                            tmpS7ProgrammFolders.Remove(z);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Combine Folder and CPU (300 ET200s)
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hkcomx" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hkcomx" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {

                        if ((int)row["RELID"] == 16)
                        {
                            int cpuid = (int)row["SOBJID"];
                            int fldid = (int)row["TOBJID"];
                            foreach (var y in CPUFolders)
                            {
                                if (y.ID == cpuid && y.CpuType == PLCType.SimaticET200)
                                {
                                    foreach (var z in S7ProgrammFolders)
                                    {
                                        if (z.ID == fldid)
                                        {
                                            z.Parent = y;
                                            y.SubItems.Add(z);
                                            tmpS7ProgrammFolders.Remove(z);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Combine Folder and CPU (400)
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk41ax" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk41ax" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {

                        if ((int)row["RELID"] == 16)
                        {
                            int cpuid = (int)row["SOBJID"];
                            int fldid = (int)row["TOBJID"];
                            foreach (var y in CPUFolders)
                            {
                                if (y.ID == cpuid && (y.CpuType == PLCType.Simatic400 || y.CpuType == PLCType.Simatic400H))
                                {
                                    foreach (var z in S7ProgrammFolders)
                                    {
                                        if (z.ID == fldid)
                                        {
                                            z.Parent = y;
                                            y.SubItems.Add(z);
                                            tmpS7ProgrammFolders.Remove(z);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Add the BlockFolders without CPU to the Ground project
            foreach (var z in tmpS7ProgrammFolders)
            {
                z.Parent = ProjectStructure;
                ProjectStructure.SubItems.Add(z);
            }

            //Get Symbol Tables
            foreach (var z in S7ProgrammFolders)
            {
                var symtab = _GetSymTabForProject(z, this._showDeleted);
                if (symtab != null)
                {
                    symtab.Parent = z;
                    z.SymbolTable = symtab;
                    z.SubItems.Add(symtab);
                    _allFolders.Add(symtab);
                }
            }

            var tmpBlocksOfflineFolders = new List<BlocksOfflineFolder>();
            //Create the Programm Block folders...
            if (_ziphelper.FileExists(ProjectFolder + "ombstx" + _DirSeperator + "offline" + _DirSeperator + "BSTCNTOF.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "ombstx" + _DirSeperator + "offline" + _DirSeperator + "BSTCNTOF.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        var x = new BlocksOfflineFolder() { Project = this };
                        x.Name = ((string)row["Name"]).Replace("\0", "");
                        if ((bool)row["DELETED_FLAG"]) x.Name = "$$_" + x.Name;
                        x.ID = (int)row["ID"];
                        x.Folder = ProjectFolder + "ombstx" + _DirSeperator + "offline" + _DirSeperator + x.ID.ToString("X").PadLeft(8, '0') + _DirSeperator;
                        tmpBlocksOfflineFolders.Add(x);
                        _blocksOfflineFolders.Add(x);
                        _allFolders.Add(x);
                    }
                }
            }

            var Step7ProjectTypeStep7Sources = new List<SourceFolder>();
            //Create the Source Block folders...
            if (_ziphelper.FileExists(ProjectFolder + "s7asrcom" + _DirSeperator + "S7CNTREF.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "s7asrcom" + _DirSeperator + "S7CNTREF.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        var x = new SourceFolder() { Project = this };
                        x.Name = ((string)row["Name"]).Replace("\0", "");
                        if ((bool)row["DELETED_FLAG"]) x.Name = "$$_" + x.Name;
                        x.ID = (int)row["ID"];
                        x.Folder = ProjectFolder + "s7asrcom" + _DirSeperator + x.ID.ToString("X").PadLeft(8, '0') + _DirSeperator;
                        Step7ProjectTypeStep7Sources.Add(x);
                        _allFolders.Add(x);
                    }
                }
            }

            var pbMasterSystems = new List<ProfibusMasterSystem>();

            //Get all Profibus Master Systems
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "S7HDPSSX" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "S7HDPSSX" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        if ((int)row["OBJTYP"] == 1314971)
                        {
                            var x = new ProfibusMasterSystem() { Project = this, Name = row["NAME"].ToString().Replace("\0", ""), Id = (int)row["ID"] };
                            pbMasterSystems.Add(x);
                            _allFolders.Add(x);
                        }
                    }
                }
            }

            //Link all PbMasterSystems to the Stations             
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "S7HDPSSX" + _DirSeperator + "HRELATI1.DBF"))
            {
                var lnkLst = new List<LinkHelp>();

                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "S7HDPSSX" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        lnkLst.Add(new LinkHelp() { SOBJID = (int)row["SOBJID"], SOBJTYP = (int)row["SOBJTYP"], RELID = (int)row["RELID"], TOBJID = (int)row["TOBJID"], TOBJTYP = (int)row["TOBJTYP"], TUNITID = (int)row["TUNITID"], TUNITTYP = (int)row["TUNITTYP"] });
                    }
                }

                foreach (StationConfigurationFolder station in stations)
                {
                    var lnks = lnkLst.Where(x => x.TOBJTYP == station.ObjTyp && x.TOBJID == station.ID);
                    foreach (LinkHelp linkHelp in lnks)
                    {
                        var ms = pbMasterSystems.FirstOrDefault(x => x.Id == linkHelp.SOBJID);
                        if (ms != null)
                        {
                            station.MasterSystems.Add(ms);
                            station.SubItems.Add(ms);
                        }
                    }
                }
            }

            //Get all Profibus Parts            
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hslntx" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hslntx" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        if ((int)row["OBJTYP"] == 1314988)
                        {
                            var node = new ProfibusNode() { Name = row["NAME"].ToString().Replace("\0", ""), NodeId = (int)row["SUBSTATN"], GsdFile = row["CEXTTYPE"].ToString() };

                            var ma = pbMasterSystems.FirstOrDefault(x => x.Id == (int)row["UNITID"]);
                            if (ma != null)
                                ma.Children.Add(node);
                        }
                    }
                }
            }





            var pnMasterSystems = new List<ProfinetMasterSystem>();

            //Get all Profibus Master Systems
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hssiox" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hssiox" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        int objType = Convert.ToInt32(row["OBJTYP"]);
                        if (objType == 1316787)
                        {
                            var x = new ProfinetMasterSystem() { Project = this, Name = row["NAME"].ToString().Replace("\0", ""), Id = (int)row["ID"] };
                            pnMasterSystems.Add(x);
                            _allFolders.Add(x);
                        }
                        else if (objType == objectType_EternetInCPU3xx || objType == objectType_EternetInCPU4xx)
                        {
                            var cpu = CPUFolders.FirstOrDefault(x => x.ID == Convert.ToInt32(row["UNITID"]));
                            if (cpu != null) cpu.IdTobjId = Convert.ToInt32(row["ID"]);
                        }
                        else if ( objType == 2364971 || objType == 2367589 )
                        {
                            var cp = CPFolders.FirstOrDefault(x => x.ID == (int)row["UNITID"]);
                            if (cp != null)
                            {
                                if (cp.IdTobjId == null) cp.IdTobjId = new List<int>();
                                cp.IdTobjId.Add((int)row["ID"]);
                            }
                        }
                    }
                }
            }

            //Link all PnMasterSystems to the Stations             
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hssiox" + _DirSeperator + "HRELATI1.DBF"))
            {
                var lnkLst = new List<LinkHelp>();

                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hssiox" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        if ( Convert.ToInt32(row["RELID"]) == 64)
                        {
                            var cpu = CPUFolders.FirstOrDefault(x => x.IdTobjId == Convert.ToInt32(row["SOBJID"]));
                            if (cpu != null) cpu.TobjId = Convert.ToInt32(row["TOBJID"]);

                            var cp = CPFolders.FirstOrDefault(x => x.IdTobjId != null && x.IdTobjId.Any(c => c == (int)row["SOBJID"]));
                            if (cp != null)
                            {
                                if (cp.TobjId == null) cp.TobjId = new List<int>();
                                cp.TobjId.Add((int)row["TOBJID"]);
                            }
                        }

                        lnkLst.Add(new LinkHelp() { SOBJID = (int)row["SOBJID"], SOBJTYP = (int)row["SOBJTYP"], RELID = (int)row["RELID"], TOBJID = (int)row["TOBJID"], TOBJTYP = (int)row["TOBJTYP"], TUNITID = (int)row["TUNITID"], TUNITTYP = (int)row["TUNITTYP"] });
                    }
                }
                foreach (StationConfigurationFolder station in stations)
                {
                    var lnks = lnkLst.Where(x => x.TOBJTYP == station.ObjTyp && x.TOBJID == station.ID);
                    foreach (LinkHelp linkHelp in lnks)
                    {
                        var ms = pnMasterSystems.FirstOrDefault(x => x.Id == linkHelp.SOBJID);
                        if (ms != null)
                        {
                            station.MasterSystems.Add(ms);
                            station.SubItems.Add(ms);
                        }
                    }
                }
            }

            //Get all Profinet Parts ...     
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hdevnx" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var attLst = new List<AttrMeHelp>();

                //Read real name from hattrme.dbf          
                if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hdevnx" + _DirSeperator + "HATTRME1.DBF"))
                {
                    var dbfTbl2 = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hdevnx" + _DirSeperator + "HATTRME1.DBF", _ziphelper, _DirSeperator);

                    foreach (DataRow row in dbfTbl2.Rows)
                    {
                        if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                        {
                            attLst.Add(new AttrMeHelp() { IDM = (int)row["IDM"], ATTRIIDM = (int)row["ATTRIIDM"], ATTFORMATM = (int)row["ATTFORMATM"], MEMOARRAYM = (byte[])row["MEMOARRAYM"] });
                        }
                    }
                }

                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hdevnx" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        if ((int)row["OBJTYP"] == 1316803)
                        {
                            var node = new ProfibusNode() { Name = row["NAME"].ToString().Replace("\0", ""), NodeId = (int)row["SUBSTATN"], GsdFile = row["CEXTTYPE"].ToString() };

                            var inf = attLst.FirstOrDefault(x => x.IDM == (int)row["ID"] && x.ATTRIIDM == 111386);

                            if (inf != null)
                                node.Name = ProjectEncoding.GetString(inf.MEMOARRAYM).Replace("\0", "");

                            var ma = pnMasterSystems.FirstOrDefault(x => x.Id == (int)row["UNITID"]);
                            if (ma != null)
                                ma.Children.Add(node);
                        }
                    }
                }
            }


            //Infos about Link file hrs\linkhrs.lnk
            //Size of a Structure in the Link File: 512 bytes
            //Offset of Linkfile is in hrs\S7RESOFF.DBF, Filed 12 (RSRVD3_L)
            //after 0x04, 0x20, 0x11 follows the Step7ProjectBlockFolder ID (2 Bytes) or maybe the source folder id
            //after 0x01, 0x60, 0x11 follows the Step7Programm ID (2 Bytes)

            //Create the Link BlocksOfflineFolder Folder with S7ProgrammFolders...
            if (_ziphelper.FileExists(ProjectFolder + "hrs" + _DirSeperator + "linkhrs.lnk"))
            {

                //FileStream hrsLink = new FileStream(ProjectFolder + "hrs" + _DirSeperator + "linkhrs.lnk", FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite);
                Stream hrsLink = _ziphelper.GetReadStream(ProjectFolder + "hrs" + _DirSeperator + "linkhrs.lnk");
                BinaryReader rd = new BinaryReader(hrsLink);
                byte[] completeBuffer = rd.ReadBytes((int)_ziphelper.GetStreamLength(ProjectFolder + "hrs" + _DirSeperator + "linkhrs.lnk", hrsLink));
                rd.Close();
                hrsLink.Close();
                hrsLink = new MemoryStream(completeBuffer);

                foreach (var x in S7ProgrammFolders)
                {
                    byte[] tmpLink = new byte[0x200];
                    hrsLink.Position = x._linkfileoffset;
                    hrsLink.Read(tmpLink, 0, 0x200);

                    int pos1 = ASCIIEncoding.ASCII.GetString(tmpLink).IndexOf(ASCIIEncoding.ASCII.GetString(new byte[] { 0x01, 0x60, 0x11 }));
                    int wrt1 = tmpLink[pos1 + 3] * 0x100 + tmpLink[pos1 + 4];

                    int pos2 = ASCIIEncoding.ASCII.GetString(tmpLink).IndexOf(ASCIIEncoding.ASCII.GetString(new byte[] { 0x04, 0x20, 0x11 }));
                    int wrt2 = tmpLink[pos2 + 3] * 0x100 + tmpLink[pos2 + 4];

                    BlocksOfflineFolder fld = null;
                    foreach (var y in tmpBlocksOfflineFolders)
                    {
                        if (y.ID == wrt1)
                        {
                            y.Parent = x;
                            x.SubItems.Add(y);
                            x.BlocksOfflineFolder = y;
                            fld = y;
                            break;
                        }
                    }

                    if (fld != null)
                        tmpBlocksOfflineFolders.Remove(fld);

                    foreach (var y in Step7ProjectTypeStep7Sources)
                    {
                        if (y.ID == wrt2)
                        {
                            y.Parent = x;
                            x.SubItems.Add(y);
                            x.SourceFolder = y;
                        }
                    }

                }
                hrsLink.Close();
            }
            else
            {
                foreach (var y in tmpBlocksOfflineFolders)
                {
                    y.Parent = ProjectStructure;
                    ProjectStructure.SubItems.Add(y);
                }

                foreach (var y in Step7ProjectTypeStep7Sources)
                {
                    y.Parent = ProjectStructure;
                    ProjectStructure.SubItems.Add(y);
                }
            }

            if (_showDeleted)
            {
                foreach (var y in tmpBlocksOfflineFolders)
                {
                    var x = new S7ProgrammFolder() { Name = "$$tmpProgram_for_deleted" };
                    x.Project = this;
                    x.Parent = ProjectStructure;
                    y.Parent = x;
                    x.SubItems.Add(y);
                    x.BlocksOfflineFolder = y;
                    ProjectStructure.SubItems.Add(x);
                }
            }
            //Get The ProfiBus and MPI
            List<DpHelp> DPlist = new List<DpHelp>();
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hkdmax" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hkdmax" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        var dp = new DpHelp();
                        dp.id = Convert.ToInt32(row["ID"]);
                        DPlist.Add(dp);
                    }
                }
            }
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hkdmax" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hkdmax" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);

                var rrr = DPFolders;
                foreach (DataRow row in dbfTbl.Rows)
                {
                    int relID = Convert.ToInt32(row["RELID"]);
                    if ( relID == 1315837)
                    {
                        var dp = DPlist.FirstOrDefault(x => x.id == Convert.ToInt32(row["SOBJID"]));
                        if ( dp != null)
                        {
                            dp.TobjID = Convert.ToInt32(row["TOBJID"]);
                        }
                    }
                    else if (relID == 64)
                    {
                        var dp = DPlist.FirstOrDefault(x => x.id == Convert.ToInt32(row["SOBJID"]));
                        if (dp != null)
                        {
                            dp.addr = Convert.ToInt32(row["TOBJID"]);
                        }
                    }
                }
            }
            //remove DP from DPlist to DPFolder
            foreach(var dp in DPlist)
            {
                var dpf = DPFolders.FirstOrDefault(x => x.IdTobjId != null && x.IdTobjId.Any(y => y == dp.TobjID));
                if ( dpf != null)
                {
                    if (dpf.TobjId == null) dpf.TobjId = new List<int>();
                    dpf.TobjId.Add(dp.addr);
                }
            }
            //add subitem to parent
            //foreach (var cp in CPFolders.Where(x => x.SubModulNumber > 0))
            //{
            //    var parent = CPFolders.FirstOrDefault(x => x.ID == cp.UnitID);
            //    if (parent != null) parent.SubModul = cp;
            //}

            try {
                //read IP address from S7Netze\S7NONFGX.tab
                if (_ziphelper.FileExists(ProjectFolder + "S7Netze" + _DirSeperator + "S7NONFGX.tab"))
                {
                    Stream hrsLink = _ziphelper.GetReadStream(ProjectFolder + "S7Netze" + _DirSeperator + "S7NONFGX.tab");
                    BinaryReader rd = new BinaryReader(hrsLink);
                    int lengthFile = (int)_ziphelper.GetStreamLength(ProjectFolder + "S7Netze" + _DirSeperator + "S7NONFGX.tab", hrsLink);
                    byte[] completeBuffer = rd.ReadBytes(lengthFile);
                    rd.Close();
                    hrsLink.Close();

                    char[] searchValid = { 'A', 'd', 'd', 'r', 'e', 's', 's', 'I', 's', 'V', 'a', 'l', 'i', 'd' };
                    byte[] searchName = { (byte)'B', (byte)'a', (byte)'u', (byte)'g', (byte)'r', (byte)'u', (byte)'p', (byte)'p', (byte)'e', (byte)'n', (byte)'n', (byte)'a', (byte)'m', (byte)'e' };

                    byte[] startStructure = { 0x03, 0x52, 0x14, 0x00 };
                    byte[] startIP = { 0xE0, 0x0F, 0x00, 0x00, 0xE0, 0x0F, 0x00, 0x00 };
                    byte[] startMAC = { 0xA2, 0x0F, 0x00, 0x00, 0xA2, 0x0F, 0x00, 0x00 };
                    byte[] startMask = { 0xE5, 0x0F, 0x00, 0x00, 0xE5, 0x0F, 0x00, 0x00 };
                    byte[] startRouter = { 0xE3, 0x0F, 0x00, 0x00, 0xE3, 0x0F, 0x00, 0x00 };
                    byte[] startUseRouter = { 0xE8, 0x0F, 0x00, 0x00, 0xE8, 0x0F, 0x00, 0x00 };
                    byte[] startUseIP = { 0xEA, 0x0F, 0x00, 0x00, 0xEA, 0x0F, 0x00, 0x00 };
                    byte[] startUseMac = { 0xED, 0x0F, 0x00, 0x00, 0xED, 0x0F, 0x00, 0x00 };
                    int position = 0;
                    int lenStructure = 1705;// 1960;  //I don't think this len is correct... (look)
                    while ((position = indexOfByteArray(completeBuffer, startStructure, position + 1, lengthFile)) >= 0)
                    {
                        int number = BitConverter.ToInt32(completeBuffer, position + 4);//or ToInt16
                        //Debug.Print(number.ToString());
                        var cp = CPFolders.FirstOrDefault(x => x.TobjId != null && x.TobjId.Any(y => y == number));
                        var cpu = CPUFolders.FirstOrDefault(x => x.TobjId == number);
                        EthernetNetworkInterface ethernet = new EthernetNetworkInterface();
                        if (cp != null)
                        {
                            if (cp.NetworkInterfaces == null) cp.NetworkInterfaces = new List<NetworkInterface>();
                            cp.NetworkInterfaces.Add(ethernet);
                        }
                        else if (cpu != null)
                        {
                            if (cpu.NetworkInterfaces == null) cpu.NetworkInterfaces = new List<NetworkInterface>();
                            cpu.NetworkInterfaces.Add(ethernet);
                        }
                        else continue;


                        int pos = indexOfByteArray(completeBuffer, searchName, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                string strName = System.Text.Encoding.Default.GetString(completeBuffer, pos + 25, (int)completeBuffer[pos + 24]);

                                ethernet.Name = strName;
                            }
                            catch
                            { }
                        }

                        pos = indexOfByteArray(completeBuffer, startIP, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                string strIP = System.Text.Encoding.Default.GetString(completeBuffer, pos + 20, (int)completeBuffer[pos + 19]);
                                byte[] bIP = new byte[4];
                                bIP[0] = byte.Parse(strIP.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                bIP[1] = byte.Parse(strIP.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                bIP[2] = byte.Parse(strIP.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                bIP[3] = byte.Parse(strIP.Substring(6, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

                                ethernet.IpAddress = new System.Net.IPAddress(bIP);
                            }
                            catch
                            { }
                        }
                        pos = indexOfByteArray(completeBuffer, startMAC, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                string strMAC = System.Text.Encoding.Default.GetString(completeBuffer, pos + 20, (int)completeBuffer[pos + 19]);

                                ethernet.Mac = System.Net.NetworkInformation.PhysicalAddress.Parse(strMAC);
                            }
                            catch
                            { }
                        }
                        pos = indexOfByteArray(completeBuffer, startMask, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                string strMask = System.Text.Encoding.Default.GetString(completeBuffer, pos + 20, (int)completeBuffer[pos + 19]);
                                byte[] bIP = new byte[4];
                                bIP[0] = byte.Parse(strMask.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                bIP[1] = byte.Parse(strMask.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                bIP[2] = byte.Parse(strMask.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                bIP[3] = byte.Parse(strMask.Substring(6, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

                                ethernet.SubnetMask = new System.Net.IPAddress(bIP);
                            }
                            catch
                            { }
                        }
                        pos = indexOfByteArray(completeBuffer, startRouter, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                string strRouter = System.Text.Encoding.Default.GetString(completeBuffer, pos + 20, (int)completeBuffer[pos + 19]);
                                byte[] bIP = new byte[4];
                                bIP[0] = byte.Parse(strRouter.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                bIP[1] = byte.Parse(strRouter.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                bIP[2] = byte.Parse(strRouter.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                bIP[3] = byte.Parse(strRouter.Substring(6, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                ethernet.Router = new System.Net.IPAddress(bIP);
                            }
                            catch
                            { }
                        }
                        pos = indexOfByteArray(completeBuffer, startUseRouter, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                ethernet.UseRouter = Convert.ToBoolean(completeBuffer[pos + 19]);
                            }
                            catch
                            { }
                        }
                        pos = indexOfByteArray(completeBuffer, startUseIP, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                ethernet.UseIp = Convert.ToBoolean(completeBuffer[pos + 19]);
                            }
                            catch
                            { }
                        }
                        pos = indexOfByteArray(completeBuffer, startUseMac, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                ethernet.UseIso = Convert.ToBoolean(completeBuffer[pos + 19]);
                            }
                            catch
                            { }
                        }
                    }
                    //read ProfiBus Parametrs
                    startStructure[0] = 0x02;
                    byte[] startAddress = { 0x36, 0x08, 0x00, 0x00, 0x36, 0x08, 0x00, 0x00 };
                    lenStructure = 2000;
                    position = 0;
                    while ((position = indexOfByteArray(completeBuffer, startStructure, position + 1, lengthFile)) >= 0)
                    {
                        int number = BitConverter.ToInt32(completeBuffer, position + 4);//or ToInt16
                        var dp = DPFolders.FirstOrDefault(x => x.TobjId != null && x.TobjId.Any(y => y == number));
                        CPUFolder cpu = null;
                        if ( dp != null)
                            cpu = CPUFolders.FirstOrDefault(x => x.UnitID == dp.UnitID);
                        MpiProfiBusNetworkInterface MpiDP = new MpiProfiBusNetworkInterface() { NetworkInterfaceType = NetworkType.Profibus};
                        if (cpu != null)
                        {
                            if (cpu.NetworkInterfaces == null) cpu.NetworkInterfaces = new List<NetworkInterface>();
                            cpu.NetworkInterfaces.Add(MpiDP);
                        }
                        else continue;

                        int pos = indexOfByteArray(completeBuffer, startAddress, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                MpiDP.Address = (int)Convert.ToByte(completeBuffer[pos + 19 + (int)Convert.ToByte(completeBuffer[pos + 8])]);
                            }
                            catch
                            { }
                        }
                        pos = indexOfByteArray(completeBuffer, searchName, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                string strName = System.Text.Encoding.Default.GetString(completeBuffer, pos + 25, (int)completeBuffer[pos + 24]);

                                MpiDP.Name = strName;
                            }
                            catch
                            { }
                        }

                    }
                    //read MPI Parametrs
                    startStructure[0] = 0x01;
                    byte[] startMPIAddress = { 0x9A, 0x08, 0x00, 0x00, 0x9A, 0x08, 0x00, 0x00 };
                    lenStructure = 2000;
                    position = 0;
                    while ((position = indexOfByteArray(completeBuffer, startStructure, position + 1, lengthFile)) >= 0)
                    {
                        int number = BitConverter.ToInt32(completeBuffer, position + 4);//or ToInt16
                        var dp = DPFolders.FirstOrDefault(x => x.TobjId != null && x.TobjId.Any(y => y == number));
                        CPUFolder cpu = null;
                        if (dp != null)
                            cpu = CPUFolders.FirstOrDefault(x => x.UnitID == dp.UnitID);

                        MpiProfiBusNetworkInterface MpiDP = new MpiProfiBusNetworkInterface() { NetworkInterfaceType = NetworkType.Mpi };
                        if (cpu != null)
                        {
                            if (cpu.NetworkInterfaces == null) cpu.NetworkInterfaces = new List<NetworkInterface>();
                            cpu.NetworkInterfaces.Add(MpiDP);
                        }
                        else continue;

                        int pos = indexOfByteArray(completeBuffer, startMPIAddress, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                MpiDP.Address = (int)Convert.ToByte(completeBuffer[pos + 19 + (int)Convert.ToByte(completeBuffer[pos + 8])]);
                            }
                            catch
                            { }
                        }
                        pos = indexOfByteArray(completeBuffer, searchName, position, lenStructure);
                        if (pos > 0)
                        {
                            try
                            {
                                string strName = System.Text.Encoding.Default.GetString(completeBuffer, pos + 25, (int)completeBuffer[pos + 24]);

                                MpiDP.Name = strName;
                            }
                            catch
                            { }
                        }
                    }
                }
            }
            catch { }
            //union SubModul Cp
            bool repeat;
            do
            {
                repeat = false;
                foreach (var cp in CPFolders.Where(x => x.SubModul != null))
                {
                    if (cp.NetworkInterfaces == null) cp.NetworkInterfaces = new List<NetworkInterface>();
                    cp.NetworkInterfaces.AddRange(cp.SubModul.NetworkInterfaces);
                    CPFolders.Remove(cp.SubModul);
                    cp.SubModul = null;
                    repeat = true;
                    break;
                }
            } while (repeat);
        }
        private int indexOfByteArray(byte[] array, byte[] pattern, int offset, int maxLen)
        {
            int success = 0;
            int length = array.Length;
            for (int i = offset; i < length; i++)
            {
                if (array[i] == pattern[success])
                    success++;
                else if (success > 0)
                {
                    i--;
                    maxLen++;
                    success = 0;
                }
                if (pattern.Length == success)
                    return i - pattern.Length + 1;
                if (--maxLen == 0) return -1;
            }
            return -1;
        }

        private SymbolTable _GetSymTabForProject(S7ProgrammFolder myBlockFolder, bool showDeleted)
        {
            //string tmpId1 = "";

            var retVal = new SymbolTable() { Project = this };

            int tmpId2 = 0;

            //Look in Sym-LinkList for ID
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "YDBs" + _DirSeperator + "YLNKLIST.DBF", _ziphelper, _DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool) row["DELETED_FLAG"])
                    {
                        if ((int) row["TOI"] == myBlockFolder.ID)
                        {
                            tmpId2 = (int) row["SOI"];
                            break;
                        }
                    }
                }

                if (tmpId2 == 0 && showDeleted)
                    foreach (DataRow row in dbfTbl.Rows)
                    {
                        if ((int) row["TOI"] == myBlockFolder.ID)
                        {
                            tmpId2 = (int) row["SOI"];
                            retVal.Folder = ProjectFolder + "YDBs" + _DirSeperator + tmpId2.ToString() + _DirSeperator;
                            break;
                        }
                    }
            }

            //Look fro Symlist Name
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "YDBs" + _DirSeperator + "SYMLISTS.DBF", _ziphelper, _DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || showDeleted)
                    {
                        if ((int)row["_ID"] == tmpId2)
                        {
                            retVal.Name = (string)row["_UName"];
                            if ((bool)row["DELETED_FLAG"]) retVal.Name = "$$_" + retVal.Name;
                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(retVal.Name) && !File.Exists(ProjectFolder + "YDBs" + _DirSeperator + tmpId2.ToString() + _DirSeperator + "SYMLIST.DBF"))
                return null;

            retVal.showDeleted = showDeleted;
            if (tmpId2 != 0)
                retVal.Folder = ProjectFolder + "YDBs" + _DirSeperator + tmpId2.ToString() + _DirSeperator;
            
            return retVal;
        }


        private class LinkHelp
        {
            public int SOBJID { get; set; }
            public int SOBJTYP { get; set; }
            public int RELID { get; set; }
            public int TOBJID { get; set; }
            public int TOBJTYP { get; set; }
            public int TUNITID { get; set; }
            public int TUNITTYP { get; set; }
        }

        private class AttrMeHelp
        {
            public int IDM { get; set; }
            public int ATTRIIDM { get; set; }
            public int ATTFORMATM { get; set; }
            public byte[] MEMOARRAYM { get; set; }          
        }
        
        private class DpHelp
        {
            public int id;
            public int addr;
            public int TobjID;
        }
    }    
}
