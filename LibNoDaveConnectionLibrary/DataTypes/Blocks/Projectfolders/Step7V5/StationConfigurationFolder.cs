using System;
using System.Collections.Generic;
using System.Text;

namespace LibNoDaveConnectionLibrary.DataTypes.Step7Project
{
    public class StationConfigurationFolder : Step7ProjectFolder
    {
        internal int UnitID;
        public PLCType StationType { get; set; } 
    }
}
