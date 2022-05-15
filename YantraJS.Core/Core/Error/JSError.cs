using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{


    public class JSError : JSObject
    {

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            return new JSError(new JSException(a.Get1().ToString()), JSContext.Current.ErrorPrototype);
        }

        [Prototype("toString")]
        public static JSValue ToString(in Arguments a)
        {
            if (!(a.This is JSError e))
                throw JSContext.Current.NewTypeError($"{a.This} is not an Error");
            var name = e.prototypeChain.@object[KeyStrings.constructor][KeyStrings.name];
            return new JSString($"{name}: {e[KeyStrings.message]}");
        }


        public const string Cannot_convert_undefined_or_null_to_object = "Cannot convert undefined or null to object";

        public const string Parameter_is_not_an_object = "Parameter is not an object";

        public JSException Exception { get; }

        //protected JSError( JSValue message, JSValue stack,  JSObject prototype) : base(prototype)
        //{
        //    this.DefineProperty(KeyStrings.message, JSProperty.Property(message, JSPropertyAttributes.ConfigurableValue));
        //    this.DefineProperty(KeyStrings.stack, JSProperty.Property(stack, JSPropertyAttributes.ConfigurableValue));
        //}

        internal JSError(JSException ex, JSObject prototype) : base(prototype)
        {
            this.Exception = ex;
            this.FastAddValue(KeyStrings.message, ex.Message.Marshal(), JSPropertyAttributes.ConfigurableValue);
            this.FastAddValue(KeyStrings.stack, ex.JSStackTrace, JSPropertyAttributes.ConfigurableValue);
        }


        internal JSError(JSException ex) : base(JSContext.Current.ErrorPrototype)
        {
            this.Exception = ex;
            this.FastAddValue(KeyStrings.message, ex.Message.Marshal(), JSPropertyAttributes.ConfigurableValue);
            this.FastAddValue(KeyStrings.stack, ex.JSStackTrace, JSPropertyAttributes.ConfigurableValue);
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
