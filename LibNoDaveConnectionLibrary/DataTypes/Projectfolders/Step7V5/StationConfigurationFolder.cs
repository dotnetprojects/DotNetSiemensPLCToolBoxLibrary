using DotNetSiemensPLCToolBoxLibrary.DataTypes.Hardware.Step7V5;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class StationConfigurationFolder : Step7ProjectFolder
    {
        internal int UnitID;

        internal int ObjTyp;

        public PLCType StationType { get; set; }

        private List<MasterSystem> _masterSystems = new List<MasterSystem>();

        public List<MasterSystem> MasterSystems
        {
            get { return _masterSystems; }
            set { _masterSystems = value; }
        }
    }
}