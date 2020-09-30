using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core {
    public abstract class JSValue : IDynamicMetaObjectProvider {

        public virtual bool IsUndefined => false;

        public virtual bool IsNull => false;

        internal virtual bool IsNullOrUndefined => false;

        public virtual bool IsNumber => false;

        public virtual bool IsObject => false;

        public virtual bool IsArray => false;

        public virtual bool IsString => false;

        public virtual bool IsBoolean => false;

        public virtual bool IsFunction => false;

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

        public virtual int IntValue => (int)this.DoubleValue;

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


        public JSValue this[JSValue key]
        {
            get
            {
                var k = key.ToKey();
                return k.IsUInt ? this[k.Key] : this[k];
            }
            set
            {
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

        internal virtual IEnumerable<JSValue> GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            yield break;
        }


        public virtual JSValue CreateInstance(in Arguments a)
        {
            throw new NotImplementedException();
        }

        public abstract JSValue InvokeFunction(in Arguments a);

        /// <summary>
        /// Warning do not use in concatenation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // use inherited version..
            throw new NotSupportedException($"Use inherited version ... ");
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

        public virtual string ToDetailString()
        {
            return (InvokeMethod(KeyStrings.toString, Arguments.Empty) as JSString)?.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual JSValue InvokeMethod(KeyString name, in Arguments a)
        {
            var fx = this[name];
            if (fx.IsUndefined)
                throw new JSException($"Method {name} not found on {this}");
            return fx.InvokeFunction(a.OverrideThis(this));
        }

        public JSValue InvokeMethod(uint name, in Arguments a)
        {
            var fx = this[name];
            if (fx.IsUndefined)
                throw new JSException($"Method {name} not found on {this}");
            return fx.InvokeFunction(a.OverrideThis(this));
        }

        public JSValue InvokeMethod(JSValue name, in Arguments a)
        {
            var key = name.ToKey();
            if (key.IsUInt)
                return InvokeMethod(key.Key, a);
            return InvokeMethod(key, a);
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

        /// <summary>
        /// Returns Elements of an Array or an Iterable...
        /// </summary>
        internal abstract IEnumerable<(uint index,JSValue value)> AllElements { get; }


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
    }
}
