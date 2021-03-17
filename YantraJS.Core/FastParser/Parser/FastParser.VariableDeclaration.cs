using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



        bool VariableDeclaration(out AstStatement node, bool isLet = false, bool isConst = false)
        {
            var begin = Location;
            node = default;

            stream.Consume();
            var declarators = Pool.AllocateList<VariableDeclarator>();
            try
            {
                do
                {
                    if (AssignmentLeftPattern(out var pattern))
                    {
                        if (stream.CheckAndConsume(TokenTypes.Assign))
                        {
                            if (!Expression(out var init))
                            {
                                throw new FastParseException(stream.Current, $"Init expression expected");
                            }
                            declarators.Add(new VariableDeclarator(pattern, init));
                            continue;
                        }
                    }
                    if (!stream.CheckAndConsume(TokenTypes.Comma))
                        break;

                } while (true);

                node = new AstVariableDeclaration(begin.Token, stream.Previous, declarators.Release(), isLet, isConst);
            } finally {
                declarators.Clear();
            }

            return begin.Reset();
        }


    }

}
