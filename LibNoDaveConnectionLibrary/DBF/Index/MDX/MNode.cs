using System;
using System.IO;
using DotNetSiemensPLCToolBoxLibrary.DBF.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Index.MDX {
    public class MNode : Node
    {
        private int prev_page = 0;

        public MNode(int keys_in, int key_size, dBaseType keyType, int rn, bool iBranch)
            : base(keys_in, key_size, keyType, rn, iBranch)
        {

        }

        /// <summary>
        /// Read node from the parent MDX Entry
        /// </summary>
        /// <param name="Reader">The BinaryReader that contains the MDXFile Stream</param>
        public void Read(BinaryReader Reader)
        {
            int i, j, k;
            long longrecn = intRecordNumber;
            Reader.BaseStream.Position = (longrecn*512);
            intKeysInThisNode = Reader.ReadInt32();
            prev_page = Reader.ReadInt32();
            boolBranch = prev_page == 0 ? false : true;

            byte[] b = new byte[12];
            for (i = 0; i < intGeneralKeysPerNode; i++)
            {
                key_record_number[i] = Reader.ReadInt32();
                if (objKeyType == dBaseType.F)
                {
                    Reader.Read(b, 0, b.Length);
                    if (i < intKeysInThisNode)
                    {
                        key_expression[i] = new NodeKey(dBaseConverter.F_ToDouble(b));
                    }
                    else
                    {
                        key_expression[i] = new NodeKey(0.0d);
                    }
                }
                else if (objKeyType == dBaseType.N)
                {
                    //Key is a double number
                    key_expression[i] = new NodeKey(Convert.ToDouble(Reader.ReadInt64()));
                }
                else
                {
                    key_buffer = Reader.ReadBytes(intKeyExpressionSize);
                    for (k = 0; k < intKeyExpressionSize && key_buffer[k] != 0; k++) ;
                    key_expression[i] = new NodeKey(dBaseConverter.C_ToString(key_buffer));
                }


                j = intKeyExpressionSize%4;
                if (j > 0) j = 4 - j;
                for (k = 0; k < j; k++)
                    Reader.ReadByte();
            } // for i

            key_record_number[i] = Reader.ReadInt32();

            boolBranch = key_record_number[intKeysInThisNode] > 0 ? true : false;

        }

        #region SET / GET

        /// <summary>
        /// Sets / Returns the next lower level of the b-tree structrue, containing the nodes
        /// </summary>
        public int LowerLevel
        {
            get
            {
                if (boolBranch)
                {
                    return key_record_number[intPosition];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (boolBranch)
                {
                    key_record_number[intPosition] = value;
                }
            }
        }

        /// <summary>
        /// Sets / Returns the previous node page of the index
        /// </summary>
        public int PreviousPage
        {
            get { return prev_page; }
            set { this.prev_page = value; }
        }

        #endregion

    }
}
