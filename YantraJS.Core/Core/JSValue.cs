using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core {

    public abstract partial class JSValue : IDynamicMetaObjectProvider {

        public virtual bool IsUndefined => false;

        public virtual bool IsNull => false;

        internal virtual bool IsNullOrUndefined => false;

        public virtual bool IsNumber => false;

        public virtual bool IsObject => false;

        public virtual bool IsArray => false;

        public virtual bool IsString => false;

        public virtual bool IsBoolean => false;

        public virtual bool IsFunction => false;

        internal object Convert(Type type, object def)
        {
            if (type.IsAssignableFrom(typeof(JSValue)))
                return this;
            if (ConvertTo(type, out var v))
                return v;
            return def;
        }

        internal object ForceConvert(Type type) { 
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

        public abstract JSValue TypeOf();

        public virtual int IntValue => (int)(uint)this.DoubleValue;

        public virtual long BigIntValue => (long)(ulong)this.DoubleValue;

        internal JSObject prototypeChain;

        /// <summary>
        /// Speed improvements for string contact operations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual JSValue AddValue(JSValue value)
        {
            if (this.CanBeNumber && value.CanBeNumber)
            {
                return new JSNumber(this.DoubleValue + value.DoubleValue);
            }
            if (value.ToString().Length == 0)
                return this.IsString ? this : new JSString(this.ToString());
            return new JSString(this.ToString() + value.ToString());
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
            this.prototypeChain = prototype;
        }

        internal abstract KeyString ToKey(bool create = true);
        

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

        internal JSValue this[JSObject super, KeyString name]
        {
            get => this.GetValue(super.GetInternalProperty(name));
            set
            {
                ref var p = ref super.GetInternalProperty(name);
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

        internal JSValue this[JSObject super, uint index]
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

        internal JSValue this[JSObject super, JSValue name]
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

        internal static bool StaticEquals(JSValue left, JSValue right)
        {
            return left.Equals(right).BooleanValue;
        }

        public abstract JSBoolean StrictEquals(JSValue value);

        internal virtual JSBoolean Less(JSValue value)
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
        internal virtual JSBoolean LessOrEqual(JSValue value)
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

        internal virtual JSBoolean Greater(JSValue value)
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
        internal virtual JSBoolean GreaterOrEqual(JSValue value)
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

        internal virtual IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true) {
            return new ElementEnumerator();
        }

        internal virtual JSBoolean Is(JSValue value)
        {
            return object.ReferenceEquals(this, value) ? JSBoolean.True : JSBoolean.False;
        }


        public virtual JSValue CreateInstance(in Arguments a)
        {
            throw new NotImplementedException();
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

        public virtual JSValue Delete(KeyString key)
        {
            return JSBoolean.False;
        }
        public virtual JSValue Delete(uint key)
        {
            return JSBoolean.False;
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

        internal virtual bool TryGetValue(uint i, out JSProperty value)
        {
            value = new JSProperty { };
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

        internal virtual IElementEnumerator GetElementEnumerator()
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
        }
    }
}
