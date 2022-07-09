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
        class SwitchInfo {

            public SwitchInfo(FastPool.Scope scope)
            {
                Tests = new Sequence<Exp>();
            }

            public Sequence<Exp> Tests;
            public Sequence<Exp> Body;
            public readonly LabelTarget Label = Exp.Label("case-start");
        }

        protected override Expression VisitSwitchStatement(AstSwitchStatement switchStatement) {
            bool allStrings = true;
            bool allNumbers = true;
            bool allIntegers = true;


            var scope = pool.NewScope();

            try
            {

                Sequence<Exp> defBody = null;
                var @continue = this.scope.Top.Loop?.Top?.Continue;
                var @break = Exp.Label();
                var ls = new LoopScope(@break, @continue, true);
                var cases = new Sequence<SwitchInfo>(switchStatement.Cases.Count + 2);
                using (var bt = this.scope.Top.Loop.Push(ls))
                {
                    SwitchInfo lastCase = new SwitchInfo(scope);
                    var casesEn = switchStatement.Cases.GetFastEnumerator();
                    while(casesEn.MoveNext(out var c))
                    {
                        var body = new Sequence<Exp>(c.Statements.Count);
                        var en = c.Statements.GetFastEnumerator();
                        while(en.MoveNext(out var es))
                        {
                            switch (es)
                            {
                                case AstStatement stmt:
                                    body.Add(VisitStatement(stmt));
                                    break;
                                //case Esprima.Ast.Expression exp:
                                //    body.Add(VisitExpression(exp));
                                //    break;
                                default:
                                    throw new FastParseException(es.Start, $"Invalid statement {es.Type}");
                            }
                        }

                        if (c.Test == null)
                        {
                            defBody = body;
                            lastCase = new SwitchInfo(scope);
                            continue;
                        }

                        Exp test = null;
                        switch (c.Test.Type)
                        {
                            case FastNodeType.UnaryExpression:
                                var unary = c.Test as AstUnaryExpression;
                                var isTestSet = false;
                                switch (unary.Operator)
                                {
                                    case UnaryOperator.Plus:
                                    case UnaryOperator.Minus:
                                        if (unary.Argument.Type == FastNodeType.Literal)
                                        {
                                            var l = unary.Argument as AstLiteral;
                                            if (l.TokenType == TokenTypes.Number)
                                            {
                                                var n = l.NumericValue;
                                                if ((n % 1) != 0)
                                                {
                                                    allIntegers = false;
                                                }
                                                var ln = l.NumericValue;
                                                if (unary.Operator ==  UnaryOperator.Minus)
                                                {
                                                    ln = -ln;
                                                }
                                                test = Exp.Constant(ln);
                                                isTestSet = true;
                                                break;
                                            }
                                        }
                                        break;
                                }
                                if (!isTestSet)
                                {
                                    test = VisitExpression(c.Test);
                                    allNumbers = false;
                                    allStrings = false;
                                    allIntegers = false;
                                }
                                break;

                            case FastNodeType.Literal:
                                var literal = c.Test as AstLiteral;

                                switch (literal.TokenType)
                                {
                                    case TokenTypes.String:
                                        allNumbers = false;
                                        // allStrings = allStrings && true ;
                                        test = Exp.Constant(literal.StringValue);
                                        break;
                                    case TokenTypes.Number:
                                        var n = literal.NumericValue;
                                        if ((n % 1) != 0)
                                        {
                                            allIntegers = false;
                                        }
                                        test = Exp.Constant(literal.NumericValue);
                                        break;
                                    case TokenTypes.True:
                                        allNumbers = false;
                                        allStrings = false;
                                        allIntegers = false;
                                        test = JSBooleanBuilder.True;
                                        break;
                                    case TokenTypes.False:
                                        allNumbers = false;
                                        allStrings = false;
                                        allIntegers = false;
                                        test = JSBooleanBuilder.False;
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }

                                break;
                            default:
                                test = VisitExpression(c.Test);
                                allNumbers = false;
                                allStrings = false;
                                allIntegers = false;
                                break;
                        }
                        lastCase.Tests.Add(test);

                        if (body.Count > 0)
                        {
                            cases.Add(lastCase);
                            body.Insert(0, Exp.Label(lastCase.Label));
                            lastCase.Body = body;
                            lastCase = new SwitchInfo(scope);
                        }
                    }

                    System.Reflection.MethodInfo equalsMethod = null;

                    SwitchInfo last = null;
                    foreach (var @case in cases)
                    {
                        // if last one is not break statement... make it fall through...
                        if (last != null)
                        {
                            last.Body.Add(Exp.Goto(@case.Label));
                        }
                        last = @case;

                        if (allNumbers)
                        {
                            if (allIntegers)
                            {
                                @case.Tests = @case.Tests.ConvertToInteger(scope);
                            }
                            else
                            {
                                // convert every case to double..
                                @case.Tests = @case.Tests.ConvertToNumber(scope);
                            }
                        }
                        else
                        {
                            if (allStrings)
                            {
                                // force everything to string if it isn't
                                @case.Tests = @case.Tests.ConvertToString(scope);
                            }
                            else
                            {
                                @case.Tests = @case.Tests.ConvertToJSValue(scope);
                                equalsMethod = ExpHelper.JSValueBuilder.StaticEquals;
                            }
                        }


                    }

                    var testTarget = VisitExpression(switchStatement.Target);
                    if (allNumbers)
                    {
                        if (allIntegers)
                        {
                            testTarget = JSValueBuilder.IntValue(testTarget);
                        }
                        else
                        {
                            testTarget = JSValueBuilder.DoubleValue(testTarget);
                        }
                    }
                    else
                    {
                        if (allStrings)
                        {
                            testTarget = ObjectBuilder.ToString(testTarget);
                        }
                        else
                        {

                        }
                    }

                    Exp d = null;
                    var lastLine = switchStatement.Start.Start.Line;
                    if (defBody != null)
                    {
                        var defLabel = Exp.Label($"default-start-{lastLine}");
                        if (last != null)
                        {
                            last.Body.Add(Exp.Goto(defLabel));
                        }
                        defBody.Insert(0, Exp.Label(defLabel));
                        d = Exp.Block(defBody);
                    }

                    var r = Exp.Block(
                        Exp.Switch(
                            testTarget,
                            d.ToJSValue() ?? JSUndefinedBuilder.Value,
                            equalsMethod,
                            cases.Select(x => Exp.SwitchCase(Exp.Block(x.Body).ToJSValue(), x.Tests)).ToList()),
                        Exp.Label(@break));
                    return r;
                }
            } finally
            {
                scope.Dispose();
            }
        }
    }
}
