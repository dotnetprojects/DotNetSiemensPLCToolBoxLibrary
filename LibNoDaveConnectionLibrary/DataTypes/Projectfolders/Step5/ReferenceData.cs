using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5
{
    public class ReferenceData : Step5ProjectFolder
    {
        public String Folder { get; set; }

        private Dictionary<string, ReferenceDataEntry> operandIndexList = new Dictionary<string, ReferenceDataEntry>();

        private List<ReferenceDataEntry> _ReferenceDataEntrys;
        public List<ReferenceDataEntry> ReferenceDataEntrys
        {
            get
            {
                return _ReferenceDataEntrys;
            }
            set { _ReferenceDataEntrys = value; }
        }

        internal bool showDeleted { get; set; }

        public ReferenceDataEntry GetEntryFromOperand(string operand)
        {
            string tmpname = operand.Trim().ToUpper();
            ReferenceDataEntry retval = null;
            operandIndexList.TryGetValue(tmpname, out retval);
            return retval;
        }

        public ReferenceData()
        {
            Name = "ReferenzDaten";

            //Load Referencedata in a Background Thread.
            Thread trd = new Thread(new ThreadStart(LoadReferenceData));
            trd.Start();
        }
        
        private void LoadReferenceData()
        {
            _ReferenceDataEntrys = new List<ReferenceDataEntry>();            
        }                
    }
}
