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
using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    [Serializable()]
    public class TIADataBlock : TIAProgrammBlock, IDataBlock
    {
        public IDataRow Structure { get; set; }

        /// <summary>
        /// With this function you get the Structure with expanden Arrays!
        /// </summary>
        /// <returns></returns>
        public IDataRow GetArrayExpandedStructure(S7DataBlockExpandOptions myExpOpt)
        {
            return ((TiaAndSTep7DataBlockRow)Structure)._GetExpandedChlidren(myExpOpt)[0];
        }

        public DataBlockRow GetDataRowWithAddress(ByteBitAddress address)
        {
            var allRw = this.GetArrayExpandedStructure();
            return TiaAndSTep7DataBlockRow.GetDataRowWithAddress((TiaAndSTep7DataBlockRow)allRw, address);
        }

        private DataBlockRow expStruct = null;

        public IDataRow GetArrayExpandedStructure()
        {
            //Todo: Vergleich der Expand Options, und beim änderen eines inneren wertes des blocks, diesen löschen (erst bei schreibsup wichtig!)
            if (expStruct != null)
                return expStruct;
            return expStruct = (DataBlockRow)GetArrayExpandedStructure(new S7DataBlockExpandOptions());
        }
    }
}