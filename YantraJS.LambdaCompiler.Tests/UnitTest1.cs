using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using YantraJS.Core;

namespace YantraJS.LambdaCompiler.Tests
{
    [TestClass]
    public class UnitTest1: BaseTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var x = LambdaRewriter.Rewrite<string,string>(x => this.Simple<string>(() => x == null ? x : null));

            Assert.IsNotNull(x);

        }


        [TestMethod]
        public void Compile()
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

            Expression<Func<string,string>> y = x => this.Simple<string>(() => x == null ? x : null);

            YantraJS.LambdaCompiler.LambdaCompiler.CompileToMethod(y as LambdaExpression, method);


        }

    }
}
