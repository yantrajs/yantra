using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core.Storage;
using WebAtoms.CoreJS.ExpHelper;
using WebAtoms.CoreJS.LinqExpressions;

namespace WebAtoms.CoreJS.Core.Clr
{
    internal class CachedTypes : ConcurrentSharedStringTrie<ClrType>
    {

    }

    /// <summary>
    /// We might improve statup time by moving reflection code (setting up methods/properties) to proxy.
    /// </summary>
    public class ClrType : JSFunction
    {

        private static CachedTypes cachedTypes = new CachedTypes();

        public static ClrType From(Type type)
        {
            // need to create base type first...
            ClrType baseType = null;
            if(type.BaseType != null && type.BaseType != typeof(object))
            {
                baseType = From(type.BaseType);
            }

            return cachedTypes.GetOrCreate(type.FullName, () => new ClrType(type, baseType));
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
            value = null;
            return false;
        }

        internal static void Generate(JSObject target, Type type, bool isStatic)
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



        private ClrType(Type type, ClrType baseType = null) : base(null, type.Name)
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

            if(baseType != null)
            {
                prototypeChain = baseType.prototype;
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

    }
}
