using System;
using System.Collections.Generic;
using System.Text;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;
using LibNoDaveConnectionLibrary.DataTypes.Blocks.Step5;

namespace LibNoDaveConnectionLibrary.PLCs.S5.MC5
{
    public static class MC5toDB
    {
        public static S5DataBlock GetDB(ProjectBlockInfo blkInfo, byte[] block, byte[] preHeader, byte[] commentBlock)
        {
            S5DataBlock retVal = new S5DataBlock();

            retVal.BlockType = blkInfo.BlockType;
            retVal.BlockNumber = blkInfo.BlockNumber;

            PLCDataRow main = new PLCDataRow("STATIC", PLCDataRowType.STRUCT, retVal);
            retVal.Structure = main;

            if (preHeader != null)
            {
                int akcnt = 0;
                PLCDataRowType akRwTp = (PLCDataRowType) (preHeader[9] | 0xf00);
                int anzTypes = ((preHeader[7] - 2)/2); //How many different Types are in the Header
                for (int n = 1; n <= anzTypes; n++)
                {
                    if (n == anzTypes)
                    {
                        int rowcnt = preHeader[n*4 + 11];
                        int crcnt = rowcnt - akcnt;
                        for (int p = 0; p < crcnt; p++)
                        {
                            PLCDataRow addRw = new PLCDataRow("", akRwTp, retVal);
                            main.Add(addRw);
                        }
                    }
                    else
                    {
                        int rowcnt = preHeader[n*4 + 11];
                        for (int p = akcnt; p < rowcnt; p++)
                        {
                            PLCDataRow addRw = new PLCDataRow("", akRwTp, retVal);
                            main.Add(addRw);
                        }
                        akcnt = rowcnt;
                        akRwTp = (PLCDataRowType) (preHeader[9 + n*4] | 0xf00);
                    }
                }

            }

            if (commentBlock != null &&  main._children != null && main._children.Count > 0)
            {
                string aa = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(commentBlock);
           
                int nr = 28;
                int hdlen = 0x7f & commentBlock[nr];

                retVal.Name = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(commentBlock, nr + 1, hdlen);
           
                nr += hdlen + 1;
                while (nr + 3 < commentBlock.Length)
                {
                    int zeile = commentBlock[nr];
                    int len = 0x7f & commentBlock[nr + 2];
                    string cmt = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(commentBlock, nr + 3, len);
                    main._children[zeile].Comment = cmt;

                    nr += len + 3;
                }

            }
            return retVal;
        }
    }
}
