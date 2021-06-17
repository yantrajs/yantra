using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser.Compiler
{
    internal static class StrictModeExtensions
    {
        public static void VerifyIdentifierForUpdate(this AstIdentifier id)
        {
            if (id.Name.Equals("arguments") || id.Name.Equals("eval") || id.Name.Equals("this")) {
                throw new FastParseException(id.Start, $"Invalid left-hand side expression for update");
            }
        }

        public static void VerifyIdentifierForUpdate(this AstExpression expression)
        {
            switch(expression.Type)
            {
                case FastNodeType.Identifier:
                    VerifyIdentifierForUpdate(expression as AstIdentifier);
                    return;
            }
        }

    }
}
