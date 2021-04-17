using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    partial class FastParser
    {
        bool Import(out AstStatement statement)
        {
            var token = stream.Current;
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
            AstExpression all = null;
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
                    else if (!AssignmentLeftPattern(out all, FastVariableKind.Var, true))
                        throw stream.Unexpected();
                }

                stream.ExpectContextualKeyword(FastKeywords.from);
                var literal = ExpectStringLiteral();

                statement = new AstImportStatement(token, id, all, literal);
                return true;
            }

            if(AssignmentLeftPattern(out all, FastVariableKind.Var, true))
            {
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
