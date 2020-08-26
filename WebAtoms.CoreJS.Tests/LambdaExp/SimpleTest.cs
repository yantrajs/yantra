using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Tests.Core;

namespace WebAtoms.CoreJS.Tests.LambdaExp
{
    [TestClass]
    public class SimpleTest: BaseTest
    {

        [TestMethod]
        public void Literal()
        {
            var r = CoreScript.Evaluate("4");

            Assert.AreEqual(4, r.IntValue);

            r = CoreScript.Evaluate("'4'");

            Assert.AreEqual("4", r.ToString());

        }

        [TestMethod]
        public void GlobalVariable()
        {
            context["a"] = new JSNumber(3);
            var r = CoreScript.Evaluate("a++");
            Assert.AreEqual(3, r.IntValue);
            var a = context["a"];
            Assert.AreEqual(4, a.IntValue);
        }

        [TestMethod]
        public void Function()
        {
            Assert.IsNotNull(context.Scope);
            var f = CoreScript.Evaluate("function a(a, b) { return a+b; }");

            Assert.IsTrue(f.IsFunction);

            var r = f.InvokeFunction(context, JSArguments.From(1,2));

            Assert.AreEqual(3, r.IntValue);
        }


        [TestMethod]
        public void FunctionWithVars()
        {
            var f = CoreScript.Evaluate(@"function a(a) { 
                var b = 4;
                return a+b; }");

            Assert.IsTrue(f.IsFunction);

            var r = f.InvokeFunction(context, JSArguments.From(1));

            Assert.AreEqual(5, r.IntValue);
        }

        [TestMethod]
        public void FunctionExpression()
        {
            var f = CoreScript.Evaluate(@"(function (a) { 
                var b = 4;
                return a+b; })");

            Assert.IsTrue(f.IsFunction);

            var r = f.InvokeFunction(context, JSArguments.From(1));

            Assert.AreEqual(5, r.IntValue);

        }

    }
}
