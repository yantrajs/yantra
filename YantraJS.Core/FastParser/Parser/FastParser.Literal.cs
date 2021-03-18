using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {




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
