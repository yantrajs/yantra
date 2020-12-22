using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.LinqExpressions.Generators;

namespace YantraJS.Core.Tests.Core.Generators
{
    [TestClass]
    public class GeneratorTests
    {

        [TestMethod]
        public void TestSimple()
        {
            var g = new ClrGenerator();
            int a = 0;
            g.Build(g.Block(
                g.Yield(() => new JSNumber(a++)),
                g.Yield(() => new JSNumber(a++))
            ));
            JSValue n = null;
            Assert.IsTrue(g.Next(n, out n));
            Assert.AreEqual(0, n.IntValue);
            Assert.IsTrue(g.Next(n, out n));
            Assert.AreEqual(1, n.IntValue);
            Assert.IsFalse(g.Next(n, out n));
        }

        [TestMethod]
        public void TestLoop()
        {
            var g = new ClrGenerator();
            int a = 0;

            var @break = g.NewLabel();

            var @continue = g.NewLabel();

            // only iterate even numbers... till 6
            /**
             * function () {
             *    var a = 0;
             *    while(1) {
             *        if (a % 2 == 1)
             *            continue;
             *        yield a;
             *        if (a == 6)
             *            break;
             *    }
             *    yield -1;
             * }
             */

            g.Build(
                g.Block(
                    g.Loop(g.Block(
                        () => {
                            a++;
                            return null;
                        },
                        g.If(() => a % 2 == 1, g.Goto(@continue)),
                        g.Yield(() => new JSNumber(a)),
                        g.If(() => a == 6, g.Goto(@break))
                    ), @break, @continue),
                g.Yield(() => new JSNumber(-1))
                )
            );
            JSValue n = null;
            for (int i = 2; i <= 6; i += 2)
            {
                Assert.IsTrue(g.Next(n, out n));
                Assert.AreEqual(i, n.IntValue);
            }
            Assert.IsTrue(g.Next(n, out n));
            Assert.AreEqual(-1, n.IntValue);
            Assert.IsFalse(g.Next(n, out n));
        }


    }
}
