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
class SP {

    constructor() {
        this.instances = {};
    }

    put(key, value) {
        var v = this.instances[key.id];
        if (!v) {
            this.instances[key.id] = value;
        } else {
            console.log(`v is ${v}`);
        }
    }

    get(key) {
        return this.instances[key.id];
    }

}

let s = new SP();

s.put({ id: 'a1' }, 'a1');

let a1 = s.get({ id: 'a1' });

assert.strictEqual(a1, 'a1');
assert.strictEqual(s.instances['a1'], 'a1');
");
        }

    }
}
