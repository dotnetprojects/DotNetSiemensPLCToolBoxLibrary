using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S5.MC5;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5
{
    class Step5BlocksFolder : ProjectFolder, IBlocksFolder
    {
        internal List<ProjectBlockInfo> step5BlocksinfoList =new List<ProjectBlockInfo>();

        public List<ProjectBlockInfo> readPlcBlocksList(bool useSymbolTable)
        {
            NumericComparer<ProjectBlockInfo> nc = new NumericComparer<ProjectBlockInfo>();
            step5BlocksinfoList.Sort(nc);
            return step5BlocksinfoList;
        }

        public Block GetBlock(string BlockName)
        {
            var tmp = GetBlockInfo(BlockName);
            if (tmp != null)
                return GetBlock(tmp);
            return null;
        }

        internal S5ProjectBlockInfo GetBlockInfo(string BlockName)
        {
            foreach (var projectBlockInfo in step5BlocksinfoList)
            {
                if (BlockName == projectBlockInfo.BlockType.ToString() + projectBlockInfo.BlockNumber.ToString())
                    return (S5ProjectBlockInfo)projectBlockInfo;
            }
            return null;
        }

        internal byte[] GetBlockInByte(string BlockName)
        {
            S5ProjectBlockInfo blkInf = GetBlockInfo(BlockName);
            if (blkInf != null)
                return blkInf._blkByte;
            return null;
        }

        public Block GetBlock(ProjectBlockInfo blkInfo)
        {
            //string aa = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(((Step5ProjectBlockInfo)blkInfo)._blkByte);
            //string bb = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(((Step5ProjectBlockInfo)blkInfo)._blkHeaderByte);
            if (blkInfo.BlockType == PLCBlockType.S5_DV)
                return MC5toDB.GetDB(blkInfo, null, ((S5ProjectBlockInfo)blkInfo)._blkByte, null);
            else if (blkInfo.BlockType == PLCBlockType.S5_DVX)
                return MC5toDB.GetDB(blkInfo, null, ((S5ProjectBlockInfo)blkInfo)._blkByte, null);
            else if (blkInfo.BlockType == PLCBlockType.S5_DB)
                return MC5toDB.GetDB(blkInfo, ((S5ProjectBlockInfo)blkInfo)._blkByte, GetBlockInByte("S5_DV" + blkInfo.BlockNumber.ToString()), GetBlockInByte("S5_DK" + blkInfo.BlockNumber.ToString()));
            else if (blkInfo.BlockType == PLCBlockType.S5_DX)
                return MC5toDB.GetDB(blkInfo, ((S5ProjectBlockInfo)blkInfo)._blkByte, GetBlockInByte("S5_DVX" + blkInfo.BlockNumber.ToString()), GetBlockInByte("S5_DKX" + blkInfo.BlockNumber.ToString()));
            else if (blkInfo.BlockType == PLCBlockType.S5_DK)
                return MC5toComment.GetCommentBlock(blkInfo, ((S5ProjectBlockInfo)blkInfo)._blkByte);
            else if (blkInfo.BlockType == PLCBlockType.S5_DKX)
                return MC5toComment.GetCommentBlock(blkInfo, ((S5ProjectBlockInfo)blkInfo)._blkByte);
            else if (blkInfo.BlockType == PLCBlockType.S5_PB)
                return MC5toAWL.GetFunctionBlock(blkInfo, ((S5ProjectBlockInfo)blkInfo)._blkByte, null, GetBlockInByte("S5_PK" + blkInfo.BlockNumber.ToString()));
            else if (blkInfo.BlockType == PLCBlockType.S5_FB)
                return MC5toAWL.GetFunctionBlock(blkInfo, ((S5ProjectBlockInfo)blkInfo)._blkByte, GetBlockInByte("S5_FV" + blkInfo.BlockNumber.ToString()), GetBlockInByte("S5_FK" + blkInfo.BlockNumber.ToString()));
            else if (blkInfo.BlockType == PLCBlockType.S5_SB)
                return MC5toAWL.GetFunctionBlock(blkInfo, ((S5ProjectBlockInfo)blkInfo)._blkByte, null, GetBlockInByte("S5_SK" + blkInfo.BlockNumber.ToString()));
            else if (blkInfo.BlockType == PLCBlockType.S5_FX)
                return MC5toAWL.GetFunctionBlock(blkInfo, ((S5ProjectBlockInfo)blkInfo)._blkByte, GetBlockInByte("S5_FVX" + blkInfo.BlockNumber.ToString()), GetBlockInByte("S5_FKX" + blkInfo.BlockNumber.ToString()));
            else if (blkInfo.BlockType == PLCBlockType.S5_OB)
                return MC5toAWL.GetFunctionBlock(blkInfo, ((S5ProjectBlockInfo)blkInfo)._blkByte, null, GetBlockInByte("S5_OK" + blkInfo.BlockNumber.ToString()));
            
            return null;            
        }
    }
}
