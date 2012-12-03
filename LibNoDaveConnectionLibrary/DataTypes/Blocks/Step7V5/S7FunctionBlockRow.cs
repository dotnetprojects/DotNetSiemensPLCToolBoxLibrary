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
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

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

        private bool _CombineDbAccess = false;
        internal bool CombineDBAccess
        {
            get { return _CombineDbAccess; }
            set { _CombineDbAccess = value; }
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

        /// <summary>
        /// Returns the Number of Lines this Command needs (Calls needs more then 1 Line)
        /// </summary>
        /// <returns></returns>
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

            //Todo: Look at this commands wich registeres are changed
            if (this.Command == Mnemonic.opCALL[MN] || this.Command == Mnemonic.opCC[MN] ||
                this.Command == Mnemonic.opENT[MN] || this.Command == Mnemonic.opEXP[MN] ||
                this.Command == Mnemonic.opFR[MN] ||
                this.Command == Mnemonic.opINC[MN] || this.Command == Mnemonic.opINVD[MN] ||
                this.Command == Mnemonic.opINVI[MN] || this.Command == Mnemonic.opITB[MN] ||
                this.Command == Mnemonic.opITD[MN] ||

                this.Command == Mnemonic.opLC[MN] || this.Command == Mnemonic.opLEAVE[MN] ||
                this.Command == Mnemonic.opLN[MN] || this.Command == Mnemonic.opLOOP[MN] ||
                this.Command == Mnemonic.opMCRA[MN] || this.Command == Mnemonic.opMCRC[MN] ||
                this.Command == Mnemonic.opMCRD[MN] || this.Command == Mnemonic.opMCRO[MN] ||
                this.Command == Mnemonic.opNEGD[MN] ||
                this.Command == Mnemonic.opNEGI[MN] || this.Command == Mnemonic.opNEGR[MN] ||
                this.Command == Mnemonic.opNOT[MN] ||
                this.Command == Mnemonic.opOD[MN] ||
                this.Command == Mnemonic.opONO[MN] || this.Command == Mnemonic.opOO[MN] ||
                this.Command == Mnemonic.opOW[MN] || this.Command == Mnemonic.opPAR1[MN] ||
                this.Command == Mnemonic.opPAR2[MN] ||
                this.Command == Mnemonic.opPOP[MN] || this.Command == Mnemonic.opPUSH[MN] ||
                this.Command == Mnemonic.opRLD[MN] ||
                this.Command == Mnemonic.opRLDA[MN] || this.Command == Mnemonic.opRND[MN] ||
                this.Command == Mnemonic.opRNDM[MN] || this.Command == Mnemonic.opRNDP[MN] ||
                this.Command == Mnemonic.opRRD[MN] || this.Command == Mnemonic.opRRDA[MN] ||
                this.Command == Mnemonic.opSA[MN] ||
                this.Command == Mnemonic.opSAVE[MN] || this.Command == Mnemonic.opSE[MN] ||
                this.Command == Mnemonic.opSI[MN] ||
                this.Command == Mnemonic.opSS[MN] || this.Command == Mnemonic.opSSD[MN] ||
                this.Command == Mnemonic.opSSI[MN] || this.Command == Mnemonic.opSV[MN] ||
                this.Command == Mnemonic.opTRUNC[MN] ||
                this.Command == Mnemonic.opUO[MN] || this.Command == Mnemonic.opUC[MN] ||
                this.Command == Mnemonic.opUNO[MN] || this.Command == Mnemonic.opUD[MN] ||
                this.Command == Mnemonic.opUW[MN] ||
                this.Command == Mnemonic.opXNO[MN] || this.Command == Mnemonic.opXO[MN] ||
                this.Command == Mnemonic.opXOD[MN] || this.Command == Mnemonic.opXOW[MN] ||
                this.Command == Mnemonic.opZR[MN] || this.Command == Mnemonic.opZV[MN])
                retVal = (SelectedStatusValues.Akku1 | SelectedStatusValues.Akku2 | SelectedStatusValues.AR1 |
                        SelectedStatusValues.AR2 | SelectedStatusValues.DB | SelectedStatusValues.STW) & mySel;
            else if (this.Command == "+I" || this.Command == "-I" || this.Command == "*I" || this.Command == "/I" || this.Command == "+D" || this.Command == "-D" || this.Command == "*D" || this.Command == "/D" || this.Command == "+R" || this.Command == "-R" || this.Command == "*R" || this.Command == "/R" || this.Command == "MOD")
                retVal = (SelectedStatusValues.Akku1 | SelectedStatusValues.Akku2 | SelectedStatusValues.STW) & mySel;
            else if (this.Command == Mnemonic.opPLU[MN] || this.Command == Mnemonic.opABS[MN] || this.Command == Mnemonic.opBTD[MN] ||
                this.Command == Mnemonic.opACOS[MN] || this.Command == Mnemonic.opASIN[MN] || this.Command == Mnemonic.opBTI[MN] ||
                this.Command == Mnemonic.opATAN[MN] || this.Command == Mnemonic.opTAN[MN] || this.Command == Mnemonic.opSIN[MN] ||
                this.Command == Mnemonic.opCOS[MN] || this.Command == Mnemonic.opSQR[MN] || this.Command == Mnemonic.opSQRT[MN] ||
                this.Command == Mnemonic.opDEC[MN] || this.Command == Mnemonic.opTAW[MN] || this.Command == Mnemonic.opTAD[MN] ||
                this.Command == Mnemonic.opDTB[MN] || this.Command == Mnemonic.opDTR[MN] || this.Command == Mnemonic.opSLD[MN] ||
                this.Command == Mnemonic.opSLW[MN] || this.Command == Mnemonic.opSRD[MN] || this.Command == Mnemonic.opSRW[MN])
                retVal = (SelectedStatusValues.Akku1) & mySel;
            else if (this.Command == Mnemonic.opTAR[MN])
                retVal = (SelectedStatusValues.AR1 | SelectedStatusValues.AR2) & mySel;
            else if (this.Command == Mnemonic.opLAR1[MN])
                retVal = (SelectedStatusValues.AR1) & mySel;
            else if (this.Command == Mnemonic.opLAR2[MN])
                retVal = (SelectedStatusValues.AR2) & mySel;
            else if ((this.Command == Mnemonic.opL[MN]) && isDBcall)
                retVal = (SelectedStatusValues.Akku1 | SelectedStatusValues.Akku2 | SelectedStatusValues.DB) & mySel;
            else if ((this.Command == Mnemonic.opT[MN]) && isDBcall)
                retVal = (SelectedStatusValues.DB) & mySel;
            else if (this.Command == Mnemonic.opTAK[MN] || this.Command == Mnemonic.opL[MN] || this.Command == Mnemonic.opTAR1[MN] || this.Command == Mnemonic.opTAR2[MN])
                retVal = (SelectedStatusValues.Akku1 | SelectedStatusValues.Akku2) & mySel;
            else if ((this.Command == Mnemonic.opU[MN] || this.Command == Mnemonic.opUN[MN] || this.Command == Mnemonic.opO[MN] || this.Command == Mnemonic.opON[MN] || this.Command == Mnemonic.opX[MN] || this.Command == Mnemonic.opXN[MN] || this.Command == Mnemonic.opFN[MN] || this.Command == Mnemonic.opFP[MN] || this.Command == Mnemonic.opZUW[MN] || this.Command == Mnemonic.opS[MN] || this.Command == Mnemonic.opR[MN] || this.Command == Mnemonic.opT[MN]) && isDBcall)
                retVal = (SelectedStatusValues.STW | SelectedStatusValues.DB) & mySel;
            else if (this.Command == Mnemonic.opU[MN] || this.Command == Mnemonic.opUN[MN] || this.Command == Mnemonic.opO[MN] ||
                    this.Command == Mnemonic.opON[MN] || this.Command == Mnemonic.opX[MN] || this.Command == Mnemonic.opXN[MN] ||
                    this.Command == Mnemonic.opFN[MN] || this.Command == Mnemonic.opFP[MN] || this.Command == Mnemonic.opZUW[MN] ||
                    this.Command == Mnemonic.opS[MN] || this.Command == Mnemonic.opR[MN] || this.Command == Mnemonic.opBE[MN] ||
                    this.Command == Mnemonic.opBEA[MN] || this.Command == Mnemonic.opBEB[MN] || this.Command == ")")
                retVal = (SelectedStatusValues.STW) & mySel;
            else if (this.Command == Mnemonic.opT[MN] || this.Command == Mnemonic.opBLD[MN] || this.Command == Mnemonic.opNOP[MN] ||
                     this.Command == Mnemonic.opSPA[MN])
                retVal = 0;
            else if (this.Command == Mnemonic.opAUF[MN] || this.Command == Mnemonic.opTDB[MN])
                retVal = (SelectedStatusValues.DB) & mySel;
            else if (this.Command == Mnemonic.opSPB[MN] || this.Command == Mnemonic.opSPBB[MN] ||
                     this.Command == Mnemonic.opSPBI[MN] || this.Command == Mnemonic.opSPBIN[MN] ||
                     this.Command == Mnemonic.opSPBN[MN] || this.Command == Mnemonic.opSPBNB[MN] ||
                     this.Command == Mnemonic.opSPL[MN] || this.Command == Mnemonic.opSPM[MN] ||
                     this.Command == Mnemonic.opSPMZ[MN] || this.Command == Mnemonic.opSPN[MN] ||
                     this.Command == Mnemonic.opSPO[MN] || this.Command == Mnemonic.opSPP[MN] ||
                     this.Command == Mnemonic.opSPPZ[MN] || this.Command == Mnemonic.opSPS[MN] ||
                     this.Command == Mnemonic.opSPU[MN] || this.Command == Mnemonic.opSPZ[MN] ||
                     this.Command == Mnemonic.opSET[MN] || this.Command == Mnemonic.opCLR[MN])
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
                return (MnemonicLanguage == MnemonicLanguage.English ? "Network " : "Netzwerk ") + Parameter + " : " + NetworkName
                       + ( string.IsNullOrEmpty(Comment) ? string.Empty : "\r\n\t Comment : " + Comment.Replace("\n", "\r\n\t           ") );

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
