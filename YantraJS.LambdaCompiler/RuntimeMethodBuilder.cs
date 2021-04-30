using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Expressions;
using YantraJS.Runtime;

namespace YantraJS
{
    public class Closures
    {
        internal static FieldInfo boxesField = typeof(Closures).GetField("Boxes");
        internal static FieldInfo delegateField = typeof(Closures).GetField("Delegate");

        public readonly Box[] Boxes;
        public readonly string IL;
        public readonly string Exp;
        
        public Closures(
            Box[] boxes, 
            string il,
            string exp)
        {
            this.Boxes = boxes;
            this.IL = il;
            this.Exp = exp;
        }
    }

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

        

        //public YExpression Create(string name, YLambdaExpression lambdaExpression)
        //{
        //    if (lambdaExpression.Repository == null)
        //        throw new NotSupportedException($"Compile with Method Repository");
        //    var (method, il, exp) = ExpressionCompiler.Compile(lambdaExpression, methods);

        //    List<Type> types = new List<Type>();
        //    types.AddRange(method.GetParameters().Select(p => p.ParameterType));
        //    types.Add(method.ReturnType);
        //    var dt = System.Linq.Expressions.Expression.GetDelegateType(types.ToArray());
        //    var d = method.CreateDelegate(dt);
        //    var id = methods.RegisterNew(d);
        //    return YExpression.Call(lambdaExpression.Repository, run, YExpression.Constant(id));
        //    // return YExpression.Call(lambdaExpression.Repository, run, YExpression.Delegate(method, dt));
        //}

        public YExpression Relay(YExpression[] closures, YLambdaExpression innerLambda)
        {
            if (innerLambda.Repository == null)
                throw new NotSupportedException($"Compile with Method Repository");
            var (method, il, exp) = innerLambda.CompileToBoundDynamicMethod();
            List<Type> types = new List<Type>();
            types.AddRange(innerLambda.ParameterTypes);
            types.Add(method.ReturnType);
            types.RemoveAt(0);
            var dt = System.Linq.Expressions.Expression.GetDelegateType(types.ToArray());

            var id = methods.RegisterNew(method, il, exp, dt);
            return YExpression.Call(
                innerLambda.Repository, 
                create , 
                YExpression.NewArray(typeof(Box), closures),
                YExpression.Constant(id));
        }
    }
}
