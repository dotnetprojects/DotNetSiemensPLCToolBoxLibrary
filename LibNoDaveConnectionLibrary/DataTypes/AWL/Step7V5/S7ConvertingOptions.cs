using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5
{
    public class S7ConvertingOptions
    {
        protected bool Equals(S7ConvertingOptions other)
        {
            return this.UseComments.Equals(other.UseComments) && this.Mnemonic == other.Mnemonic && this.CombineDBOpenAndDBAccess.Equals(other.CombineDBOpenAndDBAccess) && this.ReplaceDBAccessesWithSymbolNames.Equals(other.ReplaceDBAccessesWithSymbolNames) && this.ReplaceLokalDataAddressesWithSymbolNames.Equals(other.ReplaceLokalDataAddressesWithSymbolNames) && this.ReplaceDIAccessesWithSymbolNames.Equals(other.ReplaceDIAccessesWithSymbolNames) && this.GenerateCallsfromUCs.Equals(other.GenerateCallsfromUCs) && this.UseInFCStoredFCsForCalls.Equals(other.UseInFCStoredFCsForCalls);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.UseComments.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.Mnemonic;
                hashCode = (hashCode * 397) ^ this.CombineDBOpenAndDBAccess.GetHashCode();
                hashCode = (hashCode * 397) ^ this.ReplaceDBAccessesWithSymbolNames.GetHashCode();
                hashCode = (hashCode * 397) ^ this.ReplaceLokalDataAddressesWithSymbolNames.GetHashCode();
                hashCode = (hashCode * 397) ^ this.ReplaceDIAccessesWithSymbolNames.GetHashCode();
                hashCode = (hashCode * 397) ^ this.GenerateCallsfromUCs.GetHashCode();
                hashCode = (hashCode * 397) ^ this.UseInFCStoredFCsForCalls.GetHashCode();
                return hashCode;
            }
        }

        public S7ConvertingOptions()
            : this(MnemonicLanguage.German)
        {
        }

        public S7ConvertingOptions(MnemonicLanguage Mnemonic)
        {
            this.Mnemonic = Mnemonic;
            this.CombineDBOpenAndDBAccess = true;
            this.ReplaceDBAccessesWithSymbolNames = true;
            this.ReplaceDIAccessesWithSymbolNames = true;
            this.ReplaceLokalDataAddressesWithSymbolNames = true;
            this.GenerateCallsfromUCs = true;
            this.UseInFCStoredFCsForCalls = true; //todo implement the reading of them in the step7 project
            this.UseComments = true;
        }

        public bool UseComments { get; set; }
        public MnemonicLanguage Mnemonic { get; set; }
        public bool CombineDBOpenAndDBAccess { get; set; }
        public bool ReplaceDBAccessesWithSymbolNames { get; set; }

        public bool ReplaceLokalDataAddressesWithSymbolNames { get; set; }
        public bool ReplaceDIAccessesWithSymbolNames { get; set; } 
       

        public bool GenerateCallsfromUCs { get; set;}
        public bool UseInFCStoredFCsForCalls { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((S7ConvertingOptions)obj);
        }

    }
}
