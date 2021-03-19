using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

   
    partial class FastParser
    {



        bool Expression(out AstExpression node)
        {
            var begin = Location;
            node = default;

            if (SingleExpression(out node))
                return true;

            return begin.Reset();
        }


    }

}
