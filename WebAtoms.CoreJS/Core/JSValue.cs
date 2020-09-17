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

        public virtual int IntValue => 0;

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

        internal abstract KeyString ToKey();

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

        public abstract JSBooleanPrototype Equals(JSValue value);

        internal static bool StaticEquals(JSValue left, JSValue right)
        {
            return left.Equals(right).BooleanValue;
        }

        public abstract JSBooleanPrototype StrictEquals(JSValue value);

        internal virtual JSBooleanPrototype Less(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined))
            {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue < value.DoubleValue)
                        return JSBooleanPrototype.True;
                }
                else if (this.ToString().CompareTo(value.ToString()) < 0)
                    return JSBooleanPrototype.True;
            }
            return JSBooleanPrototype.False;

        }
        internal virtual JSBooleanPrototype LessOrEqual(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined))
            {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue <= value.DoubleValue)
                        return JSBooleanPrototype.True;
                }
                else if (this.ToString().CompareTo(value.ToString()) <= 0)
                    return JSBooleanPrototype.True;
            }
            return JSBooleanPrototype.False;

        }

        internal virtual JSBooleanPrototype Greater(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined))
            {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue > value.DoubleValue)
                        return JSBooleanPrototype.True;
                }
                else if (this.ToString().CompareTo(value.ToString()) > 0)
                    return JSBooleanPrototype.True;
            }
            return JSBooleanPrototype.False;

        }
        internal virtual JSBooleanPrototype GreaterOrEqual(JSValue value)
        {
            if (!(this.IsUndefined || value.IsUndefined)) {
                if (this.CanBeNumber || value.CanBeNumber)
                {
                    if (this.DoubleValue >= value.DoubleValue)
                        return JSBooleanPrototype.True;
                }
                else if (this.ToString().CompareTo(value.ToString()) >= 0)
                    return JSBooleanPrototype.True;
            }
            return JSBooleanPrototype.False;
        }

        internal virtual IEnumerable<JSValue> GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            yield break;
        }


        public virtual JSValue CreateInstance(JSValue[] args)
        {
            throw new NotImplementedException();
        }

        public abstract JSValue InvokeFunction(JSValue thisValue,params JSValue[] args);

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
            return (InvokeMethod(KeyStrings.toString, JSArguments.Empty) as JSString)?.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual JSValue InvokeMethod(KeyString name,params JSValue[] args)
        {
            var fx = this[name];
            if (fx.IsUndefined)
                throw new MethodAccessException($"Method {name} not found on {this}");
            return fx.InvokeFunction(this, args);
        }

        public JSValue InvokeMethod(uint name, params JSValue[] args)
        {
            var fx = this[name];
            if (fx.IsUndefined)
                throw new MethodAccessException($"Method {name} not found on {this}");
            return fx.InvokeFunction(this, args);
        }

        public JSValue InvokeMethod(JSValue name,params JSValue[] args)
        {
            var key = name.ToKey();
            if (key.IsUInt)
                return InvokeMethod(key.Key, args);
            return InvokeMethod(key, args);
        }

        public virtual JSValue Delete(KeyString key)
        {
            return JSBooleanPrototype.False;
        }
        public virtual JSValue Delete(uint key)
        {
            return JSBooleanPrototype.False;
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
        internal abstract IEnumerable<JSValue> AllElements { get; }


        internal JSValue InternalInvoke(object name,params JSValue[] args)
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
            return fx.InvokeFunction(this, args);
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new JSDynamicMetaData(parameter, this);
        }

    }
}
