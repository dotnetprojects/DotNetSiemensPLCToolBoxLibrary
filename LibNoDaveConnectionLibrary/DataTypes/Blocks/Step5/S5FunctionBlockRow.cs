using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
{
    public class S5FunctionBlockRow
    {
         public string Label { get; set; }

        //These Commands are Combined...
        public List<S7FunctionBlockRow> CombinedCommands { get; internal set; }

        private string _command;
        public string Command
        {
            get { return _command.Trim().ToUpper(); }
            set
            {
                _command = value.Trim().ToUpper();
                _MC5 = null;
                CombinedCommands = null;
            }
        }

        private string _parameter;
        public string Parameter
        {
            get { return _parameter; }
            set
            {
                _parameter = value;
                _symbol = null;
                _MC5 = null;
                CombinedCommands = null;
            }
        }

        private string _symbol;
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;                
            }
        }

        private List<string> _extparameter;
        public List<string> ExtParameter
        {
            get { return _extparameter; }
            set
            {
                _extparameter = value;
                _MC5 = null;
                CombinedCommands = null;
            }
        }

        public string Comment { get; set; }

        public string NetworkName { get; set; }

        private int _jumpwidth;
        internal int JumpWidth
        {
            get { return _jumpwidth; }
            set
            {
                _jumpwidth = value;
                _MC5 = null;
                CombinedCommands = null;
            }
        }

        private byte[] _MC5;
        public byte[] MC5
        {
            get
            {
                if (_MC5 != null)
                    return _MC5;
                //else
                    //return AWLtoMC7.GetMC7(this);
                return null;
            }
            internal set
            {
                _MC5 = value;
            }
        }

        public PLCFunctionBlockAdressType GetParameterType()
        {
            return PLCFunctionBlockAdressType.Direct;
        }

        public S5FunctionBlockRow()
        {
            Label = "";
            Command = "";
            Parameter = "";
            Comment = "";
            NetworkName = "";
        }

        public int ByteSize
        {
            get
            {
                if (MC5 != null)
                    return MC5.Length;
                else
                    return 0;
            }
        }

        public override string ToString()
        {
            if (Command == "BLD" && Parameter == "255")
            {
                string lbl = "";
                if (!string.IsNullOrEmpty(Label))
                    lbl = Label + ": ";               
                if (string.IsNullOrEmpty(Comment))
                    return lbl+"Netzwerk " + " : " + NetworkName;
                else
                    return lbl+"Netzwerk " + " : " + NetworkName + "\r\n\t Comment : " +
                           Comment.Replace("\n", "\r\n\t           ");
            }
            else if (Command == "BLD" && Parameter == "130")
            {
                return ""; // +"Sz:" + ByteSize.ToString();
            }
            string retVal = "";
            if (Label == null || Label == "")
                retVal += new string(' ', 6);
            else
                retVal += Label.PadRight(4) + ": ";

            string cmt = "";
            if (Comment != null && Comment != "")
                cmt = "//" + Comment;

            string ext = "";
            if (ExtParameter != null && ExtParameter.Count > 0)
            {
                foreach (string myStr in ExtParameter)
                    ext += "\r\n" + " ".PadLeft(12) + myStr;
            }

            if (Command == "" && Parameter == "")
                return cmt;

            string par = "";
            if (Parameter != null)
                par = Parameter;
            if (Symbol != null && Symbol != "")
                par = Symbol;

            return retVal + Command.PadRight(6) + par.PadRight(14) + cmt + ext; // +"Sz:" + ByteSize.ToString();
        }
    }
}
