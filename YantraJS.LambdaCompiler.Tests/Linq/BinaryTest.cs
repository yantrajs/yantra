using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YantraJS.Linq
{
    [TestClass]
    public class BinaryTest
    {

        [TestMethod]
        public void Add()
        {
            Expression<Func<int, int, int>> e = (a, b) => a + b;

            var fx = e.FastCompile();

            Assert.AreEqual(1, fx(1, 0));
            Assert.AreEqual(2, fx(1, 2));
        }

    }
}
