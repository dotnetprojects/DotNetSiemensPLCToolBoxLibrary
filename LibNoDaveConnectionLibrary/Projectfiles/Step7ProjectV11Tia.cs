using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public partial class Step7ProjectV11
    {

        private TiaPortal tiaPortal;
        private Siemens.Engineering.Project tiapProject;

        public virtual void Dispose()
        {
            tiaPortal.Dispose();
        }


        public class TIAOpennessProjectFolder : ProjectFolder
        {
            //internal string ID { get; private set; }
            internal string InstID { get; private set; }

            protected Step7ProjectV11 TiaProject;

            public object TiaPortalItem { get; set; }

            public override string Name { get; set; }

            public TIAOpennessProjectFolder(Step7ProjectV11 Project)
            {
                this.Project = Project;
                this.TiaProject = Project;
            }

        }

        public class TIAOpennessProjectBlockInfo : ProjectBlockInfo
        {
            public IBlock IBlock { get; set; }
            public int BlockNumber { get; set; }

            public string BlockName
            {
                get
                {
                    string retVal = BlockType.ToString().Replace("S5_", "") + BlockNumber.ToString();
                    return retVal;
                }
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
        }

        public class TIAOpennessProjectDataTypeInfo : ProjectBlockInfo
        {
            public ControllerDatatype IBlock { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        public class TIAOpennessControllerFolder : TIAOpennessProjectFolder, IRootProgrammFolder
        {
            public TIAOpennessControllerFolder(Step7ProjectV11 Project)
                : base(Project)
            {
                this.Project = Project;
                this.TiaProject = Project;
            }

            public TIAOpennessProgramFolder ProgramFolder { get; set; }
            public TIAOpennessPlcDatatypeFolder PlcDatatypeFolder { get; set; }
        }


        public class TIAOpennessPlcDatatypeFolder : TIAOpennessProjectFolder, IBlocksFolder
        {
            public TIAOpennessControllerFolder ControllerFolder { get; set; }

            public TIAOpennessPlcDatatypeFolder(Step7ProjectV11 Project, TIAOpennessControllerFolder ControllerFolder)
                : base(Project)
            {
                this.ControllerFolder = ControllerFolder;
                this.Project = Project;
                this.TiaProject = Project;
            }

            public List<ProjectBlockInfo> readPlcBlocksList()
            {
                if (BlockInfos != null)
                    return BlockInfos;
                ControllerDatatypeAggregation blocks = null;
                var o = this.TiaPortalItem as ControllerDatatypeUserFolder;
                if (o != null)
                    blocks = o.Datatypes;
                var q = this.TiaPortalItem as ControllerDatatypeSystemFolder;
                if (q != null)
                    blocks = q.Datatypes;

                BlockInfos = new List<ProjectBlockInfo>();

                foreach (var block in blocks)
                {
                    var info = new TIAOpennessProjectDataTypeInfo() { Name = block.Name, IBlock = block };
                    info.BlockType = DataTypes.PLCBlockType.UDT;
                    BlockInfos.Add(info);
                }

                return BlockInfos;
            }

            public List<ProjectBlockInfo> BlockInfos { get; private set; }
            public Block GetBlock(string BlockName)
            {
                if (BlockInfos == null)
                    readPlcBlocksList();

                return GetBlock(BlockInfos.First(x => x.Name == BlockName));
            }

            public Block GetBlock(ProjectBlockInfo blkInfo)
            {
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + ".tmp");

                var iv = blkInfo as TIAOpennessProjectDataTypeInfo;
                iv.IBlock.Export(file, ExportOptions.None);
                var text = File.ReadAllText(file);
                File.Delete(file);

                return ParseTiaDbUdtXml(text);
            }
        }

        public class TIAOpennessProgramFolder : TIAOpennessProjectFolder, IBlocksFolder
        {
            public TIAOpennessControllerFolder ControllerFolder { get; set; }

            public TIAOpennessProgramFolder(Step7ProjectV11 Project, TIAOpennessControllerFolder ControllerFolder)
                : base(Project)
            {
                this.ControllerFolder = ControllerFolder;
                this.Project = Project;
                this.TiaProject = Project;
            }

            public List<ProjectBlockInfo> readPlcBlocksList()
            {
                if (BlockInfos != null)
                    return BlockInfos;

                IBlockAggregation blocks = null;
                var o = this.TiaPortalItem as ProgramblockUserFolder;
                if (o != null)
                    blocks = o.Blocks;
                var q = this.TiaPortalItem as ProgramblockSystemFolder;
                if (q != null)
                    blocks = q.Blocks;

                BlockInfos = new List<ProjectBlockInfo>();

                foreach (var block in blocks)
                {
                    var info = new TIAOpennessProjectBlockInfo() { Name = block.Name, IBlock = block };
                    if (block.Type == BlockType.DB)
                        info.BlockType = DataTypes.PLCBlockType.DB;
                    else if (block.Type == BlockType.FB)
                        info.BlockType = DataTypes.PLCBlockType.FB;
                    else if (block.Type == BlockType.FC)
                        info.BlockType = DataTypes.PLCBlockType.FC;
                    else if (block.Type == BlockType.OB)
                        info.BlockType = DataTypes.PLCBlockType.OB;
                    else if (block.Type == BlockType.UDT)
                        info.BlockType = DataTypes.PLCBlockType.UDT;
                    info.BlockNumber = block.Number;
                    BlockInfos.Add(info);
                }
                return BlockInfos;
            }

            public List<ProjectBlockInfo> BlockInfos { get; private set; }
            public Block GetBlock(string BlockName)
            {
                if (BlockInfos == null)
                    readPlcBlocksList();

                return
                    GetBlock(
                        BlockInfos.Cast<TIAOpennessProjectBlockInfo>()
                            .First(x => x.Name == BlockName || x.BlockName == BlockName));
            }

            public Block GetBlock(ProjectBlockInfo blkInfo)
            {
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + ".tmp");
                
                var iv = blkInfo as TIAOpennessProjectBlockInfo;
                iv.IBlock.Export(file, ExportOptions.None);
                var text = File.ReadAllText(file);
                File.Delete(file);

                return ParseTiaDbUdtXml(text);
            }
        }
        internal void LoadViaOpennessDlls()
        {
            tiaPortal = new TiaPortal(TiaPortalMode.WithoutUserInterface);
            tiapProject = tiaPortal.Projects.Open(ProjectFile);

            var main = new TIAOpennessProjectFolder(this) { Name = "Main" };
            ProjectStructure = main;

            //var frm = new sliver.Windows.Forms.StateBrowserForm();
            //frm.ObjectToBrowse = tiapProject;
            //frm.Show();

            foreach (var d in tiapProject.Devices)
            {
                if (d.Subtype.StartsWith("S7300") || d.Subtype.StartsWith("S7400"))
                {
                    

                    var controller = d.DeviceItems.OfType<ControllerTarget>().FirstOrDefault();
                    if (controller == null)
                    {
                        var fld = new TIAOpennessProjectFolder(this)
                        {
                            Name = d.Name,
                            TiaPortalItem = d,
                            Comment = d.Comment != null ? d.Comment.GetText(CultureInfo.CurrentCulture) : null
                        };
                        main.SubItems.Add(fld);

                        LoadSubDevicesViaOpennessDlls(fld, d);
                    }
                    else
                    {
                        var fld = new TIAOpennessControllerFolder(this)
                        {
                            Name = d.Name,
                            TiaPortalItem = d,
                            Comment = d.Comment != null ? d.Comment.GetText(CultureInfo.CurrentCulture) : null
                        };
                        main.SubItems.Add(fld);

                        LoadControlerFolderViaOpennessDlls(fld, controller);
                    }
                }
            }
        }

        internal void LoadSubDevicesViaOpennessDlls(TIAOpennessProjectFolder parent, IHardwareObject device)
        {
            foreach (var e in device.DeviceItems)
            {
                var fld = new TIAOpennessProjectFolder(this)
                {
                    TiaPortalItem = e,
                    Name = e.Name,
                    Parent = parent,
                };
                var d = e as Siemens.Engineering.HW.DeviceItem;
                //d.Elements.ToList()

                parent.SubItems.Add(fld);
                LoadSubDevicesViaOpennessDlls(fld, e);
            }
        }

        internal void LoadControlerFolderViaOpennessDlls(TIAOpennessControllerFolder parent, ControllerTarget controller)
        {
            var fld = new TIAOpennessProgramFolder(this, parent)
            {
                TiaPortalItem = controller.ProgramblockFolder,
                Name = controller.ProgramblockFolder.Name,
                Parent = parent,
            };
            parent.ProgramFolder = fld;
            parent.SubItems.Add(fld);
            LoadSubProgramBlocksFoldersViaOpennessDlls(fld, controller.ProgramblockFolder);


            var fld2 = new TIAOpennessPlcDatatypeFolder(this, parent)
            {
                TiaPortalItem = controller.ControllerDatatypeFolder,
                Name = "PLC data types",
                Parent = parent,
            };
            parent.PlcDatatypeFolder = fld2;
            parent.SubItems.Add(fld2);
            LoadSubPlcDatatypeFoldersViaOpennessDlls(fld2, controller.ControllerDatatypeFolder);
        }

        internal void LoadSubProgramBlocksFoldersViaOpennessDlls(TIAOpennessProgramFolder parent, ProgramblockSystemFolder blockFolder)
        {
            foreach (var e in blockFolder.Folders)
            {
                var fld = new TIAOpennessProgramFolder(this, parent.ControllerFolder)
                {
                    TiaPortalItem = e,
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubProgramBlocksFoldersViaOpennessDlls(fld, e);
            }
        }
        internal void LoadSubProgramBlocksFoldersViaOpennessDlls(TIAOpennessProgramFolder parent, ProgramblockUserFolder blockFolder)
        {
            foreach (var e in blockFolder.Folders)
            {
                var fld = new TIAOpennessProgramFolder(this, parent.ControllerFolder)
                {
                    TiaPortalItem = e,
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubProgramBlocksFoldersViaOpennessDlls(fld, e);
            }
        }
        internal void LoadSubPlcDatatypeFoldersViaOpennessDlls(TIAOpennessPlcDatatypeFolder parent, ControllerDatatypeSystemFolder blockFolder)
        {
            foreach (var e in blockFolder.Folders)
            {
                var fld = new TIAOpennessPlcDatatypeFolder(this, parent.ControllerFolder)
                {
                    TiaPortalItem = e,
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubPlcDatatypeFoldersViaOpennessDlls(fld, e);
            }
        }
        internal void LoadSubPlcDatatypeFoldersViaOpennessDlls(TIAOpennessPlcDatatypeFolder parent, ControllerDatatypeUserFolder blockFolder)
        {
            foreach (var e in blockFolder.Folders)
            {
                var fld = new TIAOpennessPlcDatatypeFolder(this, parent.ControllerFolder)
                {
                    TiaPortalItem = e,
                    Name = e.Name,
                    Parent = parent,
                };
                parent.SubItems.Add(fld);
                LoadSubPlcDatatypeFoldersViaOpennessDlls(fld, e);
            }
        }


        public static Block ParseTiaDbUdtXml(string xml)
        {
            XElement xelement = XElement.Parse(xml);
            var structure = xelement.Elements().FirstOrDefault(x => x.Name.LocalName.StartsWith("SW."));

            var sections = structure.Element("AttributeList").Element("Interface").Elements().First();

            foreach (var xElement in sections.Elements())
            {
                
            }
            
            return null;
        }
    }
}
