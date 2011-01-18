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
        public PLCConnectionConfiguration ConnectionConfig { get; set;}
        private PLCConnection myConn;

        public bool IsOnline { get; set; }

        public OnlineBlocksFolder(string ConnectionName)
        {
            ConnectionConfig=new PLCConnectionConfiguration(ConnectionName);
            this.Name = "Online Blocks (" + ConnectionConfig.ConnectionName + ")";
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

            myConn=new PLCConnection(ConnectionConfig);
            myConn.Connect();
            List<string> blks = myConn.PLCListBlocks(PLCBlockType.AllBlocks);

            foreach (var blk in blks)
            {
                ProjectBlockInfo tmp = new ProjectBlockInfo();
                tmp.ParentFolder = this;
                if (blk.Substring(0, 2) == "DB")
                {
                    tmp.BlockType = PLCBlockType.DB;
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

        public Block GetBlock(string BlockName)
        {
            byte[] tmp=myConn.PLCGetBlockInMC7(BlockName);
            return MC7Converter.GetAWLBlock(tmp, 0, (S7ProgrammFolder) Parent);
        }

        public Block GetBlock(ProjectBlockInfo blkInfo)
        {
            return GetBlock(blkInfo.BlockName);
        }       
    }
}
