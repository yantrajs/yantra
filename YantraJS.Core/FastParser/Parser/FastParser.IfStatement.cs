using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



        bool IfStatement(out AstStatement node)
        {
            var begin = stream.Current;
            node = default;
            stream.Consume();

            stream.Expect(TokenTypes.BracketStart);
            if (!ExpressionSequence(out var test))
                throw stream.Unexpected();

            // stream.Expect(TokenTypes.BracketEnd);

            if (!Statement(out var @true))
                throw stream.Unexpected();

            if (stream.CheckAndConsume(FastKeywords.@else))
            {
                if (!Statement(out var @else))
                    throw stream.Unexpected();
                node = new AstIfStatement(begin, PreviousToken, test, @true, @else);
                return true;
            }

            node = new AstIfStatement(begin, PreviousToken, test, @true);
            return true;
        }


    }

}
