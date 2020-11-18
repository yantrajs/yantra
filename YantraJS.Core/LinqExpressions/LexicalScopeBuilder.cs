using Esprima;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public class LexicalScopeBuilder
    {
        private static Type type = typeof(LexicalScope);

        private static PropertyInfo _Index =
            type.IndexProperty(typeof(Core.KeyString));

        private static PropertyInfo _Top
            = typeof(LinkedStack<LexicalScope>).GetProperty(nameof(LinkedStack<LexicalScope>.Top));

        public static Expression Index(Expression exp)
        {
            var top = Expression.Property(null, _Top);
            return Expression.MakeIndex(top,
                _Index,
                new Expression[] { exp });
        }

        private static MethodInfo _Push
            = typeof(LinkedStack<Core.LexicalScope>)
            .GetMethod(nameof(Core.LinkedStack<Core.LexicalScope>.Push));

        private static MethodInfo _Pop
            = type.GetMethod(nameof(Core.LexicalScope.Pop));


        private static ConstructorInfo _New
            = typeof(Core.LexicalScope)
            .GetConstructor(BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] {
                    typeof(string),
                    StringSpanBuilder.RefType,
                    typeof(int),
                    typeof(int)
                }, null);

        public static Expression NewScope(
            Expression fileName,
            Expression function,
            int line,
            int column)
        {
            return Expression.Call(
                JSContextBuilder.Stack,
                _Push,
                Expression.New(_New,
                fileName,
                function,
                Expression.Constant(line),
                Expression.Constant(column)));
        }

        //private static PropertyInfo _Position =
        //    type.Property(nameof(Core.LexicalScope.Position));

        private static ConstructorInfo _NewPosition =
            typeof(Position).Constructor(typeof(int), typeof(int));

        private static FieldInfo _Line =
            type.InternalField(nameof(LexicalScope.Line));

        private static FieldInfo _Column =
            type.InternalField(nameof(LexicalScope.Column));

        public static Expression Update(Expression exp, int line, int column, Expression next)
        {
            return Expression.Block(
                Expression.Assign(Expression.Field(exp, _Line), Expression.Constant(line)),
                Expression.Assign(Expression.Field(exp, _Column), Expression.Constant(column)),
                next
                );
        }

        public static Expression Pop(Expression exp)
        {
            return Expression.Call(exp, _Pop);
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
