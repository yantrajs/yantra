using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.ExpHelper;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Core.LinqExpressions;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {

        protected override Expression VisitBlock(AstBlock block) {

            if (block.Statements.Length == 0)
                return Expression.Empty;

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
                    //LexicalScopeBuilder.Update(
                    //    blockList, 
                    //    scope.StackItem, 
                    //    stmt.Start.Start.Line, 
                    //    stmt.Start.Start.Column);
                    var exp = Visit(stmt);
                    if (exp == null)
                        continue;
                    blockList.Add(CallStackItemBuilder.Step(scope.StackItem, stmt.Start.Start.Line, stmt.Start.Start.Column));
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
