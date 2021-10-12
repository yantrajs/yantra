using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YantraJS.Core.FastParser;

namespace YantraJS.Core.Tests.Parser
{
    public class BaseParserTest
    {

        public BaseParserTest()
        {

        }

        public AstProgram Parse(string text)
        {
            var stream = new FastTokenStream(text);
            FastParser.FastParser parser = new FastParser.FastParser(stream);
            return parser.ParseProgram();
        }

        public AstExpression Expression(string text)
        {
            var stream = new FastTokenStream(text);
            FastParser.FastParser parser = new FastParser.FastParser(stream);
            return (parser.ParseProgram().Statements.FirstOrDefault() as AstExpressionStatement)?.Expression;
        }


        public void Fail(string text)
        {
            try {
                Parse(text);
                Assert.Fail($"Expected failure for {text}");
            } catch
            {

            }
        }

    }
}
