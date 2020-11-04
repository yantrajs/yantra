using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using YantraJS.Core;

namespace YantraJS.Tests.Core.JSON
{
    [TestClass]
    public class JsonTests: BaseTest
    {

        [TestMethod]
        public void StringifyTest()
        {
            var JSON = DynamicContext.JSON;
            string stringify(object a1)
            {
                JSValue r = JSON.stringify(a1);
                return r.IsUndefined ? null : r.ToString();
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
            string stringify(JSValue a1)
            {
                return JSJSON.Stringify(a1);
            }

            var a = new JSObject();
            var f = new JSFunction((in Arguments a1) => new JSString("test"), "toJSON");
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
            string stringify(object a1, object replacer)
            {
                JSValue r = JSON.stringify(a1, replacer);
                return r.IsUndefined ? null : r.ToString();
            }


            var a = new JSObject();
            dynamic da = a;
            da["a"] = "a";
            da["b"] = 1;

            var fx = new JSFunction((in Arguments a1) => {
                if (a1.Get2().Item2 is JSString js)
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
            string stringify(object a1, object i)
            {
                JSValue r = JSON.stringify(a1, JSUndefined.Value, i);
                return r.IsUndefined ? null : r.ToString();
            }

            var indent = new JsonSerializerOptions { WriteIndented = true };

            var a = new JSObject();
            dynamic da = a;
            da["a"] = "a";
            // da["b"] = 1;

            var expected = JsonSerializer.Serialize(new {
                a = "a"
            }, indent);

            Assert.AreEqual(expected, stringify(a, "  "));

            a["b"] = new JSObject();

            da.b.a = "b";

            expected = JsonSerializer.Serialize(new {
                a = "a",
                b = new { 
                   a = "b"  
                }
            }, indent);

            Assert.AreEqual(expected, stringify(a, "  "));

            expected = JsonSerializer.Serialize(new
            {
                a = "a",
                b = new
                {
                    a = "b"
                },
                c = new object[] { 1, 2, new {
                    c = "c"
                } }
            }, indent);

            da.c = new JSArray();

            da.c[0] = 1;

            da.c[1] = 2;

            da.c[2] = new JSObject();

            da.c[2].c = "c";

            Assert.AreEqual(expected, stringify(a, "  "));

        }

    }
}
