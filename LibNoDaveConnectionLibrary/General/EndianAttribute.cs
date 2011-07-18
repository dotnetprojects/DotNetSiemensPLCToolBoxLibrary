using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EndianAttribute : Attribute
    {
        public Endianness Endianness { get; private set; }

        public EndianAttribute(Endianness endianness)
        {
            this.Endianness = endianness;
        }
    }

    public enum Endianness
    {
        BigEndian,
        LittleEndian
    }

    public static class EndianessMarshaler
    {
        public static void RespectEndianness(Type type, byte[] data)
        {
            foreach (FieldInfo f in type.GetFields()) //BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (f.IsDefined(typeof(EndianAttribute), false))
                {
                    EndianAttribute att = (EndianAttribute)f.GetCustomAttributes(typeof(EndianAttribute), false)[0];
                    int offset = Marshal.OffsetOf(type, f.Name).ToInt32();
                    if ((att.Endianness == Endianness.BigEndian && BitConverter.IsLittleEndian) || (att.Endianness == Endianness.LittleEndian && !BitConverter.IsLittleEndian))
                    {
                        Array.Reverse(data, offset, Marshal.SizeOf(f.FieldType));
                    }
                }
            }
        }

        public static T BytesToStruct<T>(byte[] rawData) //where T : class 
        {
            T result = default(T);

            RespectEndianness(typeof(T), rawData);

            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);

            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                result = (T)Marshal.PtrToStructure(rawDataPtr, typeof(T));
            }
            finally
            {
                handle.Free();
            }

            return result;
        }

        public static byte[] StructToBytes<T>(T data) //where T : class
        {
            byte[] rawData = new byte[Marshal.SizeOf(data)];
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                Marshal.StructureToPtr(data, rawDataPtr, false);
            }
            finally
            {
                handle.Free();
            }

            RespectEndianness(typeof(T), rawData);

            return rawData;
        }
    }
}
