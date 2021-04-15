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
            bool afterDot = false;
            if (!SingleExpression(out node, afterDot))
            {
                return false;
            }

            StreamLocation begin;
            FastToken token;

            while (true) {

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
                        if (!ExpressionArray(out var arguments))
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
                        afterDot = true;
                        if(SingleExpression(out index, afterDot)) {
                            node = node.Member(index);
                        }
                        continue;
                    default:
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
