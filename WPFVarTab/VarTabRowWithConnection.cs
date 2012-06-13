using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private bool _online;
        private string _connectionName;

        public PLCConnection Connection
        {
            get
            {
                if (MainWindow.ConnectionDictionary.ContainsKey(ConnectionName))
                    return MainWindow.ConnectionDictionary[ConnectionName];

                return null;
            }
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

        public bool Online
        {
            get { return _online; }
            set { _online = value; NotifyPropertyChanged("Online"); }
        }
    }
}
