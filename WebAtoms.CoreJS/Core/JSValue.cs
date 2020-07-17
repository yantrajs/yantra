using System;
using System.Linq;

namespace WebAtoms.CoreJS.Core {
    public abstract class JSValue {

        public bool IsUndefined => this is JSUndefined;

        public bool IsNull => this is JSNull;

        public bool IsNumber => this is JSNumber;

        public bool IsObject => this is JSObject;

        public bool IsArray => this is JSArray;

        public bool IsString => this is JSString;


        internal BinaryUInt32Map<JSValue> ownProperties;

        internal JSValue prototype;

        public abstract JSValue this [JSValue key]
        {
            get;
            set;
        }

        public virtual JSValue InvokeFunction(JSValue thisValue, JSValue args)
        {
            throw new NotImplementedException();
        }
    }



}
