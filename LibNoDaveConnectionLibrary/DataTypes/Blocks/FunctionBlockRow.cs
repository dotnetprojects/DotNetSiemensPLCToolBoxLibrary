using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public abstract class FunctionBlockRow
    {
        internal virtual void resetByte() {}

        public string Label { get; set; }

        public DotNetSiemensPLCToolBoxLibrary.Projectfiles.Project.Language ProjectLanguage { get; set; }

        private string _command;
        public string Command
        {
            get { return _command.Trim().ToUpper(); }
            set
            {
                _command = value.Trim().ToUpper();
                resetByte();
            }
        }
    }
}
