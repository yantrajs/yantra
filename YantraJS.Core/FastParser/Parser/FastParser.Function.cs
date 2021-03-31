using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {

        bool Function(out AstStatement statement, bool isAsync = false)
        {
            statement = default;
            if (!FunctionExpression(out var expression, isAsync))
                return false;
            statement = new AstExpressionStatement(expression);
            return true;
        }


        bool FunctionExpression(out AstExpression node, bool isAsync = false)
        {
            var begin = Location;
            node = default;
            stream.Consume();
            var generator = false;
            if (stream.CheckAndConsume(TokenTypes.Multiply))
            {
                generator = true;
            }

            Identitifer(out var id);

            stream.Expect(TokenTypes.BracketStart);
            if (!Parameters(out var declarators, TokenTypes.BracketEnd, false))
                throw stream.Unexpected();

            if (stream.Current.Type != TokenTypes.CurlyBracketStart)
                throw stream.Unexpected();

            if (!Statement(out var body))
                throw stream.Unexpected();

            node = new AstFunctionExpression(begin.Token, PreviousToken, isAsync, generator, id, declarators, body);

            return true;
        }


    }
}
