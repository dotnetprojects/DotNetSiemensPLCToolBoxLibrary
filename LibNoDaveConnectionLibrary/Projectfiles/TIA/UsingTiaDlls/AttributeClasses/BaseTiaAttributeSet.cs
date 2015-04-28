using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.UsingTiaDlls.AttributeClasses
{
    public class BaseTiaAttributeSet : TiaObject
    {
        internal BaseTiaAttributeSet(TiaObjectStructure tiaObjectStructure)
            : base(tiaObjectStructure)
        {
        }

        public static BaseTiaAttributeSet CreateBaseTiaAttribute(TiaObjectStructure tiaObjectStructure, int id)
        {
            return new BaseTiaAttributeSet(tiaObjectStructure);
        }
    }
}
