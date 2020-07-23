using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Core
{
    [TestClass]
    public class JSArrayTests: BaseTest
    {

        [TestMethod]
        public void PushPop()
        {
            dynamic a = context.CreateArray();
            JSValue n = a.push(11, 22 ,44);

            Assert.AreEqual(3, n.IntValue);
            Assert.AreEqual(3, (a as JSValue).Length);

            n = a.push(55);

            Assert.AreEqual(4, n.IntValue);
            Assert.AreEqual(4, (a as JSValue).Length);

            n = a.pop();

            Assert.AreEqual(55, n.IntValue);
            Assert.AreEqual(3, (a as JSValue).Length);

            n = a.length;

            Assert.AreEqual(3, n.IntValue);

        }

        [TestMethod]
        public void Sparse()
        {
            var a = context.CreateArray();
            a.Length = 20;
            a[0] = context.CreateNumber(0);
            a[5] = context.CreateNumber(4);

            var list = a.All.ToList();
            Assert.AreEqual(20, list.Count);

            Assert.AreEqual(0, list[0].IntValue);
            Assert.AreEqual(4, list[5].IntValue);
            Assert.AreEqual(JSUndefined.Value, list[1]);
            Assert.AreEqual(JSUndefined.Value, list[19]);

            a[25] = context.CreateNumber(22);

            Assert.AreEqual(26, a.Length);

            list = a.All.ToList();
            Assert.AreEqual(26, list.Count);
            Assert.AreEqual(0, list[0].IntValue);
            Assert.AreEqual(4, list[5].IntValue);
            Assert.AreEqual(JSUndefined.Value, list[1]);
            Assert.AreEqual(JSUndefined.Value, list[19]);
            Assert.AreEqual(22, list[25].IntValue);
            Assert.AreEqual(JSUndefined.Value, a[109]);
            Assert.AreEqual(26, list.Count);
        }

        [TestMethod]
        public void SliceTest()
        {
            var a = context.CreateArray();
        }

    }
}
