using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {




        bool PropertyName(out AstExpression node)
        {
            var begin = Location;

            if (Identitifer(out var id)) {
                node = id;
                return true;
            }
            if (StringLiteral(out node)) {
                return true;
            }

            if(NumberLiteral(out node))
            {
                return true;
            }
            if(stream.CheckAndConsume(TokenTypes.SquareBracketStart))
            {
                if (!SingleExpression(out node))
                    throw stream.Unexpected();
                stream.Expect(TokenTypes.SquareBracketEnd);
                node = new AstMemberExpression(begin.Token, PreviousToken, node, true);
                return true;
            }
            throw stream.Unexpected();
        }


    }

}
