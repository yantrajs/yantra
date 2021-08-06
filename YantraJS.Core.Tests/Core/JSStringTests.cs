using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Tests.Core
{
    [TestClass]
    public class CodeTest: BaseTest
    {
        [TestMethod]
        public void Function()
        {

            // this.context.Eval("class A { constructor(a) { this.a = a; } } class B extends A { constructor(a) { super(a); } }");
            // Assert.AreEqual(1, context.Eval("x = {get f() { return 1; }}; x.f = 5; x.f"));
            this.context["array"] = new JSArray().Add(new JSNumber(1));
            this.context.Eval(@"
function* g1(n) {
    try {
        yield 1;
    } catch(e) {
        yield e;
    }
}

var g = g1(1);
var a = g.next();
assert.strictEqual(1, a.value);
g.throw(new Error('2'));
a = g.next();
assert.strictEqual('2', a.value.toString());

");
        }

    }
}
