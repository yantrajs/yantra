using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {




        bool PropertyName(out AstExpression node, out bool computed, bool acceptKeywords = false)
        {
            var begin = BeginUndo();

            if (acceptKeywords)
            {
                var token = begin.Token;
                switch (token.Type)
                {
                    case TokenTypes.True:
                    case TokenTypes.False:
                    case TokenTypes.In:
                    case TokenTypes.InstanceOf:
                    case TokenTypes.Null:
                    case TokenTypes.Identifier:
                        stream.Consume();
                        if (token.ContextualKeyword != FastKeywords.none)
                        {
                            node = new AstIdentifier(token);
                            computed = false;
                            return true;
                        }
                        node = new AstIdentifier(token.AsString());
                        computed = false;
                        return true;
                }
            }

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
