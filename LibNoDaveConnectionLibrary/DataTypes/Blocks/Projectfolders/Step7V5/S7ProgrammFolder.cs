using System;
using System.Collections.Generic;
using System.Text;

namespace LibNoDaveConnectionLibrary.DataTypes.Step7Project
{
    public class S7ProgrammFolder : Step7ProjectFolder
    {
        internal int _linkfileoffset;
        public SymbolTable SymbolTable { get; set; }
        public BlocksOfflineFolder BlocksOfflineFolder { get; set; }
    }
}
