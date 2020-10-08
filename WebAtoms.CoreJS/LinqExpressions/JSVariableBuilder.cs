using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSVariableBuilder : TypeHelper<Core.JSVariable>
    {
        private static ConstructorInfo _New
            = Constructor<Core.JSValue, string>();

        public static Expression New(Expression value, string name)
        {
            return Expression.New(_New, value, Expression.Constant(name, typeof(string)));
        }

        private static ConstructorInfo _NewFromException
            = Constructor<Exception, string>();

        public static Expression NewFromException(Expression value, string name)
        {
            return Expression.New(_NewFromException, value, Expression.Constant(name, typeof(string)));
        }

        private static MethodInfo _NewFromArgument
            = InternalMethod(nameof(JSVariable.New));

        public static Expression FromArgument(Expression args, int i, string name)
        {
            return Expression.Call(null, _NewFromArgument, args, Expression.Constant(i), Expression.Constant(name));
        }


        public static Expression New(string name)
        {
            return Expression.New(_New, ExpHelper.JSUndefinedBuilder.Value, Expression.Constant(name));
        }

    }
}
