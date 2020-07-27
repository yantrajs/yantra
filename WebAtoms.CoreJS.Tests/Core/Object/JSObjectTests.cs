using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Core.Object
{
    [TestClass]
    public class JSObjectTests: BaseTest
    {

        [TestMethod]
        public void AssignTest()
        {
            var Object = DynamicContext.Object;
            JSValue Assign(object t, object s)
            {
                return Object.assign(t, s);
            }

            Assert.ThrowsException<JSException>(
                () => Assign(JSNull.Value, JSNull.Value),
                JSError.Cannot_convert_undefined_or_null_to_object);
            Assert.ThrowsException<JSException>(
                () => Assign(JSUndefined.Value, JSNull.Value), 
                JSError.Cannot_convert_undefined_or_null_to_object);

            Assert.AreEqual(2, Assign(2, JSNull.Value).IntValue);
            Assert.AreEqual(2, Assign(2, JSUndefined.Value).IntValue);

            var source = new JSObject(
                JSProperty.Property("b", new JSNumber(4)),
                JSProperty.Property("c", new JSNumber(5))
            );

            Assert.AreEqual(2, Assign(2, source).IntValue);

            var target = new JSObject(
                JSProperty.Property("a", new JSNumber(1)),
                JSProperty.Property("b", new JSNumber(2))
            );

            var r = Assign(target, source);

            // Assert.AreEqual("", r.ToDetailString());


        }

        [TestMethod]
        public void CreateTest()
        {

        }

        [TestMethod]
        public void FromEntriesTest()
        {

        }

    }
}
