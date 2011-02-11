using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class SourceFolder : Step7ProjectFolder,IBlocksFolder
    {
        public string Folder { get; set; }

        public List<ProjectBlockInfo> readPlcBlocksList()
        {
            bool showDeleted = ((Step7ProjectV5)this.Project)._showDeleted;

            List<ProjectBlockInfo> tmpBlocks = new List<ProjectBlockInfo>();

            if (ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "S7CONTAI.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(Folder + "S7CONTAI.DBF", ((Step7ProjectV5)Project)._zipfile,((Step7ProjectV5)Project)._DirSeperator);
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

        public Block GetBlock(string BlockName)
        {
           

            throw new NotImplementedException();
        }

        public Block GetBlock(ProjectBlockInfo blkInfo)
        {
            S7ProjectSourceInfo srcInfo = (S7ProjectSourceInfo)blkInfo;

            S7SourceBlock retVal = new S7SourceBlock();

            if (ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, srcInfo.Filename))
            {
                Stream strm = ZipHelper.GetReadStream(((Step7ProjectV5)Project)._zipfile, srcInfo.Filename);

                retVal.Text = new System.IO.StreamReader(strm,Encoding.UTF7).ReadToEnd();
                //ReadToEnd();
            }

            retVal.Name = srcInfo.Name;
            retVal.ParentFolder = srcInfo.ParentFolder;
            retVal.Filename = srcInfo.Filename;

            return retVal;
        }
    }
}
