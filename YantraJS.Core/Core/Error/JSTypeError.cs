using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{
    public static class JSTypeError
    {

        public const string Cannot_convert_undefined_or_null_to_object = "Cannot convert undefined or null to object";

        public const string Parameter_is_not_an_object = "Parameter is not an object";


        public static string NotIterable(object name) => $"{name} is not iterable";

        public static string NotEntry(object name) => $"Iterator value {name} is an entry object";

        //internal JSTypeError(JSValue message, JSValue stack) : base(message, stack, JSContext.Current.TypeErrorPrototype)
        //{
        //}

    }
}
