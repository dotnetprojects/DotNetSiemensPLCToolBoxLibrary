using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DotNetSiemensPLCToolBoxLibrary.Simulator
{
    public class Simulator
    {
        [StructLayout(LayoutKind.Explicit, Size = 4)]
        public struct Akku
        {
            [FieldOffset(0)]
            public byte Byte1;

            [FieldOffset(1)]
            public byte Byte2;

            [FieldOffset(2)]
            public byte Byte3;

            [FieldOffset(3)]
            public byte Byte4;

            [FieldOffset(0)]
            public UInt32 UInt32;

            [FieldOffset(0)]
            public UInt16 UInt16;

            [FieldOffset(0)]
            public Int16 Int16;

            [FieldOffset(0)]
            public Int32 Int32;

            [FieldOffset(0)]
            public Single Single;
        }

        public Akku Akku1;
        public UInt32 Akku2 { get; set; }
        public UInt32 Akku3 { get; set; }
        public UInt32 Akku4 { get; set; }
        public UInt16 DB { get; set; }
        public UInt16 DI { get; set; }
        public UInt32 AR1 { get; set; }
        public UInt32 AR2 { get; set; }

        public bool BIE { get; set; }
        public bool A1 { get; set; }
        public bool A0 { get; set; }
        public bool OV { get; set; }
        public bool OS { get; set; }
        public bool OR { get; set; }
        public bool STA { get; set; }
        public bool VKE { get; set; }
        public bool _ER { get; set; }

        public Dictionary<int, byte[]> DataBlocks { get; private set; }
        public Dictionary<int, byte[]> LocalDataStack { get; private set; }

        private int LocalDataStackStart;
        private Queue<int> LocalDataStackStartQueue { get; set; }

        public Simulator()
        {
            DataBlocks = new Dictionary<int, byte[]>();

            LocalDataStack = new Dictionary<int, byte[]>();
            LocalDataStackStart = 0;
            LocalDataStackStartQueue = new Queue<int>();

            Akku1.UInt32 = 0;
            Akku2 = 0;
            Akku3 = 0;
            Akku4 = 0;
            DB = 0;
            DI = 0;
        }

        public void ExecuteCommand(S7FunctionBlockRow row)
        {
            if (row.Command == Mnemonic.opABS[0])
            {
                Akku1.Int32 = Math.Abs(Akku1.Int32);
            }
            else if (row.Command == Mnemonic.opACOS[0])
            {
                Akku1.Single = (Single)Math.Acos(Akku1.Single);
            }
            else if (row.Command == Mnemonic.opL[0])
            {
                Akku1.Single = (Single)Math.Acos(Akku1.Single);
            }
            else if (row.Command == Mnemonic.opTAK[0])
            {
                var sp = Akku2;
                Akku2 = Akku1.UInt32;
                Akku1.UInt32 = sp;
            }
        }
    }
}