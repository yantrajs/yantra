using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool Identitifer(out AstIdentifier node)
        {
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
