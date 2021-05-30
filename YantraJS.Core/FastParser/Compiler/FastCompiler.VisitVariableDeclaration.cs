using System;
using System.Collections.Generic;
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

        protected override Expression VisitVariableDeclaration(AstVariableDeclaration variableDeclaration)
        {
            var list = pool.AllocateList<Exp>();
            var top = this.scope.Top;
            var newScope = variableDeclaration.Kind == FastVariableKind.Const
                || variableDeclaration.Kind == FastVariableKind.Let;
            try {
                var ed = variableDeclaration.Declarators.GetEnumerator();
                while(ed.MoveNext(out var d)) {
                    switch(d.Identifier.Type) {
                        case FastNodeType.Identifier:
                            var id = d.Identifier as AstIdentifier;
                            var v = top.CreateVariable(id.Name, JSVariableBuilder.New(id.Name.Value), newScope);
                            if(d.Init==null) {
                                list.Add(v.Expression);
                            } else {
                                list.Add(Exp.Assign(v.Expression, Visit(d.Init)));
                            }
                            break;
                        case FastNodeType.ObjectPattern:
                            var objectPattern = d.Identifier as AstObjectPattern;
                            using (var temp = top.GetTempVariable()) {
                                if (d.Init != null)
                                    list.Add(Exp.Assign(temp.Variable, Visit(d.Init)));
                                list.Add(CreateAssignment(objectPattern, temp.Expression, true, newScope));
                            }
                            break;
                        case FastNodeType.ArrayPattern: 
                            var arrayPattern = d.Identifier as AstArrayPattern;
                            using (var temp = this.scope.Top.GetTempVariable()) {
                                if(d.Init != null )
                                    list.Add(Exp.Assign(temp.Variable, Visit(d.Init)));
                                list.Add(CreateAssignment(arrayPattern, temp.Expression, true, newScope));
                            }
                            break;
                        default:
                            throw new FastParseException(d.Identifier.Start, $"Invalid pattern {d.Identifier.Type}");
                    }
                }
                if (list.Count == 1)
                    return list[0];
                return Exp.Block(list);
            } finally {
                list.Clear();
            }
        }
    }
}
