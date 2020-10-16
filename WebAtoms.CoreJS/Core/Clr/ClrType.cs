using System;
using System.Linq;
using System.Reflection;

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
        Properties propertyCache;
        (MethodBase method, ParameterInfo[] parameters)[] constructorCache;
        Methods methodCache;

        public ClrType(Type type) : base(null, type.Name)
        {
            this.f = Create;
            this.type = type;

            this.propertyCache = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetAccessors().Length > 0)
                .GroupBy(x => x.Name)
                .ToTrie();

            this.methodCache = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => !x.IsSpecialName)
                .GroupBy(x => x.Name)
                .ToTrie();

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
