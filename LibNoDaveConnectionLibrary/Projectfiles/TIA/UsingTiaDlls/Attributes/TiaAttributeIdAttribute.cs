using System;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.UsingTiaDlls
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TiaAttributeIdAttribute : Attribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}