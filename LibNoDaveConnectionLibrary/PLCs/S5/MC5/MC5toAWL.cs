using System;
using System.Collections.Generic;
using System.Diagnostics;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S5.MC5
{
    public static class MC5toAWL
    {
        const int code = 0;
        const int code1 = 1;
        const int codelen = 2;
        const int parlen = 3;
        const int cmask = 4;
        const int pmask = 5;
        const int operation = 6;
        const int operand = 7;
        const int printpar = 8;

        static object[] sym = MC5LIB_SYMTAB.symtab;

        public static S5FunctionBlock GetFunctionBlock(ProjectPlcBlockInfo blkInfo, byte[] block, byte[] preHeader, byte[] commentBlock, Step5ProgrammFolder prjBlkFld)
        {
            S5FunctionBlock retVal = new S5FunctionBlock();

            retVal.BlockType = blkInfo.BlockType;
            retVal.BlockNumber = blkInfo.BlockNumber;

            if (blkInfo.BlockType == PLCBlockType.S5_PB || blkInfo.BlockType == PLCBlockType.S5_SB || blkInfo.BlockType == PLCBlockType.S5_OB)
                retVal.AWLCode = GetMC5Rows(block, 10, null, (Step5BlocksFolder)blkInfo.ParentFolder, retVal);
            else
            {
                string fbNM = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(block, 12, 8);

                retVal.Name = fbNM;

                List<S5Parameter> pars = GetParameters(block);

                retVal.Parameter = pars;

                retVal.AWLCode = GetMC5Rows(block, /*10 + spa*2*/ 10 + ((pars.Count * 3) + 5) * 2, pars, (Step5BlocksFolder)blkInfo.ParentFolder, retVal);
            }
            //Go throug retval and add Symbols...
            if (prjBlkFld != null)
            {
                if (prjBlkFld.SymbolTable != null)
                {
                    foreach (S5FunctionBlockRow awlRow in retVal.AWLCode)
                    {
                        string para = awlRow.Parameter.Replace(" ", "");

                        awlRow.SymbolTableEntry = prjBlkFld.SymbolTable.GetEntryFromOperand(para);
                    }
                }
            }

            retVal.Networks = new List<Network>();

            S5FunctionBlockNetwork nw = null;

            if (retVal.AWLCode != null)
                foreach (S5FunctionBlockRow s5FunctionBlockRow in retVal.AWLCode)
                {
                    if (s5FunctionBlockRow.Command == "BLD" && s5FunctionBlockRow.Parameter == "255")
                    {
                        if (nw != null)
                            nw.AWLCode.Add(s5FunctionBlockRow);
                        nw = new S5FunctionBlockNetwork();
                        nw.Parent = retVal;
                        nw.AWLCode = new List<FunctionBlockRow>();
                        ((S5FunctionBlockNetwork)nw).NetworkFunctionBlockRow = s5FunctionBlockRow;
                        retVal.Networks.Add(nw);
                    }
                    else
                    {
                        nw.AWLCode.Add(s5FunctionBlockRow);
                    }

                }

            if (commentBlock != null)
            {
                int nr = 26;
                int netzwnr = 0;
                Dictionary<int, object> commandLineNumerList = new Dictionary<int, object>();
                while (nr + 3 < commentBlock.Length)
                {
                    int zeile = commentBlock[nr];

                    int len = 0x7f & commentBlock[nr + 2];
                    string cmt = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(commentBlock, nr + 3, len);

                    if (commentBlock[nr + 1] == 0x00)
                    {
                        netzwnr = zeile;
                        if (retVal.Networks.Count > netzwnr - 1 && netzwnr > 0)
                        {
                            ((S5FunctionBlockNetwork)retVal.Networks[netzwnr - 1]).Name = cmt;
                            commandLineNumerList.Clear();
                            int lineNr = 0;
                            foreach (S5FunctionBlockRow akRow in retVal.Networks[netzwnr - 1].AWLCode)
                            {

                                commandLineNumerList.Add(lineNr, akRow);
                                lineNr++;

                                if (IsCall(akRow))
                                {
                                    foreach (S5Parameter extPar in akRow.ExtParameter)
                                    {
                                        commandLineNumerList.Add(lineNr, extPar);
                                        lineNr++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Todo: Throw an error when this happens???
                            //throw new Exception("Error in Block: " + retVal.BlockName);
                        }

                    }
                    else
                    {
                        if (commandLineNumerList.ContainsKey(zeile - 1))
                        {
                            object obj = commandLineNumerList[zeile - 1];
                            if (obj is S5Parameter)
                                ((S5Parameter)obj).Comment = cmt;
                            else
                                ((S5FunctionBlockRow)obj).Comment = cmt;
                        }
                    }
                    nr += len + 3;
                }

            }

            return retVal;
        }

        private static List<S5Parameter> GetParameters(byte[] code)
        {
            List<S5Parameter> retVal = new List<S5Parameter>();
            int index = find_mc5_code(code, 10);
            if (index < 0 || (string)((object[])sym[index])[operation] != "SPA=")
                throw new Exception("Error converting FB!");
            int spa = find_mc5_param(code, 10, index);

            int paranz = (spa - 5) / 3;

            for (int n = 0; n < paranz; n++)
            {
                S5Parameter tmp = new S5Parameter();
                int t1 = code[n * 6 + 20];
                switch (t1)
                {
                    case 2:
                        tmp.S5ParameterType = S5ParameterType.E;
                        break;
                    case 4:
                        tmp.S5ParameterType = S5ParameterType.D;
                        break;
                    case 5:
                        tmp.S5ParameterType = S5ParameterType.T;
                        break;
                    case 7:
                        tmp.S5ParameterType = S5ParameterType.B;
                        break;
                    case 8:
                        tmp.S5ParameterType = S5ParameterType.A;
                        break;
                }

                int t2 = code[n * 6 + 21];
                if (tmp.S5ParameterType == S5ParameterType.D)
                {
                    switch (t2)
                    {
                        case 1:
                            tmp.S5ParameterFormat = S5ParameterFormat.KZ;
                            break;
                        case 2:
                            tmp.S5ParameterFormat = S5ParameterFormat.KT;
                            break;
                        case 4:
                            tmp.S5ParameterFormat = S5ParameterFormat.KF;
                            break;
                        case 8:
                            tmp.S5ParameterFormat = S5ParameterFormat.KG;
                            break;
                        case 16:
                            tmp.S5ParameterFormat = S5ParameterFormat.KC;
                            break;
                        case 32:
                            tmp.S5ParameterFormat = S5ParameterFormat.KY;
                            break;
                        case 64:
                            tmp.S5ParameterFormat = S5ParameterFormat.KH;
                            break;
                        case 128:
                            tmp.S5ParameterFormat = S5ParameterFormat.KM;
                            break;
                    }
                }
                else
                    switch (t2)
                    {
                        case 16:
                            tmp.S5ParameterFormat = S5ParameterFormat.D;
                            break;
                        case 32:
                            tmp.S5ParameterFormat = S5ParameterFormat.W;
                            break;
                        case 64:
                            tmp.S5ParameterFormat = S5ParameterFormat.BY;
                            break;
                        case 128:
                            tmp.S5ParameterFormat = S5ParameterFormat.BI;
                            break;
                    }
                tmp.Name = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(code, n * 6 + 22, 4);
                retVal.Add(tmp);
            }

            return retVal;
        }


        private static bool IsCall(S5FunctionBlockRow newRow)
        {
            if ((newRow.Command == "SPA" && newRow.Parameter.Substring(0, 2) == "FB") ||
                        (newRow.Command == "SPB" && newRow.Parameter.Substring(0, 2) == "FB") ||
                        (newRow.Command == "BA" && newRow.Parameter.Substring(0, 2) == "FX") ||
                        (newRow.Command == "BAB" && newRow.Parameter.Substring(0, 2) == "FX"))
            {
                return true;
            }
            return false;
        }

        private static List<FunctionBlockRow> GetMC5Rows(byte[] code, int codestart, List<S5Parameter> parameters, Step5BlocksFolder blkFld, S5FunctionBlock block)
        {
            List<FunctionBlockRow> retVal = new List<FunctionBlockRow>();
            int codepos = codestart;

            retVal.Add(new S5FunctionBlockRow() { Command = "BLD", Parameter = "255", Parent = block }); //Command for first Network does not exist!
                                                                                                         //This needs to be removen when written back to S5D!

            while (codepos < code.Length)
            {

                byte mc = code[codepos];

                // finde Befehle in der Symbol Tabelle 
                int index = find_mc5_code(code, codepos);



                S5FunctionBlockRow newRow = new S5FunctionBlockRow() { Parent = block };

                if (index >= 0)
                {
                    newRow.MC5LIB_SYMTAB_Row = (object[])sym[index];

                    bool fb = false;
                    newRow.Command = (string)((object[])sym[index])[operation];

                    string oper = (string)((object[])sym[index])[operand];

                    if (oper == "PAR")
                    {
                        int paridx = find_mc5_param(code, codepos, index);

                        if (parameters != null && parameters.Count > paridx - 1)
                        {
                            newRow.Parameter = "=" + parameters[paridx - 1].Name;
                        }
                        else
                        {
                            newRow.Parameter = "$$error$$ -> invalid Parameter, idx:" + (paridx - 1);
                        }
                    }
                    else

                    {

                        if (oper != null)
                            newRow.Parameter = oper;

                        fb = IsCall(newRow);    //false;
                                                /*if ((newRow.Command=="SPA" && newRow.Parameter=="FB") ||
                                                    (newRow.Command=="SPB" && newRow.Parameter=="FB") ||
                                                    (newRow.Command=="BA" && newRow.Parameter=="FX") ||
                                                    (newRow.Command=="BAB" && newRow.Parameter=="FX"))
                                                {
                                                    fb = true;
                                                }*/

                        int par = find_mc5_param(code, codepos, index);

                        switch ((string)((object[])sym[index])[printpar])
                        {
                            case null:
                                break;
                            case "PrintNummer":
                                if (!string.IsNullOrEmpty(newRow.Parameter))
                                    newRow.Parameter += " ";
                                Int16 wrt = (Int16)par;
                                if (wrt >= 0 && newRow.Parameter == "KF ")
                                    newRow.Parameter += " +";
                                newRow.Parameter += wrt.ToString();
                                break;
                            case "PrintKY":
                                newRow.Parameter += BitConverter.GetBytes(par)[1].ToString() + "," +
                                                    BitConverter.GetBytes(par)[0].ToString();
                                break;
                            case "PrintChar":
                                if (!string.IsNullOrEmpty(newRow.Parameter))
                                    newRow.Parameter += " ";
                                newRow.Parameter += ((char)(BitConverter.GetBytes((Int16)par)[0])).ToString() + ((char)(BitConverter.GetBytes((Int16)par)[1]));
                                break;
                            case "PrintHEX4":
                                if (!string.IsNullOrEmpty(newRow.Parameter))
                                    newRow.Parameter += " ";
                                newRow.Parameter += par.ToString("X").PadLeft(4, '0');
                                break;
                            case "PrintGleitpunkt":
                                if (!string.IsNullOrEmpty(newRow.Parameter))
                                    newRow.Parameter += " ";
                                newRow.Parameter += "gleit";
                                break;
                            case "PrintHEX8":
                                if (!string.IsNullOrEmpty(newRow.Parameter))
                                    newRow.Parameter += " ";
                                newRow.Parameter += par.ToString("X").PadLeft(4, '0');
                                break;
                            case "PrintMerkerBit":
                                if (!string.IsNullOrEmpty(newRow.Parameter))
                                    newRow.Parameter += " ";
                                newRow.Parameter += (par & 0x00FF).ToString() + "." + (par >> 8).ToString();
                                break;
                            case "PrintSMerkerBit":
                                if (!string.IsNullOrEmpty(newRow.Parameter))
                                    newRow.Parameter += " ";
                                newRow.Parameter += (par & 0x0FFF).ToString() + "." + (par >> 12).ToString();
                                break;
                        }
                    }

                    int _parlen = (int)((object[])sym[index])[parlen];
                    int _codelen = (int)((object[])sym[index])[codelen];


                    int btSize = (_codelen + _parlen) / 8;

                    codepos += btSize; // Schleifenzähler     

                    if (fb)
                    {
                        //FB-Call
                        index = find_mc5_code(code, codepos);
                        if (index < 0 || (string)((object[])sym[index])[operation] != "SPA=")
                            newRow.Command = "Error reading Jump after FB/FX Call!";
                        int spa = find_mc5_param(code, codepos, index);

                        newRow.ExtParameter = new List<S5Parameter>();

                        //newRow.ExtParameter = new List<string>();



                        byte[] calledfb = blkFld.GetBlockInByte(/* "S5_" + */ newRow.Parameter.Replace(" ", ""));
                        if (calledfb != null)
                        {
                            S5Parameter newPar = new S5Parameter();
                            newRow.ExtParameter.Add(newPar);

                            string fbNM = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(calledfb, 12, 8);
                            //newRow.ExtParameter.Add("Name :" + fbNM);

                            newPar.Name = "Name :";
                            newPar.Value = fbNM;

                            List<S5Parameter> par = GetParameters(calledfb);
                            int j = 0;


                            foreach (var s5Parameter in par)
                            {
                                newPar = new S5Parameter();
                                newRow.ExtParameter.Add(newPar);

                                j++;
                                string akOper = "";

                                int pos = codepos + j * 2;

                                switch (s5Parameter.S5ParameterType)
                                {
                                    case S5ParameterType.A:
                                    case S5ParameterType.E:
                                        int ber = code[pos];
                                        switch (s5Parameter.S5ParameterFormat)
                                        {

                                            case S5ParameterFormat.BI:
                                                switch (ber & 0xf0)
                                                {
                                                    case 0x80:
                                                        akOper += "M";
                                                        break;
                                                }
                                                akOper += code[pos + 1].ToString();
                                                akOper += "." + (code[pos] & 0x0f).ToString();
                                                break;
                                            case S5ParameterFormat.BY:
                                                switch (ber)
                                                {
                                                    case 0x0a:
                                                        akOper += "MB";
                                                        break;
                                                    case 0x2a:
                                                        akOper += "DR";
                                                        break;
                                                    case 0x22:
                                                        akOper += "DL";
                                                        break;
                                                    default:
                                                        akOper += "";
                                                        break;
                                                }
                                                akOper += code[pos + 1].ToString();
                                                break;
                                            case S5ParameterFormat.W:
                                                switch (ber)
                                                {
                                                    case 0x12:
                                                        akOper += "MW";
                                                        break;
                                                    case 0x32:
                                                        akOper += "DW";
                                                        break;
                                                }

                                                akOper += code[pos + 1].ToString();
                                                break;
                                            case S5ParameterFormat.D:
                                                break;
                                        }
                                        break;
                                    case S5ParameterType.D:
                                        int wrt = code[pos] * 0x100 + code[pos + 1];
                                        akOper += s5Parameter.S5ParameterFormat.ToString() + " ";
                                        switch (s5Parameter.S5ParameterFormat)
                                        {
                                            case S5ParameterFormat.KF:
                                                if (((Int16)wrt) > 0)
                                                    akOper += "+";
                                                akOper += ((Int16)wrt).ToString();
                                                break;
                                            case S5ParameterFormat.KT:
                                                //code[pos + 1]
                                                // Bit      4-7       0-3    4-7   0-3         
                                                //Format: Zeitbasis, 100er, 10er, 1er
                                                akOper += wrt.ToString().PadLeft(3, '0') + ".0";
                                                break;
                                            case S5ParameterFormat.KC:
                                                akOper += (char)code[pos] + (char)code[pos + 1];
                                                break;
                                            case S5ParameterFormat.KM:
                                                akOper += libnodave.dec2bin(code[pos]) + " ";
                                                akOper += libnodave.dec2bin(code[pos + 1]);
                                                break;
                                            case S5ParameterFormat.KY:
                                                akOper += code[pos].ToString() + "," + code[pos + 1].ToString();
                                                break;
                                            default:
                                                akOper += ((Int16)wrt).ToString("X").PadLeft(4, '0');
                                                break;
                                        }
                                        break;
                                    case S5ParameterType.B:
                                        int bst = code[pos];
                                        switch (bst)
                                        {
                                            case 0x20:
                                                akOper += "DB";
                                                break;
                                        }
                                        akOper += code[pos + 1].ToString();

                                        break;
                                    case S5ParameterType.T:
                                        //int abst = code[pos];                                                                                              
                                        akOper += "T " + code[pos + 1].ToString();
                                        break;
                                    case S5ParameterType.Z:
                                        akOper += "Z " + code[pos + 1].ToString();
                                        break;
                                }
                                newPar.Name = s5Parameter.Name;
                                newPar.Value = akOper;
                                //newRow.ExtParameter.Add(s5Parameter.Name.PadRight(5, ' ') + ":  " + akOper);

                            }
                        }
                        else
                            for (int j = 1; j < spa; j++)
                            {
                                int myPar = (code[codepos + j * 2] << 8) | code[codepos + j * 2 + 1];
                                //newRow.ExtParameter.Add(myPar.ToString());
                                S5Parameter newPar = new S5Parameter();
                                newRow.ExtParameter.Add(newPar);
                                newPar.Name = myPar.ToString();
                            }

                        btSize += spa * 2;

                        codepos += spa * 2;
                        fb = false;
                    }

                    byte[] MC5 = new byte[btSize];

                    Array.Copy(code, codepos - btSize, MC5, 0, btSize);

                    newRow.MC5 = MC5;

                }
                else
                {
                    newRow.Command = "Error converting command!";
                    codepos += 2;
                    //throw new Exception("Code nicht in Symtab!");
                }
                retVal.Add(newRow);

            }

            #region Build the Jumpsmarks....
            int JumpCount = 1;
            int akBytePos = 0;
            Dictionary<int, S5FunctionBlockRow> ByteAdressNumerPLCFunctionBlocks = new Dictionary<int, S5FunctionBlockRow>();
            foreach (S5FunctionBlockRow tmp in retVal)
            {
                if (tmp.ByteSize > 0)
                {
                    ByteAdressNumerPLCFunctionBlocks.Add(akBytePos, tmp);
                    akBytePos += tmp.ByteSize;
                }
            }

            akBytePos = 0;
            foreach (S5FunctionBlockRow tmp in retVal)
            {
                if (tmp.Command == "SPA=" || tmp.Command == "SPB=" || tmp.Command == "SPZ=" || tmp.Command == "SPN=" || tmp.Command == "SPP=" || tmp.Command == "SPM=" || tmp.Command == "SPO=")
                {
                    int jmpBytePos = 0;

                    byte[] backup = tmp.MC5;

                    tmp.Command = tmp.Command.Substring(0, tmp.Command.Length - 1);

                    tmp.JumpWidth = (SByte)Convert.ToInt32(tmp.Parameter);

                    jmpBytePos = akBytePos + (tmp.JumpWidth * 2);

                    if (ByteAdressNumerPLCFunctionBlocks.ContainsKey(jmpBytePos))
                    {
                        var target = ByteAdressNumerPLCFunctionBlocks[jmpBytePos];
                        if (target.Label.Trim() == "")
                        {
                            target.Label = "M" + JumpCount.ToString().PadLeft(3, '0');
                            JumpCount++;
                        }

                        //Backup the MC7 Code, because the MC7 Code is always deleted when the command or parameter changes!

                        tmp.Parameter = "=" + target.Label;
                        tmp.MC5 = backup;
                    }
                    else
                    {
                        tmp.Parameter = "Error! JumpWidth :" + tmp.JumpWidth;
                        tmp.MC5 = backup;
                    }


                }
                akBytePos += tmp.ByteSize;
            }
            #endregion            

            return retVal;
        }


        private static UInt32 swab_word(UInt32 inWrt)
        {
            return (inWrt << 16) | (inWrt >> 16);
        }

        private static int find_mc5_param(byte[] parcode, int codepos, int index)
        {
            int C16; // Code, Parameter = 16 bit
            int C32 = 0; // Code, Parameter = 32 bit
            C16 = (parcode[codepos] << 8) | parcode[codepos + 1];
            if (codepos + 3 < parcode.Length)
                C32 = (parcode[codepos] << 24) | (parcode[codepos + 1] << 16) | (parcode[codepos + 2] << 8) | (parcode[codepos + 3]);


            int P16; // Code, Parameter = 16 bit
            int P32; // Code, Parameter = 32 bit

            int _parlen = (int)((object[])sym[index])[parlen];
            int _codelen = (int)((object[])sym[index])[codelen];
            UInt32 _pmask = System.Convert.ToUInt32(((object[])sym[index])[pmask]);


            if (_parlen == 0)
            {
                return 0;
            }

            if (_codelen + _parlen <= 16)
            {
                return (int)(C16 & _pmask);
            }
            else if (_codelen + _parlen <= 32)
            {
                return (int)(C32 & _pmask);
            }
            /*
           else {
             P32 = *(unsigned int*)(opcode+2);
             swab_word(&P32, &P32);
             return P32 & sym[symtab_index].pmask;
           }
                */
            return 0;
        }

        private static int find_mc5_code(byte[] parcode, int codepos)
        {
            int i;

            int C16; // Code, Parameter = 16 bit
            int C32 = 0; // Code, Parameter = 32 bit
            C16 = (parcode[codepos] << 8) | parcode[codepos + 1];
            if (codepos + 3 < parcode.Length)
                C32 = (parcode[codepos] << 24) | (parcode[codepos + 1] << 16) | (parcode[codepos + 2] << 8) | (parcode[codepos + 3]);

            for (i = 0; i < sym.Length; i++)
            {
                if ((int)((object[])sym[i])[code] == 0x7803)
                    i = i;

                if ((int)((object[])sym[i])[codelen] <= 16)
                {

                    int _cmask = (int)((object[])sym[i])[cmask];
                    int _code = (int)((object[])sym[i])[code];

                    if ((C16 & _cmask) == _code)
                    {
                        return i;
                    }
                }
                else if (C32 != 0)
                {

                    //C32 = swab_word(C32);
                    int _cmask = (int)((object[])sym[i])[cmask];
                    int _code = (int)((object[])sym[i])[code];
                    int _code1 = (int)((object[])sym[i])[code1];
                    int _codelen = (int)((object[])sym[i])[codelen];
                    if ((C32 & (_cmask << (32 - _codelen))) == ((_code << 16) | _code1))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}
