using System.Collections.Generic;

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