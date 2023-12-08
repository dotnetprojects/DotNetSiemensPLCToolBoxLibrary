using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using System.ComponentModel;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    public class TIADataRow : TiaAndSTep7DataBlockRow, INotifyPropertyChanged
    {
        public Block DataTypeBlock { get; set; } //When the Type is SFB, FB or UDT, this contains the Block!

        public TIADataRow(string name, S7DataRowType datatype, Block block)
        {
            this.Name = name;
            this.DataType = datatype;
            this.CurrentBlock = block;
        }

        public bool isRootBlock { get; set; }

        public override TiaAndSTep7DataBlockRow DeepCopy()
        {
            var newRow = new TIADataRow(this.Name, this.DataType, this.PlcBlock);
            newRow.Parent = this.Parent;
            newRow.ArrayStart = this.ArrayStart;
            newRow.ArrayStop = this.ArrayStop;
            newRow.IsArray = this.IsArray;
            newRow.WasFirstInArray = this.WasFirstInArray;
            newRow.WasArray = this.WasArray;
            newRow.WasNextHigherIndex = this.WasNextHigherIndex;
            newRow.DataTypeBlock = DataTypeBlock;

            newRow.Comment = this.Comment;
            newRow.DataTypeBlockNumber = this.DataTypeBlockNumber;
            newRow.ReadOnly = this.ReadOnly;
            newRow.StartValue = this.StartValue;
            newRow.StringSize = this.StringSize;
            newRow.isInOut = this.isInOut;
            newRow.isRootBlock = this.isRootBlock;
            newRow.Value = this.Value;

            //newRow.Attributes = this.Attributes;
            //newRow.TimeStampConflict = this.TimeStampConflict;

            if (Children != null)
                foreach (TIADataRow plcDataRow in Children)
                {
                    var copy = plcDataRow.DeepCopy();
                    copy.Parent = newRow;
                    newRow.Add(copy);
                }

            return newRow;
        }

        private ByteBitAddress _parentOldAddress;

        internal override ByteBitAddress FillBlockAddresses(ByteBitAddress startAddr)
        {
            if (isRootBlock && this.Name == "TEMP")
            {
                _BlockAddress = new ByteBitAddress(0, 0);
                startAddr = new ByteBitAddress(0, 0);
            }

            ByteBitAddress akAddr = new ByteBitAddress(startAddr);
            if (Parent != null && startAddr == null)
                ((TIADataRow)Parent).FillBlockAddresses(null);
            else
            {
                //Create a function wich fills in the Block address for all Subitems...
                if (Children != null || _datatype != S7DataRowType.STRUCT)
                {
                    if (akAddr.BitAddress != 0)
                        akAddr.ByteAddress++;
                    if (akAddr.ByteAddress % 2 != 0)
                        akAddr.ByteAddress++;

                    bool lastRowWasArrayOrStruct = false;

                    //int structlen = 0;
                    foreach (TIADataRow plcDataRow in Children)
                    {
                        if (akAddr.BitAddress != 0 && plcDataRow._datatype == S7DataRowType.BOOL && plcDataRow.WasArray && !plcDataRow.WasFirstInArray && plcDataRow.WasNextHigherIndex)
                        {
                            akAddr.BitAddress = 0;
                            akAddr.ByteAddress++;
                        }
                        else if (akAddr.BitAddress != 0 && (plcDataRow._datatype != S7DataRowType.BOOL || plcDataRow.IsArray || plcDataRow.WasFirstInArray || (lastRowWasArrayOrStruct && !plcDataRow.WasArray && !plcDataRow.WasFirstInArray)))
                        {
                            akAddr.BitAddress = 0;
                            akAddr.ByteAddress++;
                        }

                        if (akAddr.ByteAddress % 2 != 0 && ((plcDataRow._datatype != S7DataRowType.BOOL && plcDataRow._datatype != S7DataRowType.BYTE && plcDataRow._datatype != S7DataRowType.CHAR) || plcDataRow.IsArray || plcDataRow.WasFirstInArray || (lastRowWasArrayOrStruct && !plcDataRow.WasArray && !plcDataRow.WasFirstInArray)))
                            if (!(this.PlcBlock is Step5.S5DataBlock))
                            {
                                akAddr.ByteAddress++;
                            }

                        if (plcDataRow.Children != null && plcDataRow.Children.Count > 0)
                        {
                            plcDataRow._BlockAddress = new ByteBitAddress(akAddr);

                            var useAddr = akAddr;
                            if (plcDataRow.Parent != null && ((TIADataRow)plcDataRow.Parent).isInOut)
                                useAddr = new ByteBitAddress(0, 0);
                            plcDataRow.FillBlockAddresses(useAddr);

                            //Struct or UDT are Handeled as Pointer in IN_OUT so only Increase about 6 Byte
                            //if (plcDataRow.Parent != null && ((S7DataRow) plcDataRow.Parent).isInOut)
                            akAddr.ByteAddress += plcDataRow.ByteLength;
                            //if (!plcDataRow.IsArray)
                            //    akAddr = plcDataRow.FillBlockAddresses(akAddr);
                            //else
                            //    akAddr = plcDataRow.FillBlockAddresses(akAddr);
                            if (akAddr.BitAddress != 0)
                            {
                                akAddr.BitAddress = 0;
                                akAddr.ByteAddress++;
                            }
                            if (akAddr.ByteAddress % 2 != 0)
                                akAddr.ByteAddress++;

                            plcDataRow._NextBlockAddress = new ByteBitAddress(akAddr);
                        }
                        else
                        {
                            plcDataRow._BlockAddress = new ByteBitAddress(akAddr);
                            if (plcDataRow._datatype == S7DataRowType.BOOL && !plcDataRow.IsArray)
                                akAddr = Helper.GetNextBitAddress(akAddr);
                            else
                            {
                                akAddr.BitAddress = 0;
                                akAddr.ByteAddress += plcDataRow.ByteLength;
                            }
                            plcDataRow._NextBlockAddress = new ByteBitAddress(akAddr);
                        }

                        //structlen += plcDataRow.ByteLength;

                        lastRowWasArrayOrStruct = plcDataRow.WasArray;
                    }
                    //this. = structlen;
                }
            }
            return akAddr;
        }

        /// </summary>
        internal override void ClearBlockAddress()
        {
            if (_BlockAddress != null || _parentOldAddress != null)
            {
                _BlockAddress = null;
                _parentOldAddress = null;
                //_structureLength = null;

                if (Children != null)
                    foreach (TIADataRow plcDataRow in Children)
                    {
                        plcDataRow.ClearBlockAddress();
                    }
                if (Parent != null)
                    ((TIADataRow)Parent).ClearBlockAddress();
            }
        }

        public override ByteBitAddress BlockAddress
        {
            get
            {
                if (_BlockAddress == null)
                    FillBlockAddresses(null);
                if (this.PlcBlock is Step5.S5DataBlock && _BlockAddress != null)
                    return new ByteBitAddress(_BlockAddress.ByteAddress / 2, _BlockAddress.BitAddress);
                return new ByteBitAddress(_BlockAddress);
            }
        }

        public override string DataTypeAsString
        {
            get
            {
                string retVal = "";
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

                if (DataTypeBlock != null)
                    retVal += "\"" + DataTypeBlock.Name + "\"";
                else
                {
                    retVal += DataType.ToString();
                }
                if (DataType == S7DataRowType.STRING)
                    retVal += "[" + StringSize.ToString() + "]";

                return retVal;
            }
        }

        //        public uint LocalIdentifier
        //        {
        //            get
        //            {
        //                var lidNode = node.SelectSingleNode("attribSet[@id='" + tiaProject.asId2Names.First(itm => itm.Value == "Siemens.Automation.DomainModel.ITagAddress").Key + "']/attrib[@name='LocalIdentifier']");
        //                if (lidNode != null) return Convert.ToUInt32(lidNode.InnerText);
        //                return 0;
        //            }
        //        }

        //        public string SymbolicVisuAccessKey
        //        {
        //            get
        //            {
        //                var crc = TiaCrcHelper.getcrc(Encoding.ASCII.GetBytes(Name));
        //                return "8a0e" + block.BlockNumber.ToString("X").PadLeft(4, '0') + crc.ToString("X").PadLeft(8, '0') + "4" + LocalIdentifier.ToString("X").PadLeft(7, '0');
        //            }
        //        }
    }
}