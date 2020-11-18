using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.LightWeight;

namespace YantraJS.ExpHelper
{
    public class JSContextStackBuilder
    {
        private readonly static Type type = typeof(LightWeightStack<CallStackItem>);

        private readonly static Type itemType = typeof(CallStackItem);

        public readonly static Type itemTypeRef = typeof(CallStackItem).MakeByRefType();

        public static MethodInfo _Push =
            type.InternalMethod(nameof(LightWeightStack<CallStackItem>.Push));

        public static MethodInfo _Pop =
            type.InternalMethod(nameof(LightWeightStack<CallStackItem>.Pop));

        private static FieldInfo _fileName =
            itemType.InternalField(nameof(CallStackItem.FileName));

        private static FieldInfo _Function =
            itemType.InternalField(nameof(CallStackItem.Function));

        private static FieldInfo _Line =
            itemType.InternalField(nameof(CallStackItem.Line));

        private static FieldInfo _Column =
            itemType.InternalField(nameof(CallStackItem.Column));


        public static void Push(List<Expression> stmtList, Expression stack, 
            Expression fileName, 
            Expression  function,
            int line,
            int column)
        {
            stmtList.Add(Expression.Assign(stack, Expression.Call(JSContextBuilder.Stack, _Push)));
            stmtList.Add(Expression.Assign(Expression.Field(stack, _fileName),fileName));
            stmtList.Add(Expression.Assign(Expression.Field(stack, _Function), function));
            stmtList.Add(Expression.Assign(Expression.Field(stack, _Line), Expression.Constant(line)));
            stmtList.Add(Expression.Assign(Expression.Field(stack, _Column), Expression.Constant(column)));
        }

        public static Expression Pop(Expression stack)
        {
            return Expression.Call(JSContextBuilder.Stack, _Pop);
        }

        internal static Expression Update(ParameterExpression stack, int line, int column, Expression next)
        {
            return Expression.Block(
                Expression.Assign(Expression.Field(stack, _Line), Expression.Constant(line)),
                Expression.Assign(Expression.Field(stack, _Column), Expression.Constant(column)),
                next
                );
        }
    }

    public class JSContextBuilder
    {

        private static Type type = typeof(JSContext);


        public static Expression Current =
            Expression.Property(null, type.Property(nameof(JSContext.Current)));

        //public static Expression Current =
        //    Expression.Field(null, type.InternalField(nameof(JSContext.CurrentContext)));

        public static Expression Object =
            Expression.Field(Current, type.GetField(nameof(JSContext.Object)));

        public static Expression Stack =
            Expression.Field(Current, type.InternalField(nameof(JSContext.Stack)));

        public static MethodInfo _Register =
            type.InternalMethod(nameof(JSContext.Register), typeof(JSVariable));

        private static PropertyInfo _Index =
            type.IndexProperty(typeof(Core.KeyString));
        public static Expression Index(Expression key)
        {
            return Expression.MakeIndex(Current, _Index, new Expression[] { key });
        }

        //public static Expression Pop(Expression context)s
        //{
        //    return Expression.Call(context, _Pop);
        //}

        //public static void Push(List<Expression> stmtList,
        //    ParameterExpression refStack,
        //    Expression fileName, 
        //    Expression str, 
        //    int line, 
        //    int column)
        //{
        //    //return Expression.Call(
        //    //    context,
        //    //    _Push,
        //    //    fileName,
        //    //    str,
        //    //    Expression.Constant(line),
        //    //    Expression.Constant(column));
        //}
        //public static Expression Update(
        //    ParameterExpression stack, 
        //    Expression function,
        //    int line, int column)
        //{
        //    return Expression.Block();
        //}


        public static Expression Register(ParameterExpression lScope, ParameterExpression variable)
        {
            return Expression.Call(lScope, _Register, variable);
        }
    }
}
