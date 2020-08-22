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
            Assert.AreEqual(0, n.Add(n).DoubleValue);

            Assert.IsTrue(double.IsNaN(n.Add(JSUndefined.Value).DoubleValue));

            Assert.AreEqual("nullnull", n.Add("null").ToString());

            Assert.AreEqual("null 5", n.Add(new JSString(" 5")).ToString());

            Assert.AreEqual(5, n.Add(new JSNumber(5)).DoubleValue);

            Assert.AreEqual(5, n.Add(5).DoubleValue);
        }

        [TestMethod]
        public void Undefined()
        {
            var undefined = JSUndefined.Value;
            Assert.IsTrue(double.IsNaN(undefined.Add(4).DoubleValue));

            Assert.IsTrue(double.IsNaN(undefined.Add(undefined).DoubleValue));

            Assert.IsTrue(double.IsNaN(undefined.Add(new JSNumber(4)).DoubleValue));


            Assert.AreEqual("undefined 4", undefined.Add(" 4").ToString());

            Assert.AreEqual("undefined 4", undefined.Add(new JSString(" 4")).ToString());
        }

        [TestMethod]
        public void Object()
        {
            var obj = JSContext.Current.CreateObject();

            Assert.AreEqual("[object Object]4", obj.Add(4).ToString());

            Assert.AreEqual("[object Object]4", obj.Add("4").ToString());

            Assert.AreEqual("[object Object]4", obj.Add(new JSString("4")).ToString());
        }

        [TestMethod]
        public void Number()
        {
            var NaN = JSContext.Current.NaN;

            Assert.IsTrue(JSNumber.IsNaN(NaN.Add(NaN)));

            Assert.IsTrue(JSNumber.IsNaN(NaN.Add(JSNull.Value)));
            Assert.IsTrue(JSNumber.IsNaN(NaN.Add(JSUndefined.Value)));
            Assert.IsTrue(JSNumber.IsNaN(NaN.Add(JSContext.Current.One)));

            var One = JSContext.Current.One;

            Assert.AreEqual(One, One.Add(0));

            Assert.AreEqual(JSContext.Current.Two, One.Add(1));

            Assert.AreEqual(JSContext.Current.Two, One.Add(One));


            Assert.IsTrue(double.IsNaN(One.Add(JSUndefined.Value).DoubleValue));

            Assert.IsTrue(double.IsNaN(One.Add(JSUndefined.Value).DoubleValue));
        }

        [TestMethod]
        public void Boolean()
        {
            
            var True = JSContext.Current.True;
            var False = JSContext.Current.False;
            Assert.IsTrue(JSNumber.IsNaN(True.Add(JSContext.Current.NaN)));

            Assert.IsTrue(JSNumber.IsNaN(True.Add(JSUndefined.Value)));
            Assert.AreEqual(JSContext.Current.One, True.Add(JSNull.Value));
            Assert.AreEqual(JSContext.Current.Zero, False.Add(JSNull.Value));
            Assert.AreEqual(JSContext.Current.Two, (True.Add(JSContext.Current.One)));


        }
    }
}
