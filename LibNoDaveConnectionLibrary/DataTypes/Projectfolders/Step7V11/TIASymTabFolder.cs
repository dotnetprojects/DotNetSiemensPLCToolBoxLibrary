using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V11;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class TIASymTabFolder : TIAProjectFolder, ISymbolTable
    {
        private List<SymbolTableEntry> symbolTableEntrys;

        public String Folder { get; set; }

        public SymbolTableEntry GetEntryFromOperand(string operand)
        {
            throw new NotImplementedException();
        }

        public SymbolTableEntry GetEntryFromSymbol(string symbol)
        {
            throw new NotImplementedException();
        }

        public List<SymbolTableEntry> SymbolTableEntrys
        {
            get
            {
                if (this.symbolTableEntrys == null)
                {
                    symbolTableEntrys = new List<SymbolTableEntry>();

                    foreach (XmlNode mySymTableEntry in SubNodes)
                    {
                        var akSymName = mySymTableEntry.SelectSingleNode("attribSet[@id='" + TiaProject.CoreAttributesId + "']/attrib[@name='Name']").InnerText;

                        string akSymAddress = "";
                        
                        var nd = mySymTableEntry.SelectSingleNode("attribSet[@id='" + TiaProject.asId2Names.First(itm => itm.Value == "Siemens.Automation.DomainModel.ITagAddress").Key + "']/attrib[@name='LogicalAddress']");
                        if (nd != null) akSymAddress = nd.InnerText;

                        UInt32 akLitID = 0;
                        nd = mySymTableEntry.SelectSingleNode("attribSet[@id='" + TiaProject.asId2Names.First(itm => itm.Value == "Siemens.Automation.DomainModel.ITagAddress").Key + "']/attrib[@name='LocalIdentifier']");
                        if (nd != null) akLitID = Convert.ToUInt32(nd.InnerText);

                        string akSymType = "";
                        nd = mySymTableEntry.SelectSingleNode("attribSet[@id='" + TiaProject.asId2Names.First(itm => itm.Value == "Siemens.Automation.DomainServices.CommonTypeSystem.IStructureItem").Key + "']/attrib[@name='DisplayTypeName']");
                        if (nd != null) akSymType = nd.InnerText;

                        var entry = new TIASymbolTableEntry() { Symbol = akSymName, OperandIEC = akSymAddress, DataType = akSymType };
                        symbolTableEntrys.Add(entry);

                        var tiaCrc = Activator.CreateInstance(TiaProject.tiaCrcType);
                        TiaProject.tiaCrcType.InvokeMember("adds", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, tiaCrc, new object[] { akSymName });
                        entry.TIATagAccessKey = "00000052" + ((uint)TiaProject.tiaCrcType.InvokeMember("get", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, tiaCrc, null)).ToString("X").PadLeft(8,'0') + "4" + akLitID.ToString("X").PadLeft(7, '0');                        
                    }
                }
                return this.symbolTableEntrys;
            }
            set
            {
                this.symbolTableEntrys = value;
            }
        }

        public TIASymTabFolder(Step7ProjectV11 Project, XmlNode Node)
            : base(Project, Node)
        {
            //SymbolTableEntrys = new List<SymbolTableEntry>();
        }
    }
}
