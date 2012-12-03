/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.

 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 *
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 WPFToolboxForSiemensPLCs is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 WPFToolboxForSiemensPLCs is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
*/
using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    /*
     * These functions to Convert from MC7 to AWL could not be done withou the help of Steffen Krayer !
     * Many of the Code is from him (From his Delphi Prog) and also he created Excel sheets wich decribe
     * how MC7 for most of the AWL Commands looks.
     */
    public static class MC7Converter
    {
        public static S7Block GetAWLBlock(byte[] MC7Code, int MnemoricLanguage)
        {
            return GetAWLBlock(MC7Code, MnemoricLanguage, null);
        }


        public static S7Block GetAWLBlock(byte[] MC7Code, int MnemoricLanguage, S7ProgrammFolder prjBlkFld)
		{

            /*
            string ttmp = "";
            for (int i = 0; i < MC7Code.Length; i++)
            {
                ttmp += MC7Code[i].ToString("X").PadLeft(2, '0');
            }
            MessageBox.Show(ttmp);
            Clipboard.SetText(ttmp);
            */

            S7Block retBlock = null;
            if (MC7Code != null)
            {
                if ((MC7Code[5] == 0x0a) || (MC7Code[5] == 0x0b))
                    retBlock = (S7Block) new S7DataBlock();
                else
                    retBlock = (S7Block)new S7FunctionBlock();

                const int MC7Start_or_DBBodyStart = 36;

                /*
                 * Description of a MC7 Block (Common)
                 *
                 * 0,1     = Signature ('pp')
                 * 2       = Block Version
                 * 3       = Block Attribute (.0 not unlinked, .1 standart block + know how protect, .3 know how protect, .5 not retain
                 * 4       = Block Language
                 * 5       = Block Type (a=DB, b=SDB)
                 * 6,7     = Block Number
                 * 8-11    = Block Length
                 * 12-15   = Block Password
                 * 16-21   = Last Modified
                 * 22-27   = Last Interface Change
                 * 28,29   = Interface length or DB Body (actual Values Part) length
                 * 30,31   = Segment Table Length?  (Normaly 0 on a DB) (Length of networks!)
                 * 32,33   = Local Data Length? (Normaly 0 on a DB)
                 * 34,35   = MC7-Length or DB Body (definitions/initial values)
                 */

                /*
                 * Description of a MC7 Block (Function - Block)
                 * 36-xx     = AWL
                 * xx+1,     = 0x01
                 * xx+2,xx+3 = Again Block number, Zero on OB            (but bytes swapped)
                 * xx+4,xx+5 = Interface Length (from xx+6 to yy)
                 * xx+6,xx+7 = Interface Blocks Count (In,Out,Satic,TMP etc) * 2
                 * xx+8,xx+9 = allways Zero
                 * xx+10-yy  = Interface
                 * yy+1-zz   = Networks
                 *
                 */

                /*
                 * Description of a MC7 Block (Data - Block)
                 * 36-xx   = AWL
                 * xx+1,     = 0x05 (DB) 0x10 (DI)
                 * xx+2,xx+3 = Again Block Number or FB Number on a DI   (but bytes swapped)
                 * xx+4,xx+5 = Interface Length 
                 * xx+6-yy   = Interface
                 * yy-zz   = Start-Values
                 * xx      = Nertworks
                 */

                retBlock.BlockVersion = Convert.ToString(MC7Code[2] - 1);
                retBlock.BlockAttribute = Convert.ToString(MC7Code[3] - 1);
                retBlock.BlockLanguage = (DataTypes.PLCLanguage) MC7Code[4]; // Enum.Parse(typeof(DataTypes.PLCLanguage), Helper.GetLang(MC7Code[4]));
                retBlock.MnemonicLanguage = (MnemonicLanguage)MnemoricLanguage;
                retBlock.BlockType = Helper.GetPLCBlockType(MC7Code[5]);
                retBlock.BlockNumber = (MC7Code[6]*0x100) + MC7Code[7];
                retBlock.Length = libnodave.getU32from(MC7Code, 8);
                retBlock.Password = new byte[] {MC7Code[12], MC7Code[13], MC7Code[14], MC7Code[15]};
                retBlock.LastCodeChange = Helper.GetDT(MC7Code[16], MC7Code[17], MC7Code[18], MC7Code[19], MC7Code[20], MC7Code[21]);
                retBlock.LastInterfaceChange = Helper.GetDT(MC7Code[22], MC7Code[23], MC7Code[24], MC7Code[25], MC7Code[26], MC7Code[27]);

                int InterfaceLength_or_DBActualValuesLength = libnodave.getU16from(MC7Code, 28);
                retBlock.SegmentTableSize = libnodave.getU16from(MC7Code, 30); //(Length of networks?)
                int LocalDataLength = libnodave.getU16from(MC7Code, 32);
                int MC7Length_or_DBBodyLength = libnodave.getU16from(MC7Code, 34);
                int IntfStart = MC7Start_or_DBBodyStart + MC7Length_or_DBBodyLength + 3;
                int IntfLength = BitConverter.ToUInt16(MC7Code, IntfStart) + 2;
                int IntfValStart = IntfStart + IntfLength + 2;

                retBlock.InterfaceSize = InterfaceLength_or_DBActualValuesLength;
                retBlock.LocalDataSize = LocalDataLength;
                retBlock.CodeSize = MC7Length_or_DBBodyLength;

                int FooterStart = MC7Start_or_DBBodyStart + MC7Length_or_DBBodyLength + InterfaceLength_or_DBActualValuesLength + retBlock.SegmentTableSize;
                retBlock.Author = Helper.GetString(FooterStart + 0 , 8, MC7Code);
                retBlock.Family = Helper.GetString(FooterStart + 8, 8, MC7Code);
                retBlock.Name = Helper.GetString(FooterStart + 16, 8, MC7Code);
                retBlock.Version = Helper.GetVersion(MC7Code[FooterStart + 24]);

                if ((MC7Code[5] == 0x0a) || (MC7Code[5] == 0x0b))
                {
                    //Instance DB??
                    if (MC7Code[MC7Start_or_DBBodyStart + MC7Length_or_DBBodyLength] == 0x0a)
                    {
                        ((S7DataBlock) retBlock).IsInstanceDB = true;
                        ((S7DataBlock)retBlock).FBNumber = BitConverter.ToUInt16(MC7Code, MC7Start_or_DBBodyStart + MC7Length_or_DBBodyLength + 1);
                    }
                    //((PLCDataBlock) retBlock).Structure = MC7toDB.GetDBInterface(IntfStart, IntfLength, AWLStart, IntfValStart, MC7Code);

                    /*var interf = new byte[IntfLength];
                    Array.Copy(MC7Code, IntfStart, interf, 0, IntfLength);
                    string wr = "";
                    for (int i=0;i<interf.Length-1;i=i+2)
                    {
                        wr += interf[i+1].ToString("X").PadLeft(2, '0');
                        wr += interf[i].ToString("X").PadLeft(2, '0');
                    }
                    wr = wr;*/
                    List<string> tmp = new List<string>();
                    var interfaceBytes = new byte[IntfLength + 3];
                    var actualValues = new byte[MC7Length_or_DBBodyLength];
                    Array.Copy(MC7Code, IntfStart - 3, interfaceBytes, 0, IntfLength + 3); //-3 because of in the project file in the structere ssbpart is also the same structure with this 4 bytes!!
                    Array.Copy(MC7Code, MC7Start_or_DBBodyStart, actualValues, 0, MC7Length_or_DBBodyLength);

                    ((S7DataBlock)retBlock).StructureFromMC7 = Parameter.GetInterface(interfaceBytes, actualValues, ref tmp, retBlock.BlockType, ((S7DataBlock)retBlock).IsInstanceDB, retBlock);

                }
                else
                {
                    var interfaceBytes = new byte[IntfLength + 3];
                    Array.Copy(MC7Code, IntfStart - 3, interfaceBytes, 0, IntfLength + 3); //-3 because of in the project file in the structere ssbpart is also the same structure with this 4 bytes!!
                    
                    List<string> ParaList = new List<string>();
                    ((S7FunctionBlock)retBlock).Parameter = Parameter.GetInterface(interfaceBytes, null, ref ParaList, retBlock.BlockType, false, retBlock);

                    int[] Networks;
                    Networks = NetWork.GetNetworks(MC7Start_or_DBBodyStart + MC7Length_or_DBBodyLength + InterfaceLength_or_DBActualValuesLength, MC7Code);
                    ((S7FunctionBlock) retBlock).AWLCode = MC7toAWL.GetAWL(MC7Start_or_DBBodyStart, MC7Length_or_DBBodyLength - 2, MnemoricLanguage, MC7Code, Networks, ParaList, prjBlkFld);

                    ((S7FunctionBlock) retBlock).Networks = NetWork.GetNetworksList((S7FunctionBlock) retBlock);
                }


            }
            return retBlock;
		}

        static public byte[] GetMC7Block(S7Block myBlock)
        {
            byte[] retByte = AWLtoMC7.GetMC7(myBlock);

            return retByte;
        }

    }
}
