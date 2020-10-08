using Esprima;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
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
            var top = Expression.Property(JSContextBuilder.CurrentScope, _Top);
            return Expression.MakeIndex(top,
                _Index,
                new Expression[] { exp });
        }

        private static MethodInfo _Push
            = typeof(LinkedStack<Core.LexicalScope>)
            .GetMethod(nameof(Core.LinkedStack<Core.LexicalScope>.Push));

        private static ConstructorInfo _New
            = typeof(Core.LexicalScope)
            .GetConstructor(BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] {
                    typeof(string),
                    typeof(string),
                    typeof(int),
                    typeof(int)
                }, null);

        public static Expression NewScope(
            Expression fileName,
            string function,
            int line,
            int column)
        {
            return Expression.Call(
                JSContextBuilder.CurrentScope,
                _Push,
                Expression.New(_New,
                fileName,
                Expression.Constant(function),
                Expression.Constant(line),
                Expression.Constant(column)));
        }

        private static PropertyInfo _Position =
            type.Property(nameof(Core.LexicalScope.Position));

        private static ConstructorInfo _NewPosition =
            typeof(Position).Constructor(typeof(int), typeof(int));

        public static Expression SetPosition(Expression exp, int line, int column)
        {
            return Expression.Assign(
                Expression.Property(exp, _Position),
                Expression.New(_NewPosition, Expression.Constant(line), Expression.Constant(column)
                ));
        }

    }
}
