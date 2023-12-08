//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Xml;

//using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
//using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11;
//using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

//namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
//{
//    public class TIABlocksFolder : TIAProjectFolder, IBlocksFolder
//    {
//        public String Folder { get; set; }

//        private static string IXmlPartDataId = null;
//        private static string BlockInterfaceBaseDataSourceId = null;

//        public TIABlocksFolder(Step7ProjectV11 Project, XmlNode Node)
//            : base(Project, Node)
//        {
//            if (IXmlPartDataId == null)
//                IXmlPartDataId = Project.asId2Names.FirstOrDefault(itm => itm.Value == "Siemens.Simatic.PlcLanguages.Model.IXmlPartData").Key;

//            if (BlockInterfaceBaseDataSourceId == null)
//                BlockInterfaceBaseDataSourceId = Project.relationId2Names.FirstOrDefault(itm => itm.Value == "Siemens.Simatic.PlcLanguages.Model.BlockInterfaceBaseData.Source").Key;

//            //"Siemens.Simatic.PlcLanguages.Model.StructureItemData"
//        }

//        private static byte[] StringToByteArrayFastest(string hex)
//        {
//            if (hex.Length % 2 == 1)
//                throw new Exception("The binary key cannot have an odd number of digits");

//            byte[] arr = new byte[hex.Length >> 1];

//            for (int i = 0; i < hex.Length >> 1; ++i)
//            {
//                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
//            }

//            return arr;
//        }

//        private static int GetHexVal(char hex)
//        {
//            int val = (int)hex;
//            //For uppercase A-F letters:
//            return val - (val < 58 ? 48 : 55);
//            //For lowercase a-f letters:
//            //return val - (val < 58 ? 48 : 87);
//            //Or the two combined, but a bit slower:
//            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
//        }

//        private List<ProjectBlockInfo> blockList = null;
//        public List<ProjectBlockInfo> readPlcBlocksList()
//        {
//            if (blockList == null)
//            {
//                blockList = new List<ProjectBlockInfo>();
//                foreach (XmlNode subNode in SubNodes)
//                {
//                    string id = subNode.Attributes["id"].Value;
//                    string tiaType = TiaProject.importTypeInfos[id];

//                    if (tiaType == "Siemens.Simatic.PlcLanguages.Model.DataBlockData")
//                    {
//                        //BlockInterfaceBaseDataSourceId+"-"+
//                        var nm = subNode.SelectSingleNode("attribSet[@id='" + TiaProject.CoreAttributesId + "']/attrib[@name='Name']").InnerText;

//                        var link = subNode.SelectSingleNode("relation[@id='" + BlockInterfaceBaseDataSourceId + "']/link").InnerText;

//                        var payloadNode = ((Step7ProjectV11)this.Project).xmlDoc.SelectSingleNode("root/objects/StorageObject[@instId='" + link.Split('-')[1] + "']");
//                        var payload = payloadNode.SelectSingleNode("attribSet[@id='" + IXmlPartDataId + "']/attrib[@name='PayLoad']").InnerText;

//                        var bytes = StringToByteArrayFastest(payload);
//                        var txt = Encoding.UTF8.GetString(bytes);
//                        var blk = new TIAProjectBlockInfo(subNode) { Name = nm, BlockType = PLCBlockType.DB };
//                        blockList.Add(blk);
//                    }
//                    else if (tiaType == "Siemens.Simatic.PlcLanguages.Model.CodeBlockData")
//                    {
//                        var nm = subNode.SelectSingleNode("attribSet[@id='" + TiaProject.CoreAttributesId + "']/attrib[@name='Name']").InnerText;
//                        var tp = subNode.SelectSingleNode("attribSet[@id='" + TiaProject.CoreAttributesId + "']/attrib[@name='Subtype']").InnerText;

//                        var typ = PLCBlockType.FC;
//                        if (tp == "FB") typ = PLCBlockType.FB;

//                        var blk = new TIAProjectBlockInfo(subNode) { Name = nm, BlockType = typ };
//                        blockList.Add(blk);
//                    }
//                }
//            }

//            return blockList;
//        }

//        public List<ProjectBlockInfo> BlockInfos
//        {
//            get
//            {
//                return readPlcBlocksList();
//            }
//        }

//        public Block GetBlock(string BlockName)
//        {
//            readPlcBlocksList();

//            var info=blockList.FirstOrDefault(itm => itm.Name == BlockName);

//            return GetBlock(info);
//        }

//        public Block GetBlock(ProjectBlockInfo blkInfo)
//        {
//            if (blkInfo == null) return null;

//            switch (blkInfo.BlockType)
//            {
//                case PLCBlockType.DB:
//                    return new TIADataBlock(TiaProject, ((TIAProjectBlockInfo)blkInfo).Node) { BlockType = blkInfo.BlockType };
//                case PLCBlockType.FB:
//                case PLCBlockType.FC:
//                    return new TIAFunctionBlock(TiaProject, ((TIAProjectBlockInfo)blkInfo).Node) { BlockType = blkInfo.BlockType };
//            }
//            return null;
//        }
//    }
//}