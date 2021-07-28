using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using YantraJS.Core;
using YantraJS.Emit;
using YantraJS.Expressions;

namespace YantraJS
{
    public class AssemblyCodeCache : ICodeCache
    {
        private DirectoryInfo cacheFolder = new DirectoryInfo(".\\cache");

        private SHA256 sha = SHA256.Create();

        public JSFunctionDelegate GetOrCreate(in JSCode code)
        {

            var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(code.Code.Value));
            string name = string.Join("", hash.Select(x => x.ToString("X2")));
            var i = 0;
            while (true)
            {
                i++;
                var fileName = $"{cacheFolder.FullName}\\JS{name}_{i:X2}";
                var js = $"{fileName}.js";
                var dll = $"{fileName}.dll";

                if(!(File.Exists(dll) && File.Exists(js)))
                {
                    return Create(in code, js, dll);
                }

                if(System.IO.File.ReadAllText(js) == code.Code.Value)
                {
                    throw new NotSupportedException();
                }
            }
        }

        private JSFunctionDelegate Create(in JSCode code, string js, string dll)
        {
            var expression = code.Compiler();

            var outerLambda = YExpression
                .InstanceLambda<JSFunctionDelegate>(
                    expression.Name + "_outer",
                    expression,
                    YExpression.Parameter(typeof(Closures))
                    , new YParameterExpression[] { })
                as YLambdaExpression;

            var asmName = System.IO.Path.GetFileNameWithoutExtension(dll);

            var asm = AssemblyBuilder.DefineDynamicAssembly(new System.Reflection.AssemblyName(asmName), AssemblyBuilderAccess.RunAndCollect);

            var mod = asm.DefineDynamicModule(asmName);

            var type = mod.DefineType("JSScript", System.Reflection.TypeAttributes.Public, typeof(Closures));

            var r = outerLambda.CompileToInstnaceMethod(type);

            var m = type.CreateType().GetMethod(r.method.Name);

            //  lets save...
            var g = new Lokad.ILPack.AssemblyGenerator();
            var dir = System.IO.Path.GetDirectoryName(dll);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            g.GenerateAssembly(m.DeclaringType.Assembly, dll);

            return (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate));
        }
    }
}
