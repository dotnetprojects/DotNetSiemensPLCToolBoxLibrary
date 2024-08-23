using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace ToolBoxLibUnitTests
{
	[TestFixture]
	public class TestPLCBlockName
	{
		[Test]
		public void TestParse()
		{
			PLCBlockName test = new PLCBlockName("DB5");
			ClassicAssert.AreEqual(5, test.BlockNumber);
			ClassicAssert.AreEqual(PLCBlockType.DB, test.BlockType);

			test = new PLCBlockName("db 5");
			ClassicAssert.AreEqual(5, test.BlockNumber);
			ClassicAssert.AreEqual(PLCBlockType.DB, test.BlockType);

			test = new PLCBlockName("FC109");
			ClassicAssert.AreEqual(109, test.BlockNumber);
			ClassicAssert.AreEqual(PLCBlockType.FC, test.BlockType);

			test = new PLCBlockName("FB9999");
			ClassicAssert.AreEqual(9999, test.BlockNumber);
			ClassicAssert.AreEqual(PLCBlockType.FB, test.BlockType);

			test = new PLCBlockName("SDB189");
			ClassicAssert.AreEqual(189, test.BlockNumber);
			ClassicAssert.AreEqual(PLCBlockType.SDB, test.BlockType);

			test = new PLCBlockName("OB1");
			ClassicAssert.AreEqual(1, test.BlockNumber);
			ClassicAssert.AreEqual(PLCBlockType.OB, test.BlockType);

			try
			{
				test = new PLCBlockName("FX77");
				Assert.Fail("Parsing invalid Block types must fail");
			}
			catch { } //Must throw error

			try
			{
				test = new PLCBlockName("FC7a");
				Assert.Fail("Parsing invalid Block numbers must fail");
			}
			catch { } //Must throw error
		}
	}
}
