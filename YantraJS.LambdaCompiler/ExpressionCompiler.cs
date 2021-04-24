using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;
using YantraJS.Generator;

namespace YantraJS
{
    public class ExpressionCompiler
    {

        private static int id = 1;

        internal static string GetUniqueName(string name)
        {
            return $"<YantraJSHidden>{name}<ID>{System.Threading.Interlocked.Increment(ref id)}";
        }


        internal static void InternalCompileToMethod(
            YLambdaExpression exp,
            MethodBuilder builder)
        {

            NestedRewriter nw = new NestedRewriter(exp, new LambdaMethodBuilder(builder));
            exp = nw.Visit(exp) as YLambdaExpression;

            ILCodeGenerator icg = new ILCodeGenerator(builder.GetILGenerator());
            // icg.Emit(exp);
        }


        public static void CompileToMethod(
            YLambdaExpression lambdaExpression,
            MethodBuilder methodBuilder)
        {
            var exp = LambdaRewriter.Rewrite(lambdaExpression, new LambdaMethodBuilder(methodBuilder))
                as YLambdaExpression;

            InternalCompileToMethod(exp, methodBuilder);



        }

    }
}
