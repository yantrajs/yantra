using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class IElementEnumeratorBuilder
    {
        private static readonly Type type = typeof(IElementEnumerator);

        private static MethodInfo getMethod =
            typeof(JSValue).InternalMethod(
                nameof(JSValue.GetElementEnumerator));

        private static MethodInfo moveNext =
            type.InternalMethod(nameof(IElementEnumerator.MoveNext), typeof(JSValue).MakeByRefType());

        public static Expression Get(Expression target)
        {
            return Expression.Call(target, getMethod);
        }

        public static Expression MoveNext(Expression target, Expression item)
        {
            return Expression.Call(target, moveNext, item);
        }

        public static Expression AssignMoveNext(
            Expression assignee,
            Expression target,
            Expression item)
        {
            return Expression.Assign(assignee,
                Expression.Condition(
                    Expression.Call(target, moveNext, item),
                    item,
                    JSUndefinedBuilder.Value
                    ));
        }
    }
}
