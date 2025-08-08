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

#ifdef __cplusplus

extern "C" {
#endif

#ifndef __nodave
#define __nodave

#define daveSerialConnection 0
#define daveTcpConnection 1
#define daveS7OnlineConnection 2

#ifdef LINUX
#define DECL2
#define EXPORTSPEC
	typedef struct dost {
		int rfd;
		int wfd;
		//    int connectionType;
	} _daveOSserialType;
#include <stdlib.h>
#define tmotype int
#define OS_KNOWN	// get rid of nested ifdefs.
#endif    

#ifdef BCCWIN
#define WIN32_LEAN_AND_MEAN
#include <winsock2.h>
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#define DECL2 __stdcall
#define tmotype int

#ifdef DOEXPORT
#define EXPORTSPEC __declspec (dllexport) 
#else
#define EXPORTSPEC __declspec (dllimport) 
#endif

	typedef struct dost {
		HANDLE rfd;
		HANDLE wfd;
		//    int connectionType;
	} _daveOSserialType;

#define OS_KNOWN
#endif

#ifdef DOS
#include <stdio.h>
#include <stdlib.h>
	//#define DECL2 WINAPI
	// #define DECL2
#define DECL2 __cedcl
#define EXPORTSPEC

	typedef struct dost {
		int rfd;
		int wfd;
		int connectionType;
	} _daveOSserialType;
#define OS_KNOWN
#endif

#ifdef AVR
#include <stdio.h>
#include <stdlib.h>
#define DECL2
#define EXPORTSPEC
	typedef struct dost {
		int rfd;
		int wfd;
		int connectionType;
	} _daveOSserialType;
#define tmotype long
#define OS_KNOWN
#endif

#ifndef OS_KNOWN
#error Fill in what you need for your OS or API.
#endif

	/*
		some frequently used ASCII control codes:
		*/
#define DLE 0x10
#define ETX 0x03
#define STX 0x02
#define SYN 0x16
#define NAK 0x15
#define EOT 0x04	//  for S5
#define ACK 0x06	//  for S5
	/*
		Protocol types to be used with newInterface:
		*/
#define daveProtoMPI	0	/* MPI for S7 300/400 */
#define daveProtoMPI2	1	/* MPI for S7 300/400, "Andrew's version" without STX */
#define daveProtoMPI3	2	/* MPI for S7 300/400, Step 7 Version, not yet implemented */
#define daveProtoMPI4	3	/* MPI for S7 300/400, "Andrew's version" with STX */

#define daveProtoPPI	10	/* PPI for S7 200 */

#define daveProtoAS511	20	/* S5 programming port protocol */

#define daveProtoS7online 50	/* use s7onlinx.dll for transport */

#define daveProtoISOTCP	122	/* ISO over TCP */
#define daveProtoISOTCP243 123	/* ISO over TCP with CP243 */
#define daveProtoISOTCPR 124	/* ISO over TCP with Routing */

#define daveProtoMPI_IBH 223	/* MPI with IBH NetLink MPI to ethernet gateway */
#define daveProtoPPI_IBH 224	/* PPI with IBH NetLink PPI to ethernet gateway */

#define daveProtoNLPro 230	/* MPI with NetLink 50 MPI to ethernet gateway */

#define daveProtoNLProFamily 231	/* MPI with NetLink 50 MPI to ethernet gateway */

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

#define daveS5BlockType_DB  0x01
#define daveS5BlockType_SB  0x02
#define daveS5BlockType_PB  0x04
#define daveS5BlockType_FX  0x05
#define daveS5BlockType_FB  0x08
#define daveS5BlockType_DX  0x0C
#define daveS5BlockType_OB  0x10

	/*
		Use these constants for parameter "area" in daveReadBytes and daveWriteBytes
		*/
#define daveSysInfo 0x3		/* System info of 200 family */
#define daveSysFlags  0x5	/* System flags of 200 family */
#define daveAnaIn  0x6		/* analog inputs of 200 family */
#define daveAnaOut  0x7		/* analog outputs of 200 family */

#define daveP 0x80    		/* direct peripheral access */
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
#define daveSysDataS5 0x86	/* system data area ? */
#define daveRawMemoryS5 0		/* just the raw memory */

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
#define daveResCannotEvaluatePDU -123    	/* PDU is not understood by libnodave */
#define daveResCPUNoData -124 
#define daveUnknownError -125 
#define daveEmptyResultError -126 
#define daveEmptyResultSetError -127 
#define daveResUnexpectedFunc -128 
#define daveResUnknownDataUnitSize -129
#define daveResNoBuffer -130
#define daveNotAvailableInS5 -131
#define daveResInvalidLength -132
#define daveResInvalidParam -133
#define daveResNotYetImplemented -134

#define daveResShortPacket -1024 
#define daveResTimeout -1025 

	/*
		Error code to message string conversion:
		Call this function to get an explanation for error codes returned by other functions.
		*/
	EXPORTSPEC char * DECL2 daveStrerror(int code); // result is char because this is usual for strings

	/*
		Copy an internal String into an external string buffer. This is needed to interface
		with Visual Basic. Maybe it is helpful elsewhere, too.
		*/
	EXPORTSPEC void DECL2 daveStringCopy(char * intString, char * extString); // args are char because this is usual for strings


	/*
		Max number of bytes in a single message.
		An upper limit for MPI over serial is:
		8		transport header
		+2*240	max PDU len *2 if every character were a DLE
		+3		DLE,ETX and BCC
		= 491

		Later I saw some programs offering up to 960 bytes in PDU size negotiation

		Max number of bytes in a single message.
		An upper limit for MPI over serial is:
		8		transport header
		+2*960	max PDU len *2 if every character were a DLE
		+3		DLE,ETX and BCC
		= 1931

		For now, we take the rounded max of all this to determine our buffer size. This is ok
		for PC systems, where one k less or more doesn't matter.
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
	  IBH-NetLink packet types:
	  */
#define _davePtEmpty -2
#define _davePtMPIAck -3
#define _davePtUnknownMPIFunc -4
#define _davePtUnknownPDUFunc -5
#define _davePtReadResponse 1
#define _davePtWriteResponse 2

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
		Definitions of prototypes for the protocol specific functions. The library "switches"
		protocol by setting pointers to the protol specific implementations.
		*/
	typedef int (DECL2 *  _initAdapterFunc) (daveInterface *);
	typedef int (DECL2 *  _connectPLCFunc) (daveConnection *);
	typedef int (DECL2 * _disconnectPLCFunc) (daveConnection *);
	typedef int (DECL2 * _disconnectAdapterFunc) (daveInterface *);
	typedef int (DECL2 * _exchangeFunc) (daveConnection *, PDU *);
	typedef int (DECL2 * _sendMessageFunc) (daveConnection *, PDU *);
	typedef int (DECL2 * _getResponseFunc) (daveConnection *);
	typedef int (DECL2 * _listReachablePartnersFunc) (daveInterface * di, char * buf); // changed to unsigned char because it is a copy of an uc buffer

	/*
		Definitions of prototypes for i/O functions.
		*/
	typedef int (DECL2 *  _writeFunc) (daveInterface *, char *, int); // changed to char because char is what system read/write expects
	typedef int (DECL2 *  _readFunc) (daveInterface *, char *, int);

	/*
		This groups an interface together with some information about it's properties
		in the library's context.
		*/
	struct _daveInterface {
		tmotype timeout;	/* Timeout in microseconds used in transort. */
		_daveOSserialType fd; /* some handle for the serial interface */
		int localMPI;	/* the adapter's MPI address */

		int users;		/* a counter used when multiple PLCs are accessed via */
		/* the same serial interface and adapter. */
		char * name;	/* just a name that can be used in programs dealing with multiple */
		/* daveInterfaces */
		int protocol;	/* The kind of transport protocol used on this interface. */
		int speed;		/* The MPI or Profibus speed */
		int ackPos;		/* position of some packet number that has to be repeated in ackknowledges */
		int nextConnection;
		_initAdapterFunc initAdapter;		/* pointers to the protocol */
		_connectPLCFunc connectPLC;			/* specific implementations */
		_disconnectPLCFunc disconnectPLC;		/* of these functions */
		_disconnectAdapterFunc disconnectAdapter;
		_exchangeFunc exchange;
		_sendMessageFunc sendMessage;
		_getResponseFunc getResponse;
		_listReachablePartnersFunc listReachablePartners;
		char realName[20];
		_readFunc ifread;
		_writeFunc ifwrite;
		int seqNumber;
	};

	EXPORTSPEC daveInterface * DECL2 daveNewInterface(_daveOSserialType nfd, char * nname, int localMPI, int protocol, int speed);
	EXPORTSPEC daveInterface * DECL2 davePascalNewInterface(_daveOSserialType* nfd, char * nname, int localMPI, int protocol, int speed);
	/*
		This is the packet header used by IBH ethernet NetLink.
		*/
	typedef struct {
		uc ch1;	// logical connection or channel ?
		uc ch2;	// logical connection or channel ?
		uc len;	// number of bytes counted from the ninth one.
		uc packetNumber;	// a counter, response packets refer to request packets
		us sFlags;		// my guess
		us rFlags;		// my interpretation
	} IBHpacket;

	/*
		Header for MPI packets on IBH-NetLink:
		*/

	typedef struct {
		uc src_conn;
		uc dst_conn;
		uc MPI;
		uc localMPI;
		uc len;
		uc func;
		uc packetNumber;
	} MPIheader;

	typedef struct {
		uc src_conn;
		uc dst_conn;
		uc MPI;
		uc xxx1;
		uc xxx2;
		uc xx22;
		uc len;
		uc func;
		uc packetNumber;
	}  MPIheader2;

	typedef struct _daveS5AreaInfo  {
		int area;
		int DBnumber;
		int address;
		int len;
		struct _daveS5AreaInfo * next;
	} daveS5AreaInfo;

	typedef struct _daveS5cache {
		int PAE;	// start of inputs
		int PAA;	// start of outputs
		int flags;	// start of flag (marker) memory
		int timers;	// start of timer memory
		int counters;// start of counter memory
		int systemData;// start of system data
		daveS5AreaInfo * first;
	} daveS5cache;

	/*
		This holds data for a PLC connection;
		*/

	struct _daveConnection {
		int AnswLen;	/* length of last message */
		uc * resultPointer;	/* used to retrieve single values from the result byte array */
		int maxPDUlength;
		int MPIAdr;		/* The PLC's address */
		daveInterface * iface; /* pointer to used interface */
		int needAckNumber;	/* message number we need ackknowledge for */
		int PDUnumber; 	/* current PDU number */
		int ibhSrcConn;
		int ibhDstConn;
		uc msgIn[daveMaxRawLen];
		uc msgOut[daveMaxRawLen];
		uc * _resultPointer;
		int PDUstartO;	/* position of PDU in outgoing messages. This is different for different transport methodes. */
		int PDUstartI;	/* position of PDU in incoming messages. This is different for different transport methodes. */
		int rack;		/* rack number for ISO over TCP */
		int slot;		/* slot number for ISO over TCP */
		int connectionNumber;
		int connectionNumber2;
		uc 	messageNumber;  /* current MPI message number */
		uc	packetNumber;	/* packetNumber in transport layer */
		void * hook;	/* used in CPU/CP simulation: pointer to the rest we have to send if message doesn't fit in a single packet */
		daveS5cache * cache; /* used in AS511: We cache addresses of memory areas and datablocks here */

		int TPDUsize; 		// size of TPDU for ISO over TCP
		int partPos;  		// remember position for ISO over TCP fragmentation

		uc application_block_subsystem; /* used in S7Online */

		int DestinationIsIP;

		int _Destination1;
		int _Destination2;
		int _Destination3;
		int _Destination4;
		int _DestinationSize;

		int ConnectionType; // 1=PG Conn, 2=OP Conn

		int routing;
		unsigned int routingSubnetFirst;
		unsigned int routingSubnetSecond;
		int routingRack;
		int routingSlot;
		int routingDestinationIsIP;
		int routingConnectionType; // 1=PG Conn, 2=OP Conn

		int _routingDestination1;
		int _routingDestination2;
		int _routingDestination3;
		int _routingDestination4;
		int _routingDestinationSize;
	};


	/*
		Setup a new connection structure using an initialized
		daveInterface and PLC's MPI address.
		*/
	EXPORTSPEC
		daveConnection * DECL2 daveNewConnection(daveInterface * di, int MPI, int rack, int slot);

	/*
		Setup a new Extended Connection (or a TCPIP Connection to
		*/
	EXPORTSPEC
		daveConnection * DECL2 daveNewExtendedConnection(daveInterface * di, void * Destination, int DestinationIsIP, int rack, int slot, int routing, int routingSubnetFirst, int routingSubnetSecond, int routingRack, int routingSlot, void * routingDestination, int routingDestinationIsIP, int ConnectionType, int routingConnectionType);

	/*
		Setup a new connection structure using an daveConnection Structure
		*/
	daveConnection * DECL2 _daveNewConnection(daveConnection * dc);

	typedef struct {
		uc type[2];
		unsigned short count;
	} daveBlockTypeEntry;

	typedef struct {
		unsigned short number;
		uc type[2];
	} daveBlockEntry;

	typedef struct {
		uc type[2];
		uc x1[2];  /* 00 4A */
		uc w1[2];  /* some word var? */
		char pp[2]; /* allways 'pp' */
		uc x2[4];  /* 00 4A */
		unsigned short number; /* the block's number */
		uc x3[26];  /* ? */
		unsigned short length; /* the block's length */
		uc x4[16];
		uc name[8];
		uc x5[12];
	} daveBlockInfo;
	/**
		PDU handling:
		PDU is the central structure present in S7 communication.
		It is composed of a 10 or 12 byte header,a parameter block and a data block.
		When reading or writing values, the data field is itself composed of a data
		header followed by payload data
		**/
	typedef struct {
		uc P;	/* allways 0x32 */
		uc type;	/* Header type, one of 1,2,3 or 7. type 2 and 3 headers are two bytes longer. */
		uc a, b;	/* currently unknown. Maybe it can be used for long numbers? */
		us number;	/* A number. This can be used to make sure a received answer */
		/* corresponds to the request with the same number. */
		us plen;	/* length of parameters which follow this header */
		us dlen;	/* length of data which follow the parameters */
		uc result[2]; /* only present in type 2 and 3 headers. This contains error information. */
	} PDUHeader;

	/*
		same as above, but made up of single bytes only, so that every single byte can be adressed separately
		*/
	typedef struct {
		uc P;	/* allways 0x32 */
		uc type;	/* Header type, one of 1,2,3 or 7. type 2 and 3 headers are two bytes longer. */
		uc a, b;	/* currently unknown. Maybe it can be used for long numbers? */
		uc numberHi, numberLo;	/* A number. This can be used to make sure a received answer */
		/* corresponds to the request with the same number. */
		uc plenHi, plenLo;	/* length of parameters which follow this header */
		uc dlenHi, dlenLo;	/* length of data which follow the parameters */
		uc result[2]; /* only present in type 2 and 3 headers. This contains error information. */
	} PDUHeader2;

	/*
		set up the header. Needs valid header pointer in the struct p points to.
		*/
	EXPORTSPEC void DECL2 _daveInitPDUheader(PDU * p, int type);
	/*
		add parameters after header, adjust pointer to data.
		needs valid header
		*/
	EXPORTSPEC void DECL2 _daveAddParam(PDU * p, uc * param, us len);
	/*
		add data after parameters, set dlen
		needs valid header,and valid parameters.
		*/
	EXPORTSPEC void DECL2 _daveAddData(PDU * p, void * data, int len);
	/*
		add values after value header in data, adjust dlen and data count.
		needs valid header,parameters,data,dlen
		*/
	EXPORTSPEC void DECL2 _daveAddValue(PDU * p, void * data, int len);
	/*
		add data in user data. Add a user data header, if not yet present.
		*/
	EXPORTSPEC void DECL2 _daveAddUserData(PDU * p, uc * da, int len);
	/*
		set up pointers to the fields of a received message
		*/
	EXPORTSPEC int DECL2 _daveSetupReceivedPDU(daveConnection * dc, PDU * p);
	/*
		Get the eror code from a PDU, if one.
		*/
	EXPORTSPEC int DECL2 daveGetPDUerror(PDU * p);
	/*
		send PDU to PLC and retrieve the answer
		*/
	EXPORTSPEC int DECL2 _daveExchange(daveConnection * dc, PDU *p);
	/*
		retrieve the answer
		*/
	EXPORTSPEC int DECL2 daveGetResponse(daveConnection * dc);
	/*
		send PDU to PLC
		*/
	EXPORTSPEC int DECL2 daveSendMessage(daveConnection * dc, PDU * p);

	EXPORTSPEC int DECL2 daveGetPDUData(daveConnection * dc, PDU*p2, uc* data, int* ldata, uc* param, int* lparam);

	/******

		Utilities:

		****/
	/*
		Hex dump PDU:
		*/
	EXPORTSPEC void DECL2 _daveDumpPDU(PDU * p);

	/*
		This is an extended memory compare routine. It can handle don't care and stop flags
		in the sample data. A stop flag lets it return success, if there were no mismatches
		up to this point.
		*/
	EXPORTSPEC int DECL2 _daveMemcmp(us * a, uc *b, size_t len);

	/*
		Hex dump. Write the name followed by len bytes written in hex and a newline:
		*/
	//EXPORTSPEC void DECL2 _daveDump(char * name, uc *b, int len);
	EXPORTSPEC void DECL2 _daveDump(char * name, void *b, int len);

	/*
		names for PLC objects:
		*/
	EXPORTSPEC char * DECL2 daveBlockName(uc bn);  // char or uc,to decide
	EXPORTSPEC char * DECL2 daveAreaName(uc n); // to decide

	/*
		swap functions. These swap function do a swao on little endian machines only:
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
	EXPORTSPEC float DECL2 daveGetKGAt(daveConnection * dc, int pos);


	EXPORTSPEC float DECL2 toPLCfloat(float ff);
	EXPORTSPEC int DECL2 daveToPLCfloat(float ff);
	EXPORTSPEC int DECL2 daveToKG(float ff);


	EXPORTSPEC int DECL2 daveGetS8from(uc *b);
	EXPORTSPEC int DECL2 daveGetU8from(uc *b);
	EXPORTSPEC int DECL2 daveGetS16from(uc *b);
	EXPORTSPEC int DECL2 daveGetU16from(uc *b);
	EXPORTSPEC int DECL2 daveGetS32from(uc *b);
	EXPORTSPEC unsigned int DECL2 daveGetU32from(uc *b);
	EXPORTSPEC float DECL2 daveGetFloatfrom(uc *b);
	EXPORTSPEC float DECL2 daveGetKGfrom(uc *b);
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
	EXPORTSPEC float DECL2 daveGetKG(daveConnection * dc);
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
	EXPORTSPEC uc * DECL2 davePut8(uc *b, int v);
	EXPORTSPEC uc * DECL2 davePut16(uc *b, int v);
	EXPORTSPEC uc * DECL2 davePut32(uc *b, int v);
	EXPORTSPEC uc * DECL2 davePutFloat(uc *b, float v);
	EXPORTSPEC uc * DECL2 davePutKG(uc *b, float v);
	EXPORTSPEC void DECL2 davePut8At(uc *b, int pos, int v);
	EXPORTSPEC void DECL2 davePut16At(uc *b, int pos, int v);
	EXPORTSPEC void DECL2 davePut32At(uc *b, int pos, int v);
	EXPORTSPEC void DECL2 davePutFloatAt(uc *b, int pos, float v);
	EXPORTSPEC void DECL2 davePutKGAt(uc *b, int pos, float v);
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
	EXPORTSPEC int DECL2 daveGetCounterValueAt(daveConnection * dc, int pos);

	/*
		Functions to load blocks from PLC:
		*/
	EXPORTSPEC void DECL2 _daveConstructUpload(PDU *p, char blockType, int blockNr); // char or uc,to decide
	EXPORTSPEC void DECL2 _daveConstructDoUpload(PDU * p, uc *uploadID);
	EXPORTSPEC void DECL2 _daveConstructEndUpload(PDU * p, uc *uploadID);

	/*
		Functions to load files from NC:
		*/
	EXPORTSPEC void DECL2 _daveConstructUploadNC(PDU *p, const char *filename);
	EXPORTSPEC void DECL2 _daveConstructDoUploadNC(PDU * p, uc *uploadID);
	EXPORTSPEC void DECL2 _daveConstructEndUploadNC(PDU * p, uc *uploadID);
	/*
		Get the PLC's order code as ASCIIZ. Buf must provide space for
		21 characters at least.
		*/

#define daveOrderCodeSize 21
	EXPORTSPEC int DECL2 daveGetOrderCode(daveConnection * dc, char * buf); // char, users buffer, or to decide

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
	EXPORTSPEC int DECL2 daveReadManyBytes(daveConnection * dc, int area, int DBnum, int start, int len, void * buffer);

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
	EXPORTSPEC int DECL2 daveWriteBytes(daveConnection * dc, int area, int DB, int start, int len, void * buffer);

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
	EXPORTSPEC int DECL2 daveWriteManyBytes(daveConnection * dc, int area, int DB, int start, int len, void * buffer);
	/*
		Bit manipulation:
		*/
	EXPORTSPEC int DECL2 daveReadBits(daveConnection * dc, int area, int DB, int start, int len, void * buffer);
	EXPORTSPEC int DECL2 daveWriteBits(daveConnection * dc, int area, int DB, int start, int len, void * buffer);
	EXPORTSPEC int DECL2 daveSetBit(daveConnection * dc, int area, int DB, int byteAdr, int bitAdr);
	EXPORTSPEC int DECL2 daveClrBit(daveConnection * dc, int area, int DB, int byteAdr, int bitAdr);

	/*
		PLC diagnostic and inventory functions:
		*/
	EXPORTSPEC int DECL2 daveBuildAndSendPDU(daveConnection * dc, PDU*p2, uc *pa, int psize, uc *ud, int usize);
	EXPORTSPEC int DECL2 daveReadSZL(daveConnection * dc, int ID, int index, void * buf, int buflen);
	EXPORTSPEC int DECL2 daveListBlocksOfType(daveConnection * dc, uc type, daveBlockEntry * buf);
	EXPORTSPEC int DECL2 daveListBlocks(daveConnection * dc, daveBlockTypeEntry * buf);
	EXPORTSPEC int DECL2 daveGetBlockInfo(daveConnection * dc, daveBlockInfo *dbi, uc type, int number);
	/*
		PLC program read functions:
		*/
	EXPORTSPEC int DECL2 initUpload(daveConnection * dc, char blockType, int blockNr, uc *uploadID); // char or uc,to decide
	EXPORTSPEC int DECL2 doUpload(daveConnection*dc, int * more, uc**buffer, int*len, uc *uploadID);
	EXPORTSPEC int DECL2 endUpload(daveConnection*dc, uc *uploadID);

	/*
		NC file read functions:
		*/
	EXPORTSPEC int DECL2 initUploadNC(daveConnection *dc, const char *filename, uc *uploadID);
	EXPORTSPEC int DECL2 doUploadNC(daveConnection *dc, int *more, uc**buffer, int *len, uc *uploadID);
	EXPORTSPEC int DECL2 doSingleUploadNC(daveConnection *dc, int *more, uc *buffer, int *len, uc *uploadID);
	EXPORTSPEC int DECL2 endUploadNC(daveConnection *dc, uc *uploadID);


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
	/* Adds a new symbol variable to a prepared request: */
	EXPORTSPEC void DECL2 daveAddSymbolVarToReadRequest(PDU *p, void * completeSymbol, int completeSymbolLength);
	/* Adds a fill byte to the last request */
	EXPORTSPEC void DECL2 daveAddFillByteToReadRequest(PDU *p);
	/* Adds a new symbol variable to a prepared request: */
	EXPORTSPEC void DECL2 daveAddDbRead400ToReadRequest(PDU *p, int DBnum, int offset, int byteCount);
	/* Executes the complete request. */
	EXPORTSPEC int DECL2 daveExecReadRequest(daveConnection * dc, PDU *p, daveResultSet * rl);
	/* Lets the functions daveGet<data type> work on the n-th result: */
	EXPORTSPEC int DECL2 daveUseResult(daveConnection * dc, daveResultSet * rl, int n, void * buffer);
	/* Lets the Result get into the buffer */
	EXPORTSPEC int DECL2 daveUseResultBuffer(daveResultSet * rl, int n, void * buffer);
	/* Frees the memory occupied by the result structure */
	EXPORTSPEC void DECL2 daveFreeResults(daveResultSet * rl);
	/* Adds a new bit variable to a prepared request: */
	EXPORTSPEC void DECL2 daveAddBitVarToReadRequest(PDU *p, int area, int DBnum, int start, int byteCount);
	/* Adds a new NCK read Request... (see: http://www.sps-forum.de/hochsprachen-opc/80971-dotnetsiemensplctoolboxlibrary-libnodave-zugriff-auf-dual-port-ram-fb15-post611026.html#post611026) */
	EXPORTSPEC void DECL2 daveAddNCKToReadRequest(PDU *p, int area, int unit, int column, int line, int module, int linecount);
	/* Adds a new NCK write Request... */
	EXPORTSPEC void DECL2 daveAddNCKToWriteRequest(PDU *p, int area, int unit, int column, int line, int module, int linecount, int byteCount, void * buffer);

	/* use this to initialize a NC PI-Service: (see: http://www.sps-forum.de/hochsprachen-opc/80971-dotnetsiemensplctoolboxlibrary-libnodave-zugriff-auf-dual-port-ram-fb15-12.html#post619571)*/
	EXPORTSPEC int DECL2 davePIstart_nc(daveConnection *dc, const char *piservice, const char *param[], int paramCount);

	/* use this to initialize a multivariable write: */
	EXPORTSPEC void DECL2 davePrepareWriteRequest(daveConnection * dc, PDU *p);
	/* Add a preformed variable aderess to a read request: */
	EXPORTSPEC void DECL2 daveAddToReadRequest(PDU *p, int area, int DBnum, int start, int byteCount, int isBit);
	/* Adds a new variable to a prepared request: */
	EXPORTSPEC void DECL2 daveAddVarToWriteRequest(PDU *p, int area, int DBnum, int start, int bytes, void * buffer);
	/* Adds a new bit variable to a prepared write request: */
	EXPORTSPEC void DECL2 daveAddBitVarToWriteRequest(PDU *p, int area, int DBnum, int start, int byteCount, void * buffer);
	EXPORTSPEC int DECL2 _daveTestResultData(PDU * p);
	EXPORTSPEC int DECL2 _daveTestReadResult(PDU * p);
	EXPORTSPEC int DECL2 _daveTestPGReadResult(PDU * p);

	/* Executes the complete request. */
	EXPORTSPEC int DECL2 daveExecWriteRequest(daveConnection * dc, PDU *p, daveResultSet * rl);
	EXPORTSPEC int DECL2 _daveTestWriteResult(PDU * p);

	EXPORTSPEC int DECL2 daveInitAdapter(daveInterface * di);
	EXPORTSPEC int DECL2 daveConnectPLC(daveConnection * dc);
	EXPORTSPEC int DECL2 daveDisconnectPLC(daveConnection * dc);

	EXPORTSPEC int DECL2 daveDisconnectAdapter(daveInterface * di);
	EXPORTSPEC int DECL2 daveListReachablePartners(daveInterface * di, char * buf);

	EXPORTSPEC int DECL2 _daveReturnOkDummy(daveInterface * di);
	EXPORTSPEC int DECL2 _daveReturnOkDummy2(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveListReachablePartnersDummy(daveInterface * di, char * buf);

	EXPORTSPEC int DECL2 _daveNegPDUlengthRequest(daveConnection * dc, PDU *p);

	/* MPI specific functions */

#define daveMPIReachable 0x30
#define daveMPIActive 0x30
#define daveMPIPassive 0x00
#define daveMPIunused 0x10
#define davePartnerListSize 126

	EXPORTSPEC int DECL2 _daveListReachablePartnersMPI(daveInterface * di, char * buf);
	EXPORTSPEC int DECL2 _daveInitAdapterMPI1(daveInterface * di);
	EXPORTSPEC int DECL2 _daveInitAdapterMPI2(daveInterface * di);
	EXPORTSPEC int DECL2 _daveConnectPLCMPI1(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveConnectPLCMPI2(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveDisconnectPLCMPI(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveDisconnectAdapterMPI(daveInterface * di);
	EXPORTSPEC int DECL2 _daveExchangeMPI(daveConnection * dc, PDU * p1);

	EXPORTSPEC int DECL2 _daveListReachablePartnersMPI3(daveInterface * di, char * buf);
	EXPORTSPEC int DECL2 _daveInitAdapterMPI3(daveInterface * di);
	EXPORTSPEC int DECL2 _daveConnectPLCMPI3(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveDisconnectPLCMPI3(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveDisconnectAdapterMPI3(daveInterface * di);
	EXPORTSPEC int DECL2 _daveExchangeMPI3(daveConnection * dc, PDU * p1);
	EXPORTSPEC int DECL2 _daveIncMessageNumber(daveConnection * dc);

	/* ISO over TCP specific functions */
	EXPORTSPEC int DECL2 _daveExchangeTCP(daveConnection * dc, PDU * p1);
	EXPORTSPEC int DECL2 _daveConnectPLCTCP(daveConnection * dc);
	/*
		make internal PPI functions available for experimental use:
		*/
	EXPORTSPEC int DECL2 _daveExchangePPI(daveConnection * dc, PDU * p1);
	EXPORTSPEC void DECL2 _daveSendLength(daveInterface * di, int len);
	EXPORTSPEC void DECL2 _daveSendRequestData(daveConnection * dc, int alt);
	EXPORTSPEC void DECL2 _daveSendIt(daveInterface * di, uc * b, int size);
	EXPORTSPEC int DECL2 _daveGetResponsePPI(daveConnection *dc);
	EXPORTSPEC int DECL2 _daveReadChars(daveInterface * di, uc *b, tmotype tmo, int max);
	EXPORTSPEC int DECL2 _daveReadChars2(daveInterface * di, uc *b, int max);
	EXPORTSPEC int DECL2 _daveConnectPLCPPI(daveConnection * dc);

	/*
		make internal MPI functions available for experimental use:
		*/
	EXPORTSPEC int DECL2 _daveReadMPI(daveInterface * di, uc *b);
	EXPORTSPEC void DECL2 _daveSendSingle(daveInterface * di, uc c);
	EXPORTSPEC int DECL2 _daveSendAck(daveConnection * dc, int nr);
	EXPORTSPEC int DECL2 _daveGetAck(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveSendDialog2(daveConnection * dc, int size);
	EXPORTSPEC int DECL2 _daveSendWithPrefix(daveConnection * dc, uc * b, int size);
	EXPORTSPEC int DECL2 _daveSendWithPrefix2(daveConnection * dc, int size);
	EXPORTSPEC int DECL2 _daveSendWithCRC(daveInterface * di, uc *b, int size);
	EXPORTSPEC int DECL2 _daveReadSingle(daveInterface * di);
	EXPORTSPEC int DECL2 _daveReadOne(daveInterface * di, uc *b);
	EXPORTSPEC int DECL2 _daveReadMPI2(daveInterface * di, uc *b);
	EXPORTSPEC int DECL2 _daveGetResponseMPI(daveConnection *dc);
	EXPORTSPEC int DECL2 _daveSendMessageMPI(daveConnection * dc, PDU * p);

	/*
		make internal ISO_TCP functions available for experimental use:
		*/
	/*
		Read one complete packet. The bytes 3 and 4 contain length information.
		*/
	EXPORTSPEC int DECL2 _daveReadISOPacket(daveInterface * di, uc *b);
	EXPORTSPEC int DECL2 _daveGetResponseISO_TCP(daveConnection *dc);

	/* Sendet eine PDU, ohne auf Antwort zu warten */
	EXPORTSPEC int DECL2 _daveSendTCP(daveConnection *dc, PDU *p);


	typedef uc * (*userReadFunc) (int, int, int, int, int *);
	typedef void(*userWriteFunc) (int, int, int, int, int *, uc *);
	extern userReadFunc readCallBack;
	extern userWriteFunc writeCallBack;

	EXPORTSPEC void DECL2 _daveConstructReadResponse(PDU * p);
	EXPORTSPEC void DECL2 _daveConstructWriteResponse(PDU * p);
	EXPORTSPEC void DECL2 _daveConstructBadReadResponse(PDU * p);
	EXPORTSPEC void DECL2 _daveHandleRead(PDU * p1, PDU * p2);
	EXPORTSPEC void DECL2 _daveHandleWrite(PDU * p1, PDU * p2);
	/*
		make internal IBH functions available for experimental use:
		*/
	EXPORTSPEC int DECL2 _daveReadIBHPacket(daveInterface * di, uc *b);
	EXPORTSPEC int DECL2 _daveWriteIBH(daveInterface * di, uc * buffer, int len);
	EXPORTSPEC int DECL2 _davePackPDU(daveConnection * dc, PDU *p);
	EXPORTSPEC void DECL2 _daveSendMPIAck_IBH(daveConnection*dc);
	EXPORTSPEC void DECL2 _daveSendIBHNetAck(daveConnection * dc);
	EXPORTSPEC int DECL2 __daveAnalyze(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveExchangeIBH(daveConnection * dc, PDU * p);
	EXPORTSPEC int DECL2 _daveSendMessageMPI_IBH(daveConnection * dc, PDU * p);
	EXPORTSPEC int DECL2 _daveGetResponseMPI_IBH(daveConnection *dc);
	EXPORTSPEC int DECL2 _daveInitStepIBH(daveInterface * iface, uc * chal, int cl, us* resp, int rl, uc*b);

	EXPORTSPEC int DECL2 _daveConnectPLC_IBH(daveConnection*dc);
	EXPORTSPEC int DECL2 _daveDisconnectPLC_IBH(daveConnection*dc);
	EXPORTSPEC void DECL2 _daveSendMPIAck2(daveConnection *dc);
	EXPORTSPEC int DECL2 _davePackPDU_PPI(daveConnection * dc, PDU *p);
	EXPORTSPEC void DECL2 _daveSendIBHNetAckPPI(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveListReachablePartnersMPI_IBH(daveInterface *di, char * buf);
	EXPORTSPEC int DECL2 __daveAnalyzePPI(daveConnection * dc, uc sa);
	EXPORTSPEC int DECL2 _daveExchangePPI_IBH(daveConnection * dc, PDU * p);
	EXPORTSPEC int DECL2 _daveGetResponsePPI_IBH(daveConnection *dc);

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
	EXPORTSPEC int DECL2 daveGetErrorOfResult(daveResultSet *, int number);

	/*
		Special function do disconnect arbitrary connections on IBH-Link:
		*/
	EXPORTSPEC int DECL2 daveForceDisconnectIBH(daveInterface * di, int src, int dest, int mpi);
	/*
		Special function do reset an IBH-Link:
		*/
	EXPORTSPEC int DECL2 daveResetIBH(daveInterface * di);
	/*
		Program Block from PLC:
		*/
	EXPORTSPEC int DECL2 daveGetProgramBlock(daveConnection * dc, int blockType, int number, char* buffer, int * length);
	/*
		Program Block to PLC:
		*/
	EXPORTSPEC int DECL2 davePutProgramBlock(daveConnection * dc, int blockType, int number, char* buffer, int length);
	/*
		Delete Block from PLC:
		*/
	EXPORTSPEC int DECL2 daveDeleteProgramBlock(daveConnection*dc, int blockType, int number);

	/*
	   Send Receive NC Program:
	   */
	EXPORTSPEC int DECL2 daveGetNCProgram(daveConnection *dc, const char *filename, uc *buffer, int *length);

	EXPORTSPEC int DECL2 daveGetNcFile(daveConnection *dc, const char *filename, char *buffer, int *length);

	EXPORTSPEC int DECL2 daveGetNcFileSize(daveConnection *dc, const char *filename, int *length);

	//DateTime ts Format: yyMMddHHmmss
	EXPORTSPEC int DECL2 davePutNCProgram(daveConnection *dc, char *filename, char *pathname, char *ts, char *buffer, int length);

	/*
	Receive Alarm query:
	*/
	EXPORTSPEC int DECL2 alarmQueryAlarm_S(daveConnection *dc, void *buffer, int buflen, int *number_of_alarms);

	/*
		PLC realtime clock handling:
		*/
	/*
		read out clock:
		*/
	EXPORTSPEC int DECL2 daveReadPLCTime(daveConnection * dc);
	/*
		set clock to a value given by user:
		*/
	EXPORTSPEC int DECL2 daveSetPLCTime(daveConnection * dc, uc * ts);
	/*
		set clock to PC system clock:
		*/
	EXPORTSPEC int DECL2 daveSetPLCTimeToSystime(daveConnection * dc);
	/*
		BCD conversions:
		*/
	EXPORTSPEC uc DECL2 daveToBCD(uc i);
	EXPORTSPEC uc DECL2 daveFromBCD(uc i);
	/*
		S5:
		*/
	EXPORTSPEC int DECL2 _daveFakeExchangeAS511(daveConnection * dc, PDU * p);
	EXPORTSPEC int DECL2 _daveExchangeAS511(daveConnection * dc, uc * b, int len, int maxLen, int trN);
	EXPORTSPEC int DECL2 _daveSendMessageAS511(daveConnection * dc, PDU * p);
	EXPORTSPEC int DECL2 _daveConnectPLCAS511(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveDisconnectPLCAS511(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveIsS5BlockArea(uc area);
	EXPORTSPEC int DECL2 _daveReadS5BlockAddress(daveConnection * dc, uc area, uc BlockN, daveS5AreaInfo * ai);
	EXPORTSPEC int DECL2 _daveReadS5ImageAddress(daveConnection * dc, uc area, daveS5AreaInfo * ai);
	EXPORTSPEC int DECL2 _daveIsS5ImageArea(uc area);
	EXPORTSPEC int DECL2 _daveIsS5DBBlockArea(uc area);
	EXPORTSPEC int DECL2 daveReadS5Bytes(daveConnection * dc, uc area, uc BlockN, int offset, int count);
	EXPORTSPEC int DECL2 daveWriteS5Bytes(daveConnection * dc, uc area, uc BlockN, int offset, int count, void * buf);
	EXPORTSPEC int DECL2 daveStopS5(daveConnection * dc);
	EXPORTSPEC int DECL2 daveStartS5(daveConnection * dc);
	EXPORTSPEC int DECL2 daveGetS5ProgramBlock(daveConnection * dc, int blockType, int number, char* buffer, int * length);

	/*
		MPI version 3:
		*/
	EXPORTSPEC int DECL2 _daveSendWithPrefix31(daveConnection * dc, uc *b, int size);
	EXPORTSPEC int DECL2 _daveGetResponseMPI3(daveConnection *dc);
	EXPORTSPEC int DECL2 _daveSendMessageMPI3(daveConnection * dc, PDU * p);

	/*
		using S7 dlls for transporrt:
		*/

#pragma pack(1)

#ifndef BCCWIN  //We can use this under windows only, but avoid error messages
#define HANDLE int    
#endif

	/*
		Prototypes for the functions in S7onlinx.dll:
		They are guessed.
		*/
	typedef int (DECL2 * _openFunc) (const uc *);
	typedef int (DECL2 * _closeFunc) (int);
	typedef int (DECL2 * _sendFunc) (int, us, uc *);
	typedef int (DECL2 * _receiveFunc) (int, us, int *, us, uc *);
	//typedef int (DECL2 * _SetHWndMsgFunc) (int, int, ULONG);
	//typedef int (DECL2 * _SetHWndFunc) (int, HANDLE);
	typedef int (DECL2 * _get_errnoFunc) (void);

	/*
		And pointers to the functions. We load them using GetProcAddress() on their names because:
		1. We have no .lib file for s7onlinx.
		2. We don't want to link with a particular version.
		3. Libnodave shall remain useable without Siemens .dlls. So it shall not try to access them
		unless the user chooses the daveProtoS7online transport.
		*/
	extern _openFunc SCP_open;
	extern _closeFunc SCP_close;
	extern _sendFunc SCP_send;
	extern _receiveFunc SCP_receive;
	//_SetHWndMsgFunc SetSinecHWndMsg;
	//_SetHWndFunc SetSinecHWnd;
	extern _get_errnoFunc SCP_get_errno;
	/*
		A block of data exchanged between S7onlinx.dll and a program using it. Most fields seem to
		be allways 0. Meaningful names are guessed.
		*/

	typedef struct {
		//Header
		us		unknown[2];
		uc		headerlength;   //Length of the Request Block without Userdata_1 and 2 (80 Bytes!)
		us		user;			//Application Specific
		uc		rb_type;		//Request Block type (always 2)
		uc		priority;		//Priority of the Task, identical like serv_class in the application block
		uc		reserved_1;
		us		reserved_2;
		uc		subsystem;		//For FDL Communication this is 22h = 34
		uc		opcode;			//request, confirm, indication => same as opcode in application block
		us		response;		//return-parameter => same as l_status in application block
		us		fill_length_1;
		uc      reserved_3;
		us      seg_length_1;   //Lengthz of Userdata_1
		us		offset_1;
		us		reserved_4;
		us		fill_length_2;
		uc      reserved_5;
		us      seg_length_2;
		us		offset_2;
		us		reserved_6;
		//End of Header

		//Application Block
		uc		application_block_opcode;						// class of communication   (00 = request, 01=confirm, 02=indication)                                             
		uc		application_block_subsystem;					// number of source-task (only necessary for MTK-user !!!!!)             
		us		application_block_id;							// identification of FDL-USER                                            
		us		application_block_service;						// identification of service (00 -> SDA, send data with acknowlege)                                         
		uc		application_block_local_address_station;        // only for network-connection !!!                                       
		uc		application_block_local_address_segment;        // only for network-connection !!!                                      
		uc		application_block_ssap;							// source-service-access-point                                          
		uc		application_block_dsap;							// destination-service-access-point                                      
		uc		application_block_remote_address_station;		// address of the remote-station                                        
		uc		application_block_remote_address_segment;       // only for network-connection !!!                                      
		us		application_block_service_class;				// priority of service                                  
		void*	application_block_receive_l_sdu_buffer_ptr;		// address and length of received netto-data, exception:                
		uc		application_block_receive_l_sdu_length;			// address and length of received netto-data, exception:                    
		uc		application_block_reserved_1;					// (reserved for FDL !!!!!!!!!!)                                        
		uc		application_block_reserved;						// (reserved for FDL !!!!!!!!!!) 
		void*	application_block_send_l_sdu_buffer_ptr;		// address and length of send-netto-data, exception:                    
		uc		application_block_send_l_sdu_length;			// address and length of send-netto-data, exception:                        
		// 1. csrd  : length means number of POLL-elements       
		// 2. await_indication    : concatenation of application-blocks and   
		//    withdraw_indication : number of application-blocks               
		us		application_block_l_status;						// link-status of service or update_state for srd-indication           
		us		application_block_reserved_2[2];				// for concatenated lists       (reserved for FDL !!!!!!!!!!)          
		//End Application block

		uc			reserveed[12];
		uc			reference[2];

		uc			user_data_1[260];
		uc			user_data_2[260];

	} S7OexchangeBlock;

	EXPORTSPEC int DECL2 _daveCP_send(int fd, int len, uc * reqBlock);
	EXPORTSPEC int DECL2 _daveSCP_send(int fd, uc * reqBlock);
	EXPORTSPEC int DECL2 _daveConnectPLCS7online(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveSendMessageS7online(daveConnection * dc, PDU *p);
	EXPORTSPEC int DECL2 _daveGetResponseS7online(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveExchangeS7online(daveConnection * dc, PDU * p);
	EXPORTSPEC int DECL2 _daveListReachablePartnersS7online(daveInterface * di, char * buf);
	EXPORTSPEC int DECL2 _daveDisconnectAdapterS7online(daveInterface * di);
	EXPORTSPEC int DECL2 _daveDisconnectPLCS7online(daveConnection*dc);

	EXPORTSPEC int DECL2 stdwrite(daveInterface * di, char * buffer, int length);
	EXPORTSPEC int DECL2 stdread(daveInterface * di, char * buffer, int length);

	EXPORTSPEC int DECL2 _daveInitAdapterNLPro(daveInterface * di);
	EXPORTSPEC int DECL2 _daveInitStepNLPro(daveInterface * iface, int nr, uc* fix, int len, char*caller, uc * buffer);
	EXPORTSPEC int DECL2 _daveConnectPLCNLPro(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveSendMessageNLPro(daveConnection *dc, PDU *p);
	EXPORTSPEC int DECL2 _daveGetResponseNLPro(daveConnection *dc);
	EXPORTSPEC int DECL2 _daveExchangeNLPro(daveConnection * dc, PDU * p);
	EXPORTSPEC int DECL2 _daveSendDialogNLPro(daveConnection * dc, int size);
	EXPORTSPEC int DECL2 _daveSendWithPrefixNLPro(daveConnection * dc, uc * b, int size);
	EXPORTSPEC int DECL2 _daveSendWithPrefix2NLPro(daveConnection * dc, int size);
	EXPORTSPEC int DECL2 _daveDisconnectPLCNLPro(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveDisconnectAdapterNLPro(daveInterface * di);
	EXPORTSPEC int DECL2 _daveListReachablePartnersNLPro(daveInterface * di, char * buf);

	/*
	EXPORTSPEC int DECL2 _daveConnectPLCNLProFamily(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveExchangeNLProFamily(daveConnection * dc, PDU * p);
	EXPORTSPEC int DECL2 _daveGetResponseNLProFamily_TCP(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveReadNLProFamilyPacket(daveInterface * di,uc *b);
	EXPORTSPEC int DECL2 _daveSendNLProFamilyPacket(daveConnection * dc, int size);
	EXPORTSPEC int DECL2 _daveInitAdapterNLProFamily(daveInterface * di);
	EXPORTSPEC int DECL2 _daveDisconnectPLCNLProFamily(daveConnection * dc);
	EXPORTSPEC int DECL2 _daveDisconnectAdapterNLProFamily(daveInterface * di);
	EXPORTSPEC int DECL2 _daveListReachablePartnersNLProFamily(daveInterface * di, char * buf);
	*/

#endif /* _nodave */

#ifdef __cplusplus
	//#ifdef CPLUSPLUS
}
#endif


/*
	Changes:
	07/19/04  added the definition of daveExchange().
	09/09/04  applied patch for variable Profibus speed from Andrew Rostovtsew.
	09/09/04  applied patch from Bryan D. Payne to make this compile under Cygwin and/or newer gcc.
	12/09/04  added daveReadBits(), daveWriteBits()
	12/09/04  added some more comments.
	12/09/04  changed declaration of _daveMemcmp to use typed pointers.
	01/15/04  generic getResponse, more internal functions, use a single dummy to replace
	initAdapterDummy,
	01/26/05  replaced _daveConstructReadRequest by the sequence prepareReadRequest, addVarToReadRequest
	01/26/05  added multiple write
	02/02/05  added readIBHpacket
	02/06/05  replaced _daveConstructBitWriteRequest by the sequence prepareWriteRequest, addBitVarToWriteRequest
	02/08/05  removed inline functions.
	03/23/05  added _daveGetResponsePPI_IBH().
	03/24/05  added function codes for download.
	03/28/05  added some comments.
	04/05/05  reworked error reporting.
	04/06/05  renamed swap functions. When I began libnodave on little endian i386 and Linux, I used
	used Linux bswap functions. Then users (and later me) tried other systems without
	a bswap. I also cannot use inline functions in Pascal. So I made my own bswaps. Then
	I, made the core of my own swaps dependent of LITTLEENDIAN conditional to support also
	bigendien systems. Now I want to rename them from bswap to something else to avoid
	confusion for LINUX/UNIX users. The new names are swapIed_16 and swapIed_32. This
	shall say swap "if endianness differs". While I could have used similar functions
	from the network API (htons etc.) on Unix and Win32, they may not be present on
	e.g. microcontrollers.
	I highly recommend to use these functions even when writing software for big endian
	systems, where they effectively do nothing, as it will make your software portable.
	04/08/05  removed deprecated conversion functions.
	04/09/05  removed daveDebug. Use setDebug and getDebug instead. Some programming environments
	do not allow access to global variables in a shared library.
	04/09/05  removed CYGWIN defines. As there were no more differences against LINUX, it should
	work with LINUX defines.
	04/09/05  reordered fields in daveInterface to put fields used in normal test programs at the
	beginning. This allows to make a simplified version in nodavesimple as short as
	possible.
	05/09/05  renamed more functions to daveXXX.
	05/11/05  added some functions for the convenience of usage with .net or mono. The goal is
	that the application doesn't have to use members of data structures defined herein
	directly. This avoids "unsafe" pointer expressions in .net/MONO. It should also ease
	porting to VB or other languages for which it could be difficult to define byte by
	byte equivalents of these structures.
	07/31/05  added message string copying for Visual Basic.
	08/28/05  added some functions to read and set PLC realtime clock.
	09/02/05  added the pointer "hook" to daveConnection. This can be used by applications to pass
	data between function calls using the dc.
	09/11/05  added read/write functions for long blocks of data.
	09/17/05  incorporation of S5 functions
	09/18/05  implemented conversions from and to S5 KG format (old, propeiatary floating point format).
	*/
