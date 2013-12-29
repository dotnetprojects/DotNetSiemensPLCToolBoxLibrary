using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA
{
    public class TiaCompression
    {
        public static bool IsDeflateStream(Stream compressedStream)
        {
            if (compressedStream == null)
            {
                throw new ArgumentNullException("compressedStream");
            }
            byte[] buffer = new byte[4];
            compressedStream.Read(buffer, 0, 4);
            compressedStream.Position -= 4L;
            return (BitConverter.ToInt32(buffer, 0) == 0x6007bded);
        }

        //public static void DecompressStream(Stream compressedStream, Stream decompressedStream)
        //{
        //    compressedStream.Position = 0L;
        //    decompressedStream.Position = 0L;
        //    if (IsDeflateStream(compressedStream))
        //    {
        //        stream = new DeflateStream(compressedStream, CompressionMode.Decompress, true);
        //    }
        //    else
        //    {
        //        stream = new new ZLibNet.DeflateStream(compressedStream, CompressionMode.Decompress, true);
        //    }
        //    CopyStream(stream, decompressedStream);
        //    stream.Close();
        //    decompressedStream.Position = 0L;
        //}
    }
}
