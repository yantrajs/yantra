using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YantraJS.Core.Tests.Parser
{
    [TestClass]
    public class ForTest : BaseParserTest
    {

        [TestMethod]
        public void ForLet()
        {
            var p = Parse(@"
for(let i = 0;i<10;i++) {
    console.log(i);
}
");
            Assert.IsTrue(true);
        }


        [TestMethod]
        public void ForOfLet()
        {
            var p = Parse(@"
for(let {a,b} of m) {
    a = a+i;
}
");
            Assert.IsTrue(true);
        }

    }
}
