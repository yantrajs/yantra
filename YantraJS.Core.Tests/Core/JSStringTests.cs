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

            this.context.Eval(@"    function getCollectionImplementation(name, nativeFactory, shimFactory) {
        var _a;
        // NOTE: ts.ShimCollections will be defined for typescriptServices.js but not for tsc.js, so we must test for it.
        var constructor = (_a = ts.NativeCollections[nativeFactory]()) !== null && _a !== void 0 ? _a : ts.ShimCollections === null || ts.ShimCollections === void 0 ? void 0 : ts.ShimCollections[shimFactory](getIterator);
        if (constructor)
            return constructor;
        throw new Error('TypeScript requires an environment that provides a compatible native ' + name + ' implementation.');
    }");
        }

    }
}
