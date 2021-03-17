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
            List<VariableDeclarator> declarators = new List<VariableDeclarator>();

            do
            {
                if (stream.CheckAndConsume(TokenTypes.Identifier, out var id))
                {
                    // simple... variable declaration with identifier.. var a;
                    var identifier = new AstIdentifier(id);
                    if (stream.CheckAndConsume(TokenTypes.Assign))
                    {
                        if (!Expression(out var init))
                        {
                            throw new FastParseException(stream.Current, $"Init expression expected");
                        }
                        declarators.Add(new VariableDeclarator(identifier, init));
                        return true;
                    }
                }

                if(stream.CheckAndConsume(TokenTypes.CurlyBracketStart))
                {
                    // read pattern ...
                }

                if(stream.CheckAndConsume(TokenTypes.SquareBracketStart))
                {
                    // read pattern ...
                }

                if (!stream.CheckAndConsume(TokenTypes.Comma))
                    break;

            } while (true);

            node = new AstVariableDeclaration(begin.Token, stream.Previous, declarators, isLet, isConst);

            return begin.Reset();
        }


    }

}
