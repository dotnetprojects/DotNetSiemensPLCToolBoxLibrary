using System;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.UsingTiaDlls
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TiaTypeIdAttribute : Attribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}