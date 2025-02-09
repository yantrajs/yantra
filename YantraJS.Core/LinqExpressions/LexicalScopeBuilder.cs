using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Core.LambdaGen;

namespace YantraJS.ExpHelper
{
    public class LexicalScopeBuilder
    {
        //private static Type type = typeof(CallStackItem);

        //private static MethodInfo _Pop
        //    = type.GetMethod(nameof(Core.CallStackItem.Pop));


        //private static ConstructorInfo _New
        //    = typeof(Core.CallStackItem)
        //    .PublicConstructor(
        //            typeof(JSContext),
        //            typeof(string),
        //            StringSpanBuilder.RefType,
        //            typeof(int),
        //            typeof(int)
        //        );

        public static Expression NewScope(
            Expression context,
            Expression fileName,
            Expression function,
            int line,
            int column)
        {
            return NewLambdaExpression.NewExpression<CallStackItem>(() => () => new CallStackItem(null as JSContext, "", "", 0, 0)
                ,context
                ,fileName
                ,function
                ,Expression.Constant(line)
                ,Expression.Constant(column));
            //return Expression.New(_New,
            //    context,
            //    fileName,
            //    function,
            //    Expression.Constant(line),
            //    Expression.Constant(column));
        }

        //private static PropertyInfo _Position =
        //    type.Property(nameof(Core.LexicalScope.Position));

        //private static FieldInfo _Line =
        //    type.PublicField(nameof(CallStackItem.Line));

        //private static FieldInfo _Column =
        //    type.PublicField(nameof(CallStackItem.Column));

        //private static MethodInfo _Update =
        //    type.InternalMethod(nameof(CallStackItem.Update));


        //public static Expression Update(Expression exp, int line, int column, Expression next)
        //{
        //    return Expression.Block(
        //        Expression.Assign(Expression.Field(exp, _Line), Expression.Constant(line)),
        //        Expression.Assign(Expression.Field(exp, _Column), Expression.Constant(column)),
        //        next
        //        );
        //}

        public static Expression Pop(Expression exp, Expression context)
        {
            return exp.CallExpression<CallStackItem>(() => (x) => x.Pop(null as JSContext), context);
            // return Expression.Call(exp, _Pop , context);
        }

        //public static Expression SetPosition(Expression exp, int line, int column)
        //{
        //    return Expression.Assign(
        //        Expression.Property(exp, _Position),
        //        Expression.New(_NewPosition, Expression.Constant(line), Expression.Constant(column)
        //        ));
        //}

    }
}
