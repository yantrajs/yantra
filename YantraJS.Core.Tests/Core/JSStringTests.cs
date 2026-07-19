using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;
using YantraJS.Generator;

namespace YantraJS.Tests.Core
{
    [TestClass]
    public class CodeTest: BaseTest
    {
        [TestMethod]
        public void Function()
        {

            ILCodeGenerator.GenerateLogs = true;

            // this.context.Eval("class A { constructor(a) { this.a = a; } } class B extends A { constructor(a) { super(a); } }");
            // Assert.AreEqual(1, context.Eval("x = {get f() { return 1; }}; x.f = 5; x.f"));
            // this.context["array"] = new JSArray( new JSNumber(1) );
            //            this.context.Eval(@"

            //(function(){return 1; /***/ })()
            //");
            this.context.Execute(@"
class A {

    aa = 4;

    constructor(a) {
        this.a = a;
    }
}

class B extends A {

}

class C extends A {
    constructor(a, c) {
        super(a);
        this.c = c;
    }
}

var b = new B(1);

assert.strictEqual(b.a, 1);

var c = new C(1, 2);
assert.strictEqual(c.a, 1);
assert.strictEqual(c.c, 2);


assert.strictEqual(c.aa, 4);
assert.strictEqual(b.aa, 4);


");

}

    }
}
