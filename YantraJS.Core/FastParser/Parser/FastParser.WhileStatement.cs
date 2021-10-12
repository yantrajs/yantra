using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

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
            var begin = stream.Current;
            var nodes = new Sequence<AstExpression>();
            try
            {
                do
                {
                    if (allowEmpty && stream.Current.Type == TokenTypes.CurlyBracketEnd)
                        break;
                    if (allowEmpty && stream.CheckAndConsumeAny(endWith, TokenTypes.EOF, TokenTypes.SemiColon))
                        break;
                    allowEmpty = false;
                    if (Expression(out var node))
                        nodes.Add(node);
                    if (stream.CheckAndConsume(TokenTypes.Comma))
                        continue;
                    if (stream.CheckAndConsumeAny(endWith, TokenTypes.EOF, TokenTypes.SemiColon))
                        break;
                    if (stream.Current.Type == TokenTypes.CurlyBracketEnd)
                        break;
                    if (stream.LineTerminator())
                        break;
                    //throw stream.Unexpected();
                    break;
                } while (true);
                switch(nodes.Count)
                {
                    case 0:
                        expressions = new AstEmptyExpression(begin);
                        break;
                    case 1:
                        expressions = nodes[0];
                        break;
                    default:
                        expressions = new AstSequenceExpression(begin, PreviousToken, nodes);
                        break;
                }
                return true;
            } finally
            {
                //nodes.Clear();
            }
        }


        bool WhileStatement(out AstStatement node)
        {
            var begin = stream.Current;
            stream.Consume();

            stream.Expect(TokenTypes.BracketStart);

            ExpressionSequence(out var test);

            // stream.Expect(TokenTypes.BracketEnd);

            if (!NonDeclarativeStatement(out var statement))
                throw stream.Unexpected();


            node = new AstWhileStatement(begin, PreviousToken, test, statement);
            return true;
        }


    }

}
