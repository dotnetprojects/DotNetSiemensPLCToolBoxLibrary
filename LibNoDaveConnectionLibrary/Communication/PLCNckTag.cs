/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 WPFToolboxForSiemensPLCs is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 WPFToolboxForSiemensPLCs is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
using System;



namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
#if !IPHONE
    [System.ComponentModel.Editor(typeof(NckTagUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
	[Serializable]
	public class PLCNckTag : PLCTag
	{
		public PLCNckTag()
		{
		}

		public int NckArea { get; set; }
		public int NckUnit { get; set; }
		public int NckColumn { get; set; }
		public int NckLine { get; set; }
		public int NckModule { get; set; }
		public int NckLinecount { get; set; }

		public override  bool DontSplitValue
		{
			get { return true; }
			set { }
		}

        public override string ToString()
        {
            string old = "";
            if (_oldvalues != null)
            {
                old = "   -- Old-Values: ";
                foreach (var oldvalue in _oldvalues)
                {
                    old += oldvalue.ToString() + ",";
                }
                old += "";
            }

            string s = string.Format("0x{0},0x{1},0x{2},0x{3},0x{4},0x{5},{6},0x{7}", NckArea.ToString("X"), NckUnit.ToString("X"), NckColumn.ToString("X"), NckLine.ToString("X"), NckModule.ToString("X"), NckLinecount.ToString("X"), TagDataType, _internalGetSize().ToString("X"));

            if (Value != null)
            {
                return s + " = " + GetValueAsString() + old;
            }
            return s;
        }

        //Todo: look how long a NCK Request is???
        //internal override int _internalGetSize()
        //{
        //	return 1;
        //}
	}
}
