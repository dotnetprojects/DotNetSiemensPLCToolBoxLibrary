using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace TiaGitHandler
{
    class Program
    {
        private static string folder = "";

        [STAThread]
        static void Main(string[] args)
        {

            string file = "";
            string exportPath = "";

            if (args.Count() < 1)
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "TIA-Portal Project|*.ap13;*.ap14";
                op.CheckFileExists = false;
                op.ValidateNames = false;
                var ret = op.ShowDialog();
                if (ret == DialogResult.OK)
                {
                    file = op.FileName;
                }
                else
                {
                    Console.WriteLine("Bitte S7 projekt als Parameter angeben!");
                    return;
                }

                exportPath = Path.GetDirectoryName(file);
                exportPath = Path.GetFullPath(Path.Combine(exportPath, "..\\Export"));
                if (Directory.Exists(exportPath))
                {
                    if (
                        MessageBox.Show(exportPath + " wird gelöscht. Möchten Sie fortfahren?", "Sicherheitsabfrage",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Directory.Delete(exportPath, true);
                    }
                    else
                    {
                        Environment.Exit(-1);
                    }
                    
                }
                Directory.CreateDirectory(exportPath);
                
            }
            else
            {
                file = args[0];
            }

            var prj = Projects.LoadProject(file, false);

            ParseFolder(prj.ProjectStructure, exportPath);

            //Console.ReadLine();
        }

        private class EncodingStringWriter : StringWriter
        {
            private readonly Encoding _encoding;

            public EncodingStringWriter(StringBuilder builder, Encoding encoding) : base(builder)
            {
                _encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return _encoding; }
            }
        }

        private static void ParseFolder(ProjectFolder folder, string dir)
        {
            //Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, NormalizeFolderName(folder.Name));
            
            foreach (var projectFolder in folder.SubItems)
            {
                ParseFolder(projectFolder, path);
            }

            if (folder is IBlocksFolder)
            {
                var blkFld = folder as IBlocksFolder;
                
                foreach (var projectBlockInfo in blkFld.BlockInfos)
                {
                    try
                    {
                        var src = projectBlockInfo.Export(ExportFormat.Default);
                        string xml = null;
                        if (src != null)
                        {
                            var ext = "xml";
                            if (projectBlockInfo.BlockLanguage == PLCLanguage.DB && projectBlockInfo.BlockType == PLCBlockType.DB)
                            {
                                ext = "db";
                                xml = projectBlockInfo.Export(ExportFormat.Xml);
                            }
                            else if (projectBlockInfo.BlockLanguage == PLCLanguage.SCL)
                            {
                                ext = "scl";
                                xml = projectBlockInfo.Export(ExportFormat.Xml);
                            }
                            else if (projectBlockInfo.BlockLanguage == PLCLanguage.KOP)
                            {
                                ext = "xml";
                            }
                            else if (projectBlockInfo.BlockLanguage == PLCLanguage.FUP)
                            {
                                ext = "xml";
                            }
                            else if (projectBlockInfo.BlockLanguage == PLCLanguage.AWL)
                            {
                                ext = "awl";
                                xml = projectBlockInfo.Export(ExportFormat.Xml);
                            }
                            else if (projectBlockInfo.BlockType == PLCBlockType.UDT)
                            {
                                ext = "udt";
                            }
                            var file = Path.Combine(path, projectBlockInfo.Name.Replace("\\", "_").Replace("/", "_") + "." + ext);
                            var xmlfile = Path.Combine(path, projectBlockInfo.Name.Replace("\\", "_").Replace("/", "_") + ".xml");

                            var xmlValid = false;
                            XmlDocument xmlDoc = new XmlDocument();
                            try
                            {
                                xmlDoc.LoadXml(src);
                                xmlValid = true;
                            }
                            catch
                            {
                                xmlValid = false;
                            }

                            if (xmlValid)
                            {
                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//Created");
                                    var node = nodes[0];
                                    node.ParentNode.RemoveChild(node);
                                }
                                catch
                                {
                                }
                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//DocumentInfo");
                                    var node = nodes[0];
                                    node.ParentNode.RemoveChild(node);
                                }
                                catch
                                {
                                }

                                StringBuilder sb = new StringBuilder();
                                XmlWriterSettings settings = new XmlWriterSettings
                                {
                                    Indent = true,
                                    IndentChars = "  ",
                                    NewLineChars = "\r\n",
                                    NewLineHandling = NewLineHandling.Replace
                                };
                                using (TextWriter writer = new EncodingStringWriter(sb, Encoding.UTF8))
                                {
                                    xmlDoc.Save(writer);
                                }
                                src = sb.ToString();
                            }

                            if (src != null && ext != "db")
                            {
                                Directory.CreateDirectory(path);
                                File.WriteAllText(file, src, new UTF8Encoding(true));
                            }

                            if (xml != null)
                            {
                                var xmlValid2 = false;
                                XmlDocument xmlDoc2 = new XmlDocument();
                                try
                                {
                                    xmlDoc2.LoadXml(xml);
                                    xmlValid2 = true;
                                }
                                catch
                                {
                                    xmlValid2 = false;
                                }

                                if (xmlValid2)
                                {
                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//Created");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    {
                                    }
                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//DocumentInfo");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    {
                                    }

                                    StringBuilder sb = new StringBuilder();
                                    XmlWriterSettings settings = new XmlWriterSettings
                                    {
                                        Indent = true,
                                        IndentChars = "  ",
                                        NewLineChars = "\r\n",
                                        NewLineHandling = NewLineHandling.Replace
                                    };
                                    using (TextWriter writer = new EncodingStringWriter(sb, Encoding.UTF8))
                                    {
                                        xmlDoc2.Save(writer);
                                    }

                                    xml = sb.ToString();

                                    xml = xml.Replace("<ProgrammingLanguage>SCL</ProgrammingLanguage>", "<ProgrammingLanguage>STL</ProgrammingLanguage>");
                                }

                                Directory.CreateDirectory(path);
                                File.WriteAllText(xmlfile, xml, new UTF8Encoding(true));

                            }
                        }
                        else
                        {
                            Console.WriteLine("Skipping Block (null)" + projectBlockInfo.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Skipping Block: \"" + projectBlockInfo.Name + "\" Exception: " + ex.Message);
                    }
                }
            }
        }

        private static string NormalizeFolderName(string name)
        {
            return name.Replace("-", "").Replace(".", "").Replace(" ", "");
        }
    }
}
