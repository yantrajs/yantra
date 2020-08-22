using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace WebAtoms.CoreJS.Tests.CSharp
{
    [TestClass]
    public class CodeGenerator
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
            var ctx = JSContext.Current;
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

    }
}
