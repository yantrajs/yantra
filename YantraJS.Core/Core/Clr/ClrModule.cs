using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YantraJS.Core.Storage;

namespace YantraJS.Core.Clr
{

    public abstract class JSClrObject<T>: JSObject
    {
        public JSClrObject()
        {
            this.BasePrototypeObject = ClrType.From(typeof(T));
        }
    }

    public static class ClrModule
    {

        public static JSObject Default = JSObject.NewWithProperties()
            .AddProperty(KeyStrings.@default, ClrType.From(typeof(ClrModule)));

        public static JSValue Temp1 { get; set; } = new JSNumber(1);

        /// <summary>
        /// Returns JavaScript native class for C# Type Equivalent, which you can use
        /// to create the object of given type and access methods/properties.
        /// 
        /// Usage: 
        /// 
        /// import clr from "clr";
        /// 
        /// let FileInfo = clr.getClass("System.IO.FileInfo");
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static JSValue GetClass(in Arguments a)
        {
            var a1 = a.Get1();
            if (!a1.BooleanValue)
                throw JSContext.Current.NewTypeError("First parameter should be non empty string");
            var name = a1.ToString();
            return ClrType.From(Type.GetType(name));
        }

        public static JSValue ToInt32(in Arguments a)
        {
            return ClrProxy.From(a.Get1().IntValue);
        }

        public static JSValue ToString(in Arguments a)
        {
            return ClrProxy.From(a.Get1().ToString());
        }

        public static JSValue ToBool(in Arguments a)
        {
            return ClrProxy.From(a.Get1().BooleanValue);
        }

        public static JSValue ToDateTime(in Arguments a)
        {
            var a1 = a.Get1();
            if (a1 is JSDate date)
            {
                return ClrProxy.From(date.value);
            }
            throw JSContext.Current.NewTypeError($"Not a Date");
        }

    }

}
