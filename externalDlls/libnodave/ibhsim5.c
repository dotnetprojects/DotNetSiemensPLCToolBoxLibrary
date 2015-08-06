/*
 Part of Libnodave, a free communication libray for Siemens S7 300/400.
 This program simulates the IBHLink MPI-Ethernet-Adapter from IBH-Softec.
 www.ibh-softec.de
 
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
#include <stdlib.h>
#include <unistd.h>
#include <string.h>

#include <pthread.h>

#include "log2.h"

#define ThisModule "IBHtest : "
#define uc unsigned char

#include "nodave.h"
#include "ibhsamples6.c"

#include <sys/time.h>
#include <sys/socket.h>

#include <netinet/in.h>
#include <arpa/inet.h>
#include <errno.h>


#include "accepter.c"

#include "simProperties.c"

#define bSize 1256
#define us unsigned short

#define debug 12

extern int daveDebug;
#define daveDebugAnalyze 0x40000

void analyze(daveConnection * dc);
/*
    many bytes. hopefully enough to serve any read request.
*/
uc dummyRes[] = {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,
12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,12,13,14,1,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,
12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,12,13,14,1,
5,16,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,5,16,1,2,3,4,5,6,7,8,9,10,
11,12,13,14,15,16,12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,12,13,14,1,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,
12,13,14,15,16,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,12,13,14,1,
5,16,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,5,16,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,};
/*
    a read callback function
*/
uc * dummyRead (int area, int DBnumber, int start, int len, int * res) {
    printf("User callback should deliver pointer to %d bytes from %s %d beginning at %d.\n",
	len, daveAreaName(area),DBnumber,start);
    *res=0;	
    
    return dummyRes;
};

void myWrite (int area, int DBnumber, int start, int len, int * res, uc*bytes) {
    printf("User callback1 should write %d bytes to %s %d beginning at %d.\n",
	len, daveAreaName(area),DBnumber,start);
    printf("User callback 1.\n");	
    *res=0;
    start=start/8;	
    memcpy(dummyRes+start,bytes,len);
    printf("User callback done.\n");
    fflush(stdout);
};

int handleTime(PDU *p1, PDU *p2){
	if (p1->param[6]==1) {
	    uc pa[]={0x00,0x01,0x12,0x08,0x12,0x87,0x01,0x01, 0x00,0x00,0,0,}; 
	    printf("Get time from CPU.\n");
	    _daveInitPDUheader(p2,7);
	    _daveAddParam(p2, pa, sizeof(pa));
    	    _daveAddUserData(p2, CpuTimeStamp, sizeof(CpuTimeStamp));
	    if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	    return 1;	
	} else if (p1->param[6]==2) {
	    uc pa[]={0x00,0x01,0x12,0x08,0x12,0x87,0x02,0x01, 0x00,0x00,0,0,}; 
	    printf("Set CPU time.\n");
	    memcpy(CpuTimeStamp, p1->data+4,sizeof(CpuTimeStamp));
	    _daveInitPDUheader(p2,7);
	    _daveAddParam(p2, pa, sizeof(pa));
    	    _daveAddUserData(p2, CpuTimeStamp, sizeof(CpuTimeStamp));
	    if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	    return 1;	
	} else {
	    printf("Cannot handle this\n");	
	    return 0;
	}    
}    

int handleBlocks(PDU *p1, PDU *p2){
    printf("Block handling Commands(%d) ???? ????\n",p1->param[6]);
    if (p1->param[6]==1) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x83,0x01,0x00, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	_daveAddUserData(p2, blockList, sizeof(blockList));
        if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;
    } else if (p1->param[6]==2) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x83,0x02,0x00, 0x00,0x00,0x00,0x00,}; 
	uc da[]={0x0a,0x0,0x0,0x0,}; 
	uc SDBs[]={0x00,0x01,0x22,0x11,0x00,0x02,0x22,0x11};
	printf("List Blocks of type %s\n",daveBlockName(p1->data[5]));	
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	
	if(p1->data[5]==0x42) {
	    _daveAddUserData(p2, SDBs, sizeof(SDBs));
    	} else {
	    _daveAddData(p2, da, sizeof(da));
	}    
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;
    } else if (p1->param[6]==3) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x83,0x03,0x00, 0x00,0x00,0xd2,0x09,}; 
	uc da[]={0x0a,0x0,0x0,0x0,}; 
	printf("get Block info/header %s\n",daveBlockName(p1->data[5]));	
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, da, sizeof(da));
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;
    } else {
	printf("Cannot handle this\n");	
	return 0;
    }	
}

int handleReadProgram(PDU *p1, PDU *p2){
    uc pa[]={
	0x1D,0x00,0x01,0x00,0x00,0x00,0x00,0x07,0x07,0x30,0x30,0x30,0x30,0x30,0x37,0x32,
    };
    printf("Program block readout.\n");
    _daveInitPDUheader(p2,3);
    _daveAddParam(p2, pa, sizeof(pa));
//    if(daveDebug & daveDebugPDU)
    _daveDumpPDU(p2);    
    return 1;
}

typedef struct __ls {
    uc * content;
    int gotlen;
    int totlen;
    int number;
    int PDUnumber;
    int typ;
} loadStruct;

//#define ploaderror    
int handleLoadProgram(daveConnection *dc,PDU *p1, PDU *p2,int PDUnumber, uc*resp, loadStruct * ls){
    int n;
    uc pa[]={
	0x1A
    };
    uc pa2[]={
	0x1B,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x09,0x5F,0x30,0x38,0x30,0x30,0x30,0x30,0x31,0x50,
    };	
#ifndef ploaderror    
    printf("Program block load.\n");
    _daveInitPDUheader(p2,3);
    _daveAddParam(p2, pa, sizeof(pa));
    ls->typ=p1->param[11];
    printf("block type %s. ",daveBlockName(ls->typ));
    ls->number=atol((char*)p1->param+12);
    ls->PDUnumber=1;
    printf("number: %d\n",ls->number);
    p1->param[26]=0;
    ls->totlen=atol((char*)p1->param+20);
    ls->gotlen=0;
    printf("total size: %d\n",ls->totlen);
    ls->content=(uc*)malloc(ls->totlen);
    resp[22]=PDUnumber % 256;	// test!
    resp[21]=PDUnumber / 256;	// test!
    _daveDumpPDU(p2);    
    write(dc->iface->fd.rfd,resp,resp[2]+8);    
    _daveInitPDUheader(p2,1);
    pa2[11]=ls->typ;
    n=ls->number;
    pa2[16]=0x30+(n %10);
    n/=10;
    pa2[15]=0x30+(n %10);
    n/=10;
    pa2[14]=0x30+(n %10);
    n/=10;
    pa2[13]=0x30+(n %10);
    _daveAddParam(p2, pa2, sizeof(pa2));
#else
    _daveInitPDUheader(p2,2);
    p2->header[10]=0xD2;
    p2->header[11]=0x09;
#endif    
//    _daveAddParam(p2, pa, sizeof(pa));
//    if(daveDebug & daveDebugPDU)
/*
    resp[22]=PDUnumber % 256;	// test!
    resp[21]=PDUnumber / 256;	// test!
*/    
    _daveDumpPDU(p2);    
    write(dc->iface->fd.rfd,resp,resp[2]+8);
/*    
    dc->AnswLen=_daveReadIBHPacket(dc->iface, dc->msgIn);    
    analyze(dc);
//	_daveDump("I sent:",resp,resp[2]+8);    
    LOG1(ThisModule);		
	_daveDump("epacket 1", dc->msgIn, dc->AnswLen);
    dc->AnswLen=_daveReadIBHPacket(dc->iface, dc->msgIn);    
    analyze(dc);
//	_daveDump("I sent:",resp,resp[2]+8);    
    LOG1(ThisModule);		
	_daveDump("epacket 2", dc->msgIn, dc->AnswLen);	
    dc->AnswLen=_daveReadIBHPacket(dc->iface, dc->msgIn);    
    analyze(dc);
//	_daveDump("I sent:",resp,resp[2]+8);    
    LOG1(ThisModule);		
	_daveDump("epacket 3", dc->msgIn, dc->AnswLen);	
    dc->AnswLen=_daveReadIBHPacket(dc->iface, dc->msgIn);    
    analyze(dc);
//	_daveDump("I sent:",resp,resp[2]+8);    
    LOG1(ThisModule);		
	_daveDump("epacket 4", dc->msgIn, dc->AnswLen);	
*/	
    return 0;
    
}

int handleContLoadProgram(daveConnection *dc,PDU *p1, PDU *p2,int PDUnumber, uc*resp, loadStruct * ls){
    int n;
    uc pa2[]={
	0x1B,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x09,0x5F,0x30,0x38,0x30,0x30,0x30,0x30,0x34,0x50,
    };
    uc pa3[]={
//	0x1C,0x00,0x84,0x04,0x00,0x00,0x00,0x00,0x09,0x5F,0x30,0x38,0x30,0x30,0x30,0x30,0x34,0x50,
	0x1C,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x09,0x5F,0x30,0x38,0x30,0x30,0x30,0x30,0x34,0x50,
    };
    printf("total size: %d have %d\n",ls->totlen,ls->gotlen);
    ls->gotlen +=p1->dlen-4;
    printf("total size: %d have %d\n",ls->totlen,ls->gotlen);
    
    n=ls->number;
    pa2[16]=0x30+(n %10);
    n/=10;
    pa2[15]=0x30+(n %10);
    n/=10;
    pa2[14]=0x30+(n %10);
    n/=10;
    pa2[13]=0x30+(n %10);
    pa2[11]=ls->typ;
    
    n=ls->number;
    pa3[16]=0x30+(n %10);
    n/=10;
    pa3[15]=0x30+(n %10);
    n/=10;
    pa3[14]=0x30+(n %10);
    n/=10;
    pa3[13]=0x30+(n %10);
    pa3[11]=ls->typ;

    if (p1->param[1]==1) {
	_daveAddParam(p2, pa2, sizeof(pa2));
	return 1;
    }	
    else {	
	_daveAddParam(p2, pa3, sizeof(pa3));
        return 1;
    }	
}

int handleEndLoadProgram(daveConnection *dc,PDU *p1, PDU *p2,int PDUnumber, uc*resp, loadStruct * ls){
    uc pa[]={
	0x1C
    };
    printf("Program block readout.\n");
    _daveInitPDUheader(p2,3);
    _daveAddParam(p2, pa, sizeof(pa));
//    if(daveDebug & daveDebugPDU)
    _daveDumpPDU(p2);    
    return 1;
}

int handleContinueReadProgram(PDU *p1, PDU *p2){
    uc pa[]={
	0x1E,0x00
    };
    uc da[]={
	0x00,0x48,0x00,0xFB, 0x70,0x70,0x00,0x0A, 0x11,0x0B,0x00,0x01, 0x00,0x00,0x00,0x48, 
	0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x0E,0x45,0x00,0x00, 0x00,0x00,0x0E,0x45,
	0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x24, 0x00,0x00,0x00,0x01, 0x00,0x00,0x00,0x00,
	0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
	0x00,0x01,0x7F,0xFF, 0x00,0x00,0x22,0x00, 0x00,0x01,0x00,0xC0,
    };

    printf("Program block readout.\n");
    _daveInitPDUheader(p2,3);
    _daveAddParam(p2, pa, sizeof(pa));
    _daveAddData(p2, da, sizeof(da));
//    if(daveDebug & daveDebugPDU)
    _daveDumpPDU(p2);    
    return 1;
}

int handleEndReadProgram(PDU *p1, PDU *p2){
    uc pa[]={
	0x1F
    };
    printf("Program block readout.\n");
    _daveInitPDUheader(p2,3);
    _daveAddParam(p2, pa, sizeof(pa));
//    if(daveDebug & daveDebugPDU)
    _daveDumpPDU(p2);    
    return 1;
}


int handleSZL(int number, int index, PDU *p2){
    if ((number==292) && (index==0)) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	_daveAddUserData(p2, SZL_292_0, sizeof(SZL_292_0));
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;
    } else if ((number==274) && (index==512)) {	/* read order code (MLFB) */
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	_daveAddUserData(p2, SZL_274_512, sizeof(SZL_274_512));
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;	
    } else if ((number==0) && (index==0)) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	_daveAddUserData(p2, SZL_0_0, sizeof(SZL_0_0));
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;
    } else if ((number==25) && (index==0)) {	
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	_daveAddUserData(p2, SZL_25_0, sizeof(SZL_25_0));
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;		
    } else if ((number==273) && (index==1)) {	/* read order code (MLFB) */
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	_daveAddUserData(p2, SZL_273_1, sizeof(SZL_273_1));
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;
    } else if (number==305) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	switch (index) {
	    case 1: _daveAddUserData(p2, SZL_305_1, sizeof(SZL_305_1)); break;
	    case 2: _daveAddUserData(p2, SZL_305_2, sizeof(SZL_305_1)); break;
	    case 3: _daveAddUserData(p2, SZL_305_3, sizeof(SZL_305_1)); break;
	    case 4: _daveAddUserData(p2, SZL_305_4, sizeof(SZL_305_1)); break;
	    case 5: _daveAddUserData(p2, SZL_305_5, sizeof(SZL_305_1)); break;
	    case 6: _daveAddUserData(p2, SZL_305_6, sizeof(SZL_305_1)); break;
	    case 7: _daveAddUserData(p2, SZL_305_7, sizeof(SZL_305_1)); break;
	    case 8: _daveAddUserData(p2, SZL_305_8, sizeof(SZL_305_1)); break;
	    case 9: _daveAddUserData(p2, SZL_305_9, sizeof(SZL_305_1)); break;
	}
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;
    } else if (number==306) {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	switch (index) {
	    case 1: _daveAddUserData(p2, SZL_306_1, sizeof(SZL_306_1)); break;
	    case 2: _daveAddUserData(p2, SZL_306_2, sizeof(SZL_306_2)); break;
	    case 4: _daveAddUserData(p2, SZL_306_4, sizeof(SZL_306_4)); break;
	}
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;	
    } else if ((number==0xD91) && (index==0)) {	
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	_daveAddUserData(p2, SZL_3473_0, sizeof(SZL_3473_0));
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;
    } else if ((number==1060) && (index==0)) {	
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
	if (runStop == runModeRun)
	    _daveAddUserData(p2, SZL_1060_0, sizeof(SZL_1060_0));
	else if (runStop == runModeStop)    
	    _daveAddUserData(p2, SZL_1060_0S, sizeof(SZL_1060_0S));
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;
    } else if ((number==1316) && (index==20480)) {	
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0x00,0x00,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddUserData(p2, SZL_1316_20480, sizeof(SZL_1316_20480));
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;	
    	
    } else {
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x84,0x01,0x01, 0x00,0x00,0xD4,0x01,}; 
	uc da[]={0x0a,0x0,0x0,0x0,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, da, sizeof(da));
	if(daveDebug & daveDebugPDU)_daveDumpPDU(p2);    
	return 1;	
    }	
}

int handleSystemMessage(PDU * p1,PDU * p2) {
    int number;
    int index;
    if (
	(p1->param[1]==1)  &&
	(p1->param[2]==18) &&
	(p1->param[3]==4)  &&
	(p1->param[4]==17)  &&
	(p1->param[5]=='D') 
    ) {
	number=0x100*(p1->data[4])+p1->data[5];
	index=0x100*(p1->data[6])+p1->data[7];
	printf("SZL read ID: %04X index: %d\n",number,index);
        return handleSZL(number,index,p2);
    } else if (
	(p1->param[1]==1)  &&
	(p1->param[2]==18) &&
	(p1->param[3]==4)  &&
	(p1->param[4]==17)  &&
	(p1->param[5]=='C') 
    ) {
	printf("Block functions\n");
        return handleBlocks(p1,p2);
    }
    else if (
	(p1->param[1]==1)  &&
	(p1->param[2]==18) &&
	(p1->param[3]==4)  &&
	(p1->param[4]==17)  &&
	(p1->param[5]=='G') 
    ) {
	printf("Time System ????\n");
	return handleTime(p1,p2);
    } else if (
	(p1->param[1]==1)  &&
	(p1->param[2]==18) &&
	(p1->param[3]==8)  &&
	(p1->param[4]==18)  &&
	(p1->param[5]=='A')  
    ) {
	if(p1->param[6]==16) { 
	printf("Forces ???? ????\n");
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x81,0x10,0x00, 0x00,0x00,0xD0,0x02,}; 
	uc da[]={0,4,0,4,1,0,0,1,16,1,0,0}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, da, sizeof(da));
	_daveDumpPDU(p2);    
	return 1;	
	} else if(p1->param[6]==12) { 
	printf("Erase ???? ????\n");
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x81,0x12,0x00, 0x00,0x00,0x0,0x0,}; 
	uc da[]={0x0a,0x0,0x0,0x0,}; 
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, da, sizeof(da));
	_daveDumpPDU(p2);    
	return 1;	
	} else {
	printf("Programmer Commands(%d) ???? ????\n",p1->param[6]);
	uc pa[]={0x00,0x01,0x12,0x08,0x12,0x81,0x10,0x00, 0x00,0x00,0xD0,0x04,}; 
	uc da[]={0x0a,0x0,0x0,0x0,}; 
	pa[6]=p1->param[6];
	_daveInitPDUheader(p2,7);
	_daveAddParam(p2, pa, sizeof(pa));
        _daveAddData(p2, da, sizeof(da));
	_daveDumpPDU(p2);    
	return 1;	
	}
    }
    printf("Cannot handle this!\n");
    return 0;
};

int handleNegociate(PDU * p1,PDU * p2) {
    uc pa[]={0xF0,0x0,0x0,0x1, 0x00,0x01,0x00,0xF0}; 
    davePut16At(pa, 6, simMaxPDUlength);
    _daveInitPDUheader(p2,3);
    _daveAddParam(p2, pa, sizeof(pa));
    return 1;		
};

int handleStop(PDU * p1,PDU * p2) {
    uc pa[]={0x29}; 
    runStop=runModeStop;
    _daveInitPDUheader(p2,3);
    _daveAddParam(p2, pa, sizeof(pa));
    return 1;		
};

int handleRun(PDU * p1,PDU * p2, loadStruct * ls) {
    uc pa[]={0x28}; 
    runStop=runModeRun;
    _daveInitPDUheader(p2,3);
    _daveAddParam(p2, pa, sizeof(pa));
    return 1;		
};

int gpacketNumber=0;

uc r5[]={ 
		0xff,0x07,0x13,0x00,0x00,0x00,0xc2,0x02, 0x14,0x14,0x03,0x00,0x00,0x22,0x0c,0xd0,
		0x04,0x00,0x80,0x00,0x02,0x00,0x02,0x01,0x00,0x01,0x00,
	    };

void _daveSendIBHNetAck2(daveConnection * dc) {
//    0xff,0x07,0x05,0x02,0x82,0x00,0x00,0x00, 0x14,0x00,0x03,0x01,0x09,
    IBHpacket * p;
    uc ack[13],c;
    us d;
    memcpy(ack, dc->msgIn, sizeof(ack));
    p= (IBHpacket*) ack;
    d=p->sFlags; p->sFlags=p->rFlags; p->rFlags=d;
    c=p->ch1; p->ch1=p->ch2; p->ch2=c;		// certainly nonsense, but I cannot test it
    c=ack[9];						// at the moment, and because it DID work,
    ack[9]=ack[10];
    ack[10]=c;						// I'll leave it as it is.
    p->len=sizeof(ack)-sizeof(IBHpacket);
    ack[11]=1;
    ack[12]=9;
//    LOG2("Sending net level ack for number: %d\n",p->packetNumber);
    if (daveDebug & daveDebugMPI){
	_daveDump("I send ack", ack,sizeof(ack));
    }    
    _daveWriteIBH(dc->iface, ack,sizeof(ack));
}

loadStruct lost;

void analyze(daveConnection * dc) {
    IBHpacket * p2;
    MPIheader2 * m2;
    uc resp[2000];
    int PDUnumber;
    int haveResp=0;
    PDU p1,pr;
    IBHpacket * p= (IBHpacket*) dc->msgIn;
    dc->needAckNumber=-1;		// Assume no ack
/*    
    printf("Channel: %d\n",p->ch1);
    printf("Channel: %d\n",p->ch2);
    printf("Length:  %d\n",p->len);
    printf("Number:  %d\n",p->packetNumber);
    printf("sFlags:  %04x rFlags:%04x\n",p->sFlags,p->rFlags);
*/    
    if (p->rFlags==0x82) {
	MPIheader * pm= (MPIheader*) (dc->msgIn+sizeof(IBHpacket));
	if (daveDebug & daveDebugAnalyze){
	    printf("srcconn: %d\n",pm->src_conn);
	    printf("dstconn: %d\n",pm->dst_conn);
	    printf("MPI:     %d\n",pm->MPI);
	    printf("MPI len: %d\n",pm->len);
	    printf("MPI func:%d\n",pm->func);
	}	
	if (pm->func==0xf1) {
	    if (daveDebug & daveDebugAnalyze){	
		printf("0xf1:PDU transport, MPI packet number: %d\n",pm->packetNumber);
	    }	
	    dc->needAckNumber=pm->packetNumber;
	    dc->PDUstartI=sizeof(IBHpacket)+sizeof(MPIheader);
	    _daveSetupReceivedPDU(dc, &p1);

	    PDUnumber=p1.header[5]+256*p1.header[4];

	    _daveDumpPDU(&p1);
// construct response:	    
	    pr.header=resp+sizeof(IBHpacket)+sizeof(MPIheader2);
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
	    m2->packetNumber=gpacketNumber;
	    gpacketNumber++;
	    if (p1.param[0]==daveFuncRead) {
		_daveHandleRead(&p1,&pr);
		haveResp=1;
		m2->func=0xf1; //!! guessed
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		p2->len=m2->len+7;
	    } else if (p1.param[0]==daveFuncWrite) {
		printf("before _daveHandleWrite() %p\n",m2);
		_daveHandleWrite(&p1,&pr);
		printf("after _daveHandleWrite() %p\n",m2);
		fflush(stdout);
		haveResp=1;
//		m2->func=0xf1; //!! guessed
		printf("after _daveHandleWrite()\n");
		fflush(stdout);
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		printf("after _daveHandleWrite()\n");
		fflush(stdout);
		p2->len=m2->len+7;
		printf("after _daveHandleWrite()\n");
		fflush(stdout);
	    } else if (p1.param[0]==240) {
		printf("PDU function code: %d, negociate PDU len\n",p1.param[0]);
//		_daveDump("packet:",dc->msgIn,dc->msgIn[2]+8);
		handleNegociate(&p1,&pr);
		dc->packetNumber=0;
		haveResp=1;
		m2->func=0xf1; //!! guessed
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		p2->len=m2->len+7;	
	    } else if (p1.param[0]==0x28) {
		printf("PDU function code: %d, run CPU\n",p1.param[0]);
//		_daveDump("packet:",dc->msgIn,dc->msgIn[2]+8);
		handleRun(&p1,&pr,&lost);
//		dc->packetNumber=0;
		haveResp=1;
		m2->func=0xf1; //!! guessed
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		p2->len=m2->len+7;		
	    } else if (p1.param[0]==0x29) {
		printf("PDU function code: %d, stop CPU\n",p1.param[0]);
//		_daveDump("packet:",dc->msgIn,dc->msgIn[2]+8);
		handleStop(&p1,&pr);
		dc->packetNumber=0;
		haveResp=1;
		m2->func=0xf1; //!! guessed
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		p2->len=m2->len+7;	
	    } else if (p1.param[0]==0x1d) {		
		handleReadProgram(&p1,&pr);
		haveResp=1;
		m2->func=0xf1; //!! guessed
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		p2->len=m2->len+7;	
	    } else if (p1.param[0]==0x1e) {
		handleContinueReadProgram(&p1,&pr);
		haveResp=1;
		m2->func=0xf1; //!! guessed
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		p2->len=m2->len+7;		
	    } else if (p1.param[0]==0x1f) {
		handleEndReadProgram(&p1,&pr);
		haveResp=1;
		m2->func=0xf1; //!! guessed
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		p2->len=m2->len+7;		
	    } else if (p1.param[0]==0x1a) {		
		handleLoadProgram(dc, &p1,&pr,PDUnumber,resp, &lost);
		haveResp=1;
		m2->func=0xf1; //!! guessed
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		p2->len=m2->len+7;	
//		PDUnumber=lost.PDUnumber;
	    } else if (p1.param[0]==0x1b) {		
		haveResp=handleContLoadProgram(dc, &p1,&pr,PDUnumber,resp, &lost);
		m2->func=0xf1; //!! guessed
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		p2->len=m2->len+7;		
	    } else if (p1.param[0]==0x1c) {		
		handleEndLoadProgram(dc, &p1,&pr,PDUnumber,resp, &lost);
		haveResp=0;
	    } else if (p1.param[0]==0) {
		printf("PDU function code: %d, system Messaga ?\n",p1.param[0]);
		_daveSendMPIAck2(dc);
//		_daveDump("packet:",b,b[2]+8);
		haveResp=handleSystemMessage(&p1,&pr);
//		pr.header[4]=p1.header[4];		// give the PDU a number
//		pr.header[5]=p1.header[5];		// give the PDU a number
		m2->func=0xf1; //!! guessed
		m2->len=pr.hlen+pr.plen+pr.dlen+2;
		p2->len=m2->len+7;	
	    } else {
		printf("Unsupported PDU function code: %d\n",p1.param[0]);
		
	    }
	    
	}
	if (pm->func==0xb0) {
//    	    printf("Ackknowledge for packet number: %d\n",*(dc->msgIn+15));
	}
	if (pm->func==0xe0) {
	    printf("Connect to MPI: %d\n",pm->MPI);
	    
	    memcpy(resp, r5, sizeof(r5));
	    resp[8]=pm->src_conn;
	    resp[9]=pm->src_conn;
	    resp[10]=pm->MPI;
	    resp[11]=7;		//????
	    haveResp=1;
	}    
    }	
    if (((p->rFlags==0x82) /*||(p->sFlags==0x82)*/)&&(p->packetNumber)&&(p->len)) {
//	printf("before _daveSendIBHNetAck()\n");
	fflush(stdout);
	_daveSendIBHNetAck2(dc);
    }	
    if (haveResp) {
//	printf("have response\n");
	resp[22]=PDUnumber % 256;	// test!
        resp[21]=PDUnumber / 256;	// test!
	
	write(dc->iface->fd.rfd,resp,resp[2]+8);
	_daveDump("I sent:",resp,resp[2]+8);    
	fflush(stdout);
    }	
};

typedef struct _portInfo {
    int fd;
}portInfo;

#define mymemcmp _daveMemcmp
void *portServer(void *arg)
{
    int waitCount, res, pcount, r2;
    _daveOSserialType s;
    daveInterface * di;
    daveConnection * dc;
    
    portInfo * pi=(portInfo *) arg;
    LOG2(ThisModule "portMy fd is:%d\n", 
	pi->fd);
    FLUSH;
    waitCount= 0;
//    daveDebug=daveDebugAll;
    pcount=0;
    
    s.rfd=pi->fd;
    s.wfd=pi->fd;
    di=daveNewInterface(s,"IF",0,daveProtoMPI_IBH,daveSpeed187k);
    di->timeout=900000;
    dc=daveNewConnection(di,0,0,0);
    di->timeout=1900000;
    while (waitCount < 1000) {
	dc->AnswLen=_daveReadIBHPacket(dc->iface, dc->msgIn);
	if (dc->AnswLen>0) {
	    res=dc->AnswLen;
	    if (daveDebug & daveDebugPacket) {
	    LOG2(ThisModule "%d ", pcount);		
		_daveDump("packet", dc->msgIn, dc->AnswLen);
	    }
	    waitCount = 0;
	    analyze(dc);
		    
	    r2=2*res;

	    if(r2==sizeof(cha0)) {
	        if (0==mymemcmp(cha0, dc->msgIn, res)) {
			LOG1(ThisModule "found challenge 0, write response 0\n");
			write(pi->fd, res0, sizeof(res0));
		    }
	    }
	    if(r2==sizeof(cha1)) {
	        if (0==mymemcmp(cha1, dc->msgIn, res)) {
		    LOG1(ThisModule "found challenge 1, write response 1\n");
		    write(pi->fd, res1, sizeof(res1));
		}
	    }
	    if(r2==sizeof(cha2)) {
	        if (0==mymemcmp(cha2, dc->msgIn, res)) {
		    LOG1(ThisModule "found challenge 2, write response 2\n");
		    write(pi->fd, res2, sizeof(res2));
		}
	    }
	    if(r2==sizeof(cha3)) {
	        if (0==mymemcmp(cha3, dc->msgIn, res)) {
		    LOG1(ThisModule "found challenge 3, write response 3\n");
		    res3[8]=dc->msgIn[8];    
		    write(pi->fd, res3, sizeof(res3));
		}
	    }

	    if(r2==sizeof(cha8)) {
	        if (0==mymemcmp(cha8, dc->msgIn, res)) {
		    LOG1(ThisModule "found challenge 8, write response 7\n");
		    res7[8]=dc->msgIn[8];    
		    res7[9]=dc->msgIn[9];    
		    res7[10]=dc->msgIn[10];    
		    write(pi->fd, res7, sizeof(res7));
		}
	    }

	    if(r2==sizeof(cha11)) {
	        if (0==mymemcmp(cha11, dc->msgIn, res)) {
		    LOG1(ThisModule "found challenge 11, response 10\n");
		    res10[8]=dc->msgIn[8];    
		    res10[9]=dc->msgIn[9];    
//			res10[10]=dc->msgIn[10];    
//			res10[32]=dc->msgIn[28];    
//			res10[34]=dc->msgIn[30];    
//			sendMPIAck2(dc->msgIn, pi->fd, ackPacketNumber);
////		    dc->needAckNumber=0;
////		    _daveSendMPIAck2(dc);
////		    write(pi->fd, res10, sizeof(res10));
		}
	    }
	    pcount++;
	} else {
	    waitCount++;
	}    
    }
    LOG1(ThisModule "portserver: I closed my fd.\n");
    FLUSH;
    return NULL;
}


/*
    This waits in select for a file descriptor from accepter and starts a new child server
    with this file descriptor.
*/
int PID;
int main(int argc, char **argv)
{
    portInfo pi;
    fd_set FDS;
    int filedes[2], res, newfd;
    pthread_attr_t attr;
    pthread_t ac, ps;
    accepter_info ai;
    PID=getpid();
    if (argc<2) {
	printf("Usage: ibhtest port\n");
	printf("Example: ibhtest 1099 (used by IBHNetLink)\n");
	return -1;
    }
    readCallBack=dummyRead;
    writeCallBack=myWrite;
    
    pipe(filedes);
    ai.port = atol(argv[1]);
    LOG2(ThisModule "Main serv: %d\n", ai.port);
    LOG2(ThisModule "Main serv: Accepter pipe fd: %d\n", ai.fd);
    ai.fd = filedes[1];
    pthread_attr_init(&attr);
    pthread_attr_setdetachstate(&attr,PTHREAD_CREATE_DETACHED);
    res=pthread_create(&ac, &attr, accepter, &ai /*&filedes[1] */ );
    do {
	FD_ZERO(&FDS);
	FD_SET(filedes[0], &FDS);

	LOG2(ThisModule "Main serv: about to select on %d\n",
	       filedes[0]);
	FLUSH;
	if (select(filedes[0] + 1, &FDS, NULL, &FDS, NULL) > 0) {
	    LOG1(ThisModule "Main serv: about to read\n");
	    res = read(filedes[0], &pi.fd, sizeof(pi.fd));
	    ps=0;		   
	    pthread_attr_init(&attr);
	    pthread_attr_setdetachstate(&attr,PTHREAD_CREATE_DETACHED);
	    res=pthread_create(&ps, &attr, portServer, &pi);
	    if(res) {
		LOG2(ThisModule
		   "Main serv: create error:%s\n", strerror(res));		   
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
    02/01/2005 fixed argc, it's 2 if there is 1 argument.
    03/06/2005 removed byteswap.h, it is not needed.
*/
