﻿using System;
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

            stream.Expect(FastKeywords.@while);

            stream.Expect(TokenTypes.BracketStart);
            ExpressionSequence(out var test);
            stream.Expect(TokenTypes.BracketEnd);
            
            EndOfStatement();

            node = new AstDoWhileStatement(begin.Token, PreviousToken, test, statement);
            return true;
        }


    }

}