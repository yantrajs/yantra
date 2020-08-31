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
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Select(x => (method: x, attribute: x.GetCustomAttribute<PrototypeAttribute>()))
                .Where(x => x.attribute != null)
                .GroupBy(x => x.attribute.Name).ToList();
            foreach (var mg in all)
            {
                
                var f = mg.First();
                var (m, pr) = f;
                if (pr.MemberType == MemberType.Method)
                {

                    if (pr is StaticAttribute s)
                    {
                        r.DefineProperty(s.Name, JSProperty.Function(pr.Name,
                            (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate))));
                        continue;
                    }
                    p.DefineProperty(pr.Name, JSProperty.Function(pr.Name,
                        (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate))));
                    continue;
                }
                
                if(mg.Count () == 2)
                {
                    var l = mg.Last();
                    var fdel = (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate));
                    var ldel = (JSFunctionDelegate)l.method.CreateDelegate(typeof(JSFunctionDelegate));
                    var target = (pr is StaticAttribute) ? r : p;
                    target.DefineProperty(mg.Key, JSProperty.Property(
                        mg.Key,
                        f.attribute.MemberType == MemberType.Get ? fdel : ldel,
                        f.attribute.MemberType != MemberType.Get ? fdel : ldel,
                        JSPropertyAttributes.ConfigurableProperty
                        ));
                    continue;
                }

            }

            var fields = type
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Select(x => (method: x, attribute: x.GetCustomAttribute<PrototypeAttribute>()))
                .Where(x => x.attribute != null);
            foreach(var (f,pr) in fields)
            {
                var target = (pr is StaticAttribute) ? r : p;
                var v = f.GetValue(null);
                JSValue jv;
                if (f.FieldType == typeof(double))
                {
                    jv = new JSNumber((double)v, type == typeof(JSNumber) ? p : JSContext.Current.NumberPrototype);
                } else
                {
                    jv = new JSString(v.ToString(), type == typeof(JSString) ? p : JSContext.Current.StringPrototype);
                }

                target.DefineProperty(pr.Name, JSProperty.Property(pr.Name, jv));
            }

            return r;
        }

    }
}
