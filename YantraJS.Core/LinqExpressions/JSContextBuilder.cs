using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.LambdaGen;
using YantraJS.Core.LightWeight;
using YantraJS.Core.Types;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;


namespace YantraJS.ExpHelper
{
    public class JSContextStackBuilder
    {
        private readonly static Type itemType = typeof(CallStackItem);

        public readonly static Type itemTypeRef = typeof(CallStackItem).MakeByRefType();

        //public static MethodInfo _Push =
        //    type.InternalMethod(nameof(LinkedStack<LexicalScope>.Push));

        //public static MethodInfo _Pop =
        //    type.InternalMethod(nameof(LightWeightStack<CallStackItem>.Pop));

        //private static FieldInfo _fileName =
        //    itemType.InternalField(nameof(CallStackItem.FileName));

        //private static FieldInfo _Function =
        //    itemType.InternalField(nameof(CallStackItem.Function));

        //private static FieldInfo _Line =
        //    itemType.InternalField(nameof(CallStackItem.Line));

        //private static FieldInfo _Column =
        //    itemType.InternalField(nameof(CallStackItem.Column));


        public static void Push(Sequence<Expression> stmtList, Expression context,  Expression stack, 
            Expression fileName, 
            Expression  function,
            int line,
            int column)
        {
            var newScope = LexicalScopeBuilder.NewScope(context, fileName, function, line, column);
            stmtList.Add(Expression.Assign(stack, newScope ));
            //stmtList.Add(Expression.Assign(Expression.Field(stack, _fileName),fileName));
            //stmtList.Add(Expression.Assign(Expression.Field(stack, _Function), function));
            //stmtList.Add(Expression.Assign(Expression.Field(stack, _Line), Expression.Constant(line)));
            //stmtList.Add(Expression.Assign(Expression.Field(stack, _Column), Expression.Constant(column)));
        }

        public static Expression Pop(Expression stack, Expression context)
        {
            return LexicalScopeBuilder.Pop(stack, context);
        }

    }

    public class JSContextBuilder
    {

        public static Expression Current =
            NewLambdaExpression.StaticFieldExpression<JSContext>(() => () => JSContext.Current);

        public static Expression Object(Expression current) {
            return current.FieldExpression<JSContext, JSObject>(() => (x) => x.Object);
        }

        public static Expression NewTarget()
        {

            return Current
                .FieldExpression<JSContext, CallStackItem>(() => (x) => x.Top)
                .FieldExpression<CallStackItem, JSFunction>(() => (x) => x.NewTarget);
        }


        public static Expression Register(ParameterExpression lScope, ParameterExpression variable)
        {
            return lScope.CallExpression<JSContext, JSVariable, JSValue>(
                () => (x, a) => x.Register(a),
                variable);
        }
    }
}
