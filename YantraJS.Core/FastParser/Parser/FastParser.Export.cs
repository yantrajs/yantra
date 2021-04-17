using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    partial class FastParser
    {

        public bool Export(out AstStatement statement)
        {
            var token = stream.Current;
            stream.Consume();

            switch (token.Keyword)
            {
                case FastKeywords.@default:
                    stream.Consume();
                    if (!Expression(out var argument))
                        throw stream.Unexpected();
                    statement = new AstExportStatement(token, null, argument);
                    return true;
                case FastKeywords.function:
                    if(!FunctionExpression(out var f))
                        throw stream.Unexpected();
                    var fn = f as AstFunctionExpression;
                    if (fn.Id == null)
                        throw new FastParseException(f.Start, "exported function must have a name");
                    statement = new AstExportStatement(token, fn.Id, fn);
                    return true;
                case FastKeywords.@class:
                    if (!ClassExpression(out var @class))
                        throw stream.Unexpected();
                    var c = @class as AstClassExpression;
                    if (c.Identifier == null)
                        throw new FastParseException(c.Start, "exported class must have a name");
                    statement = new AstExportStatement(token, c.Identifier, c);
                    return true;
            }

            if(!AssignmentLeftPattern(out var name, FastVariableKind.Var, modulePattern: true))
                throw stream.Unexpected();
        }

    }

}
