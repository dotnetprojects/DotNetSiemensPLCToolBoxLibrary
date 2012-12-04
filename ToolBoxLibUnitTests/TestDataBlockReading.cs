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

        [TestMethod]
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
                                                                                                                fld, blk);
            Assert.AreEqual(test.Children[0].Name, "Fachkoordinate");
            Assert.AreEqual(test.Children[0].ArrayStart[0], 0);
            Assert.AreEqual(test.Children[0].ArrayStart[1], 1);
            Assert.AreEqual(test.Children[0].ArrayStop[0], 67);
            Assert.AreEqual(test.Children[0].ArrayStop[1], 2);
            Assert.AreEqual(test.Children[0].ByteLength, 544);
        }

        [TestMethod]
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
                                                                                                                fld, blk);
            Assert.AreEqual(test.Children[0].Name, "X_KOORDINATE");
            Assert.AreEqual(test.Children[0].ArrayStart[0], 0);
            Assert.AreEqual(test.Children[0].ArrayStart[1], 0);
            Assert.AreEqual(test.Children[0].ArrayStart[2], 1);
            Assert.AreEqual(test.Children[0].ArrayStop[0], 67);
            Assert.AreEqual(test.Children[0].ArrayStop[1], 16);
            Assert.AreEqual(test.Children[0].ArrayStop[2], 2);
            Assert.AreEqual(test.Children[0].ByteLength, 4624);
        }
    }
}
