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
            string Expo(double x, double f) {
                return context["Number"]
                    .InvokeMethod("parseFloat", JSArguments.From(x))
                    .InvokeMethod("toExponential", JSArguments.From(f))
                    .ToString();
            }

            Assert.AreEqual("1.23e+5", Expo(123456, 2));

        }

    }
}
