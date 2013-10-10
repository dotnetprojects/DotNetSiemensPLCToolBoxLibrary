using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Hardware.Step7V5
{
    public class MasterSystem : ProjectFolder
    {
        public int Id { get; set; }        

        private List<Node> _children = new List<Node>();
        public List<Node> Children
        {
            get { return _children; }
            set { _children = value; }
        }
    }
}
