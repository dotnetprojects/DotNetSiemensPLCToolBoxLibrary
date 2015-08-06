/*
 Part of Libnodave, a free communication libray for Siemens S7 200/300/400 via
 the MPI adapter 6ES7 972-0CA22-0XAC
 or  MPI adapter 6ES7 972-0CA23-0XAC
 or  TS adapter 6ES7 972-0CA33-0XAC
 or  MPI adapter 6ES7 972-0CA11-0XAC,
 IBH/MHJ-NetLink or CPs 243, 343 and 443
 or VIPA Speed7 with builtin ethernet support.
  
 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2002..2005

 Libnodave is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 Libnodave is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
/*
    Do NOT use this include file in C programs. It is just here for people who want
    to interface Libnodave with other programming languages, so they see that they
    do not neccessarily need to implement all the internal structures.
    
    So if you plan to interface with other languages, this file shall show you
    show the minimum of structures and funtions you'll need to make known to your compiler.
*/

#ifdef __cplusplus
extern "C" {
#endif

#ifndef __nodave
#define __nodave

#ifdef LINUX
#define DECL2
#define EXPORTSPEC
typedef struct {
    int rfd;
    int wfd;
} _daveOSserialType;
#include <stdlib.h>
#else    
#ifdef BCCWIN
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#define DECL2 __stdcall
//#define DECL2 cdecl
#ifdef DOEXPORT
#define EXPORTSPEC __declspec (dllexport)
#else
#define EXPORTSPEC __declspec (dllimport)
#endif
typedef struct {
    HANDLE rfd;
    HANDLE wfd;
} _daveOSserialType;
#else
#error Fill in what you need for your OS or API.
#endif /* BCCWIN */
#endif /* LINUX */

/*
    Protocol types to be used with newInterface:
*/
#define daveProtoMPI	0	/* MPI for S7 300/400 */
#define daveProtoMPI2	1	/* MPI for S7 300/400, "Andrew's version" */
#define daveProtoMPI3	2	/* MPI for S7 300/400, Step 7 Version, not well tested */
#define daveProtoMPI4	3	/* MPI for S7 300/400, "Andrew's version" with extra STX */
#define daveProtoPPI	10	/* PPI for S7 200 */

#define daveProtoAS511	20	/* S5 programming port protocol */

#define daveProtoS7online 50	/* use s7onlinx.dll for transport */

#define daveProtoISOTCP	122	/* ISO over TCP */
#define daveProtoISOTCP243 123	/* ISO over TCP with CP243 */

#define daveProtoNLpro 230	/* MPI with NetLink Pro MPI to ethernet gateway */

#define daveProtoMPI_IBH 223	/* MPI with IBH NetLink MPI to ethernet gateway */
#define daveProtoPPI_IBH 224	/* PPI with IBH NetLink PPI to ethernet gateway */

#define daveProtoUserTransport 255	/* Libnodave will pass the PDUs of S7 Communication to user */
					/* defined call back functions. */

/*
 *    ProfiBus speed constants:
*/
#define daveSpeed9k     0
#define daveSpeed19k    1
#define daveSpeed187k   2
#define daveSpeed500k   3
#define daveSpeed1500k  4
#define daveSpeed45k    5
#define daveSpeed93k    6

/*
    Some MPI function codes (yet unused ones may be incorrect).
*/
#define daveFuncOpenS7Connection	0xF0
#define daveFuncRead			0x04
#define daveFuncWrite			0x05
#define daveFuncRequestDownload		0x1A
#define daveFuncDownloadBlock		0x1B
#define daveFuncDownloadEnded		0x1C
#define daveFuncStartUpload		0x1D
#define daveFuncUpload			0x1E
#define daveFuncEndUpload		0x1F
#define daveFuncInsertBlock		0x28
/*
    S7 specific constants:
*/
#define daveBlockType_OB  '8'
#define daveBlockType_DB  'A'
#define daveBlockType_SDB 'B'
#define daveBlockType_FC  'C'
#define daveBlockType_SFC 'D'
#define daveBlockType_FB  'E'
#define daveBlockType_SFB 'F'
/*
    Use these constants for parameter "area" in daveReadBytes and daveWriteBytes
*/    
#define daveSysInfo 0x3		/* System info of 200 family */
#define daveSysFlags  0x5	/* System flags of 200 family */
#define daveAnaIn  0x6		/* analog inputs of 200 family */
#define daveAnaOut  0x7		/* analog outputs of 200 family */

#define daveP 0x80    
#define daveInputs 0x81    
#define daveOutputs 0x82    
#define daveFlags 0x83
#define daveDB 0x84	/* data blocks */
#define daveDI 0x85	/* instance data blocks */
#define daveLocal 0x86 	/* not tested */
#define daveV 0x87	/* don't know what it is */
#define daveCounter 28	/* S7 counters */
#define daveTimer 29	/* S7 timers */
#define daveCounter200 30	/* IEC counters (200 family) */
#define daveTimer200 31		/* IEC timers (200 family) */

/**
    Library specific:
**/
/*
    Result codes. Genarally, 0 means ok, 
    >0 are results (also errors) reported by the PLC
    <0 means error reported by library code.
*/
#define daveResOK 0				/* means all ok */
#define daveResNoPeripheralAtAddress 1		/* CPU tells there is no peripheral at address */
#define daveResMultipleBitsNotSupported 6 	/* CPU tells it does not support to read a bit block with a */
						/* length other than 1 bit. */
#define daveResItemNotAvailable200 3		/* means a a piece of data is not available in the CPU, e.g. */
						/* when trying to read a non existing DB or bit bloc of length<>1 */
						/* This code seems to be specific to 200 family. */
					    
#define daveResItemNotAvailable 10		/* means a a piece of data is not available in the CPU, e.g. */
						/* when trying to read a non existing DB */

#define daveAddressOutOfRange 5			/* means the data address is beyond the CPUs address range */
#define daveWriteDataSizeMismatch 7		/* means the write data size doesn't fit item size */
#define daveResCannotEvaluatePDU -123    
#define daveResCPUNoData -124 
#define daveUnknownError -125 
#define daveEmptyResultError -126 
#define daveEmptyResultSetError -127 
#define daveResUnexpectedFunc -128 
#define daveResUnknownDataUnitSize -129

#define daveResShortPacket -1024 
#define daveResTimeout -1025 

/*
    error code to message string conversion:
    Call this function to get an explanation for error codes returned by other functions.
*/
EXPORTSPEC char * DECL2 daveStrerror(int code);
/*
    Copy an internal String into an external string buffer. This is needed to interface
    with Visual Basic. Maybe it is helpful elsewhere, too.
*/
EXPORTSPEC void DECL2 daveStringCopy(char * intString, char * extString);

/* 
    Max number of bytes in a single message. 
*/
#define daveMaxRawLen 2048
/*
    Some definitions for debugging:
*/
#define daveDebugRawRead  	0x01	/* Show the single bytes received */
#define daveDebugSpecialChars  	0x02	/* Show when special chars are read */
#define daveDebugRawWrite	0x04	/* Show the single bytes written */
#define daveDebugListReachables 0x08	/* Show the steps when determine devices in MPI net */
#define daveDebugInitAdapter 	0x10	/* Show the steps when Initilizing the MPI adapter */
#define daveDebugConnect 	0x20	/* Show the steps when connecting a PLC */
#define daveDebugPacket 	0x40
#define daveDebugByte 		0x80
#define daveDebugCompare 	0x100
#define daveDebugExchange 	0x200
#define daveDebugPDU 		0x400	/* debug PDU handling */
#define daveDebugUpload		0x800	/* debug PDU loading program blocks from PLC */
#define daveDebugMPI 		0x1000
#define daveDebugPrintErrors	0x2000	/* Print error messages */
#define daveDebugPassive 	0x4000
#define daveDebugErrorReporting	0x8000
#define daveDebugOpen		0x10000  /* print messages in openSocket and setPort */

#define daveDebugAll 0x1ffff
/*
    set and read debug level:
*/
EXPORTSPEC void DECL2 daveSetDebug(int nDebug);
EXPORTSPEC int DECL2 daveGetDebug(void);
/*
    Some data types:
*/
#define uc unsigned char
#define us unsigned short
#define u32 unsigned int

/* 
    This is a wrapper for the serial or ethernet interface. This is here to make porting easier.
*/

typedef struct _daveConnection  daveConnection;
typedef struct _daveInterface  daveInterface;

/*
    Helper struct to manage PDUs. This is NOT the part of the packet I would call PDU, but
    a set of pointers that ease access to the "private parts" of a PDU.
*/
/* While gcc-3.3 can handle a 0 size struct (at least pointers to it), BCC can't. */
/*
typedef struct {
} PDU;
*/
typedef struct {
    uc * header;	/* pointer to start of PDU (PDU header) */
    uc * param;		/* pointer to start of parameters inside PDU */
    uc * data;		/* pointer to start of data inside PDU */
    uc * udata;		/* pointer to start of data inside PDU */
    int hlen;		/* header length */
    int plen;		/* parameter length */
    int dlen;		/* data length */
    int udlen;		/* user or result data length */
} PDU;



/* 
    This groups an interface together with some information about it's properties
    in the library's context.
*/
struct _daveInterface {
    int _timeout;	/* Timeout in microseconds used in transort. */
};

EXPORTSPEC daveInterface * DECL2 daveNewInterface(_daveOSserialType nfd, char * nname, int localMPI, int protocol, int speed);

/* 
    This holds data for a PLC connection;
*/
struct _daveConnection {
    int AnswLen;	/* length of last message */
    uc * resultPointer;	/* used to retrieve single values from the result byte array */
    int maxPDUlength;
//    int MPIAdr;		/* The PLC's address */
}; 

/* 
    Setup a new connection structure using an initialized
    daveInterface and PLC's MPI address.
*/
EXPORTSPEC daveConnection * DECL2 daveNewConnection(daveInterface * di, int MPI,int rack, int slot);

typedef struct {
    uc type[2];
    unsigned short count;
} daveBlockTypeEntry;

typedef struct {
    unsigned short number;
    uc type[2];
} daveBlockEntry;
/**
    PDU handling:
    PDU is the central structure present in S7 communication.
    It is composed of a 10 or 12 byte header,a parameter block and a data block.
    When reading or writing values, the data field is itself composed of a data
    header followed by payload data
**/
/*
    retrieve the answer
*/
EXPORTSPEC int DECL2 daveGetResponse(daveConnection * dc);
/*
    send PDU to PLC
*/
EXPORTSPEC int DECL2 daveSendMessage(daveConnection * dc, PDU * p);

/******
    
    Utilities:
    
****/
/*
    Hex dump PDU:
*/
EXPORTSPEC void DECL2 _daveDumpPDU(PDU * p);

/*
    Hex dump. Write the name followed by len bytes written in hex and a newline:
*/
EXPORTSPEC void DECL2 _daveDump(char * name,uc*b,int len);

/*
    names for PLC objects:
*/
EXPORTSPEC char * DECL2 daveBlockName(uc bn);
EXPORTSPEC char * DECL2 daveAreaName(uc n);

/*
    swap functions:
*/
EXPORTSPEC short DECL2 daveSwapIed_16(short ff);
EXPORTSPEC int DECL2 daveSwapIed_32(int ff);

/**
    Data conversion convenience functions. The older set has been removed.
    Newer conversion routines. As the terms WORD, INT, INTEGER etc have different meanings
    for users of different programming languages and compilers, I choose to provide a new 
    set of conversion routines named according to the bit length of the value used. The 'U'
    or 'S' stands for unsigned or signed.
**/
/*
    Get a value from the position b points to. B is typically a pointer to a buffer that has
    been filled with daveReadBytes:
*/
EXPORTSPEC float DECL2 daveGetFloatAt(daveConnection * dc, int pos);


EXPORTSPEC float DECL2 toPLCfloat(float ff);
EXPORTSPEC int DECL2 daveToPLCfloat(float ff);



EXPORTSPEC int DECL2 daveGetS8from(uc *b);
EXPORTSPEC int DECL2 daveGetU8from(uc *b);
EXPORTSPEC int DECL2 daveGetS16from(uc *b);
EXPORTSPEC int DECL2 daveGetU16from(uc *b);
EXPORTSPEC int DECL2 daveGetS32from(uc *b);
EXPORTSPEC unsigned int DECL2 daveGetU32from(uc *b);
EXPORTSPEC float DECL2 daveGetFloatfrom(uc *b);
/*
    Get a value from the current position in the last result read on the connection dc.
    This will increment an internal pointer, so the next value is read from the position
    following this value.
*/

EXPORTSPEC int DECL2 daveGetS8(daveConnection * dc);
EXPORTSPEC int DECL2 daveGetU8(daveConnection * dc);
EXPORTSPEC int DECL2 daveGetS16(daveConnection * dc);
EXPORTSPEC int DECL2 daveGetU16(daveConnection * dc);
EXPORTSPEC int DECL2 daveGetS32(daveConnection * dc);
EXPORTSPEC unsigned int DECL2 daveGetU32(daveConnection * dc);
EXPORTSPEC float DECL2 daveGetFloat(daveConnection * dc);
/*
    Get a value from a given position in the last result read on the connection dc.
*/
EXPORTSPEC int DECL2 daveGetS8At(daveConnection * dc, int pos);
EXPORTSPEC int DECL2 daveGetU8At(daveConnection * dc, int pos);
EXPORTSPEC int DECL2 daveGetS16At(daveConnection * dc, int pos);
EXPORTSPEC int DECL2 daveGetU16At(daveConnection * dc, int pos);
EXPORTSPEC int DECL2 daveGetS32At(daveConnection * dc, int pos);
EXPORTSPEC unsigned int DECL2 daveGetU32At(daveConnection * dc, int pos);
/*
    put one byte into buffer b:
*/
EXPORTSPEC uc * DECL2 davePut8(uc *b,int v);
EXPORTSPEC uc * DECL2 davePut16(uc *b,int v);
EXPORTSPEC uc * DECL2 davePut32(uc *b,int v);
EXPORTSPEC uc * DECL2 davePutFloat(uc *b,float v);

EXPORTSPEC void DECL2 davePut8At(uc *b, int pos, int v);
EXPORTSPEC void DECL2 davePut16At(uc *b, int pos, int v);
EXPORTSPEC void DECL2 davePut32At(uc *b, int pos, int v);
EXPORTSPEC void DECL2 davePutFloatAt(uc *b,int pos, float v);
/**
    Timer and Counter conversion functions:
**/
/*	
    get time in seconds from current read position:
*/
EXPORTSPEC float DECL2 daveGetSeconds(daveConnection * dc);
/*	
    get time in seconds from random position:
*/
EXPORTSPEC float DECL2 daveGetSecondsAt(daveConnection * dc, int pos);
/*	
    get counter value from current read position:
*/
EXPORTSPEC int DECL2 daveGetCounterValue(daveConnection * dc);
/*	
    get counter value from random read position:
*/
EXPORTSPEC int DECL2 daveGetCounterValueAt(daveConnection * dc,int pos);

/*
    Functions to load blocks from PLC:
*/
EXPORTSPEC void DECL2 _daveConstructUpload(PDU *p,char blockType, int blockNr);

EXPORTSPEC void DECL2 _daveConstructDoUpload(PDU * p, int uploadID);

EXPORTSPEC void DECL2 _daveConstructEndUpload(PDU * p, int uploadID);
/*
    Get the PLC's order code as ASCIIZ. Buf must provide space for
    21 characters at least.
*/

#define daveOrderCodeSize 21
EXPORTSPEC int DECL2 daveGetOrderCode(daveConnection * dc,char * buf);
/*
    connect to a PLC. returns 0 on success.
*/

EXPORTSPEC int DECL2 daveConnectPLC(daveConnection * dc);

/* 
    Read len bytes from the PLC. Start determines the first byte.
    Area denotes whether the data comes from FLAGS, DATA BLOCKS,
    INPUTS or OUTPUTS, etc. 
    DB is the number of the data block to be used. Set it to zero
    for other area types.
    Buffer is a pointer to a memory block provided by the calling
    program. If the pointer is not NULL, the result data will be copied thereto.
    Hence it must be big enough to take up the result.
    In any case, you can also retrieve the result data using the get<type> macros
    on the connection pointer.
    
    RESTRICTION:There is no check for max. message len or automatic splitting into 
		multiple messages. Use daveReadManyBytes() in case the data you want
		to read doesn't fit into a single PDU.
		
*/
EXPORTSPEC int DECL2 daveReadBytes(daveConnection * dc, int area, int DB, int start, int len, void * buffer);

/* 
    Read len bytes from the PLC. Start determines the first byte.
    In contrast to daveReadBytes(), this function can read blocks 
    that are too long for a single transaction. To achieve this,
    the data is fetched with multiple subsequent read requests to
    the CPU.
    Area denotes whether the data comes from FLAGS, DATA BLOCKS,
    INPUTS or OUTPUTS, etc. 
    DB is the number of the data block to be used. Set it to zero
    for other area types.
    Buffer is a pointer to a memory block provided by the calling
    program. It may not be NULL, the result data will be copied thereto.
    Hence it must be big enough to take up the result.
    You CANNOT read result bytes from the internal buffer of the
    daveConnection. This buffer is overwritten in each read request.
*/
EXPORTSPEC int DECL2 daveReadManyBytes(daveConnection * dc,int area, int DBnum, int start,int len, void * buffer);

/* 
    Write len bytes from buffer to the PLC. 
    Start determines the first byte.
    Area denotes whether the data goes to FLAGS, DATA BLOCKS,
    INPUTS or OUTPUTS, etc.
    DB is the number of the data block to be used. Set it to zero
    for other area types.
    RESTRICTION: There is no check for max. message len or automatic splitting into 
		 multiple messages. Use daveReadManyBytes() in case the data you want
		 to read doesn't fit into a single PDU.

*/
EXPORTSPEC int DECL2 daveWriteBytes(daveConnection * dc,int area, int DB, int start, int len, void * buffer);

/* 
    Write len bytes to the PLC. Start determines the first byte.
    In contrast to daveWriteBytes(), this function can write blocks 
    that are too long for a single transaction. To achieve this, the
    the data is transported with multiple subsequent write requests to
    the CPU.
    Area denotes whether the data comes from FLAGS, DATA BLOCKS,
    INPUTS or OUTPUTS, etc. 
    DB is the number of the data block to be used. Set it to zero
    for other area types.
    Buffer is a pointer to a memory block provided by the calling
    program. It may not be NULL.
*/
EXPORTSPEC int DECL2 daveWriteManyBytes(daveConnection * dc,int area, int DB, int start, int len, void * buffer);

/* 
    Bit manipulation:
*/
EXPORTSPEC int DECL2 daveReadBits(daveConnection * dc, int area, int DB, int start, int len, void * buffer);
EXPORTSPEC int DECL2 daveWriteBits(daveConnection * dc,int area, int DB, int start, int len, void * buffer);
EXPORTSPEC int DECL2 daveSetBit(daveConnection * dc,int area, int DB, int byteAdr, int bitAdr);
EXPORTSPEC int DECL2 daveClrBit(daveConnection * dc,int area, int DB, int byteAdr, int bitAdr);

/*
    PLC diagnostic and inventory functions:
*/
EXPORTSPEC int DECL2 daveReadSZL(daveConnection * dc, int ID, int index, void * buf, int buflen);
EXPORTSPEC int DECL2 daveListBlocksOfType(daveConnection * dc,uc type,daveBlockEntry * buf);
EXPORTSPEC int DECL2 daveListBlocks(daveConnection * dc,daveBlockTypeEntry * buf);
/*
    PLC program read functions:
*/
EXPORTSPEC int DECL2 initUpload(daveConnection * dc,char blockType, int blockNr, int * uploadID);
EXPORTSPEC int DECL2 doUpload(daveConnection*dc, int * more, uc**buffer, int*len, int uploadID);
EXPORTSPEC int DECL2 endUpload(daveConnection*dc, int uploadID);
EXPORTSPEC int DECL2 daveGetProgramBlock(daveConnection * dc, int blockType, int number, char* buffer, int * length);
/*
    PLC run/stop control functions:
*/
EXPORTSPEC int DECL2 daveStop(daveConnection*dc);
EXPORTSPEC int DECL2 daveStart(daveConnection*dc);
/*
    PLC special commands
*/    
EXPORTSPEC int DECL2 daveCopyRAMtoROM(daveConnection*dc);
	
EXPORTSPEC int DECL2 daveForce200(daveConnection * dc, int area, int start, int val);
/*
    Multiple variable support:
*/
typedef struct {
    int error;
    int length;
    uc * bytes;
} daveResult;

typedef struct {
    int numResults;
    daveResult * results;
} daveResultSet;


/* use this to initialize a multivariable read: */
EXPORTSPEC void DECL2 davePrepareReadRequest(daveConnection * dc, PDU *p);
/* Adds a new variable to a prepared request: */
EXPORTSPEC void DECL2 daveAddVarToReadRequest(PDU *p, int area, int DBnum, int start, int bytes);
/* Executes the complete request. */
EXPORTSPEC int DECL2 daveExecReadRequest(daveConnection * dc, PDU *p, daveResultSet * rl);
/* Lets the functions daveGet<data type> work on the n-th result: */
EXPORTSPEC int DECL2 daveUseResult(daveConnection * dc, daveResultSet * rl, int n);
/* Frees the memory occupied by the result structure */
EXPORTSPEC void DECL2 daveFreeResults(daveResultSet * rl);
/* Adds a new bit variable to a prepared request: */
EXPORTSPEC void DECL2 daveAddBitVarToReadRequest(PDU *p, int area, int DBnum, int start, int byteCount);

/* use this to initialize a multivariable write: */
EXPORTSPEC void DECL2 davePrepareWriteRequest(daveConnection * dc, PDU *p);
/* Adds a new variable to a prepared request: */
EXPORTSPEC void DECL2 daveAddVarToWriteRequest(PDU *p, int area, int DBnum, int start, int bytes, void * buffer);
/* Adds a new bit variable to a prepared write request: */
EXPORTSPEC void DECL2 daveAddBitVarToWriteRequest(PDU *p, int area, int DBnum, int start, int byteCount, void * buffer);
/* Executes the complete request. */
EXPORTSPEC int DECL2 daveExecWriteRequest(daveConnection * dc, PDU *p, daveResultSet * rl);


EXPORTSPEC int DECL2 daveInitAdapter(daveInterface * di);
EXPORTSPEC int DECL2 daveConnectPLC(daveConnection * dc);
EXPORTSPEC int DECL2 daveDisconnectPLC(daveConnection * dc);

EXPORTSPEC int DECL2 daveDisconnectAdapter(daveInterface * di);
EXPORTSPEC int DECL2 daveListReachablePartners(daveInterface * di,char * buf);

/* MPI specific functions */

#define daveMPIReachable 0x30
#define daveMPIActive 0x30
#define daveMPIPassive 0x00
#define daveMPIunused 0x10
#define davePartnerListSize 126
/**
    C# interoperability:
**/
EXPORTSPEC void DECL2 daveSetTimeout(daveInterface * di, int tmo);
EXPORTSPEC int DECL2 daveGetTimeout(daveInterface * di);

EXPORTSPEC char * DECL2 daveGetName(daveInterface * di);

EXPORTSPEC int DECL2 daveGetMPIAdr(daveConnection * dc);
EXPORTSPEC int DECL2 daveGetAnswLen(daveConnection * dc);
EXPORTSPEC int DECL2 daveGetMaxPDULen(daveConnection * dc);
EXPORTSPEC daveResultSet * DECL2 daveNewResultSet();
EXPORTSPEC void DECL2 daveFree(void * dc);
EXPORTSPEC PDU * DECL2 daveNewPDU();
EXPORTSPEC int DECL2 daveGetErrorOfResult(daveResultSet *,int number);

/*
    Special function do disconnect arbitrary connections on IBH-Link:
*/
EXPORTSPEC int DECL2 daveForceDisconnectIBH(daveInterface * di, int src, int dest, int mpi);
/*
    Special function do reset an IBH-Link:
*/
EXPORTSPEC int DECL2 daveResetIBH(daveInterface * di);
/**
    Program Block from PLC:
*/
EXPORTSPEC int DECL2 daveGetProgramBlock(daveConnection * dc, int blockType, int number, char* buffer, int * length);
/**
    PLC realtime clock handling:
*/ 
/*
    read out clock:
*/ 
EXPORTSPEC int DECL2 daveReadPLCTime(daveConnection * dc);
/*
    set clock to a value given by user:
*/ 
EXPORTSPEC int DECL2 daveSetPLCTime(daveConnection * dc,uc * ts);
/*
    set clock to PC system clock:
*/ 
EXPORTSPEC int DECL2 daveSetPLCTimeToSystime(daveConnection * dc);

EXPORTSPEC uc DECL2 daveToBCD(uc i);
EXPORTSPEC uc DECL2 daveFromBCD(uc i);

#endif /* _nodave */

#ifdef __cplusplus
 }
#endif


/*
    Changes: 
    04/10/05  first version.
    09/11/05  added read/write functions for long blocks of data.
    10/26/07  fixed __cplusplus
*/
