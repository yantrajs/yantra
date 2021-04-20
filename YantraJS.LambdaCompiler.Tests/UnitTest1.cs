using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace YantraJS.LambdaCompiler.Tests
{
    [TestClass]
    public class UnitTest1: BaseTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var x = LambdaRewriter.Rewrite<string,string>(x => this.Simple<string>(() => x == null ? x : null));

            Assert.IsNotNull(x);

        }

        
    }
}
