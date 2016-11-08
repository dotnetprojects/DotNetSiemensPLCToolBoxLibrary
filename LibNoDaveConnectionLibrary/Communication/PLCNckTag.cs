using DotNetSiemensPLCToolBoxLibrary.DataTypes;
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



namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
#if !IPHONE
    [System.ComponentModel.Editor(typeof(NckTagUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
	[Serializable]
	public class PLCNckTag : PLCTag
	{
		public PLCNckTag()
		{
		}

		public int NckArea { get; set; }
		public int NckUnit { get; set; }
		public int NckColumn { get; set; }
		public int NckLine { get; set; }
		public int NckModule { get; set; }
		public int NckLinecount { get; set; }
		public override  bool DontSplitValue
		{
			get { return true; }
			set { }
		}


        public override string ToString()
        {
            string old = "";
            if (_oldvalues != null)
            {
                old = "   -- Old-Values: ";
                foreach (var oldvalue in _oldvalues)
                {
                    old += oldvalue.ToString() + ",";
                }
                old += "";
            }

            string s = string.Format("0x{0},0x{1},0x{2},0x{3},0x{4},0x{5},{6},0x{7}", NckArea.ToString("X"), NckUnit.ToString("X"), NckColumn.ToString("X"), NckLine.ToString("X"), NckModule.ToString("X"), NckLinecount.ToString("X"), TagDataType, _internalGetSize().ToString("X"));

            if (Value != null)
            {
                return s + " = " + GetValueAsString() + old;
            }
            return s;
        }

        //Todo: look how long a NCK Request is???
        //internal override int _internalGetSize()
        //{
        //	return 1;
        //}
	}

    public class NC_Var
    {
        public NC_Var()
        {
        }
        public NC_Var(string ncVarSelector)
        {
            throw new NotImplementedException();
            //bereich_u_einheit = 0x40;
            //spalte = 0x78;
            //zeile = 0x1;
            //bausteintyp = 0x7F;
            //ZEILENANZAHL = 0x1;
            //typ = 0xF;
            //laenge = 0x8;
        }

        public NC_Var(int syntaxId, int bereich_u_einheit, int spalte, int zeile, int bausteinTyp, int zeilenAnzahl, int typ, int laenge)
        {
            this.SYNTAX_ID = (byte)syntaxId;
            this.Bereich_u_einheit = (byte)bereich_u_einheit;
            this.Spalte = (UInt16)spalte;
            this.Zeile = (UInt16)zeile;
            this.Bausteintyp = (byte)bausteinTyp;
            this.ZEILENANZAHL = (byte)zeilenAnzahl;
            this.Typ = (byte)typ;
            this.Laenge = (byte)laenge;
        }

        public byte SYNTAX_ID;
        public byte Bereich_u_einheit;
        public UInt16 Spalte;
        public UInt16 Zeile;
        public byte Bausteintyp;
        public byte ZEILENANZAHL;
        public byte Typ;
        public byte Laenge;

        public PLCNckTag GetNckTag(int unit, int rowOffset)
        {
            //byte SYNTAX_ID = 0x82;
            byte bereich_u_einheit = (byte)(this.Bereich_u_einheit + unit);
            byte _bereich = (byte)((bereich_u_einheit & 0xE0) >> 5);         // (bereich_u_einheit & 2#11100000) schiebe rechts um 5 Bit
            byte _einheit = (byte)(bereich_u_einheit & 0x1F);                // & 2#00011111

            #region TYP
            TagDataType dataType = new TagDataType();
            int _ArraySize = 0;
            switch (this.Typ)
            {
                case 1:
                    dataType = TagDataType.Bool;
                    break;
                case 3:
                    dataType = TagDataType.Byte; //eNCK_LE_Int8;
                    break;
                case 4:
                    dataType = TagDataType.Word;
                    break;
                case 5:
                    dataType = TagDataType.Int; //eNCK_LE_Int16;
                    break;
                case 6:
                    dataType = TagDataType.Dword; //eNCK_LE_Uint32;
                    break;
                case 7:
                    dataType = TagDataType.Dint; //eNCK_LE_Int32;
                    break;
                case 8:
                    dataType = TagDataType.Float; //eNCK_LE_Float32;
                    break;
                case 15:
                    dataType = TagDataType.LReal; //eNCK_LE_Float64;
                    break;
                case 18:
                    dataType = TagDataType.LInt; //eNCK_LE_Int64;
                    break;
                case 19:
                    //if (_bereich == 2)// && NC_Var.bausteintyp == 0x7f)
                    //    dataType = TagDataType.String; //eNCK_LE_String;
                    //else
                        dataType = TagDataType.CharArray; //eNCK_LE_String;

                    _ArraySize = this.Laenge;
                    break;
                default:
                    throw new Exception("Unknown Type");
                    break;
            }
            #endregion

            return new PLCNckTag() { TagDataType = dataType, NckArea = _bereich, NckUnit = _einheit, NckColumn = (int)this.Spalte, NckLine = (int)this.Zeile + rowOffset, NckModule = this.Bausteintyp, NckLinecount = this.ZEILENANZAHL, ArraySize = _ArraySize };
        }


    }

}
