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

  ((_a = jsBundle.sources) === null || _a === void 0 ? void 0 : _a.prologues)
");
        }

    }
}
