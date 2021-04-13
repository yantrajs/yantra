using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



        bool DoWhileStatement(out AstStatement node)
        {
            var begin = Location;
            stream.Consume();

            if (!Statement(out var statement))
                throw stream.Unexpected();

            stream.CheckAndConsume(TokenTypes.SemiColon);

            stream.Expect(FastKeywords.@while);

            stream.Expect(TokenTypes.BracketStart);
            ExpressionSequence(out var test);
            
            EndOfStatement();

            node = new AstDoWhileStatement(begin.Token, PreviousToken, test, statement);
            return true;
        }


    }

}
