/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 WPFToolboxForSiemensPLCs is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 WPFToolboxForSiemensPLCs is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    //internal 
    public static class Helper
    {

        public static string MemoryAreaToPointer(MemoryArea area, MnemonicLanguage language)
        {
            switch (area)
            {
                case MemoryArea.Inputs:
                    return Mnemonic.adE[(int)language];
                case MemoryArea.Outputs:
                    return Mnemonic.adA[(int)language];
                case MemoryArea.Timer:
                    return Mnemonic.adT[(int)language];
                case MemoryArea.Flags:
                    return Mnemonic.adM[(int)language];
                case MemoryArea.Counter:
                    return Mnemonic.adZ[(int)language];
                case MemoryArea.Datablock:
                    return Mnemonic.adDBX[(int)language];
                case MemoryArea.InstanceDatablock:
                    return Mnemonic.adDIX[(int)language];
            }
            return "";
        }

        public static ByteBitAddress GetNextBitAddress(ByteBitAddress tmp)
        {
            if (tmp.BitAddress > 7)
                throw new Exception("Unpossible ByteBitAddress specified");
            if (tmp.BitAddress == 7)
                return new ByteBitAddress(tmp.ByteAddress + 1, 0);
            else
                return new ByteBitAddress(tmp.ByteAddress, tmp.BitAddress + 1);
        }

        public static byte[] CombineByteArray(byte[] b1, byte[] b2)
        {
            int len = 0;
            if (b1 != null)
                len += b1.Length;
            if (b2 != null)
                len += b2.Length;

            byte[] res = new byte[len];

            if (b1 != null)
                Array.Copy(b1, res, b1.Length);
            if (b1 != null && b2 != null)
                Array.Copy(b2, 0, res, b1.Length, b2.Length);
            else if (b2 != null)
                Array.Copy(b2, res, b2.Length);
            return res;
        }

        public static object StringValueToObject(string Value, S7DataRowType DataType)
        {
            if (DataType == S7DataRowType.WORD)
                return Convert.ToUInt16(Helper.GetUIntFromHexString(Value));
            else if (DataType == S7DataRowType.BOOL)
                return bool.Parse(Value);
            else if (DataType == S7DataRowType.DWORD)
                return Convert.ToUInt32(Helper.GetUIntFromHexString(Value));
            else if (DataType == S7DataRowType.BYTE)
                return Convert.ToByte(Helper.GetUIntFromHexString(Value));
            else if (DataType == S7DataRowType.INT)
                return Int16.Parse(Value);
            else if (DataType == S7DataRowType.DINT)
                return Int32.Parse(Value.Replace("L#", ""));
            else if (DataType == S7DataRowType.REAL) 
                return float.Parse(Value.Replace('.',','));
            else if (DataType == S7DataRowType.S5TIME)
                return Helper.GetTimespanFromS5TimeorTime(Value);
            else if (DataType == S7DataRowType.TIME)
                return Helper.GetTimespanFromS5TimeorTime(Value);
            else if (DataType == S7DataRowType.DATE)
                return Helper.GetDateTimeFromDateString(Value);
            else if (DataType == S7DataRowType.TIME_OF_DAY)
                return Helper.GetDateTimeFromTimeOfDayString(Value);
            else if (DataType == S7DataRowType.CHAR)
                return Value.ToString()[1];
            else if (DataType == S7DataRowType.DATE_AND_TIME)
                return Helper.GetDateTimeFromDateAndTimeString(Value);
            else if (DataType == S7DataRowType.STRING)
                return Value.ToString();
            else if (DataType == S7DataRowType.STRUCT)
                return null;
            else if (DataType == S7DataRowType.UDT)
                return null;

            return null;
        }

        public static object DefaultValueForType(S7DataRowType DataType)
        {
            if (DataType == S7DataRowType.WORD)
                return (UInt16)0;
            else if (DataType == S7DataRowType.BOOL)
                return false;
            else if (DataType == S7DataRowType.BYTE)
                return (byte)0;
            else if (DataType == S7DataRowType.DWORD)
                return (UInt32)0;
            else if (DataType == S7DataRowType.INT)
                return (Int16)0;
            else if (DataType == S7DataRowType.DINT)
                return (Int32)0;
            else if (DataType == S7DataRowType.REAL)
                return (float)0;
            else if (DataType == S7DataRowType.S5TIME)
                return new TimeSpan();
            else if (DataType == S7DataRowType.TIME)
                return new TimeSpan();
            else if (DataType == S7DataRowType.DATE) 
                return new DateTime(1990, 1, 1, 0, 0, 0, 0);
            else if (DataType == S7DataRowType.TIME_OF_DAY)
                return new DateTime(1990, 1, 1, 0, 0, 0, 0);
            else if (DataType == S7DataRowType.CHAR)
                return ' ';
            else if (DataType == S7DataRowType.DATE_AND_TIME)
                return new DateTime(1990, 1, 1, 0, 0, 0, 0);
            else if (DataType == S7DataRowType.STRING)
                return "";
            else if (DataType == S7DataRowType.STRUCT)
                return null;
            else if (DataType == S7DataRowType.UDT)
                return null;

            return null;
        }

        public static string ValueToString(object Value, S7DataRowType DataType)
        {
            if (DataType == S7DataRowType.S5_KY) return ((UInt16)Value / 256).ToString().PadLeft(3, '0') + "," + ((UInt16)Value % 256).ToString().PadLeft(3, '0');
            else if (DataType == S7DataRowType.S5_KF) return ((Int16)Value > 0 ? "+" : "") + Value.ToString();
            else if (DataType == S7DataRowType.S5_KH) return ((UInt16)Value).ToString("X", NumberFormatInfo.CurrentInfo).PadLeft(4, '0');
            else if (DataType == S7DataRowType.S5_KG) return SingleExtensions.ToS5(((float)Value));
            else if (DataType == S7DataRowType.S5_C || DataType == S7DataRowType.S5_KC) return "'" + ((string)Value) + "'"; //.PadLeft(4, ' ') 
            else if (DataType == S7DataRowType.S5_KC) return "'" + ((string)Value).PadLeft(2, ' ') + "'";
            else if (DataType == S7DataRowType.S5_KT) return Helper.GetS5TimeFromTimeSpan(((TimeSpan)Value));
            else if (DataType == S7DataRowType.S5_KM)
            {
                var bt = BitConverter.GetBytes((UInt16)Value);
                string ret = "";
                foreach (byte b in bt)
                {
                    if (ret != "") ret = " " + ret;
                    ret = libnodave.dec2bin(b) + ret;

                }
                return ret;
            }
            else if (DataType == S7DataRowType.WORD) return "W#16#" + ((UInt16)Value).ToString("X", NumberFormatInfo.CurrentInfo);
            else if (DataType == S7DataRowType.BYTE) return "B#16#" + ((byte)Value).ToString("X", NumberFormatInfo.CurrentInfo);
            else if (DataType == S7DataRowType.DWORD) return "DW#16#" + ((UInt32)Value).ToString("X", NumberFormatInfo.CurrentInfo);
            else if (DataType == S7DataRowType.INT) return ((Int16)Value).ToString();
            else if (DataType == S7DataRowType.DINT) return "L#" + ((Int32)Value).ToString();
            else if (DataType == S7DataRowType.REAL) return ((Single)Value).ToString("0.000000e+000", CultureInfo.InvariantCulture);
            else if (DataType == S7DataRowType.S5TIME)
            {
                var bt = new byte[2];
                libnodave.putS5Timeat(bt, 0, (TimeSpan)Value);
                return Helper.GetS5Time(bt[0], bt[1]);
            }
            else if (DataType == S7DataRowType.TIME)
            {
                var tm = (TimeSpan)Value;
                var ret = new StringBuilder("T#");
                if (tm.TotalMilliseconds < 0) ret.Append("-");
                if (tm.Days != 0) ret.Append(tm.Days + "D");
                if (tm.Hours != 0) ret.Append(tm.Hours + "H");
                if (tm.Minutes != 0) ret.Append(tm.Minutes + "M");
                if (tm.Seconds != 0) ret.Append(tm.Seconds + "S");
                if (tm.Milliseconds != 0 || tm.Ticks == 0) ret.Append(tm.Milliseconds + "MS");

                return ret.ToString();
            }
            else if (DataType == S7DataRowType.DATE)
            {
                DateTime ak = (DateTime)Value;
                StringBuilder sb = new StringBuilder();
                sb.Append("D#");
                sb.Append(ak.Year);
                sb.Append("-");
                sb.Append(ak.Month);
                sb.Append("-");
                sb.Append(ak.Day);
                return sb.ToString();
            }
            else if (DataType == S7DataRowType.TIME_OF_DAY)
            {
                DateTime ak = (DateTime)Value;
                StringBuilder sb = new StringBuilder();
                sb.Append("TOD#");
                sb.Append(ak.Hour);
                sb.Append(":");
                sb.Append(ak.Minute);
                sb.Append(":");
                sb.Append(ak.Second);
                sb.Append(".");
                sb.Append(ak.Millisecond.ToString().PadRight(3, '0'));
                return sb.ToString();
            }
            else if (DataType == S7DataRowType.CHAR) return ((char)Value).ToString();
            else if (DataType == S7DataRowType.DATE_AND_TIME)
            {
                DateTime ak = (DateTime)Value;
                StringBuilder sb = new StringBuilder();
                sb.Append("DT#");
                sb.Append(ak.Year.ToString().Substring(2));
                sb.Append("-");
                sb.Append(ak.Month);
                sb.Append("-");
                sb.Append(ak.Day);
                sb.Append("-");
                sb.Append(ak.Hour);
                sb.Append(":");
                sb.Append(ak.Minute);
                sb.Append(":");
                sb.Append(ak.Second);
                sb.Append(".");
                sb.Append(ak.Millisecond.ToString().PadRight(3, '0'));
                return sb.ToString();
            }
            else if (DataType == S7DataRowType.STRING) return Value.ToString();
            else if (DataType == S7DataRowType.STRUCT) return null;
            else if (DataType == S7DataRowType.UDT) return null;

            if (Value != null) return Value.ToString();

            return null;
        }

        public static bool IsNetwork(S7FunctionBlockRow row)
        {
            return row.Command == "NETWORK";
        }

        //public static bool IsWithStartVal(byte b)
        //{
        //    bool Result;
        //    Result = (b == 0x09) || (b == 0x0A) || (b == 0x0b) || (b == 0x0C);
        //    return Result;
        //}

        /// <summary>
        /// Returns the Parameter as an Int!
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        static public int GetParameterValueInt(string parameter)
        {
            return Convert.ToInt32(parameter);
        }

        /// <summary>
        /// This Function Returns a byte Array of a Value Parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        static public byte[] GetParameterValueBytes(string parameter)
        {
            int tmp = Convert.ToInt32(parameter);
            byte[] ret = BitConverter.GetBytes(tmp);
            return ret;
        }

        static public DateTime GetDateTimeFromDateString(string myString)
        {
            var tmp = myString.ToLower().Replace("d#", "").Replace("\t", "").Split('-');

            int y = 0, m = 0, d = 0;
            if (tmp.Length > 0)
                y = Convert.ToInt32(tmp[0]);
            if (tmp.Length > 1)
                m = Convert.ToInt32(tmp[1]);
            if (tmp.Length > 2)
                d = Convert.ToInt32(tmp[2]);

            if (y < 100)
                y += y >= 90 ? 1900 : 2000;

            return new DateTime(y, m, d);
        }

        static public DateTime GetDateTimeFromTimeOfDayString(string myString)
        {
            var tmp = myString.ToLower().Replace("tod#", "").Replace("\t", "").Split(':');

            int h = 0, m = 0, s = 0, ms = 0;
            if (tmp.Length > 0)
                h = Convert.ToInt32(tmp[0]);
            if (tmp.Length > 1)
                m = Convert.ToInt32(tmp[1]);
            if (tmp.Length > 2)
                if (tmp[2].Contains("."))
                {
                    s = Convert.ToInt32(tmp[2].Split('.')[0]);
                    ms = Convert.ToInt32(tmp[2].Split('.')[1]);
                }
                else
                    s = Convert.ToInt32(tmp[2]);

            return new DateTime(1990, 1, 1, h, m, s, ms);
        }

        static public DateTime GetDateTimeFromDateAndTimeString(string myString)
        {
            var tmp = myString.ToLower().Replace("dt#", "").Replace("\t", "");
            int pos = tmp.LastIndexOf('-');

            var tmp1 = tmp.Substring(0, pos);
            var tmp2 = tmp.Substring(pos + 1);

            var dtm1 = GetDateTimeFromDateString(tmp1);
            var dtm2 = GetDateTimeFromTimeOfDayString(tmp2);

            return new DateTime(dtm1.Year, dtm1.Month, dtm1.Day, dtm2.Hour, dtm2.Minute, dtm2.Second, dtm2.Millisecond);
        }
        static public int GetIntFromBinString(string myString)
        {
            int val = 0;
            foreach (char tmp in myString.ToLower().Replace("2#", ""))
            {
                if (tmp == '1')
                {
                    val *= 2;
                    val += 1;
                }
                else if (tmp == '0')
                    val *= 2;
            }
            return val;
        }

        static public int GetIntFromHexString(string myString)
        {
            return Convert.ToInt32(GetUIntFromHexString(myString));
            /*
            int val = 0;
            foreach (char tmp in myString.ToLower().Replace("dw#16#", "").Replace("w#16#", "").Replace("b#16#", "").Replace("\t",""))
            {
                val *= 16;
                switch (tmp)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        val += tmp - '0';
                        break;
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                        val += tmp - 'a' + 10;
                        break;
                }                
            }
            return val;*/
        }

        static public uint GetUIntFromHexString(string myString)
        {
            uint val = 0;
            foreach (char tmp in myString.ToLower().Replace("//", "").Replace("dw#16#", "").Replace("w#16#", "").Replace("b#16#", "").Replace("\t", "").Trim())
            {
                val *= 16;
                switch (tmp)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        val += (uint)(tmp - '0');
                        break;
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                        val += (uint)(tmp - 'a' + 10);
                        break;
                }
            }
            return val;
        }

        static public TimeSpan GetTimespanFromS5TimeorTime(string time)
        {
            time = time.ToLower().Replace("s5t#", "").Replace("t#", "").Replace("\t", "");
            //need another text for ms (because it could be minute)
            time = time.Replace("ms", "a").Trim();

            int d = 0, h = 0, m = 0, s = 0, ms = 0;
            int val = 0;
            bool negativ = false;
            foreach (char tmp in time)
            {
                if (tmp == 'd')
                {
                    d = val;
                    val = 0;
                }
                else if (tmp == 'h')
                {
                    h = val;
                    val = 0;
                }
                else if (tmp == 'm')
                {
                    m = val;
                    val = 0;
                }
                else if (tmp == 's')
                {
                    s = val;
                    val = 0;
                }
                else if (tmp == 'a') //ms are converted to a!
                {
                    ms = val;
                    val = 0;
                }
                else if (tmp == '-')
                {
                    negativ = true;
                }
                else if (tmp == '\t')
                {
                    
                }
                else
                {
                    val *= 10;
                    val += int.Parse(tmp.ToString());
                }
            }

            TimeSpan retVal = new TimeSpan(d, h, m, s, ms);
            /*TimeSpan max = new TimeSpan(0, 2, 46, 30);
            TimeSpan min = new TimeSpan(0, 0, 0, 0, 10);

            if (retVal > max)
                return max;
            if (retVal < min)
                return min;*/
            return negativ ? retVal.Negate() : retVal;

        }

        //Todo: This Function
        /// <summary>
        /// This Function Returns the Hex Value of a Parameter Pointer of a UC or CC.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        static public byte[] GetCallParameterByte(string parameter)
        {
            return null;
        }

        static public byte[] EncodePassword(string pwd)
        {
            byte[] ASCIIByteArray = System.Text.ASCIIEncoding.ASCII.GetBytes(pwd);

            byte[] retByteArray = new byte[] { 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75 };

            int i = 0;
            foreach (byte b in ASCIIByteArray)
            {
                retByteArray[i++] = (byte)((b ^ 0x55));
            }
            retByteArray[2] = (byte)(retByteArray[2] ^ retByteArray[0]);
            retByteArray[3] = (byte)(retByteArray[3] ^ retByteArray[1]);
            retByteArray[4] = (byte)(retByteArray[4] ^ retByteArray[2]);
            retByteArray[5] = (byte)(retByteArray[5] ^ retByteArray[3]);
            retByteArray[6] = (byte)(retByteArray[6] ^ retByteArray[4]);
            retByteArray[7] = (byte)(retByteArray[7] ^ retByteArray[5]);

            return retByteArray;
        }

        static public byte[] GetIndirectBytesWord(string parameter) //For this:  [DBW3] [LW4] and so on... must be 3 Bytes. 1 for type DBW, DIW, LW
        {
            //It has to return in the 3 Byte 0x30 for MW 0x40 for DBW 0x50 for DIW and 0x60 for LW!
            string tmp = parameter.Replace(" ", "").ToUpper().Substring(parameter.IndexOf('[')).Replace("]", "");
            int nr = 0;
            int nr2 = 0;
            switch (tmp.Substring(0,2))
            {
                case "PEW":
                case "PIW":
                case "PAW":
                case "POW":
                    nr = Convert.ToInt32(tmp.Replace("PEW", "").Replace("PAW", "").Replace("PIW", "").Replace("POW", ""));
                    nr2 = 0;
                    break;
                case "EW":
                case "IW":
                    nr = Convert.ToInt32(tmp.Replace("EW", "").Replace("IW", ""));
                    nr2 = 10;
                    break;
                case "AW":
                case "OW":
                    nr = Convert.ToInt32(tmp.Replace("AW", "").Replace("OW", ""));
                    nr2 = 20;
                    break;
                case "MW":
                    nr = Convert.ToInt32(tmp.Replace("MW", ""));
                    nr2 = 30;
                    break;
                case "DBW":
                    nr = Convert.ToInt32(tmp.Replace("DBW", ""));
                    nr2 = 40;
                    break;
                case "DIW":
                    nr = Convert.ToInt32(tmp.Replace("DIW", ""));
                    nr2 = 50;
                    break;
                case "LW":
                    nr = Convert.ToInt32(tmp.Replace("LW", ""));
                    nr2 = 60;
                    break;
            }
            return new byte[] {BitConverter.GetBytes(nr)[0], BitConverter.GetBytes(nr)[1], Convert.ToByte(nr2)};
        }

        static public List<S7FunctionBlockRow> GenerateFBParameterUsesFromAR2Calls(S7FunctionBlock myBlk, int Memnoic)
        {
            List<S7FunctionBlockRow> retVal = new List<S7FunctionBlockRow>();

            //In this list the Values are stored when we try to combine them,
            //when we are succesfull, this list is cleard, when not, it is added to the retVal
            List<S7FunctionBlockRow> tmpVal = new List<S7FunctionBlockRow>();

            //if the command is a TAR xx this will be set!
            int combineStep = 0;

            int byteaddr = 0;
            int bitaddr = 0;

            string TarLdTarget = "";

            /* Todo: Converting FB Commands to Access to Static Data
                 * 
                 * Like:
                 * 
                 TAR2  LD1           This is done, wehen the static address gets to high!
                 +AR2  P#4090.0      so the ar2 is saved, then increased and the it loads the data.
                 +AR2  P#4090.0      later ar2 is saved back!
                 L     DID[AR2,P#1840.0]
                 LAR2  LD1  
                 * 
                 * 
                 * or:
                 TAR2  LD1           
                 +AR2  P#4090.0      
                 +AR2  P#4090.0      
                 U     DIX[AR2,P#1838.0]
                 LAR2  LD1    
                 * or a simple one:
                 * L     DID[AR2,P#294.0]   should be:  L #blabla (wher blabla is static data at 294)
                 */

            //Check if Command is a TAR to LokalData, wich is not defined in the Interface
            //If next command is a + stor the address
            //if next command contains [ar2,xx) add the stored vakue to xx
            //and change the whole command to the parameter if there is a matching one
            //check if command is a lar2 from the lokaldata used before
            //if not role back!

            foreach (S7FunctionBlockRow plcFunctionBlockRow in myBlk.AWLCode)
            {
                if (plcFunctionBlockRow.Command == MC7.Mnemonic.opTAR2[Memnoic] && plcFunctionBlockRow.Parameter.Contains("LD"))
                {
                    retVal.AddRange(tmpVal);
                    tmpVal.Clear();
                    tmpVal.Add(plcFunctionBlockRow);
                    combineStep = 1;
                    byteaddr = 0;
                    bitaddr = 0;
                    TarLdTarget = plcFunctionBlockRow.Parameter;
                }
                else if (combineStep == 1 && plcFunctionBlockRow.Command == MC7.Mnemonic.opPAR2[Memnoic])
                {
                    tmpVal.Add(plcFunctionBlockRow);
                    byteaddr = Convert.ToInt32(plcFunctionBlockRow.Parameter.Split(new char[] {'#'})[1]);
                }
                else if (combineStep == 1 && plcFunctionBlockRow.Parameter.Contains("[AR2,P#"))
                {
                    tmpVal.Add(plcFunctionBlockRow);
                    combineStep = 2;
                }
                else if (
                    (combineStep == 2 && plcFunctionBlockRow.Command == MC7.Mnemonic.opLAR2[Memnoic] && plcFunctionBlockRow.Parameter == TarLdTarget) ||
                    (combineStep == 0 && plcFunctionBlockRow.Parameter.Contains("[AR2,P#"))
                    )
                {
                    //OK we commbined the command sucessfully, add the new command, delete the old ones...
                    //And add the Byte Sequence from the old commands to the new.
                    List<byte> bytes =new List<byte>();

                    foreach (S7FunctionBlockRow tmpFunctionBlockRow in tmpVal)                    
                        bytes.AddRange(tmpFunctionBlockRow.MC7);

                    retVal.Add(new S7FunctionBlockRow() {MC7 = bytes.ToArray()});

                    tmpVal.Clear();
                    combineStep = 0;
                }
                else
                {
                    retVal.AddRange(tmpVal);
                    tmpVal.Clear();
                    retVal.Add(plcFunctionBlockRow);
                    combineStep = 0;
                }
            }

            return retVal;
        }

        //Todo: Check if Jump label is used in the Block!
        static public bool IsJumpTarget(S7FunctionBlockRow myCmd, S7FunctionBlock myBlk)
        {
            if (!string.IsNullOrEmpty(myCmd.Label))
                return true;
            return false;
        }

        static public bool IsJump(S7FunctionBlockRow myCmd, int akMemnoic)
        {
            if (myCmd == null)
                return false;

            int MN = akMemnoic;

            if (myCmd.Command == Mnemonic.opSPA[MN]
               || myCmd.Command == Mnemonic.opSPB[MN]
               || myCmd.Command == Mnemonic.opSPBB[MN]
               || myCmd.Command == Mnemonic.opSPBI[MN]
               || myCmd.Command == Mnemonic.opSPBIN[MN]
               || myCmd.Command == Mnemonic.opSPBN[MN]
               || myCmd.Command == Mnemonic.opSPBNB[MN]
               || myCmd.Command == Mnemonic.opSPL[MN]
               || myCmd.Command == Mnemonic.opSPM[MN]
               || myCmd.Command == Mnemonic.opSPMZ[MN]
               || myCmd.Command == Mnemonic.opSPN[MN]
               || myCmd.Command == Mnemonic.opSPO[MN]
               || myCmd.Command == Mnemonic.opSPP[MN]
               || myCmd.Command == Mnemonic.opSPPZ[MN]
               || myCmd.Command == Mnemonic.opSPS[MN]
               || myCmd.Command == Mnemonic.opSPU[MN]
               || myCmd.Command == Mnemonic.opSPZ[MN]
               || myCmd.Command == Mnemonic.opLOOP[MN])
                return true;
            return false;
        }       

        public static string GetDaT(double LW1, double LW2)
        {
            string Result = "";

            int[] b1 = new int[4];
            int[] b2 = new int[4];
            int Year, MilliSecond;

            int[] tYear = new int[2];
            int[] tMonth = new int[2];
            int[] tDay = new int[2];
            int[] tHour = new int[2];
            int[] tMinute = new int[2];
            int[] tSecond = new int[2];
            int[] tMilliSecond = new int[2];


            int tMilliSec;
            int Month, Day, Hour, Minute, Second;
            bool Wrong;

            Wrong = false;

            tYear[0] = (b1[3]/16)*10; //  Jahr
            tYear[1] = (b1[3]%16);
            if ((tYear[0]/10 > 9) || (tYear[1] > 9))
                Wrong = true;
            Year = tYear[0] + tYear[1];

            if (Year < 90)
                Year = Year + 2000;
            else
                Year = Year + 1900;

            tMonth[0] = (b1[2]/16)*10; //  Monat
            tMonth[1] = (b1[2]%16);
            if ((tMonth[0]/10 > 9) || (tMonth[1] > 9))
                Wrong = true;
            Month = tMonth[0] + tMonth[1];

            tDay[0] = (b1[1]/16)*10; //  Tag
            tDay[1] = (b1[1]%16);
            if ((tDay[0]/10 > 9) || (tDay[1] > 9))
                Wrong = true;
            Day = tDay[0] + tDay[1];

            tHour[0] = (b1[0]/16)*10; //  Stunde
            tHour[1] = (b1[0]%16);
            if ((tHour[0]/10 > 9) || (tHour[1] > 9))
                Wrong = true;
            Hour = tHour[0] + tHour[1];

            tMinute[0] = (b2[3]/16)*10; //  Minute
            tMinute[1] = (b2[3]%16);
            if ((tMinute[0]/10 > 9) || (tMinute[1] > 9))
                Wrong = true;
            Minute = tMinute[0] + tMinute[1];

            tSecond[0] = (b2[2]/16)*10; //  Sekunde
            tSecond[1] = (b2[2]%16);
            if ((tSecond[0]/10 > 9) || (tSecond[1] > 9))
                Wrong = true;
            Second = tSecond[0] + tSecond[1];

            tMilliSec = (b2[1]/16)*100; //  Millisekunden
            tMilliSecond[0] = (b2[1]%16)*10;
            tMilliSecond[1] = (b2[0]/16);
            if ((tMilliSec/100 > 9) || (tMilliSecond[0]/10 > 9) || (tMilliSecond[1] > 9))
                Wrong = true;
            MilliSecond = tMilliSec + tMilliSecond[0] + tMilliSecond[1];

            if (Wrong)
                Result = "invalid value";
            else
                Result = "DT#" + Convert.ToString(Year) + "-" + Convert.ToString(Month) + "-" + Convert.ToString(Day) +
                         "-" + Convert.ToString(Hour) + ":" + Convert.ToString(Minute) + ":" + Convert.ToString(Second) +
                         "." + Convert.ToString(MilliSecond);

            return Result;
        }

        public static bool steuerZ(byte val)
        {
            if (val < 0x20 || val == 0x7F || val == 0x81 || val == 0x8D || val == 0x8F || val == 0x90 || val == 0x9D)
                return true;
            return false;
        }

        public static string GetStringChar(byte val)
        {
            if (steuerZ(val))
                return "0x" + val.ToString("X");
            else
            {
                return ((char)val).ToString();
            }
        }

        public static string GetChar(byte b)
        {

            string Result;

            if (steuerZ(b))
                Result = GetStringChar(b);
            else
                Result = "'" + GetStringChar(b) + "'";

            return Result;
        }

        public static string GetString(int Start, int Count, byte[] BD)
        {
            int i;
            string Result;

            Result = "";
            for (i = 0; i <= Count - 1; i++)
                Result = Result + (char)(BD[Start + i]);
            Result = Result.Trim().Replace("\0", string.Empty); //Remove Null chars

            return Result;
        }

        public static string GetVersion(byte b)
        {

            string Result;

            Result = Convert.ToString((b & 0xF0) >> 4) + "." + Convert.ToString(b & 0x0F);

            return Result;
        }


        public static DataTypes.PLCBlockType GetPLCBlockType(byte b)
        {
            switch (b)
            {
                case 0x08:
                    return DataTypes.PLCBlockType.OB;
                case 0x0A:
                    return DataTypes.PLCBlockType.DB;
                case 0x0b:
                    return DataTypes.PLCBlockType.SDB;
                case 0x0C:
                    return DataTypes.PLCBlockType.FC;
                case 0x0D:
                    return DataTypes.PLCBlockType.SFC;
                case 0x0E:
                    return DataTypes.PLCBlockType.FB;
                case 0x0F:
                    return DataTypes.PLCBlockType.SFB;
            }
            throw new Exception("Unkown PLCBlockType");
        }

        public static int GetPLCBlockTypeForBlockList(DataTypes.PLCBlockType myTP)
        {
            switch (myTP)
            {
                case DataTypes.PLCBlockType.OB:
                    return '8';
                case DataTypes.PLCBlockType.DB:
                    return 'A';
                case DataTypes.PLCBlockType.SDB:
                    return 'B';
                case DataTypes.PLCBlockType.FC:
                    return 'C';
                case DataTypes.PLCBlockType.SFC:
                    return 'D';
                case DataTypes.PLCBlockType.FB:
                    return 'E';
                case DataTypes.PLCBlockType.SFB:
                    return 'F';
            }
            throw new Exception("Unkown PLCBlockType");
        }


        public static string GetLang(byte b)
        {

            string Result;
            switch (b)
            {
                case 1: Result = "AWL"; break;
                case 2: Result = "KOP"; break;
                case 3: Result = "FUP"; break;
                case 4: Result = "SCL"; break;
                case 5: Result = "DB"; break;
                case 6: Result = "GRAPH"; break;               
                default: Result = "unbekannt(0x" + (b).ToString("X") + ")";
                    break;
            }
            return Result;
        }

        public static DateTime GetDT(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6)
        {
            System.DateTime DT;
            string Result;

            DT = new DateTime(1984, 1, 1, 0, 0, 0, 0);
            DT = DT.AddMilliseconds((b1 * 0x1000000) + (b2 * 0x10000) + (b3 * 0x100) + b4);
            DT = DT.AddDays((b5 * 0x100) + b6);
           
            return DT;
        }

        /*
        public static string JumpStr(int i)
        {

            string Result;

            if (i >= 0)
                Result = "+";
            else
                Result = "";
            Result = Result + Convert.ToString(i);

            return Result;
        }
         */

        public static string GetFCPointer(byte b1, byte b2, byte b3, byte b4)
        {
            string anf = "";
            int wrt = 0;
            switch (b1)
            {                
                case 0x80:
                    anf = "P#P ";
                    wrt = (b3 * 0x100 + b4) >> 3;
                    break;
                case 0x81:
                    anf = "P#E ";
                    wrt = (b3 * 0x100 + b4) >> 3;
                    break;
                case 0x82:
                    anf = "P#A ";
                    wrt = (b3 * 0x100 + b4) >> 3;
                    break;
                case 0x83:
                    anf = "P#M ";
                    wrt = (b3 * 0x100 + b4) >> 3;
                    break;
                case 0x84:
                    anf = "P#DBX ";
                    wrt = (b3 * 0x100 + b4) >> 3;
                    break;
                case 0x85:
                    anf = "P#DIX ";
                    wrt = (b3 * 0x100 + b4) >> 3;
                    break;
                case 0x86:
                    anf = "P#L ";
                    wrt = (b3 * 0x100 + b4) >> 3;
                    break;
                case 0x87:
                    anf = "P#V "; //Lokaldata of the Previous function
                    wrt = (b3 * 0x100 + b4) >> 3;
                    break;
                default:
                    anf = "P#";
                    wrt = (b1 * 0x100 + b2);
                    return anf + (wrt>>3).ToString() + "." + (wrt & 0x07).ToString();     
                    break;
            }


            return anf + wrt.ToString() + "." + (b4 & 0x07).ToString();     
                   
        }

        public static string GetS5TimeFromTimeSpan(TimeSpan ts)
        {
            if (ts.Ticks > ((long)10000000 * 999))
            {
                //basis 3
                var wr = ts.Ticks / 100000000;
                return wr + "." + "3";
            }
            else if (ts.Ticks > (1000000 * 999))
            {
                //basis 2
                var wr = ts.Ticks / 10000000;
                return wr + "." + "2";
            }
            else if (ts.Ticks > (100000 * 999))
            {
                //basis 1
                var wr = ts.Ticks / 1000000;
                return wr + "." + "1";
            }
            else
            {
                //basis 0
                var wr = ts.Ticks / 100000;
                return wr + "." + "0";
            }
        }


        public static string GetS5Time(byte b1, byte b2)
        {
            bool found;
            int ms;
            string Result;

            found = false;
            ms = ((b1 & 0x0F) * 100) + (((b2 & 0xF0) >> 4) * 10) + (b2 & 0x0F); switch ((b1 >> 4) & 0x03)
            {
                case 0x00: ms = ms * 10; break;
                case 0x01: ms = ms * 100; break;
                case 0x02: ms = ms * 1000; break;
                case 0x03: ms = ms * 10000; break;
            }
            Result = "S5T#";
            if (ms >= 3600000)
            {
                Result = Result + Convert.ToString(ms / 3600000) + "h";
                ms = ms - ((ms / 3600000) * 3600000);
                found = true;
            }
            if (ms >= 60000)
            {
                Result = Result + Convert.ToString(ms / 60000) + "m";
                ms = ms - ((ms / 60000) * 60000);
                found = true;
            }
            if (ms >= 1000)
            {
                Result = Result + Convert.ToString(ms / 1000) + "s";
                ms = ms - ((ms / 1000) * 1000);
                found = true;
            }
            if ((ms > 0) || (!found))
                Result = Result + Convert.ToString(ms) + "ms";

            return Result;
        }

        public static string GetDWordBool(byte b1, byte b2, byte b3, byte b4)
        {
            int i;
            string Result;

            Result = "";
            for (i = 7; i > 0; i--)
                Result = Result + Convert.ToString((b1 >> i) & 0x01);
            for (i = 7; i > 0; i--)
                Result = Result + Convert.ToString((b2 >> i) & 0x01);
            for (i = 7; i > 0; i--)
                Result = Result + Convert.ToString((b3 >> i) & 0x01);
            for (i = 7; i > 0; i--)
                Result = Result + Convert.ToString((b4 >> i) & 0x01);

            return Result;
        }

        public static string GetPointer(byte[] BD, int pos, int language)
        //public static string GetPointer(byte b1, byte b2, byte b3, byte b4)
        {
            double tmp;
            string Result;

            byte[] tmpb = new byte[] {0, BD[pos + 1], BD[pos + 2], BD[pos + 3]};

            tmp = libnodave.getU32from(tmpb, 0);
            //tmp = GetDWord(0, b2, b3, b4);
            Result = "P#";

            if (BD[pos] > 0x87 && BD[pos] != 0x8f)
                Result = Result + "? ";
            else
                switch (BD[pos])
                {
                    case 0x80:
                        Result = Result + "P ";
                        break;
                    case 0x81:
                        Result = Result + Mnemonic.adE[language] + " ";
                        break;
                    case 0x82:
                        Result = Result + Mnemonic.adA[language] + " ";
                        break;
                    case 0x83:
                        Result = Result + Mnemonic.adM[language] + " ";
                        break;
                    case 0x84:
                        Result = Result + Mnemonic.adDBX[language] + " ";
                        break;
                    case 0x85:
                        Result = Result + Mnemonic.adDIX[language] + " ";
                        break;
                    case 0x86:
                        Result = Result + Mnemonic.adL[language] + " ";
                        break;
                    case 0x87:
                        Result = Result + "V ";
                        break;
                    case 0x8f:
                        Result = Result + "L ";
                        Result = Result + Convert.ToString((BD[pos + 1] * 0x10000 + BD[pos + 2] * 0x100 + BD[pos + 3]) >> 3) + "." + Convert.ToString(BD[pos+3] & 0x07);
                        return Result;
                        break;
                }
            Result = Result + Convert.ToString(((int)tmp / 8)) + "." + Convert.ToString(tmp % 8);

            return Result;
        }

        public static string GetShortPointer(byte b1, byte b2)
        {
            double tmp;
            string Result;

            tmp = libnodave.getU16from(new byte[] { b1, b2}, 0);
            //tmp = GetWord(b1, b2);
            Result = "P#" + Convert.ToString(((int)tmp / 8)) + "." + Convert.ToString(tmp % 8);

            return Result;
        }

        public static string GetDTime(byte[] BD, int pos)
        //public static string GetDTime(byte b1, byte b2, byte b3, byte b4)
        {
            bool found;
            long ms;
            string Result;

            found = false;
            ms = libnodave.getS32from(BD, pos);
            //ms = GetDInt(b1, b2, b3, b4);
            Result = "T#";
            if (ms < 0)
            {
                Result = Result + "-";
                ms = ms * -1;
            }
            if (ms >= 86400000)
            {
                Result = Result + Convert.ToString(ms / 86400000) + "d";
                ms = ms - ((ms / 86400000) * 86400000);
                found = true;
            }
            if (ms >= 3600000)
            {
                Result = Result + Convert.ToString(ms / 3600000) + "h";
                ms = ms - ((ms / 3600000) * 3600000);
                found = true;
            }
            if (ms >= 60000)
            {
                Result = Result + Convert.ToString(ms / 60000) + "m";
                ms = ms - ((ms / 60000) * 60000);
                found = true;
            }
            if (ms >= 1000)
            {
                Result = Result + Convert.ToString(ms / 1000) + "s";
                ms = ms - ((ms / 1000) * 1000);
                found = true;
            }
            if ((ms > 0) || (!found))
                Result = Result + Convert.ToString(ms) + "ms";

            return Result;
        }

        public static string GetTOD(byte[] BD, int pos)
        //public static string GetTOD(byte b1, byte b2, byte b3, byte b4)
        {
            System.DateTime DT;
            string Result;

            DT = new DateTime(1990, 1, 1, 0, 0, 0);
            DT.AddMilliseconds(libnodave.getU32from(BD, pos));
            //DT.AddMilliseconds(GetDWord(b1, b2, b3, b4));
            Result = "TOD#" + DT.Hour.ToString() + ":" +
                      DT.Minute.ToString() + ":" +
                      DT.Second.ToString() + "." +
                      DT.Millisecond.ToString();

            return Result;            
        }

        /*
        public static string GetBOOL(byte b, byte bit)
        {
            string Result;

            if (((b >> bit) & 0x01) == 1)
                Result = "TRUE";
            else
                Result = "FALSE";

            return Result;
        }
         */

        public static string GetWordBool(byte b1, byte b2)
        {
            int i;
            System.Text.StringBuilder Result = new StringBuilder();

            for (i = 7; i > 0; i--)
                Result.Append(Convert.ToString((b1 >> i) & 0x01));
            for (i = 7; i > 0; i--)
                Result.Append(Convert.ToString((b2 >> i) & 0x01));

            return Result.ToString();
        }

        public static string GetDate(byte b1, byte b2)
        {
            System.DateTime DT;
            string Result;
            DT = new DateTime(1990, 1, 1, 0, 0, 0, 0);
            //DT.AddDays(GetWord(b1, b2));
            DT.AddDays(libnodave.getU16from(new byte[] { b1, b2 }, 0));
            Result = "D#" + Convert.ToString(DT.Year) + "-" + Convert.ToString(DT.Month) + "-" +
                     Convert.ToString(DT.Day);
            return Result;
        }

        /*
        public static ushort GetWord(byte b1, byte b2)
        {
            ushort Result;
            Result = Convert.ToUInt16((b1 * 0x100) + b2);
            return Result;
        }

        public static int GetInt(byte b1, byte b2)
        {
            byte[] tmp=new byte[]{b2,b1};
            
            return BitConverter.ToInt16(tmp,0);
        }
        */

        public static string GetS7String(int Start, int Count, byte[] BD)
        {
            int i;
            bool klammer;
            string Result;

            Result = "";
            klammer = false;
            if (Count == -1)
            {
                Count = BD[Start + 1];
                Start = Start + 2;
            }

            if (Count != 0)
            {
                if (true)

                    for (i = Start; i <= Start + Count - 1; i++)
                    {
                        if (klammer)
                        {
                            if (steuerZ(BD[i]))
                            {
                                Result = Result + "'" + GetStringChar(BD[i]);
                                klammer = false;
                            }
                            else
                                Result = Result + GetStringChar(BD[i]);
                        }
                        else
                        {
                            if (steuerZ(BD[i]))
                                Result = Result + GetStringChar(BD[i]);
                            else
                            {
                                Result = Result + "'" + GetStringChar(BD[i]);
                                klammer = true;
                            }
                        }
                    }
                if (klammer)
                    Result = Result + "'";
            }
            else
                Result = "''";

            return Result;
        }

        public static string TryGetSymbolFromOperand(IProjectFolder folder, string operand)
         {
             var programFolder = folder.Parent as S7ProgrammFolder;
             if(programFolder == null) return null;
             if (programFolder.SymbolTable == null) return null;
             var ste = programFolder.SymbolTable.GetEntryFromOperand(operand);
             return ste == null ? null : ste.Symbol;
         }
 
         public static string TryGetOperandFromSymbol(IProjectFolder folder, string symbol)
         {
             var programFolder = folder.Parent as S7ProgrammFolder;
             if (programFolder == null) return null;
             if (programFolder.SymbolTable == null) return null;
             var o = symbol.Replace("\"", "");
             var ste = programFolder.SymbolTable.GetEntryFromSymbol(o);
             return ste == null ? null : ste.Operand;
         }
 
         private static ByteBitAddress blockAddress = new ByteBitAddress(0, 0);
         public static string TryGetStructuredName(ProjectFolder folder, string operand, string dbAddress, bool isForAnyPointer = false)
         {
             if (!(folder is BlocksOfflineFolder)) return dbAddress;
             var f = folder as BlocksOfflineFolder;
             var o = operand.Replace(" ", "");
             var dbBlock = f.GetBlock(o, new S7ConvertingOptions { Mnemonic = folder.Project.ProjectLanguage }) as S7DataBlock;
             if (dbBlock == null) return dbAddress;
             var address = dbAddress.Replace(" ", "");
             var pointPosition = address.IndexOf('.');
             blockAddress.ByteAddress = int.Parse(pointPosition< 0 ? address.Substring(3) : address.Substring(3, pointPosition - 3));
             blockAddress.BitAddress = int.Parse(pointPosition< 0 ? "0" : address.Substring(address.Length - 1));
             var dbRow = dbBlock.GetDataRowWithAddress(blockAddress);
             if (dbRow == null) return dbAddress;
             if (dbRow.DataType != S7DataRowType.STRING)
                 return  dbRow.StructuredName;
             var byteOffset = blockAddress.ByteAddress - dbRow.BlockAddress.ByteAddress;
             if (pointPosition< 0 && (byteOffset)>1) //if its not using a bit access to access the string variable
                 return dbRow.StructuredName + "[" + (byteOffset - 2) + "]";
             if (isForAnyPointer && byteOffset == 0)
                 return dbRow.StructuredName;
             return dbAddress;
         }
    }
}
