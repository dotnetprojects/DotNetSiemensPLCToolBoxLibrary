using DotNetSiemensPLCToolBoxLibrary.General;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace ToolBoxLibUnitTests
{
	[TestFixture]
	public class TestCRC
	{
		[Test]
		public void TestCRC16BlockSum()
		{
			//see: https://www.sps-forum.de/hochsprachen-opc/56412-upload-von-block-zur-s7-2.html
			var bytes = new byte[] { 0x0C, 0x00, 0x0E, 0xC0, 0x00, 0xC1, 0x00, 0xC3, 0x00, 0xC4, 0x00, 0xC5, 0x00, 0xD8, 0x80, 0x65, 0x00 };
			var crc = CrcHelper.GetCrc16(bytes);
			ClassicAssert.AreEqual(crc, 0x7822);
		}
	}
}
