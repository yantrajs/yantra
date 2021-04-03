using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

        /// <summary>
        /// delete SingleComputedExpression
        /// void SingleComputedExpression
        /// typeof SingleComputedExpression
        /// +SingleComputedExpression
        /// -SingleComputedExpression
        /// ~SingleComputedExpression
        /// !SingleComputedExpression
        /// ++SingleComputedExpression
        /// --SingleComputedExpression
        /// SingleComputedExpression++
        /// SingleComputedExpression--
        /// </summary>
        /// <param name="node"></param>
        /// <param name="hasAsync"></param>
        /// <param name="hasGenerator"></param>
        /// <returns></returns>
        bool SinglePrefixPostfixExpression(out AstExpression node, out bool hasAsync, out bool hasGenerator)
        {
            var begin = Location;
            var (prefix, token) = GetUnaryOperator();
            hasAsync = false;
            hasGenerator = false;

            if (stream.CheckAndConsume(FastKeywords.async))
                hasAsync = true;

            if (stream.CheckAndConsume(TokenTypes.Multiply))
                hasGenerator = true;

            if (!SingleMemberExpression(out node))
                return begin.Reset();

            if(prefix != UnaryOperator.None)
            {
                node = new AstUnaryExpression(token, node, prefix);
            }
            var (postfix, postFixToken) = GetUnaryOperator(false);
            if(postfix != UnaryOperator.None)
            {
                node = new AstUnaryExpression(postFixToken, node, postfix, false);
            }

            return true;

            (UnaryOperator, FastToken) GetUnaryOperator(bool prefix = true)
            {
                var token = stream.Current;
                switch (token.Type)
                {
                    case TokenTypes.Plus:
                        if (prefix)
                        {
                            stream.Consume();
                            return (UnaryOperator.Plus, token);
                        }
                        return (UnaryOperator.None, token);
                    case TokenTypes.Minus:
                        if (prefix)
                        {
                            stream.Consume();
                            return (UnaryOperator.Minus, token);
                        }
                        return (UnaryOperator.None, token);
                    case TokenTypes.Increment:
                        stream.Consume();
                        return (UnaryOperator.Increment, token);
                    case TokenTypes.Decrement:
                        stream.Consume();
                        return (UnaryOperator.Decrement, token);
                    case TokenTypes.Negate:
                        stream.Consume();
                        return (UnaryOperator.Negate, token);
                    case TokenTypes.BitwiseNot:
                        stream.Consume();
                        return (UnaryOperator.BitwiseNot, token);
                }
                if (!prefix)
                    return (UnaryOperator.None, token);
                switch (token.Keyword)
                {
                    case FastKeywords.@typeof:
                        stream.Consume();
                        return (UnaryOperator.@typeof, token);
                    case FastKeywords.delete:
                        stream.Consume();
                        return (UnaryOperator.delete, token);
                    case FastKeywords.@void:
                        stream.Consume();
                        return (UnaryOperator.@void, token);
                    default:
                        return (UnaryOperator.None, token);
                }
            }
        }
        
    }

}
