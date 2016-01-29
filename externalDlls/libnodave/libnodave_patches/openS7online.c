
#include "openS7online.h"
#include "log2.h"
#ifdef BCCWIN
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <stdio.h>
#endif

/*
    Access S7onlinx.dll and load pointers to the functions. 
    We load them using GetProcAddress on their names because:
    1. We have no .lib file for them.
    2. We don't want to link with a particular version.
    3. Libnodave shall remain useable without Siemens .dlls. So it shall not try to access them
       unless the user chooses the daveProtoS7online.
*/

extern int daveDebug;

typedef int (DECL2 * _setHWnd) (int, HWND);

EXPORTSPEC HANDLE DECL2 openS7online(const char * accessPoint, HWND handle) {
    HMODULE hmod;
    int h,en;
	_setHWnd SetSinecHWnd; 

    hmod=LoadLibrary("S7onlinx.dll");
    if (daveDebug & daveDebugOpen)
	LOG2("LoadLibrary(S7onlinx.dll) returned %p)\n",hmod);

    SCP_open=GetProcAddress(hmod,"SCP_open");
    if (daveDebug & daveDebugOpen)
    	LOG2("GetProcAddress returned %p)\n",SCP_open);

    SCP_close=GetProcAddress(hmod,"SCP_close");
    if (daveDebug & daveDebugOpen)
	LOG2("GetProcAddress returned %p)\n",SCP_close);

    SCP_get_errno=GetProcAddress(hmod,"SCP_get_errno");
    if (daveDebug & daveDebugOpen)
	LOG2("GetProcAddress returned %p)\n",SCP_get_errno);

    SCP_send=GetProcAddress(hmod,"SCP_send");
    if (daveDebug & daveDebugOpen)
	LOG2("GetProcAddress returned %p)\n",SCP_send);

    SCP_receive=GetProcAddress(hmod,"SCP_receive");
    if (daveDebug & daveDebugOpen)
	LOG2("GetProcAddress returned %p)\n",SCP_receive);
    
    SetSinecHWnd=GetProcAddress(hmod,"SetSinecHWnd");
    if (daveDebug & daveDebugOpen)
	LOG2("GetProcAddress returned %p)\n",SetSinecHWnd);

    en = SCP_get_errno();
    h = SCP_open(accessPoint);
	/*if (h == -1)
	{
		en = SCP_get_errno();
		return en * -1;
	}*/
    LOG3("handle: %d  error:%d\n", h, en);
	SetSinecHWnd(h, handle);
    return h;
};

EXPORTSPEC int DECL2 getS7onlineErr()
{
    int en = SCP_get_errno();
    return en;
};
   
/*
LRESULT CALLBACK HookProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    //if (nCode >= 0) können wir uns sparen, da nCode eh niemals kleiner 0 sein wird!
    /*MOUSEHOOKSTRUCT mhs = *(MOUSEHOOKSTRUCT*)lParam;
    LONG lPoint = MAKELONG((WORD)mhs.pt.x, (WORD)mhs.pt.y);
    HWND MyWin = FindWindow("TForm1", "Form1");
    if (MyWin != NULL) //Das Fenster an das die Botschaft geschickt wird.
        SendMessage(MyWin, WM_MOUSEMOVE, wParam, (LPARAM)lPoint);
    return CallNextHookEx(0, nCode, wParam, lParam);*/
/*}

__declspec(dllexport) BOOL UninstallHook(HHOOK hook)
{
    return UnhookWindowsHookEx(hook); //Hook deinstallieren.
}

__declspec(dllexport) HHOOK InstallHook(HHOOK hook)
{    //Hook installieren.
    return SetWindowsHookEx(WM_SINEC, (HOOKPROC)HookProc, HInstance, 0);
}*/


EXPORTSPEC HANDLE DECL2 closeS7online(int h) {
    SCP_close(h);
}

/*
    01/09/07  Used Axel Kinting's version.
*/
