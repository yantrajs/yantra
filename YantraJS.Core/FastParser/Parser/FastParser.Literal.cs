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
                case TokenTypes.False:
                case TokenTypes.String:
                case TokenTypes.Number:
                case TokenTypes.Null:
                case TokenTypes.RegExLiteral:
                case TokenTypes.BigInt:
                case TokenTypes.Decimal:
                    stream.Consume();
                    node = new AstLiteral(token.Type, token);
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
