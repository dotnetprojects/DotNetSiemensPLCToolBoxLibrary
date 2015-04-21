using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.UsingTiaDlls.AttributeClasses;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.UsingTiaDlls
{
    public class TiaStorrageObject : TiaObject
    {
        protected TiaStorrageObject(TiaObjectStructure tiaObjectStructure)
            : base(tiaObjectStructure)
        {
            AttributeSets = new List<BaseTiaAttributeSet>();

        }

        public static TiaStorrageObject CreateStorrageObject(TiaObjectStructure tiaObjectStructure, int id, int instId,
            int clusterId)
        {
            return new TiaStorrageObject(tiaObjectStructure);
        }

        public int InstId { get; set; }

        public List<BaseTiaAttributeSet> AttributeSets { get; private set; }

        //coreText
        //relation (multiple) typ abhänig, multiple childs
        //parentlink
    }
}
