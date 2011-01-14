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
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    
    public class PLCDataRow : INotifyPropertyChanged
    {
        public PLCDataRow(string name, PLCDataRowType datatype, Block plcblock)
        {
            this.PlcBlock = plcblock;
            this.Name = name;
            this.DataType = datatype;
        }

        public PLCDataRow DeepCopy()
        {
            PLCDataRow newRow = new PLCDataRow(this.Name, this.DataType, this.PlcBlock);
            newRow.ArrayStart = this.ArrayStart;
            newRow.ArrayStop = this.ArrayStop;
            newRow.IsArray = this.IsArray;
            newRow.Attributes = this.Attributes;
            newRow.Comment = this.Comment;
            newRow.DataTypeBlockNumber = this.DataTypeBlockNumber;
            newRow.ReadOnly = this.ReadOnly;
            newRow.StartValue = this.StartValue;
            newRow.StringSize = this.StringSize;
            newRow.TimeStampConflict = this.TimeStampConflict;
            

            if (Children!=null)
                foreach (PLCDataRow plcDataRow in Children)
                {
                    PLCDataRow copy = plcDataRow.DeepCopy();
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
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public string Name { get; set; }

        public bool isRootBlock { get; set; }

        Block PlcBlock { get; set; }

        private PLCDataRowType _datatype;
        public PLCDataRowType DataType
        {
            get { return _datatype; }
            set
            {
                if (value == PLCDataRowType.UNKNOWN)
                    return;
                _datatype = value;
                ClearBlockAddress();
                NotifyPropertyChanged("DataType");
            }
        }

        private LibNoDaveValue _LibNoDaveValue;
        public LibNoDaveValue LibNoDaveValue
        {
            get
            {
                if (_LibNoDaveValue != null)
                    return _LibNoDaveValue;               

                switch (DataType)
                {
                    case PLCDataRowType.BOOL:
                        _LibNoDaveValue = new LibNoDaveValue() { DatablockNumber = PlcBlock.BlockNumber, LibNoDaveDataSource = TagDataSource.Datablock, ByteAddress = BlockAddress.ByteAddress };
                        _LibNoDaveValue.LibNoDaveDataType = TagDataType.Bool;
                        _LibNoDaveValue.BitAddress = BlockAddress.BitAddress;
                        break;
                    case PLCDataRowType.WORD:
                        _LibNoDaveValue = new LibNoDaveValue() { DatablockNumber = PlcBlock.BlockNumber, LibNoDaveDataSource = TagDataSource.Datablock, ByteAddress = BlockAddress.ByteAddress };
                        _LibNoDaveValue.LibNoDaveDataType = TagDataType.Word;                        
                        break;
                    case PLCDataRowType.DWORD:
                        _LibNoDaveValue = new LibNoDaveValue() { DatablockNumber = PlcBlock.BlockNumber, LibNoDaveDataSource = TagDataSource.Datablock, ByteAddress = BlockAddress.ByteAddress };
                        _LibNoDaveValue.LibNoDaveDataType = TagDataType.Dword;
                        break;
                    case PLCDataRowType.INT:
                        _LibNoDaveValue = new LibNoDaveValue() { DatablockNumber = PlcBlock.BlockNumber, LibNoDaveDataSource = TagDataSource.Datablock, ByteAddress = BlockAddress.ByteAddress };
                        _LibNoDaveValue.LibNoDaveDataType = TagDataType.Int;
                        break;
                    case PLCDataRowType.DINT:
                        _LibNoDaveValue = new LibNoDaveValue() { DatablockNumber = PlcBlock.BlockNumber, LibNoDaveDataSource = TagDataSource.Datablock, ByteAddress = BlockAddress.ByteAddress };
                        _LibNoDaveValue.LibNoDaveDataType = TagDataType.Dint;
                        break;
                    case PLCDataRowType.REAL:
                        _LibNoDaveValue = new LibNoDaveValue() { DatablockNumber = PlcBlock.BlockNumber, LibNoDaveDataSource = TagDataSource.Datablock, ByteAddress = BlockAddress.ByteAddress };
                        _LibNoDaveValue.LibNoDaveDataType = TagDataType.Float;
                        break;
                    case PLCDataRowType.DATE_AND_TIME:
                        _LibNoDaveValue = new LibNoDaveValue() { DatablockNumber = PlcBlock.BlockNumber, LibNoDaveDataSource = TagDataSource.Datablock, ByteAddress = BlockAddress.ByteAddress };
                        _LibNoDaveValue.LibNoDaveDataType = TagDataType.DateTime;
                        break;
                    case PLCDataRowType.CHAR:
                        _LibNoDaveValue = new LibNoDaveValue() { DatablockNumber = PlcBlock.BlockNumber, LibNoDaveDataSource = TagDataSource.Datablock, ByteAddress = BlockAddress.ByteAddress };
                        _LibNoDaveValue.LibNoDaveDataType = TagDataType.CharArray;
                        _LibNoDaveValue.ArraySize = this.GetArrayLines();
                        break;
                    case PLCDataRowType.STRING:
                        _LibNoDaveValue = new LibNoDaveValue() { DatablockNumber = PlcBlock.BlockNumber, LibNoDaveDataSource = TagDataSource.Datablock, ByteAddress = BlockAddress.ByteAddress };
                        _LibNoDaveValue.LibNoDaveDataType = TagDataType.String;
                        _LibNoDaveValue.ArraySize = this.StringSize;
                        break;
                    default:
                        return null;
                }
                //_LibNoDaveValue.PropertyChanged += new PropertyChangedEventHandler(_LibNoDaveValue_PropertyChanged);
                NotifyPropertyChanged("LibNoDaveValue");
                return _LibNoDaveValue;
            }           
        }

        public string GetSymbolicAddress()
        {
            switch (DataType)
            {
                case PLCDataRowType.BOOL:
                    return "X" + BlockAddress.ToString();
                case PLCDataRowType.WORD:
                case PLCDataRowType.INT:
                case PLCDataRowType.S5TIME:
                case PLCDataRowType.DATE:
                    return "W" + BlockAddress.ByteAddress.ToString();
                    break;
                case PLCDataRowType.DWORD:
                case PLCDataRowType.DINT:
                case PLCDataRowType.REAL:
                case PLCDataRowType.TIME:
                case PLCDataRowType.TIME_OF_DAY:
                    return "D" + BlockAddress.ByteAddress.ToString();
                    break;
                case PLCDataRowType.CHAR:
                case PLCDataRowType.BYTE:
                    return "B" + BlockAddress.ByteAddress.ToString();
                    break;
                default:
                    return null;
            }
        }


        void _LibNoDaveValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("LibNoDaveValue");
        }

        public int DataTypeBlockNumber { get; set; } //When the Type is SFB, FB or UDT, this contains the Number!
       
        public string StructuredName
        {
            get
            {
                if (Parent != null)
                {
                    if (string.IsNullOrEmpty(Parent.StructuredName))
                        return Name;
                    return Parent.StructuredName + "." + Name;
                }
                return "";
            }
            
        }

        public List<Step7Attribute> Attributes { get; set; }

        //This Returns the Length in Bytes
        public int ByteLength
        {
            get
            {
                int len = 0;
                switch (_datatype)
                {
                    case PLCDataRowType.ANY:
                        len = 10;
                        break;                  
                    case PLCDataRowType.UNKNOWN:
                        len = 0;
                        break;
                    case PLCDataRowType.FB:
                    case PLCDataRowType.SFB:   
                    case PLCDataRowType.STRUCT:
                    case PLCDataRowType.UDT:
                         int size =0;
                         if (Children != null)
                         {
                             if (Children.Count > 0)
                             {
                                 ByteBitAddress tmp = Children[Children.Count - 1].NextBlockAddress;
                                 if (tmp.BitAddress != 0)
                                     tmp.ByteAddress++;
                                 if (tmp.ByteAddress % 2 != 0)
                                     tmp.ByteAddress++;
                                 size = tmp.ByteAddress - Children[0]._BlockAddress.ByteAddress;
                                 if (IsArray)
                                 {
                                     size *= this.GetArrayLines();
                                 }
                             }
                         }
                        return size;
                    case PLCDataRowType.BOOL:
                    case PLCDataRowType.BYTE:
                    case PLCDataRowType.CHAR:
                        len = 1;
                        break;                    
                    case PLCDataRowType.TIME_OF_DAY:
                    case PLCDataRowType.DINT:
                    case PLCDataRowType.DWORD:
                    case PLCDataRowType.TIME:
                    case PLCDataRowType.REAL:
                        len = 4;
                        break;
                    case PLCDataRowType.POINTER:
                        len = 6;
                        break;
                    case PLCDataRowType.DATE_AND_TIME:
                        len = 8;
                        break;
                    case PLCDataRowType.S5TIME:
                    case PLCDataRowType.WORD:
                    case PLCDataRowType.INT:
                    case PLCDataRowType.BLOCK_DB:
                    case PLCDataRowType.BLOCK_FB:
                    case PLCDataRowType.BLOCK_FC:
                    case PLCDataRowType.BLOCK_SDB:
                    case PLCDataRowType.COUNTER:
                    case PLCDataRowType.TIMER:
                    case PLCDataRowType.DATE:         
                        len = 2;
                        break;
                    case PLCDataRowType.S5_KH:
                    case PLCDataRowType.S5_KF:
                    case PLCDataRowType.S5_KM:
                    case PLCDataRowType.S5_A:
                    case PLCDataRowType.S5_KG:
                    case PLCDataRowType.S5_KT:
                    case PLCDataRowType.S5_KZ:
                    case PLCDataRowType.S5_KY:
                        len = 2;
                        break;
                    case PLCDataRowType.S5_KC:
                    case PLCDataRowType.S5_C:
                        len = 2;
                        if (StringSize > 0)
                            len = StringSize;
                        break;
                    case PLCDataRowType.STRING:
                        len = StringSize + 2;
                        break;
                }

                if (IsArray)
                {                  
                    if (_datatype == PLCDataRowType.BOOL)
                    {
                        int retInt = 0;
                        retInt =(len * this.GetArrayLines()) / 8;
                        if ((len * this.GetArrayLines()) % 8 != 0) retInt++;
                        if ((len * this.GetArrayLines()) % 2 != 0) retInt++;
                        return retInt;
                    }
                    else
                    {
                        int retInt = 0;
                        retInt =(len * GetArrayLines());                        
                        if (retInt % 2 !=0) retInt++;
                        return retInt;
                    }
                }
                return len;
            }
        }


        private ByteBitAddress _BlockAddress;
        public ByteBitAddress BlockAddress
        {
            get
            {
                if (_BlockAddress != null)
                    return new ByteBitAddress(_BlockAddress);
                else
                {
                    FillBlockAddresses(null);
                    return new ByteBitAddress(_BlockAddress);
                }
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
                Parent.FillBlockAddresses(null);
            else
            {
                //Create a function wich fills in the Block address for all Subitems...
                if (Children != null || _datatype != PLCDataRowType.STRUCT)
                {                    
                    if (akAddr.BitAddress != 0)
                        akAddr.ByteAddress++;
                    if (akAddr.ByteAddress%2 != 0)
                        akAddr.ByteAddress++;

                    foreach (PLCDataRow plcDataRow in Children)
                    {
                        if (akAddr.BitAddress != 0 && plcDataRow._datatype != PLCDataRowType.BOOL)
                        {
                            akAddr.BitAddress = 0;
                            akAddr.ByteAddress++;
                        }
                        if (akAddr.ByteAddress%2 != 0 && plcDataRow._datatype != PLCDataRowType.BOOL && plcDataRow._datatype != PLCDataRowType.BYTE && plcDataRow._datatype != PLCDataRowType.CHAR)
                            akAddr.ByteAddress++;
                        if (plcDataRow.Children != null && plcDataRow.Children.Count > 0)
                        {
                            plcDataRow._BlockAddress = new ByteBitAddress(akAddr);

                            plcDataRow.FillBlockAddresses(akAddr);
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
                            if (plcDataRow._datatype == PLCDataRowType.BOOL && !plcDataRow.IsArray)
                                akAddr = Helper.GetNextBitAddress(akAddr);
                            else
                            {
                                akAddr.BitAddress = 0;
                                akAddr.ByteAddress += plcDataRow.ByteLength;
                            }
                            plcDataRow._NextBlockAddress = new ByteBitAddress(akAddr);
                        }
                    }
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

                if (Children!=null)
                    foreach (PLCDataRow plcDataRow in Children)
                    {
                        plcDataRow.ClearBlockAddress();
                    }
                Parent.ClearBlockAddress();
            }
        }

        public string Comment { get; set; }
        public int StringSize { get; set; } //Only Relevant for String                  
        public object StartValue { get; set; }  //Only for FB and DB not for FC
        public object Value { get; set; } //Only used in DBs
        public bool ReadOnly { get; set; }

        public bool IsArray { get; set; }

        public List<int> ArrayStart { get; set; }
        public List<int> ArrayStop { get; set; }
        public int GetArrayLines()
        {
            int arrcnt = 1;
            if (ArrayStart!=null)
                for (int n = 0; n < ArrayStart.Count; n++)
                {
                    arrcnt *= ArrayStop[n] - ArrayStart[n] + 1;
                }
            return arrcnt;
        }

        public PLCDataRow Parent { get; set; }

        public void Add(PLCDataRow par)
        {
            if (ReadOnly) return;
            if (_children == null)
                _children = new List<PLCDataRow>();
            _children.Add(par);
            par.Parent = this;

            ClearBlockAddress();
        }

        public void AddRange(IEnumerable<PLCDataRow> par)
        {
            if (ReadOnly) return;
            if (_children == null)
                _children = new List<PLCDataRow>();
            _children.AddRange(par);
            foreach (PLCDataRow plcDataRow in par)
            {
                plcDataRow.Parent = this;
            }
            ClearBlockAddress();
        }

        public void Remove(PLCDataRow par)
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


        internal List<PLCDataRow> _children;

        public List<PLCDataRow> Children
        {
            get
            {
                if (_children == null)
                    return new List<PLCDataRow>();
                return _children;
            }
        }

        public static List<PLCDataRow> GetChildrowsAsList(PLCDataRow akRow)
        {
            List<PLCDataRow> retVal = new List<PLCDataRow>();
            retVal.Add(akRow);
            if (akRow != null && akRow.Children != null && (akRow.DataType == PLCDataRowType.STRUCT ||  akRow.DataType == PLCDataRowType.UDT ||  akRow.DataType == PLCDataRowType.FB))
                foreach (PLCDataRow plcDataRow in akRow.Children)
                    retVal.AddRange(GetChildrowsAsList(plcDataRow));            
            return retVal;
        }

        public static List<LibNoDaveValue> GetLibnoDaveValues(List<PLCDataRow> rowList)
        {
            List<LibNoDaveValue> retVal = new List<LibNoDaveValue>();
            foreach (PLCDataRow plcDataRow in rowList)
            {
                if (plcDataRow.LibNoDaveValue != null)
                    retVal.Add(plcDataRow.LibNoDaveValue);
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

        internal List<PLCDataRow> _GetExpandedChlidren(PLCDataBlockExpandOptions myExpOpt)
        {
            PLCDataRow retVal = (PLCDataRow)this.DeepCopy();
            retVal._children = new List<PLCDataRow>();

            

            if (Children != null)
            {
                foreach (PLCDataRow plcDataRow in this.Children)
                {
                    List<PLCDataRow> tmp = plcDataRow._GetExpandedChlidren(myExpOpt);
                    retVal.AddRange(tmp);                   
                }                        
            }

            if (this.IsArray && (this.DataType!=PLCDataRowType.CHAR || myExpOpt.ExpandCharArrays))
            {
                List<PLCDataRow> arrAsList = new List<PLCDataRow>();

                int[] arrAk = ArrayStart.ToArray();
                for (int i = 0; i < this.GetArrayLines(); i++)
                {
                    string nm = "";
                    for (int n = 0; n < arrAk.Length; n++)
                    {
                        if (nm != "") nm += ", ";
                        nm += arrAk[n];                        
                    }

                    PLCDataRow tmp = (PLCDataRow)retVal.DeepCopy();
                    tmp.Name = tmp.Name + "[" + nm + "]";
                    tmp.IsArray = false;
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

            return new List<PLCDataRow>() {retVal};
        }


        //This List contains the orginal Structure, if the Block has a TimeStamp Conflict!
        public IList<PLCDataRow> OrginalChildren { get; set; }

        public bool TimeStampConflict
        {
            get; internal set;
        }

        public string GetCallingString()
        {
            if (Parent == null)
                return Name;
            else
                return Parent.GetCallingString() + "." + Name;
        }

        public override string ToString()
        {
            return ToString("");
        }

        public string DataTypeAsString
        {
            get
            {
                string retVal = "";
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

                return retVal;
            }
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
                if (DataType == PLCDataRowType.STRING)
                    retVal += " := '" + StartValue + "'";
                else
                    retVal += " := " + StartValue;
            if (!string.IsNullOrEmpty(Comment))
                retVal += " //" + Comment;
            retVal += Environment.NewLine;

            if (Children != null)
                foreach (PLCDataRow plcDataRow in Children)
                {
                    retVal += plcDataRow.ToString("\t" + spacer);

                }
            return retVal;
        }
    }
}
