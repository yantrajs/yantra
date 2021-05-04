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
const a = ['a', 'b', 'c', 2, 3];
const first = a.find((x) => typeof x === 'number');
            assert.strictEqual(2, first);

class A { }

        class B { }


let aa = [{ id: A, value: 1 }, { id: B, value: 2 }];
assert.strictEqual(aa.find((x) => x.id === A).value, 1);
assert.strictEqual(aa.find((x) => x.id === B).value, 2);

class ArrayHelper {
    static remove(a, filter) {
        for (let i = 0; i < a.length; i++) {
            const item = a[i];
            if (filter(item)) {
                a.splice(i, 1);
                return true;
            }
        }
        return false;
    }
}

ArrayHelper.remove(aa, (x) => x.id === ArrayHelper || x.value === ArrayHelper);
// assert.strictEqual(2, aa.length);

        ");
        }

    }
}
