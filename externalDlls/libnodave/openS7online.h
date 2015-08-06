/*
 Part of Libnodave, a free communication libray for Siemens S7 300/400.
 
 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2002, 2003.2004

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
#ifndef __openS7online
#define __openS7online


#ifdef __cplusplus
extern "C" {		// All here is C, *** NOT *** C++
#endif

#include "nodave.h"

#ifdef BCCWIN
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#define DECL2 __stdcall
#define us unsigned short
#ifdef DOEXPORT
#define EXPORTSPEC __declspec (dllexport)
#else
#define EXPORTSPEC __declspec (dllimport)
#endif
//EXPORTSPEC HANDLE DECL2 openS7online(const char * accessPoint);
EXPORTSPEC HANDLE DECL2 openS7online(const char * accessPoint, HWND handle);
EXPORTSPEC int DECL2 getS7onlineErr();
EXPORTSPEC HANDLE DECL2 closeS7online(int h);
#endif

#ifdef __cplusplus
}
#endif

#endif

/*
    01/09/07  Used Axel Kinting's version.
*/
