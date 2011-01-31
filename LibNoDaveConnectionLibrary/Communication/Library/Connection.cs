using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public class Connection : IDisposable
    {
        public void Dispose()
        { }         

        public Interface Interface { get; set; }

        public Pdu PrepareReadRequest()
        {
            return null;
        }

        public ResultSet ExecReadRequest(Pdu myPdu)
        {
            return null;
        }

        public void SendPdu(Pdu myPdu)
        { }

        public void DeleteprogrammBlock(int BlockType, int Nummer)
        { }
    }
}
