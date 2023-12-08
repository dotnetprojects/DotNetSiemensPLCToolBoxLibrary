using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    [Serializable()]
    public class TIAProgrammBlock : TIABlock
    {
        //        public TIAProgrammBlock(Step7ProjectV11 TIAProject, XmlNode Node) : base(TIAProject, Node)
        //        { }

        //        public override int BlockNumber
        //        {
        //            get
        //            {
        //                var lidNode = node.SelectSingleNode("attribSet[@id='" + tiaProject.asId2Names.First(itm => itm.Value == "Siemens.Simatic.PlcLanguages.Model.IGeneralBlockData").Key + "']/attrib[@name='Number']");

        //                if (lidNode != null)
        //                    return Convert.ToInt32(lidNode.InnerText);

        //                return 0;
        //            }
        //        }

        //        public override string Title
        //        {
        //            get
        //            {
        //                if (Comments == null)
        //                {
        //                    var lidNode =
        //                        node.SelectSingleNode("relation[@id='" +
        //                                              tiaProject.relationId2Names.First(
        //                                                  itm =>
        //                                                      itm.Value ==
        //                                                      "Siemens.Automation.ObjectFrame.CoreObject.Comment")
        //                                                  .Key + "']/link");
        //                    if (lidNode != null)
        //                    {
        //                        var prjObjId = lidNode.InnerText;
        //                        var xmlnode =
        //                            this.tiaProject.xmlDoc.SelectSingleNode("root/objects/StorageObject[@instId='" +
        //                                                                    prjObjId.Split('-')[1] + "']");

        //                        this.Titels = new MultiLanguangeString(xmlnode);
        //                    }
        //                }

        //                if (this.Titels != null)
        //                    return this.Titels.GetText(this.tiaProject.Culture, this.tiaProject);
        //                return null;
        //            }
        //        }

        //        public override string Comment
        //        {
        //            get
        //            {
        //                if (Comments == null)
        //                {
        //                    var lidNode =
        //                        node.SelectSingleNode("relation[@id='" +
        //                                              tiaProject.relationId2Names.First(
        //                                                  itm =>
        //                                                      itm.Value ==
        //                                                      "Siemens.Simatic.PlcLanguages.Model.CodeBlockData.BlockComment")
        //                                                  .Key + "']/link");
        //                    if (lidNode != null)
        //                    {
        //                        var prjObjId = lidNode.InnerText;
        //                        var xmlnode =
        //                            this.tiaProject.xmlDoc.SelectSingleNode("root/objects/StorageObject[@instId='" +
        //                                                                    prjObjId.Split('-')[1] + "']");

        //                        this.Comments = new MultiLanguangeString(xmlnode);
        //                    }
        //                }

        //                if (this.Comments!=null)
        //                    return this.Comments.GetText(this.tiaProject.Culture, this.tiaProject);
        //                return null;
        //            }
        //        }
    }
}