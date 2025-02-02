using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Core.Core.Disposable;
using YantraJS.Core.LambdaGen;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {


        private Expression Scoped(FastFunctionScope scope, IFastEnumerable<Expression> body) {

            var list = new Sequence<Exp>();
            list.AddRange(scope.InitList);
            list.AddRange(body);
            if (scope.VariableParameters.Any() && !list.Any())
                throw new InvalidOperationException();
            if (!list.Any())
                return Exp.Empty;

            //if (scope.HasDisposable)
            //{
            //    // create new disposable and assign ...
            //    list.Add(
            //        Expression.Assign(
            //            scope.Disposable.Variable,
            //            Expression.New(scope.Disposable.Type)
            //        )
            //    );
            //}


            var r = Exp.Block(scope.VariableParameters.AsSequence(), list);
            // list.Clear();

            if (scope.HasDisposable)
            {
                list = new Sequence<Exp>
                {
                    // create new disposable and assign ...
                    Expression.Assign(
                        scope.Disposable,
                        Expression.New(scope.Disposable.Type)
                    )
                };

                var d = scope.Disposable;
                var dispose = d.CallExpression<JSDisposableStack, JSValue>(() => (j) => j.Dispose());
                if (scope.Function.Async)
                {
                    // we will move everything inside await dispose...
                    list.Add(Exp.TryFinally(
                        r,
                        Exp.Yield(dispose)
                    ));
                } else
                {
                    list.Add(Exp.TryFinally(
                        r,
                        dispose
                    ));
                }

                return Exp.Block( new Sequence<ParameterExpression> { scope.Disposable }, list);
            }
            return r;
        }


        protected override Expression VisitProgram(AstProgram program) {
            var blockList = new Sequence<Expression>(program.Statements.Count);
            ref var hoistingScope = ref program.HoistingScope;
            var scope = this.scope.Push(new FastFunctionScope(this.scope.Top));
            if (hoistingScope != null)
            {
                var en = hoistingScope.GetFastEnumerator();
                var top = this.scope.Top;
                while (en.MoveNext(out var v))
                {
                    var g = JSValueBuilder.Index(top.Context, KeyOfName(v));

                    var vs = scope.CreateVariable(v, null, true);
                    vs.Expression = JSVariableBuilder.Property(vs.Variable);
                    vs.SetInit(JSVariableBuilder.New(g, v.Value));
                }
            }

            var se = program.Statements.GetFastEnumerator();
            while (se.MoveNext(out var stmt)) {
                var exp = Visit(stmt);
                if (exp == null)
                    continue;
                blockList.Add(CallStackItemBuilder.Step(scope.StackItem, stmt.Start.Start.Line, stmt.Start.Start.Column));
                blockList.Add(exp);
            }
            var r = Scoped(scope, blockList);
            // blockList.Clear();
            scope.Dispose();
            return r;
        }
    }
}
