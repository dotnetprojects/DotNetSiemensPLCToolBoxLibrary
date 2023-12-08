//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.AttribSets
//{
//    public class CoreAttributes
//    {
//        private string Author { get; set; }
//        //ICoreText Comment { get; set; }
//        private DateTime CreationTime { get; set; }
//        private string Culture { get; set; }
//        //ICoreObject DerivedFrom { get; set; }
//        //ICoreObject Environment { get; }
//        //ICoreObject InstanceOf { get; set; }
//        private bool IsConstant { get; set; }
//        private bool IsCreated { get; }
//        private bool IsCreating { get; }
//        private bool IsDeleted { get; }
//        private bool IsDeleting { get; }
//        private bool IsInternal { get; set; }

//        [Obsolete(
//            "The attribute is marked as 'obsolete' in the meta model and is about to be removed later on, its use should be avoided"
//            )]
//        private bool IsKnowHowProtected { get; set; }

//        private bool IsModified { get; }
//        private bool IsReadLocked { get; }
//        private bool IsReadOnly { get; }
//        private bool IsShell { get; set; }
//        private bool IsType { get; }
//        private bool IsTypePart { get; }
//        private bool IsUndeletable { get; set; }
//        private bool IsWriteLocked { get; }
//        private string LastModifiedBy { get; set; }
//        private DateTime ModifiedTime { get; set; }
//        private string Name { get; set; }
//        //ICoreObject Parent { get; set; }
//        private string Password { get; set; }
//        //ProtectionMode Protection { get; set; }
//        private int ProtectionVersionId { get; set; }
//        private string Subtype { get; set; }
//        //ICoreObject Target { get; }
//        //TRefStateEnum TRefState { get; }
//        private string Version { get; set; }
//    }
//}