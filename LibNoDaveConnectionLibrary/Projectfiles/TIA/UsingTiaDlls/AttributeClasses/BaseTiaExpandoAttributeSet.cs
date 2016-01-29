using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.UsingTiaDlls.AttributeClasses
{
    public class BaseTiaExpandoAttributeSet : BaseTiaAttributeSet
    {
        internal BaseTiaExpandoAttributeSet(TiaObjectStructure tiaObjectStructure)
            : base(tiaObjectStructure)
        { }

        public List<BaseTiaAttributeSet> Entries { get; set; }
    }
}
