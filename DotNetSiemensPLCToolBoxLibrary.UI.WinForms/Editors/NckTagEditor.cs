using System;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    public partial class NckTagEditor : Form
    {
        public static PLCNckTag ShowPLCTagEditor(PLCNckTag tag)
        {
            NckTagEditor tagedt = new NckTagEditor();
            tagedt._libnodavevalue = tag;
            tagedt.ShowDialog();
            return tagedt._libnodavevalue;
        }

        internal NckTagEditor()
        {
            //_libnodavevalue = value;
            InitializeComponent();
        }

        private C_PropertyNck pNCK = new C_PropertyNck();
        private PLCNckTag _libnodavevalue;
        private bool startWasNull = false;

        private void LibNoDaveValueEditor_Load(object sender, EventArgs e)
        {
            if (_libnodavevalue != null)
            {
                pNCK.SYNTAX_ID = 0x82;
                pNCK.bereich_u_einheit = (byte)(_libnodavevalue.NckArea << 5 | _libnodavevalue.NckUnit);
                pNCK.spalte = (uint)_libnodavevalue.NckColumn;
                pNCK.zeile = (uint)_libnodavevalue.NckLine;
                pNCK.bausteintyp = (byte)_libnodavevalue.NckModule;
                pNCK.ZEILENANZAHL = (byte)_libnodavevalue.NckLinecount;
                pNCK.typ = getType(_libnodavevalue.TagDataType);
                pNCK.laenge = (byte)_libnodavevalue.ReadByteSize;
            }
            else
            {
                startWasNull = true;
                _libnodavevalue = new PLCNckTag();
            }

            pGridNCK.SelectedObject = pNCK;
        }

        private byte getType(TagDataType type)
        {
            switch (type)
            {
                case TagDataType.Bool:
                    return 1;
                case TagDataType.Byte:
                    return 3;
                case TagDataType.Word:
                    return 4;
                case TagDataType.Int:
                    return 5;
                case TagDataType.Dword:
                    return 6;
                case TagDataType.Dint:
                    return 7;
                case TagDataType.Float:
                    return 8;
                case TagDataType.LReal:
                    return 15;
                case TagDataType.LInt:
                    return 18;
                case TagDataType.String:
                case TagDataType.CharArray:
                    return 19;
                default:
                    return 0;
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            byte _bereich = (byte)((pNCK.bereich_u_einheit & 0xE0) >> 5);         // (bereich_u_einheit & 2#11100000) schiebe rechts um 5 Bit
            byte _einheit = (byte)(pNCK.bereich_u_einheit & 0x1F);                // & 2#00011111

            #region TYP
            TagDataType dataType = new TagDataType();
            int _ArraySize = 0;
            switch (pNCK.typ)
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
                    if (_bereich == 2)
                        dataType = TagDataType.String; //eNCK_LE_String;
                    else
                        dataType = TagDataType.CharArray; //eNCK_LE_String;
                    _ArraySize = pNCK.laenge;
                    break;
                default:
                    throw new Exception("Unknown Type");
            }
            #endregion

            _libnodavevalue = new PLCNckTag() { TagDataType = dataType, NckArea = _bereich, NckUnit = _einheit, NckColumn = (int)pNCK.spalte, NckLine = (int)pNCK.zeile, NckModule = pNCK.bausteintyp, NckLinecount = pNCK.ZEILENANZAHL, ArraySize = _ArraySize };

            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (startWasNull)
                _libnodavevalue = null;
            this.Close();
        }
    }
}
