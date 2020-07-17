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
        internal JSString(string value, uint key)
        {
            this.value = value;
            this.Key = key;
        }

        public uint Key { get; private set; }

        public override JSValue this[JSValue key] {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override string ToString()
        {
            return value;
        }
        public static JSObject CreatePrototype(JSContext context, JSObject prototype)
        {
            var str = new JSObject();
            str.prototype = prototype;
            context[KeyStrings.String] = str;
            return str;

        }

    }
}
