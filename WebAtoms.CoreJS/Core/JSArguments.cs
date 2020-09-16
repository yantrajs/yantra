using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSArguments: JSValue
    {

        public static JSValue[] Empty = new JSValue[] { };

        public override int Length { 
            get => (int)this._length;
            set { } }

        public override bool BooleanValue => true;

        public override JSValue TypeOf()
        {
            return JSConstants.Object;
        }

        internal override KeyString ToKey()
        {
            throw new NotImplementedException();
        }


        public static JSValue[] From(params double[] args)
        {
            return args.Select((n) => new JSNumber(n)).ToArray();
        }

        public static JSValue[] From(params string[] args)
        {
            return args.Select((n) => new JSString(n)).ToArray();
        }

        public override JSBoolean Equals(JSValue value)
        {
            throw new NotImplementedException();
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            throw new NotImplementedException();
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
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

        public new JSValue this[uint key] { 
            get => key>= _length ? JSUndefined.Value : elements[key];
            set { } 
        }

        internal override IEnumerable<JSValue> AllElements => throw new NotImplementedException();
    }
}
