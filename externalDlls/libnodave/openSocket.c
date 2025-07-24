/*
 Part of Libnodave, a free communication libray for Siemens S7 300/400.
 This version uses the IBHLink MPI-Ethernet-Adapter from IBH-Softec.
 www.ibh.de
 
 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2002, 2003.

 Libnodave is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 Libnodave is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/

#ifndef __openSocket
#define __openSocket

#define ThisModule "openSocket: "

#ifndef DONT_USE_GETHOSTBYNAME
#include <netdb.h>		// for gethostbyname
#endif

#include <stdio.h>
#include <string.h>
#include <errno.h>
#include <unistd.h>
#include <fcntl.h>
#include "nodave.h"
#include "log2.h"
/* The following two lines seem to be necessary for FreeBSD and do no harm on Linux */

#include <netinet/in.h>
#include <sys/socket.h>

extern int daveDebug;

int openSocket(const int port, const char * peer) {
    int fd,res,opt;
    struct sockaddr_in addr;
    socklen_t addrlen;
#ifndef DONT_USE_GETHOSTBYNAME    
    struct hostent *he;
#endif    
    if (daveDebug & daveDebugOpen) {
	LOG1(ThisModule "enter OpenSocket");
	FLUSH;
    }
    addr.sin_family = AF_INET;
    addr.sin_port =htons(port);
//	(((port) & 0xff) << 8) | (((port) & 0xff00) >> 8);
#ifndef DONT_USE_GETHOSTBYNAME
    he = gethostbyname(peer);
    memcpy(&addr.sin_addr, he->h_addr_list[0], sizeof(addr.sin_addr));
#else
    inet_aton(peer, &addr.sin_addr);
#endif

    fd = socket(AF_INET, SOCK_STREAM, 0);
    if (daveDebug & daveDebugOpen) {
	LOG2(ThisModule "OpenSocket: socket is %d\n", fd);
    }	
    
    addrlen = sizeof(addr);
    if (connect(fd, (struct sockaddr *) & addr, addrlen)) {
	LOG2(ThisModule "Socket error: %s \n", strerror(errno));
	close(fd);
	fd = 0;
    } else {
	if (daveDebug & daveDebugOpen) {
	    LOG2(ThisModule "Connected to host: %s \n", peer);
	}    
/*
	Need this, so we can read a packet with a single read call and make
	read return if there are too few bytes.
*/	
	errno=0;
//	res=fcntl(fd, F_SETFL, O_NONBLOCK);
//	if (daveDebug & daveDebugOpen) 
//	    LOG3(ThisModule "Set mode to O_NONBLOCK %s %d\n", strerror(errno),res);
/*
	I thought this might solve Marc's problem with the CP closing
	a connection after 30 seconds or so, but the Standrad keepalive time
	on my box is 7200 seconds.	    
*/	
	errno=0;
	opt=1;
	res=setsockopt(fd, SOL_SOCKET, SO_KEEPALIVE, &opt, 4);
	if (daveDebug & daveDebugOpen) {	
	    LOG3(ThisModule "setsockopt %s %d\n", strerror(errno),res);	
	}    
    }	
    FLUSH;
    return fd;
}

int closeSocket(int h) {
    return close(h);
}


#endif
/*
    Changes: 
    07/12/2003	moved openSocket to it's own file, because it can be reused in other TCP clients
    04/07/2004  ported C++ version to C
    07/19/2004  removed unused vars.
    03/06/2005  added Hugo Meiland's includes for FreeBSD.
Version 0.8.4.5    
    07/10/09  	Added closeSocket()
    
*/
