using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DBF.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Index
{
    /// <summary>
    /// A node key with a dynamic type (string, float, double)
    /// </summary>
    public class NodeKey
    {
        private dBaseType objType;
        private object objKey = null;

        public NodeKey(String Key)
        {
            this.objKey = Key;
            objType = dBaseType.C;
        }

        public NodeKey(Double Key)
        {
            this.objKey = Key;
            objType = dBaseType.N;
        }

        public NodeKey(NodeFloat Key)
        {
            this.objKey = Key;
            objType = dBaseType.F;
        }

        public String RebuildString(String inString)
        {
            char[] a = new char[inString.Length];
            for (int i = 0; i < inString.Length; i++)
            {
                if (inString[i] == '_')
                {
                    a[i] = (char) 31;
                }
                else
                {
                    a[i] = inString[i];
                }
            }

            return new String(a);
        }

        public int CompareKey(NodeKey keyCompareTo)
        {
            int ret = 0;
            if (this.objType != keyCompareTo.NodeKeyType)
                return -1; // throw new xBaseJException("Node key types do not match");
            if (this.objType == dBaseType.C)
            {
                String s = (String) this.objKey;
                s = RebuildString(s);
                String t = keyCompareTo.ToString();
                t = RebuildString(t);
                return s.CompareTo(t);
            }
            if (this.objType == dBaseType.F)
            {
                NodeFloat nf = (NodeFloat) this.objKey;
                NodeFloat nft = (NodeFloat) keyCompareTo.objKey;
                return nf.CompareTo(nft);
            }
            Double d = (Double) this.objKey;

            double d2 = d - keyCompareTo.GetDouble;
            if (d2 < 0.0) return -1;
            if (d2 > 0.0) return 1;
            return ret;
        }

        #region SET / GET

        public dBaseType NodeKeyType
        {
            get { return this.objType; }
        }

        public int Length
        {
            get
            {
                if (objType == dBaseType.C)
                    return ((String) objKey).Length;
                if (objType == dBaseType.F)
                    return 12;
                return 8;
            }
        }

        public String GetString
        {
            get
            {
                string OutputString = string.Empty;
                switch (this.objType)
                {
                    case dBaseType.C:
                        OutputString = (string) this.objKey;
                        break;
                    case dBaseType.N:
                        OutputString = ((double) objKey).ToString("#");
                        break;

                    case dBaseType.F:
                        NodeFloat tempfloat = (NodeFloat) this.objKey;
                        OutputString = tempfloat.ToString();
                        break;

                }
                return OutputString;
            }
        }

        public Double GetDouble
        {
            get
            {
                if (this.objType == dBaseType.N)
                {
                    Double d = (Double) this.objKey;
                    return d;
                }
                return 0.0;
            }
        }

        public NodeFloat GetNodeFloat
        {
            get
            {
                if (objType == dBaseType.F)
                {
                    NodeFloat f = (NodeFloat) this.objKey;
                    return f;
                }
                return null;
            }
        }

        #endregion
    }


    public class KeyList : IComparer<KeyList>
    {
        private NodeKey objKey;
        private int intWhere;

        public KeyList(NodeKey Key, int i)
        {
            this.objKey = Key;
            this.intWhere = i;
        }

        #region SET / GET

        public int Where
        {
            get { return this.intWhere; }
        }

        #endregion

        public int Compare(KeyList x, KeyList y)
        {
            return x.objKey.CompareKey(y.objKey);
        }
    }
}
