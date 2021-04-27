using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace YantraJS
{
    internal static class TypeExtensions
    {

        public static ConstructorInfo GetConstructor(this Type type, params Type[] args)
            => type.GetConstructor(args);

    }

}
