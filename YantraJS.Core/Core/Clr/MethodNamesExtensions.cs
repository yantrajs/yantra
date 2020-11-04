using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace YantraJS.Core.Clr
{
    internal static class MethodNamesExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (T method, object[] values) Match<T>(
            this (T method, ParameterInfo[] parameters)[] methods, 
            in Arguments a, 
            in KeyString name)
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
                        if (i < a.Length)
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
                        else
                        {
                            if (pt.HasDefaultValue)
                            {
                                values[i] = pt.DefaultValue;
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
                        return (method, values);
                    }
                }
            }

            throw JSContext.Current.NewTypeError($"No matching parameters found for {name}");
        }

        public static (T method, ParameterInfo[] parameters)[] ToPairs<T>(this IEnumerable<T> methods)
            where T: MethodBase
        {
            return methods
                .Select(x => (method: x, parameters: x.GetParameters()))
                .OrderByDescending(x => x.parameters.RequiredCount())
                .ToArray();
        }

        public static int RequiredCount(this ParameterInfo[] parameters)
        {
            int n = 0;
            foreach (var p in parameters)
            {
                if (p.HasDefaultValue)
                    continue;
                n++;
            }
            return n;
        }

    }
}
