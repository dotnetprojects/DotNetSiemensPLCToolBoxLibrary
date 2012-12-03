using System;
using System.Collections.Generic;
using System.ComponentModel;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    public class OnlineBlocksFolder : ProjectFolder, IBlocksFolder
    {
        public PLCConnection Connection;

        public bool IsOnline { get; set; }

        public OnlineBlocksFolder(string ConnectionName)
        {
            Connection = new PLCConnection(ConnectionName);
            this.Name = "Online BlockInfos (" + Connection.Configuration.ConnectionName + ")";
        }

        public OnlineBlocksFolder(PLCConnection conn)
        {
            Connection = conn;
            this.Name = "Online BlockInfos (" + Connection.Configuration.ConnectionName + ")";
        }

        public void UploadBlock(Block myBlk)
        {

        }

        public List<ProjectBlockInfo> readPlcBlocksList()
        {
            SymbolTable symtab = null;
            if (((S7ProgrammFolder)Parent) != null)
                symtab = (SymbolTable)((S7ProgrammFolder) Parent).SymbolTable;

            List<ProjectBlockInfo> retVal=new List<ProjectBlockInfo>();

            Connection.Connect();
            List<string> blks = Connection.PLCListBlocks(PLCBlockType.AllBlocks);

            foreach (var blk in blks)
            {
                ProjectPlcBlockInfo tmp = new ProjectPlcBlockInfo();
                tmp.ParentFolder = this;
                if (blk.Substring(0, 2) == "DB")
                {
                    tmp.BlockType = PLCBlockType.DB;
                    tmp.BlockNumber = Convert.ToInt32(blk.Substring(2));
                }
                else if (blk.Substring(0, 2) == "OB")
                {
                    tmp.BlockType = PLCBlockType.OB;
                    tmp.BlockNumber = Convert.ToInt32(blk.Substring(2));
                }
                else if (blk.Substring(0, 2) == "FC")
                {
                    tmp.BlockType = PLCBlockType.FC;
                    tmp.BlockNumber = Convert.ToInt32(blk.Substring(2));
                }
                else if (blk.Substring(0, 2) == "FB")
                {
                    tmp.BlockType = PLCBlockType.FB;
                    tmp.BlockNumber = Convert.ToInt32(blk.Substring(2));
                }
                else if (blk.Substring(0, 3) == "SFC")
                {
                    tmp.BlockType = PLCBlockType.SFC;
                    tmp.BlockNumber = Convert.ToInt32(blk.Substring(3));
                }
                else if (blk.Substring(0, 3) == "SFB")
                {
                    tmp.BlockType = PLCBlockType.SFB;
                    tmp.BlockNumber = Convert.ToInt32(blk.Substring(3));
                }
                else if (blk.Substring(0, 3) == "SDB")
                {
                    tmp.BlockType = PLCBlockType.SDB;
                    tmp.BlockNumber = Convert.ToInt32(blk.Substring(3));
                }

                /*
                if (symtab != null)
                {
                    SymbolTableEntry sym = symtab.GetEntryFromOperand(tmp.ToString());
                    if (sym != null)
                        tmp.Symbol = sym.Symbol;
                }
                */
                retVal.Add(tmp);
            }


            return retVal;
        }

        public List<ProjectBlockInfo> BlockInfos
        {
            get { return readPlcBlocksList(); }
        }

        public Block GetBlock(string BlockName)
        {
            byte[] tmp = Connection.PLCGetBlockInMC7(BlockName);
            return MC7Converter.GetAWLBlock(tmp, Project != null ? (int)Project.ProjectLanguage : 0, Parent as S7ProgrammFolder);
        }

        public Block GetBlock(ProjectBlockInfo blkInfo)
        {
            return GetBlock(((ProjectPlcBlockInfo)blkInfo).BlockName);
        }
    }
}
