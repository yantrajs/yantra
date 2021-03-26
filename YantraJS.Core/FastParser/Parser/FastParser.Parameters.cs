using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {

        bool Parameters(out ArraySpan<VariableDeclarator> node)
        {
            node = null;
            if (!stream.CheckAndConsume(TokenTypes.BracketStart))
                return false;
            var list = Pool.AllocateList<VariableDeclarator>();
            try
            {
                while(!stream.CheckAndConsume(TokenTypes.BracketEnd)) {

                    if (!AssignmentLeftPattern(out var left))
                        throw stream.Unexpected();
                    if (stream.CheckAndConsume(TokenTypes.Assign))
                    {
                        if (!Expression(out var init))
                            throw stream.Unexpected();
                        list.Add(new VariableDeclarator(left, init));
                    } else
                    {
                        list.Add(new VariableDeclarator(left, null));
                    }
                    if (!stream.CheckAndConsume(TokenTypes.Comma))
                        throw stream.Unexpected();
                }
                node = list;
                return true;
            } finally
            {
                list.Clear();
            }
        }


    }

}
