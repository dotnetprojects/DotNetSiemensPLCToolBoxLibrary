/*
 Part of Libnodave, a free communication libray for Siemens S7 300/400.

 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2004.

 Libnodave is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 Libnodave is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/

#define debug 0
#define ThisModule "setPort : "
#include "log2.h"
#ifdef BCCWIN
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <stdio.h>
#include "nodave.h"

extern int daveDebug;


/*
   You may wonder why a pair of identical file handles is set up and
   returned. It's for compatibility with an extended UNIX version of
   this code, which can use two separate pipes for reads and writes.
*/

__declspec(dllexport)
HANDLE __stdcall setPort(char * devname, char * baud,char parity /*, HANDLE * wfd*/){
	HANDLE hComm;
	DCB dcb;
	
       hComm = CreateFile( devname, 
       GENERIC_READ | GENERIC_WRITE,       
       0, 
       0,
       OPEN_EXISTING,
       FILE_FLAG_WRITE_THROUGH,
       0);
	if (daveDebug & daveDebugOpen) {
       
	    LOG2(ThisModule "setPort %s\n",devname);
	    LOG2(ThisModule "setPort %s\n",baud);
	    LOG2(ThisModule "setPort %c\n",parity);
	}

//	printf("Handle to %s opened! %d\n",devname,hComm);
	GetCommState(hComm,&dcb);
//	printf("got Comm State. %d\n ",dcb.BaudRate);
	dcb.ByteSize = 8;
	dcb.fOutxCtsFlow=FALSE;
	dcb.fOutxDsrFlow=FALSE;
//	dcb.fDtrControl=DTR_CONTROL_DISABLE; // this seems to be the evil. Guess do not understand the meaning of this parameter

	dcb.fDtrControl=DTR_CONTROL_ENABLE;
	
	dcb.fDsrSensitivity=FALSE;
        dcb.fInX=FALSE;
	dcb.fOutX=FALSE;
	dcb.fNull=FALSE;
	dcb.fAbortOnError=FALSE;
	dcb.fBinary=TRUE;
	dcb.fParity=TRUE;
	dcb.fOutxCtsFlow=FALSE;
	dcb.fOutxDsrFlow=FALSE;
//	dcb.fRtsControl=FALSE;   // this seems to be the evil. Guess do not understand the meaning of this parameter
	dcb.fRtsControl=TRUE;
	dcb.fTXContinueOnXoff=TRUE;
	dcb.StopBits=2;  ///that was 2 !!!

	//JK changed this, because step 7 also uses only one!
	dcb.StopBits=0;  ///that was 2 !!!
	if (0==strncmp(baud,"115200",6))
	dcb.BaudRate = CBR_115200;
    else if (0==strncmp(baud,"57600",5))
	dcb.BaudRate = CBR_57600;
    else if (0==strncmp(baud,"38400",5))	
	dcb.BaudRate = CBR_38400;
    else if (0==strncmp(baud,"19200",5))	
	dcb.BaudRate = CBR_19200;
    else if (0==strncmp(baud,"9600",4))	
	dcb.BaudRate = CBR_9600;
    else if (0==strncmp(baud,"4800",4))	
	dcb.BaudRate = CBR_4800;
    else if (0==strncmp(baud,"2400",4))	
	dcb.BaudRate = CBR_2400;
    else if (0==strncmp(baud,"1200",4))	
	dcb.BaudRate = CBR_1200;
    else if (0==strncmp(baud,"600",3))	
	dcb.BaudRate = CBR_600;
    else if (0==strncmp(baud,"300",3))		
	dcb.BaudRate = CBR_300;
    else if (daveDebug & daveDebugPrintErrors) {
		LOG2(ThisModule "illegal Baudrate: %s\n", baud);
    }
	parity=tolower(parity);
    if (parity == 'e')
	dcb.Parity = 2;
	else if (parity == 'o')
	dcb.Parity = 1;
	else if (parity == 'n')
	dcb.Parity = 0;
	else if (daveDebug & daveDebugPrintErrors) {
	    LOG2(ThisModule "illegal parity mode:%c\n", parity);
	}
	    

	SetCommState(hComm,&dcb);
//	printf("got Comm State. %d\n ",dcb.BaudRate);

//printf("Comm State set.\n");
//	*wfd=hComm;
	return hComm;
}

__declspec(dllexport)
int __stdcall closePort(HANDLE port){
	int res=CloseHandle(port);
	return res;
}
#endif

/*
    Changes:
    
    12/17/2004 1st Version for Windows.
    04/03/2005 Hopefully really fixed COM port setting.
    05/08/2005 Removed printfs for quiet operation.
*/
