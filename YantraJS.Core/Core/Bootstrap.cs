//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Reflection;
//using System.Security.Cryptography;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using YantraJS.Core.Core.Storage;
//using YantraJS.ExpHelper;

//using Exp = YantraJS.Expressions.YExpression;
//using Expression = YantraJS.Expressions.YExpression;
//using ParameterExpression = YantraJS.Expressions.YParameterExpression;
//using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
//using LabelTarget = YantraJS.Expressions.YLabelTarget;
//using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
//using GotoExpression = YantraJS.Expressions.YGoToExpression;
//using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
//using YantraJS.Runtime;

//namespace YantraJS.Core
//{
//    internal static class Bootstrap
//    {

//        static readonly ConcurrentUInt32Map<JSFunction> cache = ConcurrentUInt32Map<JSFunction>.Create();

//        static readonly ConcurrentStringMap<PropertySequence> propertyCache
//            = ConcurrentStringMap<PropertySequence>.Create();

//        public static void Fill<T>(this JSContext context)
//        {
//            var type = typeof(T);
//            var key = type.FullName;
//            var cached = propertyCache.GetOrCreate(key, (_) =>
//            {
//                var ps = new JSObject();
//                Fill(type, ps);
//                return ps.GetOwnProperties(false);
//            });

//            ref var co = ref context.GetOwnProperties();
//            var e = cached.GetEnumerator(false);
//            while(e.MoveNext(out var keyString, out var value))
//            {
//                co.Put(keyString.Key) = value;
//            }
//        }

//        public static JSFunction Create<T>(
//            this JSContext context, 
//            in KeyString key, 
//            JSObject chain = null, bool addToContext = true)
//        {
//            var type = typeof(T);
//            var rt = type.GetCustomAttribute<JSRuntimeAttribute>();
//            var jsf = cache.GetOrCreate(key.Key, (xkey) =>
//            {
//                JSFunction r = Create(xkey, type);

//                if (rt != null)
//                {


//                    var cx = Fill(rt.StaticType, r);
//                    if (cx.function != null && r.f == JSFunction.empty)
//                    {
//                        r.f = cx.function;
//                        r.FastAddValue(KeyStrings.length, new JSNumber(cx.length), JSPropertyAttributes.EnumerableConfigurableValue);
//                        // r[KeyStrings.length] = new JSNumber(cx.length);
                        
//                    }

//                    cx = Fill(rt.Prototype, r.prototype);
//                    if (cx.function != null && r.f == JSFunction.empty)
//                    {
//                        r.f = cx.function;
//                        r.FastAddValue(KeyStrings.length, new JSNumber(cx.length), JSPropertyAttributes.EnumerableConfigurableValue);
//                        // r[KeyStrings.length] = new JSNumber(cx.length);
//                    }
//                }

//                return r;
//            }, key);
//            string source = $"function {key.ToString()}() {{ [native code] }}";
//            var copy = (rt?.PreventConstructorInvoke  ?? false)
//                ? new JSClassFunction(jsf.f, key.ToString(), source)
//                :  new JSFunction(jsf.f, key.ToString(), source);

//            // copy prototype
//            copy.CloneFrom(jsf);

//            if (addToContext)
//            {
//                context.GetOwnProperties().Put(key.Key) = JSProperty.Property(key, copy, JSPropertyAttributes.EnumerableConfigurableReadonlyValue);
//            }

//            // seal copy...
//            copy.Seal();
//            copy.BasePrototypeObject = chain ?? context.FunctionPrototype ?? context.ObjectPrototype;
//            return copy;
//        }

//        #region Fill

//        private static (JSFunctionDelegate getter, JSFunctionDelegate setter) 
//            CreateProperty(in this (PropertyInfo method, PrototypeAttribute attribute) m)
//        {
            
//            var (property, p) = m;
//            if (property.GetAccessors().FirstOrDefault().IsStatic)
//            {
//                throw new NotSupportedException();
//            }

//            var name = property.DeclaringType.Name;
//            if (name.StartsWith("JS"))
//            {
//                name = name.Substring(2);
//            }

//            //var peList = new List<ParameterExpression>();
//            //ParameterExpression targetExp = null;
//            //var toType = m.method.DeclaringType;
//            //targetExp = Expression.Parameter(typeof(JSValue));
//            //// var target = JSVariable.ValueExpression(targetExp);
//            //var target = targetExp;
//            //// this is a set method...
//            //peList.Add(targetExp);
//            //var rType = property.PropertyType;

//            // wrap...
//            //var pe = Expression.Parameter(typeof(Arguments).MakeByRefType());
//            //var peThis = ArgumentsBuilder.This(pe);
//            //var arg1 = ArgumentsBuilder.Get1(pe);
//            //var coalesce = Expression.Coalesce(
//            //    Expression.TypeAs(peThis, toType),
//            //    Expression.Throw(
//            //        JSExceptionBuilder.New($"{name}.prototype.{p.Name} called with object not of type {name}"), toType));

//            JSFunctionDelegate getter = null;
//            JSFunctionDelegate setter = null;

//            if (property.CanRead)
//            {
//                //var getterBody = Expression.Property(coalesce, property);
//                //var getterLambda = Expression.Lambda<JSFunctionDelegate>($"get {property.Name}", getterBody, pe);
//                //getter = getterLambda.Compile();
//                getter = DelegateHelper.CreatePropertyGetter(property);
//            }
//            if (property.CanWrite)
//            {
//                setter = DelegateHelper.CreatePropertySetter(property);
//            }
//            //if (property.CanWrite) 
//            //{
//            //    var setterBody = Expression.Assign(
//            //        Expression.Property(coalesce, property),
//            //        JSValueBuilder.ForceConvert(arg1, rType));
//            //    var setterLambda = Expression.Lambda<JSFunctionDelegate>($"get {property.Name}", Expression.Block(peList,
//            //        setterBody), pe);
//            //    setter = setterLambda.Compile();
//            //}
//            //setter = (in Arguments a) =>
//            //{
//            //    var f = a.Get1();
//            //    var tx = f.ForceConvert(property.PropertyType);
//            //    property.SetValue(a.This, tx);
//            //    return f;
//            //};
//            return (getter, setter);
//        }

//        private static JSFunction CreateJSFunction(in this (MethodInfo method, PrototypeAttribute attribute, SymbolAttribute _) m, string name = null)
//        {
//            name ??= m.attribute.Name.ToString();
//            var length = m.attribute?.Length ?? 0;
//            return new JSFunction(CreateJSFunctionDelegate(m), name, $"{name}() {{ native }}", length, false);
//        }


//        private static JSFunctionDelegate CreateJSFunctionDelegate(in this (MethodInfo method, PrototypeAttribute attribute, SymbolAttribute s) m)
//        {
//            var (method, p, _) = m;
//            if (method.IsStatic)
//            {
//                return (JSFunctionDelegate)method.CreateDelegate(typeof(JSFunctionDelegate));
//                // var d = (JSFunctionDelegate)method.CreateDelegate(typeof(JSFunctionDelegate));
//                //JSValue DelegateWithTryCatch(in Arguments a)
//                //{
//                //    try
//                //    {
//                //        return d(in a);
//                //    } catch (Exception ex) when (!(ex is JSException)) {
//                //        throw new JSException(ex.ToString());
//                //    }
//                //}
//                //return DelegateWithTryCatch;
//            }

//            var name = method.DeclaringType.Name;
//            if(name.StartsWith("JS"))
//            {
//                name = name.Substring(2);
//            }

//            return DelegateHelper.CreateMethod(method);

//            //var peList = new List<ParameterExpression>();
//            //ParameterExpression targetExp = null;
//            //var toType = m.method.DeclaringType;
//            //var paramList = m.method.GetParameters();
//            //if (paramList?.Length > 0)
//            //{
//            //    targetExp = Expression.Parameter(typeof(JSValue));
//            //    // this is a set method...
//            //    peList.Add(targetExp);
//            //}
//            //var rType = m.method.GetParameters()?.FirstOrDefault()?.ParameterType;

//            //// wrap...
//            //var pe = Expression.Parameter(typeof(Arguments).MakeByRefType());
//            //var peThis = ArgumentsBuilder.This(pe);
//            //var arg1 = ArgumentsBuilder.Get1(pe);
//            //var coalesce = Expression.Coalesce(
//            //    Expression.TypeAs(peThis, toType), 
//            //    Expression.Throw(
//            //        JSExceptionBuilder.New($"{name}.prototype.{p.Name} called with object not of type {name}"), toType));
//            //var body = Expression.Block( peList, targetExp == null
//            //    ? Expression.Call(coalesce, method)
//            //    : Expression.Call(coalesce, method, JSValueBuilder.Coalesce(arg1, rType, targetExp, p.Name.ToString())),
//            //    peThis);
//            //var lambda = Expression.Lambda<JSFunctionDelegate>(method.Name, body, pe);
//            //return lambda.Compile();
//        }

//        public static (JSFunctionDelegate function, int length) Fill(Type type, JSObject target)
//        {

//            JSFunctionDelegate r = null;
//            int length = 0;

//            ref var ownProperties = ref target.GetOwnProperties();

//            var p = target;
//            var all = type
//                .GetMethods(BindingFlags.NonPublic
//                    | BindingFlags.DeclaredOnly
//                    | BindingFlags.Public
//                    | BindingFlags.Static
//                    | BindingFlags.Instance)
//                .Select(x => (
//                    method: x,
//                    attribute: x.GetCustomAttribute<PrototypeAttribute>(),
//                    symbol: x.GetCustomAttribute<SymbolAttribute>()))
//                .Where(x => x.attribute != null || x.symbol != null)
//                .GroupBy(x => x.attribute?.Name ?? x.symbol.Name);
//            foreach (var mg in all)
//            {

//                var f = mg.First();
//                var (m, pr, symbol) = f;

//                //if (mg.Any((x => !x.method.IsStatic)))
//                //    throw new NotSupportedException($"{f.method.Name} should be static method");

//                if(pr == null)
//                {
//                    ref var symbols = ref target.GetSymbols();
//                    var globalSymbol = JSSymbolStatic.GlobalSymbol(symbol.Name);
//                    if (symbols.TryGetValue(globalSymbol.Key, out var x))
//                        continue;
//                    var smf = f.CreateJSFunction(symbol.Name);
//                    symbols.Put(globalSymbol.Key) = JSProperty.Property(globalSymbol.Key, smf, JSPropertyAttributes.ConfigurableValue);
//                    continue;
//                }

//                if (ownProperties.TryGetValue(pr.Name.Key, out var _))
//                    continue;


//                if (pr is ConstructorAttribute)
//                {
//                    r = f.CreateJSFunctionDelegate();
//                    length = pr.Length;
//                    continue;
//                }

//                if (pr.IsMethod)
//                {
//                    var jsf = f.CreateJSFunction();

//                    var fxp = JSProperty.Property(pr.Name,
//                        jsf, pr.ConfigurableValue);

//                    ownProperties.Put(pr.Name.Key) = fxp;
//                    if (symbol != null)
//                    {
//                        ref var symbols = ref target.GetSymbols();
//                        var globalSymbol = JSSymbolStatic.GlobalSymbol(symbol.Name);
//                        symbols.Put(globalSymbol.Key) = JSProperty.Property(globalSymbol.Key, jsf, pr.ConfigurableValue);
//                    }
//                    continue;
//                }

//                if (mg.Count() == 2)
//                {
//                    var l = mg.Last();
//                    var fdel = f.CreateJSFunction();
//                    var ldel = l.CreateJSFunction();
//                    ownProperties.Put(pr.Name.Key) = JSProperty.Property(
//                        mg.Key,
//                        f.attribute.IsGetProperty ? fdel : ldel,
//                        !f.attribute.IsGetProperty ? fdel : ldel,
//                        f.attribute.ConfigurableProperty
//                        );
//                    continue;
//                }

//                var fx = f.CreateJSFunction();
//                ownProperties.Put(pr.Name.Key) = JSProperty.Property(
//                    mg.Key,
//                    f.attribute.IsGetProperty ? fx : null,
//                    !f.attribute.IsGetProperty ? fx : null,
//                    f.attribute.ConfigurableProperty
//                    );


//            }

//            var fields = type
//                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
//                .Select(x => (method: x, attribute: x.GetCustomAttribute<PrototypeAttribute>()))
//                .Where(x => x.attribute != null);
//            foreach (var (f, pr) in fields)
//            {
//                var v = f.GetValue(null);
//                if (!(v is JSValue jv))
//                {
//                    if (f.FieldType == typeof(double))
//                    {
//                        jv = new JSNumber((double)v);
//                    }
//                    else
//                    {
//                        jv = new JSString(v.ToString());
//                    }
//                }
//                if (ownProperties.TryGetValue(pr.Name.Key, out var _))
//                    continue;
//                ownProperties.Put(pr.Name, jv, pr.ReadonlyValue);
//            }

//            return (r, length);
//        }
//        #endregion

//        public static JSObject CreateSharedObject(
//            this JSContext context, 
//            in KeyString key, 
//            Type type, 
//            bool addToContext)
//        {
//            var c = cache.GetOrCreate(key.Key, (x) => {
                
//                return Create(x, type);
//            }, key);
//            if(addToContext)
//            {
//                context[key] = c;
//            }
//            return c;
//        }

//        public static JSFunction Create(in KeyString key, Type type)
//        {
//            JSFunction r = typeof(JSFunction).IsAssignableFrom(type) && type != typeof(JSFunction)
//                ? (JSFunction)Activator.CreateInstance(type, key.Value)
//                : new JSFunction(JSFunction.empty, key.Value, StringSpan.Empty);

//            var p = r.prototype;

//            ref var pe = ref p.GetOwnProperties();

//            // Properties can only be defined on the type...
//            var properties = type
//                .GetProperties(BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
//                .Select(x => (property: x, attribute: x.GetCustomAttribute<PrototypeAttribute>()))
//                .Where(x => x.attribute != null && x.property.DeclaringType == type)
//                .ToList();

//            foreach(var property in properties)
//            {
//                if (
//                    (property.property.CanRead && property.property.GetMethod.IsStatic) ||
//                    (property.property.CanWrite && property.property.SetMethod.IsStatic)) {
//                    // this is static property...
//                    throw new NotImplementedException();
//                } else
//                {
//                    var a = property.attribute;
//                    var name = a.Name;
//                    var (getter, setter) = (property.property, a).CreateProperty();
//                    pe.Put(name, getter, setter, a.ConfigurableProperty);
//                }
//            }

//            var all = type
//                .GetMethods(BindingFlags.NonPublic 
//                    | BindingFlags.DeclaredOnly 
//                    | BindingFlags.Public
//                    | BindingFlags.Static
//                    | BindingFlags.Instance)
//                .Select(x => (method: x, attribute: x.GetCustomAttribute<PrototypeAttribute>(), symbol: x.GetCustomAttribute<SymbolAttribute>()))
//                .Where(x => x.attribute != null && x.method.DeclaringType == type)
//                .GroupBy(x => x.attribute.Name).ToList();

//            List<JSFunction> functionMembers = null;
//            if (type == typeof(JSFunction))
//            {
//                functionMembers = new List<JSFunction>();
//            }

//            ref var staticProperties = ref r.GetOwnProperties();
//            ref var instanceProperties = ref pe;

//            foreach (var mg in all)
//            {
                
//                var f = mg.First();

//                //if (mg.Any((x => !x.method.IsStatic)))
//                //    throw new NotSupportedException($"{f.method.Name} should be static method");

//                var (m, pr, symbol) = f;

//                if (pr is ConstructorAttribute ca)
//                {
//                    r.f = f.CreateJSFunctionDelegate();
//                    r[KeyStrings.length] = new JSNumber(ca.Length);
//                    continue;
//                }

//                ref var target = ref (pr.IsStatic ? ref staticProperties : ref instanceProperties);
//                if (pr.IsMethod)
//                {
//                    var jsf = f.CreateJSFunction();

//                    var fxp = JSProperty.Property(pr.Name,
//                        jsf, pr.ConfigurableValue);
//                    functionMembers?.Add(jsf);
//                    target.Put(pr.Name.Key) = fxp;

//                    if(symbol != null)
//                    {
//                        ref var symbols = ref (pr.IsStatic ? ref r.GetSymbols() : ref p.GetSymbols());
//                        var globalSymbol = JSSymbolStatic.GlobalSymbol(symbol.Name);
//                        symbols.Put(globalSymbol.Key) = JSProperty.Property(jsf, pr.ConfigurableValue);
//                    }

//                    continue;
//                }
                
//                if(mg.Count () == 2)
//                {
//                    var l = mg.Last();
//                    var fdel = f.CreateJSFunction();
//                    var ldel = l.CreateJSFunction();
//                    // target = pr.IsStatic ? r : p;
//                    target.Put(
//                        mg.Key,
//                        f.attribute.IsGetProperty ? fdel : ldel,
//                        !f.attribute.IsGetProperty ? fdel : ldel,
//                        f.attribute.ConfigurableProperty
//                        );
//                    continue;
//                }

//                var fx = f.CreateJSFunction();
//                // target = pr.IsStatic ? r : p;
//                target.Put(
//                    mg.Key,
//                    f.attribute.IsGetProperty ? fx : null,
//                    !f.attribute.IsGetProperty ? fx : null,
//                    f.attribute.ConfigurableProperty
//                    );


//            }

//            var fields = type
//                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
//                .Select(x => (method: x, attribute: x.GetCustomAttribute<PrototypeAttribute>()))
//                .Where(x => x.attribute != null);
//            foreach(var (f,pr) in fields)
//            {
//                // var target = pr.IsStatic ? r : p;
//                var v = f.GetValue(null);
//                if (!(v is JSValue jv))
//                {
//                    if (f.FieldType == typeof(double))
//                    {
//                        jv = new JSNumber((double)v);
//                    }
//                    else
//                    {
//                        jv = new JSString(v.ToString());
//                    }
//                }

//                ref var target = ref (pr.IsStatic ? ref staticProperties : ref instanceProperties);
//                target.Put(pr.Name, jv, pr.ReadonlyValue);
//            }

//            if (functionMembers != null)
//            {
//                // need to set prototype of bind/apply/call... as they are function and prototype would'nt be set
//                foreach(var f in functionMembers)
//                {
//                    f.BasePrototypeObject = r.prototype;
//                }
//            }
//            return r;
//        }

//    }
//}
