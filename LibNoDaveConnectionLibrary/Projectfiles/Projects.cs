using System;
using System.Collections.Generic;
using System.Text;
using LibNoDaveConnectionLibrary.General;

namespace LibNoDaveConnectionLibrary.Projectfiles
{
    public static class Projects
    {
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
                    retVal.Add(new Step5Project(fl));    
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

                        fls = System.IO.Directory.GetFiles(subd, "*.ap11");
                        if (fls.Length > 0)
                            retVal.Add(new Step7ProjectV11(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.s5d");
                        if (fls.Length > 0)
                            retVal.Add(new Step5Project(fls[0]));    
                    }
                    catch (Exception)
                    { }
                }

                string[] zips = System.IO.Directory.GetFiles(dirname, "*.zip");
                foreach (string zip in zips)
                {
                    string entr = ZipHelper.GetFirstZipEntryWithEnding(zip, ".s7p");
                    if (entr != null)
                        retVal.Add(new Step7ProjectV5(zip, false));

                    entr = ZipHelper.GetFirstZipEntryWithEnding(zip, ".s5d");
                    if (entr != null)
                        retVal.Add(new Step5Project(zip));
                }                                    
            }
            catch(Exception)
            { }

            return retVal.ToArray();
        }

        static public Project LoadProject(string file, bool showDeleted)
        {
            if (file.ToLower().EndsWith(".s5d"))
                return new Step5Project(file);
            else if (file.ToLower().EndsWith(".s7p"))
                return new Step7ProjectV5(file, showDeleted);
            else if (!string.IsNullOrEmpty(ZipHelper.GetFirstZipEntryWithEnding(file, ".s5d")))
                return new Step5Project(file);
            else if (!string.IsNullOrEmpty(ZipHelper.GetFirstZipEntryWithEnding(file, ".s7p")))
                return new Step7ProjectV5(file, showDeleted);
            return null;

        }

    }
}
