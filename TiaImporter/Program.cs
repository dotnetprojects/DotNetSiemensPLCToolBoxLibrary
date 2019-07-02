using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.V15_1;

namespace TiaImporter
{
    class Program
    {
        private static string importExtension = "*.xml";

        static void Main(string[] args)
        {
            Project prj = null;

            string file = "";
            string folderToImport = "";
            string baseTiaFld = "";

            try
            {
                file = args[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                folderToImport = args[1];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                baseTiaFld = args[2];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            prj = Step7ProjectV15_1.AttachToInstanceWithFilename(file);
            var prjV151 = prj as Step7ProjectV15_1;

            var projectFolder = prjV151.ProjectStructure;
            var flds = baseTiaFld.Split('/');
            foreach (var s in flds)
            {
                projectFolder = projectFolder.SubItems.First(x => x.Name == s);
            }

            int i = 0;
            var fileList = BuildImportFileList(folderToImport).ToList();
            var count = 0;
            while (count != fileList.Count() && fileList.Count() > 0)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Durchlauf " + ++i);
                Console.WriteLine("-----------------------------------");
                Console.WriteLine();
                count = fileList.Count();
                foreach (var importFile in fileList.ToList())
                {
                    var relativePath = importFile.Substring(folderToImport.Length + 1);

                    var importFolder = projectFolder;
                    foreach (var p in relativePath.Split('\\').Reverse().Skip(1).Reverse())
                    {
                        var prevFolder = importFolder;
                        importFolder = importFolder.SubItems.FirstOrDefault(x => x.Name == p);
                        if (importFolder == null)
                        {
                            importFolder = prevFolder.CreateFolder(p);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Folder Created: " + p);
                        }
                    }


                    try
                    {
                        var dtFolder = importFolder as Step7ProjectV15_1.TIAOpennessPlcDatatypeFolder;
                        var pgFolder = importFolder as Step7ProjectV15_1.TIAOpennessProgramFolder;
                        if (dtFolder != null)
                        {
                            dtFolder.ImportFile(new FileInfo(importFile), true);
                        }
                        else if (pgFolder != null)
                        {
                            pgFolder.ImportFile(new FileInfo(importFile), true);
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Imported File: " + relativePath);

                        fileList.Remove(importFile);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error Importing File: " + relativePath + " - " +ex.Message);
                    }
                }
            }

            if (fileList.Count() != 0)
            {
                Console.WriteLine(count + " files could not be imported");
                //Console.ReadLine();
                Environment.Exit(1);
            }
        }


        public static IEnumerable<string> BuildImportFileList(string path)
        {
            foreach (var file in Directory.GetFiles(path, importExtension))
            {
                yield return file;
            }

            foreach (var dir in Directory.GetDirectories(path))
            {
                foreach (var file in BuildImportFileList(Path.Combine(path, dir)))
                {
                    yield return file;
                }
                
            }
        }
    }
}
