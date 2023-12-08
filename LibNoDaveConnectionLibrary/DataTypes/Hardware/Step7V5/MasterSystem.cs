using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using System.Collections.Generic;

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