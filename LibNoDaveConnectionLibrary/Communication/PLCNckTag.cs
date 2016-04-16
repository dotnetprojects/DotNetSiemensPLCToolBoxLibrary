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

		//Todo: look how long a NCK Request is???
		//internal override int _internalGetSize()
		//{
		//	return 1;
		//}
	}
}
