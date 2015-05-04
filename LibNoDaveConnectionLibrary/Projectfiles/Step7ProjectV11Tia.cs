using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
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
            public Siemens.Engineering.SW.IBlock IBlock { get; set; }
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

                return GetBlock(BlockInfos.FirstOrDefault(x => x.Name == BlockName));
            }

            public Block GetBlock(ProjectBlockInfo blkInfo)
            {
                if (blkInfo == null)
                    return null;
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + ".tmp");

                var iv = blkInfo as TIAOpennessProjectDataTypeInfo;
                iv.IBlock.Export(file, ExportOptions.None);
                var text = File.ReadAllText(file);
                File.Delete(file);

                return ParseTiaDbUdtXml(text, blkInfo, ControllerFolder, ParseType.DataType);
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
                            .FirstOrDefault(x => x.Name == BlockName || x.BlockName == BlockName));
            }

            public Block GetBlock(ProjectBlockInfo blkInfo)
            {
                if (blkInfo == null)
                    return null;
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + ".tmp");
                
                var iv = blkInfo as TIAOpennessProjectBlockInfo;
                iv.IBlock.Export(file, ExportOptions.None);
                var text = File.ReadAllText(file);
                File.Delete(file);

                return ParseTiaDbUdtXml(text, blkInfo, ControllerFolder, ParseType.Programm);
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
                Console.WriteLine(d.Subtype);                
                if (d.Subtype.EndsWith(".Device") && !d.Subtype.StartsWith("GSD.") && !d.Subtype.StartsWith("ET200eco.")) //d.Subtype.StartsWith("S7300") || d.Subtype.StartsWith("S7400") || d.Subtype.StartsWith("S71200") || d.Subtype.StartsWith("S71500"))
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

            block.BlockType = DataTypes.PLCBlockType.DB;
            block.Structure = parameterRoot;
            
            return block;
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
                else if (xElement.Name.LocalName == "Member")
                {
                    var name = xElement.Attribute("Name").Value;
                    var datatype = xElement.Attribute("Datatype").Value;

                    var row = new TIADataRow(name, S7DataRowType.STRUCT, (TIABlock) parentRow.PlcBlock);
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
                            string[] akar = array.Split(new string[] {".."}, StringSplitOptions.RemoveEmptyEntries);
                            arrayStart.Add(Convert.ToInt32(akar[0].Trim()));
                            arrayStop.Add(Convert.ToInt32(akar[1].Trim()));
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
                        var udt = controllerFolder.PlcDatatypeFolder.GetBlock(datatype.Substring(1, datatype.Length - 2));
                        var tiaUdt = udt as TIADataBlock;
                        row.AddRange(((TIADataRow) tiaUdt.Structure).DeepCopy().Children);
                        row.DataType = S7DataRowType.UDT;
                        row.DataTypeBlock = udt;
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
                        switch (datatype)
                        {
                            case "Byte":
                                row.DataType = S7DataRowType.BYTE;
                                break;
                            case "Bool":
                                row.DataType = S7DataRowType.BOOL;
                                break;
                            case "Int":
                                row.DataType = S7DataRowType.INT;
                                break;
                            case "UInt":
                                row.DataType = S7DataRowType.DWORD;
                                break;
                            case "DInt":
                                row.DataType = S7DataRowType.DINT;
                                break;
                            case "Word":
                                row.DataType = S7DataRowType.WORD;
                                break;
                            case "Char":
                                row.DataType = S7DataRowType.CHAR;
                                break;
                            default:
                                row.DataType = S7DataRowType.UNKNOWN;
                                Console.WriteLine("unkown Datatype");
                                break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("unkown XML Element");
                }
            }
        }
        #endregion
    }
}
