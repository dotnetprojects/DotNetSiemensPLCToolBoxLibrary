using System;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Index
{
    /// <summary>
    /// Custom Type to represent Step7 float datatype with integrated conversion functions.
    /// </summary>
    public class NodeFloat : IComparable
    {
        private byte bSize;
        private byte bSign;
        private byte[] bValue;
        private double dSaveValue;

        private const byte bNegativSign1 = 0xd1;
        private const byte bNegativSign2 = 0xa9;

        /// <summary>
        /// Create a float from a byte array, containing a S7 float value
        /// </summary>
        /// <param name="ByteValue">The BCD byte array</param>
        public NodeFloat(byte[] ByteValue)
        {
            StringBuilder sb = new StringBuilder(15);

            bSize = ByteValue[0];
            bSign = ByteValue[1];

            bValue = new byte[10];
            int i;
            for (i = 0; i < 10; i++)
                bValue[i] = ByteValue[i + 2];

            if (bSign == 0x10)
            {
                dSaveValue = 0.0;
                return;
            }

            bool neg = false;
            if ((bSign == bNegativSign1) || (bSign == bNegativSign2))
                neg = true;

            byte b;
            char c1;

            bool negC;
            int i2;
            int j;
            for (j = 11; j > 0; j--)
            {
                if (ByteValue[j] != 0) break;
            }

            if (j == 1)
            {
                dSaveValue = 0.0;
                return;
            }

            j++;
            for (i = 2; i < j; i++)
            {
                b = ByteValue[i];
                negC = (b < 0);
                i2 = b & 0x70;
                i2 >>= 4;
                if (negC) i2 += 8;
                c1 = i2.ToString()[0];
                sb.Append(c1);
                if (i == 2)
                    sb.Append('.');
                i2 = b & 0x0f;
                c1 = i2.ToString()[0];
                sb.Append(c1);
            }

            sb.Append('e');
            sb.Append((bSize - 0x35).ToString());
            String s = sb.ToString();
            dSaveValue = Double.Parse(s)*(neg ? -1 : 1);

        }

        /// <summary>
        /// Create a float from a double value
        /// </summary>
        /// <param name="DoubleValue">The double value to create the float from</param>
        public NodeFloat(double DoubleValue)
        {

            this.bSize = 0x35;
            this.bValue = new byte[10];

            dSaveValue = DoubleValue;

            int i;

            for (i = 0; i < 10; i++)
                bValue[i] = 0;

            if (DoubleValue == 0.0)
            {
                bSign = 0x10;
                return;
            }



            int[] bLine = {0, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90};
            int start = 0;


            bool neg = (DoubleValue < 0.0);
            if (neg) DoubleValue *= -1.0;

            Double d = DoubleValue;
            String s = d.ToString();

            int epos = (s.IndexOf('e') < 0 ? 0 : s.IndexOf('e') + 1) + (s.IndexOf('E') < 0 ? 0 : s.IndexOf('E') + 1);
            int decpos = s.IndexOf('.');
            String s2;
            int t;
            if (epos > 0)
            {
                s2 = s.Substring(decpos + 1, epos - 1);
            }
            else
            {
                s2 = s.Substring(decpos + 1);
            }
            t = int.Parse(s2);

            bool decfound = (t > 0);

            if (neg)
                if (decfound) bSign = (byte) 0xd1;
                else bSign = (byte) 0xa9;
            else if (decfound) bSign = (byte) 0x51;
            else bSign = (byte) 0x29;

            if (epos > 0)
            {
                bSize += byte.Parse(s.Substring(epos));
                s = s2;
            }
            else if ((decpos == 1) && (s[0] == '0'))
            {
                s = s2;
                bSize--;
                i = 2;
                while (i < s.Length && s[i] == '0')
                {
                    bSize--;
                    i++;
                }
                start = i;
            }
            else
            {
                bSize--;
                i = 0;
                while (s[i] != '.')
                {
                    bSize++;
                    i++;
                }
            }

            int v;
            start = 0;
            int j;
            char c;
            bool top = true;
            for (i = start, j = 0; i < s.Length && j < 10; i++)
            {
                // first nibble
                c = s[i];

                if (c != '.')
                    if (top)
                    {
                        v = Convert.ToInt32(Char.GetNumericValue(c));
                        bValue[j] = (byte) bLine[v];
                        top = false;
                    }
                    else
                    {
                        // second nibble
                        v = Convert.ToInt32(Char.GetNumericValue(c));
                        bValue[j] += (byte) v;
                        top = true;
                        j++;
                    }
            }
            return;
        }

        /// <summary>
        /// Compare this NodeFloat object against another
        /// </summary>
        /// <param name="obj">The NodeFloat object you want to compare</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {

            if (obj.GetType() == typeof (NodeFloat))
            {
                NodeFloat nf2 = (NodeFloat) obj;
                if (bSign == 0x10)
                {
                    if (nf2.bSign == 0x10) return 0;
                    return nf2.bSign < 0 ? -1 : 1; // nf2 is bigger if its > than 0;
                }

                if (nf2.bSign == 0x10)
                    return bSign < 0 ? 1 : -1; // nf2 is bigger if its > than 0;

                if (bSign < 0)
                    if (nf2.bSign > 0) return -1;

                if (bSign > 0)
                    if (nf2.bSign < 0) return 1;

                // assume both the same sign

                if (this.bSize < nf2.bSize) return -1;
                if (this.bSize > nf2.bSize) return 1;

                for (int i = 0; i < 10; i++)
                {
                    if (bValue[i] == nf2.bValue[i]) // they're equal go get next
                        continue;

                    if (bValue[i] < 0)
                    {
                        // reverse all logic for bytes whose first nibble is 8 or 9

                        if ((nf2.bValue[i] < 0) && (nf2.bValue[i] < bValue[i])) // though it's smaller it's bigger
                            return -1;
                        return 1;
                    }
                    if (nf2.bValue[i] < 0)
                    {
                        // reverse all logic for bytes whose first nibble is 8 or 9

                        return -1; // if nf2 is negative, we know this.value is not neg so nf2 must be bigger.
                    }
                    if (bValue[i] < nf2.bValue[i]) return -1;
                    if (bValue[i] > nf2.bValue[i]) return 1;
                }
            }
            return 0;
        }

        #region SET / GET

        /// <summary>
        /// The value of this NodeFloat as Double
        /// </summary>
        public double GetDouble
        {
            get { return this.dSaveValue; }
        }

        /// <summary>
        /// The value of this NodeFloat as String
        /// </summary>
        public string GetString
        {
            get { return this.dSaveValue.ToString(); }
        }

        /// <summary>
        /// The value of this NodeFloat as byte array
        /// </summary>
        public byte[] GetByteArray
        {
            get
            {
                byte[] returnArray = new byte[12];
                returnArray[0] = this.bSize;
                returnArray[1] = this.bSign;
                for (int i = 0; i < 10; i++)
                {
                    returnArray[i + 2] = bValue[i];
                }
                return returnArray;
            }
        }

        #endregion

    }
}
