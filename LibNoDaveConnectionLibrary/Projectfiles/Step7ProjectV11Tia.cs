﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public interface ITiaProjectBlockInfo : IProjectBlockInfo
    {
        string ExportToString();

        string GenerateSource();
    }

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
        
        public class TIAOpennessProjectBlockInfo : ProjectBlockInfo, ITiaProjectBlockInfo
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

            public string ProgrammingLanguage
            {
                get { return IBlock.ProgrammingLanguage.ToString(); }
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

            public virtual string ExportToString()
            {
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + ".tmp");
                IBlock.Export(file, ExportOptions.None);

                var text = File.ReadAllText(file);
                File.Delete(file);

                return text;
            }

            public virtual string GenerateSource()
            {
                //if (this.IBlock.ProgrammingLanguage != Siemens.Engineering.SW.ProgrammingLanguage.F_DB ||
                //    this.IBlock.ProgrammingLanguage != Siemens.Engineering.SW.ProgrammingLanguage.F_FBD ||
                //    this.IBlock.ProgrammingLanguage != Siemens.Engineering.SW.ProgrammingLanguage.F_FBD_LIB ||
                //    this.IBlock.ProgrammingLanguage != Siemens.Engineering.SW.ProgrammingLanguage.F_LAD ||
                //    this.IBlock.ProgrammingLanguage != Siemens.Engineering.SW.ProgrammingLanguage.F_LAD_LIB ||
                //    this.IBlock.ProgrammingLanguage != Siemens.Engineering.SW.ProgrammingLanguage.F_STL)
                //{
                //    return null;
                //}
                if (this.IBlock.ProgrammingLanguage != Siemens.Engineering.SW.ProgrammingLanguage.SCL &&
                    this.IBlock.ProgrammingLanguage != Siemens.Engineering.SW.ProgrammingLanguage.STL)
                {
                    return GenerateSourceCode();
                }

                return GenerateSourceXML();
            }

            public virtual string GenerateSourceXML()
            {
                var rootFolder = (TIAOpennessProjectFolder)ParentFolder;
                while (!(rootFolder.TiaPortalItem is Siemens.Engineering.SW.ProgramblockSystemFolder))
                {
                    rootFolder = (TIAOpennessProjectFolder)rootFolder.Parent;
                }
                var ext = this.IBlock.ProgrammingLanguage.ToString().ToLower();
                if (ext == "stl")
                {
                    ext = "awl";
                }

                dynamic tiaItem = ((TIAOpennessProgramFolder)rootFolder).TiaPortalItem;
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + "." + ext);
                tiaItem.GenerateSourceFromBlocks(new[] { this.IBlock }, file);

                var text = File.ReadAllText(file);
                File.Delete(file);

                return text;
            }

            public virtual string GenerateSourceCode()
            {
                return this.ExportToString();
            }
        }

        public class TIAOpennessProjectDataTypeInfo : ProjectBlockInfo, ITiaProjectBlockInfo
        {
            public ControllerDatatype IBlock { get; set; }

            public override string ToString()
            {
                return Name;
            }

            public virtual string ExportToString()
            {
                var tmp = Path.GetTempPath();
                var file = Path.Combine(tmp, "tmp_dnspt_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Replace(" ", "") + ".tmp");
                 
                IBlock.Export(file, ExportOptions.None);
                var text = File.ReadAllText(file);
                File.Delete(file);

                return text;
            }

            public string GenerateSource()
            {
                return ExportToString();
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
                if (_blockInfos != null)
                    return _blockInfos;
                ControllerDatatypeAggregation blocks = null;
                var o = this.TiaPortalItem as ControllerDatatypeUserFolder;
                if (o != null)
                    blocks = o.Datatypes;
                var q = this.TiaPortalItem as ControllerDatatypeSystemFolder;
                if (q != null)
                    blocks = q.Datatypes;

                _blockInfos = new List<ProjectBlockInfo>();

                foreach (var block in blocks)
                {
                    var info = new TIAOpennessProjectDataTypeInfo() { Name = block.Name, IBlock = block, ParentFolder = this};
                    info.BlockType = DataTypes.PLCBlockType.UDT;
                    _blockInfos.Add(info);
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

                return GetBlock(BlockInfos.FirstOrDefault(x => x.Name == BlockName));
            }

            public Block GetBlock(ProjectBlockInfo blkInfo)
            {
                if (blkInfo == null)
                    return null;
                
                var iv = blkInfo as ITiaProjectBlockInfo;
                var text = iv.ExportToString();

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
                if (_blockInfos != null)
                    return _blockInfos;

                IBlockAggregation blocks = null;
                var o = this.TiaPortalItem as ProgramblockUserFolder;
                if (o != null)
                    blocks = o.Blocks;
                var q = this.TiaPortalItem as ProgramblockSystemFolder;
                if (q != null)
                    blocks = q.Blocks;

                _blockInfos = new List<ProjectBlockInfo>();

                foreach (var block in blocks)
                {
                    var info = new TIAOpennessProjectBlockInfo() { Name = block.Name, IBlock = block, ParentFolder = this};
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

                var iv = blkInfo as ITiaProjectBlockInfo;
                var text = iv.ExportToString();
                  
                return ParseTiaDbUdtXml(text, blkInfo, ControllerFolder, ParseType.Programm);
            }
        }
        internal void LoadViaOpennessDlls()
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
                    tiaPortal = new TiaPortal(TiaPortalMode.WithoutUserInterface);
                    tiapProject = tiaPortal.Projects.Open(ProjectFile);
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
                    var row = ParseTiaDbUdtSections(xElement, (TIADataBlock) parentRow.CurrentBlock, controllerFolder);
                    parentRow.AddRange(row.Children);
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
                        var udt =
                            controllerFolder.PlcDatatypeFolder.GetBlock(datatype.Substring(1, datatype.Length - 2));
                        if (udt != null)
                        {
                            var tiaUdt = udt as TIADataBlock;
                            row.AddRange(((TIADataRow) tiaUdt.Structure).DeepCopy().Children);

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
                                row.DataType = S7DataRowType.DWORD;
                                break;
                            case "dint":
                                row.DataType = S7DataRowType.DINT;
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
                                Console.WriteLine("unkown Datatype: " + datatype);
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
