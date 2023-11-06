using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.InternalExtensions;
using YantraJS.Extensions;

namespace YantraJS.Core.Core.Primitive
{
    public class JSPrimitiveObject: JSObject
    {
        internal readonly JSValue value;

        public JSPrimitiveObject(JSPrimitive value): base(JSContext.Current.ObjectPrototype)
        {
            this.value = value;
            value.ResolvePrototype();
            prototypeChain = value.prototypeChain;
            //this.DefineProperty(KeyStrings.constructor, 
            //    JSProperty.Property(value.prototypeChain[KeyStrings.constructor], JSPropertyAttributes.ReadonlyValue));
        }


        public override string ToString()
        {
            return value.ToString();
        }

        public override double DoubleValue => value.DoubleValue;

        public override long BigIntValue => value.BigIntValue;

        public override bool BooleanValue => value.BooleanValue;

        public override bool ConvertTo(Type type, out object value)
        {
            return this.value.ConvertTo(type, out value);
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            // return value.CreateInstance(a);
            throw JSContext.Current.NewTypeError($"Cannot create instance of {this}");
        }

        public override JSValue AddValue(JSValue value)
        {
            return this.value.AddValue(value);
        }

        protected internal override JSValue GetValue(KeyString key, JSValue receiver, bool throwError = true)
        {
            if (key.Key == KeyStrings.length.Key)
            {
                if (value is JSString @string)
                {
                    return new JSNumber(@string.Length);
                }
            }
            return base.GetValue(key, receiver, throwError);
        }

        public override JSValue this[uint name] {
            get {
                ref var elements = ref GetElements();
                if (elements.TryGetValue(name, out var p))
                    return this.GetValue(p);
                return value[name];
            }
            set {
                if (value is JSString @string) {
                    if (name < @string.Length) {
                        return;
                    }
                }
                base[name] = value;
            }
        }

        /// <summary> Added for below TCs in ExpressionTests.cs
        /// Assert.AreEqual(false, Evaluate("var x = new Number(10); x == new Number(10)"));
        // Assert.AreEqual(true, Evaluate("var x = new Number(10); x == x"));
       /// </summary>
       /// <param name="value"></param>
       /// <returns></returns>
    
        public override bool Equals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return true;
            if (value is JSPrimitiveObject)
            {
                return false;
            }
            return base.Equals(value);
        }
    }
}
