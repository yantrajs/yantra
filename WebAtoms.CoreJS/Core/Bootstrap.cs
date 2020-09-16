using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    internal class Bootstrap
    {

        public static JSFunction Create(KeyString key, Type type)
        {
            JSFunction r = new JSFunction(JSFunction.empty, key.ToString());

            var p = r.prototype;
            var all = type
                .GetMethods(BindingFlags.NonPublic 
                    | BindingFlags.DeclaredOnly 
                    | BindingFlags.Public
                    | BindingFlags.Static
                    | BindingFlags.Instance)
                .Select(x => (method: x, attribute: x.GetCustomAttribute<PrototypeAttribute>()))
                .Where(x => x.attribute != null)
                .GroupBy(x => x.attribute.Name).ToList();
            foreach (var mg in all)
            {
                
                var f = mg.First();

                if (mg.Any((x => !x.method.IsStatic)))
                    throw new NotSupportedException($"{f.method.Name} should be static method");

                var (m, pr) = f;

                if (pr is ConstructorAttribute)
                {
                    r.f = (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate));
                    continue;
                }

                var target = pr.IsStatic ? r : p;
                if (pr.IsMethod)
                {

                    target.DefineProperty(pr.Name, JSProperty.Function(pr.Name,
                        (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate)), pr.ConfigurableValue));
                    continue;
                }
                
                if(mg.Count () == 2)
                {
                    var l = mg.Last();
                    var fdel = (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate));
                    var ldel = (JSFunctionDelegate)l.method.CreateDelegate(typeof(JSFunctionDelegate));
                    target = pr.IsStatic ? r : p;
                    target.DefineProperty(mg.Key, JSProperty.Property(
                        mg.Key,
                        f.attribute.IsGetProperty ? fdel : ldel,
                        !f.attribute.IsGetProperty ? fdel : ldel,
                        f.attribute.ConfigurableProperty
                        ));
                    continue;
                }

                var fx = (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate));
                target = pr.IsStatic ? r : p;
                target.DefineProperty(mg.Key, JSProperty.Property(
                    mg.Key,
                    f.attribute.IsGetProperty ? fx : null,
                    !f.attribute.IsGetProperty ? fx : null,
                    f.attribute.ConfigurableProperty
                    ));


            }

            var fields = type
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Select(x => (method: x, attribute: x.GetCustomAttribute<PrototypeAttribute>()))
                .Where(x => x.attribute != null);
            foreach(var (f,pr) in fields)
            {
                var target = pr.IsStatic ? r : p;
                var v = f.GetValue(null);
                JSValue jv;
                if (f.FieldType == typeof(double))
                {
                    jv = new JSNumber((double)v);
                } else
                {
                    jv = new JSString(v.ToString(), type == typeof(JSString) ? p : JSContext.Current.StringPrototype);
                }

                target.DefineProperty(pr.Name, JSProperty.Property(pr.Name, jv, JSPropertyAttributes.ConfigurableProperty));
            }

            return r;
        }

    }
}
