using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.General;

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
    public class xy00Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]        
        private ushort _szlId;
        public UInt16 SZL_id
        {
            get { return _szlId; }
            set { _szlId = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy11Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]        
        private short _index;
        public short Index
        {
            get { return _index; }
            set { _index = value; }
        }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)] 
        private string _mlfB;     
        public string MlfB
        {
            get { return _mlfB; }
            set { _mlfB = value; }
        }

        [Endian(Endianness.BigEndian)]      
        private ushort _bgTyp;
        public UInt16 BGTyp
        {
            get { return _bgTyp; }
            set { _bgTyp = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _ausbg1;
        public UInt16 Ausbg1
        {
            get { return _ausbg1; }
            set { _ausbg1 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _ausbg2;
        public UInt16 Ausbg2
        {
            get { return _ausbg2; }
            set { _ausbg2 = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy12Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _feature;
        public UInt16 Feature
        {
            get { return _feature; }
            set { _feature = value; }
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy13Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _code;
        public UInt16 Code
        {
            get { return _code; }
            set { _code = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _size;
        public UInt32 Size
        {
            get { return _size; }
            set { _size = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _mode;
        public UInt16 Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _granu;
        public UInt16 Granu
        {
            get { return _granu; }
            set { _granu = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _ber1;
        public UInt32 Ber1
        {
            get { return _ber1; }
            set { _ber1 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _belegt1;
        public UInt32 Belegt1
        {
            get { return _belegt1; }
            set { _belegt1 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _block1;
        public UInt32 Block1
        {
            get { return _block1; }
            set { _block1 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _ber2;
        public UInt32 Ber2
        {
            get { return _ber2; }
            set { _ber2 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _belegt2;
        public UInt32 Belegt2
        {
            get { return _belegt2; }
            set { _belegt2 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _block2;
        public UInt32 Block2
        {
            get { return _block2; }
            set { _block2 = value; }
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy14Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _code;
        public UInt16 Code
        {
            get { return _code; }
            set { _code = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _anzahl;
        public UInt16 Anzahl
        {
            get { return _anzahl; }
            set { _anzahl = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _reman;
        public UInt16 Reman
        {
            get { return _reman; }
            set { _reman = value; }
        }
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy15Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _maxAnz;
        public UInt16 MaxAnz
        {
            get { return _maxAnz; }
            set { _maxAnz = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _maxLng;
        public UInt16 MaxLng
        {
            get { return _maxLng; }
            set { _maxLng = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _maxabl;
        public UInt32 Maxabl
        {
            get { return _maxabl; }
            set { _maxabl = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy16Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _maxAnz;
        public UInt16 MaxAnz
        {
            get { return _maxAnz; }
            set { _maxAnz = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _anzAkt;
        public UInt16 AnzAkt
        {
            get { return _anzAkt; }
            set { _anzAkt = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy17Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _sdbNr;
        public UInt16 SDBNr
        {
            get { return _sdbNr; }
            set { _sdbNr = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _state;
        public UInt16 State
        {
            get { return _state; }
            set { _state = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy18Dataset : SZLDataset
    {
        private ushort _rackNr;
        public UInt16 RackNr
        {
            get { return _rackNr; }
            set { _rackNr = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _anzSt;
        public UInt16 AnzSt
        {
            get { return _anzSt; }
            set { _anzSt = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy19Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private byte _ledOn;
        public byte Led_On
        {
            get { return _ledOn; }
            set { _ledOn = value; }
        }

        private byte _ledBlink;
        public byte Led_Blink
        {
            get { return _ledBlink; }
            set { _ledBlink = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy21Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _ereig;
        public UInt16 ereig
        {
            get { return _ereig; }
            set { _ereig = value; }
        }

        private byte _ae;
        public byte Ae
        {
            get { return _ae; }
            set { _ae = value; }
        }

        private byte _ob;
        public byte OB
        {
            get { return _ob; }
            set { _ob = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy1CDataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        private byte[] _info;
        public byte[] Info
        {
            get { return _info; }
            set { _info = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _al1;
        public UInt16 Al1
        {
            get { return _al1; }
            set { _al1 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _al2;
        public UInt16 Al2
        {
            get { return _al2; }
            set { _al2 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _al3;
        public UInt32 Al3
        {
            get { return _al3; }
            set { _al3 = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy23Dataset : SZLDataset
    {
        private byte _ae;
        public byte Ae
        {
            get { return _ae; }
            set { _ae = value; }
        }

        private byte _aeStat;
        public byte AeStat
        {
            get { return _aeStat; }
            set { _aeStat = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _aefstat;
        public UInt16 Aefstat
        {
            get { return _aefstat; }
            set { _aefstat = value; }
        }

        private byte _maxBst;
        public byte MaxBst
        {
            get { return _maxBst; }
            set { _maxBst = value; }
        }

        private byte _maxSti;
        public byte MaxSti
        {
            get { return _maxSti; }
            set { _maxSti = value; }
        }

        private byte _aktSiv;
        public byte AktSiv
        {
            get { return _aktSiv; }
            set { _aktSiv = value; }
        }

        private byte _aktSib;
        public byte AktSib
        {
            get { return _aktSib; }
            set { _aktSib = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _grld;
        public UInt16 Grld
        {
            get { return _grld; }
            set { _grld = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _progFm;
        public UInt32 ProgFm
        {
            get { return _progFm; }
            set { _progFm = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _syncFm;
        public UInt32 SyncFm
        {
            get { return _syncFm; }
            set { _syncFm = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy24Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _ereig;
        public UInt16 Ereig
        {
            get { return _ereig; }
            set { _ereig = value; }
        }

        private byte _ae;
        public byte Ae
        {
            get { return _ae; }
            set { _ae = value; }
        }

        private byte _bzüId;
        public byte bzü_id
        {
            get { return _bzüId; }
            set { _bzüId = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _res;
        public UInt32 Res
        {
            get { return _res; }
            set { _res = value; }
        }

        private byte _anlInfo1;
        public byte AnlInfo1
        {
            get { return _anlInfo1; }
            set { _anlInfo1 = value; }
        }

        private byte _anlInfo2;
        public byte AnlInfo2
        {
            get { return _anlInfo2; }
            set { _anlInfo2 = value; }
        }

        private byte _anlInfo3;
        public byte AnlInfo3
        {
            get { return _anlInfo3; }
            set { _anlInfo3 = value; }
        }

        private byte _anlInfo4;
        public byte AnlInfo4
        {
            get { return _anlInfo4; }
            set { _anlInfo4 = value; }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] _time;
        public byte[] Time
        {
            get { return _time; }
            set { _time = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy25Dataset : SZLDataset
    {
        private byte _tpaNr;
        public byte tpa_nr
        {
            get { return _tpaNr; }
            set { _tpaNr = value; }
        }

        private byte _tpaUse;
        public byte tpa_use
        {
            get { return _tpaUse; }
            set { _tpaUse = value; }
        }

        private byte _obNr;
        public byte ob_nr
        {
            get { return _obNr; }
            set { _obNr = value; }
        }

        private byte _res;
        public byte res
        {
            get { return _res; }
            set { _res = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy31_1Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _pdu;
        public UInt16 Pdu
        {
            get { return _pdu; }
            set { _pdu = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _anz;
        public UInt16 Anz
        {
            get { return _anz; }
            set { _anz = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _mpiBps;
        public UInt32 Mpi_bps
        {
            get { return _mpiBps; }
            set { _mpiBps = value; }
        }

        [Endian(Endianness.BigEndian)]
        private uint _kbusBps;
        public UInt32 Kbus_bps
        {
            get { return _kbusBps; }
            set { _kbusBps = value; }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        private byte[] _res;
        public byte[] res
        {
            get { return _res; }
            set { _res = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy31_2Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private byte _funkt0;
        public byte funkt_0
        {
            get { return _funkt0; }
            set { _funkt0 = value; }
        }

        private byte _funkt1;
        public byte funkt_1
        {
            get { return _funkt1; }
            set { _funkt1 = value; }
        }

        private byte _funkt2;
        public byte funkt_2
        {
            get { return _funkt2; }
            set { _funkt2 = value; }
        }

        private byte _funkt3;
        public byte funkt_3
        {
            get { return _funkt3; }
            set { _funkt3 = value; }
        }

        private byte _funkt4;
        public byte funkt_4
        {
            get { return _funkt4; }
            set { _funkt4 = value; }
        }

        private byte _funkt5;
        public byte funkt_5
        {
            get { return _funkt5; }
            set { _funkt5 = value; }
        }

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

        private byte _trgereig0;
        public byte trgereig_0
        {
            get { return _trgereig0; }
            set { _trgereig0 = value; }
        }

        private byte _trgereig1;
        public byte trgereig_1
        {
            get { return _trgereig1; }
            set { _trgereig1 = value; }
        }

        private byte _trgereig2;
        public byte trgereig_2
        {
            get { return _trgereig2; }
            set { _trgereig2 = value; }
        }

        private byte _trgbed;
        public byte trgbed
        {
            get { return _trgbed; }
            set { _trgbed = value; }
        }

        private byte _pfad;
        public byte pfad
        {
            get { return _pfad; }
            set { _pfad = value; }
        }

        private byte _tiefe;
        public byte tiefe
        {
            get { return _tiefe; }
            set { _tiefe = value; }
        }

        private byte _systrig;
        public byte systrig
        {
            get { return _systrig; }
            set { _systrig = value; }
        }

        private byte _ergPar;
        public byte erg_par
        {
            get { return _ergPar; }
            set { _ergPar = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _ergPat1;
        public UInt16 erg_pat_1
        {
            get { return _ergPat1; }
            set { _ergPat1 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _ergPat2;
        public UInt16 erg_pat_2
        {
            get { return _ergPat2; }
            set { _ergPat2 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _force;
        public UInt16 force
        {
            get { return _force; }
            set { _force = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _time;
        public UInt16 time
        {
            get { return _time; }
            set { _time = value; }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private byte[] _res;
        public byte[] res
        {
            get { return _res; }
            set { _res = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy31_3Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private byte _funkt0;
        public byte funkt_0
        {
            get { return _funkt0; }
            set { _funkt0 = value; }
        }

        private byte _funkt1;
        public byte funkt_1
        {
            get { return _funkt1; }
            set { _funkt1 = value; }
        }

        private byte _funkt2;
        public byte funkt_2
        {
            get { return _funkt2; }
            set { _funkt2 = value; }
        }

        private byte _funkt3;
        public byte funkt_3
        {
            get { return _funkt3; }
            set { _funkt3 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private short _data;
        public Int16 Data
        {
            get { return _data; }
            set { _data = value; }
        }

        [Endian(Endianness.BigEndian)]
        private short _anz;
        public Int16 Anz
        {
            get { return _anz; }
            set { _anz = value; }
        }

        [Endian(Endianness.BigEndian)]
        private short _perMin;
        public Int16 Per_Min
        {
            get { return _perMin; }
            set { _perMin = value; }
        }

        [Endian(Endianness.BigEndian)]
        private short _perMax;
        public Int16 Per_Max
        {
            get { return _perMax; }
            set { _perMax = value; }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        private byte[] _res;
        public byte[] res
        {
            get { return _res; }
            set { _res = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy31_4Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private byte _funkt0;
        public byte funkt_0
        {
            get { return _funkt0; }
            set { _funkt0 = value; }
        }

        private byte _funkt1;
        public byte funkt_1
        {
            get { return _funkt1; }
            set { _funkt1 = value; }
        }

        private byte _funkt2;
        public byte funkt_2
        {
            get { return _funkt2; }
            set { _funkt2 = value; }
        }

        private byte _funkt3;
        public byte funkt_3
        {
            get { return _funkt3; }
            set { _funkt3 = value; }
        }

        private byte _funkt4;
        public byte funkt_4
        {
            get { return _funkt4; }
            set { _funkt4 = value; }
        }

        private byte _funkt5;
        public byte funkt_5
        {
            get { return _funkt5; }
            set { _funkt5 = value; }
        }

        private byte _funkt6;
        public byte funkt_6
        {
            get { return _funkt6; }
            set { _funkt6 = value; }
        }

        private byte _funkt7;
        public byte funkt_7
        {
            get { return _funkt7; }
            set { _funkt7 = value; }
        }

        private byte _kop;
        public byte kop
        {
            get { return _kop; }
            set { _kop = value; }
        }

        private byte _del;
        public byte del
        {
            get { return _del; }
            set { _del = value; }
        }

        private byte _kett;
        public byte kett
        {
            get { return _kett; }
            set { _kett = value; }
        }

        private byte _hoch;
        public byte hoch
        {
            get { return _hoch; }
            set { _hoch = value; }
        }

        private byte _ver;
        public byte ver
        {
            get { return _ver; }
            set { _ver = value; }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]        
        private byte[] _res;
        public byte[] res
        {
            get { return _res; }
            set { _res = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy31_5Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private byte _funkt0;
        public byte funkt_0
        {
            get { return _funkt0; }
            set { _funkt0 = value; }
        }

        private byte _funkt1;
        public byte funkt_1
        {
            get { return _funkt1; }
            set { _funkt1 = value; }
        }

        private byte _funkt2;
        public byte funkt_2
        {
            get { return _funkt2; }
            set { _funkt2 = value; }
        }

        private byte _funkt3;
        public byte funkt_3
        {
            get { return _funkt3; }
            set { _funkt3 = value; }
        }

        private byte _funkt4;
        public byte funkt_4
        {
            get { return _funkt4; }
            set { _funkt4 = value; }
        }

        private byte _funkt5;
        public byte funkt_5
        {
            get { return _funkt5; }
            set { _funkt5 = value; }
        }

        private byte _funkt6;
        public byte funkt_6
        {
            get { return _funkt6; }
            set { _funkt6 = value; }
        }

        private byte _funkt7;
        public byte funkt_7
        {
            get { return _funkt7; }
            set { _funkt7 = value; }
        }

        private byte _anzSen;
        public byte anz_sen
        {
            get { return _anzSen; }
            set { _anzSen = value; }
        }

        private byte _anzEin;
        public byte anz_ein
        {
            get { return _anzEin; }
            set { _anzEin = value; }
        }

        private byte _anzMel;
        public byte anz_mel
        {
            get { return _anzMel; }
            set { _anzMel = value; }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        private byte[] _res;
        public byte[] res
        {
            get { return _res; }
            set { _res = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy31_6Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private byte _funkt0;
        public byte funkt_0
        {
            get { return _funkt0; }
            set { _funkt0 = value; }
        }

        private byte _funkt1;
        public byte funkt_1
        {
            get { return _funkt1; }
            set { _funkt1 = value; }
        }

        private byte _funkt2;
        public byte funkt_2
        {
            get { return _funkt2; }
            set { _funkt2 = value; }
        }

        private byte _funkt3;
        public byte funkt_3
        {
            get { return _funkt3; }
            set { _funkt3 = value; }
        }

        private byte _funkt4;
        public byte funkt_4
        {
            get { return _funkt4; }
            set { _funkt4 = value; }
        }

        private byte _funkt5;
        public byte funkt_5
        {
            get { return _funkt5; }
            set { _funkt5 = value; }
        }

        private byte _funkt6;
        public byte funkt_6
        {
            get { return _funkt6; }
            set { _funkt6 = value; }
        }

        private byte _funkt7;
        public byte funkt_7
        {
            get { return _funkt7; }
            set { _funkt7 = value; }
        }

        private byte _schnell;
        public byte schnell
        {
            get { return _schnell; }
            set { _schnell = value; }
        }

        [Endian(Endianness.BigEndian)]
        private int _zugTyp;
        public Int32 zug_typ
        {
            get { return _zugTyp; }
            set { _zugTyp = value; }
        }

        private byte _zugtyp0;
        public byte zugtyp_0
        {
            get { return _zugtyp0; }
            set { _zugtyp0 = value; }
        }

        private byte _zugtyp1;
        public byte zugtyp_1
        {
            get { return _zugtyp1; }
            set { _zugtyp1 = value; }
        }

        private byte _zugtyp2;
        public byte zugtyp_2
        {
            get { return _zugtyp2; }
            set { _zugtyp2 = value; }
        }

        private byte _zugtyp3;
        public byte zugtyp_3
        {
            get { return _zugtyp3; }
            set { _zugtyp3 = value; }
        }

        private byte _zugtyp4;
        public byte zugtyp_4
        {
            get { return _zugtyp4; }
            set { _zugtyp4 = value; }
        }

        private byte _zugtyp5;
        public byte zugtyp_5
        {
            get { return _zugtyp5; }
            set { _zugtyp5 = value; }
        }

        private byte _zugtyp6;
        public byte zugtyp_6
        {
            get { return _zugtyp6; }
            set { _zugtyp6 = value; }
        }

        private byte _zugtyp7;
        public byte zugtyp_7
        {
            get { return _zugtyp7; }
            set { _zugtyp7 = value; }
        }

        private short _res1;
        public Int16 res1
        {
            get { return _res1; }
            set { _res1 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private short _maxSdEmpf;
        public Int16 max_sd_empf
        {
            get { return _maxSdEmpf; }
            set { _maxSdEmpf = value; }
        }

        [Endian(Endianness.BigEndian)]
        private short _maxSdAl8P;
        public Int16 max_sd_al8p
        {
            get { return _maxSdAl8P; }
            set { _maxSdAl8P = value; }
        }

        [Endian(Endianness.BigEndian)]
        private short _maxInst;
        public Int16 max_inst
        {
            get { return _maxInst; }
            set { _maxInst = value; }
        }

        [Endian(Endianness.BigEndian)]
        private short _res2;
        public Int16 res2
        {
            get { return _res2; }
            set { _res2 = value; }
        }

        private byte _verbProj;
        public byte verb_proj
        {
            get { return _verbProj; }
            set { _verbProj = value; }
        }

        private byte _verbProg;
        public byte verb_prog
        {
            get { return _verbProg; }
            set { _verbProg = value; }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        private byte[] _res;
        public byte[] res
        {
            get { return _res; }
            set { _res = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy32_1Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _index;
        public UInt16 Index
        {
            get { return _index; }
            set { _index = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _res_pg;
        public UInt16 Res_pg
        {
            get { return _res_pg; }
            set { _res_pg = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _res_os;
        public UInt16 Res_os
        {
            get { return _res_os; }
            set { _res_os = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _u_pg;
        public UInt16 U_pg
        {
            get { return _u_pg; }
            set { _u_pg = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _u_os;
        public UInt16 U_os
        {
            get { return _u_os; }
            set { _u_os = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _proj;
        public UInt16 Proj
        {
            get { return _proj; }
            set { _proj = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _auf;
        public UInt16 Auf
        {
            get { return _auf; }
            set { _auf = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _free;
        public UInt16 Free
        {
            get { return _free; }
            set { _free = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _used;
        public UInt16 Used
        {
            get { return _used; }
            set { _used = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _last;
        public UInt16 Last
        {
            get { return _last; }
            set { _last = value; }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        private byte[] _res;
        public byte[] res
        {
            get { return _res; }
            set { _res = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy71Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _redinf;
        public UInt16 redinf
        {
            get { return _redinf; }
            set { _redinf = value; }
        }

        private byte _mwstat1;
        public byte mwstat1
        {
            get { return _mwstat1; }
            set { _mwstat1 = value; }
        }

        private byte _mwstat2;
        public byte mwstat2
        {
            get { return _mwstat2; }
            set { _mwstat2 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _hsfcinfo;
        public UInt16 hsfcinfo
        {
            get { return _hsfcinfo; }
            set { _hsfcinfo = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _samfehl;
        public UInt16 samfehl
        {
            get { return _samfehl; }
            set { _samfehl = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _bzCpu0;
        public UInt16 bz_cpu_0
        {
            get { return _bzCpu0; }
            set { _bzCpu0 = value; }
        }

        [Endian(Endianness.BigEndian)]
        private ushort _bzCpu1;
        public UInt16 bz_cpu_1
        {
            get { return _bzCpu1; }
            set { _bzCpu1 = value; }
        }

        private byte _cpuValid;
        public byte cpu_valid
        {
            get { return _cpuValid; }
            set { _cpuValid = value; }
        }

        private byte _hsyncF;
        public byte hsync_f
        {
            get { return _hsyncF; }
            set { _hsyncF = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class xy74Dataset : SZLDataset
    {
        [Endian(Endianness.BigEndian)]
        private ushort _ledKennung;
        public UInt16 led_kennung
        {
            get { return _ledKennung; }
            set { _ledKennung = value; }
        }

        private byte _ledOn;
        public byte led_on
        {
            get { return _ledOn; }
            set { _ledOn = value; }
        }

        private byte _ledBlink;
        public byte led_blink
        {
            get { return _ledBlink; }
            set { _ledBlink = value; }
        }
    }
}
