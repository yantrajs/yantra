using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace YantraJS.Core.Clr
{
    internal static class ExtensionMethodHelper
    {
        private static ConcurrentDictionary<Type, List<MethodInfo>> cachedMethods = new ConcurrentDictionary<Type, List<MethodInfo>>();
        
        
        /// <summary>
        /// Get all extension methods, that have type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>All extension methods</returns>
        internal static IEnumerable<MethodInfo> GetExtensionMethods(Type type)
        {
            return cachedMethods.GetOrAdd(type, t =>
            {
                List<Type> AssTypes = new List<Type>();

                foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        AssTypes.AddRange(item.GetTypes());
                    }
                    catch(Exception e)
                    {
                        //Anti crash 
                    }
                   
                }

                var query = from type in AssTypes
                    where !type.IsGenericType && !type.IsNested
                    from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    where method.IsDefined(typeof(ExtensionAttribute), false)
                    where method.GetParameters()[0].ParameterType == t
                    select method;
                return query.ToList();
            });
        }
        
        
        /// <summary>
        /// Check implement Method <see cref="JSFunctionDelegate"/>
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal static bool IsJsFunctionDelegate(MethodInfo info)
        {
            return (info.GetParameters()[1].ParameterType.GetElementType() == typeof(Arguments) && info.ReturnType == typeof(JSValue));
        }



        internal static JSFunction ExtensionMethodToFunction(Type invoker,MethodInfo info,object target, bool isJsFuncDelegate)
        {
            if (isJsFuncDelegate)
            {
                return new JSFunction((in Arguments args) =>
                {
                    return info.Invoke(invoker, new []{target, args} ).Marshal();

                });
            }
            else
            {
                return new JSFunction(((in Arguments arguments) =>
                {
                    var args = arguments
                        .ToArray()
                        .Select(x =>
                        {
                            if (x.ConvertTo(typeof(object), out var ret))
                            {
                                return ret;
                            }

                            return null;
                        })
                        .Prepend(target)
                        .ToArray();
                    return info.Invoke(invoker, args).Marshal();
                }));
            }
           
        }


    }
}