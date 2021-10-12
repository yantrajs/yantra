using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public AssemblyCodeCache(string path = ".\\cache")
        {
            cacheFolder = new DirectoryInfo(path);
            this.version = this.GetType().Assembly.GetName().Version?.ToString();
        }

        private readonly DirectoryInfo cacheFolder;
        private readonly string version;
        private SHA256 sha = SHA256.Create();

        static AssemblyCodeCache()
        {
            
        }

        public JSFunctionDelegate GetOrCreate(in JSCode code)
        {
            
            var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(version + "\r\n" + code.Code.Value));
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

                if (System.IO.File.ReadAllText(js) == code.Code.Value)
                {
                    var a = AppDomain.CurrentDomain.Load(System.IO.File.ReadAllBytes(dll));
                    // var a = Assembly.LoadFile(dll);
                    var t = a.GetType("JSScript");
                    var m = t.GetMethod("Run");

                    var fx = (JSFunctionDelegate) m.Invoke(null, null);
                    //var fxt = fx.GetType();
                    //var fxm = fxt.GetMethod("Invoke");
                    //System.Diagnostics.Debug.WriteLine(fxm.Name);
                    return fx;
                }
            }
        }

        private JSFunctionDelegate Create(in JSCode code, string js, string dll)
        {
            var expression = code.Compiler();

            //var outerLambda = YExpression
            //    .InstanceLambda<JSFunctionDelegate>(
            //        expression.Name + "_outer",
            //        expression,
            //        YExpression.Parameter(typeof(Closures))
            //        , new YParameterExpression[] { })
            //    as YLambdaExpression;

            var asmName = System.IO.Path.GetFileNameWithoutExtension(dll);

            var asm = AssemblyBuilder.DefineDynamicAssembly(new System.Reflection.AssemblyName(asmName), AssemblyBuilderAccess.RunAndCollect);

            var mod = asm.DefineDynamicModule(asmName);

            var type = mod.DefineType("JSScript", System.Reflection.TypeAttributes.Public);

            // type.DefineDefaultConstructor(System.Reflection.MethodAttributes.Public);

            var sm = type.DefineMethod("Run",
                MethodAttributes.Public | MethodAttributes.Static,
                typeof(JSFunctionDelegate),
                new Type[] { });

            var r = expression.CompileToStaticMethod(type, sm, true);

            var m = type.CreateType().GetMethod(r.Name);

            //  lets save...
            var g = new Lokad.ILPack.AssemblyGenerator();
            var dir = System.IO.Path.GetDirectoryName(dll);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            //var list = typeof(JSContext).Assembly.GetReferencedAssemblies()
            //    .Select(x =>
            //    {
            //        if(x.Name == "netstandard")
            //        {
            //            var a1 = typeof(System.String).Assembly;
            //            return a1;
            //        }
            //        var a = Assembly.Load(x);
            //        return a;
            //    })
            //    .ToList();

            //list.Add(typeof(JSContext).Assembly);
            

            g.GenerateAssembly(m.DeclaringType.Assembly,
                dll);

            System.IO.File.WriteAllText(js, code.Code.Value);

            return (JSFunctionDelegate)m.Invoke(null, null);
        }
    }
}
