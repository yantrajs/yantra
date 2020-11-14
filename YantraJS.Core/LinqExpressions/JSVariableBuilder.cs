using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public class JSVariableBuilder
    {
        static readonly Type type = typeof(JSVariable);

        static readonly ConstructorInfo _New
            = type.Constructor(typeof(JSValue), typeof(string));

        public static Expression New(Expression value, string name)
        {
            return Expression.New(_New, value, Expression.Constant(name, typeof(string)));
        }

        static readonly ConstructorInfo _NewFromException
            = type.Constructor(typeof(Exception), typeof(string));

        public static Expression NewFromException(Expression value, string name)
        {
            return Expression.New(_NewFromException, value, Expression.Constant(name, typeof(string)));
        }

        static readonly MethodInfo _NewFromArgument
            = type.InternalMethod(nameof(JSVariable.New), typeof(Arguments).MakeByRefType(), typeof(int), typeof(string));

        static readonly PropertyInfo _GlobalValue
            = type.Property(nameof(JSVariable.GlobalValue));

        public static Expression FromArgument(Expression args, int i, string name)
        {
            return Expression.Call(null, _NewFromArgument, args, Expression.Constant(i), Expression.Constant(name));
        }

        public static Expression FromArgumentOptional(Expression args, int i, Expression optional)
        {
            // check if is undefined...
            var argAt = ArgumentsBuilder.GetAt(args, i);
            return Expression.Coalesce(JSValueExtensionsBuilder.NullIfUndefined(argAt), optional);
        }

        public static Expression New(string name)
        {
            return Expression.New(_New, ExpHelper.JSUndefinedBuilder.Value, Expression.Constant(name));
        }

        public static Expression Property(Expression target)
        {
            return Expression.Property(target, _GlobalValue);
        }
        
    }
}
