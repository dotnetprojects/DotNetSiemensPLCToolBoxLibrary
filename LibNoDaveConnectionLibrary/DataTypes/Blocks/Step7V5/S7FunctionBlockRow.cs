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
using System.ComponentModel;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    [Serializable()]
    public class S7FunctionBlockRow : FunctionBlockRow, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        internal override void resetByte()
        {
            _MC7 = null;
            CombinedCommands = null;
        }

        //These Commands are Combined...
        public List<FunctionBlockRow> CombinedCommands { get; internal set; }

        private string _parameter;
        public string Parameter
        {
            get { return _parameter; }
            set
            {
                _parameter = value;
                _SymbolTableEntry = null;
                _MC7 = null;
                CombinedCommands = null;
            }
        }

        private SymbolTableEntry _SymbolTableEntry;
        public SymbolTableEntry SymbolTableEntry
        {
            get { return _SymbolTableEntry; }
            set
            {
                _SymbolTableEntry = value;
            }
        }

        private List<string> _extparameter;
        public List<string> ExtParameter
        {
            get { return _extparameter; }
            set
            {
                _extparameter = value;
                _MC7 = null;
                CombinedCommands = null;
            }
        }

        internal int GetNumberOfLines()
        {
            if ((Command == "UC" || Command == "CC") && ExtParameter != null)
                return 1 + ExtParameter.Count;
            else if (Command == "CALL" && CallParameter != null)
                return 1 + CallParameter.Count;
            return 1;
        }

        private List<S7FunctionBlockParameter> _callparameter;
        public List<S7FunctionBlockParameter> CallParameter
        {
            get { return _callparameter; }
            set
            {
                _callparameter = value;
                _MC7 = null;
                CombinedCommands = null;
            }
        }


        public string Comment { get; set; }

        public string NetworkName { get; set; }

        private int _jumpwidth;
        internal int JumpWidth
        {
            get { return _jumpwidth; }
            set
            {
                _jumpwidth = value;
                _MC7 = null;
                CombinedCommands = null;
            }
        }

        private byte[] _MC7;
        public byte[] MC7
        {
            get
            {
                if (_MC7 != null)
                    return _MC7;
                else
                    return AWLtoMC7.GetMC7(this);
            }
            internal set
            {
                _MC7 = value;
            }
        }

        public PLCFunctionBlockAdressType GetParameterType()
        {
            return PLCFunctionBlockAdressType.Direct;
        }

        #region PlcBlockStatus

        //This stores wich registers where asked within the last diagnostiv request!
        internal SelectedStatusValues askedStatusValues;

        [Flags]
        public enum SelectedStatusValues
        {
            STW = 1,
            Akku1 = 2,
            Akku2 = 4,
            AR1 = 8,
            AR2 = 16,
            DB = 32,
            ALL = 0x3F
        }
        public static short _GetCommandStatusAskSize(SelectedStatusValues mySel, byte telegrammType)
        {
            short size = 0;

            if (telegrammType == 0x13)
            {
                if ((mySel & SelectedStatusValues.STW) > 0)
                    size += 2;
                if ((mySel & SelectedStatusValues.Akku1) > 0)
                    size += 4;
                if ((mySel & SelectedStatusValues.Akku2) > 0)
                    size += 4;
                if ((mySel & SelectedStatusValues.AR1) > 0)
                    size += 4;
                if ((mySel & SelectedStatusValues.AR2) > 0)
                    size += 4;
                if ((mySel & SelectedStatusValues.DB) > 0)
                    size += 6;
                return size;
            }
            else
            {
                if ((mySel & SelectedStatusValues.STW) > 0)
                    size += 2;
                if ((mySel & SelectedStatusValues.Akku1) > 0 || (mySel & SelectedStatusValues.Akku2) > 0)
                    size += 8;
                if ((mySel & SelectedStatusValues.AR1) > 0)
                    size += 4;
                if ((mySel & SelectedStatusValues.AR2) > 0)
                    size += 4;
                if ((mySel & SelectedStatusValues.DB) > 0)
                    size += 6;
                return size;
            }
        }

        /*public byte _GetCommandStatusAskByte(SelectedStatusValues mySel, byte telegrammType)
        {
            byte retval = 0x00;

            if (telegrammType == 0x13)
                return (byte) _GetCommandStatusAskValues(mySel);
            else
            {
                SelectedStatusValues tmp = _GetCommandStatusAskValues(mySel);
                retval |= (mySel & SelectedStatusValues.STW) > 0 ? (byte)0x00 : (byte)0x00;
                retval |= (mySel & SelectedStatusValues.Akku1) > 0 ? (byte)0x01 : (byte)0x00;
                retval |= (mySel & SelectedStatusValues.Akku2) > 0 ? (byte)0x01 : (byte)0x00;
                retval |= (mySel & SelectedStatusValues.AR1) > 0 ? (byte)0x02 : (byte)0x00;
                retval |= (mySel & SelectedStatusValues.AR2) > 0 ? (byte)0x04 : (byte)0x00;
                retval |= (mySel & SelectedStatusValues.DB) > 0 ? (byte)0x08 : (byte)0x00;
                return retval;
            }
        }*/

        public SelectedStatusValues _GetCommandStatusAskValues(SelectedStatusValues mySel, byte DiagDataTeletype)
        {
            int MN = 0;

            SelectedStatusValues retVal;

            bool isDBcall = false;
            isDBcall = System.Text.RegularExpressions.Regex.IsMatch(this.Parameter, "DB[0-9]");

            //Todo: Look at this command wich registeres are changed
            if (this.Command == Memnoic.opCALL[MN] || this.Command == Memnoic.opCC[MN] ||
                this.Command == Memnoic.opENT[MN] || this.Command == Memnoic.opEXP[MN] ||
                this.Command == Memnoic.opFR[MN] ||
                this.Command == Memnoic.opINC[MN] || this.Command == Memnoic.opINVD[MN] ||
                this.Command == Memnoic.opINVI[MN] || this.Command == Memnoic.opITB[MN] ||
                this.Command == Memnoic.opITD[MN] ||

                this.Command == Memnoic.opLC[MN] || this.Command == Memnoic.opLEAVE[MN] ||
                this.Command == Memnoic.opLN[MN] || this.Command == Memnoic.opLOOP[MN] ||
                this.Command == Memnoic.opMCRA[MN] || this.Command == Memnoic.opMCRC[MN] ||
                this.Command == Memnoic.opMCRD[MN] || this.Command == Memnoic.opMCRO[MN] ||
                this.Command == Memnoic.opNEGD[MN] ||
                this.Command == Memnoic.opNEGI[MN] || this.Command == Memnoic.opNEGR[MN] ||
                this.Command == Memnoic.opNOT[MN] ||
                this.Command == Memnoic.opOD[MN] ||
                this.Command == Memnoic.opONO[MN] || this.Command == Memnoic.opOO[MN] ||
                this.Command == Memnoic.opOW[MN] || this.Command == Memnoic.opPAR1[MN] ||
                this.Command == Memnoic.opPAR2[MN] ||
                this.Command == Memnoic.opPOP[MN] || this.Command == Memnoic.opPUSH[MN] ||
                this.Command == Memnoic.opRLD[MN] ||
                this.Command == Memnoic.opRLDA[MN] || this.Command == Memnoic.opRND[MN] ||
                this.Command == Memnoic.opRNDM[MN] || this.Command == Memnoic.opRNDP[MN] ||
                this.Command == Memnoic.opRRD[MN] || this.Command == Memnoic.opRRDA[MN] ||
                this.Command == Memnoic.opSA[MN] ||
                this.Command == Memnoic.opSAVE[MN] || this.Command == Memnoic.opSE[MN] ||
                this.Command == Memnoic.opSI[MN] ||
                this.Command == Memnoic.opSS[MN] || this.Command == Memnoic.opSSD[MN] ||
                this.Command == Memnoic.opSSI[MN] || this.Command == Memnoic.opSV[MN] ||
                this.Command == Memnoic.opTRUNC[MN] ||
                this.Command == Memnoic.opUO[MN] || this.Command == Memnoic.opUC[MN] ||
                this.Command == Memnoic.opUNO[MN] || this.Command == Memnoic.opUD[MN] ||
                this.Command == Memnoic.opUW[MN] ||
                this.Command == Memnoic.opXNO[MN] || this.Command == Memnoic.opXO[MN] ||
                this.Command == Memnoic.opXOD[MN] || this.Command == Memnoic.opXOW[MN] ||
                this.Command == Memnoic.opZR[MN] || this.Command == Memnoic.opZV[MN])
                retVal = (SelectedStatusValues.Akku1 | SelectedStatusValues.Akku2 | SelectedStatusValues.AR1 |
                        SelectedStatusValues.AR2 | SelectedStatusValues.DB | SelectedStatusValues.STW) & mySel;
            else if (this.Command == "+I" || this.Command == "-I" || this.Command == "*I" || this.Command == "/I" || this.Command == "+D" || this.Command == "-D" || this.Command == "*D" || this.Command == "/D" || this.Command == "+R" || this.Command == "-R" || this.Command == "*R" || this.Command == "/R" || this.Command == "MOD")
                retVal = (SelectedStatusValues.Akku1 | SelectedStatusValues.Akku2 | SelectedStatusValues.STW) & mySel;
            else if (this.Command == Memnoic.opPLU[MN] || this.Command == Memnoic.opABS[MN] || this.Command == Memnoic.opBTD[MN] ||
                this.Command == Memnoic.opACOS[MN] || this.Command == Memnoic.opASIN[MN] || this.Command == Memnoic.opBTI[MN] ||
                this.Command == Memnoic.opATAN[MN] || this.Command == Memnoic.opTAN[MN] || this.Command == Memnoic.opSIN[MN] ||
                this.Command == Memnoic.opCOS[MN] || this.Command == Memnoic.opSQR[MN] || this.Command == Memnoic.opSQRT[MN] ||
                this.Command == Memnoic.opDEC[MN] || this.Command == Memnoic.opTAW[MN] || this.Command == Memnoic.opTAD[MN] ||
                this.Command == Memnoic.opDTB[MN] || this.Command == Memnoic.opDTR[MN] || this.Command == Memnoic.opSLD[MN] ||
                this.Command == Memnoic.opSLW[MN] || this.Command == Memnoic.opSRD[MN] || this.Command == Memnoic.opSRW[MN])
                retVal = (SelectedStatusValues.Akku1) & mySel;
            else if (this.Command == Memnoic.opTAR[MN])
                retVal = (SelectedStatusValues.AR1 | SelectedStatusValues.AR2) & mySel;
            else if (this.Command == Memnoic.opLAR1[MN])
                retVal = (SelectedStatusValues.AR1) & mySel;
            else if (this.Command == Memnoic.opLAR2[MN])
                retVal = (SelectedStatusValues.AR2) & mySel;
            else if ((this.Command == Memnoic.opL[MN]) && isDBcall)
                retVal = (SelectedStatusValues.Akku1 | SelectedStatusValues.Akku2 | SelectedStatusValues.DB) & mySel;
            else if ((this.Command == Memnoic.opT[MN]) && isDBcall)
                retVal = (SelectedStatusValues.DB) & mySel;
            else if (this.Command == Memnoic.opTAK[MN] || this.Command == Memnoic.opL[MN] || this.Command == Memnoic.opTAR1[MN] || this.Command == Memnoic.opTAR2[MN])
                retVal = (SelectedStatusValues.Akku1 | SelectedStatusValues.Akku2) & mySel;
            else if ((this.Command == Memnoic.opU[MN] || this.Command == Memnoic.opUN[MN] || this.Command == Memnoic.opO[MN] || this.Command == Memnoic.opON[MN] || this.Command == Memnoic.opX[MN] || this.Command == Memnoic.opXN[MN] || this.Command == Memnoic.opFN[MN] || this.Command == Memnoic.opFP[MN] || this.Command == Memnoic.opZUW[MN] || this.Command == Memnoic.opS[MN] || this.Command == Memnoic.opR[MN] || this.Command == Memnoic.opT[MN]) && isDBcall)
                retVal = (SelectedStatusValues.STW | SelectedStatusValues.DB) & mySel;
            else if (this.Command == Memnoic.opU[MN] || this.Command == Memnoic.opUN[MN] || this.Command == Memnoic.opO[MN] ||
                    this.Command == Memnoic.opON[MN] || this.Command == Memnoic.opX[MN] || this.Command == Memnoic.opXN[MN] ||
                    this.Command == Memnoic.opFN[MN] || this.Command == Memnoic.opFP[MN] || this.Command == Memnoic.opZUW[MN] ||
                    this.Command == Memnoic.opS[MN] || this.Command == Memnoic.opR[MN] || this.Command == Memnoic.opBE[MN] ||
                    this.Command == Memnoic.opBEA[MN] || this.Command == Memnoic.opBEB[MN] || this.Command == ")")
                retVal = (SelectedStatusValues.STW) & mySel;
            else if (this.Command == Memnoic.opT[MN] || this.Command == Memnoic.opBLD[MN] || this.Command == Memnoic.opNOP[MN] ||
                     this.Command == Memnoic.opSPA[MN])
                retVal = 0;
            else if (this.Command == Memnoic.opAUF[MN] || this.Command == Memnoic.opTDB[MN])
                retVal = (SelectedStatusValues.DB) & mySel;
            else if (this.Command == Memnoic.opSPB[MN] || this.Command == Memnoic.opSPBB[MN] ||
                     this.Command == Memnoic.opSPBI[MN] || this.Command == Memnoic.opSPBIN[MN] ||
                     this.Command == Memnoic.opSPBN[MN] || this.Command == Memnoic.opSPBNB[MN] ||
                     this.Command == Memnoic.opSPL[MN] || this.Command == Memnoic.opSPM[MN] ||
                     this.Command == Memnoic.opSPMZ[MN] || this.Command == Memnoic.opSPN[MN] ||
                     this.Command == Memnoic.opSPO[MN] || this.Command == Memnoic.opSPP[MN] ||
                     this.Command == Memnoic.opSPPZ[MN] || this.Command == Memnoic.opSPS[MN] ||
                     this.Command == Memnoic.opSPU[MN] || this.Command == Memnoic.opSPZ[MN] ||
                     this.Command == Memnoic.opSET[MN] || this.Command == Memnoic.opCLR[MN])
                retVal = (SelectedStatusValues.STW) & mySel;
            else
                retVal = (SelectedStatusValues.Akku1 | SelectedStatusValues.Akku2 | SelectedStatusValues.AR1 |
                      SelectedStatusValues.AR2 | SelectedStatusValues.DB | SelectedStatusValues.STW) & mySel;

            //Return STW as Required minimum every Time (even if nothing is read, 
            //but you need it to detect a not called line! (in a jump or smth. similar))
            //if (retVal == 0)
            //    retVal = SelectedStatusValues.STW;
            if (DiagDataTeletype == 0x01 && ((retVal & SelectedStatusValues.Akku1) > 0) || (retVal & SelectedStatusValues.Akku2) > 0)
                retVal |= SelectedStatusValues.Akku1 | SelectedStatusValues.Akku2;
            if (DiagDataTeletype == 0x01 && retVal != 0)
                return retVal | SelectedStatusValues.STW;

            return retVal;
        }

        private BlockStatus _actualBlockStatus;
        public BlockStatus ActualBlockStatus
        {
            get { return _actualBlockStatus; }
            set
            {
                _actualBlockStatus = value;
                NotifyPropertyChanged("ActualBlockStatus");
            }
        }

        public class BlockStatus : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private
            void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }


            private short? _stw;
            public short? STW
            {
                get { return _stw; }
                set
                {
                    _stw = value;
                    NotifyPropertyChanged("STW");
                    NotifyPropertyChanged("VKE");
                    NotifyPropertyChanged("STA");
                    NotifyPropertyChanged("OR");
                    NotifyPropertyChanged("OS");
                    NotifyPropertyChanged("OV");
                    NotifyPropertyChanged("A1");
                    NotifyPropertyChanged("A0");
                    NotifyPropertyChanged("BIE");
                }
            }

            public bool? VKE { get { return Convert.ToBoolean(STW & 2); } }
            public bool? STA { get { return Convert.ToBoolean(STW & 4); } }
            public bool? OR { get { return Convert.ToBoolean(STW & 8); } }
            public bool? OS { get { return Convert.ToBoolean(STW & 16); } }
            public bool? OV { get { return Convert.ToBoolean(STW & 32); } }
            public bool? A1 { get { return Convert.ToBoolean(STW & 64); } }
            public bool? A0 { get { return Convert.ToBoolean(STW & 128); } }
            public bool? BIE { get { return Convert.ToBoolean(STW & 256); } }

            private int? _akku1;
            public int? Akku1
            {
                get { return _akku1; }
                set
                {
                    _akku1 = value;
                    NotifyPropertyChanged("Akku1");
                }
            }

            private int? _akku2;
            public int? Akku2
            {
                get { return _akku2; }
                set
                {
                    _akku2 = value;
                    NotifyPropertyChanged("Akku2");
                }
            }

            private int? _ar1;
            public int? AR1
            {
                get { return _ar1; }
                set
                {
                    _ar1 = value;
                    NotifyPropertyChanged("AR1");
                }
            }

            private int? _ar2;
            public int? AR2
            {
                get { return _ar2; }
                set
                {
                    _ar2 = value;
                    NotifyPropertyChanged("AR2");
                }
            }

            private int? _db;
            public int? DB
            {
                get { return _db; }
                set
                {
                    _db = value;
                    NotifyPropertyChanged("DB");
                }
            }

            private int? _di;
            public int? DI
            {
                get { return _di; }
                set
                {
                    _di = value;
                    NotifyPropertyChanged("DI");
                }
            }

            public override string ToString()
            {
                StringBuilder ret = new StringBuilder();
                if (STW == null)
                    ret.Append("STW: ---------");
                else
                    ret.Append("STW: " + Convert.ToString((int)STW, 2).PadLeft(9, '0'));
                ret.Append(" | ");
                if (Akku1 == null)
                    ret.Append("A1: --------");
                else
                    ret.Append("A1: " + ((int)Akku1).ToString("X").PadLeft(8, '0'));
                ret.Append(" | ");
                if (Akku2 == null)
                    ret.Append("A2: --------");
                else
                    ret.Append("A2: " + ((int)Akku2).ToString("X").PadLeft(8, '0'));
                ret.Append(" | ");
                if (DB == null)
                    ret.Append("DB: ----");
                else
                    ret.Append("DB: " + ((short)DB).ToString().PadLeft(4, '0'));
                ret.Append(" | ");
                if (DI == null)
                    ret.Append("DI: ----");
                else
                    ret.Append("DI: " + ((short)DI).ToString().PadLeft(4, '0'));
                ret.Append(" | ");
                if (AR1 == null)
                    ret.Append("A1R: --------");
                else
                    ret.Append("AR1: " + ((int)AR1).ToString("X").PadLeft(8, '0'));
                ret.Append(" | ");
                if (AR2 == null)
                    ret.Append("AR2: --------");
                else
                    ret.Append("AR2: " + ((int)AR2).ToString("X").PadLeft(8, '0'));
                return ret.ToString();
            }

            public static BlockStatus ReadBlockStatus(byte[] daten, int startpos, SelectedStatusValues selStat, BlockStatus oldStatus)
            {
                BlockStatus ret = new BlockStatus();

                ret.STW = oldStatus.STW;
                ret.Akku1 = oldStatus.Akku1;
                ret.Akku2 = oldStatus.Akku2;
                ret.AR1 = oldStatus.AR1;
                ret.AR2 = oldStatus.AR2;
                ret.DB = oldStatus.DB;
                ret.DI = oldStatus.DI;

                int pos = startpos;

                if ((selStat & SelectedStatusValues.STW) > 0)
                {
                    ret.STW = (short?)(daten[pos] * 0x100 + daten[pos + 1]);
                    pos += 2;
                }
                if ((selStat & SelectedStatusValues.Akku1) > 0)
                {
                    ret.Akku1 = libnodave.getS32from(daten, pos);
                    pos += 4;
                }
                if ((selStat & SelectedStatusValues.Akku2) > 0)
                {
                    ret.Akku2 = libnodave.getS32from(daten, pos);
                    pos += 4;
                }
                if ((selStat & SelectedStatusValues.AR1) > 0)
                {
                    ret.AR1 = libnodave.getS32from(daten, pos);
                    pos += 4;
                }
                if ((selStat & SelectedStatusValues.AR2) > 0)
                {
                    ret.AR2 = libnodave.getS32from(daten, pos);
                    pos += 4;
                }
                if ((selStat & SelectedStatusValues.DB) > 0)
                {
                    //if (daten[pos] > 0)
                    ret.DB = libnodave.getU16from(daten, pos + 2);
                    //if (daten[pos + 1] > 0)
                    ret.DI = libnodave.getU16from(daten, pos + 4);
                    pos += 6;
                }
                return ret;
            }
        }
        #endregion

        public S7FunctionBlockRow()
        {
            Label = "";
            Command = "";
            Parameter = "";
            Comment = "";
            NetworkName = "";
        }

        public int ByteSize
        {
            get
            {
                if (MC7 != null)
                    return MC7.Length;
                else
                    return 0;
            }
        }

        public override string ToString()
        {
            return ToString(true,false);
        }


        public string ToString(bool useSymbol, bool addSemicolonAfterCommand)
        {
            if (Command == "NETWORK")
            {
                if (string.IsNullOrEmpty(Comment))
                    return "Netzwerk " + Parameter + " : " + NetworkName;
                else
                    return "Netzwerk " + Parameter + " : " + NetworkName + "\r\n\t Comment : " + Comment.Replace("\n", "\r\n\t           ");
            }

            string retVal = "";
            if (Label == null || Label == "")
                retVal += new string(' ', 6);
            else
                retVal += Label.PadRight(4) + ": ";

            string cmt = "";
            if (Comment != null && Comment != "")
                cmt = "//" + Comment;

            string ext = "";

            if (ExtParameter != null && ExtParameter.Count > 0 && (Command == "UC" || Command == "CC"))
            {
                foreach (string myStr in ExtParameter)
                    ext += "\r\n" + " ".PadLeft(12) + myStr;
            }

            if (CallParameter != null && CallParameter.Count > 0 && (Command == "CALL"))
            {
                int len = 0;
                foreach (var cpar in CallParameter)
                    len = cpar.Name.Length > len ? cpar.Name.Length : len;
                foreach (var cpar in CallParameter)
                {
                    ext += "\r\n" + " ".PadLeft(12) + cpar.Name.PadRight(len) + ":=" + cpar.Value;
                    if (!string.IsNullOrEmpty(cpar.Comment))
                        ext += "  //" + cpar.Comment;
                }
            }

            if (ActualBlockStatus != null)
                cmt += "     Status: " + ActualBlockStatus.ToString();

            if (Command == "" && Parameter == "")
                return cmt;

            string par = "";
            if (Parameter != null)
                par = Parameter;
            if (_SymbolTableEntry != null && useSymbol)
                par = SymbolTableEntry.Symbol;

            if (!string.IsNullOrEmpty(cmt))
                par = par.PadRight(14);


            return retVal + Command.PadRight(6) + par + (addSemicolonAfterCommand == true ? ";" : "") + cmt + ext; // +"Sz:" + ByteSize.ToString();
        }
    }
}
