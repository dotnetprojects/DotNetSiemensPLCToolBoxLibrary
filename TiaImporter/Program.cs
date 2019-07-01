using System;
using System.IO;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.General;
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

            prj = Projects.AttachProject("15.1");

            var prjV151 = prj as Step7ProjectV15_1;

            var fld = prjV151.ProjectStructure.SubItems[0].SubItems[1] as Step7ProjectV15_1.TIAOpennessPlcDatatypeFolder;
            fld.ImportFile(new FileInfo("xxx.xml"), true);

        }
    }
}
