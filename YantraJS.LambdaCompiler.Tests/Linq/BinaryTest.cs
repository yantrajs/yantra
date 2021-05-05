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
    public class BinaryTest
    {

        [TestMethod]
        public void Add()
        {
            var a = YExpression.Parameter(typeof(int));
            var b = YExpression.Parameter(typeof(int));

            var exp = YExpression.Lambda<Func<int,int,int>>("add",
                YExpression.Binary(a, YOperator.Add, b), new YParameterExpression[] { 
                    a, b
                });

            var fx = exp.Compile();

            Assert.AreEqual(1, fx(1, 0));
            Assert.AreEqual(3, fx(1, 2));
        }

        [TestMethod]
        public void NestedConditions()
        {
            var a = YExpression.Parameters(typeof(int), typeof(int), typeof(int));
            var a1 = a[0];
            var a2 = a[1];
            var a3 = a[2];

            var exp = YExpression.Lambda<Func<int, int, int, int>>("con",
                YExpression.Conditional( a1 > a2 ,
                    YExpression.Conditional( a1 > a3,
                        a1,
                        a3),
                    a2)
                , a);

            var f = exp.CompileInAssembly();

            Assert.AreEqual(10, f(1, 10, 2));
        }
    }
}
