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
            this.context["array"] = new JSArray( new JSNumber(1) );
            this.context.Eval(@"
//let date = new Date(1970, 1, 1, 0, 0, 0);
// Sun Feb 01 1970 00:00:00 GMT+0530 (India Standard Time)
//  2658600000 =  date.getTime()
date = new Date(2658600000);
//date.setTime(0);
assert.strictEqual(0, date.setTime(0));

assert(isNaN(date.setTime(NaN)));

date = new Date(2658600000);

assert.strictEqual(1, date.setTime(1.123456));
assert.strictEqual(1, date.setTime(1.8));
assert.strictEqual(-1, date.setTime(-1.123456));
assert.strictEqual(-1, date.setTime(-1.8));
assert(isNaN(date.setTime(9e15)));
date = new Date(2658600000);
assert(isNaN(date.setTime(Infinity)));
date = new Date(2658600000);
assert.strictEqual(Number.POSITIVE_INFINITY, 1 / date.setTime(-0));
assert.strictEqual(1, date.setTime.length);
");
        }

    }
}
