/*
 Part of multithreaded network servers. The servers MAIN thread is started by
 the main program. It starts in turn the ACCEPTER thread, which accepts a
 connection and passes the file descriptor to the MAIN thread. The server MAIN
 thread starts a PORT SERVER thread to handle the connection. Thus, the 
 PORT SERVER thread is NOT a child of ACCEPTER, but a brother and SIGCHILD goes
 to the server MAIN. If it would go to ACCEPTER, which is blocked in the 
 accept() system call, we would get "process interrupted in system call" and 
 that's it. Maybe its different with pthread library..?

 Part of Libnodave, a free communication libray for Siemens S7 300/400.
 
 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2001..2005.

 LIBNDAVE is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 LIBNODAVE is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/

typedef struct  {
    int fd;
    int port;
} accepter_info;

void *accepter(void *arg)
{
    int s;
    int newfd, pipefd;
    socklen_t addrlen;
    struct sockaddr_in addr;
    int i;
    int opt;
    usleep(10000);
    pipefd = ((accepter_info *) arg)->fd;
    LOG2(ThisModule "Accepter : My pipe is:%d\n", pipefd);
    FLUSH;
    s = socket(AF_INET, SOCK_STREAM, 0);
    if (errno != 0) {
	LOG2(ThisModule "Accepter : socket %s\n", strerror(errno));
	FLUSH;
    }
    LOG2(ThisModule "Accepter : port %d\n",
	   ((accepter_info *) arg)->port);
    addr.sin_family = AF_INET;
    addr.sin_port =htons(((accepter_info *) arg)->port);
//	(((((accepter_info *) arg)->port) & 0xff) << 8) |
//	(((((accepter_info *) arg)->port) & 0xff00) >> 8);
//    LOG2(ThisModule"Accepter : port %d\n",addr.sin_port);
    inet_aton("0.0.0.0", &addr.sin_addr);
    opt = 1;
    i = setsockopt(s, SOL_SOCKET, SO_REUSEADDR, &opt, 4);
    LOG2(ThisModule "Accepter : setsockopt %s\n", strerror(errno));
    addrlen = sizeof(addr);
    bind(s, (struct sockaddr *) & addr, addrlen);
    LOG2(ThisModule "Accepter : bind %s\n", strerror(errno));
    listen(s, 1);
    LOG2(ThisModule "Accepter : listen: %s\n", strerror(errno));
    while (1) {
	addrlen = sizeof(addr);
	LOG1(ThisModule "Accepter : before accept\n");
	newfd = (accept(s, (struct sockaddr *) & addr, &addrlen));
	LOG2(ThisModule "Accepter: after accept. New fd:%d\n", newfd);
	FLUSH;
	LOG3(ThisModule "Accepter: PID(%d) client:%s\n", getpid(),
	       inet_ntoa(addr.sin_addr));
	FLUSH;
	write(pipefd, &newfd, sizeof(newfd));
    }
    shutdown(s, 2);
    LOG3(ThisModule "Accepter : %d shutdown done. Socket is: %d\n",
	   getpid(), s);
}

struct abstr {
    int fd;
    int mask;
};
