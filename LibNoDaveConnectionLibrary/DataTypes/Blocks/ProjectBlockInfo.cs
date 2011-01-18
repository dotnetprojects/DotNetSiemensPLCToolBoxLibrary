using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public class ProjectBlockInfo
    {
        internal int id;

        public ProjectFolder ParentFolder { get; set; }

        virtual public string Name { get; set;}

        public int BlockNumber { get; set; }
        public PLCBlockType BlockType { get; set; }

        public Block GetBlock()
        {
            IBlocksFolder blkFld = (IBlocksFolder)ParentFolder;
            return blkFld.GetBlock(this);
        }

        public string BlockTypeString
        {
            get
            {
                switch (BlockType)
                {
                    case PLCBlockType.DB:
                        return "Datablock";
                    case PLCBlockType.FB:
                        return "Functionblock";
                    case PLCBlockType.FC:
                        return "Function";
                    case PLCBlockType.OB:
                        return "Organisationblock";
                    case PLCBlockType.UDT:
                        return "Userdatatype";
                    case PLCBlockType.VAT:
                        return "Variabletable";
                    case PLCBlockType.SFC:
                        return "Systemfunction";
                    case PLCBlockType.SFB:
                        return "Systemfunctionblock";
                    case PLCBlockType.SDB:
                        return "Systemdatablock";
                    case PLCBlockType.S5_DB:
                        return "S5-Datablock";
                    case PLCBlockType.S5_FB:
                        return "S5-Functionblock";
                    case PLCBlockType.S5_PB:
                        return "S5-Programblock";
                    case PLCBlockType.S5_FX:
                        return "S5-ExtenedFunctionblock";
                    case PLCBlockType.S5_SB:
                        return "S5-Stepblock";
                    case PLCBlockType.S5_DV:
                        return "S5-Datablock-Preheader";
                    case PLCBlockType.S5_FV:
                        return "S5-Functionblock-Preheader";
                    case PLCBlockType.S5_FVX:
                        return "S5-Extendedfunctionblock-Preheader";
                    case PLCBlockType.S5_DX:
                        return "S5-Extendeddatablock";
                    case PLCBlockType.S5_DVX:
                        return "S5-Extendeddatablock-Preheader";
                    case PLCBlockType.S5_OB:
                        return "S5-Organisationblock";
                    case PLCBlockType.S5_PK:
                        return "S5-Programcommentblock";
                    case PLCBlockType.S5_FK:
                        return "S5-Functioncommentblock";
                    case PLCBlockType.S5_FKX:
                        return "S5-Extendedfunctioncommentblock";
                    case PLCBlockType.S5_SK:
                        return "S5-Stepcommentblock";
                    case PLCBlockType.S5_DK:
                        return "S5-Datacommentblock";
                    case PLCBlockType.S5_DKX:
                        return "S5-Extendeddatacommentblock";
                    case PLCBlockType.S5_OK:
                        return "S5-Organisationcommentblock";
                    case PLCBlockType.S5_BB:
                        return "S5-Variabletable";
                }
                return "";

            }
        }

        public string BlockName
        {
            get
            {
                string retVal = BlockType.ToString().Replace("S5_", "") + BlockNumber.ToString();
                if (Deleted)
                    retVal = "$$_" + retVal;
                return retVal;
            }
        }

        public bool Deleted { get; set; }

        public SymbolTableEntry SymbolTabelEntry
        {
            get
            {
                ISymbolTable tmp = ((IProgrammFolder)ParentFolder.Parent).SymbolTable;
                if (tmp != null)
                    return tmp.GetEntryFromOperand(BlockName);
                return null;
            }
        }
       
        public override string ToString()
        {
            string retVal = "";           
            if (Deleted)
                retVal += "$$_";
            if (SymbolTabelEntry != null)
                retVal += BlockName + " (" + SymbolTabelEntry.Symbol + ")";
            else
                retVal += BlockName;
            return retVal;
        }
    }
}
