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

        public short BGTyp { get; set; }

        public short Ausbg1 { get; set; }

        public short Ausbg2 { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy12Dataset : SZLDataset
    {
        public short Feature { get; set; }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy13Dataset : SZLDataset
    {
        public short Index { get; set; }

        public short Code { get; set; }

        public int Size { get; set; }

        public short Mode { get; set; }

        public short Granu { get; set; }

        public int Ber1 { get; set; }

        public int Belegt1 { get; set; }

        public int Block1 { get; set; }

        public int Ber2 { get; set; }

        public int Belegt2 { get; set; }

        public int Block2 { get; set; }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy14Dataset : SZLDataset
    {
        public short Index { get; set; }

        public short Code { get; set; }

        public short Count { get; set; }

        public short Reman { get; set; }
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy15Dataset : SZLDataset
    {
        public short Index { get; set; }

        public short MaxCount { get; set; }

        public short MaxSize { get; set; }

        public short Maxabl { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy1CDataset : SZLDataset
    {
        public Int16 Index { get; set; }
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        private short[] _info;
        public Int16[] Info
        {
            get { return _info; }
            set { _info = value; }
        }

        public Int16 Al1 { get; set; }
        public Int16 Al2 { get; set; }
        public Int32 Al3 { get; set; }
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
    public class xy71Dataset : SZLDataset
    {
        public Int16 redinf { get; set; }
        public byte mwstat1 { get; set; }
        public byte mwstat2 { get; set; }
        public Int16 hsfcinfo { get; set; }
        public Int16 samfehl { get; set; }
        public Int16 bz_cpu_0 { get; set; }
        public Int16 bz_cpu_1 { get; set; }
        public byte cpu_valid { get; set; }
        public byte hsync_f { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy74Dataset : SZLDataset
    {
        public Int16 led_kennung { get; set; }
        public byte led_on { get; set; }
        public byte led_blink { get; set; }
    }
}
