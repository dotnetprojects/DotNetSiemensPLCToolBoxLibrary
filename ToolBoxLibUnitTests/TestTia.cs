using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA;
using NUnit.Framework;

namespace ToolBoxLibUnitTests
{
    [TestFixture]
    public class TestTia
    {

        [Test]
        public void TiaTest1()
        {
            var prj = new Step7ProjectTiaBinaryParsed(".\\S7Blocks\\TestProjekt.zap19", null);

            var lst = prj.TiaObjectsList;
        }
    }
}
