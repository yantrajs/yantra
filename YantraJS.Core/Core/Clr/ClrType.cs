using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.Storage;
using YantraJS.ExpHelper;
using YantraJS.LinqExpressions;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Runtime;
using System.ComponentModel;
using YantraJS.Expressions;
using YantraJS.Generator;
using YantraJS.Core.Core.Clr;

namespace YantraJS.Core.Clr
{


    /// <summary>
    /// We might improve statup time by moving reflection code (setting up methods/properties) to proxy.
    /// </summary>
    public class ClrType : JSFunction
    {

        private static ConcurrentUInt32Map<ClrType> cachedTypes = ConcurrentUInt32Map<ClrType>.Create();

        internal class ClrPrototype: JSObject
        {
            internal Func<object, uint, JSValue> GetElementAt;

            internal Func<object, uint, object, JSValue> SetElementAt;

        }


        public static ClrType From(Type type)
        {
            // need to create base type first...
            ClrType baseType = null;
            if(type.BaseType != null && type.BaseType != typeof(object))
            {
                baseType = From(type.BaseType);
            }
            var nc = JSContext.Current.ClrMemberNamingConvention;
            var key = ConcurrentTypeCache.GetOrCreate(type, nc.Name);
            return cachedTypes.GetOrCreate(key, () => new ClrType(nc, type, baseType));
        }

        private readonly ClrMemberNamingConvention namingConvention;
        public readonly Type Type;

        (ConstructorInfo method, ParameterInfo[] parameters)[] constructorCache;

        public override bool ConvertTo(Type type, out object value)
        {
            if(type == typeof(Type))
            {
                value = this.Type;
                return true;
            }
            return base.ConvertTo(type, out value);
        }

        internal void Generate(JSObject target, Type type, bool isStatic)
        {
            if (type.IsGenericTypeDefinition)
                return;

            var isJavaScriptObject = typeof(IJavaScriptObject).IsAssignableFrom(type);

            Func<object, uint, JSValue> indexGetter = null;
            Func<object, uint, object, JSValue> indexSetter = null; ;


            var flags = isStatic
                ? BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static
                : BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance;

            var declaredFields = type.GetTypeInfo().DeclaredFields.Where(x => x.IsStatic == isStatic && x.IsPublic);

            foreach(var field in declaredFields)
            {
                var f = new JSFieldInfo(this.namingConvention, field);
                var name = f.Name;
                if (isJavaScriptObject)
                {
                    if (!f.Export)
                        continue;
                }
                JSFunction getter = f.GenerateFieldGetter();
                JSFunction setter = null;
                if (!(field.IsInitOnly || field.IsLiteral))
                {
                    // you can only read...
                    setter = f.GenerateFieldSetter();
                }
                target.FastAddProperty(name, getter, setter, JSPropertyAttributes.EnumerableConfigurableProperty);
            }

            var declaredProperties = isStatic
                ? type.GetProperties(flags)
                : type.GetTypeInfo()
                    .DeclaredProperties
                    .Where(x => 
                        x.GetMethod?.IsStatic == isStatic 
                        || x.SetMethod?.IsStatic == isStatic).ToArray();


            foreach (var property in declaredProperties
                .GroupBy(x => x.Name)) {
                // only indexer property can have more items...
                var list = property.ToList();
                if (list.Count > 1)
                {
                    throw new NotImplementedException();
                } else
                {
                    var f = new JSPropertyInfo(this.namingConvention, property.First());
                    if (f.PropertyType.IsGenericTypeDefinition)
                        continue;

                    KeyString name = f.Name;
                    if (isJavaScriptObject)
                    {
                        if (!f.Export)
                            continue;
                    }

                    var fgm = f.GetMethod;
                    var fsm = f.SetMethod;
                    if (fgm?.GetParameters().Length > 0)
                    {
                        // it is an index property...
                        name = "index";
                    }

                    if (f.GetMethod?.GetParameters().Length > 0)
                    {
                        var ip = fgm.GetParameters()[0];
                        if (ip.ParameterType == typeof(int))
                        {
                            indexGetter = f.GenerateIndexedGetter();
                            indexSetter = f.GenerateIndexedSetter();
                        } else
                        {
                            if (indexGetter != null)
                                continue;
                            indexGetter = f.GenerateIndexedGetter();
                            indexSetter = f.GenerateIndexedSetter();
                        }
                    } else
                    {
                        JSFunction getter = f.CanRead
                            ? f.GeneratePropertyGetter()
                            : null;
                        JSFunction setter = f.CanWrite
                            ? f.GeneratePropertySetter()
                            : null;

                        if (getter != null || setter != null)
                        {                            
                            target.FastAddProperty(name, getter, setter, JSPropertyAttributes.EnumerableConfigurableProperty);
                        }
                    }

                }
            }

            var clrPrototype = target as ClrPrototype;

            if (isJavaScriptObject) {

                foreach(var method in type.GetMethods(flags))
                {
                    var jsm = new JSMethodInfo(namingConvention, method);
                    if (!jsm.Export)
                        continue;
                    var name = jsm.Name;
                    if (method.IsJSFunctionDelegate())
                    {
                        target.FastAddValue(name,
                            jsm.GenerateInvokeJSFunction(), JSPropertyAttributes.EnumerableConfigurableValue);
                    } else
                    {
                        target.FastAddValue(name
                            ,new JSFunction( jsm.GenerateMethod(), name)
                            , JSPropertyAttributes.EnumerableConfigurableValue);
                    }
                }


                if (indexGetter != null)
                    clrPrototype.GetElementAt = indexGetter;
                if (indexSetter != null)
                    clrPrototype.SetElementAt = indexSetter;

                return;
            }


            foreach (var methods in type.GetMethods(flags)
                .Where(x => !x.IsSpecialName)
                .GroupBy(x => x.Name)) {
                var g = new JSMethodGroup(namingConvention, type, methods);
                var jsMethod = g.JSMethod;
                if (jsMethod != null)
                {
                    var jsm = new JSMethodInfo(namingConvention, jsMethod);
                    var name = jsm.Name;
                    target.FastAddValue(name,
                        jsm.GenerateInvokeJSFunction(), JSPropertyAttributes.EnumerableConfigurableValue);


                    continue;
                }
                //target.FastAddValue(name, isStatic
                //    ? new JSFunction((in Arguments a) => {
                //        return StaticInvoke(name, all, a);
                //    }, name)
                //    : new JSFunction((in Arguments a) => {
                //        return Invoke(name, type, all, a);
                //        }, name)
                //    , JSPropertyAttributes.EnumerableConfigurableValue);
                target.FastAddValue(g.name, g.Generate(isStatic), JSPropertyAttributes.EnumerableConfigurableValue);
            }

            if (isStatic)
                return;

            if (indexGetter != null)
                clrPrototype.GetElementAt = indexGetter;
            if (indexSetter != null)
                clrPrototype.SetElementAt = indexSetter;

            // setup disposables...
            var disposableType = typeof(IDisposable);
            var asyncDisposableType = typeof(IAsyncDisposable);
            if (disposableType.IsAssignableFrom(type) && type.GetInterfaceMap(disposableType)
                .InterfaceMethods
                .Any())
            {
                target.FastAddValue(JSSymbol.dispose, new JSFunction((in Arguments a) =>
                {
                    if (a.This is ClrProxy p && p.value is IDisposable d)
                    {
                        d.Dispose();
                    }
                    return JSUndefined.Value;
                }), JSPropertyAttributes.ConfigurableValue);
            }
            if (asyncDisposableType.IsAssignableFrom(type) && type.GetInterfaceMap(typeof(IAsyncDisposable))
                .InterfaceMethods
                .Any())
            {
                target.FastAddValue(JSSymbol.asyncDispose, new JSFunction((in Arguments a) =>
                {
                    if (a.This is ClrProxy p && p.value is IAsyncDisposable d)
                    {
                        return ClrProxy.From(d.DisposeAsync().AsTask());
                    }
                    return JSUndefined.Value;
                }), JSPropertyAttributes.ConfigurableValue);
            }
        }

        private ClrType(
            ClrMemberNamingConvention namingConvention,
            Type type, 
            ClrType baseType = null) : 
            base(
                type.Name, 
                $"function {type.Name}() {{ [clr-native] }}", 
                new ClrPrototype())
        {
            this.f = Create;
            this.namingConvention = namingConvention;
            this.Type = type;

            Generate(this, type, true);

            Generate(this.prototype, type, false);

            this.constructorCache = type.GetConstructors()
                .Select(c => (method: c, parameters: c.GetParameters()))
                .OrderByDescending(x => x.parameters.RequiredCount())
                .ToArray();

            foreach (var (method, parameters) in constructorCache)
            {
                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Arguments).MakeByRefType())
                {
                    var cx = CreateConstuctorDelegate(method);
                    f = cx.f;
                }
            }

            if (type.IsGenericTypeDefinition)
            {
                // make generic type..

                this.FastAddValue(
                    "makeGenericType",
                    new JSFunction(MakeGenericType, "makeGenericType"), JSPropertyAttributes.EnumerableConfigurableValue);
            }
            else
            {
                // getMethod... name and types...
                this.FastAddValue("getMethod",
                    new JSFunction(GetMethod, "getMethod"), JSPropertyAttributes.EnumerableConfigurableValue);
                this.FastAddValue("getConstructor",
                    new JSFunction(GetConstructor, "getConstructor"),
                    JSPropertyAttributes.EnumerableConfigurableValue);
            }

            if(baseType != null)
            {
                BasePrototypeObject = baseType;
                prototype.BasePrototypeObject = baseType.prototype;

                // set indexer... for int/uint
                
                if (prototype is ClrPrototype p)
                {
                    if (baseType.prototype is ClrPrototype bp)
                    {
                        if (p.GetElementAt == null)
                        {
                            // add converter here...
                            p.GetElementAt = (a1, a2) =>
                            {
                                return bp.GetElementAt(a1, a2);
                            };
                            var et = type.GetElementTypeOrGeneric();
                            if (et != null)
                            {
                                p.SetElementAt = (a1, a2, a3) =>
                                {
                                    if (a3 is JSValue j3)
                                    {
                                        a3 = j3.ForceConvert(et);
                                    }
                                    return bp.SetElementAt(a1, a2, a3);
                                };

                            }
                            else
                            {
                                p.SetElementAt = (a1, a2, a3) =>
                                {
                                    return bp.SetElementAt(a1, a2, a3);
                                };
                            }
                        }
                    } else
                    {
                        var old = p.SetElementAt;
                        if (old != null)
                        {
                            var et = type.GetElementTypeOrGeneric();
                            if (et != null)
                            {
                                p.SetElementAt = (a1, a2, a3) => {
                                    if (a3 is JSValue j3)
                                        a3 = j3.ForceConvert(et);
                                    return old(a1, a2, a3);
                                };
                            }
                        }
                    }
                }
            }

        }

        //private JSValue Create2(ConstructorInfo c, in Arguments a)
        //{
        //    // improve later...
        //    // return ClrProxy.From(c.Invoke(new object[] { a }), prototype);
            
        //}

        public JSFunction CreateConstuctorDelegate(ConstructorInfo c)
        {
            var pe = Expression.Parameter(ArgumentsBuilder.refType);
            var name = this.name.Value;
            JSFunctionDelegate newDelegate =
                Expression.Lambda<JSFunctionDelegate>(name,
                    ClrProxyBuilder.From(Expression.New(c,pe)),
                    pe
                ).Compile();
            return new JSFunction(newDelegate, name);
        }

        // private static MethodInfo createConstuctorDelegate = typeof(ClrType).GetMethod(nameof(CreateConstuctorDelegate));


        public JSValue Create(in Arguments a)
        {
            var (c, values) = constructorCache.Match(a, KeyStrings.constructor);
            return ClrProxy.From(c.Invoke(values), prototype);
        }

        public JSValue GetMethod(in Arguments a)
        {
            var a1 = a.Get1();
            if (a1.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError($"Name is required");
            var name = a1.ToString();
            MethodInfo method;
            Type[] types = null;
            var flags = BindingFlags.IgnoreCase 
                | BindingFlags.Default 
                | BindingFlags.Public 
                | BindingFlags.FlattenHierarchy
                | BindingFlags.Instance
                | BindingFlags.Static;
            if (a.Length == 1)
            {
                method = Type.GetMethod(name, flags);
            } else {
                types = new Type[a.Length - 1];
                for (int i = 1; i < a.Length; i++)
                {
                    var v = a.GetAt(i);
                    types[i-1] = (Type)v.ForceConvert(typeof(Type));
                }
                method = Type.GetMethod(name, flags, null, types, null);
            }
            if (method == null)
                throw new JSException($"Method {name} not found on {Type.Name}");
            var jsm = new JSMethodInfo(namingConvention, method);
            return new JSFunction(jsm.GenerateMethod(), name, "native");
        }


        public JSValue GetConstructor(in Arguments a)
        {
            ConstructorInfo method;
            Type[] types = null;
            if (a.Length == 0)
            {
                method = Type.GetConstructor(new Type[] { });
            }
            else
            {
                types = new Type[a.Length];
                for (int i = 0; i < a.Length; i++)
                {
                    var v = a.GetAt(i);
                    types[i] = (Type)v.ForceConvert(typeof(Type));
                }
                method = Type.GetConstructor(types);
            }
            if (method == null) 
                throw new JSException($"Constructor({string.Join(",", types.Select(x => x.Name))}) not found on {Type.Name}");
            return new JSFunction(GenerateConstructor(method, this.prototype), this);
        }

        public delegate object JSValueFactory(in Arguments a);

        public static JSFunctionDelegate JSValueFactoryDelegate(JSValueFactory fx, JSObject prototype)
        {
            JSValue Factory(in Arguments a)
            {
                var r = fx(in a);
                return ClrProxy.From(r, prototype);
            }
            return Factory;
        }

        private JSFunctionDelegate GenerateConstructor(ConstructorInfo m, JSObject prototype)
        {
            var jfs = m.CompileToJSFunctionDelegate(m.DeclaringType.Name);
            JSValue Factory(in Arguments a)
            {
                var r = jfs(in a);
                return ClrProxy.From(r, prototype);
            }
            return Factory;

            //var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            //var parameters = m.GetArgumentsExpression(args);
            //var call = Expression.TypeAs( Expression.New(m, parameters), typeof(object));
            //var lambda = Expression.Lambda<JSValueFactory>(m.DeclaringType.Name, call, args);
            //var factory = lambda.Compile();
            //return JSValueFactoryDelegate(factory, prototype);
        }


        public JSValue MakeGenericType(in Arguments a)
        {
            var types = new Type[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                var v = a.GetAt(i);
                types[i] = (Type)v.ForceConvert(typeof(Type));
            }
            return ClrType.From(Type.MakeGenericType(types));
        }

        

    }
}
