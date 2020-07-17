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
        public void Substring()
        {
            var js = context.CreateString("akash");

            Assert.AreEqual(5, js.Length);

            var jsLength = js[KeyStrings.length];

            Assert.AreEqual(5, jsLength.IntValue);

            var zero = new JSNumber(0);
            var length = new JSNumber(2);
            var prefix = js.InvokeMethod(KeyStrings.GetOrCreate("substr"), JSArguments.From(zero, length));

            Assert.AreEqual(2, prefix.Length);

            Assert.AreEqual("ak", prefix.ToString());

        }

    }
}
