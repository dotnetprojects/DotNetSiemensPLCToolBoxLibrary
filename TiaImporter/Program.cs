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

            var extension = "xml";
            try
            {
                if (args.Length > 3)
                    extension = args[3];
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

            var programFolderToCompile = projectFolder;
            var pgFolderToCompile = programFolderToCompile as Step7ProjectV15_1.TIAOpennessProgramFolder;

            int i = 0;
            var fileList = BuildImportFileList(folderToImport, extension).ToList();
            var count = 0;

            ImportFiles();

            void ImportFiles()
            {
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
                                dtFolder.ImportFile(new FileInfo(importFile), true, false);
                            }
                            else if (pgFolder != null)
                            {
                                pgFolder.ImportFile(new FileInfo(importFile), true, !importFile.ToLower().EndsWith("xml"));
                            }

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Imported File: " + relativePath);
                            //if (importFile.ToLower().EndsWith("awl") || importFile.ToLower().EndsWith("scl"))
                            //{
                            //    File.Delete(importFile);
                            //}

                            fileList.Remove(importFile);
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error Importing File: " + relativePath + " - " + ex.Message);
                        }
                    }
                }
            }

            if (fileList.Count() != 0 && extension == "xml")
            {
                Console.WriteLine(count + " files could not be imported.");
                //Console.ReadLine();
                Environment.Exit(1);
            }

            if (fileList.Count() != 0 && extension != "xml")
            {
                Console.WriteLine(count + " files could not be imported after " + i + " runs.");
                var countBeforeCompile = fileList.Count();
                var countAfterCompile = 0;
                while (countAfterCompile < countBeforeCompile)
                {
                    countBeforeCompile = fileList.Count();
                    Console.WriteLine("... trying to compile project");
                    pgFolderToCompile.CompileBlocks();
                    Console.WriteLine("... restarting to import files");
                    count = 0;
                    ImportFiles();
                    countAfterCompile = fileList.Count();
                    if (countAfterCompile == 0)
                    {
                        break;
                    }
                }

                if (countAfterCompile == countBeforeCompile && fileList.Count() != 0)
                {
                    Environment.Exit(1);
                }

            }
        }


        public static IEnumerable<string> BuildImportFileList(string path, string extension)
        {
            foreach (var ext in extension.Split(';'))
            {
                foreach (var file in Directory.GetFiles(path, "*." + ext))
                {
                    yield return file;
                }
            }

            foreach (var dir in Directory.GetDirectories(path))
            {
                foreach (var file in BuildImportFileList(Path.Combine(path, dir), extension))
                {
                    yield return file;
                }

            }
        }
    }
}
