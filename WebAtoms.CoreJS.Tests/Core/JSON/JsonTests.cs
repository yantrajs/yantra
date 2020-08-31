using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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

            var aa = new JSArray();
            dynamic daa = aa;
            daa[2] = 2;
            da["c"] = aa;

            Assert.AreEqual("{\"a\":1,\"b\":2,\"c\":[null,null,2]}", stringify(a));
        }

        [TestMethod]
        public void ToJSONTest()
        {
            string stringify(JSValue a)
            {
                return JSJSON.Stringify(a);
            }

            var a = new JSObject();
            var f = new JSFunction((t, a) => new JSString("test"), "toJSON");
            dynamic da = a;
            da["data"] = "data";
            da["toJSON"] = f;

            Assert.AreEqual("{\"data\":\"data\"}", stringify(a));

            var aa = new JSObject();
            aa["obj"] = a;

            Assert.AreEqual("{\"obj\":\"test\"}", stringify(aa));

            var ar = new JSArray();
            ar[0] = a;

            Assert.AreEqual("[\"test\"]", stringify(ar));
        }

        [TestMethod]
        public void Replacer()
        {

            var JSON = DynamicContext.JSON;
            string stringify(object a, object replacer)
            {
                JSValue r = JSON.stringify(a, replacer);
                return r is JSUndefined ? null : r.ToString();
            }


            var a = new JSObject();
            dynamic da = a;
            da["a"] = "a";
            da["b"] = 1;

            var fx = new JSFunction((t, a) => {
                if (a[1] is JSString js)
                    return js;
                return JSUndefined.Value;
            });

            Assert.AreEqual("{\"a\":\"a\"}", stringify(a, fx));
            var ba = new JSArray(new JSString("b"));

            Assert.AreEqual("{\"b\":1}", stringify(a, ba));
        }

        [TestMethod]
        public void Indent()
        {

            var JSON = DynamicContext.JSON;
            string stringify(object a, object indent)
            {
                JSValue r = JSON.stringify(a, JSUndefined.Value, indent);
                return r is JSUndefined ? null : r.ToString();
            }


            var a = new JSObject();
            dynamic da = a;
            da["a"] = "a";
            // da["b"] = 1;

            var expected = JsonConvert.SerializeObject(new {
                a = "a"
            }, Formatting.Indented);

            Assert.AreEqual(expected, stringify(a, "  "));

            a["b"] = new JSObject();

            da.b.a = "b";

            expected = JsonConvert.SerializeObject(new {
                a = "a",
                b = new { 
                   a = "b"  
                }
            }, Formatting.Indented);

            Assert.AreEqual(expected, stringify(a, "  "));

            expected = JsonConvert.SerializeObject(new
            {
                a = "a",
                b = new
                {
                    a = "b"
                },
                c = new object[] { 1, 2, new {
                    c = "c"
                } }
            }, Formatting.Indented);

            da.c = new JSArray();

            da.c[0] = 1;

            da.c[1] = 2;

            da.c[2] = new JSObject();

            da.c[2].c = "c";

            Assert.AreEqual(expected, stringify(a, "  "));

        }

    }
}
