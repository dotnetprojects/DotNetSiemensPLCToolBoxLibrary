using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolBoxLibUnitTests
{
    public class ConnectionWrapper : IDaveConnection
    {
        int _pduSize;
        public ConnectionWrapper(int pduSize)
        {
            _pduSize = pduSize;
        }

        public int connectPLC()
        {
            throw new NotImplementedException();
        }

        public int daveBuildAndSendPDU(IPDU myPDU, byte[] Parameter, byte[] Data)
        {
            throw new NotImplementedException();
        }

        public int daveGetPDUData(IPDU myPDU, out byte[] data, out byte[] param)
        {
            throw new NotImplementedException();
        }

        public DateTime daveReadPLCTime()
        {
            throw new NotImplementedException();
        }

        public int daveRecieveData(out byte[] data, out byte[] param)
        {
            throw new NotImplementedException();
        }

        public int daveSetPLCTime(DateTime tm)
        {
            throw new NotImplementedException();
        }

        public int deleteProgramBlock(int blockType, int number)
        {
            throw new NotImplementedException();
        }

        public int disconnectPLC()
        {
            throw new NotImplementedException();
        }

        public List<IPDU> PDUs = new List<IPDU>();

        public int execReadRequest(IPDU p, IresultSet rl)
        {
            PDUs.Add(p);

            return 0;
        }

        public int execWriteRequest(IPDU p, IresultSet rl)
        {
            throw new NotImplementedException();
        }

        public int force200(int area, int start, int val)
        {
            throw new NotImplementedException();
        }

        public int forceDisconnectIBH(int src, int dest, int MPI)
        {
            throw new NotImplementedException();
        }

        public int getAnswLen()
        {
            throw new NotImplementedException();
        }

        public int getGetResponse()
        {
            throw new NotImplementedException();
        }

        public int getMaxPDULen()
        {
            return _pduSize;
        }

        public int getMessage(IPDU p)
        {
            throw new NotImplementedException();
        }

        public int getProgramBlock(int blockType, int number, byte[] buffer, ref int length)
        {
            throw new NotImplementedException();
        }

        public int getU8()
        {
            throw new NotImplementedException();
        }

        public int ListBlocksOfType(int blockType, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public IPDU prepareReadRequest()
        {
            return new PDUWrapper();
        }

        public IPDU prepareWriteRequest()
        {
            return new PDUWrapper();
        }

        public IPDU createPDU()
        {
            return new PDUWrapper();
        }

        public int putProgramBlock(int blockType, int number, byte[] buffer, int length)
        {
            throw new NotImplementedException();
        }

        public int readBits(int area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int readBytes(int area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int readManyBytes(int area, int DBnumber, int start, int len, ref byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int readSZL(int id, int index, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int resetIBH()
        {
            throw new NotImplementedException();
        }

        public int start()
        {
            throw new NotImplementedException();
        }

        public int stop()
        {
            throw new NotImplementedException();
        }

        public List<IresultSet> IresultSets = new List<IresultSet>();

        public int useResult(IresultSet rs, int number, byte[] buffer)
        {
            IresultSets.Add(rs);

            return 0;
        }

        public int writeBits(int area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int writeBytes(int area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int writeManyBytes(int area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public IresultSet getResultSet()
        {
            return new ResultSetWrapper();
        }

        public int PI_StartNC(string piservice, string[] param, int paramCount)
        {
            throw new NotImplementedException();
        }

        public int initUploadNC(string file, ref byte[] uploadID)
        {
            throw new NotImplementedException();
        }

        public int doUploadNC(out int more, byte[] buffer, out int len, byte[] uploadID)
        {
            throw new NotImplementedException();
        }

        public int endUploadNC(byte[] uploadID)
        {
            throw new NotImplementedException();
        }

        public int daveGetNCProgram(string filename, byte[] buffer, ref int length)
        {
            throw new NotImplementedException();
        }

        public int daveGetNcFile(string filename, byte[] buffer, ref int length)
        {
            throw new NotImplementedException();
        }

        public int daveGetNcFileSize(string filename, ref int length)
        {
            throw new NotImplementedException();
        }

        public int davePutNCProgram(string filename, string path, string ts, byte[] buffer, int length)
        {
            throw new NotImplementedException();
        }

        public int alarmQueryAlarm_S(byte[] buffer, int length, ref int alarmCount)
        {
            throw new NotImplementedException();
        }

        public int daveReadPLCTime(out DateTime dateTime)
        {
            throw new NotImplementedException();
        }
    }
}
