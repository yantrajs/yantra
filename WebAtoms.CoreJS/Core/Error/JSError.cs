using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{

    public class JSError : JSObject
    {

        public const string Cannot_convert_undefined_or_null_to_object = "Cannot convert undefined or null to object";

        public const string Parameter_is_not_an_object = "Parameter is not an object";

        public JSException Exception { get; }

        //protected JSError( JSValue message, JSValue stack,  JSObject prototype) : base(prototype)
        //{
        //    this.DefineProperty(KeyStrings.message, JSProperty.Property(message, JSPropertyAttributes.ConfigurableValue));
        //    this.DefineProperty(KeyStrings.stack, JSProperty.Property(stack, JSPropertyAttributes.ConfigurableValue));
        //}

        internal JSError(JSException ex) : base(JSContext.Current.ErrorPrototype)
        {
            this.Exception = ex;
            this.DefineProperty(KeyStrings.message, JSProperty.Property(ex.Message.Marshal(), JSPropertyAttributes.ConfigurableValue));
            this.DefineProperty(KeyStrings.stack, JSProperty.Property(ex.JSStackTrace, JSPropertyAttributes.ConfigurableValue));
        }

        public static JSValue From(Exception ex)
        {
            if(ex is JSException jse)
            {
                return jse.Error;
            }
            return new JSError(new JSException(ex.Message));
        }
    }
}
