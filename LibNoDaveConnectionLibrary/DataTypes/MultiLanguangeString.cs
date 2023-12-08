//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Xml;
//using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

//namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
//{
//    public class MultiLanguangeString
//    {
//        private XmlNode node;

//        public MultiLanguangeString(XmlNode node)
//        {
//            this.node = node;

//        }

//        public string GetText(CultureInfo cultureInfo, Step7ProjectV11 tiaProject)
//        {
//            var xmlnode = node.SelectSingleNode("coreText/cultures/culture[@id='" + cultureInfo.LCID + "']");

//            if (xmlnode != null)
//            {
//                var val = xmlnode.Attributes["value"];
//                if (val != null)
//                    return val.Value;
//            }

//            var lidNode = node.SelectSingleNode("attribSet[@id='" + tiaProject.asId2Names.First(itm => itm.Value == "Siemens.Automation.ObjectFrame.ICoreTextExtendedData").Key + "']/attrib[@name='DefaultText']");

//            return lidNode.InnerText;
//        }
//    }
//}