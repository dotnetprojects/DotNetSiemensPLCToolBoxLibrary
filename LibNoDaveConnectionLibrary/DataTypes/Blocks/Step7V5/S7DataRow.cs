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
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public class S7DataRow : TiaAndSTep7DataBlockRow
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
       
        public S7DataRow(string name, S7DataRowType datatype, Block plcblock)
        {
            this.CurrentBlock = plcblock;
            this.Name = name;
            this.DataType = datatype;

            if (datatype == S7DataRowType.S5_KC || datatype == S7DataRowType.S5_C) StringSize = 2;
        }

        public override TiaAndSTep7DataBlockRow DeepCopy()
        {
            S7DataRow newRow = new S7DataRow(this.Name, this.DataType, this.PlcBlock);
            newRow.Parent = this.Parent;
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
                    TiaAndSTep7DataBlockRow copy = plcDataRow.DeepCopy();
                    copy.Parent = newRow;
                    newRow.Add(copy);
                }

            return newRow;   
        }
        
        public bool isRootBlock { get; set; }
        
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
        
        private ByteBitAddress _parentOldAddress;
        internal override ByteBitAddress FillBlockAddresses(ByteBitAddress startAddr)
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
        internal override void ClearBlockAddress()
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

       
        public override IDataRow Parent { get; set; }

        public override List<IDataRow> Children
        {
            get
            {
                if (_children == null)
                    return new List<IDataRow>();
                return _children;
            }
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

     

        //This List contains the orginal Structure, if the Block has a TimeStamp Conflict!
        public IList<S7DataRow> OrginalChildren { get; set; }

        public bool TimeStampConflict
        {
            get; internal set;
        }

        public string GetCallingString()
        {
            if (Parent == null || Parent.Parent == null)
            {
                return isRootBlock ? null : Name;
            }
            var pcs = ((S7DataRow) Parent).GetCallingString();
            if (isRootBlock) return pcs;
            return (pcs==null?"":pcs+".") + Name;
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
