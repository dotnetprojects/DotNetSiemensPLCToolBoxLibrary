using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class SourceFolder : Step7ProjectFolder, IBlocksFolder
    {
        public string Folder { get; set; }

        public List<ProjectBlockInfo> readPlcBlocksList()
        {
            bool showDeleted = ((Step7ProjectV5)this.Project)._showDeleted;

            List<ProjectBlockInfo> tmpBlocks = new List<ProjectBlockInfo>();

            if (((Step7ProjectV5)Project)._ziphelper.FileExists(Folder + "S7CONTAI.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(Folder + "S7CONTAI.DBF", ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || showDeleted)
                    {
                        S7ProjectSourceInfo tmp = new S7ProjectSourceInfo();
                        tmp.Deleted = (bool)row["DELETED_FLAG"];
                        tmp.Name = (string)row["NAME"];
                        tmp.Filename = Folder + (string)row["FILENAME"];
                        tmp.ParentFolder = this;

                        tmpBlocks.Add(tmp);
                    }
                }
            }
            return tmpBlocks;
        }

        public List<ProjectBlockInfo> BlockInfos
        {
            get { return readPlcBlocksList(); }
        }

        public ProjectBlockInfo GetProjectBlockInfoFromBlockName(string BlockName)
        {
            var tmp = readPlcBlocksList();
            foreach (ProjectPlcBlockInfo step7ProjectBlockInfo in tmp)
            {
                if (step7ProjectBlockInfo.BlockType.ToString() + step7ProjectBlockInfo.BlockNumber.ToString() == BlockName.ToUpper())
                    return step7ProjectBlockInfo;
            }
            return null;
        }

        public Block GetBlock(string BlockName)
        {
            var prjBlkInf = GetProjectBlockInfoFromBlockName(BlockName);
            if (prjBlkInf != null)
                return GetBlock(prjBlkInf);
            return null;
        }

        public Block GetBlock(ProjectBlockInfo blkInfo)
        {
            S7ProjectSourceInfo srcInfo = (S7ProjectSourceInfo)blkInfo;

            S7SourceBlock retVal = new S7SourceBlock();

            retVal.Text = GetSource(srcInfo);

            retVal.Name = srcInfo.Name;
            retVal.ParentFolder = srcInfo.ParentFolder;
            retVal.Filename = srcInfo.Filename;
            retVal.Comment = getBetween(retVal.Text, "Comment '", "'");

            return retVal;
        }

        public string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        public string GetSource(S7ProjectSourceInfo blkInfo)
        {
            if (((Step7ProjectV5)Project)._ziphelper.FileExists(blkInfo.Filename))
            {
                using (Stream strm = ((Step7ProjectV5)Project)._ziphelper.GetReadStream(blkInfo.Filename))
                    return new System.IO.StreamReader(strm, Encoding.UTF7).ReadToEnd();
            }

            return null;
        }
    }
}