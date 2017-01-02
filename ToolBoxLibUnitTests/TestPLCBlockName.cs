using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using NUnit.Framework;

namespace ToolBoxLibUnitTests
{
	[TestFixture]
	public class TestPLCBlockName
	{
		[Test]
		public void TestParse()
		{
			PLCBlockName test = new PLCBlockName("DB5");
			Assert.AreEqual(5, test.BlockNumber);
			Assert.AreEqual(PLCBlockType.DB, test.BlockType);

			test = new PLCBlockName("db 5");
			Assert.AreEqual(5, test.BlockNumber);
			Assert.AreEqual(PLCBlockType.DB, test.BlockType);

			test = new PLCBlockName("FC109");
			Assert.AreEqual(109, test.BlockNumber);
			Assert.AreEqual(PLCBlockType.FC, test.BlockType);

			test = new PLCBlockName("FB9999");
			Assert.AreEqual(9999, test.BlockNumber);
			Assert.AreEqual(PLCBlockType.FB, test.BlockType);

			test = new PLCBlockName("SDB189");
			Assert.AreEqual(189, test.BlockNumber);
			Assert.AreEqual(PLCBlockType.SDB, test.BlockType);

			test = new PLCBlockName("OB1");
			Assert.AreEqual(1, test.BlockNumber);
			Assert.AreEqual(PLCBlockType.OB, test.BlockType);

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
