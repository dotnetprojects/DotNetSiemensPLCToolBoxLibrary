using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus
{
    internal static class daveConst
    {
        public static int daveAnaIn = 0x6;		/* analog inputs of 200 family */
        public static int daveAnaOut = 0x7;		/* analog outputs of 200 family */
        public static int daveCounter = 28;	/* S7 counters */
        public static int daveTimer = 29;	/* S7 timers */
        public static int daveCounter200 = 30;	/* IEC counters (200 family) */
        public static int daveTimer200 = 31;		/* IEC timers (200 family) */

        public static int ISOTCPminPacketLength = 16;
        public static int daveResTimeout = -1025;
        public static int daveResShortPacket = -1024;
        public static int daveMaxRawLen = 2048;
       
        public static int daveFuncOpenS7Connection = 0xF0;
        public static int daveFuncRead = 0x04;
        public static int daveFuncWrite = 0x05;
        public static int daveFuncRequestDownload = 0x1A;
        public static int daveFuncDownloadBlock = 0x1B;
        public static int daveFuncDownloadEnded = 0x1C;
        public static int daveFuncStartUpload = 0x1D;
        public static int daveFuncUpload = 0x1E;
        public static int daveFuncEndUpload = 0x1F;
        public static int daveFuncInsertBlock = 0x28;
    }
}
