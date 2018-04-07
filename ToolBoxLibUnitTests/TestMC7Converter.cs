using System;
using System.IO;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using System.Collections.Generic;
using NUnit.Framework;

namespace ToolBoxLibUnitTests
{
	[TestFixture]
	public class TestMC7Converter
	{
        //The bin files are plain dumps obtained via an call to "PLCConnection.PLCGetBlockInMC7(...)"
        //The Awl files are the parsed awl code files for the corresponding files. these files are compared to 
        //default Simatic manager online only output, in order to ensure correctness of the parsing
        //Lengths values are taken from the Simatic Manager Properties dialog

        //Set up templates for parsing
        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "S7Blocks", "MC7Converter") + Path.DirectorySeparatorChar;

        [Test]
		public void ParseDataBlocks()
		{
            byte[] block;

			//_____________________________________________________________________________
			//Read Data-block with lots of Structs and Structs in Arrays
			//This db has an complex combination of Structures, arrays and Structures in Arrays
			block = File.ReadAllBytes(dir + "DB121.bin");
			S7DataBlock DB = (S7DataBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

			Assert.AreEqual(PLCBlockType.DB, DB.BlockType);
			Assert.AreEqual(121, DB.BlockNumber);
			Assert.AreEqual(21418, DB.Length); //Load memory Size
			Assert.AreEqual(20824, DB.CodeSize); //Data size, this is the relevant data length
			Assert.AreEqual(File.ReadAllText(dir + "DB121.awl").Trim().Replace("\r\n", "\n"), DB.ToString().Trim().Replace("\r\n", "\n"));

			//_____________________________________________________________________________
			//Read Data-block With long Array of Structs
			//This is an really long data-block with an really long Array of structures
			block = File.ReadAllBytes(dir + "DB13.bin");
			DB = (S7DataBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

			Assert.AreEqual(PLCBlockType.DB, DB.BlockType);
			Assert.AreEqual(13, DB.BlockNumber);
			Assert.AreEqual(64214, DB.Length); //Load memory Size
			Assert.AreEqual(64040, DB.CodeSize); //Data size, this is the relevant data length
			Assert.AreEqual(File.ReadAllText(dir + "DB13.awl").Trim().Replace("\r\n", "\n"), DB.ToString().Trim().Replace("\r\n", "\n"));

			//_____________________________________________________________________________
			//Read Data-block With array and single Static Reals
			//An relatively simple block, with an Array and some simple reals at the end
			block = File.ReadAllBytes(dir + "DB4.bin");
			DB = (S7DataBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

			Assert.AreEqual(PLCBlockType.DB, DB.BlockType);
			Assert.AreEqual(4, DB.BlockNumber);
			Assert.AreEqual(8094, DB.Length); //Load memory Size
			Assert.AreEqual(6000, DB.CodeSize); //Data size, this is the relevant data length
			Assert.AreEqual(File.ReadAllText(dir + "DB4.awl").Trim().Replace("\r\n", "\n"), DB.Structure.ToString().Trim().Replace("\r\n", "\n"));
		}

		[Test]
		public void ParseFunctionCodeBlocks()
		{
			byte[] block;

			//_____________________________________________________________________________
			//Read Simple Function-Code with some calls
			//An relatively simple block, a few segments ans calls to sub functions
			block = File.ReadAllBytes(dir + "FC1.bin");
			S7FunctionBlock FC = (S7FunctionBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

			Assert.AreEqual(PLCBlockType.FC, FC.BlockType);
			Assert.AreEqual(1, FC.BlockNumber);
			Assert.AreEqual(434, FC.Length); //Load memory Size
			Assert.AreEqual(342, FC.CodeSize); //Code size of pure MC7
			Assert.AreEqual(6, FC.LocalDataSize); //Actually there are no "Temp" but the local data is needed for the "Call"'s 
			Assert.AreEqual(4, FC.Networks.Count);

			//Some of the called functions
			List<string> tmp = new List<string>(FC.CalledBlocks);
			Assert.AreEqual("FC96", tmp[2]);
			Assert.AreEqual("FC97", tmp[7]);
			Assert.AreEqual("FC16", tmp[11]);
			Assert.AreEqual("FC21", tmp[13]);

			Assert.AreEqual(File.ReadAllText(dir + "FC1.awl").Trim().Replace("\r\n", "\n"), FC.ToString().Trim().Replace("\r\n", "\n"));

			//_____________________________________________________________________________
			//Read complex Function-code 
			//Complex function with calls, Address-register manipulations, Indirect addressing of DB.
			//System functions, and lots of segments. AND indirect FC calls (Call FC[#tempvar])
			block = File.ReadAllBytes(dir + "FC100.bin");
			FC = (S7FunctionBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

			Assert.AreEqual(PLCBlockType.FC, FC.BlockType);
			Assert.AreEqual(100, FC.BlockNumber);
			Assert.AreEqual(4182, FC.Length); //Load memory Size
			Assert.AreEqual(3882, FC.CodeSize); //Code size of pure MC7
			Assert.AreEqual(58, FC.LocalDataSize);
			Assert.AreEqual(9, FC.Networks.Count);

			//Some of the called functions
			tmp = new List<string>(FC.CalledBlocks);
			Assert.AreEqual("SFC20", tmp[0]);
			Assert.AreEqual("FC6", tmp[6]);
			Assert.AreEqual("FC[LW16]", tmp[18]);
			Assert.AreEqual("SFC20", tmp[22]);

			Assert.AreEqual(File.ReadAllText(dir + "FC100.awl").Trim().Replace("\r\n", "\n"), FC.ToString().Trim().Replace("\r\n", "\n"));
		}

		[Test]
		public void ParseFunctionBlocks()
		{
			byte[] block;

			//_____________________________________________________________________________
			//Read Simple Function-Block without Instance data
			block = File.ReadAllBytes(dir + "FB101.bin");
			S7FunctionBlock FB = (S7FunctionBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

			Assert.AreEqual(PLCBlockType.FB, FB.BlockType);
			Assert.AreEqual(101, FB.BlockNumber);
			Assert.AreEqual(128, FB.Length); //Load memory Size
			Assert.AreEqual(24, FB.CodeSize); //Code size of pure MC7
			Assert.AreEqual(4, FB.LocalDataSize);
			Assert.AreEqual(3, FB.Networks.Count);

			//Some of the called functions
			List<string> tmp = new List<string>(FB.CalledBlocks);
			Assert.AreEqual("FC100", tmp[0]);

			Assert.AreEqual(File.ReadAllText(dir + "FB101.awl").Trim().Replace("\r\n", "\n"), FB.ToString().Trim().Replace("\r\n", "\n"));

			//_____________________________________________________________________________
			//Read Function-Block with Instance data
			block = File.ReadAllBytes(dir + "FB1001.bin");
			FB = (S7FunctionBlock)DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

			Assert.AreEqual(PLCBlockType.FB, FB.BlockType);
			Assert.AreEqual(1001, FB.BlockNumber);
			Assert.AreEqual(126, FB.Length); //Load memory Size
			Assert.AreEqual(22, FB.CodeSize); //Code size of pure MC7
			Assert.AreEqual(0, FB.LocalDataSize); //No temp data, all data is in STAT
			Assert.AreEqual(1, FB.Networks.Count);

			string t = FB.ToString();
			Assert.AreEqual(File.ReadAllText(dir + "FB1001.awl").Trim().Replace("\r\n", "\n"), FB.ToString().Trim().Replace("\r\n", "\n"));
		}


	    [Test(Description = "Parse an FB that has no code and no segment table defined")]
	    public void ParseFBWithoutSegmentTAble()
	    {
	        byte[] block = File.ReadAllBytes(dir + "FB751.bin");
	        var DB = DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

	        //Fix up different culture dependent Date conversion
	        //this might not be optimal, but i dont have any better solution at the moment.
	        string AWL = File.ReadAllText(dir + "FB751.awl").Trim().Replace("\r\n", "\n");
	        Assert.AreEqual(AWL, DB.ToString().Trim().Replace("\r\n", "\n"));
	    }

	    [Test(Description = "This block contains an unconditional call of an IN.Block_FB parameter without Parameters 'UC #IN2'")]
	    public void ParseFcWithParameterlessUnconditionalCall()
	    {
	        byte[] block = File.ReadAllBytes(dir + "FC21.bin");
	        var DB = DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.MC7Converter.GetAWLBlock(block, MnemonicLanguage.German);

	        //Fix up different culture dependent Date conversion
	        //this might not be optimal, but i dont have any better solution at the moment.
	        string AWL = File.ReadAllText(dir + "FC21.awl").Trim().Replace("\r\n", "\n");
	        Assert.AreEqual(AWL, DB.ToString().Trim().Replace("\r\n", "\n"));
	    }
    }
}
