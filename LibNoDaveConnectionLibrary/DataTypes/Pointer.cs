using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    [Serializable()]
    public class Pointer : ByteBitAddress
    {
        public MemoryArea MemoryArea { get; set; }

        public Pointer(string address) : base(0, 0)
        {
            if (address.StartsWith("P#"))
            {
                address = address.Substring(2);
            }

            if (address.StartsWith("DIX"))
            {
                MemoryArea = MemoryArea.InstanceDatablock;
                address = address.Substring(3);
            }
            else if (address.StartsWith("DBX"))
            {
                MemoryArea = MemoryArea.Datablock;
                address = address.Substring(3);
            }

            if (address.Contains("."))
            {
                ByteAddress = Convert.ToInt32(address.Split('.')[0]);
                BitAddress = Convert.ToInt32(address.Split('.')[1]);
            }
            else
            {
                ByteAddress = Convert.ToInt32(address);
                BitAddress = 0;
            }
        }

        public Pointer(int byteaddress, int bitaddress) : base(byteaddress, bitaddress)
        { }

        public Pointer(ByteBitAddress startAddr) : base(startAddr)
        { }

        public string ToString(MnemonicLanguage language)
        {
            return "P#" + Helper.MemoryAreaToPointer(this.MemoryArea, language) + " " + base.ToString();
        }

        public static Pointer operator +(Pointer b1, Pointer b2)
        {
            Pointer retVal = new Pointer(0, 0);

            retVal.BitAddress = b1.BitAddress + b2.BitAddress;
            retVal.ByteAddress = b1.ByteAddress + b2.ByteAddress;
            if (retVal.BitAddress > 7)
            {
                retVal.BitAddress -= 8;
                retVal.ByteAddress++;
            }

            return retVal;
        }

        public static bool operator ==(Pointer a, Pointer b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.BitAddress == b.BitAddress && a.ByteAddress == b.ByteAddress && a.MemoryArea == b.MemoryArea;
        }

        public static bool operator !=(Pointer a, Pointer b)
        {
            return !(a == b);
        }
    }
}