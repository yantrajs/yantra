using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using YantraJS.Core;
using YantraJS.Expressions;
using YantraJS.Runtime;

namespace YantraJS.Linq
{
    [TestClass]
    public class RecursiveTests {

        [TestMethod]
        public void Fib()
        {
            var fibs = YExpression.Parameters(typeof(Func<int,int>));
            var fib = fibs[0];

            var fibp = YExpression.Parameters(typeof(int));
            var p1 = fibp[0];

            var f = YExpression.Lambda<Func<Func<int,int>>>("fib_outer", 
                YExpression.Block(
                    fibs.AsSequence(),
                    YExpression.Assign(fib, YExpression.Lambda<Func<int, int>>("fib",
                        YExpression.Block(
                                YExpression.Conditional(
                                    p1 <= 1, 
                                    YExpression.Constant(0),
                                    YExpression.Conditional(
                                        YExpression.Equal(p1, YExpression.Constant(2)), 
                                        YExpression.Constant(1),
                                            YExpression.Invoke(fib, p1 - 1)
                                            +
                                            YExpression.Invoke(fib, p1 - 2)
                                        ))
                            )
                        ,
                        fibp) )
                    )
                ); ;

            var outer = f.CompileInAssembly();

            var fx = outer();

            Assert.AreEqual(3, fx(5));

        }
    }
}
