using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSArguments: JSPrimitive
    {
        [GetProperty("length")]
        public static JSValue GetLength(in Arguments a)
        {
            return new JSNumber(a.This.Length);
        }

        [SetProperty("length")]
        public static JSValue SetLength(in Arguments a)
        {
            // do nothing...
            return a.Get1();
        }

        public static JSValue[] Empty = new JSValue[] { };

        public override int Length { 
            get => arguments.Length;
            set { } }

        public override bool BooleanValue => true;

        public override JSValue TypeOf()
        {
            return JSConstants.Arguments;
        }

        internal override KeyString ToKey(bool create = false)
        {
            return KeyStrings.arguments;
        }


        public override JSBoolean Equals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException();
        }

        internal Arguments arguments = Arguments.Empty;

        public JSArguments(in Arguments args)
        {
            arguments = args;
        }

        protected override JSObject GetPrototype()
        {
            return JSContext.Current.ObjectPrototype;
        }

        public override JSValue this[uint key] 
        {
            get => arguments.GetAt((int)key);
            set => base[key] = value;
        }

        internal override IElementEnumerator GetElementEnumerator()
        {
            return new Enumerator(this.arguments);
        }

        private struct Enumerator : IElementEnumerator
        {
            private Arguments arguments;
            private uint i;

            public Enumerator(Arguments arguments)
            {
                this.arguments = arguments;
                i = uint.MaxValue;
            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                i = (i == uint.MaxValue) ? 0 : (i + 1);
                if(i < arguments.Length)
                {
                    value = arguments.GetAt((int)i);
                    index = i;
                    hasValue = true;
                    return true;
                }
                hasValue = false;
                index = 0;
                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNext(out JSValue value)
            {
                i = (i == uint.MaxValue) ? 0 : (i + 1);
                if (i < arguments.Length)
                {
                    value = arguments.GetAt((int)i);
                    return true;
                }
                value = JSUndefined.Value;
                return false;
            }

        }

    }
}
