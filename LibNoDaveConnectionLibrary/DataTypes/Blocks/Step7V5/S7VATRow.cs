using DotNetSiemensPLCToolBoxLibrary.Communication;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    [JsonObject(MemberSerialization.OptIn)]
    [Serializable()]
    public class S7VATRow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private PLCTag _libNoDaveValue;
        private string _comment;

        [JsonProperty(Order = 1)]
        public string address
        {
            get
            {
                if (_libNoDaveValue != null)
                {
                    return _libNoDaveValue.S7FormatAddress;
                }
                return null;
            }
        }

        [JsonProperty(Order = 2)]
        public string datatype
        {
            get
            {
                if (_libNoDaveValue != null)
                {
                    return _libNoDaveValue.DataTypeStringFormat;
                }
                return null;
            }
        }

        public PLCTag LibNoDaveValue
        {
            get { return _libNoDaveValue; }
            set { _libNoDaveValue = value; NotifyPropertyChanged("LibNoDaveValue"); }
        }

        #region Wrapper for PLCTag Properties...

        [XmlIgnore]
        public virtual string S7FormatAddress
        {
            get
            {
                if (_libNoDaveValue != null)
                    return _libNoDaveValue.S7FormatAddress;
                return null;
            }
            set
            {
                if (value == null || value.ToString().Trim() == "")
                    LibNoDaveValue = null;
                else if (value.StartsWith("//"))
                {
                    Comment = value.Substring(2);
                }
                else
                {
                    if (_libNoDaveValue == null)
                        LibNoDaveValue = new PLCTag();
                    _libNoDaveValue.S7FormatAddress = value;
                }
                NotifyInternalPLCTagPropertyChanges();
            }
        }

        [XmlIgnore]
        public virtual int ArraySize
        {
            get
            {
                if (_libNoDaveValue != null)
                    return _libNoDaveValue.ArraySize;
                return 0;
            }
            set
            {
                if (_libNoDaveValue == null) LibNoDaveValue = new PLCTag();
                _libNoDaveValue.ArraySize = value;
                NotifyInternalPLCTagPropertyChanges();
            }
        }

        [XmlIgnore]
        public TagDataType? TagDataType
        {
            get
            {
                if (_libNoDaveValue != null)
                    return _libNoDaveValue.TagDataType;
                return null;
            }
            set
            {
                if (value == null)
                    LibNoDaveValue = null;
                else
                {
                    if (_libNoDaveValue == null)
                        LibNoDaveValue = new PLCTag();
                    _libNoDaveValue.TagDataType = value.Value;
                }
                NotifyInternalPLCTagPropertyChanges();
            }
        }

        [XmlIgnore]
        public string DataTypeStringFormat
        {
            get
            {
                if (_libNoDaveValue != null)
                    return _libNoDaveValue.DataTypeStringFormat;
                return null;
            }
            set
            {
                if (value == null)
                    LibNoDaveValue = null;
                else
                {
                    if (_libNoDaveValue == null)
                        LibNoDaveValue = new PLCTag();
                    _libNoDaveValue.DataTypeStringFormat = value;
                }
                NotifyInternalPLCTagPropertyChanges();
            }
        }

        [XmlIgnore]
        public string ControlValueAsString
        {
            get
            {
                if (_libNoDaveValue != null)
                    return _libNoDaveValue.ControlValueAsString;
                return null;
            }
            set
            {
                if (_libNoDaveValue != null)
                    _libNoDaveValue.ControlValueAsString = value;

                NotifyInternalPLCTagPropertyChanges();
            }
        }

        protected virtual void NotifyInternalPLCTagPropertyChanges()
        {
            NotifyPropertyChanged("S7FormatAddress");
            NotifyPropertyChanged("TagDataType");
            NotifyPropertyChanged("DataTypeStringFormat");
            NotifyPropertyChanged("ControlValueAsString");
            NotifyPropertyChanged("ArraySize");
        }

        #endregion Wrapper for PLCTag Properties...

        [JsonProperty("description", Order = 4)]
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; NotifyPropertyChanged("Comment"); }
        }

        public override string ToString()
        {
            if (_libNoDaveValue == null)
                return null;
            return LibNoDaveValue.S7FormatAddress;
        }
    }
}