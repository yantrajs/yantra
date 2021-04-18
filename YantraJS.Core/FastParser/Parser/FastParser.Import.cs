using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    partial class FastParser
    {
        bool Import(FastToken token, out AstStatement statement)
        {
            stream.Consume();

            AstIdentifier id;

            if (stream.CheckAndConsume(TokenTypes.Multiply))
            {
                stream.ExpectContextualKeyword(FastKeywords.@as);
                if (!Identitifer(out id))
                    throw stream.Unexpected();

                stream.ExpectContextualKeyword(FastKeywords.from);

                var literal = ExpectStringLiteral();

                statement = new AstImportStatement(token, null, id, literal);
                return true;

            }
            AstExpression ap = null;
            AstNode all = null;
            if(Identitifer(out id))
            {
                if(stream.CheckAndConsume(TokenTypes.Comma))
                {
                    if (stream.CheckAndConsume(TokenTypes.Multiply))
                    {
                        stream.ExpectContextualKeyword(FastKeywords.@as);
                        if (!Identitifer(out var allid))
                            throw stream.Unexpected();
                        all = allid;
                    }
                    else if (AssignmentLeftPattern(out ap, FastVariableKind.Var, true)) {

                        var vd = VariableDeclarator.From(Pool, ap);
                        // convert to vd...
                        all = new AstVariableDeclaration(token, all.End, vd);

                    } else throw stream.Unexpected();
                }

                stream.ExpectContextualKeyword(FastKeywords.from);
                var literal = ExpectStringLiteral();

                statement = new AstImportStatement(token, id, all, literal);
                return true;
            }

            if(AssignmentLeftPattern(out ap, FastVariableKind.Var, true))
            {
                var vd = VariableDeclarator.From(Pool, ap);
                // convert to vd...
                all = new AstVariableDeclaration(token, ap.End, vd);
                if (stream.CheckAndConsume(TokenTypes.Comma))
                {
                    if (!Identitifer(out id))
                        throw stream.Unexpected();
                }
                stream.ExpectContextualKeyword(FastKeywords.from);
                var literal = ExpectStringLiteral();
                statement = new AstImportStatement(token, id, all, literal);
                return true;
            }



            throw stream.Unexpected();
        }
    }

}
