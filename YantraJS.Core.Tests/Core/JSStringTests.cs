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
function t() {
    
}

function* g1(n) {
    try {
        yield 1;
        t();
    } catch(e) {
        yield 2;
    }
}

var a = Array.from(g1(0));

assert.strictEqual('1', a.toString());
");
        }

    }
}
