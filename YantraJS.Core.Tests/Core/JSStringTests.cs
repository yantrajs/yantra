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
var a = { a: 1, b: 2 };
var r = [];

function* m() {
    yield 1;
    if (a) {
        for (var i in a) {
            if (a.hasOwnProperty(i)) {
                if (i === 'a')
                    continue;
                try {
                    r.push(i);
                    yield 2;
                } catch (e) {
                    console.log(e);
                }
            }
        }
    }
    yield 3;
}
assert.strictEqual(Array.from(m()).toString(), '1,2,3');
");
        }

    }
}
