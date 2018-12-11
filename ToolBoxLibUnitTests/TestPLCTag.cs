using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.General;
using NUnit.Framework;

namespace ToolBoxLibUnitTests
{
	[TestFixture]
	public class TestPLCTag
    {
        public enum Test1
        {
            a = 1
        }

        public enum Test2 : short
        {
            a = 1
        }

        [Test]
		public void TestEnumTag()
        {
            var t = new PLCTag<Test1>();
            t.GenericValue = Test1.a;
            var wr = t.GenericValue;
            t._setValueProp = 1;
            var wr2 = t.GenericValue;
        }

        [Test]
        public void TestEnumTag2()
        {
            var t = new PLCTag<Test2>();
            t.GenericValue = Test2.a;
            var wr = t.GenericValue;
            t._setValueProp = 1;
            var wr2 = t.GenericValue;
        }

        [Test]
        public void TestEnumTag3()
        {
            var t = new PLCTag<Test1>("MW10", TagDataType.Int);
            t.GenericValue = Test1.a;
            var wr = t.GenericValue;
            t._setValueProp = (short)1;
            var wr2 = t.GenericValue;
        }
    }
}
