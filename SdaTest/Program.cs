using System;

namespace SdaTest
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing...");
            var projectPath = "/Users/tom/Downloads/S7/test.zip";
            var exportPath = "/Users/tom/Downloads/S7/export";
            ProjectConversionCommandHandler.Export(projectPath, exportPath);
        }
    }
}