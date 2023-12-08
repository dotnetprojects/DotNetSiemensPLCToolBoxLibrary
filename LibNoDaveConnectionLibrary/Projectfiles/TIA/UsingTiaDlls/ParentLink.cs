using System;

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