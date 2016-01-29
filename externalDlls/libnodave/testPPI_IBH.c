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

#include "nodavesimple.h"
#include "openSocket.h"

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
    printf("Usage: testPPI_IBH [-d] [-w] IP address.\n");
    printf("-w will try to write to Flag words. It will overwrite FB0 to FB15 (MB0 to MB15) !\n");
    printf("-d will produce a lot of debug messages.\n");
    printf("-b will run benchmarks. Specify -b and -w to run write benchmarks.\n");
    printf("-m will run a test for multiple variable reads.\n");
    printf("-c will write 0 to the PLC memory used in write tests.\n");
    printf("-n will test newly added functions.\n");
    printf("-l will list reachable partners on the network.\n");
    printf("--ppi=<number> will use number as the PPI adddres of the PLC. Default is 2.\n");
    printf("--mpi=<number> will use number as the PPI adddres of the PLC. Default is 2.\n");
    printf("Note: the local PPI Adress of the NetLink can only be set with NetLink configuration software.\n");
    printf("-s stops the PLC.\n");
    printf("-r tries to put the PLC in run mode.\n");
    printf("--readout read program and data blocks from PLC.\n");
    printf("--debug=<number> will set daveDebug to number.\n");
    printf("Example: testPPI_IBH -w 192.168.1.1\n");
}

void wait() {
    uc c;
    printf("Press return to continue.\n");
#ifdef UNIX_STYLE
    read(0,&c,1);
#endif    
}    

void loadBlocksOfType(daveConnection * dc, int blockType) {
    int i,j,uploadID, len, more;
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
	printf("error %d = %s\n",-j,daveStrerror(-j));
	return;
    }
    printf("%d blocks of type %s\n",j,daveBlockName(blockType));
    for (i=0; i<j; i++) {
	printf("%s%d  %d %d\n",
	    daveBlockName(blockType),
	    dbe[i].number, dbe[i].type[0],dbe[i].type[1]);	
	bb=blockBuffer;
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

#include "benchmark.c"

int main(int argc, char **argv) {
    int i,j, a,b,c, adrPos, doWrite, doBenchmark,
	doMultiple, doClear, doNewfunctions, 
	initSuccess, doReadout, doRun, doStop, doList,
	saveDebug,
	res, useProto, plcPPI;
    PDU p;	
    float d;
    char buf1 [davePartnerListSize];
    daveInterface * di;
    daveConnection * dc;
    _daveOSserialType fds;
    daveResultSet rs;
    
    adrPos=1;
    doWrite=0;
    doBenchmark=0;
    doMultiple=0;
    doClear=0;
    doNewfunctions=0;
    doRun=0;
    doStop=0;
    doReadout=0;
    doList=0;
    
    useProto=daveProtoPPI_IBH;
    plcPPI=2;
    
    if (argc<2) {
	usage();
	exit(-1);
    }    

    while (argv[adrPos][0]=='-') {
//	printf("arg %s\n",argv[adrPos]);
	
	if (strcmp(argv[adrPos],"-d")==0) {
	    daveSetDebug(daveDebugAll);
	} else
	if (strcmp(argv[adrPos],"-s")==0) {
	    doStop=1;
	} else
	if (strcmp(argv[adrPos],"-r")==0) {
	    doRun=1;
	} else
	if (strncmp(argv[adrPos],"--readout",9)==0) {
	    doReadout=1;
	} else
	if (strncmp(argv[adrPos],"--debug=", 8)==0) {
	    res=atol(argv[adrPos]+8);
	    printf("setting debug to:%d\n", res);
	    daveSetDebug(res);
	}
	if (strcmp(argv[adrPos],"-w")==0) {
	    doWrite=1;
	} else
	if (strcmp(argv[adrPos],"-b")==0) {
	    doBenchmark=1;
	} else
	if (strcmp(argv[adrPos],"-l")==0) {
	    doList=1;
	} else
	if (strncmp(argv[adrPos],"--mpi=",6)==0) {
	    plcPPI=atol(argv[adrPos]+6);
	    printf("setting PPI address of PLC to:%d\n", plcPPI);
	} else
	if (strncmp(argv[adrPos],"--ppi=",6)==0) {
	    plcPPI=atol(argv[adrPos]+6);
	    printf("setting PPI address of PLC to:%d\n", plcPPI);
	}
	else if (strcmp(argv[adrPos],"-m")==0) {
	    doMultiple=1;
	}
	else if (strcmp(argv[adrPos],"-c")==0) {
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
    
    fds.rfd=openSocket(1099, argv[adrPos]);
    fds.wfd=fds.rfd;
    initSuccess=0;	
    if (fds.rfd>0) { 
	di =daveNewInterface(fds, "IF1", 0, useProto, daveSpeed187k);
	daveSetTimeout(di, 5000000);
	for (i=0; i<3; i++) {
	    if (0==daveInitAdapter(di)) {
		initSuccess=1;	
		if(doList) {
		a= daveListReachablePartners(di,buf1);
		printf("daveListReachablePartners List length: %d\n",a);
		if (a>0) {
		    for (j=0;j<a;j++) {
			if (buf1[j]==daveMPIReachable) printf("Device at address:%d\n",j);
		    }	
		}
		}
		break;	
	    } else daveDisconnectAdapter(di);
	}
	if (!initSuccess) {
	    printf("Couldn't connect to Adapter!.\n Please try again. You may also try the option -2 for some adapters.\n");	
	    return -3;
	}
	dc =daveNewConnection(di, plcPPI, 0, 0);
	printf("ConnectPLC\n");
	if (0==daveConnectPLC(dc)) {
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
	daveReadBytes(dc, daveFlags, 0, 0, 16,NULL);
/*
 *	daveGetU32(dc); reads a word (2 bytes) from the current buffer position and increments
 *	an internal pointer by 2, so next daveGetXXX() wil read from the new position behind that
 *	word.	
 */	
        a=daveGetS32(dc);	
        b=daveGetS32(dc);
        c=daveGetS32(dc);
        d=daveGetFloat(dc);
	printf("FD0: %d\n",a);
	printf("FD4: %d\n",b);
	printf("FD8: %d\n",c);
	printf("FD12: %f\n",d);
/*	
	a=1;
	printf("Trying to set single bit A0.4\n");
	res=daveWriteBits(dc, daveOutputs, 0, 4, 1, &a);
	printf("function result:%d=%s\n", res, daveStrerror(res));
*/	
/*
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
*/
	
	if(doNewfunctions) {
	    saveDebug=daveGetDebug();
	    
	    printf("Trying to read two consecutive bits from DB11.DBX0.1\n");;
	    res=daveReadBits(dc, daveDB, 11, 1, 2,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	    
	    printf("Trying to read no bit (length 0) from DB17.DBX0.1\n");
	    res=daveReadBits(dc, daveDB, 17, 1, 0,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));

	    daveSetDebug(daveGetDebug()|daveDebugPDU);
	    printf("Trying to read a single bit from DB17.DBX0.3\n");
	    res=daveReadBits(dc, daveDB, 17, 3, 1,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	
	    printf("Trying to read a single bit from E0.2\n");
	    res=daveReadBits(dc, daveInputs, 0, 2, 1,NULL);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	    
	    a=0;
	    printf("Writing 0 to EB0\n");
	    res=daveWriteBytes(dc, daveOutputs, 0, 0, 1, &a);
	    if(res)
		printf("Error in writeBytes:%d=%s\n", res, daveStrerror(res));
	    

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
	    if(res==0) {
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
	    }
	    if(res==0) {
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
	    }
	    printf("Trying to read some items from mixed areas.\n");
	    davePrepareReadRequest(dc, &p);
	    daveAddVarToReadRequest(&p,daveInputs,0,0,1);
	    daveAddVarToReadRequest(&p,daveFlags,0,0,4);
	    daveAddVarToReadRequest(&p,daveDB,6,20,2);
	    daveAddVarToReadRequest(&p,daveTimer,0,0,4);
	    daveAddVarToReadRequest(&p,daveTimer,0,1,4);
	    daveAddVarToReadRequest(&p,daveTimer,0,2,4);
	    daveAddVarToReadRequest(&p,daveCounter,0,0,4);
	    daveAddVarToReadRequest(&p,daveCounter,0,1,4);
//	    daveAddVarToReadRequest(&p,daveCounter,0,2,4);
	    res=daveExecReadRequest(dc, &p, &rs);
	    printf("function result:%d=%s\n", res, daveStrerror(res));
	    
	    daveSetDebug(saveDebug);
	}
	
	if(doMultiple) {
    	    printf("Now testing read multiple variables.\n"
		"This will read 1 Byte from inputs,\n"
		"4 bytes from flags, 2 bytes from DB6\n"
		" and other 2 bytes from flags\n");
    	    wait();
	    davePrepareReadRequest(dc, &p);
	    daveAddVarToReadRequest(&p,daveInputs,0,0,1);
	    daveAddVarToReadRequest(&p,daveFlags,0,0,4);
	    daveAddVarToReadRequest(&p,daveDB,6,20,2);
	    daveAddVarToReadRequest(&p,daveFlags,0,12,2);
	    daveAddBitVarToReadRequest(&p, daveFlags, 0, 25 /* 25 is 3.1*/, 1);
	    res=daveExecReadRequest(dc, &p, &rs);
	    if (res==0) {
	    printf("Input Byte 0 ");
	    res=daveUseResult(dc, &rs, 0); // first result
	    if (res==0) {
		a=daveGetU8(dc);
        	printf("%d\n",a);
	    } else 
		printf("*** Error: %s\n",daveStrerror(res));
		
	    printf("Flag DWord 0 ");	
	    res=daveUseResult(dc, &rs, 1); // 2nd result
	    if (res==0) {
		a=daveGetS16(dc);
        	printf("%d\n",a);
	    } else 
		printf("*** Error: %s\n",daveStrerror(res));
		
	    printf("DB 6 Word 20: ");	
	    res=daveUseResult(dc, &rs, 2); // 3rd result
	    if (res==0) {
		a=daveGetS16(dc);
        	printf("%d\n",a);
	    } else 
		printf("*** Error: %s\n",daveStrerror(res));
		
	    printf("Flag Word 12: ");		
	    res=daveUseResult(dc, &rs, 3); // 4th result
	    if (res==0) {
		a=daveGetU16(dc);
        	printf("%d\n",a);
	    } else 
		printf("*** Error: %s\n",daveStrerror(res));	
	    
	    printf("Flag F3.1: ");		
	    res=daveUseResult(dc, &rs, 4); // 4th result
	    if (res==0) {
		a=daveGetU8(dc);
        	printf("%d\n",a);
	    } else 
		printf("*** Error: %s\n",daveStrerror(res));		
		
	    printf("non existing result (we read 5 items, but try to use a 6th one): ");
	    res=daveUseResult(dc, &rs, 5); // 5th result
	    if (res==0) {
		a=daveGetU16(dc);
        	printf("%d\n",a);
	    } else 
		printf("*** Error: %s\n",daveStrerror(res));
	    } else {
		printf("***Error in multiple read: %s\n",daveStrerror(res));
	    }
	    daveFreeResults(&rs);  
	    
	    if (doWrite){
		printf("Now testing write multiple variables:\n"
		    "IB0, FW0, QB0, DB6:DBW20 and DB20:DBD24 in a single multiple write.\n");
		wait();
		daveSetDebug(0xffff);
		a=0;
	        davePrepareWriteRequest(dc, &p);
		daveAddBitVarToWriteRequest(&p, daveFlags, 0, 27 /* 27 is 3.3*/, 1, &a);
		daveAddBitVarToWriteRequest(&p, daveFlags, 0, 17 /* 27 is 2.1*/, 1, &a);
		daveAddVarToWriteRequest(&p,daveInputs,0,0,1,&a);
//		daveAddVarToWriteRequest(&p,daveFlags,0,4,3,&a);
//	        daveAddVarToWriteRequest(&p,daveOutputs,0,0,2,&a);
//		daveAddVarToWriteRequest(&p,daveInputs,0,0,2,&a);
//		daveAddVarToWriteRequest(&p,daveDB,6,20,2,&a);
//		daveAddVarToWriteRequest(&p,daveDB,20,24,4,&a);
		a=0xff;
	    	
		daveAddBitVarToWriteRequest(&p, daveFlags, 0, 7 /* 7 is 0.7 */, 1, &a);
	        daveAddVarToWriteRequest(&p,daveOutputs,0,0,3,&a);
		res=daveExecWriteRequest(dc, &p, &rs);
		printf("Result code for the entire multiple write operation: %d=%s\n",res, daveStrerror(res));
/*
//	I could list the single result codes like this, but I want to tell
//	which item should have been written, so I do it in 5 individual lines:
*/	
		for (i=0;i<rs.numResults;i++){
		    res=rs.results[i].error;
		    printf("result code from writing item %d: %d=%s\n",i,res,daveStrerror(res));
		}
/*		
		if(rs.numResults>0) printf("Result code for writing IB0:        %d=%s\n",rs.results[0].error, daveStrerror(rs.results[0].error));
//		printf("Result code for writing FW4:        %d=%s\n",rs.results[1].error, daveStrerror(rs.results[1].error));
//		printf("Result code for writing QB0:        %d=%s\n",rs.results[2].error, daveStrerror(rs.results[2].error));
//		printf("Result code for writing DB6:DBW20:  %d=%s\n",rs.results[3].error, daveStrerror(rs.results[3].error));
//		printf("Result code for writing DB20:DBD24: %d=%s\n",rs.results[4].error, daveStrerror(rs.results[4].error));
		printf("Result code for writing F3.3: 	    %d=%s\n",rs.results[5].error, daveStrerror(rs.results[5].error));
*/		
/*
 *   Read back and show the new values, so users may notice the difference:
 */	    
	        daveReadBytes(dc,daveFlags,0,0,16,NULL);
    		a=daveGetU32(dc);
	        b=daveGetU32(dc);
    		c=daveGetU32(dc);
    		d=daveGetFloat(dc);
	        printf("FD0: %d\n",a);
		printf("FD4: %d\n",b);
		printf("FD8: %d\n",c);
	        printf("FD12: %f\n",d);		
	    } // doWrite
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
    	    d=toPLCfloat(d+1.1);
    	    daveWriteBytes(dc,daveFlags,0,12,4,&d);
/*
 *   Read back and show the new values, so users may notice the difference:
 */	    
    	    daveReadBytes(dc,daveFlags,0,0,16,NULL);
    	    a=daveGetU32(dc);
    	    b=daveGetU32(dc);
    	    c=daveGetU32(dc);
    	    d=daveGetFloat(dc);
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
    	    d=daveGetFloat(dc);
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
	if(doReadout) {
	    loadBlocksOfType(dc, daveBlockType_OB);
	    loadBlocksOfType(dc, daveBlockType_DB);
	    loadBlocksOfType(dc, daveBlockType_SDB);
	}    

	if(doBenchmark) {
	    rBenchmark(dc, daveDB);
	    if(doWrite) {
		wBenchmark(dc, daveDB);
	    } // doWrite
	} // doBenchmark

	printf("Now disconnecting\n");	
	daveDisconnectPLC(dc);
	daveDisconnectAdapter(di);
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
*/
