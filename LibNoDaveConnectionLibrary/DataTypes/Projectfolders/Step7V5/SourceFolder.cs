using System.Collections.Generic;
using System.Data;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class SourceFolder : Step7ProjectFolder
    {
        public string Folder { get; set; }

        public List<Step7ProjectSourceInfo> readPlcBlocksList(bool showDeleted)
        {
            List<Step7ProjectSourceInfo> tmpBlocks = new List<Step7ProjectSourceInfo>();
            if (System.IO.File.Exists(Folder + "S7CONTAI.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(Folder + "S7CONTAI.DBF");
                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || showDeleted)
                    {

                        Step7ProjectSourceInfo tmp = new Step7ProjectSourceInfo();
                        tmp.Deleted = (bool)row["DELETED_FLAG"];
                        tmp.Name = (string)row["NAME"];
                        tmp.Filename = Folder + (string)row["FILENAME"];

                        tmpBlocks.Add(tmp);
                    }
                }
            }
            return tmpBlocks;
        }
    }
}
