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

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {


        private Expression Scoped(FastFunctionScope scope, FastList<Expression> body) {
            var list = pool.AllocateList<Exp>();
            list.AddRange(scope.InitList);
            list.AddRange(body);
            if (scope.VariableParameters.Any() && !list.Any())
                throw new InvalidOperationException();
            if (!list.Any())
                return Exp.Empty;
            var r = Exp.Block(scope.VariableParameters, list);
            list.Clear();
            return r;
        }


        protected override Expression VisitProgram(AstProgram program) {
            var blockList = pool.AllocateList<Expression>();
            ref var hoistingScope = ref program.HoistingScope;
            var scope = this.scope.Push(new FastFunctionScope(this.scope.Top));
            if (hoistingScope != null)
            {
                var en = hoistingScope.Value.GetEnumerator();
                var top = this.scope.Top;
                while (en.MoveNext(out var v))
                {
                    var g = JSValueBuilder.Index(top.Context, KeyOfName(v));

                    var vs = scope.CreateVariable(v, null, true);
                    vs.Expression = JSVariableBuilder.Property(vs.Variable);
                    vs.SetInit(JSVariableBuilder.New(g, v.Value));
                }
            }

            var se = program.Statements.GetEnumerator();
            while (se.MoveNext(out var stmt)) {
                var exp = Visit(stmt);
                if (exp == null)
                    continue;
                blockList.Add(CallStackItemBuilder.Step(scope.StackItem, stmt.Start.Start.Line, stmt.Start.Start.Column));
                blockList.Add(exp);
            }
            var r = Scoped(scope, blockList);
            blockList.Clear();
            scope.Dispose();
            return r;
        }
    }
}
