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
    public static class Mnemonic
    {
        public static string[] adB = new string[] { "B", "B" };
        public static string[] adW = new string[] { "W", "W" };
        public static string[] adD = new string[] { "D", "D" };
        public static string[] adPEB = new string[] { "PEB", "PIB" };
        public static string[] adPEW = new string[] { "PEW", "PIW" };
        public static string[] adPED = new string[] { "PED", "PID" };
        public static string[] adPAB = new string[] { "PAB", "PQB" };
        public static string[] adPAW = new string[] { "PAW", "PQW" };
        public static string[] adPAD = new string[] { "PAD", "PQD" };
        public static string[] adM = new string[] { "M", "M" };
        public static string[] adMB = new string[] { "MB", "MB" };
        public static string[] adMW = new string[] { "MW", "MW" };
        public static string[] adMD = new string[] { "MD", "MD" };
        public static string[] adE = new string[] { "E", "I" };
        public static string[] adEB = new string[] { "EB", "IB" };
        public static string[] adEW = new string[] { "EW", "IW" };
        public static string[] adED = new string[] { "ED", "ID" };
        public static string[] adA = new string[] { "A", "Q" };
        public static string[] adAB = new string[] { "AB", "QB" };
        public static string[] adAW = new string[] { "AW", "QW" };
        public static string[] adAD = new string[] { "AD", "QD" };
        public static string[] adL = new string[] { "L", "L" };
        public static string[] adLB = new string[] { "LB", "LB" };
        public static string[] adLW = new string[] { "LW", "LW" };
        public static string[] adLD = new string[] { "LD", "LD" };
        public static string[] adDB = new string[] { "DB", "DB" };
        public static string[] adDBX = new string[] { "DBX", "DBX" };
        public static string[] adDBB = new string[] { "DBB", "DBB" };
        public static string[] adDBW = new string[] { "DBW", "DBW" };
        public static string[] adDBD = new string[] { "DBD", "DBD" };
        public static string[] adDI = new string[] { "DI", "DI" };
        public static string[] adDIX = new string[] { "DIX", "DIX" };
        public static string[] adDIB = new string[] { "DIB", "DIB" };
        public static string[] adDIW = new string[] { "DIW", "DIW" };
        public static string[] adDID = new string[] { "DID", "DID" };
        public static string[] adT = new string[] { "T", "T" };
        public static string[] adZ = new string[] { "Z", "C" };

        public static string[] adAR1 = new string[] { "AR1", "AR1" };
        public static string[] adAR2 = new string[] { "AR2", "AR2" };
        public static string[] adSTW = new string[] { "STW", "STW" };
        public static string[] adOS = new string[] { "OS", "OS" };
        public static string[] adOV = new string[] { "OV", "OV" };
        public static string[] adUO = new string[] { "UO", "UO" };
        public static string[] adBIE = new string[] { "BIE", "BR" };

        public static string[] adDBLG = new string[] { "DBLG", "DBLG" };
        public static string[] adDILG = new string[] { "DILG", "DILG" };
        public static string[] adDBNO = new string[] { "DBNO", "DBNO" };
        public static string[] adDINO = new string[] { "DINO", "DINO" };

        public static string[] adSFC = new string[] { "SFC", "SFC" };
        public static string[] adSFB = new string[] { "SFB", "SFB" };
        public static string[] adFC = new string[] { "FC", "FC" };
        public static string[] adFB = new string[] { "FB", "FB" };

        public static string[] opCALL = new string[] { "CALL", "CALL" };

        public static string[] opABS = new string[] { "ABS", "ABS" };
        public static string[] opACOS = new string[] { "ACOS", "ACOS" };
        public static string[] opASIN = new string[] { "ASIN", "ASIN" };
        public static string[] opATAN = new string[] { "ATAN", "ATAN" };
        public static string[] opAUF = new string[] { "AUF", "OPN" };
        public static string[] opBE = new string[] { "BE", "BE" };
        public static string[] opBEA = new string[] { "BEA", "BEU" };
        public static string[] opBEB = new string[] { "BEB", "BEC" };
        public static string[] opBLD = new string[] { "BLD", "BLD" };
        public static string[] opBTD = new string[] { "BTD", "BTD" };
        public static string[] opBTI = new string[] { "BTI", "BTI" };
        public static string[] opCC = new string[] { "CC", "CC" };
        public static string[] opCLR = new string[] { "CLR", "CLR" };
        public static string[] opCOS = new string[] { "COS", "COS" };
        public static string[] opDEC = new string[] { "DEC", "DEC" };
        public static string[] opDTB = new string[] { "DTB", "DTB" };
        public static string[] opDTR = new string[] { "DTR", "DTR" };
        public static string[] opENT = new string[] { "ENT", "ENT" };
        public static string[] opEXP = new string[] { "EXP", "EXP" };
        public static string[] opFN = new string[] { "FN", "FN" };
        public static string[] opFP = new string[] { "FP", "FP" };
        public static string[] opFR = new string[] { "FR", "FR" };
        public static string[] opINC = new string[] { "INC", "INC" };
        public static string[] opINVD = new string[] { "INVD", "INVD" };
        public static string[] opINVI = new string[] { "INVI", "INVI" };
        public static string[] opITB = new string[] { "ITB", "ITB" };
        public static string[] opITD = new string[] { "ITD", "QD" };
        public static string[] opL = new string[] { "L", "L" };
        public static string[] opLAR1 = new string[] { "LAR1", "LAR1" };
        public static string[] opLAR2 = new string[] { "LAR2", "LAR2" };
        public static string[] opLC = new string[] { "LC", "LC" };
        public static string[] opLEAVE = new string[] { "LEAVE", "LEAVE" };
        public static string[] opLN = new string[] { "LN", "LN" };
        public static string[] opLOOP = new string[] { "LOOP", "LOOP" };
        public static string[] opMCRA = new string[] { "MCRA", "MCRA" };
        public static string[] opMCRC = new string[] { ")MCR", ")MCR" };
        public static string[] opMCRD = new string[] { "MCRD", "MCRD" };
        public static string[] opMCRO = new string[] { "MCR(", "MCR(" };
        public static string[] opMOD = new string[] { "MOD", "MOD" };
        public static string[] opNEGD = new string[] { "NEGD", "NEGD" };
        public static string[] opNEGI = new string[] { "NEGI", "NEGI" };
        public static string[] opNEGR = new string[] { "NEGR", "NEGR" };
        public static string[] opNOP = new string[] { "NOP", "NOP" };
        public static string[] opNOT = new string[] { "NOT", "NOT" };
        public static string[] opO = new string[] { "O", "O" };
        public static string[] opOD = new string[] { "OD", "OD" };
        public static string[] opON = new string[] { "ON", "ON" };
        public static string[] opONO = new string[] { "ON(", "ON(" };
        public static string[] opOO = new string[] { "O(", "O(" };
        public static string[] opOW = new string[] { "OW", "OW" };
        public static string[] opPAR1 = new string[] { "+AR1", "+AR1" };
        public static string[] opPAR2 = new string[] { "+AR2", "+AR2" };
        public static string[] opPLU = new string[] { "+", "+" };
        public static string[] opPOP = new string[] { "POP", "POP" };
        public static string[] opPUSH = new string[] { "PUSH", "PUSH" };
        public static string[] opR = new string[] { "R", "R" };
        public static string[] opRLD = new string[] { "RLD", "RLD" };
        public static string[] opRLDA = new string[] { "RLDA", "RLDA" };
        public static string[] opRND = new string[] { "RND", "RND" };
        public static string[] opRNDM = new string[] { "RRD-", "RRD-" };
        public static string[] opRNDP = new string[] { "RRD+", "RRD+" };
        public static string[] opRRD = new string[] { "RRD", "RRD" };
        public static string[] opRRDA = new string[] { "RRDA", "RRDA" };
        public static string[] opS = new string[] { "S", "S" };
        public static string[] opSA = new string[] { "SA", "SF" };
        public static string[] opSAVE = new string[] { "SAVE", "SAVE" };
        public static string[] opSE = new string[] { "SE", "SD" };
        public static string[] opSET = new string[] { "SET", "SET" };
        public static string[] opSI = new string[] { "SI", "SP" };
        public static string[] opSIN = new string[] { "SIN", "SIN" };
        public static string[] opSLD = new string[] { "SLD", "SLD" };
        public static string[] opSLW = new string[] { "SLW", "SLW" };
        public static string[] opSPA = new string[] { "SPA", "JU" };
        public static string[] opSPB = new string[] { "SPB", "JC" };
        public static string[] opSPBB = new string[] { "SPBB", "JCB" };
        public static string[] opSPBI = new string[] { "SPBI", "JBI" };
        public static string[] opSPBIN = new string[] { "SPBIN", "JNBI" };
        public static string[] opSPBN = new string[] { "SPBN", "JCN" };
        public static string[] opSPBNB = new string[] { "SPBNB", "JNB" };
        public static string[] opSPL = new string[] { "SPL", "JL" };
        public static string[] opSPM = new string[] { "SPM", "JM" };
        public static string[] opSPMZ = new string[] { "SPMZ", "JMZ" };
        public static string[] opSPN = new string[] { "SPN", "JN" };
        public static string[] opSPO = new string[] { "SPO", "JO" };
        public static string[] opSPP = new string[] { "SPP", "JP" };
        public static string[] opSPPZ = new string[] { "SPPZ", "JPZ" };
        public static string[] opSPS = new string[] { "SPS", "JOS" };
        public static string[] opSPU = new string[] { "SPU", "JUO" };
        public static string[] opSPZ = new string[] { "SPZ", "JZ" };
        public static string[] opSQR = new string[] { "SQR", "SQR" };
        public static string[] opSQRT = new string[] { "SQRT", "SQRT" };
        public static string[] opSRD = new string[] { "SRD", "SRD" };
        public static string[] opSRW = new string[] { "SRW", "SRW" };
        public static string[] opSS = new string[] { "SS", "SS" };
        public static string[] opSSD = new string[] { "SSD", "SSD" };
        public static string[] opSSI = new string[] { "SSI", "SSI" };
        public static string[] opSV = new string[] { "SV", "SE" };
        public static string[] opT = new string[] { "T", "T" };
        public static string[] opTAD = new string[] { "TAD", "CAD" };
        public static string[] opTAK = new string[] { "TAK", "TAK" };
        public static string[] opTAN = new string[] { "TAN", "TAN" };
        public static string[] opTAR = new string[] { "TAR", "CAR" };
        public static string[] opTAR1 = new string[] { "TAR1", "TAR1" };
        public static string[] opTAR2 = new string[] { "TAR2", "TAR2" };
        public static string[] opTAW = new string[] { "TAW", "CAW" };
        public static string[] opTDB = new string[] { "TDB", "CDB" };
        public static string[] opTRUNC = new string[] { "TRUNC", "TRUNC" };
        public static string[] opU = new string[] { "U", "A" };
        public static string[] opUO = new string[] { "U(", "A(" };
        public static string[] opUC = new string[] { "UC", "UC" };
        public static string[] opUN = new string[] { "UN", "AN" };
        public static string[] opUNO = new string[] { "UN(", "AN(" };
        public static string[] opUD = new string[] { "UD", "AD" };
        public static string[] opUW = new string[] { "UW", "AW" };
        public static string[] opX = new string[] { "X", "X" };
        public static string[] opXN = new string[] { "XN", "XN" };
        public static string[] opXNO = new string[] { "XN(", "XN(" };
        public static string[] opXO = new string[] { "X(", "X(" };
        public static string[] opXOD = new string[] { "XOD", "XOD" };
        public static string[] opXOW = new string[] { "XOW", "XOW" };
        public static string[] opZR = new string[] { "ZR", "CD" };
        public static string[] opZV = new string[] { "ZV", "CU" };
        public static string[] opZUW = new string[] { "=", "=" };        
    }
}
