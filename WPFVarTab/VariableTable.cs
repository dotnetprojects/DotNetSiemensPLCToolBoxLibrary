using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace WPFVarTab
{
    [Serializable]
    public class VariableTable
    {
        public int Version { get; set; }

        public ObservableCollection<VarTabRowWithConnection> VarTabRows { get; set; }

        public ObservableCollection<PLCConnection> Connections { get; set; }

        public Dictionary<PLCConnection, string> SymbolTable { get; set; }
    }
}
