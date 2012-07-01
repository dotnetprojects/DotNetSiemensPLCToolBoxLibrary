using System.Collections;

namespace JFKCommonLibrary.ExtensionMethods
{
    public static class ByteArrayExtensions
    {
        public static bool ByteArrayCompare(this byte[] a1, byte[] a2)
        {
            IStructuralEquatable eqa1 = a1;
            return eqa1.Equals(a2, StructuralComparisons.StructuralEqualityComparer);
        }
    }
}
