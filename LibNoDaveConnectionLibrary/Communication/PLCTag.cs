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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;


namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
#if !IPHONE
    [System.ComponentModel.Editor(typeof(PLCTagUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
	[Serializable]
    public class PLCTag: INotifyPropertyChanged
    {
        public bool RaiseValueChangedEvenWhenNoChangeHappened { get; set; }

        private string _valueName;
        /// <summary>
        /// This is a Property wich addresses the values you've read with a Name
        /// </summary>        
        public String ValueName
        {
            get { return _valueName; }
            set { _valueName = value;
            NotifyPropertyChanged("ValueName");
            }
        }

        private int _byteAddress;
        public int ByteAddress
        {
            get { return _byteAddress; }
            set { _byteAddress = value;
            NotifyPropertyChanged("ByteAddress"); NotifyPropertyChanged("S7FormatAddress"); 
            }
        }

        private int _bitAddress;
        public int BitAddress
        {
            get { return _bitAddress; }
            set { _bitAddress = value;
                _bitAddress = _bitAddress > 7 ? 7 : _bitAddress;
                _bitAddress = _bitAddress < 0 ? 0 : _bitAddress;

            NotifyPropertyChanged("BitAddress"); NotifyPropertyChanged("S7FormatAddress"); 
            }
        }

        private bool isSymbolicAccessKeyTag;
        public bool IsSymbolicAccessKeyTag
        {
            get
            {
                return this.isSymbolicAccessKeyTag;
            }
            set
            {
                this.isSymbolicAccessKeyTag = value;
                if (!value) 
                    symbolicAccessKey = null;

                NotifyPropertyChanged("SymbolicAccessKey");
                NotifyPropertyChanged("IsSymbolicAccessKeyTag");
                NotifyPropertyChanged("S7FormatAddress"); 
            }
        }

        private string symbolicAccessKey;
        public string SymbolicAccessKey
        {
            get
            {
                return this.symbolicAccessKey;
            }
            set
            {
                this.symbolicAccessKey = value;
                if (!string.IsNullOrEmpty(value)) 
                    isSymbolicAccessKeyTag = true;
                NotifyPropertyChanged("SymbolicAccessKey");
                NotifyPropertyChanged("IsSymbolicAccessKeyTag");
                NotifyPropertyChanged("S7FormatAddress"); 
            }
        }

        //For Tags used with Full Symbolic in TIA Portal

        private bool _itemDoesNotExist;
        [XmlIgnore]
        public virtual bool ItemDoesNotExist
        {
            get { return _itemDoesNotExist; }
            set
            {
                if (_itemDoesNotExist != value)
                {
                    _itemDoesNotExist = value;
                    NotifyPropertyChanged("ItemDoesNotExist");
                    NotifyPropertyChanged("ValueAsString");
                }
            }
        }

        public PLCTag()
        { }

        public PLCTag(PLCTag oldTag)
        {
            if (oldTag != null)
            {
                this.TagDataSource = oldTag.TagDataSource;
                this.TagDataType = oldTag.TagDataType;
                this.ByteAddress = oldTag.ByteAddress;
                this.BitAddress = oldTag.BitAddress;
                this.ArraySize = oldTag.ArraySize;
                this.DataTypeStringFormat = oldTag.DataTypeStringFormat;
                this.DataBlockNumber = oldTag.DataBlockNumber;
                this.Controlvalue = oldTag.Controlvalue;
                this.DontSplitValue = oldTag.DontSplitValue;
            }
        }

        public PLCTag(string address, TagDataType type)
        {
            this.ChangeAddressFromString(address);
            this.TagDataType = type;
        }


        /// <summary>
        /// The initalizationString can be a PLC Address like: "DB100.DBW2" or a init String like: "User,DB100.DBW2,Word,Decimal"
        /// </summary>
        /// <param name="initalizationString"></param>
        public PLCTag(string initalizationString)
        {
            if (initalizationString.Contains(","))
            {
                string[] values = initalizationString.Split(',');
                this.ValueName = values[0];
                this.ChangeAddressFromString(values[1]);
                if (values.Length > 2) this.ChangeDataTypeFromString(values[2]);
                if (values.Length > 3) this.ChangeDataTypeStringFormatFromString(values[3]);
            }
            else
            {
                var low = initalizationString.ToLower();
                if (low.StartsWith("p#"))
                {
                    if (low.Contains("bool")) this.TagDataType = TagDataType.Bool;
                    else if (low.Contains("byte")) this.TagDataType = TagDataType.Byte;
                    else if (low.Contains("dword")) this.TagDataType = TagDataType.Dword;
                    else if (low.Contains("word")) this.TagDataType = TagDataType.Word;
                    else if (low.Contains("dint")) this.TagDataType = TagDataType.Dint;
                    else if (low.Contains("int")) this.TagDataType = TagDataType.Int;
                    else if (low.Contains("time_of_day")) this.TagDataType = TagDataType.TimeOfDay;                    
                    else if (low.Contains("date")) this.TagDataType = TagDataType.Date;
                    else if (low.Contains("s5time")) this.TagDataType = TagDataType.S5Time;
                    else if (low.Contains("real")) this.TagDataType = TagDataType.Float; 
                    else if (low.Contains("time")) this.TagDataType = TagDataType.Time;                    
                }
                this.ChangeAddressFromString(initalizationString);
            }
        }

        private int _datablockNumber = 1;
        public int DataBlockNumber
        {
            get
            {
                if (this.TagDataSource == MemoryArea.Datablock || this.TagDataSource == MemoryArea.InstanceDatablock)
                    return _datablockNumber > 0 ? _datablockNumber : 1;
                else
                    return 0;
            }
            set { _datablockNumber = value;
            NotifyPropertyChanged("DataBlockNumber"); NotifyPropertyChanged("S7FormatAddress"); 
            }
        }

        private MemoryArea tagDataSource = MemoryArea.Datablock;
        public MemoryArea TagDataSource
        {
            get
            {
                return this.tagDataSource;
            }
            set
            {
                this.tagDataSource = value;
                if (value == MemoryArea.Timer || value == MemoryArea.Counter)
                    if (this.TagDataType != TagDataType.Bool && this.TagDataType != TagDataType.Int && this.TagDataType != TagDataType.Word && this.TagDataType != TagDataType.S5Time && this.TagDataType != TagDataType.BCDWord)
                        this.tagDataType = value == MemoryArea.Timer ? TagDataType.S5Time : TagDataType.Int;
                NotifyPropertyChanged("TagDataSource"); NotifyPropertyChanged("S7FormatAddress"); 
            }
        }


        private TagDisplayDataType _dataTypeStringFormat = TagDisplayDataType.Bool;
        public TagDisplayDataType DataTypeStringFormat
        {
            get
            {
                switch (this.TagDataType)
                {
                    case TagDataType.Bool:
                        if (_dataTypeStringFormat != TagDisplayDataType.Bool && _dataTypeStringFormat != TagDisplayDataType.Binary)
                            return TagDisplayDataType.Bool;
                        return _dataTypeStringFormat;
                    case TagDataType.S5Time:
                    case TagDataType.Time:
                        if (_dataTypeStringFormat != TagDisplayDataType.TimeSpan && _dataTypeStringFormat != TagDisplayDataType.S5Time && _dataTypeStringFormat != TagDisplayDataType.Time)
                            return TagDisplayDataType.TimeSpan;
                        return _dataTypeStringFormat;
                    case TagDataType.Date:
                        if (_dataTypeStringFormat != TagDisplayDataType.DateTime && _dataTypeStringFormat != TagDisplayDataType.S7Date)
                            return TagDisplayDataType.S7Date;
                        return _dataTypeStringFormat;                    
                    case TagDataType.TimeOfDay:
                        if (_dataTypeStringFormat != TagDisplayDataType.DateTime && _dataTypeStringFormat != TagDisplayDataType.S7TimeOfDay)
                            return TagDisplayDataType.S7TimeOfDay;
                        return _dataTypeStringFormat;
                    case TagDataType.DateTime:
                        if (_dataTypeStringFormat != TagDisplayDataType.DateTime && _dataTypeStringFormat != TagDisplayDataType.S7DateTime)
                            return TagDisplayDataType.DateTime;
                        return _dataTypeStringFormat;
                    case TagDataType.String:
                    case TagDataType.CharArray:
                        return TagDisplayDataType.String;                        
                    case TagDataType.Int:
                    case TagDataType.Dint:
                    case TagDataType.Byte:
                    case TagDataType.SByte:
                    case TagDataType.Word:
                    case TagDataType.Dword:
                    case TagDataType.BCDByte:
                    case TagDataType.BCDWord:
                    case TagDataType.BCDDWord:
                        if (_dataTypeStringFormat != TagDisplayDataType.Decimal && _dataTypeStringFormat != TagDisplayDataType.Hexadecimal && _dataTypeStringFormat != TagDisplayDataType.Float && _dataTypeStringFormat != TagDisplayDataType.Binary && _dataTypeStringFormat != TagDisplayDataType.Pointer && _dataTypeStringFormat != TagDisplayDataType.String)
                            return TagDisplayDataType.Decimal;
                        return _dataTypeStringFormat;
                    case TagDataType.Float:
                        if (_dataTypeStringFormat != TagDisplayDataType.Decimal && _dataTypeStringFormat != TagDisplayDataType.Hexadecimal && _dataTypeStringFormat != TagDisplayDataType.Float && _dataTypeStringFormat != TagDisplayDataType.Binary)
                            return TagDisplayDataType.Float;
                        return _dataTypeStringFormat;
                    case TagDataType.ByteArray:
                        return TagDisplayDataType.ByteArray;
                }
                return TagDisplayDataType.Decimal;
            }
            set { _dataTypeStringFormat = value; NotifyPropertyChanged("DataTypeStringFormat"); NotifyPropertyChanged("Value"); NotifyPropertyChanged("ValueAsString"); }
        }

        private TagDataType tagDataType;
        public virtual TagDataType TagDataType
        {
            get
            {
                return this.tagDataType;
            }
            set
            {
                if (value == TagDataType.Struct)
                    return;
                //if (value == TagDataType.DateTime)
                //    ArraySize = 1;
                else if (value != TagDataType.Bool)
                    _bitAddress = 0;
                //else if (value == TagDataType.CharArray || value == TagDataType.ByteArray || value == TagDataType.String)
                //    ArraySize = _internalGetSize();
                if (this.tagDataSource == MemoryArea.Timer || this.tagDataSource == MemoryArea.Counter)
                {
                    if (value != TagDataType.Bool && value != TagDataType.Int && value != TagDataType.Word && value != TagDataType.S5Time && value != TagDataType.BCDWord)
                        this.tagDataType = this.TagDataSource == MemoryArea.Timer ? TagDataType.S5Time : TagDataType.Int;
                    else
                        this.tagDataType = value;

                }
                else
                    this.tagDataType = value;
                NotifyPropertyChanged("TagDataType");
                NotifyPropertyChanged("S7FormatAddress");
                NotifyPropertyChanged("DataTypeStringFormat");
                NotifyPropertyChanged("ReadByteSize");
                NotifyPropertyChanged("ArraySize");
            }
        }
        
        //For a List of old Values...
        protected List<Object> _oldvalues;
        [XmlIgnore]
        public List<Object> OldValues
        {
            get { return _oldvalues; }
        }

        protected int _backupvaluescount = 0;
        public int BackupValuesCount
        {
            get
            {
                return _backupvaluescount;
            }
            set
            {
                _backupvaluescount = value;
                if (value > 0 && _oldvalues == null)
                    _oldvalues = new List<object>();
                else if (value == 0)
                    _oldvalues = null;
                NotifyPropertyChanged("BackupValuesCount");
            }
        }


        private bool _dontSplitValue = true;
        /// <summary>
        /// This Tag can not be splittet into more then one PDU (if there are a few rest Bytes in a PDU aviable)
        /// --> If this is set, the Tag is not allowed to be bigger!
        /// This bit should be set in all Tags wich are used for indirect Addressing! (Becaus eit could be you read 2 bytes of a double, then the value chages in the plc, then you read the rest!)
        /// This is also used when Writing the Value, and when set, it is not splitted. When the Value is bigger then a PDU, an error occurs
        /// </summary>
        public virtual bool DontSplitValue
        {
            get { return _dontSplitValue; }
            set { _dontSplitValue = value; NotifyPropertyChanged("DontSplitValue"); }
        }

        private int _arraySize = 1;
        /// <summary>
        /// Only valid (and used!) with String, CharArray and ByteArray Type!
        /// </summary>
        public int ArraySize
        {
            get
            {
                //if (TagDataType== TagDataType.String || TagDataType==TagDataType.CharArray|| TagDataType==TagDataType.ByteArray)
                
                return _arraySize; 
                //return 1;
            }
            set
            {
                _arraySize = value == 0 ? 1 : value;
                NotifyPropertyChanged("ArraySize");
                NotifyPropertyChanged("S7FormatAddress");
                NotifyPropertyChanged("ReadByteSize");
            }
        }



        #region Read Write Value of the TAG
        /// <summary>
        /// Helper Property to Set the Value, because the Setter of the normal Value Sets Control Value!
        /// </summary>
        protected internal virtual Object _setValueProp
        {
            set
            {
                _value = value;

                if (_value == null)
                {
                    NotifyPropertyChanged("Value");
                    NotifyPropertyChanged("ValueAsString");
                    
                    if (ValueChanged != null) ValueChanged(this, new ValueChangedEventArgs(_oldvalue, _value));

                    _oldvalue = _value;
                }
                else
                {
                    if (!_value.Equals(_oldvalue) || RaiseValueChangedEvenWhenNoChangeHappened)
                    {
                        NotifyPropertyChanged("Value");
                        NotifyPropertyChanged("ValueAsString");

                        if (ValueChanged != null) ValueChanged(this, new ValueChangedEventArgs(_oldvalue, _value));

                        if (MaximumReached != null && _value is IComparable && ((IComparable)_value).CompareTo(Maximum) >= 0) MaximumReached(this, new LimitReachedEventArgs(_oldvalue, Maximum));

                        if (MinimumReached != null && _value is IComparable && ((IComparable)_value).CompareTo(Minimum) <= 0) MinimumReached(this, new LimitReachedEventArgs(_oldvalue, Minimum));

                        _oldvalue = _value;
                    }

                    if (BackupValuesCount > 0 && _oldvalues != null)
                    {
                        _oldvalues.Add(_value);
                        if (_oldvalues.Count - _backupvaluescount > 0) _oldvalues.RemoveRange(0, _oldvalues.Count - _backupvaluescount);
                        NotifyPropertyChanged("OldValues");
                    }
                }


            }
        }

        protected Object _value;
        private Object _oldvalue;
        /// <summary>
        /// Value of the Tag. The Setter of this Property sets Controlvalue, because the Tag firstly got this Value when it's written!
        /// </summary>
        public virtual Object Value
        {
            get { return _value; }
            set
            {
                if (_controlvalue == null || !_controlvalue.Equals(value))
                {
                    _controlvalue = value;
                    NotifyPropertyChanged("Controlvalue");
                }
            }
        }

        public void ClearValue()
        {
            _value = null;
            _oldvalue = null;
        }

        [XmlIgnore]
        public String ValueAsString
        {
            get { return GetValueAsString(); }
            set
            {
                ParseControlValueFromString((String) value);
                NotifyPropertyChanged("ControlValueAsString");    
            }
        }

        protected Object _controlvalue;
        public virtual Object Controlvalue
        {
            get { return _controlvalue; }
            set
            {
                if (_controlvalue == null || !_controlvalue.Equals(value))
                {
                    _controlvalue = value;
                    //_oldvalue = null;
                    NotifyPropertyChanged("Controlvalue");
                    NotifyPropertyChanged("ControlValueAsString");    
                }
            }
        }

        [XmlIgnore]
        public String ControlValueAsString
        {
            get { return Controlvalue != null ? GetValueAsString(_controlvalue) : null; }
            set
            {
                ParseControlValueFromString((String)value);
                NotifyPropertyChanged("ControlValueAsString");
            }
        }

        #endregion

        [XmlIgnore]
        public string S7FormatAddress
        {
            get
            {
                int aksz = _internalGetSize();
                StringBuilder ret = new StringBuilder();

                if (aksz == 3 || (aksz > 4 && this.TagDataSource != MemoryArea.Timer && this.TagDataSource != MemoryArea.Counter))
                    ret.Append("P#");

                if (this.TagDataSource == MemoryArea.Datablock || this.TagDataSource == MemoryArea.InstanceDatablock)
                {
                    if (this.TagDataSource == MemoryArea.InstanceDatablock)
                        ret.Append("DI");
                    else
                        ret.Append("DB");
                    ret.Append(this.DataBlockNumber);
                    ret.Append(".");
                    if (this.TagDataSource == MemoryArea.InstanceDatablock)
                        ret.Append("DI");
                    else
                        ret.Append("DB");
                    if (this.TagDataType == TagDataType.Bool || aksz == 3 || aksz > 4)
                        ret.Append("X");
                }

                switch (this.TagDataSource)
                {
                    case MemoryArea.Inputs:
                        ret.Append("E");
                        break;
                    case MemoryArea.Outputs:
                        ret.Append("A");
                        break;
                    case MemoryArea.Flags:
                        ret.Append("M");
                        break;
                    case MemoryArea.Timer:
                        ret.Append("T");
                        break;
                    case MemoryArea.Counter:
                        ret.Append("Z");
                        break;
                    case MemoryArea.LocalData:
                        ret.Append("L");
                        break;
                    case MemoryArea.PreviousLocalData:
                        ret.Append("V");
                        break;
                }

                if (this.TagDataType == TagDataType.Bool && ArraySize < 2)
                {
                    ret.Append(ByteAddress);
                    if (this.TagDataSource != MemoryArea.Timer && this.TagDataSource != MemoryArea.Counter)
                    {
                        ret.Append(".");
                        ret.Append(BitAddress);
                    }
                }
                else if (this.TagDataSource != MemoryArea.Counter && this.TagDataSource != MemoryArea.Timer)
                {

                    if (aksz == 3 || aksz > 4)
                    {
                        ret.Append(ByteAddress);
                        ret.Append(".");
                        ret.Append(BitAddress);
                        ret.Append(" BYTE ");
                        ret.Append(_internalGetSize().ToString());
                    }
                    else if (aksz == 4)
                    {
                        ret.Append("D");
                        ret.Append(ByteAddress);
                    }
                    else if (aksz == 1)
                    {
                        ret.Append("B");
                        ret.Append(ByteAddress);
                    }
                    else
                    {
                        ret.Append("W");
                        ret.Append(ByteAddress);
                    }
                }
                else
                {
                    ret.Append(ByteAddress);
                }

                if (isSymbolicAccessKeyTag)
                {
                    ret.Append(" (" + symbolicAccessKey + ")");
                }

                return ret.ToString();
            }

            set
            {
                ChangeAddressFromString(value);     
                NotifyPropertyChanged("SymbolicAccessKey"); 
                NotifyPropertyChanged("S7FormatAddress"); 
                NotifyPropertyChanged("ReadByteSize"); 
                NotifyPropertyChanged("ArraySize");
            }
        }

        public string GetControlValueAsString()
        {
            return GetValueAsString(this.Controlvalue);
        }

        public void ParseControlValueFromString(string myValue)
        {
            string myValueStrip = myValue.ToLower().Trim();
            switch (this.TagDataType)
            {
                case TagDataType.S5Time:
                case TagDataType.Time:
                    if (myValueStrip.Contains("t#") || myValueStrip.Contains("s5t#"))
                    {
                        Controlvalue = Helper.GetTimespanFromS5TimeorTime(myValue);
                    }
                    else
                    {
                        TimeSpan ret;
                        TimeSpan.TryParse(myValue, out ret);
                        Controlvalue = ret;
                    }                        
                    break;
                case TagDataType.BCDWord:                
                case TagDataType.Int:
                    if (myValueStrip.Contains("w#16#") || myValueStrip.Contains("dw#16#"))
                    {
                        Controlvalue = Convert.ToInt16(Helper.GetIntFromHexString(myValue));
                    }
                    else if (myValue.StartsWith("2#"))
                    {
                        Controlvalue = Convert.ToInt16(Helper.GetIntFromBinString(myValue));
                    }
                    else
                    {
                        Int16 ret;
                        Int16.TryParse(myValue, out ret);
                        Controlvalue = ret;
                    }
                    break;
                case TagDataType.BCDDWord:
                case TagDataType.Dint:
                    if (myValueStrip.Contains("w#16#") || myValueStrip.Contains("dw#16#"))
                        Controlvalue = Convert.ToInt32(Helper.GetIntFromHexString(myValue));
                    else if (myValue.StartsWith("2#"))
                        Controlvalue = Convert.ToInt32(Helper.GetIntFromBinString(myValue));
                    else
                        try
                        {
                            if (!string.IsNullOrEmpty(myValue))
                                Controlvalue = Int32.Parse(myValue);
                        } catch (Exception) {}
                    break;                
                case TagDataType.Byte:
                    if (myValueStrip.Contains("w#16#") || myValueStrip.Contains("dw#16#"))
                    {
                        Controlvalue = Convert.ToByte(Helper.GetUIntFromHexString(myValue));
                    }
                    else if (myValue.StartsWith("2#"))
                    {
                        Controlvalue = Convert.ToByte(Helper.GetIntFromBinString(myValue));
                    }
                    else
                    {
                        Byte ret;
                        Byte.TryParse(myValue, out ret);
                        Controlvalue = ret;
                    }
                    break;
                case TagDataType.BCDByte:
                case TagDataType.SByte:
                    if (myValueStrip.Contains("w#16#") || myValueStrip.Contains("dw#16#"))
                    {
                        Controlvalue = Convert.ToSByte(Helper.GetIntFromHexString(myValue));
                    }
                    else if (myValue.StartsWith("2#"))
                    {
                        Controlvalue = Convert.ToSByte(Helper.GetIntFromBinString(myValue));
                    }
                    else
                    {
                        SByte ret;
                        SByte.TryParse(myValue, out ret);
                        Controlvalue = ret;
                    }
                    break;                               
                case TagDataType.Word:
                    if (myValueStrip.Contains("w#16#") || myValueStrip.Contains("dw#16#"))
                    {
                        Controlvalue = Convert.ToUInt16(Helper.GetUIntFromHexString(myValue));
                    }
                    else if (myValue.StartsWith("2#"))
                    {
                        Controlvalue = Convert.ToUInt16(Helper.GetIntFromBinString(myValue));
                    }
                    else
                    {
                        UInt16 ret;
                        UInt16.TryParse(myValue, out ret);
                        Controlvalue = ret;
                    }
                    break;                              
                case TagDataType.Dword:
                    if (myValueStrip.Contains("w#16#") || myValueStrip.Contains("dw#16#"))
                    {
                        Controlvalue = Convert.ToUInt32(Helper.GetUIntFromHexString(myValue));
                    }
                    else if (myValue.StartsWith("2#"))
                    {
                        Controlvalue = Convert.ToUInt32(Helper.GetIntFromBinString(myValue));
                    }
                    else
                    {
                        UInt32 ret;
                        UInt32.TryParse(myValue, out ret);
                        Controlvalue = ret;
                    }
                    break;
                case TagDataType.Bool:
                    if (myValue=="1")
                        Controlvalue = true;
                    else if (myValue=="0")
                        Controlvalue = false;
                    else
                    {
                        bool bvalue;
                        bool.TryParse(myValue, out bvalue);
                        Controlvalue = bvalue;
                    }
                    break;
                case TagDataType.String:
                case TagDataType.CharArray:
                    Controlvalue = myValue;
                    break;             
                case TagDataType.DateTime:            
                case TagDataType.Date:
                case TagDataType.TimeOfDay:
                    if (myValueStrip.StartsWith("d#"))
                        Controlvalue = Helper.GetDateTimeFromDateString(myValue);
                    else if (myValueStrip.StartsWith("tod#"))
                        Controlvalue = Helper.GetDateTimeFromTimeOfDayString(myValue);
                    else if (myValueStrip.StartsWith("dt#"))
                        Controlvalue = Helper.GetDateTimeFromDateAndTimeString(myValue);
                    else if (!string.IsNullOrEmpty(myValue))
                        try
                        {
                            if (!string.IsNullOrEmpty(myValue))
                                Controlvalue = DateTime.Parse(myValue);
                        }
                        catch (Exception) { }
                    break;
                case TagDataType.ByteArray:
                    {
                        if (myValueStrip.Length > 2 && myValueStrip[0] == '{' && myValueStrip[myValueStrip.Length - 1] == '}')
                            myValueStrip = myValueStrip.Substring(1, myValueStrip.Length - 2);
                        string[] vals = myValueStrip.Split(',');
                        byte[] wrt = new byte[vals.Length];
                        int i = 0;

                        foreach (string val in vals)
                        {
                            try
                            {
                                wrt[i++] = Convert.ToByte(val);
                            }
                            catch (Exception)
                            { }
                        }
                        Controlvalue = wrt;
                    }
                    break;
                case TagDataType.Float:
                    {
                        Single val;
                        Controlvalue = Single.TryParse(myValue, out val);
                        Controlvalue = val;
                    }
                    break;
/* 
 *  case TagDataType.TimeOfDay:    
    case TagDataType.Float:
    */
            }
        }

        private string GetValueAsString(object myValue)
        {
            if (myValue != null)
            {
                switch (this.TagDataType)
                {
                    case TagDataType.String:
                    case TagDataType.CharArray:
                        return myValue.ToString();
                    case TagDataType.ByteArray:
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("{");
                            foreach (Byte bt in ((System.Byte[])myValue))
                            {
                                if (sb.Length > 1) sb.Append(";");
                                sb.Append(bt.ToString());
                            }
                            sb.Append("}");
                            return sb.ToString();
                        }

                    case TagDataType.S5Time:
                    case TagDataType.Time:
                    case TagDataType.Bool:
                    case TagDataType.DateTime:
                    case TagDataType.Date:
                    case TagDataType.TimeOfDay:
                    case TagDataType.Int:
                    case TagDataType.Dint:
                    case TagDataType.Byte:
                    case TagDataType.SByte:
                    case TagDataType.Word:
                    case TagDataType.Dword:
                    case TagDataType.Float:
                    case TagDataType.BCDByte:
                    case TagDataType.BCDWord:
                    case TagDataType.BCDDWord:
                        if (ArraySize < 2)
                        {
                            return this.GetValueAsStringInternal(myValue);
                        }
                        else
                        {
                            string ret = "";
                            var ienumer = myValue as IEnumerable;
                            if (ienumer != null)
                            {
                                var enumer = ienumer.GetEnumerator();
                                for (int i = 0; i < ArraySize; i++)
                                {
                                    if (enumer.MoveNext())
                                    {
                                        if (ret != "") ret += ";";
                                        ret += this.GetValueAsStringInternal(enumer.Current);
                                    }
                                }
                                return ret;
                            }
                        }
                        break;
                }
            }

            return "";
        }


        private string GetValueAsStringInternal(object myValue)
        {
            if (myValue != null)
            {
                switch (this.TagDataType)
                {
                    case TagDataType.S5Time:
                        {
                            if (DataTypeStringFormat == TagDisplayDataType.S5Time)
                            {
                                var bt = new byte[2];
                                libnodave.putS5Timeat(bt, 0, (TimeSpan)myValue);
                                return Helper.GetS5Time(bt[0], bt[1]);
                            }
                            return ((TimeSpan)myValue).ToString();
                        }
                    case TagDataType.Time:
                        {
                            var tm = (TimeSpan)myValue;
                            var ret = new StringBuilder("T#");
                            if (tm.TotalMilliseconds < 0) ret.Append("-");
                            if (tm.Days != 0) ret.Append(tm.Days + "D");
                            if (tm.Hours != 0) ret.Append(tm.Hours + "H");
                            if (tm.Minutes != 0) ret.Append(tm.Minutes + "M");
                            if (tm.Seconds != 0) ret.Append(tm.Seconds + "S");
                            if (tm.Milliseconds != 0) ret.Append(tm.Milliseconds + "MS");
                            return ret.ToString();
                        }                                         
                    case TagDataType.Bool:
                        if (DataTypeStringFormat == TagDisplayDataType.Binary)
                        {
                            if (((bool)myValue) == true)
                                return "1";
                            return "0";
                        }
                        return myValue.ToString();
                    case TagDataType.DateTime:
                    case TagDataType.Date:
                    case TagDataType.TimeOfDay:
                        if (DataTypeStringFormat == TagDisplayDataType.S7DateTime)
                        {
                            DateTime ak = (DateTime) myValue;
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
                        else if (DataTypeStringFormat == TagDisplayDataType.S7Date)
                        {
                            DateTime ak = (DateTime)myValue;
                            StringBuilder sb = new StringBuilder();
                            sb.Append("D#");
                            sb.Append(ak.Year);
                            sb.Append("-");
                            sb.Append(ak.Month);
                            sb.Append("-");
                            sb.Append(ak.Day);                            
                            return sb.ToString();
                        }
                        else if (DataTypeStringFormat == TagDisplayDataType.S7TimeOfDay)
                        {
                            DateTime ak = (DateTime)myValue;
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
                        return myValue.ToString();
                    
                    case TagDataType.Int:
                    case TagDataType.Dint:
                    case TagDataType.Byte:
                    case TagDataType.SByte:
                    case TagDataType.Word:
                    case TagDataType.Dword:
                    case TagDataType.Float:
                    case TagDataType.BCDByte:
                    case TagDataType.BCDWord:
                    case TagDataType.BCDDWord:
                        IFormattable val = myValue as IFormattable ?? 0;
                        switch (DataTypeStringFormat)
                        {
                            case TagDisplayDataType.String:
                                switch (this.TagDataType)
                                {
                                    case TagDataType.Int:
                                        return Encoding.ASCII.GetString(BitConverter.GetBytes((Int16)myValue));
                                    case TagDataType.Dint:
                                        return Encoding.ASCII.GetString(BitConverter.GetBytes((Int32)myValue));
                                    case TagDataType.Byte:
                                        return Encoding.ASCII.GetString(new []{(Byte)myValue});
                                    case TagDataType.SByte:
                                        return Encoding.ASCII.GetString(new[] { BitConverter.GetBytes((SByte)myValue)[0] });
                                    case TagDataType.Word:
                                        return Encoding.ASCII.GetString(BitConverter.GetBytes((UInt16)myValue));
                                    case TagDataType.Dword:
                                        return Encoding.ASCII.GetString(BitConverter.GetBytes((UInt32)myValue));
                                    case TagDataType.Float:
                                        return Encoding.ASCII.GetString(BitConverter.GetBytes((Single)myValue));
                                    case TagDataType.BCDByte:
                                        return Encoding.ASCII.GetString(BitConverter.GetBytes((Byte)myValue));
                                    case TagDataType.BCDWord:
                                        return Encoding.ASCII.GetString(BitConverter.GetBytes((UInt16)myValue));
                                    case TagDataType.BCDDWord:
                                        return Encoding.ASCII.GetString(BitConverter.GetBytes((UInt32)myValue));                                        
                                }
                                break;
                            case TagDisplayDataType.Pointer:
                                return "P#" + (Convert.ToInt32(myValue) / 8).ToString() + "." + (Convert.ToInt32(myValue) % 8).ToString();
                                break;
                            case TagDisplayDataType.Hexadecimal:
                                string ad = "";
                                switch (_internalGetSize())
                                {
                                    case 1:
                                        ad = "B#16#";
                                        break;
                                    case 2:
                                        ad = "W#16#";
                                        break;
                                    case 4:
                                        ad = "DW#16#";
                                        break;
                                }
                                return ad + val.ToString("X", NumberFormatInfo.CurrentInfo).PadLeft(_internalGetSize() * 2, '0');
                            case TagDisplayDataType.Binary:
                                byte[] bt = new byte[] { };
                                switch (this.TagDataType)
                                {
                                    case TagDataType.Int:
                                        bt = BitConverter.GetBytes((Int16)myValue);
                                        break;
                                    case TagDataType.Dint:
                                        bt = BitConverter.GetBytes((Int32)myValue);
                                        break;
                                    case TagDataType.Byte:
                                        bt = new byte[] { (byte)myValue };
                                        break;
                                    case TagDataType.SByte:
                                        bt = new byte[] { (byte)myValue };
                                        break;
                                    case TagDataType.Word:
                                        bt = BitConverter.GetBytes((UInt16)myValue);
                                        break;
                                    case TagDataType.Dword:
                                        bt = BitConverter.GetBytes((UInt32)myValue);
                                        break;
                                    case TagDataType.Float:
                                        bt = BitConverter.GetBytes((float)myValue);
                                        break;
                                    case TagDataType.BCDByte:
                                        bt = BitConverter.GetBytes((byte)myValue);
                                        break;
                                    case TagDataType.BCDWord:
                                        bt = BitConverter.GetBytes((Int16)myValue);
                                        break;
                                    case TagDataType.BCDDWord:
                                        bt = BitConverter.GetBytes((Int32)myValue);
                                        break;
                                }
                                string ret = "";
                                foreach (byte b in bt)
                                {
                                    if (ret != "") ret = "_" + ret;
                                    ret = libnodave.dec2bin(b).Substring(0, 4) + "_" + libnodave.dec2bin(b).Substring(4, 4) + ret;

                                }
                                return "2#" + ret;

                        }
                        return val.ToString();                    
                }
            }
            return "";
        }

        public string GetValueAsString()
        {
            if (ItemDoesNotExist)
                return "Item does not exist on the PLC!";
            return GetValueAsString(this.Value);
        }       

        public override string ToString()
        {           
            string old = "";
            if (_oldvalues != null)
            {
                old = "   -- Old-Values: ";
                foreach (var oldvalue in _oldvalues)
                {
                    old += oldvalue.ToString() + ",";
                }
                old += "";
            }

            if (Value != null)
            {                
                return S7FormatAddress + " = " + GetValueAsString() + old;
            }
            return S7FormatAddress;
        }

        public void ChangeAddressFromString(String newPlcAddress)
        {
            var plcAddress = newPlcAddress;
            if (plcAddress.Contains("("))
            {
                var pos = newPlcAddress.IndexOf('(');
                plcAddress = newPlcAddress.Substring(0, pos);
                symbolicAccessKey = newPlcAddress.Substring(pos + 1, newPlcAddress.Length - pos - 2);
                isSymbolicAccessKeyTag = true;
            }

            if (plcAddress.StartsWith("%")) 
                plcAddress = plcAddress.Substring(1);
            
            try
            {
                if (!string.IsNullOrEmpty(plcAddress))
                {
                    plcAddress = plcAddress.Trim();
                    if (plcAddress.Length > 1 && plcAddress.Substring(0, 2).ToLower() == "p#")
                    {
                        string[] myPlcAddress = plcAddress.ToLower().Replace("byte", " byte ").Replace("  ", " ").Replace("p#", "").Split(' ');
                        BitAddress = 0;
                        if (!myPlcAddress[0].Contains("db"))
                        {
                            this.DataBlockNumber = 0;
                            var tmp = myPlcAddress[0].Split('.')[0];
                            if (tmp.Contains("e") || tmp.Contains("i"))
                                this.TagDataSource = MemoryArea.Inputs;
                            else if (tmp.Contains("a") || tmp.Contains("q"))
                                this.TagDataSource = MemoryArea.Outputs;
                            else if (tmp.Contains("l"))
                                this.TagDataSource = MemoryArea.LocalData;
                            else if (tmp.Contains("v"))
                                this.TagDataSource = MemoryArea.PreviousLocalData;
                            else if (tmp.Contains("m"))
                                this.TagDataSource = MemoryArea.Flags;
                            else if (tmp.Contains("t"))
                                this.TagDataSource = MemoryArea.Timer;
                            else if (tmp.Contains("z") || tmp.Contains("c"))
                                this.TagDataSource = MemoryArea.Counter;
                            ByteAddress = Convert.ToInt32(Regex.Replace(myPlcAddress[0].Split('.')[0], "[a-z]", ""));
                        }
                        else
                        {
                            this.TagDataSource = MemoryArea.Datablock;
                            this.DataBlockNumber = Convert.ToInt32(myPlcAddress[0].Split('.')[0].Replace("db", ""));
                            ByteAddress = Convert.ToInt32(myPlcAddress[0].Split('.')[1].Replace("dbx", ""));
                        }
                        ArraySize = Convert.ToInt32(myPlcAddress[2]);

                        var tsize = 1;
                        switch (myPlcAddress[1])
                        {
                            case "word":
                            case "int":
                            case "date":
                            case "s5time":
                                tsize = 2;
                                break;
                            case "real":
                            case "dword":
                            case "dint":
                            case "time":
                            case "time_of_day":
                                tsize = 4;
                                break;
                        }

                        var baseS = _internalGetBaseTypeSize();
                        if (baseS > 0) ArraySize = ArraySize / baseS;

                        ArraySize = ArraySize * tsize;

                        //if (this.TagDataType != TagDataType.ByteArray && this.TagDataType != TagDataType.CharArray && this.TagDataType != TagDataType.String && this.TagDataType != TagDataType.DateTime)
                        //    this.TagDataType = TagDataType.ByteArray;
                        //if (ArraySize != 8 && this.TagDataType == TagDataType.DateTime)
                        //    this.TagDataType = TagDataType.ByteArray;

                        if (this.TagDataType == TagDataType.String)
                            ArraySize -= 2;
                    }
                    else
                    {
                        string[] myPlcAddress = plcAddress.ToUpper().Trim().Replace(" ", "").Split('.');
                        if (myPlcAddress.Length >= 2 && (myPlcAddress[0].Contains("DB") || myPlcAddress[0].Contains("DI")))
                        {
                            this.TagDataSource = MemoryArea.Datablock;
                            this.DataBlockNumber = Convert.ToInt32(myPlcAddress[0].Replace("DB", "").Replace("DI", "").Trim());
                            if (myPlcAddress[1].Contains("DBW"))
                            {
                                if (this.TagDataType == TagDataType.String || this.TagDataType == TagDataType.CharArray || this.TagDataType == TagDataType.ByteArray)
                                    ArraySize = 2;
                                else 
                                    ArraySize = 1;
                                if (this._internalGetSize() != 2)
                                    this.TagDataType = TagDataType.Word;
                            }
                            else if (myPlcAddress[1].Contains("DBB"))
                            {
                                ArraySize = 1;
                                if (this.TagDataType == TagDataType.Bool || this._internalGetSize() != 1)
                                    this.TagDataType = TagDataType.Byte;
                            }
                            else if (myPlcAddress[1].Contains("DBD"))
                            {
                                if (this.TagDataType == TagDataType.String || this.TagDataType == TagDataType.CharArray || this.TagDataType == TagDataType.ByteArray)
                                    ArraySize = 4;
                                else 
                                    ArraySize = 1;
                                if (this._internalGetSize() != 4)
                                    this.TagDataType = TagDataType.Dword;
                            }
                            else if (myPlcAddress[1].Contains("DBX"))
                            {
                                ArraySize = 1;
                                this.TagDataType = TagDataType.Bool;
                                if (myPlcAddress.Length > 2)
                                    this.BitAddress = Convert.ToInt32(myPlcAddress[2]);
                                else
                                    this.BitAddress = 0;
                            }
                            this.ByteAddress = Convert.ToInt32(myPlcAddress[1].Replace("DBW", "").Replace("DBD", "").Replace("DBX", "").Replace("DBB", "").Trim());
                        }
                        else
                        {
                            if (myPlcAddress[0].Contains("E") || myPlcAddress[0].Contains("I"))
                                this.TagDataSource = MemoryArea.Inputs;
                            else if (myPlcAddress[0].Contains("A") || myPlcAddress[0].Contains("Q"))
                                this.TagDataSource = MemoryArea.Outputs;
                            else if (myPlcAddress[0].Contains("M"))
                                this.TagDataSource = MemoryArea.Flags;
                            else if (myPlcAddress[0].Contains("T"))
                                this.TagDataSource = MemoryArea.Timer;
                            else if (myPlcAddress[0].Contains("Z") || myPlcAddress[0].Contains("C"))
                                this.TagDataSource = MemoryArea.Counter;

                            if (myPlcAddress[0].Contains("W"))
                            {
                                if (this.TagDataType == TagDataType.String || this.TagDataType == TagDataType.CharArray || this.TagDataType == TagDataType.ByteArray)
                                    ArraySize = 2;
                                else 
                                    ArraySize = 1;
                                if (_internalGetSize() != 2)
                                    this.TagDataType = TagDataType.Int;
                            }
                            else if (myPlcAddress[0].Contains("DBB"))
                            {
                                ArraySize = 1;
                                if (this.TagDataType == TagDataType.Bool || this._internalGetSize() != 1)
                                    this.TagDataType = TagDataType.Byte;
                            }
                            else if (myPlcAddress[0].Contains("D"))
                            {
                                if (this.TagDataType == TagDataType.String || this.TagDataType == TagDataType.CharArray || this.TagDataType == TagDataType.ByteArray)
                                    ArraySize = 4;
                                else
                                    ArraySize = 1;
                                if (_internalGetSize() != 4)
                                    this.TagDataType = TagDataType.Dint;
                            }
                            else if (myPlcAddress[0].Contains("B"))
                            {
                                ArraySize = 1;
                                if (this.TagDataType == TagDataType.Bool || this._internalGetSize() != 1)
                                    this.TagDataType = TagDataType.Byte;
                            }
                            else if (!myPlcAddress[0].Contains("T") && !myPlcAddress[0].Contains("Z"))
                            {
                                ArraySize = 1;
                                this.TagDataType = TagDataType.Bool;
                                if (myPlcAddress.Length >= 2)
                                    this.BitAddress = Convert.ToInt32(myPlcAddress[1]);
                                else
                                    this.BitAddress = 0;
                            }
                            else if (myPlcAddress[0].Contains("T"))
                            {
                                ArraySize = 1;
                                this.TagDataType = TagDataType.S5Time;
                            }
                            else if (myPlcAddress[0].Contains("Z") || myPlcAddress[0].Contains("C"))
                            {
                                ArraySize = 1;
                                this.TagDataType = TagDataType.Int;
                            }
                            else
                            {
                                ArraySize = 1;
                                if (_internalGetSize() != 1)
                                    this.TagDataType = TagDataType.Bool;
                            }

                            this.ByteAddress = Convert.ToInt32(Regex.Replace(myPlcAddress[0].ToLower(), "[a-z]", "").Trim());

                            if (this.TagDataType == TagDataType.String)
                                ArraySize -= 2;
                        }
                    }
                }
            }
            catch(Exception)
            {
                if (plcAddress!=null)
                    if (plcAddress.ToLower().Contains("p#"))
                    {
                        this.TagDataType = TagDataType.ByteArray;
                        this.ArraySize = 10;
                    }
            }
        }

        public void ChangeDataTypeFromString(String datatype)
        {
            if (!string.IsNullOrEmpty(datatype))
            {
                TagDataType tp = TagDataType.Word;
                datatype = datatype.ToLower().Trim().Replace(" ", "");
                switch (datatype)
                {
                    case "bool":
                        tp = TagDataType.Bool;
                        break;
                    case "word":
                        tp = TagDataType.Word;
                        break;
                    case "int":
                    case "integer":
                        tp = TagDataType.Int;
                        break;
                    case "dword":
                        tp = TagDataType.Dword;
                        break;
                    case "dint":
                        tp = TagDataType.Dint;
                        break;
                    case "byte":
                        tp = TagDataType.Byte;
                        break;
                    case "sbyte":
                        tp = TagDataType.SByte;
                        break;
                    case "string":
                        tp = TagDataType.String;
                        break;
                    case "time":
                        tp = TagDataType.Time;
                        break;
                    case "s5time":
                        tp = TagDataType.S5Time;
                        break;
                    case "timeofday":
                        tp = TagDataType.TimeOfDay;
                        break;
                    case "date":
                        tp = TagDataType.Date;
                        break;
                    case "bcdbyte":
                    case "bcd":
                        tp = TagDataType.BCDByte;
                        break;
                    case "bcdword":
                        tp = TagDataType.BCDWord;
                        break;
                    case "bcddword":
                        tp = TagDataType.BCDDWord;
                        break;
                    case "datetime":
                    case "dateandtime":
                        tp = TagDataType.DateTime;
                        break;
                    case "char":
                    case "chararray":
                        tp = TagDataType.CharArray;
                        break;
                    case "bytearray":
                        tp = TagDataType.ByteArray;
                        break;
                    case "float":
                    case "real":
                        tp = TagDataType.Float;
                        break;
                }
                this.TagDataType = tp;
            }
        }

        public void ChangeDataTypeStringFormatFromString(String datatype)
        {
            if (!string.IsNullOrEmpty(datatype))
            {
                TagDisplayDataType tp = TagDisplayDataType.Decimal;
                datatype = datatype.ToLower().Trim().Replace(" ", "");
                switch (datatype.Replace(" ", "").Replace("_", "").Trim())
                {
                    case "decimal":
                    case "dec":
                        tp = TagDisplayDataType.Decimal;
                        break;
                    case "hexadecimal":
                    case "hex":
                        tp = TagDisplayDataType.Hexadecimal;
                        break;
                    case "binary":
                    case "bin":
                        tp = TagDisplayDataType.Binary;
                        break;
                    case "pointer":
                        tp = TagDisplayDataType.Pointer;
                        break;
                    case "bool":
                        tp = TagDisplayDataType.Bool;
                        break;
                    case "byte":
                    case "bytearray":
                        tp = TagDisplayDataType.ByteArray;
                        break;
                    case "datetime":
                        tp = TagDisplayDataType.DateTime;
                        break;
                    case "date":
                    case "s7date":
                        tp = TagDisplayDataType.S7Date;
                        break;
                    case "s7timeofday":
                    case "timeofday":
                        tp = TagDisplayDataType.S7TimeOfDay;
                        break;
                    case "float":
                    case "real":
                        tp = TagDisplayDataType.Float;
                        break;
                    case "s5time":
                        tp = TagDisplayDataType.S5Time;
                        break;
                    case "s7datetime":
                    case "dateandtime":
                        tp = TagDisplayDataType.S7DateTime;
                        break;
                    case "string":
                    case "strg":
                        tp = TagDisplayDataType.String;
                        break;
                    case "time":
                    case "s7time":
                        tp = TagDisplayDataType.Time;
                        break;
                    case "timespan":
                        tp = TagDisplayDataType.TimeSpan;
                        break;
                }
                this.DataTypeStringFormat = tp;
            }
        }

        public static PLCTag GetPLCTagFromString(String plcAddress)
        {
            PLCTag retValue = new PLCTag();
            retValue.ChangeAddressFromString(plcAddress);
            return retValue;
        }

        public bool LimitControlValuesRange { get; set; }

        internal virtual void _putControlValueIntoBuffer(byte[] buff, int startpos)
        {
            if (this.ArraySize == 1 || this.TagDataType == TagDataType.String || this.TagDataType == TagDataType.CharArray || this.TagDataType == TagDataType.ByteArray)
            {
                _putControlValueIntoBuffer(buff, startpos, Controlvalue);
            }
            else
            {
                int n = 0;
                foreach (var ctlVal in (IEnumerable)Controlvalue)
                {
                    _putControlValueIntoBuffer(buff, startpos + (n * this._internalGetBaseTypeSize()), ctlVal);
                    n++;
                    if (n >= ArraySize) break;
                }                
            }
        }


        internal virtual void _putControlValueIntoBuffer(byte[] buff, int startpos, object ctlValue)
        {
            if (ctlValue != null)
                switch (this.TagDataType)
                {
                    case TagDataType.Word:
                        libnodave.putU16at(buff, startpos, Convert.ToUInt16(ctlValue));
                        break;
                    case TagDataType.String:
                        libnodave.putS7Stringat(buff, startpos, ctlValue.ToString(), ArraySize);
                        break;
                    case TagDataType.CharArray:
                        libnodave.putStringat(buff, startpos, ctlValue.ToString(), ArraySize);
                        break;
                    case TagDataType.ByteArray:
                        {
                            byte[] tmp = (byte[])ctlValue;
                            for (int n = 0; n < ArraySize; n++)
                            {
                                if (n > tmp.Length)
                                    buff[startpos + n] = 0x00;
                                else
                                    buff[startpos + n] = tmp[n];
                            }
                        }
                        break;
                    case TagDataType.Bool:
                        bool tmp1 = false;
                        tmp1 = Convert.ToBoolean(ctlValue);
                        buff[startpos] = Convert.ToByte(tmp1);
                        break;
                    case TagDataType.Byte:
                        buff[startpos] = Convert.ToByte(ctlValue);
                        break;
                    case TagDataType.SByte:
                        buff[startpos] = (Byte)Convert.ToSByte(ctlValue);
                        break;
                    case TagDataType.Time:
                        libnodave.putTimeat(buff, startpos, (TimeSpan)ctlValue);
                        break;
                    case TagDataType.TimeOfDay:
                        libnodave.putTimeOfDayat(buff, startpos, (DateTime)ctlValue);
                        break;
                    case TagDataType.BCDByte:
                        libnodave.putBCD8at(buff, startpos, Convert.ToInt32(ctlValue));
                        break;
                    case TagDataType.Int:
                        libnodave.putS16at(buff, startpos, Convert.ToInt16(ctlValue));
                        break;
                    case TagDataType.S5Time:
                        //if (Controlvalue.GetType() == typeof(TimeSpan))
                        libnodave.putS5Timeat(buff, startpos, (TimeSpan)ctlValue);
                        //else
                        //    libnodave.putS5Timeat(buff, startpos, TimeSpan.Parse(Controlvalue.ToString()));
                        break;
                    case TagDataType.BCDWord:
                        libnodave.putBCD16at(buff, startpos, Convert.ToInt32(ctlValue));
                        break;
                    case TagDataType.BCDDWord:
                        libnodave.putBCD32at(buff, startpos, Convert.ToInt32(ctlValue));
                        break;
                    case TagDataType.Dint:
                        libnodave.putS32at(buff, startpos, Convert.ToInt32(ctlValue));
                        break;
                    case TagDataType.Dword:
                        libnodave.putU32at(buff, startpos, Convert.ToUInt32(ctlValue));
                        break;
                    case TagDataType.Float:
                        libnodave.putFloatat(buff, startpos, Convert.ToSingle(ctlValue));
                        break;
                    case TagDataType.DateTime:
                        //if (Controlvalue.GetType() == typeof(DateTime))
                        libnodave.putDateTimeat(buff, startpos, (DateTime)ctlValue);
                        //else
                        //    libnodave.putDateTimeat(buff, startpos, Convert.ToDateTime(Controlvalue));
                        break;
                    case TagDataType.Date:
                        //if (Controlvalue.GetType() == typeof(DateTime))
                        libnodave.putDateat(buff, startpos, (DateTime)ctlValue);
                        //else
                        //    libnodave.putDateTimeat(buff, startpos, Convert.ToDateTime(Controlvalue));
                        break;
                }
        }

        /// <summary>
        /// This Parses the PLCTag From a Byte Array
        /// this is used, when the Tag is not Read via my Functions, but the PLCTags are used as Wrapper
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="startpos"></param>
        public void ParseValueFromByteArray(byte[] buff, int startpos)
        {
            _readValueFromBuffer(buff, startpos);
        }

        internal virtual void _readValueFromBuffer(byte[] buff, int startpos)
        {
            switch (this.TagDataType)
            {
                case TagDataType.String:                    
                    {
                        var sb = new StringBuilder();
                        int size = ArraySize > buff[startpos + 1] ? buff[startpos+1] : ArraySize;
                        for (var n = 2; n < size+2; n++)
                            sb.Append((char) buff[n + startpos]);
                        _setValueProp = sb.ToString();
                    }
                    break;
                case TagDataType.CharArray:
                    {
                        var sb = new StringBuilder();
                        for (var n = 0; n < ((buff.Length - startpos) < ArraySize ? buff.Length - startpos : ArraySize); n++)
                            sb.Append((char)buff[n + startpos]);
                        _setValueProp = sb.ToString();
                    }
                    break;
                case TagDataType.ByteArray:
                    {
                        var val = new Byte[ArraySize];
                        Array.Copy(buff, startpos, val, 0, ArraySize);

                        /*
                        for (var n = 0; n < ArraySize; n++)
                            val[n] = buff[n + startpos];
                        */
                        _setValueProp = val;
                    }
                    break;

                case TagDataType.Bool:                   
                case TagDataType.Byte:                   
                case TagDataType.SByte:
                case TagDataType.Time:
                case TagDataType.Date:
                case TagDataType.TimeOfDay:
                case TagDataType.Word:
                case TagDataType.BCDByte:
                case TagDataType.Int:
                case TagDataType.S5Time:
                case TagDataType.BCDWord:
                case TagDataType.BCDDWord:
                case TagDataType.Dint:
                case TagDataType.Dword:
                case TagDataType.Float:
                case TagDataType.DateTime:
                    {
                        if (ArraySize<2)
                        {
                            switch (this.TagDataType)
                            {
                                case TagDataType.Bool:
                                    _setValueProp = libnodave.getBit(buff[startpos], BitAddress);
                                    break;
                                case TagDataType.Byte:
                                    _setValueProp = buff[startpos];
                                    break;
                                case TagDataType.SByte:
                                    _setValueProp = libnodave.getS8from(buff, startpos);
                                    break;
                                case TagDataType.Time:
                                    _setValueProp = libnodave.getTimefrom(buff, startpos);
                                    break;
                                case TagDataType.Date:
                                    _setValueProp = libnodave.getDatefrom(buff, startpos);
                                    break;
                                case TagDataType.TimeOfDay:
                                    _setValueProp = libnodave.getTimeOfDayfrom(buff, startpos);
                                    break;
                                case TagDataType.Word:
                                    _setValueProp = libnodave.getU16from(buff, startpos);
                                    break;
                                case TagDataType.BCDByte:
                                    _setValueProp = libnodave.getBCD8from(buff, startpos);
                                    break;
                                case TagDataType.Int:
                                    _setValueProp = libnodave.getS16from(buff, startpos);
                                    break;
                                case TagDataType.S5Time:
                                    _setValueProp = libnodave.getS5Timefrom(buff, startpos);
                                    break;
                                case TagDataType.BCDWord:
                                    _setValueProp = libnodave.getBCD16from(buff, startpos);
                                    break;
                                case TagDataType.BCDDWord:
                                    _setValueProp = libnodave.getBCD32from(buff, startpos);
                                    break;
                                case TagDataType.Dint:
                                    _setValueProp = libnodave.getS32from(buff, startpos);
                                    break;
                                case TagDataType.Dword:
                                    _setValueProp = libnodave.getU32from(buff, startpos);
                                    break;
                                case TagDataType.Float:
                                    _setValueProp = libnodave.getFloatfrom(buff, startpos);
                                    break;
                                case TagDataType.DateTime:
                                    _setValueProp = libnodave.getDateTimefrom(buff, startpos);
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.TagDataType)
                            {
                                case TagDataType.Bool:
                                    {
                                        var values = new List<bool>();
                                        var akBit = BitAddress;
                                        var akbyte = startpos;
                                        for (int n = 0; n < ArraySize; n++)
                                        {
                                            values.Add(libnodave.getBit(buff[akbyte], akBit));
                                            akBit++;
                                            if (akBit>7)
                                            {
                                                akBit = 0;
                                                akbyte++;
                                            }
                                        }

                                        _setValueProp = values.ToArray();
                                        break;
                                    }
                                case TagDataType.Byte:
                                    {
                                        var values = new List<Byte>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(buff[startpos + n * mSize]);
                                        _setValueProp = values.ToArray();
                                        break;
                                    }                                                                        
                                case TagDataType.SByte:
                                    {
                                        var values = new List<SByte>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++) 
                                            values.Add(libnodave.getS8from(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    }                                     
                                case TagDataType.Time:
                                    {
                                        var values = new List<TimeSpan>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getTimefrom(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    }                                     
                                case TagDataType.Date:
                                    {
                                        var values = new List<DateTime>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getDatefrom(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.TimeOfDay:
                                    {
                                        var values = new List<DateTime>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getTimeOfDayfrom(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.Word:
                                    {
                                        var values = new List<UInt16>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getU16from(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.BCDByte:
                                    {
                                        var values = new List<int>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getBCD8from(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.Int:
                                    {
                                        var values = new List<Int16>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getS16from(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.S5Time:
                                    {
                                        var values = new List<TimeSpan>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getS5Timefrom(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.BCDWord:
                                    {
                                        var values = new List<Int32>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getBCD16from(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.BCDDWord:
                                    {
                                        var values = new List<Int32>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getBCD32from(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.Dint:
                                    {
                                        var values = new List<Int32>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getS32from(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.Dword:
                                    {
                                        var values = new List<UInt32>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getU32from(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.Float:
                                    {
                                        var values = new List<float>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getFloatfrom(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                                case TagDataType.DateTime:
                                    {
                                        var values = new List<DateTime>();
                                        var mSize = _internalGetBaseTypeSize();
                                        for (int n = 0; n < ArraySize; n++)
                                            values.Add(libnodave.getDateTimefrom(buff, startpos + n * mSize));
                                        _setValueProp = values.ToArray();
                                        break;
                                    } 
                            }
                        }
                    }
                    break;                
            }

        }

        [XmlIgnore]
        public int ReadByteSize { get { return _internalGetSize(); } }

        internal virtual int _internalGetSize()
        {            
            switch (this.TagDataType)
            {
                case TagDataType.String:
                    return ArraySize + 2;                   
                case TagDataType.CharArray:
                case TagDataType.ByteArray:
                    return ArraySize;                    
                case TagDataType.Byte:
                case TagDataType.SByte:
                case TagDataType.BCDByte:                                       
                case TagDataType.Word:                
                case TagDataType.BCDWord:
                case TagDataType.Int:
                case TagDataType.S5Time:
                case TagDataType.Date:                                          
                case TagDataType.Dint:
                case TagDataType.Dword:
                case TagDataType.Time:                
                case TagDataType.TimeOfDay:
                case TagDataType.Float: 
                case TagDataType.BCDDWord:                    
                case TagDataType.DateTime:
                    return _internalGetBaseTypeSize() * ArraySize;
                case TagDataType.Bool:
                    {
                        var akbyte = 1;
                        var akBit = BitAddress;
                        for (int i = 0; i < ArraySize; i++)
                        {
                            if (akBit > 7)
                            {
                                akbyte++;
                                akBit = 0;
                            }
                            akBit++;
                        }
                        return akbyte;
                    }
            }
            return 0;
        }

        internal virtual int _internalGetBaseTypeSize()
        {
            switch (this.TagDataType)
            {
                case TagDataType.Byte:
                case TagDataType.SByte:
                case TagDataType.BCDByte:
                    return 1;
                case TagDataType.Word:
                case TagDataType.BCDWord:
                case TagDataType.Int:
                case TagDataType.S5Time:
                case TagDataType.Date:
                    return 2;
                case TagDataType.Dint:
                case TagDataType.Dword:
                case TagDataType.Time:
                case TagDataType.TimeOfDay:
                case TagDataType.Float:
                case TagDataType.BCDDWord:
                    return 4;
                case TagDataType.DateTime:
                    return 8;
            }
            return 0;
        }

        #region Events for Tag Checking 

        private bool _raiseValueChangedOnFirstRead = true;
        public bool RaiseValueChangedOnFirstRead
        {
            get { return _raiseValueChangedOnFirstRead; }
            set { _raiseValueChangedOnFirstRead = value; }
        }

        public delegate void ValueChangedEventHandler(PLCTag Sender, ValueChangedEventArgs e);
        public class ValueChangedEventArgs
        {
            public ValueChangedEventArgs(object OldValue, object NewValue)
            {
                this.OldValue = OldValue;
                this.NewValue = NewValue;
            }
            public object OldValue;
            public object NewValue;
        }
        public event ValueChangedEventHandler ValueChanged;

        public object Maximum { get; set; }
        public object Minimum { get; set; }

        public delegate void LimitReachedEventHandler(PLCTag Sender, LimitReachedEventArgs e);
        public class LimitReachedEventArgs
        {
            public LimitReachedEventArgs(object Value, object Limit)
            {
                this.Value = Value;
                this.Limit = Limit;
            }
            public object Value;
            public object Limit;
        }
        public event LimitReachedEventHandler MaximumReached;
        public event LimitReachedEventHandler MinimumReached;

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

    }                   
}
