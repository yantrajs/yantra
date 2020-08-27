using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Core.Object
{
    [TestClass]
    public class AddTests: BaseTest
    {

        [TestMethod]
        public void Null()
        {
            var n = JSNull.Value;
            Assert.AreEqual(0, n.AddValue(n).DoubleValue);

            Assert.IsTrue(double.IsNaN(n.AddValue(JSUndefined.Value).DoubleValue));

            Assert.AreEqual("nullnull", n.AddValue("null").ToString());

            Assert.AreEqual("null 5", n.AddValue(new JSString(" 5")).ToString());

            Assert.AreEqual(5, n.AddValue(new JSNumber(5)).DoubleValue);

            Assert.AreEqual(5, n.AddValue(5).DoubleValue);
        }

        [TestMethod]
        public void Undefined()
        {
            var undefined = JSUndefined.Value;
            Assert.IsTrue(double.IsNaN(undefined.AddValue(4).DoubleValue));

            Assert.IsTrue(double.IsNaN(undefined.AddValue(undefined).DoubleValue));

            Assert.IsTrue(double.IsNaN(undefined.AddValue(new JSNumber(4)).DoubleValue));


            Assert.AreEqual("undefined 4", undefined.AddValue(" 4").ToString());

            Assert.AreEqual("undefined 4", undefined.AddValue(new JSString(" 4")).ToString());
        }

        [TestMethod]
        public void Object()
        {
            var obj = JSContext.Current.CreateObject();

            Assert.AreEqual("[object Object]4", obj.AddValue(4).ToString());

            Assert.AreEqual("[object Object]4", obj.AddValue("4").ToString());

            Assert.AreEqual("[object Object]4", obj.AddValue(new JSString("4")).ToString());
        }

        [TestMethod]
        public void Number()
        {
            var NaN = JSContext.Current.NaN;

            Assert.IsTrue(JSNumber.IsNaN(NaN.AddValue(NaN)));

            Assert.IsTrue(JSNumber.IsNaN(NaN.AddValue(JSNull.Value)));
            Assert.IsTrue(JSNumber.IsNaN(NaN.AddValue(JSUndefined.Value)));
            Assert.IsTrue(JSNumber.IsNaN(NaN.AddValue(JSContext.Current.One)));

            var One = JSContext.Current.One;

            Assert.AreEqual(One, One.AddValue(0));

            Assert.AreEqual(JSContext.Current.Two, One.AddValue(1));

            Assert.AreEqual(JSContext.Current.Two, One.AddValue(One));


            Assert.IsTrue(double.IsNaN(One.AddValue(JSUndefined.Value).DoubleValue));

            Assert.IsTrue(double.IsNaN(One.AddValue(JSUndefined.Value).DoubleValue));
        }

        [TestMethod]
        public void Boolean()
        {
            
            var True = JSContext.Current.True;
            var False = JSContext.Current.False;
            Assert.IsTrue(JSNumber.IsNaN(True.AddValue(JSContext.Current.NaN)));

            Assert.IsTrue(JSNumber.IsNaN(True.AddValue(JSUndefined.Value)));
            Assert.AreEqual(JSContext.Current.One, True.AddValue(JSNull.Value));
            Assert.AreEqual(JSContext.Current.Zero, False.AddValue(JSNull.Value));
            Assert.AreEqual(JSContext.Current.Two, (True.AddValue(JSContext.Current.One)));


        }
    }
}
