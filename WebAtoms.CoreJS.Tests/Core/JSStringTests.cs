using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Core
{
    [TestClass]
    public class JSStringTests: BaseTest
    {
        [TestMethod]
        public void Index()
        {
            CoreScript.Evaluate(@"var a = 'akash';
assert(a[1] === 'k', a[1]); ");
        }

        [TestMethod]
        public void Substring()
        {
            var js = new JSString("akash");

            Assert.AreEqual(5, js.Length);

            var jsLength = js[KeyStrings.length];

            Assert.AreEqual(5, jsLength.IntValue);

            var zero = new JSNumber(0);
            var length = new JSNumber(2);
            var prefix = js.InvokeMethod("substr", zero, length);

            Assert.AreEqual(2, prefix.Length);

            Assert.AreEqual("ak", prefix.ToString());

        }

    }
}
