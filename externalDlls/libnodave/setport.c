/*
 Part of Libnodave, a free communication libray for Siemens S7 300/400.
 
 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2001.

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
//#ifdef __cplusplus  // some people try to compile this with C++ compilers...
		    // Then the linker will expect mangled names, but libnodave dont have them.
//extern "C" {
//#endif

#include <termios.h>
#include <fcntl.h>
#include <stdio.h>
#include <unistd.h>
#include <errno.h>
#include <string.h>
#include <ctype.h>
#include "setport.h"
#define debug 0
#define ThisModule "setPort : "

int setPort(char * name, char* baud,char parity)
{
    int fd,res;
    struct termios oldtio, newtio;
//    fd = open(name,  O_RDWR | O_NOCTTY | O_NONBLOCK);
    fd = open(name,  O_RDWR | O_NOCTTY);
#if debug >3
    printf(ThisModule "%s = %d\n", name, fd);
#endif
//    res=fcntl(fd, F_SETFL, 0x2000 /* FASYNC */ );
#if debug >3
    printf(ThisModule "fcntl: %s\n", strerror(errno));
#endif
    tcgetattr(fd, &oldtio);	// save current settings
#if debug >3
    printf(ThisModule "tcget...:%s\n", strerror(errno));
#endif
// set new port settings for noncanonical input processing:
    if (0==strncmp(baud,"115200",6))
	newtio.c_cflag = B115200;
    else if (0==strncmp(baud,"57600",5))
	newtio.c_cflag = B57600;
    else if (0==strncmp(baud,"38400",5))	
	newtio.c_cflag = B38400;
    else if (0==strncmp(baud,"19200",5))	
	newtio.c_cflag = B19200;
    else if (0==strncmp(baud,"9600",4))	
	newtio.c_cflag = B9600;
    else if (0==strncmp(baud,"4800",4))	
	newtio.c_cflag = B4800;
    else if (0==strncmp(baud,"2400",4))	
	newtio.c_cflag = B2400;
    else if (0==strncmp(baud,"1200",4))	
	newtio.c_cflag = B1200;
    else if (0==strncmp(baud,"600",3))	
	newtio.c_cflag = B600;
    else if (0==strncmp(baud,"300",3))		
	newtio.c_cflag = B300;
    else
	printf(ThisModule "illegal Baudrate: %s\n", baud);
    parity=tolower(parity);
    if (parity == 'e')
//	newtio.c_cflag = newtio.c_cflag & (~PARODD | PARENB); // wrong !!
//	newtio.c_cflag = newtio.c_cflag & ~PARODD | PARENB;   // gives compiler warning 	
	newtio.c_cflag = (newtio.c_cflag & ~PARODD) | PARENB;
    else if (parity == 'o')
	newtio.c_cflag = newtio.c_cflag | PARODD | PARENB;
    else if (parity == 'n')
	newtio.c_cflag = newtio.c_cflag & ~(PARODD | PARENB);
    else
	printf(ThisModule "illegal parity mode:%c\n", parity);

    newtio.c_cflag |= CS8; // should allways be eight bit

    newtio.c_cflag = newtio.c_cflag | CLOCAL | CREAD;
    newtio.c_iflag = 0;		// {IGNPAR |} ICRNL;
    newtio.c_oflag = ONOCR;
    newtio.c_lflag = 0;		//ICANON
    newtio.c_cc[VMIN] = 1;
    newtio.c_cc[VTIME] = 0;
    tcflush(fd, TCIFLUSH);
#if debug >3
    printf(ThisModule "tcflush:%s\n", strerror(errno));
#endif
    tcsetattr(fd, TCSANOW, &newtio);
#if debug >3
    printf(ThisModule "tcset...:%s\n", strerror(errno));
#endif
    return (fd);
}

/*
    Not acting on -1 is just a kludge for s7online. See comment in nodave.c before 
    daveDisconnectAdapterS7online for detailed explanation.
*/
int closePort(int port){
    int res=0;
    if (port!=-1)
        res=close(port);
    return res;
}
//#ifdef __cplusplus  
//    }		 
//#endif

/*
    Changes:
    
    02/12/2002 change output statements to printf, this gives less code size.
    06/21/2002 	* got rid of some warnings
    10/04/2003	fixed an error with parity even
Version 0.8.4.5
    07/12/09  	Changed flags in setport.c. Older version does not work with recent Linux 
		kernels.
    
*/
