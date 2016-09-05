using System;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave
{
    public interface IDaveConnection
    {
        int connectPLC();
        int daveBuildAndSendPDU(IPDU myPDU, byte[] Parameter, byte[] Data);
        int daveGetPDUData(IPDU myPDU, out byte[] data, out byte[] param);
        DateTime daveReadPLCTime();
        int daveRecieveData(out byte[] data, out byte[] param);
        int daveSetPLCTime(DateTime tm);
        int deleteProgramBlock(int blockType, int number);
        int disconnectPLC();
        int execReadRequest(IPDU p, IresultSet rl);
        int execWriteRequest(IPDU p, IresultSet rl);
        int force200(int area, int start, int val);
        int forceDisconnectIBH(int src, int dest, int MPI);
        int getAnswLen();
        int getGetResponse();
        int getMaxPDULen();
        int getMessage(IPDU p);
        int getProgramBlock(int blockType, int number, byte[] buffer, ref int length);
        int getU8();
        int ListBlocksOfType(int blockType, byte[] buffer);
        IPDU prepareReadRequest();
        IPDU prepareWriteRequest();
        IPDU createPDU();
        int putProgramBlock(int blockType, int number, byte[] buffer, int length);
        int readBits(int area, int DBnumber, int start, int len, byte[] buffer);
        int readBytes(int area, int DBnumber, int start, int len, byte[] buffer);
        int readManyBytes(int area, int DBnumber, int start, int len, ref byte[] buffer);
        int readSZL(int id, int index, byte[] buffer);
        int resetIBH();
        int start();
        int stop();
        int useResult(IresultSet rs, int number, byte[] buffer);
        int writeBits(int area, int DBnumber, int start, int len, byte[] buffer);
        int writeBytes(int area, int DBnumber, int start, int len, byte[] buffer);
        int writeManyBytes(int area, int DBnumber, int start, int len, byte[] buffer);
        IresultSet getResultSet();
        int PI_StartNC(string piservice, string[] param, int paramCount);
        int initUploadNC(string file, ref byte[] uploadID);
        int doUploadNC(out int more, byte[] buffer, out int len, byte[] uploadID);
        int endUploadNC(byte[] uploadID);
        int daveGetNCProgram(string filename, byte[] buffer, ref int length);
        int davePutNCProgram(string filename, string path, string ts, byte[] buffer, int length);
    }
}