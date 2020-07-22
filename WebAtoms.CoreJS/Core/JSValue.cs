using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace WebAtoms.CoreJS.Core {
    public abstract class JSValue {

        public bool IsUndefined => this is JSUndefined;

        public bool IsNull => this is JSNull;

        public bool IsNumber => this is JSNumber;

        public bool IsObject => this is JSObject;

        public bool IsArray => this is JSArray;

        public bool IsString => this is JSString;

        public bool IsBoolean => this is JSBoolean;

        public virtual int Length {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public virtual double DoubleValue { get => throw new NotImplementedException(); }

        public virtual int IntValue { get => throw new NotImplementedException(); }

        internal BinaryUInt32Map<JSProperty> ownProperties;

        internal JSValue prototypeChain;

        protected JSValue(JSValue prototype)
        {
            this.prototypeChain = prototype;
        }

        internal JSProperty GetInternalProperty(KeyString key, bool inherited = true)
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
                    if (p.Value.IsValue)
                    {
                        yield return new KeyValuePair<string, JSValue>(p.Value.key.ToString(), p.Value.value);
                        continue;
                    }
                    if (p.Value.get != null)
                    {
                        var g = p.Value.get;
                        g = g.InvokeFunction(this, JSArguments.Empty);
                        yield return new KeyValuePair<string, JSValue>(p.Value.key.ToString(), g);
                        continue;
                    }
                }
            }
        }

        public JSValue this[JSName name]
        {
            get
            {
                var p = GetInternalProperty(name.Key);
                if (p.IsEmpty) return JSUndefined.Value;
                return p.IsValue ? p.value : p.get.InvokeFunction(this, JSArguments.Empty);
            }
            set
            {
                var p = GetInternalProperty(name.Key);
                if (p.IsEmpty)
                {
                    p = JSProperty.Property(value, JSPropertyAttributes.Property | JSPropertyAttributes.Enumerable | JSPropertyAttributes.Configurable);
                    p.key = name;
                    ownProperties[name.Key.Key] = p;
                    return;
                }
                if (!p.IsValue && p.set != null)
                {
                    p.set.InvokeFunction(this, JSArguments.From(value));
                }else
                {
                    p.value = value;
                    ownProperties[name.Key.Key] = p;
                }
            }
        }

        public virtual JSValue this[JSValue key]
        {
            get
            {
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
                    ownProperties[p.key.Key.Key] = p;
                }
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual JSValue InvokeMethod(JSName name, JSArray args)
        {
            var fx = this[name];
            if (fx.IsUndefined)
                throw new MethodAccessException();
            return fx.InvokeFunction(this, args);
        }
        public virtual JSValue InvokeMethod(JSString name, JSArray args)
        {
            var fx = this[name];
            if (fx.IsUndefined)
                throw new InvalidOperationException();
            return fx.InvokeFunction(this, args);
        }
    }



}
