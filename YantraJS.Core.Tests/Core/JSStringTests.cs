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
var a = [];
function b() { return 1 }
for(var i =0, j=b();i<10;i++,j++) {
    a.push(j);
}
console.log(a.toString());
assert.strictEqual(a.toString(), '1,2,3,4,5,6,7,8,9,10');
");
        }

    }
}
