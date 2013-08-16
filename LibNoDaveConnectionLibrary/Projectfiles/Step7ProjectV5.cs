using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public class Step7ProjectV5 : Project, IDisposable
    {
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

            ProjectEncoding = (prEn ?? Encoding.GetEncoding("ISO-8859-1")) ;

            if (projectfile.ToLower().EndsWith("zip"))
            {
                _projectfilename = ZipHelper.GetFirstZipEntryWithEnding(projectfile, ".s7p");

                if (string.IsNullOrEmpty(_projectfilename))
                    _projectfilename = ZipHelper.GetFirstZipEntryWithEnding(projectfile, ".s7l");

                if (string.IsNullOrEmpty(_projectfilename))
                    throw new Exception("Zip-File contains no valid Step7 Project !");
                this._ziphelper = new ZipHelper(projectfile);

            }
            LoadProjectHeader(projectfile, showDeleted);
        }

        private void LoadProjectHeader(string projectfile, bool showDeleted)
        {
            ProjectFile = projectfile;
            ProjectFolder = _projectfilename.Substring(0, _projectfilename.LastIndexOf(_DirSeperator)) + _DirSeperator;

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

        override internal void LoadProject()
        {
            _projectLoaded = true;

            ProjectStructure = new Step7ProjectFolder() {Project = this};
            CPUFolders = new List<CPUFolder>();
            CPFolders=new List<CPFolder>();
            S7ProgrammFolders = new List<S7ProgrammFolder>();
            BlocksOfflineFolders = new List<BlocksOfflineFolder>();

            ProjectStructure.Name = this.ToString();

            //Get The Project Stations...
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hstatx" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7hstatx" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                    {
                        if ((int)row["OBJTYP"] == 1314969 || (int)row["OBJTYP"] == 1314970 || (int)row["OBJTYP"] == 1315650)
                        {
                            var x = new StationConfigurationFolder() { Project = this, Parent = ProjectStructure};
                            x.Name = ((string)row["Name"]).Replace("\0", "");
                            if ((bool) row["DELETED_FLAG"]) x.Name = "$$_" + x.Name;
                            x.ID = (int) row["ID"];
                            x.UnitID = (int)row["UNITID"];
                            switch ((int)row["OBJTYP"])
                            {
                                case 1314969:
                                    x.StationType = PLCType.Simatic300;
                                    break;
                                case 1314970:
                                    x.StationType = PLCType.Simatic400;
                                    break;
                                case 1315650:
                                    x.StationType = PLCType.Simatic400H;
                                    break;
                            }
                            x.Parent = ProjectStructure;
                            ProjectStructure.SubItems.Add(x);
                        }
                    }
                }
            }


            //Get The CP Folders
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7wb53ax" + _DirSeperator + "HRELATI1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7wb53ax" + _DirSeperator + "HRELATI1.DBF", _ziphelper, _DirSeperator);
                foreach (var y in ProjectStructure.SubItems)
                {
                    if (y.GetType() == typeof(StationConfigurationFolder))
                    {
                        var z = (StationConfigurationFolder)y;
                        foreach (DataRow row in dbfTbl.Rows)
                        {
                            if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                            {
                                if ((int)row["TUNITID"] == z.ID &&
                                    ((int)row["TUNITTYP"] == 1314969 || (int)row["TUNITTYP"] == 1314970 || (int)row["TUNITTYP"] == 1315650))//as in "Get The Project Stations"
                                {
                                    var x = new CPFolder() {Project = this};
                                    x.UnitID = Convert.ToInt32(row["TUNITID"]);
                                    x.TobjTyp = Convert.ToInt32(row["TOBJTYP"]);
                                    x.ID = Convert.ToInt32(row["SOBJID"]);
                                    x.Parent = z;
                                    z.SubItems.Add(x);
                                    CPFolders.Add(x);

                                }
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
                                    var x = new CPUFolder() {Project = this};
                                    x.UnitID = Convert.ToInt32(row["TUNITID"]);
                                    x.TobjTyp = Convert.ToInt32(row["TOBJTYP"]);
                                    x.CpuType = z.StationType;
                                    x.ID = Convert.ToInt32(row["SOBJID"]);
                                    x.Parent = z;
                                    z.SubItems.Add(x);
                                    CPUFolders.Add(x);
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
                                    if (i < 2) mempass[i] = (byte) (memoarray[i + 4] ^ 0xAA);
                                    else mempass[i] = (byte) (memoarray[i + 2] ^ memoarray[i + 4] ^ 0xAA);
                                }
                                string res = ProjectEncoding.GetString(mempass);
                                foreach (var y in CPUFolders)
                                {
                                    if ((int) row["IDM"] == y.ID)
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

            //Get The CPs...
            if (_ziphelper.FileExists(ProjectFolder + "hOmSave7" + _DirSeperator + "s7wb53ax" + _DirSeperator + "HOBJECT1.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(ProjectFolder + "hOmSave7" + _DirSeperator + "s7wb53ax" + _DirSeperator + "HOBJECT1.DBF", _ziphelper, _DirSeperator);

                foreach (var y in CPFolders)
                {
                    foreach (DataRow row in dbfTbl.Rows)
                    {
                        if (!(bool)row["DELETED_FLAG"] || _showDeleted)
                        {
                            if ((int)row["ID"] == y.ID)
                            {
                                y.Name = ((string)row["Name"]).Replace("\0", "");
                                if ((bool)row["DELETED_FLAG"]) y.Name = "$$_" + y.Name;
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
                        x.Name = ((string)row["Name"]).Replace("\0","");
                        if ((bool) row["DELETED_FLAG"]) x.Name = "$$_" + x.Name;
                        x.ID = (int) row["ID"];
                        x._linkfileoffset = (int) row["RSRVD4_L"];
                        S7ProgrammFolders.Add(x);
                        tmpS7ProgrammFolders.Add(x);
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
                        x.ID = (int) row["ID"];
                        x.Folder = ProjectFolder + "ombstx" + _DirSeperator + "offline" + _DirSeperator + x.ID.ToString("X").PadLeft(8, '0') + _DirSeperator;
                        tmpBlocksOfflineFolders.Add(x);
                        _blocksOfflineFolders.Add(x);
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
                        x.Name = ((string)row["Name"]).Replace("\0","");
                        if ((bool)row["DELETED_FLAG"]) x.Name = "$$_" + x.Name;
                        x.ID = (int)row["ID"];
                        x.Folder = ProjectFolder + "s7asrcom" + _DirSeperator + x.ID.ToString("X").PadLeft(8, '0') + _DirSeperator;
                        Step7ProjectTypeStep7Sources.Add(x);
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

                    foreach (var y in tmpBlocksOfflineFolders)
                    {
                        if (y.ID == wrt1)
                        {
                            y.Parent = x;
                            x.SubItems.Add(y);
                            x.BlocksOfflineFolder = y;
                        }
                    }

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

        }

        private SymbolTable _GetSymTabForProject(S7ProgrammFolder myBlockFolder, bool showDeleted)
        {
            string tmpId1 = "";

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
    }
}
