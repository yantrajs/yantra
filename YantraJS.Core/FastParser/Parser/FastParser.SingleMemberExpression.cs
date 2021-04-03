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
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool SingleMemberExpression(out AstExpression node)
        {
            AstExpression prev = null;
            bool computed = false;
            do
            {

                // var begin = Location;
                if (!SingleExpression(out node))
                    return false;
                node = prev.Member(node, computed);

                var begin = Location;
                var token = begin.Token;
                switch (token.Type)
                {
                    case TokenTypes.SquareBracketStart:
                        stream.Consume();
                        if (!ExpressionSequence(out var index, TokenTypes.SquareBracketEnd))
                            throw stream.Unexpected();
                        node = node.Member(index, true);
                        break;
                    case TokenTypes.BracketStart:
                        stream.Consume();
                        if (!ExpressionArray(out var arguments))
                            throw stream.Unexpected();
                        node = new AstCallExpression(node, arguments);
                        break;
                }

                begin = Location;
                token = begin.Token;
                switch (token.Type)
                {
                    case TokenTypes.Dot:
                        prev = node;
                        stream.Consume();
                        continue;
                    default:
                        return true;
                }
            } while (true);

        }

    }

}
