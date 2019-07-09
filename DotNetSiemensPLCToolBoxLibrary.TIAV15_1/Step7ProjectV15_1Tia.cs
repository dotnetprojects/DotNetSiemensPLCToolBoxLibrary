using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml.Linq;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using Siemens.Engineering;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Types;
using Siemens.Engineering.SW.Tags;
using DotNetSiemensPLCToolBoxLibrary.General;
using System.Text.RegularExpressions;
using Siemens.Engineering.Compiler;
using Siemens.Engineering.SW.WatchAndForceTables;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.V15_1
{
    public interface ITiaProjectBlockInfo : IProjectBlockInfo
    {
    }

    public partial class Step7ProjectV15_1
    {

        private Siemens.Engineering.TiaPortal tiaPortal;

        private Siemens.Engineering.Project tiapProject;

        public virtual void Dispose()
        {
            tiaPortal.Dispose();
        }


        public class TIAOpennessProjectFolder : ProjectFolder
        {
            internal string InstID { get; private set; }

            protected Step7ProjectV15_1 TiaProject;

            public override string Name { get; set; }

            public TIAOpennessProjectFolder(Step7ProjectV15_1 Project)
            {
                this.Project = Project;
                this.TiaProject = Project;
            }

            public virtual void ImportFile(FileInfo file, bool overwrite, bool importFromSource)
            { }

            public virtual void CompileBlocks()
            { }
        }

        public class TIAOpennessProjectBlockInfo : ProjectBlockInfo, ITiaProjectBlockInfo
        {
            public override bool IsInstance
            {
                get
                {
                    return this.plcBlock.ToString().Contains("InstanceDB");
                }
            }

            internal TIAOpennessProjectBlockInfo(PlcBlock plcBlock)
            {
                this.plcBlock = plcBlock;
            }
            PlcBlock plcBlock;

            internal PLCLanguage SetBlockLanguage;
            public override PLCLanguage BlockLanguage { get { return SetBlockLanguage; } }

            public int BlockNumber { get; set; }

            public string BlockName
            {
                get
                {
                    string retVal = BlockType.ToString().Replace("S5_", "") + BlockNumber.ToString();
                    return retVal;
                }
            }

            public string ProgrammingLanguage
            {
                get { return plcBlock.ProgrammingLanguage.ToString(); }
            }

            public override string ToString()
            {
                string retVal = "";
                if (Deleted)
                    retVal += "$$_";
                if (Name != null)
                    retVal += BlockName + " (" + Name + ")";
                else
                    retVal += BlockName;
                return retVal;
            }

            public override string Export(ExportFormat exportFormat)
            {
                var ext = "xml";
                if (exportFormat != ExportFormat.Xml)
                {
                    if (this.plcBlock.ProgrammingLanguage == Siemens.Engineering.SW.Blocks.ProgrammingLanguage.SCL)
                    {
                        ext = "scl";
                    }
                    else if (this.plcBlock.ProgrammingLanguage == Siemens.Engineering.SW.Blocks.ProgrammingLanguage.STL || this.plcBlock.ProgrammingLanguage == Siemens.Engineering.SW.Blocks.ProgrammingLanguage.F_STL)
                    {
                        ext = "awl";
                    }
                    else if (this.plcBlock.ProgrammingLanguage == Siemens.Engineering.SW.Blocks.ProgrammingLanguage.DB || this.plcBlock.ProgrammingLanguage == Siemens.Engineering.SW.Blocks.ProgrammingLanguage.F_DB)
                    {
                        ext = "db";
                    }
                }

                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + "." + ext);
                if (ext == "xml")
                {
                    plcBlock.Export(new FileInfo(file), ExportOptions.WithDefaults | ExportOptions.WithReadOnly);
                }
                else
                {
                    var fld = this.ParentFolder;
                    while (!(fld is TIAOpennessControllerFolder))
                    {
                        fld = fld.Parent;
                    }
                     ((TIAOpennessControllerFolder)fld).plcSoftware.ExternalSourceGroup.GenerateSource(new[] { this.plcBlock }, new FileInfo(file), Siemens.Engineering.SW.ExternalSources.GenerateOptions.None);
                }
                var text = File.ReadAllText(file);
                File.Delete(file);

                return text;
            }
            private PLCBlockType? _plcBlockType;

            public override PLCBlockType BlockType
            {
                get
                {
                    if (_plcBlockType == null)
                    {
                        _plcBlockType = PLCBlockType.FC;
                        if (plcBlock is FB)
                        {
                            _plcBlockType = PLCBlockType.FB;
                        }
                        else if (plcBlock is OB)
                        {
                            _plcBlockType = PLCBlockType.OB;
                        }
                    }
                    return _plcBlockType.Value;
                }
                set { _plcBlockType = value; }
            }

            private string xml;
        }

        public class TIAOpennessProjectDataTypeInfo : ProjectBlockInfo, ITiaProjectBlockInfo
        {
            internal TIAOpennessProjectDataTypeInfo(PlcType plcType)
            {
                this.plcType = plcType;
            }
            private PlcType plcType;

            public override string ToString()
            {
                return Name;
            }

            public override PLCBlockType BlockType
            {
                get
                {
                    return PLCBlockType.UDT;
                }
                set { }
            }

            public override PLCLanguage BlockLanguage
            {
                get
                {
                    return PLCLanguage.DB;
                }
            }

            public override string Export(ExportFormat exportFormat)
            {
                var ext = "udt";
                if (exportFormat == ExportFormat.Xml)
                {
                    ext = "xml";
                }

                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + "." + ext);
                if (ext == "xml")
                {
                    plcType.Export(new FileInfo(file), Siemens.Engineering.ExportOptions.WithDefaults | ExportOptions.WithReadOnly);
                }
                else
                {
                    var fld = this.ParentFolder;
                    while (!(fld is TIAOpennessControllerFolder))
                    {
                        fld = fld.Parent;
                    }
                     ((TIAOpennessControllerFolder)fld).plcSoftware.ExternalSourceGroup.GenerateSource(new[] { this.plcType }, new FileInfo(file), Siemens.Engineering.SW.ExternalSources.GenerateOptions.None);
                }
                var text = File.ReadAllText(file);
                File.Delete(file);

                return text;
            }
        }

        public class TIAOpennessTagTable : ITIAVarTab
        {
            public string Name { get; set; }

            public List<ITIAConstant> Constants { get; set; }

            public List<ITIATag> Tags { get; internal set; }

            internal PlcTagTable PlcTagTable { get; set; }

            public virtual string Export(ExportFormat exportFormat)
            {
                var ext = "xml";
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + "." + ext);
                if (ext == "xml")
                {
                    PlcTagTable.Export(new FileInfo(file), Siemens.Engineering.ExportOptions.None);
                }
                var text = File.ReadAllText(file);
                File.Delete(file);

                return text;
            }

            public string Export()
            {
                return Export(ExportFormat.Xml);
            }
        }

        public class TIAOpennessTag : ITIATag
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string DataTypeName { get; set; }
            public List<TIAOpennessComment> Comments { get; set; }
            public bool IsExternalAccessible { get; set; }
            public bool IsExternalVisible { get; set; }

            internal TIAOpennessTag(PlcTag source)
            {
                Name = source.Name;
                Address = source.LogicalAddress;
                DataTypeName = source.DataTypeName;
                Comments = source.Comment.Items
                    .Select(c => new TIAOpennessComment() { Culture = c.Language.Culture, Text = c.Text }).ToList();
                IsExternalAccessible = source.ExternalAccessible;
                IsExternalVisible = source.ExternalVisible;
            }
        }

        public class TIAOpennessComment
        {
            public object Culture { get; internal set; }
            public string Text { get; internal set; }
        }

        public class TIAOpennessConstant : ITIAConstant
        {
            private readonly PlcUserConstant controllerConstant;

            internal TIAOpennessConstant(PlcUserConstant controllerConstant)
            {
                this.controllerConstant = controllerConstant;
            }

            public string Name { get; set; }

            private Regex _rgx = new Regex("<Value>(.*)</Value>", RegexOptions.Compiled);

            public object Value
            {
                get
                {
                    var tp = this.controllerConstant.DataTypeName;
                    var strg = ExportToString();

                    var m = _rgx.Match(strg).Groups[1].Value;
                    if (tp == "Int")
                        return int.Parse(m);
                    return m;
                }
            }

            private string ExportToString()
            {
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + ".tmp");
                controllerConstant.Export(new FileInfo(file), Siemens.Engineering.ExportOptions.None);

                var text = File.ReadAllText(file);
                File.Delete(file);

                return text;
            }
        }

        public class TIAOpennessControllerFolder : TIAOpennessProjectFolder, IRootProgrammFolder
        {
            internal TIAOpennessControllerFolder(Step7ProjectV15_1 Project, PlcSoftware plcSoftware)
                : base(Project)
            {
                this.Project = Project;
                this.TiaProject = Project;
                this.plcSoftware = plcSoftware;
            }

            //public override void ImportFile(FileInfo file, bool overwrite, bool importFromSource)
            //{
            //    plcSoftware.BlockGroup.Blocks.Import(file, overwrite ? ImportOptions.Override : ImportOptions.None);
            //}

            internal PlcSoftware plcSoftware;

            public TIAOpennessProgramFolder ProgramFolder { get; set; }
            public TIAOpennessPlcDatatypeFolder PlcDatatypeFolder { get; set; }
            public TIAOpennessVariablesFolder VarTabFolder { get; set; }
            public TIAOpennessWatchAndForceTablesFolder WatchAndForceTablesFolder { get; set; }

            public Block GetBlockRecursive(string name)
            {
                var block = GetBlockRecursive(ProgramFolder, name);
                if (block == null)
                {
                    block = GetBlockRecursive(PlcDatatypeFolder, name);
                }

                return block;
            }

            private Block GetBlockRecursive(TIAOpennessProgramFolder folder, string name)
            {
                var block = folder.GetBlock(name);
                if (block == null)
                {
                    foreach (TIAOpennessProgramFolder projectFolder in folder.SubItems)
                    {
                        block = GetBlockRecursive(projectFolder, name);
                        if (block != null)
                            return block;
                    }
                }

                return block;
            }

            private Block GetBlockRecursive(TIAOpennessPlcDatatypeFolder folder, string name)
            {
                var block = folder.GetBlock(name);
                if (block == null)
                {
                    foreach (TIAOpennessPlcDatatypeFolder projectFolder in folder.SubItems)
                    {
                        block = GetBlockRecursive(projectFolder, name);
                        if (block != null)
                            return block;
                    }
                }

                return block;
            }
        }

        public class TIAOpennessVariablesFolder : TIAOpennessProjectFolder, ITIAVarTabFolder
        {
            public TIAOpennessControllerFolder ControllerFolder { get; set; }

            private PlcTagTableGroup group;

            public TIAOpennessVariablesFolder(Step7ProjectV15_1 Project, TIAOpennessControllerFolder ControllerFolder, PlcTagTableGroup group) : base(Project)
            {
                this.ControllerFolder = ControllerFolder;
                this.Project = Project;
                this.TiaProject = Project;
                this.group = group;
            }

            public TIAOpennessConstant FindConstant(string name)
            {
                foreach (var t in TagTables)
                {
                    var c = t.Constants.FirstOrDefault(x => x.Name == name);
                    if (c != null)
                        return (TIAOpennessConstant)c;
                }
                foreach (var f in SubItems.Flatten(x => x.SubItems))
                {
                    foreach (var t in TagTables)
                    {
                        var c = t.Constants.FirstOrDefault(x => x.Name == name);
                        if (c != null)
                            return (TIAOpennessConstant)c;
                    }
                }
                return null;
            }

            public List<ITIAVarTab> TagTables
            {
                get
                {
                    PlcTagTableComposition tags = null;
                    var o = this.group as PlcTagTableUserGroup;
                    if (o != null)
                        tags = o.TagTables;
                    var q = this.group as PlcTagTableSystemGroup;
                    if (q != null)
                        tags = q.TagTables;
                    var retVal = new List<ITIAVarTab>();
                    
                    foreach (var tagList in tags)
                    {
                        var info = new TIAOpennessTagTable() { Name = tagList.Name, PlcTagTable = tagList };
                        retVal.Add(info);
                        info.Tags = tagList.Tags.Select(t => new TIAOpennessTag(t)).Cast<ITIATag>().ToList();
                        info.Constants = new List<ITIAConstant>();
                        foreach (var c in tagList.UserConstants)
                        {
                            info.Constants.Add(new TIAOpennessConstant(c) { Name = c.Name });
                        }
                    }
                    return retVal;
                }
            }
        }

        public class TIAOpennessWatchTable : ITIAWatchTable
        {
            internal PlcWatchTable PlcWatchTable { get; set; }

            public string Name { get; set; }

            public virtual string Export(ExportFormat exportFormat)
            {
                var ext = "xml";
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + "." + ext);
                if (ext == "xml")
                {
                    PlcWatchTable.Export(new FileInfo(file), Siemens.Engineering.ExportOptions.WithDefaults);
                }
                var text = File.ReadAllText(file);
                File.Delete(file);

                return text;
            }

            public string Export()
            {
                return Export(ExportFormat.Xml);
            }
        }

        public class TIAOpennessForceTable : ITIAForceTable
        {
            internal PlcForceTable PlcForceTable { get; set; }

            public string Name { get; set; }

            public virtual string Export(ExportFormat exportFormat)
            {
                var ext = "xml";
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + "." + ext);
                if (ext == "xml")
                {
                    PlcForceTable.Export(new FileInfo(file), Siemens.Engineering.ExportOptions.None);
                }
                var text = File.ReadAllText(file);
                File.Delete(file);

                return text;
            }

            public string Export()
            {
                return Export(ExportFormat.Xml);
            }
        }

        

        public class TIAOpennessWatchAndForceTablesFolder : TIAOpennessProjectFolder, ITIAWatchAndForceTablesFolder
        {
            public TIAOpennessControllerFolder ControllerFolder { get; set; }

            private PlcWatchAndForceTableGroup group;

            public TIAOpennessWatchAndForceTablesFolder(Step7ProjectV15_1 Project, TIAOpennessControllerFolder ControllerFolder, PlcWatchAndForceTableGroup group) : base(Project)
            {
                this.ControllerFolder = ControllerFolder;
                this.Project = Project;
                this.TiaProject = Project;
                this.group = group;
            }

            public List<ITIAWatchTable> WatchTables
            {
                get
                {
                    var retVal = new List<ITIAWatchTable>();
                    
                    foreach (var wt in group.WatchTables)
                    {
                        var info = new TIAOpennessWatchTable() { Name = wt.Name, PlcWatchTable = wt };
                        retVal.Add(info);
                    }
                    return retVal;
                }
            }

            public List<ITIAForceTable> ForceTables
            {
                get
                {
                    var retVal = new List<ITIAForceTable>();

                    foreach (var wt in group.ForceTables)
                    {
                        var info = new TIAOpennessForceTable() { Name = wt.Name, PlcForceTable = wt };
                        retVal.Add(info);
                    }
                    return retVal;
                }
            }
        }

        public class TIAOpennessPlcDatatypeFolder : TIAOpennessProjectFolder, IBlocksFolder
        {
            private PlcTypeComposition composition;
            private PlcTypeGroup plcTypeGroup;

            public TIAOpennessControllerFolder ControllerFolder { get; set; }

            public TIAOpennessPlcDatatypeFolder(Step7ProjectV15_1 Project, TIAOpennessControllerFolder ControllerFolder, PlcTypeComposition composition, PlcTypeGroup plcTypeGroup)
                : base(Project)
            {
                this.ControllerFolder = ControllerFolder;
                this.Project = Project;
                this.TiaProject = Project;
                this.composition = composition;
                this.plcTypeGroup = plcTypeGroup;
            }

            public override ProjectFolder CreateFolder(string name)
            {
                var gp = plcTypeGroup.Groups.Create(name);
                var newFld = new TIAOpennessPlcDatatypeFolder((Step7ProjectV15_1)Project, ControllerFolder, gp.Types, gp);
                newFld.Name = gp.Name;
                newFld.Parent = this;
                this.SubItems.Add(newFld);
                return newFld;
            }

            public override void ImportFile(FileInfo file, bool overwrite, bool importFromSource)
            {
                composition.Import(file, overwrite ? ImportOptions.Override : ImportOptions.None);
            }

            public List<ProjectBlockInfo> readPlcBlocksList()
            {
                if (_blockInfos != null)
                    return _blockInfos;

                _blockInfos = new List<ProjectBlockInfo>();

                if (composition != null)
                {
                    foreach (var block in composition)
                    {
                        var info = new TIAOpennessProjectDataTypeInfo(block) { Name = block.Name, ParentFolder = this };
                        info.BlockType = DataTypes.PLCBlockType.UDT;
                        _blockInfos.Add(info);
                    }
                }

                return BlockInfos;
            }

            private List<ProjectBlockInfo> _blockInfos;
            public List<ProjectBlockInfo> BlockInfos
            {
                get
                {
                    if (_blockInfos == null)
                        readPlcBlocksList();
                    return _blockInfos;
                }
                private set { _blockInfos = value; }
            }

            public Block GetBlock(string BlockName)
            {
                if (BlockInfos == null)
                    readPlcBlocksList();

                var block = GetBlock(BlockInfos.FirstOrDefault(x => x.Name == BlockName));

                if (block != null)
                    return block;

                foreach (TIAOpennessPlcDatatypeFolder s in this.SubItems)
                {
                    block = s.GetBlock(BlockName);
                    if (block != null)
                        return block;
                }

                return null;
            }

            public Block GetBlock(ProjectBlockInfo blkInfo)
            {
                if (blkInfo == null)
                    return null;
                
                var iv = blkInfo as TIAOpennessProjectDataTypeInfo;
                var text = iv.Export(ExportFormat.Xml);

                return ParseTiaDbUdtXml(text, blkInfo, ControllerFolder, ParseType.DataType);
            }
        }

        public class TIAOpennessProgramFolder : TIAOpennessProjectFolder, IBlocksFolder
        {
            public TIAOpennessControllerFolder ControllerFolder { get; set; }

            private PlcBlockComposition blocks;
            private PlcBlockGroup plcBlockGroup;

            public TIAOpennessProgramFolder(Step7ProjectV15_1 Project, TIAOpennessControllerFolder ControllerFolder, PlcBlockComposition blocks, PlcBlockGroup plcBlockGroup)
                : base(Project)
            {
                this.ControllerFolder = ControllerFolder;
                this.Project = Project;
                this.TiaProject = Project;
                this.blocks = blocks;
                this.plcBlockGroup = plcBlockGroup;
            }

            public override ProjectFolder CreateFolder(string name)
            {
                var gp = plcBlockGroup.Groups.Create(name);
                var newFld = new TIAOpennessProgramFolder((Step7ProjectV15_1)Project, ControllerFolder, gp.Blocks, gp);
                newFld.Name = gp.Name;
                newFld.Parent = this;
                this.SubItems.Add(newFld);
                return newFld;
            }

            public override void CompileBlocks()
            {
                CompilerResult result;

                var compiler = plcBlockGroup.GetService<ICompilable>();
                if (compiler != null)
                    result = compiler.Compile();
                else
                    throw new ArgumentException("Parameter cannot be compiled.", nameof(plcBlockGroup));
           }

            public override void ImportFile(FileInfo file, bool overwrite, bool importFromSource)
            {
                if (!importFromSource)
                {
                    blocks.Import(file, overwrite ? ImportOptions.Override : ImportOptions.None);
                }
                else
                {
                    var currentDestination = plcBlockGroup as IEngineeringObject;
                    while (!(currentDestination is PlcSoftware))
                    {
                        currentDestination = currentDestination.Parent;
                    }

                    var col = (currentDestination as PlcSoftware).ExternalSourceGroup.ExternalSources;

                    var sourceName = Path.GetRandomFileName();
                    sourceName = Path.ChangeExtension(sourceName, ".src");
                    var src = col.CreateFromFile(sourceName, file.FullName);
                    try
                    {                     
                        src.GenerateBlocksFromSource();
                    }
                    finally
                    {
                        src.Delete();
                    }
                }
            }

            public List<ProjectBlockInfo> readPlcBlocksList()
            {
                if (_blockInfos != null)
                    return _blockInfos;

                _blockInfos = new List<ProjectBlockInfo>();

                foreach (var block in blocks)
                {
                    var info = new TIAOpennessProjectBlockInfo(block) { Name = block.Name, ParentFolder = this };
                    info.BlockType = DataTypes.PLCBlockType.FB;
                    info.SetBlockLanguage = PLCLanguage.unkown;
                    if (block.ProgrammingLanguage == ProgrammingLanguage.DB ||
                        block.ProgrammingLanguage == ProgrammingLanguage.CPU_DB ||
                        block.ProgrammingLanguage == ProgrammingLanguage.F_DB ||
                        block.ProgrammingLanguage == ProgrammingLanguage.Motion_DB)
                    {
                        info.BlockType = DataTypes.PLCBlockType.DB;
                        info.SetBlockLanguage = PLCLanguage.DB;
                    }
                    else if (block.ProgrammingLanguage == ProgrammingLanguage.LAD ||
                             block.ProgrammingLanguage == ProgrammingLanguage.F_LAD ||
                             block.ProgrammingLanguage == ProgrammingLanguage.F_LAD_LIB)
                        info.SetBlockLanguage = PLCLanguage.KOP;
                    else if (block.ProgrammingLanguage == ProgrammingLanguage.STL ||
                             block.ProgrammingLanguage == ProgrammingLanguage.F_STL)
                        info.SetBlockLanguage = PLCLanguage.AWL;
                    else if (block.ProgrammingLanguage == ProgrammingLanguage.FBD ||
                             block.ProgrammingLanguage == ProgrammingLanguage.F_FBD ||
                             block.ProgrammingLanguage == ProgrammingLanguage.F_FBD_LIB)
                        info.SetBlockLanguage = PLCLanguage.FUP;
                    else if (block.ProgrammingLanguage == ProgrammingLanguage.CFC)
                        info.SetBlockLanguage = PLCLanguage.CFC;
                    else if (block.ProgrammingLanguage == ProgrammingLanguage.SCL)
                        info.SetBlockLanguage = PLCLanguage.SCL;
                    info.BlockNumber = block.Number;
                    _blockInfos.Add(info);
                }
                return _blockInfos;
            }

            private List<ProjectBlockInfo> _blockInfos;
            public List<ProjectBlockInfo> BlockInfos
            {
                get
                {
                    if (_blockInfos == null)
                        readPlcBlocksList();
                    return _blockInfos;
                }
                private set { _blockInfos = value; }
            }

            public Block GetBlock(string BlockName)
            {
                if (BlockInfos == null)
                    readPlcBlocksList();

                return
                    GetBlock(
                        BlockInfos.Cast<TIAOpennessProjectBlockInfo>()
                            .FirstOrDefault(x => x.Name == BlockName || x.BlockName == BlockName));
            }

            public Block GetBlock(ProjectBlockInfo blkInfo)
            {
                if (blkInfo == null)
                    return null;

                var iv = blkInfo as TIAOpennessProjectBlockInfo;
                var text = iv.Export(ExportFormat.Xml);
                  
                return ParseTiaDbUdtXml(text, blkInfo, ControllerFolder, ParseType.Programm);
            }
        }

        internal void OpenViaOpennessDlls(Credentials credentials)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    if (tiaPortal != null)
                    {
                        tiaPortal.Dispose();
                        tiaPortal = null;
                    }

                    tiaPortal = new Siemens.Engineering.TiaPortal(
                        Siemens.Engineering.TiaPortalMode.WithoutUserInterface);
                    if (credentials != null)
                    {

                        tiapProject = tiaPortal.Projects.Open(new FileInfo(ProjectFile), c =>
                        {
                            c.Type = UmacUserType.Project;
                            c.Name = credentials.Username;
                            c.SetPassword(credentials.Password);
                        });
                    }
                    else
                    {
                        tiapProject = tiaPortal.Projects.Open(new FileInfo(ProjectFile));
                    }
                }
                catch (Siemens.Engineering.EngineeringSecurityException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (i == 9)
                        throw;
                }

                if (tiapProject != null)
                    break;
            }

            LoadViaOpennessDlls();
        }

        internal void LoadViaOpennessDlls()
        {
            var main = new TIAOpennessProjectFolder(this) { Name = "Main" };
            ProjectStructure = main;

            //var frm = new sliver.Windows.Forms.StateBrowserForm();
            //frm.ObjectToBrowse = tiapProject;
            //frm.Show();

            foreach (var d in tiapProject.Devices)
            {
                if (d.TypeIdentifier != null && d.TypeIdentifier.EndsWith(".S71500"))
                {
                    foreach (DeviceItem deviceItem in d.DeviceItems)
                    {
                        var target = ((IEngineeringServiceProvider)deviceItem).GetService<SoftwareContainer>();
                        if (target != null && target.Software is PlcSoftware)
                        {
                            var software =(PlcSoftware)target.Software;
                                var fld = new TIAOpennessControllerFolder(this, software)
                                {
                                    Name = software.Name,
                                    //TiaPortalItem = software,
                                    //Comment = d.Comment != null ? d.Comment.GetText(CultureInfo.CurrentCulture) : null
                                };
                                main.SubItems.Add(fld);

                                LoadControlerFolderViaOpennessDlls(fld, software);
                           
                        }
                    }

                    //var controller = d.DeviceItems.OfType<Siemens.Engineering.HW.ControllerTarget>().FirstOrDefault();
                    //if (controller == null)
                    //{
                    //    var fld = new TIAOpennessProjectFolder(this)
                    //    {
                    //        Name = d.Name,
                    //        TiaPortalItem = d,
                    //        Comment = d.Comment != null ? d.Comment.GetText(CultureInfo.CurrentCulture) : null
                    //    };
                    //    main.SubItems.Add(fld);

                    //    //LoadSubDevicesViaOpennessDlls(fld, d);
                    //}
                    //else
                    //{
                    //    var fld = new TIAOpennessControllerFolder(this)
                    //    {
                    //        Name = d.Name,
                    //        TiaPortalItem = d,
                    //        Comment = d.Comment != null ? d.Comment.GetText(CultureInfo.CurrentCulture) : null
                    //    };
                    //    main.SubItems.Add(fld);

                    //    //LoadControlerFolderViaOpennessDlls(fld, controller);
                    //}
                }
            }
        }

        //internal void LoadSubDevicesViaOpennessDlls(TIAOpennessProjectFolder parent, Siemens.Engineering.HW.IHardwareObject device)
        //{
        //    foreach (var e in device.DeviceItems)
        //    {
        //        var fld = new TIAOpennessProjectFolder(this)
        //        {
        //            TiaPortalItem = e,
        //            Name = e.Name,
        //            Parent = parent,
        //        };
        //        var d = e as Siemens.Engineering.HW.DeviceItem;
        //        //d.Elements.ToList()

        //        parent.SubItems.Add(fld);
        //        LoadSubDevicesViaOpennessDlls(fld, e);
        //    }
        //}

        internal void LoadControlerFolderViaOpennessDlls(TIAOpennessControllerFolder parent, PlcSoftware software)
        {
            var fld = new TIAOpennessProgramFolder(this, parent, software.BlockGroup.Blocks, software.BlockGroup)
            {
                //TiaPortalItem = controller.ProgramblockFolder,
                Name = "software",
                Parent = parent,
            };
            parent.ProgramFolder = fld;
            parent.SubItems.Add(fld);
            LoadSubProgramBlocksFoldersViaOpennessDlls(fld, software.BlockGroup);

            var t = (PlcTypeGroup)software.TypeGroup;
            
            var fld2 = new TIAOpennessPlcDatatypeFolder(this, parent, t.Types, t)
            {
                //TiaPortalItem = controller.ControllerDatatypeFolder,
                Name = "data types",
                Parent = parent,
            };
            parent.PlcDatatypeFolder = fld2;
            parent.SubItems.Add(fld2);
            LoadSubPlcDatatypeFoldersViaOpennessDlls(fld2, software.TypeGroup);

            var fld3 = new TIAOpennessVariablesFolder(this, parent, software.TagTableGroup)
            {
                Name = "variables",
                Parent = parent,
            };
            parent.VarTabFolder = fld3;
            parent.SubItems.Add(fld3);
            LoadSubVartabFoldersViaOpennessDlls(fld3, software.TagTableGroup);

            var fld4 = new TIAOpennessWatchAndForceTablesFolder(this, parent, software.WatchAndForceTableGroup)
            {
                Name = "watches and forces",
                Parent = parent,
            };
            parent.WatchAndForceTablesFolder = fld4;
            parent.SubItems.Add(fld4);
            LoadSubWatchAndForceTablesFoldersViaOpennessDlls(fld4, software.WatchAndForceTableGroup);
        }

        internal void LoadSubProgramBlocksFoldersViaOpennessDlls(TIAOpennessProgramFolder parent, PlcBlockGroup plcBlockGroup)
        {
            foreach (var e in plcBlockGroup.Groups)
            {
                var fld = new TIAOpennessProgramFolder(this, parent.ControllerFolder, e.Blocks, e)
                {
                    //TiaPortalItem = e,
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubProgramBlocksFoldersViaOpennessDlls(fld, e);
            }
        }

        internal void LoadSubPlcDatatypeFoldersViaOpennessDlls(TIAOpennessPlcDatatypeFolder parent, PlcTypeSystemGroup p)
        {
            foreach (var e in p.Groups)
            {
                var fld = new TIAOpennessPlcDatatypeFolder(this, parent.ControllerFolder, e.Types, e)
                {
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubPlcDatatypeFoldersViaOpennessDlls(fld, e);
            }
        }

        internal void LoadSubPlcDatatypeFoldersViaOpennessDlls(TIAOpennessPlcDatatypeFolder parent, PlcTypeUserGroup p)
        {
            foreach (var e in p.Groups)
            {
                var fld = new TIAOpennessPlcDatatypeFolder(this, parent.ControllerFolder, e.Types, e)
                {
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubPlcDatatypeFoldersViaOpennessDlls(fld, e);
            }
        }

        internal void LoadSubVartabFoldersViaOpennessDlls(TIAOpennessVariablesFolder parent, PlcTagTableSystemGroup blockFolder)
        {
            foreach (var e in blockFolder.Groups)
            {
                var fld = new TIAOpennessVariablesFolder(this, parent.ControllerFolder, e)
                {
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubVartabFoldersViaOpennessDlls(fld, e);
            }
        }

        internal void LoadSubVartabFoldersViaOpennessDlls(TIAOpennessVariablesFolder parent, PlcTagTableUserGroup blockFolder)
        {
            foreach (var e in blockFolder.Groups)
            {
                var fld = new TIAOpennessVariablesFolder(this, parent.ControllerFolder, e)
                {
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubVartabFoldersViaOpennessDlls(fld, e);
            }
        }

        internal void LoadSubWatchAndForceTablesFoldersViaOpennessDlls(TIAOpennessWatchAndForceTablesFolder parent, PlcWatchAndForceTableSystemGroup blockFolder)
        {
            foreach (var e in blockFolder.Groups)
            {
                var fld = new TIAOpennessWatchAndForceTablesFolder(this, parent.ControllerFolder, e)
                {
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubWatchAndForceTablesFoldersViaOpennessDlls(fld, e);
            }
        }

        internal void LoadSubWatchAndForceTablesFoldersViaOpennessDlls(TIAOpennessWatchAndForceTablesFolder parent, PlcWatchAndForceTableUserGroup blockFolder)
        {
            foreach (var e in blockFolder.Groups)
            {
                var fld = new TIAOpennessWatchAndForceTablesFolder(this, parent.ControllerFolder, e)
                {
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubWatchAndForceTablesFoldersViaOpennessDlls(fld, e);
            }
        }
        #region Parse DB UDT XML

        internal enum ParseType
        {
            Programm,
            DataType
        }

        internal static Block ParseTiaDbUdtXml(string xml, ProjectBlockInfo projectBlockInfo, TIAOpennessControllerFolder controllerFolder, ParseType parseType)
        {
            XElement xelement = XElement.Parse(xml);
            var structure = xelement.Elements().FirstOrDefault(x => x.Name.LocalName.StartsWith("SW."));

            var sections = structure.Element("AttributeList").Element("Interface").Elements().First();

            var block = new TIADataBlock();
            block.Name = projectBlockInfo.Name;

            if (projectBlockInfo is TIAOpennessProjectBlockInfo)
                block.BlockNumber = ((TIAOpennessProjectBlockInfo) projectBlockInfo).BlockNumber;

            if (parseType == ParseType.DataType)
                block.BlockType = DataTypes.PLCBlockType.UDT;
            else if (parseType == ParseType.Programm)
                block.BlockType = DataTypes.PLCBlockType.DB;
            
            var parameterRoot = ParseTiaDbUdtSections(sections, block, controllerFolder);

            block.BlockType = DataTypes.PLCBlockType.DB;
            block.Structure = parameterRoot;
            
            return block;
        }

        internal static TIADataRow ParseTiaDbUdtSections(XElement sections, TIADataBlock block, TIAOpennessControllerFolder controllerFolder)
        {
            var parameterRoot = new TIADataRow("ROOTNODE", S7DataRowType.STRUCT, block);
            var parameterIN = new TIADataRow("IN", S7DataRowType.STRUCT, block);
            parameterIN.Parent = parameterRoot;
            var parameterOUT = new TIADataRow("OUT", S7DataRowType.STRUCT, block);
            parameterOUT.Parent = parameterRoot;
            var parameterINOUT = new TIADataRow("IN_OUT", S7DataRowType.STRUCT, block);
            parameterINOUT.Parent = parameterRoot;
            var parameterSTAT = new TIADataRow("STATIC", S7DataRowType.STRUCT, block);
            parameterSTAT.Parent = parameterRoot;
            var parameterTEMP = new TIADataRow("TEMP", S7DataRowType.STRUCT, block);
            parameterTEMP.Parent = parameterRoot;

            foreach (var xElement in sections.Elements())
            {
                TIADataRow useRow = parameterRoot;
                //var sectionName = xElement.Attribute("Name").Value;
                //if (sectionName == "None" || sectionName == "Static")
                //{
                //    useRow = parameterSTAT;
                //    parameterRoot.Add(useRow);
                //}
                //else if (sectionName == "In")
                //{
                //    useRow = parameterIN;
                //    parameterRoot.Add(useRow);
                //}

                parseChildren(useRow, xElement, controllerFolder);
            }

            return parameterRoot;
        }

        internal static void parseChildren(TIADataRow parentRow, XElement parentElement, TIAOpennessControllerFolder controllerFolder)
        {
            foreach (var xElement in parentElement.Elements())
            {
                if (xElement.Name.LocalName == "Comment")
                {
                    var text = xElement.Elements().FirstOrDefault(x => x.Attribute("Lang").Value == "de-DE");
                    if (text == null)
                        text = xElement.Elements().FirstOrDefault();
                    if (text != null)
                        parentRow.Comment = text.Value;
                }
                else if (xElement.Name.LocalName == "StartValue")
                {
                    parentRow.StartValue = xElement.Value;
                }
                else if (xElement.Name.LocalName == "Sections")
                {
                    var row = ParseTiaDbUdtSections(xElement, (TIADataBlock)parentRow.CurrentBlock, controllerFolder);
                    parentRow.AddRange(row.Children);
                }
                else if (xElement.Name.LocalName == "Member")
                {
                    var name = xElement.Attribute("Name").Value;
                    var datatype = xElement.Attribute("Datatype").Value;

                    var row = new TIADataRow(name, S7DataRowType.STRUCT, (TIABlock)parentRow.PlcBlock);
                    row.Parent = parentRow;

                    if (datatype.Contains("Array["))
                    {
                        List<int> arrayStart = new List<int>();
                        List<int> arrayStop = new List<int>();

                        int pos1 = datatype.IndexOf("[");
                        int pos2 = datatype.IndexOf("]", pos1);
                        string[] arrays = datatype.Substring(pos1 + 1, pos2 - pos1 - 1).Split(',');

                        foreach (string array in arrays)
                        {
                            string[] akar = array.Split(new string[] { ".." }, StringSplitOptions.RemoveEmptyEntries);
                            int start = 0;
                            if (akar[0].StartsWith("\""))
                            {
                                start = (int)controllerFolder.VarTabFolder.FindConstant(akar[0].Substring(1, akar[0].Length - 2)).Value;
                            }
                            else
                            {
                                start = Convert.ToInt32(akar[0].Trim());
                            }

                            int stop = 0;
                            if (akar[1].StartsWith("\""))
                            {
                                stop = (int)controllerFolder.VarTabFolder.FindConstant(akar[1].Substring(1, akar[1].Length - 2)).Value;
                            }
                            else
                            {
                                stop = Convert.ToInt32(akar[1].Trim());
                            }

                            arrayStart.Add(start);
                            arrayStop.Add(stop);
                        }

                        row.ArrayStart = arrayStart;
                        row.ArrayStop = arrayStop;
                        row.IsArray = true;
                        datatype = datatype.Substring(pos2 + 5);
                    }

                    parentRow.Add(row);

                    parseChildren(row, xElement, controllerFolder);

                    if (datatype.StartsWith("\""))
                    {
                        var udt =
                            controllerFolder.PlcDatatypeFolder.GetBlock(datatype.Substring(1, datatype.Length - 2));
                        if (udt != null)
                        {
                            var tiaUdt = udt as TIADataBlock;
                            row.AddRange(((TIADataRow)tiaUdt.Structure).DeepCopy().Children);

                            row.DataTypeBlock = udt;
                        }
                        row.DataType = S7DataRowType.UDT;
                    }
                    else if (datatype == "Struct")
                    {

                    }
                    else if (datatype.StartsWith("String["))
                    {
                        row.DataType = S7DataRowType.STRING;
                        row.StringSize = int.Parse(datatype.Substring(7, datatype.Length - 8));
                    }
                    else
                    {
                        switch (datatype.ToLower())
                        {
                            case "byte":
                                row.DataType = S7DataRowType.BYTE;
                                break;
                            case "bool":
                                row.DataType = S7DataRowType.BOOL;
                                break;
                            case "int":
                                row.DataType = S7DataRowType.INT;
                                break;
                            case "uint":
                                row.DataType = S7DataRowType.WORD;
                                break;
                            case "dint":
                                row.DataType = S7DataRowType.DINT;
                                break;
                            case "udint":
                                row.DataType = S7DataRowType.DWORD;
                                break;
                            case "word":
                                row.DataType = S7DataRowType.WORD;
                                break;
                            case "dword":
                                row.DataType = S7DataRowType.DWORD;
                                break;
                            case "char":
                                row.DataType = S7DataRowType.CHAR;
                                break;
                            case "any":
                                row.DataType = S7DataRowType.ANY;
                                break;
                            case "date":
                                row.DataType = S7DataRowType.DATE;
                                break;
                            case "date_and_time":
                                row.DataType = S7DataRowType.DATE_AND_TIME;
                                break;
                            case "real":
                                row.DataType = S7DataRowType.REAL;
                                break;
                            case "s5time":
                                row.DataType = S7DataRowType.S5TIME;
                                break;
                            case "time_of_day":
                                row.DataType = S7DataRowType.TIME_OF_DAY;
                                break;
                            case "time":
                                row.DataType = S7DataRowType.TIME;
                                break;
                            default:
                                row.DataType = S7DataRowType.UNKNOWN;

                                var udt = controllerFolder.PlcDatatypeFolder.GetBlock(datatype);
                                if (udt != null)
                                {
                                    var tiaUdt = udt as TIADataBlock;
                                    row.AddRange(((TIADataRow)tiaUdt.Structure).DeepCopy().Children);

                                    row.DataTypeBlock = udt;
                                    row.DataType = S7DataRowType.UDT;
                                }

                                //Console.WriteLine("unkown Datatype: " + datatype);
                                break;
                        }
                    }
                }
                else if (xElement.Name.LocalName == "AttributeList")
                { }
                else if (xElement.Name.LocalName == "Subelement") //todo -> startwerte von arrays von UDTs
                { }
                else
                {
                    //Console.WriteLine("unkown XML Element");
                }
            }
        }
        #endregion
    }
}
