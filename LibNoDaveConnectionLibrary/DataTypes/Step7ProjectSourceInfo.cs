namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public class Step7ProjectSourceInfo
    {
        internal int id;
        public bool Deleted { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }

        public override string ToString()
        {
            if (Deleted)
                return "$$_" + Name;
            return Name;
        }
    }
}
