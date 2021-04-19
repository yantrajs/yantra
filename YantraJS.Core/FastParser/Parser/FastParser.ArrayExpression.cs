using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    partial class FastParser
    {
        /// <summary>
        /// ArrayExpression does not break with line terminator. It is used by method parameters and array initialization
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        bool ArrayExpression(out ArraySpan<AstExpression> nodes, TokenTypes endsWith = TokenTypes.BracketEnd)
        {
            var list = Pool.AllocateList<AstExpression>();
            try
            {
                do
                {
                    if (stream.CheckAndConsumeAny(endsWith, TokenTypes.EOF, TokenTypes.LineTerminator))
                        break;
                    var isSpread = stream.CheckAndConsume(TokenTypes.TripleDots, out var token);
                    if (Expression(out var node))
                    {
                        if (isSpread)
                        {
                            node = new AstSpreadElement(token, node.End, node);
                        }
                        list.Add(node);
                    }
                    if (stream.CheckAndConsume(TokenTypes.Comma))
                        continue;
                    if (stream.CheckAndConsume(endsWith))
                        break;
                    throw stream.Unexpected();
                } while (true);
                nodes = list.ToSpan();
                return true;
            }
            finally
            {
                list.Clear();
            }
        }
    }

}
