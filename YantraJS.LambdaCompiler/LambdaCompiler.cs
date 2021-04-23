using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Generator;

namespace YantraJS
{
    public class LambdaCompiler
    {

        private static int id = 1;

        internal static string GetUniqueName(string name)
        {
            return $"<YantraJSHidden>{name}<ID>{System.Threading.Interlocked.Increment(ref id)}";
        }


        internal static void InternalCompileToMethod(
            LambdaExpression exp,
            MethodBuilder builder)
        {

            NestedRewriter nw = new NestedRewriter(exp, new LambdaMethodBuilder(builder));
            exp = nw.Visit(exp) as LambdaExpression;

            ILCodeGenerator icg = new ILCodeGenerator(builder.GetILGenerator());
            icg.Emit(exp);
        }


        public static void CompileToMethod(
            LambdaExpression lambdaExpression,
            MethodBuilder methodBuilder)
        {
            var exp = LambdaRewriter.Rewrite(lambdaExpression, new LambdaMethodBuilder(methodBuilder))
                as LambdaExpression;

            InternalCompileToMethod(exp, methodBuilder);



        }

    }
}
