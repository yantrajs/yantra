using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.FastParser
{
    partial class FastParser
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NonDeclarativeStatement(out AstStatement statement)
        {
            if (!Statement(out statement))
                return false;
            if(statement.Type == FastNodeType.ExpressionStatement && statement is AstExpressionStatement exp)
            {
                switch (exp.Expression.Type)
                {
                    case FastNodeType.FunctionExpression:
                    case FastNodeType.ClassStatement:
                        throw new FastParseException(exp.Start, $"Unexpected declaration");
                }
            }
            return true;
        }
    }

}
