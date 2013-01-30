using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    public class TIADataRow : DataBlockRow, INotifyPropertyChanged
    {
        private XmlNode node;
        private Step7ProjectV11 tiaProject;
        private TIABlock block;

        public TIADataRow(XmlNode Node, Step7ProjectV11 Project, TIABlock Block)
        {
            this.node = Node;
            this.tiaProject = Project;
            this.block = Block;

            var id = this.node.Attributes["id"].Value;
            string instId = this.node.Attributes["instId"].Value;

            var arrayLowerBounds = this.node.SelectSingleNode("attribSet[@id='" + tiaProject.asId2Names.First(itm => itm.Value == "Siemens.Automation.DomainServices.CommonTypeSystem.IStructureItem").Key + "']/attrib[@name='LowerBounds']/array");
            if (arrayLowerBounds != null && (arrayLowerBounds.Attributes["nil"] == null || arrayLowerBounds.Attributes["nil"].Value != "1"))
            {
                ArrayStart = new List<int>();
                ArrayStop = new List<int>();
                var arrayUpperBounds = node.SelectSingleNode("attribSet[@id='" + tiaProject.asId2Names.First(itm => itm.Value == "Siemens.Automation.DomainServices.CommonTypeSystem.IStructureItem").Key + "']/attrib[@name='UpperBounds']/array");
                this.IsArray = true;
                foreach (var low in arrayLowerBounds.InnerText.Split(','))
                {
                    ArrayStart.Add(int.Parse(low));
                }
                foreach (var high in arrayUpperBounds.InnerText.Split(','))
                {
                    ArrayStop.Add(int.Parse(high));
                }

                this.node = Project.xmlDoc.SelectSingleNode("root/objects/StorageObject[parentlink[@link='" + id + "-" + instId + "']]");
                id = this.node.Attributes["id"].Value;
                instId = this.node.Attributes["instId"].Value;
            }

            var idChildRow = Project.importTypeInfos.First(itm => itm.Value == "Siemens.Simatic.PlcLanguages.Model.StructureItemData").Key;
           

            

            var subNodes = Project.xmlDoc.SelectNodes("root/objects/StorageObject[parentlink[@link='" + id + "-" + instId + "']]");
            this.Children = new List<IDataRow>();
            foreach (XmlNode subNode in subNodes)
            {
                if (subNode.Attributes["id"].Value == idChildRow)
                {
                    var row = new TIADataRow(subNode, Project, Block);
                    row.Parent = this;
                    this.Children.Add(row);
                }
            }            
        }

        public uint LocalIdentifier
        {
            get
            {
                var lidNode = node.SelectSingleNode("attribSet[@id='" + tiaProject.asId2Names.First(itm => itm.Value == "Siemens.Automation.DomainModel.ITagAddress").Key + "']/attrib[@name='LocalIdentifier']");
                if (lidNode != null) return Convert.ToUInt32(lidNode.InnerText);
                return 0;
            }
        }

        public override S7DataRowType DataType
        {
            get
            {
                var lidNode = node.SelectSingleNode("attribSet[@id='" + tiaProject.asId2Names.First(itm => itm.Value == "Siemens.Automation.DomainServices.CommonTypeSystem.IStructureItem").Key + "']/attrib[@name='DisplayTypeName']");
                if (lidNode != null)
                {
                    switch (lidNode.InnerText.ToLower())
                    {
                        case "struct":
                            return S7DataRowType.STRUCT;
                        case "block_db":
                            return S7DataRowType.BLOCK_DB;
                        case "word":
                            return S7DataRowType.WORD;
                        case "dword":
                            return S7DataRowType.DWORD;
                        case "real":
                            return S7DataRowType.REAL;
                        case "int":
                            return S7DataRowType.INT;
                        case "dint":
                            return S7DataRowType.DINT;
                        case "bool":
                            return S7DataRowType.BOOL;
                        case "byte":
                            return S7DataRowType.BYTE;
                    }
                }
                return S7DataRowType.UNKNOWN;
            }
        }


        public string Name
        {
            get
            {
                var nameNode = node.SelectSingleNode("attribSet[@id='" + tiaProject.CoreAttributesId + "']/attrib[@name='Name']");
                if (nameNode != null) return nameNode.InnerText;
                return "";
            }
            set
            {
            }
        }
        
        public string SymbolicVisuAccessKey
        {
            get
            {
                //var tiaCrc = Activator.CreateInstance(tiaProject.tiaCrcType);
                //tiaProject.tiaCrcType.InvokeMember("adds", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, tiaCrc, new object[] { Name });
                //return "8a0e" + block.BlockNumber.ToString("X").PadLeft(4, '0') + ((uint)tiaProject.tiaCrcType.InvokeMember("get", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, tiaCrc, null)).ToString("X").PadLeft(8, '0') + "4" + LocalIdentifier.ToString("X").PadLeft(7, '0');
                return "";
            }
        }       
    }
}
