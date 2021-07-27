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
class BaseClass {
    m() {
        return 'base';
    }
}

class ChildClass extends BaseClass {

    constructor() {
        super();

        this.n = this.m();
    }
}

var c = new ChildClass();

assert.strictEqual(c.n, 'base');

assert.strictEqual(Object.getPrototypeOf(ChildClass), BaseClass);

class Shape {

    constructor(n) {
        this.name = n;
    }

}

class Circle extends Shape {
    constructor() {
        super(...arguments);
    }
}

let c = new Circle('Circle');
assert.strictEqual('Circle', c.name);
");
        }

    }
}
