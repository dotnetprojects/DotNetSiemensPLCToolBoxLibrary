using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
{
    public class S5FunctionBlockRow : FunctionBlockRow
    {
         internal override void resetByte()
         {
             _MC5 = null;
         }


        //This Property has nothing to do with the Symboltable, its the S5 command
         internal object[] MC5LIB_SYMTAB_Row { get; set; }

        //These Commands are Combined...
        //public List<S7FunctionBlockRow> CombinedCommands { get; internal set; }

        private string _parameter;
        public string Parameter
        {
            get { return _parameter; }
            set
            {
                _parameter = value;
                _SymbolTableEntry = null;
                _MC5 = null;
                //CombinedCommands = null;
            }
        }

        private SymbolTableEntry _SymbolTableEntry;
        public SymbolTableEntry SymbolTableEntry
        {
            get { return _SymbolTableEntry; }
            set
            {
                _SymbolTableEntry = value;
            }
        }

        /*
        private string _symbol;
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;                
            }
        }
        */

        private List<S5Parameter> _extparameter;
        public List<S5Parameter> ExtParameter
        {
            get { return _extparameter; }
            set
            {
                _extparameter = value;
                _MC5 = null;
                //CombinedCommands = null;
            }
        }

        public string NetworkName { get; set; }

        private int _jumpwidth;
        internal int JumpWidth
        {
            get { return _jumpwidth; }
            set
            {
                _jumpwidth = value;
                _MC5 = null;
                //CombinedCommands = null;
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

        /*
        public PLCFunctionBlockAdressType GetParameterType()
        {
            return PLCFunctionBlockAdressType.Direct;
        }
        */

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
            /*if (Command == "BLD" && Parameter == "255")
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
            else*/ if (Command == "BLD" && Parameter == "130")
            {
                if (!string.IsNullOrEmpty(Label))
                    return Label.PadRight(4) + ": ";
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
                foreach (S5Parameter myStr in ExtParameter)
                {
                    //newRow.ExtParameter.Add(s5Parameter.Name.PadRight(5, ' ') + ":  " + akOper);
                    string akcmt = (myStr.Comment ?? "") == "" ? "" : "//" + myStr.Comment;
                    ext += "\r\n" + (" ".PadLeft(12) + myStr.Name.PadRight(5, ' ') + ": " + (myStr.Value ?? "")).PadRight(35) + akcmt;
                }
            }

            if (Command == "" && Parameter == "" && retVal=="")
                return cmt;

            string par = "";
            if (Parameter != null)
                par = Parameter;

            if (_SymbolTableEntry != null && !string.IsNullOrEmpty(SymbolTableEntry.Symbol))
                par = "-" + SymbolTableEntry.Symbol;

            return (retVal + Command.PadRight(6) + par).PadRight(35) + cmt + ext; // +"Sz:" + ByteSize.ToString();
        }
    }
}
