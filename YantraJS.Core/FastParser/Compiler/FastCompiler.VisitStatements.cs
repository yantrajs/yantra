using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.ExpHelper;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {

        private Expression VisitStatements(ArraySpan<string>? hoistingScope, in ArraySpan<AstStatement> statements)
        {
            if (hoistingScope != null)
            {
                var en = hoistingScope.Value.GetEnumerator();
                var top = this.scope.Top;
                while (en.MoveNext(out var v))
                {
                    var g = JSValueBuilder.Index(top.Context, KeyOfName(v));

                    var vs = this.scope.Top.CreateVariable(v);
                    vs.Expression = JSVariableBuilder.Property(vs.Variable);
                    vs.SetInit(JSVariableBuilder.New(g, v));
                }
            }

            var se = statements.GetEnumerator();
            var blockList = pool.AllocateList<Expression>();
            try
            {
                while (se.MoveNext(out var stmt))
                {
                    var exp = Visit(stmt);
                    if (exp == null)
                        continue;
                    blockList.Add(exp);
                }
                return Expression.Block(blockList.Release());
            }
            finally
            {
                blockList.Clear();
            }
        }
    }
}
