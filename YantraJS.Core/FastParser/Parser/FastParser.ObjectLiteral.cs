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
            var nodes = Pool.AllocateList<ObjectProperty>();
            try
            {
                while (!stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                {

                    if (!PropertyName(out var key))
                        throw stream.Unexpected();

                    if (!stream.CheckAndConsume(TokenTypes.Colon))
                    {
                        // it is short circuit property name..
                        // only if the property name is an identifier...
                        if (key.Type != FastNodeType.Identifier)
                            throw stream.Unexpected();


                    }

                    var spread = false;
                    // check for spread operator ...
                    if (stream.CheckAndConsume(TokenTypes.TripleDots))
                    {
                        spread = true;
                    }
                }



            } finally {
                nodes.Clear();
            }

            return begin.Reset();
        }


    }

}
