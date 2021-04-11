using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.ExpHelper;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {

        protected override Expression VisitBlock(AstBlock block) {
            var blockList = pool.AllocateList<Expression>();
            var hoistingScope = block.HoistingScope;
            var scope = this.scope.Push(new FastFunctionScope(this.scope.Top));
            try
            {
                if (hoistingScope != null)
                {
                    var en = hoistingScope.Value.GetEnumerator();
                    while (en.MoveNext(out var v))
                    {
                         scope.CreateVariable(v);
                    }
                }

                var se = block.Statements.GetEnumerator();
                while (se.MoveNext(out var stmt))
                {
                    var exp = Visit(stmt);
                    if (exp == null)
                        continue;
                    blockList.Add(exp);
                }
                return Expression.Block(scope.VariableParameters, blockList.Release());
            }
            finally
            {
                blockList.Clear();
                scope.Dispose();
            }
        }
    }
}
