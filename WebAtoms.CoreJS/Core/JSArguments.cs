using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSArguments: JSValue
    {

        public static JSArguments Empty = new JSArguments();

        public override int Length { 
            get => (int)this._length;
            set { } }

        internal static JSArguments FromParameters(params JSValue[] args)
        {
            return new JSArguments(args);
        }


        public static JSArguments From(params JSValue[] args)
        {
            return new JSArguments(args);
        }

        public static JSArguments From(params double[] args)
        {
            return new JSArguments(args.Select((n) => new JSNumber(n)).ToArray());
        }

        public static JSArguments From(params string[] args)
        {
            return new JSArguments(args.Select((n) => new JSString(n)).ToArray());
        }

        public override JSValue AddValue(JSValue value)
        {
            throw new NotImplementedException();
        }

        public override JSValue AddValue(double value)
        {
            throw new NotImplementedException();
        }

        public override JSValue AddValue(string value)
        {
            throw new NotImplementedException();
        }

        public override JSBoolean Equals(JSValue value)
        {
            throw new NotImplementedException();
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            throw new NotImplementedException();
        }

        internal override JSBoolean Less(JSValue value)
        {
            throw new NotImplementedException();
        }

        internal override JSBoolean LessOrEqual(JSValue value)
        {
            throw new NotImplementedException();
        }

        internal override JSBoolean Greater(JSValue value)
        {
            throw new NotImplementedException();
        }

        internal override JSBoolean GreaterOrEqual(JSValue value)
        {
            throw new NotImplementedException();
        }

        internal (JSValue, JSArguments) Slice()
        {
            if (this._length == 0)
                return (JSUndefined.Value, new JSArguments());
            if (this._length == 1)
                return (elements[0], new JSArguments());
            var a = new JSArguments();
            var n = this._length - 1;
            a.elements = new JSArray[n];
            Array.Copy(elements, 1, a.elements, 0, n);
            return (elements[0], a);
        }

        public override JSValue InvokeFunction(JSValue thisValue, JSArguments args)
        {
            throw new NotImplementedException();
        }

        internal readonly uint _length;
        internal JSValue[] elements = null;

        public JSArguments(JSArray a): base(JSContext.Current.ObjectPrototype)
        {
            _length = (uint)a._length;
            uint i;
            elements = new JSValue[_length];
            for(i = 0; i<_length; i++)
            {
                elements[i] = a[i];
            }
        }

        private JSArguments(): base(JSContext.Current.ObjectPrototype)
        {

        }

        public JSArguments(JSValue[] args) : base(JSContext.Current.ObjectPrototype)
        {
            _length = (uint)args.Count();
            uint i = 0;
            elements = new JSValue[_length];
            foreach (var item in args)
            {
                elements[i++] = item;
            }
        }

        public override JSValue this[uint key] { 
            get => key>= _length ? JSUndefined.Value : elements[key];
            set { } 
        }

    }
}
