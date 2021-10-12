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
            node = null;
            var current = stream.Current;
            if (current.Keyword == FastKeywords.@new)
            {
                // next must be .target...
                if (stream.Next.Type != TokenTypes.Dot)
                    throw stream.Unexpected();

                stream.Consume();
                stream.Consume();
                if (!stream.CheckAndConsume(TokenTypes.Identifier, out var id))
                    throw stream.Unexpected();

                node = new AstMeta(new AstIdentifier(current.AsString()), new AstIdentifier(id));
            }
            else if (!SingleExpression(out node))
            {
                return false;
            }


            FastToken begin;
            FastToken token;

            while (true) {

                var m = stream.SkipNewLines();

                begin = stream.Current;
                token = begin;
                switch (token.Type) {

                    case TokenTypes.TemplateBegin:
                        var template = Template();
                        //node = new AstCallExpression(node,
                        //    ArraySpan<AstExpression>.From(new AstTaggedTemplateExpression(node, template.Parts)));
                        node = new AstTaggedTemplateExpression(node, template.Parts);
                        continue;

                    case TokenTypes.OptionalIndex:
                    case TokenTypes.SquareBracketStart:
                        stream.Consume();
                        if (!ExpressionSequence(out var index, TokenTypes.SquareBracketEnd))
                            throw stream.Unexpected();
                        node = node.Member(index, true, token.Type == TokenTypes.OptionalIndex);
                        continue;
                    case TokenTypes.BracketStart:
                    case TokenTypes.OptionalCall:
                        stream.Consume();
                        if (!ArrayExpression(out var arguments))
                            throw stream.Unexpected();
                        if (asNew)
                        {
                            node = new AstNewExpression(token, node, arguments);
                            asNew = false;
                        }
                        else
                            node = new AstCallExpression(node, arguments, token.Type == TokenTypes.OptionalCall);
                        continue;
                    case TokenTypes.QuestionDot:
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
                                node = node.Member(
                                    new AstIdentifier(next.AsString()), 
                                    false,
                                    token.Type == TokenTypes.QuestionDot);
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
                node = new AstNewExpression(token, node, Sequence<AstExpression>.Empty);
            }


            return true;

        }

    }

}
