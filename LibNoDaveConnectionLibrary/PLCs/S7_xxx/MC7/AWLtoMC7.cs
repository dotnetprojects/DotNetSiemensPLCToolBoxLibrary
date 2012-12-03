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
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

//Todo: Finish AWL to MC7
namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    internal static class AWLtoMC7
    {
        static public byte[] GetMC7(S7Block myCmd)
        {
            //if (myCmd.GetType() == typeof(PLCFunctionBlock))
            //    return GetMC7((PLCFunctionBlock) myCmd);
            List<byte> retVal = new List<byte>();

            foreach (var myLine in ((S7FunctionBlock)myCmd).AWLCode)
            {
                if (((S7FunctionBlockRow)myLine).MC7 == null) 
                    ((S7FunctionBlockRow)myLine).MC7 = GetMC7(((S7FunctionBlockRow)myLine));
                retVal.AddRange(((S7FunctionBlockRow)myLine).MC7);
            }
            return retVal.ToArray();
        }



        public static int GetSize(S7FunctionBlockRow myCmd)
        {

            int MN = 0; //Memnoic still needs to be implemented!

            //This function is there, because for a Jump it sould not call GetMC7, because then we can get circular calls!
            //A Jump in GetMC7 will also call this, to callculate the distance to the jump mark!            
            if (Helper.IsJump(myCmd, MN)) return 4;
            else
                try
                {
                    if (myCmd.MC7 == null) myCmd.MC7 = GetMC7(myCmd);
                    return myCmd.MC7.Length;
                }
                catch (Exception)
                {
                    return 0;
                }
        }

        static public byte[] GetMC7(S7FunctionBlockRow myCmd)
        {
            int MN=0; //Memnoic still needs to be implemented!


            if (myCmd.Command == "NETWORK")
                return null;
            else if (myCmd.Command == Mnemonic.opTAK[MN])
                return new byte[] { 0x70, 0x02 };
            else if (myCmd.Command == Mnemonic.opPUSH[MN])
                return new byte[] { 0x68, 0x2E };
            else if (myCmd.Command == Mnemonic.opPOP[MN])
                return new byte[] { 0x68, 0x3E };
            else if (myCmd.Command == Mnemonic.opENT[MN])
                return new byte[] { 0x60, 0x08 };
            else if (myCmd.Command == Mnemonic.opLEAVE[MN])
                return new byte[] { 0x68, 0x4E };
            else if (myCmd.Command == Mnemonic.opPAR1[MN])
                return new byte[] { 0xFE, 0x06 };
            else if (myCmd.Command == Mnemonic.opPAR2[MN])
                return new byte[] { 0xFE, 0x0E };
            else if (myCmd.Command == Mnemonic.opNOP[MN])
                if (myCmd.Parameter == "0")
                    return new byte[] { 0x00, 0x00 };
                else
                    return new byte[] { 0xFF, 0xFF };
            else if (myCmd.Command == Mnemonic.opINC[MN])
                return new byte[] { 0x11, Convert.ToByte(myCmd.Parameter) };
            else if (myCmd.Command == Mnemonic.opBLD[MN])
                return new byte[] { 0x10, Convert.ToByte(myCmd.Parameter) };
            else if (myCmd.Command == Mnemonic.opDEC[MN])
                return new byte[] { 0x19, Convert.ToByte(myCmd.Parameter) };
            else if (myCmd.Command == "+D")
                return new byte[] { 0x60, 0x0D };
            else if (myCmd.Command == "-D")
                return new byte[] { 0x60, 0x09 };
            else if (myCmd.Command == "*D")
                return new byte[] { 0x60, 0x0A };
            else if (myCmd.Command == "/D")
                return new byte[] { 0x60, 0x0E };
            else if (myCmd.Command == Mnemonic.opMOD[MN])
                return new byte[] { 0x60, 0x01 };
            else if (myCmd.Command == "+I")
                return new byte[] { 0x79, 0x05 };
            else if (myCmd.Command == "-I")
                return new byte[] { 0x59, 0x05 };
            else if (myCmd.Command == "*I")
                return new byte[] { 0x60, 0x04 };
            else if (myCmd.Command == "/I")
                return new byte[] { 0x60, 0x00 };
            else if (myCmd.Command == Mnemonic.opSQR[MN])
                return new byte[] { 0x60, 0x1C };
            else if (myCmd.Command == Mnemonic.opSQRT[MN])
                return new byte[] { 0x60, 0x14 };
            else if (myCmd.Command == Mnemonic.opEXP[MN])
                return new byte[] { 0x60, 0x1B };
            else if (myCmd.Command == Mnemonic.opLN[MN])
                return new byte[] { 0x60, 0x13 };
            else if (myCmd.Command == Mnemonic.opSIN[MN])
                return new byte[] { 0x60, 0x10 };
            else if (myCmd.Command == Mnemonic.opCOS[MN])
                return new byte[] { 0x60, 0x11 };
            else if (myCmd.Command == Mnemonic.opTAN[MN])
                return new byte[] { 0x60, 0x12 };
            else if (myCmd.Command == Mnemonic.opASIN[MN])
                return new byte[] { 0x60, 0x18 };
            else if (myCmd.Command == Mnemonic.opACOS[MN])
                return new byte[] { 0x60, 0x19 };
            else if (myCmd.Command == Mnemonic.opATAN[MN])
                return new byte[] { 0x60, 0x1A };
            else if (myCmd.Command == "+R")
                return new byte[] { 0x60, 0x0F };
            else if (myCmd.Command == "-R")
                return new byte[] { 0x60, 0x0B };
            else if (myCmd.Command == "*R")
                return new byte[] { 0x60, 0x07 };
            else if (myCmd.Command == "/R")
                return new byte[] { 0x60, 0x03 };
            else if (myCmd.Command == Mnemonic.opBTI[MN])
                return new byte[] { 0x68, 0x0C };
            else if (myCmd.Command == Mnemonic.opITB[MN])
                return new byte[] { 0x68, 0x08 };
            else if (myCmd.Command == Mnemonic.opBTD[MN])
                return new byte[] { 0x68, 0x08 };
            else if (myCmd.Command == Mnemonic.opITD[MN])
                return new byte[] { 0x68, 0x1E };
            else if (myCmd.Command == Mnemonic.opDTB[MN])
                return new byte[] { 0x68, 0x0A };
            else if (myCmd.Command == Mnemonic.opDTR[MN])
                return new byte[] { 0x68, 0x06 };
            else if (myCmd.Command == Mnemonic.opINVI[MN])
                return new byte[] { 0x01, 0x00 };
            else if (myCmd.Command == Mnemonic.opINVD[MN])
                return new byte[] { 0x68, 0x0D };
            else if (myCmd.Command == Mnemonic.opNEGI[MN])
                return new byte[] { 0x09, 0x00 };
            else if (myCmd.Command == Mnemonic.opNEGD[MN])
                return new byte[] { 0x68, 0x07 };
            else if (myCmd.Command == Mnemonic.opNEGR[MN])
                return new byte[] { 0x60, 0x06 };
            else if (myCmd.Command == Mnemonic.opTAW[MN])
                return new byte[] { 0x68, 0x1A };
            else if (myCmd.Command == Mnemonic.opTAD[MN])
                return new byte[] { 0x68, 0x1B };
            else if (myCmd.Command == Mnemonic.opRND[MN])
                return new byte[] { 0x68, 0x5C };
            else if (myCmd.Command == Mnemonic.opTRUNC[MN])
                return new byte[] { 0x68, 0x5F };
            else if (myCmd.Command == Mnemonic.opRNDP[MN])
                return new byte[] { 0x68, 0x5E };
            else if (myCmd.Command == Mnemonic.opRNDM[MN])
                return new byte[] { 0x68, 0x5D };
            else if (myCmd.Command == Mnemonic.opABS[MN])
                return new byte[] { 0x60, 0x02 };
            else if (myCmd.Command == Mnemonic.opBE[MN])
                return new byte[] { 0x65, 0x00 };
            else if (myCmd.Command == Mnemonic.opBEA[MN])
                return new byte[] { 0x65, 0x01 };
            else if (myCmd.Command == Mnemonic.opBEB[MN])
                return new byte[] { 0x05, 0x00 };
            else if (myCmd.Command == Mnemonic.opCLR[MN])
                return new byte[] { 0x68, 0x1C };
            else if (myCmd.Command == Mnemonic.opNOT[MN])
                return new byte[] { 0x68, 0x2D };
            else if (myCmd.Command == Mnemonic.opSET[MN])
                return new byte[] { 0x68, 0x1D };
            else if (myCmd.Command == Mnemonic.opSAVE[MN])
                return new byte[] { 0x68, 0x2C };
            else if (myCmd.Command == Mnemonic.opUO[MN])
                return new byte[] { 0xBA, 0x00 };
            else if (myCmd.Command == Mnemonic.opOO[MN])
                return new byte[] { 0xBB, 0x00 };
            else if (myCmd.Command == Mnemonic.opUNO[MN])
                return new byte[] { 0xFF, 0xF1 };
            else if (myCmd.Command == Mnemonic.opONO[MN])
                return new byte[] { 0xFF, 0xF3 };
            else if (myCmd.Command == Mnemonic.opMCRA[MN])
                return new byte[] { 0x68, 0x3a };
            else if (myCmd.Command == Mnemonic.opMCRD[MN])
                return new byte[] { 0x68, 0x3b };
            else if (myCmd.Command == Mnemonic.opMCRO[MN])
                return new byte[] { 0x68, 0x3c };
            else if (myCmd.Command == Mnemonic.opMCRC[MN])
                return new byte[] { 0x68, 0x3d };
            else if (myCmd.Command == Mnemonic.opXO[MN])
                return new byte[] { 0xFF, 0xF4 };
            else if (myCmd.Command == Mnemonic.opXNO[MN])
                return new byte[] { 0xFF, 0xF5 };
            else if (myCmd.Command == Mnemonic.opTAR[MN])
                return new byte[] { 0xFE, 0x08 };
            else if (myCmd.Command == Mnemonic.opTDB[MN])
                return new byte[] { 0xFB, 0x7C };
            else if (myCmd.Command == Mnemonic.opUC[MN])
            {
                switch (myCmd.GetParameterType())
                {
                    case PLCFunctionBlockAdressType.Direct:
                        if (myCmd.Parameter.ToUpper().Contains("FB"))
                            if (Helper.GetParameterValueInt(myCmd.Parameter) < 256)
                                return new byte[] { 0x75, Helper.GetParameterValueBytes(myCmd.Parameter)[0] };
                            else
                                return new byte[] { 0xFB, 0x72, Helper.GetParameterValueBytes(myCmd.Parameter)[1], Helper.GetParameterValueBytes(myCmd.Parameter)[0] };
                        else if (myCmd.ExtParameter == null || myCmd.ExtParameter.Count == 0)
                            if (Helper.GetParameterValueInt(myCmd.Parameter) < 256)
                                return new byte[] { 0x3D, Helper.GetParameterValueBytes(myCmd.Parameter)[0], 0x70, 0x0B, 0x00, 0x02 };
                            else
                                return new byte[] { 0xFB, 0x70, Helper.GetParameterValueBytes(myCmd.Parameter)[1], Helper.GetParameterValueBytes(myCmd.Parameter)[0], 0x70, 0x0B, 0x00, 0x02 };
                        else
                        {
                            List<byte> myAddVal = new List<byte>();
                            if (Helper.GetParameterValueInt(myCmd.Parameter) < 256)
                                myAddVal.AddRange(new byte[] { 0xFB, 0x70, 0x00, Helper.GetParameterValueBytes(myCmd.Parameter)[0], 0x70, 0x0B, 0x00 });
                            else
                                myAddVal.AddRange(new byte[] { 0xFB, 0x70, Helper.GetParameterValueBytes(myCmd.Parameter)[1], Helper.GetParameterValueBytes(myCmd.Parameter)[0], 0x70, 0x0B, 0x00 });
                            myAddVal.Add(Convert.ToByte(myCmd.ExtParameter.Count * 2 + 2));
                            foreach (string par in myCmd.ExtParameter)
                                myAddVal.AddRange(Helper.GetCallParameterByte(par));
                            return myAddVal.ToArray();
                        }
                    case PLCFunctionBlockAdressType.Indirect:
                        if (myCmd.Parameter.ToUpper().Contains("FB"))
                            return new byte[] { 0xFB, Convert.ToByte(Helper.GetIndirectBytesWord(myCmd.Parameter)[2] | 0x02), Helper.GetIndirectBytesWord(myCmd.Parameter)[1], Helper.GetIndirectBytesWord(myCmd.Parameter)[0], 0x70, 0x0B, 0x00, 0x02 };
                        else
                            return new byte[] { 0xFB, Convert.ToByte(Helper.GetIndirectBytesWord(myCmd.Parameter)[2] | 0x00), Helper.GetIndirectBytesWord(myCmd.Parameter)[1], Helper.GetIndirectBytesWord(myCmd.Parameter)[0], 0x70, 0x0B, 0x00, 0x02 };
                }
            }
            /*
        else if (myCmd.Command == Memnoic.opAUF[MN])
            switch (myCmd.GetParameterType())
            {
                case PLCFunctionBlockAdressType.
                if (myCmd.Parameter.ToUpper().Contains("FB"))
                        if (Helper.GetParameterValueInt(myCmd.Parameter) < 256)
            return new byte[] { 0xFB, 0x7C };  0x20 //auf db
                                                0xFB, 0x38 //db [mw]  48=dbw    58=diw  68=lw
                                                      0x39 //di [mw]
                                                0xfb, 0x78  //auf db >256
                                                0xfb, 0x79  //auf di >256
                                                0xfb, 0xd8  //auf db #par

             * 
             * 
                        public static string[] opU = new[] { "U", "U" };
                        public static string[] opFN = new[] { "FN", "FN" };
                        public static string[] opFP = new[] { "FP", "FP" };
                        public static string[] opO = new[] { "O", "O" };
                        public static string[] opR = new[] { "R", "R" };
                        public static string[] opS = new[] { "S", "S" };
                        public static string[] opUN = new[] { "UN", "UN" };
                        public static string[] opX = new[] { "X", "X" };
                        public static string[] opXN = new[] { "XN", "XN" };
                        public static string[] opZUW = new[] { "=", "=" }; 

             */
            /*
                 * Commands still to do:
                        public static string[] opAUF = new[] { "AUF", "OPN" };                       
                        public static string[] opCC = new[] { "CC", "CC" };
             * 
                        
             *          public static string[] opFR = new[] { "FR", "FR" };
             *          
                        public static string[] opL = new[] { "L", "L" };
                        
             *          public static string[] opLAR1 = new[] { "LAR1", "LAR1" };
                        public static string[] opLAR2 = new[] { "LAR2", "LAR2" };
                        public static string[] opLC = new[] { "LC", "LC" };
                        public static string[] opLN = new[] { "LN", "LN" };
                                               
             *          public static string[] opOD = new[] { "OD", "OD" };
                        public static string[] opON = new[] { "ON", "ON" };
                        public static string[] opOW = new[] { "OW", "OW" };
                        public static string[] opPLU = new[] { "+", "+" };
                        public static string[] opRLD = new[] { "RLD", "RLD" };
                        public static string[] opRLDA = new[] { "RLDA", "RLDA" };
                        public static string[] opRRD = new[] { "RRD", "RRD" };
                        public static string[] opRRDA = new[] { "RRDA", "RRDA" };
                        public static string[] opSA = new[] { "SA", "SF" };
                        public static string[] opSE = new[] { "SE", "SD" };
                        public static string[] opSI = new[] { "SI", "SP" };
                        public static string[] opSLD = new[] { "SLD", "SLD" };
                        public static string[] opSLW = new[] { "SLW", "SLW" };
                      
             *        * public static string[] opLOOP = new[] { "LOOP", "LOOP" };
                        public static string[] opSPA = new[] { "SPA", "JU" };
                        public static string[] opSPB = new[] { "SPB", "JC" };
                        public static string[] opSPBB = new[] { "SPBB", "JCB" };
                        public static string[] opSPBI = new[] { "SPBI", "JBI" };
                        public static string[] opSPBIN = new[] { "SPBIN", "JBNI" };
                        public static string[] opSPBN = new[] { "SPBN", "JCN" };
                        public static string[] opSPBNB = new[] { "SPBNB", "JNB" };
                        public static string[] opSPL = new[] { "SPL", "SPL" };
                        public static string[] opSPM = new[] { "SPM", "SPM" };
                        public static string[] opSPMZ = new[] { "SPMZ", "SPMZ" };
                        public static string[] opSPN = new[] { "SPN", "SPN" };
                        public static string[] opSPO = new[] { "SPO", "SPO" };
                        public static string[] opSPP = new[] { "SPP", "SPP" };
                        public static string[] opSPPZ = new[] { "SPPZ", "SPPZ" };
                        public static string[] opSPS = new[] { "SPS", "SPS" };
                        public static string[] opSPU = new[] { "SPU", "SPU" };
                        public static string[] opSPZ = new[] { "SPZ", "SPZ" };

                        public static string[] opSRD = new[] { "SRD", "SRD" };
                        public static string[] opSRW = new[] { "SRW", "SRW" };
                        public static string[] opSS = new[] { "SS", "SS" };
                        public static string[] opSSD = new[] { "SSD", "SSD" };
                        public static string[] opSSI = new[] { "SSI", "SSI" };
                        public static string[] opSV = new[] { "SV", "SV" };
                        public static string[] opT = new[] { "T", "T" };
                        public static string[] opTAR1 = new[] { "TAR1", "TAR1" };
                        public static string[] opTAR2 = new[] { "TAR2", "TAR2" };
                        public static string[] opUD = new[] { "UD", "UD" };
                        public static string[] opUW = new[] { "UW", "UW" };
                        public static string[] opXOD = new[] { "XOD", "XOD" };
                        public static string[] opXOW = new[] { "XOW", "XOW" };
                        public static string[] opZR = new[] { "ZR", "ZR" };
                        public static string[] opZV = new[] { "ZV", "ZV" };
                        
                 * End of commands list
                                */



            return null;
        }
    }
}
