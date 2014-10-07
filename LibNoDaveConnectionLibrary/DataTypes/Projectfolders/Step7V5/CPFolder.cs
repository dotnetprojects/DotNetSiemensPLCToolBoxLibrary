namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class CPFolder : Step7ProjectFolder, IHardwareFolder
    {
        internal int UnitID;
        internal int TobjTyp;

        public int Rack { get; set; }
        public int Slot { get; set; }
    }
}
