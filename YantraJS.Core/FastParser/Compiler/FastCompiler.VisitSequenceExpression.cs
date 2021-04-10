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
        protected override Expression VisitSequenceExpression(AstSequenceExpression sequenceExpression)
        {
            var list = pool.AllocateList<Exp>();
            try {
                var e = sequenceExpression.Expressions.GetEnumerator();
                while (e.MoveNext(out var exp))
                {
                    if (exp != null) list.Add(Visit(exp));
                }
                return Exp.Block(list.ToSpan());
            } finally {
                list.Clear();
            }
        }
    }
}
