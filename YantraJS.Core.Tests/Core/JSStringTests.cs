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
function add({ a = 1 }) {
    return a + a;
}

assert.strictEqual(4, add({ a: 2 }));
assert.strictEqual(2, add());

function addAll([a = 2, b = 2, c = 2] = [1, 1, 1]) {
    return a + b + c;
}

assert.strictEqual(6, addAll([1, 2, 3]));
assert.strictEqual(6, addAll([]));
assert.strictEqual(3, addAll());
");
        }

    }
}
