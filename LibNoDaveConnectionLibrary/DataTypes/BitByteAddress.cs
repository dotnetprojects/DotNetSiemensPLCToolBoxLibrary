using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    [Serializable()]
    public class ByteBitAddress
    {
        public int ByteAddress { get; set; }
        public int BitAddress { get; set; }


        public ByteBitAddress(string address)
        {
            if (address.Contains("."))
            {
                ByteAddress = Convert.ToInt32(address.Split('.')[0]);
                BitAddress = Convert.ToInt32(address.Split('.')[1]);
            }
            else if (address.Contains("."))
            {
                ByteAddress = Convert.ToInt32(address);
                BitAddress = 0;
            }
        }

        public ByteBitAddress(int byteaddress, int bitaddress)
        {
            ByteAddress = byteaddress;
            BitAddress = bitaddress;
        }

        public ByteBitAddress(ByteBitAddress startAddr)
        {
            if (startAddr != null)
            {
                ByteAddress = startAddr.ByteAddress;
                BitAddress = startAddr.BitAddress;
            }
            else
            {
                ByteAddress = 0;
                BitAddress = 0;
            }

        }
        public static bool operator <(ByteBitAddress b1, ByteBitAddress b2)
        {
            if (b1.ByteAddress * 8 + b1.BitAddress < b2.ByteAddress * 8 + b2.BitAddress)
                return true;
            return false;
        }

        public static bool operator >(ByteBitAddress b1, ByteBitAddress b2)
        {
            if (b1.ByteAddress * 8 + b1.BitAddress > b2.ByteAddress * 8 + b2.BitAddress)
                return true;
            return false;
        }

        public override string ToString()
        {
            return ByteAddress.ToString() + "." + BitAddress.ToString();
        }

        public static bool operator ==(ByteBitAddress a, ByteBitAddress b)
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
            return a.BitAddress == b.BitAddress && a.ByteAddress == b.ByteAddress;
        }

        public static bool operator !=(ByteBitAddress a, ByteBitAddress b)
        {
            return !(a == b);
        }
        /*
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        */
    }
}
