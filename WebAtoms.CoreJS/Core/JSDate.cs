using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSDate: JSObject
    {

        internal DateTime value;

        public DateTime Value
        {
            get => value;
            set => this.value = value;
        }

        public JSDate(DateTime time): base(JSContext.Current.DatePrototype)
        {
            this.value = time;
        }

        [Static("UTC")]
        internal static JSValue UTC(JSValue t,params JSValue[] a)
        {
            return new JSDate(DateTime.UtcNow);
        }

        [Static("now")]
        internal static JSValue Now(JSValue t,params JSValue[] a)
        {
            return new JSDate(DateTime.UtcNow);
        }

        [Static("parse")]
        internal static JSValue Parse(JSValue t,params JSValue[] a)
        {
            return new JSDate(DateTime.Parse(a[0].ToString()));
        }

        [Prototype("getYear")]
        internal static JSValue GetYear(JSValue t,params JSValue[] a)
        {
            if (!(t is JSDate d))
                throw JSContext.Current.TypeError("Method Date.prototype.getYear called on incompatible receiver");
            return new JSNumber(d.value.Year - 2000);
        }

    }
}
