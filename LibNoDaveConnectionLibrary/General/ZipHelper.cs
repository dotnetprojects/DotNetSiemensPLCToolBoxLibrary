using System.IO;
using ICSharpCode.SharpZipLib.Zip;
#if SHARPZIPLIB

#endif

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public static class ZipHelper
    {
        public static string GetFirstZipEntryWithEnding(string zipfile, string ending)
        {
#if SHARPZIPLIB
            if (string.IsNullOrEmpty(zipfile))
                return null;
            ZipFile zf = new ZipFile(zipfile);

            string name = null;
            foreach (ZipEntry zipEntry in zf)
            {
                if (zipEntry.Name.ToLower().EndsWith(ending))
                {
                    name = zipEntry.Name;
                    break;
                }
            }            
            zf.Close();
            return name;
#else
            return null;
#endif
        }        

        public static object OpenZipfile(string file)
        {
#if SHARPZIPLIB
            return new ZipFile(file);
#else
            return null;
#endif
        }

        public static void CloseZipfile(object zipfile)
        {
#if SHARPZIPLIB
            if (zipfile != null)
            {
                ZipFile zf = (ZipFile) zipfile;                
                zf.Close();
            }
#endif
        }

        public static Stream GetReadStream(object zipfile, string file)
        {            
#if SHARPZIPLIB
            if (zipfile==null)
            {
#endif
                return new FileStream(file, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite);
#if SHARPZIPLIB
            }
            else
            {
                ZipFile zf = (ZipFile)zipfile;
                int fileEntry = zf.FindEntry(file.Replace("\\", "/"), true);
                return zf.GetInputStream(fileEntry);
            }
#endif
        }

        public static long GetStreamLength(object zipfile, string file, Stream strm)
        {
#if SHARPZIPLIB
            if (zipfile == null)
            {
#endif
                return strm.Length;
#if SHARPZIPLIB
            }
            else
            {
                ZipFile zf = (ZipFile)zipfile;
                return zf.GetEntry(file.Replace("\\", "/")).Size;
            }
#endif
        }
        public static bool FileExists(object zipfile, string file)
        {
#if SHARPZIPLIB
            if (zipfile==null)
            {
#endif
                return System.IO.File.Exists(file);
#if SHARPZIPLIB
            }
            else
            {
                ZipFile zf = (ZipFile)zipfile;
                int fileEntry = zf.FindEntry(file.Replace("\\","/"), true);
                return fileEntry >= 0;                    
            }
#endif
        }
        
    }
}
