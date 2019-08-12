using System;
using System.Collections.Generic;
using System.Threading;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5
{
    public class ReferenceData : Step5ProjectFolder
    {
        public String Folder { get; set; }

        private Dictionary<string, ReferenceDataEntry> operandIndexList = new Dictionary<string, ReferenceDataEntry>();

        private bool ReferenceDataLoaded = false;

        private List<ReferenceDataEntry> _ReferenceDataEntrys = new List<ReferenceDataEntry>();
        public List<ReferenceDataEntry> ReferenceDataEntrys
        {
            get
            {
                while (!ReferenceDataLoaded)
                { Thread.Sleep(500); }
                return _ReferenceDataEntrys;
            }            
        }

        internal bool showDeleted { get; set; }

        public ReferenceDataEntry GetEntryFromOperand(string operand)
        {
            while (!ReferenceDataLoaded)
            { Thread.Sleep(500); }

            string tmpname = operand.Trim().ToUpper();
            ReferenceDataEntry retval = null;
            operandIndexList.TryGetValue(tmpname, out retval);
            return retval;
        }

        public ReferenceData(Step5ProgrammFolder Parent, Project Project)
        {
            this.Parent = Parent;
            this.Project = Project;

            Name = "ReferenzDaten";

            //Load Referencedata in a Background Thread.
            Thread trd = new Thread(new ThreadStart(LoadReferenceData));
            trd.Start();
        }
        
        private void LoadReferenceData()
        {
            ReferenceDataLoaded = false;

            Step5ProgrammFolder prgFld = (Step5ProgrammFolder) this.Parent;
            Step5BlocksFolder blkFld = (Step5BlocksFolder) prgFld.BlocksFolder;
            SymbolTable smyTab = (SymbolTable) prgFld.SymbolTable;

            //try
            {
                foreach (ProjectBlockInfo projectBlockInfo in blkFld.readPlcBlocksList())
                {
                    if (projectBlockInfo.BlockType == PLCBlockType.S5_PB || projectBlockInfo.BlockType == PLCBlockType.S5_FB || projectBlockInfo.BlockType == PLCBlockType.S5_FX || projectBlockInfo.BlockType == PLCBlockType.S5_OB || projectBlockInfo.BlockType == PLCBlockType.S5_SB)
                    {
                        S5FunctionBlock blk = (S5FunctionBlock)projectBlockInfo.GetBlock();
                        int networkNR = 0;
                        foreach (Blocks.Network network in blk.Networks)
                        {
                            int rowNR = 0;
                            networkNR++;
                            string akdb = "";

                            foreach (S5FunctionBlockRow functionBlockRow in network.AWLCode)
                            {
                                if (!string.IsNullOrEmpty(functionBlockRow.Label))
                                    akdb = "";
                                if (functionBlockRow.Command == "A" || functionBlockRow.Command == "AX")
                                    if (functionBlockRow.Parameter == "DB 0" || functionBlockRow.Parameter == "DX 0")
                                        akdb = "";
                                    else
                                        akdb = functionBlockRow.Parameter;



                                rowNR++;
                                if (functionBlockRow.MC5LIB_SYMTAB_Row != null && ((ReferenceDataAccessMode)functionBlockRow.MC5LIB_SYMTAB_Row[9]) != ReferenceDataAccessMode.None)
                                {
                                    //Normal reference...
                                    string operand = functionBlockRow.Parameter;
                                    if (operand == "DB 0") operand = "DB ??";
                                    ReferenceDataEntry entr;
                                    operandIndexList.TryGetValue(operand, out entr);
                                    if (entr == null)
                                    {
                                        SymbolTableEntry symb = null;
                                        if (smyTab != null)
                                            symb = smyTab.GetEntryFromOperand(operand);
                                        entr = new ReferenceDataEntry() { Operand = operand, SymbolTableEntry = symb };
                                        operandIndexList.Add(operand, entr);
                                        _ReferenceDataEntrys.Add(entr);
                                    }
                                    entr.ReferencePoints.Add(new ReferencePoint() { Block = blk, Network = network, NetworkNumber = networkNR, LineNumber = rowNR, BlockRow = functionBlockRow, AccessMode = (ReferenceDataAccessMode)functionBlockRow.MC5LIB_SYMTAB_Row[9] });

                                    //Reference to DB
                                    if (akdb != "")
                                        if (functionBlockRow.Parameter.StartsWith("D ") || functionBlockRow.Parameter.StartsWith("DW") || functionBlockRow.Parameter.StartsWith("DL") || functionBlockRow.Parameter.StartsWith("DR") || functionBlockRow.Parameter.StartsWith("DX"))
                                        {
                                            //Normal reference...
                                            operand = akdb + "." + functionBlockRow.Parameter;
                                            entr = null;
                                            operandIndexList.TryGetValue(operand, out entr);
                                            if (entr == null)
                                            {
                                                SymbolTableEntry symb = null;
                                                if (smyTab != null)
                                                    symb = smyTab.GetEntryFromOperand(operand);
                                                entr = new ReferenceDataEntry() { Operand = operand, SymbolTableEntry = symb };
                                                operandIndexList.Add(operand, entr);
                                                _ReferenceDataEntrys.Add(entr);
                                            }
                                            entr.ReferencePoints.Add(new ReferencePoint() { Block = blk, Network = network, NetworkNumber = networkNR, LineNumber = rowNR, BlockRow = functionBlockRow, AccessMode = (ReferenceDataAccessMode)functionBlockRow.MC5LIB_SYMTAB_Row[9] });
                                        }
                                }
                            }
                        }


                    }
                }
            }
            /*catch (Exception ex)
            {
                MessageBox.Show("There was an error generating the Reference Data! Maybe the Step5 project is broken? \n" + ex.Message);
            }*/

            ReferenceDataLoaded = true;
        }                
    }
}
