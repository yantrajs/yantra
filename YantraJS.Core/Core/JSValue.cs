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

        public virtual bool IsArray => false;

        public virtual bool IsString => false;

        public virtual bool IsBoolean => false;

        public virtual bool IsFunction => false;

        internal virtual bool IsSpread => false;

        internal object Convert(Type type, object def)
        {
            if (type.IsAssignableFrom(typeof(JSValue)))
                return this;
            if (ConvertTo(type, out var v))
                return v;
            return def;
        }

        public object ForceConvert(Type type) { 
            if (type.IsAssignableFrom(typeof(JSValue)))
            {
                return this;
            }
            if (ConvertTo(type, out var value))
                return value;
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

        public virtual int IntValue => (int)(uint)this.DoubleValue;

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

        internal JSPrototype prototypeChain;

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

        /// <summary>
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
        public JSValue AddValue(double value)
        {
            if (this.CanBeNumber)
                return new JSNumber(this.DoubleValue + value);
            return new JSString(this.ToString() + value.ToString());
        }
        /// <summary>
        /// Speed improvements for string contact operations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public JSValue AddValue(string value)
        {
            if (value.Length == 0)
                return this.IsString ? this : new JSString(this.ToString());
            return new JSString(this.ToString() + value);
        }



        protected JSValue(JSObject prototype)
        {
            this.BasePrototypeObject = prototype;
        }

        internal abstract KeyString ToKey(bool create = true);

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
                return GetOwnProperty(key.Key);
            }
            return GetOwnProperty(in key);
        }

        public JSValue PropertyOrUndefined(in KeyString name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            return GetOwnProperty(in name);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue PropertyOrUndefined(JSObject super, in KeyString name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            var pc = prototypeChain;
            if (pc == null)
                return JSUndefined.Value;
            return this.GetValue(super.GetInternalProperty(name));
        }

        public JSValue PropertyOrUndefined(uint name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            return GetOwnProperty(name);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue PropertyOrUndefined(JSObject super, uint name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            var pc = this.prototypeChain;
            if (pc == null)
                return JSUndefined.Value;
            return this.GetValue(super.GetInternalProperty(name));
        }

        public JSValue PropertyOrUndefined(JSSymbol name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            return this.GetOwnProperty(name);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue PropertyOrUndefined(JSObject super, JSSymbol name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            var pc = prototypeChain;
            if (pc == null)
                return JSUndefined.Value;
            return this.GetValue(super.GetInternalProperty(name));
        }

        public JSValue PropertyOrUndefined(JSValue name)
        {
            if (this == JSNull.Value || this == JSUndefined.Value)
                return JSUndefined.Value;
            if (name is JSSymbol s)
                return PropertyOrUndefined(s);
            var k = name.ToKey(false);
            if (k.IsUInt)
                return PropertyOrUndefined(k.Key);
            return PropertyOrUndefined(k);
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
                return PropertyOrUndefined(k.Key);
            return PropertyOrUndefined(k);
        }

        public virtual JSValue this[KeyString name]
        {
            get
            {
                if (prototypeChain == null)
                    return JSUndefined.Value;
                return this.GetValue(prototypeChain.GetInternalProperty(name));
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
                if (prototypeChain == null)
                    return JSUndefined.Value;
                return this.GetValue(prototypeChain.GetInternalProperty(key));
            }
            set { }
        }

        public virtual JSValue this[JSSymbol symbol]
        {
            get
            {
                if (prototypeChain == null)
                    return JSUndefined.Value;
                return this.GetValue(prototypeChain.GetInternalProperty(symbol));
            }
            set { }
        }

        public JSValue this[JSValue key]
        {
            get
            {
                if (key is JSSymbol symbol)
                    return this[symbol];
                var k = key.ToKey();
                return k.IsUInt ? this[k.Key] : this[k];
            }
            set
            {
                if (key is JSSymbol symbol)
                {
                    this[symbol] = value;
                    return;
                }
                var k = key.ToKey();
                if (k.IsUInt)
                {
                    this[k.Key] = value;
                }
                else
                {
                    this[k] = value;
                }
            }
        }

        internal virtual JSValue this[KeyString name, JSValue @this]
        {
            get
            {
                if (prototypeChain == null)
                    return JSUndefined.Value;
                return @this.GetValue(prototypeChain.GetInternalProperty(name));
            }
            set { }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue this[JSObject super, KeyString name]
        {
            get => this.GetValue(super.GetInternalProperty(name));
            set
            {
                var p = super.GetInternalProperty(name);
                if (p.IsProperty)
                {
                    if (p.set != null)
                    {
                        p.set.f(new Arguments(this, value));
                    }
                    return;
                }
                throw JSContext.Current.NewTypeError($"{name} accessor not found on super");
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue this[JSObject super, uint index]
        {
            get => this.GetValue(super.GetInternalProperty(index));
            set
            {
                var p = super.GetInternalProperty(index);
                if (p.IsProperty)
                {
                    if (p.set != null)
                    {
                        p.set.f(new Arguments(this, value));
                    }
                    return;
                }
                throw JSContext.Current.NewTypeError($"{index} accessor not found on super");
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSValue this[JSObject super, JSValue name]
        {
            get
            {
                if (name is JSSymbol symbol)
                    return this[super, symbol];
                var key = name.ToKey();
                if (key.IsUInt)
                    return this[super, key.Key];
                return this[super, key];
            }
            set {
                if (name is JSSymbol symbol)
                    this[super, symbol] = value;
                var key = name.ToKey();
                if (key.IsUInt)
                {
                    this[super, key.Key] = value;
                    return;
                } 
                this[super, key] = value;
            }
        }


        public abstract JSBoolean Equals(JSValue value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool StaticEquals(JSValue left, JSValue right)
        {
            return left.Equals(right).BooleanValue;
        }

        public abstract JSBoolean StrictEquals(JSValue value);

        /// <summary>
        /// 1. NaN is considered equal to NaN.
        /// 2. +0 and -0 are considered to be equal.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual JSBoolean SameValueZero(JSValue value) {
            return this.StrictEquals(value);
        }

        public virtual JSBoolean Less(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined))
            {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue < value.DoubleValue)
                        return JSBoolean.True;
                }
                else if (this.ToString().CompareTo(value.ToString()) < 0)
                    return JSBoolean.True;
            }
            return JSBoolean.False;

        }
        public virtual JSBoolean LessOrEqual(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined))
            {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue <= value.DoubleValue)
                        return JSBoolean.True;
                }
                else if (this.ToString().CompareTo(value.ToString()) <= 0)
                    return JSBoolean.True;
            }
            return JSBoolean.False;

        }

        public virtual JSBoolean Greater(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined))
            {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue > value.DoubleValue)
                        return JSBoolean.True;
                }
                else if (this.ToString().CompareTo(value.ToString()) > 0)
                    return JSBoolean.True;
            }
            return JSBoolean.False;

        }
        public virtual JSBoolean GreaterOrEqual(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined)) {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue >= value.DoubleValue)
                        return JSBoolean.True;
                }
                else if (this.ToString().CompareTo(value.ToString()) >= 0)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
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

        public JSValue Delete(JSValue index)
        {
            var key = index.ToKey();
            if (key.IsUInt)
                return this.Delete(key.Key);
            return Delete(key);
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
        }
    }
}
