using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Expressions;
using YantraJS.Runtime;

namespace YantraJS.Linq
{
    [TestClass]
    public class ClosureTest
    {

        [TestMethod]
        public void OneLevel()
        {
            var a = YExpression.Parameters(typeof(int));

            var b = YExpression.Parameters(typeof(int));

            var a1 = YExpression.Lambda("a1",
                YExpression.Lambda("a2",
                    YExpression.Binary(a[0], YOperator.Add, b[0]),b
                )
                , a);

            Func<int, Func<int,int>> fx = a1.CompileWithNestedLambdas<Func<int, Func<int,int>>>();

            var f1 = fx(1);

            Assert.AreEqual(2, f1(1));
        }

        [TestMethod]
        public void OneLevelCompiled()
        {
            var a = YExpression.Parameters(typeof(int));

            var b = YExpression.Parameters(typeof(int));

            var a1 = YExpression.Lambda("a1",
                YExpression.Lambda("a2",
                    YExpression.Binary(a[0], YOperator.Add, b[0]), b
                )
                , a);

            Func<int, Func<int, int>> fx = a1.CompileInAssembly<Func<int, Func<int, int>>>();

            var f1 = fx(1);

            Assert.AreEqual(2, f1(1));
        }

    }
}
