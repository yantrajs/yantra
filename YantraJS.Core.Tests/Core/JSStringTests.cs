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
var a = [1, 2, 3, 4];

var [a1, a2, ...all] = a;

assert.strictEqual(1, a1);
assert.strictEqual(2, a2);
assert.strictEqual('3,4', all.toString());");
//            this.context.Eval(@"
//(function () {
//    var a = 1;
//    var b = '';
//    while (a < 10) {
//        a++;
//        b += a;
//    }
//    assert.strictEqual('2345678910', b);
//})();
//(function () {
//    var a = 1;
//    var b = '';
//    while (a <= 10) {
//        b += a;
//        a++;
//    }
//    assert.strictEqual('12345678910', b);
//})()

//");

        }

    }
}
