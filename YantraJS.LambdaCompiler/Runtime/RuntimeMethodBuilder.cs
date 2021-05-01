using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Expressions;
using YantraJS.Runtime;

namespace YantraJS
{

    public class RuntimeMethodBuilder : IMethodBuilder
    {
        private readonly IMethodRepository methods;

        public RuntimeMethodBuilder(IMethodRepository methods)
        {
            this.methods = methods;
        }

        private static Type type = typeof(IMethodRepository);

        private static MethodInfo create
            = type.GetMethod(nameof(IMethodRepository.Create));


        public YExpression Relay(YExpression[] closures, YLambdaExpression innerLambda)
        {
            if (innerLambda.Repository == null)
                throw new NotSupportedException($"Compile with Method Repository");
            var (method, il, exp) = innerLambda.CompileToBoundDynamicMethod();
            //List<Type> types = new List<Type>();
            //types.AddRange(innerLambda.ParameterTypes);
            //types.Add(method.ReturnType);
            //types.RemoveAt(0);
            //var dt = System.Linq.Expressions.Expression.GetDelegateType(types.ToArray());

            var id = methods.RegisterNew(method, il, exp, innerLambda.Type);
            return YExpression.Call(
                innerLambda.Repository, 
                create , 
                YExpression.NewArray(typeof(Box), closures),
                YExpression.Constant(id));
        }
    }
}
