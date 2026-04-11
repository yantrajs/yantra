using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Tests.Core.Object
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

            Assert.Throws<JSException>(
                () => Assign(JSNull.Value, JSNull.Value),
                JSError.Cannot_convert_undefined_or_null_to_object);
            Assert.Throws<JSException>(
                () => Assign(JSUndefined.Value, JSNull.Value), 
                JSError.Cannot_convert_undefined_or_null_to_object);

            Assert.AreEqual(2, Assign(2, JSNull.Value).IntValue);
            Assert.AreEqual(2, Assign(2, JSUndefined.Value).IntValue);

            var source = JSObject.NewWithProperties()
                .AddProperty("c".ToKeyString(), new JSNumber(4))
                .AddProperty("d".ToKeyString(), new JSNumber(5));

            Assert.AreEqual(2, Assign(2, source).IntValue);

            var target = JSObject.NewWithProperties()
                .AddProperty("a".ToKeyString(), new JSNumber(1))
                .AddProperty("b".ToKeyString(), new JSNumber(2));

            var r = Assign(target, source);

            Assert.AreEqual(target["c".ToKeyString()], source["c".ToKeyString()]);
            Assert.AreEqual(target["d".ToKeyString()], source["d".ToKeyString()]);


        }

        [TestMethod]
        public void Function()
        {
            var a = CoreScript.Evaluate("Function('return this;')()");
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public void EqualsTest()
        {
            //var a = CoreScript.Evaluate("[2]");
            //var obj = context["object"];
            //var p = context["Object"].InvokeMethod("getPrototypeOf", new Arguments(obj, a));

            //var ap = context["Array"]["prototype"];

            //Assert.IsTrue(p.StrictEquals(ap).BooleanValue);

            CoreScript.Evaluate(@"var a = [1];
var p = Object.getPrototypeOf(a);
assert(p === Array.prototype, p);");
        }


    }
}
