using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    /// <summary>
    /// This is a internal used Tag for Read optimazions
    /// </summary>
    internal class PLCTagReadHelper : PLCTag
    {
        public Dictionary<PLCTag, int> PLCTags = new Dictionary<PLCTag, int>();

        internal override void _readValueFromBuffer(byte[] buff, int startpos)
        {
            foreach (KeyValuePair<PLCTag, int> keyValuePair in PLCTags)
            {
                keyValuePair.Key._readValueFromBuffer(buff, keyValuePair.Value + startpos);
            }            
        }
    }
}
