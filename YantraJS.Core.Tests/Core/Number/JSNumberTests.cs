using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Tests.Core.Number
{
    [TestClass]
    public class JSNumberTests: BaseTest
    {

        [TestMethod]
        public void ExponentialTest()
        {
            var Number = DynamicContext.Number;
            string Expo(JSValue x, JSValue f = null) {
                f = f ?? JSUndefined.Value;
                return Number
                    .parseFloat(x)
                    .toExponential(f)
                    .ToString();
            }

            dynamic n = new JSNumber(123456);
            Assert.AreEqual("1.23e+5", Expo(n, new JSNumber(2)));
            Assert.AreEqual("1.23456e+5", Expo(n));
            Assert.AreEqual("NaN", Expo(new JSString("oink")));

        }

        [TestMethod]
        public void Fixed()
        {
            var Number = DynamicContext.Number;
            string financial(object x)
            {
                JSString d = Number.parseFloat(x).toFixed(2);
                return d.ToString();
            }

            Assert.AreEqual("123.46", financial(123.456));
            Assert.AreEqual("0.00", financial(0.004));
            Assert.AreEqual("123000.00", financial("1.23e+5"));
        }

        [TestMethod]
        public void Precision()
        {
            var Number = DynamicContext.Number;
            string Precision(object x)
            {
                JSString s = Number.parseFloat(x).toPrecision(4);
                return s.ToString();
            }

            Assert.AreEqual("123.5", Precision(123.456));
            Assert.AreEqual("0.004000", Precision(0.004));
            Assert.AreEqual("1.230e+5", Precision("1.23e+5"));
        }

        [TestMethod]
        public void FiniteTest()
        {
            var Number = DynamicContext.Number;
            var i = double.PositiveInfinity;
            JSBoolean b = Number.isFinite(i);


            Assert.IsFalse(b.BooleanValue);
            b = Number.isFinite(4);
            Assert.IsTrue(b.BooleanValue);
        }

        [TestMethod]
        public void IsIntegerTest()
        {
            var Number = DynamicContext.Number;
            bool Fits(double x, double y)
            {
                JSValue b = Number.isInteger(y / x);
                return b.BooleanValue;
            }

            Assert.IsTrue(Fits(5, 10));
            Assert.IsFalse(Fits(7, 10));
        }

        [TestMethod]
        public void NaNTest()
        {
            var Number = DynamicContext.Number;
            bool IsNaN(object x)
            {
                JSBoolean b = Number.isNaN(x);
                return b._value;
            }

            Assert.IsTrue(IsNaN(double.NaN));
            Assert.IsFalse(IsNaN(4));

            Assert.IsTrue(IsNaN(Number.parseFloat("ab")));

        }

        [TestMethod]
        public void IsSafeInteger()
        {
            var Number = DynamicContext.Number;
            bool IsSafeInteger(object x)
            {
                JSBoolean b = Number.isSafeInteger(x);
                return b._value;
            }

            var lossy = (double)Math.Pow((double)2, (double)53);
            Assert.IsFalse(IsSafeInteger(lossy));
            Assert.IsTrue(IsSafeInteger(lossy - (double)1));

        }

        [TestMethod]
        public void ParseFloat()
        {
            var Number = DynamicContext.Number;
            bool IsNumber(object x)
            {
                JSNumber n = Number.parseFloat(x);
                return !double.IsNaN(n.value);
            }

            Assert.IsFalse(IsNumber("a"));
            Assert.IsTrue(IsNumber("1.34343abcd"));
            Assert.IsTrue(IsNumber("1.34343eabcd"));
            Assert.IsTrue(IsNumber("1.34343e+abcd"));
            Assert.IsTrue(IsNumber("1.34343e+1abcd"));
        }


        [TestMethod]
        public void ParseInteger()
        {
            var Number = DynamicContext.Number;
            bool IsNumber(object x)
            {
                JSNumber n = Number.parseInt(x);
                return !double.IsNaN(n.value);
            }

            Assert.IsFalse(IsNumber("a"));
            Assert.IsTrue(IsNumber("1g.34343abcd"));
            Assert.IsTrue(IsNumber("1.34343eabcd"));
            Assert.IsTrue(IsNumber("1.34343e+abcd"));
            Assert.IsTrue(IsNumber("1.34343e+1abcd"));

            
        }
    }
}
