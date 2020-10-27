using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core.Storage;
using WebAtoms.CoreJS.ExpHelper;
using WebAtoms.CoreJS.LinqExpressions;

namespace WebAtoms.CoreJS.Core.Clr
{

    /// <summary>
    /// We might improve statup time by moving reflection code (setting up methods/properties) to proxy.
    /// </summary>
    public class ClrType : JSFunction
    {

        private static ConcurrentUInt32Trie<ClrType> cachedTypes = new ConcurrentUInt32Trie<ClrType>();

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


        private Type type;
        (ConstructorInfo method, ParameterInfo[] parameters)[] constructorCache;

        public override bool ConvertTo(Type type, out object value)
        {
            if(type == typeof(Type))
            {
                value = this.type;
                return true;
            }
            return base.ConvertTo(type, out value);
        }

        internal static void Generate(JSObject target, Type type, bool isStatic)
        {
            if (type.IsGenericTypeDefinition)
                return;

            Func<object, uint, JSValue> indexGetter = null;
            Func<object, uint, object, JSValue> indexSetter = null; ;


            var flags = isStatic
                ? BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static
                : BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance;

            var declaredProperties = isStatic
                ? type.GetProperties(flags)
                : type.GetTypeInfo()
                    .DeclaredProperties
                    .Where(x => 
                        x.GetMethod?.IsStatic == isStatic 
                        || x.SetMethod?.IsStatic == isStatic).ToArray();


            foreach (var property in declaredProperties
                .GroupBy(x => x.Name)) {
                var name = property.Key.ToCamelCase();
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
                    var fgm = f.GetMethod;
                    var fsm = f.SetMethod;
                    if (fgm.GetParameters().Length > 0)
                    {
                        // it is an index property...
                        name = "index";
                    }

                    if (f.GetMethod?.GetParameters().Length > 0)
                    {
                        var ip = fgm.GetParameters()[0];
                        if (ip.ParameterType == typeof(int))
                        {
                            indexGetter = PrepareIndexedGetter(f);
                            indexSetter = PrepareIndexedSetter(f);
                        } else
                        {
                            if (indexGetter != null)
                                continue;
                            indexGetter = PrepareIndexedGetter(f);
                            indexSetter = PrepareIndexedSetter(f);
                        }
                    } else
                    {
                        JSFunctionDelegate getter = f.CanRead
                            ? PreparePropertyGetter(isStatic, f)
                            : null;
                        JSFunctionDelegate setter = f.CanWrite
                            ? PreparePropertySetter(isStatic, f)
                            : null;

                        var jsProperty = JSProperty.Property(name, getter, setter);
                        target.DefineProperty(name, jsProperty);
                    }

                }
            }

            foreach (var methods in type.GetMethods(flags)
                .Where(x => !x.IsSpecialName)
                .GroupBy(x => x.Name)) {
                var name = methods.Key.ToCamelCase();
                var all = methods.ToPairs();
                var jsMethod = all.FirstOrDefault(x => 
                    x.parameters?.Length == 1 
                    && typeof(JSValue).IsAssignableFrom(x.method.ReturnType)
                    && x.parameters[0].ParameterType == typeof(Arguments).MakeByRefType());
                if (jsMethod.method != null)
                {
                    // call directly...
                    // do not worry about @this... 

                    if (isStatic)
                    {

                        target.DefineProperty(name,
                            JSProperty.Function((JSFunctionDelegate)jsMethod.method.CreateDelegate(typeof(JSFunctionDelegate))));
                        continue;
                    }

                    target.DefineProperty(name,
                        JSProperty.Function(ToInstanceDelegate(jsMethod.method)));


                    continue;
                }
                target.DefineProperty(name, isStatic
                    ? JSProperty.Function((in Arguments a) => {
                        return StaticInvoke(name, all, a);
                    })
                    : JSProperty.Function((in Arguments a) => {
                        return Invoke(name, type, all, a);
                        })
                    );
            }

            if (isStatic)
                return;
            var clrPrototype = target as ClrPrototype;

            if (indexGetter != null)
                clrPrototype.GetElementAt = indexGetter;
            if (indexSetter != null)
                clrPrototype.SetElementAt = indexSetter;

        }

        private static JSFunctionDelegate ToInstanceDelegate(MethodInfo method)
        {
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var target = Expression.Parameter(method.DeclaringType);
            var convert = JSValueBuilder.Coalesce(ArgumentsBuilder.This(args), method.DeclaringType, target, method.Name);

            var body = Expression.Block(new ParameterExpression[] { target },
                ClrProxyBuilder.Marshal(
                    Expression.Call(
                        convert, method, args)));

            return Expression.Lambda<JSFunctionDelegate>(body, args).Compile();
        }

        private static JSValue Invoke(in KeyString name, Type type, (MethodInfo method, ParameterInfo[] parameters)[] methods, in Arguments a)
        {
            if (!a.This.ConvertTo(type, out var target))
                throw JSContext.Current.NewTypeError($"{type.Name}.prototype.{name} called with object not of type {type.Name}");

            var (method, args) = methods.Match(a, name);
            return ClrProxy.Marshal(method.Invoke(target, args));
        }

        private static JSValue StaticInvoke(in KeyString name, (MethodInfo method, ParameterInfo[] parameters)[] methods, in Arguments a)
        {
            var (method, args) = methods.Match(a, name);
            return ClrProxy.Marshal(method.Invoke(null, args));
        }


        private static JSFunctionDelegate PreparePropertyGetter(bool isStatic, PropertyInfo property)
        {
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            Expression convertedThis = isStatic
                ? null
                : JSValueBuilder.ForceConvert(ArgumentsBuilder.This(args), property.DeclaringType);
            var body = Expression.Block( 
                ClrProxyBuilder.Marshal( 
                    Expression.Property(
                        convertedThis, property) ));

            var lambda = Expression.Lambda<JSFunctionDelegate>(body, args);
            return lambda.Compile();

        }

        private static JSFunctionDelegate PreparePropertySetter(bool isStatic, PropertyInfo property)
        {
            //if (property.GetIndexParameters()?.Length > 0)
            //{
            //    return PrepareIndexedPropertySetter(property);
            //}
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var a1 = ArgumentsBuilder.Get1(args);
            var target = Expression.Parameter(property.PropertyType);
            var convert = isStatic
                ? null
                : JSValueBuilder.ForceConvert(ArgumentsBuilder.This(args), property.DeclaringType);

            var clrArg1 = JSValueBuilder.ForceConvert(a1, property.PropertyType);

            var body = Expression.Block(new ParameterExpression[] { target },
                Expression.Assign(
                    Expression.Property(
                        convert, property),
                    clrArg1), a1);

            var lambda = Expression.Lambda<JSFunctionDelegate>(body, args);
            return lambda.Compile();
        }

        private static Func<object,uint,JSValue> PrepareIndexedGetter(PropertyInfo property)
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
            Expression body = ClrProxyBuilder.Marshal(indexExpression);
            var lambda = Expression.Lambda<Func<object,uint,JSValue>>(body, @this, index);
            return lambda.Compile();
        }

        private static Func<object, uint, object, JSValue> PrepareIndexedSetter(PropertyInfo property)
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


            Expression body = Expression.Block( Expression.Assign(indexExpression , Expression.TypeAs(value, elementType)), JSUndefinedBuilder.Value);
            var lambda = Expression.Lambda<Func<object, uint, object, JSValue>>(body, @this, index, value);
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
            this.type = type;

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
                    var cx = method as ConstructorInfo;
                    f = (in Arguments a) => Create2(cx, a);
                }
            }

            if (type.IsGenericTypeDefinition)
            {
                // make generic type..

                this.DefineProperty(
                    "makeGenericType",
                    JSProperty.Function(MakeGenericType));
            }
            else
            {
                // getMethod... name and types...
                this.DefineProperty("getMethod",
                    JSProperty.Function(GetMethod));
            }

            if(baseType != null)
            {
                prototypeChain = baseType.prototype;

                // set indexer... for int/uint
                
                if (prototype is ClrPrototype p)
                {
                    if (prototypeChain is ClrPrototype bp)
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

        private JSValue Create2(ConstructorInfo c, in Arguments a)
        {
            // improve later...
            return ClrProxy.Marshal(c.Invoke(new object[] { a }));
        }


        public JSValue Create(in Arguments a)
        {
            var (c, values) = constructorCache.Match(a, KeyStrings.constructor);
            return ClrProxy.Marshal(c.Invoke(values));
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
                method = type.GetMethod(name, flags);
            } else {
                types = new Type[a.Length - 1];
                for (int i = 1; i < a.Length; i++)
                {
                    var v = a.GetAt(i);
                    types[i-1] = (Type)v.ForceConvert(typeof(Type));
                }
                method = type.GetMethod(name, flags, null, types, null);
            }
            if (method == null)
                throw new JSException($"Method {name} not found on {type.Name}");
            return new JSFunction(GenerateMethod(method), name, "native");
        }

        private JSFunctionDelegate GenerateMethod(MethodInfo m)
        {
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var @this = ArgumentsBuilder.This(args);

            var convertedThis = m.IsStatic
                ? null
                : JSValueBuilder.ForceConvert(@this, m.DeclaringType);
            var parameters = new List<Expression>();
            var pList = m.GetParameters();
            for (int i = 0; i < pList.Length; i++)
            {
                var ai = ArgumentsBuilder.GetAt(args, i);
                var pi = pList[i];
                var defValue = pi.HasDefaultValue
                    ? Expression.Constant(pi.DefaultValue)
                    : (pi.ParameterType.IsValueType
                        ? Expression.Constant(Activator.CreateInstance(pi.ParameterType))
                        : Expression.Constant(null, pi.ParameterType));
                parameters.Add(JSValueBuilder.Convert(ai, pi.ParameterType, defValue));
            }
            var call = Expression.Call(convertedThis, m, parameters);
            var marshal = ClrProxyBuilder.Marshal(call);
            var wrapTryCatch = JSExceptionBuilder.Wrap(marshal);

            var lambda = Expression.Lambda<JSFunctionDelegate>(wrapTryCatch, args);
            return lambda.Compile();
        }

        public JSValue MakeGenericType(in Arguments a)
        {
            var types = new Type[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                var v = a.GetAt(i);
                types[i] = (Type)v.ForceConvert(typeof(Type));
            }
            return ClrType.From(type.MakeGenericType(types));
        }

        

    }
}
