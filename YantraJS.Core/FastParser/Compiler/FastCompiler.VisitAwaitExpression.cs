using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using YantraJS.Expressions;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override YExpression VisitAwaitExpression(AstAwaitExpression node)
        {
            var target = VisitExpression(node.Argument);
            return YExpression.Yield(target);
        }
    }
}
