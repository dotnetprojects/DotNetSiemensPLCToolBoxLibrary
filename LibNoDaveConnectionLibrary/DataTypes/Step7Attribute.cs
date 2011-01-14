namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public class Step7Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Step7Attribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public Step7Attribute(string NameGleichValue)
        {
            if (NameGleichValue.Contains("="))
            {
                Name = NameGleichValue.Split('=')[0];
                Value = NameGleichValue.Split('=')[1];
            }
            else
            {
                Name = NameGleichValue;
            }
        }

        public override string ToString()
        {
            return Name + "=" + Value;
        }
    }
}
