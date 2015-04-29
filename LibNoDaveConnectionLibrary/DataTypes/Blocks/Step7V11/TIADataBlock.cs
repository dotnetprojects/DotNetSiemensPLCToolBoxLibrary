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
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    [Serializable()]
    public class TIADataBlock : TIAProgrammBlock, IDataBlock
    {


        //        public TIADataBlock(Step7ProjectV11 TIAProject, XmlNode Node)
        //            : base(TIAProject, Node)
        //        {            
        //        }

        //        public override string Name
        //        {
        //            get
        //            {
        //                return base.Name;
        //            }
        //            set
        //            {

        //            }
        //        }

        //        public bool SymbolicDataBlock { get; set; }

        //        //public override int BlockNumber
        //        //{
        //        //    get
        //        //    {
        //        //        var lidNode = node.SelectSingleNode("attribSet[@id='" + tiaProject.asId2Names.First(itm => itm.Value == "Siemens.Simatic.PlcLanguages.Model.IGeneralBlockData").Key + "']/attrib[@name='Number']");
        //        //        if (lidNode != null) return Convert.ToInt32(lidNode.InnerText);
        //        //        return 0;
        //        //    }
        //        //}

        //        public int FBNumber { get; set;}  //If it is a Instance DB
        //        public bool IsInstanceDB { get; set; }

        //        public IDataRow Structure
        //        {
        //            get
        //            {
        //                return new TIADataRow(node, tiaProject, this);
        //            }
        //            set
        //            {

        //            }
        //        }

        //        public override string ToString()
        //        {
        //            string retVal = "";
        //            if (this.BlockType == PLCBlockType.UDT)
        //                retVal += "UDT";
        //            else
        //                retVal += "DB";
        //            retVal += this.BlockNumber.ToString() + Environment.NewLine;
        //            if (this.Structure != null)
        //                retVal += this.Structure.ToString();
        //            return retVal;
        //        }       
        public IDataRow Structure { get; set; }
    }
}
