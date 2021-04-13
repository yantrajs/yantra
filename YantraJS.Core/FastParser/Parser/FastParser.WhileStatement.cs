using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

        bool ExpressionArray(out ArraySpan<AstExpression> nodes, TokenTypes endsWith = TokenTypes.BracketEnd)
        {
            var list = Pool.AllocateList<AstExpression>();
            try
            {
                do
                {
                    if(stream.CheckAndConsume(endsWith))
                    {
                        // empty...
                        break;
                    }
                    var isSpread = stream.CheckAndConsume(TokenTypes.TripleDots, out var token);
                    if (Expression(out var node)) {
                        if(isSpread) {
                            node = new AstSpreadElement(token, node.End, node);
                        }
                        list.Add(node);
                    }
                    if (stream.CheckAndConsume(TokenTypes.Comma))
                        continue;
                    if (stream.CheckAndConsumeAny(endsWith, TokenTypes.EOF))
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

        /// <summary>
        /// Expression Sequence represents a comma separated expressions
        /// terminated by new line or semi colon
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="endWith"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        bool ExpressionSequence(
            out AstExpression expressions, 
            TokenTypes endWith = TokenTypes.BracketEnd,
            bool allowEmpty = false)
        {
            var begin = Location;
            var nodes = Pool.AllocateList<AstExpression>();
            try
            {
                do
                {
                    if (allowEmpty && stream.CheckAndConsume(endWith))
                        break;
                    allowEmpty = false;
                    if (Expression(out var node))
                        nodes.Add(node);
                    if (stream.CheckAndConsume(TokenTypes.Comma))
                        continue;
                    if (stream.CheckAndConsumeAny(endWith, TokenTypes.EOF))
                        break;
                    if (stream.Current.Type == TokenTypes.CurlyBracketEnd)
                        break;
                    if (PreviousToken.LineTerminator)
                        break;
                    throw stream.Unexpected();
                } while (true);
                switch(nodes.Count)
                {
                    case 0:
                        expressions = new AstEmptyExpression(begin.Token);
                        break;
                    case 1:
                        expressions = nodes[0];
                        break;
                    default:
                        expressions = new AstSequenceExpression(begin.Token, PreviousToken, nodes);
                        break;
                }
                return true;
            } finally
            {
                nodes.Clear();
            }
        }


        bool WhileStatement(out AstStatement node)
        {
            var begin = Location;
            stream.Consume();

            stream.Expect(TokenTypes.BracketStart);

            ExpressionSequence(out var test);

            // stream.Expect(TokenTypes.BracketEnd);

            if (!Statement(out var statement))
                throw stream.Unexpected();

            node = new AstWhileStatement(begin.Token, PreviousToken, test, statement);
            return true;
        }


    }

}
