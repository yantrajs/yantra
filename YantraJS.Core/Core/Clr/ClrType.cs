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

namespace YantraJS.Core.Clr
{
    public class JSExportAttribute: Attribute {

        public readonly string? Name;

        public bool AsCamel = true;

        public JSExportAttribute(
            string? name = null)
        {
            this.Name = name;
        }

    }

    public class JSExportSameNameAttribute : JSExportAttribute
    {
        public JSExportSameNameAttribute()
        {
            this.AsCamel = false;
        }
    }

    internal static class ClrTypeExtensions
    {

        public static string GetJSName(this MemberInfo member)
        {
            var export = member.GetCustomAttribute<JSExportAttribute>();
            if (export == null)
            {
                return member.Name.ToCamelCase();
            }
            return export.Name != null
                ? export.Name
                : (export.AsCamel ? member.Name.ToCamelCase() : member.Name);
        }

        public static bool CanExport(this MemberInfo member,out string name)
        {
            var export = member.GetCustomAttribute<JSExportAttribute>();
            if (export == null)
            {
                name = default;
                return false;
            }
            name = export.Name != null
                ? export.Name
                : ( export.AsCamel ? member.Name.ToCamelCase() : member.Name);
            return true;
        }

        public static   bool IsJSFunctionDelegate( this MethodInfo method)
        {
            var p = method.GetParameters();
            return p.Length == 1
                && typeof(JSValue).IsAssignableFrom(method.ReturnType)
                && p[0].ParameterType == ArgumentsBuilder.refType;
        }
    }


    /// <summary>
    /// We might improve statup time by moving reflection code (setting up methods/properties) to proxy.
    /// </summary>
    public class ClrType : JSFunction
    {

        private static MethodInfo genericJSFunctionDelegate = typeof(ClrType).GetMethod(nameof(ToInstanceJSFunctionDelegate));

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
            var key = ConcurrentTypeCache.GetOrCreate(type);
            return cachedTypes.GetOrCreate(key, () => new ClrType(type, baseType));
        }


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
                var name = field.GetJSName();
                if (isJavaScriptObject)
                {
                    if (!field.CanExport(out name))
                        continue;
                }
                JSFunction getter = GenerateFieldGetter(field);
                JSFunction setter = null;
                if (!(field.IsInitOnly || field.IsLiteral))
                {
                    // you can only read...
                    setter = GenerateFieldSetter(field);
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
                    var f = property.First();
                    if (f.PropertyType.IsGenericTypeDefinition)
                        continue;

                    KeyString name = f.GetJSName();
                    if (isJavaScriptObject)
                    {
                        if (!f.CanExport(out var n))
                            continue;
                        name = n;
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
                            indexGetter = GenerateIndexedGetter(f);
                            indexSetter = GenerateIndexedSetter(f);
                        } else
                        {
                            if (indexGetter != null)
                                continue;
                            indexGetter = GenerateIndexedGetter(f);
                            indexSetter = GenerateIndexedSetter(f);
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
                    if (!method.CanExport(out string name))
                        continue;
                    if (method.IsJSFunctionDelegate())
                    {
                        target.FastAddValue(name,
                            ToJSFunctionDelegate(method, name), JSPropertyAttributes.EnumerableConfigurableValue);
                    } else
                    {
                        target.FastAddValue(name
                            ,new JSFunction( this.GenerateMethod(method), name)
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
                var name = methods.Key.ToCamelCase();
                var all = methods.ToPairs();
                var jsMethod = all.FirstOrDefault(x => x.method.IsJSFunctionDelegate());
                if (jsMethod.method != null)
                {
                    // call directly...
                    // do not worry about @this... 

                    //if (isStatic)
                    //{
                    //    var methodDelegate = (JSFunctionDelegate)jsMethod.method.CreateDelegate(typeof(JSFunctionDelegate));
                    //    target.DefineProperty(name,
                    //        JSProperty.Function(ToStaticDelegate(methodDelegate));
                    //    continue;
                    //}

                    target.FastAddValue(name,
                        ToJSFunctionDelegate(jsMethod.method, name), JSPropertyAttributes.EnumerableConfigurableValue);


                    continue;
                }
                target.FastAddValue(name, isStatic
                    ? new JSFunction((in Arguments a) => {
                        return StaticInvoke(name, all, a);
                    }, name)
                    : new JSFunction((in Arguments a) => {
                        return Invoke(name, type, all, a);
                        }, name)
                    , JSPropertyAttributes.EnumerableConfigurableValue);
            }

            if (isStatic)
                return;

            if (indexGetter != null)
                clrPrototype.GetElementAt = indexGetter;
            if (indexSetter != null)
                clrPrototype.SetElementAt = indexSetter;
        }

        public delegate JSValue InstanceDelegate<T>(T @this, in Arguments a);

        private static JSFunction ToJSFunctionDelegate(MethodInfo method, string name)
        {
            return (JSFunction)genericJSFunctionDelegate.MakeGenericMethod(method.DeclaringType).Invoke(null, new object[] { method, name });
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSFunction ToInstanceJSFunctionDelegate<T>(MethodInfo method, string name)
        {
            if (method.IsStatic)
            {
                var staticDel = (JSFunctionDelegate)method.CreateDelegate(typeof(JSFunctionDelegate));
                return new JSFunction((in Arguments a) =>
                {
                    return staticDel(a);
                }, name);
            }
            var del = (InstanceDelegate<T>)method.CreateDelegate(typeof(InstanceDelegate<T>));
            var type = typeof(T);
            return new JSFunction((in Arguments a) =>
            {
                var @this = (T)a.This.ForceConvert(type);
                return del(@this, a);
            }, name);
            //var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            //var target = Expression.Parameter(method.DeclaringType);
            //var convert = method.IsStatic
            //    ? null
            //    : JSValueBuilder.Coalesce(ArgumentsBuilder.This(args), method.DeclaringType, target, method.Name);

            //var body = Expression.Block(target.AsSequence(),
            //    JSExceptionBuilder.Wrap(ClrProxyBuilder.Marshal(
            //        Expression.Call(
            //            convert, method, args))));
            //var name = method.Name.ToCamelCase();
            //var d = Expression.Lambda<JSFunctionDelegate>(method.Name, body, args).Compile();
            //return new JSFunction(d, name);
        }

        private static JSValue Invoke(in KeyString name, Type type, (MethodInfo method, ParameterInfo[] parameters)[] methods, in Arguments a)
        {
            if (!a.This.ConvertTo(type, out var target))
                throw JSContext.Current.NewTypeError($"{type.Name}.prototype.{name} called with object not of type {type.Name}");
            try
            {
                var (method, args) = methods.Match(a, name);
                return ClrProxy.Marshal(method.Invoke(target, args));
            } catch (Exception ex)
            {
                throw JSException.From(ex);
            }
        }

        private static JSValue StaticInvoke(in KeyString name, (MethodInfo method, ParameterInfo[] parameters)[] methods, in Arguments a)
        {
            try
            {
                var (method, args) = methods.Match(a, name);
                return ClrProxy.Marshal(method.Invoke(null, args));
            }catch (Exception ex)
            {
                throw JSException.From(ex);
            }
        }

        private static JSFunction GenerateFieldGetter(FieldInfo field)
        {
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            Expression convertedThis = field.IsStatic
                ? null
                : JSValueToClrConverter.Get(ArgumentsBuilder.This(args), field.DeclaringType, "this");
            var body = 
                ClrProxyBuilder.Marshal(
                    Expression.Field(
                        convertedThis, field));
            var name = $"get {field.Name.ToCamelCase()}";
            var lambda = Expression.Lambda<JSFunctionDelegate>(name, body, args);
            return new JSFunction(lambda.Compile(), name);

        }

        private static JSFunction GenerateFieldSetter(FieldInfo field)
        {
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var a1 = ArgumentsBuilder.Get1(args);
            var convert = field.IsStatic
                ? null
                : JSValueToClrConverter.Get(ArgumentsBuilder.This(args), field.DeclaringType, "this");

            var clrArg1 = JSValueToClrConverter.Get(a1, field.FieldType, "value");


            var fieldExp = Expression.Field(convert, field);

            // todo
            // not working for `char`
            var assign = Expression.Assign(fieldExp, clrArg1).ToJSValue();

            var body = assign;
            var name = $"set {field.Name.ToCamelCase()}";
            var lambda = Expression.Lambda<JSFunctionDelegate>(name, body, args);
            return new JSFunction(lambda.Compile(), name);
        }

        private static Func<object,uint,JSValue> GenerateIndexedGetter(PropertyInfo property)
        {
            var @this = Expression.Parameter(typeof(object));
            var index = Expression.Parameter(typeof(uint));
            var indexParameter = property.GetMethod.GetParameters()[0];
            Expression indexAccess = index.Type != indexParameter.ParameterType
                ? Expression.Convert(index, indexParameter.ParameterType)
                : index as Expression;
            Expression indexExpression;
            Expression convertThis = Expression.TypeAs(@this, property.DeclaringType);
            if (property.DeclaringType.IsArray)
            {
                // this is direct array.. cast and get.. 
                indexExpression = Expression.ArrayIndex(convertThis, indexAccess);
            } else
            {
                indexExpression = Expression.MakeIndex(convertThis, property, new Expression[] { indexAccess });
            }
            Expression body = JSExceptionBuilder.Wrap(ClrProxyBuilder.Marshal(indexExpression));
            var lambda = Expression.Lambda<Func<object,uint,JSValue>>($"set {property.Name}", body, @this, index);
            return lambda.Compile();
        }

        private static Func<object, uint, object, JSValue> GenerateIndexedSetter(PropertyInfo property)
        {
            if (!property.CanWrite)
                return null;
            
            var type = property.DeclaringType;
            var elementType = type.GetElementTypeOrGeneric() ?? property.PropertyType;

            var @this = Expression.Parameter(typeof(object));
            var index = Expression.Parameter(typeof(uint));
            var value = Expression.Parameter(typeof(object));
            var indexParameter = property.SetMethod.GetParameters()[0];
            Expression indexAccess = index.Type != indexParameter.ParameterType
                ? Expression.Convert(index, indexParameter.ParameterType)
                : index as Expression;
            Expression indexExpression;
            Expression convertThis = Expression.TypeAs(@this, property.DeclaringType);
            if (property.DeclaringType.IsArray)
            {
                // this is direct array.. cast and get.. 
                indexExpression = Expression.ArrayIndex(convertThis, indexAccess);
            }
            else
            {
                indexExpression = Expression.MakeIndex(convertThis, property, new Expression[] { indexAccess });
            }


            Expression body = Expression.Block( 
                JSExceptionBuilder.Wrap( 
                    Expression.Assign(indexExpression , Expression.TypeAs(value, elementType)).ToJSValue()));
            var lambda = Expression.Lambda<Func<object, uint, object, JSValue>>("get " + property.Name, body, @this, index, value);
            return lambda.Compile();
        }

        private ClrType(
            Type type, 
            ClrType baseType = null) : 
            base(
                type.Name, 
                $"function {type.Name}() {{ [clr-native] }}", 
                new ClrPrototype())
        {
            this.f = Create;
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

        private static MethodInfo createConstuctorDelegate = typeof(ClrType).GetMethod(nameof(CreateConstuctorDelegate));


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
            return new JSFunction(GenerateMethod(method), name, "native");
        }

        private JSFunctionDelegate GenerateMethod(MethodInfo m)
        {
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var @this = ArgumentsBuilder.This(args);

            var convertedThis = m.IsStatic
                ? null
                : JSValueToClrConverter.Get(@this, m.DeclaringType, "this");
            var parameters = new List<Expression>();
            var pList = m.GetParameters();
            for (int i = 0; i < pList.Length; i++)
            {
                var pi = pList[i];
                var defValue = pi.HasDefaultValue
                    ? Expression.Constant((object)pi.DefaultValue, typeof(object))
                    : (pi.ParameterType.IsValueType
                        ? Expression.Constant((object)Activator.CreateInstance(pi.ParameterType),typeof(object))
                        : null);
                parameters.Add(JSValueToClrConverter.GetArgument(args, i, pi.ParameterType, defValue, pi.Name));
            }
            var call = Expression.Call(convertedThis, m, parameters);
            var marshal = call.Type == typeof(void)
                ? YExpression.Block(call, JSUndefinedBuilder.Value)
                : ClrProxyBuilder.Marshal(call);
            var wrapTryCatch = JSExceptionBuilder.Wrap(marshal);

            ILCodeGenerator.GenerateLogs = true;
            var lambda = Expression.Lambda<JSFunctionDelegate>(m.Name, wrapTryCatch, args);
            var method = lambda.Compile();
            return method;
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
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());

            var parameters = new List<Expression>();
            var pList = m.GetParameters();
            for (int i = 0; i < pList.Length; i++)
            {
                var ai = ArgumentsBuilder.GetAt(args, i);
                var pi = pList[i];
                Expression defValue;
                if (pi.HasDefaultValue)
                {
                    defValue = Expression.Constant(pi.DefaultValue);
                    if (pi.ParameterType.IsValueType)
                    {
                        defValue = Expression.Box(Expression.Constant(pi.DefaultValue));
                    }
                    parameters.Add(JSValueToClrConverter.Get(ai, pi.ParameterType, defValue, pi.Name));
                    continue;
                }
                defValue = null;
                if(pi.ParameterType.IsValueType)
                {
                    defValue = Expression.Box(Expression.Constant(Activator.CreateInstance(pi.ParameterType)));
                } else
                {
                    defValue = Expression.Null;
                }
                parameters.Add(JSValueToClrConverter.Get(ai, pi.ParameterType, defValue, pi.Name));
            }
            var call = Expression.TypeAs( Expression.New(m, parameters), typeof(object));
            var lambda = Expression.Lambda<JSValueFactory>(m.DeclaringType.Name, call, args);
            var factory = lambda.Compile();
            return JSValueFactoryDelegate(factory, prototype);
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
