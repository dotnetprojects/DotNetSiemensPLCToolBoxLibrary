using System;
using System.Text;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public class S7VATBlock : S7Block
    {
        public List<PLCTag> Rows{ get; set;}
        public List<S7VATRow> VATRows { get; set; }

        public S7VATBlock(byte[] hexCode, byte[] comments, int blocknumber, Encoding projEncoding)
        {
            BlockType = PLCBlockType.VAT;
            //BlockLanguage=
            BlockNumber = blocknumber;
            //Every Vats starts with:
            //0001 31 20 32 20 30 20 32 20 31 20 31 30 20 30 20 32    1 2 0 2 1 10 0 2
            //0010 30 20 31 20 33 30 20 31 20 34 30 20 31 20 35 30    0 1 30 1 40 1 50
            //0020 20 36 30 00 32 20 33 20 38 38 20 30 20 30 20 30     60.

            Rows = new List<PLCTag>();
            VATRows = new List<S7VATRow>();

            int akAddr = 36;

            while (akAddr < hexCode.Length)
            {
                if (hexCode[akAddr] != 0x00)
                {
                    PLCTag tmp = new PLCTag();
                    string var = "";
                    string type = "";
                    string addr = "";
                    string controlval = "";
                    string db = "";
                    string showtype = "";
                    bool bit = false;

                    switch (hexCode[akAddr])
                    {
                        case 0x32:
                            var = "E";
                            tmp.LibNoDaveDataSource = TagDataSource.Inputs;
                            break;
                        case 0x33:
                            var = "A";
                            tmp.LibNoDaveDataSource = TagDataSource.Outputs;
                            break;
                        case 0x34:
                            tmp.LibNoDaveDataSource = TagDataSource.Flags;
                            var = "M";
                            break;
                        case 0x35:
                            tmp.LibNoDaveDataSource = TagDataSource.Datablock;
                            var = "DB";
                            db = "DB";
                            break;
                        case 0x38:
                            tmp.LibNoDaveDataSource = TagDataSource.Timer;
                            var = "T";
                            break;
                        case 0x39:
                            tmp.LibNoDaveDataSource = TagDataSource.Counter;
                            var = "Z";
                            break;
                        default:
                            var = "  0x" + hexCode[akAddr].ToString("X") + "  ";
                            break;
                    }
                    akAddr++;
                    akAddr++;
                    switch (hexCode[akAddr])
                    {
                        case 0x31:
                            if (db != "")
                                var += "X";
                            tmp.LibNoDaveDataType = TagDataType.Bool;
                            bit = true;
                            break;
                        case 0x32:
                            var += "B";
                            tmp.LibNoDaveDataType = TagDataType.Byte;
                            break;
                        case 0x33:
                            tmp.LibNoDaveDataType = TagDataType.Word;
                            var += "W";
                            break;
                        case 0x34:
                            tmp.LibNoDaveDataType = TagDataType.Dword;
                            var += "D";
                            break;
                        default:
                            var = "  0x" + hexCode[akAddr].ToString("X") + "  ";
                            break;
                    }
                    akAddr++;
                    akAddr++;
                    while (hexCode[akAddr] != 0x20)
                    {
                        addr += hexCode[akAddr] - 0x30;
                        akAddr++;
                    }
                    tmp.ByteAddress = Convert.ToInt32(addr);
                    akAddr++;
                    if (bit == true)
                    {
                        tmp.BitAddress = Convert.ToInt32(hexCode[akAddr] - 0x30);
                    }
                    akAddr++;
                    akAddr++;
                    if (db != "")
                    {
                        addr = "";
                        while (hexCode[akAddr] != 0x20)
                        {
                            addr += hexCode[akAddr] - 0x30;
                            akAddr++;
                        }
                        tmp.DatablockNumber = Convert.ToInt32(addr);
                    }
                    akAddr++;
                    akAddr++;
                    //0x30
                    akAddr++;
                    akAddr++;
                    //again
                    akAddr++;
                    akAddr++;
                    //again size
                    akAddr++;
                    akAddr++;
                    //0x30
                    akAddr++;
                    akAddr++;
                    //0x31
                    akAddr++;
                    akAddr++;
                    akAddr++;
                    akAddr++;
                    akAddr++;
                    //View Sign
                    switch (hexCode[akAddr])
                    {
                        case 0x30:
                            if (hexCode[akAddr - 1] == 0x31)
                            {
                                //showtype = "ZAEHLER";
                                tmp.LibNoDaveDataType = TagDataType.BCDWord;
                                tmp.DataTypeStringFormat = TagDisplayDataType.Decimal;
                            }
                            else
                            {
                                //showtype = "BIN";
                                tmp.DataTypeStringFormat = TagDisplayDataType.Binary;
                            }
                            break;
                        case 0x31:
                            if (hexCode[akAddr - 1] == 0x31)
                            {
                                //showtype = "ZEIGER";
                                tmp.DataTypeStringFormat = TagDisplayDataType.Pointer;
                            }
                            else
                            {
                                //showtype = "HEX";
                                tmp.DataTypeStringFormat = TagDisplayDataType.Hexadecimal;
                            }
                            break;
                        case 0x32:
                            //showtype = "DEZ";
                            tmp.DataTypeStringFormat = TagDisplayDataType.Decimal;
                            break;
                        case 0x33:
                            //showtype = "GLEITPUNKT";
                            tmp.LibNoDaveDataType = TagDataType.Float;
                            tmp.DataTypeStringFormat = TagDisplayDataType.Float;
                            break;
                        case 0x34:
                            //showtype = "ZEICHEN";
                            tmp.LibNoDaveDataType = TagDataType.Byte;
                            tmp.DataTypeStringFormat = TagDisplayDataType.String;
                            break;
                        case 0x35:
                            //showtype = "BOOL";
                            tmp.DataTypeStringFormat = TagDisplayDataType.Bool;
                            break;
                        case 0x36:
                            //showtype = "ZEIT";
                            tmp.LibNoDaveDataType = TagDataType.Time;
                            tmp.DataTypeStringFormat = TagDisplayDataType.Time;
                            break;
                        case 0x37:
                            //showtype = "DATUM";
                            tmp.LibNoDaveDataType = TagDataType.Date;
                            tmp.DataTypeStringFormat = TagDisplayDataType.S7Date;
                            break;
                        case 0x38:
                            tmp.LibNoDaveDataType = TagDataType.S5Time;
                            tmp.DataTypeStringFormat = TagDisplayDataType.S5Time;
                            //showtype = "SIMATIC_ZEIT";
                            break;
                        case 0x39:
                            tmp.LibNoDaveDataType = TagDataType.TimeOfDay;
                            tmp.DataTypeStringFormat = TagDisplayDataType.S7TimeOfDay;
                            //showtype = "TAGESZEIT";
                            break;
                        default:
                            var = "  0x" + hexCode[akAddr].ToString("X") + "  ";
                            break;
                    }
                    akAddr++;
                    while (hexCode[akAddr] != 0x00)
                    {
                        controlval += System.Text.Encoding.ASCII.GetString(new byte[] { hexCode[akAddr] });
                        akAddr++;
                    }
                    tmp.ParseControlValueFromString(controlval);

                    akAddr++;
                    Rows.Add(tmp);
                    VATRows.Add(new S7VATRow() { LibNoDaveValue = tmp });

                }
                else
                {
                    akAddr++;
                    VATRows.Add(new S7VATRow() {});
                }
            }

            //string aa = System.Text.Encoding.ASCII.GetString(comments);

            int commentsRow = 12;
            int akLine = -1;
            while (commentsRow<comments.Length)
            {
                int akLen = comments[commentsRow] + comments[commentsRow + 1]*0x100;
                int akLinePlus = comments[commentsRow + 2] + comments[commentsRow + 3]*0x100;
                akLine += akLinePlus;

                VATRows[akLine].Comment = projEncoding.GetString(comments, commentsRow + 6, akLen);

                commentsRow += 6 + akLen;
                akLine++;
            }

        }

        public override string ToString()
        {
            string retVal = "";
            foreach (var akRow in VATRows)
            {
                if (akRow.LibNoDaveValue != null)
                {
                    retVal += akRow.LibNoDaveValue.ToString() + "; " + akRow.LibNoDaveValue.LibNoDaveDataType.ToString() + "; " + akRow.LibNoDaveValue.DataTypeStringFormat.ToString();
                    if (akRow.LibNoDaveValue.Controlvalue != null)
                        retVal += "; " + akRow.LibNoDaveValue.Controlvalue;
                }
                if (!string.IsNullOrEmpty(akRow.Comment))
                    retVal += "//" + akRow.Comment;
                retVal += "\r\n";
            }
            return retVal;
        }
    }
}
