using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class TIABlocksFolder : TIAProjectFolder, IBlocksFolder
    {
        public String Folder { get; set; }

        public TIABlocksFolder(Step7ProjectV11 Project, XmlNode Node)
            : base(Project, Node)
        {
         //"Siemens.Simatic.PlcLanguages.Model.StructureItemData"   
        }

        private List<ProjectBlockInfo> blockList = null; 
        public List<ProjectBlockInfo> readPlcBlocksList()
        {
            if (blockList == null)
            {
                blockList = new List<ProjectBlockInfo>();
                foreach (XmlNode subNode in SubNodes)
                {
                    string id = subNode.Attributes["id"].Value;
                    string tiaType = TiaProject.importTypeInfos[id];

                    if (tiaType == "Siemens.Simatic.PlcLanguages.Model.DataBlockData")
                    {
                        var nm = subNode.SelectSingleNode("attribSet[@id='" + TiaProject.CoreAttributesId + "']/attrib[@name='Name']").InnerText;

                        var blk = new TIAProjectBlockInfo(subNode) { Name = nm, BlockType = PLCBlockType.DB };
                        blockList.Add(blk);
                    }
                    else if (tiaType == "Siemens.Simatic.PlcLanguages.Model.CodeBlockData")
                    {
                        var nm = subNode.SelectSingleNode("attribSet[@id='" + TiaProject.CoreAttributesId + "']/attrib[@name='Name']").InnerText;
                        var tp = subNode.SelectSingleNode("attribSet[@id='" + TiaProject.CoreAttributesId + "']/attrib[@name='Subtype']").InnerText;

                        var typ = PLCBlockType.FC;
                        if (tp == "FB") typ = PLCBlockType.FB;

                        var blk = new TIAProjectBlockInfo(subNode) { Name = nm, BlockType = typ };
                        blockList.Add(blk);
                    }
                }
            }

            return blockList;
        }

        public List<ProjectBlockInfo> BlockInfos
        {
            get
            {
                return readPlcBlocksList();
            }
        }

        public Block GetBlock(string BlockName)
        {
            readPlcBlocksList();

            var info=blockList.FirstOrDefault(itm => itm.Name == BlockName);
            
            return GetBlock(info);
        }

        public Block GetBlock(ProjectBlockInfo blkInfo)
        {
            if (blkInfo == null) return null;

            switch (blkInfo.BlockType)
            {
                case PLCBlockType.DB:
                    return new TIADataBlock(TiaProject, ((TIAProjectBlockInfo)blkInfo).Node) { BlockType = blkInfo.BlockType };
                case PLCBlockType.FB:
                case PLCBlockType.FC:
                    return new TIAFunctionBlock(TiaProject, ((TIAProjectBlockInfo)blkInfo).Node) { BlockType = blkInfo.BlockType };
            }
            return null;
        }
    }
}
