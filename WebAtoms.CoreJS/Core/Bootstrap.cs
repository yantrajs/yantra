using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.ExpHelper;

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

        public static JSFunction Create<T>(
            this JSContext context, 
            KeyString key, 
            JSObject chain = null, bool addToContext = true)
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
            if (addToContext)
            {
                context.ownProperties[key.Key] = JSProperty.Property(copy, JSPropertyAttributes.ConfigurableReadonlyValue);
            }
            copy.prototypeChain = chain ?? context.ObjectPrototype;
            return copy;
        }

        #region Fill

        private static (JSFunctionDelegate getter, JSFunctionDelegate setter) 
            CreateProperty(this (PropertyInfo method, PrototypeAttribute attribute) m)
        {
            
            var (property, p) = m;
            if (property.GetAccessors().FirstOrDefault().IsStatic)
            {
                throw new NotSupportedException();
            }

            var name = property.DeclaringType.Name;
            if (name.StartsWith("JS"))
            {
                name = name.Substring(2);
            }

            var peList = new List<ParameterExpression>();
            ParameterExpression targetExp = null;
            var toType = m.method.DeclaringType;
            targetExp = Expression.Parameter(typeof(JSValue));
            // var target = JSVariable.ValueExpression(targetExp);
            var target = targetExp;
            // this is a set method...
            peList.Add(targetExp);
            var rType = property.PropertyType;

            // wrap...
            var pe = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var peThis = ArgumentsBuilder.This(pe);
            var arg1 = ArgumentsBuilder.Get1(pe);
            var coalesce = Expression.Coalesce(
                Expression.TypeAs(peThis, toType),
                Expression.Throw(
                    JSExceptionBuilder.New($"{name}.prototype.{p.Name} called with object not of type {name}"), toType));

            JSFunctionDelegate getter = null;
            JSFunctionDelegate setter = null;

            if (property.CanRead)
            {
                var getterBody = Expression.Property(coalesce, property);
                var getterLambda = Expression.Lambda<JSFunctionDelegate>(getterBody, pe);
                getter = getterLambda.Compile();
            }
            if (property.CanWrite)
            {
                var setterBody = Expression.Assign(
                    Expression.Property(coalesce, property),
                    JSValueBuilder.ForceConvert(arg1, rType));
                var setterLambda = Expression.Lambda<JSFunctionDelegate>(Expression.Block(peList,
                    setterBody), pe);
                setter = setterLambda.Compile();
            }
            setter = (in Arguments a) =>
            {
                var f = a.Get1();
                var tx = f.ForceConvert(property.PropertyType);
                property.SetValue(a.This, tx);
                return f;
            };
            return (getter, setter);
        }


        private static JSFunctionDelegate CreateJSFunctionDelegate(this (MethodInfo method, PrototypeAttribute attribute) m)
        {
            var (method, p) = m;
            if (method.IsStatic)
                return (JSFunctionDelegate)method.CreateDelegate(typeof(JSFunctionDelegate));

            var name = method.DeclaringType.Name;
            if(name.StartsWith("JS"))
            {
                name = name.Substring(2);
            }

            var peList = new List<ParameterExpression>();
            ParameterExpression targetExp = null;
            var toType = m.method.DeclaringType;
            var paramList = m.method.GetParameters();
            if (paramList?.Length > 0)
            {
                targetExp = Expression.Parameter(typeof(JSValue));
                // this is a set method...
                peList.Add(targetExp);
            }
            var rType = m.method.GetParameters()?.FirstOrDefault()?.ParameterType;

            // wrap...
            var pe = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var peThis = ArgumentsBuilder.This(pe);
            var arg1 = ArgumentsBuilder.Get1(pe);
            var coalesce = Expression.Coalesce(
                Expression.TypeAs(peThis, toType), 
                Expression.Throw(
                    JSExceptionBuilder.New($"{name}.prototype.{p.Name} called with object not of type {name}"), toType));
            var body = Expression.Block( peList, targetExp == null
                ? Expression.Call(coalesce, method)
                : Expression.Call(coalesce, method, JSValueBuilder.Coalesce(arg1, rType, targetExp, p.Name.ToString())),
                peThis);
            var lambda = Expression.Lambda<JSFunctionDelegate>(body, pe);
            return lambda.Compile();
        }

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

                //if (mg.Any((x => !x.method.IsStatic)))
                //    throw new NotSupportedException($"{f.method.Name} should be static method");

                if (ownProperties.TryGetValue(f.attribute.Name.Key, out var _))
                    continue;

                var (m, pr) = f;

                if (pr is ConstructorAttribute)
                {
                    r = f.CreateJSFunctionDelegate();
                    continue;
                }

                if (pr.IsMethod)
                {
                    ownProperties[pr.Name.Key] = JSProperty.Function(pr.Name,
                        f.CreateJSFunctionDelegate(), pr.ConfigurableValue, pr.Length);
                    continue;
                }

                if (mg.Count() == 2)
                {
                    var l = mg.Last();
                    var fdel = f.CreateJSFunctionDelegate();
                    var ldel = l.CreateJSFunctionDelegate();
                    ownProperties[pr.Name.Key] = JSProperty.Property(
                        mg.Key,
                        f.attribute.IsGetProperty ? fdel : ldel,
                        !f.attribute.IsGetProperty ? fdel : ldel,
                        f.attribute.ConfigurableProperty
                        );
                    continue;
                }

                var fx = f.CreateJSFunctionDelegate();
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

        public static JSObject CreateSharedObject(
            this JSContext context, 
            KeyString key, 
            Type type, 
            bool addToContext)
        {
            var c = cache.GetOrCreate(key.Key, () => {
                return Create(key, type);
            });
            if(addToContext)
            {
                context[key] = c;
            }
            return c;
        }

        public static JSFunction Create(in KeyString key, Type type)
        {
            JSFunction r = typeof(JSFunction).IsAssignableFrom(type) && type != typeof(JSFunction)
                ? (JSFunction)Activator.CreateInstance(type, key.ToString())
                : new JSFunction(JSFunction.empty, key.ToString());

            var p = r.prototype;

            // Properties can only be defined on the type...
            var properties = type
                .GetProperties(BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Select(x => (property: x, attribute: x.GetCustomAttribute<PrototypeAttribute>()))
                .Where(x => x.attribute != null && x.property.DeclaringType == type)
                .ToList();

            foreach(var property in properties)
            {
                if (
                    (property.property.CanRead && property.property.GetMethod.IsStatic) ||
                    (property.property.CanWrite && property.property.SetMethod.IsStatic)) {
                    // this is static property...
                    throw new NotImplementedException();
                } else
                {
                    var a = property.attribute;
                    var name = a.Name;
                    var (getter, setter) = (property.property, a).CreateProperty();
                    p.DefineProperty(name, JSProperty.Property(name, getter, setter, a.ConfigurableProperty));
                }
            }

            var all = type
                .GetMethods(BindingFlags.NonPublic 
                    | BindingFlags.DeclaredOnly 
                    | BindingFlags.Public
                    | BindingFlags.Static
                    | BindingFlags.Instance)
                .Select(x => (method: x, attribute: x.GetCustomAttribute<PrototypeAttribute>()))
                .Where(x => x.attribute != null && x.method.DeclaringType == type)
                .GroupBy(x => x.attribute.Name).ToList();
            foreach (var mg in all)
            {
                
                var f = mg.First();

                //if (mg.Any((x => !x.method.IsStatic)))
                //    throw new NotSupportedException($"{f.method.Name} should be static method");

                var (m, pr) = f;

                if (pr is ConstructorAttribute)
                {
                    r.f = f.CreateJSFunctionDelegate();
                    continue;
                }

                var target = pr.IsStatic ? r : p;
                if (pr.IsMethod)
                {

                    target.DefineProperty(pr.Name, JSProperty.Function(pr.Name,
                        f.CreateJSFunctionDelegate(), pr.ConfigurableValue, pr.Length));
                    continue;
                }
                
                if(mg.Count () == 2)
                {
                    var l = mg.Last();
                    var fdel = f.CreateJSFunctionDelegate();
                    var ldel = l.CreateJSFunctionDelegate();
                    target = pr.IsStatic ? r : p;
                    target.DefineProperty(mg.Key, JSProperty.Property(
                        mg.Key,
                        f.attribute.IsGetProperty ? fdel : ldel,
                        !f.attribute.IsGetProperty ? fdel : ldel,
                        f.attribute.ConfigurableProperty
                        ));
                    continue;
                }

                var fx = f.CreateJSFunctionDelegate();
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
