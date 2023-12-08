namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.UsingTiaDlls
{
    public class TiaObject
    {
        public TiaObject(TiaObjectStructure tiaObjectStructure)
        {
            this.TiaObjectStructure = tiaObjectStructure;
        }

        public TiaObjectStructure TiaObjectStructure { get; private set; }
    }
}