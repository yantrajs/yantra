using System;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Expressions;

namespace YantraJS
{
    public abstract class BoxHelper
    {
        public static BoxHelper For(Type type)
        {
            return Activator.CreateInstance(typeof(BoxHelper<>).MakeGenericType(type)) as BoxHelper;
        }

        public abstract Type BoxType { get; }

        // public abstract YExpression New();
        public abstract YExpression New(YExpression value);

        public abstract ConstructorInfo Constructor { get; }

    }

    public class BoxHelper<T>: BoxHelper
    {
        public static readonly  Type _BoxType = typeof(Box<T>);

        public override Type BoxType => _BoxType;

        public static readonly ConstructorInfo _new
            = _BoxType.GetConstructor(Array.Empty<Type>());

        private static ConstructorInfo _newFromValue
            = _BoxType.GetConstructor(new Type[] { typeof(T) });

        public override ConstructorInfo Constructor => _new;

        //public override YExpression New()
        //{
        //    return YExpression.New(_new);
        //}

        public override YExpression New(YExpression value)
        {
            return YExpression.New(_newFromValue, value);
        }

    }

    public abstract class Box
    {
    }

    public class Box<T> : Box
    {

        public Box()
        {

        }

        public Box(T value)
        {
            this.Value = value;
        }

        public T Value;
    }

}
