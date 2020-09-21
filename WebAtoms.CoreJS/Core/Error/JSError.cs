using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSError : JSObject
    {

        public static readonly KeyString KeyMessage = "message";

        public static readonly KeyString KeyStack = "stack";

        public static readonly KeyString KeyError = "Error";

        public static readonly KeyString KeyRangeError = "RangeError";

        public const string Cannot_convert_undefined_or_null_to_object = "Cannot convert undefined or null to object";

        public const string Parameter_is_not_an_object = "Parameter is not an object";


        protected JSError( JSValue message, JSValue stack,  JSObject prototype) : base(prototype)
        {
            this.DefineProperty(KeyMessage, JSProperty.Property(message, JSPropertyAttributes.Property | JSPropertyAttributes.Readonly));
            this.DefineProperty(KeyStack, JSProperty.Property(stack, JSPropertyAttributes.Property | JSPropertyAttributes.Readonly));
        }

        internal JSError(JSValue message, JSValue stack) : this(message, stack, JSContext.Current.ErrorPrototype)
        {

        }
    }
}
