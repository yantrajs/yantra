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
        }

    }
}
