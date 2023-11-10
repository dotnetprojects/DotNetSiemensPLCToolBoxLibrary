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
using System.Globalization;
using System.Linq;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using DotNetSiemensPLCToolBoxLibrary.General;
using System.Text.RegularExpressions;


namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public abstract class TiaAndSTep7DataBlockRow : DataBlockRow
    {
        public abstract TiaAndSTep7DataBlockRow DeepCopy();

        //Array-element was the first at a higher index (bools start with zero bit address)
        internal bool WasNextHigherIndex { get; set; }
        //First element in a array
        internal bool WasFirstInArray { get; set; }
        //was a elemnt in a array
        internal bool WasArray { get; set; }

        public string BlockAddressInDbFormat
        {
            get
            {
                if (this.DataType == S7DataRowType.BOOL)
                    return "DBX" + BlockAddress.ToString();
                switch (this.ByteLength)
                {
                    case 1:
                        return "DBB" + BlockAddress.ByteAddress.ToString();
                    case 2:
                        return "DBW" + BlockAddress.ByteAddress.ToString();
                    case 4:
                        return "DBD" + BlockAddress.ByteAddress.ToString();
                }

                return "";
            }
        }

        public string FullBlockAddressInDbFormat
        {
            get
            {
                if (this.DataType == S7DataRowType.BOOL)
                    return "DB" + BaseBlockNumber + ".DBX" + BlockAddress.ToString();
                switch (this.ByteLength)
                {
                    case 1:
                        return "DB" + BaseBlockNumber + ".DBB" + BlockAddress.ByteAddress.ToString();
                    case 2:
                        return "DB" + BaseBlockNumber + ".DBW" + BlockAddress.ByteAddress.ToString();
                    case 4:
                        return "DB" + BaseBlockNumber + ".DBD" + BlockAddress.ByteAddress.ToString();
                }

                return "P#DB" + BaseBlockNumber + "DBX" + BlockAddress.ByteAddress + ".0 BYTE " + this.ByteLength;
            }
        }

        internal List<string> GetStartValuesArray(object startValues)
        {
            if (startValues == null)
            {
                return new List<string>();
            }

            var strStartValues = startValues.ToString();

            // matches on shortened pattern like "2(3),2(3)"
            var shortPattern = @"(\d+)\(([^)]+)\)";
            var matches = Regex.Matches(strStartValues, shortPattern);

            if (matches.Count == 0)
            {
                // assumes "1,2,3,4,5" format
                return strStartValues.Split(',').ToList();
            }

            var ret = new List<string>();

            foreach (Match match in matches)
            {
                var count = int.Parse(match.Groups[1].Value);
                var value = match.Groups[2].Value;

                for (int i = 0; i < count; i++)
                {
                    ret.Add(value);
                }
            }

            return ret;
        }

        internal List<TiaAndSTep7DataBlockRow> _GetExpandedChlidren(S7DataBlockExpandOptions myExpOpt)
        {
            TiaAndSTep7DataBlockRow retVal = (TiaAndSTep7DataBlockRow)this.DeepCopy();
            retVal._children = new List<IDataRow>();

            if (Children != null)
            {
                if (this.Parent == null || ((TiaAndSTep7DataBlockRow)this.Parent).isInOut == false || myExpOpt.ExpandSubChildInINOUT)
                    foreach (TiaAndSTep7DataBlockRow plcDataRow in this.Children)
                    {
                        List<TiaAndSTep7DataBlockRow> tmp = plcDataRow._GetExpandedChlidren(myExpOpt);
                        retVal.AddRange(tmp);
                    }
            }

            if (this.IsArray && (this.DataType != S7DataRowType.CHAR || myExpOpt.ExpandCharArrays))
            {
                List<TiaAndSTep7DataBlockRow> arrAsList = new List<TiaAndSTep7DataBlockRow>();

                var startValues = GetStartValuesArray(StartValue);

                var lastCnt = (ArrayStop.Last() - ArrayStart.Last()) + 1;

                int[] arrAk = ArrayStart.ToArray();
                for (int i = 0; i < this.GetArrayLines(); i++)
                {
                    string nm = "";
                    for (int n = 0; n < arrAk.Length; n++)
                    {
                        if (nm != "") nm += ", ";
                        nm += arrAk[n];
                    }

                    var frst = (i % lastCnt) == 0;  //Erstes Elment des letzten Index eines Arrays 


                    TiaAndSTep7DataBlockRow tmp = (TiaAndSTep7DataBlockRow)retVal.DeepCopy();
                    tmp.Name = tmp.Name + "[" + nm + "]";
                    tmp.WasFirstInArray = retVal.IsArray && i == 0;
                    tmp.WasArray = retVal.IsArray;
                    tmp.IsArray = false;
                    tmp.WasNextHigherIndex = frst; // arrAk[ArrayStart.Count - 1] == ArrayStart[ArrayStart.Count - 1];
                    if (i < startValues.Count)
                    {
                        tmp.StartValue = startValues[i];
                    }
                    else
                    {
                        tmp.StartValue = Helper.DefaultValueForType(DataType);
                    }
                    arrAsList.Add(tmp);

                    for (int n = arrAk.Length - 1; n >= 0; n--)
                    {
                        arrAk[n]++;
                        if (arrAk[n] > ArrayStop[n])
                        {
                            arrAk[n] = ArrayStart[n];
                        }
                        else
                            break;
                    }
                }
                return arrAsList;
            }

            return new List<TiaAndSTep7DataBlockRow>() { retVal };
        }


        public static DataBlockRow GetDataRowWithAddress(DataBlockRow startRow, ByteBitAddress address, bool dontLookInTemp = false)
        {
            IList<IDataRow> col = startRow.Children;
            if (dontLookInTemp)
                col = startRow.Children.Where(itm => itm.Name != "TEMP").ToList();

            for (int n = 0; n < col.Count; n++)
            {
                var s7DataRow = col[n];
                if (n == col.Count - 1 || address < ((DataBlockRow)col[n + 1]).BlockAddress)
                {
                    if (((DataBlockRow)s7DataRow).BlockAddress == address && (s7DataRow.Children == null || s7DataRow.Children.Count == 0))
                        return ((DataBlockRow)s7DataRow);
                    //fix for finding the absoluteaddress of a string
                    var stringDataRow = (TiaAndSTep7DataBlockRow)s7DataRow;
                    if (stringDataRow.DataType == S7DataRowType.STRING)
                    {
                        int firstByte = stringDataRow.BlockAddress.ByteAddress;
                        int lastByte = firstByte + stringDataRow.ByteLength;
                        //If is a string the calling logic has determine which character is bein accessed
                        if (address.ByteAddress >= (firstByte) && address.ByteAddress <= lastByte)
                            return stringDataRow;
                    }
                    var tmp = GetDataRowWithAddress(((DataBlockRow)s7DataRow), address);
                    if (tmp != null)
                        return tmp;
                }
            }
            return null;
        }

        public static DataBlockRow GetDataRowWithAddress(IEnumerable<DataBlockRow> startRows, ByteBitAddress address)
        {
            foreach (var s7DataRow in startRows)
            {
                var row = GetDataRowWithAddress(s7DataRow, address);
                if (row != null)
                    return row;
            }

            return null;
        }

        public object StartValue { get; set; }  //Only for FB and DB not for FC
        public object Value { get; set; } //Only used in DBs
        public bool ReadOnly { get; set; }

        public void Add(IDataRow par)
        {
            if (ReadOnly) return;
            if (_children == null)
                _children = new List<IDataRow>();
            _children.Add(par);
            par.Parent = this;

            ClearBlockAddress();
        }

        public void AddRange(IEnumerable<IDataRow> par)
        {
            if (ReadOnly) return;
            if (_children == null)
                _children = new List<IDataRow>();
            _children.AddRange(par);
            foreach (IDataRow plcDataRow in par)
            {
                plcDataRow.Parent = this;
            }
            ClearBlockAddress();
        }

        public void Remove(S7DataRow par)
        {
            if (this._children != null)
            {
                if (ReadOnly) return;
                _children.Remove(par);
                if (_children.Count == 0)
                    this._children = null;
            }
            ClearBlockAddress();
        }

        public void Clear()
        {
            if (this._children != null)
            {
                this._children.Clear();
                this._children = null;
            }
        }


        internal List<IDataRow> _children;

        public override List<IDataRow> Children
        {
            get
            {
                if (_children == null)
                    return new List<IDataRow>();
                return _children;
            }
        }

        internal virtual void ClearBlockAddress()
        {

        }

        public bool isInOut { get; set; }

        protected ByteBitAddress _NextBlockAddress;
        public ByteBitAddress NextBlockAddress
        {
            get
            {
                if (_NextBlockAddress != null)
                    return new ByteBitAddress(_NextBlockAddress);
                else
                {
                    FillBlockAddresses(null);
                    return new ByteBitAddress(_NextBlockAddress);
                }
            }
        }

        internal abstract ByteBitAddress FillBlockAddresses(ByteBitAddress startAddr);
        //This Returns the Length in Bytes
        
        public virtual int ByteLength
        {
            get
            {
                int len = 0;
                switch (_datatype)
                {
                    case S7DataRowType.ANY:
                        len = 10;
                        break;
                    case S7DataRowType.UNKNOWN:
                        len = 0;
                        break;
                    case S7DataRowType.FB:
                    case S7DataRowType.SFB:
                    case S7DataRowType.STRUCT:
                    case S7DataRowType.UDT:
                        if (this.Parent != null && ((TiaAndSTep7DataBlockRow)this.Parent).isInOut)
                            return 6;   //On InOut -> It's handeled as Pointer
                        int size = 0;
                        if (Children != null)
                        {
                            if (Children.Count > 0)
                            {
                                ByteBitAddress tmp = ((TiaAndSTep7DataBlockRow)Children[Children.Count - 1]).NextBlockAddress;
                                if (tmp.BitAddress != 0)
                                    tmp.ByteAddress++;
                                if (tmp.ByteAddress % 2 != 0)
                                    tmp.ByteAddress++;
                                size = tmp.ByteAddress - ((TiaAndSTep7DataBlockRow)Children[0])._BlockAddress.ByteAddress;
                                if (IsArray)
                                {
                                    size *= this.GetArrayLines();
                                }
                            }
                        }
                        return size;
                    case S7DataRowType.BOOL:
                    case S7DataRowType.BYTE:
                    case S7DataRowType.SINT:
                    case S7DataRowType.USINT:
                    case S7DataRowType.CHAR:
                        len = 1;
                        break;
                    case S7DataRowType.TIME_OF_DAY:
                    case S7DataRowType.DINT:
                    case S7DataRowType.DWORD:
                    case S7DataRowType.TIME:
                    case S7DataRowType.REAL:
                    case S7DataRowType.UDINT:
                        len = 4;
                        break;
                    case S7DataRowType.POINTER:
                        len = 6;
                        break;
                    case S7DataRowType.DATE_AND_TIME:
                    case S7DataRowType.LREAL:
                    case S7DataRowType.ULINT:
                    case S7DataRowType.LINT:
                        len = 8;
                        break;
                    case S7DataRowType.S5TIME:
                    case S7DataRowType.WORD:
                    case S7DataRowType.INT:
                    case S7DataRowType.BLOCK_DB:
                    case S7DataRowType.BLOCK_FB:
                    case S7DataRowType.BLOCK_FC:
                    case S7DataRowType.BLOCK_SDB:
                    case S7DataRowType.COUNTER:
                    case S7DataRowType.TIMER:
                    case S7DataRowType.DATE:
                    case S7DataRowType.UINT:
                        len = 2;
                        break;
                    case S7DataRowType.S5_KH:
                    case S7DataRowType.S5_KF:
                    case S7DataRowType.S5_KM:
                    case S7DataRowType.S5_A:
                    case S7DataRowType.S5_KT:
                    case S7DataRowType.S5_KZ:
                    case S7DataRowType.S5_KY:
                        len = 2;
                        break;
                    case S7DataRowType.S5_KG:
                        len = 4;
                        break;
                    case S7DataRowType.S5_KC:
                    case S7DataRowType.S5_C:
                        len = StringSize;
                        //len = 2;
                        break;
                    case S7DataRowType.STRING:
                        len = StringSize + 2;
                        break;
                }

                if (IsArray)
                {
                    if (_datatype == S7DataRowType.BOOL)
                    {
                        int bytesize = 0;
                        for (int n = ArrayStart.Count - 1; n >= 0; n--)
                        {
                            var start = ArrayStart[n];
                            var end = ArrayStop[n];

                            if (n == ArrayStart.Count - 1)
                            {
                                bytesize = ((end - start) + 8) / 8; //normal ((end - start) + 1); but that the division with 8 rounds down +7 to rund up!                                
                            }
                            else
                            {
                                bytesize = bytesize * ((end - start) + 1);
                            }
                        }

                        if (bytesize % 2 > 0) bytesize++;

                        return bytesize;

                        /*
                        int retInt = 0;
                        retInt =(len * this.GetArrayLines()) / 8;

                        if (this.GetArrayLines() < 4)
                            retInt++;

                        if ((len * this.GetArrayLines()) % 8 != 0) retInt++;
                        if ((len * this.GetArrayLines()) % 2 != 0) retInt++;
                        return retInt;*/
                    }
                    else
                    {
                        int retInt = (len * GetArrayLines());
                        return retInt + retInt % 2;
                    }
                }
                return len;
            }
        }
    }
}
