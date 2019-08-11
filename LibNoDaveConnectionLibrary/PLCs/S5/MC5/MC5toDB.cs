using System;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S5.MC5
{
    using System.Collections.Generic;

    public static class MC5toDB
    {
        public static S5DataBlock GetDB(ProjectPlcBlockInfo blkInfo, byte[] block, byte[] preHeader, byte[] commentBlock)
        {
            S5DataBlock retVal = new S5DataBlock();

            retVal.BlockType = blkInfo.BlockType;
            retVal.BlockNumber = blkInfo.BlockNumber;

            S7DataRow main = new S7DataRow("STATIC", S7DataRowType.STRUCT, retVal);
            retVal.Structure = main;

            var zeilenListe = new List<S7DataRow>();

            if (preHeader != null)
            {
                int akcnt = 0;
                //S7DataRowType akRwTp = (S7DataRowType) (preHeader[9] | 0xf00);
                int anzTypes = (((preHeader[6] * 256 + preHeader[7]) - 2)/2); //How many different Types are in the Header
                for (int n = 1; n <= anzTypes; n++)
                {
                    int rowStart = preHeader[(n - 1) * 4 + 10] * 256 + preHeader[(n - 1) * 4 + 11];
                    int rowStop = preHeader[n * 4 + 10] * 256 + preHeader[n * 4 + 11];
                    var akRwTp = (S7DataRowType)(preHeader[9 + (n - 1) * 4] | 0xf00);
                    

                    if (akRwTp == S7DataRowType.S5_C || akRwTp == S7DataRowType.S5_KC)
                    {
                        for (int j = rowStart; j < rowStop; j += 24)
                        {
                            var row = new S7DataRow(string.Empty, akRwTp, retVal);
                            row.StringSize = rowStop - j > 12 ? 24 : (rowStop - j)*2;
                            main.Add(row);
                            for (int q = 0; q < (row.StringSize/2); q++)
                            {
                                zeilenListe.Add(row);
                            }
                        }
                    }
                    else if (akRwTp == S7DataRowType.S5_KG)
                    {
                        for (int j = rowStart; j < rowStop; j += 2)
                        {
                            var row = new S7DataRow(string.Empty, akRwTp, retVal);
                            main.Add(row);
                            zeilenListe.Add(row);
                            zeilenListe.Add(row);
                        }
                    }
                    else
                    {
                        for (int j = rowStart; j < rowStop; j += 1)
                        {
                            var row = new S7DataRow(string.Empty, akRwTp, retVal);
                            main.Add(row);
                            zeilenListe.Add(row);
                        }
                    }                                                           
                }

            }

            
            try
            {
                int st = 10;

                int maxZ = (block[8] * 256 + block[9]);
                int n = 0;

                while(n < maxZ - 5)
                    //foreach (var s7DataRow in main.Children)
                {
                    S7DataRow s7DataRow;
                    if (zeilenListe.Count > n) 
                        s7DataRow = zeilenListe[n];
                    else
                    {
                        s7DataRow = new S7DataRow(string.Empty, S7DataRowType.S5_KH, retVal);
                        main.Add(s7DataRow);
                    }

                    switch (s7DataRow.DataType)
                    {
                        case S7DataRowType.S5_KF:
                            s7DataRow.Value = libnodave.getS16from(block, st);
                            break;
                        case S7DataRowType.S5_KZ:
                            s7DataRow.Value = libnodave.getBCD16from(block, st);
                            break;
                        case S7DataRowType.S5_KM:
                            s7DataRow.Value = libnodave.getU16from(block, st);
                            break;
                        case S7DataRowType.S5_KH:
                            s7DataRow.Value = libnodave.getU16from(block, st);
                            break;
                        case S7DataRowType.S5_KY:
                            s7DataRow.Value = libnodave.getU16from(block, st);
                            break;
                        case S7DataRowType.S5_KG:
                            s7DataRow.Value = libnodave.getS5Floatfrom(block, st);
                            break;
                        case S7DataRowType.S5_KC:
                        case S7DataRowType.S5_C:
                            s7DataRow.Value = System.Text.ASCIIEncoding.ASCII.GetString(block, st, s7DataRow.StringSize);
                            break;
                        case S7DataRowType.S5_KT:
                            s7DataRow.Value = libnodave.getS5Timefrom(block, st);
                            break;
                        default:
                            s7DataRow.Value = libnodave.getU16from(block, st);                        
                            break;
                    }

                    st += s7DataRow.ByteLength;
                    n += s7DataRow.ByteLength / 2;
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
                        int zeile = ((commentBlock[nr + 1] - 128) * 256) + commentBlock[nr];
                        int len = 0x7f & commentBlock[nr + 2];
                        string cmt = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(commentBlock, nr + 3, len);
                        //main._children[zeile].Comment = cmt;
                        zeilenListe[zeile].Comment = cmt;

                        nr += len + 3;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error parsing the Block Comments! Maybe the Step5 project is broken? \n", ex);
            }

            return retVal;
        }
    }
}
