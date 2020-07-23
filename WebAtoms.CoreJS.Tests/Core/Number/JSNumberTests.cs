using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Core.Number
{
    [TestClass]
    public class JSNumberTests: BaseTest
    {

        [TestMethod]
        public void ExponentialTest()
        {
            string Expo(JSValue x, JSValue f = null) {
                f = f ?? JSUndefined.Value;
                dynamic c = context;
                return c.Number.parseFloat(x).toExponential(f).ToString();
            }

            dynamic n = new JSNumber(123456);
            Assert.AreEqual("1.23e+5", Expo(n, new JSNumber(2)));
            Assert.AreEqual("1.23456e+5", Expo(n));
            Assert.AreEqual("NaN", Expo(new JSString("oink")));

        }

    }
}
