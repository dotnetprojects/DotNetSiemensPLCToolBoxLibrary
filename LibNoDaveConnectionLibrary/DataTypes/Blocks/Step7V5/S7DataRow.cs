﻿/*
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
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using DotNetSiemensPLCToolBoxLibrary.General;


namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public class S7DataRow : DataBlockRow
    {
        public IEnumerable<String> Dependencies
        {
            get
            {
                if (this.DataType == S7DataRowType.FB || this.DataType == S7DataRowType.UDT)
                {
                    //var fld = (this.PlcBlock).ParentFolder as BlocksOfflineFolder;
                    //var blk = fld.GetBlock(this.DataTypeAsString);
                    return new List<String>() { this.DataTypeAsString };
                }
                else if (this.DataType == S7DataRowType.STRUCT)
                {
                    var retVal = new List<String>();
                    foreach (var s7DataRow in Children)
                    {
                        retVal.AddRange(((S7DataRow) s7DataRow).Dependencies);
                    }
                    return retVal;
                }
                return new List<String>();
            }
        }

        public static S7DataRow GetDataRowWithAddress(S7DataRow startRow, ByteBitAddress address, bool dontLookInTemp = false)
        {
            IList<IDataRow> col = startRow.Children;
            if (dontLookInTemp)
                col = startRow.Children.Where(itm => itm.Name != "TEMP").ToList();

            for (int n = 0; n < col.Count; n++)
            {
                var s7DataRow = col[n];
                if (n == col.Count - 1 || address < ((S7DataRow)col[n + 1]).BlockAddress)
                {
                    if (((S7DataRow)s7DataRow).BlockAddress == address && (s7DataRow.Children == null || s7DataRow.Children.Count == 0)) 
                        return ((S7DataRow)s7DataRow);
                    var tmp = GetDataRowWithAddress(((S7DataRow)s7DataRow), address);
                    if (tmp != null) 
                        return tmp;
                }
            }
            return null;
        }

        public static S7DataRow GetDataRowWithAddress(IEnumerable<S7DataRow> startRows, ByteBitAddress address)
        {
            foreach (var s7DataRow in startRows)
            {
                var row = GetDataRowWithAddress(s7DataRow, address);
                if (row != null) 
                    return row;
            }

            return null;
        }

        public S7DataRow(string name, S7DataRowType datatype, Block plcblock)
        {
            this.CurrentBlock = plcblock;
            this.Name = name;
            this.DataType = datatype;

            if (datatype == S7DataRowType.S5_KC || datatype == S7DataRowType.S5_C) StringSize = 2;
        }

        public S7DataRow DeepCopy()
        {
            S7DataRow newRow = new S7DataRow(this.Name, this.DataType, this.PlcBlock);
            newRow.ArrayStart = this.ArrayStart;
            newRow.ArrayStop = this.ArrayStop;
            newRow.IsArray = this.IsArray;
            newRow.WasFirstInArray = this.WasFirstInArray;
            newRow.WasArray = this.WasArray;
            newRow.WasNextHigherIndex = this.WasNextHigherIndex;
            newRow.Attributes = this.Attributes;
            newRow.Comment = this.Comment;
            newRow.DataTypeBlockNumber = this.DataTypeBlockNumber;
            newRow.ReadOnly = this.ReadOnly;
            newRow.StartValue = this.StartValue;
            newRow.StringSize = this.StringSize;
            newRow.TimeStampConflict = this.TimeStampConflict;
            newRow.isInOut = this.isInOut;
            newRow.isRootBlock = this.isRootBlock;
            newRow.Value = this.Value;
            

            if (Children!=null)
                foreach (S7DataRow plcDataRow in Children)
                {
                    S7DataRow copy = plcDataRow.DeepCopy();
                    copy.Parent = newRow;
                    newRow.Add(copy);
                }

            return newRow;

            /*
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;
            object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
             * */            
        }
        
        public bool isRootBlock { get; set; }
        public bool isInOut { get; set; }    

        private S7DataRowType _datatype;
        public override S7DataRowType DataType
        {
            get { return _datatype; }
            set
            {
                if (value == S7DataRowType.UNKNOWN)
                    return;
                _datatype = value;
                ClearBlockAddress();
                this.OnPropertyChanged("DataType");
            }
        }

        private int BaseBlockNumber
        {
            get
            {
                if (this.Parent != null)
                    return ((S7DataRow)Parent).BaseBlockNumber;
                return PlcBlock.BlockNumber;
            }
        }

        private PLCTag _plctag;
        public PLCTag PlcTag
        {
            get
            {
                if (_plctag != null)
                    return _plctag;               
                    _plctag = new PLCTag() { DataBlockNumber = BaseBlockNumber, TagDataSource = MemoryArea.Datablock, ByteAddress = BlockAddress.ByteAddress };

                switch (DataType)
                {

                    case S7DataRowType.BOOL:
                        _plctag.TagDataType = TagDataType.Bool;
                        _plctag.BitAddress = BlockAddress.BitAddress;
                        break;
                    case S7DataRowType.BYTE:
                        _plctag.TagDataType = TagDataType.Byte;
                        break;
                    case S7DataRowType.WORD:
                        _plctag.TagDataType = TagDataType.Word;                        
                        break;
                    case S7DataRowType.DWORD:
                        _plctag.TagDataType = TagDataType.Dword;
                        break;
                    case S7DataRowType.INT:
                        _plctag.TagDataType = TagDataType.Int;
                        break;
                    case S7DataRowType.DINT:
                        _plctag.TagDataType = TagDataType.Dint;
                        break;
                    case S7DataRowType.REAL:
                        _plctag.TagDataType = TagDataType.Float;
                        break;
                    case S7DataRowType.TIME:
                        _plctag.TagDataType = TagDataType.Time;
                        break;
                    case S7DataRowType.DATE_AND_TIME:
                        _plctag.TagDataType = TagDataType.DateTime;
                        break;
                    case S7DataRowType.CHAR:
                        _plctag.TagDataType = TagDataType.CharArray;
                        _plctag.ArraySize = this.GetArrayLines();
                        break;
                    case S7DataRowType.STRING:
                        _plctag.TagDataType = TagDataType.String;
                        _plctag.ArraySize = this.StringSize;
                        break;
                    default:
                        _plctag = null;
                        return null;
                }
                //_LibNoDaveValue.PropertyChanged += new PropertyChangedEventHandler(_LibNoDaveValue_PropertyChanged);
                OnPropertyChanged("LibNoDaveValue");
                return _plctag;
            }           
        }

        public string GetSymbolicAddress()
        {
            switch (DataType)
            {
                case S7DataRowType.BOOL:
                    return "X" + BlockAddress.ToString();
                case S7DataRowType.WORD:
                case S7DataRowType.INT:
                case S7DataRowType.S5TIME:
                case S7DataRowType.DATE:
                    return "W" + BlockAddress.ByteAddress.ToString();
                    break;
                case S7DataRowType.DWORD:
                case S7DataRowType.DINT:
                case S7DataRowType.REAL:
                case S7DataRowType.TIME:
                case S7DataRowType.TIME_OF_DAY:
                    return "D" + BlockAddress.ByteAddress.ToString();
                    break;
                case S7DataRowType.CHAR:
                case S7DataRowType.BYTE:
                    return "B" + BlockAddress.ByteAddress.ToString();
                    break;
                default:
                    return null;
            }
        }


        void _LibNoDaveValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("LibNoDaveValue");
        }

        public List<Step7Attribute> Attributes { get; set; }


        //Contains the Length of a Structure or UDT...
        //private int _structLength = 0;

        //This Returns the Length in Bytes
        public int ByteLength
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
                        if (this.Parent != null && ((S7DataRow) this.Parent).isInOut)
                            return 6;   //On InOut -> It's handeled as Pointer
                         int size =0;
                         if (Children != null)
                         {
                             if (Children.Count > 0)
                             {
                                 ByteBitAddress tmp = ((S7DataRow)Children[Children.Count - 1]).NextBlockAddress;
                                 if (tmp.BitAddress != 0)
                                     tmp.ByteAddress++;
                                 if (tmp.ByteAddress % 2 != 0)
                                     tmp.ByteAddress++;
                                 size = tmp.ByteAddress - ((S7DataRow)Children[0])._BlockAddress.ByteAddress;
                                 if (IsArray)
                                 {
                                     size *= this.GetArrayLines();
                                 }
                             }
                         }
                        return size;
                    case S7DataRowType.BOOL:
                    case S7DataRowType.BYTE:
                    case S7DataRowType.CHAR:
                        len = 1;
                        break;                    
                    case S7DataRowType.TIME_OF_DAY:
                    case S7DataRowType.DINT:
                    case S7DataRowType.DWORD:
                    case S7DataRowType.TIME:
                    case S7DataRowType.REAL:
                        len = 4;
                        break;
                    case S7DataRowType.POINTER:
                        len = 6;
                        break;
                    case S7DataRowType.DATE_AND_TIME:
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


        private ByteBitAddress _BlockAddress;
        public override ByteBitAddress BlockAddress
        {
            get
            {
                if (_BlockAddress == null)
                    FillBlockAddresses(null);
                if (this.PlcBlock is Step5.S5DataBlock && _BlockAddress != null)
                    return new ByteBitAddress(_BlockAddress.ByteAddress / 2, _BlockAddress.BitAddress);
                return new ByteBitAddress(_BlockAddress);
                
            }
        }

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

        private ByteBitAddress _NextBlockAddress;
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

        

        private ByteBitAddress _parentOldAddress;
        internal ByteBitAddress FillBlockAddresses(ByteBitAddress startAddr)
        {
            if (isRootBlock && this.Name == "TEMP")
            {
                _BlockAddress = new ByteBitAddress(0, 0);
                startAddr = new ByteBitAddress(0, 0);
            }

            ByteBitAddress akAddr = new ByteBitAddress(startAddr);
            if (Parent != null && startAddr == null)
                ((S7DataRow)Parent).FillBlockAddresses(null);
            else
            {
                //Create a function wich fills in the Block address for all Subitems...
                if (Children != null || _datatype != S7DataRowType.STRUCT)
                {                    
                    if (akAddr.BitAddress != 0)
                        akAddr.ByteAddress++;
                    if (akAddr.ByteAddress%2 != 0)
                        akAddr.ByteAddress++;

                    bool lastRowWasArrayOrStruct = false;

                    //int structlen = 0;
                    foreach (S7DataRow plcDataRow in Children)
                    {

                        if (akAddr.BitAddress != 0 && plcDataRow._datatype == S7DataRowType.BOOL && plcDataRow.WasArray && !plcDataRow.WasFirstInArray && plcDataRow.WasNextHigherIndex)
                        {
                            akAddr.BitAddress = 0;
                            akAddr.ByteAddress++;
                        }
                        else if (akAddr.BitAddress != 0 && (plcDataRow._datatype != S7DataRowType.BOOL || plcDataRow.IsArray || plcDataRow.WasFirstInArray || (lastRowWasArrayOrStruct && !plcDataRow.WasArray && !plcDataRow.WasFirstInArray)))
                        {
                            akAddr.BitAddress = 0;
                            akAddr.ByteAddress++;
                        }

                        
                        if (akAddr.ByteAddress % 2 != 0 && ((plcDataRow._datatype != S7DataRowType.BOOL && plcDataRow._datatype != S7DataRowType.BYTE && plcDataRow._datatype != S7DataRowType.CHAR) || plcDataRow.IsArray || plcDataRow.WasFirstInArray || (lastRowWasArrayOrStruct && !plcDataRow.WasArray && !plcDataRow.WasFirstInArray)))
                            if (!(this.PlcBlock is Step5.S5DataBlock))
                            {
                                akAddr.ByteAddress++;
                            }
                        
                        
                        if (plcDataRow.Children != null && plcDataRow.Children.Count > 0)
                        {
                            plcDataRow._BlockAddress = new ByteBitAddress(akAddr);

                            var useAddr = akAddr;
                            if (plcDataRow.Parent != null && ((S7DataRow) plcDataRow.Parent).isInOut)
                                useAddr = new ByteBitAddress(0, 0);
                            plcDataRow.FillBlockAddresses(useAddr);

                            //Struct or UDT are Handeled as Pointer in IN_OUT so only Increase about 6 Byte
                            //if (plcDataRow.Parent != null && ((S7DataRow) plcDataRow.Parent).isInOut)
                            akAddr.ByteAddress += plcDataRow.ByteLength;
                            //if (!plcDataRow.IsArray)
                            //    akAddr = plcDataRow.FillBlockAddresses(akAddr);
                            //else
                            //    akAddr = plcDataRow.FillBlockAddresses(akAddr);
                            if (akAddr.BitAddress != 0)
                            {
                                akAddr.BitAddress = 0;
                                akAddr.ByteAddress++;
                            }
                            if (akAddr.ByteAddress%2 != 0)
                                akAddr.ByteAddress++;

                            plcDataRow._NextBlockAddress = new ByteBitAddress(akAddr);
                        }
                        else
                        {
                            plcDataRow._BlockAddress = new ByteBitAddress(akAddr);
                            if (plcDataRow._datatype == S7DataRowType.BOOL && !plcDataRow.IsArray)
                                akAddr = Helper.GetNextBitAddress(akAddr);
                            else
                            {
                                akAddr.BitAddress = 0;
                                akAddr.ByteAddress += plcDataRow.ByteLength;
                            }
                            plcDataRow._NextBlockAddress = new ByteBitAddress(akAddr);
                        }

                        //structlen += plcDataRow.ByteLength;

                        lastRowWasArrayOrStruct = plcDataRow.WasArray;
                    }
                    //this. = structlen;
                }
            }
            return akAddr;
        }

        /// <summary>
        /// This function is used, when a Member is added or Removed, or the Type is changed so the the Addresses are recalculated!
        /// </summary>
        internal void ClearBlockAddress()
        {
            if (_BlockAddress != null || _parentOldAddress != null)
            {
                _BlockAddress = null;
                _parentOldAddress = null;
                //_structureLength = null;

                if (Children!=null)
                    foreach (S7DataRow plcDataRow in Children)
                    {
                        plcDataRow.ClearBlockAddress();
                    }
                if (Parent != null)
                    ((S7DataRow)Parent).ClearBlockAddress();
            }
        }

        public string ValueAsString
        {
            get
            {
                if (Value != null)
                {
                    return Helper.ValueToString(Value, DataType);
                }
                else
                {
                    return Helper.ValueToString(Helper.DefaultValueForType(DataType), DataType);
                }
                return null;
            }
        }

        public string StartValueAsString
        {
            get
            {
                if (this.IsArray)
                {
                    if (StartValue != null) return StartValue.ToString();
                    return null;
                }
                else if (StartValue != null)
                {
                    return Helper.ValueToString(StartValue, DataType);
                }
                else
                {
                    return Helper.ValueToString(Helper.DefaultValueForType(DataType), DataType);
                }
                return null;
            }
        }

        public object StartValue { get; set; }  //Only for FB and DB not for FC
        public object Value { get; set; } //Only used in DBs
        public bool ReadOnly { get; set; }

        

        //Array-element was the first at a higher index (bools start with zero bit address)
        internal bool WasNextHigherIndex { get; set; }
        //First element in a array
        internal bool WasFirstInArray { get; set; }
        //was a elemnt in a array
        internal bool WasArray { get; set; }

        public override IDataRow Parent { get; set; }

        public void Add(S7DataRow par)
        {
            if (ReadOnly) return;
            if (_children == null)
                _children = new List<IDataRow>();
            _children.Add(par);
            par.Parent = this;

            ClearBlockAddress();
        }

        public void AddRange(IEnumerable<S7DataRow> par)
        {
            if (ReadOnly) return;
            if (_children == null)
                _children = new List<IDataRow>();
            _children.AddRange(par);
            foreach (S7DataRow plcDataRow in par)
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

        public static List<S7DataRow> GetChildrowsAsList(S7DataRow akRow)
        {
            List<S7DataRow> retVal = new List<S7DataRow>();
            retVal.Add(akRow);
            if (akRow != null && akRow.Children != null && (akRow.DataType == S7DataRowType.STRUCT ||  akRow.DataType == S7DataRowType.UDT ||  akRow.DataType == S7DataRowType.FB))
                foreach (S7DataRow plcDataRow in akRow.Children)
                    retVal.AddRange(GetChildrowsAsList(plcDataRow));            
            return retVal;
        }

        public static List<PLCTag> GetLibnoDaveValues(List<S7DataRow> rowList)
        {
            List<PLCTag> retVal = new List<PLCTag>();
            foreach (S7DataRow plcDataRow in rowList)
            {
                if (plcDataRow.PlcTag != null)
                    retVal.Add(plcDataRow.PlcTag);
            }
            return retVal;
        }
        /*
        internal static List<PLCDataRow> _GetRowsAsList(PLCDataRow currRow)
        {
            List<PLCDataRow> retVal = new List<PLCDataRow>();
            retVal.Add(currRow);
            if (currRow.Children != null && currRow.DataType == PLCDataRowType.STRUCT)
                foreach (PLCDataRow plcDataRow in currRow.Children)
                    retVal.AddRange(_GetRowsAsList(plcDataRow));
            return retVal;
        }
        */

        internal List<S7DataRow> _GetExpandedChlidren(S7DataBlockExpandOptions myExpOpt)
        {
            S7DataRow retVal = (S7DataRow)this.DeepCopy();
            retVal._children = new List<IDataRow>();
           
            if (Children != null)
            {
                if (this.Parent==null || ((S7DataRow)this.Parent).isInOut==false || myExpOpt.ExpandSubChildInINOUT)
                    foreach (S7DataRow plcDataRow in this.Children)
                    {
                        List<S7DataRow> tmp = plcDataRow._GetExpandedChlidren(myExpOpt);
                        retVal.AddRange(tmp);
                    }
            }

            if (this.IsArray && (this.DataType!=S7DataRowType.CHAR || myExpOpt.ExpandCharArrays))
            {
                List<S7DataRow> arrAsList = new List<S7DataRow>();

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
                    

                    S7DataRow tmp = (S7DataRow)retVal.DeepCopy();
                    tmp.Name = tmp.Name + "[" + nm + "]";
                    tmp.WasFirstInArray = retVal.IsArray && i == 0;
                    tmp.WasArray = retVal.IsArray;
                    tmp.IsArray = false;
                    tmp.WasNextHigherIndex = frst; // arrAk[ArrayStart.Count - 1] == ArrayStart[ArrayStart.Count - 1];
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

            return new List<S7DataRow>() {retVal};
        }


        //This List contains the orginal Structure, if the Block has a TimeStamp Conflict!
        public IList<S7DataRow> OrginalChildren { get; set; }

        public bool TimeStampConflict
        {
            get; internal set;
        }

        public string GetCallingString()
        {
            if (Parent == null)
                return Name;
            else
                return ((S7DataRow)Parent).GetCallingString() + "." + Name;
        }

        public override string ToString()
        {
            return ToString("");
        }    

        public string ToString(string spacer)
        {
            string retVal = "";

            if (BlockAddress != null)
                retVal += BlockAddress.ToString().PadLeft(5, '0') + ": ";

            retVal += spacer + Name;

            if (Attributes!=null)
            {
                retVal += "{";
                foreach (Step7Attribute step7Attribute in Attributes)
                {
                    if (Attributes.IndexOf(step7Attribute) != 0) retVal += "; ";
                    retVal += step7Attribute.Name + "=" + "'" + step7Attribute.Value.Replace("$","$$").Replace("'","$'") + "'";                  
                }
                retVal += "}";
            }                                
                
            retVal+= ": ";

            /*
            if (IsArray)
            {
                retVal += "ARRAY [";
                for (int n = 0; n < ArrayStart.Count; n++)
                {
                    retVal += ArrayStart[n].ToString() + ".." + ArrayStop[n].ToString();
                    if (n < ArrayStart.Count - 1)
                        retVal += ",";
                }
                retVal += "] OF ";
            }
            retVal += DataType.ToString();
            if (DataType == PLCDataRowType.FB || DataType == PLCDataRowType.UDT || DataType == PLCDataRowType.SFB)
                retVal += DataTypeBlockNumber.ToString();
            if (DataType == PLCDataRowType.STRING)
                retVal += "[" + StringSize.ToString() + "]";
            */
            retVal += DataTypeAsString;

            if (StartValue != null)
                if (DataType == S7DataRowType.STRING)
                    retVal += " := '" + StartValue + "'";
                else
                    retVal += " := " + StartValue;
            if (!string.IsNullOrEmpty(Comment))
                retVal += " //" + Comment;
            retVal += Environment.NewLine;

            if (Children != null)
                foreach (S7DataRow plcDataRow in Children)
                {
                    retVal += plcDataRow.ToString("\t" + spacer);

                }
            return retVal;
        }
    }
}
