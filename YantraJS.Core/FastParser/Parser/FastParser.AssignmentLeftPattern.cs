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
                default:
                    throw stream.Unexpected();
            }

            bool ReadObjectPattern(out AstExpression objectPattern, FastVariableKind kind, bool modulePattern = false)
            {
                var begin = Location;
                objectPattern = default;
                AstExpression left;
                AstExpression right;
                var nodes = Pool.AllocateList<ObjectProperty>();
                try
                {
                    do
                    {
                        if(Identitifer(out var id))
                        {
                            left = id;
                        } else if (!AssignmentLeftPattern(out left, kind, modulePattern))
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
                                nodes.Add(new ObjectProperty(left, right));
                            }
                            else if (AssignmentLeftPattern(out right, kind, modulePattern)) {
                                nodes.Add(new ObjectProperty(left, right));
                            }
                            else throw stream.Unexpected();
                        } else
                        {
                            variableScope.Top.AddVariable(left.Start, left.Start.Span, kind);
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

            bool ReadArrayPattern(out AstExpression arrayPattern, FastVariableKind kind, bool modulePattern = false)
            {
                var begin = Location;
                arrayPattern = default;
                var nodes = Pool.AllocateList<AstExpression>();
                try
                {
                    do
                    {
                        var spread = stream.CheckAndConsume(TokenTypes.TripleDots, out var token);
                        if (!AssignmentLeftPattern(out var left, kind, modulePattern))
                            throw stream.Unexpected();
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
