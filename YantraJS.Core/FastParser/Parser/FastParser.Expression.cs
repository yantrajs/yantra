using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

   
    partial class FastParser
    {



        bool Expression(out AstExpression node)
        {
            var begin = Location;
            node = default;
            var token = begin.Token;

            switch(token.Type)
            {
                case TokenTypes.Plus:
                case TokenTypes.Minus:
                case TokenTypes.Increment:
                case TokenTypes.Decrement:
                case TokenTypes.Negate:
                case TokenTypes.BitwiseNot:
                    stream.Consume();
                    if(Expression(out var n))
                    {
                        node = new AstUnaryExpression(token, n, token.Type);
                        return true;
                    }
                    throw stream.Unexpected();
            }
            switch (token.Keyword)
            {
                case FastKeywords.@typeof:
                case FastKeywords.delete:
                case FastKeywords.@void:
                    stream.Consume();
                    if (Expression(out var n))
                    {
                        node = new AstUnaryExpression(token, n, token.Type);
                        return true;
                    }
                    throw stream.Unexpected();
            }

            if (!SingleExpression(out node))
                return begin.Reset();

            // now let us evaluate operators...
            token = stream.Current;
            switch(token.Type)
            {
                case TokenTypes.Increment:
                case TokenTypes.Decrement:
                    stream.Consume();
                    node = new AstUnaryExpression(token, node, token.Type, false);
                    return true;
            }

            return begin.Reset();
        }


    }

}
