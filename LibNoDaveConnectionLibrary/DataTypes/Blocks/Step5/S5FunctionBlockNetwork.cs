namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
{
    public class S5FunctionBlockNetwork : Network
    {
        public S5FunctionBlockRow NetworkFunctionBlockRow { get; set;}

        public override string Name
        {
            get {return NetworkFunctionBlockRow.NetworkName; }
            set { NetworkFunctionBlockRow.NetworkName = value; }
        }
    }
}
