using System;
using System.Collections.Generic;
using System.IO;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public class Step5Project : Project
    {
        //When a Zip File is used, here is the s5d name!
        internal string _projectfilename;

        internal bool _showDeleted = false;

        //Zipfile is used as Object, because SharpZipLib is not available on every platform!
        internal ZipHelper _ziphelper = new ZipHelper(null);

        public Step5BlocksFolder BlocksFolder { get; set; }

        public Step5Project(string filename, bool showDeleted)
        {
            _showDeleted = showDeleted;

            _projectfilename = filename;

            if (filename.ToLower().EndsWith("zip"))
            {
                _projectfilename = ZipHelper.GetFirstZipEntryWithEnding(filename, ".s5d");
                if (string.IsNullOrEmpty(_projectfilename))
                    throw new Exception("Zip-File contains no valid Step5 Project !");
                this._ziphelper = new ZipHelper(filename);
            }

            ProjectFile = filename;

            LoadProject();
        }

        internal byte[] s5ProjectByteArray;

        public int Size { get; set; }

        public override ProjectType ProjectType
        {
            get { return ProjectType.Step5; }
        }

        protected override void LoadProject()
        {
            _projectLoaded = true;

            //Read Step5 Project into a Byte-Array
            Stream fsProject = _ziphelper.GetReadStream(_projectfilename);
            s5ProjectByteArray = new byte[_ziphelper.GetStreamLength(_projectfilename, fsProject)];
            fsProject.Read(s5ProjectByteArray, 0, s5ProjectByteArray.Length);
            fsProject.Close();

            //Read the Project Name
            ProjectName = System.Text.Encoding.UTF7.GetString(s5ProjectByteArray, 0x08, 8);

            //Read the Project Size
            Size = s5ProjectByteArray[0x14] + s5ProjectByteArray[0x15]*0x100;

            //Create the main Project Folder
            ProjectStructure = new Step5ProgrammFolder() {Project = this, Name = this.ToString()};
            _allFolders.Add(ProjectStructure);

            //int startpos = s5ProjectByteArray[0x12] * 0x80;

            int anz_sections = s5ProjectByteArray[0x16];

            List<int> sections_lst = new List<int>();

            for (int j = 0; j < anz_sections; j++)
            {
                int pos = 0x44 + j*19;
                sections_lst.Add(s5ProjectByteArray[pos + 15] + s5ProjectByteArray[pos + 16]*0x100);
            }


            Step5BlocksFolder blkFld = new Step5BlocksFolder() {Name = "Blocks", Project = this, Parent = ProjectStructure};
            BlocksFolder = blkFld;
            ProjectStructure.SubItems.Add(blkFld);

            //int section_start = startpos;

            int n = 0;

            List<int> ByteAddressOFExistingBlocks = new List<int>();

            foreach (int secpos in sections_lst)
            {
                int section_start = secpos*0x80;
                /* The len for a Section is not always calculated right, so if the Section does not begin with the filename add 0x80 until it works */
                /* But I don't know why it's wrong */

                /*
                while (System.Text.Encoding.UTF7.GetString(s5ProjectByteArray, section_start, 8) != ProjectName)
                {
                    section_start += 0x80;
                    if (section_start>=s5ProjectByteArray.Length)
                        break;
                }
                */

                if (section_start >= s5ProjectByteArray.Length)
                    break;

                //if (section_start == 0x1580)  //only for debbuging
                //    section_start = section_start;

                //Don't know what this Byte means, maybe it describes wich Blocks are in the Section?
                int section_header_type = s5ProjectByteArray[section_start + 8];

                int akanz = 0;
                int anzbst = s5ProjectByteArray[section_start + 22];

                List<byte[]> bstHeaders = new List<byte[]>();
                for (int j = 0; j < anzbst; j++)
                {
                    byte[] tmp = new byte[15];
                    Array.Copy(s5ProjectByteArray, section_start + 68 + j*15, tmp, 0, 15);
                    bstHeaders.Add(tmp);
                }


                //int section_size = ((s5ProjectByteArray[section_start + 21] * 0x100) + s5ProjectByteArray[section_start + 20]) * 0x80;

                //if (section_size == 0)   //only for debbuging
                //    section_size = section_size;

                /*
                int section_header_size = anzbst * 15 + 68;
                section_header_size = (section_header_size / 0x80) * 0x80;
                if ((anzbst * 15 + 68) % 0x80 != 0)
                    section_header_size += 0x80;
                */

                //Don't know wich Information is in the Section Header!
                int section_header_size = s5ProjectByteArray[section_start + 18]*0x80;



                //Read the Block normaly (using the Section-Headers)
                {
                    //for (int n = blkstart + blkheadersize; n < blkstart + blksize /* s5ProjectByteArray.Length - 2 */; n++)
                    n = section_start + section_header_size;
                    //while (s5ProjectByteArray[n] == 0x00 && s5ProjectByteArray[n + 1] == 0x00)
                    //    n += 0x80;

                    while (akanz < anzbst && n + 1 < s5ProjectByteArray.Length)
                        //n < section_start + section_size)                       
                    {
                        akanz++;
                        int len = 0;

                        ByteAddressOFExistingBlocks.Add(n);
                        var tmp = AddBlockInfo(s5ProjectByteArray, ref n, blkFld,
                                               bstHeaders[akanz - 1]);
                        if (tmp != null)
                            blkFld.step5BlocksinfoList.Add(tmp);

                        /*
                        if (s5ProjectByteArray[n] == 0x70 && s5ProjectByteArray[n + 1] == 0x70) //Step5 Block
                        // && s5ProjectByteArray[n - 1] == 0x00)
                        {
                            len = (s5ProjectByteArray[n + 8] * 0x100 + s5ProjectByteArray[n + 9]) * 2;

                            Step5ProjectBlockInfo tmpBlk = new Step5ProjectBlockInfo();
                            //S5Block tmpBlk = new S5Block();

                            //if (((IList<int>)Enum.GetValues(typeof(PLCBlockType))).Contains((s5ProjectByteArray[n + 2] | 0xf00))) //only for debbuging
                            tmpBlk.BlockType = (PLCBlockType)(s5ProjectByteArray[n + 2] | 0xf00);
                            //else
                            //    tt_nr = tt_nr;

                            tmpBlk.BlockNumber = s5ProjectByteArray[n + 3];

                            tmpBlk._blkHeaderByte = bstHeaders[akanz - 1];

                            //byte n+4 -> kennungen für das programiergerät
                            //byte n+5,6,7 -> bib nummer

                            byte[] code = new byte[len];
                            Array.Copy(s5ProjectByteArray, n, code, 0, len);
                            tmpBlk._blkByte = code;
                            tmpBlk.ParentFolder = blkFld;

                            blkFld.step5BlocksinfoList.Add(tmpBlk);
                            //string aa = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(code);                        
                        }
                        else if (s5ProjectByteArray[n] == 0x06) //DB - Vorkopf
                        {
                            len = (s5ProjectByteArray[n + 4] * 0x100 + s5ProjectByteArray[n + 5]) * 2;
                            Step5ProjectBlockInfo tmpBlk = new Step5ProjectBlockInfo();
                            //S5Block tmpBlk = new S5Block();

                            tmpBlk.BlockType = PLCBlockType.S5_DV;

                            tmpBlk.BlockNumber = s5ProjectByteArray[n + 1];

                            byte[] code = new byte[len];
                            Array.Copy(s5ProjectByteArray, n, code, 0, len);
                            tmpBlk._blkByte = code;
                            tmpBlk.ParentFolder = blkFld;

                            blkFld.step5BlocksinfoList.Add(tmpBlk);
                        }
                        else if (s5ProjectByteArray[n] == 0x11) //DX - Vorkopf
                        {
                            len = (s5ProjectByteArray[n + 4] * 0x100 + s5ProjectByteArray[n + 5]) * 2;
                            Step5ProjectBlockInfo tmpBlk = new Step5ProjectBlockInfo();
                            //S5Block tmpBlk = new S5Block();

                            tmpBlk.BlockType = PLCBlockType.S5_DVX;

                            tmpBlk.BlockNumber = s5ProjectByteArray[n + 1];

                            byte[] code = new byte[len];
                            Array.Copy(s5ProjectByteArray, n, code, 0, len);
                            tmpBlk._blkByte = code;
                            tmpBlk.ParentFolder = blkFld;

                            blkFld.step5BlocksinfoList.Add(tmpBlk);
                        }
                        else if (s5ProjectByteArray[n] == 0x0D) //FB - Vorkopf
                        {
                            len = (s5ProjectByteArray[n + 4] * 0x100 + s5ProjectByteArray[n + 5]) * 2;
                            Step5ProjectBlockInfo tmpBlk = new Step5ProjectBlockInfo();
                            //S5Block tmpBlk = new S5Block();

                            tmpBlk.BlockType = PLCBlockType.S5_FV;

                            tmpBlk.BlockNumber = s5ProjectByteArray[n + 1];

                            byte[] code = new byte[len];
                            Array.Copy(s5ProjectByteArray, n, code, 0, len);
                            tmpBlk._blkByte = code;
                            tmpBlk.ParentFolder = blkFld;

                            blkFld.step5BlocksinfoList.Add(tmpBlk);
                        }
                        else if (s5ProjectByteArray[n] == 0x0A) //FX - Vorkopf
                        {
                            len = (s5ProjectByteArray[n + 4] * 0x100 + s5ProjectByteArray[n + 5]) * 2;
                            Step5ProjectBlockInfo tmpBlk = new Step5ProjectBlockInfo();
                            //S5Block tmpBlk = new S5Block();

                            tmpBlk.BlockType = PLCBlockType.S5_FVX;

                            tmpBlk.BlockNumber = s5ProjectByteArray[n + 1];

                            byte[] code = new byte[len];
                            Array.Copy(s5ProjectByteArray, n, code, 0, len);
                            tmpBlk._blkByte = code;
                            tmpBlk.ParentFolder = blkFld;

                            blkFld.step5BlocksinfoList.Add(tmpBlk);
                        }
                        else
                        {
                            //Here are the $ Blocks woch are not yet implemented!
                            //akanz--;
                            len = 0x80;
                        }

                        n += (len / 0x80) * 0x80;
                        if (len % 0x80 != 0)
                            n += 0x80;
                        */
                    }
                }
                section_start = n;
                //section_start += section_size;
            }

            if (_showDeleted)
            {
                //Read also the deleted Blocks, that means, don't use the Section Headers ...
                int akpos = s5ProjectByteArray[0x12]*0x80;

                while (akpos <= s5ProjectByteArray.Length - 0x80)
                {
                    while (!IsCurrentPosABlockStart(s5ProjectByteArray, akpos) &&
                           akpos <= s5ProjectByteArray.Length - 0x80)
                        akpos += 0x80;
                    
                    if (akpos <= s5ProjectByteArray.Length - 0x80)
                    {
                        bool blkExists = ByteAddressOFExistingBlocks.Contains(akpos);
                        var tmp=AddBlockInfo(s5ProjectByteArray, ref akpos, blkFld, null);
                        if (!blkExists)
                        {
                            tmp.Deleted = true;
                            blkFld.step5BlocksinfoList.Add(tmp);
                        }                        
                    }
                }
            }

            if (_projectfilename.ToLower().Contains("st.s5d") && _ziphelper.FileExists(_projectfilename.ToLower().Replace("st.s5d", "z0.seq")))
            {
                Stream symTabStream = _ziphelper.GetReadStream(_projectfilename.ToLower().Replace("st.s5d", "z0.seq"));

                SymbolTable symtab=new SymbolTable();
                symtab.LoadSymboltable(symTabStream);               
                symTabStream.Close();
                symtab.Parent = ProjectStructure;
                symtab.Project = this;
                ProjectStructure.SubItems.Add(symtab);
                _allFolders.Add(symtab);
            }

            var refFld = new ReferenceData((Step5ProgrammFolder) ProjectStructure, this);
            ProjectStructure.SubItems.Add(refFld); // { Parent = ProjectStructure, Project = this });
            _allFolders.Add(refFld);

        }

        private bool IsCurrentPosABlockStart(byte[] s5ProjectByteArray, int n)
        {
            if (s5ProjectByteArray[n] == 0x70 && s5ProjectByteArray[n + 1] == 0x70)
                return true;
            else if (s5ProjectByteArray[n] == 0x06) //DB - Vorkopf
                return true;
            else if (s5ProjectByteArray[n] == 0x11) //DX - Vorkopf
                return true;
            else if (s5ProjectByteArray[n] == 0x0D) //FB - Vorkopf
                return true;
            else if (s5ProjectByteArray[n] == 0x0A) //FX - Vorkopf
                return true;
            return false;
        }

        private S5ProjectBlockInfo AddBlockInfo(byte[] s5ProjectByteArray, ref int pos, Step5BlocksFolder blkFld, byte[] header)
        {
            int len = 0;
            S5ProjectBlockInfo tmpBlk = new S5ProjectBlockInfo();
            tmpBlk._blkHeaderByte = header;

            if (s5ProjectByteArray[pos] == 0x70 && s5ProjectByteArray[pos + 1] == 0x70) //Step5 Block
            // && s5ProjectByteArray[n - 1] == 0x00)
            {
                len = (s5ProjectByteArray[pos + 8] * 0x100 + s5ProjectByteArray[pos + 9]) * 2;

                tmpBlk.BlockType = (PLCBlockType)(s5ProjectByteArray[pos + 2] | 0xf00);

                tmpBlk.BlockNumber = s5ProjectByteArray[pos + 3];

                var bits = s5ProjectByteArray[pos + 4]; //siehe: https://www.yumpu.com/de/document/view/5702154/3-s5-power-bios-process-informatik-entwicklungsgesellschaft-mbh/26

                tmpBlk.Assembler = (bits & 0b00001000) > 0; // -> Assembler
                var bibNumber = s5ProjectByteArray[pos + 5] * 0x10000 + s5ProjectByteArray[pos + 6] * 0x100 + s5ProjectByteArray[pos + 7];

                //Console.WriteLine(tmpBlk.BlockType.ToString() + " - " + tmpBlk.BlockNumber + " - " + bibNumber);
                //byte n+4 -> kennungen für das programiergerät
                //byte n+5,6,7 -> bib nummer

                byte[] code = new byte[len];
                Array.Copy(s5ProjectByteArray, pos, code, 0, len);
                tmpBlk._blkByte = code;
                tmpBlk.ParentFolder = blkFld;

                //blkFld.step5BlocksinfoList.Add(tmpBlk);
                //string aa = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(code);                        
            }
            else if (s5ProjectByteArray[pos] == 0x06) //DB - Vorkopf
            {
                len = (s5ProjectByteArray[pos + 4] * 0x100 + s5ProjectByteArray[pos + 5]) * 2;

                tmpBlk.BlockType = PLCBlockType.S5_DV;

                tmpBlk.BlockNumber = s5ProjectByteArray[pos + 1];

                byte[] code = new byte[len];
                Array.Copy(s5ProjectByteArray, pos, code, 0, len);
                tmpBlk._blkByte = code;
                tmpBlk.ParentFolder = blkFld;

                //blkFld.step5BlocksinfoList.Add(tmpBlk);
            }
            else if (s5ProjectByteArray[pos] == 0x11) //DX - Vorkopf
            {
                len = (s5ProjectByteArray[pos + 4] * 0x100 + s5ProjectByteArray[pos + 5]) * 2;

                tmpBlk.BlockType = PLCBlockType.S5_DVX;

                tmpBlk.BlockNumber = s5ProjectByteArray[pos + 1];

                byte[] code = new byte[len];
                Array.Copy(s5ProjectByteArray, pos, code, 0, len);
                tmpBlk._blkByte = code;
                tmpBlk.ParentFolder = blkFld;

                //blkFld.step5BlocksinfoList.Add(tmpBlk);
            }
            else if (s5ProjectByteArray[pos] == 0x0D) //FB - Vorkopf
            {
                len = (s5ProjectByteArray[pos + 4] * 0x100 + s5ProjectByteArray[pos + 5]) * 2;

                tmpBlk.BlockType = PLCBlockType.S5_FV;

                tmpBlk.BlockNumber = s5ProjectByteArray[pos + 1];

                byte[] code = new byte[len];
                Array.Copy(s5ProjectByteArray, pos, code, 0, len);
                tmpBlk._blkByte = code;
                tmpBlk.ParentFolder = blkFld;

                //blkFld.step5BlocksinfoList.Add(tmpBlk);
            }
            else if (s5ProjectByteArray[pos] == 0x0A) //FX - Vorkopf
            {
                len = (s5ProjectByteArray[pos + 4] * 0x100 + s5ProjectByteArray[pos + 5]) * 2;

                tmpBlk.BlockType = PLCBlockType.S5_FVX;

                tmpBlk.BlockNumber = s5ProjectByteArray[pos + 1];

                byte[] code = new byte[len];
                Array.Copy(s5ProjectByteArray, pos, code, 0, len);
                tmpBlk._blkByte = code;
                tmpBlk.ParentFolder = blkFld;

                //blkFld.step5BlocksinfoList.Add(tmpBlk);
            }
            else if (s5ProjectByteArray[pos] == 0x6E) //?????
            {
                len = 0x80;
                tmpBlk = null;
            }
            else
            {
                //Here are the $ Blocks woch are not yet implemented!
                //akanz--;
                len = 0x80;
                tmpBlk = null;
            }

            if (len == 0) len = 0x80;

            pos += (len / 0x80) * 0x80;
            if (len % 0x80 != 0)
                pos += 0x80;

            return tmpBlk;
        }

        public override string ToString()
        {
            string retVal = base.ToString();
            if (_ziphelper.IsZipped())
                retVal += "(zipped)";
            if (_showDeleted == true)
                retVal += " (show deleted)";
            return retVal;            
        }
    }    
}
