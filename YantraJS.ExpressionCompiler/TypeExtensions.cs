#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS
{
    internal static class TypeExtensions
    {

        public static string Quoted(this string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var che in text)
            {
                switch(che)
                {
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        sb.Append(che);
                        break;
                }
            }
            return $"\"{sb.ToString()}\"";
        }

        public static ConstructorInfo GetConstructor(this Type type, params Type[] args)
            => type.GetConstructor(args);


        public static Type? GetUnderlyingTypeIfRef(this Type? type)
        {
            if (type == null)
            {
                return type;
            }
            if (type.IsByRef)
            {
                return type.GetElementType();
            }
            return type;
        }
        public static string GetFriendlyName(this MethodInfo method)
        {
            if (method.IsGenericMethod)
            {
                return method.Name + "<" + string.Join(",", method.GetGenericArguments().Select(x => x.GetFriendlyName())) + ">";
            }
            return method.Name;
        }

            public static string GetFriendlyName(this Type? type)
        {
            if (type == null)
                return "";
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

        public static System.Reflection.Emit.MethodBuilder CreateMethod(
            this System.Reflection.Emit.TypeBuilder type,
            YLambdaExpression exp,
            string name, bool hasThis)
        {
            var dt = exp.Type;// this is delegate type...
            MethodInfo invoke = dt.GetMethod("Invoke");
            var pa = invoke.GetParameters();
            var pat = pa.Select(x => x.ParameterType).ToArray();
            var m = type.DefineMethod(
                name, 
                MethodAttributes.Public, 
                hasThis ? CallingConventions.HasThis : CallingConventions.Standard,
                invoke.ReturnType,
                pat);

            for (int i = 0; i < pa.Length; i++)
            {
                var p = pa[i];
                var pd = m.DefineParameter(i + 1, ParameterAttributes.None, p.Name);
                //foreach(var cb in p.GetCustomAttributes())
                //{
                //    System.Diagnostics.Debug.WriteLine(cb);
                //}
            }


            return m;
        }
    }

}
