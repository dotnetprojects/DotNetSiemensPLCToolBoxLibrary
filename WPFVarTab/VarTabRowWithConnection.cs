using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Serialization;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace WPFVarTab
{
    [Serializable]
    public class VarTabRowWithConnection : S7VATRow
    {
        public UIElement ValueAsUI
        {
            get
            {
                if (LibNoDaveValue != null)
                {
                    if (LibNoDaveValue.LibNoDaveDataType == DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Bool)
                    {
                        var bind = new Binding();
                        bind.Path = new PropertyPath("LibNoDaveValue.Value");
                        bind.Mode = BindingMode.TwoWay;
                        var ctl = new CheckBox();
                        ctl.Click += ctl_Checked_Changed;
                        ctl.SetBinding(CheckBox.IsCheckedProperty, bind);
                        return ctl;
                    }
                    else
                    {
                        var bind = new Binding();
                        bind.Path = new PropertyPath("LibNoDaveValue.ValueAsString");
                        var ctl = new TextBlock();
                        ctl.SetBinding(TextBlock.TextProperty, bind);
                        return ctl;
                    }
                }
                return null;
            }    
        }

        void ctl_Checked_Changed(object sender, RoutedEventArgs e)
        {
            if (Connection != null && Connected.HasValue && Connected.Value)
            {
                Connection.WriteValue(new PLCTag(LibNoDaveValue) {Controlvalue = ((CheckBox) sender).IsChecked.Value});             
            }
            if (LibNoDaveValue != null)
            {
                LibNoDaveValue.ClearValue();
            }
        }

        public override string S7FormatAddress
        {
            get
            {
                return base.S7FormatAddress;
            }
            set
            {
                base.S7FormatAddress = value;
                MainWindow.RefreshSymbol(this);
                RaiseConnectedChanged();
                NotifyPropertyChanged("ValueAsUI");
            }
        }

        protected override void NotifyInternalPLCTagPropertyChanges()
        {
            base.NotifyInternalPLCTagPropertyChanges();
            NotifyPropertyChanged("ValueAsUI");
        }
        
        public override int ArraySize
        {
            get
            {
                return base.ArraySize;
            }
            set
            {
                base.ArraySize = value;
                MainWindow.RefreshSymbol(this);
                RaiseConnectedChanged();
            }
        }

        public VarTabRowWithConnection GetNextRow()
        {
            var rw = new VarTabRowWithConnection();
            rw.LibNoDaveValue = new PLCTag(LibNoDaveValue);
            rw.ConnectionName = ConnectionName;
            
            if (rw.LibNoDaveValue.LibNoDaveDataType == DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Bool)
            {
                if (rw.LibNoDaveValue.BitAddress < 7)
                    rw.LibNoDaveValue.BitAddress++;
                else
                {
                    rw.LibNoDaveValue.BitAddress = 0;
                    rw.LibNoDaveValue.ByteAddress++;
                }
            }
            else
            {
                rw.LibNoDaveValue.ByteAddress += rw.LibNoDaveValue.ReadByteSize;
            }

            return rw;
        }

        public VarTabRowWithConnection()
        { }

        public VarTabRowWithConnection(S7VATRow baseRow)
        {
            this.LibNoDaveValue = baseRow.LibNoDaveValue;
            this.Comment = baseRow.Comment;
        }

        private string _connectionName;
        private bool _connected;

        [XmlIgnore]
        public bool? Connected
        {
            get
            {
                if (string.IsNullOrEmpty(S7FormatAddress))
                    return null;

                if (Connection != null)
                    return Connection.Connected;
                return false;
            }
        }

        private PLCConnection oldConn;
        private string _symbol;

        [XmlIgnore]
       public PLCConnection Connection
        {
            get
            {
                if (oldConn != null)
                {
                    oldConn.PropertyChanged -= conn_PropertyChanged;
                    oldConn = null;
                }

                PLCConnection conn = null;
                if (string.IsNullOrEmpty(ConnectionName) && MainWindow.DefaultConnectionStatic != null &&
                    MainWindow.ConnectionDictionary.ContainsKey(MainWindow.DefaultConnectionStatic))
                    conn = MainWindow.ConnectionDictionary[MainWindow.DefaultConnectionStatic];
                else if (!string.IsNullOrEmpty(ConnectionName) &&
                         MainWindow.ConnectionDictionary.ContainsKey(ConnectionName))
                    conn = MainWindow.ConnectionDictionary[ConnectionName];

                if (conn != null)
                {
                    oldConn = conn;
                    conn.PropertyChanged += conn_PropertyChanged;
                }
                return conn;
            }
        }

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; NotifyPropertyChanged("Symbol"); }
        }

        void conn_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Connected")
                NotifyPropertyChanged("Connected");

        }

        public void RaiseConnectedChanged()
        {
            NotifyPropertyChanged("Connected");
        }

        public string ConnectionName
        {
            get { return _connectionName; }
            set
            {
                _connectionName = value;
                NotifyPropertyChanged("ConnectionName");
                NotifyPropertyChanged("Connection");
                NotifyPropertyChanged("Online");

                MainWindow.RefreshSymbol(this);
            }
        }
    }
}
