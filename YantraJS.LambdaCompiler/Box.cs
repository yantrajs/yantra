using System;
using System.Linq.Expressions;
using System.Reflection;

namespace YantraJS
{
    public abstract class BoxHelper
    {
        public static BoxHelper For(Type type)
        {
            return Activator.CreateInstance(typeof(BoxHelper<>).MakeGenericType(type)) as BoxHelper;
        }

        public abstract Expression New();
        public abstract Expression New(Expression value);

    }

    public class BoxHelper<T>: BoxHelper
    {
        private static Type boxType = typeof(Box<T>);

        private static ConstructorInfo _new
            = boxType.GetConstructor(Array.Empty<Type>());

        private static ConstructorInfo _newFromValue
            = boxType.GetConstructor(new Type[] { typeof(T) });

        public override Expression New()
        {
            return Expression.New(_new);
        }

        public override Expression New(Expression value)
        {
            return Expression.New(_newFromValue, value);
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
