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
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/

#ifndef setport__
#define setport__

#ifdef __cplusplus  
    extern "C" {        
#endif              

#ifdef BCCWIN

#ifdef DOEXPORT
#define EXPORTSPEC __declspec (dllexport)
#else
#define EXPORTSPEC __declspec (dllimport)
#endif

EXPORTSPEC HANDLE __stdcall setPort(char * name, char* baud,char parity);

EXPORTSPEC int __stdcall closePort(HANDLE port);
#endif

#ifdef LINUX
int setPort(char * name, char* baud, char parity);

int closePort(int port);
#endif

#ifdef __cplusplus
    }
#endif

#endif // setport__
/*
    01/08/07  Put __cplusplus directive as where suggested by Keith Harrison.
*/
