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
        protected override Exp VisitNewExpression(AstNewExpression newExpression) {
            var constructor = VisitExpression(newExpression.Callee);
            var args = VisitArguments(null, in newExpression.Arguments);
            return ExpHelper.JSValueBuilder.CreateInstance(constructor, args);
        }
    }
}
