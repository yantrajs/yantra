using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YantraJS
{
    internal static class TypeExtensions
    {

        public static ConstructorInfo GetConstructor(this Type type, params Type[] args)
            => type.GetConstructor(args);


        public static string GetFriendlyName(this Type type)
        {
            if(type.IsArray)
            {
                return type.GetElementType().GetFriendlyName() + "[]";
            }
            if(type.IsConstructedGenericType)
            {
                var a = string.Join(", ", type.GetGenericArguments().Select(x => x.GetFriendlyName()));
                return $"{type.Name}<{a}>";
            }
            if(type.IsGenericTypeDefinition)
            {
                return $"{type.Name}<>";
            }
            return type.Name;
        }
    }

}
