using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    internal static class Bootstrap
    {

        private static ConcurrentUInt32Trie<JSFunction> cache = new ConcurrentUInt32Trie<JSFunction>();

        private static ConcurrentStringTrie<PropertySequence> propertyCache
            = new ConcurrentStringTrie<PropertySequence>(64);

        public static void Fill<T>(this JSContext context)
        {
            var type = typeof(T);
            var key = type.FullName;
            var cached = propertyCache.GetOrCreate(key, () =>
            {
                var ps = new JSObject();
                Fill(type, ps);
                return ps.ownProperties;
            });

            foreach (var pk in cached.AllValues())
            {
                context.ownProperties[pk.Key] = pk.Value;
            }
        }

        public static JSObject Create<T>(
            this JSContext context, 
            KeyString key, 
            JSObject chain = null)
        {
            var jsf = cache.GetOrCreate(key.Key, () =>
            {
                var type = typeof(T);
                JSFunction r = Create(key, type);

                var rt = type.GetCustomAttribute<JSRuntimeAttribute>();
                if (rt != null)
                {


                    var cx = Fill(rt.StaticType, r);
                    if (cx != null && r.f == JSFunction.empty)
                    {
                        r.f = cx;
                    }

                    cx = Fill(rt.Prototype, r.prototype);
                    if (cx != null && r.f == JSFunction.empty)
                    {
                        r.f = cx;
                    }
                }

                return r;
            });

            var copy = new JSFunction(jsf.f, key.ToString());
            var target = copy.prototype.ownProperties;
            foreach (var p in jsf.prototype.ownProperties.AllValues())
            {
                target[p.Key] = p.Value;
            }
            var ro = copy.ownProperties;
            foreach (var p in jsf.ownProperties.AllValues())
            {
                /// this is the case when we do not
                /// want to overwrite Function.prototype
                if (p.Key != KeyStrings.prototype.Key)
                {
                    ro[p.Key] = p.Value;
                }
            }
            context.ownProperties[key.Key] = JSProperty.Property(copy, JSPropertyAttributes.ConfigurableReadonlyValue);
            copy.prototypeChain = chain ?? context.ObjectPrototype;
            return copy.prototype;
        }

        #region Fill

        public static JSFunctionDelegate Fill(Type type, JSObject target)
        {

            JSFunctionDelegate r = null;

            var ownProperties = target.ownProperties ?? (target.ownProperties = new PropertySequence());

            var p = target;
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

                if (ownProperties.TryGetValue(f.attribute.Name.Key, out var _))
                    continue;

                var (m, pr) = f;

                if (pr is ConstructorAttribute)
                {
                    r = (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate));
                    continue;
                }

                if (pr.IsMethod)
                {

                    ownProperties[pr.Name.Key] = JSProperty.Function(pr.Name,
                        (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate)), pr.ConfigurableValue);
                    continue;
                }

                if (mg.Count() == 2)
                {
                    var l = mg.Last();
                    var fdel = (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate));
                    var ldel = (JSFunctionDelegate)l.method.CreateDelegate(typeof(JSFunctionDelegate));
                    ownProperties[pr.Name.Key] = JSProperty.Property(
                        mg.Key,
                        f.attribute.IsGetProperty ? fdel : ldel,
                        !f.attribute.IsGetProperty ? fdel : ldel,
                        f.attribute.ConfigurableProperty
                        );
                    continue;
                }

                var fx = (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate));
                ownProperties[pr.Name.Key] = JSProperty.Property(
                    mg.Key,
                    f.attribute.IsGetProperty ? fx : null,
                    !f.attribute.IsGetProperty ? fx : null,
                    f.attribute.ConfigurableProperty
                    );


            }

            var fields = type
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Select(x => (method: x, attribute: x.GetCustomAttribute<PrototypeAttribute>()))
                .Where(x => x.attribute != null);
            foreach (var (f, pr) in fields)
            {
                var v = f.GetValue(null);
                JSValue jv = v as JSValue;
                if (jv == null)
                {
                    if (f.FieldType == typeof(double))
                    {
                        jv = new JSNumber((double)v);
                    }
                    else
                    {
                        jv = new JSString(v.ToString());
                    }
                }
                if (ownProperties.TryGetValue(pr.Name.Key, out var _))
                    continue;
                ownProperties[pr.Name.Key] = 
                    JSProperty.Property(pr.Name, jv, JSPropertyAttributes.ConfigurableReadonlyValue);
            }

            return r;
        }
        #endregion



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
                JSValue jv = v as JSValue;
                if (jv == null)
                {
                    if (f.FieldType == typeof(double))
                    {
                        jv = new JSNumber((double)v);
                    }
                    else
                    {
                        jv = new JSString(v.ToString());
                    }
                }

                target.DefineProperty(pr.Name, JSProperty.Property(pr.Name, jv, JSPropertyAttributes.ConfigurableReadonlyValue));
            }

            return r;
        }

    }
}
