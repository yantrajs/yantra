using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {




        bool PropertyName(out AstExpression node, out bool computed)
        {
            var begin = Location;

            if (Identitifer(out var id)) {
                node = id;
                computed = false;
                return true;
            }
            if (StringLiteral(out node)) {
                computed = false;
                return true;
            }

            if(NumberLiteral(out node))
            {
                computed = false;
                return true;
            }
            if(stream.CheckAndConsume(TokenTypes.SquareBracketStart))
            {
                if (!Expression(out node))
                    throw stream.Unexpected();
                stream.Expect(TokenTypes.SquareBracketEnd);
                computed = true;
                return true;
            }
            node = null;
            computed = false;
            return begin.Reset();
        }


    }

}
