using System;
using System.ComponentModel;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    [Serializable()]
    public class S7VATRow: INotifyPropertyChanged
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

        public PLCTag LibNoDaveValue
        {
            get { return _libNoDaveValue; }
            set { _libNoDaveValue = value; NotifyPropertyChanged("LibNoDaveValue"); }
        }

        #region Wrapper for PLCTag Properties...
        public string S7FormatAddress
        {
            get
            {
                if (_libNoDaveValue != null)
                    return _libNoDaveValue.S7FormatAddress;
                return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
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

        public TagDataType? LibNoDaveDataType
        {
            get
            {
                if (_libNoDaveValue != null)
                    return _libNoDaveValue.LibNoDaveDataType;
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
                    _libNoDaveValue.LibNoDaveDataType = value.Value;
                }
                NotifyInternalPLCTagPropertyChanges();
            }
        }

        public TagDisplayDataType? DataTypeStringFormat
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
                    _libNoDaveValue.DataTypeStringFormat = value.Value;
                }
                NotifyInternalPLCTagPropertyChanges();
            }
        }

        private void NotifyInternalPLCTagPropertyChanges()
        {
            NotifyPropertyChanged("S7FormatAddress");
            NotifyPropertyChanged("LibNoDaveDataType");
            NotifyPropertyChanged("DataTypeStringFormat");
        }

        #endregion

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
