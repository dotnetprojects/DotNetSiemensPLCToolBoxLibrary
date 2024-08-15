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

using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    [Serializable()]
    public class S7Block : Block
    {
        internal S7ConvertingOptions usedS7ConvertingOptions;

        /// <summary>
        /// The blocks version
        /// </summary>
        /// <remarks>This field does not correspond to the Version field in Simatic Manager</remarks>
        public string BlockVersion;

        /// <summary>
        /// The Block Attributes that contain information about the block status and special properties
        /// </summary>
        public S7BlockAtributes BlockAttribute { get; set; } 

        /// <summary>
        /// The Block Attributes defined from the Simatic Manager in Attributes Tab
        /// </summary>
        public List<Step7Attribute> Attributes { get; set; }

        /// <summary>
        /// The total lenght of the Block. Correspnds to the "Load Memory Requirement" in Simatic Manager
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// The Title of the Block from the S7 Project file. it is not the online name of the block. 
        /// </summary>
        /// <remarks>The Header name of an online block is in the "Name" field</remarks>
        public string Title { get; set; }

        /// <summary>
        /// The Author of the Block
        /// </summary>
        /// <remarks>Limited to 8 chars</remarks>
        public string Author { get; set; }

        /// <summary>
        /// The Family of the Block
        /// </summary>
        /// <remarks>Limited to 8 chars</remarks>
        public string Family { get; set; }

        /// <summary>
        /// The version of the Block
        /// </summary>
        /// <remarks>Limited from 0.0 to 9.9</remarks>
        public string Version { get; set; }

        /// <summary>
        /// Timestamp of the last change to the blocks MC7 code
        /// </summary>
        public DateTime LastCodeChange { get; set; }
        
        /// <summary>
        /// Timestamp of the last change to the interface of the blocks
        /// </summary>
        public DateTime LastInterfaceChange { get; set; }

        /// <summary>
        /// Timestamp of the last interface change associated with the plaintext database record
        /// If a block is uploaded to the project with a timestamp conflict, this field contains the previous modified date of the interface before the upload
        /// </summary>
        public DateTime LastInterfaceChangeHistory { get; set; }

        /// <summary>
        /// The total size of the Interface table 
        /// </summary>
        /// <remarks>this is an internal property, that is not shown in Simatic Manager</remarks>
        public int InterfaceSize { get; set; }

        /// <summary>
        /// The total size of the Segement table in the header
        /// </summary>
        /// <remarks>this is an internal property, that is not shown in Simatic Manager</remarks>
        public int SegmentTableSize { get; set; }

        /// <summary>
        /// The size of the local Temp data stack. Only aplicable to OB, FC or FB blocks
        /// </summary>
        public int LocalDataSize { get; set; }

        /// <summary>
        /// The actual MC7 code size of the block. This corresonds to the "MC7" field in Simatic Manager
        /// </summary>
        public int CodeSize { get; set; }

        /// <summary>
        /// the Work memory requirement from Simatic Manager
        /// </summary>
        public int WorkMemorySize { get; set;}
        
        /// <summary>
        /// The block has currently set an Password
        /// </summary>
        /// <remarks>be aware that the block can also be be marked as protected via the "BlockAttributes"</remarks>
        public bool KnowHowProtection { get; set; }
        
        /// <summary>
        /// The checksum of the Blocks MC7 code (without the actual values of Datablocks)
        /// This property can be used to detect Block changes
        /// </summary>
        public int CheckSum { get; set; }

        public virtual string GetSourceBlock(bool useSymbols = false)
        {
            return null;
        }

        private byte[] _password;

        public byte[] Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = new byte[4];
                if (value.Length >= 1) _password[0] = value[0];
                if (value.Length >= 2) _password[1] = value[1];
                if (value.Length >= 3) _password[2] = value[2];
                if (value.Length >= 4) _password[3] = value[3];
            }
        }

        public override SymbolTableEntry SymbolTableEntry
        {
            get
            {
                if (ParentFolder != null)
                {
                    ISymbolTable tmp = ((IProgrammFolder)ParentFolder.Parent).SymbolTable;
                    if (tmp != null)
                        return tmp.GetEntryFromOperand(BlockName);
                }
                return null;
            }
        }

        [Flags]
        public enum S7BlockAtributes: byte
        {
            /// <summary>
            /// The block exists in the controller, and is also linked into execution.
            /// if this attribute is FALSE:
            /// -For Code blocks such as FB or FC, this means that they are existing in the controller but not actually executed
            /// -For data blocks this means, that they do not have any Actual values assigned to them. Any attempt to read current data from them will fail.
            /// </summary>
            /// <remarks>
            /// This corresponds to the "Unlinked" attribute in the Simatic manager, which actually shows the status in reverse
            /// This Attribute is only false when either especifically selected from simatic manager (only possible for Datablocks)
            /// or during a breif period when an Code block is alrady downloaded, but not yet linked (usually part of the "Block Download" process
            /// </remarks>
            Linked = 1, //.0

            /// <summary>
            /// This is an standard block from the default library
            /// </summary>
            StandardBlock = 2, //.1

            /// <summary>
            /// The block is protected by an Password
            /// </summary>
            KnowHowProtected = 8, //.3

            /// <summary>
            /// Only applies to datablocks. if an DB is non retentive, its actual data get reset to its initial values every time the controller
            /// restarts
            /// </summary>
            NonRetain = 32, //.5

            /// <summary>
            /// This is an Safety Block in an Safety PLC. 
            /// </summary>
            FBlock = 64 //.6

            //These two Attributes somehow do not appear on online blocks, even though they are settabele in Simatic Manager
            //Maybe some more testing is necesary
            //WriteProtected
            //ReadOnly

        }

        /// <summary>
        /// Returns true if dt1 and dt2 are not equal
        /// Also checks that both values are not the default value (DateTime.MinValue)
        /// </summary>
        public static bool HasTimestampConflict(DateTime dt1, DateTime dt2)
        {
            if (!dt1.Equals(dt2))
            {
                return true;
            }

            return false;
        }
    }
}

