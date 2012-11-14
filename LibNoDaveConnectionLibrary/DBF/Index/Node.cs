using System;
using System.IO;
using DotNetSiemensPLCToolBoxLibrary.DBF.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Index {
    public class Node : ICloneable
    {
        protected int intPosition = 0;
        protected int intGeneralKeysPerNode = 0;
        protected int intKeysInThisNode = 0;
        protected int intKeyExpressionSize = 0;
        protected int intRecordNumber = 0;
        protected dBaseType objKeyType = dBaseType.C;

        protected byte[] key_buffer;

        protected int[] lower_level;
        protected int[] key_record_number;
        protected NodeKey[] key_expression;

        protected Node objPreviousNode = null;
        protected Node objNextNode = null;
        protected bool boolBranch = false;

        public Node(int keys_in, int key_size, dBaseType keyType, int rn, bool iBranch)
        {
            intGeneralKeysPerNode = keys_in;
            intKeyExpressionSize = key_size;
            intKeysInThisNode = 0;
            this.objKeyType = keyType;
            intRecordNumber = rn;
            boolBranch = iBranch;

            key_buffer = new byte[key_size];
            key_expression = new NodeKey[keys_in + 2];

            key_record_number = new int[keys_in + 2];
            lower_level = new int[keys_in + 2];

            for (int i = 0; i < keys_in; i++)
            {
                key_record_number[i] = 0;
                lower_level[i] = 0;
            }

            objPreviousNode = null;
            objNextNode = null;

        }

        public void Read(BinaryReader nfile)
        {
            int i, j, k;
            long longrecn = intRecordNumber;
            nfile.BaseStream.Position = (longrecn*512);
            intKeysInThisNode = nfile.ReadInt32();
            for (i = 0; i < intGeneralKeysPerNode; i++)
            {
                lower_level[i] = nfile.ReadInt32();
                key_record_number[i] = nfile.ReadInt32();

                if (objKeyType == dBaseType.N)
                {
                    //Key is a double number
                    key_expression[i] = new NodeKey(Convert.ToDouble(nfile.ReadInt64()));
                }
                else
                {
                    key_buffer = nfile.ReadBytes(intKeyExpressionSize);
                    for (k = 0; k < intKeyExpressionSize && key_buffer[k] != 0; k++) ;
                    key_expression[i] = new NodeKey(dBaseConverter.C_ToString(key_buffer));
                }

                j = intKeyExpressionSize%4;
                if (j > 0) j = 4 - j;
                for (k = 0; k < j; k++)
                    nfile.ReadByte();
            } // for i

            if (lower_level[0] > 0) boolBranch = true;
            else boolBranch = false;

            lower_level[i] = nfile.ReadInt32();
        }


        public void set_key_expression_size(int l)
        {
            intKeyExpressionSize = l;
        }


        public int PositionUp()
        {
            return ++intPosition;
        }

        public int PositionDown()
        {
            return --intPosition;
        }

        public void SetKeyValue(NodeKey key)
        {
            key_expression[intPosition] = key;
        }

        public void SetKeyValue(String key)
        {
            key_expression[intPosition] = new NodeKey(key);
        }

        public void SetKeyValue(double key)
        {
            key_expression[intPosition] = new NodeKey(key);
        }

        #region SET / GET

        public int RecordNumber
        {
            get { return intRecordNumber; }

            set
            {
                if (value == 0)
                    throw new Exception("Invalid record number in set");

                intRecordNumber = value;
            }
        }

        public int Position
        {
            get { return intPosition; }
            set { this.intPosition = value; }

        }

        public int KeyRecordNumber
        {
            get { return key_record_number[intPosition]; }
            set { key_record_number[intPosition] = value; }
        }

        public int LowerLevel
        {
            get { return lower_level[intPosition]; }
            set { lower_level[intPosition] = value; }
        }

        public int KeysInNode
        {
            get { return intKeysInThisNode; }
            set { intKeysInThisNode = value; }
        }

        public NodeKey KeyValue
        {
            get { return key_expression[intPosition]; }
        }

        public Node NextNode
        {
            get { return objNextNode; }
            set { this.objNextNode = value; }
        }

        public Node PreviousNode
        {
            get { return objPreviousNode; }
            set { this.objPreviousNode = value; }
        }

        #endregion

        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}
