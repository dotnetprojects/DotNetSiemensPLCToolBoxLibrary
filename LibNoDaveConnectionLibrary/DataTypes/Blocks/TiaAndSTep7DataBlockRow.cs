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
using System.ComponentModel;
using System.Globalization;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using DotNetSiemensPLCToolBoxLibrary.General;


namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public abstract class TiaAndSTep7DataBlockRow : DataBlockRow
    {
        public object StartValue { get; set; }  //Only for FB and DB not for FC
        public object Value { get; set; } //Only used in DBs
        public bool ReadOnly { get; set; }

        public void Add(IDataRow par)
        {
            if (ReadOnly) return;
            if (_children == null)
                _children = new List<IDataRow>();
            _children.Add(par);
            par.Parent = this;

            ClearBlockAddress();
        }

        public void AddRange(IEnumerable<IDataRow> par)
        {
            if (ReadOnly) return;
            if (_children == null)
                _children = new List<IDataRow>();
            _children.AddRange(par);
            foreach (IDataRow plcDataRow in par)
            {
                plcDataRow.Parent = this;
            }
            ClearBlockAddress();
        }

        public void Remove(S7DataRow par)
        {
            if (this._children != null)
            {
                if (ReadOnly) return;
                _children.Remove(par);
                if (_children.Count == 0)
                    this._children = null;
            }
            ClearBlockAddress();
        }

        public void Clear()
        {
            if (this._children != null)
            {
                this._children.Clear();
                this._children = null;
            }
        }


        internal List<IDataRow> _children;

        public override List<IDataRow> Children
        {
            get
            {
                if (_children == null)
                    return new List<IDataRow>();
                return _children;
            }
        }

        internal virtual void ClearBlockAddress()
        {

        }

        public bool isInOut { get; set; }

        protected ByteBitAddress _NextBlockAddress;
        public ByteBitAddress NextBlockAddress
        {
            get
            {
                if (_NextBlockAddress != null)
                    return new ByteBitAddress(_NextBlockAddress);
                else
                {
                    FillBlockAddresses(null);
                    return new ByteBitAddress(_NextBlockAddress);
                }
            }
        }

        internal abstract ByteBitAddress FillBlockAddresses(ByteBitAddress startAddr);
        //This Returns the Length in Bytes
        
        public virtual int ByteLength
        {
            get
            {
                int len = 0;
                switch (_datatype)
                {
                    case S7DataRowType.ANY:
                        len = 10;
                        break;
                    case S7DataRowType.UNKNOWN:
                        len = 0;
                        break;
                    case S7DataRowType.FB:
                    case S7DataRowType.SFB:
                    case S7DataRowType.STRUCT:
                    case S7DataRowType.UDT:
                        if (this.Parent != null && ((TiaAndSTep7DataBlockRow)this.Parent).isInOut)
                            return 6;   //On InOut -> It's handeled as Pointer
                        int size = 0;
                        if (Children != null)
                        {
                            if (Children.Count > 0)
                            {
                                ByteBitAddress tmp = ((TiaAndSTep7DataBlockRow)Children[Children.Count - 1]).NextBlockAddress;
                                if (tmp.BitAddress != 0)
                                    tmp.ByteAddress++;
                                if (tmp.ByteAddress % 2 != 0)
                                    tmp.ByteAddress++;
                                size = tmp.ByteAddress - ((TiaAndSTep7DataBlockRow)Children[0])._BlockAddress.ByteAddress;
                                if (IsArray)
                                {
                                    size *= this.GetArrayLines();
                                }
                            }
                        }
                        return size;
                    case S7DataRowType.BOOL:
                    case S7DataRowType.BYTE:
                    case S7DataRowType.CHAR:
                        len = 1;
                        break;
                    case S7DataRowType.TIME_OF_DAY:
                    case S7DataRowType.DINT:
                    case S7DataRowType.DWORD:
                    case S7DataRowType.TIME:
                    case S7DataRowType.REAL:
                        len = 4;
                        break;
                    case S7DataRowType.POINTER:
                        len = 6;
                        break;
                    case S7DataRowType.DATE_AND_TIME:
                        len = 8;
                        break;
                    case S7DataRowType.S5TIME:
                    case S7DataRowType.WORD:
                    case S7DataRowType.INT:
                    case S7DataRowType.BLOCK_DB:
                    case S7DataRowType.BLOCK_FB:
                    case S7DataRowType.BLOCK_FC:
                    case S7DataRowType.BLOCK_SDB:
                    case S7DataRowType.COUNTER:
                    case S7DataRowType.TIMER:
                    case S7DataRowType.DATE:
                        len = 2;
                        break;
                    case S7DataRowType.S5_KH:
                    case S7DataRowType.S5_KF:
                    case S7DataRowType.S5_KM:
                    case S7DataRowType.S5_A:
                    case S7DataRowType.S5_KT:
                    case S7DataRowType.S5_KZ:
                    case S7DataRowType.S5_KY:
                        len = 2;
                        break;
                    case S7DataRowType.S5_KG:
                        len = 4;
                        break;
                    case S7DataRowType.S5_KC:
                    case S7DataRowType.S5_C:
                        len = StringSize;
                        //len = 2;
                        break;
                    case S7DataRowType.STRING:
                        len = StringSize + 2;
                        break;
                }

                if (IsArray)
                {
                    if (_datatype == S7DataRowType.BOOL)
                    {
                        int bytesize = 0;
                        for (int n = ArrayStart.Count - 1; n >= 0; n--)
                        {
                            var start = ArrayStart[n];
                            var end = ArrayStop[n];

                            if (n == ArrayStart.Count - 1)
                            {
                                bytesize = ((end - start) + 8) / 8; //normal ((end - start) + 1); but that the division with 8 rounds down +7 to rund up!                                
                            }
                            else
                            {
                                bytesize = bytesize * ((end - start) + 1);
                            }
                        }

                        if (bytesize % 2 > 0) bytesize++;

                        return bytesize;

                        /*
                        int retInt = 0;
                        retInt =(len * this.GetArrayLines()) / 8;

                        if (this.GetArrayLines() < 4)
                            retInt++;

                        if ((len * this.GetArrayLines()) % 8 != 0) retInt++;
                        if ((len * this.GetArrayLines()) % 2 != 0) retInt++;
                        return retInt;*/
                    }
                    else
                    {
                        int retInt = (len * GetArrayLines());
                        return retInt + retInt % 2;
                    }
                }
                return len;
            }
        }
    }
}
