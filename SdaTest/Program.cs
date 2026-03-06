using System;

namespace SdaTest
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing...");
            var projectPath = "/Users/tom/Downloads/broken_s7.zip";
            var exportPath = "/Users/tom/Downloads/export";
            ProjectConversionCommandHandler.Export(projectPath, exportPath);
        }
    }
}