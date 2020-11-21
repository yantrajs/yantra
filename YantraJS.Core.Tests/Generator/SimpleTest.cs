using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using YantraJS.Core;
using YantraJS.Tests.Core;

namespace YantraJS.Tests.Generator
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
            // Assert.IsNotNull(context.Scope);
            var f = CoreScript.Evaluate("function a(a, b) { return a+b; }");

            Assert.IsTrue(f.IsFunction);

            var r = f.InvokeFunction(new Arguments(context, new JSNumber(1), new JSNumber(2)));

            Assert.AreEqual(3, r.IntValue);
        }


        [TestMethod]
        public void FunctionWithVars()
        {
            var f = CoreScript.Evaluate(@"function a(a) { 
                var b = 4;
                return a+b; }");

            Assert.IsTrue(f.IsFunction);

            var r = f.InvokeFunction(new Arguments(context, new JSNumber(1)));

            Assert.AreEqual(5, r.IntValue);
        }

        [TestMethod]
        public void FunctionExpression()
        {
            var f = CoreScript.Evaluate(@"(function (a) { 
                var b = 4;
                return a+b; })");

            Assert.IsTrue(f.IsFunction);

            var r = f.InvokeFunction(new Arguments(context, new JSNumber(1)));

            Assert.AreEqual(5, r.IntValue);

        }


        [TestMethod]
        public void NumberStrictEquals()
        {
            var f = CoreScript.Evaluate(@"(function () {
    return 2 === 3;
})()");

            Assert.IsFalse(f.BooleanValue);

        }

        [TestMethod]
        public void StringStrictEquals()
        {
            var f = CoreScript.Evaluate(@"(function () {
    return 2 === ""2"";
})()");

            Assert.IsFalse(f.BooleanValue);

            // f = CoreScript.Evaluate(@"""2"" === ""2""");
            f = CoreScript.Evaluate(@"(function() { return ""2"" === ""2"";})()");
            Assert.IsTrue(f.BooleanValue);

        }

        [TestMethod]
        public void AssertString()
        {
            Assert.ThrowsException<JSException>(() =>
            {
                var f = CoreScript.Evaluate(@"(function () {
    return assert(2 === ""2"", ""failed .."");
})()");
            });

            // f = CoreScript.Evaluate(@"""2"" === ""2""");
            var f1 = CoreScript.Evaluate(@"(function() { return ""2"" === ""2"";})()");
            Assert.IsTrue(f1.BooleanValue);

        }

        [TestMethod]
        public void StringEquals()
        {
            var f = CoreScript.Evaluate(@"(function () {
return assert(""2"" + ""2"" === ""22"", ""failed .."");
})()");

            // f = CoreScript.Evaluate(@"""2"" === ""2""");
            var f1 = CoreScript.Evaluate(@"(function() { return ""2"" === ""2"";})()");
            Assert.IsTrue(f1.BooleanValue);

        }
    }
}
