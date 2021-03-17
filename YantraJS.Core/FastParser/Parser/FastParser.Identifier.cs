using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



        bool Identitifer(out AstIdentifier node)
        {
            var begin = Location;
            if (stream.CheckAndConsume(TokenTypes.Identifier, out var token))
            {
                node = new AstIdentifier(token);
                return true;
            }
            node = null;
            return false;
        }


    }

}
