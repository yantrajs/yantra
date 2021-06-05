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

        function Type(x) {
            if (x === null)
                return 1;
            switch (typeof x) {
                case 'undefined': return 0;
                case 'boolean': return 2;
                case 'string': return 3;
                case 'symbol': return 4;
                case 'number': return 5;
                case 'object': return x === null ? 1 : 6;
            default: return 6;
        }
    }
    assert.strictEqual(1, Type(null));

");
        }

    }
}
