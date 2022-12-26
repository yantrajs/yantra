using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



        bool AssignmentLeftPattern(
            out AstExpression node, 
            FastVariableKind kind,
            bool modulePattern = false)
        {
            node = default;
            stream.SkipNewLines();
            var token = stream.Current;
            switch (token.Type)
            {
                case TokenTypes.TripleDots:
                    stream.Consume();
                    if (!AssignmentLeftPattern(out var p, kind))
                        throw stream.Unexpected();
                    node = new AstSpreadElement(token, p.End, p);
                    return true;
                case TokenTypes.Identifier:
                    if (token.IsKeyword)
                        throw stream.Unexpected();
                    stream.Consume();
                    node = new AstIdentifier(token);
                    variableScope.Top.AddVariable(token, token.Span, kind);
                    return true;
                case TokenTypes.SquareBracketStart:
                    stream.Consume();
                    return ReadArrayPattern(out node, kind, modulePattern);
                case TokenTypes.CurlyBracketStart:
                    stream.Consume();
                    return ReadObjectPattern(out node, kind, modulePattern);
                //case TokenTypes.String:
                //    if(stream.CheckAndConsume(TokenTypes.Colon, TokenTypes.Identifier, out var _, out var id))
                //    {

                //    }
                //    throw stream.Unexpected();
                default:
                    throw stream.Unexpected();
            }

            bool ReadObjectPattern(out AstExpression objectPattern, FastVariableKind kind, bool modulePattern = false)
            {
                var begin = stream.Current;
                objectPattern = default;
                AstExpression left;
                AstExpression right;
                AstExpression init = null;
                var nodes = new Sequence<ObjectProperty>();
                try
                {
                    do
                    {
                        // added for empty object pattern
                        if (stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                            break;
                        if (Identitifer(out var id))
                        {
                            left = id;
                        }
                        else if (StringLiteral(out var str))
                        {
                            left = str;
                        }
                        else if (NumberLiteral(out var num)) {
                            left = num;
                        } else  if (!AssignmentLeftPattern(out left, kind, modulePattern))
                            throw stream.Unexpected();

                        bool renamed = modulePattern
                            ? stream.CheckAndConsumeContextualKeyword(FastKeywords.@as)
                            : stream.CheckAndConsume(TokenTypes.Colon);

                        if(renamed)
                        {
                            if (Identitifer(out var rid))
                            {
                                right = rid;
                                variableScope.Top.AddVariable(right.Start, right.Start.Span, kind);
                            }
                            else if (AssignmentLeftPattern(out right, kind, modulePattern)) {
                            }
                            else throw stream.Unexpected();
                        } else
                        {
                            variableScope.Top.AddVariable(left.Start, left.Start.Span, kind);
                            right = left;
                        }

                        if (stream.CheckAndConsume(TokenTypes.Assign))
                        {
                            if (!Expression(out init))
                                throw stream.Unexpected();
                        }

                        nodes.Add(new ObjectProperty(left, right, init));

                        if (stream.CheckAndConsume(TokenTypes.Comma))
                            continue;
                        if (stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                            break;
                        throw stream.Unexpected();
                    } while (true);
                    objectPattern = new AstObjectPattern(begin, PreviousToken, nodes);
                    return true;
                } finally {
                    // nodes.Clear();
                }
            }

            bool ReadArrayPattern(out AstExpression arrayPattern, FastVariableKind kind, bool modulePattern = false)
            {
                var begin = stream.Current;
                arrayPattern = default;
                var nodes = new Sequence<AstExpression>();
                try
                {
                    do
                    {
                        // added for empty square bracket pattern
                        if (stream.CheckAndConsume(TokenTypes.SquareBracketEnd))
                            break;
                        var spread = stream.CheckAndConsume(TokenTypes.TripleDots, out var token);
                        if (!AssignmentLeftPattern(out var left, kind, modulePattern))
                            throw stream.Unexpected();
                        if (stream.CheckAndConsume(TokenTypes.Assign))
                        {
                            if (spread)
                                throw stream.Unexpected();
                            if (!Expression(out var right))
                                throw stream.Unexpected();
                            left = new AstBinaryExpression(left, TokenTypes.Assign, right);
                        }
                        if (spread)
                            left = new AstSpreadElement(token, left.End, left);
                        nodes.Add(left);
                        if (stream.CheckAndConsume(TokenTypes.Comma))
                        {
                            continue;
                        }
                        if (stream.CheckAndConsume(TokenTypes.SquareBracketEnd))
                            break;
                    } while (true);
                    arrayPattern = new AstArrayPattern(begin, PreviousToken, nodes);
                    return true;
                }
                finally
                {
                    // nodes.Clear();
                }
            }
        }


    }

}
