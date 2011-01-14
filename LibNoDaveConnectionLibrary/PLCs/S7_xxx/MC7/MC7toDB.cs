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
namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    internal static class MC7toDB
    {        
        /*
        internal static PLCDataRow GetDBInterface( int Start,int Count,int AWLSt,int ValStart, byte[] BD)
        {
            PLCDataRow dbRoot = new PLCDataRow("ROOTNODE", PLCDataRowType.STRUCT);

            int pos, addr;
            string addrvalue;                        
            addr = 0;
            pos = Start + 4;            
            while (pos <= (Start + Count))
            {
                switch (BD[pos + 1])
                {
                    case 0x01:
                    case 0x09:
                        //GetDBVarType(BD[pos], false, false, Helper.IsWithStartVal(BD[pos + 1]), "", "IN", ref addr, AWLSt, BD);
                        break;
                    case 0x02:
                    case 0x9a:
                        //GetDBVarType(BD[pos], false, false, Helper.IsWithStartVal(BD[pos + 1]), "", "OUT", ref addr, AWLSt, BD);
                        break;
                    case 0x03:
                    case 0x9b:
                        //GetDBVarType(BD[pos], false, false, Helper.IsWithStartVal(BD[pos + 1]), "", "IN_OUT", ref addr, AWLSt, BD);
                        break;
                    case 0x04:
                    case 0x9C:
                        //GetDBVarType(BD[pos], false, false, Helper.IsWithStartVal(BD[pos + 1]), "", "STAT", ref addr, AWLSt, BD, ref dbRoot);
                        break;
                    case 0x05:
                        //GetDBVarType(BD[pos], false, false, Helper.IsWithStartVal(BD[pos + 1]), "", "TEMP", ref addr, AWLSt, BD);
                        break;
                    case 0x06:
                        //GetDBVarType(BD[pos], false, false, Helper.IsWithStartVal(BD[pos + 1]), "", "RET_VAL", ref addr, AWLSt, BD);
                        break;
                }
                pos += 2;
            }

            return dbRoot;
        }
        */

        /*
        public static string GetAdress( byte b, byte[] BD, ref int addr, int AWLSt)
        {

            string Result;

            string addrvalue = "";
            Result = "";
            switch (b)
            {
                case 0x01:
                    {
                        addrvalue = Helper.GetBOOL(BD[(addr/8) + AWLSt], Convert.ToByte(addr%8));
                        Result = Convert.ToString(addr/8) + "." + Convert.ToString(addr%8);
                        addr = addr + 1;
                    }
                    break;
                case 0x02:
                case 0x03:
                    {
                        if ((addr%8) != 0)
                            addr = addr + (8 - (addr%8));
                        switch (b)
                        {
                            case 0x02:
                                addrvalue = "B#16#" + (BD[(addr/8) + AWLSt]).ToString("X");
                                break;
                            case 0x03:
                                addrvalue = Helper.GetChar(BD[(addr/8) + AWLSt]);
                                break;
                        }
                        Result = Convert.ToString(addr/8) + "." + Convert.ToString(addr%8);
                        addr = addr + 8;
                    }
                    break;
                case 0x04:
                case 0x05:
                case 0x09:
                case 0x0C:
                    {
                        if ((addr%16) != 0)
                            addr = addr + (16 - (addr%16));
                        switch (b)
                        {
                            case 0x04:
                                addrvalue = "W#16#" +
                                            (Helper.GetWord(BD[(addr/8) + AWLSt], BD[(addr/8) + AWLSt + 1])).ToString(
                                                "X");
                                break;
                            case 0x05:
                                addrvalue =
                                    Convert.ToString(Helper.GetInt(BD[(addr/8) + AWLSt], BD[(addr/8) + AWLSt + 1]));
                                break;
                            case 0x09:
                                addrvalue = Helper.GetDate(BD[(addr/8) + AWLSt], BD[(addr/8) + AWLSt + 1]);
                                break;
                            case 0x0C:
                                addrvalue = Helper.GetS5Time(BD[(addr/8) + AWLSt], BD[(addr/8) + AWLSt + 1]);
                                break;
                        }
                        Result = Convert.ToString(addr/8) + "." + Convert.ToString(addr%8);
                        addr = addr + 16;
                    }
                    break;
                case 0x06:
                case 0x07:
                case 0x08:
                case 0x0A:
                case 0x0B:
                    {
                        if ((addr%16) != 0)
                            addr = addr + (16 - (addr%16));
                        switch (b)
                        {
                            case 0x06:
                                addrvalue = "DW#16#" +
                                            (Helper.GetDWord(BD[(addr/8) + AWLSt], BD[(addr/8) + AWLSt + 1],
                                                             BD[(addr/8) + AWLSt + 2], BD[(addr/8) + AWLSt + 3])).
                                                ToString("X");
                                break;
                            case 0x07:
                                addrvalue = "L#" +
                                            Convert.ToString(Helper.GetDInt(BD[(addr/8) + AWLSt],
                                                                            BD[(addr/8) + AWLSt + 1],
                                                                            BD[(addr/8) + AWLSt + 2],
                                                                            BD[(addr/8) + AWLSt + 3]));
                                break;
                            case 0x08:
                                addrvalue = Helper.GetReal(BD[(addr/8) + AWLSt], BD[(addr/8) + AWLSt + 1],
                                                           BD[(addr/8) + AWLSt + 2], BD[(addr/8) + AWLSt + 3]);
                                break;
                            case 0x0a:
                                addrvalue = Helper.GetTOD(BD[(addr/8) + AWLSt], BD[(addr/8) + AWLSt + 1],
                                                          BD[(addr/8) + AWLSt + 2], BD[(addr/8) + AWLSt + 3]);
                                break;
                            case 0x0b:
                                addrvalue = Helper.GetDTime(BD[(addr/8) + AWLSt], BD[(addr/8) + AWLSt + 1],
                                                            BD[(addr/8) + AWLSt + 2], BD[(addr/8) + AWLSt + 3]);
                                break;
                        }
                        Result = Convert.ToString(addr/8) + "." + Convert.ToString(addr%8);
                        addr = addr + 32;
                    }
                    break;
                case 0x0E:
                    {
                        if ((addr%16) != 0)
                            addr = addr + (16 - (addr%16));
                        addrvalue =
                            Helper.GetDaT(
                                Helper.GetDWord(BD[(addr/8) + AWLSt], BD[(addr/8) + AWLSt + 1], BD[(addr/8) + AWLSt + 2],
                                                BD[(addr/8) + AWLSt + 3]),
                                Helper.GetDWord(BD[(addr/8) + AWLSt + 4], BD[(addr/8) + AWLSt + 5],
                                                BD[(addr/8) + AWLSt + 6], BD[(addr/8) + AWLSt + 7]));
                        Result = Convert.ToString(addr/8) + "." + Convert.ToString(addr%8);
                        addr = addr + 64;
                    }
                    break;
                case 0x10:
                    {
                        if ((addr%16) != 0)
                            addr = addr + (16 - (addr%16));
                        Result = "";
                    }
                    break;
                case 0x11:
                    {
                        if ((addr%16) != 0)
                            addr = addr + (16 - (addr%16));
                        Result = Convert.ToString(addr/8) + "." + Convert.ToString(addr%8);
                    }
                    break;
                case 0x13:
                    {
                        if ((addr%16) != 0)
                            addr = addr + (16 - (addr%16));
                        Result = Convert.ToString(addr/8) + "." + Convert.ToString(addr%8);
                        addrvalue = Helper.GetS7String((addr/8) + AWLSt, -1, BD);
                        addr = addr + ((BD[(addr/8) + AWLSt] + 2)*8);
                    }
                    break;
            }
            return Result;
        }

        public static string GetVarTypeVal(byte b, bool startVal, byte[] BD, ref int Valpos)
        {
            byte b1, b2, b3, b4, b5, b6, b7, b8;
            string Result;

            if (startVal)
            {
                b1 = BD[Valpos];
                b2 = BD[Valpos + 1];
                b3 = BD[Valpos + 2];
                b4 = BD[Valpos + 3];
                b5 = BD[Valpos + 4];
                b6 = BD[Valpos + 5];
                b7 = BD[Valpos + 6];
                b8 = BD[Valpos + 7];
            }
            else
            {
                b1 = 0;
                b2 = 0;
                b3 = 0;
                b4 = 0;
                b5 = 0;
                b6 = 0;
                b7 = 0;
                b8 = 0;
            }
            switch (b)
            {
                case 0x01:
                    {
                        // 'BOOL';
                        if (b1 == 0)
                            Result = "FALSE";
                        else
                            Result = "TRUE";
                        if (startVal)
                            Valpos = Valpos + 1;
                    }
                    break;
                case 0x02:
                    {
                        // 'BYTE';
                        Result = "B#16#" + (b1).ToString("X");
                        if (startVal)
                            Valpos = Valpos + 1;
                    }
                    break;
                case 0x03:
                    {
                        // 'CHAR';
                        Result = ((char) b1).ToString();
                        if (startVal)
                            Valpos = Valpos + 1;
                    }
                    break;
                case 0x04:
                    {
                        // 'WORD';
                        Result = "W#16#" + (b2).ToString("X") + (b1).ToString("X");
                        if (startVal)
                            Valpos = Valpos + 2;
                    }
                    break;
                case 0x05:
                    {
                        // 'INT';
                        Result = Convert.ToString(Helper.GetInt(b2, b1));
                        if (startVal)
                            Valpos = Valpos + 2;
                    }
                    break;
                case 0x06:
                    {
                        // 'DWORD';
                        Result = "DW#16#" + (b4).ToString("X") + (b3).ToString("X") + (b2).ToString("X") +
                                 (b1).ToString("X");
                        if (startVal)
                            Valpos = Valpos + 4;
                    }
                    break;
                case 0x07:
                    {
                        // 'DINT';
                        Result = "L#" + Convert.ToString(Helper.GetDInt(b4, b3, b2, b1));
                        if (startVal)
                            Valpos = Valpos + 4;
                    }
                    break;
                case 0x08:
                    {
                        // 'REAL';
                        Result = Helper.GetReal(b4, b3, b2, b1);
                        if (startVal)
                            Valpos = Valpos + 4;
                    }
                    break;
                case 0x09:
                    {
                        // 'DATE';
                        Result = Helper.GetDate(b2, b1);
                        if (startVal)
                            Valpos = Valpos + 2;
                    }
                    break;
                case 0x0a:
                    {
                        // 'TIME_OF_DAY';
                        Result = Helper.GetTOD(b4, b3, b2, b1);
                        if (startVal)
                            Valpos = Valpos + 4;
                    }
                    break;
                case 0x0b:
                    {
                        // 'TIME';
                        Result = Helper.GetDTime(b4, b3, b2, b1);
                        if (startVal)
                            Valpos = Valpos + 4;
                    }
                    break;
                case 0x0C:
                    {
                        // 'S5TIME';
                        Result = Helper.GetS5Time(b2, b1);
                        if (startVal)
                            Valpos = Valpos + 2;
                    }
                    break;
                case 0x0E:
                    {
                        // 'DATE_AND_TIME';
                        Result = Helper.GetDaT(Helper.GetDWord(b1, b2, b3, b4), Helper.GetDWord(b5, b6, b7, b8));
                        if (startVal)
                            Valpos = Valpos + 8;
                    }
                    break;
                case 0x13:
                    {
                        // 'STRING';
                        if (startVal)
                        {
                            Result = Helper.GetS7String(Valpos, -1,BD);
                            Valpos = Valpos + BD[Valpos + 1] + 2;
                        }
                        else
                        {
                            Result = "''";
                        }
                    }
                    break;
                default:
                    Result = "UNKNOWN (" + Convert.ToString(b) + ")";
                    break;
            }
            return Result;
        }
         * 

        public static string GetDBVarType( byte b, bool Struct,bool Arry,bool GetVal, string structn,string typname, ref int addr, int AWLSt, byte[] BD, ref int STATp, ref string addrvalue, ref int Valpos, ref int pos, ref PLCDataRow myRow)
		{
		 int i, max;
		       string temp;
		string Result;
		  switch(b) {
		        case 0x01:{
		            temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"BOOL", GetVarTypeVal(b,GetVal,BD,ref Valpos),addrvalue);
		             } break;
		        case 0x02:{
		            temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"BYTE",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x03:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"CHAR",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x04:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"WORD",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x05:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"INT",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x06:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"DWORD",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x07:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"DINT",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x08:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"REAL",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x09:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"DATE",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x0a:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"TIME_OF_DAY",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x0b:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"TIME",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x0C:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"S5TIME",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x0E:{
		               temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"DATE_AND_TIME",GetVarTypeVal(b,GetVal),addrvalue);
		             } break;
		        case 0x10:{
		               if( BD[pos+7] != 0x11 )
		               {
		                 if( GetVal )
		                   Valpos = Valpos + 6;
		                temp = GetAdress(b, BD, ref addr, AWLSt);
		                 AddRow("",structn + typname + Convert.ToString( STATp ),"ARRAY [" + Convert.ToString( Helper.GetWord(BD[pos+4],BD[pos+3]) ) + ".." + Convert.ToString( GetWord(BD[pos+6],BD[pos+5]) ) + "] OF " + GetVarType(BD[pos+7]),GetVarTypeVal(BD[pos+7],GetVal),"");
		                 for( i = Helper.GetWord(BD[pos+4],BD[pos+3]);i <= Helper.GetWord(BD[pos+6],BD[pos+5]);i++)
		                 {
                             temp = GetAdress(BD[pos + 7], BD, ref addr, AWLSt);
		                   AddRow(temp,structn + typname + Convert.ToString( STATp ) + "[" + Convert.ToString( i ) + "]",GetVarType(BD[pos+7]),"",addrvalue);
		                 }
		                 if( BD[pos+7] == 0x13 ) pos += 1; pos +=  7;
		               }
		             } break;
		        case 0x11:{
		               AddRow(GetAdress(b, BD, ref addr, AWLSt),structn + typname + Convert.ToString( STATp ),"STRUCT","","");
		               STATp = STATp + 1;
		               if( Arry ) pos += 7;
		               max = BD[pos+2] - 1;
		               for( i = 0;i <= max;i++)
		               {
		                 if( (BD[pos+3] == 0x11) || (BD[pos+3] == 0x10) )
		                 { pos += 3;
		                   GetDBVartype(BD[pos], true, false, IsWithStartVal(BD[pos+1]), structn + ".", typname); pos -= 3;
		                 }
		                 else
		                   GetDBVartype(BD[pos+3], true, false, IsWithStartVal(BD[pos+4]), structn + ".", typname);
		                 if( i != max )
		                   Result = Result + ","; pos +=  2;
		               }
		               if( Arry ) pos -= 7;
		               AddRow(GetAdress(b, BD, ref addr, AWLSt),"","END_STRUCT","",""); pos +=  1;
		             } break;
		        case 0x13:{
                    temp = GetAdress(b, BD, ref addr, AWLSt);
		               AddRow(temp,structn + typname + Convert.ToString( STATp ),"STRING[" + Convert.ToString( BD[pos+5] ) + "]",GetVarTypeVal(b,GetVal),addrvalue); pos +=  1;
		             } break;
		        default: AddRow(GetAdress(b, BD, ref addr, AWLSt),structn + typname + Convert.ToString( STATp ),"UNKNOWN (0x" + IntToHex(b,2) + ")","",""); break;}
		      STATp = STATp + 1;
		   
		return Result; }		
         */
    }
}
