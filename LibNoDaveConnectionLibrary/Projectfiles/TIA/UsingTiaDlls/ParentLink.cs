using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.UsingTiaDlls
{
    public class ParentLink
    {
        public ParentLink(string relId, string link)
        {

        }

        public Type RelationType { get; set; }

        public TiaStorrageObject Link { get; set; }
    }
}
