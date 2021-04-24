using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS.LambdaCompiler.Tests
{
    [TestClass]
    public class UnitTest1: BaseTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            
        }


        public void Compile(YLambdaExpression exp)
        {
            AssemblyName name = new AssemblyName("demo");

            // lets generate and save...
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);
            // string fileName = System.IO.Path.GetFileNameWithoutExtension(location);
            // name.CodeBase = filePath;
            var mm = ab.DefineDynamicModule("JSModule");

            var type = mm.DefineType("JSCodeClass",
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);

            type.DefineDefaultConstructor(MethodAttributes.Public);

            var method = type.DefineMethod("Run", MethodAttributes.Public | MethodAttributes.Static,
                typeof(string),
                new Type[] { typeof(string) });

            // Expression<Func<string,string>> y = x => this.Simple<string>(() => x == null ? x : null);

            ExpressionCompiler.CompileToMethod(exp, method);


        }

    }
}
