using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    partial class FastParser
    {

        public AstLiteral ExpectStringLiteral()
        {
            var token = stream.Expect(TokenTypes.String);
            return new AstLiteral(TokenTypes.String, token);
        }

        public bool Export(out AstStatement statement)
        {
            var token = stream.Current;
            stream.Consume();

            if (token.IsKeyword)
            {
                switch (token.Keyword)
                {
                    case FastKeywords.@default:
                        stream.Consume();
                        if (!Expression(out var argument))
                            throw stream.Unexpected();
                        statement = new AstExportStatement(token, argument, true);
                        return true;
                    case FastKeywords.function:
                        if (!FunctionExpression(out var f))
                            throw stream.Unexpected();
                        var fn = f as AstFunctionExpression;
                        if (fn.Id == null)
                            throw new FastParseException(f.Start, "exported function must have a name");
                        statement = new AstExportStatement(token, fn.Id);
                        return true;
                    case FastKeywords.@class:
                        if (!ClassExpression(out var @class))
                            throw stream.Unexpected();
                        var c = @class as AstClassExpression;
                        if (c.Identifier == null)
                            throw new FastParseException(c.Start, "exported class must have a name");
                        statement = new AstExportStatement(token, c);
                        return true;
                    case FastKeywords.var:
                        if (!VariableDeclaration(out var stmt))
                            throw stream.Unexpected();
                        statement = new AstExportStatement(token, stmt);
                        return true;
                    case FastKeywords.let:
                        if (!VariableDeclaration(out stmt, FastVariableKind.Let))
                            throw stream.Unexpected();
                        statement = new AstExportStatement(token, stmt);
                        return true;
                    case FastKeywords.@const:
                        if (!VariableDeclaration(out stmt, FastVariableKind.Const))
                            throw stream.Unexpected();
                        statement = new AstExportStatement(token, stmt);
                        return true;
                }
            }

            if(stream.CheckAndConsume(TokenTypes.Multiply))
            {
                stream.ExpectContextualKeyword(FastKeywords.from);

                var literal = ExpectStringLiteral();
                statement = new AstExportStatement(token, null, literal);
                return true;
            }

            if(AssignmentLeftPattern(out var declaration, FastVariableKind.Var, true))
            {
                stream.ExpectContextualKeyword(FastKeywords.from);

                var literal = ExpectStringLiteral();
                statement = new AstExportStatement(token, declaration, literal);
                return true;
            }
            throw stream.Unexpected();
        }

    }

}
