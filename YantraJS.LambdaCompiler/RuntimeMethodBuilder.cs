using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Expressions;

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

        private static MethodInfo run
            = type.GetMethod(nameof(IMethodRepository.Run));

        public YExpression Create(string name, YLambdaExpression lambdaExpression)
        {
            if (lambdaExpression.Repository == null)
                throw new NotSupportedException($"Compile with Method Repository");
            var (method, il, exp) = ExpressionCompiler.Compile(lambdaExpression, methods);

            List<Type> types = new List<Type>();
            types.AddRange(method.GetParameters().Select(p => p.ParameterType));
            types.Add(method.ReturnType);

            var d = method.CreateDelegate(System.Linq.Expressions.Expression.GetDelegateType(types.ToArray()));
            var id = methods.RegisterNew(d);
            return YExpression.Call(lambdaExpression.Repository, run, YExpression.Constant(id));
        }
    }
}
