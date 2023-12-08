using System;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.S7_xxx
{
    public class SZLData
    {
        public Int16 SzlId;
        public Int16 Index;
        public Int16 Size;
        public Int16 Count;
        public SZLDataset[] SZLDaten;
    }
}