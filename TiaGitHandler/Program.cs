using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
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
                op.Filter = "TIA-Portal Project|*.ap13";
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
                        var src = projectBlockInfo.Export(ExportFormat.Xml);
                        if (src != null)
                        {
                            var ext = projectBlockInfo.BlockType.ToString().ToLower();
                            var file = Path.Combine(path, projectBlockInfo.Name + "." + ext);

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
                                XmlNodeList nodes = xmlDoc.SelectNodes("//Created");
                                XmlNode node = nodes[0];
                                node.ParentNode.RemoveChild(node);

                                var sb = new StringBuilder();
                                using (TextWriter writer = new EncodingStringWriter(sb, Encoding.UTF8))
                                {
                                    xmlDoc.Save(writer);
                                }
                                src = sb.ToString();
                                Directory.CreateDirectory(path);
                                File.WriteAllText(file, src);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Skipping Block (null)" + projectBlockInfo.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Skipping Block (Exception)" + projectBlockInfo.Name);
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
