using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.LinqExpressions;
using AssignmentOperator = Esprima.Ast.AssignmentOperator;

namespace WebAtoms.CoreJS.Utils
{
    public static class BinaryOperation
    {
        public static Expression Assign(Expression left, Expression right, AssignmentOperator assignmentOperator)
        {
            switch(assignmentOperator)
            {
                case AssignmentOperator.Assign:
                    return Expression.Assign(left, right);
                case AssignmentOperator.PlusAssign:
                    return Expression.Assign(left, Add(left,right));
            }

            throw new NotSupportedException();
        }

        public static Expression Add(Expression leftExp, Expression right)
        {
            object obj = 4;
            
            var undefined = Expression.Constant("undefined");
            var @null = Expression.Constant("null");
            var nan = ExpHelper.JSContext.NaN;
            var zero = ExpHelper.JSContext.Zero;
            Func<ParameterExpression, Expression> caseUndefined = (left) => 
                Switch(right,
                Case<JSUndefined>(x => nan),
                Case<JSNumber>(x => nan),
                Case<double>(x => nan),
                Case<string>(x => ExpHelper.JSString.ConcatBasicStrings(undefined, right)),
                Default(ExpHelper.JSString.ConcatBasicStrings(undefined, ExpHelper.Object.ToString(right) ))
                );

            Func<ParameterExpression, Expression> caseNull = (left) =>
                Switch(right,
                Case<JSUndefined>(x => nan),
                Case<JSNull>(x => zero),
                Case<JSNumber>(x => right),
                Case<double>(x => ExpHelper.JSNumber.New(right)),
                Case<string>(x => ExpHelper.JSString.ConcatBasicStrings(@null, right)),
                Default(ExpHelper.JSString.ConcatBasicStrings(@null, ExpHelper.Object.ToString(right)))
                );

            // string case avoids toString  of JSString by accessing value directly...
            Func<ParameterExpression, Expression> caseJSString = (left) =>
                Switch(right,
                Case<JSUndefined>(x => ExpHelper.JSString.ConcatBasicStrings( ExpHelper.JSString.Value(left), undefined)),
                Case<JSNull>(x => ExpHelper.JSString.ConcatBasicStrings(ExpHelper.JSString.Value(left), @null)),
                Case<JSNumber>(x => ExpHelper.JSString.ConcatBasicStrings(
                    ExpHelper.JSString.Value(left),
                    ExpHelper.Object.ToString(ExpHelper.JSNumber.Value(x)))),
                Case<double>(x => ExpHelper.JSString.ConcatBasicStrings(
                    ExpHelper.JSString.Value(left),
                    ExpHelper.Object.ToString(x))),
                Case<string>(x => ExpHelper.JSString.ConcatBasicStrings(@null, x)),
                Default(ExpHelper.JSString.ConcatBasicStrings(@null, ExpHelper.Object.ToString(right)))
                );

            // JSNumber is the most complicated one, and will be too big, so we will
            // call a method on it ..
            // also it should be the first case as most likely we will add numbers and strings...
            Func<ParameterExpression, Expression> caseJSNumber = (left) =>
                ExpHelper.JSNumber.AddValue(left, right);

            return Switch(leftExp,
                    Case<JSNumber>(caseJSNumber),
                    Case<JSString>(caseJSString),
                    Case<JSUndefined>(caseUndefined),
                    Case<JSNull>(caseNull),
                    Default(ExpHelper.JSContext.NaN)
                );
             
            
        }
        public static TypeCheckCase Case<T>(Func<ParameterExpression, Expression> e)
        {
            return new TypeCheckCase
            {
                Type = typeof(T),
                TrueCase = e
            };
        }

        public static TypeCheckCase Default(Expression e)
        {
            return new TypeCheckCase
            {
                Type = null,
                TrueCase = (a) => e
            };
        }

        public class TypeCheckCase
        {
            public Type Type { get; set; }

            public Func<ParameterExpression,Expression> TrueCase { get; set; }


        }

        public static Expression 
            Switch(Expression right,
                    params TypeCheckCase[] cases)
        {

            var defaultCase = cases.First(x => x.Type == null);
            var allCases = cases.Where(x => x.Type != null);

            Expression condition = defaultCase.TrueCase(null);
            foreach (var @case in allCases)
            {
                var bp = Expression.Parameter(@case.Type);

                if (@case.Type.IsValueType)
                {
                    condition = Expression.Condition(
                        Expression.TypeIs(right, @case.Type),
                        Expression.Block(new ParameterExpression[] { bp },
                            @case.TrueCase(bp)
                        ),
                        condition,
                        typeof(JSValue)
                        );
                    continue;
                }

                var nbt = Expression.Constant(null, @case.Type);
                condition = Expression.Block( new ParameterExpression[] {bp},
                    Expression.Assign(bp,Expression.TypeAs(right,@case.Type)),
                    Expression.Condition(
                        Expression.NotEqual(nbt,bp),
                        @case.TrueCase(bp),
                        condition,
                        typeof(JSValue)
                    ));
            }
            return condition;
        }

        //public static (ParameterExpression pe, Expression exp)
        //    Check<TLeft, TRight>(Expression target, Expression andAlso, Func<Expression> trueExp, Func<Expression> falseExp)
        //{
        //    var type = typeof(TLeft);
        //    var nt = Expression.Constant(null, type);
        //    var pe = Expression.Parameter(type);
        //    var exp = Expression.Block(new ParameterExpression[] { pe },
        //        Expression.Assign(pe, Expression.TypeAs(target, type)),
        //        Expression.Condition(
        //            Expression.AndAlso( Expression.NotEqual(nt, pe), andAlso),
        //            trueExp(),
        //            falseExp()
        //        ));
        //    return (pe, exp);
        //}

    }
}
