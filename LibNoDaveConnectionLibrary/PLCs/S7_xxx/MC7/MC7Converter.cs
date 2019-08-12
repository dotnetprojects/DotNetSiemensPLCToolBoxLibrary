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
    /// <summary>
    /// These functions to Convert from MC7 to AWL could not be done withou the help of Steffen Krayer !
    /// Many of the Code is from him (From his Delphi Prog) and also he created Excel sheets wich decribe
    /// how MC7 for most of the AWL Commands looks.
    /// </summary>
    /// <remarks>
    /// The general structure of an online block is as follows:
    /// 0  - 35 Block header 
    /// 36 - x  MC7 code for Code blocks; Current data for datablocks
    /// x  - x  Interface
    /// x  - x  Start Values (if any)
    /// x  - x  Segment table
    /// Len -36 Block footer. The footer is always located 36 bytes at the end of the block
    /// 
    /// all values x are depending on the blocks layout and apropiate length fields must be parsed from code
    /// </remarks>
    public static class MC7Converter
    {

        /// <summary>
        /// This value marks the lenght of the actual Block header, and since the MC7 code immeadiatly follows,
        /// also marks the start of the MC7 Code or Datablock body
        /// </summary>
        const int MC7Start_or_DBBodyStart = 36;

        /// <summary>
        /// This value marks the lenght of the actual Block footer which is always situatted at the end of the MC7 code data
        /// </summary>
        const int FooterLength = 36;

        /// <summary>
        /// Parse the Header and Footer information from an online MC7Code byte blob
        /// </summary>
        /// <param name="MC7Code">The online MC7 Code blob from the PLC</param>
        /// <param name="MnemoricLanguage">The Mnemoric that should be used when parsing the Blocks data</param>
        /// <returns></returns>
        internal static S7Block ParseBlockHeaderAndFooterFromMC7(byte[] MC7Code, MnemonicLanguage MnemoricLanguage)
        {
            S7Block retBlock = null;
            if (MC7Code != null)
            {
                //Parse Block type and prepare the Return block acordingly
                PLCBlockType BlockType = Helper.GetPLCBlockType(MC7Code[5]);

                if (BlockType == PLCBlockType.DB || BlockType == PLCBlockType.SDB) retBlock = (S7Block)new S7DataBlock();
                else retBlock = (S7Block)new S7FunctionBlock();

                /*
                 * Description of a MC7 Block Header
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
                 * 30,31   = Segment Table Length  (Normaly 0 on a DB) (Length of networks!)
                 * 32,33   = Local Data Length? (Normaly 0 on a DB)
                 * 34,35   = MC7-Length or DB Body (definitions/initial values)
                 */

                /*
                 * Description of a MC7 Block (Function - Block)
                 * 36-xx     = Header
                 * xx+1,     = 0x01
                 * xx+2,xx+3 = Again Block number, Zero on OB            (but bytes swapped)
                 * xx+4,xx+5 = Interface Length (from xx+6 to yy)
                 * xx+6,xx+7 = Interface Blocks Count (In,Out,Satic,TMP etc) * 2
                 * xx+9-yy   = Interface
                 * yy+1-zz   = Networks
                 *
                 */

                /*
                 * Description of a MC7 Block (Data - Block)
                 * 36-xx        = Header
                 * xx+1,        = 0x05 (DB) 0x10 (DI)
                 * xx+2,xx+3    = Again Block Number or FB Number on a DI   (but bytes swapped)
                 * xx+4,xx+5    = Interface Length
                 *xx+6,xx+7     = Interface Blocks Count (In,Out,Satic,TMP etc) * 2
                 * xx+8-yy      = Interface
                 * yy-zz        = Start-Values
                 */

                //----------------------------------------------------------------------------------------------------
                //Parse header data
                //----------------------------------------------------------------------------------------------------
                retBlock.BlockVersion = Convert.ToString(MC7Code[2] - 1);  //This is not the Block version from the Simatic Manager. It is unclar what data is stored in MC7Code[2]
                retBlock.BlockAttribute = (S7Block.S7BlockAtributes)MC7Code[3];
                retBlock.BlockLanguage = (PLCLanguage)MC7Code[4]; // Enum.Parse(typeof(DataTypes.PLCLanguage), Helper.GetLang(MC7Code[4]));
                retBlock.MnemonicLanguage = (MnemonicLanguage)MnemoricLanguage;
                retBlock.BlockType = Helper.GetPLCBlockType(MC7Code[5]);
                retBlock.BlockNumber = (MC7Code[6] * 0x100) + MC7Code[7];
                retBlock.Length = libnodave.getU32from(MC7Code, 8);
                retBlock.Password = new byte[] { MC7Code[12], MC7Code[13], MC7Code[14], MC7Code[15] };
                retBlock.KnowHowProtection = (MC7Code[12] + MC7Code[13] + MC7Code[14] + MC7Code[15]) != 0; //if any of the Password bytes contains an non Zero value
                retBlock.LastCodeChange = Helper.GetDT(MC7Code[16], MC7Code[17], MC7Code[18], MC7Code[19], MC7Code[20], MC7Code[21]);
                retBlock.LastInterfaceChange = Helper.GetDT(MC7Code[22], MC7Code[23], MC7Code[24], MC7Code[25], MC7Code[26], MC7Code[27]);

                //----------------------------------------------------------------------------------------------------
                //Parse Code and Data lengths
                //----------------------------------------------------------------------------------------------------
                int InterfaceLength_or_DBActualValuesLength = libnodave.getU16from(MC7Code, 28);
                retBlock.SegmentTableSize = libnodave.getU16from(MC7Code, 30); //(Length of networks?)
                int LocalDataLength = libnodave.getU16from(MC7Code, 32);
                int MC7Length_or_DBBodyLength = libnodave.getU16from(MC7Code, 34);
               
                retBlock.InterfaceSize = InterfaceLength_or_DBActualValuesLength;
                retBlock.LocalDataSize = LocalDataLength;
                retBlock.CodeSize = MC7Length_or_DBBodyLength;
                retBlock.WorkMemorySize = retBlock.CodeSize + MC7Start_or_DBBodyStart;  //in my tests(about 10 different blocks), this was alwasy 36 bytes more then the MC7 code size

                //----------------------------------------------------------------------------------------------------
                //Parse footer data
                //----------------------------------------------------------------------------------------------------
                //Old: int FooterStart = MC7Start_or_DBBodyStart + MC7Length_or_DBBodyLength + InterfaceLength_or_DBActualValuesLength + retBlock.SegmentTableSize;

                //Testing showed that the Footer always starts 36 bytes (footer lenght) before the end of the MC7 block.
                //so use this version, so that now there is no need for "GetAWLBlockBasicInfoFromBlockHeader" because its actually the same
                int FooterStart = MC7Code.Length - FooterLength;

                retBlock.Author = Helper.GetString(FooterStart + 0, 8, MC7Code);
                retBlock.Family = Helper.GetString(FooterStart + 8, 8, MC7Code);
                retBlock.Name = Helper.GetString(FooterStart + 16, 8, MC7Code);
                retBlock.Version = Helper.GetVersion(MC7Code[FooterStart + 24]);
                retBlock.CheckSum = libnodave.getU16from(MC7Code ,FooterStart + 26);

            }

            return retBlock;
        }

        [Obsolete("use GetAWLBlock override with proper Enumeration instead of Integer in MnemoricLanguage parameter")]
        public static S7Block GetAWLBlock(byte[] MC7Code, int MnemoricLanguage)
        {
            return GetAWLBlock(MC7Code, MnemoricLanguage, null);
        }

        [Obsolete ("use GetAWLBlock override with proper Enumeration instead of Integer in MnemoricLanguage parameter")]
        public static S7Block GetAWLBlock(byte[] MC7Code, int MnemoricLanguage, S7ProgrammFolder prjBlkFld)
        {
           return GetAWLBlock(MC7Code, (MnemonicLanguage)MnemoricLanguage, prjBlkFld);
        }

        public static S7Block GetAWLBlock(byte[] MC7Code, MnemonicLanguage MnemoricLanguage)
        {
            return GetAWLBlock(MC7Code, MnemoricLanguage, null);
        }

        public static S7Block GetAWLBlock(byte[] MC7Code, MnemonicLanguage MnemoricLanguage, S7ProgrammFolder prjBlkFld)
        {
            var retBlock = ParseBlockHeaderAndFooterFromMC7(MC7Code, MnemoricLanguage);


            if (retBlock != null)
            {

                int IntfStart = MC7Start_or_DBBodyStart + retBlock.CodeSize + 3;
                int IntfLength = BitConverter.ToUInt16(MC7Code, IntfStart) + 4;
                int InitialValStart = IntfStart + IntfLength;
                
                if (retBlock.BlockType == PLCBlockType.DB || retBlock.BlockType == PLCBlockType.SDB) //Block is an Data block or System Data block
                {
                    //Instance DB??
                    if (MC7Code[MC7Start_or_DBBodyStart + retBlock.CodeSize] == 0x0a)
                    {
                        ((S7DataBlock)retBlock).IsInstanceDB = true;
                        ((S7DataBlock)retBlock).FBNumber = BitConverter.ToUInt16(MC7Code, MC7Start_or_DBBodyStart + retBlock.CodeSize + 1);
                    }

                    List<string> tmp = new List<string>(); //to hold the temporary parameter list while parsing the Interface
                    var interfaceBytes = new byte[IntfLength + 3];
                    var actualValues = new byte[retBlock.CodeSize];

                    Array.Copy(MC7Code, IntfStart - 3, interfaceBytes, 0, IntfLength + 3); //-3 because of in the project file in the structere ssbpart is also the same structure with this 4 bytes!!
                    Array.Copy(MC7Code, MC7Start_or_DBBodyStart, actualValues, 0, retBlock.CodeSize);

                    //Some blocks do not have current values. These can be identified by checking the MC7 code length.
                    //the Interface Header is structured as follows
                    ///     0       Block Type: 0x05 (DB) 0x10 (DI),...
                    ///     1-2     Again Block Number or FB Number on a DI   (but bytes swapped)
                    ///     3-4     Interface Length minus this header (7 bytes)
                    ///     5-6     Start Value Length
                    byte[] startValues = null;
                    int startValueLength = BitConverter.ToUInt16(interfaceBytes, 5);

                    if (startValueLength > 0)
                    {
                        startValues = new byte[startValueLength];
                        Array.Copy(MC7Code, InitialValStart, startValues, 0, startValueLength);
                    }

                    //Parse the Interface from MC7 code
                    ((S7DataBlock)retBlock).StructureFromMC7 = Parameter.GetInterface(interfaceBytes, startValues, actualValues, ref tmp, retBlock.BlockType, ((S7DataBlock)retBlock).IsInstanceDB, retBlock);

                }
                else //Block is an code block (FB, FC or OB)
                {
                    var interfaceBytes = new byte[IntfLength + 3];
                    Array.Copy(MC7Code, IntfStart - 3, interfaceBytes, 0, IntfLength + 3); //-3 because of in the project file in the structere ssbpart is also the same structure with this 4 bytes!!

                    //Some blocks do not have current values. These can be identified by checking the MC7 code length.
                    //the Interface Header is structured as follows
                    ///     0       Block Type: 0x05 (DB) 0x10 (DI),...
                    ///     1-2     Again Block Number or FB Number on a DI   (but bytes swapped)
                    ///     3-4     Interface Length minus this header (7 bytes)
                    ///     5-6     Start Value Length
                    byte[] startValues = null;
                    int startValueLength = BitConverter.ToUInt16(interfaceBytes, 5);

                    if (retBlock.BlockType == PLCBlockType.FB && startValueLength > 0) //only FB's my have start values, even then they might not exist if none are defined (length = 0)
                    {
                        startValues = new byte[startValueLength];
                        Array.Copy(MC7Code, InitialValStart, startValues, 0, startValueLength);
                    }

                    List<string> ParaList = new List<string>();
                    ((S7FunctionBlock)retBlock).Parameter = Parameter.GetInterface(interfaceBytes, startValues, null /*there are never Current values in code blocks*/, ref ParaList, retBlock.BlockType, false, retBlock);

                    int[] Networks = null;

                    //Only if there are network descriptions. This generall only happens when the Block is empty and does not contain any code. 
                    //the Network list always starts after the Interface of the block and have the following format

                    if (retBlock.SegmentTableSize > 0)
                    {
                        Networks = NetWork.GetNetworks(MC7Start_or_DBBodyStart + retBlock.CodeSize + retBlock.InterfaceSize, MC7Code);
                    }
                    ((S7FunctionBlock)retBlock).AWLCode = MC7toAWL.GetAWL(MC7Start_or_DBBodyStart, retBlock.CodeSize - 2, (int)MnemoricLanguage, MC7Code, Networks, ParaList, prjBlkFld, (S7FunctionBlock)retBlock, ((S7FunctionBlock)retBlock).Parameter);

                    ((S7FunctionBlock)retBlock).Networks = NetWork.GetNetworksList((S7FunctionBlock)retBlock);
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
