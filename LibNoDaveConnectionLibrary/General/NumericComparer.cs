// (c) Vasian Cepa 2005
// Version 2

using System.Collections;

// required for NumericComparer : IComparer only

namespace DotNetSiemensPLCToolBoxLibrary.General
{

	public class NumericComparer<T> : System.Collections.Generic.IComparer<T>
	{
		public NumericComparer()
		{}
				
	    public int Compare(T x, T y)
	    {
            return StringLogicalComparer.Compare(x.ToString(), y.ToString());            
	    }
	}

    public class NumericComparer : IComparer
    {
        public NumericComparer()
        { }

        public int Compare(object x, object y)
        {
            return StringLogicalComparer.Compare(x.ToString(), y.ToString());                
        }
    }
}