using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public class ResultSet
    {
        public ResultSet(int count, byte[] buffer)
        {
            this.buffer = buffer;
            _count = count;
            int pos = 0;
            for (int n = 0; n < _count; n++)
            {                
                startAddrResult.Add(pos);
                if (buffer[pos + 1] == 4) //Transportsize == Bits
                    pos += (buffer[pos + 2]*0x100 + buffer[pos + 3])/8;
                else
                    pos += buffer[pos + 2] * 0x100 + buffer[pos + 3];
                pos += 4;                
            }
            _bufferPos = 0;
        }

        
        private List<int> startAddrResult = new List<int>();
        private byte[] buffer;
        private int _bufferPos;
        
        private int akResultnumber = 0;
        private int akresultpos;
        private int _count = 0;

        public int useResult(int number)
        {
            akResultnumber = number;
            akresultpos = startAddrResult[akResultnumber];
            return buffer[akresultpos] != 0xff ? buffer[akresultpos] : 0;
        }

        public byte[] Buffer
        {
            get
            {
                int len = (buffer[akresultpos + 2]*0x100 + buffer[akresultpos + 3]);
                if (buffer[akresultpos + 1] == 4) //Transportsize == Bits
                    len /= 8;
                byte[] intBuff = new byte[len];
                Array.Copy(buffer, akresultpos + 4, intBuff, 0, len);
                return intBuff;
            }
        }
    }
}
