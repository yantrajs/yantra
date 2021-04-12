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

            this.context.Eval(@"let fruits = ['Banana', 'Orange', 'Lemon', 'Apple', 'Mango']
let citrus = fruits.slice(1, 3);

assert.strictEqual(citrus.toString(), 'Orange,Lemon');

function list(a1) {
    return Array.prototype.slice.call(a1)
}

var args = {
    0: 1,
    1: 2,
    2: 3,
    length: 3
};

let list1 = list(args);

assert.strictEqual(list1.toString(), '1,2,3');

function list2() {
    return Array.prototype.slice.call(arguments);
}

list1 = list2(1,2,3);

assert.strictEqual(list1.toString(), '1,2,3');");
        }

    }
}
