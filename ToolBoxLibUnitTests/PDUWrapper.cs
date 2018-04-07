using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using System;
using System.Collections.Generic;

namespace ToolBoxLibUnitTests
{
    public class PDUWrapper : IPDU
    {
        public IntPtr pointer { get; set; }

        public List<string> Requests = new List<string>();

        public void addBitVarToReadRequest(int area, int DBnum, int start, int bytes)
        {
            Requests.Add("READ  Area:" + area + ", DBnum:" + DBnum + ", Start:" + start + ", Bytes:" + bytes);
        }

        public void addBitVarToWriteRequest(int area, int DBnum, int start, int bytes, byte[] buffer)
        {
            Requests.Add("WRITE Area:" + area + ", DBnum:" + DBnum + ", Start:" + start + ", Bytes:" + bytes);
        }

        public void addDbRead400ToReadRequest(int DBnum, int offset, int byteCount)
        {
            Requests.Add("READ  DBnum:" + DBnum + ", Offset:" + offset);
        }

        public void addSymbolVarToReadRequest(string completeSymbol)
        { }

        public void addVarToReadRequest(int area, int DBnum, int start, int bytes)
        {
            Requests.Add("READ  Area:" + area + ", DBnum:" + DBnum + ", Start:" + start + ", Bytes:" + bytes);
        }

        public void addVarToWriteRequest(int area, int DBnum, int start, int bytes, byte[] buffer)
        {
            Requests.Add("WRITE Area:" + area + ", DBnum:" + DBnum + ", Start:" + start + ", Bytes:" + bytes);
        }

        public void addNCKToReadRequest(int area, int unit, int column, int line, int module, int linecount)
        {
            Requests.Add("Read NCK");
        }

        public void addNCKToWriteRequest(int area, int unit, int column, int line, int module, int linecount, int bytes, byte[] buffer)
        {
            Requests.Add("Write NCK");
        }

        public void daveAddFillByteToReadRequest()
        { }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Requests);
        }
    }
}
