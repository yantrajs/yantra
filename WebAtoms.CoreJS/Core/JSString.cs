using System;
using System.Collections.Generic;
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


        internal static JSProperty toString = JSProperty.Function((t, a) => t);

        internal static JSProperty substr = JSProperty.Function((t, a) =>
        {
            var j = t as JSString;
            if (a.Length == 1)
                return new JSString(j.value.Substring(a[0].IntValue));
            if (a.Length == 2)
                return new JSString(j.value.Substring(a[0].IntValue, a[1].IntValue));
            return JSUndefined.Value;
        });

        internal static JSProperty length = new JSProperty {
            get = new JSFunction( (t, a) => new JSNumber(t.Length)),
            set = new JSFunction((t, a) => a[0])
        };


    }
}
