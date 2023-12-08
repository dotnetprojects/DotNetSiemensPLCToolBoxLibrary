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

using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DataBlockRow : IDataRow, INotifyPropertyChanged
    {
        public static List<DataBlockRow> GetChildrowsAsList(DataBlockRow akRow)
        {
            var retVal = new List<DataBlockRow>();
            retVal.Add(akRow);
            if (akRow != null && akRow.Children != null && (akRow.DataType == S7DataRowType.STRUCT || akRow.DataType == S7DataRowType.UDT || akRow.DataType == S7DataRowType.FB))
                foreach (DataBlockRow plcDataRow in akRow.Children)
                    retVal.AddRange(GetChildrowsAsList(plcDataRow));
            return retVal;
        }

        public virtual List<IDataRow> Children { get; protected set; }

        protected internal S7DataRowType _datatype;

        public virtual S7DataRowType DataType
        {
            get { return _datatype; }
            set { _datatype = value; }
        }

        [JsonProperty("symbol", Order = 3)]
        public virtual string Name { get; set; }

        public virtual string FullName
        { get { return Parent == null ? string.Empty : (Parent.FullName + '.' + Name).Trim('.'); } }

        [JsonProperty("description", Order = 4)]
        public virtual string Comment { get; set; }

        public virtual string FullComment
        { get { return Parent == null ? string.Empty : (Parent.FullComment + '.' + Comment).Trim('.'); } }

        public virtual IDataRow Parent { get; set; }

        protected internal ByteBitAddress _BlockAddress;

        public virtual ByteBitAddress BlockAddress
        {
            get { return _BlockAddress; }
            set { _BlockAddress = value; }
        }

        public virtual Block CurrentBlock { get; protected set; }

        public virtual Block PlcBlock
        {
            get
            {
                if (this.Parent != null)
                    return this.Parent.PlcBlock;

                return CurrentBlock;
            }
        }

        protected int BaseBlockNumber
        {
            get
            {
                if (this.Parent != null)
                    return ((DataBlockRow)Parent).BaseBlockNumber;
                return PlcBlock.BlockNumber;
            }
        }

        public string StructuredName
        {
            get
            {
                if (Parent != null)
                {
                    if (string.IsNullOrEmpty((Parent).StructuredName)) return Name;
                    return (Parent).StructuredName + "." + Name;
                }
                return "";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsArray { get; set; }

        public List<int> ArrayStart { get; set; }

        public List<int> ArrayStop { get; set; }

        public int GetArrayLines()
        {
            int arrcnt = 1;
            if (ArrayStart != null)
                for (int n = 0; n < ArrayStart.Count; n++)
                {
                    arrcnt *= ArrayStop[n] - ArrayStart[n] + 1;
                }
            return arrcnt;
        }

        public int DataTypeBlockNumber { get; set; } //When the Type is SFB, FB or UDT, this contains the Number!

        public int StringSize { get; set; } //Only Relevant for String

        [JsonProperty("datatype", Order = 2)]
        public virtual string DataTypeAsString
        {
            get
            {
                string retVal = "";

                //For arrays add the Declared dimension
                if (IsArray)
                {
                    retVal += "ARRAY [";
                    for (int n = 0; n < ArrayStart.Count; n++)
                    {
                        retVal += ArrayStart[n].ToString() + ".." + ArrayStop[n].ToString();
                        if (n < ArrayStart.Count - 1)
                            retVal += ",";
                    }
                    retVal += "] OF ";
                }

                //if the row is an MultiInstance Static FB or SFB, then mention only the Block Number
                //in this case omit the Datatype entirely and just write the referenced Block number
                if (DataType == S7DataRowType.MultiInst_FB)
                    retVal += "FB" + DataTypeBlockNumber.ToString();
                else if (DataType == S7DataRowType.MultiInst_SFB)
                    retVal += "SFB" + DataTypeBlockNumber.ToString();
                else
                    retVal += DataType.ToString();

                //in case of Block types, add the actual Block number as well
                if (DataType == S7DataRowType.FB || DataType == S7DataRowType.UDT || DataType == S7DataRowType.SFB)
                    retVal += DataTypeBlockNumber.ToString();

                //for strings mention the String Declaration size
                if (DataType == S7DataRowType.STRING)
                    retVal += "[" + StringSize.ToString() + "]";

                return retVal;
            }
        }
    }
}