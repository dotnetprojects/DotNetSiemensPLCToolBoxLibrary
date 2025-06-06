using System;
using System.IO;
using System.Text.RegularExpressions;

#if SHARPZIPLIB
using ICSharpCode.SharpZipLib.Zip;
#endif

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public class ZipHelper
    {
#if SHARPZIPLIB
        private ZipFile _zipFile;
#endif
        private string _zipFileName;

        public string GetFirstZipEntryWithEnding(string ending)
        {
#if SHARPZIPLIB

            string name = null;
            foreach (ZipEntry zipEntry in this._zipFile)
            {
                if (zipEntry.Name.ToLower().EndsWith(ending))
                {
                    name = zipEntry.Name;
                    break;
                }
            }
            return name;
#else
            return null;
#endif
        }

        public string GetFirstZipEntryWithMatch(string pattern)
        {
#if SHARPZIPLIB

            string name = null;
            foreach (ZipEntry zipEntry in this._zipFile)
            {
                if (Regex.IsMatch(zipEntry.Name.ToLower(), pattern))
                {
                    name = zipEntry.Name;
                    break;
                }
            }
            return name;
#else
            return null;
#endif
        }

        public static ZipHelper GetZipHelper(string zipfile)
        {
#if SHARPZIPLIB
            if (zipfile == null)
                return null;
            try
            {
                var zipHelper = new ZipHelper(zipfile);
                return zipHelper;
            }
            catch (Exception)
            { }
#endif
            return null;
        }

        public static ZipHelper GetZipHelper(Stream zipfile)
        {
#if SHARPZIPLIB
            if (zipfile == null)
                return null;
            try
            {
                var zipHelper = new ZipHelper(zipfile);
                return zipHelper;
            }
            catch (Exception)
            { }
#endif
            return null;
        }

        public bool IsZipFile { get { return this._zipFile != null; } }

        public ZipHelper(string file)
        {
#if SHARPZIPLIB
            _zipFileName = file;
            if (!string.IsNullOrEmpty(file))
            {
                try
                {
                    this._zipFile = null;
                    this._zipFile = new ZipFile(file);
                }
                catch (Exception)
                { }
            }
#endif
        }

        private ZipHelper(Stream file)
        {
#if SHARPZIPLIB
            if (file != null)
            {
                this._zipFile = new ZipFile(file);
            }
#endif
        }

        public void Close()
        {
#if SHARPZIPLIB
            if (_zipFile != null)
            {
                _zipFile.Close();
                _zipFile = null;
            }
#endif
        }

        public Stream GetReadStream(string file)
        {
            if (!FileExists(file))
                return null;
#if SHARPZIPLIB
            if (_zipFile == null)
            {
#endif
                return new FileStream(file, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite);
#if SHARPZIPLIB
            }
            else
            {
                ZipFile zf = (ZipFile)_zipFile;
                int fileEntry = zf.FindEntry(file.Replace("\\", "/"), true);
                return zf.GetInputStream(fileEntry);

            }
#endif
        }

        public bool IsZipped()
        {
#if SHARPZIPLIB
            if (_zipFile != null)
                return true;
#endif
            return false;
        }

        public Stream GetWriteStream(string file)
        {
#if SHARPZIPLIB
            if (_zipFile == null)
            {
#endif
                return new FileStream(file, FileMode.Open, FileAccess.Write, System.IO.FileShare.ReadWrite);
#if SHARPZIPLIB
            }
            else
            {
                Stream tmp = GetReadStream(file);
                byte[] bt = new byte[GetStreamLength(file, tmp)];
                tmp.Read(bt, 0, bt.Length);
                tmp.Close();
                MemoryStream ms = new MemoryStream();
                ms.Write(bt, 0, bt.Length);
                ms.Position = 0;
                return ms;
            }
#endif
        }

#if SHARPZIPLIB
        public class CustomStaticDataSource : IStaticDataSource
        {
            private Stream _stream;
            // Implement method from IStaticDataSource
            public Stream GetSource()
            {
                return _stream;
            }

            // Call this to provide the memorystream
            public void SetStream(Stream inputStream)
            {
                _stream = inputStream;
                _stream.Position = 0;
            }
        }
#endif
        public void WriteBackStream(string file, Stream strm)
        {
#if SHARPZIPLIB
            if (_zipFile == null)
            {
#endif
                strm.Close();
#if SHARPZIPLIB
            }
            else
            {

                _zipFile.BeginUpdate();

                int nr = _zipFile.FindEntry(file.Replace("\\", "/"), true);
                _zipFile.Delete(_zipFile[nr]);

                CustomStaticDataSource sds = new CustomStaticDataSource();
                sds.SetStream(strm);
                _zipFile.Add(sds, file.Replace("\\", "/"));
                _zipFile.CommitUpdate();
                _zipFile.Close();
                _zipFile = new ZipFile(_zipFileName);

                /*
                while (_zipFile.IsUpdating)
                {
                    System.Threading.Thread.Sleep(100);
                }
                */
                strm.Close();
            }
#endif
        }


        public long GetStreamLength(string file, Stream strm)
        {
#if SHARPZIPLIB
            if (_zipFile == null)
            {
#endif
                return strm.Length;
#if SHARPZIPLIB
            }
            else
            {
                ZipFile zf = (ZipFile)_zipFile;
                return zf.GetEntry(file.Replace("\\", "/")).Size;
                //return zf[file.Replace("\\", "/")].UncompressedSize;
            }
#endif
        }
        public bool FileExists(string file)
        {
#if SHARPZIPLIB
            if (_zipFile == null)
            {
#endif
                return System.IO.File.Exists(file);
#if SHARPZIPLIB
            }
            else
            {
                ZipFile zf = (ZipFile)_zipFile;
                //return zf.ContainsEntry(file.Replace("\\", "/"));
                int fileEntry = zf.FindEntry(file.Replace("\\", "/"), true);
                return fileEntry >= 0;
            }
#endif
        }

    }
}
