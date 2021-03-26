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
                    if (stream.CheckAndConsume(TokenTypes.LineTerminator))
                        continue;
                    if (stream.CheckAndConsume(TokenTypes.SemiColon))
                        continue;
                    if (stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                        break;
                    throw stream.Unexpected();
                } while (true);
                node = new AstBlock(begin.Token, PreviousToken, list);
            } finally
            {
                list.Clear();
            }
            return true;
        }


    }

}
