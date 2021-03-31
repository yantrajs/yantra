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
                    if (stream.CheckAndConsumeAny(TokenTypes.CurlyBracketEnd, TokenTypes.EOF))
                        break;
                    if (Statement(out var stmt))
                    {
                        list.Add(stmt);
                        continue;
                    }
                    if (stream.CheckAndConsumeAny(TokenTypes.LineTerminator,TokenTypes.SemiColon))
                        continue;
                    if (stream.CheckAndConsumeAny(TokenTypes.CurlyBracketEnd, TokenTypes.EOF))
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
