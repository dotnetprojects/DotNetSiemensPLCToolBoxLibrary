using System;
using System.Collections.Generic;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DBF.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Index
{

    public abstract class Index
    {
        protected int top_Node;

        /// <summary>
        /// Address of the next available node
        /// </summary>
        protected int next_available;

        protected int reserved_02;

        /// <summary>
        /// Length of a key without overhead
        /// </summary>
        protected short shortKeyLength;

        /// <summary>
        /// Number of Keys per Page/Node
        /// </summary>
        protected short shortKeysPerNode;

        /// <summary>
        /// Datatype of the Key
        /// </summary>
        protected dBaseType objKeyType;

        /// <summary>
        /// Length of a key entry (must be devidable by 4)
        /// </summary>
        protected short key_entry_size;

        protected byte reserved_01;
        protected byte reserved_03;
        protected byte reserved_04;

        /// <summary>
        /// Is this Index a unique key?
        /// </summary>
        protected byte unique_key;

        /// <summary>
        /// Syntax definition of the key
        /// </summary>
        protected byte[] key_definition = new byte[488];

        protected List<Field> keyControl = new List<Field> {};
        protected NodeKey objActiveKey = null;
        protected int record; // the current key's record

        protected Node topNode = null;
        protected Node workNode;
        //public File file;
        //public RandomAccessFile nfile;
        //public FileChannel channel;
        //public ByteBuffer bytebuffer;
        protected String dosname = String.Empty;
        //public DBF database;

        protected String stringKey;

        protected static int findFirstMatchingKey = -1;
        protected static int findAnyKey = -2;
        protected static int keyNotFound = -3;
        protected static int foundMatchingKeyButNotRecord = -4;

        protected bool foundExact = false;

        public Index()
        {

        }

        public bool CompareKey(String keyToCompare)
        {
            NodeKey tempKey;

            if (objKeyType == dBaseType.F)
            {
                tempKey = new NodeKey(new NodeFloat(Double.Parse(keyToCompare)));
            }
            else
            {
                if (objKeyType == dBaseType.N)
                {
                    Double d = Double.Parse(keyToCompare);
                    tempKey = new NodeKey(d);
                }
                else
                {
                    tempKey = new NodeKey(keyToCompare);
                }
            }
            return (objActiveKey.CompareKey(tempKey) == 0);

        }

        public abstract int add_entry(NodeKey key, int recno);

        public int add_entry(int recno)
        {
            NodeKey newkey = BuildKey();
            return add_entry(newkey, recno);
        }

        /// <summary>
        /// Builds a NodeKey from this Index
        /// </summary>
        /// <returns>The created NodeKey</returns>
        public NodeKey BuildKey()
        {
            NodeKey dataptr;
            int i;
            Field Field;
            double doubleer = 0.0;
            switch (this.objKeyType)
            {
                case dBaseType.F:
                    foreach (Field f in this.keyControl)
                    {
                        Field = f;
                        if (Field.get() == null || Field.get().Length == 0)
                        {
                        }
                        else if (Field.getType() == dBaseType.D)
                        {
                            doubleer += Util.DoubleDate(Field.get());
                        }
                        else
                        {
                            doubleer += Double.Parse(Field.get());
                        }
                    } /* endfor */
                    dataptr = new NodeKey(new NodeFloat(doubleer));
                    break;
                case dBaseType.N:
                    for (i = 0; i < keyControl.Count; i++)
                    {
                        Field = keyControl[i];
                        if (Field.get() == null || Field.get().Length == 0)
                        {

                        }
                        else if (Field.getType() == dBaseType.D)
                        {
                            doubleer += Util.DoubleDate(Field.get());
                        }
                        else
                        {
                            doubleer += Double.Parse(Field.get());
                        } /* endfor */
                    }
                    dataptr = new NodeKey(doubleer);
                    break;
                default:
                    StringBuilder sb = new StringBuilder();
                    for (i = 0; i < keyControl.Count; i++)
                    {
                        Field = (Field) keyControl[i];

                        sb.Append(Field.get());
                    } /* endfor */
                    dataptr = new NodeKey(sb.ToString());
                    break;
            }
            return dataptr;
        }

        #region SET / GET

        /// <summary>
        /// Tells if the Key is unique or not
        /// </summary>
        public bool IsUnique
        {
            get { return Convert.ToBoolean(this.unique_key); }
        }

        #endregion
    }
}
