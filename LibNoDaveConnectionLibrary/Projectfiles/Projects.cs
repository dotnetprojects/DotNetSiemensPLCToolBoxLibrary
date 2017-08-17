using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public static class Projects
    {
        private static object _lockObject = new object();

        private static Func<string, Project> _createV13ProjectInstance;
        
        private static Func<string, Project> createV13ProjectInstance
        {
            get
            {
                
                if (_createV13ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV13ProjectInstance == null)
                        {
                            if (_createV14SP1ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V13 Project when you already have had opened a V14 Project. You need to close the Application!");
                            }
                            var assembly = Assembly.LoadFrom("DotNetSiemensPLCToolBoxLibrary.TIAV13.dll");
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.Step7ProjectV13");
                            _createV13ProjectInstance = (file) => (Project)Activator.CreateInstance(type, new object[] { file, null });
                        }
                    }
                }
                return _createV13ProjectInstance;
            }
        }

        private static Func<string, Project> _createV14SP1ProjectInstance;

        private static Func<string, Project> createV14SP1ProjectInstance
        {
            get
            {
                if (_createV14SP1ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV14SP1ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V14 SP1 Project when you already have had opened a V14 Project. You need to close the Application!");
                            }
                            var assembly = Assembly.LoadFrom("DotNetSiemensPLCToolBoxLibrary.TIAV14SP1.dll");
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V14SP1.Step7ProjectV14SP1");
                            _createV14SP1ProjectInstance = (file) => (Project)Activator.CreateInstance(type, new object[] { file, null });
                        }
                    }
                }
                return _createV14SP1ProjectInstance;
            }
        }

        /// <summary>
        /// This Function Returens a Step7 Project Instance for every Project Folder in the Path.
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        static public Project[] GetStep7ProjectsFromDirectory(string dirname)
        {
            List<Project> retVal = new List<Project>();

            try
            {
                string[] fls = System.IO.Directory.GetFiles(dirname, "*.s5d");
                foreach (var fl in fls)
                {
                    retVal.Add(new Step5Project(fl, false));
                }                
                    
            }
            catch(Exception)
            {
            }
            try
            {
                foreach (string subd in System.IO.Directory.GetDirectories(dirname))
                {
                    try
                    {
                        string[] fls = System.IO.Directory.GetFiles(subd, "*.s7p");
                        if (fls.Length > 0)
                            retVal.Add(new Step7ProjectV5(fls[0], false));

                        fls = System.IO.Directory.GetFiles(subd, "*.s7l");
                        if (fls.Length > 0)
                            retVal.Add(new Step7ProjectV5(fls[0], false));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap11");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap12");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap13");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.al11");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.al12");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.al13");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.s5d");
                        if (fls.Length > 0)
                            retVal.Add(new Step5Project(fls[0], false));
                    }
                    catch (Exception)
                    { }
                }

                foreach (var ending in "*.zip;*.zap13".Split(';'))
                {
                    string[] zips = System.IO.Directory.GetFiles(dirname, ending);
                    foreach (string zip in zips)
                    {
                        string entr = ZipHelper.GetFirstZipEntryWithEnding(zip, ".s7p");
                        if (entr != null)
                            retVal.Add(new Step7ProjectV5(zip, false));

                        entr = ZipHelper.GetFirstZipEntryWithEnding(zip, ".s7l");
                        if (entr != null)
                            retVal.Add(new Step7ProjectV5(zip, false));

                        entr = ZipHelper.GetFirstZipEntryWithEnding(zip, "*.ap11");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = ZipHelper.GetFirstZipEntryWithEnding(zip, "*.ap12");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = ZipHelper.GetFirstZipEntryWithEnding(zip, "*.ap13");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = ZipHelper.GetFirstZipEntryWithEnding(zip, "*.al11");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = ZipHelper.GetFirstZipEntryWithEnding(zip, "*.al12");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = ZipHelper.GetFirstZipEntryWithEnding(zip, "*.al13");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = ZipHelper.GetFirstZipEntryWithEnding(zip, ".s5d");
                        if (entr != null)
                            retVal.Add(new Step5Project(zip, false));
                    }
                }
            }
            catch(Exception)
            { }

            return retVal.ToArray();
        }

        static public Project LoadProject(string file, bool showDeleted)
        {
            if (file.ToLower().EndsWith(".s5d"))
                return new Step5Project(file, showDeleted);
            else if (file.ToLower().EndsWith(".s7p"))
                return new Step7ProjectV5(file, showDeleted);
            else if (file.ToLower().EndsWith(".s7l"))
                return new Step7ProjectV5(file, showDeleted);
            else if (file.ToLower().EndsWith(".ap11"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".ap12"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".ap13"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".ap14"))
                return createV14SP1ProjectInstance(file);
            else if (file.ToLower().EndsWith(".al11"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".al12"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".al13"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".al14"))
                return createV14SP1ProjectInstance(file);
            else if (!string.IsNullOrEmpty(ZipHelper.GetFirstZipEntryWithEnding(file, ".s5d")))
                return new Step5Project(file, showDeleted);
            else if (!string.IsNullOrEmpty(ZipHelper.GetFirstZipEntryWithEnding(file, ".s7p")))
                return new Step7ProjectV5(file, showDeleted);
            else if (!string.IsNullOrEmpty(ZipHelper.GetFirstZipEntryWithEnding(file, ".s7l")))
                return new Step7ProjectV5(file, showDeleted);
            return null;

        }

        public static ProjectFolder LoadProjectFolder(string projectAndStructuredFolderName)
        {
            var parts = projectAndStructuredFolderName.Split('|');
            var project = parts[0];
            var folder = parts[1];

            var prj = LoadProject(project, false);

            return prj.AllFolders.FirstOrDefault(x => x.StructuredFolderName == folder);
        }

    }
}
