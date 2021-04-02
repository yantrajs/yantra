using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



        bool VariableDeclaration(out AstStatement node, FastVariableKind kind = FastVariableKind.Var)
        {
            var begin = Location;
            node = default;
            stream.Consume();

            if (!Parameters(out var declarators, TokenTypes.SemiColon, false))
                throw stream.Unexpected();

            node = new AstVariableDeclaration(begin.Token, PreviousToken, declarators, kind);
            return true;
        }

        bool VariableDeclarationStatement(out AstVariableDeclaration node, FastVariableKind kind = FastVariableKind.Var)
        {
            var begin = Location;
            node = default;
            stream.Consume();

            if (!Parameters(out var declarators, TokenTypes.SemiColon, false))
                throw stream.Unexpected();

            node = new AstVariableDeclaration(begin.Token, PreviousToken, declarators, kind);
            return true;
        }


    }

}
