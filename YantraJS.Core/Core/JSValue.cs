using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using YantraJS.Core.Clr;
using YantraJS.Core.Core;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core {

    public abstract partial class JSValue : IDynamicMetaObjectProvider {

        public bool IsUndefined
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this == JSUndefined.Value;
        }

        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this == JSNull.Value;
        }

        internal bool IsNullOrUndefined
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this == JSNull.Value || this == JSUndefined.Value;
        }

        public virtual bool IsNumber => false;

        public virtual bool IsObject => false;

        public virtual bool IsSymbol => false;

        public virtual bool IsArray => false;

        public virtual bool IsString => false;

        public virtual bool IsBoolean => false;

        public virtual bool IsFunction => false;

        internal virtual bool IsSpread => false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Convert(Type type, object def)
        {
            if (type.IsAssignableFrom(typeof(JSValue)))
                return this;
            if (ConvertTo(type, out var v))
                return v;
            return def;
        }

        public object ForceConvert(Type type) { 
            if (type.IsAssignableFrom(GetType()))
            {
                return this;
            }
            if (ConvertTo(type, out var value))
                return value;
            if (prototypeChain?.@object is ClrProxy proxy)
            {
                if (type.IsAssignableFrom(proxy.value.GetType()))
                {
                    return proxy.value;
                }
            }
            //if (type.IsAssignableFrom(this.GetType()))
            //    return this;
            throw JSContext.Current.NewTypeError($"Cannot convert {this} to type {type.Name}");
        }

        internal bool TryConvertTo(Type type, out object value)
        {
            if (typeof(JSValue).IsAssignableFrom(type))
            {
                value = this;
                return true;
            }
            return ConvertTo(type, out value);
        }
        public virtual bool ConvertTo(Type type, out object value) {
            if (type == typeof(JSValue)) {
                value = this;
                return true;
            }
            value = null;
            return false;
        }

        public bool CanBeNumber
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.IsNumber || this.IsBoolean || this.IsNull;
            }
        } 

        public virtual int Length {
            get => 0;
            set { }
        }

        public virtual double DoubleValue => Double.NaN;

        public abstract bool BooleanValue { get; }

        internal virtual string StringValue => this.ToString();

        public abstract JSValue TypeOf();

        public virtual int IntValue => (int)(((long)this.DoubleValue << 32) >> 32);

        /// <summary>
        /// Integer value restricts value within int.MaxValue and
        /// more than int.MaxValue is returned as int.MaxValue
        /// </summary>
        public virtual int IntegerValue
        {
            get
            {
                var v = DoubleValue;
                if (v > 2147483647.0)
                    return 2147483647;
#pragma warning disable 1718
                if (v != v)
                    return 0;
#pragma warning restore 1718
                return (int)v;
            }
        }

        public virtual long BigIntValue => (long)(ulong)this.DoubleValue;

        public virtual uint UIntValue => (uint)(((long)this.DoubleValue << 32) >> 32);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSPrototype prototypeChain;

        internal virtual JSObject BasePrototypeObject
        {
            set
            {
                prototypeChain = value?.PrototypeObject;
            }
        }

        
        /// <summary>
        /// Unless overriden, it returns self
        /// </summary>
        /// <returns></returns>
        public virtual JSValue ValueOf() {
            return this;
        }

        public virtual JSValue Negate()
        {
            return new JSNumber(-DoubleValue);
        }

        public virtual JSValue Subtract(JSValue value)
        {
            return new JSNumber(this.DoubleValue - value.DoubleValue);
        }

        public virtual JSValue Multiply(JSValue value)
        {
            return new JSNumber(this.DoubleValue * value.DoubleValue);
        }

        /// <summary>
        public virtual JSValue Divide(JSValue value)
        {
            // if (value.DoubleValue == 0)
                // return JSNumber.PositiveInfinity;
            return new JSNumber(this.DoubleValue / value.DoubleValue);
        }

        public virtual JSValue BitwiseAnd(JSValue value)
        {
            return new JSNumber(this.IntValue & value.IntValue);
        }

        public virtual JSValue BitwiseOr(JSValue value)
        {
            return new JSNumber(this.IntValue | value.IntValue);
        }

        public virtual JSValue BitwiseXor(JSValue value)
        {
            return new JSNumber(this.IntValue ^ value.IntValue);
        }

        public virtual JSValue LeftShift(JSValue value)
        {
            return new JSNumber(this.IntValue << value.IntValue);
        }

        public virtual JSValue RightShift(JSValue value)
        {
            return new JSNumber(this.IntValue >> (value.IntValue & 0x1F));
        }

        public virtual JSValue UnsignedRightShift(JSValue value)
        {
            return new JSNumber(this.UIntValue >> value.IntValue);
        }

        public virtual JSValue Modulo(JSValue value)
        {
            return new JSNumber(this.DoubleValue % value.DoubleValue);
        }

        /// Speed improvements for string contact operations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual JSValue AddValue(JSValue value)
        {
            var self = this.ValueOf();
            value = value.IsObject ? value.ValueOf() : value;

            if (self.CanBeNumber && value.CanBeNumber)
            {
                return new JSNumber(self.DoubleValue + value.DoubleValue);
            }
            if (value.ToString().Length == 0)
                return self.IsString ? self : new JSString(self.StringValue);
            return new JSString(self.StringValue + value.StringValue);
        }
        /// <summary>
        /// Speed improvements for string contact operations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual JSValue AddValue(double value)
        {
            var self = this.ValueOf();
            if (self.CanBeNumber)
            {
                return new JSNumber(self.DoubleValue + value);
            }
            //if (value.ToString().Length == 0)
            //    return self.IsString ? self : new JSString(self.StringValue);
            return new JSString(self.StringValue + value);
        }
        /// <summary>
        /// Speed improvements for string contact operations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual JSValue AddValue(string value)
        {
            var self = this.ValueOf();

            if (value.Length == 0)
                return self.IsString ? self : new JSString(self.StringValue);
            return new JSString(self.StringValue + value);
        }



        protected JSValue(JSObject prototype)
        {
            this.BasePrototypeObject = prototype ?? GetCurrentPrototype();
        }

        protected virtual JSObject GetCurrentPrototype()
        {
            return null;
        }

        internal abstract PropertyKey ToKey(bool create = true);

        public virtual JSValue GetPrototypeOf()
        {
            return prototypeChain?.@object ?? JSNull.Value;
        }

        public virtual void SetPrototypeOf(JSValue target)
        {
            if (target == JSNull.Value)
            {
                BasePrototypeObject = null;
                return;
            }
            if (!(target is JSObject proto))
                throw JSContext.Current.NewTypeError($"Prototype must be an object or null");
            BasePrototypeObject = proto;
        }

        public virtual JSValue GetOwnPropertyDescriptor(JSValue name)
        {
            throw new NotImplementedException();
        }

        public virtual JSValue GetOwnProperty(in KeyString name)
        {
            var pc = prototypeChain;
            if (pc != null)
                return this.GetValue(pc.GetInternalProperty(name));
            return JSUndefined.Value;
        }

        public virtual JSValue GetOwnProperty(uint name)
        {
            var pc = prototypeChain;
            if (pc != null)
                return this.GetValue(pc.GetInternalProperty(name));
            return JSUndefined.Value;
        }

        public virtual JSValue GetOwnProperty(JSSymbol name)
        {
            var pc = prototypeChain;
            if (pc != null)
                return this.GetValue(pc.GetInternalProperty(name));
            return JSUndefined.Value;
        }

        public JSValue GetOwnProperty(JSValue name)
        {
            if (name is JSSymbol symbol)
                return GetOwnProperty(symbol);
            var key = name.ToKey(false);
            if (key.IsUInt)
            {
                return GetOwnProperty(key.Index);
            }
            return GetOwnProperty(in key.KeyString);
        }

        public JSValue PropertyOrUndefined(in KeyString name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            return GetValue(name, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue PropertyOrUndefined(JSObject super, in KeyString name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            var pc = prototypeChain;
            if (pc == null)
                return JSUndefined.Value;
            return super.GetValue(name, this);
        }

        public JSValue PropertyOrUndefined(uint name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            return GetValue(name, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue PropertyOrUndefined(JSObject super, uint name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            var pc = this.prototypeChain;
            if (pc == null)
                return JSUndefined.Value;
            return super.GetValue(name, this);
        }

        public JSValue PropertyOrUndefined(JSSymbol name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            return GetValue(name, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue PropertyOrUndefined(JSObject super, JSSymbol name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            var pc = prototypeChain;
            if (pc == null)
                return JSUndefined.Value;
            return super.GetValue(name, this);
        }

        public JSValue PropertyOrUndefined(JSValue name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            if (name is JSSymbol s)
                return PropertyOrUndefined(s);
            var k = name.ToKey(false);
            if (k.IsUInt)
                return PropertyOrUndefined(k.Index);
            return PropertyOrUndefined(k.KeyString);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue PropertyOrUndefined(JSObject super, JSValue name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            if (name is JSSymbol s)
                return PropertyOrUndefined(super, s);
            var k = name.ToKey(false);
            if (k.IsUInt)
                return PropertyOrUndefined(k.Index);
            return PropertyOrUndefined(k.KeyString);
        }

        public virtual JSValue this[KeyString name]
        {
            get
            {
                return GetValue(name, this);
            }
            set
            {
                // throw new NotSupportedException();
            }
        }

        public virtual JSValue this[uint key]
        {
            get
            {
                return this.GetValue(key, this);
            }
            set { }
        }

        public virtual JSValue this[JSSymbol symbol]
        {
            get
            {
                return GetValue(symbol, this);
            }
            set { }
        }

        public JSValue this[JSValue key]
        {
            get
            {
                return GetValue(key, this);
            }
            set
            {
                SetValue(key, value, this);
            }
        }

        internal virtual JSValue this[KeyString name, JSValue @this]
        {
            get
            {
                if (prototypeChain == null)
                    return JSUndefined.Value;
                return GetValue(name, this);
            }
            set { }
        }

        internal protected virtual JSValue GetValue(uint key, JSValue receiver, bool throwError = true)
        {
            if (prototypeChain != null)
            {
                var p = prototypeChain.GetInternalProperty(key);
                return (receiver ?? this).GetValue(p);
            }
            return JSUndefined.Value;
        }

        internal protected virtual JSValue GetValue(KeyString key, JSValue receiver, bool throwError = true)
        {
            if (prototypeChain != null)
            {
                var p = prototypeChain.GetInternalProperty(key);
                return (receiver ?? this).GetValue(p);
            }
            return JSUndefined.Value;
        }

        internal protected virtual JSValue GetValue(JSSymbol key, JSValue receiver, bool throwError = true)
        {
            if (prototypeChain != null)
            {
                var p = prototypeChain.GetInternalProperty(key);
                return (receiver ?? this).GetValue(p);
            }
            return JSUndefined.Value;
        }

        internal JSValue GetValue(JSValue key, JSValue receiver, bool throwError = true)
        {
            var k = key.ToKey(false);
            switch (k.Type)
            {
                case KeyType.UInt:
                    return GetValue(k.Index, receiver, throwError);
                case KeyType.String:
                    return GetValue(k.KeyString, receiver, throwError);
                case KeyType.Symbol:
                    return GetValue(k.Symbol, receiver, throwError);
            }
            return JSUndefined.Value;

        }

        internal protected virtual bool SetValue(uint key, JSValue value, JSValue receiver, bool throwError = true)
        {
            return false;
        }

        internal protected virtual bool SetValue(KeyString key, JSValue value, JSValue receiver, bool throwError = true)
        {
            return false;
        }

        internal protected virtual bool SetValue(JSSymbol key, JSValue value, JSValue receiver, bool throwError = true)
        {
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool SetValue(JSValue key, JSValue value, JSValue receiver, bool throwError = true)
        {
            var k = key.ToKey();
            switch (k.Type)
            {
                case KeyType.Empty:
                    return false;
                case KeyType.UInt:
                    return SetValue(k.Index, value, receiver, throwError);
                case KeyType.String:
                    return SetValue(k.KeyString, value, receiver, throwError);
                case KeyType.Symbol:
                    return SetValue(k.Symbol, value, receiver, throwError);
            }
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue this[JSObject super, KeyString name]
        {
            get => super.GetValue(name, this);
            set
            {
                super.SetValue(name, value, this);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue this[JSObject super, uint index]
        {
            get => super.GetValue(index, this);
            set
            {
                super.SetValue(index, value, this);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue this[JSObject super, JSValue name]
        {
            get
            {
                return super.GetValue(name, this);
            }
            set {
                super.SetValue(name, value, this);
            }
        }


        public abstract bool Equals(JSValue value);

        public virtual bool EqualsLiteral (string value)
        {
            return false;
        }
        public virtual bool EqualsLiteral(double value)
        {
            return false;
        }

        public virtual bool StrictEqualsLiteral(string value)
        {
            return false;
        }
        public virtual bool StrictEqualsLiteral(double value)
        {
            return false;
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool StaticEquals(JSValue left, JSValue right)
        {
            return left.Equals(right);
        }

        public abstract bool StrictEquals(JSValue value);

        /// <summary>
        /// 1. NaN is considered equal to NaN.
        /// 2. +0 and -0 are considered to be equal.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool SameValueZero(JSValue value) {
            return this.StrictEquals(value);
        }

        public virtual bool Less(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined))
            {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue < value.DoubleValue)
                        return true;
                }
                else if (this.ToString().Less(value.ToString()))
                    return true;
            }
            return false;

        }
        public virtual bool LessOrEqual(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined))
            {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue <= value.DoubleValue)
                        return true;
                }
                else if (this.ToString().LessOrEqual(value.ToString()))
                    return true;
            }
            return false;

        }

        public virtual bool Greater(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined))
            {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue > value.DoubleValue)
                        return true;
                }
                else if (this.ToString().Greater(value.ToString()))
                    return true;
            }
            return false;

        }
        public virtual bool GreaterOrEqual(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined)) {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue >= value.DoubleValue)
                        return true;
                }
                else if (this.ToString().Greater(value.ToString()))
                    return true;
            }
            return false;
        }

        //internal virtual IEnumerable<JSValue> GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        //{
        //    yield break;
        //}

        public virtual IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true) {
            return new ElementEnumerator();
        }

        internal virtual JSBoolean Is(JSValue value)
        {
            return object.ReferenceEquals(this, value) ? JSBoolean.True : JSBoolean.False;
        }


        public virtual JSValue CreateInstance(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"Cannot create instance of {this}");
        }

        public abstract JSValue InvokeFunction(in Arguments a);

        internal virtual JSFunctionDelegate GetMethod(in KeyString key)
        {
            return prototypeChain.GetMethod(key);
        }

        /// <summary>
        /// Warning do not use in concatenation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // use inherited version..
            throw new NotSupportedException($"Use inherited version ... {this.GetType().Name} ");
            //var fx = this[KeyStrings.toString];
            //if (fx.IsUndefined)
            //    return "undefined";
            //var obj = fx.InvokeFunction(this, JSArguments.Empty);
            //if (obj == this)
            //{
            //    throw new StackOverflowException();
            //}
            //return obj.ToString();
        }


        /// <summary>
        /// Returns a string containing a locale-dependant version of the number.
        /// </summary>
        /// <returns> A string containing a locale-dependant version of the number. </returns>
        /// 
        public virtual string ToLocaleString(string format, CultureInfo culture) {
            throw new NotImplementedException();
        }


        public virtual string ToDetailString()
        {
            return this.ToString();
        }

        public virtual JSValue Delete(in KeyString key)
        {
            return JSBoolean.True;
        }
        public virtual JSValue Delete(uint key)
        {
            return JSBoolean.True;
        }

        public virtual JSValue Delete(JSSymbol symbol)
        {
            return JSBoolean.True;
        }

        public virtual JSValue Delete(JSValue index)
        {
            var key = index.ToKey(false);
            switch (key.Type)
            {
                case KeyType.Empty:
                    return JSBoolean.False;
                case KeyType.UInt:
                    return Delete(key.Index);
                case KeyType.String:
                    return Delete(key.KeyString);
                case KeyType.Symbol:
                    return Delete(key.Symbol);
            }
            return JSBoolean.False;
        }

        internal JSValue InternalInvoke(object name, in Arguments a)
        {
            JSValue fx = null;
            switch(name)
            {
                case JSValue v:
                    fx = this[v];
                    break;
                case KeyString ks:
                    fx = this[ks];
                    break;
                case string str:
                    fx = this[str];
                    break;
            }
            if (fx.IsUndefined)
                throw JSContext.Current.NewTypeError($"Cannot invoke {name} of object as it is undefined");
            return fx.InvokeFunction(a.OverrideThis(this));
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new JSDynamicMetaData(parameter, this);
        }

        public JSValue Power(JSValue a) {
            var v = this.DoubleValue;
            var a1 = a.DoubleValue;
            if (a1 == 0)
                return JSNumber.One;
            if (a1 == Double.PositiveInfinity || a1 == Double.NegativeInfinity)
            {
                if (v == 1 || v == -1)

                    return JSNumber.NaN;
            }

            return new JSNumber(Math.Pow(this.DoubleValue, a1));
        }

        internal virtual bool TryGetValue(uint i, out JSProperty value)
        {
            value = new JSProperty { };
            return false;
        }

        internal virtual bool TryGetElement(uint i, out JSValue value)
        {
            value = null;
            return false;
        }

        internal virtual void MoveElements(int start, int to)
        {

        }

        internal virtual bool TryRemove(uint i, out JSProperty p)
        {
            p = new JSProperty();
            return false;
        }

        public virtual IElementEnumerator GetElementEnumerator()
        {
            return ElementEnumerator.Empty;
        }


        private readonly struct ElementEnumerator : IElementEnumerator
        {
            public static IElementEnumerator Empty = new ElementEnumerator();


            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                value = JSUndefined.Value;
                index = 0;
                hasValue = false;
                return false;
            }

            public bool MoveNext(out JSValue value)
            {
                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNextOrDefault(out JSValue value, JSValue @default)
            {
                value = @default;
                return false;
            }
            public JSValue NextOrDefault(JSValue @default)
            {
                return @default;
            }
        }
    }
}
