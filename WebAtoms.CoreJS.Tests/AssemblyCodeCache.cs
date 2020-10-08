using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Emit;

namespace WebAtoms.CoreJS.Tests
{
    public class AssemblyCodeCache: ICodeCache
    {

        public static ICodeCache Instance = new AssemblyCodeCache();

        private static ConcurrentDictionary<string, JSFunctionDelegate> cache = new ConcurrentDictionary<string, JSFunctionDelegate>();

        public JSFunctionDelegate GetOrCreate(in JSCode code, JSCodeCompiler compiler)
        {
            var cc = code.Clone();
            return cache.GetOrAdd(code.Key, (_) => {
                return compiler(cc);
            });
        }

        public void Save(string location, Expression<JSFunctionDelegate> expression)
        {
            if (System.IO.File.Exists(location))
            {

                var filePath = location + ".dll";

                var fileName = System.IO.Path.GetFileName(filePath) + DateTime.UtcNow.Ticks.ToString();
                AssemblyName name = new AssemblyName(fileName);

                // lets generate and save...
                AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
                // string fileName = System.IO.Path.GetFileNameWithoutExtension(location);
                name.CodeBase = filePath;
                var mm = ab.DefineDynamicModule("JSModule", fileName);

                var type = mm.DefineType("JSCodeClass", 
                    TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);

                type.DefineDefaultConstructor(MethodAttributes.Public);

                var method = type.DefineMethod("Run", MethodAttributes.Public | MethodAttributes.Static, 
                    typeof(JSValue), 
                    new Type[] { typeof(Arguments).MakeByRefType() });

                expression.CompileToMethod(method);
                type.CreateType();

                ab.Save(name.Name);

                System.IO.File.Move(name.Name, filePath);
                
            }
        }
    }
}
