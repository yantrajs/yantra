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
class Shape {

    constructor(name) {
        this.n = name;
    }

    get name() {
        return this.n;
    }
    set name(v) {
        this.n = v;
    }
}

let s = new Shape('shape');

assert.strictEqual('shape', s.n);
assert.strictEqual('shape', s.name);

class Circle extends Shape {

    get name() {
        return super.name + '$' + super.name;
    }

}

s = new Circle('circle');
assert.strictEqual('circle', s.n);
assert.strictEqual('circle$circle', s.name);
");
        }

    }
}
