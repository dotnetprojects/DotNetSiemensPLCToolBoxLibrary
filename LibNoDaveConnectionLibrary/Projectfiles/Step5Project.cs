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
        internal object _zipfile;

        public Step5Project(string filename)
        {
            _projectfilename = filename;

            if (filename.ToLower().EndsWith("zip"))
            {
                _projectfilename = ZipHelper.GetFirstZipEntryWithEnding(filename, ".s5d");
                if (string.IsNullOrEmpty(_projectfilename))
                    throw new Exception("Zip-File contains no valid Step5 Project !");
                this._zipfile = ZipHelper.OpenZipfile(filename);
            }

            ProjectFile = filename;

            LoadProject();
        }

        internal byte[] s5ProjectByteArray;

        public int Size { get; set; }

        internal override void LoadProject()
        {
            _projectLoaded = true;
         
            //Read Step5 Project into a Byte-Array
            Stream fsProject = ZipHelper.GetReadStream(_zipfile, _projectfilename);
            s5ProjectByteArray = new byte[ZipHelper.GetStreamLength(_zipfile, _projectfilename, fsProject)];
            fsProject.Read(s5ProjectByteArray, 0, s5ProjectByteArray.Length);
            fsProject.Close();

            //Read the Project Name
            ProjectName = System.Text.Encoding.UTF7.GetString(s5ProjectByteArray, 0x08, 8);

            //Read the Project Size
            Size = s5ProjectByteArray[0x14] + s5ProjectByteArray[0x15] * 0x100;

            //Create the main Project Folder
            ProjectStructure = new Step5ProjectFolder() { Project = this, Name = ProjectName };

            //int startpos = s5ProjectByteArray[0x12] * 0x80;

            int anz_sections = s5ProjectByteArray[0x16];

            List<int> sections_lst = new List<int>();

            for (int j = 0; j < anz_sections; j++)
            {
                int pos = 0x44 + j * 19;
                sections_lst.Add(s5ProjectByteArray[pos + 15] + s5ProjectByteArray[pos + 16] * 0x100);
            }

            Step5BlocksFolder blkFld = new Step5BlocksFolder() { Name = "Blocks", Project = this, Parent = ProjectStructure};
            ProjectStructure.SubItems.Add(blkFld);

            //int section_start = startpos;

            int n = 0;

            foreach (int secpos in sections_lst)

            //while (section_start < s5ProjectByteArray.Length)
            {
                int section_start = secpos * 0x80;
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
                    Array.Copy(s5ProjectByteArray, section_start + 68 + j * 15, tmp, 0, 15);
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
                int section_header_size = s5ProjectByteArray[section_start + 18] * 0x80;



                //if (section_header_typ != 0x20)
                {
                    //for (int n = blkstart + blkheadersize; n < blkstart + blksize /* s5ProjectByteArray.Length - 2 */; n++)
                    n = section_start + section_header_size;
                    //while (s5ProjectByteArray[n] == 0x00 && s5ProjectByteArray[n + 1] == 0x00)
                    //    n += 0x80;

                    while (akanz < anzbst && n+1<s5ProjectByteArray.Length ) //n < section_start + section_size)                       
                    {
                        akanz++;
                        int len = 0;
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
                    }
                }
                section_start = n;
                //section_start += section_size;
            }
        }

    }
}
