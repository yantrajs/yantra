using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Tests.Core;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace WebAtoms.CoreJS.Tests.CSharp
{
    [TestClass]
    public class CodeGenerator: BaseTest
    {

        [TestMethod]
        public void Generate()
        {

            var text = Literal("a\"b").ToFullString();

            Assert.AreEqual("\"a\\\"b\"", text);
        }

        [TestMethod]
        public void ClosureSample()
        {
            var f1 = new JSFunctionImpl((__t, __a, c) =>
            {
                var a = __a[0];
                var ac = new JSVariable { Value = a };

                var f2 = new JSFunctionImpl((__t, __a, c) =>
                {
                    var a = c[0];
                    a.Value = a.Value.Add(new JSNumber(4));
                    return JSUndefined.Value;
                }, "", "", new JSVariable[] { ac });

                return f2;
            }, "", "", null);
        }

        [TestMethod]
        public void HoistingSample()
        {
            var f1 = new JSFunctionImpl((__t, __a, c) =>
            {

                // first intialize incoming parameters
                var a = __a[0];
                var ac = new JSVariable { Value = a };

                // hoist function before using specific name of function...
                // but do not put it on the top
                // it may require some variables inside it

                var f2 = new JSFunctionImpl((__t, __a, c) =>
                {
                    var a = c[0];
                    a.Value = a.Value.Add(new JSNumber(4));
                    return JSUndefined.Value;
                }, "", "", new JSVariable[] { ac });

                var f2c = new JSVariable { Value = f2 };

                return f2;
            }, "", "", null);
        }

    }
}
