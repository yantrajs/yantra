using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace YantraJS.LambdaCompiler
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

            var fx = exp.Compile();

            var d = (Delegate)(object)fx;
            var type = typeof(System.Reflection.Emit.DynamicMethod);
            var rtd = type.GetNestedType("RTDynamicMethod", System.Reflection.BindingFlags.NonPublic);
            var field = rtd.GetTypeInfo().DeclaredFields.FirstOrDefault(x => x.Name == "m_owner");
            DynamicMethod dm = field?.GetValue(d.Method) as DynamicMethod;


            var il = dm.GetILGenerator();

            var data = typeof(ILGenerator).GetField("m_ILStream", 
                BindingFlags.NonPublic 
                | BindingFlags.FlattenHierarchy
                | BindingFlags.GetField
                | BindingFlags.Default).GetValue(il);

            var labels = typeof(ILGenerator).GetField("m_labelList", BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).GetValue(il);


            var ilDest = builder.GetILGenerator();

            Console.WriteLine("a");
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
