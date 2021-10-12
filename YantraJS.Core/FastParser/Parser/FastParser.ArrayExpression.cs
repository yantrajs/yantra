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
        bool ArrayExpression(out IFastEnumerable<AstExpression> nodes, TokenTypes endsWith = TokenTypes.BracketEnd)
        {
            var list = new Sequence<AstExpression>();
            try
            {
                do
                {

                    stream.SkipNewLines();

                    if (stream.CheckAndConsumeAny(endsWith, TokenTypes.EOF))
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
                    if (stream.CheckAndConsumeAny(endsWith, TokenTypes.EOF))
                        break;
                    if (stream.CheckAndConsume(TokenTypes.Comma))
                        continue;
                    if (stream.LineTerminator())
                        continue;
                    throw stream.Unexpected();
                } while (true);
                nodes = list;
                return true;
            }
            finally
            {
                //list.Clear();
            }
        }
    }

}
