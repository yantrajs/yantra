using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.CodeGen;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Expressions;
using YantraJS.Core.Types;
using YantraJS.Core.LambdaGen;

namespace YantraJS.ExpHelper
{
    public static class JSClassBuilder
    {
        //static Type type = typeof(JSClass);

        //private static ConstructorInfo _New =
        //    type.Constructor(new Type[] {
        //        typeof(JSFunctionDelegate), typeof(JSFunction), typeof(string), typeof(string)  });

        //public static MethodInfo _AddConstructor =
        //    type.PublicMethod(nameof(JSClass.AddConstructor), typeof(JSFunction));

        public static YElementInit AddConstructor(YExpression exp)
        {
            // return YExpression.ElementInit(_AddConstructor, exp);
            return YExpression.ElementInit(TypeQuery.QueryInstanceMethod<JSClass>(() =>
                (x) => x.AddConstructor((JSFunction)null))
                , exp
            );
        }


        public static YNewExpression New(
            Expression constructor,
            Expression super,
            string name,
            string code = "")
        {
            return NewLambdaExpression.NewExpression<JSClass>(
                () => () => new JSClass(
                    (JSFunctionDelegate)null,
                    (JSFunction)null,
                    (string)null,
                    (string)null),
                constructor ?? Expression.Null,
                super ?? Expression.Null,
                Expression.Constant(name),
                Expression.Constant(code)
            );
            //return Expression.New(_New,
            //    constructor ?? Expression.Null,
            //    super ?? Expression.Null,
            //    Expression.Constant(name),
            //    Expression.Constant(code));
        }
    }
}
