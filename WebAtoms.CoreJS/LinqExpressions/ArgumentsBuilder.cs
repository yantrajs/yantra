using System;
using System.Collections.Generic;
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
            typeof(JSValue).InternalMethod(nameof(JSValue.GetElementEnumerator));

        private static MethodInfo moveNext =
            type.GetMethod(nameof(IElementEnumerator.MoveNext));

        public static Expression Get(Expression target)
        {
            return Expression.Call(target, getMethod);
        }

        public static Expression MoveNext(Expression target, Expression hasValue, Expression item, Expression index)
        {
            return Expression.Call(target, moveNext, hasValue, item, index);
        }

        public static Expression AssignMoveNext(
            Expression assignee,
            Expression target, 
            Expression hasValue,
            Expression item,
            Expression index)
        {
            return Expression.Assign(assignee, 
                Expression.Condition(
                    Expression.Call(target, moveNext, hasValue, item, index),
                    item,
                    JSUndefinedBuilder.Value
                    ) );
        }
    }

    public class ArgumentsBuilder
    {
        private static readonly Type type = typeof(Arguments);

        internal static readonly Type refType = type.MakeByRefType();

        private readonly static Expression _Empty =
            Expression.Field(null, type.GetField(nameof(Arguments.Empty)));

        public static Expression Empty()
        {
            return _Empty;
        }

        private readonly static ConstructorInfo _New0
            = type.Constructor(new Type[] { typeof(JSValue) });

        private readonly static ConstructorInfo _New1
            = type.Constructor(new Type[] { typeof(JSValue), typeof(JSValue) });

        private readonly static ConstructorInfo _New2
            = type.Constructor(new Type[] { typeof(JSValue), typeof(JSValue), typeof(JSValue) });

        private readonly static ConstructorInfo _New3
            = type.Constructor(new Type[] { typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue) });

        private readonly static ConstructorInfo _New4
            = type.Constructor(new Type[] { typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue) });

        private readonly static ConstructorInfo _New
            = type.Constructor(new Type[] { typeof(JSValue), typeof(JSValue[]) });
        public static Expression New(Expression @this, List<Expression> args)
        {
            var newList = new List<Expression>() { @this };
            newList.AddRange(args);
            switch (args.Count)
            {
                case 0:
                    return Expression.New(_New0, newList);
                case 1:
                    return Expression.New(_New1, newList);
                case 2:
                    return Expression.New(_New2, newList);
                case 3:
                    return Expression.New(_New3, newList);
                case 4:
                    return Expression.New(_New4, newList);
            }
            var a = Expression.NewArrayInit(typeof(JSValue), args);
            return Expression.New(_New, @this, a);
        }

        private readonly static FieldInfo _This =
            type.GetField(nameof(Arguments.This));
        public static Expression This(Expression arguments)
        {
            return Expression.Field(arguments, _This);
        }
        private readonly static FieldInfo _Length =
            type.GetField(nameof(Arguments.Length));
        public static Expression Length(Expression arguments)
        {
            return Expression.Field(arguments, _Length);
        }

    }
}
