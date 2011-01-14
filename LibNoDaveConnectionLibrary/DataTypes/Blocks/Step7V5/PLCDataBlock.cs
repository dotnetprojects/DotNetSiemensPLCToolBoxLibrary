/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 WPFToolboxForSiemensPLCs is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 WPFToolboxForSiemensPLCs is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    [Serializable()]
    public class PLCDataBlock : PLCBlock, IDataBlock
    {
        public int FBNumber { get; set;}  //If it is a Instance DB
        public bool IsInstanceDB { get; set; }
        public PLCDataRow Structure { get; set; }

        /*
        public List<PLCDataRow> GetRowsAsList()
        {
            List<PLCDataRow> retVal = new List<PLCDataRow>();
            if (Structure != null && Structure.Children != null && Structure.DataType == PLCDataRowType.STRUCT)
                foreach (PLCDataRow plcDataRow in Structure.Children)
                    retVal.AddRange(_GetRowsAsList(plcDataRow));
            return retVal;
        }
        */

        /*
        public List<PLCDataRow> GetRowsAsArrayExpandedList()
        {
            return GetRowsAsArrayExpandedList(new PLCDataBlockExpandOptions());
        }

        public List<PLCDataRow> GetRowsAsArrayExpandedList(PLCDataBlockExpandOptions myExpOpt)
        {
            List<PLCDataRow> retVal = new List<PLCDataRow>();
            if (Structure != null && Structure.Children != null && Structure.DataType == PLCDataRowType.STRUCT)
                foreach (PLCDataRow plcDataRow in GetArrayExpandedStructure().Children)
                    retVal.AddRange(_GetRowsAsList(plcDataRow));
            return retVal;
        }
        */
        /*
        public List<LibNoDaveValue> GetLibnoDaveValues()
        {
            List<LibNoDaveValue> retVal = new List<LibNoDaveValue>();
            var tmp = GetRowsAsList();
            foreach (PLCDataRow plcDataRow in tmp)
            {
                if (plcDataRow.LibNoDaveValue != null)
                    retVal.Add(plcDataRow.LibNoDaveValue);
            }
            return retVal;
        }
        */
        /// <summary>
        /// With this function you get the Structure with expanden Arrays!
        /// </summary>
        /// <returns></returns>
        public PLCDataRow GetArrayExpandedStructure(PLCDataBlockExpandOptions myExpOpt)
        {
            return Structure._GetExpandedChlidren(myExpOpt)[0];
        }

        public PLCDataRow GetArrayExpandedStructure()
        {
            return GetArrayExpandedStructure(new PLCDataBlockExpandOptions());
        }

        /*
        internal List<PLCDataRow> _GetRowsAsList(PLCDataRow currRow)
        {
            List<PLCDataRow> retVal = new List<PLCDataRow>();
            retVal.Add(currRow);
            if (currRow.Children != null && currRow.DataType == PLCDataRowType.STRUCT)
                foreach (PLCDataRow plcDataRow in currRow.Children)
                    retVal.AddRange(_GetRowsAsList(plcDataRow));
            return retVal;
        }
        */

        public override string ToString()
        {
            string retVal = "";
            if (this.BlockType == PLCBlockType.UDT)
                retVal += "UDT";
            else
                retVal += "DB";
            retVal += BlockNumber.ToString() + Environment.NewLine;
            if (Structure != null)
                retVal += Structure.ToString();
            return retVal;
        }       
    }
}
