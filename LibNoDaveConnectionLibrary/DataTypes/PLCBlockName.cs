using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    /// <summary>
    /// Represents an PLC Block type and name
    /// </summary>
    public class PLCBlockName
    {
        /// <summary>
        /// The type of Block
        /// </summary>
        public PLCBlockType BlockType { get; set; }

        /// <summary>
        /// The number of the Block
        /// </summary>
        public int BlockNumber { get; set; }

        public PLCBlockName(PLCBlockType blockType, int blockNumber)
        {
            BlockType = blockType;
            BlockNumber = blockNumber;
        }

        public PLCBlockName(string blockName)
        {
            string tmp = blockName.ToUpper().Trim().Replace(" ", "");
            string block = "";
            int nr = 0;

            //SDB's are the only block types with 3 letter digits
            if (tmp.StartsWith("SDB"))
            {
                block = tmp.Substring(0, 3);
                nr = Int32.Parse(tmp.Substring(3));
            }
            else
            {
                block = tmp.Substring(0, 2);
                nr = Int32.Parse(tmp.Substring(2));
            }
            DataTypes.PLCBlockType blk = DataTypes.PLCBlockType.AllBlocks; //Preset with default (in this case invalid) block type

            switch (block)
            {
                case "FC":
                    blk = DataTypes.PLCBlockType.FC;
                    break;

                case "FB":
                    blk = DataTypes.PLCBlockType.FB;
                    break;

                case "DB":
                    blk = DataTypes.PLCBlockType.DB;
                    break;

                case "OB":
                    blk = DataTypes.PLCBlockType.OB;
                    break;

                case "SDB":
                    blk = DataTypes.PLCBlockType.SDB;
                    break;
            }
            if (blk == PLCBlockType.AllBlocks) throw new NotSupportedException("Unsupported Block type!");
            if (nr < 0) throw new NotSupportedException("Unsupported Block number!");
            if (nr > 99999) throw new NotSupportedException("Unsupported Block number!"); //Maximum number for requests to S7 plcs is usually 5 digits

            BlockNumber = nr;
            BlockType = blk;
        }

        public PLCBlockName()
        {
            BlockType = PLCBlockType.DB;
            BlockNumber = 1;
        }

        public override string ToString()
        {
            return BlockType.ToString() + BlockNumber.ToString();
        }

        public PLCBlockName Parse(string blockName)
        {
            return new PLCBlockName(blockName);
        }

        //TODO: Implement better version, without exception
        public bool TryParse(string blockName, out PLCBlockName plcBlockName)
        {
            try
            {
                plcBlockName = new PLCBlockName(blockName);
                return true;
            }
            catch
            {
                Console.WriteLine("1 PLCBlockName.cs threw exception");
                plcBlockName = null;
                return false;
            }
        }

        //private bool isValidBlockType(PLCBlockType blockType)
        //{
        //    switch (blockType)
        //    {
        //        case PLCBlockType.DB:
        //        case PLCBlockType.FB:
        //        case PLCBlockType.FC:
        //        case PLCBlockType.OB:
        //        case PLCBlockType.SDB:
        //        case PLCBlockType.SFB:
        //        case PLCBlockType.SFC:
        //            return true;
        //        default:
        //            return false;
        //    }

        //}
    }
}