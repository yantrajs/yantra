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

            this.context.Eval(@"let a = [1,2,3];

let b = [];
for(let n of a) {
    b.push(n);
}

assert.strictEqual('1,2,3', b.toString());

b = [];

for (let n of a) {
    b.push(() => n);
}

b = b.map(n => n());
assert.strictEqual('1,2,3', b.toString());

a = [[1, 1], [2, 2], [3, 3]];
b = [];
for (let [n, m] of a) {
    b.push(() => n + m);
}
b = b.map(n => n());
assert.strictEqual('2,4,6', b.toString());

a = [{ n: 1, m: 1 }, { n: 2, m: 2 }, { n: 3, m: 3 }];
b = [];
for (let { n, m } of a) {
    b.push(() => n + m);
}
b = b.map(n => n());
assert.strictEqual('2,4,6', b.toString());

");
        }

    }
}
