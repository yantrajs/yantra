using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Expressions;

namespace YantraJS.Linq
{
    [TestClass]
    public class BinaryTest
    {

        [TestMethod]
        public void Add()
        {
            var a = YExpression.Parameter(typeof(int));
            var b = YExpression.Parameter(typeof(int));

            var exp = YExpression.Lambda("add",
                YExpression.Binary(a, YOperator.Add, b), new YParameterExpression[] { 
                    a, b
                });

            var fx = exp.Compile<Func<int, int, int>>();

            Assert.AreEqual(1, fx(1, 0));
            Assert.AreEqual(3, fx(1, 2));
        }

    }

    [TestClass]
    public class ArrayTest
    {

        [TestMethod]
        public void Load()
        {
            var args = YExpression.Parameters(typeof(int[]), typeof(int));

            var exp = YExpression.Lambda("load", YExpression.ArrayIndex(
                args[0],
                args[1]
                ), args);

            var fx = exp.Compile<Func<int[], int, int>>();

            var n = new int[]{1,2,3 };

            Assert.AreEqual(1, fx(n, 0));
        }


        [TestMethod]
        public void Save()
        {


            var args = YExpression.Parameters(typeof(int[]), typeof(int), typeof(int));

            var ret = YExpression.Label("ret", typeof(int));

            var exp = YExpression.Lambda("save", 
                YExpression.Block(null, 
                    YExpression.Return(ret,
                        YExpression.Assign( 
                            YExpression.ArrayIndex(
                                args[0],
                                args[1]
                            ), 
                            args[2]
                        )
                    ),
                    YExpression.Label(ret, YExpression.Constant(0))
                ), args);

            var fx = exp.Compile<Func<int[], int, int, int>>();

            var n = new int[] { 1, 2, 3 };


            var m = fx(n, 1, 4);
            Assert.AreEqual(m, 4);
            Assert.AreEqual(n[1], 4);
        }

    }
}
