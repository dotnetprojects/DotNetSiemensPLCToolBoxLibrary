using System;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S5.MC5
{
    public static class MC5toDB
    {
        public static S5DataBlock GetDB(ProjectPlcBlockInfo blkInfo, byte[] block, byte[] preHeader, byte[] commentBlock)
        {
            S5DataBlock retVal = new S5DataBlock();

            retVal.BlockType = blkInfo.BlockType;
            retVal.BlockNumber = blkInfo.BlockNumber;

            S7DataRow main = new S7DataRow("STATIC", S7DataRowType.STRUCT, retVal);
            retVal.Structure = main;

            if (preHeader != null)
            {
                int akcnt = 0;
                S7DataRowType akRwTp = (S7DataRowType) (preHeader[9] | 0xf00);
                int anzTypes = (((preHeader[6] * 256 + preHeader[7]) - 2)/2); //How many different Types are in the Header
                for (int n = 1; n <= anzTypes; n++)
                {
                    if (n == anzTypes)
                    {
                        int rowcnt =preHeader[n*4 + 10] * 256 + preHeader[n*4 + 11];
                        int crcnt = rowcnt - akcnt;
                        if (akRwTp == S7DataRowType.S5_KG || akRwTp == S7DataRowType.S5_C || akRwTp == S7DataRowType.S5_KC)
                            crcnt = crcnt/2;
                        for (int p = 0; p < crcnt; p++)
                        {
                            S7DataRow addRw = new S7DataRow("", akRwTp, retVal);
                            main.Add(addRw);
                        }
                    }
                    else
                    {
                        int rowcnt = preHeader[n * 4 + 10] * 256 + preHeader[n * 4 + 11];
                        akcnt = rowcnt;
                        if (akRwTp == S7DataRowType.S5_KG || akRwTp == S7DataRowType.S5_C || akRwTp == S7DataRowType.S5_KC)
                            rowcnt = rowcnt / 2;
                        
                        for (int p = akcnt; p < rowcnt; p++)
                        {
                            S7DataRow addRw = new S7DataRow("", akRwTp, retVal);
                            main.Add(addRw);
                        }
                        
                        akRwTp = (S7DataRowType) (preHeader[9 + n*4] | 0xf00);
                    }
                }

            }

            try
            {
                int st = 10;

                for (int n = 0; n < (block[8]*256 + block[9]) - 5; n++)
                    //foreach (var s7DataRow in main.Children)
                {
                    if (main.Children.Count <= n)
                    {
                        var addVal = new S7DataRow("", S7DataRowType.S5_KH, retVal);
                        main.Add(addVal);
                    }
                    var s7DataRow = main.Children[n];
                    switch (s7DataRow.DataType)
                    {
                        case S7DataRowType.S5_KF:
                            s7DataRow.Value = libnodave.getS16from(block, st);
                            st += 2;
                            break;
                        case S7DataRowType.S5_KM:
                            s7DataRow.Value = libnodave.getU16from(block, st);
                            st += 2;
                            break;
                        case S7DataRowType.S5_KH:
                            s7DataRow.Value = libnodave.getU16from(block, st);
                            st += 2;
                            break;
                        case S7DataRowType.S5_KY:
                            s7DataRow.Value = libnodave.getU16from(block, st);
                            st += 2;
                            break;
                        case S7DataRowType.S5_KG:
                            s7DataRow.Value = libnodave.getFloatfrom(block, st);
                            st += 4;
                            break;
                        case S7DataRowType.S5_C:
                        case S7DataRowType.S5_KC:
                            s7DataRow.Value = ((char) block[st]).ToString() + ((char) block[st + 1]).ToString() + ((char) block[st + 2]).ToString() + ((char) block[st + 3]).ToString();
                            st += 4;
                            break;
                        case S7DataRowType.S5_KT:
                            s7DataRow.Value = libnodave.getS5Timefrom(block, st);
                            st += 2;
                            break;
                        default:
                            s7DataRow.Value = libnodave.getU16from(block, st);
                            st += 2;
                            break;
                    }
                }
            }
            catch (Exception ex)
            { }


            try
            {
                if (commentBlock != null && main._children != null && main._children.Count > 0)
                {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error parsing the Block Comments! Maybe the Step5 project is broken? \n" + ex.Message);
            }

            return retVal;
        }
    }
}
