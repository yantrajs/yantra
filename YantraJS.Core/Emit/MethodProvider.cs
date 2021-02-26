using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace YantraJS.Core.Emit
{
    public abstract class MethodProvider
    {

        public static MethodProvider Current = new ListMethodProvider();

        public abstract Expression Compile(LambdaExpression expression);

        public abstract T Get<T>(int index);
    }

    public class ListMethodProvider : MethodProvider
    {

        private List<object> list = new List<object>();

        private static Type type = typeof(MethodProvider);
        private static MethodInfo @get = type.GetMethod("Get");
        private static Expression CurrentProperty = Expression.Field(null, type.GetField( "Current"));

        public override Expression Compile(LambdaExpression expression)
        {
            var d = expression.Compile().ToTailDelegate(expression.Type, expression.Name ?? "native");
            int index = 0;
            lock (this)
            {
                index = list.Count;
                list.Add(d);
            }
            var m = get.MakeGenericMethod(expression.Type);
            return Expression.Call(CurrentProperty, m, Expression.Constant(index));
        }

        public override T Get<T>(int index)
        {
            return (T)list[index];
        }
    }
}
