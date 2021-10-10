using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.FastParser;

namespace YantraJS.Core.Tests.Parser
{

    [TestClass]
    public class BasicTest: BaseParserTest
    {
        [TestMethod]
        public void Invoke()
        {
            //var program = Parse("a()");
            //var fx = (program.Statements[0] as AstExpressionStatement)?.Expression;
            //Assert.AreEqual(fx.Type, FastParser.FastNodeType.CallExpression);

            //Parse("a.b()");
            //Parse("a.b(a)");
            //Parse("a.b(a.b)");
            //Parse("a.b(a.b())");

            //Parse("a.b.c()");

            //Parse("a + c()");
            //Parse("a() + c()");
            var x = Parse("a.a() + c().m");
            var exp = (x.Statements.First() as AstExpressionStatement).Expression;
        }

        [TestMethod]
        public void Computed()
        {
            // var x = Expression("a[1]");
            var x1 = Expression("a + a[1] * a[2]");
        }


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

            var r = Parse("var a = b = c = d;");
        }

        [TestMethod]
        public void TemplateExpressions()
        {
            Parse("``");
            Parse("`ab`");
            Parse("`ab ${n} dfdfsd ${m}`");
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
            // Fail("for");
            // Fail("for;;");
            Fail("{,}");
            Fail("{a,}");
        }

        [TestMethod]
        public void For()
        {
            Parse("for(;;);");
            Parse("for(var a = 1;;);");
            Parse("for(var a = 1,b=2;;);");
            Parse("for(var a = 1,b=2;c;d++);");
            Parse(@"let str = '';
loop1:
for (let i = 0; i < 5; i++) {
  if (i === 1) {
    continue loop1;
  }
  str = str + i;
}");
        }
    }
}
