using System;
using System.IO;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace ToolBoxLibUnitTests
{
	[TestFixture]
	public class TestParameter
	{
        [SetUp]
        public void Setup()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo ("en");
        }

        //The bin files are plain dumps obtained via an call to "PLCConnection.PLCGetBlockInMC7(...)"
        //The Awl files are the parsed awl code files for the corresponding files. these files are compared to 
        //default Simatic manager online only output, in order to ensure correctness of the parsing
        //Lengths values are taken from the Simatic Manager Properties dialog

        //Set up templates for parsing
        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "S7Blocks", "Parameter") + Path.DirectorySeparatorChar ;

        [Test(Description = "Parse Datablock with Boolean and some of them, not all of them, defined as having Initial Values")]
        public void ParseDatablockWithBooleanInitialValues()
        {
            byte[] block = File.ReadAllBytes(dir + "DB3001.bin");
            S7DataBlock DB = (S7DataBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

            //General Structure must be the same
            string Test = DB.ToString().Trim().Replace("\r\n", "\n");
            ClassicAssert.AreEqual(File.ReadAllText(dir + "DB3001.awl").Trim().Replace("\r\n", "\n"), DB.ToString().Trim().Replace("\r\n", "\n"));
        }

        [Test(Description = "Parse Datablock with Real and Integers and some of them, not all of them, defined as having Initial Values")]
        public void ParseDatablockWithRealInitialValues()
        {
            byte[] block = File.ReadAllBytes(dir + "DB3002.bin");
            S7DataBlock DB = (S7DataBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

            //General Structure must be the same
            string Test = DB.ToString().Trim().Replace("\r\n", "\n");
            ClassicAssert.AreEqual(File.ReadAllText(dir + "DB3002.awl").Trim().Replace("\r\n", "\n"), DB.ToString().Trim().Replace("\r\n", "\n"));
        }

        [Test(Description = "Parse Datablock with Complex data, containing Arrays with non zero Lower bounds, DateAndTime ans Strings. All of them have initial values. This is one of the most complex Block interfaces to construct")]
        public void ParseDatablockWithComplexInitialValues()
        {
            byte[] block = File.ReadAllBytes(dir + "DB3003.bin");
            S7DataBlock DB = (S7DataBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

            //General Structure must be the same
            string Test = DB.ToString().Trim().Replace("\r\n", "\n");
            string AWL = File.ReadAllText(dir + "DB3003.awl").Trim().Replace("\r\n", "\n");
            ClassicAssert.AreEqual(AWL, DB.ToString().Trim().Replace("\r\n", "\n"));
        }

        [Test(Description = "Parse Datablock with multi dimensional Arrays")]
        public void ParseDatablockWithMultiDimensionalArrays()
        {
            byte[] block = File.ReadAllBytes(dir + "DB3004.bin");
            S7DataBlock DB = (S7DataBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

            //General Structure must be the same
            string Test = DB.ToString().Trim().Replace("\r\n", "\n");
            ClassicAssert.AreEqual(File.ReadAllText(dir + "DB3004.awl").Trim().Replace("\r\n", "\n"), DB.ToString().Trim().Replace("\r\n", "\n"));
        }

        [Test(Description = "Parse Datablock with two UDT's")]
        public void ParseDatablockWithUDT()
        {
            byte[] block = File.ReadAllBytes(dir + "DB3005.bin");
            S7DataBlock DB = (S7DataBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

            //General Structure must be the same
            string Test = DB.ToString().Trim().Replace("\r\n", "\n");
            ClassicAssert.AreEqual(File.ReadAllText(dir + "DB3005.awl").Trim().Replace("\r\n", "\n"), DB.ToString().Trim().Replace("\r\n", "\n"));
        }

        [Test(Description = "Parse Function block with very complex Interface, using all Interface types and Initial Values")]
        public void ParseFunctionBlockComplexInterface()
        {
            byte[] block = File.ReadAllBytes(dir + "FB3003.bin");
            var DB = DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

            //Fix up different culture dependent Date conversion
            //this might not be optimal, but i dont have any better solution at the moment.
            string AWL = File.ReadAllText(dir + "FB3003.awl").Trim().Replace("\r\n", "\n");
            ClassicAssert.AreEqual(AWL, DB.ToString().Trim().Replace("\r\n", "\n"));
        }

        [Test(Description = "Parse Function block with Multi Instance funcionalities")]
        public void ParseFunctionBlockMultiInstance()
        {
            byte[] block = File.ReadAllBytes(dir + "FB80.bin");
            var DB = DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

            //Fix up different culture dependent Date conversion
            //this might not be optimal, but i dont have any better solution at the moment.
            string AWL = File.ReadAllText(dir + "FB80.awl").Trim().Replace("\r\n", "\n");
            ClassicAssert.AreEqual(AWL, DB.ToString().Trim().Replace("\r\n", "\n"));
        }

        [Test(Description = "Parse Function block with BLOCK_XX Parameters and some Multi Instance Parameters")]
        public void ParseBlockParameter()
        {
            byte[] block = File.ReadAllBytes(dir + "FB3004.bin");
            var DB = DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

            //Fix up different culture dependent Date conversion
            //this might not be optimal, but i dont have any better solution at the moment.
            string AWL = File.ReadAllText(dir + "FB3004.awl").Trim().Replace("\r\n", "\n");
            ClassicAssert.AreEqual(AWL, DB.ToString().Trim().Replace("\r\n", "\n"));
        }


	    [Test(Description = "This is an test for some unusual datablocks that i found on an older PLC. it seams that these type of interface " +
	                        "gets created by older versions of Simatic Manager")]
	    public void ParseUnusualDataBlock()
	    {
	        byte[] block = File.ReadAllBytes(dir + "DB310.bin");
	        var DB = DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

	        //Fix up different culture dependent Date conversion
	        //this might not be optimal, but i dont have any better solution at the moment.
	        string AWL = File.ReadAllText(dir + "DB310.awl").Trim().Replace("\r\n", "\n");
	        ClassicAssert.AreEqual(AWL, DB.ToString().Trim().Replace("\r\n", "\n"));
	    }

	    [Test(Description = "There are 'Extended' versions for each Parameter type. Parse these")]
	    public void ParseParameterTypesExtended()
	    {
	        byte[] block = File.ReadAllBytes(dir + "FB41.bin");
	        var DB = DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

	        //Fix up different culture dependent Date conversion
	        //this might not be optimal, but i dont have any better solution at the moment.
	        string AWL = File.ReadAllText(dir + "FB41.awl").Trim().Replace("\r\n", "\n");
	        ClassicAssert.AreEqual(AWL, DB.ToString().Trim().Replace("\r\n", "\n"));
	    }
    }
}
