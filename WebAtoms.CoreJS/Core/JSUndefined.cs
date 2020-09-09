using System;
using System.Linq;

namespace WebAtoms.CoreJS.Core
{
    public sealed class JSUndefined : JSValue
    {
        private JSUndefined():base(null)
        {

        }

        public static JSValue Value = new JSUndefined();

        internal override KeyString ToKey()
        {
            return KeyStrings.undefined;
        }

        public override JSValue TypeOf()
        {
            return JSConstants.Undefined;
        }

        public override bool BooleanValue => false;

        public override double DoubleValue => double.NaN;

        public override bool IsUndefined => true;

        public override string ToString()
        {
            return "undefined";
        }

        public override JSValue Delete(KeyString key)
        {
            throw JSContext.Current.NewTypeError($"Unable to delete {key} of undefined");
        }

        public override JSValue Delete(uint key)
        {
            throw JSContext.Current.NewTypeError($"Unable to delete {key} of undefined");
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (value.IsNull)
                return JSContext.Current.True;
            if (value.IsUndefined)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (value.IsUndefined)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw new NotImplementedException("undefined is not a function");
        }

    }
}
