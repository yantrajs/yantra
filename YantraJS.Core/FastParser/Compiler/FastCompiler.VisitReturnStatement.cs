using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Exp VisitReturnStatement(AstReturnStatement returnStatement) {
            return Exp.Return(this.scope.Top.ReturnLabel,
                returnStatement.Argument != null
                ? VisitExpression(returnStatement.Argument)
                : JSUndefinedBuilder.Value);
        }
    }
}
