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


        private Expression Scoped(FastFunctionScope scope, FastList<Expression> body) {
            var list = pool.AllocateList<Exp>();
            try {
                list.AddRange(scope.InitList);
                list.AddRange(body);
                return Exp.Block(scope.VariableParameters, list);
            } finally {
                list.Clear();
            }
        }


        protected override Expression VisitProgram(AstProgram program) {
            var blockList = pool.AllocateList<Expression>();
            var hoistingScope = program.HoistingScope;
            var scope = this.scope.Push(new FastFunctionScope(this.scope.Top));
            try {
                if (hoistingScope != null) {
                    var en = hoistingScope.Value.GetEnumerator();
                    var top = this.scope.Top;
                    while (en.MoveNext(out var v)) {
                        var g = JSValueBuilder.Index(top.Context, KeyOfName(v));

                        var vs = scope.CreateVariable(v, null, true);
                        vs.Expression = JSVariableBuilder.Property(vs.Variable);
                        vs.SetInit(JSVariableBuilder.New(g, v));
                    }
                }

                var se = program.Statements.GetEnumerator();
                while (se.MoveNext(out var stmt)) {
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
            } finally {
                blockList.Clear();
                scope.Dispose();
            }
        }
    }
}
