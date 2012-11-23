using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    internal static class JumpMarks
    {
        public static int[] GetJumpadresses(byte[] BD)
        {
            if (BD != null)
            {
                if (BD.Length == 0)
                    return null;
                int i;

                int nwanz = BitConverter.ToUInt16(BD, 0);
                int startJump = 2 + 2 * nwanz;

                int jumpAnz = BitConverter.ToUInt16(BD, startJump);

                int[] JumpPos = new int[jumpAnz];
                for (i = 0; i < jumpAnz; i++ )
                {
                    JumpPos[i] = BitConverter.ToUInt16(BD, startJump+2 + (i * 2)); // Helper.GetWord(BD[Start + (i * 2) + 1], BD[Start + (i * 2)]);
                }

                return JumpPos;
            }
            return null;
        }

        public static List<FunctionBlockRow> AddJumpmarks(List<FunctionBlockRow> myBlk, byte[] JumpMarks, byte[] JumpPosAndNWInfos )
        {
            int NetworkCount = 0;
            foreach (S7FunctionBlockRow plcFunctionBlockRow in myBlk)
            {
                if (Helper.IsNetwork(plcFunctionBlockRow))
                    NetworkCount++;   
            }


            int[] jpAddr = GetJumpadresses(JumpPosAndNWInfos);
            //Use the Jump-Marks from the Step7 project....
            List<string> jumpNames = new List<string>();
            int cntJ = 0;

            string aa = "";

            if (JumpMarks != null && JumpMarks.Length >= 5)
            {
                int anzJ = BitConverter.ToInt16(JumpMarks, NetworkCount * 2);  //Todo: find out why it does not always contain the right amount of JumpNames

                for (int n = NetworkCount * 2 + 4; n < JumpMarks.Length; n += 5)
                {
                    //Todo: Sometimes JumeMarks are seperated by 0x00 and somtimes by 0xD0 look what this means.
                    string aknm = "";
                    if (JumpMarks[n] != 0 && JumpMarks.Length >= n + 3)
                    {
                        aknm += ((char)JumpMarks[n]);
                        if (JumpMarks[n + 1] != 0)
                        {
                            aknm += ((char)JumpMarks[n + 1]);
                            if (JumpMarks[n + 2] != 0)
                            {
                                aknm += ((char)JumpMarks[n + 2]);
                                if (JumpMarks[n + 3] != 0) aknm += ((char)JumpMarks[n + 3]);
                            }
                        }
                    }

                    jumpNames.Add(aknm);

                }                               
            }


            Dictionary<string, string> ChangeLabelList = new Dictionary<string, string>();
            int pos = 0;
            foreach (S7FunctionBlockRow plcFunctionBlockRow in myBlk)
            {

                if (Helper.IsJump(plcFunctionBlockRow, 0))
                {
                    int arrPos = Array.IndexOf<int>(jpAddr, pos/2);
                    if (arrPos != -1)
                    {
                        if (jumpNames.Count > arrPos)
                        {
                            if (!ChangeLabelList.ContainsKey(plcFunctionBlockRow.Parameter))
                                ChangeLabelList.Add(plcFunctionBlockRow.Parameter, jumpNames[arrPos]);
                            byte[] backup = plcFunctionBlockRow.MC7;
                            plcFunctionBlockRow.Parameter = jumpNames[arrPos];
                            plcFunctionBlockRow.MC7 = backup;
                        }
                    }
                }
                pos += plcFunctionBlockRow.ByteSize;
            }

            foreach (S7FunctionBlockRow plcFunctionBlockRow in myBlk)
            {
                if (ChangeLabelList.ContainsKey(plcFunctionBlockRow.Label))
                    plcFunctionBlockRow.Label = ChangeLabelList[plcFunctionBlockRow.Label];
            }

            return myBlk;
        }
    }
}
