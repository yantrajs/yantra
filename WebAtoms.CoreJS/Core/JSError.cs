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


        protected JSError( JSValue message, JSValue stack,  JSValue prototype) : base(prototype)
        {
            ownProperties[KeyMessage] = JSProperty.Property(message, JSPropertyAttributes.Property | JSPropertyAttributes.Readonly);
            ownProperties[KeyStack] = JSProperty.Property(stack, JSPropertyAttributes.Property | JSPropertyAttributes.Readonly);
        }

        internal JSError(JSValue message, JSValue stack) : this(message, stack, JSContext.Current.ErrorPrototype)
        {

        }
    }
}
