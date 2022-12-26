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
            this.context.Execute(@"

            class g {
                constructor(n) {
                    this.n = n;
                }

                *entries() {
                    const m = this.n;
                    const a = () => m;
                    for(let i=0;i<a();i++) 
                        yield i;
                }
            }
            

            let g1 = new g(2);
            assert.strictEqual(Array.from(g1.entries(1)).toString(), '0,1');

");
}

    }
}
