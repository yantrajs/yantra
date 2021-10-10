using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {

        bool Parameters(out IFastEnumerable<VariableDeclarator> node, 
            TokenTypes endsWith = TokenTypes.BracketEnd,
            bool checkForBracketStart = true, 
            FastVariableKind kind = FastVariableKind.Var)
        {
            node = null;
            if (!stream.CheckAndConsume(TokenTypes.BracketStart))
            {
                if(checkForBracketStart)
                    return false;
            }
            var list = new Sequence<VariableDeclarator>();
            while(!stream.CheckAndConsume(endsWith)) {

                if (!AssignmentLeftPattern(out var left, kind))
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
                if (stream.CheckAndConsumeAny(endsWith, TokenTypes.EOF))
                    break;
                if (!stream.CheckAndConsume(TokenTypes.Comma))
                    break;
            }
            node = list;
            return true;
        }


    }

}
