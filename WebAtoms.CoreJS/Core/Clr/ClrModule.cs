using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WebAtoms.CoreJS.Core.Storage;

namespace WebAtoms.CoreJS.Core.Clr
{

    public abstract class JSClrObject<T>: JSObject
    {
        public JSClrObject()
        {
            this.prototypeChain = ClrType.From(typeof(T));
        }
    }

    public class ClrModule : JSClrObject<ClrModule>
    {

        public static JSObject Default = JSObject.NewWithProperties()
            .AddProperty(KeyStrings.@default, new ClrModule());

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
        public JSValue GetClass(in Arguments a)
        {
            var a1 = a.Get1();
            if (!a1.BooleanValue)
                throw JSContext.Current.NewTypeError("First parameter should be non empty string");
            var name = a1.ToString();
            return ClrType.From(Type.GetType(name));
        }

        public JSValue ToInt32(in Arguments a)
        {
            return new ClrProxy(a.Get1().IntValue);
        }

        public JSValue ToString(in Arguments a)
        {
            return new ClrProxy(a.Get1().ToString());
        }

        public JSValue ToBool(in Arguments a)
        {
            return new ClrProxy(a.Get1().BooleanValue);
        }


    }

}
