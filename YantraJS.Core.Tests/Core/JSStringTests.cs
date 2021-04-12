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

            this.context.Eval(@"
function a(a, ...c) {
    return c.join(',');
}

assert.strictEqual('', a(1));
assert.strictEqual('2', a(1, 2));
assert.strictEqual('2,3', a(1, 2, 3));

function b(a, b, ...c) {
    return c.join(',');
}

assert.strictEqual('', b(1));
assert.strictEqual('', b(1, 2));
assert.strictEqual('3', b(1, 2, 3));
assert.strictEqual('3,4', b(1, 2, 3, 4));
var cc = [3,4];
assert.strictEqual('3,4', b(1, 2, ... cc));
");
        }

    }
}
