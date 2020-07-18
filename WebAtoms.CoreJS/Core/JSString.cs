using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSString : JSValue
    {
        private string value;

        private JSString(string value)
        {
            this.value = value;
        }
        internal JSString(string value, uint key = 0)
        {
            this.value = value;
            this.Key = key;
        }

        public uint Key { get; private set; }

        public override string ToString()
        {
            return value;
        }

        public override int Length => value.Length;

        public static JSValue Substring(JSValue t, JSArray a) 
        {
            var j = t as JSString;
            if (a.Length == 1)
                return new JSString(j.value.Substring(a[0].IntValue));
            if (a.Length == 2)
                return new JSString(j.value.Substring(a[0].IntValue, a[1].IntValue));
            return JSUndefined.Value;
        }

        internal static JSFunction Create()
        {
            var r = new JSFunction(JSFunction.empty);
            var p = r.prototype;

            p.DefineProperty(KeyStrings.length, JSProperty.Property(
                (t, a) => new JSNumber(t.Length),
                (t, a) => a[0]));
            
            p.DefineProperty(KeyStrings.toString, JSProperty.Function((t, a) => t));

            var substr = JSProperty.Function(Substring);
            p.DefineProperty(KeyStrings.GetOrCreate("substr"), substr);
            p.DefineProperty(KeyStrings.GetOrCreate("substring"), substr);
            return r;
        }

    }
}
