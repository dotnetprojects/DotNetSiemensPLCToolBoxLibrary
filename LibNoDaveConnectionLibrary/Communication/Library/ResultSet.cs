using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public class ResultSet
    {
        public ResultSet(byte[] buffer)
        {
            _buffer = buffer;
            _bufferPos = 0;
        }

        private readonly byte[] _buffer;
        public byte[] Buffer
        {
            get { return _buffer; }            
        }

        private int _bufferPos;

        public byte getU8()
        {
            _bufferPos += 1;
            return ByteFunctions.getU8from(_buffer, _bufferPos - 1);
        }

        public UInt16 getU16()
        {
            _bufferPos += 2;
            return ByteFunctions.getU16from(_buffer, _bufferPos - 2);
        }

        public UInt32 getU32()
        {
            _bufferPos += 4;
            return ByteFunctions.getU32from(_buffer, _bufferPos - 4);
        }

        public sbyte getS8()
        {
            _bufferPos += 1;
            return ByteFunctions.getS8from(_buffer, _bufferPos - 1);
        }

        public Int16 getS16()
        {
            _bufferPos += 2;
            return ByteFunctions.getS16from(_buffer, _bufferPos - 2);
        }

        public Int32 getS32()
        {
            _bufferPos += 4;
            return ByteFunctions.getS32from(_buffer, _bufferPos - 4);
        }
    }
}
