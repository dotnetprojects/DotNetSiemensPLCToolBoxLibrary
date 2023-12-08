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

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    [Serializable()]
    public class TIABlock : Block
    {
        //        protected XmlNode node;
        //        protected Step7ProjectV11 tiaProject;

        //        public TIABlock(Step7ProjectV11 TIAProject, XmlNode Node)
        //        {
        //            this.node = Node;
        //            this.tiaProject = TIAProject;
        //        }

        //        public override string Name
        //        {
        //            get
        //            {
        //                return node.SelectSingleNode("attribSet[@id='" + tiaProject.CoreAttributesId + "']/attrib[@name='Name']").InnerText;
        //            }
        //        }

        //        internal S7ConvertingOptions usedS7ConvertingOptions;

        //        public string BlockVersion;

        //        public String BlockAttribute; // .0 not unlinked, .1 standart block + know how protect, .3 know how protect, .5 not retain

        //        public List<Step7Attribute> Attributes { get; set; }

        //        public double Length;

        //        public virtual string Title { get; set; }

        //        public virtual string Comment { get; set; }

        //        public string Author { get; set; }

        //        public string Family { get; set; }

        //        public string Version { get; set; }

        //        public DateTime LastCodeChange { get; set; }

        //        public DateTime LastInterfaceChange { get; set; }

        //        public virtual MultiLanguangeString Comments { get; set; }

        //        public virtual MultiLanguangeString Titels { get; set; }
    }
}