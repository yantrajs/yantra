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

        internal virtual IEnumerable<JSValue> GetAllKeys(bool showEnumerableOnly = true)
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

    internal class JSDynamicMetaData: DynamicMetaObject
    {
        internal static MethodInfo _createArguments =
            typeof(JSDynamicMetaData).GetMethod("__CreateArguments");

        internal static MethodInfo _invokeMember =
            typeof(JSDynamicMetaData).GetMethod("__InvokeMember");

        internal static MethodInfo _setMethod =
            typeof(JSDynamicMetaData).GetMethod("__SetMethod");

        internal static MethodInfo _getMethod =
            typeof(JSDynamicMetaData).GetMethod("__GetMethod");

        public static object __InvokeMember(JSValue target, string name,params JSValue[] a)
        {
            if (name == "ToString")
                return target.ToString();
            return target.InvokeMethod(name, a);
        }

        public static JSValue[] __CreateArguments(object[] args)
        {
            var alist = args.Select((p) => {
                if (p == null)
                    return JSNull.Value;
                switch(p)
                {
                    case double d:
                        return new JSNumber(d);
                    case int i:
                        return new JSNumber(i);
                    case float f:
                        return new JSNumber(f);
                    case decimal ds:
                        return new JSNumber((double)ds);
                    case bool b:
                        return b ? JSBoolean.True : JSBoolean.False;
                    case string s:
                        return new JSString(s);
                    case JSValue v:
                        return v;
                    default:
                        throw new NotSupportedException($"Cannot convert type {p.GetType()} to JSValue");
                }
            }).ToList();
            return alist.ToArray();
        }

        public static object __GetMethod(JSValue value, object name)
        {
            if (name == null)
                throw new ArgumentNullException();
            switch(name)
            {
                case string s: return value[s];
                case uint ui: return value[ui];
                case int i: return value[(uint)i];
                case double d: return value[(uint)d];
                case decimal d1: return value[(uint)d1];
                case float f1: return value[(uint)f1];
                case JSNumber jn: return value[(uint)jn.value];
                case JSString js: return value[js.ToKey()];
            }
            return value[name.ToString()];
        }

        public static JSValue __SetMethod(JSValue target, object name, object _value)
        {
            if (name == null)
                throw new ArgumentNullException();
            JSValue value = TypeConverter.FromBasic(_value);
            switch (name)
            {
                case string s: return target[s] = value;
                case uint ui: return target[ui] = value;
                case int i: return target[(uint)i] = value;
                case double d: return target[(uint)d] = value;
                case decimal d1: return target[(uint)d1] = value;
                case float f1: return target[(uint)f1] = value;
                case JSNumber jn: return target[(uint)jn.value] = value;
                case JSString js: return target[js.ToKey()] = value;
            }
            return target[name.ToString()] = value;
        }


        internal JSDynamicMetaData(
            System.Linq.Expressions.Expression parameter,
            JSValue value) : base(parameter, BindingRestrictions.Empty, value)
        {
            
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            BindingRestrictions restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            Expression self = Expression.Convert(Expression, LimitType);
            Expression p0 = Expression.Convert(value.Expression, typeof(object));
            Expression name = Expression.Constant(binder.Name);
            Expression call = Expression.Call(_setMethod, self, name, p0);

            return new DynamicMetaObject(call, restrictions);
            
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            BindingRestrictions restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            Expression self = Expression.Convert(Expression, LimitType);
            Expression name = Expression.Constant(binder.Name);
            Expression call = Expression.Call(_getMethod, self, name);

            return new DynamicMetaObject(call, restrictions);
        }

        public override DynamicMetaObject BindSetIndex(
            SetIndexBinder binder, 
            DynamicMetaObject[] indexes, 
            DynamicMetaObject value)
        {
            BindingRestrictions restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            Expression self = Expression.Convert(Expression, LimitType);
            Expression p0 = Expression.Convert(value.Expression, typeof(object));
            Expression name = Expression.Convert(indexes[0].Expression, typeof(object));
            Expression call = Expression.Call(_setMethod, self, name, p0);

            return new DynamicMetaObject(call, restrictions);
        }

        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            BindingRestrictions restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            Expression self = Expression.Convert(Expression, LimitType);
            Expression name = Expression.Convert(indexes[0].Expression, typeof(object));
            Expression call = Expression.Call(_getMethod, self, name);

            return new DynamicMetaObject(call, restrictions);
        }

        public override DynamicMetaObject BindInvokeMember(
            InvokeMemberBinder binder, 
            DynamicMetaObject[] args)
        {

            BindingRestrictions restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            var argList = Expression.NewArrayInit(typeof(object),
                args.Select(x => Expression.Convert(x.Expression, typeof(object))).ToArray());

            var ce = Expression.Call(_createArguments, argList);

            Expression self = Expression.Convert(Expression, LimitType);
            Expression name = Expression.Constant(binder.Name);
            Expression call = Expression.Call(_invokeMember, self, name, ce);

            return new DynamicMetaObject(call, restrictions);
        }
    }



}
