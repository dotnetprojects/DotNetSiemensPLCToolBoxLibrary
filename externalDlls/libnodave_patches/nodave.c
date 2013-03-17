/*
Part of Libnodave, a free communication libray for Siemens S7 200/300/400 via
the MPI adapter 6ES7 972-0CA22-0XAC
or  MPI adapter 6ES7 972-0CA23-0XAC
or  TS adapter 6ES7 972-0CA33-0XAC
or  MPI adapter 6ES7 972-0CA11-0XAC,
IBH/MHJ-NetLink or CPs 243, 343 and 443

(C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2002..2005

S5 basic communication parts (C) Andrew Rostovtsew 2004.

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
#include "nodave.h"
#include <stdio.h>
#include "log2.h"
#include <string.h>


//#define DEBUG_CALLS	// Define this and recompile to get parameters and results
// of each function call printed. I could have made this an
// option bit in daveDebug, but most applications will never,never
// need it. Still they would have to do the jumps...
// This option is just useful when developing bindings to new
// programming languages.
/**
Library specific:
**/
#ifdef LINUX
#define HAVE_UNISTD
#define HAVE_SELECT
#define DECL2
#include <time.h>
#include <sys/time.h>
#endif

#ifdef HAVE_UNISTD
#include <unistd.h>
#define daveWriteFile(a,b,c,d) d=write(a,b,c)
#endif

#ifdef AVR_NOOS
#undef BCCWIN
#undef AVR
#define HAVE_SELECT
#endif

#ifdef AVR
#include <unistd.h>
#include <time.h>
#include <sys/time.h>
#endif

int daveDebug=0;

#ifdef BCCWIN
#include <winsock2.h>
#include "openS7online.h"	// We can use the Siemens transport dlls only on Windows

void setTimeOut(daveInterface * di, int tmo) {
	COMMTIMEOUTS cto;
#ifdef DEBUG_CALLS
	LOG3("setTimeOut(di:%p, time:%d)\n",	di,tmo);
	FLUSH;
#endif	    
	//    if(di->fd.connectionType==daveSerialConnection) {
	GetCommTimeouts(di->fd.rfd, &cto);
	cto.ReadIntervalTimeout=0;
	cto.ReadTotalTimeoutMultiplier=0;
	cto.ReadTotalTimeoutConstant=tmo/1000;
	SetCommTimeouts(di->fd.rfd,&cto);
	//    } else if(di->fd.connectionType==daveTcpConnection) {  
	//    }
}    
#endif

#ifdef AVR_NOOS
int DECL2 stdwrite(daveInterface * di, char * buffer, int length) {
	return 0;
}

int DECL2 stdread(daveInterface * di, char * buffer, int length) {	
	return 0;
}
#endif

#ifndef AVR_NOOS
#ifdef HAVE_SELECT
int DECL2 stdwrite(daveInterface * di, char * buffer, int length) {
	if (daveDebug & daveDebugByte)
		_daveDump("I send", (uc*)buffer, length);
	return write(di->fd.wfd, buffer,length);
}

int DECL2 stdread(daveInterface * di, char * buffer, int length) {
	fd_set FDS;	
	struct timeval t;
	int i;
	t.tv_sec = di->timeout / 1000000;
	t.tv_usec = (di->timeout % 1000000);
	FD_ZERO(&FDS);
	FD_SET(di->fd.rfd, &FDS);
	i=0;
	if(select(di->fd.rfd + 1, &FDS, NULL, NULL, &t)>0) {
		i=read(di->fd.rfd, buffer, length);
	}
	//    if (daveDebug & daveDebugByte)
	//	_daveDump("got",buffer,i);
	return i;
}
#endif
#endif

#ifdef BCCWIN

int DECL2 stdread(daveInterface * di,
	char * buffer, 
	int length) {
		unsigned long i;
		ReadFile(di->fd.rfd, buffer, length, &i, NULL);
		//    if (daveDebug & daveDebugByte)
		//	_daveDump("got",buffer,i);
		return i;
}

int DECL2 stdwrite(daveInterface * di, char * buffer, int length) {
	unsigned long i;
	if (daveDebug & daveDebugByte)
		_daveDump("I send",buffer,length);
	//    EscapeCommFunction(di->fd.rfd, CLRRTS);  // patch from Keith Harris. He says:
	//******* this is what microwin does (needed for usb-serial)	
	WriteFile(di->fd.rfd, buffer, length, &i,NULL);
	// patch from Andrea. He says:
	// In this way the PC Adapter connected to CPU313C hangs waiting for RTS line before answering back.
	// Added the following to regain answers:
	//    EscapeCommFunction(di->fd.rfd, SETRTS);
	return i;
}
#endif 


/* 
Setup a new interface structure from an initialized serial interface's handle and a name.
*/
daveInterface * DECL2 daveNewInterface(_daveOSserialType nfd, char * nname, int localMPI, int protocol, int speed){
	daveInterface * di=(daveInterface *) calloc(1, sizeof(daveInterface));
#ifdef DEBUG_CALLS
	LOG7("daveNewInterface(fd.rfd:%d fd.wfd:%d name:%s local MPI:%d protocol:%d PB speed:%d)\n",
		nfd.rfd,nfd.wfd,nname, localMPI, protocol, speed);
	FLUSH;	
#endif	
	if (di) {
		//	di->name=nname;
		strncpy(di->realName,nname,20);
		di->name=di->realName;
		di->fd=nfd;
		di->localMPI=localMPI;
		di->protocol=protocol;
		di->timeout=2500000; /* 2.5 second */
		di->nextConnection=0x14;
		di->speed=speed;
#ifndef AVR_NOOS
		di->getResponse=_daveGetResponseISO_TCP;
#endif
		di->ifread=stdread;
		di->ifwrite=stdwrite;
		di->initAdapter=_daveReturnOkDummy;	
		di->connectPLC=_daveReturnOkDummy2;
		di->disconnectPLC=_daveReturnOkDummy2;
		di->disconnectAdapter=_daveReturnOkDummy;	
		di->listReachablePartners=_daveListReachablePartnersDummy;
		switch (protocol) {
		case daveProtoMPI:
			di->initAdapter=_daveInitAdapterMPI1;
			di->connectPLC=_daveConnectPLCMPI1;
			di->disconnectPLC=_daveDisconnectPLCMPI;
			di->disconnectAdapter=_daveDisconnectAdapterMPI;
			di->exchange=_daveExchangeMPI;
			di->sendMessage=_daveSendMessageMPI;
			di->getResponse=_daveGetResponseMPI;
			di->listReachablePartners=_daveListReachablePartnersMPI;
			break;	

		case daveProtoMPI2:
		case daveProtoMPI4:
			di->initAdapter=_daveInitAdapterMPI2;
			di->connectPLC=_daveConnectPLCMPI2;
			di->disconnectPLC=_daveDisconnectPLCMPI;
			di->disconnectAdapter=_daveDisconnectAdapterMPI;
			di->exchange=_daveExchangeMPI;
			di->sendMessage=_daveSendMessageMPI;
			di->getResponse=_daveGetResponseMPI;
			di->listReachablePartners=_daveListReachablePartnersMPI;
			di->nextConnection=0x3;
			break;

		case daveProtoMPI3:
			di->initAdapter=_daveInitAdapterMPI3;
			di->connectPLC=_daveConnectPLCMPI3;
			di->disconnectPLC=_daveDisconnectPLCMPI3;
			di->disconnectAdapter=_daveDisconnectAdapterMPI3;
			di->exchange=_daveExchangeMPI3;
			di->sendMessage=_daveSendMessageMPI3;
			di->getResponse=_daveGetResponseMPI3;
			di->listReachablePartners=_daveListReachablePartnersMPI3;
			di->nextConnection=0x3;
			break;	
#ifndef AVR_NOOS
		case daveProtoISOTCP:
		case daveProtoISOTCP243:
			//case daveProtoISOTCPR:	// routing over MPI network
			di->getResponse=_daveGetResponseISO_TCP;
			di->connectPLC=_daveConnectPLCTCP;
			di->exchange=_daveExchangeTCP;
			break;		

		case daveProtoPPI:
			di->getResponse=_daveGetResponsePPI;
			di->exchange=_daveExchangePPI;
			di->connectPLC=_daveConnectPLCPPI;
			di->timeout=150000; /* 0.15 seconds */
			break;
		case daveProtoMPI_IBH:
			di->exchange=_daveExchangeIBH;
			di->connectPLC=_daveConnectPLC_IBH;
			di->disconnectPLC=_daveDisconnectPLC_IBH;
			di->sendMessage=_daveSendMessageMPI_IBH;
			di->getResponse=_daveGetResponseMPI_IBH;
			di->listReachablePartners=_daveListReachablePartnersMPI_IBH;
			break;	
		case daveProtoPPI_IBH:
			di->exchange=_daveExchangePPI_IBH;
			di->connectPLC=_daveConnectPLCPPI;
			di->sendMessage=_daveSendMessageMPI_IBH;
			di->getResponse=_daveGetResponsePPI_IBH;
			di->listReachablePartners=_daveListReachablePartnersMPI_IBH;
			break;	
		case daveProtoS7online:
			di->exchange=_daveExchangeS7online;
			di->connectPLC=_daveConnectPLCS7online;
			di->sendMessage=_daveSendMessageS7online;
			di->getResponse=_daveGetResponseS7online;
			di->listReachablePartners=_daveListReachablePartnersS7online;
			di->disconnectPLC=_daveDisconnectPLCS7online; //JK
			//		di->disconnectAdapter=_daveDisconnectAdapterS7online;
			break;		
		case daveProtoAS511:
			di->connectPLC=_daveConnectPLCAS511;
			di->disconnectPLC=_daveDisconnectPLCAS511;
			di->exchange=_daveFakeExchangeAS511;
			di->sendMessage=_daveFakeExchangeAS511;
			break;		
		case daveProtoNLPro:
			di->initAdapter=_daveInitAdapterNLPro;
			di->connectPLC=_daveConnectPLCNLPro;
			di->disconnectPLC=_daveDisconnectPLCNLPro;
			di->disconnectAdapter=_daveDisconnectAdapterNLPro;
			di->exchange=_daveExchangeNLPro;
			di->sendMessage=_daveSendMessageNLPro;
			di->getResponse=_daveGetResponseNLPro;
			di->listReachablePartners=_daveListReachablePartnersNLPro;
			break;
#endif		
		}
#ifdef BCCWIN
		setTimeOut(di, di->timeout);
#endif
	}
	return di;	
}

daveInterface * DECL2 davePascalNewInterface(_daveOSserialType* nfd, char * nname, int localMPI, int protocol, int speed){
#ifdef DEBUG_CALLS
	LOG7("davePascalNewInterface(fd.rfd:%d fd.wfd:%d name:%s local MPI:%d protocol:%d PB speed:%d)\n",
		nfd->rfd,nfd->wfd,nname, localMPI, protocol, speed);
	FLUSH;	
#endif	
	return daveNewInterface(*nfd,nname, localMPI, protocol, speed);
}

/*
debugging:
set debug level by setting variable daveDebug. Debugging is split into
several topics. Output goes to stderr.
The file descriptor is written after the module name, so you may
distinguish messages from multiple connections.

naming: all stuff begins with dave... to avoid conflicts with other
namespaces. Things beginning with _dave.. are not intended for
public use.
*/



void DECL2 daveSetDebug(int nDebug) {
#ifdef DEBUG_CALLS
	LOG2("daveSetDebug(%d)\n",nDebug);
	FLUSH;
#endif	
	daveDebug=nDebug;
}

int DECL2 daveGetDebug() {
#ifdef DEBUG_CALLS
	LOG1("daveGetDebug()\n");
	FLUSH;
#endif	
	return daveDebug;
}
/**
C# interoperability:
**/
void DECL2 daveSetTimeout(daveInterface * di, int tmo) {
#ifdef DEBUG_CALLS
	LOG3("daveSetTimeOut(di:%p, time:%d)\n",	di,tmo);
#endif	    
	di->timeout=tmo;
#ifdef BCCWIN
	setTimeOut(di,tmo);
#endif    
}

int DECL2 daveGetTimeout(daveInterface * di) {
#ifdef DEBUG_CALLS
	LOG2("daveGetTimeOut(di:%p)\n",di);
	FLUSH;
#endif	    
	return di->timeout;
}

char * DECL2 daveGetName(daveInterface * di) {
#ifdef DEBUG_CALLS
	LOG2("daveGetName(di:%p)\n",di);
	FLUSH;
#endif	    
	return di->name;
}


int DECL2 daveGetMPIAdr(daveConnection * dc) {
#ifdef DEBUG_CALLS
	LOG2("daveGetMPIAdr(dc:%p)\n",dc);
	FLUSH;
#endif	    
	return dc->MPIAdr;
}

int DECL2 daveGetAnswLen(daveConnection * dc) {
#ifdef DEBUG_CALLS
	LOG2("daveGetAnswLen(dc:%p)\n",dc);
	FLUSH;
#endif	    
	return dc->AnswLen;
}

int DECL2 daveGetMaxPDULen(daveConnection * dc) {
#ifdef DEBUG_CALLS
	LOG2("daveGetMaxPDULen(dc:%p)\n",dc);
	FLUSH;
#endif	    
	return dc->maxPDUlength;
}

/**
PDU handling:
**/

/*
set up the header. Needs valid header pointer
*/
void DECL2 _daveInitPDUheader(PDU * p, int type) {
	memset(p->header, 0, sizeof(PDUHeader)); 
	if (type==2 || type==3)
		p->hlen=12;
	else
		p->hlen=10;
	p->param=p->header+p->hlen;
	((PDUHeader*)p->header)->P=0x32;
	((PDUHeader*)p->header)->type=type;
	p->dlen=0;
	p->plen=0;
	p->udlen=0;
	p->data=NULL;
	p->udata=NULL;
}

/*
add parameters after header, adjust pointer to data.
needs valid header
*/
void DECL2 _daveAddParam(PDU * p,uc * param,us len) {	
#ifdef ARM_FIX
	us tmplen;
#endif    
	p->plen=len;
	memcpy(p->param, param, len);
#ifdef ARM_FIX
	tmplen=daveSwapIed_16(len);
	memcpy(&(((PDUHeader*)p->header)->plen),&tmplen,sizeof(us));
#else
	((PDUHeader*)p->header)->plen=daveSwapIed_16(len);
#endif    
	p->data=p->param+len;
	p->dlen=0;
}

/*
add data after parameters, set dlen
needs valid header,parameters
*/
void DECL2 _daveAddData(PDU * p,void * data,int len) {
#ifdef ARM_FIX
	us tmplen;
#endif    
	uc * dn= p->data+p->dlen;
	p->dlen+=len;
	memcpy(dn, data, len);
#ifdef ARM_FIX
	tmplen=daveSwapIed_16(p->dlen);
	memcpy(&(((PDUHeader*)p->header)->dlen),&tmplen,sizeof(us));
#else    
	((PDUHeader*)p->header)->dlen=daveSwapIed_16(p->dlen);
#endif    
}

/*
add values after value header in data, adjust dlen and data count.
needs valid header,parameters,data,dlen
*/
void DECL2 _daveAddValue(PDU * p,void * data,int len) {
	us dCount;
	uc * dtype;
	dtype=p->data+p->dlen-4+1;			/* position of first byte in the 4 byte sequence */
#ifdef ARM_FIX    
	memcpy(&dCount, (p->data+p->dlen-4+2), sizeof(us));
#else
	dCount=* ((us *)(p->data+p->dlen-4+2));  /* changed for multiple write */
#endif    
	dCount=daveSwapIed_16(dCount);
	if (daveDebug & daveDebugPDU)
		LOG2("dCount: %d\n", dCount);
	if (*dtype==4) {	/* bit data, length is in bits */
		dCount+=8*len;
	} else if (*dtype==9) {	/* byte data, length is in bytes */
		dCount+=len;
	} else if (* dtype==3) {	/* bit data, length is in bits */
		dCount+=len;	
	} else {
		if (daveDebug & daveDebugPDU)
			LOG2("unknown data type/length: %d\n", *dtype);
	}	    
	if (p->udata==NULL) p->udata=p->data+4;
	p->udlen+=len;	
	if (daveDebug & daveDebugPDU)
		LOG2("dCount: %d\n", dCount);
	dCount=daveSwapIed_16(dCount);
#ifdef ARM_FIX    
	memcpy((p->data+p->dlen-4+2), &dCount, sizeof(us));
#else
	*((us *)(p->data+p->dlen-4+2))=dCount;
#endif    
	_daveAddData(p, data, len);
}

/*
add data in user data. Add a user data header, if not yet present.
*/
void DECL2 _daveAddUserData(PDU * p, uc * da, int len) {
	uc udh[]={0xff,9,0,0};
	if (p->dlen==0) {
		if (daveDebug & daveDebugPDU)
			LOG1("adding user data header.\n");	
		_daveAddData(p, udh, sizeof(udh));
	}
	_daveAddValue(p, da, len);
}

void DECL2 davePrepareReadRequest(daveConnection * dc, PDU *p) {
	uc pa[]=	{daveFuncRead,0};
#ifdef DEBUG_CALLS
	LOG3("davePrepareReadRequest(dc:%p PDU:%p)\n", dc, p);
	FLUSH;
#endif	    
	p->header=dc->msgOut+dc->PDUstartO;
	_daveInitPDUheader(p,1);
	_daveAddParam(p, pa, sizeof(pa));
}    

PDU * DECL2 daveNewPDU() {
	PDU * p;
	p=(PDU*)malloc(sizeof(PDU));
#ifdef DEBUG_CALLS
	LOG2("daveNewPDU() = %p\n",p);
	FLUSH;
#endif	        
	return p;
}    


void DECL2 davePrepareWriteRequest(daveConnection * dc, PDU *p) {
	uc pa[]=	{daveFuncWrite,	0};
#ifdef DEBUG_CALLS
	LOG3("davePrepareWriteRequest(dc:%p PDU:%p)\n", dc, p);
	FLUSH;
#endif	    
	p->header=dc->msgOut+dc->PDUstartO;		
	_daveInitPDUheader(p,1);
	_daveAddParam(p, pa, sizeof(pa));
	p->dlen=0;
}    

void DECL2 daveAddToReadRequest(PDU *p, int area, int DBnum, int start, int byteCount, int isBit) {
	uc pa[]=	{
		0x12, 0x0a, 0x10,
		0x02,		/* 1=single bit, 2=byte, 4=word */
		0,0,		/* length in bytes */
		0,0,		/* DB number */
		0,		/* area code */
		0,0,0		/* start address in bits */
	};
#ifdef ARM_FIX
	us tmplen;
#endif    
	if ((area==daveAnaIn) || (area==daveAnaOut) /*|| (area==daveP)*/) {
		pa[3]=4;
		start*=8;			/* bits */
	} else if ((area==daveTimer) || (area==daveCounter)||(area==daveTimer200) || (area==daveCounter200)) {
		pa[3]=area;
	} else {
		if(isBit) {
			pa[3]=1;
		} else {
			start*=8;			/* bit address of byte */
		}    
	}

	pa[4]=byteCount / 256;		
	pa[5]=byteCount & 0xff;		
	pa[6]=DBnum / 256;		
	pa[7]=DBnum & 0xff;		
	pa[8]=area;		
	pa[11]=start & 0xff;
	pa[10]=(start / 0x100) & 0xff;
	pa[9]=start / 0x10000; 

	p->param[1]++;
	memcpy(p->param+p->plen, pa, sizeof(pa));
	p->plen+=sizeof(pa);

#ifdef ARM_FIX    
	tmplen=daveSwapIed_16(p->plen);
	memcpy(&(((PDUHeader*)p->header)->plen), &tmplen, sizeof(us));
#else
	((PDUHeader*)p->header)->plen=daveSwapIed_16(p->plen);
#endif    
	p->data=p->param+p->plen;
	p->dlen=0;
	if (daveDebug & daveDebugPDU) {
		_daveDumpPDU(p);
	}	
}    

void DECL2 daveAddSymbolToReadRequest(PDU *p, void * completeSymbol, int completeSymbolLength) {
	
	uc pa[]= { 0x12, 0x00, 0xb2, 0xff };
	
#ifdef ARM_FIX
	us tmplen;
#endif    

	pa[1]=completeSymbolLength + 4;
	
	
	p->param[1]++;
	memcpy(p->param+p->plen, pa, sizeof(pa));
	memcpy(p->param+p->plen+4, completeSymbol, completeSymbolLength);
	p->plen+= pa[1];

#ifdef ARM_FIX    
	tmplen=daveSwapIed_16(p->plen);
	memcpy(&(((PDUHeader*)p->header)->plen), &tmplen, sizeof(us));
#else
	((PDUHeader*)p->header)->plen=daveSwapIed_16(p->plen);
#endif    
	p->data=p->param+p->plen;
	p->dlen=0;
	if (daveDebug & daveDebugPDU) {
		_daveDumpPDU(p);
	}	
}  

void DECL2 daveAddSymbolVarToReadRequest(PDU *p, void * completeSymbol, int completeSymbolLength) {
#ifdef DEBUG_CALLS
	LOG6("daveAddSymbolVarToReadRequest(PDU:%p symbol:%s)\n", p, completeSymbol);
	FLUSH;	
#endif	    	

	daveAddSymbolToReadRequest(p, completeSymbol, completeSymbolLength);
} 

void DECL2 daveAddVarToReadRequest(PDU *p, int area, int DBnum, int start, int byteCount) {
#ifdef DEBUG_CALLS
	LOG6("daveAddVarToReadRequest(PDU:%p area:%s area number:%d start address:%d byte count:%d)\n",
		p, daveAreaName(area), DBnum, start, byteCount);
	FLUSH;	
#endif	    	

	daveAddToReadRequest(p, area, DBnum, start, byteCount, 0);
}    

void DECL2 daveAddBitVarToReadRequest(PDU *p, int area, int DBnum, int start, int byteCount) {
	daveAddToReadRequest(p, area, DBnum, start, byteCount, 1);
}    

void DECL2 daveAddToWriteRequest(PDU *p, int area, int DBnum, int start, int byteCount, 
	void * buffer,
	uc * da,
	int dasize,
	uc * pa,
	int pasize
	) {
		uc saveData[1024];
#ifdef ARM_FIX    		
		us tmplen;
#endif    
		if ((area==daveTimer) || (area==daveCounter)||(area==daveTimer200) || (area==daveCounter200)) {    
			pa[3]=area;
			pa[4]=((byteCount+1)/2) / 0x100;
			pa[5]=((byteCount+1)/2) & 0xff;
		} else if ((area==daveAnaIn) || (area==daveAnaOut)) {
			pa[3]=4;
			pa[4]=((byteCount+1)/2) / 0x100;
			pa[5]=((byteCount+1)/2) & 0xff;
		} else {	
			pa[4]=byteCount / 0x100;		
			pa[5]=byteCount & 0xff;	
		}
		pa[6]=DBnum / 256;		
		pa[7]=DBnum & 0xff;		
		pa[8]=area;		
		pa[11]=start & 0xff;
		pa[10]=(start / 0x100) & 0xff;
		pa[9]=start / 0x10000; 
		if(p->dlen%2) {
			_daveAddData(p, da, 1); 
		}    
		p->param[1]++;
		if(p->dlen){
			memcpy(saveData, p->data, p->dlen);
			memcpy(p->data+pasize, saveData, p->dlen);
		}	 
		memcpy(p->param+p->plen, pa, pasize);
		p->plen+=pasize;
#ifdef ARM_FIX    
		tmplen=daveSwapIed_16(p->plen);
		memcpy(&(((PDUHeader*)p->header)->plen), &tmplen, sizeof(us));
#else
		((PDUHeader*)p->header)->plen=daveSwapIed_16(p->plen);
#endif    
		p->data=p->param+p->plen;
		_daveAddData(p, da, dasize);
		_daveAddValue(p, buffer, byteCount);
		if (daveDebug & daveDebugPDU) {
			_daveDumpPDU(p);
		}	
}    

void DECL2 daveAddVarToWriteRequest(PDU *p, int area, int DBnum, int start, int byteCount, void * buffer) {
	uc da[]=	{0, //Return Value
				 4, //Transport-Size
				 0, //Count of the following Data
				 0, //Count of the following Data
				 };
	
	uc pa[]=	{
		0x12, 0x0a, 0x10,
		0x02,		/* unit (for count?, for consistency?) byte */
		0,0,		/* length in bytes */
		0,0,		/* DB number */
		0,		/* area code */
		0,0,0		/* start address in bits */
	};
#ifdef DEBUG_CALLS
	LOG7("daveAddVarToWriteRequest(PDU:%p area:%s area number:%d start address:%d byte count:%d buffer:%p)\n",
		p, daveAreaName(area), DBnum, start, byteCount, buffer);
	FLUSH;	
#endif	    	

	daveAddToWriteRequest(p, area, DBnum, 8*start, byteCount,buffer,da,sizeof(da),pa,sizeof(pa));
}


void DECL2 daveAddBitVarToWriteRequest(PDU *p, int area, int DBnum, int start, int byteCount, void * buffer) {
	uc da[]=	{0,3,0,0,};
	uc pa[]=	{
		0x12, 0x0a, 0x10,
		0x01,		/* single bit */
		0,0,		/* insert length in bytes here */
		0,0,		/* insert DB number here */
		0,		/* change this to real area code */
		0,0,0		/* insert start address in bits */
	};
#ifdef DEBUG_CALLS
	LOG7("daveAddBitVarToWriteRequest(PDU:%p area:%s area number:%d start address:%d byte count:%d buffer:%p)\n",
		p, daveAreaName(area), DBnum, start, byteCount, buffer);
	FLUSH;	
#endif	    	

	daveAddToWriteRequest(p, area, DBnum, start, byteCount,buffer,da,sizeof(da),pa,sizeof(pa));		
}    

/*
Get the eror code from a PDU, if one.
*/
int DECL2 daveGetPDUerror(PDU * p) {
#ifdef DEBUG_CALLS
	LOG2("daveGetPDUerror(PDU:%p\n", p);
	FLUSH;
#endif	    	
	if (p->header[1]==2 || p->header[1]==3) {
		return daveGetU16from(p->header+10);
	} else
		return 0;
}

/*
Sets up pointers to the fields of a received message. 
*/
int DECL2 _daveSetupReceivedPDU(daveConnection * dc, PDU * p) {
	int res; /* = daveResCannotEvaluatePDU; */
	p->header=dc->msgIn+dc->PDUstartI;
	res=0;
	if (p->header[1]==2 || p->header[1]==3) {
		p->hlen=12;
		res=256*p->header[10]+p->header[11];
	} else {
		p->hlen=10;
	}    
	p->param=p->header+p->hlen;    

	p->plen=256*p->header[6] + p->header[7];
	p->data=p->param+p->plen;
	p->dlen=256*p->header[8] + p->header[9];
	p->udlen=0; 
	p->udata=NULL;
	if (daveDebug & daveDebugPDU)
		_daveDumpPDU(p);	
	return res;
}	

int DECL2 _daveTestResultData(PDU * p) {	
	int res; /*=daveResCannotEvaluatePDU;*/
	if ((p->data[0]==255)&&(p->dlen>4))
	{
		res=daveResOK;
		p->udata=p->data+4;
		p->udlen=p->data[2]*0x100+p->data[3];
		if (p->data[1]==4) {
			p->udlen>>=3;	/* len is in bits, adjust */
		} else if (p->data[1]==9) {
			/* len is already in bytes, ok */
		} else if (p->data[1]==3) {
			/* len is in bits, but there is a byte per result bit, ok */
		} else {
			if (daveDebug & daveDebugPDU)
				LOG2("fixme: what to do with data type %d?\n",p->data[1]);
			res = daveResUnknownDataUnitSize;
			//res = 0;
		}	    
	}
	else {
		res=p->data[0];
	}
	return res;    
}

int DECL2 _daveTestReadResult(PDU * p) {
	if (daveDebug & daveDebugPDU)
		LOG2("dave test result: p-param[0]: %d\n",p->param[0]);
	if (p->param[0]!=daveFuncRead) return daveResUnexpectedFunc;
	return _daveTestResultData(p);
}

int DECL2 _daveTestResultDataMulti(PDU * p) {	
	int res; /*=daveResCannotEvaluatePDU;*/
	if ((p->data[0]==255)&&(p->dlen>4))
	{
		res=daveResOK;
		p->udata=p->data+4;
		p->udlen=p->data[2]*0x100+p->data[3];
		if (p->data[1]==4) {
			p->udlen>>=3;	/* len is in bits, adjust */
		} else if (p->data[1]==9) {
			/* len is already in bytes, ok */
		} else if (p->data[1]==3) {
			/* len is in bits, but there is a byte per result bit, ok */
		} else {
			if (daveDebug & daveDebugPDU)
				LOG2("fixme: what to do with data type %d?\n",p->data[1]);
			res = daveResUnknownDataUnitSize;
			//res = 0;
		}	    
	}
	else if (p->data[0]==10 || p->data[0]==5)
	{
		//This Section returns ok, even if nothing was read,
		//because with the multiple read we get the error in (daveUseResult)
		res = daveResOK;		
	}
	else {
		res=p->data[0];
	}
	return res;    
}

int DECL2 _daveTestReadResultMulti(PDU * p) {
	if (daveDebug & daveDebugPDU)
		LOG2("dave test result: p-param[0]: %d\n",p->param[0]);
	if (p->param[0]!=daveFuncRead) return daveResUnexpectedFunc;
	return _daveTestResultDataMulti(p);
}

int DECL2 _daveTestPGReadResult(PDU * p) {	
	int pres=0;
	if (p->param[0]!=0) return daveResUnexpectedFunc;
	if (p->plen==12) pres=(256*p->param[10]+p->param[11]);
	if (pres==0)return _daveTestResultData(p); else return pres;
}

int DECL2 _daveTestWriteResult(PDU * p) {
	int res;/* =daveResCannotEvaluatePDU; */
	if (p->param[0]!=daveFuncWrite) return daveResUnexpectedFunc;
	if ((p->data[0]==255)) {
		res=daveResOK;
	} else 
		res=p->data[0];
	if (daveDebug & daveDebugPDU) {
		_daveDumpPDU(p);
	}
	return res;
}

/*****
Utilities:
****/
/*
This is an extended memory compare routine. It can handle don't care and stop flags 
in the sample data. A stop flag lets it return success.
*/
int DECL2 _daveMemcmp(us * a, uc *b, size_t len) {
	unsigned int i;
	us * a1=(us*)a;
	uc * b1=(uc*)b;
	for (i=0;i<len;i++){
		if (((*a1)&0xff)!=*b1) {
			if (((*a1)&0x100)!=0x100) {
				//		LOG3("want:%02X got:%02X\n",*a1,*b1);
				return i+1;
			}	 
			if (((*a1)&0x200)==0x200) return 0;
		}
		a1++;
		b1++;
	}
	return 0;
}

/*
Hex dump:
*/
void DECL2 _daveDump(char * name, void*b,int len) {//void DECL2 _daveDump(char * name,uc*b,int len) {
	int j;
	LOG2("%s: ",name);
	if (len>daveMaxRawLen) len=daveMaxRawLen; 	/* this will avoid to dump zillions of chars */
	for (j=0; j<len; j++){
		if((j & 0xf)==0) LOG2("\n%x:",j);
		LOG2("0x%02X,",((uc*)(b))[j]);
	}
	LOG1("\n");
}

/*
Hex dump PDU:
*/
void DECL2 _daveDumpPDU(PDU * p) {
	int i,dl;
	uc * pd;
	_daveDump("PDU header", p->header, p->hlen);
	LOG3("plen: %d dlen: %d\n",p->plen, p->dlen);
	if(p->plen>0) _daveDump("Parameter",p->param,p->plen);
	if(p->dlen>0) _daveDump("Data     ",p->data,p->dlen);
	if ((p->plen==2)&&(p->param[0]==daveFuncRead)) {
		pd=p->data;
		for (i=0;i<p->param[1];i++) {
			_daveDump("Data hdr ",pd,4);

			dl=0x100*pd[2]+pd[3];
			if (pd[1]==4) dl/=8;
			pd+=4;        
			_daveDump("Data     ",pd,dl);
			if(i<p->param[1]-1) dl=dl+(dl%2);  	// the PLC places extra bytes at the end of all 
			// but last result, if length is not a multiple 
			// of 2
			pd+=dl;
		}
	} else if ((p->header[1]==1)&&/*(p->plen==2)&&*/(p->param[0]==daveFuncWrite)) {
		pd=p->data;
		for (i=0;i<p->param[1];i++) {
			_daveDump("Write Data hdr ",pd,4);

			dl=0x100*pd[2]+pd[3];
			if (pd[1]==4) dl/=8;
			pd+=4;        
			_daveDump("Data     ",pd,dl);
			if(i<p->param[1]-1) dl=dl+(dl%2);  	// the PLC places extra bytes at the end of all 
			// but last result, if length is not a multiple 
			// of 2
			pd+=dl;
		}
	} else {    
		/*    
		if(p->dlen>0) {
		if(p->udlen==0)
		_daveDump("Data     ",p->data,p->dlen);
		else
		_daveDump("Data hdr ",p->data,4);
		}	
		if(p->udlen>0) _daveDump("result Data ",p->udata,p->udlen);
		*/	
	}
	if ((p->header[1]==2)||(p->header[1]==3)) {
		LOG2("error: %s\n",daveStrerror(daveGetPDUerror(p)));
	}	
}

/*
name Objects:
*/
char * DECL2 daveBlockName(uc bn) {
#ifdef DEBUG_CALLS
	LOG2("daveBlockName(bn:%d)\n", bn);
	FLUSH;
#endif	    	
	switch(bn) {
	case daveBlockType_OB: return "OB";
	case daveBlockType_DB: return "DB";
	case daveBlockType_SDB: return "SDB";
	case daveBlockType_FC: return "FC";
	case daveBlockType_SFC: return "SFC";
	case daveBlockType_FB: return "FB";
	case daveBlockType_SFB: return "SFB";
	default:return "unknown block type!";
	}    
}

char * DECL2 daveAreaName(uc n) {
#ifdef DEBUG_CALLS
	LOG2("daveAreaName(n:%d)\n", n);
	FLUSH;
#endif	    	
	switch (n) {
	case daveSysInfo:	return "System info mem.area of 200 family";
	case daveSysFlags:	return "System flags of 200 family";
	case daveAnaIn:		return "analog inputs of 200 family";
	case daveAnaOut:	return "analog outputs of 200 family";

	case daveP:		return "Peripheral I/O";
	case daveInputs:	return "Inputs";
	case daveOutputs:	return "Outputs";
	case daveDB:		return "DB";
	case daveDI:		return "DI (instance data)";
	case daveFlags:		return "Flags";
	case daveLocal:		return "local data";
	case daveV:		return "caller's local data";
	case daveCounter:	return "S7 counters";
	case daveTimer:		return "S7 timers";
	case daveCounter200:	return "IEC counters";
	case daveTimer200:	return "IEC timers";
	default:return "unknown area!";
	}	
}

/*
Functions to load blocks from PLC:
*/
void DECL2 _daveConstructUpload(PDU *p,char blockType, int blockNr) {
	uc pa[]=	{0x1d,
		0,0,0,0,0,0,0,9,0x5f,0x30,0x41,48,48,48,48,49,65};
	pa[11]=blockType;
	sprintf((char*)(pa+12),"%05d",blockNr);
	pa[17]='A';
	_daveInitPDUheader(p,1);
	_daveAddParam(p, pa, sizeof(pa));
	if (daveDebug & daveDebugPDU) {
		_daveDumpPDU(p);
	}	
}

void DECL2 _daveConstructDoUpload(PDU * p, int uploadID) {
	uc pa[]=	{0x1e,0,0,0,0,0,0,1};
	pa[7]=uploadID;
	_daveInitPDUheader(p,1);
	_daveAddParam(p, pa, sizeof(pa));
	if (daveDebug & daveDebugPDU) {
		_daveDumpPDU(p);
	}	
}    

void DECL2 _daveConstructEndUpload(PDU * p, int uploadID) {
	uc pa[]=	{0x1f,0,0,0,0,0,0,1};
	pa[7]=uploadID;
	_daveInitPDUheader(p,1);
	_daveAddParam(p, pa, sizeof(pa));
	if (daveDebug & daveDebugPDU) {
		_daveDumpPDU(p);
	}	
}    

uc paInsert[]= {		// sended after transmission of a complete block,
	// I guess this makes the CPU link the block into a program.
	0x28,0,0,0,0,0,0,0xFD,0,0x0A,1,0,0x30,0x42,0x30,0x30,0x30,0x30,0x34,0x50, // block type code and number	
	0x05,'_','I','N','S','E',
};

uc paMakeRun[]= {
	0x28,0,0,0,0,0,0,0xFD,0,0x00,9,'P','_','P','R','O','G','R','A','M'
};

uc paCompress[]= {
	0x28,0,0,0,0,0,0,0xFD,0,0x00,5,'_','G','A','R','B'
};

uc paMakeStop[]= {
	0x29,0,0,0,0,0,9,'P','_','P','R','O','G','R','A','M'
};

uc paCopyRAMtoROM[]= {
	0x28,0,0,0,0,0,0,0xfd,0,2,'E','P',5,'_','M','O','D','U'
};

int DECL2 daveStop(daveConnection * dc) {
	int res;
	PDU p,p2;
#ifdef DEBUG_CALLS
	LOG2("daveStop(dc:%p)\n", dc);
	FLUSH;
#endif	  
	if (dc->iface->protocol==daveProtoAS511) {	    
		return daveStopS5(dc);
	}  	
	p.header=dc->msgOut+dc->PDUstartO;
	_daveInitPDUheader(&p, 1);
	_daveAddParam(&p, paMakeStop, sizeof(paMakeStop));
	res=_daveExchange(dc, &p);
	if (res==daveResOK) {
		res=_daveSetupReceivedPDU(dc,&p2);
		if (daveDebug & daveDebugPDU) {
			_daveDumpPDU(&p2);
		}
	}
	return res;
}

int DECL2 daveStart(daveConnection*dc) {
	int res;
	PDU p,p2;
#ifdef DEBUG_CALLS
	LOG2("daveStart(dc:%p)\n", dc);
	FLUSH;
#endif	    	
	if (dc->iface->protocol==daveProtoAS511) {	    
		return daveStartS5(dc);
	}  	
	p.header=dc->msgOut+dc->PDUstartO;
	_daveInitPDUheader(&p, 1);
	_daveAddParam(&p, paMakeRun, sizeof(paMakeRun));
	res=_daveExchange(dc, &p);
	if (res==daveResOK) {
		res=_daveSetupReceivedPDU(dc, &p2);
		if (daveDebug & daveDebugPDU) {
			_daveDumpPDU(&p2);
		}
	}
	return res;
}

int DECL2 daveCopyRAMtoROM(daveConnection * dc) {
	int res;
	PDU p,p2;
#ifdef DEBUG_CALLS
	LOG2("davecopyRAMtoROM(dc:%p)\n", dc);
	FLUSH;
#endif	  
	p.header=dc->msgOut+dc->PDUstartO;
	_daveInitPDUheader(&p, 1);
	_daveAddParam(&p, paCopyRAMtoROM, sizeof(paCopyRAMtoROM));
	res=_daveExchange(dc, &p);
	if (res==daveResOK) {
		res=_daveSetupReceivedPDU(dc,&p2);  /* possible problem, Timeout */
		if (daveDebug & daveDebugPDU) {
			_daveDumpPDU(&p2);
		}
	}
	return res;
}

/*
Build a PDU with user data ud, send it and prepare received PDU.
*/
int DECL2 daveBuildAndSendPDU(daveConnection * dc, PDU*p2,uc *pa,int psize, uc *ud,int usize) {
	int res;
	PDU p;
	uc nullData[]={0x0a,0,0,0};
	p.header=dc->msgOut+dc->PDUstartO;
	_daveInitPDUheader(&p, 7);
	_daveAddParam(&p, pa, psize);
	if (ud!=NULL) _daveAddUserData(&p, ud, usize); else
		if (usize!=0) _daveAddData(&p, nullData, 4);
	if (daveDebug & daveDebugPDU) {
		_daveDumpPDU(&p);
	}	
	res=_daveExchange(dc, &p);
	if (daveDebug & daveDebugErrorReporting)
		LOG2("*** res of _daveExchange(): %d\n",res);
	if (res!=daveResOK) return res;
	res=_daveSetupReceivedPDU(dc,p2);
	if (daveDebug & daveDebugPDU) {
		_daveDumpPDU(p2);
	}
	if (daveDebug & daveDebugErrorReporting)
		LOG2("*** res of _daveSetupReceivedPDU(): %04X\n",res);
	if (res!=daveResOK) return res;
	res=_daveTestPGReadResult(p2);
	if (daveDebug & daveDebugErrorReporting)
		LOG2("*** res of _daveTestPGReadResult(): %04X\n",res);
	return res;
}    	

/*
Get the PDU Data to a ByteBuffer
*/
int DECL2 daveGetPDUData(daveConnection * dc,  PDU*p2, uc* data, int* ldata, uc* param, int* lparam)
{
	int res=0;
	memcpy(data, p2->data, p2->dlen);
	*ldata = p2->dlen;
	memcpy(param, p2->param, p2->plen);
	*lparam = p2->plen;

	return res;
}

int DECL2 daveListBlocksOfType(daveConnection * dc,uc type,daveBlockEntry * buf) {
	int res,i, len;
	PDU p2;
	uc * buffer=(uc*)buf;
	uc pa[]={0,1,18,4,17,67,2,0};
	uc da[]={'0','0'};
	uc pam[]={0,1,18,8,0x12,0x43,2,1,0,0,0,0};
#ifdef DEBUG_CALLS
	LOG4("ListBlocksOfType(dc:%p type:%d buf:%p)\n", dc, type, buf);
	FLUSH;
#endif	    	
	da[1]=type;
	res=daveBuildAndSendPDU(dc, &p2,pa, sizeof(pa), da, sizeof(da));
	if (res!=daveResOK) return -res;
	len=0;
	while (p2.param[9]!=0) {
		if (buffer!=NULL) memcpy(buffer+len,p2.udata,p2.udlen);
		dc->resultPointer=p2.udata;
		dc->_resultPointer=p2.udata;
		len+=p2.udlen;
		printf("more data\n");
		res=daveBuildAndSendPDU(dc, &p2,pam, sizeof(pam), NULL, 1);
	}


	if (res==daveResOK) {
		if (buffer!=NULL) memcpy(buffer+len,p2.udata,p2.udlen);
		dc->resultPointer=p2.udata;
		dc->_resultPointer=p2.udata;
		len+=p2.udlen;
	} else {
		if(daveDebug & daveDebugPrintErrors)
			LOG3("daveListBlocksOfType: %d=%s\n",res, daveStrerror(res));
	}
	dc->AnswLen=len;
	res=len/sizeof(daveBlockEntry);
	for (i=0; i<res; i++) {
		buf[i].number=daveSwapIed_16(buf[i].number);
	}
	return res;
}    

/*
doesn't work on S7-200
*/
int DECL2 daveGetOrderCode(daveConnection * dc,char * buf) {
	int res=0;
	PDU p2;
	uc pa[]={0,1,18,4,17,68,1,0};
	uc da[]={1,17,0,1};  /* SZL-ID 0x111 index 1 */
#ifdef DEBUG_CALLS
	LOG3("daveGetOrderCode(dc:%p buf:%p)\n", dc, buf);
	FLUSH;
#endif	    	
	daveBuildAndSendPDU(dc, &p2,pa, sizeof(pa), da, sizeof(da));
	if (buf) {
		memcpy(buf, p2.udata+10, daveOrderCodeSize);
		buf[daveOrderCodeSize]=0;
	}	
	return res;
}

int DECL2 daveReadSZL(daveConnection * dc, int ID, int index, void * buffer, int buflen) {
	int res,len,cpylen;
	int pa7;
	//    int pa6;
	PDU p2;
	uc pa[]={0,1,18,4,17,68,1,0};
	uc da[]={1,17,0,1};

	uc pam[]={0,1,18,8,18,68,1,1,0,0,0,0};
	//    uc dam[]={10,0,0,0};

#ifdef DEBUG_CALLS
	LOG5("daveReadSZL(dc:%p, ID:%d, index:%d, buffer:%p)\n", dc, ID, index, buffer);
	FLUSH;
#endif	    	
	da[0]=ID / 0x100;
	da[1]=ID % 0x100;
	da[2]=index / 0x100;
	da[3]=index % 0x100;
	res=daveBuildAndSendPDU(dc, &p2,pa, sizeof(pa), da, sizeof(da));

	len=0;
	pa7=p2.param[7];
	//    pa6=p2.param[6];
	while (p2.param[9]!=0) {
		if (buffer!=NULL) {
			cpylen = p2.udlen;
			if (len + cpylen > buflen) cpylen = buflen - len;
			if (cpylen > 0) memcpy((uc *)buffer+len,p2.udata,cpylen);
		}
		dc->resultPointer=p2.udata;
		dc->_resultPointer=p2.udata;
		len+=p2.udlen;
		pam[7]=pa7;
		//		res=daveBuildAndSendPDU(dc, &p2,pam, sizeof(pam), NULL, sizeof(dam));
		res=daveBuildAndSendPDU(dc, &p2,pam, sizeof(pam), NULL, 1);
	}


	if (res==daveResOK) {
		if (buffer!=NULL) {
			cpylen = p2.udlen;
			if (len + cpylen > buflen) cpylen = buflen - len;
			if (cpylen > 0) memcpy((uc *)buffer+len,p2.udata,cpylen);
		}
		dc->resultPointer=p2.udata;
		dc->_resultPointer=p2.udata;
		len+=p2.udlen;
	} 
	dc->AnswLen=len;
	return res;
}

int DECL2 daveGetBlockInfo(daveConnection * dc, daveBlockInfo *dbi, uc type, int number)
{
	int res;
	uc pa[]={0,1,18,4,17,67,3,0};	   /* param */
	uc da[]={'0',0,'0','0','0','1','0','A'};
	PDU p2;
	sprintf((char*)(da+2),"%05d",number);
	da[1]=type;
	da[7]='A';
	res=daveBuildAndSendPDU(dc, &p2,pa, sizeof(pa), da, sizeof(da));    
	if ((dbi!=NULL) && (p2.udlen==sizeof(daveBlockInfo))) {
		memcpy(dbi, p2.udata, p2.udlen);
		dbi->number=daveSwapIed_16(dbi->number);
		dbi->length=daveSwapIed_16(dbi->length);
	}
	return res;	
}


int DECL2 daveListBlocks(daveConnection * dc,daveBlockTypeEntry * buf) {
	int res,i;
	PDU p2;
	uc pa[]={0,1,18,4,17,67,1,0};
	daveBuildAndSendPDU(dc, &p2, pa, sizeof(pa), NULL, 1/*da, sizeof(da)*/);
	res=p2.udlen/sizeof(daveBlockTypeEntry);
	if (buf) {
		memcpy(buf, p2.udata, p2.udlen);
		for (i=0; i<res; i++) {
			buf[i].count=daveSwapIed_16(buf[i].count);
		}	
	}	
	return res;
}

int DECL2 daveReadManyBytes(daveConnection * dc,int area, int DBnum, int start,int len, void * buffer){
	int res, pos, readLen;
	uc * pbuf;
	pos=0;
	if (buffer==NULL) return daveResNoBuffer;
	pbuf=(uc*) buffer; 
	res=daveResInvalidLength; //the only chance to return this is when len<=0
	while (len>0) {
		if (len>dc->maxPDUlength-18) readLen=dc->maxPDUlength-18; else readLen=len;
		res=daveReadBytes(dc,area, DBnum, start, readLen, pbuf);
		if (res!=0) return res;
		len-=readLen;
		start+=readLen;
		pbuf+=readLen;
	}	
	return res;
}

/*
Read len bytes from PLC memory area "area", data block DBnum. 
Return the Number of bytes read.
If a buffer pointer is provided, data will be copied into this buffer.
If it's NULL you can get your data from the resultPointer in daveConnection long
as you do not send further requests.
*/
int DECL2 daveReadBytes(daveConnection * dc,int area, int DBnum, int start,int len, void * buffer){
	PDU p1,p2;
	int res;
#ifdef DEBUG_CALLS
	LOG7("daveReadBytes(dc:%p area:%s area number:%d start address:%d byte count:%d buffer:%p)\n",
		dc, daveAreaName(area), DBnum, start,len, buffer);
	FLUSH;	
#endif
	if (dc->iface->protocol==daveProtoAS511) {	    
		return daveReadS5Bytes(dc, area, DBnum, start, len/*, buffer*/);
	}
	dc->AnswLen=0;	// 03/12/05
	dc->resultPointer=NULL;
	dc->_resultPointer=NULL;
	p1.header=dc->msgOut+dc->PDUstartO;
	davePrepareReadRequest(dc, &p1);
	daveAddVarToReadRequest(&p1, area, DBnum, start, len);
	res=_daveExchange(dc, &p1);
	if (res!=daveResOK) return res;
	res=_daveSetupReceivedPDU(dc, &p2);
	if (daveDebug & daveDebugPDU)
		LOG3("_daveSetupReceivedPDU() returned: %d=%s\n", res,daveStrerror(res));
	if (res!=daveResOK) return res;

	res=_daveTestReadResult(&p2);
	if (daveDebug & daveDebugPDU)
		LOG3("_daveTestReadResult() returned: %d=%s\n", res,daveStrerror(res));
	if (res!=daveResOK) return res;	

	if (p2.udlen==0) {
		return daveResCPUNoData; 
	}	
	/*
	copy to user buffer and setup internal buffer pointers:
	*/    
	if (buffer!=NULL) memcpy(buffer,p2.udata,p2.udlen);
	dc->resultPointer=p2.udata;
	dc->_resultPointer=p2.udata;
	dc->AnswLen=p2.udlen;
	return res;
}

/*
Read len BITS from PLC memory area "area", data block DBnum. 
Return the Number of bytes read.
If a buffer pointer is provided, data will be copied into this buffer.
If it's NULL you can get your data from the resultPointer in daveConnection long
as you do not send further requests.
*/
int DECL2 daveReadBits(daveConnection * dc,int area, int DBnum, int start,int len, void * buffer){
	PDU p1,p2;
	int res;
#ifdef DEBUG_CALLS
	LOG7("daveReadBits(dc:%p area:%s area number:%d start address:%d byte count:%d buffer:%p)\n",
		dc, daveAreaName(area), DBnum, start,len,buffer);
	FLUSH;	    
#endif	    	
	dc->resultPointer=NULL;
	dc->_resultPointer=NULL;
	dc->AnswLen=0;
	p1.header=dc->msgOut+dc->PDUstartO;
	davePrepareReadRequest(dc, &p1);
	daveAddBitVarToReadRequest(&p1, area, DBnum, start, len);

	res=_daveExchange(dc, &p1);
	if (res!=daveResOK) return res;
	res=_daveSetupReceivedPDU(dc, &p2);
	if (daveDebug & daveDebugPDU)
		LOG3("_daveSetupReceivedPDU() returned: %d=%s\n", res,daveStrerror(res));
	if (res!=daveResOK) return res;

	res=_daveTestReadResult(&p2);
	if (daveDebug & daveDebugPDU)
		LOG3("_daveTestReadResult() returned: %d=%s\n", res,daveStrerror(res));
	if (res!=daveResOK) return res;	
	if (daveDebug & daveDebugPDU)
		LOG2("got %d bytes of data\n", p2.udlen);
	if (p2.udlen==0) {
		return daveResCPUNoData; 
	}	
	if (buffer!=NULL) {
		if (daveDebug & daveDebugPDU)
			LOG2("copy %d bytes to buffer\n", p2.udlen);
		memcpy(buffer,p2.udata,p2.udlen);
	}	
	dc->resultPointer=p2.udata;
	dc->_resultPointer=p2.udata;
	dc->AnswLen=p2.udlen;
	return res;
}

/*
Execute a predefined read request. Store results into the resultSet structure.
*/
int DECL2 daveExecReadRequest(daveConnection * dc, PDU *p, daveResultSet* rl){
	PDU p2;
	uc * q;
	daveResult * cr, *c2;
	int res, i, len, rlen;
#ifdef DEBUG_CALLS
	LOG4("daveExecReadRequest(dc:%p, PDU:%p, rl:%p\n", dc, p, rl);
	FLUSH;	
#endif	    	
	dc->AnswLen=0;	// 03/12/05
	dc->resultPointer=NULL;
	dc->_resultPointer=NULL;
	res=_daveExchange(dc, p);
	if (res!=daveResOK) return res;
	res=_daveSetupReceivedPDU(dc, &p2);
	if (res!=daveResOK) return res;
	res=_daveTestReadResultMulti(&p2);
	if (res!=daveResOK) return res;
	i=0;
	if (rl!=NULL) {
		cr=(daveResult*)calloc(p2.param[1], sizeof(daveResult));
		rl->numResults=p2.param[1];
		rl->results=cr;
		c2=cr;
		q=p2.data;
		rlen=p2.dlen;
		while (i<p2.param[1]) {
			/*	    printf("result %d: %d  %d %d %d\n",i, *q,q[1],q[2],q[3]); */
			if ((*q==255)&&(rlen>4)) {
				len=q[2]*0x100+q[3];
				if (q[1]==4) {
					len>>=3;	/* len is in bits, adjust */
				} else if (q[1]==9) {
					/* len is already in bytes, ok */
				} else if (q[1]==3) {
					/* len is in bits, but there is a byte per result bit, ok */
				} else {
					if (daveDebug & daveDebugPDU)
						LOG2("fixme: what to do with data type %d?\n",q[1]);
				}
			} else {
				len=0;
			}	
			/*	    printf("Store result %d length:%d\n", i, len); */
			c2->length=len;
			if(len>0){
				c2->bytes=(uc*)malloc(len);
				memcpy(c2->bytes, q+4, len);
			}	 
			c2->error=daveUnknownError;

			if (q[0]==0xFF) {
				c2->error=daveResOK;    
			} else
				c2->error=q[0];    

			/*	    printf("Error %d\n", c2->error); */
			q+=len+4;
			rlen-=len;
			if ((len % 2)==1) {
				q++;
				rlen--;
			} 
			c2++;
			i++;
		}
	}	
	return res;
}

/*
Execute a predefined write request.
*/
int DECL2 daveExecWriteRequest(daveConnection * dc, PDU *p, daveResultSet* rl){
	PDU p2;
	uc * q;
	daveResult * cr, *c2;
	int res, i;
#ifdef DEBUG_CALLS
	LOG4("daveExecWriteRequest(dc:%p, PDU:%p, rl:%p\n", dc, p, rl);
	FLUSH;
#endif	    	
	res=_daveExchange(dc, p);
	if(res!=daveResOK) return res;
	res=_daveSetupReceivedPDU(dc, &p2);
	if(res!=daveResOK) return res;
	res=_daveTestWriteResult(&p2);
	if(res!=daveResOK) return res;
	if (rl!=NULL) {
		cr=(daveResult*)calloc(p2.param[1], sizeof(daveResult));
		rl->numResults=p2.param[1];
		rl->results=cr;
		c2=cr;
		q=p2.data;
		i=0;
		while (i<p2.param[1]) {
			/*		printf("result %d: %d  %d %d %d\n",i, *q,q[1],q[2],q[3]); */
			c2->error=daveUnknownError;
			if (q[0]==0x0A) {	/* 300 and 400 families */
				c2->error=daveResItemNotAvailable;    
			} else if (q[0]==0x03) {	/* 200 family */
				c2->error=daveResItemNotAvailable;    
			} else if (q[0]==0x05) {
				c2->error=daveAddressOutOfRange;    
			} else if (q[0]==0xFF) {
				c2->error=daveResOK;   
			} else if (q[0]==0x07) {
				c2->error=daveWriteDataSizeMismatch;
			}	
			/*		    printf("Error %d\n", c2->error); */
			q++;
			c2++;
			i++;
		} 
	}
	return res;
}

int DECL2 daveUseResult(daveConnection * dc, daveResultSet * rl, int n, void * buffer){
	daveResult * dr;
#ifdef DEBUG_CALLS
	LOG4("daveUseResult(dc:%p, result set:%p, number:%d)\n", dc, rl, n);
#endif	    	
	if (rl==NULL) {
#ifdef DEBUG_CALLS
		LOG1("invalid resultSet \n");
		FLUSH;
#endif
		return daveEmptyResultSetError;
	} 
#ifdef DEBUG_CALLS
	LOG2("result set has %d results\n",rl->numResults);
	FLUSH;
#endif        
	if (rl->numResults==0) return daveEmptyResultSetError;
	if (n>=rl->numResults) return daveEmptyResultSetError;
	dr = &(rl->results[n]);
	if (dr->error!=0) return dr->error;
	if (dr->length<=0) return daveEmptyResultError;
	
	if (buffer!=NULL) memcpy(buffer,dr->bytes,dr->length);
	dc->resultPointer=dr->bytes;
	dc->_resultPointer=dr->bytes;
	return 0;
}

void DECL2 daveFreeResults(daveResultSet * rl){
	daveResult * r;
	int i;
#ifdef DEBUG_CALLS
	LOG2("daveFreeResults(%p)",rl);
#endif	        
	if (rl==NULL) {
#ifdef DEBUG_CALLS
		LOG1("no Results,ready\n");
#endif	            
		return;	// make it NULL safe
	}	
	/*    printf("result set: %p\n",rl); */
	for (i=0; i<rl->numResults; i++) {
		r=&(rl->results[i]);
		/*	printf("result: %p bytes at:%p\n",r,r->bytes); */
		if (r->bytes!=NULL) free(r->bytes);
	}
#ifdef DEBUG_CALLS
	LOG2(" free'd %d results\n",rl->numResults);
#endif	       
	free(rl->results);	// fix from Renato Gartmann     
	rl->numResults=0;
	/*    free(rl);	*/ /* This is NOT malloc'd by library but in the application's memory space! */
}

int DECL2 daveGetErrorOfResult(daveResultSet *rs, int number) {
	return rs->results[number].error;
}


daveConnection * DECL2 daveNewExtendedConnection(daveInterface * di,  void * Destination, int DestinationIsIP, int rack, int slot, int routing, int routingSubnetFirst, int routingSubnetSecond, int routingRack, int routingSlot, void * routingDestination, int routingDestinationIsIP, int ConnectionType, int routingConnectionType) {

	daveConnection * dc=(daveConnection *) calloc(1,sizeof(daveConnection));
	if (dc) {

		uc * pbuf;

		pbuf=(uc*) Destination;

		dc->DestinationIsIP=DestinationIsIP;
		if (DestinationIsIP)
		{
			dc->_DestinationSize=4;
			dc->_Destination1=pbuf[0];
			dc->_Destination2=pbuf[1];
			dc->_Destination3=pbuf[2];
			dc->_Destination4=pbuf[3];
		}
		else
		{
			dc->_DestinationSize=1;
			dc->_Destination1=pbuf[0];
			dc->MPIAdr=pbuf[0];
			dc->_Destination2=0;
			dc->_Destination3=0;
			dc->_Destination4=0;
		}


		dc->iface=di;	

		dc->rack=rack;
		dc->slot=slot;
		dc->routing=routing;
		dc->routingSubnetFirst=routingSubnetFirst;
		dc->routingSubnetSecond=routingSubnetSecond;
		dc->routingRack=routingRack;
		dc->routingSlot=routingSlot;

		dc->ConnectionType = ConnectionType;
		dc->routingConnectionType = routingConnectionType;

		pbuf=(uc*) routingDestination;

		dc->routingDestinationIsIP=routingDestinationIsIP;
		if (routingDestinationIsIP)
		{
			dc->_routingDestinationSize=4;
			dc->_routingDestination1=pbuf[0];
			dc->_routingDestination2=pbuf[1];
			dc->_routingDestination3=pbuf[2];
			dc->_routingDestination4=pbuf[3];
		}
		else
		{
			dc->_routingDestinationSize=1;
			dc->_routingDestination1=pbuf[0];
			dc->_routingDestination2=0;
			dc->_routingDestination3=0;
			dc->_routingDestination4=0;
		}

	}
	return _daveNewConnection(dc);
}

/* 
This will setup a new connection structure using an initialized
daveInterface and PLC's MPI address.
*/
daveConnection * DECL2 daveNewConnection(daveInterface * di, int MPI, int rack, int slot) {
	daveConnection * dc=(daveConnection *) calloc(1,sizeof(daveConnection));
	if (dc) {
		dc->iface=di;
		dc->MPIAdr=MPI;

		dc->rack=rack;
		dc->slot=slot;
		dc->routing=0;

		dc->ConnectionType = 1;
		dc->routingConnectionType = 1;
	}
	return _daveNewConnection(dc);
}


daveConnection * DECL2 _daveNewConnection(daveConnection * dc) {	
	if (dc) {
		dc->maxPDUlength=1920;				// assume an (unreal?) maximum
		dc->connectionNumber=dc->iface->nextConnection;	// 1/10/05 trying Andrew's patch

		dc->PDUnumber=0xFFFE;			// just a start value; // test!

		dc->messageNumber=0;			
		switch (dc->iface->protocol) {
		case daveProtoMPI:		/* my first Version of MPI */
			dc->PDUstartO=8;	/* position of PDU in outgoing messages */
			dc->PDUstartI=8;	/* position of PDU in incoming messages */
			dc->iface->ackPos=6;		/* position of 0xB0 in ack packet */
			break;
		case daveProtoMPI3:		/* Step 7 Version of MPI */
			dc->PDUstartO=8;	/* position of PDU in outgoing messages */
			dc->PDUstartI=12;	/* position of PDU in incoming messages */
			dc->iface->ackPos=10;		/* position of 0xB0 in ack packet */
			break;	
		case daveProtoMPI2:		/* Andrew's Version of MPI */
		case daveProtoMPI4:		/* Andrew's Version of MPI with extra STX */
			dc->PDUstartO=6;	/* position of PDU in outgoing messages */
			dc->PDUstartI=6;	/* position of PDU in incoming messages */
			dc->iface->ackPos=4;		/* position of 0xB0 in ack packet */
			break;	

		case daveProtoNLPro:	/* Deltalogic NetLink Pro */	
			dc->PDUstartO=6;	/* position of PDU in outgoing messages */
			dc->PDUstartI=8;	/* position of PDU in incoming messages */
			dc->iface->ackPos=4;		/* position of 0xB0 in ack packet */
			break;	

		case daveProtoNLProFamily:	/* Deltalogic NetLink Pro */	
			dc->PDUstartO=8;	/* position of PDU in outgoing messages */
			dc->PDUstartI=8;	/* position of PDU in incoming messages */
			dc->iface->ackPos=4;		/* position of 0xB0 in ack packet */
			break;	

		case daveProtoPPI:
			dc->PDUstartO=3;	/* position of PDU in outgoing messages */
			dc->PDUstartI=7;	/* position of PDU in incoming messages */
			break;	

		case daveProtoISOTCP:
		case daveProtoISOTCP243:
			dc->PDUstartO=7;	/* position of PDU in outgoing messages */
			dc->PDUstartI=7;	/* position of PDU in incoming messages */
			//dc->iface->timeout=1500000;
			break;	
		case daveProtoMPI_IBH:	
			dc->maxPDUlength=240;	// limit for NetLink as reported by AFK 
			dc->PDUstartI= sizeof(IBHpacket)+sizeof(MPIheader);	
			dc->PDUstartO= sizeof(IBHpacket)+sizeof(MPIheader); // 02/01/2005	
			break;
		case daveProtoPPI_IBH:	
			dc->maxPDUlength=240;	// limit for NetLink as reported by AFK 
			dc->PDUstartI=14; // sizeof(IBHpacket)+7;	
			dc->PDUstartO=13;// sizeof(IBHpacket)+7; // 02/01/2005	
			break;	

		case daveProtoAS511:	
			dc->PDUstartI=0; 
			dc->PDUstartO=0;
			break;		

		case daveProtoUserTransport:	
			dc->PDUstartI=0;
			dc->PDUstartO=0;
			break;	
		case daveProtoS7online:	
			dc->PDUstartI=80;
			dc->PDUstartO=80;
			break;		

		default:
			dc->PDUstartO=8;	/* position of PDU in outgoing messages */
			dc->PDUstartI=8;	/* position of PDU in incoming messages */
			fprintf(stderr,"Unknown protocol on interface %s\n",dc->iface->name);	
		}    
#ifdef BCCWIN	
		setTimeOut(dc->iface, dc->iface->timeout);
#endif
	}
	return dc;	
}

int DECL2 daveWriteManyBytes(daveConnection * dc,int area, int DBnum, int start,int len, void * buffer){
	int res, pos, writeLen;
	uc * pbuf;
	pos=0;
	if (buffer==NULL) return daveResNoBuffer;
	pbuf=(uc*) buffer;
	res=daveResInvalidLength; //the only chance to return this is when len<=0
	while (len>0) {
		if (len>dc->maxPDUlength-28) writeLen=dc->maxPDUlength-28; else writeLen=len;
		res=daveWriteBytes(dc,area, DBnum, start, writeLen, pbuf);
		if (res!=0) return res;
		len-=writeLen;
		start+=writeLen;
		pbuf+=writeLen;
	}	
	return res;
}

int DECL2 daveWriteBytes(daveConnection * dc,int area, int DB, int start, int len, void * buffer) {
	PDU p1,p2;
	int res;
	if (dc->iface->protocol==daveProtoAS511) {	    
		return daveWriteS5Bytes(dc, area, DB, start, len, buffer);
	}
	p1.header=dc->msgOut+dc->PDUstartO;
	davePrepareWriteRequest(dc, &p1);
	daveAddVarToWriteRequest(&p1, area, DB, start, len, buffer);
	res=_daveExchange(dc, &p1);
	if(res!=daveResOK) return res;
	res=_daveSetupReceivedPDU(dc, &p2);
	if(res!=daveResOK) return res;
	res=_daveTestWriteResult(&p2);
	return res;
}

int DECL2 daveWriteBits(daveConnection * dc,int area, int DB, int start, int len, void * buffer) {
	PDU p1,p2;
	int res;
	p1.header=dc->msgOut+dc->PDUstartO;
	davePrepareWriteRequest(dc,&p1);
	daveAddBitVarToWriteRequest(&p1, area, DB, start, len, buffer);
	res=_daveExchange(dc, &p1);
	if(res!=daveResOK) return res;
	res=_daveSetupReceivedPDU(dc, &p2);
	if (res!=0) return res;
	res=_daveTestWriteResult(&p2);
	return res;
}

/*
Simplified single bit set:
*/
int DECL2 daveSetBit(daveConnection * dc,int area, int DB, int byteAdr, int bitAdr) {
	int a=1;
	return daveWriteBits(dc, area, DB, 8*byteAdr+bitAdr, 1, &a);
}
/*
Simplified single bit clear:
*/
int DECL2 daveClrBit(daveConnection * dc,int area, int DB, int byteAdr, int bitAdr) {
	int a=0;
	return daveWriteBits(dc, area, DB, 8*byteAdr+bitAdr, 1, &a);
}


int DECL2 initUpload(daveConnection * dc,char blockType, int blockNr, int * uploadID){
	PDU p1,p2;
	int res;
	if (daveDebug & daveDebugUpload) {
		LOG1("****initUpload\n");
	}	
	p1.header=dc->msgOut+dc->PDUstartO;
	_daveConstructUpload(&p1, blockType, blockNr);
	res=_daveExchange(dc, &p1);
	if (daveDebug & daveDebugUpload) {
		LOG2("error:%d\n", res);
		FLUSH;
	}	
	if(res!=daveResOK) return res;
	res=_daveSetupReceivedPDU(dc, &p2);
	if(res!=daveResOK) return res;
	* uploadID=p2.param[7];
	return 0;
}


int DECL2 doUpload(daveConnection*dc, int * more, uc**buffer, int*len, int uploadID){
	PDU p1,p2;
	int res, netLen;
	p1.header=dc->msgOut+dc->PDUstartO;
	_daveConstructDoUpload(&p1, uploadID);
	res=_daveExchange(dc, &p1);
	if (daveDebug & daveDebugUpload) {
		LOG2("error:%d\n", res);
		FLUSH;
	}	
	*more=0;
	if(res!=daveResOK) return res;
	res=_daveSetupReceivedPDU(dc, &p2);
	*more=p2.param[1];
	if(res!=daveResOK) return res;
	//    netLen=p2.data[1] /* +256*p2.data[0]; */ /* for long PDUs, I guess it is so */;
	netLen=p2.data[1]+256*p2.data[0]; /* some user confirmed my guess... */;
	if (*buffer) {
		memcpy(*buffer,p2.data+4,netLen);
		*buffer+=netLen;
		if (daveDebug & daveDebugUpload) {
			LOG2("buffer:%p\n",*buffer);
			FLUSH;
		}    
	}
	*len=netLen;
	return res;
}

int DECL2 endUpload(daveConnection*dc, int uploadID){
	PDU p1,p2;
	int res;

	p1.header=dc->msgOut+dc->PDUstartO;
	_daveConstructEndUpload(&p1,uploadID);

	res=_daveExchange(dc, &p1);
	if (daveDebug & daveDebugUpload) {
		LOG2("error:%d\n", res);
		FLUSH;
	}
	if(res!=daveResOK) return res;	
	res=_daveSetupReceivedPDU(dc, &p2);
	return res;
}

/*
error code to message string conversion:
*/
char * DECL2 daveStrerror(int code) {
	switch (code) {
	case daveResOK: return "ok";
	case daveResMultipleBitsNotSupported:return "the CPU does not support reading a bit block of length<>1";
	case daveResItemNotAvailable: return "the desired item is not available in the PLC";
	case daveResItemNotAvailable200: return "the desired item is not available in the PLC (200 family)";
	case daveAddressOutOfRange: return "the desired address is beyond limit for this PLC";
	case daveResCPUNoData : return "the PLC returned a packet with no result data";
	case daveUnknownError : return "the PLC returned an error code not understood by this library";
	case daveEmptyResultError : return "this result contains no data";
	case daveEmptyResultSetError: return "cannot work with an undefined result set";
	case daveResCannotEvaluatePDU: return "cannot evaluate the received PDU";
	case daveWriteDataSizeMismatch: return "Write data size error";
	case daveResNoPeripheralAtAddress: return "No data from I/O module";
	case daveResUnexpectedFunc: return "Unexpected function code in answer";
	case daveResUnknownDataUnitSize: return "PLC responds with an unknown data type";

	case daveResShortPacket: return "Short packet from PLC";
	case daveResTimeout: return "Timeout when waiting for PLC response";
	case daveResNoBuffer: return "No buffer provided";
	case daveNotAvailableInS5: return "Function not supported for S5";

	case 0x8000: return "function already occupied.";
	case 0x8001: return "not allowed in current operating status.";
	case 0x8101: return "hardware fault.";
	case 0x8103: return "object access not allowed.";
	case 0x8104: return "context is not supported. Step7 says:Function not implemented or error in telgram.";
	case 0x8105: return "invalid address.";
	case 0x8106: return "data type not supported.";
	case 0x8107: return "data type not consistent.";
	case 0x810A: return "object does not exist.";
	case 0x8301: return "insufficient CPU memory ?";
	case 0x8402: return "CPU already in RUN or already in STOP ?";
	case 0x8404: return "severe error ?";
	case 0x8500: return "incorrect PDU size.";
	case 0x8702: return "address invalid."; ;
	case 0xd002: return "Step7:variant of command is illegal.";
	case 0xd004: return "Step7:status for this command is illegal.";
	case 0xd0A1: return "Step7:function is not allowed in the current protection level.";
	case 0xd201: return "block name syntax error.";
	case 0xd202: return "syntax error function parameter.";
	case 0xd203: return "syntax error block type.";
	case 0xd204: return "no linked block in storage medium.";
	case 0xd205: return "object already exists.";
	case 0xd206: return "object already exists.";
	case 0xd207: return "block exists in EPROM.";
	case 0xd209: return "block does not exist/could not be found.";
	case 0xd20e: return "no block present.";
	case 0xd210: return "block number too big.";
		//	case 0xd240: return "unfinished block transfer in progress?";  // my guess
	case 0xd240: return "Coordination rules were violated.";
		/*  Multiple functions tried to manipulate the same object.
		Example: a block could not be copied,because it is already present in the target system
		and
		*/    
	case 0xd241: return "Operation not permitted in current protection level.";
		/**/	case 0xd242: return "protection violation while processing F-blocks. F-blocks can only be processed after password input.";
		case 0xd401: return "invalid SZL ID.";
		case 0xd402: return "invalid SZL index.";
		case 0xd406: return "diagnosis: info not available.";
		case 0xd409: return "diagnosis: DP error.";
		case 0xdc01: return "invalid BCD code or Invalid time format?";

		default: return "no message defined!";
	}
}

/*
Copy an internal String into an external string buffer. This is needed to interface
with Visual Basic. Maybe it is helpful elsewhere, too.
*/
void DECL2 daveStringCopy(char * intString, char * extString) {
	strncpy(extString, intString, 255);	// arbritray limit. I hope each external string has at least this 
	// capacity
}

/*
I'm not quite sure whether this is all correct, but it seems to work for all numbers I tested
*/
float DECL2 daveGetKGAt(daveConnection * dc,int pos) {
	char kgExponent;
	int sign;
	union {
		uc b[4];
		int mantissa;
	} f;
	union {
		int a;
		float f;
	} v;
	uc* p=dc->_resultPointer+pos;
	kgExponent=*p;
	p++;
#ifdef DAVE_LITTLE_ENDIAN    
	f.b[3]=0;
	f.b[2]=*p;
	p++;
	f.b[1]=*p;
	p++;
	f.b[0]=*p;
	sign=(f.b[2]& 0x80);
	f.b[2]&=0x7f;
#else        
	f.b[0]=0;
	f.b[1]=*p;
	p++;
	f.b[2]=*p;
	p++;
	f.b[3]=*p;
	sign=(f.b[1]& 0x80);
	f.b[1]&=0x7f;
#endif    
	p++;
	LOG3("daveGetKG(dc:%p, mantissa:0x%08X)\n",dc, f.mantissa);
	if(sign) {
		f.mantissa=f.mantissa ^0xffffffff;
		f.mantissa=f.mantissa +0x00800000;
	}	
	v.f=f.mantissa;
	if(sign) {
		v.f=-v.f;
	}
	LOG5("daveGetKG(dc:%p, mantissa:0x%08X exponent:0x%02X %0.8f)\n",dc, f.mantissa, kgExponent,v.f);
	while (kgExponent>23) {
		v.f=v.f*2.0;
		kgExponent--;
	}
	while (kgExponent<23) {
		v.f=v.f/2.0;
		kgExponent++;
	}	
	LOG2("daveGetKG(%08X)\n",v.a);
	v.f=-v.f;
	LOG2("daveGetKG(%08X)\n",v.a);
	v.f=-v.f;
#ifdef DEBUG_CALLS
	LOG3("daveGetKG(dc:%p, result:%0.6f)\n",	dc, v.f);
	FLUSH;
#endif	    
	return (v.f);
}

float DECL2 daveGetKG(daveConnection * dc) {
	float f;
	f=daveGetKGAt(dc, ((int)dc->resultPointer-(int)dc->_resultPointer));
	dc->resultPointer+=4;
	return f;
}

float DECL2 daveGetFloat(daveConnection * dc) {
	union {
		float a;
		uc b[4];
	} f;
#ifdef DAVE_LITTLE_ENDIAN    
	f.b[3]=*(dc->resultPointer);
	dc->resultPointer++;
	f.b[2]=*(dc->resultPointer);
	dc->resultPointer++;
	f.b[1]=*(dc->resultPointer);
	dc->resultPointer++;
	f.b[0]=*(dc->resultPointer);
#else        
	f.b[0]=*(dc->resultPointer);
	dc->resultPointer++;
	f.b[1]=*(dc->resultPointer);
	dc->resultPointer++;
	f.b[2]=*(dc->resultPointer);
	dc->resultPointer++;
	f.b[3]=*(dc->resultPointer);
#endif    
	dc->resultPointer++;
#ifdef DEBUG_CALLS
	LOG3("daveGetFloat(dc:%p, result:%0.6f)\n",	dc, f.a);
	FLUSH;
#endif	    
	return (f.a);
}

float DECL2 daveGetFloatAt(daveConnection * dc, int pos) {
	union {
		float a;
		uc b[4];
	} f;
	uc* p=(uc*)dc->_resultPointer;
	p+=pos;
#ifdef DAVE_LITTLE_ENDIAN
	f.b[3]=*p;p++;
	f.b[2]=*p;p++;
	f.b[1]=*p;p++;
	f.b[0]=*p;
#else    
	f.b[0]=*p;p++;
	f.b[1]=*p;p++;
	f.b[2]=*p;p++;
	f.b[3]=*p;
#endif    
	return (f.a);
}

float DECL2 toPLCfloat(float ff) {
#ifdef DAVE_LITTLE_ENDIAN    
	union {
		float a;
		uc b[4];
	} f;
	uc c;

	f.a=ff;
	c=f.b[0];
	f.b[0]=f.b[3];
	f.b[3]=c;
	c=f.b[1];
	f.b[1]=f.b[2];
	f.b[2]=c;

//	f.a=ff;
#ifdef DEBUG_CALLS
	LOG3("toPLCfloat(%0.6f) = %0.6f\n",ff,f.a);
	FLUSH;
#endif	    
	return (f.a);
#else    
#ifdef DEBUG_CALLS
	LOG3("toPLCfloat(%0.6f) = %0.6f\n",ff,ff);
	FLUSH;
#endif	    
	return ff;
#endif        
}

int DECL2 daveToPLCfloat(float ff) {
	union {
		float a;
		uc b[4];
		int c;
	} f;
#ifdef DAVE_LITTLE_ENDIAN    
	uc c;
	f.a=ff;
	c=f.b[0];
	f.b[0]=f.b[3];
	f.b[3]=c;
	c=f.b[1];
	f.b[1]=f.b[2];
	f.b[2]=c;
#else    
	f.a=ff;
#endif    
#ifdef DEBUG_CALLS
	LOG3("toPLCfloat(%0.6f) = %08x\n",ff,f.c);
	FLUSH;
#endif	    
	return (f.c);
}

int DECL2 daveToKG(float ff) {
	union {
		uc b[4];
		int c;
	} f,f2;
	char kgExponent=23;
	LOG2("daveToKG(%0.8f)\n",ff);
	if (ff==0.0) {
		f.c=0;
		return 0;
	}	
	f2.c=(int)ff;	// attention! what does this cast? I do want to take the integer part of that float, NOT reinterpret the bit pattern as int!
	LOG4("daveToKG(mantissa:0x%08X exponent:0x%02X %0.8f)\n", f2.c, kgExponent,ff);
	while (f2.c > 0x00400000){
		ff/=2;
		f2.c=(int)ff;	// attention! what does this cast? I do want to take the integer part of that float, NOT reinterpret the bit pattern as int!
		kgExponent++;
	}
	while (f2.c < 0x00400000){
		ff*=2;
		f2.c=(int)ff;  	// attention! what does this cast? I do want to take the integer part of that float, NOT reinterpret the bit pattern as int!
		kgExponent--;
	}
	LOG4("daveToKG(mantissa:0x%08X exponent:0x%02X %0.8f)\n", f2.c, kgExponent,ff);
	f.b[0]=kgExponent;
#ifdef DAVE_LITTLE_ENDIAN    
	f.b[1]=f2.b[2];
	f.b[2]=f2.b[1];
	f.b[3]=f2.b[0];
#else    
	f.b[3]=f2.b[3];
	f.b[2]=f2.b[2];
	f.b[1]=f2.b[1];
#endif    
#ifdef DEBUG_CALLS
	LOG3("daveToKG(%0.6f) = %08x\n",ff,f.c);
	FLUSH;
#endif	    
	return (f.c);
}


short DECL2 daveSwapIed_16(short ff) {
#ifdef DAVE_LITTLE_ENDIAN
	union {
		short a;
		uc b[2];
	} f;
	uc c;
	f.a=ff;
	c=f.b[0];
	f.b[0]=f.b[1];
	f.b[1]=c;
	return (f.a);
#else
	//    printf("Here we are in BIG ENDIAN!!!\n");
	return (ff);
#endif    
}

int DECL2 daveSwapIed_32(int ff) {
#ifdef DAVE_LITTLE_ENDIAN
	union {
		int a;
		uc b[4];
	} f;
	uc c;
	f.a=ff;
	c=f.b[0];
	f.b[0]=f.b[3];
	f.b[3]=c;
	c=f.b[1];
	f.b[1]=f.b[2];
	f.b[2]=c;
	return f.a;
#else
	//    printf("Here we are in BIG ENDIAN!!!\n");
	return ff;
#endif       
}

/**
Timer and Counter conversion functions:
**/
/*	
get time in seconds from current read position:
*/
float DECL2 daveGetSeconds(daveConnection * dc) {
	uc b[2],a;
	float f;
	b[1]=*(dc->resultPointer)++;
	b[0]=*(dc->resultPointer)++;
	f=b[0] & 0xf;
	f+=10*((b[0] & 0xf0)>>4);
	f+=100*(b[1] & 0xf);
	a=((b[1] & 0xf0)>>4);
	switch (a) {
	case 0: f*=0.01;break;
	case 1: f*=0.1;break;
	case 3: f*=10.0;break;
	}
	return (f);    
}
/*	
get time in seconds from random position:
*/
float DECL2 daveGetSecondsAt(daveConnection * dc, int pos) {
	float f;
	uc b[2],a;
	uc* p=(uc*)dc->_resultPointer;
	p+=pos;
	b[1]=*p;
	p++;
	b[0]=*p;
	f=b[0] & 0xf;
	f+=10*((b[0] & 0xf0)>>4);
	f+=100*(b[1] & 0xf);
	a=((b[1] & 0xf0)>>4);
	switch (a) {
	case 0: f*=0.01;break;
	case 1: f*=0.1;break;
	case 3: f*=10.0;break;
	}
	return (f);
}
/*	
get counter value from current read position:
*/
int DECL2 daveGetCounterValue(daveConnection * dc) {
	uc b[2];
	int f;
	b[1]=*(dc->resultPointer)++;
	b[0]=*(dc->resultPointer)++;
	f=b[0] & 0xf;
	f+=10*((b[0] & 0xf0)>>4);
	f+=100*(b[1] & 0xf);
	return (f);    
}
/*	
get counter value from random read position:
*/
int DECL2 daveGetCounterValueAt(daveConnection * dc,int pos){
	int f;
	uc b[2];
	uc* p=(uc*)dc->_resultPointer;
	p+=pos;
	b[1]=*p;
	p++;
	b[0]=*p;
	f=b[0] & 0xf;
	f+=10*((b[0] & 0xf0)>>4);
	f+=100*(b[1] & 0xf);
	return (f);
}

/*
dummy functions for protocols not providing a specific function:
*/

int DECL2 _daveReturnOkDummy(daveInterface * di){
	return 0;
}

int DECL2 _daveReturnOkDummy2(daveConnection * dc){
	return 0;
}

int DECL2 _daveListReachablePartnersDummy (daveInterface * di, char * buf) {
	return 0;
}

/*
MPI specific functions:
*/

/* 
This writes a single chracter to the serial interface:
*/

void DECL2 _daveSendSingle(daveInterface * di,	/* serial interface */
	uc c  			/* chracter to be send */
	) 
{
	di->ifwrite(di, (char*)&c, 1);
}

int DECL2 _daveReadSingle(daveInterface * di) {
	char res;
	int i;
	i=di->ifread(di, &res,1);
	if ((daveDebug & daveDebugSpecialChars)!=0)
		LOG3("readSingle %d chars. 1st %02X\n",i,res);
	if (i==1) return res;    
	return 0;
}

int DECL2 _daveReadMPI(daveInterface * di, uc *b) {
	int res=0,state=0,nr_read;
	uc bcc=0;
rep:	
	{	
		nr_read= di->ifread(di, (char*)(b+res), 1);
		if (nr_read==0) return 0;
		res+=nr_read;
		if ((res==1) && (*(b+res-1)==DLE)) {
			if ((daveDebug & daveDebugSpecialChars)!=0)
				LOG1("readMPI single DLE.\n");
			return 1;
		}		
		if ((res==1) && (*(b+res-1)==STX)) {
			if ((daveDebug & daveDebugSpecialChars)!=0)
				LOG1("readMPI single STX.\n");
			return 1;
		}
		if (*(b+res-1)==DLE) {
			if (state==0) {
				state=1;
				/*		    if ((daveDebug & daveDebugSpecialChars)!=0)
				LOG1("readMPI 1st DLE in data.\n") 
				;
				*/			
			} else if (state==1) {
				state=0;
				res--;		/* forget this DLE */
				/*		    if ((daveDebug & daveDebugSpecialChars)!=0)
				LOG1("readMPI 2nd DLE in data.\n") 
				;
				*/		    
			}
		} 	
		if (state==3) {
			if ((daveDebug & daveDebugSpecialChars)!=0)
				LOG3("readMPI: packet end, got BCC: %x. I calc: %x\n",*(b+res-1),bcc);
			if ((daveDebug & daveDebugRawRead)!=0)	
				_daveDump("answer",b,res);	
			return res;	    				
		} else {
			bcc=bcc^(*(b+res-1));
		}

		if (*(b+res-1)==ETX) if (state==1) {
			state=3;
			if ((daveDebug & daveDebugSpecialChars)!=0)
				LOG1("readMPI: DLE ETX,packet end.\n");
		}	
		goto rep;
	} 
}

int DECL2 _daveReadMPI2(daveInterface * di, uc *b) {
	int res=_daveReadMPI(di, b);
	if (res>1) {
		_daveSendSingle(di, DLE);
		_daveSendSingle(di, STX);
	}
	return res;
}

int DECL2 _daveGetAck(daveConnection * dc) {
	int res;    
	daveInterface * di=dc->iface;
	int nr=dc->needAckNumber;
	uc b1[daveMaxRawLen];
	if (daveDebug & daveDebugPacket)
		LOG2("%s enter getAck ack\n", di->name);
	res = _daveReadMPI(di, b1);
	if (res<0) return res-10;
	if (res!=di->ackPos+6) {
		if (daveDebug & daveDebugPrintErrors) {
			LOG4("%s *** getAck wrong length %d for ack. Waiting for %d\n dump:", di->name, res, nr);
			_daveDump("wrong ack:",b1,res);
		}
		return -1;
	}	
	if (b1[di->ackPos]!=0xB0) {
		if (daveDebug & daveDebugPrintErrors) {
			LOG3("%s *** getAck char[6] %x no ack\n", di->name, b1[di->ackPos+2]);
		}    
		return -2;
	}		
	if (b1[di->ackPos+2]!=nr) {
		if (daveDebug & daveDebugPrintErrors) {
			LOG4("%s *** getAck got: %d need: %d\n", di->name, b1[di->ackPos+2],nr);
		}    
		return -3;
	}	
	return 0;
}


#define tmo_normal 95000

/* 
This reads up to max chracters when it can get them and returns the number:
*/
int DECL2 _daveReadChars2(daveInterface * di,	/* serial interface */
	uc *b, 		/* a buffer */
	int max		/* limit */
	)
{
	return di->ifread(di,(char*)b,max);
}

/* 
This sends a string after doubling DLEs in the String
and adding DLE,ETX and bcc.
*/
int DECL2 _daveSendWithCRC(daveInterface * di, /* serial interface */
	uc *b, 		 /* a buffer containing the message */
	int size		 /* the size of the string */
	)
{		
	uc target[daveMaxRawLen];	
	int i,targetSize=0;
	int bcc=DLE^ETX; /* preload */
	for (i=0; i<size; i++) {
		target[targetSize]=b[i];targetSize++;
		if (DLE==b[i]) {
			target[targetSize]=DLE;
			targetSize++;
		}  else 
			bcc=bcc^b[i];	/* The doubled DLE effectively contributes nothing */
	};
	target[targetSize]=DLE;
	target[targetSize+1]=ETX;
	target[targetSize+2]=bcc;
	targetSize+=3;
	//    daveWriteFile(di->fd.wfd, target, targetSize, wr);
	di->ifwrite(di, (char*)target, targetSize);
	if (daveDebug & daveDebugPacket)
		_daveDump("_daveSendWithCRC",target, targetSize);
	return 0;
}

/* 
This adds a prefix to a string and theen sends it
after doubling DLEs in the String
and adding DLE,ETX and bcc.
*/
int DECL2 _daveSendWithPrefix(daveConnection * dc, uc *b, int size)
{
	uc target[daveMaxRawLen];
	uc fix[]= {04,0x80,0x80,0x0C,0x03,0x14};
	uc fix2[]= {0x00,0x0c,0x03,0x03};
	if (dc->iface->protocol==daveProtoMPI2) {	
		fix2[2]=dc->connectionNumber2; 		// 1/10/05 trying Andrew's patch
		fix2[3]=dc->connectionNumber; 		// 1/10/05 trying Andrew's patch
		memcpy(target,fix2,sizeof(fix2));
		memcpy(target+sizeof(fix2),b,size);
		return _daveSendWithCRC(dc->iface,target,size+sizeof(fix2));
	}  else {
		fix[4]=dc->connectionNumber2; 		// 1/10/05 trying Andrew's patch
		fix[5]=dc->connectionNumber; 		// 1/10/05 trying Andrew's patch
		memcpy(target,fix,sizeof(fix));
		memcpy(target+sizeof(fix),b,size);
		target[1]|=dc->MPIAdr;
		//	target[2]|=dc->iface->localMPI;
		memcpy(target+sizeof(fix),b,size);
		return _daveSendWithCRC(dc->iface,target,size+sizeof(fix));
	}	
}

int DECL2 _daveSendWithPrefix2(daveConnection * dc, int size)
{
	uc fix[]= {04,0x80,0x80,0x0C,0x03,0x14};
	uc fix2[]= {0x00, 0x0C, 0x03, 0x03};

	if (dc->iface->protocol==daveProtoMPI2) {
		fix2[2]=dc->connectionNumber2; 		// 1/10/05 trying Andrew's patch
		fix2[3]=dc->connectionNumber; 		// 1/10/05 trying Andrew's patch
		memcpy(dc->msgOut, fix2, sizeof(fix2));
		dc->msgOut[sizeof(fix2)]=0xF1;
		return _daveSendWithCRC(dc->iface, dc->msgOut, size+sizeof(fix2));
	}
	else if (dc->iface->protocol==daveProtoMPI) {
		fix[4]=dc->connectionNumber2;		// 1/10/05 trying Andrew's patch
		fix[5]=dc->connectionNumber;		// 1/10/05 trying Andrew's patch
		memcpy(dc->msgOut, fix, sizeof(fix));
		dc->msgOut[1]|=dc->MPIAdr;
		//	dc->msgOut[2]|=dc->iface->localMPI; //???
		dc->msgOut[sizeof(fix)]=0xF1;
		/*	if (daveDebug & daveDebugPacket)
		_daveDump("_daveSendWithPrefix2",dc->msgOut,size+sizeof(fix)); */
		return _daveSendWithCRC(dc->iface, dc->msgOut, size+sizeof(fix));
	}
	return -1; /* shouldn't happen. */
}

/* 
Sends an ackknowledge message for the message number nr:
*/
int DECL2 _daveSendAck(daveConnection * dc, int nr)
{
	uc m[3];
	if (daveDebug & daveDebugPacket)
		LOG3("%s sendAck for message %d \n", dc->iface->name,nr);
	m[0]=0xB0;
	m[1]=0x01;
	m[2]=nr;
	return _daveSendWithPrefix(dc, m, 3);
}

/* 
Handle MPI message numbers in a central place:
*/
int DECL2 _daveIncMessageNumber(daveConnection * dc) {
	int res=dc->messageNumber++;	
	//	LOG2("_daveIncMessageNumber new number %d \n", dc->messageNumber);
	if ((dc->messageNumber)==0) dc->messageNumber=1;
	return res;
}	
/*
Executes part of the dialog necessary to send a message:
*/
int DECL2 _daveSendDialog2(daveConnection * dc, int size)
{
	int a;
	_daveSendSingle(dc->iface, STX);
	if (_daveReadSingle(dc->iface)!=DLE) {
		LOG2("%s *** no DLE before send.\n", dc->iface->name);	    
		return -1;
	} 
	if (size>5){
		dc->needAckNumber=dc->messageNumber;
		dc->msgOut[dc->iface->ackPos+1]=_daveIncMessageNumber(dc);
	}	
	_daveSendWithPrefix2(dc, size);
	a=_daveReadSingle(dc->iface);
	if (a!=DLE) {
		LOG3("%s *** no DLE after send(1) %02x.\n", dc->iface->name,a);
		a=_daveReadSingle(dc->iface);
		if (a!=DLE) {
			LOG3("%s *** no DLE after send(2) %02x.\n", dc->iface->name,a);
			_daveSendWithPrefix2(dc, size);
			a=_daveReadSingle(dc->iface);
			if (a!=DLE) {
				LOG3("%s *** no DLE after resend(3) %02x.\n", dc->iface->name,a);
				_daveSendSingle(dc->iface, STX);
				a=_daveReadSingle(dc->iface);
				if (a!=DLE) {
					LOG2("%s *** no DLE before resend.\n", dc->iface->name);	    
					return -1;
				} else {
					_daveSendWithPrefix2(dc, size);
					a=_daveReadSingle(dc->iface);
					if (a!=DLE) {
						LOG2("%s *** no DLE before resend.\n", dc->iface->name);
						return -1;
					} else {
						LOG2("%s *** got DLE after repeating whole transmisson.\n", dc->iface->name);
						return 0;
					}	
				}
			} else	
				LOG3("%s *** got DLE after resend(3) %02x.\n", dc->iface->name,a);
		}    

	}
	return 0;
}

int DECL2 _daveGetResponseMPI(daveConnection *dc) {
	int res;
	res= _daveReadSingle(dc->iface);
	if (res!=STX) {
		if (daveDebug & daveDebugPrintErrors) {
			LOG2("%s *** _daveGetResponseMPI no STX before answer data.\n", dc->iface->name);	    
		}        
		res= _daveReadSingle(dc->iface);
	}
	_daveSendSingle(dc->iface,DLE);
	if (daveDebug & daveDebugExchange) {
		LOG2("%s _daveGetResponseMPI receive message.\n", dc->iface->name);	    
	}	
	res = _daveReadMPI2(dc->iface,dc->msgIn);
	/*	LOG3("%s *** _daveExchange read result %d.\n", dc->iface->name, res); */
	if (res<=0) {
		if (daveDebug & daveDebugPrintErrors) {
			LOG2("%s *** _daveGetResponseMPI no answer data.\n", dc->iface->name);	    
		}        
		return -3;
	}	
	/*	This is NONSENSE!    
	if (daveDebug & daveDebugExchange) {
	LOG3("%s _daveGetResponseMPI got %d bytes\n", dc->iface->name, dc->AnswLen);	    
	}    
	*/    
	if (_daveReadSingle(dc->iface)!=DLE) {
		if (daveDebug & daveDebugPrintErrors) {
			LOG2("%s *** _daveGetResponseMPI: no DLE.\n", dc->iface->name);	    
		}	
		return -5;
	}    
	_daveSendAck(dc, dc->msgIn[dc->iface->ackPos+1]);
	if (_daveReadSingle(dc->iface)!=DLE) {
		if (daveDebug & daveDebugPrintErrors) {
			LOG2("%s *** _daveGetResponseMPI: no DLE after Ack.\n", dc->iface->name);	    
		}
		return -6;
	}    
	return 0;
}

/*
Sends a message and gets ackknowledge:
*/
int DECL2 _daveSendMessageMPI(daveConnection * dc, PDU * p) {
	if (daveDebug & daveDebugExchange) {
		LOG2("%s enter _daveSendMessageMPI\n", dc->iface->name);	    
	}    
	if (_daveSendDialog2(dc, 2+p->hlen+p->plen+p->dlen)) {
		LOG2("%s *** _daveSendMessageMPI error in _daveSendDialog.\n",dc->iface->name);	    		
		//	return -1;	
	}	
	if (daveDebug & daveDebugExchange) {
		LOG3("%s _daveSendMessageMPI send done. needAck %x\n", dc->iface->name,dc->needAckNumber);	    
	}	

	if (_daveReadSingle(dc->iface)!=STX) {
		if (daveDebug & daveDebugPrintErrors) {
			LOG2("%s *** _daveSendMessageMPI no STX after _daveSendDialog.\n",dc->iface->name);
		}    
		if ( _daveReadSingle(dc->iface)!=STX) {
			if (daveDebug & daveDebugPrintErrors) {
				LOG2("%s *** _daveSendMessageMPI no STX after _daveSendDialog.\n",dc->iface->name);
			}	
			return -2;
		} else {
			if (daveDebug & daveDebugPrintErrors) {
				LOG2("%s *** _daveSendMessageMPI got STX after retry.\n",dc->iface->name);
			}
		}    
	}
	_daveSendSingle(dc->iface,DLE);
	_daveGetAck(dc);
	_daveSendSingle(dc->iface,DLE);
	return 0;
}

int DECL2 _daveExchangeMPI(daveConnection * dc, PDU * p) {
	_daveSendMessageMPI(dc, p);
	dc->AnswLen=0;
	return _daveGetResponseMPI(dc);
}

/* 
Send a string of init data to the MPI adapter.
*/
int DECL2 _daveInitStep(daveInterface * di, int nr, uc *fix, int len, char * caller) {
	_daveSendSingle(di, STX);
	if (_daveReadSingle(di)!=DLE){
		if (daveDebug & daveDebugInitAdapter)
			LOG3("%s %s no answer (DLE) from adapter.\n", di->name, caller);
		if (_daveReadSingle(di)!=DLE){
			if (daveDebug & daveDebugInitAdapter)
				LOG3("%s %s no answer (DLE) from adapter.\n", di->name, caller);
			return nr;
		}    
	}	 
	if (daveDebug & daveDebugInitAdapter)
		LOG4("%s %s step %d.\n", di->name, caller, nr);
	_daveSendWithCRC(di, fix, len);
	if (_daveReadSingle(di)!=DLE) return nr+1;
	if (daveDebug & daveDebugInitAdapter)
		LOG4("%s %s step %d.\n", di->name, caller,nr+1);
	if (_daveReadSingle(di)!=STX) return nr+2;
	if (daveDebug & daveDebugInitAdapter)
		LOG4("%s %s step %d.\n", di->name, caller,nr+2);
	_daveSendSingle(di, DLE);
	return 0;
}    

/* 
This initializes the MPI adapter. Andrew's version.
*/
int DECL2 _daveInitAdapterMPI2(daveInterface * di)  /* serial interface */
{
	uc b3[]={
		0x01,0x03,0x02,0x17, 0x00,0x9F,0x01,0x3C,
		0x00,0x90,0x01,0x14, 0x00,	/* ^^^ MaxTsdr */
		0x00,0x5,
		0x02,/* Bus speed */

		0x00,0x0F,0x05,0x01,0x01,0x03,0x80,/* from topserverdemo */
		/*^^ - Local mpi */
	};		

	int res;
	uc b1[daveMaxRawLen];
	b3[16]=di->localMPI;
	if (di->speed==daveSpeed500k)
		b3[7]=0x64;
	if (di->speed==daveSpeed1500k)
		b3[7]=0x96;
	b3[15]=di->speed;

	res=_daveInitStep(di, 1, b3, sizeof(b3),"initAdapter()");

	res= _daveReadMPI(di, b1);
	if (daveDebug & daveDebugInitAdapter) 
		LOG2("%s initAdapter() success.\n", di->name);
	_daveSendSingle(di,DLE);
	di->users=0;	/* there cannot be any connections now */
	return 0;
}

/* 
Initializes the MPI adapter.
*/
int DECL2 _daveInitAdapterMPI1(daveInterface * di) {
	uc b2[]={
		0x01,0x0D,0x02,
	};
	//  us answ1[]={0x01,0x0D,0x20,'V','0','0','.','8','3'};
	//  us adapter0330[]={0x01,0x03,0x20,'E','=','0','3','3','0'};
	//  us answ2[]={0x01,0x03,0x20,'V','0','0','.','8','3'};
	us answ1[]={0x01,0x10D,0x20,'V','0','0','.',0x138,0x133};
	us adapter0330[]={0x01,0x03,0x20,'E','=','0','3',0x133,0x130};


	uc b3[]={
		0x01,0x03,0x02,0x27, 0x00,0x9F,0x01,0x3C,
		0x00,0x90,0x01,0x14, 0x00,
		0x00,0x05,
		0x02,
		0x00,0x1F,0x02,0x01,0x01,0x03,0x80,
		//	^localMPI
	};		
	uc v1[]={
		0x01,0x0C,0x02,
	};
	int res;
	uc b1[daveMaxRawLen];
	if (daveDebug & daveDebugInitAdapter)
		LOG2("%s enter initAdapter(1).\n", di->name);

	res=_daveInitStep(di, 1, b2, sizeof(b2),"initAdapter()");
	if (res) {
		if (daveDebug & daveDebugInitAdapter)
			LOG2("%s initAdapter() fails.\n", di->name);
		return -44;    
	}	    

	res= _daveReadMPI(di, b1);
	_daveSendSingle(di,DLE);

	if (_daveMemcmp(answ1, b1, sizeof(answ1)/2)) return 4;

	b3[16]=di->localMPI;

	if (di->speed==daveSpeed500k)
		b3[7]=0x64;
	if (di->speed==daveSpeed1500k)
		b3[7]=0x96;
	b3[15]=di->speed;
	res=_daveInitStep(di, 4, b3, sizeof(b3),"initAdapter()");	
	if (res) {
		if (daveDebug & daveDebugInitAdapter)
			LOG2("%s initAdapter() fails.\n", di->name);
		return -54;    
	}
	/*
	The following extra lines seem to be necessary for 
	TS adapter 6ES7 972-0CA33-0XAC:
	*/        
	res= _daveReadMPI(di, b1);
	_daveSendSingle(di,DLE);    
	if (!_daveMemcmp(adapter0330, b1, sizeof(adapter0330)/2)) {
		if (daveDebug & daveDebugInitAdapter)
			LOG2("%s initAdapter() found Adapter E=0330.\n", di->name);
		_daveSendSingle(di,STX);
		res= _daveReadMPI2(di, b1);
		_daveSendWithCRC(di, v1, sizeof(v1));	
		if (daveDebug & daveDebugInitAdapter)
			LOG2("%s initAdapter() Adapter E=0330 step 7.\n", di->name);
		if (_daveReadSingle(di)!=DLE) return 8;
		if (daveDebug & daveDebugInitAdapter)
			LOG2("%s initAdapter() Adapter E=0330 step 8.\n", di->name);
		res= _daveReadMPI(di, b1);
		if (res!=1 || b1[0]!=STX) return 9;
		if (daveDebug & daveDebugInitAdapter)
			LOG2("%s initAdapter() Adapter E=0330 step 9.\n", di->name);
		_daveSendSingle(di,DLE);
		/* This needed the exact Adapter version:    */
		/* instead, just read and waste it */ 
		res= _daveReadMPI(di, b1);
		if (daveDebug & daveDebugInitAdapter)
			LOG2("%s initAdapter() Adapter E=0330 step 10.\n", di->name);
		_daveSendSingle(di,DLE);    
		return 0;    

	} else if (!_daveMemcmp(answ1, b1, sizeof(answ1)/2)) {
		if (daveDebug & daveDebugInitAdapter)
			LOG2("%s initAdapter() success.\n", di->name);
		di->users=0;	/* there cannot be any connections now */
		return 0;
	} else {
		if (daveDebug & daveDebugInitAdapter)
			LOG2("%s initAdapter() failed.\n", di->name);
		return -56;    
	}
}

us ccrc(uc *b,int size) {
	us sum;
	int i,j,m,lll;
	//initialize for crc
	lll=0xcf87;
	sum=0x7e;
	for(j=2;j<=size;j++) {
		for(m=0;m<=7;m++) {
			if((lll&0x8000)!=0) {
				lll=lll^0x8408;
				lll=lll<<1;
				lll=lll+1;
			} else {
				lll=lll<<1;
			}
		}
		sum=sum^lll;
	}
	for(j=0;j<size;j++) {
		sum=sum ^ b[j];
		for(i=0;i<=7;i++) {
			if(sum&0x01) {
				sum=sum>>1;
				sum=sum^0x8408;
			} else {
				sum=sum>>1;
			}
		}
	}
	return sum;
}

/*
us ccrc(uc *b, int size, us start) {
us sum;
int i, j;
//    LOG3("crc start:%04x size%d\n",start,size);
sum = start;
for (j = 0; j < size; j++) {
sum = sum ^ (b[j]);
for (i = 0; i <= 7; i++) {
if (sum & 0x1) {
sum = sum >> 1;
sum = sum ^ 0x8408;
} else
sum = sum >> 1;
}
}
return sum;
}
*/
/*
MPI3 has a quite complicated CRC. It seems that a different start value is needed depending
on length of data. Maybe it only seems so due to my lack of mathematical capabilities...
I could find values for most message lengths making a CPU produce them. Most of the missing 
ones may never occur at all.
*/
/*
us startTab[]={0x0000 , // 0
0x0000 , // 1
0x0000 , // 2
0xbdb7 , // 3
0x0000 , // 4
0x0000 , // 5
0x0000 , // 6
0xab86 , // 7
0x4169 , // 8
0xc854 , // 9
0x0000 , // 10
0x0000 , // 11
0x0000 , // 12
0x0000 , // 13
0x0000 , // 14
0x0000 , // 15
0x0000 , // 16
0x0000 , // 17
0x2d56 , // 18
0x0000 , // 19
0x167a , // 20
0x0000 , // 21
0x0000 , // 22
0xb376 , // 23
0x0000 , // 24
0x0000 , // 25
0x7ca2 , // 26
0xe0a8 , // 27
0x23b0 , // 28
0x1f25 , // 29
0x61c8 , // 30
0x6365 , // 31
0xde47 , // 32
0x377f , // 33
0x7171 , // 34
0x5b75 , // 35
0x05ee , // 36
0x7b72 , // 37
0x08df , // 38
0x22af , // 39
0x0834 , // 40
0xc9af , // 41
0x6618 , // 42
0x8b12 , // 43
0xdf58 , // 44
0x206e , // 45
0xd916 , // 46
0x5e08 , // 47
0x50bb , // 48
0x9355 , // 49
0x59c0 , // 50
0xa0cc , // 51
0x53d2 , // 52
0xe266 , // 53
0xfd92 , // 54
0xf07d , // 55
0x77a0 , // 56
0xba13 , // 57
0x5d68 , // 58
0x2888 , // 59
0x7f9e , // 60
0xc49b , // 61
0x3ac5 , // 62
0xa3ac , // 63
0x2be1 , // 64
0x0ead , // 65
0x60c9 , // 66
0x6a74 , // 67
0x87de , // 68
0x7394 , // 69
0xae57 , // 70
0xb83c , // 71
0x624a , // 72
0xf956 , // 73
0x1439 , // 74
0x2573 , // 75
0xec43 , // 76
0xa87c , // 77
0xa35a , // 78
0xdde1 , // 79
0x894c , // 80
0x917a , // 81
0x66e2 , // 82
0x7112 , // 83
0x3875 , // 84
0x038e , // 85
0x2b14 , // 86
0xfbad , // 87
0xff1b , // 88
0x695f , // 89
0xb4ed , // 90
0xd386 , // 91
0x9ea2 , // 92
0xc61d , // 93
0xace7 , // 94
0x181e , // 95
0x62bf , // 96
0x0c56 , // 97
0x8beb , // 98
0x2658 , // 99
0xdf70 , // 100
0x086e , // 101
0x93af , // 102
0xa3c0 , // 103
0x47e1 , // 104
0x7032 , // 105
0x1064 , // 106
0x5837 , // 107
0x5fdd , // 108
0x8daa , // 109
0x573e , // 110
0x2e22 , // 111
0xe5f8 , // 112
0x5be5 , // 113
0x95ee , // 114
0xd2a6 , // 115
0xb6b3 , // 116
0x9da4 , // 117
0xd82e , // 118
0x6e19 , // 119
0xca9a , // 120
0x4b2b , // 121
0xdafe , // 122
0xae3b , // 123
0xd43c , // 124
0x1cd5 , // 125
0x89fb , // 126
0x267a , // 127
0xfd70 , // 128
0x127d , // 129
0x5115 , // 130
0x3544 , // 131
0x5a53 , // 132
0x2bff , // 133
0x10ad , // 134
0x9137 , // 135
0x2be2 , // 136
0x0dad , // 137
0x78fa , // 138
0x98ec , // 139
0xb87b , // 140
0x254a , // 141
0xd543 , // 142
0x6bc4 , // 143
0x3fcf , // 144
0x81f9 , // 145
0x64f2 , // 146
0x7130 , // 147
0x1a75 , // 148
0x199d , // 149
0xe9ae , // 150
0x6d29 , // 151
0xe2a9 , // 152
0x3292 , // 153
0xb424 , // 154
0x1a86 , // 155
0xea9d , // 156
0x461a , // 157
0x8323 , // 158
0xaed0 , // 159
0x3f3c , // 160
0x72f9 , // 161
0xcb46 , // 162
0x9f3a , // 163
0x560c , // 164
0x1433 , // 165
0x2f73 , // 166
0xbce9 , // 167
0x970e , // 168
0x2284 , // 169
0x2334 , // 170
0x9b25 , // 171
0x6948 , // 172
0xa3ed , // 173
0x6ae1 , // 174
0x12de , // 175
0xf215 , // 176
0x0f82 , // 177
0x47d8 , // 178
0x4932 , // 179
0xd3dc , // 180
0xc4a2 , // 181
0x03c5 , // 182
0x6014 , // 183
0xb774 , // 184
0x52b5 , // 185
0x8d77 , // 186
0x8a3e , // 187
0xfb49 , // 188
0x1b1b , // 189
0x7f8c , // 190
0xd69b , // 191
0xabf7 , // 192
0x3069 , // 193
0x5f06 , // 194
0x56aa , // 195
0xb233 , // 196
0x3de0 , // 197
0xbedb , // 198
0xb52c , // 199
0x1a97 , // 200
0xfb9d , // 201
0xcf1b , // 202
0xe27e , // 203
0xe592 , // 204
0x31e5 , // 205
0xdb17 , // 206
0x4f2a , // 207
0xfbba , // 208
0xe81b , // 209
0xd038 , // 210
0x3891 , // 211
0xe78e , // 212
0x3dc7 , // 213
0x99db , // 214
0x876a , // 215
0xc794 , // 216
0x2df6 , // 217
0x29cb , // 218
0x348f , // 219
0x9942 , // 220
0x1e6a , // 221
0x26d9 , // 222
0x5e70 , // 223
0x28bb , // 224
0x4c9e , // 225
0x5789 , // 226
0x9922 , // 227
0x7e6a , // 228
0x388a , // 229
0xfc8e , // 230
0xe46c , // 231
0xc7f4 , // 232
0x4df6 , // 233
0x3798 , // 234
0x9671 , // 235
0x5595 , // 236
0x9500 , // 237
0x3ca6 , // 238
0xf0ca , // 239
0xc0a0 , // 240
0x2181 , // 241
0x3e07 , // 242
0x41e8 , // 243
0x4954 , // 244
0xb5dc , // 245
0xea97 , // 246
0x4c1a , // 247
0xd389 , // 248
0x0000 , // 249
0x0000 , // 250
0x0000 , // 251
0x0000 , // 252
0x0000 , // 253
0x0000 , // 254
};
*/
int daveSendWithCRC3(daveInterface * di, uc* buffer,int length) {
	uc target[daveMaxRawLen];
	us crc;
	memcpy(target+4,buffer,length);
	target[0]=0x7e;
	if (target[10]==0xB0) {
		target[1]=di->seqNumber+1;
	} else {	
		di->seqNumber+=0x11;
		if (di->seqNumber>=0x88) di->seqNumber=0;
		target[1]=di->seqNumber;
	}	
	target[2]=(length);
	target[3]=0xff-(length);
	//    crc=ccrc(target,length+4,startTab[length]);
	crc=ccrc(target,length+4);
	target[4+length]=crc % 256;
	target[5+length]=crc / 256;
	target[6+length]=0x7e;
	di->ifwrite(di, (char*)target, length+7);
	return 0;
}

int read1(daveInterface * di, uc* b) {
	int len,res;    
	if (daveDebug & daveDebugByte)
		LOG1("enter read1\n");
	len=0;
again:    
	res=di->ifread(di, (char*)b, 5);
	if (res==5) {
		if(b[4]==0x7e) goto again;    
		if(b[2]==255-b[3])  {
			len=b[2]+7;
			//	    LOG2("need length %d\n",len);
			while (res<len) {
				res+=di->ifread(di, (char*)(b+res), len-res);
			}
		}
	}
	//    LOG3("need length %d got %d\n",len,res);
	if (daveDebug & daveDebugByte)
		_daveDump("got",b,res);
	return res;
}
/* 
This initializes the MPI adapter. Step 7 version.
*/
int DECL2 _daveInitAdapterMPI3(daveInterface * di) 
{
	uc b2[]={0x7E,0xFC,0x9B,0xCD,0x7E};
	us adapter0330[]={0x01,0x03,0x20,'E','=','0','3','3','0'};
	uc v1[]={0x01,0x0C,0x02};

	uc b3[]={
		0x01,0x03,0x02,0x17, 0x00,0x9F,0x01,0x3C,
		0x00,0x90,0x01,0x14, 0x00,	/* ^^^ MaxTsdr */
		0x00,0x5,
		0x02,/* Bus speed */

		0x00,0x1F,0x05,0x01,0x01,0x03,0x80,/* from topserverdemo */
		/*^^ - Local mpi */
	};		
	uc m4[]={0x7e,0xca,0x2e,0x99,0x7e};
	uc b55[]={0x01,0x08,0x02};
	uc b1[daveMaxRawLen];

	int res,count;

	b3[16]=di->localMPI;
	if (di->speed==daveSpeed500k)
		b3[7]=0x64;
	if (di->speed==daveSpeed1500k)
		b3[7]=0x96;
	b3[15]=di->speed;
	count=0;
again:    
	count++;
	if (count>20) return -2;
	di->seqNumber=0x77;
	di->ifwrite(di, (char*)b2, sizeof(b2));
	res=di->ifread(di, (char*)b1, 5);
	if (res==0) {
		di->ifwrite(di, (char*)b2, sizeof(b2));
		res=di->ifread(di, (char*)b1, 5);
	}
	if (res==0) {
		di->ifwrite(di, (char*)b2,sizeof(b2));
		res=di->ifread(di, (char*)b1, 5);
	}	
	if (daveDebug & daveDebugByte)
		_daveDump("got", b1, res);
	if (res==5) {
		if (b1[1]==0xCE) {
			if (daveDebug & daveDebugInitAdapter)
				LOG1("ok, I begin sequence\n");
			di->seqNumber=0x77;
		} else if (b1[1]==0xCA) {
			if (daveDebug & daveDebugInitAdapter)
				LOG1("refused.\n");
			goto again;
			//	    res=di->ifread(di, b1, 100);	//certainly nonsense after a jump
		} else if (b1[1]==0xF8) {
			if (daveDebug & daveDebugInitAdapter)
				LOG1("refused.\n");
			di->ifwrite(di, (char*)m4, sizeof(m4));
			res=di->ifread(di, (char*)b1, 100);
			goto again;
		} else if (b1[1]==0x8a) {
			if (daveDebug & daveDebugInitAdapter)
				LOG1("in sequence. set to 0x11\n");
			di->seqNumber=0x0;
		} else if (b1[1]==0x8b) {
			if (daveDebug & daveDebugInitAdapter)
				LOG1("in sequence. set to 0x22\n");
			di->seqNumber=0x22;
		} else if (b1[1]==0x8c) {
			if (daveDebug & daveDebugInitAdapter)
				LOG1("in sequence. set to 0x33\n");
			di->seqNumber=0x33;
		} else if (b1[1]==0x8d) {
			if (daveDebug & daveDebugInitAdapter)
				LOG1("in sequence. set to 0x44\n");
			di->seqNumber=0x44;
		}
	} else return -1;
	daveSendWithCRC3(di,b3,sizeof(b3));
	read1(di, b1);
	if (!_daveMemcmp(adapter0330, b1+4, sizeof(adapter0330)/2)) {
		if (daveDebug & daveDebugInitAdapter)
			LOG2("%s initAdapter() found Adapter E=0330.\n", di->name);
		daveSendWithCRC3(di,v1,sizeof(v1));    
		read1(di, b1);
		return 0;
	}    
	daveSendWithCRC3(di,b55,sizeof(b55));
	read1(di, b1);
	//    daveSendWithCRC3(di,b66,sizeof(b66));
	//    read1(di, b1);
	return 0;
}

int DECL2 _daveSendWithPrefix32(daveConnection * dc, int size) {
	uc fix[]= {04,0x80,0x80,0x0C,0x03,0x14};
	fix[4]=dc->connectionNumber2;		// 1/10/05 trying Andrew's patch
	fix[5]=dc->connectionNumber;		// 1/10/05 trying Andrew's patch
	memcpy(dc->msgOut, fix, sizeof(fix));
	dc->msgOut[1]|=dc->MPIAdr;
	dc->msgOut[sizeof(fix)]=0xF1;
	return daveSendWithCRC3(dc->iface, dc->msgOut, size+sizeof(fix));
}

int DECL2 _daveListReachablePartnersMPI3(daveInterface * di,char * buf) {
	uc b1[daveMaxRawLen];
	uc m1[]={1,7,2};
	int res;
	daveSendWithCRC3(di,m1,sizeof(m1));
	res=read1(di, b1);
	if (daveDebug & daveDebugInitAdapter)
		LOG2("res:%d\n",res);
	if(140==res){
		memcpy(buf,b1+10,126);
		return 126;
	} else
		return 0;	
}   


/* 
Open connection to a PLC. This assumes that dc is initialized by
daveNewConnection and is not yet used.
(or reused for the same PLC ?)
*/
int DECL2 _daveConnectPLCMPI3(daveConnection * dc) {
	int res, mpi;

	PDU p1;
	uc b1[daveMaxRawLen];

	uc e18[]={0x04,0x82,0x00,
		0x0d,0x00,0x14,0xe0,0x04,0x00,0x80,
		0x00,0x02,0x00,0x02,
		0x01,
		0x00,
		0x01,0x00,
		//	    0x02,0x03,0x01,0x00
	};
	uc b4[]={
		0x00,0x0d,0x00,0x03,0xe0,0x04,0x00,0x80,
		0x00,0x02,0x01,0x06,
		0x01,
		0x00,
		0x00,0x01,
		0x02,0x03,0x01,0x00
		/*^^ MPI ADDR */
	};

	us t4[]={
		0x00,0x0c,0x103,0x103,0xd0,0x04,0x00,0x80,
		0x01,0x06,
		0x00,0x02,0x00,0x01,0x02,
		0x03,0x01,0x00,
		0x01,0x00,0x10,0x03,0x4d
	};
	uc b5[]={	
		0x05,0x01,
	};

	b4[3]=dc->connectionNumber; // 1/10/05 trying Andrew's patch
	b4[sizeof(b4)-3]=dc->MPIAdr;	
	t4[15]=dc->MPIAdr;
	t4[sizeof(t4)/2-1]^=dc->MPIAdr; /* 'patch' the checksum	*/
	mpi=dc->MPIAdr;
	//    dc->MPIAdr=2;
	//    e18[sizeof(e18)-3]=dc->MPIAdr;
	e18[1]|=dc->MPIAdr;
	daveSendWithCRC3(dc->iface,e18,sizeof(e18));
	read1(dc->iface, b1);

	//    dc->connectionNumber2=b1[3]; // 1/10/05 trying Andrew's patch
	dc->connectionNumber2=b1[9];
	dc->connectionNumber=0x14;

	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 3.\n", dc->iface->name);	
	//    res=_daveReadMPI(dc->iface,b1);

	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 4.\n", dc->iface->name);	

	_daveSendWithPrefix31(dc, b5, sizeof(b5));		
	read1(dc->iface, b1);
	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 6.\n", dc->iface->name);	
	res= _daveNegPDUlengthRequest(dc, &p1);
	return 0;
}

/* 
This adds a prefix to a string and theen sends it
after doubling DLEs in the String
and adding DLE,ETX and bcc.
*/
int DECL2 _daveSendWithPrefix31(daveConnection * dc, uc *b, int size)
{
	uc target[daveMaxRawLen];
	uc fix[]= {04,0x80,0x80,0x0C,0x03,0x14};
	fix[4]=dc->connectionNumber2; 		// 1/10/05 trying Andrew's patch
	fix[5]=dc->connectionNumber; 		// 1/10/05 trying Andrew's patch
	memcpy(target,fix,sizeof(fix));
	memcpy(target+sizeof(fix),b,size);
	target[1]|=dc->MPIAdr;
	memcpy(target+sizeof(fix),b,size);
	return daveSendWithCRC3(dc->iface,target,size+sizeof(fix));
}

/*
Executes part of the dialog necessary to send a message:
*/
int DECL2 _daveSendDialog3(daveConnection * dc, int size)
{
	if (size>5){
		dc->needAckNumber=dc->messageNumber;
		dc->msgOut[dc->iface->ackPos-dc->PDUstartI+dc->PDUstartO+1]=_daveIncMessageNumber(dc);
	}	
	_daveSendWithPrefix32(dc, size);
	return 0;
}

int DECL2 _daveSendMessageMPI3(daveConnection * dc, PDU * p) {
	if (daveDebug & daveDebugExchange) {
		LOG2("%s enter _daveSendMessageMPI3\n", dc->iface->name);	    
	}    
	if (_daveSendDialog3(dc, 2+p->hlen+p->plen+p->dlen)) {
		LOG2("%s *** _daveSendMessageMPI3 error in _daveSendDialog.\n",dc->iface->name);	    		
		//	return -1;	
	}	
	if (daveDebug & daveDebugExchange) {
		LOG3("%s _daveSendMessageMPI send done. needAck %x\n", dc->iface->name,dc->needAckNumber);	    
	}	
	return 0;
}

/* 
Sends an ackknowledge message for the message number nr:
*/
int DECL2 _daveSendAckMPI3(daveConnection * dc, int nr)
{
	uc m[3];
	if (daveDebug & daveDebugPacket)
		LOG3("%s sendAck for message %d \n", dc->iface->name,nr);
	m[0]=0xB0;
	m[1]=0x01;
	m[2]=nr;
	return _daveSendWithPrefix31(dc, m, 3);
}

//#define CRC
#ifdef CRC
int testcrc(unsigned char *b, int size, int start)
{
	unsigned short sum, s2;
	int i, j;
	unsigned char *b1 = b;
	sum = start;

	for (j = 0; j < size-2; j++) {
		//	LOG2("I calc: %x.\n", sum);
		sum = sum ^ (b1[j]);
		//	LOG2("after xor data: %x.\n", sum);
		s2 = sum;
		for (i = 0; i <= 7; i++) {
			if (sum & 0x1) {
				sum = sum >> 1;
				sum = sum ^ 0x8408;
			} else
				sum = sum >> 1;
			//	    LOG2("loop: %x.\n", sum);
		}
	}
	/*    
	if (
	((sum /256)==b[size-1]) &&
	((sum %256)==b[size-2])
	) {
	printf ("found 1 %04x \n",start);
	return 1;
	}	
	*/    
	if (
		((sum %256)==b[size-2]) &&
		((sum /256)==b[size-1])
		){
			printf ("found 2 %04x %d\n",start, size-6);
			startTab[size-6]=start;
			return 1;
	}
	return 0;
}
#endif

int DECL2 _daveGetResponseMPI3(daveConnection *dc) {
	int res,count;
	if (daveDebug & daveDebugExchange)
		LOG1("enter _daveGetResponseMPI3\n");
	count=0;
	dc->msgIn[10]=0;
	do {
		//	res=dc->iface->ifread(dc->iface, dc->msgIn, 400);
		res=read1(dc->iface, dc->msgIn);
		count++;
	}while((count<5) && (dc->msgIn[10]!=0xF1));
	if (dc->msgIn[10]==0xF1) {
		dc->iface->seqNumber=dc->msgIn[1];
		_daveSendAckMPI3(dc, dc->msgIn[dc->iface->ackPos+1]);        
#ifdef CRC	
		if (startTab[res-7]==0) {
			for(count=0;count<0xffff;count++)
				testcrc(dc->msgIn,res-1, count);
		}    
#endif
		return 0;
	} 
	return -10;
}


int DECL2 _daveExchangeMPI3(daveConnection * dc, PDU * p) {
	_daveSendMessageMPI3(dc, p);
	dc->AnswLen=0;
	return _daveGetResponseMPI3(dc);
}

int DECL2 _daveDisconnectPLCMPI3(daveConnection * dc)
{
	//    uc m[]={
	//        0x80
	//    };
	uc fix[]= {04,0x82,0x0,0x0C,0x03,0x14,0x80};
	fix[4]=dc->connectionNumber2; 		// 1/10/05 trying Andrew's patch
	fix[5]=dc->connectionNumber; 		// 1/10/05 trying Andrew's patch
	//    _daveSendWithPrefix31(dc, m, 1);	
	fix[1]|=dc->MPIAdr;
	daveSendWithCRC3(dc->iface,fix,sizeof(fix));
	read1(dc->iface,dc->msgIn);
	return 0;
}    
/*
It seems to be better to complete this subroutine even if answers
from adapter are not as expected.
*/
int DECL2 _daveDisconnectAdapterMPI3(daveInterface * di) {
	//    uc m3[]={
	//        0x80
	//    };
	uc m2[]={
		1,4,2
	};
	uc b[daveMaxRawLen];
	//    uc m4[]={0x7e,0xca,0x2e,0x99,0x7e};
	//    _daveSendWithPrefix31(di, m3, sizeof(m3));
	//    read1(di,b);
	daveSendWithCRC3(di, m2, sizeof(m2));
	read1(di,b);
#ifdef CRC
	printf ("\n\n\n\nus startTab[]={");
	for (res=0;res<255;res++) {
		printf ("0x%04x , // %d\n",startTab[res],res);
	}
	printf ("}\n\n\n\n");
#endif    
	//    di->ifwrite(di, m4, 5);
	return 0;	
}

/*
It seems to be better to complete this subroutine even if answers
from adapter are not as expected.
*/
int DECL2 _daveDisconnectAdapterMPI(daveInterface * di) {
	int res;
	uc m2[]={
		1,4,2
	};

	uc b1[daveMaxRawLen];
	if (daveDebug & daveDebugInitAdapter) 
		LOG2("%s enter DisconnectAdapter()\n", di->name);	
	_daveSendSingle(di, STX);
	res=_daveReadMPI(di,b1);
	/*    if ((res!=1)||(b1[0]!=DLE)) return -1; */
	_daveSendWithCRC(di, m2, sizeof(m2));		
	if (daveDebug & daveDebugInitAdapter) 
		LOG2("%s daveDisconnectAdapter() step 1.\n", di->name);	
	res=_daveReadMPI(di, b1);
	/*    if ((res!=1)||(b1[0]!=DLE)) return -2; */
	res=_daveReadMPI(di, b1);
	/*    if ((res!=1)||(b1[0]!=STX)) return -3; */
	if (daveDebug & daveDebugInitAdapter) 
		LOG2("%s daveDisconnectAdapter() step 2.\n", di->name);	
	_daveSendSingle(di, DLE);
	_daveReadChars2(di, b1, daveMaxRawLen);
	//    _daveReadChars(di, b1, tmo_normal, daveMaxRawLen);
	_daveSendSingle(di, DLE);
	if (daveDebug & daveDebugInitAdapter) 
		_daveDump("got",b1,10);
	return 0;	
}

/*
This doesn't work yet. I'm not sure whether it is possible to get that
list after having connected to a PLC.
*/
int DECL2 _daveListReachablePartnersMPI(daveInterface * di,char * buf) {
	uc b1[daveMaxRawLen];
	uc m1[]={1,7,2};
	int res;
	res=_daveInitStep(di, 1, m1, sizeof(m1),"listReachablePartners()");	
	if (res) return 0;
	res=_daveReadMPI(di,b1);
	//    LOG2("res %d\n", res);	
	if(136==res){
		_daveSendSingle(di,DLE);
		memcpy(buf,b1+6,126);
		return 126;
	} else
		return 0;	
}   

int DECL2 _daveDisconnectPLCMPI(daveConnection * dc)
{
	int res;
	uc m[]={
		0x80
	};
	uc b1[daveMaxRawLen];

	_daveSendSingle(dc->iface, STX);

	res=_daveReadMPI(dc->iface,b1);
	if ((res!=1)||(b1[0]!=DLE)) {
		if (daveDebug & daveDebugPrintErrors)
			LOG2("%s *** no DLE before send.\n", dc->iface->name);	    
		return -1;
	}
	_daveSendWithPrefix(dc, m, 1);	

	res=_daveReadMPI(dc->iface,b1);
	if ((res!=1)||(b1[0]!=DLE)) {
		if (daveDebug & daveDebugPrintErrors)
			LOG2("%s *** no DLE after send.\n", dc->iface->name);	    
		return -2;
	}    

	_daveSendSingle(dc->iface, DLE);

	res=_daveReadMPI(dc->iface,b1);
	if ((res!=1)||(b1[0]!=STX)) return 6;
	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveDisConnectPLC() step 6.\n", dc->iface->name);	
	res=_daveReadMPI(dc->iface,b1);
	if (daveDebug & daveDebugConnect) 
		_daveDump("got",b1,10);
	_daveSendSingle(dc->iface, DLE);
	return 0;
}    

/*
build the PDU for a PDU length negotiation    
*/
int DECL2 _daveNegPDUlengthRequest(daveConnection * dc, PDU *p) {
	uc pa[]=	{0xF0, 0 ,0, 1, 0, 1, 3, 0xC0,};
	int res;
	int CpuPduLimit;
	PDU p2;
	p->header=dc->msgOut+dc->PDUstartO;
	_daveInitPDUheader(p,1);
	_daveAddParam(p, pa, sizeof(pa));
	if (daveDebug & daveDebugPDU) {
		_daveDumpPDU(p);
	}	
	res=_daveExchange(dc, p);
	if(res!=daveResOK) return res;
	res=_daveSetupReceivedPDU(dc, &p2);
	if(res!=daveResOK) return res;
	CpuPduLimit=daveGetU16from(p2.param+6);
	if (dc->maxPDUlength > CpuPduLimit) dc->maxPDUlength = CpuPduLimit; // use lower number as limit
	if (daveDebug & daveDebugConnect) {
		LOG3("\n*** Partner offered PDU length: %d used limit %d\n\n",CpuPduLimit,dc->maxPDUlength);
	}	
	return res;
}    

/* 
Open connection to a PLC. This assumes that dc is initialized by
daveNewConnection and is not yet used.
(or reused for the same PLC ?)
*/
int DECL2 _daveConnectPLCMPI2(daveConnection * dc) {
	int res;
	PDU p1;
	uc b1[daveMaxRawLen];

	uc b4[]={
		0x00,0x0d,0x00,0x03,0xe0,0x04,0x00,0x80,
		0x00,0x02,0x01,0x06,
		0x01,
		0x00,
		0x00,0x01,
		0x02,0x03,0x01,0x00
		/*^^ MPI ADDR */
	};

	us t4[]={
		0x00,0x0c,0x103,0x103,0xd0,0x04,0x00,0x80,
		0x01,0x06,
		0x00,0x02,0x00,0x01,0x02,
		0x03,0x01,0x00,
		0x01,0x00,0x10,0x03,0x4d
	};
	uc b5[]={	
		0x05,0x01,
	};

	us t5[]={    
		0x00,
		0x0c,
		0x103,0x103,0x05,0x01,0x10,0x03,0x1b
	};

	b4[3]=dc->connectionNumber; // 1/10/05 trying Andrew's patch
	b4[sizeof(b4)-3]=dc->MPIAdr;	
	t4[15]=dc->MPIAdr;
	t4[sizeof(t4)/2-1]^=dc->MPIAdr; /* 'patch' the checksum	*/

	_daveInitStep(dc->iface, 1, b4, sizeof(b4),"connectPLC(2)");    
	res=_daveReadMPI2(dc->iface,b1);
	if (_daveMemcmp(t4, b1, res)) {
		LOG2("%s daveConnectPLC() step 3 ends with 3.\n", dc->iface->name);	
		return 3;
	}	
	dc->connectionNumber2=b1[3]; // 1/10/05 trying Andrew's patch

	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 4.\n", dc->iface->name);	
	res=_daveReadMPI(dc->iface,b1);
	if ((res!=1)||(b1[0]!=DLE)) {
		LOG2("%s daveConnectPLC() step 4 ends with 4.\n", dc->iface->name);	
		return 4;
	}	

	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 5.\n", dc->iface->name);	
	_daveSendWithPrefix(dc, b5, sizeof(b5));		
	res=_daveReadMPI(dc->iface,b1);
	if ((res!=1)||(b1[0]!=DLE)) return 5;
	res=_daveReadMPI(dc->iface,b1);
	if ((res!=1)||(b1[0]!=STX)) return 5;

	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 6.\n", dc->iface->name);	
	_daveSendSingle(dc->iface, DLE);

	res=_daveReadMPI(dc->iface,b1);
	_daveSendSingle(dc->iface, DLE);
	if (dc->iface->protocol==daveProtoMPI4) _daveSendSingle(dc->iface, STX);
	if (_daveMemcmp(t5, b1, res)) return 6;

	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 6.\n", dc->iface->name);	
	res= _daveNegPDUlengthRequest(dc, &p1);
	return 0;
}

/* 
Open connection to a PLC. This assumes that dc is initialized by
daveNewConnection and is not yet used.
(or reused for the same PLC ?)
*/
int DECL2 _daveConnectPLCMPI1(daveConnection * dc) {
	int res;
	PDU p1;
	uc b4[]={
		0x04,0x80,0x80,0x0D,0x00,0x14,0xE0,0x04,0x00,0x80,0x00,0x02,0x00,0x02,0x01,0x00,0x01,0x00,
	};

	uc b4R[]={                   
		        //7E 11 1F E0 04 82 00 0D 00 14 E0 04 00 80 00 02 01 0F 01 00 06 04 02 AA BB 00 00 CC DD C0 A8 02 BC 01 03 18 87 7E 	
		 //Step7//7E 00 1F E0 04 86 00 0D 00 14 E0 04 00 80 00 02 01 0F 01 00 06 04 02 AA BB 00 00 CC DD C0 A8 02 BC 01 03 8C 60 7E 	

		 //    MPI                 connr                                                                 SUBNET              SUBNET
          0x04,0x80,0x00,0x0D,0x00,0x14,0xE0,0x04,0x00,0x80,0x00,0x02,0x01,0x0F,0x01,0x00,0x06,0x04,0x02,0xAA,0xBB,0x00,0x00,0xCC,0xDD,0xC0,0xA8,0x02,0xBC,0x01,0x03,0x18,0x87,0x7E 	

		//0x04,0x80,0x80,0x0D,0x00,0x14,0xE0,0x04,0x00,0x80,0x00,0x02,0x00,0x02,0x01,0x00,0x01,0x00,
	};

	us t4[]={
		0x04,0x80,0x180,0x0C,0x114,0x103,0xD0,0x04,	// 1/10/05 trying Andrew's patch
		0x00,0x80,
		0x00,0x02,0x00,0x02,0x01,
		0x00,0x01,0x00,
	};
	uc b5[]={	
		0x05,0x01,
	};
	us t5[]={    
		0x04,
		0x80,
		0x180,0x0C,0x114,0x103,0x05,0x01,
	};
	b4[1]|=dc->MPIAdr;	
	b4[5]=dc->connectionNumber; // 1/10/05 trying Andrew's patch

	t4[1]|=dc->MPIAdr;	
	t5[1]|=dc->MPIAdr;	

	_daveInitStep(dc->iface, 1, b4, sizeof(b4),"connectPLC(1)");

	res= _daveReadMPI2(dc->iface,dc->msgIn);
	if (_daveMemcmp(t4, dc->msgIn, sizeof(t4)/2)) return 3;
	dc->connectionNumber2=dc->msgIn[5]; // 1/10/05 trying Andrew's patch
	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC(1) step 4.\n", dc->iface->name);	

	if (_daveReadSingle(dc->iface)!=DLE) return 4;
	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 5.\n", dc->iface->name);	
	_daveSendWithPrefix(dc, b5, sizeof(b5));		
	if (_daveReadSingle(dc->iface)!=DLE) return 5;
	if (_daveReadSingle(dc->iface)!=STX) return 5;

	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 6.\n", dc->iface->name);	
	_daveSendSingle(dc->iface, DLE);
	res= _daveReadMPI2(dc->iface,dc->msgIn);
	if (_daveMemcmp(t5, dc->msgIn, sizeof(t5)/2)) return 6;
	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 6.\n", dc->iface->name);	
	res= _daveNegPDUlengthRequest(dc, &p1);
	return 0;
}

/*
Protocol specific functions for ISO over TCP:
*/
#ifndef AVR_NOOS
#ifdef HAVE_SELECT
int DECL2 _daveReadOne(daveInterface * di, uc *b) {
	fd_set FDS;
	struct timeval t;
	FD_ZERO(&FDS);
	FD_SET(di->fd.rfd, &FDS);

	t.tv_sec = di->timeout / 1000000;
	t.tv_usec = di->timeout % 1000000;
	/*	if (daveDebug & daveDebugByte) 
	LOG2("timeout %d\n",di->timeout); */
	if (select(di->fd.rfd + 1, &FDS, NULL, NULL, &t) <= 0) 
	{
		if (daveDebug & daveDebugByte) LOG1("timeout in readOne.\n");
		return (0);
	} else {
		return read(di->fd.rfd, b, 1);
	} 
};
#endif

#ifdef BCCWIN
int DECL2 _daveReadOne(daveInterface * di, uc *b) {
	unsigned long i;
	char res;
	ReadFile(di->fd.rfd, b, 1, &i,NULL);
	return i;
}
#endif 
#endif

#ifndef AVR_NOOS
#ifdef HAVE_SELECT
/*
Read one complete packet. The bytes 3 and 4 contain length information.
This version needs a socket filedescriptor that is set to O_NONBLOCK or
it will hang, if there are not enough bytes to read.
The advantage may be that the timeout is not used repeatedly.
*/
int DECL2 _daveReadISOPacket(daveInterface * di,uc *b) {
	int res,length;
	fd_set FDS;
	struct timeval t;
	FD_ZERO(&FDS);
	FD_SET(di->fd.rfd, &FDS);

	t.tv_sec = di->timeout / 1000000;
	t.tv_usec = di->timeout % 1000000;
	if (select(di->fd.rfd + 1, &FDS, NULL, NULL, &t) <= 0) {
		if (daveDebug & daveDebugByte) LOG1("timeout in ReadISOPacket.\n");
		return 0;
	} else {
		res=read(di->fd.rfd, b, 4);
		if (res<4) {
			if (daveDebug & daveDebugByte) {
				LOG2("res %d ",res);
				_daveDump("readISOpacket: short packet", b, res);
			}
			return (0); /* short packet */
		}
		length=b[3]+0x100*b[2];
		res+=read(di->fd.rfd, b+4, length-4);
		if (daveDebug & daveDebugByte) {
			LOG3("readISOpacket: %d bytes read, %d needed\n",res, length);
			_daveDump("readISOpacket: packet", b, res);    
		}
		return (res);
	}
}
/*
struct timeval t;
fd_set FDS,EFDS;
*/
int DECL2 _daveReadIBHPacket(daveInterface * di,uc *b) {
	struct timeval t;
	fd_set FDS,EFDS;
	int res,length;
	
	t.tv_sec = di->timeout / 1000000;
	t.tv_usec = di->timeout % 1000000;

	FD_ZERO(&FDS);
	FD_ZERO(&EFDS);
	FD_SET(di->fd.rfd, &FDS);
	FD_SET(di->fd.rfd, &EFDS);

	//    LOG2("time %d\n",di->timeout);	
	//    if (select(di->fd.rfd + 1, &FDS, NULL, NULL, &t) <= 0) {
	res= select(di->fd.rfd + 1, &FDS, NULL, &EFDS, &t);
	//    LOG2("select returned:%d\n",res);	
	//    LOG3("rest time %d %d\n",t.tv_sec,t.tv_usec);	
	//    if FD_ISSET(di->fd.rfd, &FDS)
	//	LOG1("true\n");	
	//    if FD_ISSET(di->fd.rfd, &EFDS)
	//	LOG1("e true\n");		
	//    if (select(di->fd.rfd + 1, &FDS, NULL, NULL, &t) <= 0) {
	if(res<=0) {
		if (daveDebug & daveDebugByte) LOG1("timeout in ReadIBHPacket.\n");
		return 0;
	} else {
		res=read(di->fd.rfd, b, 3);
		if (res==0) {
			t.tv_sec = 0;
			t.tv_usec = 20000;
			res= select(0, NULL, NULL, NULL, &t);
			//	    LOG3("rest time 2 %d %d\n",t.tv_sec,t.tv_usec);	
		}
		//        res=recv(di->fd.rfd, b, 3,0);
		if (res<3) {
			if (daveDebug & daveDebugByte) {
				//	        LOG2("res %d ",res);
				//	        _daveDump("readIBHpacket: short packet", b, res);
			}
			return (0); /* short packet */
		}
		length=b[2]+8; //b[3]+0x100*b[2];
		res+=read(di->fd.rfd, b+3, length-3);
		if (daveDebug & daveDebugByte) {
			LOG3("readIBHpacket: %d bytes read, %d needed\n",res, length);
			_daveDump("readIBHpacket: packet", b, res);    
		}
		return (res);
	}
}
#endif /* HAVE_SELECT */
#endif

#ifdef BCCWIN

int DECL2 _daveReadISOPacket(daveInterface * di,uc *b) {
	int res,i,length;
	fd_set FDS;
	struct timeval t;
	FD_ZERO(&FDS);
	FD_SET((SOCKET)(di->fd.rfd), &FDS);

	//di->timeout=10000000;
	//LOG2("timeout wrt: %d \n",di->timeout);
	t.tv_sec = di->timeout / 1000000;
	t.tv_usec = di->timeout % 1000000;
	//LOG2("timeout s: %d \n",t.tv_sec);
	//LOG2("timeout ms: %d \n",t.tv_usec );
	//printf("Socket in Read: %d\n",di->fd.rfd);
	//LOG2("WSAGetLastError bef. Select: %d \n",WSAGetLastError());
		
	if (select(/* di->fd.rfd + */ 1, &FDS, NULL, NULL, &t) <= 0) {
		LOG2("WSAGetLastError: %d \n", WSAGetLastError());
		if (daveDebug & daveDebugByte) LOG1("timeout in ReadISOPacket.\n");
		return 0;
	} else {
		i = recv((SOCKET)(di->fd.rfd), b, 4, 0);
		res = i;
		if (res <= 0) {
			if (daveDebug & daveDebugByte) LOG1("timeout in ReadISOPacket.\n");
			return 0;
		} else {
			if (res < 4) {
				if (daveDebug & daveDebugByte) {
					LOG2("res %d ",res);
					_daveDump("readISOpacket: short packet", b, res);
				}
				return (0); /* short packet */
			}
			length=b[3]+0x100*b[2];
			i=recv((SOCKET)(di->fd.rfd), b+4, length-4, 0);
			res+=i;
			if (daveDebug & daveDebugByte) {
				LOG3("readISOpacket: %d bytes read, %d needed\n",res, length);
				_daveDump("readISOpacket: packet", b, res);    
			}
			return (res);
		}
	}
}

#ifndef AVR_NOOS
int DECL2 _daveReadIBHPacket(daveInterface * di,uc *b) {
	int res,length;
	fd_set FDS;
	struct timeval t;
	FD_ZERO(&FDS);
	FD_SET((SOCKET)(di->fd.rfd), &FDS);

	t.tv_sec = di->timeout / 1000000;
	t.tv_usec = di->timeout % 1000000;
	
	if (select(/*di->fd.rfd +*/ 1, &FDS, NULL, NULL, &t) <= 0) {
		if (daveDebug & daveDebugByte) LOG1("timeout in ReadIBHPacket.\n");
		return 0;
	} else {
		//	res=read(di->fd.rfd, b, 3);
		res=recv((SOCKET)(di->fd.rfd), b, 3, 0);
		if (res<3) {
			if (daveDebug & daveDebugByte) {
				LOG2("res %d ",res);
				_daveDump("readIBHpacket: short packet", b, res);
			}
			return (0); /* short packet */
		}
		length=b[2]+8; //b[3]+0x100*b[2];
		//	res+=read(di->fd.rfd, b+3, length-3);
		res+=recv((SOCKET)(di->fd.rfd), b+3, length-3, 0);
		if (daveDebug & daveDebugByte) {
			LOG3("readIBHpacket: %d bytes read, %d needed\n",res, length);
			_daveDump("readIBHpacket: packet", b, res);    
		}
		return (res);
	}
}
#endif

/*
int DECL2 _daveReadIBHPacket(daveInterface * di,uc *b) {
int res,i,length;
i=recv((SOCKET)(di->fd.rfd), b, 3, 0);
res=i;
if (res <= 0) {
if (daveDebug & daveDebugByte) LOG1("timeout in ReadIBHPacket.\n");
return 0;
} else {
if (res<3) {
if (daveDebug & daveDebugByte) {
LOG2("res %d ",res);
_daveDump("readIBHpacket: short packet", b, res);
}
return (0); // short packet 
}
length=b[2]+8; //b[3]+0x100*b[2];
i=recv((SOCKET)(di->fd.rfd), b+3, length-3, 0);
res+=i;
if (daveDebug & daveDebugByte) {
LOG3("readIBHpacket: %d bytes read, %d needed\n",res, length);
_daveDump("readIBHpacket: packet", b, res);    
}
return (res);
}
}
*/

#endif /* */

#ifndef AVR_NOOS
int DECL2 _daveSendISOPacket(daveConnection * dc, int size) {
	unsigned long i;
	int ret;
	size+=4;
	*(dc->msgOut+3)=size % 0x100;	//was %0xFF, certainly a bug	
	*(dc->msgOut+2)=size / 0x100;
	*(dc->msgOut+1)=0;
	*(dc->msgOut+0)=3;
	if (daveDebug & daveDebugByte) 
		_daveDump("send packet: ",dc->msgOut,size);
#ifdef HAVE_SELECT
	daveWriteFile(dc->iface->fd.wfd, dc->msgOut, size, i);
#endif    
#ifdef BCCWIN
	//printf("sendsock %d\n",dc->iface->fd.wfd);
	ret = send((SOCKET)(dc->iface->fd.wfd), dc->msgOut, size, 0); //(unsigned int)
	if (ret==SOCKET_ERROR )
		if (daveDebug & daveDebugByte) LOG2("_daveSendISOPacket WSAGetLastError: %d \n",WSAGetLastError());
	
#endif
	return 0;
}
#endif

#ifndef AVR_NOOS
#define ISOTCPminPacketLength 16
int DECL2 _daveGetResponseISO_TCP(daveConnection * dc) {
	int res;
	res=_daveReadISOPacket(dc->iface,dc->msgIn);
	if(res==7) {
		if (daveDebug & daveDebugByte) 
			LOG1("CPU sends funny 7 byte packets.\n");
		res=_daveReadISOPacket(dc->iface,dc->msgIn);
	}
	if (res==0) return daveResTimeout; 
	if (res<ISOTCPminPacketLength) return  daveResShortPacket; 
	return 0;
}
/*
Executes the dialog around one message:
*/
int DECL2 _daveExchangeTCP(daveConnection * dc, PDU * p) {
	int res;
	if (daveDebug & daveDebugExchange) {
		LOG2("%s enter _daveExchangeTCP\n", dc->iface->name);
	}    
	*(dc->msgOut+6)=0x80;
	*(dc->msgOut+5)=0xf0;
	*(dc->msgOut+4)=0x02;
	_daveSendISOPacket(dc,3+p->hlen+p->plen+p->dlen);
	res=_daveReadISOPacket(dc->iface,dc->msgIn);
	if(res==7) {
		if (daveDebug & daveDebugByte) 
			LOG1("CPU sends funny 7 byte packets.\n");
		res=_daveReadISOPacket(dc->iface,dc->msgIn);
	}
	if (daveDebug & daveDebugExchange) {
		LOG3("%s _daveExchangeTCP res from read %d\n", dc->iface->name,res);	    
	}
	if (res==0) return daveResTimeout; 
	if (res<=ISOTCPminPacketLength) return  daveResShortPacket; 
	return 0;
}

int DECL2 _daveConnectPLCTCP(daveConnection * dc) {
	int res, success, retries;
	uc b4[]={
		0x11,		//Length
		0xE0,		// TDPU Type CR = Connection Request (see RFC1006/ISO8073)
		0x00, 0x00, // TPDU Destination Reference (unknown)
		0x00, 0x01, // TPDU Source-Reference (my own reference, should not be zero)
		0x00,		// TPDU Class 0 and no Option 
		0xC1,		// Parameter Source-TSAP
		2,			// Length of this parameter 
		1, 			// Function (1=PG,2=OP,3=Step7Basic)
		0,			// Rack (Bit 7-5) and Slot (Bit 4-0)
		0xC2,		// Parameter Destination-TSAP
		2,			// Length of this parameter 
		dc->ConnectionType, 			// Function (1=PG,2=OP,3=Step7Basic)
		(dc->slot + dc->rack * 32),			// Rack (Bit 7-5) and Slot (Bit 4-0)
		0xC0,		// Parameter requested TPDU-Size
		1,			// Length of this parameter 
		9,			// requested TPDU-Size 8=256 Bytes, 9=512 Bytes , a=1024 Bytes
	};

	uc b4R[]={			// for routing
		6 + 30 + 30 + 3,	// Length over all without this byte (fixed
		// Data 6 Bytes + size of Parameters (3 for C0h,30 for C1h+C2h)

		0xE0,		// TDPU Type CR = Connection Request (see RFC1006/ISO8073)
		0x00,0x00,	// TPDU Destination Reference (unknown)
		0x00,0x01,	// TPDU Source-Reference (my own reference, should not be zero)
		0x00,		// TPDU Class 0 and no Option 

		0xC1,		// Parameter Source-TSAP
		28,		// Length of this parameter 
		1,		// one block of data (???)
		0,		// Length for S7-Subnet-ID
		0,		// Length of PLC-Number
		2,		// Length of Function/Rack/Slot
		0,0,0,0,0,0,0,0,	// empty Data 
		0,0,0,0,0,0,0,0,
		0,0,0,0,0,0,
		dc->ConnectionType,		// Function (1=PG,2=OP,3=Step7Basic)
		(dc->slot + dc->rack * 32),		// Rack (Bit 7-5) and Slot (Bit 4-0)

		0xC2,		// Parameter Destination-TSAP
		28,		// Length of this parameter 
		1,		// one block of data (???)
		6,		// Length for S7-Subnet-ID
		dc->_routingDestinationSize,		// Length of PLC-Number - 04 if you use a IP as Destination!
		2,		// Length of Function/Rack/Slot

		(unsigned char) (dc->routingSubnetFirst >> 8), (unsigned char) dc->routingSubnetFirst,	// first part of S7-Subnet-ID 
		// (look into the S7Project/Network configuration)
		0x00,0x00,		// fix always 0000 (reserved for later use ?)
		(unsigned char) (dc->routingSubnetSecond >> 8), (unsigned char) dc->routingSubnetSecond,		// second part of S7-Subnet-ID 
		// (see S7Project/Network configuration)

		dc->_routingDestination1,			// PLC-Number (0-126) or IP Adress (then 4 Bytes are used)
		dc->_routingDestination2,
		dc->_routingDestination3,
		dc->_routingDestination4,

		0,0,0,0,0,	// empty 
		0,0,0,0,0,0,0,

		dc->routingConnectionType,		// Function (1=PG,2=OP,3=Step7Basic)
		(dc->routingSlot + dc->routingRack*32),		// Rack (Bit 7-5) and Slot (Bit 4-0)
		// 0 for slot = let select the plc itself the correct slotnumber

		0xC0,		// Parameter requested TPDU-Size
		1,		// Length of this parameter 
		9,		// requested TPDU-Size 8=256 Bytes, 9=512 Bytes , a=1024 Bytes
	};	

	uc b243[]={
		0x11,0xE0,0x00,
		0x00,0x00,0x01,0x00,
		0xC1,2,'M','W',
		0xC2,2,'M','W',
		0xC0,1,9,
	};

	PDU p1;	
	success=0;
	retries=0;

	if (dc->iface->protocol==daveProtoISOTCP243) {
		memcpy(dc->msgOut+4, b243, sizeof(b243));	
	} else if (dc->iface->protocol==daveProtoISOTCP &&  !dc->routing) {
		memcpy(dc->msgOut+4, b4, sizeof(b4));
		//printf("******** do inc %d\n",a);
		//dc->msgOut[17]=dc->rack+1;
		//dc->msgOut[18]=dc->slot;
		//dc->msgOut[18]=dc->slot + dc->rack * 32 ;
	} else {
		memcpy(dc->msgOut+4, b4R, sizeof(b4R));	// with routing over MPI
		//dc->msgOut[17]=dc->rack+1;			// this is probably wrong
		//dc->msgOut[18]=dc->slot;
		//dc->msgOut[18]=dc->slot + dc->rack * 32 ;
	}	

	if (!dc->routing)
		_daveSendISOPacket(dc, sizeof(b4)); 
	else
		_daveSendISOPacket(dc, sizeof(b4R)); 

	do {	
		res=_daveReadISOPacket(dc->iface,dc->msgIn);
		if (daveDebug & daveDebugConnect) {
			LOG2("%s daveConnectPLC() step 1. ", dc->iface->name);	
			_daveDump("got packet: ", dc->msgIn, res);
		}
		if ((res==22 && !dc->routing) || (res==74 && dc->routing)) {
			success=1;
		} else {
			if (daveDebug & daveDebugPrintErrors){
				LOG2("%s error in daveConnectPLC() step 1. retrying...", dc->iface->name);	
			}	
		}
		retries++;
	} while ((success==0)&&(retries<3));
	if (success==0) return -1;

	retries=0;
	do {
		res= _daveNegPDUlengthRequest(dc, &p1);
		if (res==0) {
			return res;
		} else {
			if (daveDebug & daveDebugPrintErrors){
				LOG2("%s error in daveConnectPLC() step 1. retrying...\n", dc->iface->name);	
			}	
		}
		retries++;
	} while (retries<3);	
	return -1;
}
#endif

/*
Changes: 
07/19/04 removed unused vars.
*/

/*
Changes: 
07/19/04 added return values in daveInitStep and daveSendWithPrefix2.
09/09/04 applied patch for variable Profibus speed from Andrew Rostovtsew.
*/

/* PPI specific functions: */
#define tmo_normalPPI 140000

void DECL2 _daveSendLength(daveInterface * di, int len) {
	uc c[]={104,0,0,104};
	c[1]=len;
	c[2]=len;
	di->ifwrite(di, (char *)c, 4);
	if ((daveDebug & daveDebugByte)!=0) {
		_daveDump("I send", c, 4);
	}	
}

void DECL2 _daveSendIt(daveInterface * di, uc * b, int size) {
	int i;
	us sum = 0;
	for (i=0;i<size;i++) {
		sum+=b[i];
	}
	sum=sum & 0xff;
	b[size]=sum;
	size++;
	b[size]=SYN;
	size++;
	di->ifwrite(di, (char*)b, size);

	if ((daveDebug & daveDebugByte)!=0) {
		LOG2("send %d\n",i);
		_daveDump("I send", b, size);
	}	
}

void DECL2 _daveSendRequestData(daveConnection * dc,int alt) {
	uc b[]={DLE,0,0,0x5C,0,0};
	b[1]=dc->MPIAdr;
	b[2]=dc->iface->localMPI;
	if(alt) b[3]=0x7c; else b[3]=0x5c;
	dc->iface->ifwrite(dc->iface, (char*)b, 1);  //cs: 
	_daveSendIt(dc->iface, b+1, sizeof(b)-3);
}


int seconds, thirds;

int DECL2 _daveGetResponsePPI(daveConnection *dc) {
	int res, expectedLen, expectingLength, i, sum, alt;
	uc * b;
	res = 0;
	expectedLen=6;
	expectingLength=1;
	b=dc->msgIn;
	alt=1;
	while ((expectingLength)||(res<expectedLen)) {
		i = _daveReadChars2(dc->iface, dc->msgIn+res, 1);
		res += i;
		if ((daveDebug & daveDebugByte)!=0) {
			LOG3("i:%d res:%d\n",i,res);
			FLUSH;
		}        
		if (i == 0) {
			return daveResTimeout;
		} else {
			if ( (expectingLength) && (res==1) && (b[0] == 0xE5)) {
				if(alt) {
					_daveSendRequestData(dc,alt);
					res=0;
					alt=0;
				} else {
					_daveSendRequestData(dc,alt);
					res=0;
					alt=1;
				}
			}
			if ( (expectingLength) && (res>=4) && (b[0] == b[3]) && (b[1] == b[2]) ) {
				expectedLen=b[1]+6;
				expectingLength=0;
			}
		}	
	}
	if ((daveDebug & daveDebugByte)!=0) {
		LOG2("res %d testing lastChar\n",res);
	}	
	if (b[res-1]!=SYN) {
		LOG1("block format error\n");
		return 1024;
	}
	if ((daveDebug & daveDebugByte)!=0) {
		LOG1("testing check sum\n");
	}	
	sum=0;
	for (i=4; i<res-2; i++){
		sum+=b[i];
	}
	sum=sum&0xff;
	if ((daveDebug & daveDebugByte)!=0) {
		LOG3("I calc: %x sent: %x\n", sum, b[res-2]);
	}	
	if (b[res-2]!=sum) {
		if ((daveDebug & daveDebugByte)!=0) {
			LOG1("checksum error\n");
		}	
		return 2048;
	}
	return 0;
} 

int DECL2 _daveExchangePPI(daveConnection * dc,PDU * p1) {
	int i,res=0,len;
	dc->msgOut[0]=dc->MPIAdr;	/* address ? */
	dc->msgOut[1]=dc->iface->localMPI;	
	dc->msgOut[2]=108;	
	len=3+p1->hlen+p1->plen+p1->dlen;	/* The 3 fix bytes + all parts of PDU */
	_daveSendLength(dc->iface, len);			
	_daveSendIt(dc->iface, dc->msgOut, len);
	i = _daveReadChars2(dc->iface, dc->msgIn+res, 1);
	if ((daveDebug & daveDebugByte)!=0) {
		LOG3("i:%d res:%d\n",i,res);
		_daveDump("got",dc->msgIn,i); // 5.1.2004
	}	
	if (i == 0) {
		seconds++;
		_daveSendLength(dc->iface, len);			
		_daveSendIt(dc->iface, dc->msgOut, len);
		i = _daveReadChars2(dc->iface, dc->msgIn+res, 1);
		if (i == 0) {
			thirds++;
			_daveSendLength(dc->iface, len);			
			_daveSendIt(dc->iface, dc->msgOut, len);
			i = _daveReadChars2(dc->iface, dc->msgIn+res, 1);
			if (i == 0) {
				LOG1("timeout in _daveExchangePPI!\n");
				FLUSH;
				return daveResTimeout;
			}	
		}    
	}
	_daveSendRequestData(dc,0); 
	return _daveGetResponsePPI(dc);
}    

int DECL2 _daveConnectPLCPPI(daveConnection * dc) {
	PDU p;
	return _daveNegPDUlengthRequest(dc,&p);
} 

/* 
"generic" functions calling the protocol specific ones (or the dummies)
*/
int DECL2 daveInitAdapter(daveInterface * di) {
	return di->initAdapter(di);
}

int DECL2 daveConnectPLC(daveConnection * dc) {
	return dc->iface->connectPLC(dc);
}

int DECL2 daveDisconnectPLC(daveConnection * dc) {
	return dc->iface->disconnectPLC(dc);
}

int DECL2 daveDisconnectAdapter(daveInterface * di) {
	return di->disconnectAdapter(di);
}

int DECL2 _daveExchange(daveConnection * dc, PDU *p) {
	int res;
	if ((p->header[4]==0)&&(p->header[5]==0)) { /* do not number already numbered PDUs 12/10/04 */
		dc->PDUnumber++;
		if (daveDebug & daveDebugExchange) {
			LOG2("_daveExchange PDU number: %d\n", dc->PDUnumber);
		}
		p->header[5]=dc->PDUnumber % 256;	// test!
		p->header[4]=dc->PDUnumber / 256;	// test!
	}
	res=dc->iface->exchange(dc, p);
	if (((daveDebug & daveDebugExchange)!=0) ||((daveDebug & daveDebugErrorReporting)!=0)) {
		LOG2("result of exchange: %d\n",res);
	}	
	return res;
}

int DECL2 daveSendMessage(daveConnection * dc, PDU *p) {
	return dc->iface->sendMessage(dc, p);
}

int DECL2 daveListReachablePartners(daveInterface * di, char * buf) {
	return di->listReachablePartners(di, buf);
}

int DECL2 daveGetResponse(daveConnection * dc) {
	return dc->iface->getResponse(dc);
}

/**
Newer conversion routines. As the terms WORD, INT, INTEGER etc have different meanings
for users of different programming languages and compilers, I choose to provide a new 
set of conversion routines named according to the bit length of the value used. The 'U'
or 'S' stands for unsigned or signed.
**/
/*
Get a value from the position b points to. B is typically a pointer to a buffer that has
been filled with daveReadBytes:
*/
int DECL2 daveGetS8from(uc *b) {
	char* p=(char*)b;
	return *p;
}

int DECL2 daveGetU8from(uc *b) {
	return *b;
}

int DECL2 daveGetS16from(uc *b) {
	union {
		short a;
		uc b[2];
	} u;
#ifdef DAVE_LITTLE_ENDIAN    
	u.b[1]=*b;
	b++;
	u.b[0]=*b;
#else
	u.b[0]=*b;
	b++;
	u.b[1]=*b;
#endif
	return u.a;
}

int DECL2 daveGetU16from(uc *b) {
	union {
		unsigned short a;
		uc b[2];
	} u;
#ifdef DAVE_LITTLE_ENDIAN    
	u.b[1]=*b;
	b++;
	u.b[0]=*b;
#else
	u.b[0]=*b;
	b++;
	u.b[1]=*b;
#endif    
	return u.a;
}

int DECL2 daveGetS32from(uc *b) {
	union {
		int a;
		uc b[4];
	} u;
#ifdef DAVE_LITTLE_ENDIAN
	u.b[3]=*b;
	b++;
	u.b[2]=*b;
	b++;
	u.b[1]=*b;
	b++;
	u.b[0]=*b;
#else
	u.b[0]=*b;
	b++;
	u.b[1]=*b;
	b++;
	u.b[2]=*b;
	b++;
	u.b[3]=*b;
#endif
	return u.a;
}

unsigned int DECL2 daveGetU32from(uc *b) {
	union {
		unsigned int a;
		uc b[4];
	} u;
#ifdef DAVE_LITTLE_ENDIAN
	u.b[3]=*b;
	b++;
	u.b[2]=*b;
	b++;
	u.b[1]=*b;
	b++;
	u.b[0]=*b;
#else
	u.b[0]=*b;
	b++;
	u.b[1]=*b;
	b++;
	u.b[2]=*b;
	b++;
	u.b[3]=*b;
#endif    
	return u.a;
}

float DECL2 daveGetFloatfrom(uc *b) {
	union {
		float a;
		uc b[4];
	} u;
#ifdef DAVE_LITTLE_ENDIAN
	u.b[3]=*b;
	b++;
	u.b[2]=*b;
	b++;
	u.b[1]=*b;
	b++;
	u.b[0]=*b;
#else
	u.b[0]=*b;
	b++;
	u.b[1]=*b;
	b++;
	u.b[2]=*b;
	b++;
	u.b[3]=*b;
#endif    
	return u.a;
}

/*
Get a value from the current position in the last result read on the connection dc.
This will increment an internal pointer, so the next value is read from the position
following this value.
*/
int DECL2 daveGetS8(daveConnection * dc) {
	char * p;
#ifdef DEBUG_CALLS
	LOG2("daveGetS8(dc:%p)\n",dc);
	FLUSH;
#endif	
	p=(char *) dc->resultPointer;
	dc->resultPointer++;
	return *p;
}

int DECL2 daveGetU8(daveConnection * dc) {
	uc * p;
#ifdef DEBUG_CALLS
	LOG2("daveGetU8(dc:%p)\n",dc);
	FLUSH;
#endif	
	p=dc->resultPointer;
	dc->resultPointer++;
	return *p;
}    

int DECL2 daveGetS16(daveConnection * dc) {
	union {
		short a;
		uc b[2];
	} u;
#ifdef DEBUG_CALLS
	LOG2("daveGetS16(dc:%p)\n",dc);
	FLUSH;
#endif	
#ifdef DAVE_LITTLE_ENDIAN
	u.b[1]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[0]=*(dc->resultPointer);
#else
	u.b[0]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[1]=*(dc->resultPointer);
#endif
	dc->resultPointer++;
	return u.a;
}

int DECL2 daveGetU16(daveConnection * dc) {
	union {
		unsigned short a;
		uc b[2];
	} u;
#ifdef DEBUG_CALLS
	LOG2("daveGetU16(dc:%p)\n",dc);
	FLUSH;
#endif	    
#ifdef DAVE_LITTLE_ENDIAN
	u.b[1]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[0]=*(dc->resultPointer);
#else
	u.b[0]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[1]=*(dc->resultPointer);
#endif    
	dc->resultPointer++;
	return u.a;
}

int DECL2 daveGetS32(daveConnection * dc) {
	union {
		int a;
		uc b[4];
	} u;
#ifdef DEBUG_CALLS
	LOG2("daveGetS32(dc:%p)\n",dc);
	FLUSH;
#endif	    
#ifdef DAVE_LITTLE_ENDIAN
	u.b[3]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[2]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[1]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[0]=*(dc->resultPointer);
#else
	u.b[0]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[1]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[2]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[3]=*(dc->resultPointer);
#endif
	dc->resultPointer++;
	return u.a;
}

unsigned int DECL2 daveGetU32(daveConnection * dc) {
	union {
		unsigned int a;
		uc b[4];
	} u;
#ifdef DEBUG_CALLS
	LOG2("daveGetU32(dc:%p)\n",dc);
	FLUSH;
#endif	    

#ifdef DAVE_LITTLE_ENDIAN
	u.b[3]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[2]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[1]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[0]=*(dc->resultPointer);
#else
	u.b[0]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[1]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[2]=*(dc->resultPointer);
	dc->resultPointer++;
	u.b[3]=*(dc->resultPointer);
#endif    
	dc->resultPointer++;
	return u.a;
}
/*
Get a value from a given position in the last result read on the connection dc.
*/
int DECL2 daveGetS8At(daveConnection * dc, int pos) {
	char * p=(char *)(dc->_resultPointer);
	p+=pos;
	return *p;
}

int DECL2 daveGetU8At(daveConnection * dc, int pos)  {
	uc * p=(uc *)(dc->_resultPointer);
	p+=pos;
	return *p;
}

int DECL2 daveGetS16At(daveConnection * dc, int pos) {
	union {
		short a;
		uc b[2];
	} u;
	uc * p=(uc *)(dc->_resultPointer);
	p+=pos;
#ifdef DAVE_LITTLE_ENDIAN
	u.b[1]=*p;
	p++;
	u.b[0]=*p;
#else
	u.b[0]=*p;
	p++;
	u.b[1]=*p;
#endif
	return u.a;
}

int DECL2 daveGetU16At(daveConnection * dc, int pos) {
	union {
		unsigned short a;
		uc b[2];
	} u;
	uc * p=(uc *)(dc->_resultPointer);
	p+=pos;
#ifdef DAVE_LITTLE_ENDIAN
	u.b[1]=*p;
	p++;
	u.b[0]=*p;
#else
	u.b[0]=*p;
	p++;
	u.b[1]=*p;
#endif    
	return u.a;
}

int DECL2 daveGetS32At(daveConnection * dc, int pos) {
	union {
		int a;
		uc b[4];
	} u;
	uc * p=dc->_resultPointer;
	p+=pos;
#ifdef DAVE_LITTLE_ENDIAN    
	u.b[3]=*p;
	p++;
	u.b[2]=*p;
	p++;
	u.b[1]=*p;
	p++;
	u.b[0]=*p;
#else
	u.b[0]=*p;
	p++;
	u.b[1]=*p;
	p++;
	u.b[2]=*p;
	p++;
	u.b[3]=*p;
#endif
	return u.a;
}

unsigned int DECL2 daveGetU32At(daveConnection * dc, int pos) {
	union {
		unsigned int a;
		uc b[4];
	} u;
	uc * p=(uc *)(dc->_resultPointer);
	p+=pos;
#ifdef DAVE_LITTLE_ENDIAN    
	u.b[3]=*p;
	p++;
	u.b[2]=*p;
	p++;
	u.b[1]=*p;
	p++;
	u.b[0]=*p;
#else
	u.b[0]=*p;
	p++;
	u.b[1]=*p;
	p++;
	u.b[2]=*p;
	p++;
	u.b[3]=*p;
#endif    
	return u.a;
}
/*
put one byte into buffer b:
*/
uc * DECL2 davePut8(uc *b,int v) {
	*b = v & 0xff;
	b++;
	return b;
}

uc * DECL2 davePut16(uc *b,int v) {
	union {
		short a;
		uc b[2];
	} u;
	u.a=v;
#ifdef DAVE_LITTLE_ENDIAN    
	*b=u.b[1];
	b++;
	*b=u.b[0];
#else    
	*b=u.b[0];
	b++;
	*b=u.b[1];
#endif
	b++;
	return b;
}

uc * DECL2 davePut32(uc *b, int v) {
	union {
		int a;
		uc b[2];
	} u;
	u.a=v;
#ifdef DAVE_LITTLE_ENDIAN        
	*b=u.b[3];
	b++;
	*b=u.b[2];
	b++;
	*b=u.b[1];
	b++;
	*b=u.b[0];
#else    
	*b=u.b[0];
	b++;
	*b=u.b[1];
	b++;
	*b=u.b[2];
	b++;
	*b=u.b[3];
#endif    
	b++;
	return b;
}

uc * DECL2 davePutFloat(uc *b,float v) {
	union {
		float a;
		uc b[2];
	} u;
	u.a=v;
#ifdef DAVE_LITTLE_ENDIAN        
	*b=u.b[3];
	b++;
	*b=u.b[2];
	b++;
	*b=u.b[1];
	b++;
	*b=u.b[0];
#else    
	*b=u.b[0];
	b++;
	*b=u.b[1];
	b++;
	*b=u.b[2];
	b++;
	*b=u.b[3];
#endif
	b++;
	return b;
}

void DECL2 davePut8At(uc *b, int pos, int v) {
	union {
		short a;
		uc b[2];
	} u;
	u.a=v;
	b+=pos;
	*b=v & 0xff;
}

void DECL2 davePut16At(uc *b, int pos, int v) {
	union {
		short a;
		uc b[2];
	} u;
	u.a=v;
	b+=pos;
#ifdef DAVE_LITTLE_ENDIAN        
	*b=u.b[1];
	b++;
	*b=u.b[0];
#else    
	*b=u.b[0];
	b++;
	*b=u.b[1];
#endif    
}

void DECL2 davePut32At(uc *b, int pos, int v) {
	union {
		int a;
		uc b[2];
	} u;
	u.a=v;
	b+=pos;
#ifdef DAVE_LITTLE_ENDIAN        
	*b=u.b[3];
	b++;
	*b=u.b[2];
	b++;
	*b=u.b[1];
	b++;
	*b=u.b[0];
#else    
	*b=u.b[0];
	b++;
	*b=u.b[1];
	b++;
	*b=u.b[2];
	b++;
	*b=u.b[3];
#endif    
}

void DECL2 davePutFloatAt(uc *b, int pos,float v) {
	union {
		float a;
		uc b[2];
	} u;
	u.a=v;
	b+=pos;
#ifdef DAVE_LITTLE_ENDIAN        
	*b=u.b[3];
	b++;
	*b=u.b[2];
	b++;
	*b=u.b[1];
	b++;
	*b=u.b[0];
#else    
	*b=u.b[0];
	b++;
	*b=u.b[1];
	b++;
	*b=u.b[2];
	b++;
	*b=u.b[3];
#endif    
}
/*
"passive mode" functions. Needed to "simulate" an S7 PLC.
*/
userReadFunc readCallBack=NULL;
userWriteFunc writeCallBack=NULL;

void DECL2 _daveConstructReadResponse(PDU * p) {
	uc pa[]={4,1}; 
	uc da[]={0xFF,4,0,0}; 
	_daveInitPDUheader(p,3);
	_daveAddParam(p, pa, sizeof(pa));
	_daveAddData(p, da, sizeof(da));    
}

void DECL2 _daveConstructBadReadResponse(PDU * p) {
	uc pa[]={4,1}; 
	uc da[]={0x0A,0,0,0}; 
	_daveInitPDUheader(p,3);
	_daveAddParam(p, pa, sizeof(pa));
	_daveAddData(p, da, sizeof(da));    
}

void DECL2 _daveConstructWriteResponse(PDU * p) {
	uc pa[]={5,1}; 
	uc da[]={0xFF}; 
	_daveInitPDUheader(p,3);
	_daveAddParam(p, pa, sizeof(pa));
	_daveAddData(p, da, sizeof(da));    
}

void DECL2 _daveHandleRead(PDU * p1,PDU * p2) {
	int result;
	uc * userBytes=NULL; //is this really better than reading from a dangling pointer?
	int bytes=0x100*p1->param[6]+p1->param[7];
	int DBnumber=0x100*p1->param[8]+p1->param[9];
	int area=p1->param[10];
	int start=0x10000*p1->param[11]+0x100*p1->param[12]+p1->param[13];
	LOG5("read %d bytes from %s %d beginning at %d.\n",
		bytes, daveAreaName(area),DBnumber,start);
	if (readCallBack)	
		userBytes=readCallBack(area, DBnumber,start, bytes, &result);	
	_daveConstructReadResponse(p2);	
	_daveAddValue(p2, userBytes, bytes);
	_daveDumpPDU(p2);
};

void DECL2 _daveHandleWrite(PDU * p1,PDU * p2) {
	int result,bytes=0x100*p1->param[6]+p1->param[7];
	int DBnumber=0x100*p1->param[8]+p1->param[9];
	int area=p1->param[10];
	int start=0x10000*p1->param[11]+0x100*p1->param[12]+p1->param[13];
	LOG5("write %d bytes to %s %d beginning at %d.\n",
		bytes, daveAreaName(area),DBnumber,start);
	if (writeCallBack)	
		writeCallBack(area, DBnumber,start, bytes, &result, p1->data+4);	
	LOG1("after callback\n");
	FLUSH;
	_daveConstructWriteResponse(p2);	
	LOG1("after ConstructWriteResponse()\n");
	FLUSH;
	_daveDumpPDU(p2);
	LOG1("after DumpPDU()\n");
	FLUSH;
};


/*
10/04/2003 	PPI has an address. Implemented now.
06/03/2004 	Fixed a bug in _davePPIexchange, which caused timeouts
when the first call to readChars returned less then 4 characters.
*/
#ifndef AVR_NOOS
int DECL2 _daveWriteIBH(daveInterface * di, uc * buffer, int len) { //cs: let it be uc 
	int res;
	if (daveDebug & daveDebugByte) {
		_daveDump("writeIBH: ", buffer, len);
	}	
#ifdef HAVE_SELECT    
	res=write(di->fd.wfd, buffer, len);
#endif    
#ifdef BCCWIN
	//    res=send((SOCKET)(di->fd.wfd), buffer, ((len+1)/2)*2, 0);
	res=send((SOCKET)(di->fd.wfd), buffer, len, 0);
#endif
	return res;
}    

int DECL2 _davePackPDU(daveConnection * dc,PDU *p) {
	IBHpacket * ibhp;
	MPIheader * hm= (MPIheader*) (dc->msgOut+sizeof(IBHpacket)); // MPI headerPDU begins packet header
	hm->MPI=dc->MPIAdr;
	hm->localMPI=dc->iface->localMPI;
	hm->src_conn=dc->ibhSrcConn;
	hm->dst_conn=dc->ibhDstConn;
	hm->len=2+p->hlen+p->plen+p->dlen;		// set MPI length
	hm->func=0xf1;				// set MPI "function code"
	hm->packetNumber=_daveIncMessageNumber(dc);
	ibhp = (IBHpacket*) dc->msgOut;
	ibhp->ch1=7;
	ibhp->ch2=0xff;
	ibhp->len=hm->len+5;
	ibhp->packetNumber=dc->packetNumber;
	dc->packetNumber++;
	ibhp->rFlags=0x82;
	ibhp->sFlags=0;

	return 0;
}    

uc _MPIack[]={
	0x07,0xff,0x08,0x05,0x00,0x00,0x82,0x00, 0x15,0x14,0x02,0x00,0x03,0xb0,0x01,0x00,
};


void DECL2 _daveSendMPIAck_IBH(daveConnection*dc) {
	_MPIack[15]=dc->msgIn[16];
	_MPIack[8]=dc->ibhSrcConn;
	_MPIack[9]=dc->ibhDstConn;
	_MPIack[10]=dc->MPIAdr;
	_daveWriteIBH(dc->iface,_MPIack,sizeof(_MPIack));
}

/*
send a network level ackknowledge
*/
void DECL2 _daveSendIBHNetAck(daveConnection * dc) {
	IBHpacket * p;
	uc ack[13];
	memcpy(ack, dc->msgIn, sizeof(ack));
	p= (IBHpacket*) ack;
	p->len=sizeof(ack)-sizeof(IBHpacket);
	ack[11]=1;
	ack[12]=9;
	//    LOG2("Sending net level ack for number: %d\n",p->packetNumber);
	_daveWriteIBH(dc->iface, ack,sizeof(ack));
}

uc _MPIconnectResponse[]={ 
	0xff,0x07,0x13,0x00,0x00,0x00,0xc2,0x02, 0x14,0x14,0x03,0x00,0x00,0x22,0x0c,0xd0,
	0x04,0x00,0x80,0x00,0x02,0x00,0x02,0x01, 0x00,0x01,0x00,
};

/*
packet analysis. mixes all levels.
*/
int DECL2 __daveAnalyze(daveConnection * dc) {
	int haveResp;
	IBHpacket * p, *p2;
	MPIheader * pm;
	MPIheader2 * m2;
	PDU p1;
#ifdef passiveMode    
	PDU pr;
#endif    
	uc resp[2000];

	//    if (dc->AnswLen==0) return _davePtEmpty;
	haveResp=0;

	p= (IBHpacket*) dc->msgIn;
	dc->needAckNumber=-1;		// Assume no ack
	if (daveDebug & daveDebugPacket){
		LOG2("Channel: %d\n",p->ch1);
		LOG2("Channel: %d\n",p->ch2);
		LOG2("Length:  %d\n",p->len);
		LOG2("Number:  %d\n",p->packetNumber);
		LOG3("sFlags:  %04x rFlags:%04x\n",p->sFlags,p->rFlags);
	}    
	if (p->rFlags==0x82) {
		pm= (MPIheader*) (dc->msgIn+sizeof(IBHpacket));
		if (daveDebug & daveDebugMPI){    
			LOG2("srcconn: %d\n",pm->src_conn);
			LOG2("dstconn: %d\n",pm->dst_conn);
			LOG2("MPI:     %d\n",pm->MPI);
			LOG2("MPI len: %d\n",pm->len);
			LOG2("MPI func:%d\n",pm->func);
		}    
		if (pm->func==0xf1) {
			if (daveDebug & daveDebugMPI)    
				LOG2("MPI packet number: %d needs ackknowledge\n",pm->packetNumber);
			dc->needAckNumber=pm->packetNumber;
			_daveSetupReceivedPDU(dc, &p1);
#ifdef passiveMode
			// construct response:	    
			pr.header=resp+sizeof(IBHpacket)+sizeof(MPIheader2);
#endif	    
			p2= (IBHpacket*) resp;
			p2->ch1=p->ch2;
			p2->ch2=p->ch1;
			p2->packetNumber=0;
			p2->sFlags=0;
			p2->rFlags=0x2c2;

			m2= (MPIheader2*) (resp+sizeof(IBHpacket));
			m2->src_conn=pm->src_conn;
			m2->dst_conn=pm->dst_conn;
			m2->MPI=pm->MPI;
			m2->xxx1=0;
			m2->xxx2=0;
			m2->xx22=0x22;
			if (p1.param[0]==daveFuncRead) {
#ifdef passiveMode	    
				_daveHandleRead(&p1,&pr);
				haveResp=1;
				m2->len=pr.hlen+pr.plen+pr.dlen+2; 
#endif
				p2->len=m2->len+7;
			} else if (p1.param[0]==daveFuncWrite) {
#ifdef passiveMode
				_daveHandleWrite(&p1,&pr);
				haveResp=1;
				m2->len=pr.hlen+pr.plen+pr.dlen+2;
#endif		
				p2->len=m2->len+7;
			} else {
				if (daveDebug & daveDebugPDU)    
					LOG2("Unsupported PDU function code: %d\n",p1.param[0]);
				return _davePtUnknownPDUFunc;
			}

		}
		if (pm->func==0xb0) {
			LOG2("Ackknowledge for packet number: %d\n",*(dc->msgIn+15));
			return _davePtMPIAck;
		}
		if (pm->func==0xe0) {
			LOG2("Connect to MPI: %d\n",pm->MPI);
			memcpy(resp, _MPIconnectResponse, sizeof(_MPIconnectResponse));
			resp[8]=pm->src_conn;
			resp[9]=pm->src_conn;
			resp[10]=pm->MPI;
			haveResp=1;
		}    
	}	

	if (p->rFlags==0x2c2) {
		MPIheader2 * pm= (MPIheader2*) (dc->msgIn+sizeof(IBHpacket));
		if (daveDebug & daveDebugMPI) {
			LOG2("srcconn: %d\n",pm->src_conn);
			LOG2("dstconn: %d\n",pm->dst_conn);
			LOG2("MPI:     %d\n",pm->MPI);
			LOG2("MPI len: %d\n",pm->len);
			LOG2("MPI func:%d\n",pm->func);
		}
		if (pm->func==0xf1) {
			if (daveDebug & daveDebugMPI)    
				LOG1("analyze 1\n");
			dc->needAckNumber=pm->packetNumber;
			dc->PDUstartI= sizeof(IBHpacket)+sizeof(MPIheader2);
			_daveSendMPIAck_IBH(dc);

			return 55;

			/*	
			if (daveDebug & daveDebugMPI)    
			LOG2("MPI packet number: %d\n",pm->packetNumber);
			dc->needAckNumber=pm->packetNumber;
			//	    p1.header=((uc*)pm)+sizeof(MPIheader2);
			dc->PDUstartI= sizeof(IBHpacket)+sizeof(MPIheader2);
			_daveSetupReceivedPDU(dc, &p1);

			if (p1.param[0]==daveFuncRead) {
			LOG1("read Response\n");
			_daveSendMPIAck_IBH(dc);
			dc->resultPointer=p1.data+4;
			dc->_resultPointer=p1.data+4;
			dc->AnswLen=p1.dlen-4;
			return _davePtReadResponse;
			} else if (p1.param[0]==daveFuncWrite) {
			_daveSendMPIAck_IBH(dc);
			LOG1("write Response\n");
			return _davePtWriteResponse;
			} else {
			LOG2("Unsupported PDU function code: %d\n",p1.param[0]);
			}
			*/	    
		}

		if (pm->func==0xb0) {
			if (daveDebug & daveDebugMPI)    
				LOG2("Ackknowledge for packet number: %d\n",pm->packetNumber);
		} else {
			LOG2("Unsupported MPI function code !!: %d\n",pm->func);
			_daveSendMPIAck_IBH(dc);
		}
	}
	/*
	Sending IBHNetAck also for packets with sFlags=082 nearly doubles the speed for LINUX and
	speeds up windows version to the level of LINUX.
	Thanks to Axel Kinting for this proposal and for his patience finding it out!
	*/    
	if (((p->sFlags==0x82)||(p->sFlags==0x82))&&(p->packetNumber)&&(p->len)) _daveSendIBHNetAck(dc);
	if (haveResp) {
		_daveWriteIBH(dc->iface, resp, resp[2]+8);
		_daveDump("I send response:", resp, resp[2]+8);
	}	
	return 0;
};

int DECL2 _daveExchangeIBH(daveConnection * dc, PDU * p) {

	_daveSendMessageMPI_IBH(dc, p);
	dc->AnswLen=0;
	return _daveGetResponseMPI_IBH(dc);
}

int DECL2 _daveSendMessageMPI_IBH(daveConnection * dc, PDU * p) {
	int res;
	_davePackPDU(dc, p);
	res=_daveWriteIBH(dc->iface, dc->msgOut, dc->msgOut[2]+8);
	if (daveDebug & daveDebugPDU)    
		_daveDump("I send request: ",dc->msgOut, dc->msgOut[2]+8);
	return res;	
}	

int DECL2 _daveGetResponseMPI_IBH(daveConnection * dc) {
	int res,count,pt;
	count=0;
	pt=0;
	do {
		res=_daveReadIBHPacket(dc->iface, dc->msgIn);
		count++;
		if(res>4)
			pt=__daveAnalyze(dc);
		if (daveDebug & daveDebugExchange)    
			LOG2("ExchangeIBH packet type:%d\n",pt);
	} while ((pt!=55)&&(count<5));
	if(pt!=55) return daveResTimeout;
	return 0;
}


/*
This performs initialization steps with sampled byte sequences. If chal is <>NULL
it will send this byte sequence.
It will then wait for a packet and compare it to the sample.
*/
int DECL2 _daveInitStepIBH(daveInterface * iface, uc * chal, int cl, us* resp,int rl, uc*b) {
	int res, res2, a=0;
	if (daveDebug & daveDebugConnect) 
		LOG1("_daveInitStepIBH before write.\n");
	if (chal) res=_daveWriteIBH(iface, chal, cl);else res=daveResInvalidParam;
	if (daveDebug & daveDebugConnect) 
		LOG2("_daveInitStepIBH write returned %d.\n",res);
	if (res<0) return 100;
	res=_daveReadIBHPacket(iface, b);
	/*
	We may get a network layer ackknowledge and an MPI layer ackknowledge, which we discard.
	So, normally at least the 3rd packet should have the desired response. 
	Waiting for more does:
	-discard extra packets resulting from last step.
	-extend effective timeout.
	*/    
	while (a<5) {
		if (a) {
			res=_daveReadIBHPacket(iface, b);
			//	    _daveDump("got:",b,res);
		}
		if (res>0) {
			res2=_daveMemcmp(resp,b,rl/2);
			if (0==res2) {
				if (daveDebug & daveDebugInitAdapter) 
					LOG3("*** Got response %d %d\n",res,rl);
				return a;
			}  else {	
				if (daveDebug & daveDebugInitAdapter)  
					LOG2("wrong! %d\n",res2);
			}
		}
		else
			return a;
		a++;
	}
	return a;
}

/*
uc chal0[]={
0x00,0xff,0x03,0x00,0x00,0x00,0x04,0x00, 0x03,0x07,0x02,
};
*/
/*
us resp0[]={
0xff,0x00,0xfe,0x00, 0x04,0x00,0x00,0x00, 
0x00,0x07,0x02,0x03,0x1f,
0x100,		// MPI address of NetLink
0x02,0x00,
0x103,		// 187,5, MPI = 3
// 19,5, PB = 1
0x00,0x00,0x00,0x102,
0x00,0x02,0x3e, 
0x19f,		// 187,5, MPI = 9F
// 19,5, PB = 64
0x101,		// 187,5, MPI = 1
// 19,5, PB = 0

0x101,		// 187,5, MPI = 1
// 19,5, PB = 0
0x00,
0x102,		// 187,5, MPI = 1	
// 19,5, PB = 2
0x00,
0x116,		// 187,5, MPI = 3c	
// 19,5, PB = 16
0x00,
0x13c,		// 187,5, MPI = 90
// 19,5, PB = 3c

0x101,			
0x110,
0x127,0x100,0x100,0x114,0x101, 0x100,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x0e,
'H', 'i','l','s','c','h','e','r',' ','G','m','b','H',' ',

0x200, / * do not compare the rest * /
};
*/
/*
uc chal2[]={
0x00,0x10,0x01,0x00,0x00,0x00,0x01,0x00, 0x0f,
};
*/
/*
us resp2[]={
0x10,0x00,
0x20,
0x00,
0x01,0x00,0x00,0x00, 
0x01,0x106,0x120,0x103,0x17,0x00,0x43,0x00,
0x00,0x00,
0x122,0x121,
0x00,0x00,0x00,0x00, 0x49,0x42,0x48,0x53,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 
};
*/

/* This is the correct response. I just inserted a "don't care" to make it work with latest 
IBH simulator. Better fix the simulator!
us resp7[]={
0xff,0x07,0x13,0x00,0x00,0x00,0xc2,0x02, 0x115,0x114,0x102,0x100,0x00,0x22,0x0c,0xd0,
0x04,0x00,0x80,0x00,0x02,0x00,0x02,0x01, 0x00,0x01,0x00,
};
*/

/*
Connect to a PLC via IBH-NetLink. Returns 0 for success and a negative number on errors.
*/
int DECL2 _daveConnectPLC_IBH(daveConnection*dc) {
	int a, retries;
	PDU p1;
	uc b[daveMaxRawLen];

	uc chal3[]={
	0x07,0xff,0x06,0x01,0x00,0x00,0x97,0x00,
	0x15, //ibhSrcConn
	0xff,0xf0,0xf0,0xf0,0xf0,};

	//us resp3[]={
	//0xff,0x107,0x02,0x01,0x97,0x00,0x00,0x00, 0x114,0x100,};
	us resp3[]={
	0xff,0x07,0x02,0x01,0x97,0x00,0x00,0x00,0x14,0x00,};

	uc chal8[]={
	0x07,0xff,0x11,0x02,0x00,0x00,0x82,0x00, 
	0x14, //ibhSrcConn
	0x00, 
	0x02, //MPI
	0x01,
	0x0c, //19 when routing to ip, 16 when routing to mpi, 0c without routing (count of following bytes??)
	0xe0,
	0x04,
	0x00,  
	0x80,
	0x00,
	0x02,
	0x00,
	0x02,
	dc->ConnectionType, //Connection Type (1=PG) was 0x01
	(dc->slot + dc->rack * 32),	  // Hopefully Rack/Slot is at this position! Need to test! 0x00,
    dc->ConnectionType, //Connection Type (1=PG) repeaded (only when not routing??, why? is 6 when routing)
	0x00,};

	uc chal8R[]={   //Routing
	0x07,
	0xff,
	0x1b,
	0x02,
	0x00,
	0x00,
	0x82,
	0x00, 
	0x14, //ibhSrcConn
	0x00, 
	0x02, //MPI
	0x01,
	0x16, //19 when routing to ip, 16 when routing to mpi
	0xe0,
	0x04,
	0x00,  
	0x80,
	0x00,
	0x02,
	0x01,
	0x0c,
	dc->ConnectionType, //Verb art (1=PG)
	0x00,
	0x06, //Connection Type (1=PG) repeaded (only when not routing??, why? is 6 when routing)
	0x01,
	0x02,
	0xaa, //subnet 1
	0xaa, //subnet 2
	0x00,
	0x00,
	0xbb, //subnet 3
	0xbb, //subnet 4
	0x07, //destination mpi
	dc->routingConnectionType, //0x02 //Verb art zu routing cpu
	0x00, //rack/slot
	};

	uc chal8RIP[]={   //Routing IP
	0x07,
	0xff,
	0x1e,
	0x06,
	0x00,
	0x00,
	0x82,
	0x00, 
	0x14, //ibhSrcConn
	0x00, 
	0x02, //MPI
	0x01,
	0x19,  //19 when routing to ip, 16 when routing to mpi
	0xe0,
	0x04,
	0x00,  
	0x80,
	0x00,
	0x02,
	0x01,
	0x0f,
	dc->ConnectionType, //Verb art (1=PG)
	0x00,
	0x06, //Connection Type (1=PG) repeaded (only when not routing??, why? is 6 when routing)
	0x04,
	0x02,
	0xaa, //subnet 1
	0xaa, //subnet 2
	0x00,
	0x00,
	0xbb, //subnet 3
	0xbb, //subnet 4
	0x07, //ip1
	0x07, //ip2
	0x07, //ip3
	0x07, //ip4
	dc->routingConnectionType, //Verb art zu routing cpu
	0x00, //rack/slot
	};

	us resp7[]={
	0xff,0x07,0x13,0x00,0x00,0x00,0xc2,0x02, 0x115,0x114,0x102,0x100,0x00,0x22,0x0c,0xd0,
	0x04,0x00,0x80,0x00,0x02,0x00,0x102,0x01, 0x00,0x01,0x00,};

	uc chal011[]={
	0x07,0xff,0x07,0x03,0x00,0x00,0x82,0x00,
	0x15, //ibhSrcConn
	0x14, //ibhDstConn
	0x02, //MPI
	0x00,0x02,0x05,0x01,};

	us resp09[]={
	0xff,0x07,0x09,0x00,0x00,0x00,0xc2,0x02, 0x115,0x114,0x102,0x100,0x00,0x22,0x02,0x05,
	0x101,}; //A.K.

	dc->iface->timeout=500000;
	dc->iface->localMPI=0;
	dc->ibhSrcConn=20-1;
	dc->ibhDstConn=20-1;
	retries=0;
	do {
		if (daveDebug & daveDebugConnect) 
			LOG1("trying next ID:\n");
		dc->ibhSrcConn++;
		chal3[8]=dc->ibhSrcConn;
		a=_daveInitStepIBH(dc->iface, chal3,sizeof(chal3),resp3,sizeof(resp3),b);
		retries++;
	} while ((b[9]!=0) && (retries<10));

	if (daveDebug & daveDebugConnect) 
		LOG2("_daveInitStepIBH 4:%d\n",a); if (a>3) /* !!! */ return -4;;
	chal8[10]=dc->MPIAdr;	
	chal8R[10]=dc->MPIAdr;	
	chal8RIP[10]=dc->MPIAdr;	

	/* Not yet tested:
	chal8[10]=dc->CPUConnectiontype;	
	chal8R[10]=dc->CPUConnectiontype;	
	chal8RIP[10]=dc->CPUConnectiontype;		
	*/

	//    LOG2("setting MPI %d\n",dc->MPIAdr);
	chal8[8]=dc->ibhSrcConn;
	chal8R[8]=dc->ibhSrcConn;
	chal8RIP[8]=dc->ibhSrcConn;

	if (!dc->routing)
		a=_daveInitStepIBH(dc->iface, chal8,sizeof(chal8),resp7,sizeof(resp7),b);
	else
	{
		if (!dc->routingDestinationIsIP)
		{				
			chal8R[26]=(unsigned char) (dc->routingSubnetFirst >> 8);
			chal8R[27]=(unsigned char) (dc->routingSubnetFirst);
			chal8R[30]=(unsigned char) (dc->routingSubnetSecond >> 8);
			chal8R[31]=(unsigned char) (dc->routingSubnetSecond);
			chal8R[32]=(unsigned char) dc->_routingDestination1;
			chal8R[34]=(dc->routingSlot + dc->routingRack*32); //--> Routing Rack/Slot 

			a=_daveInitStepIBH(dc->iface, chal8R,sizeof(chal8R),resp7,sizeof(resp7),b);
		}
		else
		{
			chal8RIP[26]=(unsigned char) (dc->routingSubnetFirst >> 8);
			chal8RIP[27]=(unsigned char) (dc->routingSubnetFirst);
			chal8RIP[30]=(unsigned char) (dc->routingSubnetSecond >> 8);
			chal8RIP[31]=(unsigned char) (dc->routingSubnetSecond);
			chal8RIP[32]=(unsigned char) dc->_routingDestination1;
			chal8RIP[33]=(unsigned char) dc->_routingDestination2;
			chal8RIP[34]=(unsigned char) dc->_routingDestination3;
			chal8RIP[35]=(unsigned char) dc->_routingDestination4;
			chal8RIP[37]=(dc->routingSlot + dc->routingRack*32); //--> Routing Rack/Slot 

			a=_daveInitStepIBH(dc->iface, chal8RIP,sizeof(chal8RIP),resp7,sizeof(resp7),b);
		}
	}
	dc->ibhDstConn=b[9];
	if (daveDebug & daveDebugConnect) 
		LOG3("_daveInitStepIBH 5:%d connID: %d\n",a, dc->ibhDstConn); if (a>3) return -5;

	chal011[8]=dc->ibhSrcConn;
	chal011[9]=dc->ibhDstConn;
	chal011[10]=dc->MPIAdr;	//??????
	a=_daveInitStepIBH(dc->iface, chal011,sizeof(chal011),resp09,sizeof(resp09),b);

	dc->ibhDstConn=b[9];
	if (daveDebug & daveDebugConnect) 
		LOG3("_daveInitStepIBH 5a:%d connID: %d\n",a, dc->ibhDstConn); if (a>3) return -5;

	dc->packetNumber=4;
	return _daveNegPDUlengthRequest(dc, &p1);
}    

/*
Disconnect from a PLC via IBH-NetLink. 
Returns 0 for success and a negative number on errors.
*/
int DECL2 _daveDisconnectPLC_IBH(daveConnection*dc) {
	uc b[daveMaxRawLen];

	uc chal31[]={
	0x07,0xff,0x06,0x08,0x00,0x00,0x82,0x00, 0x14,0x14,0x02,0x00,0x01,0x80,};

	chal31[8]=dc->ibhSrcConn;
	chal31[9]=dc->ibhDstConn;
	chal31[10]=dc->MPIAdr;
	_daveWriteIBH(dc->iface, chal31, sizeof(chal31));
	_daveReadIBHPacket(dc->iface, b);
#ifdef BCCWIN    
#else    
	_daveReadIBHPacket(dc->iface, b);
#endif    
	return 0;
}

/*
Disconnect from a PLC via IBH-NetLink. This can be used to free other than your own 
connections. Be careful, this may disturb third party programs/devices.
*/
int DECL2 daveForceDisconnectIBH(daveInterface * di, int src, int dest, int mpi) {
	uc b[daveMaxRawLen];

	uc chal31[]={
	0x07,0xff,0x06,0x08,0x00,0x00,0x82,0x00, 0x14,0x14,0x02,0x00,0x01,0x80,};

	chal31[8]=src;
	chal31[9]=dest;
	chal31[10]=mpi;
	_daveWriteIBH(di, chal31, sizeof(chal31));
	_daveReadIBHPacket(di, b);
#ifdef BCCWIN    
#else    
	_daveReadIBHPacket(di, b);
#endif    
	return 0;
}   

/*
Resets the IBH-NetLink.
Returns 0 for success and a negative number on errors.
*/
int DECL2 daveResetIBH(daveInterface * di) {
	uc chalReset[]={
		0x00,0xff,0x01,0x00,0x00,0x00,0x01,0x00,0x01
	};
	uc b[daveMaxRawLen];
	_daveWriteIBH(di, chalReset, sizeof(chalReset));
	_daveReadIBHPacket(di, b);
#ifdef BCCWIN
#else
	_daveReadIBHPacket(di, b);
#endif   
	return 0;
}

void DECL2 _daveSendMPIAck2(daveConnection *dc) {
	IBHpacket * p;
	uc c;
	uc ack[18];
	memcpy(ack,dc->msgIn,sizeof(ack));
	p= (IBHpacket*) ack;
	p->rFlags|=0x240;	//Why that?
	c=p->ch1; p->ch1=p->ch2; p->ch2=c;
	p->len=sizeof(ack)-sizeof(IBHpacket);
	p->packetNumber=0;	// this may mean: no net work level acknowledge
	ack[13]=0x22;
	ack[14]=3;
	ack[15]=176;
	ack[16]=1;
	ack[17]=dc->needAckNumber;
	_daveDump("send MPI-Ack2",ack,sizeof(ack));
	_daveWriteIBH(dc->iface,ack,sizeof(ack));
};

int DECL2 _davePackPDU_PPI(daveConnection * dc,PDU *p) {
	IBHpacket * ibhp;
	uc IBHPPIHeader[]={0xff,0xff,2,0,8};
	IBHPPIHeader[2]=dc->MPIAdr;
	memcpy(dc->msgOut+sizeof(IBHpacket), &IBHPPIHeader, sizeof(IBHPPIHeader));

	ibhp = (IBHpacket*) dc->msgOut;
	ibhp->ch1=7;
	ibhp->ch2=0xff;
	ibhp->len=p->hlen+p->plen+p->dlen+5;		// set MPI length
	//    dc->msgOut[3]=2;
	*(dc->msgOut+sizeof(IBHpacket)+4)=p->hlen+p->plen+p->dlen+0;
	ibhp->packetNumber=dc->packetNumber;
	ibhp->rFlags=0x80;
	ibhp->sFlags=0;
	//    dc->msgOut[3]=dc->packetNumber;
	dc->packetNumber++;
	dc->msgOut[6]=0x82;
	dc->msgOut[3]=2;
	return 0;
}    

/*
send a network level ackknowledge
*/
void DECL2 _daveSendIBHNetAckPPI(daveConnection * dc) {
	uc ack[]={7,0xff,5,3,0,0,0x82,0,0xff,0xff,2,0,0};
	ack[10]=dc->MPIAdr;
	ack[3]=_daveIncMessageNumber(dc);
	_daveWriteIBH(dc->iface, ack, sizeof(ack));
}

int DECL2 _daveListReachablePartnersMPI_IBH(daveInterface * di, char * buf) {
	int a, i;
	uc b[2*daveMaxRawLen];

	uc chal1[]={ 
	0x07,0xff,0x08,0x01,0x00,0x00,0x96,0x00, 
	0x00,0x00,0x00,0x00,0x00,0x00,0x0a,0x01,};

	us resp1[]={ 
	0xff,0x07,0x87,0x01,0x96,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x7f,0x0a,0x02,};

	a=_daveInitStepIBH(di, chal1,sizeof(chal1),resp1,16,b);
	if (daveDebug & daveDebugListReachables) 
		LOG2("_daveListReachablePartnersMPI_IBH:%d\n",a); 
	for (i=0;i<126;i++) {
		if (b[i+16]==0xFF) buf[i]=0x10; else buf[i]=0x30;
		//	LOG3(" %d %d\n",i, b[i+16]); 
	}	
	return 126;
}    

/*
packet analysis. mixes all levels.
*/
int DECL2 __daveAnalyzePPI(daveConnection * dc, uc sa) {
	IBHpacket* p;

	p= (IBHpacket*) dc->msgIn;
	if (daveDebug & daveDebugPacket){
		LOG2("Channel: %d\n",p->ch1);
		LOG2("Channel: %d\n",p->ch2);
		LOG2("Length:  %d\n",p->len);
		LOG2("Number:  %d\n",p->packetNumber);
		LOG3("sFlags:  %04x rFlags:%04x\n",p->sFlags,p->rFlags);
	}    
	if (p->sFlags==0x82) {
		if(p->len<=5) {
			if(sa) _daveSendIBHNetAckPPI(dc);
		} else    
			if ((p->len>=7) && (dc->msgIn[14]==0x32))
				return 55;
	}	
	return 0;
};

int DECL2 _daveExchangePPI_IBH(daveConnection * dc, PDU * p) {
	int res, count, pt;
	_davePackPDU_PPI(dc, p);

	res=_daveWriteIBH(dc->iface, dc->msgOut, dc->msgOut[2]+8);
	if (daveDebug & daveDebugExchange)    
		_daveDump("I send request: ",dc->msgOut, dc->msgOut[2]+8);
	//    _daveSendIBHNetAckPPI(dc);	
	count=0;
	do {
		res=_daveReadIBHPacket(dc->iface, dc->msgIn);
		count++;
		if (res>0)
			pt=__daveAnalyzePPI(dc,1);
		else 
			pt=0;    
		if (daveDebug & daveDebugExchange)    
			LOG2("ExchangeIBH packet type:%d\n",pt);
	} while ((pt!=55)&&(count<7));
	if(pt!=55) return daveResTimeout;
	return 0;
}

int DECL2 _daveGetResponsePPI_IBH(daveConnection * dc) {
	int res, count, pt;
	count=0;
	do {
		_daveSendIBHNetAckPPI(dc);
		res=_daveReadIBHPacket(dc->iface, dc->msgIn);
		LOG2("_daveReadIBHPacket():%d\n",res);
		count++;
		if (res>0)
			pt=__daveAnalyzePPI(dc,0);
		else 
			pt=0;    
		if (daveDebug & daveDebugExchange)    
			LOG2("ExchangeIBH packet type:%d\n",pt);
	} while ((pt!=55)&&(count<7));
	if(pt!=55) return daveResTimeout;
	return 0;
}
#endif
/*
Build a PDU with data from 2 data blocks.
*/
int DECL2 BuildAndSendPDU(daveConnection * dc, PDU*p2,uc *pa,int psize, uc *ud,int usize,
	uc *ud2,int usize2) {
		int res;
		PDU p,*p3;
		uc * dn;
		p.header=dc->msgOut+dc->PDUstartO;
		_daveInitPDUheader(&p, 7);
		_daveAddParam(&p, pa, psize);
		_daveAddUserData(&p, ud, usize);
		//    LOG2("*** here we are: %d\n",p.dlen);
		p3=&p;
		dn= p3->data+p3->dlen;
		p3->dlen+=usize2;
		memcpy(dn, ud2, usize2);

		((PDUHeader*)p3->header)->dlen=daveSwapIed_16(p3->dlen);

		LOG2("*** here we are: %d\n",p.dlen);
		if (daveDebug & daveDebugPDU) {
			_daveDumpPDU(&p);
		}
		res=_daveExchange(dc, &p);
		if (daveDebug & daveDebugErrorReporting)
			LOG2("*** res of _daveExchange(): %d\n",res);
		if (res!=daveResOK) return res;
		res=_daveSetupReceivedPDU(dc,p2);
		if (daveDebug & daveDebugPDU) {
			_daveDumpPDU(p2);
		}
		if (daveDebug & daveDebugErrorReporting)
			LOG2("*** res of _daveSetupReceivedPDU(): %d\n",res);
		if (res!=daveResOK) return res;
		res=_daveTestPGReadResult(p2);
		if (daveDebug & daveDebugErrorReporting)
			LOG2("*** res of _daveTestPGReadResult(): %d\n",res);
		return res;
}    	

int DECL2 daveForce200(daveConnection * dc,int area, int start, int val) {
	int res;
	PDU p2;
	//    uc pa[]={0,1,18,4,17,67,2,0};
	//    uc da[]={'0','0'};

	//32,7,0,0,0,0,0,c,0,16,

	uc pa[]={0,1,18,8,18,72,14,0,0,0,0,0};
	uc da[]={0,1,0x10,2,
		0,1,
		0,0,
		0,		// area
		0,0,0,		// start
	};
	uc da2[]={0,4,0,8,0,0,};
	//    uc da2[]={0,4,0,8,7,0,};

	if ((area==daveAnaIn) || (area==daveAnaOut) /*|| (area==daveP)*/) {
		da[3]=4;
		start*=8;			/* bits */
	} else if ((area==daveTimer) || (area==daveCounter)||(area==daveTimer200) || (area==daveCounter200)) {
		da[3]=area;
	} else {
		start*=8;
	}
	/*    else {
	if(isBit) {
	pa[3]=1;
	} else {
	start*=8;			
	}    
	}
	*/    
	da[8]=area; 
	da[9]=start / 0x10000; 
	da[10]=(start / 0x100) & 0xff;
	da[11]=start & 0xff;


	da2[4]=val % 0x100;
	da2[5]=val / 0x100;
	res=BuildAndSendPDU(dc, &p2, pa, sizeof(pa), da, sizeof(da), da2, sizeof(da2));
	return res;
}

daveResultSet * DECL2 daveNewResultSet() {
	daveResultSet * p=(daveResultSet*) calloc(1, sizeof(daveResultSet));
#ifdef DEBUG_CALLS
	LOG2("daveNewResultSet() = %p\n",p);
	FLUSH;
#endif	        
	return p;
}    

void DECL2 daveFree(void * dc) {
	//    if (dc!=NULL) {	// I'm not sure whether freeing a NULL pointer will do no harm on each and 
	// every system. So for safety, we check and set it to NULL afterwards.
	free(dc);
	//    }	
}

int DECL2 daveGetProgramBlock(daveConnection * dc, int blockType, int number, char* buffer, int * length) {
	int res, uploadID, len, more, totlen;
	uc *bb=(uc*)buffer;	//cs: is this right?
	len=0;
	totlen=0;
	if (dc->iface->protocol==daveProtoAS511) {	    
		return daveGetS5ProgramBlock(dc, blockType, number, buffer, length);
	}

	res=initUpload(dc, blockType, number, &uploadID); 
	if (res!=0) return res;
	do {
		res=doUpload(dc,&more,&bb,&len,uploadID);
		totlen+=len;
		if (res!=0) return res;
	} while (more);
	res=endUpload(dc,uploadID);
	*length=totlen;
	return res;
}

int DECL2 davePutProgramBlock(daveConnection * dc, int blockType, int blknumber, char* buffer, int * length) {
#define maxPBlockLen 0xDe	// real maximum 222 bytes

	int res=0;
	int cnt=0;
	int size=0;
	int blockNumber,rawLen,netLen,blockCont;
	int number=0;

	uc pup[]= {			// Load request
		0x1A,0,1,0,0,0,0,0,9,
		0x5F,0x30,0x42,0x30,0x30,0x30,0x30,0x34,0x50, // block type code and number
		//     _    0    B   0     0    0    0    4    P
		//		SDB		
		0x0D,
		0x31,0x30,0x30,0x30,0x32,0x30,0x38,0x30,0x30,0x30,0x31,0x31,0x30,0	// file length and netto length
		//     1   0     0    0    2    0    8    0    0    0    1    1    0
	};

	PDU p,p2;

	uc pablock[]= {	// parameters for parts of a block
		0x1B,0
	};
	
	uc progBlock[maxPBlockLen + 4]= {
		0,maxPBlockLen,0,0xFB,	// This seems to be a fix prefix for program blocks
	};

	pup[11] = blockType;
	paInsert[13] = blockType;
	/*pup[12] = number / (10*10*10*10);
	pup[13] = (number - (pup[12] * 10*10*10*10 )) / (10*10*10);
	pup[14] = (number - (pup[13] * 10*10*10)) / (10*10);
	pup[15] = (number - (pup[14] * 10*10)) / (10);
	pup[16] = (number - (pup[15] * 10));
	
	pup[12] = pup[12] + 0x30;
	pup[13] = pup[13] + 0x30;
	pup[14] = pup[14] + 0x30;
	pup[15] = pup[15] + 0x30;
	pup[16] = pup[16] + 0x30;*/
	
	memcpy(progBlock+4,buffer,maxPBlockLen);

	progBlock[9] = (blockType + 0x0A - 'A'); //Convert 'A' to 0x0A
	if (blockType == '8') progBlock[9] = 0x08;
	
	progBlock[10] = blknumber / 0x100;
	progBlock[11] = blknumber - (progBlock[10] * 0x100);
		
		
	rawLen=daveGetU16from(progBlock+14);
	netLen=daveGetU16from(progBlock+38);

	sprintf((char*)pup+19,"1%06d%06d",rawLen,netLen);

	sprintf((char*)pup+12,"%05d",blknumber);
	sprintf((char*)paInsert+14,"%05d",blknumber);
	
	pup[17]='P';
	paInsert[19]='P';
	
	p.header=dc->msgOut+dc->PDUstartO;
	_daveInitPDUheader(&p, 1);
	_daveAddParam(&p, pup, sizeof(pup)-1);

	res=_daveExchange(dc, &p);
	if (res==daveResOK) {
		res=_daveSetupReceivedPDU(dc, &p2);
		if (daveGetDebug() & daveDebugPDU) {
			_daveDumpPDU(&p2);
		}
		res=daveGetPDUerror(&p2);

		if (res==0) {
			blockCont=1;
			res=daveGetResponse(dc);
			res=_daveSetupReceivedPDU(dc, &p2);

			cnt = 0;

			do {
				res=0;
				res=_daveSetupReceivedPDU(dc, &p2);

				number=((PDUHeader*)p2.header)->number;
				if (p2.param[0]==0x1B) {
					//READFILE
					memcpy(progBlock+4,buffer+(cnt*maxPBlockLen),maxPBlockLen);
					
					if (cnt == 0)
					{
						progBlock[9] = (blockType + 0x0A - 'A'); //Convert 'A' to 0x0A
						if (blockType == '8') progBlock[9] = 0x08;
	
						progBlock[10] = blknumber / 0x100;
						progBlock[11] = blknumber - (progBlock[10] * 0x100);						
					}

					p.header=dc->msgOut+dc->PDUstartO;
					_daveInitPDUheader(&p, 3);
					size = maxPBlockLen;

					if (*length > ((cnt+1) * maxPBlockLen))  
						pablock[1]=1;
					else
					{
						size = *length - (cnt * maxPBlockLen);
						pablock[1]=0;	//last block
						blockCont=0;
					}
					   
					progBlock[1]=size;
					_daveAddParam(&p, pablock, sizeof(pablock));
					_daveAddData(&p, progBlock, size + 4 /* size of block) */);
					((PDUHeader*)p.header)->number=number;
					if (daveGetDebug() & daveDebugPDU) {
						_daveDumpPDU(&p);
					}
					_daveExchange(dc,&p);
				}
				cnt++;
			} while (blockCont);  

			res=_daveSetupReceivedPDU(dc, &p2);
			if (daveGetDebug() & daveDebugPDU) {
				_daveDumpPDU(&p2);
			}
			number=((PDUHeader*)p2.header)->number;
			if (p2.param[0]==0x1C) {
				p.header=dc->msgOut+dc->PDUstartO;

				_daveInitPDUheader(&p, 3);
				_daveAddParam(&p, p2.param,1);
				((PDUHeader*)p.header)->number=number;
				_daveExchange(dc,&p);

				p.header=dc->msgOut+dc->PDUstartO;
				_daveInitPDUheader(&p, 1);
				_daveAddParam(&p, paInsert, sizeof(paInsert));
				res=_daveExchange(dc, &p);
				res=_daveSetupReceivedPDU(dc, &p2);
				res=daveGetPDUerror(&p2);
			}
		} else {
			printf("CPU doesn't accept load request:%04X\n",res);
		}	
		return res;
	}
	return res;
}

int DECL2 daveDeleteProgramBlock(daveConnection*dc, int blockType, int number) {
	int res;
	PDU p,p2;
	uc paDelete[]= {
	0x28,0,0,0,0,0,0,0xFD,0,
	0x0a,0x01,0x00,
	'0','C', //Block type in ASCII (0C = FC)
	'0','0','0','0','1', //Block Number in ASCII
	'B', //Direction?
	0x05, //Length of Command
	'_','D','E','L','E' //Command Delete	
	};

	paDelete[13] = blockType;
	sprintf((char*)(paDelete+14),"%05d",number);
	paDelete[19] = 'B'; //This is overriden by sprintf via 0x00 as String seperator!

	p.header=dc->msgOut+dc->PDUstartO;
	_daveInitPDUheader(&p, 1);
	_daveAddParam(&p, paDelete, sizeof(paDelete));
	res=_daveExchange(dc, &p);
	if (res==daveResOK) {
		res=_daveSetupReceivedPDU(dc, &p2);
		if (daveDebug & daveDebugPDU) {
			_daveDumpPDU(&p2);
		}
	}

	//Retval of 0x28 in Recieved PDU Parameter Part means delete was sucessfull.
	//This needs to be implemneted and also error Codes Like Block does not exist or block is locked and so on...
	return res;
}

int DECL2 daveReadPLCTime(daveConnection * dc) {
	int res, len;
	PDU p2;
	uc pa[]={0,1,18,4,17,'G',1,0};
#ifdef DEBUG_CALLS
	LOG2("daveGetTime(dc:%p)\n", dc);
	FLUSH;
#endif	    	
	len=0;res=daveBuildAndSendPDU(dc, &p2,pa, sizeof(pa), NULL, 1);
	if (res==daveResOK) {
		dc->resultPointer=p2.udata;
		dc->_resultPointer=p2.udata;
		len=p2.udlen;
	} else {
		if(daveDebug & daveDebugPrintErrors)
			LOG3("daveGetTime: %04X=%s\n",res, daveStrerror(res));
	}
	dc->AnswLen=len;
	return res;
}    

int DECL2 daveSetPLCTime(daveConnection * dc,uc * ts) {
	int res, len;
	PDU p2;
	uc pa[]={0,1,18,4,17,'G',2,0};
#ifdef DEBUG_CALLS
	LOG2("daveSetTime(dc:%p)\n", dc);
	FLUSH;
#endif	    	
	len=0;
	res=daveBuildAndSendPDU(dc, &p2,pa, sizeof(pa), ts, 10);
	if (res==daveResOK) {
		dc->resultPointer=p2.udata;
		dc->_resultPointer=p2.udata;
		len=p2.udlen;
	} else {
		if(daveDebug & daveDebugPrintErrors)
			LOG3("daveGetTime: %04X=%s\n",res, daveStrerror(res));
	}
	dc->AnswLen=len;
	return res;
}    

uc DECL2 daveToBCD(uc i) {
	return 16*(i /10)+(i%10);
}

uc DECL2 daveFromBCD(uc i) {
	return 10*(i /16)+(i%16);
}

int DECL2 daveSetPLCTimeToSystime(daveConnection * dc) {
	int res, len;
	PDU p2;
	uc pa[]={0,1,18,4,17,'G',2,0};
	uc ts[]={
		0x00,0x19,0x05,0x08,0x23,0x04,0x10,0x23,0x67,0x83,
	};	
#ifdef LINUX    
	struct tm systime;
	struct timeval t1;
	gettimeofday(&t1, NULL);
	localtime_r(&(t1.tv_sec),&systime);
	t1.tv_usec/=100;		//tenth of miliseconds from microseconds
	//    ts[1]=daveToBCD(systime.tm_year/100+19);
	ts[2]=daveToBCD(systime.tm_year % 100);
	ts[3]=daveToBCD(systime.tm_mon+1);
	ts[4]=daveToBCD(systime.tm_mday);
	ts[5]=daveToBCD(systime.tm_hour);
	ts[6]=daveToBCD(systime.tm_min);
	ts[7]=daveToBCD(systime.tm_sec);
	ts[8]=daveToBCD(t1.tv_usec/100);
	ts[9]=daveToBCD(t1.tv_usec%100);
	//    _daveDump("timestamp: ",ts,10);
	//    LOG2("tm.sec:  %d\n", systime.tm_sec);
	//    LOG2("tm.min:  %d\n", systime.tm_min);
	//    LOG2("tm.hour: %d\n", systime.tm_hour);
#endif    

#ifdef BCCWIN
	SYSTEMTIME t1;
	//    gettimeofday(&t1, NULL);
	GetLocalTime(&t1);
	//    tm=localtime(&t1);
	//    t1.tv_usec/=100;		//tenth of miliseconds from microseconds
	//    WORD wYear;
	/*
	WORD wMonth;
	WORD wDayOfWeek;
	WORD wDay;
	WORD wHour;
	WORD wMinute;
	WORD wSecond;
	WORD wMilliseconds;
	*/
	ts[2]=daveToBCD(t1.wYear % 100);
	ts[3]=daveToBCD(t1.wMonth);
	ts[4]=daveToBCD(t1.wDay);
	ts[5]=daveToBCD(t1.wHour);
	ts[6]=daveToBCD(t1.wMinute);
	ts[7]=daveToBCD(t1.wSecond);
	ts[8]=daveToBCD(t1.wMilliseconds/10);
	ts[9]=daveToBCD((t1.wMilliseconds%10)*10);
	//    _daveDump("timestamp: ",ts,10);
	//    LOG2("tm.sec:  %d\n", t1.wSecond);
	//    LOG2("tm.min:  %d\n", t1.wMinute);
	//    LOG2("tm.hour: %d\n", t1.wHour);
#endif    

#ifdef DEBUG_CALLS
	LOG2("SetPLCTimeToSystime(dc:%p)\n", dc);
	FLUSH;
#endif	    	
	len=0;
	res=daveBuildAndSendPDU(dc, &p2,pa, sizeof(pa), ts, sizeof(ts));
	if (res==daveResOK) {
		dc->resultPointer=p2.udata;
		dc->_resultPointer=p2.udata;
		len=p2.udlen;
	} else {
		if(daveDebug & daveDebugPrintErrors)
			LOG3("daveGetTime: %04X=%s\n",res, daveStrerror(res));
	}
	dc->AnswLen=len;
	return res;
}    


/***************
Simatic S5:
****************/

uc __davet2[]={STX};
uc __davet10[]={DLE};
char __davet1006[]={DLE,ACK};  // cs: this is only sent by system functions, so let it be char
us __daveT1006[]={DLE,ACK};
uc __davet121003[]={0x12,DLE,ETX};
us __daveT121003[]={0x12,DLE,ETX};
uc __davet161003[]={0x16,DLE,ETX};
us __daveT161003[]={0x16,DLE,ETX};
/*
Reads <count> bytes from area <BlockN> with offset <offset>, 
that can be readed with daveGetInteger etc. You can read bytes from 
PBs & FBs too, but use daveReadBlock for this:
*/
int DECL2 daveReadS5Bytes(daveConnection * dc, uc area, uc BlockN, int offset, int count)
{
	int res,datastart,dataend;
	daveS5AreaInfo ai;
	uc b1[daveMaxRawLen];
	//    if (_daveIsS5BlockArea(area)==0) {
	if (area==daveDB) {
		res=_daveReadS5BlockAddress(dc,area,BlockN,&ai);//TODO make address cache
		if (res<0) {
			LOG2("%s *** Error in ReadS5Bytes.BlockAddr request.\n", dc->iface->name);
			return res-50;
		}
		datastart=ai.address;
	} else {
		switch (area) {
		case daveRawMemoryS5: datastart=0; break;
		case daveInputs: datastart=dc->cache->PAE; break;
		case daveOutputs: datastart=dc->cache->PAA; break;
		case daveFlags: datastart=dc->cache->flags; break;
		case daveTimer: datastart=dc->cache->timers; break;
		case daveCounter: datastart=dc->cache->counters; break;
		case daveSysDataS5: datastart=dc->cache->systemData; break;
		default:
			LOG2("%s *** Unknown area in ReadS5Bytes request.\n", dc->iface->name);
			return -1;
		}	
	}
	//It's difficult to convert Intel-Motorola so I will use arithmetic:
	if ((count>daveMaxRawLen)
		//    ||(offset+count>ai.len)
		) {
			LOG2("%s *** readS5Bytes: Requested data is out-of-range.\n", dc->iface->name);
			return -1;
	}
	datastart+=offset;
	dataend=datastart+count-1;
	b1[0]=datastart/256;
	b1[1]=datastart%256;
	b1[2]=dataend/256;
	b1[3]=dataend%256;
	res=_daveExchangeAS511(dc,b1,4,2*count+7,0x04);
	if (res<0) {
		LOG2("%s *** Error in ReadS5Bytes.Exchange sequence.\n", dc->iface->name);
		return res-10;
	}
	if (dc->AnswLen<count+7) {
		LOG3("%s *** Too few chars (%d) in ReadS5Bytes data.\n", dc->iface->name,dc->AnswLen);
		return -5;
	}
	if ((dc->msgIn[0]!=0)||(dc->msgIn[1]!=0)||(dc->msgIn[2]!=0)||(dc->msgIn[3]!=0)||(dc->msgIn[4]!=0)) {
		LOG2("%s *** Wrong ReadS5Bytes data signature.\n", dc->iface->name);
		return -6;
	}
	dc->resultPointer=dc->msgIn+5;
	dc->_resultPointer=dc->resultPointer;

	//    dc->dlen=dc->AnswLen-7;
	dc->AnswLen-=7;
	return 0;
}

/* 
Write DLE,ACK to the serial interface:
*/
void DECL2 _daveSendDLEACK(daveInterface * di)	// serial interface
{
	di->ifwrite(di, __davet1006, 2);
}

/* 
Sends a sequence of characters after doubling DLEs and adding DLE,EOT.
*/
int DECL2 _daveSendWithDLEDup(daveInterface * di, // serial interface
	uc *b, 		    // a buffer containing the message
	int size		    // the size of the string
	)
{		
	uc target[daveMaxRawLen];	
	int res;
	int targetSize=0,i; //preload 
	if(daveDebug & daveDebugExchange)
		LOG1("SendWithDLEDup: \n");
	if(daveDebug & daveDebugExchange)
		_daveDump("I send",b,size);	
	for (i=0; i<size; i++) {
		target[targetSize]=b[i];targetSize++;
		if (b[i]==DLE) {
			target[targetSize]=DLE;
			targetSize++;
		}
	};
	target[targetSize]=DLE;
	target[targetSize+1]=EOT;
	targetSize+=2;
	if(daveDebug & daveDebugExchange)
		_daveDump("I send", target, targetSize);
	res=di->ifwrite(di, (char*)target, targetSize);
	if(daveDebug & daveDebugExchange)
		LOG2("send: res:%d\n",res);
	return 0;
}

/*
Executes part of the dialog that requests transaction with PLC:
*/
int DECL2 _daveReqTrans(daveConnection * dc, uc trN)
{
	uc b1[3];
	int res;
	if (daveDebug & daveDebugExchange)
		LOG3("%s daveReqTrans %d\n", dc->iface->name, trN);
	_daveSendSingle(dc->iface, STX);
	res=_daveReadChars2(dc->iface, b1, /*dc->iface->timeout,*/ sizeof(__daveT1006)/2);
	if (daveDebug & daveDebugByte)
		_daveDump("2got",b1, res);
	if (_daveMemcmp(__daveT1006, b1, sizeof(__daveT1006)/2)) {
		if (daveDebug & daveDebugPrintErrors)
			LOG3("%s daveReqTrans %d *** no DLE,ACK before send.\n", dc->iface->name, trN);
		return -1;
	}
	_daveSendSingle(dc->iface, trN);
	if (_daveReadSingle(dc->iface)!=STX) {
		if (daveDebug & daveDebugPrintErrors)
			LOG3("%s daveReqTrans %d *** no STX before send.\n", dc->iface->name, trN);
		return -2;
	}
	_daveSendDLEACK(dc->iface);
	_daveReadChars2(dc->iface, b1, /*dc->iface->timeout,*/ sizeof(__daveT161003)/2);
	if (daveDebug & daveDebugByte)
		_daveDump("1got",b1, res);
	if (_daveMemcmp(__daveT161003, b1, sizeof(__daveT161003)/2)) {    
		if (daveDebug & daveDebugPrintErrors)
			LOG3("%s daveReqTrans %d *** no accept0 from plc.\n", dc->iface->name, trN);
		return -3;
	}
	_daveSendDLEACK(dc->iface);
	return 0;
}

/*
Executes part of the dialog required to terminate transaction:
*/
int DECL2 _daveEndTrans(daveConnection * dc)
{
	int res;
	uc b1[3];
	if (daveDebug & daveDebugExchange)
		LOG2("%s daveEndTrans\n", dc->iface->name);
	if (_daveReadSingle(dc->iface)!=STX) {
		LOG2("%s daveEndTrans *** no STX at eot sequense.\n", dc->iface->name);
		//	return -1;
	}
	_daveSendDLEACK(dc->iface);
	res=_daveReadChars2(dc->iface, b1, /*dc->iface->timeout,*/ sizeof(__daveT121003)/2);
	if (daveDebug & daveDebugByte)
		_daveDump("3got",b1, res);
	if (_daveMemcmp(__daveT121003, b1, sizeof(__daveT121003)/2)) {
		LOG2("%s daveEndTrans *** no accept of eot/ETX from plc.\n", dc->iface->name);
		return -2;
	}	
	_daveSendDLEACK(dc->iface);
	return 0;
}

/*
Remove the DLE doubling:
*/
int DECL2 _daveDLEDeDup(daveConnection * dc, uc* rawBuf, int rawLen) {
	int j=0,k;
	for (k=0;k<rawLen-2;k++){
		dc->msgIn[j]=rawBuf[k]; j++;
		if (DLE==rawBuf[k]){
			if (DLE!=rawBuf[k+1]) return -1;//Bad doubling found
			k++;
		}
	}
	dc->msgIn[j]=rawBuf[k];//Copy 2 last chars (DLE,ETX)
	j++;k++;
	dc->msgIn[j]=rawBuf[k];
	dc->AnswLen=j+1;
	return 0;
}

int DECL2 _daveExchangeAS511(daveConnection * dc, uc * b, int len, int maxlen, int trN) {
	int res, i;
	uc b1[3];
	res=_daveReqTrans(dc, trN);
	if (res<0) {
		LOG2("%s *** Error in Exchange.ReqTrans request.\n", dc->iface->name);
		return res-10;
	}
	if (trN==8) {		//Block write functions have advanced syntax
		LOG1("trN 8\n");
		_daveSendWithDLEDup(dc->iface,b,4);
		LOG1("trN 8 done\n");
	} else {
		if (daveDebug & daveDebugExchange)
			LOG3("trN %d len %d\n",trN,len);
		_daveSendWithDLEDup(dc->iface,b,len);
		if (daveDebug & daveDebugExchange)
			LOG2("trN %d done\n",trN);
	}
	//    _daveSendDLEACK(dc->iface);
	//    res=_daveReadChars2(dc->iface, b1, /*dc->iface->timeout,*/ 2000 /*sizeof(__daveT1006)/2*/);
	res=_daveReadChars2(dc->iface, b1, /*dc->iface->timeout,*/ sizeof(__daveT1006)/2);
	if (daveDebug & daveDebugByte)
		_daveDump("4 got:",b1, res);
	if (_daveMemcmp(__daveT1006, b1, sizeof(__daveT1006)/2)) {
		LOG2("%s *** no DLE,ACK in Exchange request.\n", dc->iface->name);
		return -1;
	}
	if ((trN!=3)&&(trN!=7)&&(trN!=9)) {//write bytes, compress & delblk
		if (!_daveReadSingle(dc->iface)==STX) {
			LOG2("%s *** no STX in Exchange request.\n", dc->iface->name);
			return -2;
		}
		//	usleep(500000);
		_daveSendDLEACK(dc->iface);
		res=0;
		do {
			i=_daveReadChars2(dc->iface, dc->msgIn+res, /*100*dc->iface->timeout,*/ daveMaxRawLen-res);
			res+=i;
			if (daveDebug & daveDebugByte)
				_daveDump("5 got:",dc->msgIn, res);
		} while((i>0)&& ( (dc->msgIn[res-2]!=DLE) || (dc->msgIn[res-1]!=ETX)));

		if (daveDebug & daveDebugByte)
			LOG3("%s *** got %d bytes.\n", dc->iface->name,res);
		if (res<0) {
			LOG2("%s *** Error in Exchange.ReadChars request.\n", dc->iface->name);
			return res-20;
		}
		if ((dc->msgIn[res-2]!=DLE)||(dc->msgIn[res-1]!=ETX)) {
			LOG2("%s *** No DLE,ETX in Exchange data.\n", dc->iface->name);
			return -4;
		}
		if (_daveDLEDeDup(dc,dc->msgIn,res)<0) {
			LOG2("%s *** Error in Exchange rawdata.\n", dc->iface->name);
			return -3;
		}

		//	usleep(500000);
		_daveSendDLEACK(dc->iface);
	}
	if (trN==8) { //Write requests have more differences from others ;(
		if (dc->msgIn[0]!=9) {
			LOG2("%s 8 *** No 0x09 in special Exchange request.\n", dc->iface->name);
			return -5;
		}
		_daveSendSingle(dc->iface,STX);
		res=_daveReadChars2(dc->iface, b1, /*dc->iface->timeout,*/ sizeof(__daveT1006)/2);
		_daveDump("got:",b1, res);
		if (_daveMemcmp(__daveT1006, b1, sizeof(__daveT1006)/2)) {	
			LOG2("%s 8 *** no DLE,ACK in special Exchange request.\n", dc->iface->name);
			return -6;
		}
		_daveSendWithDLEDup(dc->iface,b+4,len);

		res=_daveReadChars2(dc->iface, b1, /*dc->iface->timeout,*/ sizeof(__daveT1006)/2);
		_daveDump("got:",b1, res);
		if (_daveMemcmp(__daveT1006, b1, sizeof(__daveT1006)/2)) {
			//        if (!_daveTestChars(dc->iface, __davet1006, 2)) {
			LOG2("%s 8 *** no DLE,ACK after transfer in Exchange.\n", dc->iface->name);
			return -7;
		}
	}
	if (trN==7) {
		//    	usleep(450000);
	}//TODO: check compression time
	res=_daveEndTrans(dc);
	if (res<0) {
		LOG2("%s *** Error in Exchange.EndTrans request.\n", dc->iface->name);
		return res-30;
	}
	return 0;
}

/*
In S7, we need to tell the PLC what memory area we want to read from or write to. The PLC
behaves as if each area were a different physical memory starting with an offset of 0.
In S5, everything is in common 64k of memory. For different areas, we have to add start 
offsets of areas or objects. 
The following is needed to make memory access as S7-compatible as possible.
*/
int areaFromBlockType(int area){
	switch(area) {
	case daveS5BlockType_DB:		// S5 block type
	case daveBlockType_DB:			// S7 block type
	case daveDB: 				// S7 area type
		return daveS5BlockType_DB;
	case daveS5BlockType_OB:
	case daveBlockType_OB: 
		return daveS5BlockType_OB;
	case daveS5BlockType_FB:
	case daveBlockType_FB: 
		return daveS5BlockType_FB;    
		// s5 only:	    
	case daveS5BlockType_PB:
		return daveS5BlockType_PB;
	case daveS5BlockType_SB:
		return daveS5BlockType_SB;
	default: return area;    
	}
}
/*
Requests physical addresses and lengths of blocks in PLC memory and writes
them to ai structure:
*/
int DECL2 _daveReadS5BlockAddress(daveConnection * dc, uc area, uc BlockN, daveS5AreaInfo * ai)
{
	int res,dbaddr,dblen, s5Area;
	uc b1[24];			//15 + some Dups
	//    if (_daveIsS5BlockArea(area)<0) {
	//            printf("%s *** Not block area .\n", dc->iface->name);
	//	    return -1;
	//    }

	//    b1[0]=area;

	s5Area=areaFromBlockType(area);
	b1[0]=s5Area;
	b1[1]=BlockN;
	res=_daveExchangeAS511(dc,b1,2,24,0x1A);
	if (res<0) {
		printf("%s *** Error in BlockAddr.Exchange sequense.\n", dc->iface->name);
		return res-10;
	}
	if (dc->AnswLen<15) {
		printf("%s *** Too few chars (%d) in BlockAddr data.\n", dc->iface->name,dc->AnswLen);
		return -2;
	}
	if ((dc->msgIn[0]!=0)
		||(dc->msgIn[3]!=0x70)
		||(dc->msgIn[4]!=0x70)
		||(dc->msgIn[5]!=0x40+s5Area)||(dc->msgIn[6]!=BlockN)) {
			printf("%s *** Wrong BlockAddr data signature.\n", dc->iface->name);
			return -3;
	}
	dbaddr=dc->msgIn[1];
	dbaddr=dbaddr*256+dc->msgIn[2];//Let make shift operations to compiler's optimizer
	dblen=dc->msgIn[11];
	dblen=(dblen*256+dc->msgIn[12]-5)*2; //PLC returns dblen in words including
	//5 word header (but returnes the
	//start address after the header) so 
	//dblen is length of block body
	ai->address=dbaddr;
	ai->len=dblen;
	return 0;
}

int DECL2 _daveIsS5BlockArea(uc area)
{
	if (
		//	(area!=daveBlockType_S5DB)&&
		(area!=daveS5BlockType_SB)&&
		(area!=daveS5BlockType_PB)&&
		(area!=daveS5BlockType_FX)&&
		(area!=daveS5BlockType_FB)&&
		(area!=daveS5BlockType_DX)&&
		(area!=daveS5BlockType_OB)) {
			return -1;
	}
	return 0;
}

int DECL2 _daveIsS5DBlockArea(uc area)
{
	if (area!=daveDB) {
		//        (area!=daveBlockType_S5DX))    
		//        (area!=daveBlockType_S5DX)) {
		return -1;
	}
	return 0;
}

#define maxSysinfoLen 87
/*
This is a trick which will intercept all functions not available for S5 AS511. It works 
this way:

A function for S7 forms a packet for S7 communication and then calls daveExchange which 
will send the packet and return the answer.
If a function is also vailable for S5, it must check whether the protocol is AS511. If so,
the function calls it's S5 counterpart and returns the result of it.
Hence, it will never reach daveExchange.

Now, functions for which there is no S5 counterpart simply continue, (superfluously) form
the S7 packet and call daveExchange, which will point hereto.
This fake function allways returns a specific error code so the user knows the function
is not available in the S5 protocol.

The advantage of this mechanism is that additional functions for S7 can be added at any 
time without caring about S5: If no special handling is provided, they end up here.
*/
int DECL2 _daveFakeExchangeAS511(daveConnection * dc, PDU *p){
	return daveNotAvailableInS5;
}

/*
This is a deviation from normal use of connect functions: There are no connections in AS511.
The reason why we provide a daveConnect() is this:
From an S5 CPU, you don't read inputs, outputs,flags or any other memory area but simply
bytes from global memory.
There are addresses of input image area, output image area, flags, timers etc. These depend
on CPU model. Next, there are start addresses of the data blocks. These addresses change 
whenever a data block is created or changed in size or modified by programming device.

In both cases, we could read the adresses from the PLC before reading the data. To save
time and gain efficiency, we read them once in connectPLC. We rely on users following the 
S7 scheme: connect to a PLC before reading from it !!

If we would read addresses each time, you could do something you cannot with S7: pull the
plug from one PLC, connect to another PLC and the program still works.

Here, you CANNOT do that. You have to call connectPLC again after changing to the new PLC.

Another thing are data block addresses. We could fetch all 256 possible addresses in connectPLC,
too. But that would use 256 entries that must exist while the program might not use data 
blocks at all. So we don't. We add data block addresses to the PLC address cache when they 
are used for the first time.
There are S5 programs that create data blocks dynamically. Hence cached addresses get invalid.
If you have a PLC with such a program use 
daveSetNonCacheable(dc, DBnumber);
If you suspect somebody could pull the plug, connect a programming device, modify data blocks 
and reconnect your application program, use    
daveSetNonCacheable(dc, allDBs);
In this case, the actual address will be fetched before each read or write from/to the related
data blocks (which will slow down your application).
*/

int DECL2 _daveConnectPLCAS511(daveConnection * dc){
	int res;
	uc b1[maxSysinfoLen]; //20 words + some Dups
	//    dc->maxPDUlength=1000;
	dc->maxPDUlength=240;
	dc->cache=(daveS5cache*)calloc(1,sizeof(daveS5cache));

	res=_daveExchangeAS511(dc,b1,0,maxSysinfoLen,0x18);
	if (res<0) {
		LOG2("%s *** Error in ImageAddr.Exchange sequence.\n", dc->iface->name);
		return res-10;
	}
	if (dc->AnswLen<47) {
		LOG3("%s *** Too few chars (%d) in ImageAddr data.\n", dc->iface->name,dc->AnswLen);
		return -2;
	}
	_daveDump("connect:",dc->msgIn, 47);
	dc->cache->PAE=daveGetU16from(dc->msgIn+5);	// start of inputs;
	dc->cache->PAA=daveGetU16from(dc->msgIn+7);	// start of outputs;
	dc->cache->flags=daveGetU16from(dc->msgIn+9);	// start of flag (marker) memory;
	dc->cache->timers=daveGetU16from(dc->msgIn+11);	// start of timer memory;
	dc->cache->counters=daveGetU16from(dc->msgIn+13);	// start of counter memory
	dc->cache->systemData=daveGetU16from(dc->msgIn+15);	// start of system data
	dc->cache->first=NULL;
	LOG2("start of inputs in memory %04x\n",dc->cache->PAE);
	LOG2("start of outputs in memory %04x\n",dc->cache->PAA);
	LOG2("start of flags in memory %04x\n",dc->cache->flags);
	LOG2("start of timers in memory %04x\n",dc->cache->timers);
	LOG2("start of counters in memory %04x\n",dc->cache->counters);
	LOG2("start of system data in memory %04x\n",dc->cache->systemData);
	return 0;
}

int DECL2 _daveDisconnectPLCAS511(daveConnection * dc){
	free(dc->cache);
	dc->cache=0;
	return 0;
}

/*
Writes <count> bytes from area <BlockN> with offset <offset> from buf.
You can't write data to the program blocks because you can't syncronize
with PLC cycle. For this purposes use daveWriteBlock:
*/
int DECL2 daveWriteS5Bytes(daveConnection * dc, uc area, uc BlockN, int offset, int count, void * buf)
{
	int res,datastart;
	daveS5AreaInfo ai;
	uc b1[daveMaxRawLen];
	//    if (_daveIsS5DBlockArea(area)==0) {
	if (area==daveDB) {
		//	LOG1("_daveIsS5DBlockArea\n");
		res=_daveReadS5BlockAddress(dc,area,BlockN,&ai);
		if (res<0) {
			LOG2("%s *** Error in WriteS5Bytes.BlockAddr request.\n", dc->iface->name);
			return res-50;
		}
		datastart=ai.address;
	} else {
		switch (area) {
		case daveRawMemoryS5: datastart=0; break;
		case daveInputs: datastart=dc->cache->PAE; break;
		case daveOutputs: datastart=dc->cache->PAA; break;
		case daveFlags: datastart=dc->cache->flags; break;
		case daveTimer: datastart=dc->cache->timers; break;
		case daveCounter: datastart=dc->cache->counters; break;
		case daveSysDataS5: datastart=dc->cache->systemData; break;
		default:
			LOG2("%s *** Unknown area in WriteS5Bytes request.\n", dc->iface->name);
			return -1;
		}
	}
	if ((count>daveMaxRawLen)||(offset+count>ai.len)) {
		LOG2("%s writeS5Bytes *** Requested data is out-of-range.\n", dc->iface->name);
		return -1;
	}
	//    datastart=ai.address+offset;
	LOG2("area start is %04x, ",datastart);
	datastart+=offset;
	LOG2("data start is %04x\n",datastart);
	b1[0]=datastart/256;
	b1[1]=datastart%256;
	memcpy(&b1[2],buf,count);
	res=_daveExchangeAS511(dc,b1,2+count,0,0x03);
	if (res<0) {
		LOG2("%s *** Error in WriteS5Bytes.Exchange sequense.\n", dc->iface->name);
		return res-10;
	}
	return 0;
}

int DECL2 daveStopS5(daveConnection * dc) {
	uc b1[]={0x88,0x04};	// I don't know what this mean
	return daveWriteBytes(dc,daveSysDataS5,0,0x0c,2,b1);
}

int DECL2 daveStartS5(daveConnection * dc) {
	uc b1[]={0x68,0x00};	// I don't know what this mean
	return daveWriteBytes(dc,daveSysDataS5,0,0x0c,2,b1);
}

int DECL2 daveGetS5ProgramBlock(daveConnection * dc, int blockType, int number, char* buffer, int * length) {
	//    int totlen,res;
	//    *length=totlen;
	return daveResNotYetImplemented;
}


#ifndef AVR_NOOS
/********************************************
Use Siemens DLLs and drivers for transport:
*********************************************/

/*
While the following code is useless under operating systems others than win32,
I leave it here, independent of conditionals. This ensures it is and will continue
to be at least compileable now and over version changes. Who knows what it might
be good for in the future...
*/
/*
fill some standard fields and pass it to SCP-send:
*/
int DECL2 _daveSCP_send(int fd, uc * reqBlock) {	
	S7OexchangeBlock* fdr;
	fdr=(S7OexchangeBlock*)reqBlock;
	fdr->headerlength = 80;  //Length of the Header (always 80)  (but the 4 first unkown bytes are not count)
	fdr->rb_type = 2; //rb_type is always 2
	fdr->offset_1= 80; //Offset of the Begin of userdata (but the 4 first unkown bytes are not count)	

	if (fdr->application_block_subsystem == 0xE4)   //Fix for PLCSim
		Sleep(50);									//Fix for PLCSim

	return SCP_send(fd, fdr->seg_length_1 + fdr->headerlength, reqBlock);
}

int daveSCP_receive(int h, uc * buffer) {
	int res, datalen;
	S7OexchangeBlock * fdr;
	fdr=(S7OexchangeBlock*) buffer;

	if (fdr->application_block_subsystem == 0xE4)   //Fix for PLCSim
		Sleep(50);									//Fix for PLCSim

	res=SCP_receive(h, 0xFFFF, &datalen, sizeof(S7OexchangeBlock), buffer);
	if (daveDebug & daveDebugByte) {
		_daveDump("header:",buffer, 80);
		_daveDump("data:",buffer+80, fdr->seg_length_1);
	}	
	return res;	
}    

/*
* 
*/
int DECL2 _daveConnectPLCS7online (daveConnection * dc) {

	int res=0;
	uc p2[]={
		0x00,
		0x02,
		0x01,
		0x00,
		0x0C,
		0x01,
		0x00,
		0x00,
		0x00,
		0x06,  //MPI Address or IP1
		0x00,  //IP2
		0x00,  //IP3
		0x00,  //IP4
		0x00,
		0x00,
		0x01,
		0x00,
		0x02,
		0x01, //Connection type (1=PG, 2=OP)
		0x00, //Rack+Slot
		0x00,
		0x00, //9 when routing
		0x00, //6 when routing
		0x00, //Subnet1
		0x00, //Subnet2
		0x00,
		0x00,
		0x00, //Subnet3
		0x00, //Subnet4
		0x00, //size of following address
		0x00, //Routing MPI Address or IP1 
		0x00, //Routing IP2
		0x00, //Routing IP3 
		0x00, //Routing IP4 
	};


	uc pa[]=	{0xF0, 0 ,0, 1, 0, 1, 3, 0xc0,};

	PDU pu2,pu1, *p;

	int a,b;

	S7OexchangeBlock * fdr;
	S7OexchangeBlock * rec;
	fdr=(S7OexchangeBlock*)(dc->msgOut);
	rec=(S7OexchangeBlock*)(dc->msgIn);

	dc->PDUstartI=80;

	//This 2 telegramms are only send when not using TCP/IP (maybe for initialising the Adapter?)
	if (!dc->DestinationIsIP)
	{
		fdr->subsystem= 0x22;
		fdr->response= 0xFF; 
		fdr->user= 0xFF;
		fdr->seg_length_1= 0x80;
		fdr->priority= 1;
		fdr->application_block_service= 0x1A;

		a= _daveSCP_send(((int)dc->iface->fd.wfd), dc->msgOut);
		daveSCP_receive((int)(dc->iface->fd.rfd), dc->msgIn);

		fdr->seg_length_1= 0xF2;
		fdr->application_block_service= 0xB;

		a= _daveSCP_send(((int)dc->iface->fd.wfd), dc->msgOut);
		daveSCP_receive((int)(dc->iface->fd.rfd), dc->msgIn);
	}


	//Telegramms for both Types (TCP/IP and MPI)

	//3rd telegramm / TCP(1st)
	memset(fdr,0,80);    
	fdr->response= 255; 
	fdr->subsystem= 0x40;
	a= _daveSCP_send(((int)dc->iface->fd.wfd), dc->msgOut);
	daveSCP_receive((int)(dc->iface->fd.rfd), dc->msgIn);

	//4th Telegramm / TCP(2nd)
	memset(fdr,0,206);
	fdr->user= 111;
	fdr->subsystem= 64;
	fdr->opcode= 1;
	fdr->response= 255;
	fdr->fill_length_1= 126;
	fdr->seg_length_1= 126;
	dc->connectionNumber=rec->application_block_opcode;
	fdr->application_block_opcode= dc->connectionNumber;
	fdr->application_block_ssap= 2;
	fdr->application_block_remote_address_station= 114;   //I think this should be the local MPI
	
	fdr->application_block_subsystem = rec->application_block_subsystem;  //When this is One it is a MP Connection, zero means TCP Connection!
	dc->application_block_subsystem = rec->application_block_subsystem; 
	//Maybe we remove the destination is IP Parameter and use the upper bit
	
	p2[19]=(dc->slot + dc->rack*32);	

	//Destination IP or MPI
	if (dc->DestinationIsIP)
	{
		p2[9]=dc->_Destination1;
		p2[10]=dc->_Destination2;
		p2[11]=dc->_Destination3;
		p2[12]=dc->_Destination4;
		//p2[19]=(dc->slot + dc->rack*32);				
	}
	else
		p2[9]=dc->MPIAdr;	

	if (dc->routing)
	{
		p2[0]=1; //JK (Routing enabled???)
		p2[19]=(dc->routingSlot + dc->routingRack*32); //--> Routing Rack/Slot 
		p2[21]=9;
		p2[22]=6;
		p2[23]=(unsigned char) (dc->routingSubnetFirst >> 8);
		p2[24]=(unsigned char) (dc->routingSubnetFirst);
		p2[27]=(unsigned char) (dc->routingSubnetSecond >> 8);
		p2[28]=(unsigned char) (dc->routingSubnetSecond);
		p2[29]=1;  //Maybe 4 when Routing to an IP???
		p2[30]=(unsigned char) dc->_routingDestination1;
		p2[31]=(unsigned char) dc->_routingDestination2;
		p2[32]=(unsigned char) dc->_routingDestination3;
		p2[33]=(unsigned char) dc->_routingDestination4;		
	}
	//Destination Routing
	if (dc->routing && dc->routingDestinationIsIP)
	{
		p2[21]=12; // 3 Chars more ???
		p2[29]=4;  // 4 when Routing to an IP???
	}
	memcpy(&(fdr->user_data_1),p2,sizeof(p2));
	a=_daveSCP_send((int)(dc->iface->fd.wfd), dc->msgOut);
	b=daveSCP_receive((int)(dc->iface->fd.rfd), dc->msgIn);


	//Connection error???
	if (rec->response != 0x01)
		return -1;

	//5th Telegramm / TCP(3rd)
	memset(fdr,0,98);
	fdr->subsystem= 64;
	fdr->opcode= 6;	
	fdr->response= 255;
	fdr->fill_length_1= 18;
	fdr->seg_length_1= 18;
	fdr->application_block_opcode= dc->connectionNumber;
	if (!dc->DestinationIsIP)
		fdr->application_block_subsystem= 1;
	p=&pu1;
	//p->header=dc->msgOut+dc->PDUstartO;
	p->header=fdr->user_data_1;
	_daveInitPDUheader(p,1);
	_daveAddParam(p, pa, sizeof(pa));
	if (daveGetDebug() & daveDebugPDU)
		_daveDumpPDU(p);
	fdr->application_block_subsystem = dc->application_block_subsystem; //Test
	a= _daveSCP_send((int)(dc->iface->fd.wfd), dc->msgOut);
	daveSCP_receive((int)(dc->iface->fd.rfd), dc->msgIn);

	pu2.header=dc->msgIn+80;
	_daveSetupReceivedPDU(dc, &pu2);
	if (daveGetDebug() & daveDebugPDU)
		_daveDumpPDU(&pu2);

	//6th Telegramm (this get's the PDU size)  / TCP(4th)
	memset(fdr,0,560);
	fdr->user= 0;
	fdr->subsystem= 64;
	fdr->opcode= 7;
	fdr->response= 16642;
	fdr->seg_length_1= 480;
	fdr->application_block_opcode= dc->connectionNumber; 
	
	if (!dc->DestinationIsIP)
		fdr->application_block_subsystem= 1; 
	fdr->application_block_subsystem = dc->application_block_subsystem; //Test

	//PLCSIM...
	//fdr->response= 0;
	//fdr->seg_length_1= 1700;
	//fdr->application_block_subsystem= 0xe4; 

	a= _daveSCP_send((int)(dc->iface->fd.wfd), dc->msgOut);
	daveSCP_receive((int)(dc->iface->fd.rfd), dc->msgIn);
	_daveSetupReceivedPDU(dc, &pu2);
	if (daveGetDebug() & daveDebugPDU)
		_daveDumpPDU(&pu2);
	dc->maxPDUlength=daveGetU16from(pu2.param+6);
	//    if (daveDebug & daveDebugConnect) 
	LOG2("\n*** Partner offered PDU length: %d\n\n",dc->maxPDUlength);
	return res;

	/*    
	memset((uc*)(&giveBack)+80,0,480);
	giveBack.payloadLength= 480;
	return _daveNegPDUlengthRequest(dc, &pu1);
	*/    
}

//Look at the PLC Connection resources if this Function Closes a Connection!!
//Look via Wireshark!
int DECL2 _daveDisconnectPLCS7online(daveConnection * dc)
{
	int a;
	int datalen;
	
	uc buffer[sizeof(S7OexchangeBlock)];
	S7OexchangeBlock* fdr;
	fdr=(S7OexchangeBlock*)dc->msgOut;
	memset(dc->msgOut,0,80);
	
	fdr->subsystem=64;
	fdr->opcode=8;
	fdr->response=255; 
	fdr->application_block_opcode= dc->connectionNumber; 

	if (dc->DestinationIsIP)
	{
		a= _daveSCP_send((int)(dc->iface->fd.wfd), dc->msgOut);
		SCP_receive((int)(dc->iface->fd.rfd), 0xFFFF, &datalen, sizeof(S7OexchangeBlock), buffer);
	}
	
	memset(dc->msgOut,0,80);
	
	fdr->subsystem=64;
	fdr->opcode=0xC;
	fdr->response=255; 
	fdr->application_block_service=0x8000;
	fdr->application_block_opcode= dc->connectionNumber; 

	if (!dc->DestinationIsIP)
		fdr->application_block_subsystem= 1;

	a= _daveSCP_send((int)(dc->iface->fd.wfd), dc->msgOut);
	SCP_receive((int)(dc->iface->fd.rfd), 0xFFFF, &datalen, sizeof(S7OexchangeBlock), buffer);
	
	return 0;
}

int DECL2 _daveSendMessageS7online(daveConnection *dc, PDU *p) {
	int a, b;
	int datalen;
	int len=p->hlen+p->plen+p->dlen;
	uc buffer[sizeof(S7OexchangeBlock)];
	S7OexchangeBlock* fdr;
	fdr=(S7OexchangeBlock*)dc->msgOut;
	memset(dc->msgOut,0,80);
	//    fdr->number= 114;
	fdr->subsystem=64;
	fdr->opcode=6;
	//JK fdr->response=16642;
	fdr->response=255; //JK

	fdr->fill_length_1=len;
	fdr->seg_length_1=len;
	//JK fdr->application_block_subsystem= 1;
	//fdr->application_block_opcode= 10; //JK
	fdr->application_block_opcode= dc->connectionNumber; //JK

	if (!dc->DestinationIsIP)
		fdr->application_block_subsystem= 1;
	fdr->application_block_subsystem=dc->application_block_subsystem; //Test
	
	//    memcpy(&(fdr->payload),buffer,len);
	a= _daveSCP_send((int)(dc->iface->fd.wfd), dc->msgOut);
	if (daveDebug & daveDebugErrorReporting) 
		LOG2("RetVal SCP_send in SendMessageS7Online: ",a);
	
	b= SCP_receive((int)(dc->iface->fd.rfd), 0xFFFF, &datalen, sizeof(S7OexchangeBlock), buffer);	
	if (daveDebug & daveDebugErrorReporting) 
		LOG2("RetVal SCP_recieve in SendMessageS7Online: ",b);
	//    daveSCP_receive(dc->iface->fd.rfd, dc->msgIn);
	return 0;
}    

int DECL2 _daveGetResponseS7online(daveConnection *dc) {
	int a, b;

	//if (dc->DestinationIsIP)
	//	fdr->application_block_subsystem=1;		

	a= _daveSCP_send((int)(dc->iface->fd.rfd), dc->msgIn);
	if (daveDebug & daveDebugErrorReporting) 
		LOG2("RetVal SCP_send in GetResponseS7online: ",a);

	b= daveSCP_receive((int)(dc->iface->fd.rfd), dc->msgIn);
	if (daveDebug & daveDebugErrorReporting) 
		LOG2("RetVal SCP_recieve in GetResponseS7online: ",b);

	return 0;
}

int DECL2 _daveExchangeS7online(daveConnection * dc, PDU * p) {
	int res;
	res=_daveSendMessageS7online(dc, p);
	dc->AnswLen=0;
	res=_daveGetResponseS7online(dc);
	return res;
}

int DECL2 _daveListReachablePartnersS7online (daveInterface * di, char * buf) {
	int a;
	S7OexchangeBlock reqBlock;
	uc b1[sizeof(S7OexchangeBlock)];
	S7OexchangeBlock* fdr;
	fdr=&reqBlock;

	memset(fdr,0,140);
	fdr->user= 102;
	fdr->priority= 1;
	fdr->subsystem= 34;
	fdr->response= 16642;
	fdr->seg_length_1= 60;
	fdr->application_block_service= 0x28;

	a= _daveSCP_send((int)(di->fd.wfd), (uc *) &reqBlock);
	daveSCP_receive((int)(di->fd.rfd), b1);

	fdr->user= 103;
	fdr->priority= 1;
	fdr->subsystem= 34;
	fdr->response= 16642;
	fdr->application_block_service= 0x17;

	a= _daveSCP_send((int)(di->fd.wfd), (uc *) &reqBlock);
	daveSCP_receive((int)(di->fd.rfd), b1);
	memset(fdr,0,140);

	fdr->user= 104;
	fdr->priority= 1;
	fdr->subsystem= 34;
	fdr->response= 16642;
	fdr->seg_length_1= 60;
	fdr->application_block_service= 0x28;
	a= _daveSCP_send((int)(di->fd.wfd), (uc *) &reqBlock);
	daveSCP_receive((int)(di->fd.rfd), b1);

	memset(fdr,0,208);
	fdr->user= 105;
	fdr->priority= 1;
	fdr->subsystem= 34;
	fdr->response =16642;
	fdr->seg_length_1= 128;
	fdr->application_block_service= 0x1a;
	a= _daveSCP_send((int)(di->fd.wfd), (uc *) &reqBlock);
	daveSCP_receive((int)(di->fd.rfd), b1);

	memcpy(buf,b1+80,126);
	return 126;
}

#endif

/*
This is not quite the same as in other protocols: Normally, we have a file descriptor or
file handle in di->fd.rfd, di->fd.wfd. disconnectAdapter does something like making the
MPI adapter leaving the Profibus token ring. File descriptor remains valid until it is 
closed with closePort().
In principle, instead of closing it, we could redo the sequence 
daveNewInterface, initAdapter and then continue to use it.
We cannot use closePort() on a "handle" retrieved from SCP_open(). It isn't a file handle.
We cannot make closePort() treat it differently as there is no information in di->fd.rfd
WHAT it is.
- We could make di->fd.rfd a structure with extra information.
- We could pass struct di->fd (daveOSserialtype) instead of di->fd.rfd / di->fd.wfd to all
all functions dealing with di->fd.rfd. Then we could add extra information to 
daveOSserialtype
- We could better pas a pointer to an extended daveOSserialtype as it makes less problems
when passing it to different programming languages.
These would be major changes. They would give up the (theroetical?) possibility to use file 
handles obtained in quite a different way and to put them into daveOSserialtype.
I chose to change as little as possible for s7online and just SCP_close the handle here,
expecting no one will try to reuse after this.

Up to here is what version 0.8 does. The probleme is now that an application cannot do
daveDisconnectAdapter(), closePort() as it does for other protocols.

Now comes a second kludge for 0.8.1: We replace the "file handles" value by -1. Now we can 
tell closePort() to do nothing for a value of -1.

Befor releasing that, I think it is better to use different close functions, closeS7oline 
for s7online and closePort() for everything else.
*/

/*
int DECL2 _daveDisconnectAdapterS7online(daveInterface * di) {
int res;
res=SCP_close((int)(di->fd.rfd));
di->fd.rfd=-1;
di->fd.wfd=-1;
return res;
}
*/
/***
NetLink 50
***/
#define NET

#ifndef AVR_NOOS

#ifndef NET
int DECL2 _daveReadMPINLPro(daveInterface * di, uc *b) {
	int res=0,state=0,nr_read;
	uc bcc=0;
	nr_read= di->ifread(di, b, 2);
	if (nr_read>=2) {
		res=256*b[0]+b[1]+2;
		if (res>nr_read)
			nr_read+=di->ifread(di, b+nr_read, res-nr_read);
		if (daveDebug & daveDebugInitAdapter) 
			LOG4("%s nr_read:%d res:%d.\n", di->name, nr_read, res);
		return res-2;
	}
}
#endif

#ifdef NET
#ifdef HAVE_SELECT
/*
Read one complete packet. The bytes 0 and 1 contain length information.
This version needs a socket filedescriptor that is set to O_NONBLOCK or
it will hang, if there are not enough bytes to read.
The advantage may be that the timeout is not used repeatedly.
*/
int DECL2 _daveReadMPINLPro(daveInterface * di,uc *b) {
	int res,length;
	fd_set FDS;
	struct timeval t;
	FD_ZERO(&FDS);
	FD_SET(di->fd.rfd, &FDS);

	t.tv_sec = di->timeout / 1000000;
	t.tv_usec = di->timeout % 1000000;
	if (select(di->fd.rfd + 1, &FDS, NULL, NULL, &t) <= 0) {
		if (daveDebug & daveDebugByte) LOG1("timeout in ReadMPINLPro.\n");
		return daveResTimeout;
	} else {
		res=read(di->fd.rfd, b, 2);
		if (res<2) {
			if (daveDebug & daveDebugByte) {
				LOG2("res %d ",res);
				_daveDump("readISOpacket: short packet", b, res);
			}
			return daveResShortPacket; /* short packet */
		}
		length=b[1]+0x100*b[0];
		res+=read(di->fd.rfd, b+2, length);
		if (daveDebug & daveDebugByte) {
			LOG3("readMPINLPro: %d bytes read, %d needed\n",res, length);
			_daveDump("readMPINLPro: packet", b, res);    
		}
		return (res);
	}
}
#endif /* HAVE_SELECT */

#ifdef BCCWIN
int DECL2 _daveReadMPINLPro(daveInterface * di,uc *b) {
	int res,i,length;
	i=recv((SOCKET)(di->fd.rfd), b, 2, 0);
	res=i;
	if (res <= 0) {
		if (daveDebug & daveDebugByte) LOG1("timeout in ReadMPINLPro.\n");
		return daveResTimeout;
	} else {
		if (res<2) {
			if (daveDebug & daveDebugByte) {
				LOG2("res %d ",res);
				_daveDump("ReadMPINLPro: short packet", b, res);
			}
			return daveResShortPacket; /* short packet */
		}
		length=b[1]+0x100*b[0];
		i=recv((SOCKET)(di->fd.rfd), b+2, length, 0);
		res+=i;
		if (daveDebug & daveDebugByte) {
			LOG3("ReadMPINLPro: %d bytes read, %d needed\n",res, length);
			_daveDump("ReadMPINLPro: packet", b, res);    
		}
		return (res);
	}
}

#endif /* */
#endif


/* 
This initializes the MPI adapter. Andrew's version.
*/

int DECL2 _daveInitAdapterNLPro(daveInterface * di)  /* serial interface */
{
	uc b3[]={
		0x01,0x03,0x02,0x27, 0x00,0x9F,0x01,0x14,
		0x00,0x90,0x01,0xc, 0x00,	/* ^^^ MaxTsdr */
		0x00,0x5,
		0x02,/* Bus speed */

		0x00,0x0F,0x05,0x01,0x01,0x03,0x81,/* from topserverdemo */
		/*^^ - Local mpi */
	};		

	int res;
	b3[16]=di->localMPI;
	if (di->speed==daveSpeed500k)
		b3[7]=0x64;
	if (di->speed==daveSpeed1500k)
		b3[7]=0x96;
	b3[15]=di->speed;

	//    res=_daveInitStep(di, 1, b3, sizeof(b3),"initAdapter()");
	res=_daveInitStepNLPro(di, 1, b3, sizeof(b3),"initAdapter()", NULL);

	//    res= _daveReadMPINLPro(di, b1);
	if (daveDebug & daveDebugInitAdapter) 
		LOG2("%s initAdapter() success.\n", di->name);
	//   _daveSendSingle(di,DLE);
	di->users=0;	/* there cannot be any connections now */
	return 0;
}


void DECL2 _daveSendSingleNLPro(daveInterface * di,	/* serial interface */
	uc c  			/* chracter to be send */
	) 
{
	unsigned long i;
	uc c3[3];
	c3[0]=0;
	c3[1]=1;
	c3[2]=c;
	//    di->ifwrite(di, c3, 3);
#ifdef HAVE_SELECT
	daveWriteFile(di->fd.wfd, c3, 3, i);
#endif    
#ifdef BCCWIN
	send((unsigned int)(di->fd.wfd), c3, 3, 0);
#endif

}

/*
int DECL2 _daveSendISOPacket(daveConnection * dc, int size) {
unsigned long i;
size+=4;
*(dc->msgOut+3)=size % 0x100;	//was %0xFF, certainly a bug	
*(dc->msgOut+2)=size / 0x100;
*(dc->msgOut+1)=0;
*(dc->msgOut+0)=3;
if (daveDebug & daveDebugByte) 
_daveDump("send packet: ",dc->msgOut,size);
#ifdef HAVE_SELECT
daveWriteFile(dc->iface->fd.wfd, dc->msgOut, size, i);
#endif    
#ifdef BCCWIN
send((unsigned int)(dc->iface->fd.wfd), dc->msgOut, size, 0);
#endif
return 0;
}
*/

/* 
This sends a string after doubling DLEs in the String
and adding DLE,ETX and bcc.
*/
int DECL2 _daveSendWithCRCNLPro(daveInterface * di, /* serial interface */
	uc *b, 		 /* a buffer containing the message */
	int size		 /* the size of the string */
	)
{		
	uc target[daveMaxRawLen];
	int i,targetSize=2;
	target[0]=size / 256;
	target[1]=size % 256;

	//    int bcc=DLE^ETX; /* preload */
	for (i=0; i<size; i++) {
		target[targetSize]=b[i];targetSize++;
	};
	//    targetSize+=0;
	//    di->ifwrite(di, target, targetSize);
#ifdef HAVE_SELECT
	daveWriteFile(di->fd.wfd, target, targetSize, i);
#endif    
#ifdef BCCWIN
	send((unsigned int)(di->fd.wfd), target, targetSize, 0);
#endif

	if (daveDebug & daveDebugPacket)
		_daveDump("_daveSendWithCRCNLPro",target, targetSize);
	return 0;
}


/* 
Send a string of init data to the MPI adapter.
*/
int DECL2 _daveInitStepNLPro(daveInterface * di, int nr, uc *fix, int len, char * caller, uc * buffer ) {
	uc res[500];
	int i;
	if (daveDebug & daveDebugInitAdapter)
		LOG4("%s %s step %d.\n", di->name, caller, nr);

	_daveSendWithCRCNLPro(di, fix, len);
	i=_daveReadMPINLPro(di, (buffer != NULL) ? buffer : res );
	return i;
}    

/* 
Open connection to a PLC. This assumes that dc is initialized by
daveNewConnection and is not yet used.
(or reused for the same PLC ?)
*/
int DECL2 _daveConnectPLCNLPro(daveConnection * dc) {
	int res, length;
	PDU p1;
	uc b4[]={
		0x04, //00
		0x80, //01 (0x80 | MPI)
		0x80, //02
		0x0D, //03
		0x00, //04
		0x14, //05
		0xE0, //06
		0x04, //07
		0x00, //08
		0x80, //09
		0x00, //10
		0x02, //11 
		0x00, //12 //01 ??? Routing???
		0x02, //13 //02 = no routing / 0c = Routing to MPI / 0f Routing to IP (Bytecount to End-2)
		dc->ConnectionType, //14 //Connection Type (01=PG/02=OP)
		(dc->slot + dc->rack * 32),	// hopefully this is Rack/Slot was 0x00, //15
		dc->ConnectionType, //16 //also Connection Type (01=PG/02=OP)?? //06 when Routing??
		0x00, //17 //End of Telegram when no Routing (00) / 01 Routing to MPI / 04 Routing to IP

		0x02, //18 
		0xaa, //19 subnet1
		0xaa, //20 subnet1
		0x00, //21
		0x00, //22
		0xbb, //23 subnet2
		0xbb, //24 subnet2
		0x00, //25 IP1 or MPI
		0x00, //26 IP2 //Byte not present when Routing to MPI
		0x00, //27 IP3 //Byte not present when Routing to MPI
		0x00, //28 IP4 //Byte not present when Routing to MPI
		0x02, //29 Connection Type Routing
		0x04  //30 Rack, Slot

	};	

	/*us t4[]={
		0x04,0x80,0x180,0x0C,0x114,0x103,0xD0,0x04,	// 1/10/05 trying Andrew's patch
		0x00,0x80,
		0x00,0x02,0x00,0x02,0x01,
		0x00,0x01,0x00,
	};*/
	uc b5[]={	
		0x05,0x07,
	};
	/*us t5[]={    
		0x04,
		0x80,
		0x180,0x0C,0x114,0x103,0x05,0x01,
	};*/
	b4[1]|=dc->MPIAdr;	
	b4[5]=dc->connectionNumber; // 1/10/05 trying Andrew's patch

	//t4[1]|=dc->MPIAdr;	
	//t5[1]|=dc->MPIAdr;	

	length = sizeof(b4)-13;

	if (dc->routing)
	{	
		//b4[11] = dc->MPIAdr;
		b4[12] = 1;
		//b4[13] = dc->CPUConnectiontype; //not yet tested
		b4[16] = 6;
		b4[19] = dc->routingSubnetFirst >> 8;
		b4[20] = dc->routingSubnetFirst;
		b4[23] = dc->routingSubnetSecond >> 8;
		b4[24] = dc->routingSubnetSecond;
		b4[25] = dc->_routingDestination1;

		if (dc->routingDestinationIsIP)
		{
			length = sizeof(b4);
			b4[13] = 0x0f;
			b4[17] = 0x04;
			b4[26] = dc->_routingDestination2;
			b4[27] = dc->_routingDestination3;
			b4[28] = dc->_routingDestination4;
			b4[29] = dc->routingConnectionType; //Connection type
			b4[30] = dc->routingSlot + dc->routingRack*32;			
		}
		else
		{
			length = sizeof(b4)-3;
			b4[13] = 0x0c;
			b4[17] = 0x01;
			b4[26] = dc->routingConnectionType; //Connection type
			b4[27] = dc->routingSlot + dc->routingRack*32;		
		}
	}
	

	res=_daveInitStepNLPro(dc->iface, 1, b4, length,"connectPLC(1)", dc->msgIn);
	if (res-2 != length)
		return daveResTimeout;

	// first 2 bytes of msgIn[] contain packet length
	dc->connectionNumber2=dc->msgIn[2+5]; // 1/10/05 trying Andrew's patch
	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC(1) step 4.\n", dc->iface->name);	

	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 5.\n", dc->iface->name);	

	_daveSendWithPrefixNLPro(dc, b5, sizeof(b5));		

	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 6.\n", dc->iface->name);	
	res= _daveReadMPINLPro(dc->iface,dc->msgIn);
	if (daveDebug & daveDebugConnect) 
		LOG2("%s daveConnectPLC() step 7.\n", dc->iface->name);	
	res= _daveNegPDUlengthRequest(dc, &p1);
	return 0;
}


/*
Executes part of the dialog necessary to send a message:
*/
int DECL2 _daveSendDialogNLPro(daveConnection * dc, int size)
{
	if (size>5){
		dc->needAckNumber=dc->messageNumber;
		dc->msgOut[dc->iface->ackPos+1]=_daveIncMessageNumber(dc);
	}	
	_daveSendWithPrefix2NLPro(dc, size);
	return 0;
}


/*
Sends a message and gets ackknowledge:
*/
int DECL2 _daveSendMessageNLPro(daveConnection * dc, PDU * p) {
	if (daveDebug & daveDebugExchange) {
		LOG2("%s enter _daveSendMessageNLPro\n", dc->iface->name);
	}    
	if (_daveSendDialogNLPro(dc, /*2+*/p->hlen+p->plen+p->dlen)) {
		LOG2("%s *** _daveSendMessageMPI error in _daveSendDialog.\n",dc->iface->name);	    		
		//	return -1;	
	}	
	if (daveDebug & daveDebugExchange) {
		LOG3("%s _daveSendMessageMPI send done. needAck %x\n", dc->iface->name,dc->needAckNumber);	    
	}	
	return 0;
}


int DECL2 _daveExchangeNLPro(daveConnection * dc, PDU * p) {
	_daveSendMessageNLPro(dc, p);
	dc->AnswLen=0;
	return _daveGetResponseNLPro(dc);
}

int DECL2 _daveGetResponseNLPro(daveConnection *dc) {
	int res;
	if (daveDebug & daveDebugExchange) {
		LOG2("%s _daveGetResponseNLPro receive message.\n", dc->iface->name);	    
	}	
	res = _daveReadMPINLPro(dc->iface,dc->msgIn);
	if (res<0) {
		return res;
	}	
	if (res==0) {
		if (daveDebug & daveDebugPrintErrors) {
			LOG2("%s *** _daveGetResponseNLPro no answer data.\n", dc->iface->name);
		}        
		return -3;
	}	
	return 0;
}


int DECL2 _daveSendWithPrefixNLPro(daveConnection * dc, uc *b, int size)
{
	uc target[daveMaxRawLen];
	//    uc fix[]= {04,0x80,0x80,0x0C,0x03,0x14};
	uc fix[]= {0x4,0x80,0x80,0x0C,0x14,0x14};    
	fix[4]=dc->connectionNumber2; 		// 1/10/05 trying Andrew's patch
	fix[5]=dc->connectionNumber; 		// 1/10/05 trying Andrew's patch
	memcpy(target,fix,sizeof(fix));
	memcpy(target+sizeof(fix),b,size);
	target[1]|=dc->MPIAdr;
	//	target[2]|=dc->iface->localMPI;
	memcpy(target+sizeof(fix),b,size);
	return _daveSendWithCRCNLPro(dc->iface,target,size+sizeof(fix));
}


int DECL2 _daveSendWithPrefix2NLPro(daveConnection * dc, int size)
{
	//    uc fix[]= {04,0x80,0x80,0x0C,0x03,0x14};
	uc fix[]= {0x14,0x80,0x80,0x0C,0x14,0x14};
	fix[4]=dc->connectionNumber2;		// 1/10/05 trying Andrew's patch
	fix[5]=dc->connectionNumber;		// 1/10/05 trying Andrew's patch
	memcpy(dc->msgOut, fix, sizeof(fix));
	dc->msgOut[1]|=dc->MPIAdr;
	//	dc->msgOut[2]|=dc->iface->localMPI; //???
	///	dc->msgOut[sizeof(fix)]=0xF1;
	/*	if (daveDebug & daveDebugPacket)
	_daveDump("_daveSendWithPrefix2",dc->msgOut,size+sizeof(fix)); */
	return _daveSendWithCRCNLPro(dc->iface, dc->msgOut, size+sizeof(fix));
	//    }
	return -1; /* shouldn't happen. */
}

int DECL2 _daveDisconnectPLCNLPro(daveConnection * dc)
{


	
	int res;
	uc m[]={
		0x80
	};
	uc t1[] = {0x04, 0x82, 0x80, 0x0c, 0x14, 0x14, 0x80};
	uc b1[daveMaxRawLen];

	
	_daveSendWithPrefixNLPro(dc, t1, sizeof(t1));  //New Disconnection, old one did not work on my System.

	//JK _daveSendSingleNLPro(dc->iface, STX);
	//JK res=_daveReadMPINLPro(dc->iface,b1);
	//JK _daveSendWithPrefixNLPro(dc, m, 1);	

	res=_daveReadMPINLPro(dc->iface,b1);
	/*
	res=_daveReadMPI(dc->iface,b1);
	if (daveDebug & daveDebugConnect) 
	_daveDump("got",b1,10);
	_daveSendSingle(dc->iface, DLE);
	*/
	return 0;
}    

/*
It seems to be better to complete this subroutine even if answers
from adapter are not as expected.
*/
int DECL2 _daveDisconnectAdapterNLPro(daveInterface * di) {
	int res;
	uc m2[]={
		1,4,2
	};

	uc b1[daveMaxRawLen];
	if (daveDebug & daveDebugInitAdapter) 
		LOG2("%s enter DisconnectAdapter()\n", di->name);	
	//    _daveSendSingleNLPro(di, STX);
	//    res=_daveReadMPINLPro(di,b1);
	/*    if ((res!=1)||(b1[0]!=DLE)) return -1; */
	_daveSendWithCRCNLPro(di, m2, sizeof(m2));		
	if (daveDebug & daveDebugInitAdapter) 
		LOG2("%s daveDisconnectAdapter() step 1.\n", di->name);	
	res=_daveReadMPINLPro(di, b1);
	/*    if ((res!=1)||(b1[0]!=DLE)) return -2; */
	/*
	res=_daveReadMPI(di, b1);
	*/    
	/*    if ((res!=1)||(b1[0]!=STX)) return -3; */
	/*
	if (daveDebug & daveDebugInitAdapter) 
	LOG2("%s daveDisconnectAdapter() step 2.\n", di->name);	
	_daveSendSingle(di, DLE);
	_daveReadChars2(di, b1, daveMaxRawLen);
	//    _daveReadChars(di, b1, tmo_normal, daveMaxRawLen);
	_daveSendSingle(di, DLE);
	if (daveDebug & daveDebugInitAdapter) 
	_daveDump("got",b1,10);
	*/	
	return 0;	
}

/*
*/
int DECL2 _daveListReachablePartnersNLPro(daveInterface * di,char * buf) {
	uc b1[daveMaxRawLen];
	uc m1[]={1,7,2};
	int res;
	_daveSendWithCRCNLPro(di, m1, sizeof(m1));
	res=_daveReadMPINLPro(di,b1);
	//    LOG2("res: %d\n", res);	
	if(135==res){
		memcpy(buf,b1+8,126);
		return 126;
	} else
		return 0;	
}   

#endif

_openFunc SCP_open;
_closeFunc SCP_close;
_sendFunc SCP_send;
_receiveFunc SCP_receive;
//_SetHWndMsgFunc SetSinecHWndMsg;
//_SetHWndFunc SetSinecHWnd;
_get_errnoFunc SCP_get_errno;


/*
Changes: 
09/09/04  applied patch for variable Profibus speed from Andrew Rostovtsew.
12/09/04  removed debug printf from daveConnectPLC.
12/09/04  found and fixed a bug in daveFreeResults(): The result set is provided by the 
application and not necessarily dynamic. So we shall not free() it.
12/10/04  added single bit read/write functions.
12/12/04  added Timer/Counter read functions.
12/13/04  changed dumpPDU to dump multiple results from daveFuncRead
12/15/04  changed comments to pure C style 
12/15/04  replaced calls to write() with makro daveWriteFile.
12/15/04  removed daveSendDialog. Was only used in 1 place.
12/16/04  removed daveReadCharsPPI. It is replaced by daveReadChars.
12/30/04  Read Timers and Counters from 200 family. These are different internal types!
01/02/05  Hopefully fixed local MPI<>0.
01/10/05  Fixed some debug levels in connectPLCMPI
01/10/05  Splitted daveExchangeMPI into the send and receive parts. They are separately
useable when communication is initiated by PLC.
01/10/05  Code cleanup. Some more things in connectPLC can be done using genaral
MPI communication subroutines.
01/10/05  Partially applied changes from Andrew Rostovtsew for multiple MPI connections
over the same adapter.
01/11/05  Lasts steps in connect PLC can be done with exchangeMPI.
01/26/05  replaced _daveConstructReadRequest by the sequence prepareReadRequest, addVarToReadRequest
01/26/05  added multiple write
02/02/05  added readIBHpacket
02/05/05  merged in fixes for (some?) ARM processors.
02/06/05  Code cleanup.
03/06/05  Fixed disconnectPLC_IBH for MPI adresses other than 2.
03/12/05  clear answLen before read
03/12/05  reset templ.packetNumber in connectPLC_IBH. This is necessary to reconnect if the
connection has been interrupted.
03/23/05  fixes for target PPI addresses other than 2.

04/05/05  reworked error reporting.
04/06/05  renamed swap functions. When I began libnodave on little endian i386 and Linux, I used
used Linux bswap functions. Then users (and later me) tried other systems without
a bswap. I also cannot use inline functions in Pascal. So I made my own bswaps. Then 
I, made the core of my own swaps dependent of DAVE_LITTLE_ENDIAN conditional to support also
bigendien systems. Now I want to rename them from bswap to something else to avoid 
confusion for LINUX/UNIX users. The new names are daveSwapIed_16 and daveSwapIed_32. This
shall say swap "if endianness differs". While I could have used similar functions 
from the network API (htons etc.) on Unix and Win32, they may not be present on
e.g. microcontrollers.
I highly recommend to use these functions even when writing software for big endian 
systems, where they effectively do nothing, as it will make your software portable.
04/09/05  removed template IBH_MPI header from daveConnection. Much of the information is
also available from other fields and the structure is simpler to define in other 
languages.
04/09/05  removed CYGWIN defines. As there were no more differences against LINUX, it should 
work with LINUX defines.	      
04/21/05  renamed LITTLEENDIAN to DAVE_LITTLE_ENDIAN because it seems to conflict with
another #define in winsock2.h.	      
05/09/05  renamed more functions to daveXXX.	      
05/11/05  added some functions for the convenience of usage with .net or mono. The goal is
that the application doesn't have to use members of data structures defined herein
directly. This avoids "unsafe" pointer expressions in .net/MONO. It should also ease
porting to VB or other languages for which it could be difficult to define byte by
byte equivalents of these structures.
05/12/05  applied some bug fixes from Axel Kinting.
05/12/05  applied bug fix from Lutz Nitzsche in daveSendISOpacket.
07/31/05  added message string copying for Visual Basic.
09/09/05  added code to ignore 7 byte packets from soft PLC 6ES7-4PY00-0YB7 in ISO_TCP.
09/10/05  added explicit type casts for pointers optained from malloc and calloc.
09/11/05  added read/write functions for long blocks of data.
09/24/05  Code clean up:
- Pointers to basic read/functions allow to redirect these functions, e.g. to libusb.
- More common code. Only the very fundamental read/functions differ between Linux and Win32.
09/24/05  added MPI protocol version 3. This is what Step7 talks to MPI adapters and seems
to be the only thing the Siemens USB-MPI adapter understands. This adapter is
currently only useable under Linux via libusb. 
09/27/05  added bug fix from Renato Gartmann: freeResults didn't free() the memory used for
the result pointer array.
09/29/05  hopefully fixed superfluos STX in daveConnectPLCMPI2. 
10/04/05  No there are adapters which want it...
10/05/05  Added first helper functions to use s7onlinx.dll for transport.
10/06/05  Added standard protocol specific functions to use s7onlinx.dll for transport.
10/06/05  renamed LITTLE_ENDIAN to DAVE_LITTLE_ENDIAN because it conflicts with
another #define in some headers on some ARM systems.
10/10/05  change some pointer increments for gcc-4.0.2 compatibility.
10/18/05  Indroduced a (temporary?) work around to allow applications to use normal sequence 
disconnectAdapter/closePort also with s7online.		
02/20/06  Added code to support NetLink Pro.
05/15/06  Applied changes from Ken Wenzel for NetLink Pro.
07/28/06  Added CRC calculation code from Peter Etheridge.

11/21/06  Hope to have fixed PDU length problem with IBHLink reported by Axel Kinting.

01/04/07  Set last byte of resp09 to don't care as reported by Axel Kinting.
02/07/08  Removed patch from Keith Harris for RTS line.
Version 0.8.4.5    
07/10/09  Changed readISOpacket for Win32 to select() before recv().
07/10/09  Added daveCopyRAMtoROM
07/11/09  Changed calculation of netLen in doUpload()
*/
