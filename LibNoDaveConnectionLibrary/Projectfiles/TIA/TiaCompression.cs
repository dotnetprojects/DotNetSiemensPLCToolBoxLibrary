using System;
using System.IO;

//using ZLibNet;
using CompressionMode = System.IO.Compression.CompressionMode;
using DeflateStream = System.IO.Compression.DeflateStream;

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

        public static Stream DecompressStream(Stream compressedStream)
        {
            compressedStream.Position = 0L;

            if (IsDeflateStream(compressedStream))
            {
                return new DeflateStream(compressedStream, CompressionMode.Decompress, true);
            }

            return null;
            //else
            //{
            //    return new ZLibNet.ZLibStream(compressedStream, ZLibNet.CompressionMode.Decompress, CompressionLevel.BestCompression, true);
            //}
        }
    }
}