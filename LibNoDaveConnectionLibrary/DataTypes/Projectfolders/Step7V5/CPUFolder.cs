namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class CPUFolder : Step7ProjectFolder
    {
        internal int UnitID;
        internal int TobjTyp;        
        public PLCType CpuType { get; set; }

        public string PasswdHard { get; set; }
    }
}
