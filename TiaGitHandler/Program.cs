﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using ICSharpCode.SharpZipLib.Core;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using TiaGitHandler.Properties;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace TiaGitHandler
{
    class Program
    {
        private static string folder = "";

        private static ProjectType _projectType = ProjectType.Tia15_1;

        private static bool resetSetpoints = true;
        private static bool removeCodeFromXml = true;
        private static bool removeAllBlanks = false;
        private static bool removeOnlyOneBlank = true;
        private static bool removeNoBlanks = false;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(
        IntPtr hConsoleHandle,
         out int lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(
            IntPtr hConsoleHandle,
            int ioMode);

        public const int STD_INPUT_HANDLE = -10;

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        const int ExtendedFlags = 128;
        const int QuickEditMode = 64;

        [STAThread]
        static void Main(string[] args)
        {
            bool hasArgs = args.Count() > 0;
            string file = "";
            string exportPath = Environment.CurrentDirectory;
            string user = Settings.Default.DefaultUser;
            string password = Settings.Default.DefaultPassword;
            bool attach = false;

            Project prj = null;

            if (!hasArgs)
            {
                Application app = new Application();
                var ask = new AskOpen();
                app.Run(ask);
                var res = ask.Result;
                resetSetpoints = ask.chkResetSetpoints.IsChecked == true;
                removeCodeFromXml = ask.chkRemoveCode.IsChecked == true;
                removeAllBlanks = ask.rbRemoveAllBlanks.IsChecked == true;
                removeOnlyOneBlank = ask.rbRemoveOnlyOneBlank.IsChecked == true;
                removeNoBlanks = ask.rbRemoveNoBlanks.IsChecked == true;

                DisableQuickEdit();

                if (object.Equals(res, false))
                {
                    OpenFileDialog op = new OpenFileDialog();
                    op.Filter = "TIA-Portal Project|*.ap13;*.ap14;*.ap15;*.ap15_1;*.ap16;*.ap17;*.ap18;*.ap19;*.ap20";
                    op.CheckFileExists = false;
                    op.ValidateNames = false;
                    var ret = op.ShowDialog();
                    if (ret == true)
                    {
                        file = op.FileName;
                    }
                    else
                    {
                        Console.WriteLine("Bitte S7 projekt als Parameter angeben!");
                        return;
                    }

                    if (Path.GetExtension(file) == ".ap15_1" || Path.GetExtension(file) == ".ap16")
                    {
                        if (InputBox.Show("Credentials", "Enter Username (or cancel if not used)", ref user) !=
                            DialogResult.Cancel)
                        {
                            if (InputBox.Show("Credentials", "Enter Password", ref password) != DialogResult.Cancel)
                            {

                            }
                            else
                            {
                                user = "";
                                password = "";
                            }
                        }
                        else
                        {
                            user = "";
                            password = "";
                        }
                    }

                    exportPath = Path.GetDirectoryName(file);
                    exportPath = Path.GetFullPath(Path.Combine(exportPath, "..\\Export"));

                }
                else if (res != null)
                {
                    var ver = ask.Result as string;
                    prj = Projects.AttachProject(ver);

                    exportPath = Path.GetDirectoryName(prj.ProjectFile);
                    exportPath = Path.GetFullPath(Path.Combine(exportPath, "..\\Export"));
                }
                else
                {
                    Environment.Exit(0);
                }

                if (Directory.Exists(exportPath))
                {
                    if (
                        MessageBox.Show(exportPath + " wird gelöscht. Möchten Sie fortfahren?",
                            "Sicherheitsabfrage",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DeleteDirectory(exportPath);
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
                if (args.Length > 1)
                    attach = bool.Parse(args[1]);
                if (args.Length > 2)
                    removeCodeFromXml = bool.Parse(args[2]);
                if (args.Length > 3)
                    user = args[3];
                if (args.Length > 4)
                    password = args[4];
            }

            if (prj == null)
            {
                Credentials credentials = null;
                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
                {
                    credentials = new Credentials() { Username = user, Password = new SecureString() };
                    foreach (char c in password)
                    {
                        credentials.Password.AppendChar(c);
                    }
                }

                if (attach)
                {
                    if (file.EndsWith("20"))
                        prj = Projects.AttachToInstanceWithFilename("20", file);
                    else if (file.EndsWith("19"))
                        prj = Projects.AttachToInstanceWithFilename("19", file);
                    else if (file.EndsWith("18"))
                        prj = Projects.AttachToInstanceWithFilename("18", file);
                    else if (file.EndsWith("17"))
                        prj = Projects.AttachToInstanceWithFilename("17", file);
                    else if (file.EndsWith("16"))
                        prj = Projects.AttachToInstanceWithFilename("16", file);
                    else if (file.EndsWith("15.1"))
                        prj = Projects.AttachToInstanceWithFilename("15.1", file);
                    else
                    {
                        Console.WriteLine("Attach to this TIA Version not supported");
                        Environment.Exit(-1);
                    }
                }
                else
                {
                    prj = Projects.LoadProject(file, false, credentials);
                }
            }

            _projectType = prj.ProjectType;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Opened Project - " + prj.ProjectType.ToString());
            Console.WriteLine("Exporting to Folder: " + exportPath);
            Console.WriteLine();
            List<string> skippedBlocksList = new List<string>();
            //ParseFolder(prj.ProjectStructure, exportPath, skippedBlocksList);
            //prj.ExportTextlists(prj.ProjectStructure, exportPath);
            prj.ExportAlarmTexts(prj.ProjectStructure, exportPath);

            var xlsxFiles = Directory.GetFiles(exportPath, "*.xlsx", SearchOption.AllDirectories);
            foreach (var xlsxFile in xlsxFiles.Where(x => x.Contains("plcalarmtextlistgroup")))
            {
                var fileInfo = new FileInfo(xlsxFile);
                using (var package = new ExcelPackage(fileInfo))
                {
                    var worksheet1 = package.Workbook.Worksheets[1];
                    var textlistType = worksheet1.Cells["B2"].Text; // textlist type - decimal, binary, bit

                    string targetFile = Path.Combine(
                        Path.GetDirectoryName(xlsxFile),
                        $"{Path.GetFileNameWithoutExtension(xlsxFile)}_{textlistType}.csv"
                    );

                    ConvertPlcAlarmTextListXlsxToCsv(xlsxFile, targetFile, textlistType);
                }

                File.Delete(xlsxFile);
            }
            
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            skippedBlocksList.ForEach(i => Console.WriteLine("{0}", i));
            Console.WriteLine();
            Console.WriteLine(skippedBlocksList.Count() + " blocks were skipped");

            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Export finished!");

            if (!hasArgs)
                Console.ReadKey();
        }

        public static void ConvertPlcAlarmTextListXlsxToCsv(string sourceFile, string targetFile, string textlistType)
        {
            var fileInfo = new FileInfo(sourceFile);
            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[2];

                var maxColumnNumber = worksheet.Dimension.End.Column;
                var totalRowCount = worksheet.Dimension.End.Row;

                using (var writer = new StreamWriter(targetFile, false, new UTF8Encoding(false)))
                {
                    for (int row = 1; row <= totalRowCount; row++)
                    {
                        var currentRow = new List<string>(maxColumnNumber);

                        for (int col = 1; col <= maxColumnNumber; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Text;

                            // double quotation marks (") -> ("")
                            if (cellValue.Contains("\""))
                                cellValue = cellValue.Replace("\"", "\"\"");

                            // Add quotation marks if cell contains commas or quotation marks
                            if (cellValue.Contains(",") || cellValue.Contains("\""))
                                cellValue = $"\"{cellValue}\"";

                            currentRow.Add(cellValue);
                        }

                        writer.WriteLine(string.Join(";", currentRow));
                    }
                }
            }
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

        private static void ParseFolder(ProjectFolder folder, string dir, List<string> skippedBlocksList)
        {
            //Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, NormalizeFolderName(folder.Name));

            foreach (var projectFolder in folder.SubItems)
            {
                ParseFolder(projectFolder, path, skippedBlocksList);
            }

            if (folder is IBlocksFolder)
            {
                var blkFld = folder as IBlocksFolder;

                foreach (var projectBlockInfo in blkFld.BlockInfos.ToList())
                {
                    try
                    {
                        var src = projectBlockInfo.Export(ExportFormat.Default);
                        string xml = null;
                        if (src != null)
                        {
                            if (!removeNoBlanks)
                            {
                                /*var startIndex = src.IndexOf("   VAR ");
                                var endIndex = src.IndexOf("   END_VAR", startIndex);*/
                                var startIndex = 0;
                                var endIndex = src.IndexOf("BEGIN", startIndex);
                                if (endIndex == -1) endIndex = src.IndexOf("END_TYPE", startIndex);

                                if (endIndex != -1)
                                {
                                    var search = src;
                                    var pattern = "   // ";

                                    var indexes = Enumerable.Range(startIndex, endIndex - startIndex)
                                        .Select(index =>
                                        {
                                            return new
                                            {
                                                Index = index,
                                                Length = index + pattern.Length > search.Length
                                                    ? search.Length - index
                                                    : pattern.Length
                                            };
                                        })
                                        .Where(searchbit =>
                                            searchbit.Length == pattern.Length && pattern.Equals(
                                                search.Substring(searchbit.Index, searchbit.Length),
                                                StringComparison.OrdinalIgnoreCase))
                                        .Select(searchbit => searchbit.Index);

                                    var updatedSrc = src;

                                    foreach (var x in indexes.Reverse())
                                        if (removeOnlyOneBlank)
                                            updatedSrc = updatedSrc.Remove(x + 5, 1);
                                        else if (removeAllBlanks)
                                            while (updatedSrc[x + 5].ToString() == " ")
                                                updatedSrc = updatedSrc.Remove(x + 5, 1);

                                    src = updatedSrc;
                                }
                            }

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
                                xml = projectBlockInfo.Export(ExportFormat.Xml);
                            }
                            var file = Path.Combine(path, projectBlockInfo.Name.Replace("\\", "_").Replace("/", "_") + "." + ext);
                            var xmlfile = Path.Combine(path, projectBlockInfo.Name.Replace("\\", "_").Replace("/", "_") + ".xml");

                            var xmlValid = false;
                            XmlDocument xmlDoc = new XmlDocument();
                            XmlNamespaceManager ns = new XmlNamespaceManager(xmlDoc.NameTable);
                            ns.AddNamespace("smns", "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3");
                            ns.AddNamespace("smns2", "http://www.siemens.com/automation/Openness/SW/Interface/v3");
                            ns.AddNamespace("smns3", "http://www.siemens.com/automation/Openness/SW/NetworkSource/StatementList/v3");
                            ns.AddNamespace("smns4", "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v2");
                            ns.AddNamespace("smns5", "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v4");
                            ns.AddNamespace("smns6", "http://www.siemens.com/automation/Openness/SW/NetworkSource/StatementList/v5");

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

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//CodeModifiedDate");
                                    var node = nodes[0];
                                    node.ParentNode.RemoveChild(node);
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//CompileDate");
                                    var node = nodes[0];
                                    node.ParentNode.RemoveChild(node);
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//CreationDate");
                                    var node = nodes[0];
                                    node.ParentNode.RemoveChild(node);
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//InterfaceModifiedDate");
                                    var node = nodes[0];
                                    node.ParentNode.RemoveChild(node);
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//ModifiedDate");
                                    var node = nodes[0];
                                    node.ParentNode.RemoveChild(node);
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//ParameterModified");
                                    var node = nodes[0];
                                    node.ParentNode.RemoveChild(node);
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//StructureModified");
                                    var node = nodes[0];
                                    node.ParentNode.RemoveChild(node);
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//smns:DateAttribute[@Name='ParameterModifiedTS']", ns);
                                    foreach (var node in nodes.Cast<XmlNode>())
                                    {
                                        node.ParentNode.RemoveChild(node);
                                    }
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//smns3:DateAttribute[@Name='ParameterModifiedTS']", ns);
                                    foreach (var node in nodes.Cast<XmlNode>())
                                    {
                                        node.ParentNode.RemoveChild(node);
                                    }
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//smns4:DateAttribute[@Name='ParameterModifiedTS']", ns);
                                    foreach (var node in nodes.Cast<XmlNode>())
                                    {
                                        node.ParentNode.RemoveChild(node);
                                    }
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//smns5:DateAttribute[@Name='ParameterModifiedTS']", ns);
                                    foreach (var node in nodes.Cast<XmlNode>())
                                    {
                                        node.ParentNode.RemoveChild(node);
                                    }
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//smns6:DateAttribute[@Name='ParameterModifiedTS']", ns);
                                    foreach (var node in nodes.Cast<XmlNode>())
                                    {
                                        node.ParentNode.RemoveChild(node);
                                    }
                                }
                                catch
                                {
                                }

                                //try
                                //{
                                //    var nodes = xmlDoc.SelectNodes("//smns:Address[@Area='None' and @Informative='true']", ns);
                                //    foreach (var node in nodes.Cast<XmlNode>())
                                //    {
                                //        node.ParentNode.RemoveChild(node);
                                //    }
                                //}
                                //catch
                                //{
                                //}

                                //try
                                //{
                                //    var nodes = xmlDoc.SelectNodes("//smns2:IntegerAttribute[@Name='Offset' and @Informative='true']", ns);
                                //    foreach (var node in nodes.Cast<XmlNode>())
                                //    {
                                //        node.ParentNode.RemoveChild(node);
                                //    }
                                //}
                                //catch
                                //{
                                //}

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//*[@Informative='true']");
                                    foreach (var node in nodes.Cast<XmlNode>())
                                    {
                                        node.ParentNode.RemoveChild(node);
                                    }
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//*[local-name()='InstanceOfNumber']");
                                    foreach (var node in nodes.Cast<XmlNode>())
                                    {
                                        node.ParentNode.RemoveChild(node);
                                    }
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//*[local-name()='LibraryConformanceStatus']");
                                    foreach (var node in nodes.Cast<XmlNode>())
                                    {
                                        node.ParentNode.RemoveChild(node);
                                    }
                                }
                                catch
                                {
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//*[local-name()='Member'][contains(@Datatype,'\"')]//*[local-name()='Sections']");
                                    foreach (var node in nodes.Cast<XmlNode>())
                                    {
                                        if (!node.OuterXml.Contains("Remanence=\"Retain\"") && !node.OuterXml.Contains("StartValue"))
                                            node.ParentNode.RemoveChild(node);
                                    }
                                }
                                catch
                                {
                                }

                                if (resetSetpoints)
                                {
                                    try
                                    {
                                        var nodes = xmlDoc.SelectNodes("//smns2:BooleanAttribute[@Name='SetPoint']", ns);
                                        foreach (var node in nodes.Cast<XmlNode>())
                                        {
                                            node.InnerText = "false";
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }

                                try
                                {
                                    var nodes = xmlDoc.SelectNodes("//*[@ReadOnly='true']");
                                    foreach (var node in nodes.Cast<XmlNode>())
                                    {
                                        node.ParentNode.RemoveChild(node);
                                    }
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
                                XmlNamespaceManager ns2 = new XmlNamespaceManager(xmlDoc2.NameTable);
                                ns2.AddNamespace("smns", "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3");
                                ns2.AddNamespace("smns2", "http://www.siemens.com/automation/Openness/SW/Interface/v3");
                                ns2.AddNamespace("smns3", "http://www.siemens.com/automation/Openness/SW/NetworkSource/StatementList/v3");
                                ns2.AddNamespace("smns4", "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v2");

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
                                    if (!removeNoBlanks)
                                    {
                                        try
                                        {
                                            XmlNodeList nodes = xmlDoc2.GetElementsByTagName("MultiLanguageText");

                                            var pattern = "^( *)(.*)";

                                            foreach (var n in nodes.Cast<XmlNode>())
                                            {
                                                if (removeOnlyOneBlank)
                                                {
                                                    n.InnerText = Regex.Replace(n.InnerText, pattern, m => m.Groups[1].Value.Substring(0, m.Groups[1].Value.Length - 1) + m.Groups[2].Value);
                                                }
                                                else if (removeAllBlanks)
                                                {
                                                    n.InnerXml = Regex.Replace(n.InnerXml, pattern, m => "" + m.Groups[2].Value);
                                                }
                                            }
                                        }
                                        catch
                                        { }
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//Created");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//DocumentInfo");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//CodeModifiedDate");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    {
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//CompileDate");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    {
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//CreationDate");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    {
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//InterfaceModifiedDate");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    {
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//ModifiedDate");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    {
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//ParameterModified");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    {
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//StructureModified");
                                        var node = nodes[0];
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    catch
                                    {
                                    }

                                    if (removeCodeFromXml && !xml.Contains("$$GITHANDLER-KEEPCODE$$"))
                                    {
                                        try
                                        {
                                            var nodes = xmlDoc2.SelectNodes("//SW.Blocks.CompileUnit");
                                            var node = nodes[0];
                                            node.InnerXml = "";
                                            var parent = node.ParentNode;
                                            foreach (var nd in nodes.Cast<XmlNode>().Skip(1).ToList())
                                            {
                                                parent.RemoveChild(nd);
                                            }
                                        }
                                        catch
                                        { }
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//smns:DateAttribute[@Name='ParameterModifiedTS']", ns2);
                                        foreach (var node in nodes.Cast<XmlNode>())
                                        {
                                            node.ParentNode.RemoveChild(node);
                                        }
                                    }
                                    catch
                                    {
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//smns3:DateAttribute[@Name='ParameterModifiedTS']", ns2);
                                        foreach (var node in nodes.Cast<XmlNode>())
                                        {
                                            node.ParentNode.RemoveChild(node);
                                        }
                                    }
                                    catch
                                    {
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//smns4:DateAttribute[@Name='ParameterModifiedTS']", ns2);
                                        foreach (var node in nodes.Cast<XmlNode>())
                                        {
                                            node.ParentNode.RemoveChild(node);
                                        }
                                    }
                                    catch
                                    {
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//*[@Informative='true']");
                                        foreach (var node in nodes.Cast<XmlNode>())
                                        {
                                            node.ParentNode.RemoveChild(node);
                                        }
                                    }
                                    catch
                                    {
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//*[local-name()='LibraryConformanceStatus']");
                                        foreach (var node in nodes.Cast<XmlNode>())
                                        {
                                            node.ParentNode.RemoveChild(node);
                                        }
                                    }
                                    catch
                                    {
                                    }

                                    if (projectBlockInfo.BlockLanguage == PLCLanguage.DB && projectBlockInfo.BlockType == PLCBlockType.DB && projectBlockInfo.IsInstance)
                                    {
                                        try
                                        {
                                            var nodes = xmlDoc2.SelectNodes("//*[local-name()='Interface']/*[local-name()='Sections']/*[local-name()='Section']/*[local-name()='Member']");
                                            foreach (var node in nodes.Cast<XmlNode>())
                                            {
                                                if (!node.OuterXml.Contains("Remanence=\"Retain\"") && !node.OuterXml.Contains("StartValue"))
                                                    node.ParentNode.RemoveChild(node);
                                            }
                                        }
                                        catch
                                        {
                                        }

                                        try
                                        {
                                            var nodes = xmlDoc2.SelectNodes("//*[local-name()='InstanceOfNumber']");
                                            foreach (var node in nodes.Cast<XmlNode>())
                                            {
                                                node.ParentNode.RemoveChild(node);
                                            }
                                        }
                                        catch
                                        {
                                        }

                                    }

                                    if (resetSetpoints)
                                    {
                                        try
                                        {
                                            var nodes = xmlDoc2.SelectNodes("//smns2:BooleanAttribute[@Name='SetPoint']", ns2);
                                            foreach (var node in nodes.Cast<XmlNode>())
                                            {
                                                node.InnerText = "false";
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }

                                    if (projectBlockInfo.BlockTypeString == "Userdatatype" || projectBlockInfo.BlockTypeString == "Functionblock")
                                    {
                                        try
                                        {
                                            var nodes = xmlDoc2.SelectNodes("//*[local-name()='Member'][contains(@Datatype,'\"')]//*[local-name()='Sections']");
                                            foreach (var node in nodes.Cast<XmlNode>())
                                            {
                                                if (!node.OuterXml.Contains("Remanence=\"Retain\"") && !node.OuterXml.Contains("StartValue"))
                                                    node.ParentNode.RemoveChild(node);
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }

                                    try
                                    {
                                        var nodes = xmlDoc2.SelectNodes("//*[@ReadOnly='true']");
                                        foreach (var node in nodes.Cast<XmlNode>())
                                        {
                                            node.ParentNode.RemoveChild(node);
                                        }
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

                                    if (_projectType == ProjectType.Tia14SP1)
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
                        skippedBlocksList.Add(projectBlockInfo.Name);
                    }
                }
            }
            else if (folder is ITIAVarTabFolder varTabfld)
            {
                foreach (var varTab in varTabfld.TagTables)
                {
                    var xmlValid = false;
                    string xml = null;
                    XmlDocument xmlDoc = new XmlDocument();

                    var file = Path.Combine(path, varTab.Name.Replace("\\", "_").Replace("/", "_") + ".xml");
                    try
                    {
                        var vt = varTab.Export();
                        try
                        {
                            xmlDoc.LoadXml(vt);
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

                        xml = sb.ToString();

                        Directory.CreateDirectory(path);
                        File.WriteAllText(file, xml, new UTF8Encoding(true));
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(file + " could not be exported");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            else if (folder is ITIAWatchAndForceTablesFolder wtfFld)
            {
                foreach (var varTab in wtfFld.WatchTables)
                {
                    var xmlValid = false;
                    string xml = null;
                    XmlDocument xmlDoc = new XmlDocument();

                    var file = Path.Combine(path, varTab.Name.Replace("\\", "_").Replace("/", "_") + ".watch");

                    try
                    {
                        var vt = varTab.Export();
                        try
                        {
                            xmlDoc.LoadXml(vt);
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

                        xml = sb.ToString();
                        Directory.CreateDirectory(path);
                        File.WriteAllText(file, xml, new UTF8Encoding(true));
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(file + " could not be exported");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                foreach (var varTab in wtfFld.ForceTables)
                {
                    var xmlValid = false;
                    string xml = null;
                    XmlDocument xmlDoc = new XmlDocument();

                    var file = Path.Combine(path, varTab.Name.Replace("\\", "_").Replace("/", "_") + ".force");

                    try
                    {
                        var vt = varTab.Export();

                        try
                        {
                            xmlDoc.LoadXml(vt);
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

                        xml = sb.ToString();
                        Directory.CreateDirectory(path);
                        File.WriteAllText(file, xml, new UTF8Encoding(true));
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(file + " could not be exported");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }

        private static string NormalizeFolderName(string name)
        {
            return name.Replace("-", "").Replace(".", "").Replace(" ", "");
        }

        public static void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                Thread.Sleep(1);
                DeleteDir(directory);
            }
            DeleteDir(path);
        }

        static int DeleteDirCounter = 0;
        private static void DeleteDir(string dir)
        {
            try
            {
                Thread.Sleep(1);
                Directory.Delete(dir, true);
            }
            catch (IOException)
            {
                DeleteDirCounter++;
                if (DeleteDirCounter > 10)
                    throw;
                DeleteDir(dir);
            }
            catch (UnauthorizedAccessException)
            {
                DeleteDirCounter++;
                if (DeleteDirCounter > 10)
                    throw;
                DeleteDir(dir);
            }
        }

        private static void DisableQuickEdit()
        {
            IntPtr conHandle = GetStdHandle(STD_INPUT_HANDLE);
            int mode;

            if (!GetConsoleMode(conHandle, out mode))
            {
                Console.WriteLine("err1");
                // error getting the console mode. Exit.
                return;
            }

            mode = mode & ~(QuickEditMode | ExtendedFlags);

            if (!SetConsoleMode(conHandle, mode))
            {
                Console.WriteLine("err2");
                // error setting console mode.
            }
        }
    }
}
