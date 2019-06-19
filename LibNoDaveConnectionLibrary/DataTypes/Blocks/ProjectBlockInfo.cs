using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    /// <summary>
    /// This can be Information about a PLC Block (DB, FC,...) or a Block in the Source Folder
    /// </summary>
    public class ProjectBlockInfo : IProjectBlockInfo
    {
       internal int id;
       internal Block _Block = null;

       public ProjectFolder ParentFolder { get; set; }

       public virtual string Name { get; set;}

       public virtual PLCBlockType BlockType { get; set; }

        public virtual bool IsInstance
        {
            get { return false; }
        }

        public virtual Block GetBlock()
       {
           IBlocksFolder blkFld = (IBlocksFolder) ParentFolder;
           return blkFld.GetBlock(this);
       }

        public virtual string GetSourceBlock(bool useSymbols = false)
        {
            if (ParentFolder is BlocksOfflineFolder)
            {
                BlocksOfflineFolder blkFld = (BlocksOfflineFolder) ParentFolder;
                return blkFld.GetSourceBlock(this, useSymbols);
            }
            if (ParentFolder is SourceFolder)
            {
                SourceFolder blkFld = (SourceFolder)ParentFolder;
                return blkFld.GetSource((S7ProjectSourceInfo)this);
            }

            return null;
        }

       public virtual string BlockTypeString
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
                    case PLCBlockType.SourceBlock:
                        return "Sourceblock";
                }
                return "";

            }
        }

        public virtual bool Deleted { get; set; }

        public override string ToString()
        {
            string retVal = Name;
            if (Deleted)
                retVal += "$$_";
            
            return retVal;
        }

        public virtual string Export(ExportFormat exportFormat)
        {
            return null;
        }

        public virtual PLCLanguage BlockLanguage
        {
            get { return GetBlock().BlockLanguage; }
        }
    }
}
