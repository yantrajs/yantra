using System;
using System.Collections.Generic;
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

        public virtual int Length {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public virtual double DoubleValue { get => throw new NotImplementedException(); }

        public virtual int IntValue { get => throw new NotImplementedException(); }

        internal BinaryUInt32Map<JSProperty> ownProperties;

        internal JSValue prototypeChain;

        internal JSProperty GetInternalProperty(JSString key, bool inherited = true)
        {
            if (key.Key == 0)
            {
                key = KeyStrings.GetOrCreate(key.ToString());
            }
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
                    if (p.Value.value != null)
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
                if (p.value != null)
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
                    ownProperties[kjs.Key] = new JSProperty {
                        key = kjs,
                        value = value
                    };
                    return;
                }
                p.set.InvokeFunction(this, JSArguments.From(value));
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
        public virtual JSValue InvokeMethod(string name, JSArray args)
        {
            return InvokeMethod(KeyStrings.GetOrCreate(name), args);
        }
        public virtual JSValue InvokeMethod(JSString name, JSArray args)
        {
            var fx = this[name];
            if (fx.IsUndefined)
                throw new InvalidOperationException();
            return fx.InvokeFunction(this, args);
        }

        [JSExport("__proto__", JSPropertyType.Get)]
        public static JSValue GetPrototypeChain(JSValue t, JSArray a) => t.prototypeChain;

        [JSExport("__proto__", JSPropertyType.Set)]
        public static JSValue SetPrototypeChain(JSValue t, JSArray a) => t.prototypeChain = a[0];
    }



}
