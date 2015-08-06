/*
 Part of Libnodave, a free communication libray for Siemens S7 300/400.
 This program simulates a CPx43.
 
 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2002.

 Libnodave is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 Libnodave is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Visual; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/

#include <stdio.h>
#include <unistd.h>
#include <string.h>

#include <pthread.h>

#include "log2.h"

#define uc unsigned char

#include <sys/time.h>
#include <sys/socket.h>

#include <netinet/in.h>
#include <arpa/inet.h>
#include <errno.h>
#define ThisModule __FILE__

#include "accepter.c"
//#include <byteswap.h>

#include "nodave.h"

#define bSize 1256
#define us unsigned short

#define debug 10

/*
    many (1024) bytes. hopefully enough to serve any read request.
*/
uc dummyRes[1024];

/*
    a read callback function
*/
uc * dummyRead (int area, int DBnumber, int start, int len, int * res) {
    dummyRes[3]+=5;
    dummyRes[0]+=5;
#if debug>1    
    printf("User callback should deliver pointer to %d bytes from %s %d beginning at %d.\n",
	len, daveAreaName(area),DBnumber,start);
#endif	
    *res=0;	
    
    return dummyRes;
};

void dummyWrite(int area, int DBnumber, int start, int len, int * res, uc*buffer) {
//    dummyRes[2]++;
    start/=8;
#if debug>1        
    printf("User write callback should write %d bytes from %s %d to address %d.\n",
	len, daveAreaName(area),DBnumber,start);
#endif	
    *res=0;	
    memcpy(dummyRes+start,buffer,len);
};

int handleSystemMessage(PDU * p1,PDU * p2) {
    int number=0x100*(p1->data[4])+p1->data[5];
    int count=0x100*(p1->data[6])+p1->data[7];
    printf("SysMessage number: %d\n",number);
    printf("count: %d\n",count);
    if (number==20) {
/*
    Info about CPU resources. I return some strange values in order 
    to watch which number goes where.
*/    
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x03, 0x00,0x00,0x00,0x00,}; 
	uc va[]={
//	        FF,09,00,50,
	        00,0x14,00,00,
    00,0x08,00,07,
    0,1,0,1,0,0x81,00,00,	// 129 input bytes
    0,2,0,1,0,0x7f,00,00,	// 127 output bytes
    0,3,0,1,0x0a,00,00,0x80,	// 10*32 flag bytes
    00,04,00,01,00,0x80,00,00, 	 // 128 Timer
    00,05,00,01,00,0x40,00,0x08, // 64 counter
    
    00,06,00,01,05,01,00,00,	// 5*256+1=1281 log.Address
    00,07,00,01,06,03,00,00,	// 6*256+3=1539 local data
    00,0x08,00,01,01,00,00,0x10,
    00,0x09,00,01,00,01,00,00,

	};    
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
//        _daveAddData(p2, p1->data, 8);
//	p1->data[3]=4;	// 4 bytes have been already copied to result string
//	_daveAddValue(p2, va, sizeof(va));    
	_daveAddUserData(p2, va, sizeof(va));    
	_daveDumpPDU(p2);    
	return 1;
    } else if (number==292) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x03, 0x00,0x00,0x00,0x00,}; 
	uc va[]={
	    0x00,0x14,0x00,0x00,0x43,0x02,0xff,0x68, 0xc7,0x00,0x00,0x00,
	    0x08,0x10,0x77,0x10,0x03,0x07,0x10,0x13, 0x22,0x26,0x58,0x85,

	};    
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, p1->data, 8);
	p1->data[3]=4;	// 4 bytes have been copied to result string
	_daveAddValue(p2, va, sizeof(va));    
	_daveDumpPDU(p2);    
	return 1;
    } else if (number==306) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	uc va306[]={
	        0,40,0,1,0,4,0, 1, 0,0,0,
		1, 0,2,0,0,0,0,86,86,0,0,
		0, 0,0,0,0,0,0, 0, 0,0,0,
	        0, 0,0,0,0,0,0, 0, 0,0,0,
	};    
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, p1->data, 8);
	p1->data[3]=4;	// 4 bytes have been copied to result string
	_daveAddValue(p2, va306, sizeof(va306));    
	_daveDumpPDU(p2);    
	return 1;
    } else if (number==273) {	/* read order code (MLFB) */
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x03, 0x00,0x00,0x00,0x00,}; 
	uc va273[]={
	    0x00,0x1c,0x00,0x01,0x43,0x02,
	    '6','E','S', 
//	    '7',' ','3','1','5','-','2','A','F','0','3','-','0','A','B','0',
	    '7',' ','L','I','B','N','O','D','A','V','E','-','T','.','H','.',
	    ' ',0x00,0xc0,0x00,0x02,0x00,0x00,
	};    
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, p1->data, 8);
	p1->data[3]=4;	// 4 bytes have already been copied to result string
	_daveAddValue(p2, va273, sizeof(va273));    
	_daveDumpPDU(p2);    
	return 1;	
    } else if (number==305) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	uc va306[]={
//	        0,40,0,1,0,4,0, 1, 0,0,0,
//		1, 0,2,0,0,0,0,86,86,0,0,
//		0, 0,0,0,0,0,0, 0, 0,0,0,
//	        0, 0,0,0,0,0,0, 0, 0,0,0,
		0, 40,  0,  1,  0,  2,190,253, 15,  0,
		0,  0,  0,  0,  0,  0,  0,  0, 60,  1,
		0,  0,	0,  0,125,  0,  0,  5,  3,  4, 
		0,  0,  0,  0,  0, 12,  0, 10,  0,  0,
		0,  9,  0,  0,
	};    
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, p1->data, 8);
	p1->data[3]=4;	// 4 bytes have been copied to result string
	_daveAddValue(p2, va306, sizeof(va306));    
	_daveDumpPDU(p2);    
	return 1;	
    } else if (number==0) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	uc va[]={
//	255,9,0,114,0,0,0,0,
	0,2,0,53,0,0,15,0,0,17,1,17,15,17,0,18,
	1,18,15,18,0,19,0,20,0,21,1,21,0,23,1,23,15,
23,0,24,1,24,15,24,0,25,15,25,15,26,15,27,0,26,0,
27,0,33,10,33,15,33,2,34,0,35,15,35,0,36,1,36,
4,36,5,36,1,49,1,50,2,50,0,116,1,116,15,116,12,
145,13,145,10,145,0,146,2,146,6,146,15,146,0,177,0,178,0,
179,0,180,0,160,1,160,12,139,126,
	};    
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, p1->data, 8);
	p1->data[3]=4;	// 4 bytes have been copied to result string
	_daveAddValue(p2, va, sizeof(va));    
	_daveDumpPDU(p2);    
	return 1;	
    } else if (number==1060) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	uc va[]={
//	255,9,0,114,0,0,0,0,
	0,20,0,1,81,68,255,8,0,0,0,0,0,0,0,0,148,1,2,22,83,88,104,17,
	};    
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, p1->data, 8);
	p1->data[3]=4;	// 4 bytes have been copied to result string
	_daveAddValue(p2, va, sizeof(va));    
	_daveDumpPDU(p2);    
	return 1;		
    } else 
    return 0;
};

int gpacketNumber=0;

typedef struct{
    uc prot;
    uc ch1;
    uc ch2;
    uc len;
    uc xxxx1;
    uc func;
    uc xxxx2;
} ISOpacket;

void analyze(daveConnection * dc) {
    ISOpacket * p,* p2;
    uc resp[2000];
    uc r5[]={ 
	    0x03,0x00,0x00,0x16,
	    0x11,0xd0,
	    0,0,0,1,0,
	    0xc0,1,9,
	    0x0c1,2,1,2,
	    0x0c2,2,1,0,
	    };
	
    int haveResp=0;
    PDU p1,pr;
    p= (ISOpacket*) dc->msgIn;
    dc->needAckNumber=-1;		// Assume no ack
#if debug>1    
    printf("Protocol: %d\n",p->ch1);
    printf("Channel: %d\n",p->ch1);
    printf("Channel: %d\n",p->ch2);
    printf("Length:  %d\n",p->len);
    printf("(MPI) func:%d\n",p->func);
#endif    
    if (p->func==0xf0) {
#if debug>1        
        printf("0xf0:PDU transport\n");
#endif	
//        p1.header=((uc*)p)+sizeof(ISOpacket);
	dc->PDUstartI=sizeof(ISOpacket);
        _daveSetupReceivedPDU(dc, &p1);
#if debug>1    		
        _daveDumpPDU(&p1);
#endif	
        pr.header=resp+sizeof(ISOpacket);
        p2= (ISOpacket*) resp;
        p2->ch1=p->ch2;
        p2->ch2=p->ch1;
        if (p1.param[0]==daveFuncRead) {
	    _daveHandleRead(&p1,&pr);
	    haveResp=1;
	    p2->len=pr.hlen+pr.plen+pr.dlen+7;
	} else if (p1.param[0]==daveFuncWrite) {
	    _daveHandleWrite(&p1,&pr);
	    haveResp=1;
	    p2->len=pr.hlen+pr.plen+pr.dlen+7;
	} else if (p1.param[0]==240) {
	    printf("PDU function code: %d, negociate PDU len\n",p1.param[0]);
	    _daveDump("packet:",dc->msgIn,dc->msgIn[3]);
	    memcpy(resp, dc->msgIn, dc->msgIn[3]);
	    resp[23]=960 / 0x100;
	    resp[24]=960 % 0x100;
	    haveResp=1;
	} else if (p1.param[0]==0) {
	    printf("PDU function code: %d, system Message ?\n",p1.param[0]);
	    haveResp=handleSystemMessage(&p1,&pr);
	    pr.header[4]=p1.header[4];		// give the PDU a number
	    haveResp=1;
	    p2->len=pr.hlen+pr.plen+pr.dlen+7;
	} else {
	    printf("Unsupported PDU function code: %d\n",p1.param[0]);
	}
    }    
    if (p->func==0xe0) {
	int rack=*(dc->msgIn+17)-1;
	int slot=*(dc->msgIn+18);
        printf("Connect to rack:%d slot:%d \n", rack, slot);
	
        *(r5+16)=rack+1;
	*(r5+17)=slot;    
	memcpy(resp, r5, sizeof(r5));
        haveResp=1;
    }    
    if (haveResp) {
// simulating CP response delay:    
	usleep(10000);
//#define sim_broken_transport	
#undef sim_broken_transport	
#ifdef sim_broken_transport
	double pr=random();
	pr/=RAND_MAX;
	
        if (pr>=0.90) {
	    LOG2("faking broken transport: %0.2f >= 0.95 \n",pr);
	} else   
	    write(dc->iface->fd.wfd,resp,resp[3]);
#else	    
	write(dc->iface->fd.wfd,resp,resp[3]);
#endif		    	
#if debug >1	
	_daveDump("I send:",resp,resp[3]);    
#endif	
    }	
};

typedef struct _portInfo {
    int fd;
}portInfo;

#define mymemcmp _daveMemcmp
void *portServer(void *arg)
{
    _daveOSserialType s;
    daveInterface * di;
    daveConnection * dc;
    int waitCount,res,pcount;
    portInfo * pi=(portInfo *) arg;
    LOG2("portServer: My fd is:%d\n", pi->fd);
    FLUSH;
    waitCount = 0;
    daveSetDebug(0);//daveDebugAll;
    pcount=0;
    
    s.rfd=pi->fd;
    s.wfd=pi->fd;
    di =daveNewInterface(s,"IF1",0,daveProtoISOTCP,daveSpeed187k);
    di->timeout=900000;
    dc=daveNewConnection(di,2,0,2);
    while (waitCount < 100) {
	dc->AnswLen=_daveReadISOPacket(dc->iface, dc->msgIn);
	if (dc->AnswLen>0) {
	    res=dc->AnswLen;
#if debug>1    	    
	    LOG2( "%d ", pcount);		
	    _daveDump("packet", dc->msgIn, dc->AnswLen);
#endif	    
	    waitCount = 0;
	    analyze(dc);
	    pcount++;
	} else {
	    waitCount++;
	}    
    }
    LOG1( "portServer: I closed my fd.\n");
    FLUSH;
    return NULL;
}


/*
    This waits in select for a file descriptor from accepter and starts a new child server
    with this file descriptor.
*/

int main(int argc, char **argv)
{
//    int PID=getpid();
    portInfo pi;
    int filedes[2], res, newfd;
    fd_set FDS;
    char * s2;
    pthread_attr_t attr;
    pthread_t ac, ps;
    accepter_info ai;
    
    printf("CP-Simulation for ISO over TCP\n");
    if (argc<2) {
	printf("Assuming standard port (102) for ISO over TCP\n");
	printf("If you don't want that do: isotest4 xxx, and I will listen on xxx.\n\n");
	s2 = "102 ";
    } else
	s2 = argv[1];
    printf("Assuming rack 0, slot 2 for simulated device. There's currently no way to change that.\n\n");
    readCallBack=dummyRead;
    writeCallBack=dummyWrite;

    
    pipe(filedes);
    ai.port = atol(s2);
    LOG2( "Main serv: port: %d\n", ai.port);
    ai.fd = filedes[1];
    LOG2( "Main serv: Accepter pipe fd: %d\n", ai.fd);
    
    pthread_attr_init(&attr);
    pthread_attr_setdetachstate(&attr,PTHREAD_CREATE_DETACHED);
    res=pthread_create(&ac, &attr, accepter, &ai /*&filedes[1] */ );
    do {
	FD_ZERO(&FDS);
	FD_SET(filedes[0], &FDS);

	LOG2( "Main serv: about to select on %d\n",
	       filedes[0]);
	FLUSH;
	if (select(filedes[0] + 1, &FDS, NULL, &FDS, NULL) > 0) {
	    LOG1( "Main serv: about to read\n");
	    res = read(filedes[0], &pi.fd, sizeof(pi.fd));
	    ps=0;		   
	    pthread_attr_init(&attr);
	    pthread_attr_setdetachstate(&attr,PTHREAD_CREATE_DETACHED);
	    res=pthread_create(&ps, &attr, portServer, &pi);
	    if(res) {
		LOG2( "Main serv: create error:%s\n", strerror(res));		   
		close(newfd);
		usleep(100000);
	    }	   
	}
    }
    while (1);
    return 0;
}


/*
    Changes:
    
    14/07/2003 give a hint about usage
    08/04/2004	Logging for multithreaded things now uses PID, which is set in thread creation
*/
