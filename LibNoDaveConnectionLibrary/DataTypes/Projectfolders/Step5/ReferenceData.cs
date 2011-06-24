using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        public ReferenceData()
        {
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
            
            foreach (ProjectBlockInfo projectBlockInfo in blkFld.readPlcBlocksList())
            {
                if (projectBlockInfo.BlockType == PLCBlockType.S5_PB || projectBlockInfo.BlockType == PLCBlockType.S5_FB || projectBlockInfo.BlockType == PLCBlockType.S5_FX || projectBlockInfo.BlockType == PLCBlockType.S5_OB || projectBlockInfo.BlockType == PLCBlockType.S5_SB)
                {
                    S5FunctionBlock blk = (S5FunctionBlock) projectBlockInfo.GetBlock();
                    int networkNR = 0;                    
                    foreach (Network network in blk.Networks)
                    {
                        int rowNR = 0;
                        networkNR++;
                        foreach (S5FunctionBlockRow functionBlockRow in network.AWLCode)
                        {
                            rowNR++;
                            if (functionBlockRow.MC5LIB_SYMTAB_Row != null && ((ReferenceDataAccessMode)functionBlockRow.MC5LIB_SYMTAB_Row[9]) != ReferenceDataAccessMode.None)
                            {
                                string operand = functionBlockRow.Parameter;
                                ReferenceDataEntry entr;
                                operandIndexList.TryGetValue(operand, out entr);
                                if (entr == null)
                                {
                                    entr = new ReferenceDataEntry() {Operand = operand, SymbolTableEntry = smyTab.GetEntryFromOperand(operand)};
                                    operandIndexList.Add(operand, entr);
                                    _ReferenceDataEntrys.Add(entr);
                                }

                                entr.ReferencePoints.Add(new ReferencePoint() { Block = blk,Network = network,NetworkNumber = networkNR, LineNumber = rowNR, BlockRow = functionBlockRow, AccessMode = (ReferenceDataAccessMode)functionBlockRow.MC5LIB_SYMTAB_Row[9] });
                            }
                        }
                    }
                    

                }
            }

            ReferenceDataLoaded = true;
        }                
    }
}
