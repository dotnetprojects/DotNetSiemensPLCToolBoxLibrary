using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5
{
    /// <summary>
    /// Options that define how Step7 Project will behave when opened by this library. 
    /// </summary>
    public class S7ConvertingOptions
    {
        protected bool Equals(S7ConvertingOptions other)
        {
            return this.UseComments.Equals(other.UseComments) 
                && this.Mnemonic == other.Mnemonic 
                && this.CombineDBOpenAndDBAccess.Equals(other.CombineDBOpenAndDBAccess) 
                && this.ReplaceDBAccessesWithSymbolNames.Equals(other.ReplaceDBAccessesWithSymbolNames) 
                && this.ReplaceLokalDataAddressesWithSymbolNames.Equals(other.ReplaceLokalDataAddressesWithSymbolNames) 
                && this.ReplaceDIAccessesWithSymbolNames.Equals(other.ReplaceDIAccessesWithSymbolNames) 
                && this.GenerateCallsfromUCs.Equals(other.GenerateCallsfromUCs) 
                && this.UseInFCStoredFCsForCalls.Equals(other.UseInFCStoredFCsForCalls)
                && this.UseFBDeclarationForInstanceDB.Equals(other.UseFBDeclarationForInstanceDB)
                && this.UseDBActualValues.Equals(other.UseDBActualValues)
                && this.ExpandArrays.Equals(other.ExpandArrays)
                && this.CheckForInterfaceTimestampConflicts.Equals(other.CheckForInterfaceTimestampConflicts);
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
                hashCode = (hashCode * 397) ^ this.UseFBDeclarationForInstanceDB.GetHashCode();
                hashCode = (hashCode * 397) ^ this.UseDBActualValues.GetHashCode();
                hashCode = (hashCode * 397) ^ this.ExpandArrays.GetHashCode();
                hashCode = (hashCode * 397) ^ this.CheckForInterfaceTimestampConflicts.GetHashCode();
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
            this.UseFBDeclarationForInstanceDB = true; //Default to Simatic Mamager Behavior
            this.UseDBActualValues = false;
            this.ExpandArrays = false;
            this.CheckForInterfaceTimestampConflicts = false;
        }

        public bool UseComments { get; set; }
        public MnemonicLanguage Mnemonic { get; set; }
        public bool CombineDBOpenAndDBAccess { get; set; }
        public bool ReplaceDBAccessesWithSymbolNames { get; set; }
        public bool UseDBActualValues { get; set; }
        public bool ExpandArrays { get; set; }

        public bool ReplaceLokalDataAddressesWithSymbolNames { get; set; }
        public bool ReplaceDIAccessesWithSymbolNames { get; set; } 
       

        public bool GenerateCallsfromUCs { get; set;}
        public bool UseInFCStoredFCsForCalls { get; set; }

        public bool CheckForInterfaceTimestampConflicts { get; set; }

        /// <summary>
        /// use the FB instance declartion symbolics for displaying Instance DB Variables
        /// Otherwise it is using the Symbolics stored for each individual instance DB, which may be different than the Function Block
        /// The Step7 Default is TRUE. Simatic Manager always shows the FB declarations.
        /// </summary>
        public bool UseFBDeclarationForInstanceDB { get; set; }

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
