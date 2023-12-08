namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.UsingTiaDlls.AttributeClasses
{
    [TiaAttributeId(Id = 77825, Name = "Siemens.Automation.ObjectFrame.ICoreAttributes")]
    public class CoreAttributes : BaseTiaAttributeSet
    {
        internal CoreAttributes(TiaObjectStructure tiaObjectStructure)
            : base(tiaObjectStructure)
        { }

        public bool IsShell { get; set; }
        public bool Name { get; set; }
    }
}