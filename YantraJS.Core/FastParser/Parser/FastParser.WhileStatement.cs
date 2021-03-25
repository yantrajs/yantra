using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

        bool ExpressionArray(out  AstExpression[] nodes, TokenTypes endsWith = TokenTypes.BracketEnd)
        {
            var begin = Location;
            var list = Pool.AllocateList<AstExpression>();
            try
            {
                do
                {
                    if (Expression(out var node))
                        list.Add(node);
                    if (stream.CheckAndConsume(TokenTypes.Comma))
                        continue;
                    if (stream.CheckAndConsume(endsWith))
                        break;
                    throw stream.Unexpected();
                } while (true);
                nodes = list.Release();
                return true;
            }
            finally
            {
                list.Clear();
            }
        }

        bool ExpressionSequence(out AstExpression expressions, TokenTypes endWith = TokenTypes.BracketEnd)
        {
            var begin = Location;
            var nodes = Pool.AllocateList<AstExpression>();
            try
            {
                do
                {
                    if (Expression(out var node))
                        nodes.Add(node);
                    if (stream.CheckAndConsume(TokenTypes.Comma))
                        continue;
                    if (stream.CheckAndConsume(endWith))
                        break;
                    throw stream.Unexpected();
                } while (true);
                if (nodes.Count == 1)
                {
                    expressions = nodes[0];
                } else
                {
                    expressions = new AstSequenceExpression(begin.Token, PreviousToken, nodes.Release());
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

            stream.Expect(TokenTypes.BracketEnd);

            if (!Statement(out var statement))
                throw stream.Unexpected();

            node = new AstWhileStatement(begin.Token, PreviousToken, test, statement);
            return true;
        }


    }

}
