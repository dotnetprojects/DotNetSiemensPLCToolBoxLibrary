using System;
using System.IO;
#if SHARPZIPLIB
//using ICSharpCode.SharpZipLib.Zip;
using Ionic.Zip;
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

            ZipFile zf = null;
            try
            {
                zf = new ZipFile(zipfile);
            }
            catch (Exception)
            {
                return null;
            }
            
            string name = null;
            foreach (ZipEntry zipEntry in zf)
            {
                if (zipEntry.FileName.ToLower().EndsWith(ending))
                {
                    name = zipEntry.FileName;
                    break;
                }
            }            
            zf.Dispose();
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
                zf.Dispose();
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
                //int fileEntry = zf.FindEntry(file.Replace("\\", "/"), true);                
                //return zf.GetInputStream(fileEntry);
                return zf[file.Replace("\\", "/")].InputStream;                
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
                //return zf.GetEntry(file.Replace("\\", "/")).Size;
                return zf[file.Replace("\\", "/")].UncompressedSize;
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
                //int fileEntry = zf.FindEntry(file.Replace("\\","/"), true);
                //return fileEntry >= 0;     
                return zf.ContainsEntry(file.Replace("\\", "/"));                
            }
#endif
        }
        
    }
}
