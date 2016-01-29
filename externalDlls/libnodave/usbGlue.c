/*
 Part of Libnodave, a free communication libray for Siemens S7 200/300/400 via
 the MPI adapter 6ES7 972-0CA22-0XAC
 or  MPI adapter 6ES7 972-0CA23-0XAC
 or  TS adapter 6ES7 972-0CA33-0XAC
 or  MPI adapter 6ES7 972-0CA11-0XAC,
 IBH/MHJ-NetLink or CPs 243, 343 and 443
 or VIPA Speed7 with builtin ethernet support.
 
 This file allows read and write access to an USB device which is not supported by any 
 kernel driver from a user program. While I provide a kernel driver for Linux 2.6.13
 this file could be a solution independent of kernel versions.
  
 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2002..2005
 
 Large parts of this code were copied from usb_robot_slave,

 (C) 2000 John Fremlin <vii@penguinpowered.com>

 Libnodave is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 Libnodave is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/

#include <stdio.h>
#include <usb.h>

extern int usb_set_configuration(struct usb_dev_handle *dev, int configuration);
extern int usb_claim_interface(struct usb_dev_handle *dev, int interface);
#ifndef USB_OK
#define USB_OK 0
#endif

#ifndef USB_DIR_IN
#define USB_DIR_IN 0x80
#endif

#ifndef USB_DIR_OUT
#define USB_DIR_OUT 0x00
#endif

#include "nodave.h"
#include "log2.h"
/*
    NOTE: This has worked before I optimized the MPI3 transport. The reason is:
    In the first attempts, I just called usbread() with a length larger than any expectable
    length. This results in usb_bulk_read() waiting until timeout occurs. Then I 
    optimized it in a way to read 5 bytes first. These 5 bytes contain length 
    information for longer packets. A second call to usbread() fetches the rest. This works 
    with file descriptors for serial ports or TCP/IP or anything else, but not with 
    usb_bulk_read():
    The first usbread() reads the block, returns 5 bytes and discards the rest. Hence the
    second usbread() doesn't read anything more and returns 0 after timeout.
    
    The solution is to introduce a a ring buffer, which is filled by usb_bulk_read()
    and read by usbread() calls from nodave.c. Whenever the ringbuffer hasn't enough
    bytes to satisfy an usbread() call, then and only then it calls usb_bulk_read().
    I implemented it now, but cannot test it, as I have no USB adapter at hand and don't
    know when I can borrow one again. 
    I know the code is terrible, but I tried to write it so that I have least doubt
    that it could work...
    So I publish it as is. Maybe someone else tests it and lets me know....
*/


/*
  These valus are apropriate for Siemens USB-MPI adapter 6ES7 972-0CB20-0XA0. Let me know, 
  if you find others:
*/
int idVendor = 0x0908;	
int idProduct = 0x0004;

extern int daveDebug;

#define WITH_UNTESTED_RING_BUFFER
#ifdef WITH_UNTESTED_RING_BUFFER

#define rbSize 2000 // big enough in any case

uc ringBuffer[rbSize];
int insertPosition;
int readPosition;

int bytesAvail() {
    if (readPosition<=insertPosition) return insertPosition-readPosition;
    return insertPosition+rbSize-readPosition;
}

int usbread(daveInterface *di, uc * buffer, int length) {
    int i,j,k;
    uc buffer2[rbSize];
    j=0;
repeat:    
    while (bytesAvail() >= length) {
	buffer[j]=ringBuffer[readPosition];
	readPosition++;
	readPosition %= rbSize;
	length--;
	j++;
    }
    if (length) {
	i=usb_bulk_read(
	    (usb_dev_handle *) di->fd.rfd,
	    0x82, buffer2, length, di->timeout/10000);
	if (i==0) return j;	// we cannot no get more...
    
	for (k=0;k<i;k++) {
	    ringBuffer[insertPosition]=buffer2[k];
	    insertPosition++;
	    insertPosition %= rbSize;
	}
	goto repeat;
    }	    
    
    if (daveDebug & daveDebugByte)
     _daveDump("USB read",buffer,j);	 
    return j;
}

#else
int usbread(daveInterface *di, uc * buffer, int length) {
    int i;
    i=usb_bulk_read(
	(usb_dev_handle *) di->fd.rfd,
	 0x82, buffer, length, di->timeout/10000);
    if (daveDebug & daveDebugByte)
//	if (i>0)
     _daveDump("USB read",buffer,i);	 
    return i;
}	 
#endif
int usbwrite(daveInterface *di, uc * buffer, int length) {
    int i;
//    _daveDump("USB send",buffer,length);
    i=usb_bulk_write(
	(usb_dev_handle *) di->fd.wfd,
	 0x1, buffer, length, di->timeout/10000);
//    LOG2("result: %d\n",i);	 
    return i;	 
}	 

int set_config(usb_dev_handle *handle, int number)
{
  if ( (usb_set_configuration( handle, number)) != USB_OK )
    {
      LOG2( "problem setting config %d\n",number);
      return -1;
    }
  return 0;
}

int claim_interface(usb_dev_handle *handle, int interfaceNumber)
{
  if ( usb_claim_interface( handle, interfaceNumber) != USB_OK )
    {
      LOG2( "problem setting claiming interface %d\n", interfaceNumber );
      return -1;
    }
  return 0;
}

usb_dev_handle * open_device( struct usb_device *device )
{
  usb_dev_handle *handle;

  LOG3( "opening device %s on bus %s\n",
	   device->filename,
	   device->bus->dirname
	   );

  handle = usb_open(device);
  
  if ( !handle )
    {
      LOG1( "open failed\n" );
      return NULL;
    }
    LOG2( "open ok %p\n",handle);
  return handle;
}

usb_dev_handle * scan_bus( struct usb_bus* bus )
{
  struct usb_device* roottree = bus->devices;
  struct usb_device *device;

    
  LOG2( "scanning bus %s\n", bus->dirname );

  
  for( device = roottree;device;device=device->next)
    {
      if ( (idVendor  ==-1 ? 1 : (device->descriptor.idVendor == idVendor ) ) &&
	   (idProduct ==-1 ? 1 : (device->descriptor.idProduct == idProduct ) ) )
	{
	  usb_dev_handle *device_handle;
	  LOG5( "found device %s on bus %s (idVendor 0x%x idProduct 0x%x)\n",
		   device->filename,
		   device->bus->dirname,
		   device->descriptor.idVendor,
		   device->descriptor.idProduct );

	  if ( (device_handle = open_device(device)) )
	    {
		return device_handle;
	    }

	  LOG2( "continuing to scan bus %s\n", bus->dirname );
	}
      else
	LOG3( "device %s on bus %s does not match\n",
		 device->filename,
		 device->bus->dirname);
    }
  
  return NULL;
}

int initUSB() {
  struct usb_bus* bus;
  usb_dev_handle *device_handle;

  usb_init();
  usb_find_busses();
  usb_find_devices();

  LOG1( "doing bus scan for:\n" );
#ifdef WITH_UNTESTED_RING_BUFFER
    insertPosition=0;
    readPosition=0;
#endif

  
  if ( idVendor != -1 )
    LOG2( "\tidVendor 0x%x\n", idVendor );
  else
    LOG1( "\tany idVendor\n" );
  
  if ( idProduct != -1 )
    LOG2( "\tidProduct 0x%x\n", idProduct  );
  else
    LOG1( "\tany idProduct\n" );

  for( bus = usb_busses; bus; bus = bus->next )
    {
	if (daveDebug & daveDebugOpen)
    	    LOG2( "found bus %s\n", bus->dirname );
	device_handle=scan_bus(bus);
	if(device_handle) break;
    }
   set_config(device_handle, 1);
   claim_interface(device_handle,0);
   return(int) device_handle;
}
