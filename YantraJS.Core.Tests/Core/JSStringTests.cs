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
function* m(n, r) {
    // var r1 = yield n();
    r(yield n());
}

function nn() {
    return 1;
}

var r2 = [];

function rr(a) {
    console.log(a);
    r2.push(a);
}

var r1 = m(nn, rr);

var r3 = r1.next(2);
assert.strictEqual(r3.value, 1);
r3 = r1.next(2);
assert.strictEqual(r3.done, true);

assert.strictEqual(r2[0], 2);


");
        }

    }
}
