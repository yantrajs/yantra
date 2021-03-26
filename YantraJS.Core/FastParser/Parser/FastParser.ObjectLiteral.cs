using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

   
    partial class FastParser
    {



        bool ObjectLiteral(out AstExpression node)
        {
            var begin = Location;
            node = default;
            stream.Consume();
            var nodes = Pool.AllocateList<ObjectProperty>();
            try
            {
                while (!stream.CheckAndConsumeAny(TokenTypes.CurlyBracketEnd, TokenTypes.EOF))
                {

                    var spread = false;
                    AstExpression key;
                    if (stream.CheckAndConsume(TokenTypes.TripleDots))
                    {
                        spread = true;
                        if (!SingleExpression(out key))
                            throw stream.Unexpected();
                    }
                    else
                    {
                        if (!PropertyName(out key))
                            throw stream.Unexpected();
                    }

                    if (!stream.CheckAndConsume(TokenTypes.Colon))
                    {
                        // it is short circuit property name..
                        // only if the property name is an identifier...
                        if (!spread && key.Type != FastNodeType.Identifier)
                            throw stream.Unexpected();

                        nodes.Add(new ObjectProperty(key, key, spread));

                        if (stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                            break;
                        if (!stream.CheckAndConsume(TokenTypes.Comma))
                            throw stream.Unexpected();

                        continue;

                    }

                    // check for spread operator ...
                    if (stream.CheckAndConsume(TokenTypes.TripleDots))
                    {
                        spread = true;
                    }

                    if (!Expression(out var right))
                        throw stream.Unexpected();

                    nodes.Add(new ObjectProperty(key, right, spread));

                    if (stream.CheckAndConsume(TokenTypes.Comma))
                        continue;
                }

                node = new AstObjectLiteral(begin.Token, PreviousToken, nodes);

            } finally {
                nodes.Clear();
            }

            return true;
        }


    }

}
