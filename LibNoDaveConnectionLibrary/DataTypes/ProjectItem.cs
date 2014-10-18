using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public class ProjectItem
    {
        public virtual string Name { get; set; }

        public virtual string Author { get; set; }
        public virtual string Comment { get; set; }

        public virtual DateTime? Created { get; set; }
        public virtual DateTime? Modified { get; set; }
    }
}
