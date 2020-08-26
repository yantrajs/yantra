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
            var fx = CoreScript.Compile("4");

            var r = fx(null, JSArguments.Empty);
            Assert.AreEqual(4, r.IntValue);

            fx = CoreScript.Compile("'4'");

            r = fx(null, JSArguments.Empty);
            Assert.AreEqual("4", r.ToString());

        }

        [TestMethod]
        public void GlobalVariable()
        {
            context["a"] = new JSNumber(3);
            var fx = CoreScript.Compile("a++");

            var r = fx(null, JSArguments.Empty);
            Assert.AreEqual(3, r.IntValue);
            var a = context["a"];
            Assert.AreEqual(4, a.IntValue);
        }

        [TestMethod]
        public void Function()
        {
            Assert.IsNotNull(context.Scope);
            var s = CoreScript.Compile("function a(a, b) { return a+b; }");
            var f = s(null, JSArguments.Empty);

            Assert.IsTrue(f.IsFunction);

            var r = f.InvokeFunction(context, JSArguments.From(1,2));

            Assert.AreEqual(3, r.IntValue);
        }


        [TestMethod]
        public void FunctionWithVars()
        {
            Assert.IsNotNull(context.Scope);
            var s = CoreScript.Compile(@"function a(a) { 
                var b = 4;
                return a+b; }");
            var f = s(null, JSArguments.Empty);

            Assert.IsTrue(f.IsFunction);

            var r = f.InvokeFunction(context, JSArguments.From(1));

            Assert.AreEqual(5, r.IntValue);
        }

    }
}
