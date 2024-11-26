using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DBF;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class BlocksOfflineFolder : Step7ProjectFolder, IBlocksFolder
    {

        public string Folder { get; set; }

        private DataTable _bausteinDbf;
        private DataTable bausteinDBF
        {
            get
            {
                if (_bausteinDbf == null)
                    if (((Step7ProjectV5)Project)._ziphelper.FileExists(Folder + "BAUSTEIN.DBF"))
                        _bausteinDbf = DBF.ParseDBF.ReadDBF(Folder + "BAUSTEIN.DBF", ((Step7ProjectV5)Project)._ziphelper,
                                                            ((Step7ProjectV5)Project)._DirSeperator);
                return _bausteinDbf;
            }
        }

        private DataTable _subblkDbf;
        private DataTable subblkDBF
        {
            get
            {
                if (_subblkDbf == null)
                    if (((Step7ProjectV5)Project)._ziphelper.FileExists(Folder + "SUBBLK.DBF"))
                        _subblkDbf = DBF.ParseDBF.ReadDBF(Folder + "SUBBLK.DBF", ((Step7ProjectV5)Project)._ziphelper,
                                                          ((Step7ProjectV5)Project)._DirSeperator);
                return _subblkDbf;
            }
        }


        private List<ProjectBlockInfo> _intBlockList;
        private List<ProjectBlockInfo> intBlockList
        {
            get
            {
                if (_intBlockList == null)
                    _intBlockList = intReadPlcBlocksList();
                return _intBlockList;
            }
        }

        public List<ProjectBlockInfo> readPlcBlocksList()
        {
            return intBlockList;
        }

        public List<ProjectBlockInfo> BlockInfos
        {
            get { return readPlcBlocksList(); }
        }

        /// <summary>
        /// Read all blocks from the S7 Project and cache them into the 'tmpBlocks' field.
        /// </summary>
        /// <returns></returns>
        private List<ProjectBlockInfo> intReadPlcBlocksList()
        {
            bool showDeleted = ((Step7ProjectV5)this.Project)._showDeleted;

            List<ProjectBlockInfo> tmpBlocks = new List<ProjectBlockInfo>();

            if (bausteinDBF != null) 
            {
                var dbfTbl = bausteinDBF; 

                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || showDeleted)
                    {
                        int id = (int)row["ID"];

                        int blocknumber = Convert.ToInt32(row["NUMMER"]);
                        int blocktype = Convert.ToInt32(row["TYP"]);

                        S7ProjectBlockInfo tmp = new S7ProjectBlockInfo();
                        tmp.ParentFolder = this;
                        tmp.Deleted = (bool)row["DELETED_FLAG"];
                        tmp.BlockNumber = blocknumber;
                        tmp.id = id;
                        if (blocktype == 0x00)
                            tmp.BlockType = PLCBlockType.UDT;
                        else
                            tmp.BlockType = (PLCBlockType)blocktype;

                        if (tmp.BlockType == PLCBlockType.SFB || tmp.BlockType == PLCBlockType.SFC || tmp.BlockType == PLCBlockType.SDB || tmp.BlockType == PLCBlockType.DB || tmp.BlockType == PLCBlockType.VAT || tmp.BlockType == PLCBlockType.FB || tmp.BlockType == PLCBlockType.FC || tmp.BlockType == PLCBlockType.OB || tmp.BlockType == PLCBlockType.UDT)
                            tmpBlocks.Add(tmp);
                    }
                }
            }

            if (subblkDBF != null) 
            {
                var dbfTbl = subblkDBF; 

                foreach (S7ProjectBlockInfo step7ProjectBlockInfo in tmpBlocks)
                {
                    var rows = dbfTbl.Select("OBJECTID = " + step7ProjectBlockInfo.id.ToString());
                    foreach (DataRow row in rows)
                    {
                        int subblktyp = Convert.ToInt32(row["SUBBLKTYP"]);
                        if ((int)row["OBJECTID"] == step7ProjectBlockInfo.id && (subblktyp == 12 || subblktyp == 8 || subblktyp == 14))
                        {
                            if ((int)row["PASSWORD"] == 3)
                                step7ProjectBlockInfo.KnowHowProtection = true;
                        }
                        if ((int)row["OBJECTID"] == step7ProjectBlockInfo.id && (subblktyp == 13 || subblktyp == 12 || subblktyp == 8 || subblktyp == 14))
                        {
                            var nm = row["BLOCKNAME"] as string;
                            step7ProjectBlockInfo.Name = nm?.Replace("\0","");
                            var nm2 = row["BLOCKFNAME"] as string;
                            step7ProjectBlockInfo.Family = nm2?.Replace("\0", "");
                        }
                    }
                }
            }
            NumericComparer<ProjectBlockInfo> nc = new NumericComparer<ProjectBlockInfo>();
            tmpBlocks.Sort(nc);
            return tmpBlocks;
        }

        /// <summary>
        /// Help class, used to hold unparsed raw data read from the S7 Project files from disk
        /// </summary>
        public class TmpBlock
        {
            public byte[] mc7code;
            public byte[] uda;
            public byte[] subblocks;
            public byte[] comments;
            public byte[] addinfo;
            public string blkinterface;
            public byte[] blkinterfaceInMC5;
            public byte[] nwinfo;
            public byte[] blockdescription;
            public byte[] jumpmarks;
            public bool knowHowProtection = false;
            public string username;
            public string version;
            public DateTime LastCodeChange;
            public DateTime LastInterfaceChange;
            public DateTime LastInterfaceChangeHistory;
            public bool IsInstanceDB;
            public bool IsSFB;
            public int FBNumber;
            public int CheckSum;
            public PLCLanguage BlockLanguage;
        }

        private Dictionary<string, TmpBlock> tmpBlocks; //internal cached list of blocks already read from the S7 Project

        public ProjectBlockInfo GetProjectBlockInfoFromBlockName(string BlockName)
        {
            var tmp = readPlcBlocksList();
            foreach (ProjectPlcBlockInfo step7ProjectBlockInfo in tmp)
            {
                if (step7ProjectBlockInfo.BlockType.ToString() + step7ProjectBlockInfo.BlockNumber.ToString() == BlockName.ToUpper())
                    return step7ProjectBlockInfo;
            }
            return null;
        }

        public void ChangeKnowHowProtection(S7ProjectBlockInfo blkInfo, bool KnowHowProtection)
        {
            TmpBlock myTmpBlk = new TmpBlock();

            if (subblkDBF != null)
            {
                var dbfTbl = subblkDBF; 
                foreach (DataRow row in dbfTbl.Rows)
                {

                    int subblktype = Convert.ToInt32(row["SUBBLKTYP"]);
                    int objid = (int)row["OBJECTID"];

                    if (objid == blkInfo.id && (subblktype == 12 || subblktype == 8 || subblktype == 14))
                    {
                        _bausteinDbf = null;
                        _subblkDbf = null;
                        _intBlockList = null;

                        ((Step7ProjectV5) Project).hasChanges = true;

                        if (KnowHowProtection)
                            DBF.ParseDBF.WriteValue(Folder + "SUBBLK.DBF", "PASSWORD", dbfTbl.Rows.IndexOf(row), 3, ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                        else
                            DBF.ParseDBF.WriteValue(Folder + "SUBBLK.DBF", "PASSWORD", dbfTbl.Rows.IndexOf(row), 0, ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                        break;
                    }
                }
            }
        }

        public void UndeleteBlock(S7ProjectBlockInfo blkInfo, int newBlockNumber)
        {
            TmpBlock myTmpBlk = new TmpBlock();

            if (((Step7ProjectV5)Project)._ziphelper.FileExists(Folder + "BAUSTEIN.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(Folder + "BAUSTEIN.DBF", ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    int objid = (int)row["ID"];

                    if (objid == blkInfo.id)
                    {
                        ((Step7ProjectV5)Project).hasChanges = true;
                        DBF.ParseDBF.WriteValue(Folder + "BAUSTEIN.DBF", "DELETED_FLAG", dbfTbl.Rows.IndexOf(row), false, ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                        DBF.ParseDBF.WriteValue(Folder + "BAUSTEIN.DBF", "NUMMER", dbfTbl.Rows.IndexOf(row), newBlockNumber, ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                    }
                }
            }

            if (((Step7ProjectV5)Project)._ziphelper.FileExists(Folder + "SUBBLK.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(Folder + "SUBBLK.DBF", ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    int objid = (int)row["OBJECTID"];

                    if (objid == blkInfo.id)
                    {
                        ((Step7ProjectV5)Project).hasChanges = true;
                        DBF.ParseDBF.WriteValue(Folder + "SUBBLK.DBF", "DELETED_FLAG", dbfTbl.Rows.IndexOf(row), false, ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                        DBF.ParseDBF.WriteValue(Folder + "SUBBLK.DBF", "BLKNUMBER", dbfTbl.Rows.IndexOf(row), newBlockNumber, ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                    }
                }
            }
        }

        /// <summary>
        /// Reads the raw data from the S7 Project files, without parsing the data
        /// </summary>
        /// <param name="blkInfo">The Block info object that identifies the block to read from Disk</param>
        /// <returns></returns>
        public TmpBlock GetBlockBytes(ProjectBlockInfo blkInfo)
        {
            if (subblkDBF != null) //ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "SUBBLK.DBF"))
            {
                TmpBlock myTmpBlk = new TmpBlock();

                var bstTbl = bausteinDBF;
                DataRow[] bstRows = bstTbl.Select("ID = " + blkInfo.id);
                if (bstRows != null && bstRows.Length > 0 && !(bstRows[0]["UDA"] is DBNull))
                    myTmpBlk.uda = (byte[]) bstRows[0]["UDA"];

                var dbfTbl = subblkDBF; 

                DataRow[] rows = dbfTbl.Select("OBJECTID = " + blkInfo.id);
                
                foreach (DataRow row in rows)
                {

                    int subblktype = Convert.ToInt32(row["SUBBLKTYP"]);
                    int objid = (int)row["OBJECTID"];

                    if (objid == blkInfo.id)
                    {
                        byte[] mc5code = null;
                        byte[] ssbpart = null;
                        byte[] addinfo = null;

                        if (row["MC5CODE"] != DBNull.Value)
                            mc5code = (byte[])row["MC5CODE"];
                        if (row["SSBPART"] != DBNull.Value)
                            ssbpart = (byte[])row["SSBPART"];
                        if (row["ADDINFO"] != DBNull.Value)
                            addinfo = (byte[])row["ADDINFO"];
                        int mc5codelen = (int)row["MC5LEN"];
                        int ssbpartlen = (int)row["SSBLEN"];
                        int addinfolen = (int)row["ADDLEN"];

                        if (mc5code != null && mc5code.Length > mc5codelen)
                            Array.Resize<byte>(ref mc5code, mc5codelen);
                        if (ssbpart != null && ssbpart.Length > ssbpartlen)
                            Array.Resize<byte>(ref ssbpart, ssbpartlen);
                        if (addinfo != null && addinfo.Length > addinfolen)
                            Array.Resize<byte>(ref addinfo, addinfolen);
			    
                        var tmpCheckSum = (int)row["CHECKSUM"];
                        if (myTmpBlk.CheckSum == 0 && tmpCheckSum != 0)
                        {
                            if (BitConverter.IsLittleEndian)
                            {
                                var tmp = BitConverter.GetBytes(tmpCheckSum);
                                Array.Reverse(tmp);
                                var tmp2 = BitConverter.ToUInt16(tmp, 2);
                                myTmpBlk.CheckSum = Convert.ToInt32(tmp2);
                            }
                            else
                                myTmpBlk.CheckSum = tmpCheckSum;
                        }

                        if (subblktype == 12 || subblktype == 8 || subblktype == 14 || subblktype == 13 || subblktype == 15) //FC, OB, FB, SFC, SFB
                        {
                            if (row["PASSWORD"] != DBNull.Value && (int)row["PASSWORD"] == 3)
                                myTmpBlk.knowHowProtection = true;
                            //MC7 Code in mc5code
                            myTmpBlk.mc7code = mc5code;
                            myTmpBlk.username = ((string)row["USERNAME"]).Replace("\0", "").Trim();

                            int ver = ((int)row["VERSION"]);
			    // Calculate Block version
			    // 0 .. 255
			    // 0.0 .. 15.15
                            myTmpBlk.version = (ver / 16).ToString() + "." + (ver % 16).ToString();

                            //Network Information in addinfo
                            myTmpBlk.nwinfo = addinfo;
                            //This line contains Network Information, and after it the Position of the JumpMarks

                            // ssbpart contains the MC7 interface from the PLC
                            myTmpBlk.blkinterfaceInMC5 = ssbpart;
                            myTmpBlk.LastCodeChange = GetTimeStamp((string)row["TIMESTAMP1"]);
                            myTmpBlk.LastInterfaceChange = GetTimeStamp((string)row["TIMESTAMP2"]);

                            var lngS = row["BLKLANG"] as string;
                            if (lngS != null && int.TryParse(lngS, out var lng))
                            {
                                myTmpBlk.BlockLanguage = (PLCLanguage)lng;
                            }

                        }
                        else if (subblktype == 5 || subblktype == 3 || subblktype == 4 || subblktype == 7 || subblktype == 9) //FC, OB, FB, SFC, SFB
                        {
                            // Interface in plaintext
                            if (mc5code != null)
                            {
                                myTmpBlk.blkinterface = Project.ProjectEncoding.GetString(mc5code);
                                myTmpBlk.LastInterfaceChangeHistory = GetTimeStamp((string)row["TIMESTAMP2"]);
                            }
                        }
                        else if (subblktype == 19 || subblktype == 17 || subblktype == 18 || subblktype == 22 ||
                                 subblktype == 21) //FC, OB, FB, SFC, SFB
                        {
                            myTmpBlk.comments = mc5code; //Comments of the Block
                            myTmpBlk.blockdescription = ssbpart; //Description of the Block
                            myTmpBlk.jumpmarks = addinfo;
                            //The Text of the Jump Marks, Before the Jumpmarks there is some Network Information, but don't know what!
                        }
                        else if (subblktype == 6 || subblktype == 1) //DB, UDT
                        {
                            //DB Structure in Plain Text (Structure and StartValues!)
                            if (mc5code != null)
                            {
                                myTmpBlk.blkinterface = Project.ProjectEncoding.GetString(mc5code);
                                myTmpBlk.LastInterfaceChangeHistory = GetTimeStamp((string)row["TIMESTAMP2"]);
                            }

                            // DB blocks get their timestamps from the MC7 interface record (when subblktype == 10). UDT blocks can't be downloaded and don't have this same record.
                            if (blkInfo.BlockType == PLCBlockType.UDT)
                            {
                                myTmpBlk.LastCodeChange = GetTimeStamp((string)row["TIMESTAMP1"]);
                                myTmpBlk.LastInterfaceChange = GetTimeStamp((string)row["TIMESTAMP2"]);
                            }

                            //Maybe compiled DB Structure?
                            myTmpBlk.addinfo = addinfo;
                        }
                        else if (subblktype == 10) //DB
                        {
                            // mc7code contains the actual values of the DB from the PLC
                            myTmpBlk.mc7code = mc5code;
                            // ssbpart contains the MC7 interface from the PLC
                            myTmpBlk.blkinterfaceInMC5 = ssbpart;
                            myTmpBlk.LastCodeChange = GetTimeStamp((string)row["TIMESTAMP1"]);
                            myTmpBlk.LastInterfaceChange = GetTimeStamp((string)row["TIMESTAMP2"]);

                            if (ssbpart != null && ssbpartlen > 2 && (ssbpart[0] == 0x0a || ssbpart[0] == 0x0b))
                            {
                                // if ssbpart[0] == 5 this DB is normal
                                // if ssbpart[0] == 10 this DB is instance for FB, 
                                // if ssbpart[0] == 11 this DB is instance for SFB, 
                                myTmpBlk.IsInstanceDB = true;
                                if (ssbpart[0] == 11)
                                    myTmpBlk.IsSFB = true;
                                myTmpBlk.FBNumber = (int)ssbpart[1] + 256 * (int)ssbpart[2];
                            }
                        }
                        else if (subblktype == 0x14) //DB
                        {
                            //Need to check wich Information is stored here
                        }
                        else if (subblktype == 0x42) //DB
                        {
                            //Need to check wich Information is stored here
                        }
                        else if (subblktype == 27) //VAT
                        {
                            myTmpBlk.LastCodeChange = GetTimeStamp((string)row["TIMESTAMP1"]);
                            myTmpBlk.LastInterfaceChange = GetTimeStamp((string)row["TIMESTAMP2"]);

                            //VAT in MC5Code (Absolut adressed)
                            myTmpBlk.mc7code = mc5code;
                            //VAT in ADDINFO (Symbolic adressed)
                            myTmpBlk.nwinfo = addinfo;
                        }
                        else if (subblktype == 38) //VAT
                        {
                            //VAT Comments in MC5Code
                            myTmpBlk.comments = mc5code;
                        }
                    }
                }
                return myTmpBlk;
            }
            return null;
        }

         /// <summary>
        /// Reads the raw data from the S7 Project files, without parsing the data
        /// </summary>
        /// <param name="blkName">The blockname to be read from disk. eg. DB2, FB38....</param>
        /// <returns></returns>
        private TmpBlock GetBlockBytes(string blkName)
        {
            var blkInfo = GetProjectBlockInfoFromBlockName(blkName);
            if (blkInfo == null)
                return null;
           return GetBlockBytes(blkInfo);
        }

        public S7DataRow GetInterface(string blkName)
        {
            var myConvOpt = new S7ConvertingOptions(Project.ProjectLanguage);

            return GetInterface(blkName, myConvOpt);
        }

        public S7DataRow GetInterface(string blkName, S7ConvertingOptions myConvOpt)
        {
            var blkInfo = GetProjectBlockInfoFromBlockName(blkName);
            if (blkInfo == null)
                return null;
            TmpBlock myTmpBlk = GetBlockBytes(blkInfo);
            List<string> tmpPar = new List<string>();
            if (myConvOpt.CheckForInterfaceTimestampConflicts && S7Block.HasTimestampConflict(myTmpBlk.LastInterfaceChange, myTmpBlk.LastInterfaceChangeHistory))
            {
                return GetInterfaceStructureFromMC7(blkInfo, myTmpBlk, null, ref tmpPar);
            }
            else
            {
                return Parameter.GetInterfaceOrDBFromStep7ProjectString(myTmpBlk.blkinterface, ref tmpPar, blkInfo.BlockType, false, this, null, myConvOpt);
            }
        }

        /// <summary>
        /// Reads an Block from the Project and returns the block data that is stored in the S7 Project
        /// </summary>
        /// <param name="BlockName">The blockname to be read from disk. eg. DB2, FB38....</param>
        /// <returns></returns>
        public Block GetBlock(string BlockName)
        {
            var prjBlkInf = GetProjectBlockInfoFromBlockName(BlockName);
            if (prjBlkInf != null)
                return GetBlock(prjBlkInf);
            return null;
        }

        /// <summary>
        /// Reads an Block from the Project and returns the block data that is stored in the S7 Project
        /// </summary>
        /// <param name="BlockName">The blockname to be read from disk. eg. DB2, FB38....</param>
        /// <param name="myConvOpt">Defines options that determine how the Block will be converted</param>
        /// <returns></returns>
        public Block GetBlock(string BlockName, S7ConvertingOptions myConvOpt)
        {
            var prjBlkInf = GetProjectBlockInfoFromBlockName(BlockName);
            if (prjBlkInf != null)
                return GetBlock(prjBlkInf, myConvOpt);
            return null;
        }

        /// <summary>
        /// Reads an Block from the Project and returns the block data that is stored in the S7 Project
        /// </summary>
        /// <param name="blkInfo">The Block info object that identifies the block to read from Disk</param>
        /// <returns></returns>
        public Block GetBlock(ProjectBlockInfo blkInfo)
        {
            return GetBlock(blkInfo, new S7ConvertingOptions(Project.ProjectLanguage) { GenerateCallsfromUCs = false });
        }

        /// <summary>
        /// Reads an Block from the Project and returns the block data that is stored in the S7 Project
        /// </summary>
        /// <param name="blkInfo">The Block info object that identifies the block to read from Disk</param>
        /// <param name="myConvOpt">Defines options that determine how the Block will be converted</param>
        /// <returns></returns>
        public Block GetBlock(ProjectBlockInfo blkInfo, S7ConvertingOptions myConvOpt)
        {
            if (blkInfo._Block != null && ((blkInfo._Block) as S7Block).usedS7ConvertingOptions.Equals(myConvOpt)) 
                return blkInfo._Block;


            ProjectPlcBlockInfo plcblkifo = (ProjectPlcBlockInfo)blkInfo;
            TmpBlock myTmpBlk = GetBlockBytes(blkInfo);

            List<Step7Attribute> step7Attributes = null;

            if (myTmpBlk != null)
            {
                if (myTmpBlk.uda != null)
                {

                    int uPos = 2;
                    if (myTmpBlk.uda != null && myTmpBlk.uda.Length > 0 && myTmpBlk.uda[0] > 0)
                    {
                        step7Attributes = new List<Step7Attribute>();
                        for (int j = 0; j < myTmpBlk.uda[0]; j++)
                        {
                            string t1 = Project.ProjectEncoding.GetString(myTmpBlk.uda, uPos + 1, myTmpBlk.uda[uPos]);
                            uPos += myTmpBlk.uda[uPos] + 1;
                            string t2 = Project.ProjectEncoding.GetString(myTmpBlk.uda, uPos + 1, myTmpBlk.uda[uPos]);
                            uPos += myTmpBlk.uda[uPos] + 1;
                            step7Attributes.Add(new Step7Attribute(t1, t2));
                        }
                    }
                }

                //Begin with the Block Reading...
                if (blkInfo.BlockType == PLCBlockType.VAT)
                {
                    S7VATBlock retValBlock = new S7VATBlock(myTmpBlk.mc7code, myTmpBlk.comments, plcblkifo.BlockNumber, Project.ProjectEncoding);
                    retValBlock.Attributes = step7Attributes;

                    retValBlock.LastCodeChange = myTmpBlk.LastCodeChange;
                    retValBlock.LastInterfaceChange = myTmpBlk.LastInterfaceChange;

                    retValBlock.ParentFolder = this;
                    retValBlock.usedS7ConvertingOptions = myConvOpt;
                    blkInfo._Block = retValBlock;

                    retValBlock.BlockLanguage = myTmpBlk.BlockLanguage;

                    return retValBlock;
                }
                else if (blkInfo.BlockType == PLCBlockType.DB || blkInfo.BlockType == PLCBlockType.UDT)
                {
                    List<string> tmpList = new List<string>();
                    S7DataBlock retVal = new S7DataBlock();
                    retVal.IsInstanceDB = myTmpBlk.IsInstanceDB; 
                    retVal.FBNumber = myTmpBlk.FBNumber;

                    //if this is an interface DB, then rather take the Interface declaration from the instance FB,
                    //instead of the data sotred in the project. 
                    //The reason is that if you change the comment in an FB, the DB data is not actualized and my contain outdated
                    //Declarations. When you change the interface, Step7 tells you to "regenerate" the instance DB which only then would 
                    //Actualize the comments. Simple Commentary changes do not change the Datablocks row. 
                    if (retVal.IsInstanceDB && myConvOpt.UseFBDeclarationForInstanceDB)
                    {
                        //load the FB data from the Project
                        TmpBlock InstFB = GetBlockBytes((myTmpBlk.IsSFB ? "SFB" : "FB") + myTmpBlk.FBNumber);

                        //Resolve both interfaces
                        List<string> tmpPar = new List<string>();
                        if (InstFB != null)
                        {
                            // Set the InstanceDB's history time to the most recent change of the FB
                            myTmpBlk.LastInterfaceChangeHistory = InstFB.LastInterfaceChange;

                            S7DataRow InterfaceFB =
                                Parameter.GetInterfaceOrDBFromStep7ProjectString(InstFB.blkinterface, ref tmpPar,
                                    PLCBlockType.FB, false, this, null, myConvOpt);
                            S7DataRow InterfaceDB =
                                Parameter.GetInterfaceOrDBFromStep7ProjectString(myTmpBlk.blkinterface, ref tmpPar,
                                    PLCBlockType.DB, false, this, null, myConvOpt);

                            //Only use the FB interface Declaration if they are compatible
                            if (Parameter.IsInterfaceCompatible(InterfaceFB, InterfaceDB))
                                myTmpBlk.blkinterface = InstFB.blkinterface;
                        }
                    }

                    if (myTmpBlk.mc7code != null) 
                        retVal.CodeSize = myTmpBlk.mc7code.Length;

                    retVal.LastCodeChange = myTmpBlk.LastCodeChange;
                    retVal.LastInterfaceChange = myTmpBlk.LastInterfaceChange;
                    retVal.LastInterfaceChangeHistory = myTmpBlk.LastInterfaceChangeHistory;

                    retVal.StructureFromMC7 = GetInterfaceStructureFromMC7(blkInfo, myTmpBlk, retVal, ref tmpList);
                    retVal.StructureFromString = Parameter.GetInterfaceOrDBFromStep7ProjectString(myTmpBlk.blkinterface, ref tmpList, blkInfo.BlockType, false, this, retVal, myConvOpt, myTmpBlk.mc7code);

                    retVal.BlockNumber = plcblkifo.BlockNumber;
                    retVal.Name = plcblkifo.Name;
                    retVal.Family = ((S7ProjectBlockInfo)plcblkifo).Family;
                    retVal.BlockType = blkInfo.BlockType;
                    retVal.Attributes = step7Attributes;

                    retVal.ParentFolder = this;
                    retVal.usedS7ConvertingOptions = myConvOpt;
                    retVal.CheckSum = myTmpBlk.CheckSum;
                    blkInfo._Block = retVal;

                    return retVal;

                }
                else if (blkInfo.BlockType == PLCBlockType.FC || blkInfo.BlockType == PLCBlockType.FB || blkInfo.BlockType == PLCBlockType.OB || blkInfo.BlockType == PLCBlockType.SFB || blkInfo.BlockType == PLCBlockType.SFC)
                {
                    List<string> ParaList = new List<string>();

                    S7FunctionBlock retVal = new S7FunctionBlock();                   

                    retVal.LastCodeChange = myTmpBlk.LastCodeChange;
                    retVal.LastInterfaceChange = myTmpBlk.LastInterfaceChange;
                    retVal.LastInterfaceChangeHistory = myTmpBlk.LastInterfaceChangeHistory;

                    retVal.BlockNumber = plcblkifo.BlockNumber;
                    retVal.Name = plcblkifo.Name;
                    retVal.Family = ((S7ProjectBlockInfo)plcblkifo).Family;
                    retVal.BlockType = blkInfo.BlockType;
                    retVal.Attributes = step7Attributes;
                    retVal.KnowHowProtection = myTmpBlk.knowHowProtection;
                    retVal.MnemonicLanguage = Project.ProjectLanguage;

                    retVal.Author = myTmpBlk.username;
                    retVal.Version = myTmpBlk.version;

                    if (myConvOpt.CheckForInterfaceTimestampConflicts && S7Block.HasTimestampConflict(retVal.LastInterfaceChange, retVal.LastInterfaceChangeHistory))
                    {
                        retVal.Parameter = GetInterfaceStructureFromMC7(blkInfo, myTmpBlk, retVal, ref ParaList);
                    }
                    else
                    {
                        retVal.Parameter = Parameter.GetInterfaceOrDBFromStep7ProjectString(myTmpBlk.blkinterface, ref ParaList, blkInfo.BlockType, false, this, retVal, myConvOpt);
                    }
                
                    if (myTmpBlk.blockdescription != null && myTmpBlk.blockdescription.Length > 3)
                    {
                        retVal.Title = Project.ProjectEncoding.GetString(myTmpBlk.blockdescription, 3, myTmpBlk.blockdescription[1] - 4);
                        retVal.Description = Project.ProjectEncoding.GetString(myTmpBlk.blockdescription, myTmpBlk.blockdescription[1], myTmpBlk.blockdescription.Length - myTmpBlk.blockdescription[1] - 1).Replace("\n", Environment.NewLine);
                    }

                    if (blkInfo.BlockType == PLCBlockType.FC || blkInfo.BlockType == PLCBlockType.FB || blkInfo.BlockType == PLCBlockType.OB)
                    {
                        retVal.CodeSize = myTmpBlk.mc7code.Length;

                        int[] Networks;
                        Networks = NetWork.GetNetworks(0, myTmpBlk.nwinfo);

                        S7ProgrammFolder prgFld = null;
                        if (this.Parent is S7ProgrammFolder)
                            prgFld = (S7ProgrammFolder)this.Parent;

                        retVal.AWLCode = MC7toAWL.GetAWL(0, myTmpBlk.mc7code.Length - 2, (int)myConvOpt.Mnemonic, myTmpBlk.mc7code, Networks, ParaList, prgFld, retVal, retVal.Parameter);

                        retVal.AWLCode = JumpMarks.AddJumpmarks(retVal.AWLCode, myTmpBlk.jumpmarks, myTmpBlk.nwinfo, myConvOpt);

                        LocalDataConverter.ConvertLocaldataToSymbols(retVal, myConvOpt);
                        
                        CallConverter.ConvertUCToCall(retVal, prgFld, this, myConvOpt, null);

                        FBStaticAccessConverter.ReplaceStaticAccess(retVal, prgFld, myConvOpt);                        

                        for (int i = 0; i < retVal.AWLCode.Count - 1; i++)
                        {
                            S7FunctionBlockRow akRw = (S7FunctionBlockRow)retVal.AWLCode[i];

                            if (akRw.CombineDBAccess)
                            {
                                S7FunctionBlockRow nRw = (S7FunctionBlockRow)retVal.AWLCode[i + 1];
                                if (!nRw.Parameter.Contains("["))
                                {
                                    nRw.Parameter = akRw.Parameter + "." + nRw.Parameter;
                                    nRw.MC7 = Helper.CombineByteArray(akRw.MC7, nRw.MC7);
                                    nRw.Label = akRw.Label ?? nRw.Label;
                                    retVal.AWLCode.RemoveAt(i + 1);
                                    retVal.AWLCode[i] = nRw;
                                }
                            }
                        }
                        
                        #region UseComments from Block
                        if (myConvOpt.UseComments)
                        {
                            List<FunctionBlockRow> newAwlCode = new List<FunctionBlockRow>();

                            int n = 0;
                            int akRowInAwlCode = 0;
                            int lineNumberInCall = 0; //Counter wich line in Command (for Calls and UCs)

                            if (myTmpBlk.comments != null)
                            {
                                byte[] cmt = myTmpBlk.comments;

                                //string aa = System.Text.Encoding.GetEncoding("Windows-1251").GetString(cmt);
                                //string testaa = "";

                                while (n < myTmpBlk.comments.Length)
                                {
                                    int kommLen = cmt[n + 0];
                                    int startNWKomm = cmt[n + 1];
                                    int anzUebsprungZeilen = cmt[n + 2] + cmt[n + 3] * 0x100;
                                    int lenNWKommZeile = cmt[n + 3] + cmt[n + 4] * 0x100;
                                    //Console.WriteLine(cmt[n + 5].ToString("X"));
                                    if (cmt[n + 5] == 0x06)
                                    {
                                        //NWKomentar:
                                        string tx1 = Project.ProjectEncoding.GetString(cmt, n + 6, startNWKomm - 7);
                                        string tx2 = Project.ProjectEncoding.GetString(cmt, n + startNWKomm, lenNWKommZeile - startNWKomm - 1).Replace("\n", Environment.NewLine);
                                        n += lenNWKommZeile;

                                        while (retVal.AWLCode.Count > akRowInAwlCode && retVal.AWLCode[akRowInAwlCode].Command != "NETWORK")
                                        {
                                            newAwlCode.Add(retVal.AWLCode[akRowInAwlCode]);
                                            akRowInAwlCode++;
                                        }

                                        if (retVal.AWLCode.Count > akRowInAwlCode)
                                        {
                                            ((S7FunctionBlockRow)retVal.AWLCode[akRowInAwlCode]).NetworkName = tx1;
                                            ((S7FunctionBlockRow)retVal.AWLCode[akRowInAwlCode]).Comment = tx2;
                                            newAwlCode.Add(retVal.AWLCode[akRowInAwlCode]);
                                            akRowInAwlCode++;
                                        }

                                        lineNumberInCall = 0;
                                    }
                                    else
                                    {
                                        S7FunctionBlockRow lastRow = null;

                                        //Anzahl der Anweisungen vor diesem Kommentar (inklusive aktueller Zeile!)
                                        for (int q = 0; q < (anzUebsprungZeilen); q++)
                                        {
                                            if (retVal.AWLCode.Count > akRowInAwlCode)
                                            {
                                                S7FunctionBlockRow akRw = (S7FunctionBlockRow) retVal.AWLCode[akRowInAwlCode];

                                                 if (cmt[n + 4] == 0xc0 && q == anzUebsprungZeilen-1)
                                                     akRw.CombineDBAccess = false;

                                                if (akRw.GetNumberOfLines() == 1)
                                                {
                                                    lineNumberInCall = 0;

                                                    lastRow = akRw;

                                                    //if (!newAwlCode.Contains(akRw))
                                                    //    newAwlCode.Add(akRw);

                                                    newAwlCode.Add(akRw);
                                                    akRowInAwlCode++;
                                                }
                                                else
                                                {
                                                    lastRow = akRw;
                                                    if (lineNumberInCall == 0 && !(cmt[n + 4] != 0x80 && cmt[n + 4] != 0xc0))
                                                    {
                                                        //if (!newAwlCode.Contains(akRw))
                                                        //    newAwlCode.Add(akRw);                                                        
                                                    }

                                                    if (akRw.GetNumberOfLines() - 1 == lineNumberInCall)
                                                    {
                                                        newAwlCode.Add(akRw);
                                                        akRowInAwlCode++;
                                                        lineNumberInCall = 0;
                                                        //subCnt++;    //The set to zero was wrong here, but maybe now comments on calls do not work, need to check!
                                                    }
                                                    else
                                                    {
                                                        lineNumberInCall++;
                                                    }
                                                }
                                            }
                                        }


                                        //if (lastRow == null || cmt[n + 4] != 0x80)
                                        if (lastRow == null || (cmt[n + 4] != 0x80 && cmt[n + 4] != 0xc0))
                                        {
                                            lastRow = new S7FunctionBlockRow(){ Parent = retVal };
                                            newAwlCode.Add(lastRow);
                                            lineNumberInCall = 0;
                                        }

                                        string tx1 = Project.ProjectEncoding.GetString(cmt, n + 6, kommLen);
                                        if (lineNumberInCall == 0)
                                            lastRow.Comment = tx1;
                                        else
                                            if (lastRow.Command == "CALL")
                                                if (lineNumberInCall == 1) lastRow.Comment = tx1;
                                                else
                                                {
                                                    if (lastRow.CallParameter.Count >= lineNumberInCall - 2)
                                                    {
                                                        lastRow.CallParameter[lineNumberInCall - 2].Comment = tx1;
                                                    }
                                                }
                                        n += kommLen + 6;

                                        //subCnt = 0;
                                    }
                                }
                            }
                            while (akRowInAwlCode < retVal.AWLCode.Count)
                            {
                                newAwlCode.Add(retVal.AWLCode[akRowInAwlCode]);
                                akRowInAwlCode++;
                            }
                            retVal.AWLCode = newAwlCode;
                        }
                        #endregion
                    }

                    retVal.Networks = NetWork.GetNetworksList(retVal);

                    retVal.ParentFolder = this;
                    retVal.usedS7ConvertingOptions = myConvOpt;
                    retVal.CheckSum = myTmpBlk.CheckSum;
                    blkInfo._Block = retVal;

                    retVal.BlockLanguage = myTmpBlk.BlockLanguage;

                    return retVal;
                }
            }
            return null;
        }

        /// <summary>
        /// Converts the MC7 code of the block interface to S7DataRow
        /// </summary>
        public S7DataRow GetInterfaceStructureFromMC7(ProjectBlockInfo blkInfo, TmpBlock myTmpBlk, S7Block block, ref List<string> paramList)
        {
            if (myTmpBlk.blkinterfaceInMC5 == null || myTmpBlk.blkinterfaceInMC5.Length == 0)
            {
                return null;
            }

            try
            {
                // Not sure what bytes 0-2 in the header are for
                // Bytes 3-4 are the interface length (not including header)
                var interfaceLen = BitConverter.ToUInt16(myTmpBlk.blkinterfaceInMC5, 3);
                var headerPlusInterfaceLen = 7 + interfaceLen;
                var interfaceBytes = new byte[headerPlusInterfaceLen];
                Array.Copy(myTmpBlk.blkinterfaceInMC5, 0, interfaceBytes, 0, headerPlusInterfaceLen);

                // Bytes 5-6 are the start values length
                var startValuesLen = BitConverter.ToUInt16(myTmpBlk.blkinterfaceInMC5, 5);
                byte[] startValuesBytes = null;
                if (startValuesLen > 0)
                {
                    startValuesBytes = new byte[startValuesLen];
                    Array.Copy(myTmpBlk.blkinterfaceInMC5, headerPlusInterfaceLen, startValuesBytes, 0, startValuesLen);
                }

                // Only DB blocks have actual values
                var actualValuesBytes = blkInfo.BlockType == PLCBlockType.DB ? myTmpBlk.mc7code : null;

                return Parameter.GetInterface(interfaceBytes, startValuesBytes, actualValuesBytes, ref paramList, blkInfo.BlockType, myTmpBlk.IsInstanceDB, block);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// With this Function you get the AWL Source of a Block, so that it can be imported into Step7
        /// </summary>
        /// <param name="blkInfo">The BlockInfo from the Block you wish to get the Source of!</param>
        /// <returns></returns>
        public string GetSourceBlock(ProjectBlockInfo blkInfo, bool useSymbols = false)
        {
            StringBuilder retVal = new StringBuilder();
            Block blk = GetBlock(blkInfo, new S7ConvertingOptions(Project.ProjectLanguage) { CombineDBOpenAndDBAccess = true, GenerateCallsfromUCs = true, ReplaceDBAccessesWithSymbolNames = useSymbols, ReplaceLokalDataAddressesWithSymbolNames = true, UseComments = true });
            S7Block fblk = (S7Block)blk;

            return fblk.GetSourceBlock(useSymbols);
        }

        private static DateTime GetTimeStamp(string timestamp)
        {
            try
            {
                //use Windows-1252 to get correct time because dBaseConverter uses this code page for strings
                var bytes = Util.DefaultEncoding.GetBytes(timestamp);  
                return bytes.Length == 5
                    ? Helper.GetDT((byte)bytes[0], (byte)bytes[1], (byte)bytes[2], (byte)bytes[3], (byte)bytes[4], (byte)0x20)
                    : Helper.GetDT((byte)bytes[0], (byte)bytes[1], (byte)bytes[2], (byte)bytes[3], (byte)bytes[4], (byte)bytes[5]);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}

