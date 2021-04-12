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

            this.context.Eval(@"var a = {};
var n = 1;

Object.defineProperties(a, {
    b: {
        value: 1
    },
    c: {
        set: function (v) {
            n = v;
        },
        get: function () {
            return n;
        }
    }
});
assert.strictEqual(a.b, 1);


assert.strictEqual(a.c, 1);

n = 2;

assert.strictEqual(a.c, 2);
a.c = 4;
assert.strictEqual(n, 4);
assert.strictEqual(a.c, 4);
");
        }

    }
}
