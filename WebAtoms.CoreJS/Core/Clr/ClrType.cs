using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using WebAtoms.CoreJS.ExpHelper;
using WebAtoms.CoreJS.LinqExpressions;

namespace WebAtoms.CoreJS.Core.Clr
{

    public class ClrType : JSFunction
    {

        private static CachedTypes cachedTypes = new CachedTypes();

        public static ClrType From(Type type)
        {
            return cachedTypes.GetOrCreate(type.FullName, () => new ClrType(type));
        }


        private Type type;
        (MethodBase method, ParameterInfo[] parameters)[] constructorCache;



        internal static void Generate(JSObject target, Type type, bool isStatic, JSFunctionDelegate factory = null)
        {
            
            var flags = isStatic
                ? BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static
                : BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance;

            foreach (var property in type.GetProperties(flags)
                .Where(x => x.GetAccessors().Length > 0)
                .GroupBy(x => x.Name)) {
                var name = property.Key.ToCamelCase();
                // only indexer property can have more items...
                var list = property.ToList();
                if (list.Count > 1)
                {
                    // indexer properties...
                    // indexer is only supported for uint/int type...
                    // pending...
                } else
                {
                    var f = property.First();
                    JSFunctionDelegate getter = f.CanRead
                        ? PreparePropertyGetter(isStatic, f)
                        : null;
                    JSFunctionDelegate setter = f.CanWrite
                        ? PreparePropertySetter(isStatic, f)
                        : null;
                    target.DefineProperty(name, JSProperty.Property(name, getter, setter));
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
                    target.DefineProperty(name, 
                        JSProperty.Function((JSFunctionDelegate)jsMethod.method.CreateDelegate(typeof(JSFunctionDelegate))));
                    continue;
                }
                target.DefineProperty(name, isStatic
                    ? JSProperty.Function((in Arguments a) => {
                        return Invoke(name, type, all, a);
                        })
                    : JSProperty.Function((in Arguments a) => {
                        return StaticInvoke(name, all, a);
                    }));
            }

        }



        private static JSValue Invoke(in KeyString name, Type type, (MethodInfo method, ParameterInfo[] parameters)[] methods, in Arguments a)
        {
            var jsValues = a.ToArray();
            if (!a.This.ConvertTo(type, out var target))
                throw JSContext.Current.NewTypeError($"{type.Name}.prototype.{name} called with object not of type {type.Name}");
            foreach (var (method, parameters) in methods)
            {
                if (parameters.Length >= a.Length)
                {
                    var values = new object[parameters.Length];
                    // first try the match...
                    int match = 0;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var pt = parameters[i];
                        var p = pt.ParameterType;
                        if (pt.HasDefaultValue)
                        {
                            if (i < a.Length)
                            {
                                if (jsValues[i].ConvertTo(p, out var value))
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    values[i] = pt.DefaultValue;
                                }
                            }
                            else
                            {
                                values[i] = pt.DefaultValue;
                            }
                        }
                        else
                        {
                            if (jsValues[i].ConvertTo(p, out var value))
                            {
                                values[i] = value;
                            }
                            else
                            {
                                break;
                            }
                        }
                        match++;
                    }

                    if (match == parameters.Length)
                    {
                        return ClrProxy.Marshal(method.Invoke(target, values));
                    }
                }

            }
            throw JSContext.Current.NewTypeError($"No suitable method {name} found for given values");
        }

        private static JSValue StaticInvoke(in KeyString name, (MethodInfo method, ParameterInfo[] parameters)[] methods, in Arguments a)
        {
            var jsValues = a.ToArray();
            foreach (var (method, parameters) in methods)
            {
                if (parameters.Length >= a.Length)
                {
                    var values = new object[parameters.Length];
                    // first try the match...
                    int match = 0;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var pt = parameters[i];
                        var p = pt.ParameterType;
                        if (pt.HasDefaultValue)
                        {
                            if (i < a.Length)
                            {
                                if (jsValues[i].ConvertTo(p, out var value))
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    values[i] = pt.DefaultValue;
                                }
                            }
                            else
                            {
                                values[i] = pt.DefaultValue;
                            }
                        }
                        else
                        {
                            if (jsValues[i].ConvertTo(p, out var value))
                            {
                                values[i] = value;
                            }
                            else
                            {
                                break;
                            }
                        }
                        match++;
                    }

                    if (match == parameters.Length)
                    {
                        return ClrProxy.Marshal(method.Invoke(null, values));
                    }
                }

            }
            throw JSContext.Current.NewTypeError($"No suitable method {name} found for given values");
        }


        private static JSFunctionDelegate PreparePropertyGetter(bool isStatic, PropertyInfo property)
        {
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var target = Expression.Parameter(property.DeclaringType);
            var convert = isStatic 
                ? null 
                : JSValueBuilder.Coalesce(ArgumentsBuilder.This(args), property.DeclaringType, target, property.Name);

            var body = Expression.Block(new ParameterExpression[] { target }, 
                ClrProxyBuilder.Marshal( 
                    Expression.Property(
                        convert, property) ));

            return Expression.Lambda<JSFunctionDelegate>(body, args).Compile();

        }

        private static JSFunctionDelegate PreparePropertySetter(bool isStatic, PropertyInfo property)
        {
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var a1 = ArgumentsBuilder.Get1(args);
            var target = Expression.Parameter(property.PropertyType);
            var convert = isStatic
                ? null
                : JSValueBuilder.Coalesce(ArgumentsBuilder.This(args), property.DeclaringType, target, property.Name);

            var clrArg1 = JSValueBuilder.Coalesce(a1, property.PropertyType, target, property.Name);

            var body = Expression.Block(new ParameterExpression[] { target },
                Expression.Assign(
                    Expression.Property(
                        convert, property), 
                    clrArg1), a1);

            return Expression.Lambda<JSFunctionDelegate>(body, args).Compile();
        }



        public ClrType(Type type) : base(null, type.Name)
        {
            this.f = Create;
            this.type = type;

            Generate(this, type, true);

            Generate(this.prototype, type, false);

            this.constructorCache = type.GetConstructors()
                .Select(c => (method: (MethodBase)c, parameters: c.GetParameters()))
                .OrderByDescending(x => x.parameters.RequiredCount())
                .ToArray();

            foreach (var c in constructorCache)
            {
                if (c.parameters.Length == 1 && c.parameters[0].ParameterType == typeof(Arguments).MakeByRefType())
                {
                    var cx = c.method as ConstructorInfo;
                    f = (in Arguments a) => Create2(cx, a);
                }
            }

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                this.prototypeChain = From(type.BaseType).prototype;
            }
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            return base.CreateInstance(a);
        }

        private JSValue Create2(ConstructorInfo c, in Arguments a)
        {
            // improve later...
            return ClrProxy.Marshal(c.Invoke(new object[] { a }));
        }


        public JSValue Create(in Arguments a)
        {
            var jsValues = a.ToArray();
            foreach (var c in constructorCache)
            {

                if (c.parameters.Length >= a.Length)
                {
                    var values = new object[c.parameters.Length];
                    // first try the match...
                    int match = 0;
                    for (int i = 0; i < c.parameters.Length; i++)
                    {
                        var pt = c.parameters[i];
                        var p = pt.ParameterType;
                        if (pt.HasDefaultValue)
                        {
                            if (i < a.Length)
                            {
                                if (jsValues[i].ConvertTo(p, out var value))
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    values[i] = pt.DefaultValue;
                                }
                            }
                            else
                            {
                                values[i] = pt.DefaultValue;
                            }
                        }
                        else
                        {
                            if (jsValues[i].ConvertTo(p, out var value))
                            {
                                values[i] = value;
                            }
                            else
                            {
                                break;
                            }
                        }
                        match++;
                    }

                    if (match == c.parameters.Length)
                    {
                        return ClrProxy.Marshal((c.method as ConstructorInfo).Invoke(values));
                    }
                }

            }
            throw JSContext.Current.NewTypeError("No suitable constructor found for given values");

        }

    }
}
