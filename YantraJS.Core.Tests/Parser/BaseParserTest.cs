using System;
using System.Collections.Generic;
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

    }
}
