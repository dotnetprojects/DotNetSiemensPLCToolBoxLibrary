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

#ifdef LINUX
#include <unistd.h>
#include <sys/time.h>
#include <fcntl.h>
#define UNIX_STYLE
#endif

#ifdef BCCWIN
#include <time.h>
    void usage(void);
    void wait(void);
#define WIN_STYLE
#endif

void usage()
{
    printf("Usage: testAS511 [-d] [-w] serial port.\n");
    printf("-w will try to write to Flag words. It will overwrite FB0 to FB15 (MB0 to MB15) !\n");
    printf("-d will produce a lot of debug messages.\n");
    printf("-b will run benchmarks. Specify -b and -w to run write benchmarks.\n");
    printf("-c will write 0 to the PLC memory used in write tests.\n");
    printf("-n will test newly added functions.\n");
    printf("-r tries to put the PLC in run mode.\n");
    printf("-s stops the PLC.\n");
    printf("-t reads time from PLC clock.\n");
    printf("--list show program and data blocks in PLC.\n");
    printf("--listall show program and data blocks in PLC. Includes SFBs and SFCs\n");
    printf("--readout read program and data blocks from PLC.\n");
    printf("--readoutall reads all program and data blocks from PLC. Includes SFBs and SFCs.\n");
    printf("--sync sets the PLC's clock to PC time.\n");
    printf("--many=<DB number> reads 2000 bytes from the given DB using daveReadManyBytes().\n");
    printf("  The DB should exist and be long enough.\n");
    printf("--debug=<number> will set daveDebug to number.\n");
    printf("--dump read out 64k of PLC memory.\n");
    printf("--readDB=<DB number> reads length (default 10) bytes from the given DB.\n");
    printf("--length=<number> sets read length to number.\n");
    printf("  The DB should exist and be long enough.\n");
#ifdef UNIX_STYLE
    printf("Example: testAS511 -w /dev/ttyS0\n");
#endif    
#ifdef WIN_STYLE    
    printf("Example: testAS511 -w COM1\n");
#endif    
}

void wait() {
    uc c;
#ifdef UNIX_STYLE
    printf("Press return to continue.\n");
    read(0,&c,1);
#endif    
#ifdef WIN_STYLE
    printf("Press return to continue.\n");
    getch();
#endif    
//#ifdef WIN_STYLE
//    printf("Press return to continue.\n");
//    scanf("\n");  //somebody else will find out how to read a single CR in win32...
//#endif    
}    

void loadBlocksOfType(daveConnection * dc, int blockType, int doReadout) {
    int j, i, uploadID, len, more;
#ifdef UNIX_STYLE
    int fd;
#endif	        
#ifdef WIN_STYLE	    
    HANDLE fd;
    unsigned long res;
#endif	        
    char blockName [20];
    uc blockBuffer[20000],*bb;
    daveBlockEntry dbe[256];   
    j=daveListBlocksOfType(dc, blockType, dbe);
    if (j<0) {
	printf("error %d=%s\n",-j,daveStrerror(-j));
	return;
    }
    printf("%d blocks of type %s\n",j,daveBlockName(blockType));
    
    for (i=0; i<j; i++) {
	printf("%s%d  %d %d\n",
	    daveBlockName(blockType),
	    dbe[i].number, dbe[i].type[0],dbe[i].type[1]);	
	bb=blockBuffer;
	if(doReadout) {	
	len=0;
	if (0==initUpload(dc, blockType, dbe[i].number, &uploadID)) {
    	    do {
		doUpload(dc,&more,&bb,&len,uploadID);
	    } while (more);
	    sprintf(blockName,"%s%d.mc7",daveBlockName(blockType), dbe[i].number);	
#ifdef UNIX_STYLE
    	    fd=open(blockName,O_RDWR|O_CREAT|O_TRUNC,0644);
    	    write(fd, blockBuffer, len);
    	    close(fd);
#endif	    
#ifdef WIN_STYLE
	    fd = CreateFile(blockName,
    	      GENERIC_WRITE, 0, 0, 2,
    		FILE_FLAG_WRITE_THROUGH, 0);
    	    WriteFile(fd, blockBuffer, len, &res, NULL);
    	    CloseHandle(fd);
#endif	    
    	    endUpload(dc,uploadID);
	} 
    }	
    }
}

void getBlockHeadersOfType(daveConnection * dc, int blockType) {
    int j, i;
    daveBlockEntry dbe[256]; 
    j=daveListBlocksOfType(dc, blockType, dbe);
    if (j<0) {
	printf("error %d = %s\n",-j,daveStrerror(-j));
	return;
    }
    printf("%d blocks of type %s\n",j,daveBlockName(blockType));
    
    for (i=0; i<j; i++) {
	printf("%s%d  %d %d\n",
	    daveBlockName(blockType),
	    dbe[i].number, dbe[i].type[0],dbe[i].type[1]);	
	daveGetBlockInfo(dc, NULL, blockType,dbe[i].number);
	_daveDump("header",dc->_resultPointer,78);
    }	
}

#include "benchmark.c"

int main(int argc, char **argv) {
    int i, a,b,c, adrPos, doWrite, doBenchmark, 
	doClear, doNewfunctions, doWbit,
	initSuccess, doRun, doStop, doReadout, doList, doListall, doSFBandSFC,
	doSync, doReadTime, doGetHeaders, doTestMany, doDump, aLongDB,
	res, speed, localMPI, plcMPI, plc2MPI, wbit, doReadDB, dbReadLen;
    PDU p;	
    float d;
    uc * buffer;
    daveInterface * di;
    daveConnection * dc;
    _daveOSserialType fds;
    daveResultSet rs;
    
    adrPos=1;
    doWrite=0;
    doBenchmark=0;
    doClear=0;
    doNewfunctions=0;
    doWbit=0;
    doRun=0;
    doStop=0;
    doReadout=0;
    doList=0;
    doListall=0;
    doSFBandSFC=0;
    doSync=0;
    doReadTime=0;
    doGetHeaders=0;
    doTestMany=0;
    doDump=0;
    doReadDB=-1;
    dbReadLen=10;
    
    speed=daveSpeed187k;
    localMPI=0;
    plcMPI=2;
    plc2MPI=-1;
    
    if (argc<2) {
	usage();
	exit(-1);
    }    

    while (argv[adrPos][0]=='-') {
	if (strncmp(argv[adrPos],"--debug=",8)==0) {
	    daveSetDebug(atol(argv[adrPos]+8));
	    printf("setting debug to: 0x%lx\n",atol(argv[adrPos]+8));
	} else if (strcmp(argv[adrPos],"-d")==0) {
	    daveSetDebug(daveDebugAll);
	} else
	if (strcmp(argv[adrPos],"-s")==0) {
	    doStop=1;
	} else
	if (strcmp(argv[adrPos],"-t")==0) {
	    doReadTime=1;
	} else
	if (strcmp(argv[adrPos],"-r")==0) {
	    doRun=1;
	} else
	if (strcmp(argv[adrPos],"-w")==0) {
	    doWrite=1;
	} else
	if (strcmp(argv[adrPos],"-b")==0) {
	    doBenchmark=1;
	} else
	if (strncmp(argv[adrPos],"--readoutall",12)==0) {
	    doReadout=1;
	    doSFBandSFC=1;
	} else
	if (strncmp(argv[adrPos],"--readout",9)==0) {
	    doReadout=1;
	} else if (strncmp(argv[adrPos],"--headers",9)==0) {
	    doGetHeaders=1;
	} else if (strncmp(argv[adrPos],"--dump",6)==0) {
	    doDump=1;
	} else if (strncmp(argv[adrPos],"--many=",7)==0) {
	    aLongDB=atol(argv[adrPos]+7);
	    doTestMany=1;  
	} else if (strncmp(argv[adrPos],"--listall",9)==0) {
	    doListall=1;
	    doList=1;
	} else if (strncmp(argv[adrPos],"--list",6)==0) {
	    doList=1;
	} else if (strncmp(argv[adrPos],"--local=",8)==0) {
	    localMPI=atol(argv[adrPos]+8);
	    printf("setting local MPI address to:%d\n",localMPI);
	} else if (strncmp(argv[adrPos],"--length=",9)==0) {
	    dbReadLen=atol(argv[adrPos]+9);
	    printf("setting read length to:%d\n",dbReadLen);
	} else if (strncmp(argv[adrPos],"--readDB=",9)==0) {
	    doReadDB=atol(argv[adrPos]+9);
	    printf("setting DB number to read from to:%d\n",doReadDB);
	} else if (strncmp(argv[adrPos],"--sync",6)==0) {
	    doSync=1;
	} else
	if (strncmp(argv[adrPos],"--wbit=",7)==0) {
	    wbit=atol(argv[adrPos]+7);
	    printf("setting bit number:%d\n",wbit);
	    doWbit=1;
	} else if (strcmp(argv[adrPos],"-c")==0) {
	    doClear=1;
	}
	else if (strcmp(argv[adrPos],"-n")==0) {
	    doNewfunctions=1;
	}
	adrPos++;
	if (argc<=adrPos) {
	    usage();
	    exit(-1);
	}	
    }    
    
    fds.rfd=setPort(argv[adrPos],"9600",'E');
    fds.wfd=fds.rfd;
    initSuccess=0;	
    if (fds.rfd>0) { 
	di =daveNewInterface(fds, "IF1", localMPI, daveProtoAS511, speed);
	daveSetTimeout(di,500000);
	for (i=0; i<3; i++) {
	    if (0==daveInitAdapter(di)) {
		initSuccess=1;	
		break;	
	    } else daveDisconnectAdapter(di);
	    
	}
	if (!initSuccess) {
	    printf("Couldn't connect to Adapter!.\n Please try again. You may also try the option -2 for some adapters.\n");	
	    return -3;
	}
	dc =daveNewConnection(di,plcMPI,0,0);
	printf("ConnectPLC\n");
	if (0==daveConnectPLC(dc)) {;
	    if(doWbit) {
		a=1;
		res=daveWriteBits(dc, daveFlags, 0, wbit, 1,&a);
	    }	
/*
 * Some comments about daveReadBytes():
 *
 * Here we read flags and not data blocks, because we cannot know which data blocks will
 * exist in a user's PLC, but flags are always present. To read from start of data block 5
 * (DB5.DBB0) use:
 * 	daveReadBytes(dc, daveDB, 5, 0, 16, NULL);
 * to read DBD68 and DBW72 use:
 * 	daveReadBytes(dc, daveDB, 5, 68, 6, NULL);
 * to read DBD68 and DBW72 into your applications buffer appBuffer use:	
 * 	daveReadBytes(dc, daveDB, 5, 68, 6, appBuffer);
 * to read DBD68 and DBD78 into your applications buffer appBuffer use:	
 * 	daveReadBytes(dc, daveDB, 5, 68, 14, appBuffer);
 * this reads DBD68 and DBD78 and everything in between and fills the range
 * appBuffer+4 to appBuffer+9 with unwanted bytes, but is much faster than:
 *	daveReadBytes(dc, daveDB, 5, 68, 4, appBuffer);
 *	daveReadBytes(dc, daveDB, 5, 78, 4, appBuffer+4);
 */	
	res=daveReadBytes(dc, daveFlags, 0, 0, 16,NULL);
/*
 *	daveGetU32(dc); reads a word (2 bytes) from the current buffer position and increments
 *	an internal pointer by 2, so next daveGetXXX() wil read from the new position behind that
 *	word.	
 */	
	if (res==0) {
    	    a=daveGetS32(dc);	
    	    b=daveGetS32(dc);
    	    c=daveGetS32(dc);
	    d=daveGetKG(dc);
	    printf("FD0: %d\n", a);
	    printf("FD4: %d\n", b);
	    printf("FD8: %d\n", c);
	    printf("FD12: %f\n", d);
	} else 
	    printf("error %d=%s\n", res, daveStrerror(res));

	if (doReadDB>=0) {
	res=daveReadBytes(dc, daveDB, doReadDB, 0, dbReadLen,NULL);
	if (res==0) {
	    _daveDump("bytes from DB:",dc->resultPointer,dbReadLen);
	} else 
	    printf("error %d=%s\n", res, daveStrerror(res));
	}
	if(doNewfunctions) {
//	    saveDebug=daveGetDebug();
	    
	    printf("Trying to read two consecutive bits from DB11.DBX0.1\n");;
	    res=daveReadBits(dc, daveDB, 11, 1, 2,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	    
	    printf("Trying to read no bit (length 0) from DB17.DBX0.1\n");
	    res=daveReadBits(dc, daveDB, 17, 1, 0,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));

//	    daveSetDebug(daveGetDebug()|daveDebugPDU);	
	    printf("Trying to read a single bit from DB17.DBX0.3\n");
	    res=daveReadBits(dc, daveDB, 17, 3, 1,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	
	    printf("Trying to read a single bit from E0.2\n");
	    res=daveReadBits(dc, daveInputs, 0, 2, 1,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	    
	    a=0;
	    printf("Writing 0 to EB0\n");
	    res=daveWriteBytes(dc, daveOutputs, 0, 0, 1, &a);

	    a=1;
	    printf("Trying to set single bit E0.5\n");
	    res=daveWriteBits(dc, daveOutputs, 0, 5, 1, &a);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	
	    printf("Trying to read 1 byte from AAW0\n");
	    res=daveReadBytes(dc, daveAnaIn, 0, 0, 2,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	    
	    a=2341;
	    printf("Trying to write 1 word (2 bytes) to AAW0\n");
	    res=daveWriteBytes(dc, daveAnaOut, 0, 0, 2,&a);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	
	    printf("Trying to read 4 items from Timers\n");
	    res=daveReadBytes(dc, daveTimer, 0, 0, 4,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	    d=daveGetSeconds(dc);
	    printf("Time: %0.3f, ",d);
	    d=daveGetSeconds(dc);
	    printf("%0.3f, ",d);
	    d=daveGetSeconds(dc);
	    printf("%0.3f, ",d);
	    d=daveGetSeconds(dc);
	    printf(" %0.3f\n",d);
	    
	    d=daveGetSecondsAt(dc,0);
	    printf("Time: %0.3f, ",d);
	    d=daveGetSecondsAt(dc,2);
	    printf("%0.3f, ",d);
	    d=daveGetSecondsAt(dc,4);
	    printf("%0.3f, ",d);
	    d=daveGetSecondsAt(dc,6);
	    printf(" %0.3f\n",d);
	    
	    printf("Trying to read 4 items from Counters\n");
	    res=daveReadBytes(dc, daveCounter, 0, 0, 4,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	    c=daveGetCounterValue(dc);
	    printf("Count: %d, ",c);
	    c=daveGetCounterValue(dc);
	    printf("%d, ",c);
	    c=daveGetCounterValue(dc);
	    printf("%d, ",c);
	    c=daveGetCounterValue(dc);
	    printf(" %d\n",c);
	    
	    c=daveGetCounterValueAt(dc,0);
	    printf("Count: %d, ",c);
	    c=daveGetCounterValueAt(dc,2);
	    printf("%d, ",c);
	    c=daveGetCounterValueAt(dc,4);
	    printf("%d, ",c);
	    c=daveGetCounterValueAt(dc,6);
	    printf(" %d\n",c);
	    
	    davePrepareReadRequest(dc, &p);
	    daveAddVarToReadRequest(&p,daveInputs,0,0,1);
	    daveAddVarToReadRequest(&p,daveFlags,0,0,4);
	    daveAddVarToReadRequest(&p,daveDB,6,20,2);
	    daveAddVarToReadRequest(&p,daveTimer,0,0,4);
	    daveAddVarToReadRequest(&p,daveTimer,0,1,4);
	    daveAddVarToReadRequest(&p,daveTimer,0,2,4);
	    daveAddVarToReadRequest(&p,daveCounter,0,0,4);
	    daveAddVarToReadRequest(&p,daveCounter,0,1,4);
	    daveAddVarToReadRequest(&p,daveCounter,0,2,4);
	    
	    res=daveExecReadRequest(dc, &p, &rs);
	    if (doWrite) {
		
	    }
	    
//	    daveSetDebug(saveDebug);
	}

	if(doWrite) {
    	    printf("Now we write back these data after incrementing the integers by 1,2 and 3 and the float by 1.1.\n");
    	    wait();
/*
    Attention! you need to daveSwapIed little endian variables before using them as a buffer for
    daveWriteBytes() or before copying them into a buffer for daveWriteBytes()!
*/	    
    	    a=daveSwapIed_32(a+1);
    	    daveWriteBytes(dc,daveFlags,0,0,4,&a);
    	    b=daveSwapIed_32(b+2);
    	    daveWriteBytes(dc,daveFlags,0,4,4,&b);
    	    c=daveSwapIed_32(c+3);
	    daveWriteBytes(dc,daveFlags,0,8,4,&c);
    	    c=daveToKG(d+1.1);
    	    daveWriteBytes(dc,daveFlags,0,12,4,&c);
/*
 *   Read back and show the new values, so users may notice the difference:
 */	    
    	    daveReadBytes(dc,daveFlags,0,0,16,NULL);
    	    a=daveGetU32(dc);
    	    b=daveGetU32(dc);
    	    c=daveGetU32(dc);
    	    d=daveGetKG(dc);
	    printf("FD0: %d\n",a);
	    printf("FD4: %d\n",b);
	    printf("FD8: %d\n",c);
	    printf("FD12: %f\n",d);
	} // doWrite
	if(doClear) {
    	    printf("Now writing 0 to the bytes FB0...FB15.\n");
    	    wait();
	    a=0;
    	    daveWriteBytes(dc,daveFlags,0,0,4,&a);
    	    daveWriteBytes(dc,daveFlags,0,4,4,&a);
	    daveWriteBytes(dc,daveFlags,0,8,4,&a);
    	    daveWriteBytes(dc,daveFlags,0,12,4,&a);
	    daveReadBytes(dc,daveFlags,0,0,16,NULL);
    	    a=daveGetU32(dc);
    	    b=daveGetU32(dc);
    	    c=daveGetU32(dc);
    	    d=daveGetKG(dc);
	    printf("FD0: %d\n",a);
	    printf("FD4: %d\n",b);
	    printf("FD8: %d\n",c);
	    printf("FD12: %f\n",d);
	} // doClear

	if(doStop) {
	    daveStop(dc);
	}
	if(doRun) {
	    daveStart(dc);
	}
	if(doList||doReadout) { // if readout is not set, loadBlocksOfType will only ist block names
	    daveListBlocks(dc,NULL);
	    loadBlocksOfType(dc, daveBlockType_OB, doReadout);
	    loadBlocksOfType(dc, daveBlockType_FB, doReadout);
	    loadBlocksOfType(dc, daveBlockType_FC, doReadout);
	    loadBlocksOfType(dc, daveBlockType_DB, doReadout);
	    loadBlocksOfType(dc, daveBlockType_SDB, doReadout);
	    if ((doSFBandSFC) ||(doListall)){	    
		loadBlocksOfType(dc, daveBlockType_SFB, doReadout);
	        loadBlocksOfType(dc, daveBlockType_SFC, doReadout);
	    }
	}
	
	if(doGetHeaders) { 
	    daveListBlocks(dc,NULL);
//	    saveDebug=daveGetDebug();
//	    daveSetDebug(daveDebugAll);
	    getBlockHeadersOfType(dc, daveBlockType_OB);
	    getBlockHeadersOfType(dc, daveBlockType_DB);
	    getBlockHeadersOfType(dc, daveBlockType_FC);
	    getBlockHeadersOfType(dc, daveBlockType_FB);
	    getBlockHeadersOfType(dc, daveBlockType_SFC);
	    getBlockHeadersOfType(dc, daveBlockType_SFB);
//	    daveSetDebug(saveDebug);
	}
	    

	
	if(doBenchmark) {
	    rBenchmark(dc, daveFlags);
	    if(doWrite) {
		wBenchmark(dc, daveFlags);
	    } // doWrite
	} // doBenchmark
	
	if(doSync) {
	    res=daveSetPLCTimeToSystime(dc);
	    doReadTime=1;
	    if (res!=0 && res!=10)
		printf("*** Error: %04X = %s\n",res,daveStrerror(res));		
	} // doSync
	
	if(doReadTime) {
	    printf("read time: ");
	    daveReadPLCTime(dc);
	    for (i=0;i<10;i++) {
		a=daveFromBCD(daveGetU8(dc));
		printf("%d ",a);
	    }
	    printf("\n");
	} // doSync
	
	if(doTestMany) {
	    printf("read a long block of bytes from DB%d\n",aLongDB);
	    buffer=(uc*)malloc(2000);
//	    res=daveReadBytes(dc,daveDB,aLongDB,0,2000,buffer);
//	    printf("Result code: %04x=%s\n",res, daveStrerror(res));
	    res=daveReadManyBytes(dc,daveDB,aLongDB,0,2000,buffer);
	    printf("Result code: %04x=%s\n",res, daveStrerror(res));
	} // doTestMany
	
	if(doDump) {
	    printf("read a 64k of raw memory\n");
	    buffer=(uc*)malloc(65536);
//	    res=daveReadBytes(dc,daveDB,aLongDB,0,2000,buffer);
//	    printf("Result code: %04x=%s\n",res, daveStrerror(res));
	    res=daveReadManyBytes(dc,daveRawMemoryS5,0,0,65536,buffer);
	    printf("Result code: %04x=%s\n",res, daveStrerror(res));
	} // doTestMany

	printf("Now disconnecting\n");	
	daveDisconnectPLC(dc);
	daveFree(dc);
	daveDisconnectAdapter(di);
	daveFree(di);
	return 0;
	} else {
	    printf("Couldn't connect to PLC.\n Please try again. You may also try the option -2 for some adapters.\n");	
	    daveDisconnectAdapter(di);
	    daveDisconnectAdapter(di);
	    daveDisconnectAdapter(di);
	    return -2;
	}
    } else {
	printf("Couldn't open serial port %s\n",argv[adrPos]);	
	return -1;
    }	
}

/*
    Changes: 
    07/19/04  removed unused vars.
    09/09/04  applied patch for variable Profibus speed from Andrew Rostovtsew.
    09/09/04  removed unused include byteswap.h
    09/11/04  added multiple variable read example code.
    12/09/04  removed debug printf from arg processing.
    12/12/04  added Timer/counter read functions.
    04/09/05  removed CYGWIN defines. As there were no more differences against LINUX, it should 
	      work with LINUX defines.
    09/11/05  added a test for daveReadManyBytes
*/
