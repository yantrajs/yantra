using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Core;
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


        public YExpression Relay(YExpression @this, IFastEnumerable<YExpression> closures, YLambdaExpression innerLambda)
        {

            var (method, il, exp) = innerLambda.CompileToBoundDynamicMethod(methodBuilder: this);

            var repository = YExpression.Field(@this, Closures.repositoryField);

            var id = methods.RegisterNew(method, il, exp, innerLambda.Type);
            return YExpression.Call(
                repository, 
                create,
                closures == null ? YExpression.Null : (YExpression)YExpression.NewArray(typeof(Box), closures),
                YExpression.Constant(id));
        }
    }
}
