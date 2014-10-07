namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class CPUFolder : Step7ProjectFolder, IHardwareFolder
    {
        internal int UnitID;
        internal int TobjTyp;        
        public PLCType CpuType { get; set; }

        public string PasswdHard { get; set; }

        public int Rack { get; set; }
        public int Slot { get; set; }
    }
}
