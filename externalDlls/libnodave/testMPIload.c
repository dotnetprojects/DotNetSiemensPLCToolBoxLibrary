/*
 Test and demo program for Libnodave, a free communication libray for Siemens S7.
 
 **********************************************************************
 * WARNING: This and other test programs overwrite data in your PLC.  *
 * DO NOT use it on PLC's when anything is connected to their outputs.*
 * This is alpha software. Use entirely on your own risk.             * 
 **********************************************************************
 
 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2002, 2003.

 This is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 This is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include "nodave.h"
#include "setport.h"
#include <fcntl.h>
#include "log2.h"

#ifdef LINUX
#include <unistd.h>
#define UNIX_STYLE
#endif

#ifdef BCCWIN
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <time.h>
#define WIN_STYLE
#endif

void usage(void)
{
    printf("Usage: testMPIload [-d] [-w] <serial port> <block file name>.\n");
    printf("-d will produce a lot of debug messages.\n");
    printf("--mpi=<number> will use number as the MPI adddres of the PLC. Default is 2.\n");
    printf("--debug=<number> will set daveDebug to number.\n");
    printf("Example: testMPIload /dev/ttyS0 OB1.mc7\n");
}

void wait() {
    uc c;
#ifdef UNIX_STYLE
    printf("Press return to continue.\n");
    read(0,&c,1);
#endif    
//#ifdef WIN_STYLE
//    printf("Press return to continue.\n");
//    scanf("\n");  //somebody else will find out how to read a single CR in win32...
//#endif    
}    

//#define maxPBlockLen 0xDa	// real maximum 218 bytes
#define maxPBlockLen 0xDe	// real maximum 222 bytes

//#define maxPBlockLen 90	// maximum for CPU 212

int main(int argc, char **argv) {
    int i, adrPos,
	initSuccess,
	useProto, speed, localMPI, plcMPI;
    int number, blockCont;
#ifdef UNIX_STYLE    
    int fd, res;
#endif        
#ifdef WIN_STYLE    
    HANDLE fd;
    unsigned long res;
#endif    
    int blockNumber,rawLen,netLen;
    char blockType;
    daveInterface * di;
    daveConnection * dc;
    _daveOSserialType fds;
    PDU p,p2;

    uc pup[]= {			// Load request
	0x1A,
	0,1,0,0,0,0,0,9,
	0x5F,0x30,0x42,0x30,0x30,0x30,0x30,0x34,0x50, // block type code and number
//	  _    0    B   0     0    0    0    4    P
//		   SDB		
	0x0D,
	0x31,0x30,0x30,0x30,0x32,0x30,0x38,0x30,0x30,0x30,0x31,0x31,0x30, // file length and netto length
//	  1   0     0    0    2    0    8    0    0    0    1    1    0
	0 //extra byte
    };

    uc pablock[]= {	// parameters for parts of a block
	0x1B,0
    };
    
    uc progBlock[1260]= {
	0,maxPBlockLen,0,0xFB,	// This seems to be a fix prefix for program blocks
    };
    
    uc paInsert[]= {		// sended after transmission of a complete block,
				// I this makes the CPU link the block into a program
	0x28,0x00,0x00,0x00, 
	0x00,0x00,0x00,0xFD, 
	0x00,0x0A,0x01,0x00, 
	0x30,0x42,0x30,0x30,0x30,0x30,0x34,0x50, // block type code and number	
        0x05,'_','I','N','S','E',
    };

    adrPos=1;

    daveSetDebug(daveDebugPrintErrors);
    useProto=daveProtoMPI;
    speed=daveSpeed187k;
    localMPI=0;
    plcMPI=2;
    
    if (argc<3) {
	usage();
	exit(-1);
    }    

    while (argv[adrPos][0]=='-') {
	if (strncmp(argv[adrPos],"--debug=",8)==0) {
	    daveSetDebug(atol(argv[adrPos]+8));
	    printf("setting debug to: 0x%lx\n",atol(argv[adrPos]+8));
	} else if (strcmp(argv[adrPos],"-d")==0) {
	    daveSetDebug(daveDebugAll);
	} else if (strncmp(argv[adrPos],"--local=",8)==0) {
	    localMPI=atol(argv[adrPos]+8);
	    printf("setting local MPI address to:%d\n",localMPI);
	} else if (strncmp(argv[adrPos],"--mpi=",6)==0) {
	    plcMPI=atol(argv[adrPos]+6);
	    printf("setting MPI address of PLC to:%d\n",plcMPI);
	} else if (strcmp(argv[adrPos],"-2")==0) {
	    useProto=daveProtoMPI2;
	} else if (strcmp(argv[adrPos],"-9")==0) {
 	    speed=daveSpeed9k;
 	} else if (strcmp(argv[adrPos],"-19")==0) {
 	    speed=daveSpeed19k;
 	} else if (strcmp(argv[adrPos],"-45")==0) {
 	    speed=daveSpeed45k;
 	} else if (strcmp(argv[adrPos],"-93")==0) {
 	    speed=daveSpeed93k;
 	} else if (strcmp(argv[adrPos],"-500")==0) {
 	    speed=daveSpeed500k;
 	} else if (strcmp(argv[adrPos],"-1500")==0) {
 	    speed=daveSpeed1500k;
 	} 
 	
	adrPos++;
	if (argc<=adrPos+1) {
	    usage();
	    exit(-1);
	}	
    }    
    initSuccess=0;
    fds.rfd=setPort(argv[adrPos],"38400",'O');
    fds.wfd=fds.rfd;
    if (fds.rfd>0) { 
	di =daveNewInterface(fds,"IF1", localMPI, useProto, speed);
	for(i=0;i<=3;i++){
	    if(0==daveInitAdapter(di)) {
		initSuccess=1;
		break;
	    } else daveDisconnectAdapter(di);
	}    
	if (!initSuccess) {
	    printf("Couldn't connect to Adapter!.\n Please try again. You may also try the option -2 for some adapters.\n");	
	    return -3;
	}
	initSuccess=0;
	dc =daveNewConnection(di,plcMPI,0,0);  // insert your MPI address here
	for(i=0;i<=3;i++){
	    if (0==daveConnectPLC(dc)) {
		initSuccess=1;
		break;
	    } else  daveDisconnectPLC(dc);
	}
	if (initSuccess) {
	printf("About to open: %s\n",argv[adrPos+1]);
#ifdef UNIX_STYLE
	fd=open(argv[adrPos+1],O_RDONLY);
	read(fd, progBlock+4, maxPBlockLen);
	close(fd);
#endif	    
#ifdef WIN_STYLE
//	fd=openFile(argv[adrPos+1],O_RDONLY);
	fd = CreateFile(argv[adrPos+1], 
       GENERIC_READ,       
       0, 
       0,
       OPEN_EXISTING,
       FILE_FLAG_WRITE_THROUGH,
       0);	
	printf("fd is: %d. About to read.\n",fd);
	ReadFile(fd, progBlock+4, maxPBlockLen, &res, NULL);
	printf("Read result: %d\n",res);
	CloseHandle(fd);
#endif	    
	blockNumber=daveGetU16from(progBlock+10);
	blockType=progBlock[9];
	rawLen=daveGetU16from(progBlock+14);
	netLen=daveGetU16from(progBlock+38);
	
//	if (daveDebug & daveDebugPDU) {
	    printf("Block number: %d\n",blockNumber);
	    printf("Block type: %0X\n",blockType);
	    printf("Size 1: %d\n",rawLen);
	    printf("Size 2: %d\n",netLen);
	    printf("1%06d%06d\n",rawLen,netLen);
	    printf("0%X%05d\n",blockType,blockNumber);
//	}
	sprintf((char*)pup+10,"0%X%05d",blockType,blockNumber);
	sprintf((char*)paInsert+12,"0%X%05d",blockType,blockNumber);
	pup[17]='P';
	paInsert[19]='P';
	sprintf((char*)pup+19,"1%06d%06d",rawLen,netLen);
	if (daveGetDebug() & daveDebugPDU) {
	    _daveDump("pup:",pup,sizeof(pup)-1);
	}    
		
	p.header=dc->msgOut+dc->PDUstartO;
//	printf("header at %d\n", dc->PDUstartO);
	_daveInitPDUheader(&p, 1);
	_daveAddParam(&p, pup, sizeof(pup)-1);
	if (daveGetDebug() & daveDebugPDU) {
	    _daveDumpPDU(&p);
	}

	res=_daveExchange(dc, &p);
	if (res==daveResOK) {
	    res=_daveSetupReceivedPDU(dc, &p2);
	    if (daveGetDebug() & daveDebugPDU) {
		_daveDumpPDU(&p2);
	    }
	    res=daveGetPDUerror(&p2);
	    printf("load request:%04X\n",res);
	    if (res==0) {
#ifdef UNIX_STYLE	    
		fd=open(argv[adrPos+1],O_RDONLY);
#endif	    
#ifdef WIN_STYLE
		fd = CreateFile(argv[adrPos+1],
		    GENERIC_READ,       
		    0,
	    	    0,
	    	    OPEN_EXISTING,
	    	    FILE_FLAG_WRITE_THROUGH,
	    	    0);
		printf("fd is: %d\n",fd);   
#endif
	        blockCont=1;
		res=daveGetResponse(dc);
	        res=_daveSetupReceivedPDU(dc, &p2);
		do {
	    	    res=_daveSetupReceivedPDU(dc, &p2);
	    	    if (daveGetDebug() & daveDebugPDU) {
			_daveDumpPDU(&p2);
		    }
		    number=((PDUHeader*)p2.header)->number;
		    if (p2.param[0]==0x1B) {
			if (daveGetDebug() & daveDebugPDU) {
		    	    printf("Beginning block transmission\n");
		        }    
#ifdef UNIX_STYLE
    			res=read(fd, progBlock+4, maxPBlockLen);
#endif			
#ifdef WIN_STYLE
			ReadFile(fd, progBlock+4, maxPBlockLen, &res, NULL);
			printf("Read result: %d\n",res);
#endif			
			p.header=dc->msgOut+dc->PDUstartO;
			_daveInitPDUheader(&p, 3);
			if (res==maxPBlockLen) pablock[1]=1;	//more blocks
			else {
			    pablock[1]=0;	//last block
			    blockCont=0;
			}    
			progBlock[1]=res;
			_daveAddParam(&p, pablock, sizeof(pablock));
			_daveAddData(&p, progBlock, res+4 /*sizeof(progBlock)*/);
			if (daveGetDebug() & daveDebugPDU) {
			    _daveDumpPDU(&p);
			}
			((PDUHeader*)p.header)->number=number;
			_daveExchange(dc,&p);
//			if (daveDebug & daveDebugPDU) {
			    printf("Block sent.\n");
//			}    			
		    }    
	    } while (blockCont);    
#ifdef UNIX_STYLE
	    close(fd);
#endif	    
#ifdef WIN_STYLE
	    CloseHandle(fd);
#endif	    
            res=_daveSetupReceivedPDU(dc, &p2);
            if (daveGetDebug() & daveDebugPDU) {
	        _daveDumpPDU(&p2);
	    }
	    number=((PDUHeader*)p2.header)->number;
	    if (p2.param[0]==0x1C) {
	        printf("Got end of block transmission\n");	
	        p.header=dc->msgOut+dc->PDUstartO;
//		printf("1\n");	
		_daveInitPDUheader(&p, 3);
//		printf("2\n");	
		_daveAddParam(&p, p2.param,1);
//		printf("3\n");	
		if (daveGetDebug() & daveDebugPDU) {
		    _daveDumpPDU(&p);
		}
//		printf("4\n");	
		((PDUHeader*)p.header)->number=number;
//		_daveExchange(dc,&p);
	        daveSendMessage(dc,&p);
		printf("Answer to block end sended.\n");
		p.header=dc->msgOut+dc->PDUstartO;
		_daveInitPDUheader(&p, 1);
		_daveAddParam(&p, paInsert, sizeof(paInsert));
		res=_daveExchange(dc, &p);
		res=_daveSetupReceivedPDU(dc, &p2);
		res=daveGetPDUerror(&p2);
		printf("block insert:%04X\n",res);
	    }
	    } else {
		printf("CPU doesn't accept load request:%04X = %s\n",res, daveStrerror(res));
	    }	
	}
/*	
	Just to test whether connection to PLC is still ok:
*/
/*
	printf("Trying to read 64 bytes (32 words) from flags.\n");
	wait();
        res=daveReadBytes(dc,daveFlags,0,0,64,NULL);
	if (res==0) {
	    a=daveGetU16(dc);
	    printf("FW0: %d\n",a);
	    a=daveGetU16(dc);
	    printf("FW2: %d\n...\n",a);
	}
*/	
	
        daveDisconnectPLC(dc);
	} else {
	    printf("Couldn't connect to PLC\n");	
	}    
	daveDisconnectAdapter(di);
    } else
	printf("Couldn't open serial port %s\n",argv[adrPos]);	
    return 0;	
}

/*
    04/09/05  removed CYGWIN defines. As there were no more differences against LINUX, it should 
	      work with LINUX defines.
*/
