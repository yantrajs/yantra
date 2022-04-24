using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;
using YantraJS.Generator;

namespace YantraJS.Tests.Core
{
    [TestClass]
    public class CodeTest: BaseTest
    {
        [TestMethod]
        public void Function()
        {

            ILCodeGenerator.GenerateLogs = true;

            // this.context.Eval("class A { constructor(a) { this.a = a; } } class B extends A { constructor(a) { super(a); } }");
            // Assert.AreEqual(1, context.Eval("x = {get f() { return 1; }}; x.f = 5; x.f"));
            // this.context["array"] = new JSArray( new JSNumber(1) );
//            this.context.Eval(@"

//(function(){return 1; /***/ })()
//");
            this.context.Eval(@"
function ID(x) {
  return x;
}

var object = {
  a() { return 'A'; },
  get [1]() { return 'B'; },
  // c() { return 'C'; },
  // [ID(2)]() { return 'D'; },
};
assert(object.a());

");
}

    }
}
