using Esprima;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public class LexicalScopeBuilder
    {
        private static Type type = typeof(CallStackItem);

        private static MethodInfo _Pop
            = type.GetMethod(nameof(Core.CallStackItem.Pop));


        private static ConstructorInfo _New
            = typeof(Core.CallStackItem)
            .GetConstructor(BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] {
                    typeof(JSContext),
                    typeof(string),
                    StringSpanBuilder.RefType,
                    typeof(int),
                    typeof(int)
                }, null);

        public static Expression NewScope(
            Expression context,
            Expression fileName,
            Expression function,
            int line,
            int column)
        {
            return Expression.New(_New,
                context,
                fileName,
                function,
                Expression.Constant(line),
                Expression.Constant(column));
        }

        //private static PropertyInfo _Position =
        //    type.Property(nameof(Core.LexicalScope.Position));

        private static ConstructorInfo _NewPosition =
            typeof(Position).Constructor(typeof(int), typeof(int));

        private static FieldInfo _Line =
            type.InternalField(nameof(CallStackItem.Line));

        private static FieldInfo _Column =
            type.InternalField(nameof(CallStackItem.Column));

        private static MethodInfo _Update =
            type.InternalMethod(nameof(CallStackItem.Update));

        public static void Update(IList<Expression> result, Expression exp, int line, int column)
        {
            result.Add(Expression.Assign(Expression.Field(exp, _Line), Expression.Constant(line)));
            result.Add(Expression.Assign(Expression.Field(exp, _Column), Expression.Constant(column)));
            // result.Add(Expression.Call(exp, _Update));
        }


        public static Expression Update(Expression exp, int line, int column, Expression next)
        {
            return Expression.Block(
                Expression.Assign(Expression.Field(exp, _Line), Expression.Constant(line)),
                Expression.Assign(Expression.Field(exp, _Column), Expression.Constant(column)),
                next
                );
        }

        public static Expression Pop(Expression exp, Expression context)
        {
            return Expression.Call(exp, _Pop , context);
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
