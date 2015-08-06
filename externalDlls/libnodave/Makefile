#
# removed testUSB from programs. Use "make usb" to make it.
# removed definitions for CYGWIN. There were no more differences versus LINUX
#
# These were the paths and names of the ARM cross compiling tools I tried:
# The -Bstatic option was necessary to get statically linked files needed for ARM-simulator
#
# CC=/usr/local/arm/2.95.3/bin/arm-linux-gcc
# LD=/usr/local/arm/2.95.3/bin/arm-linux-ld -Bstatic
# LDFLAGS= -Wl,-Bstatic
##LD=ld
#
# To test with g++ which does stricter type checking:
#
##CC=g++ 
CFLAGS=-Wall -Winline -DLINUX -DDAVE_LITTLE_ENDIAN
CTFLAGS=-Wall -Winline -fPID -DLINUX -DDAVE_LITTLE_ENDIAN
CPPFLAGS=-Wall -Winline -DLINUX -DDAVE_LITTLE_ENDIAN
#
# The following is needed to enable workarounds for statements that do
# not work on (some?) ARM processors:
# It also helped on some machine running HP-UX.
#
#CFLAGS+=-DARM_FIX 


#-static -Wl,static -lc.a -static -lpthread.a -nostdlib 
#CFLAGS=-O0 -Wall -Winline
PROGRAMS=testIBH testISO_TCP testMPI testPPI \
testPPIload testMPIload ibhsim5 \
isotest4 \
testISO_TCPload testMPI_IBHload testPPI_IBHload testPPI_IBH \
testNLpro \
testAS511 



# testISO2
# testMPImin
# testPPI_IBH

DYNAMIC_PROGRAMS=testMPId testPPId testISO_TCPd

LIBRARIES=libnodave.so 


all: $(PROGRAMS) $(LIBRARIES)
install: libnodave.so
	cp libnodave.so /usr/lib
	cp nodave.h /usr/include
	ldconfig
dynamic: $(DYNAMIC_PROGRAMS)
usb: testUSB

nodave.o: nodave.h log2.h
openSocket.o: openSocket.h nodave.h log2.h

testISO_TCP.o: benchmark.c nodavesimple.h
testPPI.o: benchmark.c nodavesimple.h
testPPIcpp.o: benchmark.c nodavesimple.h
testMPI.o: benchmark.c nodavesimple.h
testIBH.o: benchmark.c nodavesimple.h
testPPI_IBH.o: benchmark.c nodavesimple.h
testISO_TCPload.o: nodave.h
testPPIload.o: nodave.h
testMPIload.o: nodave.h
testMPI_IBHload.o: nodave.h
testPPI_IBHload.o: nodave.h
testNLpro.o: benchmark.c nodavesimple.h

testISO_TCP: nodave.o openSocket.o testISO_TCP.o
	$(CC) $(LDFLAGS) nodave.o openSocket.o testISO_TCP.o -o testISO_TCP
testISO2: nodave.o openSocket.o testISO2.o
	$(CC) $(LDFLAGS) nodave.o openSocket.o testISO2.o -o testISO2
testISO_TCPd: nodave.o openSocket.o testISO_TCP.o
	$(CC) -lnodave testISO_TCP.o -o testISO_TCPd
testPPIload: nodave.o setport.o testPPIload.o
	$(CC) $(LDFLAGS) nodave.o setport.o testPPIload.o -o testPPIload
testMPI: setport.o testMPI.o nodave.o
	$(CC) $(LDFLAGS) setport.o nodave.o testMPI.o -o testMPI
testMPImin: setport.o testMPImin.o nodave.o
	$(CC) $(LDFLAGS) setport.o nodave.o testMPImin.o -o testMPImin	
testMPId: setport.o testMPI.o nodave.o
	$(CC) -lnodave testMPI.o -o testMPId
testMPIload: nodave.o setport.o testMPIload.o
	$(CC) $(LDFLAGS) nodave.o setport.o testMPIload.o -o testMPIload
testMPI_IBHload: nodave.o openSocket.o testMPI_IBHload.o
	$(CC) $(LDFLAGS) nodave.o openSocket.o testMPI_IBHload.o -o testMPI_IBHload
testPPI: nodave.o setport.o testPPI.o
	$(CC) $(LDFLAGS) nodave.o setport.o testPPI.o -o testPPI
testPPId: nodave.o setport.o testPPI.o
	$(CC) -lnodave testPPI.o -o testPPId	
testISO_TCPload: nodave.o openSocket.o testISO_TCPload.o
	$(CC) $(LDFLAGS) nodave.o openSocket.o testISO_TCPload.o -o testISO_TCPload
testIBH: openSocket.o testIBH.o nodave.o
	$(CC) $(LDFLAGS) openSocket.o nodave.o testIBH.o -o testIBH
testPPI_IBH: openSocket.o testPPI_IBH.o nodave.o
	$(CC) $(LDFLAGS) openSocket.o nodave.o testPPI_IBH.o -o testPPI_IBH
testPPI_IBHload: openSocket.o testPPI_IBHload.o nodave.o
	$(CC) $(LDFLAGS) openSocket.o nodave.o testPPI_IBHload.o -o testPPI_IBHload
testPPIcpp: nodave.o setport.o testPPIcpp.o
	$(CC) $(LDFLAGS) nodave.o setport.o testPPIcpp.o -o testPPIcpp
testMPI2: setport.o testMPI2.o nodave.o nodaveext.o
	$(CC) $(LDFLAGS) setport.o nodave.o nodaveext.o  testMPI2.o -o testMPI2
testAS511: setport.o testAS511.o nodave.o
	$(CC) $(LDFLAGS) setport.o nodave.o testAS511.o -o testAS511
testUSB: testUSB.o nodave.o usbGlue.o usbGlue.h
	$(CC) $(LDFLAGS) nodave.o testUSB.o usbGlue.o -lusb -o testUSB
testNLpro: openSocket.o testNLpro.o nodave.o
	$(CC) $(LDFLAGS) openSocket.o nodave.o testNLpro.o -o testNLpro
crc: crc.o
	$(CC) $(LDFLAGS) crc.o -o crc
crc3: crc3.o
	$(CC) $(LDFLAGS) crc3.o -o crc3
testHTTP: nodave.o openSocket.o testHTTP.o
	$(CC) $(LDFLAGS) nodave.o openSocket.o testHTTP.o -o testHTTP
ibhsim9.o: simProperties2.c blocklist.h
ibhsim9: ibhsim9.o nodave.h nodave.o openSocket.o openSocket.h blocklist.o blocklist2.o setport.o
	$(CC) -lpthread ibhsim9.o openSocket.o nodave.o blocklist.o blocklist2.o setport.o -o ibhsim9
ibhsim10.o: simProperties2.c blocklist.h
ibhsim10: ibhsim10.o nodave.h nodave.o openSocket.o openSocket.h blocklist.o blocklist2.o setport.o emulator.o
	$(CC) -lm -lpthread ibhsim10.o openSocket.o nodave.o blocklist.o blocklist2.o setport.o emulator.o -o ibhsim10



libnodave.so: nodave.o setport.o openSocket.o
	$(LD) -shared nodave.o setport.o openSocket.o -o libnodave.so	

ibhsim5.o: simProperties.c
ibhsim5: ibhsim5.o nodave.h nodave.o openSocket.o openSocket.h
	$(CC) -lpthread ibhsim5.o openSocket.o nodave.o -o ibhsim5
isotest4: isotest4.o openSocket.o nodave.o nodave.h
	$(CC) $(LDFLAGS) -lpthread isotest4.o openSocket.o nodave.o $(LIB) -o isotest4

clean: 
	rm -f $(DYNAMIC_PROGRAMS)
	rm -f $(PROGRAMS)
	rm -f *.o
	rm -f *.so
	rm -f *.dll
	rm -f *.obj

