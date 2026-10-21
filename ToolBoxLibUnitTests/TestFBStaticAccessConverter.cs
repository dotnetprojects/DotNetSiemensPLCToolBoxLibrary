using System.Collections.Generic;
using System.Linq;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using NUnit.Framework;

namespace ToolBoxLibUnitTests
{
    [TestFixture]
    public class TestFBStaticAccessConverter
    {
        [TestCase("LAR2 P#0.0")]
        [TestCase("TAR2")]
        [TestCase("TAR2", "+AR2 P#2.0")]
        [TestCase("TAR2", "L DIW[AR2,P#0.0]")]
        [TestCase("TAR2", "+AR2 P#2.0", "L DIW[AR2,P#0.0]", "LAR2")]
        [TestCase("TAR2", "+AR2 P#2.0", "L DIW[AR2,P#0.0]", "NOP 0")]
        [TestCase("TAR2", "+AR2", "L DIW[AR2,P#0.0]", "LAR2")]
        public void PreservesUnsupportedInstructionsInOrder(params string[] instructions)
        {
            var block = CreateBlock(instructions);
            var rows = block.AWLCode.ToArray();
            var parameters = rows.Cast<S7FunctionBlockRow>().Select(row => row.Parameter).ToArray();
            var bytes = rows.Cast<S7FunctionBlockRow>().Select(row => row.MC7).ToArray();

            FBStaticAccessConverter.ReplaceStaticAccess(block, null, new S7ConvertingOptions());

            Assert.That(block.AWLCode, Is.EqualTo(rows));
            Assert.That(block.AWLCode.Cast<S7FunctionBlockRow>().Select(row => row.Parameter), Is.EqualTo(parameters));
            Assert.That(block.AWLCode.Cast<S7FunctionBlockRow>().Select(row => row.MC7), Is.EqualTo(bytes));
        }

        [Test]
        public void ReplacesOrdinaryStaticAccessAndPreservesMC7()
        {
            var block = CreateBlock("L DIW[AR2,P#0.0]", "T MW 0");
            var rows = block.AWLCode.ToArray();
            var load = (S7FunctionBlockRow)rows[0];
            var bytes = load.MC7;

            FBStaticAccessConverter.ReplaceStaticAccess(block, null, new S7ConvertingOptions());

            Assert.That(block.AWLCode, Is.EqualTo(rows));
            Assert.That(load.Parameter, Is.EqualTo("#VALUE"));
            Assert.That(load.MC7, Is.SameAs(bytes));
        }

        [TestCase(PLCBlockType.FC, true)]
        [TestCase(PLCBlockType.FB, false)]
        public void LeavesCodeUnchangedWhenConversionDoesNotApply(PLCBlockType blockType, bool enabled)
        {
            var block = CreateBlock("LAR2", "TAR2", "L DIW[AR2,P#0.0]");
            block.BlockType = blockType;
            var rows = block.AWLCode.ToArray();

            FBStaticAccessConverter.ReplaceStaticAccess(block, null,
                new S7ConvertingOptions { ReplaceDIAccessesWithSymbolNames = enabled });

            Assert.That(block.AWLCode, Is.EqualTo(rows));
            Assert.That(((S7FunctionBlockRow)rows[2]).Parameter, Is.EqualTo("DIW[AR2,P#0.0]"));
        }

        private static S7FunctionBlock CreateBlock(params string[] instructions)
        {
            var block = new S7FunctionBlock { BlockType = PLCBlockType.FB };
            var root = new S7DataRow("ROOTNODE", S7DataRowType.STRUCT, block);
            var statics = new S7DataRow("STATIC", S7DataRowType.STRUCT, block);
            root.Add(statics);
            statics.Add(new S7DataRow("VALUE", S7DataRowType.INT, block));
            block.Parameter = root;
            block.AWLCode = new List<FunctionBlockRow>();
            foreach (var instruction in instructions)
            {
                var parts = instruction.Split(new[] { ' ' }, 2);
                var row = new S7FunctionBlockRow
                {
                    Command = parts[0],
                    Parameter = parts.Length > 1 ? parts[1] : ""
                };
                // MC7 has an internal setter; seed it as the decoder does.
                typeof(S7FunctionBlockRow).GetProperty("MC7").SetValue(row, new byte[] { 0x12, 0x34 });
                block.AWLCode.Add(row);
            }
            return block;
        }
    }
}
