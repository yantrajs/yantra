using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {

        bool Block(out AstBlock node)
        {
            var begin = Location;
            SparseList<AstStatement> list = new SparseList<AstStatement>();
            do
            {
                if (Statement(out var stmt))
                {
                    list.Add(stmt);
                }
                if (EndOfStatement())
                    break;
            } while (true);
            node = new AstBlock(begin.Token, PreviousToken, list);
            return true;
        }


    }

}
