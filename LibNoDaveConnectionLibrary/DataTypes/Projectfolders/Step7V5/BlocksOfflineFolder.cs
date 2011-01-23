using System;
using System.Collections.Generic;
using System.Data;
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
                    if (ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "BAUSTEIN.DBF"))
                        _bausteinDbf = DBF.ParseDBF.ReadDBF(Folder + "BAUSTEIN.DBF", ((Step7ProjectV5) Project)._zipfile,
                                                            ((Step7ProjectV5) Project)._DirSeperator);
                return _bausteinDbf;
            }
        }

        private DataTable _subblkDbf;
        private DataTable subblkDBF
        {
            get
            {
                if (_subblkDbf == null)
                    if (ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "SUBBLK.DBF"))
                        _subblkDbf = DBF.ParseDBF.ReadDBF(Folder + "SUBBLK.DBF", ((Step7ProjectV5) Project)._zipfile,
                                                          ((Step7ProjectV5) Project)._DirSeperator);
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
        
        private List<ProjectBlockInfo> intReadPlcBlocksList()
        {
            bool showDeleted = ((Step7ProjectV5) this.Project)._showDeleted;

            List<ProjectBlockInfo> tmpBlocks = new List<ProjectBlockInfo>();
            if (bausteinDBF != null) //ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "BAUSTEIN.DBF"))
            {
                var dbfTbl = bausteinDBF; //DBF.ParseDBF.ReadDBF(Folder + "BAUSTEIN.DBF", ((Step7ProjectV5)Project)._zipfile, ((Step7ProjectV5)Project)._DirSeperator);

                SymbolTable symtab = (SymbolTable)((S7ProgrammFolder)Parent).SymbolTable;

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
            public byte[] nwinfo;
            public byte[] blockdescription;
            public byte[] jumpmarks;
            public bool knowHowProtection = false;
        }

        private Dictionary<string, tmpBlock> tmpBlocks;

        public ProjectBlockInfo GetProjectBlockInfoFromBlockName(string BlockName)
        {
            var tmp = readPlcBlocksList();
            foreach (var step7ProjectBlockInfo in tmp)
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

            if (subblkDBF!=null)// ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "SUBBLK.DBF"))
            {
                var dbfTbl = subblkDBF; // DBF.ParseDBF.ReadDBF(Folder + "SUBBLK.DBF", ((Step7ProjectV5)Project)._zipfile, ((Step7ProjectV5)Project)._DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {

                    int subblktype = Convert.ToInt32(row["SUBBLKTYP"]);
                    int objid = (int)row["OBJECTID"];

                    if (objid == blkInfo.id && (subblktype == 12 || subblktype == 8 || subblktype == 14))
                    {
                        if (KnowHowProtection)
                            DBF.ParseDBF.WriteValue(Folder + "SUBBLK.DBF", "PASSWORD", dbfTbl.Rows.IndexOf(row), 3, ((Step7ProjectV5)Project)._zipfile);
                        else
                            DBF.ParseDBF.WriteValue(Folder + "SUBBLK.DBF", "PASSWORD", dbfTbl.Rows.IndexOf(row), 0, ((Step7ProjectV5)Project)._zipfile);
                        break;
                    }
                }
            }
        }
        
        public void UndeleteBlock(S7ProjectBlockInfo blkInfo, int newBlockNumber)
        {
            tmpBlock myTmpBlk = new tmpBlock();

            if (ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "BAUSTEIN.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(Folder + "BAUSTEIN.DBF", ((Step7ProjectV5)Project)._zipfile, ((Step7ProjectV5)Project)._DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    int objid = (int)row["ID"];

                    if (objid == blkInfo.id)
                    {
                        DBF.ParseDBF.WriteValue(Folder + "BAUSTEIN.DBF", "DELETED_FLAG", dbfTbl.Rows.IndexOf(row), false, ((Step7ProjectV5)Project)._zipfile);
                        DBF.ParseDBF.WriteValue(Folder + "BAUSTEIN.DBF", "NUMMER", dbfTbl.Rows.IndexOf(row), newBlockNumber, ((Step7ProjectV5)Project)._zipfile);
                    }
                }
            }

            if (ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "SUBBLK.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(Folder + "SUBBLK.DBF", ((Step7ProjectV5)Project)._zipfile, ((Step7ProjectV5)Project)._DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    int objid = (int)row["OBJECTID"];

                    if (objid == blkInfo.id)
                    {
                        DBF.ParseDBF.WriteValue(Folder + "SUBBLK.DBF", "DELETED_FLAG", dbfTbl.Rows.IndexOf(row), false, ((Step7ProjectV5)Project)._zipfile);
                        DBF.ParseDBF.WriteValue(Folder + "SUBBLK.DBF", "BLKNUMBER", dbfTbl.Rows.IndexOf(row), newBlockNumber, ((Step7ProjectV5)Project)._zipfile);
                    }
                }
            }
        }

        private tmpBlock GetBlockBytes(ProjectBlockInfo blkInfo)
        {
            if (subblkDBF != null) //ZipHelper.FileExists(((Step7ProjectV5)Project)._zipfile, Folder + "SUBBLK.DBF"))
            {
                tmpBlock myTmpBlk = new tmpBlock();

                var dbfTbl = subblkDBF; // DBF.ParseDBF.ReadDBF(Folder + "SUBBLK.DBF", ((Step7ProjectV5)Project)._zipfile, ((Step7ProjectV5) Project)._DirSeperator);

                DataRow[] rows = dbfTbl.Select("OBJECTID = " + blkInfo.id);
                
                foreach (DataRow row in rows)// dbfTbl.Rows)
                {

                    int subblktype = Convert.ToInt32(row["SUBBLKTYP"]);
                    int objid = (int) row["OBJECTID"];

                    if (objid == blkInfo.id)
                    {
                        byte[] mc5code = null;
                        byte[] ssbpart = null;
                        byte[] addinfo = null;

                        if (row["MC5CODE"] != DBNull.Value)
                            mc5code = (byte[]) row["MC5CODE"];
                        if (row["SSBPART"] != DBNull.Value)
                            ssbpart = (byte[]) row["SSBPART"];
                        if (row["ADDINFO"] != DBNull.Value)
                            addinfo = (byte[]) row["ADDINFO"];
                        int mc5codelen = (int) row["MC5LEN"];
                        int ssbpartlen = (int) row["SSBLEN"];
                        int addinfolen = (int) row["ADDLEN"];

                        if (mc5code != null && mc5code.Length > mc5codelen)
                            Array.Resize<byte>(ref mc5code, mc5codelen);
                        if (ssbpart != null && ssbpart.Length > ssbpartlen)
                            Array.Resize<byte>(ref ssbpart, ssbpartlen);
                        if (addinfo != null && addinfo.Length > addinfolen)
                            Array.Resize<byte>(ref addinfo, addinfolen);

                        /*
                        string ttmc5, ttssb, ttadd;
                        if (mc5code!=null)
                            ttmc5 = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(mc5code);
                        if (ssbpart!=null)
                            ttssb = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(ssbpart);
                        if (addinfo != null)
                            ttadd = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(addinfo);
                        */

                        /*
                        if (mc5code != null)
                        {
                            FileStream aa = new FileStream("c:\\temp\\aa\\mc5code_subblk" + subblktype.ToString() + ".txt", FileMode.Create);
                            aa.Write(mc5code, 0, mc5code.Length);
                            aa.Close();
                        }
                        if (ssbpart != null)
                        {
                            FileStream aa = new FileStream("c:\\temp\\aa\\ssbpart_subblk" + subblktype.ToString() + ".txt", FileMode.Create);
                            aa.Write(ssbpart, 0, ssbpart.Length);
                            aa.Close();
                        }
                        if (addinfo != null)
                        {
                            FileStream aa = new FileStream("c:\\temp\\aa\\addinfo_subblk" + subblktype.ToString() + ".txt", FileMode.Create);
                            aa.Write(addinfo, 0, addinfo.Length);
                            aa.Close();
                        }
                        */


                        if (subblktype == 12 || subblktype == 8 || subblktype == 14 || subblktype == 13 ||
                            subblktype == 15) //FC, OB, FB, SFC, SFB
                        {
                            if (row["PASSWORD"] != DBNull.Value && (int) row["PASSWORD"] == 3)
                                myTmpBlk.knowHowProtection = true;
                            //MC7 Code in mc5code
                            myTmpBlk.mc7code = mc5code;
                            //Network Information in addinfo
                            myTmpBlk.nwinfo = addinfo;
                                //This line contains Network Information, and after it the Position of the JumpMarks
                        }
                        else if (subblktype == 5 || subblktype == 3 || subblktype == 4 || subblktype == 7 ||
                                 subblktype == 9) //FC, OB, FB, SFC, SFB
                        {
                            //Interface in mc5code
                            if (mc5code != null)
                                myTmpBlk.blkinterface =
                                    System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(mc5code);
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
                                    System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(mc5code);
                            //Maybe compiled DB Structure?                            
                            myTmpBlk.addinfo = addinfo;
                        }
                        else if (subblktype == 10) //DB
                        {
                            //actual Values in mc5code or maybe also the db code?
                            //Ssbpart contains?
                        }

                        else if (subblktype == 27) //VAT                        
                        {
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
            return GetBlock(blkInfo, new S7ConvertingOptions());
        }

        public Block GetBlock(ProjectBlockInfo blkInfo, S7ConvertingOptions myConvOpt)
        {
            //tmpBlock myTmpBlk = new tmpBlock();

            tmpBlock myTmpBlk = GetBlockBytes(blkInfo);

            if (myTmpBlk != null ) 
            {
                //Begin with the Block Reading...
                if (blkInfo.BlockType == PLCBlockType.VAT)
                {
                    return new S7VATBlock(myTmpBlk.mc7code, myTmpBlk.comments, blkInfo.BlockNumber);
                }
                else if (blkInfo.BlockType == PLCBlockType.DB || blkInfo.BlockType == PLCBlockType.UDT)
                {
                    List<string> tmpList = new List<string>();
                    S7DataBlock retVal=new S7DataBlock();
                    retVal.Structure = Parameter.GetInterfaceOrDBFromStep7ProjectString(myTmpBlk.blkinterface, ref tmpList, blkInfo.BlockType, false, this, retVal);                    
                    retVal.BlockNumber = blkInfo.BlockNumber;
                    retVal.BlockType = blkInfo.BlockType;
                    return retVal;
                }
                else if (blkInfo.BlockType == PLCBlockType.FC || blkInfo.BlockType == PLCBlockType.FB || blkInfo.BlockType == PLCBlockType.OB || blkInfo.BlockType == PLCBlockType.SFB ||blkInfo.BlockType == PLCBlockType.SFC)
                {
                    List<string> ParaList = new List<string>();

                    S7FunctionBlock retVal = new S7FunctionBlock();
                    retVal.BlockNumber = blkInfo.BlockNumber;
                    retVal.BlockType = blkInfo.BlockType;
                    retVal.KnowHowProtection = myTmpBlk.knowHowProtection;

                    retVal.Parameter = Parameter.GetInterfaceOrDBFromStep7ProjectString(myTmpBlk.blkinterface, ref ParaList, blkInfo.BlockType, false, this, retVal);                    

                    if (myTmpBlk.blockdescription!=null)
                    {
                        retVal.Name = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(myTmpBlk.blockdescription, 3, myTmpBlk.blockdescription[1] - 4);
                        retVal.Description = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(myTmpBlk.blockdescription, myTmpBlk.blockdescription[1], myTmpBlk.blockdescription.Length - myTmpBlk.blockdescription[1] - 1);
                    }

                    if (blkInfo.BlockType == PLCBlockType.FC || blkInfo.BlockType == PLCBlockType.FB || blkInfo.BlockType == PLCBlockType.OB)
                    {
                        int[] Networks;
                        Networks = NetWork.GetNetworks(0, myTmpBlk.nwinfo);
                        
                        retVal.AWLCode = MC7toAWL.GetAWL(0, myTmpBlk.mc7code.Length - 2, 0, myTmpBlk.mc7code, Networks, ParaList, (S7ProgrammFolder)this.Parent);

                        retVal.AWLCode = JumpMarks.AddJumpmarks(retVal.AWLCode, myTmpBlk.jumpmarks, myTmpBlk.nwinfo);

                        LocalDataConverter.ConvertLocaldataToSymbols(retVal, myConvOpt);

                        FBStaticAccessConverter.ReplaceStaticAccess(retVal, (S7ProgrammFolder) this.Parent, myConvOpt);

                        CallConverter.ConvertUCToCall(retVal, (S7ProgrammFolder) this.Parent, myConvOpt, null);

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

                                while (n < myTmpBlk.comments.Length)
                                {
                                    int kommLen = cmt[n + 0];
                                    int startNWKomm = cmt[n + 1];
                                    int anzUebsprungZeilen = cmt[n + 2] + cmt[n + 3]*0x100;
                                    int lenNWKommZeile = cmt[n + 3] + cmt[n + 4]*0x100;                                   

                                    if (cmt[n + 5] == 0x06)
                                    {
                                        //NWKomentar:
                                        string tx1 = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(cmt, n + 6, startNWKomm - 7);
                                        string tx2 = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(cmt, n + startNWKomm, lenNWKommZeile - startNWKomm - 1);
                                        n += lenNWKommZeile;

                                        if (retVal.AWLCode.Count > j)
                                        {
                                            while (retVal.AWLCode.Count - 1> j && retVal.AWLCode[j].Command != "NETWORK")
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
                                                if (((S7FunctionBlockRow)retVal.AWLCode[j]).GetNumberOfLines() == 1)
                                                {
                                                    subCnt = 0;
                                                    lastRow = (S7FunctionBlockRow)retVal.AWLCode[j];
                                                    newAwlCode.Add(retVal.AWLCode[j]);
                                                    j++;
                                                }
                                                else
                                                {
                                                    lastRow = (S7FunctionBlockRow)retVal.AWLCode[j];
                                                    if (subCnt == 0)
                                                    {
                                                        newAwlCode.Add(retVal.AWLCode[j]);
                                                    }

                                                    if (((S7FunctionBlockRow)retVal.AWLCode[j]).GetNumberOfLines() - 1 == subCnt)
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

                                        if (lastRow == null || cmt[n + 4] != 0x80)
                                        {
                                            lastRow = new S7FunctionBlockRow();
                                            newAwlCode.Add(lastRow);
                                            subCnt = 0;
                                        }

                                        string tx1 = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(cmt, n + 6, kommLen);
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

                    /*
                    //todo only use the networks structure!
                    retVal.Networks = new List<Network>();
                    S7FunctionBlockNetwork nw = null;
                    if (retVal.AWLCode != null)
                        foreach (S7FunctionBlockRow s7FunctionBlockRow in retVal.AWLCode)
                        {
                            if (s7FunctionBlockRow.Command == "NETWORK")
                            {
                                nw = new S7FunctionBlockNetwork();
                                nw.Parent = retVal;
                                nw.AWLCode = new List<FunctionBlockRow>();
                                retVal.Networks.Add(nw);
                                nw.Name = s7FunctionBlockRow.NetworkName;
                                nw.Comment = s7FunctionBlockRow.Comment;
                            }
                            else
                            {
                                nw.AWLCode.Add(s7FunctionBlockRow);
                            }

                        }
                       */

                    retVal.Networks = NetWork.GetNetworksList(retVal);

                    return retVal;
                }
            }
            return null;
        }
    }
}
