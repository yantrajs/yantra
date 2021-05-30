// using FastExpressionCompiler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;
using YantraJS.Emit;
using YantraJS.Runtime;

namespace YantraJS.Tests
{
    public class AssemblyCodeCache: ICodeCache
    {

        // public static ICodeCache Instance = DictionaryCodeCache.Current;

        public static ICodeCache Instance = new AssemblyCodeCache();

        private static ConcurrentDictionary<string, JSFunctionDelegate> cache = new ConcurrentDictionary<string, JSFunctionDelegate>();

        public JSFunctionDelegate GetOrCreate(in JSCode code)
        {
            var cc = code.Compiler();
            return cc.CompileWithNestedLambdas();
        }

        public void Save(string location, Expression<JSFunctionDelegate> expression)
        {
            if (System.IO.File.Exists(location))
            {

                var filePath = location + ".dll";

                // expression.CompileFastToIL()

                var fileName = System.IO.Path.GetFileName(filePath) + DateTime.UtcNow.Ticks.ToString();
                AssemblyName name = new AssemblyName(fileName);

                // lets generate and save...
                AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);
                // string fileName = System.IO.Path.GetFileNameWithoutExtension(location);
                name.CodeBase = filePath;
                var mm = ab.DefineDynamicModule("JSModule");

                var type = mm.DefineType("JSCodeClass",
                    TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);

                type.DefineDefaultConstructor(MethodAttributes.Public);

                var method = type.DefineMethod("Run", MethodAttributes.Public | MethodAttributes.Static,
                    typeof(JSValue),
                    new Type[] { typeof(Arguments).MakeByRefType() });

                

                // expression.CompileToMethod(method);
                var d = expression.Compile();
                var m = d.Method;
                var m1 = d.GetMethodInfo();
                // var data = m.GetMethodBody().GetILAsByteArray();


                

                // expression.CompileFastToIL(method.GetILGenerator());

                // var t = type.CreateType();

                //ab.Save(name.Name);

                //System.IO.File.Move(name.Name, filePath);
                var generator = new Lokad.ILPack.AssemblyGenerator();
                // generator.GenerateAssembly(d.GetMethodInfo().Module.Assembly, filePath);


                
            }
        }
    }
}
