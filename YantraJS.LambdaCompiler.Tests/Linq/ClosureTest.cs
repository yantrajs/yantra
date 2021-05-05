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
        public void TwoLevels()
        {
            var a = YExpression.Parameters(typeof(int));

            var b = YExpression.Parameters(typeof(int));

            var c = YExpression.Parameters(typeof(int));


            var lamba = YExpression.Lambda<Func<int, Func<int, Func<int, int>>>>("c",
                    YExpression.Lambda<Func<int, Func<int, int>>>("b",
                        YExpression.Lambda<Func<int, int>>("inner",
                            a[0] + b[0] + c[0],
                            c
                        ), b
                    ),
                    a
                );

            var fx1 = lamba.CompileInAssembly();

            var fx2 = fx1(1);

            var fx3 = fx2(2);

            var r = fx3(3);

            Assert.AreEqual(6, r);

        }


        [TestMethod]
        public void OneLevel()
        {
            var a = YExpression.Parameters(typeof(int));

            var b = YExpression.Parameters(typeof(int));

            var a1 = YExpression.Lambda<Func<int, Func<int, int>>>("a1",
                YExpression.Lambda<Func<int, int>>("a2",
                    YExpression.Binary(a[0], YOperator.Add, b[0]), b
                )
                , a);

            Func<int, Func<int, int>> fx = a1.CompileWithNestedLambdas();

            var f1 = fx(1);

            Assert.AreEqual(2, f1(1));
        }

        [TestMethod]
        public void OneLevelCompiled()
        {
            var a = YExpression.Parameters(typeof(int));

            var b = YExpression.Parameters(typeof(int));

            var a1 = YExpression.Lambda<Func<int, Func<int, int>>>("a1",
                YExpression.Lambda<Func<int, int>>("a2",
                    YExpression.Binary(a[0], YOperator.Add, b[0]), b
                )
                , a);

            Func<int, Func<int, int>> fx = a1.CompileInAssembly();

            var f1 = fx(1);

            Assert.AreEqual(2, f1(1));
        }

    }
}
