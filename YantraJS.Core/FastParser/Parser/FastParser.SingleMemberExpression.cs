using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

        /// <summary>
        /// SingleExpression
        /// SingleExpression[]
        /// SingleExpression.SingleExpression[]
        /// SingleExpression(.... )
        /// SingleExpression.SingleExpression(....) 
        /// SingleExpression.SingleExpression[].SingleExpression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool SingleMemberExpression(out AstExpression node, bool asNew = false)
        {
            if (!SingleExpression(out node))
            {
                return false;
            }

            StreamLocation begin;
            FastToken token;

            while (true) {

                var m = stream.SkipNewLines();

                begin = Location;
                token = begin.Token;
                switch (token.Type) {
                    case TokenTypes.SquareBracketStart:
                        stream.Consume();
                        if (!ExpressionSequence(out var index, TokenTypes.SquareBracketEnd))
                            throw stream.Unexpected();
                        node = node.Member(index, true);
                        continue;
                    case TokenTypes.BracketStart:
                        stream.Consume();
                        if (!ArrayExpression(out var arguments))
                            throw stream.Unexpected();
                        if (asNew)
                        {
                            node = new AstNewExpression(token, node, arguments);
                            asNew = false;
                        }
                        else
                            node = new AstCallExpression(node, arguments);
                        continue;
                    case TokenTypes.Dot:
                        stream.Consume();
                        stream.SkipNewLines();
                        var next = stream.Current;
                        switch (next.Type)
                        {
                            case TokenTypes.Identifier:
                            case TokenTypes.In:
                            case TokenTypes.InstanceOf:
                            case TokenTypes.Null:
                            case TokenTypes.True:
                            case TokenTypes.False:
                                stream.Consume();
                                node = node.Member(new AstIdentifier(next.AsString()));
                                break;
                            default:
                                throw stream.Unexpected();
                        }
                        continue;
                    default:
                        m.Undo();
                        break;
                }
                break;
            }

            if (asNew)
            {
                node = new AstNewExpression(token, node, ArraySpan<AstExpression>.Empty);
            }


            return true;

        }

    }

}
