using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace WPFVarTab
{
    public class VarTabRowWithConnection : S7VATRow
    {
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
        public bool Connected
        {
            get
            {
                if (Connection != null)
                    return Connection.Connected;
                return false;
            }
        }

        private PLCConnection oldConn;

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
            }
        }
    }
}
