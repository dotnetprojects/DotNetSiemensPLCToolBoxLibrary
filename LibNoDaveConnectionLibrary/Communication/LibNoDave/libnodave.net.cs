/*
 This implements a "glue" layer between libnodave_jfkmod.dll and applications written
 in MS .Net languages.
 
 Part of Libnodave, a free communication libray for Siemens S7 200/300/400 via
 the MPI adapter 6ES7 972-0CA22-0XAC
 or  MPI adapter 6ES7 972-0CA23-0XAC
 or  TS adapter 6ES7 972-0CA33-0XAC
 or  MPI adapter 6ES7 972-0CA11-0XAC,
 IBH/MHJ-NetLink or CPs 243, 343 and 443
 or VIPA Speed7 with builtin ethernet support.
  
 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2002..2005

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

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave
{
    using System.Text.RegularExpressions;

    /*internal*/
    public class libnodave
    {
        /*
            This struct contains whatever your Operating System uses to hold an in and outgoing 
            connection to external devices.
        */
        public struct daveOSserialType
        {
            public volatile IntPtr rfd;
            public volatile IntPtr wfd;
        }
        /*
            Protocol types to be used with new daveInterface:
        */
        public static readonly int daveProtoMPI = 0;	/* MPI for S7 300/400 */
        public static readonly int daveProtoMPI2 = 1;	/* MPI for S7 300/400, "Andrew's version" */
        public static readonly int daveProtoMPI3 = 2;	/* MPI for S7 300/400, Step 7 Version, experimental */
        public static readonly int daveProtoMPI4 = 3;	/* MPI for S7 300/400, "Andrew's version" with STX */
        public static readonly int daveProtoPPI = 10;	/* PPI for S7 200 */

        public static readonly int daveProtoAS511 = 20;	/* S5 via programming interface */
        public static readonly int daveProtoS7online = 50;	/* use s7onlinx.dll for transport */

        public static readonly int daveProtoISOTCP = 122;	/* ISO over TCP */
        public static readonly int daveProtoISOTCP243 = 123;	/* ISO over TCP with CP243 */

        public static readonly int daveProtoMPI_IBH = 223;	/* MPI with IBH NetLink MPI to ethernet gateway */
        public static readonly int daveProtoPPI_IBH = 224;	/* PPI with IBH NetLink PPI to ethernet gateway */

        public static readonly int daveProtoUserTransport = 255;	/* Libnodave will pass the PDUs of */
        /* S7 Communication to user defined */
        /* call back functions. */
        /*
         *    ProfiBus speed constants. This is the baudrate on MPI network, NOT between adapter and PC:
        */
        public static readonly int daveSpeed9k = 0;
        public static readonly int daveSpeed19k = 1;
        public static readonly int daveSpeed187k = 2;
        public static readonly int daveSpeed500k = 3;
        public static readonly int daveSpeed1500k = 4;
        public static readonly int daveSpeed45k = 5;
        public static readonly int daveSpeed93k = 6;

        /*
            Some function codes (yet unused ones may be incorrect).
        */
        public static readonly int daveFuncOpenS7Connection = 0xF0;
        public static readonly int daveFuncRead = 0x04;
        public static readonly int daveFuncWrite = 0x05;
        public static readonly int daveFuncRequestDownload = 0x1A;
        public static readonly int daveFuncDownloadBlock = 0x1B;
        public static readonly int daveFuncDownloadEnded = 0x1C;
        public static readonly int daveFuncStartUpload = 0x1D;
        public static readonly int daveFuncUpload = 0x1E;
        public static readonly int daveFuncEndUpload = 0x1F;
        public static readonly int daveFuncInsertBlock = 0x28;
        /*
            S7 specific constants:
        */
        public static readonly int daveBlockType_OB = '8';
        public static readonly int daveBlockType_DB = 'A';
        public static readonly int daveBlockType_SDB = 'B';
        public static readonly int daveBlockType_FC = 'C';
        public static readonly int daveBlockType_SFC = 'D';
        public static readonly int daveBlockType_FB = 'E';
        public static readonly int daveBlockType_SFB = 'F';
        /*
            Use these constants for parameter "area" in daveReadBytes and daveWriteBytes
        */
        public static readonly int daveSysInfo = 0x3;	/* System info of 200 family */
        public static readonly int daveSysFlags = 0x5;	/* System flags of 200 family */
        public static readonly int daveAnaIn = 0x6;	/* analog inputs of 200 family */
        public static readonly int daveAnaOut = 0x7;	/* analog outputs of 200 family */
        public static readonly int daveP = 0x80;    	/* direct peripheral access */
        public static readonly int daveInputs = 0x81;
        public static readonly int daveOutputs = 0x82;
        public static readonly int daveFlags = 0x83;
        public static readonly int daveDB = 0x84;		/* data blocks */
        public static readonly int daveDI = 0x85;	/* instance data blocks */
        public static readonly int daveLocal = 0x86; 	/* not tested */
        public static readonly int daveV = 0x87;	/* don't know what it is */
        public static readonly int daveCounter = 28;	/* S7 counters */
        public static readonly int daveTimer = 29;	/* S7 timers */
        public static readonly int daveCounter200 = 30;	/* IEC counters (200 family) */
        public static readonly int daveTimer200 = 31;	/* IEC timers (200 family) */
        /**
            Library specific:
        **/
        /*
            Result codes. Genarally, 0 means ok, 
            >0 are results (also errors) reported by the PLC
            <0 means error reported by library code.
        */
        public static readonly int daveResOK = 0;			/* means all ok */
        public static readonly int daveResNoPeripheralAtAddress = 1;	/* CPU tells there is no peripheral at address */
        public static readonly int daveResMultipleBitsNotSupported = 6; /* CPU tells it does not support to read a bit block with a */
        /* length other than 1 bit. */
        public static readonly int daveResItemNotAvailable200 = 3;	/* means a a piece of data is not available in the CPU, e.g. */
        /* when trying to read a non existing DB or bit bloc of length<>1 */
        /* This code seems to be specific to 200 family. */

        public static readonly int daveResItemNotAvailable = 10;	/* means a a piece of data is not available in the CPU, e.g. */
        /* when trying to read a non existing DB */

        public static readonly int daveAddressOutOfRange = 5;		/* means the data address is beyond the CPUs address range */
        public static readonly int daveWriteDataSizeMismatch = 7;	/* means the write data size doesn't fit item size */
        public static readonly int daveResCannotEvaluatePDU = -123;     /* PDU is not understood by libnodave */
        public static readonly int daveResCPUNoData = -124;
        public static readonly int daveUnknownError = -125;
        public static readonly int daveEmptyResultError = -126;
        public static readonly int daveEmptyResultSetError = -127;
        public static readonly int daveResUnexpectedFunc = -128;
        public static readonly int daveResUnknownDataUnitSize = -129;

        public static readonly int daveResShortPacket = -1024;
        public static readonly int daveResTimeout = -1025;
        /*
            Error code to message string conversion:
            Call this function to get an explanation for error codes returned by other functions.
        */
        /*
            [DllImport("libnodave_jfkmod.dll")]
            public static extern string 
            daveStrerror(int res);
        */

#if !IPHONE
        [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveStrerror")]
#else
        [DllImport("__Internal", EntryPoint = "daveStrerror")]
#endif
        private static extern IntPtr
            _daveStrerror64(int res);

#if !IPHONE	
        [DllImport("libnodave_jfkmod.dll", EntryPoint="daveStrerror" )]
#else
        [DllImport("__Internal", EntryPoint = "daveStrerror")]
#endif
        private static extern IntPtr
            _daveStrerror32(int res);

        public static string daveStrerror(int res)
        {
            if (IntPtr.Size == 8)
                return Marshal.PtrToStringAnsi(_daveStrerror64(res));
            return Marshal.PtrToStringAnsi(_daveStrerror32(res));
        }


        //Get's an Error for the Errors from S7 Online!
        public static string daveStrS7onlineError()
        {
            int err = SCP_get_errno();
            switch (err)
            {
                case 202:
                    return "S7Online: Ressourcenengpaß im Treiber oder in der Library";
                case 203:
                    return "S7Online: Konfigurationsfehler";
                case 205:
                    return "S7Online: Auftrag zur Zeit nicht erlaubt";
                case 206:
                    return "S7Online: Parameterfehler";
                case 207:
                    return "S7Online: Gerät bereits/noch nicht geöffnet.";
                case 208:
                    return "S7Online: CP reagiert nicht";
                case 209:
                    return "S7Online: Fehler in der Firmware";
                case 210:
                    return "S7Online: Speicherengpaß im Treiber";
                case 215:
                    return "S7Online: Keine Nachricht vorhanden";
                case 216:
                    return "S7Online: Fehler bei Zugriff auf Anwendungspuffer";
                case 219:
                    return "S7Online: Timeout abgelaufen";
                case 225:
                    return "S7Online: Die maximale Anzahl an Anmeldungen ist überschritten";
                case 226:
                    return "S7Online: Der Auftrag wurde abgebrochen";
                case 233:
                    return "S7Online: Ein Hilfsprogramm konnte nicht gestartet werden";
                case 234:
                    return "S7Online: Keine Autorisierung für diese Funktion vorhanden";
                case 304:
                    return "S7Online: Initialisierung noch nicht abgeschlossen";
                case 305:
                    return "S7Online: Funktion nicht implementiert";
                case 4865:
                    return "S7Online: CP-Name nicht vorhanden";
                case 4866:
                    return "S7Online: CP-Name nicht konfiguriert";
                case 4867:
                    return "S7Online: Kanalname nicht vorhanden";
                case 4868:
                    return "S7Online: Kanalname nicht konfiguriert";
            }
            if (err != 0)
                return "Fehler nicht definiert, Code: " + err.ToString();
            else
                return "Kein Fehler";
        }


        /*
            Copy an internal String into an external string buffer. This is needed to interface
            with Visual Basic. Maybe it is helpful elsewhere, too.
            C# can well work with C strings.
        */
        //EXPORTSPEC void DECL2 daveStringCopy(char * intString, char * extString);

        /* 
            Max number of bytes in a single message. 
        */
        public static readonly int daveMaxRawLen = 2048;

        /*
            Some definitions for debugging:
        */
        public static readonly int daveDebugRawRead = 0x01;	/* Show the single bytes received */
        public static readonly int daveDebugSpecialChars = 0x02;	/* Show when special chars are read */
        public static readonly int daveDebugRawWrite = 0x04;	/* Show the single bytes written */
        public static readonly int daveDebugListReachables = 0x08;	/* Show the steps when determine devices in MPI net */
        public static readonly int daveDebugInitAdapter = 0x10;	/* Show the steps when Initilizing the MPI adapter */
        public static readonly int daveDebugConnect = 0x20;	/* Show the steps when connecting a PLC */
        public static readonly int daveDebugPacket = 0x40;
        public static readonly int daveDebugByte = 0x80;
        public static readonly int daveDebugCompare = 0x100;
        public static readonly int daveDebugExchange = 0x200;
        public static readonly int daveDebugPDU = 0x400;	/* debug PDU handling */
        public static readonly int daveDebugUpload = 0x800;	/* debug PDU loading program blocks from PLC */
        public static readonly int daveDebugMPI = 0x1000;
        public static readonly int daveDebugPrintErrors = 0x2000;	/* Print error messages */
        public static readonly int daveDebugPassive = 0x4000;

        public static readonly int daveDebugErrorReporting = 0x8000;
        public static readonly int daveDebugOpen = 0x10000;

        public static readonly int daveDebugAll = 0x1ffff;
        /*
            set and read debug level:
        */

#if !IPHONE	
        [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveSetDebug")]
#else
        [DllImport("__Internal", EntryPoint = "daveSetDebug")]
#endif
        public static extern void daveSetDebug64(int newDebugLevel);

#if !IPHONE	
        [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveSetDebug")]
#else
        [DllImport("__Internal", EntryPoint = "daveSetDebug")]
#endif
        public static extern void daveSetDebug32(int newDebugLevel);

        public static void daveSetDebug(int newDebugLevel)
        {
            if (IntPtr.Size == 8)
                daveSetDebug64(newDebugLevel);
            else
                daveSetDebug32(newDebugLevel);
        }


#if !IPHONE
        [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveGetDebug")]
#else
        [DllImport("__Internal", EntryPoint = "daveGetDebug")]
#endif
        public static extern int daveGetDebug64();

#if !IPHONE	
        [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveGetDebug")]
#else
        [DllImport("__Internal", EntryPoint = "daveGetDebug")]
#endif
        public static extern int daveGetDebug32();

        public static int daveGetDebug()
        {
            if (IntPtr.Size == 8)
                return daveGetDebug64();
            return daveGetDebug32();
        }

        public static int daveMPIReachable = 0x30;
        public static int daveMPIunused = 0x10;
        public static int davePartnerListSize = 126;

        /*
            This wrapper class is used to avoid dealing with "unsafe" pointers to libnodave
            internal structures. More wrapper classes are derived from this for the different 
            structures. Constructors of derived classes will call functions in libnodave that 
            allocate internal structures via malloc. The functions used return integers by 
            declaration. These integers are stored in "pointer" In fact, these integers contain 
            the "bit patterns" of the pointers. The compiler is deceived about the real nature of 
            the return values. This is ok as long as the pointers are only used in libnodave, 
            because libnodave routines are assumed to know what they may do with them.
            The destructor here passes the pointers back to libnodave's daveFree to release memory
            when the C# object is destructed.
        */
        public class pseudoPointer
        {
            public IntPtr pointer;
#if !IPHONE
        [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveFree")]
#else
            [DllImport("__Internal", EntryPoint = "daveFree")]
#endif
            protected static extern int daveFree64(IntPtr p);

#if !IPHONE	
        [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveFree")]
#else
            [DllImport("__Internal", EntryPoint = "daveFree")]
#endif
            protected static extern int daveFree32(IntPtr p);

            ~pseudoPointer()
            {
                //	    Console.WriteLine("~pseudoPointer()"+pointer);
                if (IntPtr.Size == 8)
                    daveFree64(pointer);
                else
                    daveFree32(pointer);
            }

        }

        public class daveInterface : pseudoPointer
        {

            //	[DllImport("libnodave_jfkmod.dll"//, PreserveSig=false)]
            /*
                I cannot say why, but when I recompiled the existing code with latest libnodave_jfkmod.dll
                (after using stdcall so that VC++ producs these "decorated names", I got a runtime
                error about not finding daveNewInterface. When I state full name entry point explicitly,
                (like below) it runs. The most strange thing is that all other functions work well...
            */
#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveNewInterface")]
#else
            [DllImport("__Internal", EntryPoint = "daveNewInterface")]
#endif
            private static extern IntPtr daveNewInterface64(daveOSserialType fd, [MarshalAs(UnmanagedType.LPStr)] string name, int localMPI, int useProto, int speed);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveNewInterface")]
#else
            [DllImport("__Internal", EntryPoint = "daveNewInterface")]
#endif
            private static extern IntPtr daveNewInterface32(daveOSserialType fd, [MarshalAs(UnmanagedType.LPStr)] string name, int localMPI, int useProto, int speed);


            public daveInterface(daveOSserialType fd, string name, int localMPI, int useProto, int speed)
            {
                if (IntPtr.Size == 8)
                    pointer = daveNewInterface64(fd, name, localMPI, useProto, speed);
                else
                    pointer = daveNewInterface32(fd, name, localMPI, useProto, speed);
            }

            /*
            This was just here to check inheritance	
                ~daveInterface(){
                    Console.WriteLine("destructor("+daveGetName(pointer)+")");
                    Console.WriteLine("~daveInterface()"+pointer);
                    daveFree(pointer);
                }
            */

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveInitAdapter")]
#else
            [DllImport("__Internal", EntryPoint = "daveInitAdapter")]
#endif
            protected static extern int daveInitAdapter64(IntPtr di);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveInitAdapter")]
#else
            [DllImport("__Internal", EntryPoint = "daveInitAdapter")]
#endif
            protected static extern int daveInitAdapter32(IntPtr di);
            public int initAdapter()
            {
                if (IntPtr.Size == 8)
                    return daveInitAdapter64(pointer);
                return daveInitAdapter32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveListReachablePartners")]
#else
            [DllImport("__Internal", EntryPoint = "daveListReachablePartners")]
#endif
            protected static extern int daveListReachablePartners64(IntPtr di, byte[] buffer);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveListReachablePartners")]
#else
            [DllImport("__Internal", EntryPoint = "daveListReachablePartners")]
#endif
            protected static extern int daveListReachablePartners32(IntPtr di, byte[] buffer);
            public int listReachablePartners(byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    return daveListReachablePartners64(pointer, buffer);
                return daveListReachablePartners32(pointer, buffer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveSetTimeout")]
#else
            [DllImport("__Internal", EntryPoint = "daveSetTimeout")]
#endif
            protected static extern void daveSetTimeout64(IntPtr di, int time);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveSetTimeout")]
#else
            [DllImport("__Internal", EntryPoint = "daveSetTimeout")]
#endif
            protected static extern void daveSetTimeout32(IntPtr di, int time);
            public void setTimeout(int time)
            {
                if (IntPtr.Size == 8)
                    daveSetTimeout64(pointer, time);
                else
                    daveSetTimeout32(pointer, time);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveGetTimeout")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetTimeout")]
#endif
            protected static extern int daveGetTimeout64(IntPtr di);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveGetTimeout")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetTimeout")]
#endif
            protected static extern int daveGetTimeout32(IntPtr di);
            public int getTimeout()
            {
                if (IntPtr.Size == 8)
                    return daveGetTimeout64(pointer);
                return daveGetTimeout32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveDisconnectAdapter")]
#else
            [DllImport("__Internal", EntryPoint = "daveDisconnectAdapter")]
#endif
            protected static extern IntPtr daveDisconnectAdapter64(IntPtr di);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveDisconnectAdapter")]
#else
            [DllImport("__Internal", EntryPoint = "daveDisconnectAdapter")]
#endif
            protected static extern IntPtr daveDisconnectAdapter32(IntPtr di);
            public IntPtr disconnectAdapter()
            {
                if (IntPtr.Size == 8)
                    return daveDisconnectAdapter64(pointer);
                return daveDisconnectAdapter32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveGetName")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetName")]
#endif
            protected static extern string daveGetName64(IntPtr di);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveGetName")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetName")]
#endif
            protected static extern string daveGetName32(IntPtr di);
            public string getName()
            {
                if (IntPtr.Size == 8)
                    return daveGetName64(pointer);
                return daveGetName32(pointer);
            }

        }

        public class daveConnection : pseudoPointer
        {
#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveNewConnection")]
#else
            [DllImport("__Internal", EntryPoint = "daveNewConnection")]
#endif
            protected static extern IntPtr daveNewConnection64(IntPtr di, int MPI, int rack, int slot);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveNewConnection")]
#else
            [DllImport("__Internal", EntryPoint = "daveNewConnection")]
#endif
            protected static extern IntPtr daveNewConnection32(IntPtr di, int MPI, int rack, int slot);


            public daveConnection(daveInterface di, int MPI, int rack, int slot)
            {
                if (IntPtr.Size == 8)
                    pointer = daveNewConnection64(di.pointer, MPI, rack, slot);
                else
                    pointer = daveNewConnection32(di.pointer, MPI, rack, slot);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveNewExtendedConnection")]
#else
            [DllImport("__Internal", EntryPoint = "daveNewExtendedConnection")]
#endif
            protected static extern IntPtr daveNewExtendedConnection64(IntPtr di, byte[] destination, int DestinationIsIP, int rack, int slot, int routing, int routingSubnetFirst, int routingSubnetSecond, int routingRack, int routingSlot, byte[] routingDestination, int routingDestinationIsIP, int ConnectionType, int routingConnectionType);


#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveNewExtendedConnection")]
#else
            [DllImport("__Internal", EntryPoint = "daveNewExtendedConnection")]
#endif
            protected static extern IntPtr daveNewExtendedConnection32(IntPtr di, byte[] destination, int DestinationIsIP, int rack, int slot, int routing, int routingSubnetFirst, int routingSubnetSecond, int routingRack, int routingSlot, byte[] routingDestination, int routingDestinationIsIP, int ConnectionType, int routingConnectionType);


            public daveConnection(daveInterface di, int MPI, string IP, bool DestinationIsIP, int rack, int slot, bool routing, int routingSubnetFirst, int routingSubnetSecond, int routingRack, int routingSlot, string routingDestination, int PLCConnectionType, int routingPLCConnectionType)
            {
                string[] ip = IP.Split('.');
                byte[] myDestination;
                int myDestinationIsIP = 0;
                if (ip.Length < 4 || !DestinationIsIP)
                {
                    myDestination = new byte[] { (byte)Convert.ToInt32(MPI) };
                }
                else
                {
                    myDestinationIsIP = 1;
                    myDestination = new byte[] { (byte)Convert.ToInt32(ip[0]), (byte)Convert.ToInt32(ip[1]), (byte)Convert.ToInt32(ip[2]), (byte)Convert.ToInt32(ip[3]) };
                }
                routingDestination = routingDestination ?? "";
                ip = routingDestination.Split('.');
                byte[] myRoutingDestination = new byte[4];
                int routingDestinationIsIP = 0;
                if (ip.Length < 4)
                {
                    try
                    { myRoutingDestination = new byte[] { (byte)Convert.ToInt32(routingDestination) }; }
                    catch (Exception)
                    { }
                }
                else
                {
                    routingDestinationIsIP = 1;
                    string[] rip = routingDestination.Split('.');
                    if (rip.Length == 4)
                        myRoutingDestination = new byte[] { (byte)Convert.ToInt32(rip[0]), (byte)Convert.ToInt32(rip[1]), (byte)Convert.ToInt32(rip[2]), (byte)Convert.ToInt32(rip[3]) };
                }
                if (IntPtr.Size == 8)
                    pointer = daveNewExtendedConnection64(di.pointer, myDestination, myDestinationIsIP, rack, slot, Convert.ToInt32(routing), routingSubnetFirst, routingSubnetSecond, routingRack, routingSlot, myRoutingDestination, routingDestinationIsIP, PLCConnectionType, routingPLCConnectionType);
                else
                    pointer = daveNewExtendedConnection32(di.pointer, myDestination, myDestinationIsIP, rack, slot, Convert.ToInt32(routing), routingSubnetFirst, routingSubnetSecond, routingRack, routingSlot, myRoutingDestination, routingDestinationIsIP, PLCConnectionType, routingPLCConnectionType);
            }

            /* This wa here to test inheritance
                ~daveConnection(){
                    Console.WriteLine("~daveConnection()"+pointer);
                    daveFree(pointer);
                    daveFree(pointer);
                }
            */
#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveConnectPLC")]
#else
            [DllImport("__Internal", EntryPoint = "daveConnectPLC")]
#endif
            protected static extern int daveConnectPLC64(IntPtr dc);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveConnectPLC")]
#else
            [DllImport("__Internal", EntryPoint = "daveConnectPLC")]
#endif
            protected static extern int daveConnectPLC32(IntPtr dc);
            public int connectPLC()
            {
                if (IntPtr.Size == 8)
                    return daveConnectPLC64(pointer);
                else
                    return daveConnectPLC32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveDisconnectPLC")]
#else
            [DllImport("__Internal", EntryPoint = "daveDisconnectPLC")]
#endif
            protected static extern int daveDisconnectPLC64(IntPtr dc);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveDisconnectPLC")]
#else
            [DllImport("__Internal", EntryPoint = "daveDisconnectPLC")]
#endif
            protected static extern int daveDisconnectPLC32(IntPtr dc);
            public int disconnectPLC()
            {
                if (IntPtr.Size == 8)
                    return daveDisconnectPLC64(pointer);
                return daveDisconnectPLC32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveReadBytes")]
#else
            [DllImport("__Internal", EntryPoint = "daveReadBytes")]
#endif
            protected static extern int daveReadBytes64(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveReadBytes")]
#else
            [DllImport("__Internal", EntryPoint = "daveReadBytes")]
#endif
            protected static extern int daveReadBytes32(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            public int readBytes(int area, int DBnumber, int start, int len, byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    return daveReadBytes64(pointer, area, DBnumber, start, len, buffer);
                return daveReadBytes32(pointer, area, DBnumber, start, len, buffer);
            }


            //[DllImport("libnodave_jfkmod.dll"/*, PreserveSig=false */ )]
            //    protected static extern int daveReadManyBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            //public int readManyBytes(int area, int DBnumber, int start, int len, byte[] buffer) {
            //    return daveReadManyBytes(pointer, area, DBnumber, start, len, buffer);
            //}

            //Inserted a .NET implementation of readManyBytes, because the libNoDave one did not work!
            public int readManyBytes(int area, int DBnumber, int start, int len, ref byte[] buffer)
            {
                int res, readLen;
                int pos = 0;

                int pdulen = getMaxPDULen();

                while (len > 0)
                {
                    if (len > pdulen - 18) readLen = pdulen - 18; else readLen = len;

                    byte[] tmp = new byte[readLen];

                    res = readBytes(area, DBnumber, start + pos, readLen, tmp);
                    if (res != 0) return res;

                    tmp.CopyTo(buffer, pos);

                    len -= readLen;
                    pos += readLen;
                }

                return 0;
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveReadBits")]
#else
            [DllImport("__Internal", EntryPoint = "daveReadBits")]
#endif
            protected static extern int daveReadBits64(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveReadBits")]
#else
            [DllImport("__Internal", EntryPoint = "daveReadBits")]
#endif
            protected static extern int daveReadBits32(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            public int readBits(int area, int DBnumber, int start, int len, byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    return daveReadBits64(pointer, area, DBnumber, start, len, buffer);
                return daveReadBits32(pointer, area, DBnumber, start, len, buffer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveWriteBytes")]
#else
            [DllImport("__Internal", EntryPoint = "daveWriteBytes")]
#endif
            protected static extern int daveWriteBytes64(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveWriteBytes")]
#else
            [DllImport("__Internal", EntryPoint = "daveWriteBytes")]
#endif
            protected static extern int daveWriteBytes32(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            public int writeBytes(int area, int DBnumber, int start, int len, byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    return daveWriteBytes64(pointer, area, DBnumber, start, len, buffer);
                return daveWriteBytes32(pointer, area, DBnumber, start, len, buffer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveWriteManyBytes")]
#else
            [DllImport("__Internal", EntryPoint = "daveWriteManyBytes")]
#endif
            protected static extern int daveWriteManyBytes64(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveWriteManyBytes")]
#else
            [DllImport("__Internal", EntryPoint = "daveWriteManyBytes")]
#endif
            protected static extern int daveWriteManyBytes32(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            public int writeManyBytes(int area, int DBnumber, int start, int len, byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    return daveWriteManyBytes64(pointer, area, DBnumber, start, len, buffer);
                return daveWriteManyBytes32(pointer, area, DBnumber, start, len, buffer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveWriteBits")]
#else
            [DllImport("__Internal", EntryPoint = "daveWriteBits")]
#endif
            protected static extern int daveWriteBits64(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveWriteBits")]
#else
            [DllImport("__Internal", EntryPoint = "daveWriteBits")]
#endif
            protected static extern int daveWriteBits32(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            public int writeBits(int area, int DBnumber, int start, int len, byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    return daveWriteBits64(pointer, area, DBnumber, start, len, buffer);
                return daveWriteBits32(pointer, area, DBnumber, start, len, buffer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveBuildAndSendPDU")]
#else
            [DllImport("__Internal", EntryPoint = "daveBuildAndSendPDU")]
#endif
            protected static extern int daveBuildAndSendPDU64(IntPtr dc, IntPtr p, byte[] b1, int l1, byte[] b2, int l2);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveBuildAndSendPDU")]
#else
            [DllImport("__Internal", EntryPoint = "daveBuildAndSendPDU")]
#endif
            protected static extern int daveBuildAndSendPDU32(IntPtr dc, IntPtr p, byte[] b1, int l1, byte[] b2, int l2);
            public int daveBuildAndSendPDU(PDU myPDU, byte[] Parameter, byte[] Data)
            {
                int res = 0;
                if (IntPtr.Size == 8)
                    res = daveBuildAndSendPDU64(pointer, myPDU.pointer, Parameter, Parameter.Length, Data, Data.Length);
                else
                    res = daveBuildAndSendPDU32(pointer, myPDU.pointer, Parameter, Parameter.Length, Data, Data.Length);
                //return p;
                return res;
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveGetU8")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetU8")]
#endif
            protected static extern int daveGetU8_64(IntPtr dc);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveGetU8")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetU8")]
#endif
            protected static extern int daveGetU8_32(IntPtr dc);
            public int getU8()
            {
                if (IntPtr.Size == 8)
                    return daveGetU8_64(pointer);
                return daveGetU8_32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveGetAnswLen")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetAnswLen")]
#endif
            protected static extern int daveGetAnswLen64(IntPtr dc);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveGetAnswLen")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetAnswLen")]
#endif
            protected static extern int daveGetAnswLen32(IntPtr dc);
            public int getAnswLen()
            {
                if (IntPtr.Size == 8)
                    return daveGetAnswLen64(pointer);
                return daveGetAnswLen32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveGetMaxPDULen")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetMaxPDULen")]
#endif
            protected static extern int daveGetMaxPDULen64(IntPtr dc);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveGetMaxPDULen")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetMaxPDULen")]
#endif
            protected static extern int daveGetMaxPDULen32(IntPtr dc);
            public int getMaxPDULen()
            {
                if (IntPtr.Size == 8)
                    return daveGetMaxPDULen64(pointer);
                return daveGetMaxPDULen32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "davePrepareReadRequest")]
#else
            [DllImport("__Internal", EntryPoint = "davePrepareReadRequest")]
#endif
            protected static extern int davePrepareReadRequest64(IntPtr dc, IntPtr p);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "davePrepareReadRequest")]
#else
            [DllImport("__Internal", EntryPoint = "davePrepareReadRequest")]
#endif
            protected static extern int davePrepareReadRequest32(IntPtr dc, IntPtr p);
            public PDU prepareReadRequest()
            {
                PDU p = new PDU();
                if (IntPtr.Size == 8)
                    davePrepareReadRequest64(pointer, p.pointer);
                else
                    davePrepareReadRequest32(pointer, p.pointer);
                return p;
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "davePrepareWriteRequest")]
#else
            [DllImport("__Internal", EntryPoint = "davePrepareWriteRequest")]
#endif
            protected static extern int davePrepareWriteRequest64(IntPtr dc, IntPtr p);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "davePrepareWriteRequest")]
#else
            [DllImport("__Internal", EntryPoint = "davePrepareWriteRequest")]
#endif
            protected static extern int davePrepareWriteRequest32(IntPtr dc, IntPtr p);
            public PDU prepareWriteRequest()
            {
                PDU p = new PDU();
                if (IntPtr.Size == 8)
                    davePrepareWriteRequest64(pointer, p.pointer);
                else
                    davePrepareWriteRequest32(pointer, p.pointer);
                return p;
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveExecReadRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveExecReadRequest")]
#endif
            protected static extern int daveExecReadRequest64(IntPtr dc, IntPtr p, IntPtr rl);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveExecReadRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveExecReadRequest")]
#endif
            protected static extern int daveExecReadRequest32(IntPtr dc, IntPtr p, IntPtr rl);
            public int execReadRequest(PDU p, resultSet rl)
            {
                if (IntPtr.Size == 8)
                    return daveExecReadRequest64(pointer, p.pointer, rl.pointer);
                return daveExecReadRequest32(pointer, p.pointer, rl.pointer);
            }


#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveExecWriteRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveExecWriteRequest")]
#endif
            protected static extern int daveExecWriteRequest64(IntPtr dc, IntPtr p, IntPtr rl);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveExecWriteRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveExecWriteRequest")]
#endif
            protected static extern int daveExecWriteRequest32(IntPtr dc, IntPtr p, IntPtr rl);
            public int execWriteRequest(PDU p, resultSet rl)
            {
                if (IntPtr.Size == 8)
                    return daveExecWriteRequest64(pointer, p.pointer, rl.pointer);
                return daveExecWriteRequest32(pointer, p.pointer, rl.pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveUseResult")]
#else
            [DllImport("__Internal", EntryPoint = "daveUseResult")]
#endif
            protected static extern int daveUseResult64(IntPtr dc, IntPtr rs, int number, byte[] buffer);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveUseResult")]
#else
            [DllImport("__Internal", EntryPoint = "daveUseResult")]
#endif
            protected static extern int daveUseResult32(IntPtr dc, IntPtr rs, int number, byte[] buffer);
            public int useResult(resultSet rs, int number, byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    return daveUseResult64(pointer, rs.pointer, number, buffer);
                return daveUseResult32(pointer, rs.pointer, number, buffer);
            }


#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveReadSZL")]
#else
            [DllImport("__Internal", EntryPoint = "daveReadSZL")]
#endif
            protected static extern int daveReadSZL64(IntPtr dc, int id, int index, byte[] buffer, int len);
#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveReadSZL")]
#else
            [DllImport("__Internal", EntryPoint = "daveReadSZL")]
#endif
            protected static extern int daveReadSZL32(IntPtr dc, int id, int index, byte[] buffer, int len);

            public int readSZL(int id, int index, byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    return daveReadSZL64(pointer, id, index, buffer, buffer.Length);
                return daveReadSZL32(pointer, id, index, buffer, buffer.Length);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveStart")]
#else
            [DllImport("__Internal", EntryPoint = "daveStart")]
#endif
            protected static extern int daveStart64(IntPtr dc);
#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveStart")]
#else
            [DllImport("__Internal", EntryPoint = "daveStart")]
#endif
            protected static extern int daveStart32(IntPtr dc);

            public int start()
            {
                if (IntPtr.Size == 8)
                    return daveStart64(pointer);
                else
                    return daveStart32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveStop")]
#else
            [DllImport("__Internal", EntryPoint = "daveStop")]
#endif
            protected static extern int daveStop64(IntPtr dc);
#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveStop")]
#else
            [DllImport("__Internal", EntryPoint = "daveStop")]
#endif
            protected static extern int daveStop32(IntPtr dc);

            public int stop()
            {
                if (IntPtr.Size == 8)
                    return daveStop64(pointer);
                else
                    return daveStop32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveForce200")]
#else
            [DllImport("__Internal", EntryPoint = "daveForce200")]
#endif
            protected static extern int daveForce200_64(IntPtr dc, int area, int start, int val);
#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveForce200")]
#else
            [DllImport("__Internal", EntryPoint = "daveForce200")]
#endif
            protected static extern int daveForce200_32(IntPtr dc, int area, int start, int val);

            public int force200(int area, int start, int val)
            {
                if (IntPtr.Size == 8)
                    return daveForce200_64(pointer, area, start, val);
                else
                    return daveForce200_32(pointer, area, start, val);
            }


#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveForceDisconnectIBH")]
#else
            [DllImport("__Internal", EntryPoint = "daveForceDisconnectIBH")]
#endif
            protected static extern int daveForceDisconnectIBH64(IntPtr dc, int src, int dest, int MPI);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveForceDisconnectIBH")]
#else
            [DllImport("__Internal", EntryPoint = "daveForceDisconnectIBH")]
#endif
            protected static extern int daveForceDisconnectIBH32(IntPtr dc, int src, int dest, int MPI);
            public int forceDisconnectIBH(int src, int dest, int MPI)
            {
                if (IntPtr.Size == 8)
                    return daveForceDisconnectIBH64(pointer, src, dest, MPI);
                else
                    return daveForceDisconnectIBH32(pointer, src, dest, MPI);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveResetIBH")]
#else
            [DllImport("__Internal", EntryPoint = "daveResetIBH")]
#endif
            protected static extern int daveResetIBH64(IntPtr dc);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveResetIBH")]
#else
            [DllImport("__Internal", EntryPoint = "daveResetIBH")]
#endif
            protected static extern int daveResetIBH32(IntPtr dc);

            public int resetIBH()
            {
                if (IntPtr.Size == 8)
                    return daveResetIBH64(pointer);
                else
                    return daveResetIBH32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveGetResponse")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetResponse")]
#endif
            protected static extern int daveGetResponse64(IntPtr dc);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveGetResponse")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetResponse")]
#endif
            protected static extern int daveGetResponse32(IntPtr dc);
            public int getGetResponse()
            {
                if (IntPtr.Size == 8)
                    return daveGetResponse64(pointer);
                else
                    return daveGetResponse32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveSendMessage")]
#else
            [DllImport("__Internal", EntryPoint = "daveSendMessage")]
#endif
            protected static extern int daveSendMessage64(IntPtr dc, IntPtr p);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveSendMessage")]
#else
            [DllImport("__Internal", EntryPoint = "daveSendMessage")]
#endif
            protected static extern int daveSendMessage32(IntPtr dc, IntPtr p);
            public int getMessage(PDU p)
            {
                if (IntPtr.Size == 8)
                    return daveSendMessage64(pointer, p.pointer);
                else
                    return daveSendMessage32(pointer, p.pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveGetProgramBlock")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetProgramBlock")]
#endif
            protected static extern int daveGetProgramBlock64(IntPtr dc, int blockType, int number, byte[] buffer, ref int length);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveGetProgramBlock")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetProgramBlock")]
#endif
            protected static extern int daveGetProgramBlock32(IntPtr dc, int blockType, int number, byte[] buffer, ref int length);
            public int getProgramBlock(int blockType, int number, byte[] buffer, ref int length)
            {
                if (IntPtr.Size == 8)
                    return daveGetProgramBlock64(pointer, blockType, number, buffer, ref length);
                else
                    return daveGetProgramBlock32(pointer, blockType, number, buffer, ref length);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "davePutProgramBlock")]
#else
            [DllImport("__Internal", EntryPoint = "davePutProgramBlock")]
#endif
            protected static extern int davePutProgramBlock64(IntPtr dc, int blockType, int number, byte[] buffer, ref int length);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "davePutProgramBlock")]
#else
            [DllImport("__Internal", EntryPoint = "davePutProgramBlock")]
#endif
            protected static extern int davePutProgramBlock32(IntPtr dc, int blockType, int number, byte[] buffer, ref int length);
            public int putProgramBlock(int blockType, int number, byte[] buffer, ref int length)
            {
                if (IntPtr.Size == 8)
                    return davePutProgramBlock64(pointer, blockType, number, buffer, ref length);
                else
                    return davePutProgramBlock32(pointer, blockType, number, buffer, ref length);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveDeleteProgramBlock")]
#else
            [DllImport("__Internal", EntryPoint = "daveDeleteProgramBlock")]
#endif
            protected static extern int daveDeleteProgramBlock64(IntPtr dc, int blockType, int number);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveDeleteProgramBlock")]
#else
            [DllImport("__Internal", EntryPoint = "daveDeleteProgramBlock")]
#endif
            protected static extern int daveDeleteProgramBlock32(IntPtr dc, int blockType, int number);
            public int deleteProgramBlock(int blockType, int number)
            {
                if (IntPtr.Size == 8)
                    return daveDeleteProgramBlock64(pointer, blockType, number);
                else
                    return daveDeleteProgramBlock32(pointer, blockType, number);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveListBlocksOfType")]
#else
            [DllImport("__Internal", EntryPoint = "daveListBlocksOfType")]
#endif
            protected static extern int daveListBlocksOfType64(IntPtr dc, int blockType, byte[] buffer);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveListBlocksOfType")]
#else
            [DllImport("__Internal", EntryPoint = "daveListBlocksOfType")]
#endif
            protected static extern int daveListBlocksOfType32(IntPtr dc, int blockType, byte[] buffer);
            public int ListBlocksOfType(int blockType, byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    return daveListBlocksOfType64(pointer, blockType, buffer);
                else
                    return daveListBlocksOfType32(pointer, blockType, buffer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveSetPLCTime")]
#else
            [DllImport("__Internal", EntryPoint = "daveSetPLCTime")]
#endif
            protected static extern int daveSetPLCTime64(IntPtr dc, byte[] buffer);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveSetPLCTime")]
#else
            [DllImport("__Internal", EntryPoint = "daveSetPLCTime")]
#endif
            protected static extern int daveSetPLCTime32(IntPtr dc, byte[] buffer);

            public int daveSetPLCTime(DateTime tm)
            {
                byte[] buffer = new byte[] { 0x00, 0x19, 0x05, 0x08, 0x23, 0x04, 0x10, 0x23, 0x67, 0x83, };
                putBCD8at(buffer, 2, tm.Year % 100);
                putBCD8at(buffer, 3, tm.Month);
                putBCD8at(buffer, 4, tm.Day);
                putBCD8at(buffer, 5, tm.Hour);
                putBCD8at(buffer, 6, tm.Minute);
                putBCD8at(buffer, 7, tm.Second);
                putBCD8at(buffer, 8, tm.Millisecond / 10);
                putBCD8at(buffer, 9, (tm.Millisecond % 10) << 4);

                if (IntPtr.Size == 8)
                    return daveSetPLCTime64(pointer, buffer);
                else
                    return daveSetPLCTime32(pointer, buffer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveReadPLCTime")]
#else
            [DllImport("__Internal", EntryPoint = "daveReadPLCTime")]
#endif
            protected static extern int daveReadPLCTime64(IntPtr dc);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveReadPLCTime")]
#else
            [DllImport("__Internal", EntryPoint = "daveReadPLCTime")]
#endif
            protected static extern int daveReadPLCTime32(IntPtr dc);
            public DateTime daveReadPLCTime()
            {
                int res = 0;
                if (IntPtr.Size == 8)
                    res = daveReadPLCTime64(pointer);
                else
                    res = daveReadPLCTime32(pointer);

                int year, month, day, hour, minute, second, millisecond;
                getU8();
                getU8();
                byte[] tmp = new byte[1];
                tmp[0] = Convert.ToByte(getU8());
                year = getBCD8from(tmp, 0);
                year += year >= 90 ? 1900 : 2000;
                tmp[0] = Convert.ToByte(getU8());
                month = getBCD8from(tmp, 0);
                tmp[0] = Convert.ToByte(getU8());
                day = getBCD8from(tmp, 0);
                tmp[0] = Convert.ToByte(getU8());
                hour = getBCD8from(tmp, 0);
                tmp[0] = Convert.ToByte(getU8());
                minute = getBCD8from(tmp, 0);
                tmp[0] = Convert.ToByte(getU8());
                second = getBCD8from(tmp, 0);
                tmp[0] = Convert.ToByte(getU8());
                millisecond = getBCD8from(tmp, 0) * 10;
                tmp[0] = Convert.ToByte(getU8());
                tmp[0] = Convert.ToByte(tmp[0] >> 4);
                millisecond += getBCD8from(tmp, 0);
                DateTime ret = new DateTime(year, month, day, hour, minute, second, millisecond);

                return ret;
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveGetPDUData")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetPDUData")]
#endif
            protected static extern int daveGetPDUData64(IntPtr dc, IntPtr p, byte[] data, ref int ldata, byte[] param, ref int lparam);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveGetPDUData")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetPDUData")]
#endif
            protected static extern int daveGetPDUData32(IntPtr dc, IntPtr p, byte[] data, ref int ldata, byte[] param, ref int lparam);

            public int daveGetPDUData(PDU myPDU, out byte[] data, out byte[] param)
            {
                byte[] tmp1 = new byte[65536];
                byte[] tmp2 = new byte[65536];
                int ltmp1 = 0;
                int ltmp2 = 0;
                int res = 0;
                if (IntPtr.Size == 8)
                    res = daveGetPDUData64(pointer, myPDU.pointer, tmp1, ref ltmp1, tmp2, ref ltmp2);
                else
                    res = daveGetPDUData32(pointer, myPDU.pointer, tmp1, ref ltmp1, tmp2, ref ltmp2);
                data = new byte[ltmp1];
                param = new byte[ltmp2];
                Array.Copy(tmp1, data, ltmp1);
                Array.Copy(tmp2, param, ltmp2);
                return res;
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "_daveSetupReceivedPDU")]
#else
            [DllImport("__Internal", EntryPoint = "_daveSetupReceivedPDU")]
#endif
            private static extern int _daveSetupReceivedPDU64(IntPtr dc, IntPtr p);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "_daveSetupReceivedPDU")]
#else
            [DllImport("__Internal", EntryPoint = "_daveSetupReceivedPDU")]
#endif
            private static extern int _daveSetupReceivedPDU32(IntPtr dc, IntPtr p);

            public int daveRecieveData(out byte[] data, out byte[] param)
            {
                int res = 0;
                if (IntPtr.Size == 8)
                    res = daveGetResponse64(pointer);
                else
                    res = daveGetResponse32(pointer);

                PDU myPDU = new PDU();
                if (IntPtr.Size == 8)
                    _daveSetupReceivedPDU64(pointer, myPDU.pointer);
                else
                    _daveSetupReceivedPDU32(pointer, myPDU.pointer);

                byte[] tmp1 = new byte[65536];
                byte[] tmp2 = new byte[65536];
                int ltmp1 = 0;
                int ltmp2 = 0;

                if (IntPtr.Size == 8)
                    res = daveGetPDUData64(pointer, myPDU.pointer, tmp1, ref ltmp1, tmp2, ref ltmp2);
                else
                    res = daveGetPDUData32(pointer, myPDU.pointer, tmp1, ref ltmp1, tmp2, ref ltmp2);

                data = new byte[ltmp1];
                param = new byte[ltmp2];
                Array.Copy(tmp1, data, ltmp1);
                Array.Copy(tmp2, param, ltmp2);
                return res;
            }

        }

        public class PDU : pseudoPointer
        {
#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveNewPDU")]
#else
            [DllImport("__Internal", EntryPoint = "daveNewPDU")]
#endif
            protected static extern IntPtr daveNewPDU64();

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveNewPDU")]
#else
            [DllImport("__Internal", EntryPoint = "daveNewPDU")]
#endif
            protected static extern IntPtr daveNewPDU32();

            public PDU()
            {
                if (IntPtr.Size == 8)
                    pointer = daveNewPDU64();
                else
                    pointer = daveNewPDU32();
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveAddVarToReadRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveAddVarToReadRequest")]
#endif
            protected static extern void daveAddVarToReadRequest64(IntPtr p, int area, int DBnum, int start, int bytes);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveAddVarToReadRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveAddVarToReadRequest")]
#endif
            protected static extern void daveAddVarToReadRequest32(IntPtr p, int area, int DBnum, int start, int bytes);
            public void addVarToReadRequest(int area, int DBnum, int start, int bytes)
            {
                if (IntPtr.Size == 8)
                    daveAddVarToReadRequest64(pointer, area, DBnum, start, bytes);
                else
                    daveAddVarToReadRequest32(pointer, area, DBnum, start, bytes);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveAddBitVarToReadRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveAddBitVarToReadRequest")]
#endif
            protected static extern void daveAddBitVarToReadRequest64(IntPtr p, int area, int DBnum, int start, int bytes);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveAddBitVarToReadRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveAddBitVarToReadRequest")]
#endif
            protected static extern void daveAddBitVarToReadRequest32(IntPtr p, int area, int DBnum, int start, int bytes);
            public void addBitVarToReadRequest(int area, int DBnum, int start, int bytes)
            {
                if (IntPtr.Size == 8)
                    daveAddBitVarToReadRequest64(pointer, area, DBnum, start, bytes);
                else
                    daveAddBitVarToReadRequest32(pointer, area, DBnum, start, bytes);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveAddVarToWriteRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveAddVarToWriteRequest")]
#endif
            protected static extern void daveAddVarToWriteRequest64(IntPtr p, int area, int DBnum, int start, int bytes, byte[] buffer);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveAddVarToWriteRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveAddVarToWriteRequest")]
#endif
            protected static extern void daveAddVarToWriteRequest32(IntPtr p, int area, int DBnum, int start, int bytes, byte[] buffer);
            public void addVarToWriteRequest(int area, int DBnum, int start, int bytes, byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    daveAddVarToWriteRequest64(pointer, area, DBnum, start, bytes, buffer);
                else
                    daveAddVarToWriteRequest32(pointer, area, DBnum, start, bytes, buffer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveAddBitVarToWriteRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveAddBitVarToWriteRequest")]
#endif
            protected static extern void daveAddBitVarToWriteRequest64(IntPtr p, int area, int DBnum, int start, int bytes, byte[] buffer);

#if !IPHONE	
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveAddBitVarToWriteRequest")]
#else
            [DllImport("__Internal", EntryPoint = "daveAddBitVarToWriteRequest")]
#endif
            protected static extern void daveAddBitVarToWriteRequest32(IntPtr p, int area, int DBnum, int start, int bytes, byte[] buffer);
            public void addBitVarToWriteRequest(int area, int DBnum, int start, int bytes, byte[] buffer)
            {
                if (IntPtr.Size == 8)
                    daveAddBitVarToWriteRequest64(pointer, area, DBnum, start, bytes, buffer);
                else
                    daveAddBitVarToWriteRequest32(pointer, area, DBnum, start, bytes, buffer);
            }
        } // class PDU

        public class resultSet : pseudoPointer
        {

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveNewResultSet")]
#else
            [DllImport("__Internal", EntryPoint = "daveNewResultSet")]
#endif
            protected static extern IntPtr daveNewResultSet64();

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveNewResultSet")]
#else
            [DllImport("__Internal", EntryPoint = "daveNewResultSet")]
#endif
            protected static extern IntPtr daveNewResultSet32();
            public resultSet()
            {
                if (IntPtr.Size == 8)
                    pointer = daveNewResultSet64();
                else
                    pointer = daveNewResultSet32();
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveFreeResults")]
#else
            [DllImport("__Internal", EntryPoint = "daveFreeResults")]
#endif
            protected static extern void daveFreeResults64(IntPtr rs);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveFreeResults")]
#else
            [DllImport("__Internal", EntryPoint = "daveFreeResults")]
#endif
            protected static extern void daveFreeResults32(IntPtr rs);
            ~resultSet()
            {
                if (IntPtr.Size == 8)
                    daveFreeResults64(pointer);
                else
                    daveFreeResults32(pointer);
            }

#if !IPHONE
            [DllImport("libnodave_jfkmod64.dll", EntryPoint = "daveGetErrorOfResult")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetErrorOfResult")]
#endif
            protected static extern int daveGetErrorOfResult64(IntPtr rs, int number);

#if !IPHONE
            [DllImport("libnodave_jfkmod.dll", EntryPoint = "daveGetErrorOfResult")]
#else
            [DllImport("__Internal", EntryPoint = "daveGetErrorOfResult")]
#endif
            protected static extern int daveGetErrorOfResult32(IntPtr rs, int number);
            public int getErrorOfResult(int number)
            {
                if (IntPtr.Size == 8)
                    return daveGetErrorOfResult64(pointer, number);
                return daveGetErrorOfResult32(pointer, number);
            }

        }

#if !IPHONE
        [DllImport("libnodave_jfkmod64.dll", EntryPoint = "setPort")]
#else
        [DllImport("__Internal", EntryPoint = "setPort")]
#endif
        public static extern IntPtr setPort64([MarshalAs(UnmanagedType.LPStr)] string portName, [MarshalAs(UnmanagedType.LPStr)] string baud, int parity);

#if !IPHONE
        [DllImport("libnodave_jfkmod.dll", EntryPoint = "setPort")]
#else
        [DllImport("__Internal", EntryPoint = "setPort")]
#endif
        public static extern IntPtr setPort32([MarshalAs(UnmanagedType.LPStr)] string portName, [MarshalAs(UnmanagedType.LPStr)] string baud, int parity);

        public static IntPtr setPort(string portName, string baud, int parity)
        {
            if (IntPtr.Size == 8)
                return setPort64(portName, baud, parity);
            return setPort32(portName, baud, parity);
        }

#if !IPHONE
        [DllImport("libnodave_jfkmod64.dll", EntryPoint = "openSocket")]
#else
        [DllImport("__Internal", EntryPoint = "openSocket")]
#endif
        protected static extern IntPtr openSocket64(
            int port,
            [MarshalAs(UnmanagedType.LPStr)] string portName
            );

#if !IPHONE	
        [DllImport("libnodave_jfkmod.dll", EntryPoint = "openSocket")]
#else
        [DllImport("__Internal", EntryPoint = "openSocket")]
#endif
        protected static extern IntPtr openSocket32(
            int port,
            [MarshalAs(UnmanagedType.LPStr)] string portName
            );

        public static IntPtr openSocket(int port, string portName)
        {
            if (IntPtr.Size == 8)
                return openSocket64(port, portName);
            return openSocket32(port, portName);
        }

#if !IPHONE	
        [DllImport("libnodave_jfkmod.dll"/*, PreserveSig=false */ )]
        public static extern IntPtr openS7online(
            [MarshalAs(UnmanagedType.LPStr)] string portName,
            int hwnd
            );
#else
        public static int openS7online(string portName, int hwnd) { return 0; }
#endif

#if !IPHONE
        [DllImport("S7onlinx.dll" /*, PreserveSig=false */ )]
        private static extern int SCP_get_errno();
#else
        private static int SCP_get_errno() { return 0; }
#endif


#if !IPHONE
        [DllImport("libnodave_jfkmod64.dll", EntryPoint = "closePort")]
#else
        [DllImport("__Internal", EntryPoint = "closePort")]
#endif
        protected static extern int closePort64(IntPtr port);

#if !IPHONE
        [DllImport("libnodave_jfkmod.dll", EntryPoint = "closePort")]
#else
        [DllImport("__Internal", EntryPoint = "closePort")]
#endif
        protected static extern int closePort32(IntPtr port);

        public static int closePort(IntPtr port)
        {
            if (IntPtr.Size == 8)
                return closePort64(port);
            return closePort32(port);
        }

#if !IPHONE
        [DllImport("libnodave_jfkmod64.dll", EntryPoint = "closeSocket")]
#else
        [DllImport("__Internal", EntryPoint = "closeSocket")]
#endif
        protected static extern int closeSocket64(IntPtr port);

#if !IPHONE
        [DllImport("libnodave_jfkmod.dll", EntryPoint = "closeSocket")]
#else
        [DllImport("__Internal", EntryPoint = "closeSocket")]
#endif
        protected static extern int closeSocket32(IntPtr port);

        public static int closeSocket(IntPtr port)
        {
            if (IntPtr.Size == 8)
                return closeSocket64(port);
            return closeSocket32(port);
        }

#if !IPHONE
        [DllImport("libnodave_jfkmod.dll" /*, PreserveSig=false */)]
        public static extern int closeS7online(IntPtr port);
#else
        public static int closeS7online(IntPtr port) { return 0; }
#endif

        public static byte getU8from(byte[] b, int pos)
        {
            return Convert.ToByte(b[pos]);
        }

        public static sbyte getS8from(byte[] b, int pos)
        {

            if (b[pos] > 127)
                return Convert.ToSByte((256 - b[pos]) * -1);
            else
                return Convert.ToSByte(b[pos]);
        }

        public static short getS16from(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[2];
                b1[1] = b[pos + 0];
                b1[0] = b[pos + 1];
                return BitConverter.ToInt16(b1, 0);
            }
            else
                return BitConverter.ToInt16(b, pos);
        }

        public static void putS16at(byte[] b, int pos, short value)
        {
            byte[] bytes = BitConverter.GetBytes((value));
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 1] = bytes[0];
                b[pos] = bytes[1];
            }
            else
                Array.Copy(bytes, 0, b, pos, 2);
        }

        public static ushort getU16from(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[2];
                b1[1] = b[pos + 0];
                b1[0] = b[pos + 1];
                return BitConverter.ToUInt16(b1, 0);
            }
            else
                return BitConverter.ToUInt16(b, pos);
        }

        public static void putU16at(byte[] b, int pos, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 1] = bytes[0];
                b[pos] = bytes[1];
            }
            else
                Array.Copy(bytes, 0, b, pos, 2);
        }

        public static int getS32from(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[4];
                b1[3] = b[pos];
                b1[2] = b[pos + 1];
                b1[1] = b[pos + 2];
                b1[0] = b[pos + 3];
                return BitConverter.ToInt32(b1, 0);
            }
            else
                return BitConverter.ToInt32(b, pos);
        }

        public static void putS32at(byte[] b, int pos, int value)
        {
            byte[] bytes = BitConverter.GetBytes((value));
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 3] = bytes[0];
                b[pos + 2] = bytes[1];
                b[pos + 1] = bytes[2];
                b[pos] = bytes[3];
            }
            else
                Array.Copy(bytes, 0, b, pos, 4);
        }

        public static uint getU32from(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[4];
                b1[3] = b[pos];
                b1[2] = b[pos + 1];
                b1[1] = b[pos + 2];
                b1[0] = b[pos + 3];
                return BitConverter.ToUInt32(b1, 0);
            }
            else
                return BitConverter.ToUInt32(b, pos);
        }

        public static void putU32at(byte[] b, int pos, uint value)
        {
            byte[] bytes = BitConverter.GetBytes((value));
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 3] = bytes[0];
                b[pos + 2] = bytes[1];
                b[pos + 1] = bytes[2];
                b[pos] = bytes[3];
            }
            else
                Array.Copy(bytes, 0, b, pos, 4);
        }

        public static void putDateTimeat(byte[] b, int pos, DateTime mydatetime)
        {
            int tmp;

            tmp = mydatetime.Year / 100;
            tmp = tmp * 100;
            tmp = mydatetime.Year - tmp;
            b[pos] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Month;
            b[pos + 1] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Day;
            b[pos + 2] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Hour;
            b[pos + 3] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Minute;
            b[pos + 4] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Second;
            b[pos + 5] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Millisecond;
            b[pos + 6] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = (int)mydatetime.DayOfWeek;
            b[pos + 7] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);
        }

        public static void putS5Timeat(byte[] b, int pos, TimeSpan value)
        {
            byte basis;
            int wert;
            if (value.TotalMilliseconds <= 999 * 10)
            {
                basis = 0;
                wert = Convert.ToInt32(value.TotalMilliseconds) / 10;
            }
            else if (value.TotalMilliseconds <= 999 * 100)
            {
                basis = 1;
                wert = Convert.ToInt32(value.TotalMilliseconds) / 100;
            }
            else if (value.TotalMilliseconds <= 999 * 1000)
            {
                basis = 2;
                wert = Convert.ToInt32(value.TotalMilliseconds) / 1000;
            }
            else if (value.TotalMilliseconds <= 999 * 10000)
            {
                basis = 3;
                wert = Convert.ToInt32(value.TotalMilliseconds) / 10000;
            }
            else
            {
                basis = 3;
                wert = 999;
            }

            int p1, p2, p3;

            p3 = (wert / 100);
            p2 = ((wert - p3 * 100) / 10);
            p1 = (wert - p3 * 100 - p2 * 10);

            b[pos] = Convert.ToByte(basis << 4 | p3);
            b[pos + 1] = Convert.ToByte((p2 << 4 | p1));
        }

        public static void putTimeat(byte[] b, int pos, TimeSpan value)
        {
            putS32at(b, pos, Convert.ToInt32(value.TotalMilliseconds));
        }

        public static void putTimeOfDayat(byte[] b, int pos, DateTime value)
        {
            var tmp = new TimeSpan(0, value.Hour, value.Minute, value.Second, value.Millisecond);
            putU32at(b, pos, Convert.ToUInt32(tmp.TotalMilliseconds));
        }

        public static void putDateat(byte[] b, int pos, DateTime value)
        {
            DateTime tmp = new DateTime(1990, 1, 1);
            var tmp2 = value.Subtract(tmp);
            putU16at(b, pos, Convert.ToUInt16(tmp2.Days));
        }

        public static DateTime getDatefrom(byte[] b, int pos)
        {
            DateTime tmp = new DateTime(1990, 1, 1);
            var tmp2 = TimeSpan.FromDays(getU16from(b, pos));
            tmp = tmp.Add(tmp2);
            return tmp;
        }

        public static float getS5Floatfrom(byte[] b, int pos)
        {
            //the bit 0 to bit 22 are the Mantissa
            //bit 23 --> 0 is positive / 1 is negative 
            //bit 24 to bit 30 exponent
            //bit 31 => 0 is positive exponent / 1 is negative exponent

            byte[] b1 = new byte[4];
            b1[3] = 0;
            b1[2] = (byte)(b[pos + 1] & 0x7f);
            b1[1] = (byte)(b[pos + 2]);
            b1[0] = (byte)(b[pos + 3]);

            var sign = (b[pos + 1] & 0x80);

            long mantissa = b1[2] * 256 * 256 + b1[1] * 256 + b1[0];
            //var mantissa = BitConverter.ToSingle(b1, 0);
            float wrt = mantissa;

            var exp = (b[pos + 0]);// & 0x7f);

            if (sign>0)
            {
                mantissa = mantissa ^ 0xffffffff;
                mantissa = mantissa + 0x00800000;
                wrt = mantissa;
                wrt = -wrt;
            }
            

            /*exp = (byte)(exp >> 1);

            while (exp >0)
            {
                wrt = wrt * 2.0f;
                exp--;
            }*/
            
            while (exp > 23)
            {
                wrt = wrt * 2.0f;
                exp--;
            }
            while (exp < 23)
            {
                wrt = wrt / 2.0f;
                exp++;
            }

            /* int k = 0;
            while (wrt > 2.0f)
            {
                wrt = wrt / 2.0f;
                k++;
            }*/

            return wrt;
        }

        public static float getFloatfrom(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[4];
                b1[3] = b[pos];
                b1[2] = b[pos + 1];
                b1[1] = b[pos + 2];
                b1[0] = b[pos + 3];
                return BitConverter.ToSingle(b1, 0);
            }
            else
                return BitConverter.ToSingle(b, pos);
        }

        /// <summary>
        /// This put's a String as a S7 String to the PLC
        /// </summary>
        /// <param name="b"></param>
        /// <param name="pos"></param>
        /// <param name="value"></param>
        /// <param name="length"></param>
        public static void putS7Stringat(byte[] b, int pos, string value, int length)
        {
            b[pos] = (byte)length;
            b[pos + 1] = length > value.Length ? (byte)value.Length : (byte)length;
            Array.Copy(Encoding.ASCII.GetBytes(value), 0, b, pos + 2, value.Length > length ? length : value.Length);
        }

        /// <summary>
        /// This put's a String as a Char-Array to the PLC
        /// </summary>
        /// <param name="b"></param>
        /// <param name="pos"></param>
        /// <param name="value"></param>
        /// <param name="length"></param>
        public static void putStringat(byte[] b, int pos, string value, int length)
        {
            Array.Copy(Encoding.ASCII.GetBytes(value), 0, b, pos, value.Length > length ? length : value.Length);
        }

        public static void putFloatat(byte[] b, int pos, Single value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 3] = bytes[0];
                b[pos + 2] = bytes[1];
                b[pos + 1] = bytes[2];
                b[pos] = bytes[3];
            }
            else
                Array.Copy(bytes, 0, b, pos, 4);
        }

        public static int getBCD8from(byte[] b, int pos)
        {
            //Acepted Values 00 to 99
            int bt1 = b[pos];
            bool neg = (bt1 & 0xf0) == 0xf0 ? true : false;
            if (neg)
            {
                bt1 = -1*(bt1 & 0x0f);
                //bt1 = 0;
            }
            else
            {
                bt1 = (bt1 >> 4) * 10 + (bt1 & 0x0f);
            }
            return bt1;
        }

        public static void putBCD8at(byte[] b, int pos, int value)
        {
            int b0 = 0, b1 = 0;

            //setze höchstes bit == negativer wert!
            if (value >= 0)
            {
                b1 = (value % 100 / 10);
                b0 = value % 10;
            }
            b[pos] = (byte)((b1 << 4) + b0);
        }

        public static void putBCD16at(byte[] b, int pos, int value)
        {
            //Acepted Values -999 to +999
            int b0 = 0, b1 = 0, b2 = 0, b3 = 0;

            if (value < 0)
            {
                b3 = 0x0f;
                value = -1 * value;
            }
            else
            {
                b3 = 0x00;
            }
            b2 = (value % 1000 / 100);
            b1 = (value % 100 / 10);
            b0 = (value % 10);
            b[pos] = (byte)((b3 << 4) + b2);
            b[pos + 1] = (byte)((b1 << 4) + b0);
        }

        public static void putBCD32at(byte[] b, int pos, int value)
        {
            //Acepted Values -9999999 to +9999999
            int b0 = 0, b1 = 0, b2 = 0, b3 = 0, b4 = 0, b5 = 0, b6 = 0, b7 = 0;

            if (value < 0)
            {
                b7 = 0x0f;
                value = -1 * value;
            }
            else
            {
                //b7 = (value % 100000000 / 10000000);
                b7 = 0x00;
            }
            b6 = (value % 10000000 / 1000000);
            b5 = (value % 1000000 / 100000);
            b4 = (value % 100000 / 10000);
            b3 = (value % 10000 / 1000);
            b2 = (value % 1000 / 100);
            b1 = (value % 100 / 10);
            b0 = (value % 10);

            b[pos] = (byte)((b7 << 4) + b6);
            b[pos + 1] = (byte)((b5 << 4) + b4);
            b[pos + 2] = (byte)((b3 << 4) + b2);
            b[pos + 3] = (byte)((b1 << 4) + b0);
        }

        public static int getBCD16from(byte[] b, int pos)
        {
            int bt1 = b[pos];
            int bt2 = b[pos + 1];
            bool neg = (bt1 & 0xf0) == 0xf0 ? true : false;

            bt1 = bt1 & 0x0f;
            bt2 = (bt2 / 0x10) * 10 + (bt2 & 0x0f % 0x10);

            return neg ? (bt1 * 100 + bt2) * -1 : bt1 * 100 + bt2;
        }

        public static int getBCD32from(byte[] b, int pos)
        {
            int bt1 = b[pos];
            int bt2 = b[pos + 1];
            int bt3 = b[pos + 2];
            int bt4 = b[pos + 3];
            bool neg = (bt1 & 0xf0) == 0xf0 ? true : false;

            bt1 = bt1 & 0x0f;
            bt2 = (bt2 / 0x10) * 10 + (bt2 % 0x10);
            bt3 = (bt3 / 0x10) * 10 + (bt3 % 0x10);
            bt4 = (bt4 / 0x10) * 10 + (bt4 % 0x10);
            return neg ? (bt1 * 1000000 + bt2 * 10000 + bt3 * 100 + bt4) * -1 : bt1 * 1000000 + bt2 * 10000 + bt3 * 100 + bt4;
        }

        public static DateTime getDateTimefrom(byte[] b, int pos)
        {
            int jahr, monat, tag, stunde, minute, sekunde, mili;
            int bt = b[pos];
            //BCD Umwandlung
            bt = (((bt >> 4)) * 10) + ((bt & 0x0f));
            if (bt < 90)
                jahr = 2000;
            else
                jahr = 1900;
            jahr += bt;

            //Monat
            bt = b[pos + 1];
            monat = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Tag
            bt = b[pos + 2];
            tag = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Stunde
            bt = b[pos + 3];
            stunde = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Minute
            bt = b[pos + 4];
            minute = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Sekunde
            bt = b[pos + 5];
            sekunde = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Milisekunden
            //Byte 6 BCD + MSB (Byte 7)
            bt = b[pos + 6];
            int bt1 = b[pos + 7];
            mili = (((bt >> 4)) * 10) + ((bt & 0x0f));
            mili = mili * 10 + (bt1 >> 4);

            //Wochentag
            //LSB (Byte 7) 1=Sunday
            //bt = b[pos + 7];
            //wochentag = (bt1 & 0x0f); 
            try
            {
                return new DateTime(jahr, monat, tag, stunde, minute, sekunde, mili);
            }
            catch (Exception ex)
            {
                return new DateTime(1900, 01, 01, 00, 00, 00);
            }
        }

        public static DateTime getTimeOfDayfrom(byte[] b, int pos)
        {
            long msval = getU32from(b, pos);
            return new DateTime(msval * 10000);
        }

        public static TimeSpan getTimefrom(byte[] b, int pos)
        {
            long msval = getS32from(b, pos);
            return new TimeSpan(msval * 10000);
        }

        public static TimeSpan getS5Timefrom(byte[] b, int pos)
        {
            int w1 = getBCD8from(b, pos + 1);
            int w2 = ((b[pos] & 0x0f));

            long zahl = w2 * 100 + w1;

            int basis = (b[pos] >> 4) & 0x03;

            switch (basis)
            {
                case 0:
                    zahl = zahl * 100000;
                    break;
                case 1:
                    zahl = zahl * 1000000;
                    break;
                case 2:
                    zahl = zahl * 10000000;
                    break;
                case 3:
                    zahl = zahl * 100000000;
                    break;

            }
            return new TimeSpan(zahl);
        }

        public static bool getBit(int Byte, int Bit)
        {
            int wrt = System.Convert.ToInt32(System.Math.Pow(2, Bit));
            return ((Byte & wrt) > 0);
        }

        public static string dec2bin(byte Bytewert)
        {
            byte[] bitwert = { 128, 64, 32, 16, 8, 4, 2, 1 };
            byte[] bits = new byte[8];

            string bitstring = string.Empty; for (int Counter = 0; Counter < 8; Counter++)
            {
                if (Bytewert >= bitwert[Counter])
                {
                    bits[Counter] = 1; Bytewert -= bitwert[Counter];
                }
                bitstring += Convert.ToString(bits[Counter]);
            }
            return bitstring;
        }
    }
}