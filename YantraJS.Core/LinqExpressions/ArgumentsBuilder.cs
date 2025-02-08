using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.FastParser;
using YantraJS.Core.LambdaGen;
using YantraJS.Core.LinqExpressions;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.ExpHelper
{
    public class ArgumentsBuilder
    {
        private static readonly Type type = typeof(Arguments);

        internal static readonly Type refType = type.MakeByRefType();

        private readonly static Expression _Empty =
            Expression.Field(null, type.GetField(nameof(Arguments.Empty)));

        private readonly static MethodInfo _Get1 =
            type.InternalMethod(nameof(Arguments.Get1));

        private readonly static MethodInfo _GetAt =
            type.InternalMethod(nameof(Arguments.GetAt), typeof(int));

        //private static readonly FieldInfo _newTarget
        //    = type.GetField(nameof(Arguments.NewTarget));

        private readonly static MethodInfo _RestFrom =
            type.InternalMethod(nameof(Arguments.RestFrom), typeof(uint));

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

        private readonly static MethodInfo _spread
            = type.PublicMethod(nameof(Arguments.Spread), typeof(JSValue), typeof(JSValue[]));

        //private readonly static MethodInfo _GetElementEnumerator
        //    = type.PublicMethod(nameof(Arguments.GetElementEnumerator));

        public static Expression New(Expression @this, Expression arg0)
        {
            return Expression.New(_New1, @this, arg0);
        }

        public static Expression NewEmpty(Expression @this)
        {
            return Expression.New(_New0, @this);
        }


        public static Expression New(Expression @this, Expression arg0, Expression arg2)
        {
            return Expression.New(_New1, @this, arg0, arg2);
        }

        public static Expression Spread(Expression @this, IFastEnumerable<Expression> args)
        {
            return Expression.Call(null, _spread, @this, Expression.NewArrayInit(typeof(JSValue),args));
        }

        public static Expression New(Expression @this, IFastEnumerable<Expression> args, bool spread)
        {
            if (spread)
            {
                return Expression.Call(null, _spread, @this, Expression.NewArrayInit(typeof(JSValue), args));
            }
            var newList = new Sequence<Expression>() { @this };
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

        public static Expression New(Expression @this, Expression[] args, bool spread)
        {
            if (spread)
            {
                return Expression.Call(null, _spread, @this, Expression.NewArrayInit(typeof(JSValue), args));
            }
            var newList = new Sequence<Expression>() { @this };
            newList.AddRange(args);
            switch (args.Length)
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

        public static Expression New(Expression @this, IFastEnumerable<Expression> args)
        {
            var newList = new Sequence<Expression>() { @this };
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

        public static Expression New(Expression @this, IList<Expression> args)
        {
            var newList = new Sequence<Expression>() { @this };
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

        //public static Expression NewTarget(Expression exp)
        //{
        //    // return Expression.Field(exp, _newTarget);
        //    return JSContextBuilder.NewTarget();
        //}

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


        public static Expression Get1(Expression arguments)
        {
            return Expression.Call(arguments, _Get1);
        }

        public static Expression GetAt(Expression arguments, int index)
        {
            return Expression.Call(arguments, _GetAt, Expression.Constant(index));
        }

        public static Expression RestFrom(Expression arguments, uint index)
        {
            return Expression.Call(arguments, _RestFrom, Expression.Constant(index));
        }

        public static Expression GetElementEnumerator(Expression target)
        {
            return target.CallExpression<Arguments, IElementEnumerator>(() => (x) => x.GetElementEnumerator());
            // return Expression.Call(target, _GetElementEnumerator);
        }

    }
}
