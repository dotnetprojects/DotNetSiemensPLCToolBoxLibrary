using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToolBoxLibUnitTests
{
    [TestClass]
    public class TestDataBlockReading
    {
        [TestMethod]
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
                                                                                                                fld, blk);
            Assert.AreEqual(test.Children[0].Name, "DB_VAR");
            Assert.AreEqual(test.Children[1].Name, "aa");
            Assert.AreEqual(test.Children[2].Name, "bb");
            Assert.AreEqual(test.Children[3].Name, "aa1");
            Assert.AreEqual(test.Children[0].BlockAddress.ByteAddress, 0);
            Assert.AreEqual(test.Children[1].BlockAddress.ByteAddress, 2);
            Assert.AreEqual(test.Children[2].BlockAddress.ByteAddress, 10);
            Assert.AreEqual(test.Children[3].BlockAddress.ByteAddress, 12);
        }
    }
}
