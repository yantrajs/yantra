using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public enum UnaryOperator
    {
        Plus,
        Minus,
        Increment,
        Decrement,
        Negate,
        BitwiseNot,
        @typeof,
        @delete,
        @void,
        None
    }
   
    partial class FastParser
    {



        bool Expression(out AstExpression node)
        {
            var begin = Location;
            node = default;
            var token = begin.Token;

            var prefixUnaryToken = GetUnaryOperator(token);

            if (!SingleExpression(out node))
                return begin.Reset();

            if (prefixUnaryToken != UnaryOperator.None)
            {
                node = new AstUnaryExpression(token, node, prefixUnaryToken);
            }
            begin = Location;
            token = begin.Token;
            var postFixUnaryOperator = GetUnaryOperator(token);
            if(postFixUnaryOperator != UnaryOperator.None)
            {
                node = new AstUnaryExpression(token, node, postFixUnaryOperator, false);
            }



            return begin.Reset();
        }

        private UnaryOperator GetUnaryOperator(FastToken token, bool prefix = true)
        {
            UnaryOperator prefixUnaryToken = UnaryOperator.None;
            switch (token.Type)
            {
                case TokenTypes.Plus:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.Plus;
                    break;
                case TokenTypes.Minus:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.Minus;
                    break;
                case TokenTypes.Increment:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.Increment;
                    break;
                case TokenTypes.Decrement:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.Decrement;
                    break;
                case TokenTypes.Negate:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.Negate;
                    break;
                case TokenTypes.BitwiseNot:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.BitwiseNot;
                    break;
            }
            if (!prefix)
                return UnaryOperator.None;
            switch (token.Keyword)
            {
                case FastKeywords.@typeof:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.@typeof;
                    break;
                case FastKeywords.delete:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.delete;
                    break;
                case FastKeywords.@void:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.@void;
                    break;
            }

            return prefixUnaryToken;
        }
    }

}
