namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    public class MC7ConvertingOptions
    {
        public MC7ConvertingOptions()
        {
            this.Memnoic = 0;
            this.CombineDBOpenAndDBAccess = true;
            this.ReplaceDBAccessesWithSymbolNames = true;
            this.ReplaceDIAccessesWithSymbolNames = true;
            this.ReplaceLokalDataAddressesWithSymbolNames = true;
            this.GenerateCallsfromUCs = false;
            this.UseInFCStoredFCsForCalls = true;
            this.UseComments = true;
        }

        public bool UseComments { get; set; }
        public int Memnoic { get; set; }
        public bool CombineDBOpenAndDBAccess { get; set; }
        public bool ReplaceDBAccessesWithSymbolNames { get; set; }

        public bool ReplaceLokalDataAddressesWithSymbolNames { get; set; }
        public bool ReplaceDIAccessesWithSymbolNames { get; set; } 
       

        public bool GenerateCallsfromUCs { get; set;}
        public bool UseInFCStoredFCsForCalls { get; set; }

        
    }
}
