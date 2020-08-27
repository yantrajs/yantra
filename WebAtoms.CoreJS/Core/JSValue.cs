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
    public abstract class JSValue: IDynamicMetaObjectProvider {

        public bool IsUndefined => this is JSUndefined;

        public bool IsNull => this is JSNull;

        public bool IsNumber => this is JSNumber;

        public bool IsObject => this is JSObject;

        public bool IsArray => this is JSArray;

        public bool IsString => this is JSString;

        public bool IsBoolean => this is JSBoolean;

        public bool IsFunction => this is JSFunction;

        public virtual int Length {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public virtual double DoubleValue { get => throw new NotImplementedException(); }

        public bool BooleanValue => JSBoolean.IsTrue(this);

        public JSValue TypeOf
        {
            get
            {
                switch(this)
                {
                    case JSUndefined u:
                        return JSConstants.Undefined;
                    case JSNumber a:
                        return JSConstants.Number;
                    case JSFunction f:
                        return JSConstants.Function;
                    case JSBoolean b:
                        return JSConstants.Boolean;
                    case JSString s:
                        return JSConstants.String;
                }
                return JSConstants.Object;
            }
        }

        public virtual int IntValue { get => throw new NotImplementedException(); }

        internal BinaryUInt32Map<JSProperty> ownProperties;

        internal JSValue prototypeChain;

        /// <summary>
        /// Speed improvements for string contact operations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract JSValue Add(JSValue value);
        /// <summary>
        /// Speed improvements for string contact operations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract JSValue Add(double value);
        /// <summary>
        /// Speed improvements for string contact operations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract JSValue Add(string value);

        

        protected JSValue(JSValue prototype)
        {
            this.prototypeChain = prototype;
        }

        internal virtual JSProperty GetInternalProperty(KeyString key, bool inherited = true)
        {
            if(ownProperties != null && ownProperties.TryGetValue(key.Key, out var r))
            {
                return r;
            }
            if(inherited && prototypeChain != null)
                return prototypeChain.GetInternalProperty(key, inherited);
            return new JSProperty();
        }

        public IEnumerable<KeyValuePair<string,JSValue>> Entries
        {
            get
            {
                if (ownProperties == null)
                    yield break;
                foreach(var p in this.ownProperties.AllValues())
                {
                    if (!p.Value.IsEnumerable)
                        continue;
                    yield return new KeyValuePair<string, JSValue>(p.Value.key.ToString(), this.GetValue(p.Value));
                }
            }
        }

        public JSValue this[KeyString name]
        {
            get
            {
                if (this is JSUndefined) {
                    throw JSContext.Current.TypeError($"Unable to get {name} of undefined");
                }
                if (this is JSNull)
                {
                    throw JSContext.Current.TypeError($"Unable to get {name} of null");
                }
                var p = GetInternalProperty(name);
                if (p.IsEmpty) return JSUndefined.Value;
                return p.IsValue ? p.value : p.get.InvokeFunction(this, JSArguments.Empty);
            }
            set
            {
                if (this is JSUndefined)
                {
                    throw JSContext.Current.TypeError($"Unable to set {name} of undefined");
                }
                if (this is JSNull)
                {
                    throw JSContext.Current.TypeError($"Unable to set {name} of null");
                }
                var p = GetInternalProperty(name);
                if (p.IsEmpty)
                {
                    p = JSProperty.Property(value, JSPropertyAttributes.Value | JSPropertyAttributes.Enumerable | JSPropertyAttributes.Configurable);
                    p.key = name;
                    ownProperties[name.Key] = p;
                    return;
                }
                if (!p.IsValue && p.set != null)
                {
                    p.set.InvokeFunction(this, JSArguments.From(value));
                }else
                {
                    p.value = value;
                    ownProperties[name.Key] = p;
                }
            }
        }

        public virtual JSValue this[JSValue key]
        {
            get
            {
                if (this is JSUndefined)
                {
                    throw JSContext.Current.TypeError($"Unable to get {key} of undefined");
                }
                if (this is JSNull)
                {
                    throw JSContext.Current.TypeError($"Unable to get {key} of null");
                }

                JSProperty p = new JSProperty();
                if (key is JSString j)
                    p = GetInternalProperty(j, true);
                else if (key is JSNumber)
                    return this[(uint)key.IntValue];
                if (p.IsEmpty)
                    return JSUndefined.Value;
                if (p.IsValue)
                    return p.value;
                return p.get.InvokeFunction(this, JSArguments.Empty);
            }
            set
            {
                if (this is JSUndefined)
                {
                    throw JSContext.Current.TypeError($"Unable to set {key} of undefined");
                }
                if (this is JSNull)
                {
                    throw JSContext.Current.TypeError($"Unable to set {key} of null");
                }

                JSProperty p = new JSProperty();
                if (key is JSString j)
                    p = GetInternalProperty(j, true);
                else if (key is JSNumber)
                {
                    this[(uint)key.IntValue] = value;
                    return;
                }
                if (p.IsEmpty) {
                    // create one..
                    var kjs = KeyStrings.GetOrCreate(key.ToString());
                    var px = JSProperty.Property(
                        value,
                        JSPropertyAttributes.Value | JSPropertyAttributes.Enumerable | JSPropertyAttributes.Configurable);
                    px.key = kjs;
                    ownProperties[kjs.Key] = px;
                    return;
                }
                if (!p.IsValue && p.set != null)
                {
                    p.set.InvokeFunction(this, JSArguments.From(value));
                }else
                {
                    p.value = value;
                    ownProperties[p.key.Key] = p;
                }
            }
        }

        public JSValue Delete(JSValue key)
        {
            if (this is JSUndefined)
            {
                throw JSContext.Current.TypeError($"Unable to set {key} of undefined");
            }
            if (this is JSNull)
            {
                throw JSContext.Current.TypeError($"Unable to set {key} of null");
            }

            // return true if property was deleted successfully... 

            // or false..

            return JSContext.Current.False;
        }

        public abstract JSBoolean Equals(JSValue value);

        public abstract JSBoolean StrictEquals(JSValue value);

        internal JSValue InternalDelete(object keyExp)
        {
            if (this is JSUndefined)
            {
                throw JSContext.Current.TypeError($"Unable to set {keyExp} of undefined");
            }
            if (this is JSNull)
            {
                throw JSContext.Current.TypeError($"Unable to set {keyExp} of null");
            }
            uint key = 0;
            switch(keyExp)
            {
                case KeyString ks:
                    key = ks.Key;
                    break;
                case JSString jsString:
                    key = KeyStrings.GetOrCreate(jsString.value).Key;
                    break;
                case string s:
                    key = KeyStrings.GetOrCreate(s).Key;
                    break;
                case JSValue v:
                    key = KeyStrings.GetOrCreate(v.ToString()).Key;
                    break;
            }
            if (ownProperties.RemoveAt(key))
                return JSContext.Current.True;
            return JSContext.Current.False;
        }


        public virtual JSValue this[uint key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual JSValue CreateInstance(JSArray args)
        {
            throw new NotImplementedException();
        }

        public virtual JSValue InvokeFunction(JSValue thisValue, JSArray args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Warning do not use in concatenation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var fx = this[KeyStrings.toString];
            if (fx.IsUndefined)
                return "undefined";
            return fx.InvokeFunction(this, JSArguments.Empty).ToString();
        }

        public virtual string ToDetailString()
        {
            return (InvokeMethod(KeyStrings.toString, JSArguments.Empty) as JSString)?.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual JSValue InvokeMethod(KeyString name, JSArray args)
        {
            var fx = this[name];
            if (fx.IsUndefined)
                throw new MethodAccessException();
            return fx.InvokeFunction(this, args);
        }
        public virtual JSValue InvokeMethod(JSString name, JSArray args)
        {
            var fx = this[name.KeyString];
            if (fx.IsUndefined)
                throw new InvalidOperationException();
            return fx.InvokeFunction(this, args);
        }

        internal JSValue InternalInvoke(object name, JSArray args)
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
                throw JSContext.Current.TypeError($"Cannot invoke {name} of object as it is undefined");
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

        public static object __InvokeMember(JSValue target, string name, JSArray a)
        {
            if (name == "ToString")
                return target.ToString();
            return target.InvokeMethod(name, a);
        }

        public static JSArguments __CreateArguments(object[] args)
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
                        return b ? JSContext.Current.True : JSContext.Current.False;
                    case string s:
                        return new JSString(s);
                    case JSValue v:
                        return v;
                    default:
                        throw new NotSupportedException($"Cannot convert type {p.GetType()} to JSValue");
                }
            }).ToList();
            return new JSArguments(alist.ToArray());
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
                case JSString js: return value[js.KeyString];
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
                case JSString js: return target[js.KeyString] = value;
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
