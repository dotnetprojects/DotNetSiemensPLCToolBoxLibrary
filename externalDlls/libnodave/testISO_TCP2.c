#include <winsock2.h>
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>

//#include "nodave.h"
#include <stdio.h>
#include <fcntl.h>
#include "log2.h"
#include <string.h>
//#include "openS7online.h"

#include <winsock2.h>

void main()
	{
	WSADATA wsaData;
WORD version;
int error;
struct sockaddr_in sin;
SOCKET client;
fd_set FDS;	
struct timeval t;
int opt;

version = MAKEWORD( 2, 2 );
printf("%d",FD_SETSIZE);
error = WSAStartup( version, &wsaData );

/* check for error */
if ( error != 0 )
{
   printf("fehler1");
}


client = socket( AF_INET, SOCK_STREAM, IPPROTO_TCP );

memset( &sin, 0, sizeof sin );
sin.sin_family = AF_INET;
sin.sin_addr.s_addr = inet_addr("127.0.0.1");
sin.sin_port = htons( 102 );

if ( connect( client, &sin, sizeof sin ) == SOCKET_ERROR )
{
	
	printf("fehler2");
    /* could not connect to server */
    return FALSE;
}
opt=1;
//setsockopt(client, SOL_SOCKET, SO_KEEPALIVE, &opt, 4);

send(client, "aaaa", 4, 0);

printf("socket: %d\n", client);
printf("gesendet");

FD_ZERO(&FDS);
FD_SET(client, &FDS);
	
	t.tv_sec = 500000000 / 1000000;
	t.tv_usec = (500000000 % 1000000);

	if(select(1, &FDS, NULL, NULL, &t) == SOCKET_ERROR) {
		printf("fehler");
	}
	
printf("warte");

getch();

printf("ende");


}
