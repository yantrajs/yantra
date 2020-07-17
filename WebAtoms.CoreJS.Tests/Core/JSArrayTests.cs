using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var a = context.CreateArray();
            var n = a.InvokeMethod(JSArray.push.key, JSArguments.From(11, 22 ,44));

            Assert.AreEqual(3, n.IntValue);
            Assert.AreEqual(3, a.Length);

            n = a.InvokeMethod(JSArray.push.key, JSArguments.From(55));

            Assert.AreEqual(4, n.IntValue);
            Assert.AreEqual(4, a.Length);

            n = a.InvokeMethod(JSArray.pop.key, JSArguments.Empty);

            Assert.AreEqual(55, n.IntValue);
            Assert.AreEqual(3, a.Length);

            Assert.AreEqual(3, a[JSArray.length.key].IntValue);

        }

    }
}
