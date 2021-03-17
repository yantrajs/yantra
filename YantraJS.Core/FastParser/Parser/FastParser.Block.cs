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
            var list = Pool.AllocateList<AstStatement>();
            try
            {
                do
                {
                    if (Statement(out var stmt))
                    {
                        list.Add(stmt);
                    }
                    if (EndOfStatement())
                        break;
                } while (true);
                node = new AstBlock(begin.Token, PreviousToken, list.Release());
            } finally
            {
                list.Clear();
            }
            return true;
        }


    }

}
