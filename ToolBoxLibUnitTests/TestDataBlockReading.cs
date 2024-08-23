using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace ToolBoxLibUnitTests
{
	[TestFixture]
	public class TestDataBlockReading
	{
		[Test]
		public void TestDB1()
		{
			var txt =
				"\r\n  STRUCT \t\r\n   DB_VAR : INT  := 1;\t//vorläufige Platzhaltervariable\r\n   aa : ARRAY  [1 .. 8 ] OF BYTE ;\t\r\n   bb : INT ;\t\r\n   aa1 : INT ;\t\r\n  END_STRUCT ;\t";
			var par1 = new List<string>();
			var fld = new BlocksOfflineFolder();
			var blk = new S7Block();
			var test =
				DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.Parameter.GetInterfaceOrDBFromStep7ProjectString(txt,
																												ref par1,
																											   PLCBlockType
																													.DB,
																												false,
																												fld, blk, new S7ConvertingOptions());
			ClassicAssert.AreEqual(test.Children[0].Name, "DB_VAR");
			ClassicAssert.AreEqual(test.Children[1].Name, "aa");
			ClassicAssert.AreEqual(test.Children[2].Name, "bb");
			ClassicAssert.AreEqual(test.Children[3].Name, "aa1");
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).BlockAddress.ByteAddress, 0);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[1]).BlockAddress.ByteAddress, 2);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[2]).BlockAddress.ByteAddress, 10);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[3]).BlockAddress.ByteAddress, 12);
		}

		[Test]
		public void TestDB2()
		{
			var txt = "\r\n  STRUCT \t\r\n   Fachkoordinate : ARRAY  [0 .. 67, 1 .. 2 ] OF //X, Z (1 = links, 2 = rechts)\r\n   STRUCT \t\r\n    X : DINT ;\t\r\n   END_STRUCT ;\t\r\n  END_STRUCT ;\t";
			var par1 = new List<string>();
			var fld = new BlocksOfflineFolder();
			var blk = new S7Block();
			var test =
				DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.Parameter.GetInterfaceOrDBFromStep7ProjectString(txt,
																												ref par1,
																											   PLCBlockType
																													.DB,
																												false,
																												fld, blk, new S7ConvertingOptions());
			ClassicAssert.AreEqual(test.Children[0].Name, "Fachkoordinate");
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ArrayStart[0], 0);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ArrayStart[1], 1);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ArrayStop[0], 67);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ArrayStop[1], 2);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ByteLength, 544);
		}

		[Test]
		public void TestDB3()
		{
			var txt = "\r\n  STRUCT \t\r\n   X_KOORDINATE : ARRAY  [0 .. 67, 0 .. 16, 1 .. 2 ] OF //X, Y, Z (Z1 = links, Z2 = rechts)\r\n   INT ;\t\r\n  END_STRUCT ;\t";
			var par1 = new List<string>();
			var fld = new BlocksOfflineFolder();
			var blk = new S7Block();
			var test =
				DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.Parameter.GetInterfaceOrDBFromStep7ProjectString(txt,
																												ref par1,
																											   PLCBlockType
																													.DB,
																												false,
																												fld, blk, new S7ConvertingOptions());
			ClassicAssert.AreEqual(test.Children[0].Name, "X_KOORDINATE");
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ArrayStart[0], 0);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ArrayStart[1], 0);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ArrayStart[2], 1);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ArrayStop[0], 67);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ArrayStop[1], 16);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ArrayStop[2], 2);
			ClassicAssert.AreEqual(((S7DataRow)test.Children[0]).ByteLength, 4624);
		}

		[Test]
		public void TestDB4()
		{
			var txt = "\r\n  STRUCT \t\r\n   X_KOORDINATE : ARRAY  [0 .. 67, 0 .. 16, 1 .. 2 ] OF //X, Y, Z (Z1 = links, Z2 = rechts)\r\n   INT ;\t\r\n  END_STRUCT ;\t";
			var par1 = new List<string>();
			var fld = new BlocksOfflineFolder();
			var blk = new S7Block();
			var test =
				DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7.Parameter.GetInterfaceOrDBFromStep7ProjectString(txt,
																												ref par1,
																											   PLCBlockType
																													.DB,
																												false,
																												fld, blk, new S7ConvertingOptions());
			var rw = test.Children[0] as S7DataRow;
			var callStr = rw.GetCallingString();
		}
	}
}
