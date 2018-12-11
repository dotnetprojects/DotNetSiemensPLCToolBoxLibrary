using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    public class PLCTag<T> : PLCTag
    {
        private Type _type;
        private Type _underlyingType;
        private bool _isStructType;

        //private T _value;
        //private T _controlvalue;
        //Todo support for string and array datatypes
        internal override void _putControlValueIntoBuffer(byte[] buff, int startpos)
        {
            if (_isStructType)
            {
                byte[] tmp = ToBytes(_controlvalue);
                Array.Copy(tmp, 0, buff, startpos, tmp.Length);
            }
            else
            {
                base._putControlValueIntoBuffer(buff, startpos);
            }
        }

        internal override void _readValueFromBuffer(byte[] buff, int startpos)
        {
            if (_isStructType)
            {
                _value = (T)FromBytes(typeof(T), buff, startpos);
            }
            else
            {
                base._readValueFromBuffer(buff, startpos);
            }
        }

        protected internal override object _setValueProp
        {
            set
            {
                base._setValueProp = value;
                NotifyPropertyChanged("GenericValue");
            }
        }
        public PLCTag()
        {
            this._type = typeof(T);
            if (this._type.IsEnum)
                _underlyingType = Enum.GetUnderlyingType(this._type);
            this._isStructType = this._type.IsValueType && !this._type.IsEnum && !this._type.IsPrimitive && this._type != typeof(decimal);
            ParseGenericType();
        }

        public PLCTag(string initalizationString) : base(initalizationString)
        {
            this._type = typeof(T);
            if (this._type.IsEnum)
                _underlyingType = Enum.GetUnderlyingType(this._type);
            this._isStructType = this._type.IsValueType && !this._type.IsEnum && !this._type.IsPrimitive && this._type != typeof(decimal);
        }

        public PLCTag(string address, TagDataType type) : base(address, type)
        {
            this._type = typeof(T);
            if (this._type.IsEnum)
                _underlyingType = Enum.GetUnderlyingType(this._type);
            this._isStructType = this._type.IsValueType && !this._type.IsEnum && !this._type.IsPrimitive && this._type != typeof(decimal);
        }

        private void ParseGenericType()
        {
            var type = _underlyingType ?? _type;
            
            if (type == typeof(Int16))
                this.TagDataType = TagDataType.Int;
            else if (type == typeof(Int32))
                this.TagDataType = TagDataType.Dint;
            else if (type == typeof(Int64))
                this.TagDataType = TagDataType.LInt;
            else if (type == typeof(UInt16))
                this.TagDataType = TagDataType.Word;
            else if (type == typeof(UInt32))
                this.TagDataType = TagDataType.Dword;
            else if (type == typeof(UInt64))
                this.TagDataType = TagDataType.LWord;
            else if (type == typeof(byte))
                this.TagDataType = TagDataType.Byte;
            else if (type == typeof(sbyte))
                this.TagDataType = TagDataType.SByte;
            else if (type == typeof(Single))
                this.TagDataType = TagDataType.Float;
            else if (type == typeof(Double))
                this.TagDataType = TagDataType.LReal;
            else if (type == typeof(string))
                this.TagDataType = TagDataType.CharArray;
            else if (type == typeof(byte[]))
                this.TagDataType = TagDataType.ByteArray;
            else
                this.TagDataType = TagDataType.Struct;
        }

        public T GenericValue
        {
            get
            {
                if (_value == null)
                    return default(T);
                if (this._type.IsEnum)
                    return (T)Convert.ChangeType(_value, _underlyingType);
                return  (T)_value;
            }
            set
            {
                if (this._type.IsEnum)
                    Value = Convert.ChangeType(value, _underlyingType);
                else
                    Value = value;
            }
        }
        internal override int _internalGetSize()
        {
            if (this._isStructType)
                return GetStructSize(typeof(T));
            return base._internalGetSize();
        }


        #region Helper Functions for Structs, from S7.NET! (http://s7net.codeplex.com/)

        /// <summary>
        /// Creates a struct of a specified type by an array of bytes.
        /// </summary>
        /// <param name="structType">The struct type</param>
        /// <param name="bytes">The array of bytes</param>
        /// <returns>The object depending on the struct type or null if fails(array-length != struct-length</returns>
        private static object FromBytes(Type structType, byte[] bytes, int startpos)
        {
            if (bytes == null)
                return null;

            if (bytes.Length != GetStructSize(structType))
                return null;

            // and decode it
            int bytePos = 0;
            int bitPos = 0;
            double numBytes = startpos;
            object structValue = Activator.CreateInstance(structType);

            System.Reflection.FieldInfo[] infos = structValue.GetType().GetFields();
            foreach (System.Reflection.FieldInfo info in infos)
            {
                switch (info.FieldType.Name)
                {
                    case "Boolean":
                        // get the value
                        bytePos = (int)Math.Floor(numBytes);
                        bitPos = (int)((numBytes - (double)bytePos) / 0.125);
                        if ((bytes[bytePos] & (int)Math.Pow(2, bitPos)) != 0)
                            info.SetValue(structValue, true);
                        else
                            info.SetValue(structValue, false);
                        numBytes += 0.125;
                        break;
                    case "Byte":
                        numBytes = Math.Ceiling(numBytes);
                        info.SetValue(structValue, (byte)(bytes[(int)numBytes]));
                        numBytes++;
                        break;
                    case "String":
                        {
                            numBytes = Math.Ceiling(numBytes);
                            if ((numBytes / 2 - Math.Floor(numBytes / 2.0)) > 0)
                                numBytes++;

                            MarshalAsAttribute mAttr = null;
                            object[] attr = info.GetCustomAttributes(typeof (MarshalAsAttribute), false);
                            if (attr.Length > 0)
                                mAttr = (MarshalAsAttribute) attr[0];
                            if (mAttr == null || mAttr.Value != UnmanagedType.ByValTStr || mAttr.SizeConst <= 0)
                                throw new Exception("Strings in Structs need to be decorated with \"MarshalAs(UnmanagedType.ByValTStr, SizeConst = xx)\"");

                            var sb = new StringBuilder();
                            int size = mAttr.SizeConst > bytes[((int) numBytes) + 1] ? bytes[((int) numBytes) + 1] : mAttr.SizeConst;
                            for (var n = 2; n < size+2; n++)
                                sb.Append((char) bytes[n + (int) numBytes]);
                            info.SetValue(structValue, (String) sb.ToString());

                            numBytes += 2 + mAttr.SizeConst;
                        }
                        break;
                    case "Int16":
                         numBytes = Math.Ceiling(numBytes);
                        if ((numBytes / 2 - Math.Floor(numBytes / 2.0)) > 0)
                            numBytes++;
                        // hier auswerten
                        info.SetValue(structValue, libnodave.getS16from(bytes, (int) numBytes));
                        numBytes += 2;
                        break;
                    case "UInt16":
                        numBytes = Math.Ceiling(numBytes);
                        if ((numBytes / 2 - Math.Floor(numBytes / 2.0)) > 0)
                            numBytes++;
                        // hier auswerten
                        info.SetValue(structValue, libnodave.getU16from(bytes, (int) numBytes));
                        numBytes += 2;
                        break;
                    case "Int32":
                         numBytes = Math.Ceiling(numBytes);
                        if ((numBytes / 2 - Math.Floor(numBytes / 2.0)) > 0)
                            numBytes++;
                        // hier auswerten
                        info.SetValue(structValue, libnodave.getS32from(bytes, (int)numBytes));
                        numBytes += 4;
                        break;
                    case "UInt32":
                        numBytes = Math.Ceiling(numBytes);
                        if ((numBytes / 2 - Math.Floor(numBytes / 2.0)) > 0)
                            numBytes++;
                        // hier auswerten
                        info.SetValue(structValue, libnodave.getU32from(bytes, (int)numBytes));
                        numBytes += 4;
                        break;
                    case "Double":
                        numBytes = Math.Ceiling(numBytes);
                        if ((numBytes / 2 - Math.Floor(numBytes / 2.0)) > 0)
                            numBytes++;
                        // hier auswerten
                        info.SetValue(structValue, libnodave.getFloatfrom(bytes, (int)numBytes));
                        numBytes += 4;
                        break;
                }
            }
            return structValue;
        }

        /// <summary>
        /// Gets the size of the struct in bytes.
        /// </summary>
        /// <param name="structType">the type of the struct</param>
        /// <returns>the number of bytes</returns>
        private static int GetStructSize(Type structType)
        {
            double numBytes = 0.0;

            System.Reflection.FieldInfo[] infos = structType.GetFields();
            foreach (System.Reflection.FieldInfo info in infos)
            {
                switch (info.FieldType.Name)
                {
                    case "Boolean":
                        numBytes += 0.125;
                        break;
                    case "Byte":
                        numBytes = Math.Ceiling(numBytes);
                        numBytes++;
                        break;
                    case "Int16":
                    case "UInt16":
                        numBytes = Math.Ceiling(numBytes);
                        if ((numBytes / 2 - Math.Floor(numBytes / 2.0)) > 0)
                            numBytes++;
                        numBytes += 2;
                        break;
                    case "Int32":
                    case "UInt32":
                        numBytes = Math.Ceiling(numBytes);
                        if ((numBytes / 2 - Math.Floor(numBytes / 2.0)) > 0)
                            numBytes++;
                        numBytes += 4;
                        break;
                    case "Float":
                    case "Double":
                        numBytes = Math.Ceiling(numBytes);
                        if ((numBytes / 2 - Math.Floor(numBytes / 2.0)) > 0)
                            numBytes++;
                        numBytes += 4;
                        break;
                    case "String":
                        {
                            MarshalAsAttribute mAttr=null;
                            object[] attr = info.GetCustomAttributes(typeof (MarshalAsAttribute), false);
                            if (attr.Length > 0)
                                mAttr = (MarshalAsAttribute) attr[0];
                            if (mAttr == null || mAttr.Value != UnmanagedType.ByValTStr || mAttr.SizeConst <= 0)
                                throw new Exception("Strings in Structs need to be decorated with \"MarshalAs(UnmanagedType.ByValTStr, SizeConst = xx)\"");

                            numBytes += mAttr.SizeConst + 2;
                        }
                        break;
                        /*
                     * Typen welche noch fehlen
                     * 
                     * 
                    case Arrays von jedem Datentyp!
                    struct of struct
                    TimeSpan
                     * 
                    case "DateTime":
                    case "String": //   Look for: [MarshalAs(UnmanagedType.ByValTStr , SizeConst = 7)]
                        break;

                        */
                }
            }
            return (int)Math.Ceiling(numBytes);
        }


        /// <summary>
        /// Creates a byte array depending on the struct type.
        /// </summary>
        /// <param name="structValue">The struct object</param>
        /// <returns>A byte array or null if fails.</returns>
        public static byte[] ToBytes(object structValue)
        {
            Type type = typeof (T);
            int size = GetStructSize(type);
            byte[] bytes = new byte[size];
            byte[] bytes2 = null;

            int bytePos = 0;
            int bitPos = 0;
            double numBytes = 0.0;

            System.Reflection.FieldInfo[] infos = type.GetFields();
            foreach (System.Reflection.FieldInfo info in infos)
            {
                bytes2 = null;
                switch (info.FieldType.Name)
                {
                    case "Boolean":
                        // get the value
                        bytePos = (int)Math.Floor(numBytes);
                        bitPos = (int)((numBytes - (double)bytePos) / 0.125);
                        if ((bool)info.GetValue(structValue))
                            bytes[bytePos] |= (byte)Math.Pow(2, bitPos);            // is true
                        else
                            bytes[bytePos] &= (byte)(~(byte)Math.Pow(2, bitPos));   // is false
                        numBytes += 0.125;
                        break;
                    case "Byte":
                        numBytes = (int)Math.Ceiling(numBytes);
                        bytePos = (int)numBytes;
                        bytes[bytePos] = (byte)info.GetValue(structValue);
                        numBytes++;
                        break;
                    case "String":
                        {
                            MarshalAsAttribute mAttr = null;
                            object[] attr = info.GetCustomAttributes(typeof (MarshalAsAttribute), false);
                            if (attr.Length > 0)
                                mAttr = (MarshalAsAttribute) attr[0];
                            if (mAttr == null || mAttr.Value != UnmanagedType.ByValTStr || mAttr.SizeConst <= 0)
                                throw new Exception("Strings in Structs need to be decorated with \"MarshalAs(UnmanagedType.ByValTStr, SizeConst = xx)\"");

                            bytes2 = new byte[mAttr.SizeConst + 2];
                            libnodave.putS7Stringat(bytes2, 0, (string)info.GetValue(structValue), bytes2.Length);
                            bytePos = (int)numBytes;
                        }
                        break;
                    case "Int16":
                        bytes2 = new byte[2];
                        libnodave.putS16at(bytes2, 0, (Int16) info.GetValue(structValue));
                        bytePos = (int)numBytes;
                        break;
                    case "UInt16":
                        bytes2 = new byte[2];
                        libnodave.putU16at(bytes2, 0, (UInt16) info.GetValue(structValue));
                        bytePos = (int)numBytes;
                        break;
                    case "Int32":
                        bytes2 = new byte[4];
                        libnodave.putS32at(bytes2, 0, (Int32) info.GetValue(structValue));
                        bytePos = (int)numBytes;
                        break;
                    case "UInt32":
                        bytes2 = new byte[4];
                        libnodave.putU32at(bytes2, 0, (UInt32) info.GetValue(structValue));
                        bytePos = (int)numBytes;
                        break;
                    case "Double":
                        bytes2 = new byte[4];
                        libnodave.putFloatat(bytes2, 0, Convert.ToSingle((double)info.GetValue(structValue)));
                        bytePos = (int)numBytes;
                        break;
                    case "Single":
                        bytes2 = new byte[4];
                        libnodave.putFloatat(bytes2, 0, (Single)info.GetValue(structValue));
                        bytePos = (int)numBytes;
                        break;
                }
                if (bytes2 != null)
                {
                    // add them
                    
                    for (int bCnt = 0; bCnt < bytes2.Length; bCnt++)
                        bytes[bytePos + bCnt] = bytes2[bCnt];
                    numBytes += bytes2.Length;
                }
            }
            return bytes;
        }

        #endregion

    }
}
