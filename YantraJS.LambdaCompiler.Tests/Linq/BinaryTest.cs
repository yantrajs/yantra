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

    }
}
