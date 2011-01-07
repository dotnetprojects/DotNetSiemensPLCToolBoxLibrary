using System;
using System.Collections.Generic;
using System.Text;
using LibNoDaveConnectionLibrary.DataTypes.Projects;
using LibNoDaveConnectionLibrary.DataTypes.Step7Project;

namespace LibNoDaveConnectionLibrary.DataTypes.Blocks
{
    public class ProjectBlockInfo
    {
        internal int id;

        public ProjectFolder ParentFolder { get; set; }

        virtual public string BlockName { get; set;}

        public int BlockNumber { get; set; }
        public PLCBlockType BlockType { get; set; }

        public bool Deleted { get; set; }
        
        public string Symbol { get; set; }
       
        public override string ToString()
        {
            string retVal = "";           
            if (Deleted)
                retVal += "$$_";
            if (!string.IsNullOrEmpty(Symbol))
                retVal += Symbol + " (" + BlockName + ")";
            else
                retVal += BlockName;
            return retVal;
        }
    }
}
