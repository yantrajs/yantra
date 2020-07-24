using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Core.JSON
{
    [TestClass]
    public class JsonTests: BaseTest
    {

        [TestMethod]
        public void StringifyTest()
        {
            var JSON = DynamicContext.JSON;
            string stringify(object a)
            {
                JSValue r = JSON.stringify(a);
                return r is JSUndefined ? null : r.ToString();
            }

            Assert.IsNull(stringify(JSUndefined.Value));

            Assert.AreEqual("null", stringify(JSNull.Value));
            Assert.AreEqual("\"a\"", stringify("a"));

            Assert.AreEqual("5", stringify(5));

            Assert.AreEqual("true", stringify(true));
            Assert.AreEqual("false", stringify(false));

            var a = new JSObject();
            dynamic da = a;
            da["a"] = 1;
            da["b"] = 2;

            Assert.AreEqual("{\"a\":1,\"b\":2}", stringify(a));
        }

    }
}
