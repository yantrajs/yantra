using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {


        bool Literal(out AstExpression node)
        {
            var token = stream.Current;
            switch (token.Type)
            {
                case TokenTypes.True:
                    stream.Consume();
                    node = new AstLiteral(TokenTypes.True, token);
                    return true;
                case TokenTypes.False:
                    stream.Consume();
                    node = new AstLiteral(TokenTypes.True, token);
                    return true;
                case TokenTypes.String:
                    stream.Consume();
                    node = new AstLiteral(TokenTypes.String, token);
                    return true;
                case TokenTypes.Number:
                    stream.Consume();
                    node = new AstLiteral(TokenTypes.Number, token);
                    return true;
                case TokenTypes.Null:
                    stream.Consume();
                    node = new AstLiteral(TokenTypes.Null, token);
                    return true;
            }
            node = null;
            return false;
        }

        bool StringLiteral(out AstExpression node)
        {
            if(stream.CheckAndConsume(TokenTypes.String, out var token))
            {
                node = new AstLiteral(TokenTypes.String, token);
                return true;
            }
            node = default;
            return false;
        }

        bool NumberLiteral(out AstExpression node)
        {
            if (stream.CheckAndConsume(TokenTypes.Number, out var token))
            {
                node = new AstLiteral(TokenTypes.Number, token);
                return true;
            }
            node = default;
            return false;
        }

    }

}
