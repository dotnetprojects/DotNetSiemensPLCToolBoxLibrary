namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class CPFolder : Step7ProjectFolder, IHardwareFolder
    {
        internal int UnitID;
        internal int TobjTyp;

        public int Rack { get; set; }
        public int Slot { get; set; }

        public int TobjIdNet;
        public System.Net.IPAddress IP { get; set; }
        public System.Net.IPAddress Mask { get; set; }
        public System.Net.IPAddress Router { get; set; }
        public System.Net.NetworkInformation.PhysicalAddress MAC { get; set; }
        public bool useRouter { get; set; }
        
    }
}
