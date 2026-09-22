using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.Core.Generator;
using YantraJS.Core.LinqExpressions;
using YantraJS.Core.Types;
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
            if (this.scope.Top.Function.Generator)
            {
                var c = TypeQuery
                    .QueryConstructor<JSAsyncValue>(() => () => new JSAsyncValue((JSValue)null));
                return YExpression.Yield(YExpression.New(c, target));
            }
            return YExpression.Yield(target);
        }
    }
}
