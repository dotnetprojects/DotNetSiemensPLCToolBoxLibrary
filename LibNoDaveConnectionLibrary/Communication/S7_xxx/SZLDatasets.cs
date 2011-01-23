using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.S7_xxx
{
    public interface ISZLDataset
    {
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct DefaultSZLDataset : ISZLDataset
    {
        public byte[] Bytes;        
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct xy11Dataset: ISZLDataset
    {
        public Int16 Index;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)] public string MlfB;
        public Int16 BGTyp;
        public Int16 Ausbg1;
        public Int16 Ausbg2;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct xy12Dataset : ISZLDataset
    {
        public Int16 Feature;        
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct xy13Dataset : ISZLDataset
    {
        public Int16 Index;
        public Int16 Code;
        public Int32 Size;
        public Int16 Mode;
        public Int16 Granu; //Allways zero
        public Int32 Ber1;
        public Int32 Belegt1;
        public Int32 Block1;
        public Int32 Ber2;
        public Int32 Belegt2;
        public Int32 Block2;        
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct xy14Dataset : ISZLDataset
    {
        public Int16 Index;
        public Int16 Code;
        public Int16 Count;
        public Int16 Reman;        
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct xy15Dataset : ISZLDataset
    {
        public Int16 Index;
        public Int16 MaxCount;
        public Int16 MaxSize;
        public Int16 Maxabl;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct xy1CDataset : ISZLDataset
    {
        public Int16 Index;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string Text;        
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct xy22Dataset : ISZLDataset
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)] public Int16[] Info;
        public Int16 Al1;
        public Int16 Al2;
        public Int32 Al3;        
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct xy25Dataset : ISZLDataset
    {        
        public byte tpa_nr;
        public byte tpa_use;
        public byte ob_nr;
        public byte res;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct xy71Dataset : ISZLDataset
    {
        public Int16 redinf;
        public byte mwstat1;
        public byte mwstat2;
        public Int16 hsfcinfo;
        public Int16 samfehl;
        public Int16 bz_cpu_0;
        public Int16 bz_cpu_1;
        public byte cpu_valid;
        public byte hsync_f;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct xy74Dataset : ISZLDataset
    {
        public Int16 led_kennung;
        public byte led_on;
        public byte led_blink;        
    }
}
