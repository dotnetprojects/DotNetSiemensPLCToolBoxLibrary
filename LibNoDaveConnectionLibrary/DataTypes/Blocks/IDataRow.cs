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

using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public interface IDataRow
    {
        List<IDataRow> Children { get; }

        S7DataRowType DataType { get; set; }

        string Name { get; set; }
        string FullName { get; }

        string Comment { get; set; }
        string FullComment { get; }

        IDataRow Parent { get; set; }

        ByteBitAddress BlockAddress { get; set; }

        Block PlcBlock { get; }

        string StructuredName { get; }
    }
}