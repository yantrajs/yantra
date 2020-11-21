using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.Enumerators;
using YantraJS.ExpHelper;

namespace YantraJS.Core
{
    internal static class Bootstrap
    {

        static readonly ConcurrentUInt32Map<JSFunction> cache = ConcurrentUInt32Map<JSFunction>.Create();

        static readonly ConcurrentStringMap<PropertySequence> propertyCache
            = ConcurrentStringMap<PropertySequence>.Create();

        public static void Fill<T>(this JSContext context)
        {
            var type = typeof(T);
            var key = type.FullName;
            var cached = propertyCache.GetOrCreate(key, (_) =>
            {
                var ps = new JSObject();
                Fill(type, ps);
                return ps.GetOwnProperties(false);
            });

            ref var co = ref context.GetOwnProperties();

            foreach (var (Key, Value) in cached.AllValues())
            {
                co[Key] = Value;
            }
        }

        public static JSFunction Create<T>(
            this JSContext context, 
            KeyString key, 
            JSObject chain = null, bool addToContext = true)
        {
            var type = typeof(T);
            var rt = type.GetCustomAttribute<JSRuntimeAttribute>();
            var jsf = cache.GetOrCreate(key.Key, () =>
            {
                JSFunction r = Create(key, type);

                if (rt != null)
                {


                    var cx = Fill(rt.StaticType, r);
                    if (cx.function != null && r.f == JSFunction.empty)
                    {
                        r.f = cx.function;
                        r[KeyStrings.length] = new JSNumber(cx.length);
                        
                    }

                    cx = Fill(rt.Prototype, r.prototype);
                    if (cx.function != null && r.f == JSFunction.empty)
                    {
                        r.f = cx.function;
                        r[KeyStrings.length] = new JSNumber(cx.length);
                    }
                }

                return r;
            });
            string source = $"function {key.ToString()}() {{ [native code] }}";
            var copy = (rt?.PreventConstructorInvoke  ?? false)
                ? new JSClassFunction(jsf.f, key.ToString(), source)
                :  new JSFunction(jsf.f, key.ToString(), source);
            ref var target = ref copy.prototype.GetOwnProperties();
            var en = new PropertySequence.ValueEnumerator(jsf.prototype, false);
            while(en.MoveNextProperty(out var Value, out var Key ))
            {
                if (Key.Key != KeyStrings.constructor.Key)
                {
                    target[Key.Key] = Value;
                }
            }
            ref var ro = ref copy.GetOwnProperties();
            en = new PropertySequence.ValueEnumerator(jsf, false);
            while (en.MoveNextProperty(out var Value, out var Key))
            {
                /// this is the case when we do not
                /// want to overwrite Function.prototype
                if (Key.Key != KeyStrings.prototype.Key)
                {
                    ro[Key.Key] = Value;
                }
            }
            if (addToContext)
            {
                context.GetOwnProperties()[key.Key] = JSProperty.Property(key, copy, JSPropertyAttributes.ConfigurableReadonlyValue);
            }
            copy.prototypeChain = chain ?? context.FunctionPrototype ?? context.ObjectPrototype;
            return copy;
        }

        #region Fill

        private static (JSFunctionDelegate getter, JSFunctionDelegate setter) 
            CreateProperty(in this (PropertyInfo method, PrototypeAttribute attribute) m)
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


        private static JSFunctionDelegate CreateJSFunctionDelegate(in this (MethodInfo method, PrototypeAttribute attribute) m)
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

        public static (JSFunctionDelegate function, int length) Fill(Type type, JSObject target)
        {

            JSFunctionDelegate r = null;
            int length = 0;

            ref var ownProperties = ref target.GetOwnProperties();

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
                if (!(v is JSValue jv))
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

            return (r, length);
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
                ? (JSFunction)Activator.CreateInstance(type, key.Value)
                : new JSFunction(JSFunction.empty, key.Value, StringSpan.Empty);

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

                if (pr is ConstructorAttribute ca)
                {
                    r.f = f.CreateJSFunctionDelegate();
                    r[KeyStrings.length] = new JSNumber(ca.Length);
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
                if (!(v is JSValue jv))
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
