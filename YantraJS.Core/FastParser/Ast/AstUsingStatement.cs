using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser.Ast
{
    public class AstUsingStatement : AstStatement
    {
        public readonly bool IsAsync;

        public readonly AstExpression Init;

        public AstUsingStatement(
            FastToken start,
            FastNodeType type,
            FastToken end,
            bool isAsync,
            AstExpression init) : base(start, type, end)
        {
            IsAsync = isAsync;
            Init = init;
        }
    }
}
