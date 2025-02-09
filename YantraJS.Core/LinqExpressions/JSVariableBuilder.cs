using System;
using System.Linq;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.LambdaGen;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.ExpHelper
{
    public class JSVariableBuilder
    {
        //static readonly Type type = typeof(JSVariable);

        //static readonly ConstructorInfo _New
        //    = type.Constructor(typeof(JSValue), typeof(string));


        public static Expression New(Expression value, string name)
        {
            return NewLambdaExpression.NewExpression<JSVariable>(() => () => new JSVariable(null as JSValue, "")
                , value
                , Expression.Constant(name));
            // return Expression.New(_New, value, Expression.Constant(name));
        }

        //static readonly ConstructorInfo _NewFromException
        //    = type.Constructor(typeof(Exception), typeof(string));

        public static Expression NewFromException(Expression value, string name)
        {
            // return Expression.New(_NewFromException, value, Expression.Constant(name));
            return NewLambdaExpression.NewExpression<JSVariable>(() => () => new JSVariable(null as Exception, "")
            , value
            , Expression.Constant(name));
        }

        //static readonly ConstructorInfo _NewFromArgument
        //    = type.Constructor(ArgumentsBuilder.refType, typeof(int), typeof(string));

        public static Expression FromArgument(Expression args, int i, string name)
        {
            return NewLambdaExpression.NewExpression<JSVariable>(() => () => new JSVariable(Arguments.Empty, 0, "")
            , args
            , Expression.Constant(i)
            , Expression.Constant(name));
            // return Expression.New(_NewFromArgument, args, Expression.Constant(i), Expression.Constant(name));
        }

        public static Expression FromArgumentOptional(Expression args, int i, Expression optional)
        {
            // check if is undefined...
            if (optional == null)
                return ArgumentsBuilder.GetAt(args, i);
            var argAt = ArgumentsBuilder.GetAt(args, i);
            return Expression.Coalesce(JSValueExtensionsBuilder.NullIfUndefined(argAt), optional);
        }

        public static Expression New(string name)
        {
            return NewLambdaExpression.NewExpression<JSVariable>(() => () => new JSVariable(null as JSValue, "")
                , JSUndefinedBuilder.Value
                , Expression.Constant(name));
            // return Expression.New(_New, ExpHelper.JSUndefinedBuilder.Value, Expression.Constant(name));
        }

        public static Expression Property(Expression target)
        {
            return target.PropertyExpression<JSVariable, JSValue>(() => (x) => x.GlobalValue);
            //return Expression.Property(target, _GlobalValue);
        }
        
    }
}
