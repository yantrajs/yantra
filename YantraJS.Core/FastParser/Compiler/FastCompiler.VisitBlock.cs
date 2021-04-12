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
                         scope.CreateVariable(v, null, true);
                    }
                }

                var se = block.Statements.GetEnumerator();
                while (se.MoveNext(out var stmt))
                {
                    LexicalScopeBuilder.Update(
                        blockList, 
                        scope.StackItem, 
                        stmt.Start.Start.Line, 
                        stmt.Start.Start.Column);
                    var exp = Visit(stmt);
                    if (exp == null)
                        continue;
                    blockList.Add(exp);
                }
                return Scoped(scope, blockList);
            }
            finally
            {
                blockList.Clear();
                scope.Dispose();
            }
        }
    }
}
