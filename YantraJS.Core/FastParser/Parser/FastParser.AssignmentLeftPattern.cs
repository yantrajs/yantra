using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



        bool AssignmentLeftPattern(out AstExpression node)
        {
            node = default;

            var token = stream.Current;
            switch (token.Type)
            {
                case TokenTypes.Identifier:
                    stream.Consume();
                    node = new AstIdentifier(token);
                    return true;
                case TokenTypes.SquareBracketStart:
                    stream.Consume();
                    return ReadArrayPattern(out node);
                case TokenTypes.CurlyBracketStart:
                    stream.Consume();
                    return ReadObjectPattern(out node);
                default:
                    throw stream.Unexpected();
            }

            bool ReadObjectPattern(out AstExpression objectPattern)
            {
                var begin = Location;
                objectPattern = default;
                var nodes = Pool.AllocateList<ObjectProperty>();
                try
                {
                    do
                    {
                        if (!AssignmentLeftPattern(out var left))
                            throw stream.Unexpected();
                        if(stream.CheckAndConsume(TokenTypes.Colon))
                        {
                            if (!AssignmentLeftPattern(out var right))
                                throw stream.Unexpected();
                            nodes.Add(new ObjectProperty(left, right));
                        } else
                        {
                            nodes.Add(new ObjectProperty(left, left));
                        }
                        if (stream.CheckAndConsume(TokenTypes.Comma))
                            continue;
                        if (stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                            break;
                        throw stream.Unexpected();
                    } while (true);
                    objectPattern = new AstObjectPattern(begin.Token, PreviousToken, nodes);
                    return true;
                } finally {
                    nodes.Clear();
                }
            }

            bool ReadArrayPattern(out AstExpression arrayPattern)
            {
                var begin = Location;
                arrayPattern = default;
                var nodes = Pool.AllocateList<AstExpression>();
                try
                {
                    do
                    {
                        var spread = stream.Current;
                        if(stream.CheckAndConsume(TokenTypes.TripleDots))
                        {
                            spread = null;
                        }
                        if (!AssignmentLeftPattern(out var left))
                            throw stream.Unexpected();
                        if (spread != null)
                            left = new AstSpreadElement(spread, left.End, left);
                        if (stream.CheckAndConsume(TokenTypes.Comma))
                        {
                            if (spread != null)
                                throw stream.Unexpected();
                            continue;
                        }
                        if (stream.CheckAndConsume(TokenTypes.SquareBracketEnd))
                            break;
                    } while (true);
                    arrayPattern = new AstArrayPattern(begin.Token, PreviousToken, nodes);
                    return true;
                }
                finally
                {
                    nodes.Clear();
                }
            }
        }


    }

}
