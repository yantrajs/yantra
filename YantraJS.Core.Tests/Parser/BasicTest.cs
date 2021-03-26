using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Tests.Parser
{
    [TestClass]
    public class BasicTest: BaseParserTest
    {

        [TestMethod()]
        public void Expressions()
        {
            Parse("\"a\"");
            Parse("123");
            Parse("var a = 2;");
            Parse("var a = 2, c = d;");
            Parse("var a = (c = d);");
            Parse("var a = c + d;");
            Parse("var a = c + d * e;");
            Parse("var a = () => true;");
            Parse("var a = (b) => true;");
            Parse("var a = ({b}) => true;");

            Parse("var a = function(b) { return 4; }");
            Parse("var a = (function(b) { return 4; })()");
        }

        [TestMethod]
        public void Array()
        {
            Parse("[]");
            Parse("[1]");
            Parse("[1,3]");
            Parse("var a = [1,3]");
            Parse("var a = [1,,3]");
        }

        [TestMethod]
        public void Object()
        {
            Parse("({})");
            Parse("({ a: 1 })");
            Parse("({ a: 1, b: 1})");
            Parse("({ a })");
            Parse("({ ...a })");
            Parse("({ ...a,...b })");
            Parse("({ ...a,... {c} })");
            Parse("({ ...a,... {c}, ... [d] })");
        }

        [TestMethod]
        public void Fail()
        {
            Fail("for");
            Fail("for;;");
            Fail("{,}");
            Fail("{a,}");
        }
    }
}
