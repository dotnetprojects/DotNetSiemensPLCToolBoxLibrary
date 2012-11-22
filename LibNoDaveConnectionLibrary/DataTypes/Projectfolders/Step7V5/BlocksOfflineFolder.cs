	using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
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

        private List<ProjectBlockInfo> intReadPlcBlocksList()
        {
            bool showDeleted = ((Step7ProjectV5)this.Project)._showDeleted;

            List<ProjectBlockInfo> tmpBlocks = new List<ProjectBlockInfo>();
            if (bausteinDBF != null) //ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "BAUSTEIN.DBF"))
            {
                var dbfTbl = bausteinDBF; //DBF.ParseDBF.ReadDBF(Folder + "BAUSTEIN.DBF", ((Step7ProjectV5)Project)._zipfile, ((Step7ProjectV5)Project)._DirSeperator);

                //if (Parent is S7ProgrammFolder && ((S7ProgrammFolder)Parent).SymbolTable != null)
                //{
                //    SymbolTable symtab = (SymbolTable)((S7ProgrammFolder)Parent).SymbolTable;
                //}
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

                        /*
                        if (symtab != null)
                        {
                            SymbolTableEntry sym = symtab.GetEntryFromOperand(tmp.ToString());
                            if (sym != null)
                                tmp.Symbol = sym.Symbol;
                        }
                        */

                        if (tmp.BlockType == PLCBlockType.SFB || tmp.BlockType == PLCBlockType.SFC || tmp.BlockType == PLCBlockType.DB || tmp.BlockType == PLCBlockType.VAT || tmp.BlockType == PLCBlockType.FB || tmp.BlockType == PLCBlockType.FC || tmp.BlockType == PLCBlockType.OB || tmp.BlockType == PLCBlockType.UDT)
                            tmpBlocks.Add(tmp);
                    }
                }
            }

            if (subblkDBF != null) //ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "SUBBLK.DBF"))
            {
                var dbfTbl = subblkDBF; // DBF.ParseDBF.ReadDBF(Folder + "SUBBLK.DBF", ((Step7ProjectV5)Project)._zipfile, ((Step7ProjectV5)Project)._DirSeperator);

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
                    }
                }
            }
            NumericComparer<ProjectBlockInfo> nc = new NumericComparer<ProjectBlockInfo>();
            tmpBlocks.Sort(nc);
            return tmpBlocks;
        }

        private class tmpBlock
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
            public bool IsInstanceDB;
            public int FBNumber;
        }

        private Dictionary<string, tmpBlock> tmpBlocks;

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

        public Block GetBlock(string BlockName)
        {
            var prjBlkInf = GetProjectBlockInfoFromBlockName(BlockName);
            if (prjBlkInf != null)
                return GetBlock(prjBlkInf);
            return null;
        }

        public Block GetBlock(string BlockName, S7ConvertingOptions myConvOpt)
        {
            var prjBlkInf = GetProjectBlockInfoFromBlockName(BlockName);
            if (prjBlkInf != null)
                return GetBlock(prjBlkInf, myConvOpt);
            return null;
        }

        public void ChangeKnowHowProtection(S7ProjectBlockInfo blkInfo, bool KnowHowProtection)
        {
            tmpBlock myTmpBlk = new tmpBlock();

            if (subblkDBF != null)// ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "SUBBLK.DBF"))
            {
                var dbfTbl = subblkDBF; // DBF.ParseDBF.ReadDBF(Folder + "SUBBLK.DBF", ((Step7ProjectV5)Project)._zipfile, ((Step7ProjectV5)Project)._DirSeperator);
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
            tmpBlock myTmpBlk = new tmpBlock();

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

        private tmpBlock GetBlockBytes(ProjectBlockInfo blkInfo)
        {
            if (subblkDBF != null) //ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "SUBBLK.DBF"))
            {
                tmpBlock myTmpBlk = new tmpBlock();

                var bstTbl = bausteinDBF;
                DataRow[] bstRows = bstTbl.Select("ID = " + blkInfo.id);
                if (bstRows != null && bstRows.Length > 0 && !(bstRows[0]["UDA"] is DBNull))
                    myTmpBlk.uda = (byte[]) bstRows[0]["UDA"];

                var dbfTbl = subblkDBF; // DBF.ParseDBF.ReadDBF(Folder + "SUBBLK.DBF", ((Step7ProjectV5)Project)._zipfile, ((Step7ProjectV5) Project)._DirSeperator);

                DataRow[] rows = dbfTbl.Select("OBJECTID = " + blkInfo.id);
                //bausteinDBF

                foreach (DataRow row in rows)// dbfTbl.Rows)
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

                        if (subblktype == 12 || subblktype == 8 || subblktype == 14 || subblktype == 13 || subblktype == 15) //FC, OB, FB, SFC, SFB
                        {
                            if (row["PASSWORD"] != DBNull.Value && (int)row["PASSWORD"] == 3)
                                myTmpBlk.knowHowProtection = true;
                            //MC7 Code in mc5code
                            myTmpBlk.mc7code = mc5code;
                            myTmpBlk.username = ((string)row["USERNAME"]).Replace("\0", "").Trim();

                            int ver = ((int)row["VERSION"]);
                            myTmpBlk.version = (ver / 15).ToString() + "." + (ver % 15).ToString();

                            //Network Information in addinfo
                            myTmpBlk.nwinfo = addinfo;
                            //This line contains Network Information, and after it the Position of the JumpMarks

                            var ts1 = (string)row["TIMESTAMP1"];
                            if (ts1.Length == 5)
                            {
                                myTmpBlk.LastCodeChange = Helper.GetDT((byte)ts1[0], (byte)ts1[1], (byte)ts1[2], (byte)ts1[3], (byte)ts1[4], (byte)0);
                            }
                            else
                            {
                                myTmpBlk.LastCodeChange = Helper.GetDT((byte)ts1[0], (byte)ts1[1], (byte)ts1[2], (byte)ts1[3], (byte)ts1[4], (byte)ts1[5]);
                            }

                            ts1 = (string)row["TIMESTAMP2"];
                            if (ts1.Length == 5)
                            {
                                myTmpBlk.LastInterfaceChange = Helper.GetDT((byte)ts1[0], (byte)ts1[1], (byte)ts1[2], (byte)ts1[3], (byte)ts1[4], (byte)0);
                            }
                            else
                            {
                                myTmpBlk.LastInterfaceChange = Helper.GetDT((byte)ts1[0], (byte)ts1[1], (byte)ts1[2], (byte)ts1[3], (byte)ts1[4], (byte)ts1[5]);
                            }

                        }
                        else if (subblktype == 5 || subblktype == 3 || subblktype == 4 || subblktype == 7 || subblktype == 9) //FC, OB, FB, SFC, SFB
                        {
                            //Interface in mc5code
                            if (mc5code != null)
                                myTmpBlk.blkinterface =
                                    Project.ProjectEncoding.GetString(mc5code);
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
                                myTmpBlk.blkinterface =
                                    Project.ProjectEncoding.GetString(mc5code);
                            //Maybe compiled DB Structure?
                            myTmpBlk.addinfo = addinfo;

                            var ts1 = (string)row["TIMESTAMP1"];
                            if (ts1.Length == 5)
                            {
                                myTmpBlk.LastCodeChange = Helper.GetDT((byte)ts1[0], (byte)ts1[1], (byte)ts1[2], (byte)ts1[3], (byte)ts1[4], (byte)0);
                            }
                            else
                            {
                                myTmpBlk.LastCodeChange = Helper.GetDT((byte)ts1[0], (byte)ts1[1], (byte)ts1[2], (byte)ts1[3], (byte)ts1[4], (byte)ts1[5]);
                            }

                            ts1 = (string)row["TIMESTAMP2"];
                            if (ts1.Length == 5)
                            {
                                myTmpBlk.LastInterfaceChange = Helper.GetDT((byte)ts1[0], (byte)ts1[1], (byte)ts1[2], (byte)ts1[3], (byte)ts1[4], (byte)0);
                            }
                            else
                            {
                                myTmpBlk.LastInterfaceChange = Helper.GetDT((byte)ts1[0], (byte)ts1[1], (byte)ts1[2], (byte)ts1[3], (byte)ts1[4], (byte)ts1[5]);
                            }
                        }
                        else if (subblktype == 10) //DB
                        {
                            //Need to check wich Information is stored here
                            myTmpBlk.mc7code = mc5code;
                            myTmpBlk.blkinterfaceInMC5 = ssbpart;

                            if (ssbpart != null && ssbpartlen > 2 && (ssbpart[0] == 0x0a || ssbpart[0] == 0x0b))
                                // if ssbpart[0] == 5 this DB is normal
                                // if ssbpart[0] == 10 this DB is instance for FB, 
                                // I do not know what value for SFB
                            myTmpBlk.IsInstanceDB = true;
                            myTmpBlk.FBNumber = (int)ssbpart[1] + 256 * (int)ssbpart[2];
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
                            var ts1 = (string)row["TIMESTAMP1"];
                            myTmpBlk.LastCodeChange = Helper.GetDT((byte)ts1[0], (byte)ts1[1], (byte)ts1[2], (byte)ts1[3], (byte)ts1[4], (byte)ts1[5]);

                            ts1 = (string)row["TIMESTAMP2"];
                            myTmpBlk.LastInterfaceChange = Helper.GetDT((byte)ts1[0], (byte)ts1[1], (byte)ts1[2], (byte)ts1[3], (byte)ts1[4], (byte)ts1[5]);


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

        public S7DataRow GetInterface(string blkName)
        {
            var blkInfo = GetProjectBlockInfoFromBlockName(blkName);
            if (blkInfo == null)
                return null;
            tmpBlock myTmpBlk = GetBlockBytes(blkInfo);
            List<string> tmpPar = new List<string>();
            return Parameter.GetInterfaceOrDBFromStep7ProjectString(myTmpBlk.blkinterface, ref tmpPar, blkInfo.BlockType, false, this, null);
        }

        public Block GetBlock(ProjectBlockInfo blkInfo)
        {
            return GetBlock(blkInfo, new S7ConvertingOptions(){GenerateCallsfromUCs = false});
        }

        public Block GetBlock(ProjectBlockInfo blkInfo, S7ConvertingOptions myConvOpt)
        {
            //Todo: Enable this, but then myConvOpt is only used the first time!
            //if (blkInfo._Block != null)
            //    return blkInfo._Block;


            ProjectPlcBlockInfo plcblkifo = (ProjectPlcBlockInfo)blkInfo;
            tmpBlock myTmpBlk = GetBlockBytes(blkInfo);

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

                    return retValBlock;
                }
                else if (blkInfo.BlockType == PLCBlockType.DB || blkInfo.BlockType == PLCBlockType.UDT)
                {
                    List<string> tmpList = new List<string>();
                    S7DataBlock retVal = new S7DataBlock();
                    retVal.IsInstanceDB = myTmpBlk.IsInstanceDB; 
                    retVal.FBNumber = myTmpBlk.FBNumber;
                    retVal.StructureFromString = Parameter.GetInterfaceOrDBFromStep7ProjectString(myTmpBlk.blkinterface, ref tmpList, blkInfo.BlockType, false, this, retVal, myTmpBlk.mc7code);
                    if (myTmpBlk.blkinterfaceInMC5 != null)
                    {
                        //List<string> tmp = new List<string>();
                        //retVal.StructureFromMC7 = Parameter.GetInterface(myTmpBlk.blkinterfaceInMC5, myTmpBlk.mc7code, ref tmp, blkInfo.BlockType, myTmpBlk.IsInstanceDB, retVal);
                    }                        
                    retVal.BlockNumber = plcblkifo.BlockNumber;
                    retVal.BlockType = blkInfo.BlockType;
                    retVal.Attributes = step7Attributes;

                    retVal.LastCodeChange = myTmpBlk.LastCodeChange;
                    retVal.LastInterfaceChange = myTmpBlk.LastInterfaceChange;
                    return retVal;
                }
                else if (blkInfo.BlockType == PLCBlockType.FC || blkInfo.BlockType == PLCBlockType.FB || blkInfo.BlockType == PLCBlockType.OB || blkInfo.BlockType == PLCBlockType.SFB || blkInfo.BlockType == PLCBlockType.SFC)
                {
                    List<string> ParaList = new List<string>();

                    S7FunctionBlock retVal = new S7FunctionBlock();
                    blkInfo._Block = retVal;

                    retVal.LastCodeChange = myTmpBlk.LastCodeChange;
                    retVal.LastInterfaceChange = myTmpBlk.LastInterfaceChange;

                    retVal.BlockNumber = plcblkifo.BlockNumber;
                    retVal.BlockType = blkInfo.BlockType;
                    retVal.Attributes = step7Attributes;
                    retVal.KnowHowProtection = myTmpBlk.knowHowProtection;
                    retVal.ProjectLanguage = Project.ProjectLanguage;

                    retVal.Author = myTmpBlk.username;
                    retVal.Version = myTmpBlk.version;

                    retVal.Parameter = Parameter.GetInterfaceOrDBFromStep7ProjectString(myTmpBlk.blkinterface, ref ParaList, blkInfo.BlockType, false, this, retVal);

                    if (myTmpBlk.blockdescription != null)
                    {
                        retVal.Title = Project.ProjectEncoding.GetString(myTmpBlk.blockdescription, 3, myTmpBlk.blockdescription[1] - 4);
                        retVal.Description = Project.ProjectEncoding.GetString(myTmpBlk.blockdescription, myTmpBlk.blockdescription[1], myTmpBlk.blockdescription.Length - myTmpBlk.blockdescription[1] - 1).Replace("\n", Environment.NewLine);
                    }

                    if (blkInfo.BlockType == PLCBlockType.FC || blkInfo.BlockType == PLCBlockType.FB || blkInfo.BlockType == PLCBlockType.OB)
                    {
                        int[] Networks;
                        Networks = NetWork.GetNetworks(0, myTmpBlk.nwinfo);

                        S7ProgrammFolder prgFld = null;
                        if (this.Parent is S7ProgrammFolder)
                            prgFld = (S7ProgrammFolder)this.Parent;

                        retVal.AWLCode = MC7toAWL.GetAWL(0, myTmpBlk.mc7code.Length - 2, (int)Project.ProjectLanguage, myTmpBlk.mc7code, Networks, ParaList, prgFld);

                        retVal.AWLCode = JumpMarks.AddJumpmarks(retVal.AWLCode, myTmpBlk.jumpmarks, myTmpBlk.nwinfo);

                        LocalDataConverter.ConvertLocaldataToSymbols(retVal, myConvOpt);

                        FBStaticAccessConverter.ReplaceStaticAccess(retVal, prgFld, myConvOpt);

                        CallConverter.ConvertUCToCall(retVal, prgFld, this, myConvOpt, null);

                        #region UseComments from Block
                        if (myConvOpt.UseComments)
                        {
                            List<FunctionBlockRow> newAwlCode = new List<FunctionBlockRow>();

                            int n = 0;
                            int j = 0;
                            int subCnt = 0; //Counter wich line in Command (for Calls and UCs)

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

                                        if (retVal.AWLCode.Count > j)
                                        {
                                            while (retVal.AWLCode.Count - 1 > j && retVal.AWLCode[j].Command != "NETWORK")
                                            {
                                                newAwlCode.Add(retVal.AWLCode[j]);
                                                j++;
                                            }
                                            ((S7FunctionBlockRow)retVal.AWLCode[j]).NetworkName = tx1;
                                            ((S7FunctionBlockRow)retVal.AWLCode[j]).Comment = tx2;
                                            newAwlCode.Add(retVal.AWLCode[j]);
                                        }
                                        j++;

                                        subCnt = 0;
                                    }
                                    else
                                    {
                                        S7FunctionBlockRow lastRow = null;

                                        //Anzahl der Anweisungen vor diesem Kommentar (inklusive aktueller Zeile!)
                                        for (int q = 0; q < (anzUebsprungZeilen); q++)
                                        {
                                            if (retVal.AWLCode.Count > j)
                                            {
                                                S7FunctionBlockRow akRw = (S7FunctionBlockRow) retVal.AWLCode[j];

                                                 if (cmt[n + 4] == 0xc0 && q==anzUebsprungZeilen-1)
                                                     akRw.CombineDBAccess = false;

                                                //Db Zugriff zusammenfügen...
                                                if (akRw.CombineDBAccess)
                                                {
                                                    S7FunctionBlockRow nRw = (S7FunctionBlockRow) retVal.AWLCode[j + 1];
                                                    nRw.Parameter = akRw.Parameter + "." + nRw.Parameter;
                                                    nRw.MC7 = Helper.CombineByteArray(akRw.MC7, nRw.MC7);

                                                    akRw = nRw;
                                                    retVal.AWLCode.RemoveAt(j + 1);
                                                }


                                                if (akRw.GetNumberOfLines() == 1)
                                                {
                                                    subCnt = 0;

                                                    lastRow = akRw;

                                                    newAwlCode.Add(akRw);
                                                    j++;
                                                }
                                                else
                                                {
                                                    lastRow = akRw;
                                                    if (subCnt == 0)
                                                    {
                                                        newAwlCode.Add(akRw);
                                                    }

                                                    if (akRw.GetNumberOfLines() - 1 == subCnt)
                                                    {
                                                        j++;
                                                        //subCnt = 0;
                                                        subCnt++;
                                                    }
                                                    else
                                                    {
                                                        subCnt++;
                                                    }
                                                }
                                            }
                                        }


                                        //if (lastRow == null || cmt[n + 4] != 0x80)
                                        if (lastRow == null || (cmt[n + 4] != 0x80 && cmt[n + 4] != 0xc0))
                                        {
                                            lastRow = new S7FunctionBlockRow();
                                            newAwlCode.Add(lastRow);
                                            subCnt = 0;
                                        }

                                        string tx1 = Project.ProjectEncoding.GetString(cmt, n + 6, kommLen);
                                        if (subCnt == 0)
                                            lastRow.Comment = tx1;
                                        else
                                            if (lastRow.Command == "CALL")
                                                if (subCnt == 1) lastRow.Comment = tx1;
                                                else
                                                    lastRow.CallParameter[subCnt - 2].Comment = tx1;
                                        n += kommLen + 6;

                                        //subCnt = 0;
                                    }
                                }
                            }
                            while (j < retVal.AWLCode.Count)
                            {
                                newAwlCode.Add(retVal.AWLCode[j]);
                                j++;
                            }
                            retVal.AWLCode = newAwlCode;
                        }
                        #endregion
                    }

                    retVal.Networks = NetWork.GetNetworksList(retVal);

                    retVal.ParentFolder = this;

                    return retVal;
                }
            }
            return null;
        }


        /// <summary>
        /// This is a Helper Function wich is used by GetSourceBlock
        /// </summary>
        /// <param name="datrw"></param>
        /// <param name="leerz"></param>
        /// <returns></returns>
        private string GetSubParas(S7DataRow datrw, string leerz)
        {
            string retval = "";
            foreach (S7DataRow s7DataRow in datrw.Children)
            {
                string arr = "";
                string val = "";
                string cmt = "";
                if (s7DataRow.IsArray)
                {
                    arr += "ARRAY  [";
                    for (int i = 0; i < s7DataRow.ArrayStart.Count; i++)
                    {
                        if (i > 1) arr += ", ";
                        arr += s7DataRow.ArrayStart[i].ToString() + " .. " + s7DataRow.ArrayStop[i].ToString() + " ";
                    }
                    arr += "] OF ";
                }
                if (s7DataRow.StartValue != null)
                    val += " := " + s7DataRow.StartValue.ToString();
                if (!string.IsNullOrEmpty(s7DataRow.Comment))
                    cmt += "    //" + s7DataRow.Comment;
                if (s7DataRow.DataType == S7DataRowType.STRUCT)
                {
                    retval += leerz + s7DataRow.Name + " : " + arr + s7DataRow.DataType + cmt + Environment.NewLine;
                    retval += GetSubParas(s7DataRow, leerz + " ");
                    retval += leerz + "END_STRUCT ;" + Environment.NewLine;
                }
                else
                {
                    retval += leerz + s7DataRow.Name + " : " + arr + s7DataRow.DataType + (s7DataRow.DataTypeBlockNumber != 0 ? s7DataRow.DataTypeBlockNumber.ToString() : "") + " " + val + ";" + cmt + Environment.NewLine;
                }
            }
            return retval;
        }

        /// <summary>
        /// With this Function you get the AWL Source of a Block, so that it can be imported into Step7
        /// </summary>
        /// <param name="blkInfo">The BlockInfo from the Block you wish to get the Source of!</param>
        /// <returns></returns>
        public string GetSourceBlock(ProjectBlockInfo blkInfo)
        {
            StringBuilder retVal = new StringBuilder();
            Block blk = GetBlock(blkInfo, new S7ConvertingOptions() { CombineDBOpenAndDBAccess = true, GenerateCallsfromUCs = true, ReplaceDBAccessesWithSymbolNames = false, ReplaceLokalDataAddressesWithSymbolNames = true, UseComments = true });


            S7Block fblk = (S7Block)blk;
            S7FunctionBlock fcblk = null;

            if (blk is S7FunctionBlock)
            {
                fcblk = (S7FunctionBlock) blk;
                if (fcblk.BlockType == PLCBlockType.FC)
                    retVal.Append("FUNCTION " + blk.BlockName + " : VOID" + Environment.NewLine);
                else
                    retVal.Append("FUNCTION_BLOCK " + blk.BlockName + Environment.NewLine);
            }
            else
                retVal.Append("DATA_BLOCK " + blk.BlockName + Environment.NewLine);
            retVal.Append("TITLE =" + fblk.Title + Environment.NewLine);
            if (blk is S7FunctionBlock)
            {

                if (!String.IsNullOrEmpty(fcblk.Description))
                    retVal.Append("//" + fcblk.Description.Replace(Environment.NewLine, Environment.NewLine + "//") + Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(fblk.Author))
                retVal.Append("AUTHOR : " + fblk.Author + Environment.NewLine);
            if (!string.IsNullOrEmpty(fblk.Name))
                retVal.Append("NAME : " + fblk.Name + Environment.NewLine);
            if (!string.IsNullOrEmpty(fblk.Version))
                retVal.Append("VERSION : " + fblk.Version + Environment.NewLine);
            retVal.Append(Environment.NewLine);
            retVal.Append(Environment.NewLine);

            if (blk is S7DataBlock)
            {
                S7DataBlock dtaBlk = (S7DataBlock)fblk;
                if (dtaBlk.Structure.Children != null && !dtaBlk.IsInstanceDB)
                {
                    retVal.Append("  STRUCT" + Environment.NewLine);
                    retVal.Append(GetSubParas(dtaBlk.Structure, "    "));
                    retVal.Append("  END_STRUCT ;" + Environment.NewLine);

                }
                else if (dtaBlk.IsInstanceDB)
                    retVal.Append(" FB " + dtaBlk.FBNumber + Environment.NewLine);
                retVal.Append("BEGIN" + Environment.NewLine);
                retVal.Append("END_DATA_BLOCK" + Environment.NewLine);
            }
            else if (blk is S7FunctionBlock)
            {
                if (fcblk.Parameter.Children != null)
                {
                    foreach (S7DataRow s7DataRow in fcblk.Parameter.Children)
                    {
                        string parnm = s7DataRow.Name;
                        string ber = "VAR_" + parnm;
                        if (parnm == "IN")
                            ber = "VAR_INPUT";
                        else if (parnm == "OUT")
                            ber = "VAR_OUTPUT";
                        else if (parnm == "STATIC")
                            ber = "VAR";
                        retVal.Append(ber + Environment.NewLine);
                        retVal.Append(GetSubParas(s7DataRow, "  "));
                        retVal.Append("END_VAR" + Environment.NewLine);
                    }

                }
                retVal.Append("BEGIN" + Environment.NewLine);
                foreach (Network network in fcblk.Networks)
                {
                    retVal.Append("NETWORK" + Environment.NewLine);
                    retVal.Append("TITLE = " + network.Name + Environment.NewLine);
                    if (!String.IsNullOrEmpty(network.Comment))
                        retVal.Append("//" + network.Comment.Replace(Environment.NewLine, Environment.NewLine + "//") + Environment.NewLine);
                    else
                        retVal.Append(Environment.NewLine);
                    foreach (S7FunctionBlockRow functionBlockRow in network.AWLCode)
                    {
                        if (functionBlockRow.ToString(false, false) == "")
                            retVal.Append(Environment.NewLine);
                        else
                        {
                            retVal.Append(functionBlockRow.ToString(false, true) + Environment.NewLine);
                        }
                    }
                }
                retVal.Append("END_FUNCTION");
            }

            return retVal.ToString();
        }
    }
}

