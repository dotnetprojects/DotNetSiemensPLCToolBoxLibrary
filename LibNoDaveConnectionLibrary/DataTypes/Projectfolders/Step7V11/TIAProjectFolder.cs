//using System.Xml;

//using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

//namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
//{
//    /// <summary>
//    /// Base Abstract Class for every Project Folder.
//    /// </summary>
//    public class TIAProjectFolder : ProjectFolder
//    {
//        //internal string ID { get; private set; }
//        internal string InstID { get; private set; }

//        internal XmlNode Node { get; private set; }

//        internal XmlNodeList SubNodes { get; set; }

//        protected Step7ProjectV11 TiaProject;

//        public override string Name
//        {
//            get
//            {
//                var nameNode = Node.SelectSingleNode("attribSet[@id='" + ((Step7ProjectV11)Project).CoreAttributesId + "']/attrib[@name='Name']");
//                if (nameNode!=null)
//                    return nameNode.InnerText;
//                return "";
//            }
//            set
//            {
//                //base.Name = value;
//            }
//        }
//        public TIAProjectFolder(Step7ProjectV11 Project, XmlNode Node)
//        {
//            this.Project = Project;
//            this.TiaProject = Project;
//            this.Node = Node;
//        }

//    }
//}