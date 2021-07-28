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
function* g(i) {
    if (i === 4)
        throw new Error('error');
    if(i)
        return yield 2;
    var a = new RegExp();
    return yield 3;
}

var a = Array.from(g());
assert.strictEqual(a.toString(), '3');

a = Array.from(g(1));
assert.strictEqual(a.toString(), '2');
");
        }

    }
}
