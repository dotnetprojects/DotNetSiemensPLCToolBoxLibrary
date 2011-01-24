using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.S7_xxx
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]    
    public abstract class SZLDataset
    {
    }

    public class DefaultSZLDataset : SZLDataset
    {
        public byte[] Bytes { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy11Dataset : SZLDataset
    {
        public short Index { get; set; }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        private string _mlfB;     
        public string MlfB
        {
            get { return _mlfB; }
            set { _mlfB = value; }
        }

        public UInt16 BGTyp { get; set; }

        public UInt16 Ausbg1 { get; set; }

        public UInt16 Ausbg2 { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy12Dataset : SZLDataset
    {
        public UInt16 Feature { get; set; }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy13Dataset : SZLDataset
    {
        public UInt16 Index { get; set; }

        public UInt16 Code { get; set; }

        public UInt32 Size { get; set; }

        public UInt16 Mode { get; set; }

        public UInt16 Granu { get; set; }

        public UInt32 Ber1 { get; set; }

        public UInt32 Belegt1 { get; set; }

        public UInt32 Block1 { get; set; }

        public UInt32 Ber2 { get; set; }

        public UInt32 Belegt2 { get; set; }

        public UInt32 Block2 { get; set; }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy14Dataset : SZLDataset
    {
        public UInt16 Index { get; set; }

        public UInt16 Code { get; set; }

        public UInt16 Anzahl { get; set; }

        public UInt16 Reman { get; set; }
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy15Dataset : SZLDataset
    {
        public UInt16 Index { get; set; }

        public UInt16 MaxAnz { get; set; }

        public UInt16 MaxLng { get; set; }

        public UInt32 Maxabl { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy16Dataset : SZLDataset
    {
        public UInt16 Index { get; set; }

        public UInt16 MaxAnz { get; set; }

        public UInt16 AnzAkt { get; set; }        
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy17Dataset : SZLDataset
    {
        public UInt16 SDBNr { get; set; }

        public UInt16 State { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy18Dataset : SZLDataset
    {
        public UInt16 RackNr { get; set; }

        public UInt16 AnzSt { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy19Dataset : SZLDataset
    {
        public UInt16 Index { get; set; }

        public byte Led_On { get; set; }

        public byte Led_Blink { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy21Dataset : SZLDataset
    {
        public UInt16 ereig { get; set; }

        public byte Ae { get; set; }

        public byte OB { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy1CDataset : SZLDataset
    {
        public UInt16 Index { get; set; }
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy22Dataset : SZLDataset
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        private UInt16[] _info;
        public UInt16[] Info
        {
            get { return _info; }
            set { _info = value; }
        }

        public UInt16 Al1 { get; set; }
        public UInt16 Al2 { get; set; }
        public UInt32 Al3 { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy23Dataset : SZLDataset
    {
        public byte Ae { get; set; }
        public byte AeStat { get; set; }
        public UInt16 Aefstat { get; set; }
        public byte MaxBst { get; set; }
        public byte MaxSti { get; set; }
        public byte AktSiv { get; set; }
        public byte AktSib { get; set; }
        public UInt16 Grld { get; set; }
        public UInt32 ProgFm { get; set; }
        public UInt32 SyncFm { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy25Dataset : SZLDataset
    {
        public byte tpa_nr { get; set; }

        public byte tpa_use { get; set; }

        public byte ob_nr { get; set; }

        public byte res { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy31_1Dataset : SZLDataset
    {
        public UInt16 Index { get; set; }

        public UInt16 Pdu { get; set; }

        public UInt16 Anz { get; set; }

        public UInt32 Mpi_bps { get; set; }

        public UInt32 Kbus_bps { get; set; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        private UInt16[] _res;
        public UInt16[] res
        {
            get { return _res; }
            set { _res = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy31_2Dataset : SZLDataset
    {
        public UInt16 Index { get; set; }

        public byte funkt_0 { get; set; }

        public byte funkt_1 { get; set; }

        public byte funkt_2 { get; set; }

        public byte funkt_3 { get; set; }

        public byte funkt_4 { get; set; }

        public byte funkt_5 { get; set; }

         [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        private byte[] _aseg;
        public byte[] aseg
        {
            get { return _aseg; }
            set { _aseg = value; }
        }

         [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        private byte[] _eseg;
        public byte[] eseg
        {
            get { return _eseg; }
            set { _eseg = value; }
        }

        public byte trgereig_0 { get; set; }

        public byte trgereig_1 { get; set; }

        public byte trgereig_2 { get; set; }

        public byte trgbed { get; set; }

        public byte pfad { get; set; }

        public byte tiefe { get; set; }

        public byte systrig { get; set; }

        public byte erg_par { get; set; }

        public UInt16 erg_pat_1 { get; set; }

        public UInt16 erg_pat_2 { get; set; }

        public UInt16 force { get; set; }

        public UInt16 time { get; set; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        private UInt16[] _res;
        public UInt16[] res
        {
            get { return _res; }
            set { _res = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy71Dataset : SZLDataset
    {
        public UInt16 redinf { get; set; }
        public byte mwstat1 { get; set; }
        public byte mwstat2 { get; set; }
        public UInt16 hsfcinfo { get; set; }
        public UInt16 samfehl { get; set; }
        public UInt16 bz_cpu_0 { get; set; }
        public UInt16 bz_cpu_1 { get; set; }
        public byte cpu_valid { get; set; }
        public byte hsync_f { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy74Dataset : SZLDataset
    {
        public UInt16 led_kennung { get; set; }
        public byte led_on { get; set; }
        public byte led_blink { get; set; }
    }
}
