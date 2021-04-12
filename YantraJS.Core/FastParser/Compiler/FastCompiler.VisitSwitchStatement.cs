using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        class SwitchInfo {
            public SparseList<Exp> Tests = new SparseList<Exp>();
            public SparseList<Exp> Body;
            public readonly System.Linq.Expressions.LabelTarget Label = Exp.Label("case-start");
        }

        protected override Expression VisitSwitchStatement(AstSwitchStatement switchStatement) {
            bool allStrings = true;
            bool allNumbers = true;
            bool allIntegers = true;


            SparseList<Exp> defBody = null;
            var @continue = this.scope.Top.Loop?.Top?.Continue;
            var @break = Exp.Label();
            var ls = new LoopScope(@break, @continue, true);
            SparseList<SwitchInfo> cases = new SparseList<SwitchInfo>(switchStatement.Cases.Count + 2);
            using (var bt = this.scope.Top.Loop.Push(ls)) {
                SwitchInfo lastCase = new SwitchInfo();
                foreach (var c in switchStatement.Cases) {
                    SparseList<Exp> body = new SparseList<Exp>();
                    foreach (var es in c.Statements) {
                        switch (es) {
                            case AstStatement stmt:
                                body.Add(VisitStatement(stmt));
                                break;
                            //case Esprima.Ast.Expression exp:
                            //    body.Add(VisitExpression(exp));
                            //    break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }

                    if (c.Test == null) {
                        defBody = body;
                        lastCase = new SwitchInfo();
                        continue;
                    }

                    Exp test = null;
                    switch ((c.Test.Type, c.Test)) {
                        case (FastNodeType.Literal, AstLiteral literal):

                            switch (literal.TokenType) {
                                case TokenTypes.String:
                                    allNumbers = false;
                                    allStrings = false;
                                    test = Exp.Constant(literal.StringValue);
                                    break;
                                case TokenTypes.Number:
                                    var n = literal.NumericValue;
                                    if ((n % 1) != 0) {
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

                    if (body.Count > 0) {
                        cases.Add(lastCase);
                        body.Insert(0, Exp.Label(lastCase.Label));
                        lastCase.Body = body;
                        lastCase = new SwitchInfo();
                    }
                }

                System.Reflection.MethodInfo equalsMethod = null;

                SwitchInfo last = null;
                foreach (var @case in cases) {
                    // if last one is not break statement... make it fall through...
                    if (last != null) {
                        last.Body.Add(Exp.Goto(@case.Label));
                    }
                    last = @case;

                    if (allNumbers) {
                        if (allIntegers) {
                            @case.Tests = @case.Tests.ConvertToInteger();
                        } else {
                            // convert every case to double..
                            @case.Tests = @case.Tests.ConvertToNumber();
                        }
                    } else {
                        if (allStrings) {
                            // force everything to string if it isn't
                            @case.Tests = @case.Tests.ConvertToString();
                        } else {
                            @case.Tests = @case.Tests.ConvertToJSValue();
                            equalsMethod = ExpHelper.JSValueBuilder.StaticEquals;
                        }
                    }


                }

                var testTarget = VisitExpression(switchStatement.Target);
                if (allNumbers) {
                    if (allIntegers) {
                        testTarget = Exp.Convert(JSValueBuilder.DoubleValue(testTarget), typeof(int));
                    } else {
                        testTarget = JSValueBuilder.DoubleValue(testTarget);
                    }
                } else {
                    if (allStrings) {
                        testTarget = ObjectBuilder.ToString(testTarget);
                    } else {

                    }
                }

                Exp d = null;
                var lastLine = switchStatement.Start.Start.Line;
                if (defBody != null) {
                    var defLabel = Exp.Label($"default-start-{lastLine}");
                    if (last != null) {
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
        }
    }
}
