using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Tests.Core
{
    [TestClass]
    public class JSArrayTests: BaseTest
    {

        [TestMethod]
        public void PushPop()
        {
            JSValue a = new JSArray();
            dynamic da = a;
            JSValue n = da.push(11, 22 ,44);

            Assert.AreEqual(3, n.IntValue);
            Assert.AreEqual(3, a.Length);

            n = da.push(55);

            Assert.AreEqual(4, n.IntValue);
            Assert.AreEqual(4, a.Length);

            n = da.pop();

            Assert.AreEqual(55, n.IntValue);
            Assert.AreEqual(3, a.Length);

            n = da.length;

            Assert.AreEqual(3, n.IntValue);

        }

        [TestMethod]
        public void Sparse()
        {
            var a = new JSArray();
            a.Length = 20;
            a[0] = new JSNumber(0);
            a[5] = new JSNumber(4);

            var list = a.GetArrayElements().ToList();
            Assert.AreEqual(20, list.Count);

            Assert.AreEqual(0, list[0].value.IntValue);
            Assert.AreEqual(4, list[5].value.IntValue);
            Assert.AreEqual(JSUndefined.Value, list[1].value);
            Assert.AreEqual(JSUndefined.Value, list[19].value);

            a[25] = new JSNumber(22);

            Assert.AreEqual(26, a.Length);

            list = a.GetArrayElements().ToList();
            Assert.AreEqual(26, list.Count);
            Assert.AreEqual(0, list[0].value.IntValue);
            Assert.AreEqual(4, list[5].value.IntValue);
            Assert.AreEqual(JSUndefined.Value, list[1].value);
            Assert.AreEqual(JSUndefined.Value, list[19].value);
            Assert.AreEqual(22, list[25].value.IntValue);
            Assert.AreEqual(JSUndefined.Value, a[109]);
            Assert.AreEqual(26, list.Count);
        }

        [TestMethod]
        public void SliceTest()
        {
            var a = new JSArray();
        }

    }
}
