using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms;
using YantraJS.Core.Clr;

namespace YantraJS.Core
{

    public static class JSValueExt
    {
        public static JSValue ToJSValue(this IJSValue value)
        {
            return value == null ? JSNull.Value : value as JSValue;
        }
    }

    public partial class JSValue : IJSValue
    {
        IJSValue IJSValue.this[string name]
        {
            get => this[name];
            set => this[name] = value.ToJSValue();
        }
        IJSValue IJSValue.this[IJSValue keyOrSymbol]
        {
            get =>
                this[keyOrSymbol as JSValue];
            set =>
                this[keyOrSymbol as JSValue] = value.ToJSValue();
        }
        IJSValue IJSValue.this[int name]
        {
            get => this[(uint)name];
            set => this[(uint)name] = value.ToJSValue();
        }


        public IJSContext Context => JSContext.Current;

        public bool IsValueNull => this.IsNull;

        public bool IsDate => this is JSDate;

        public bool IsWrapped => this is ClrProxy;

        public long LongValue => this.BigIntValue;

        public float FloatValue => (float)this.DoubleValue;

        public DateTime DateValue => (this as JSDate).value.LocalDateTime;

        public IEnumerable<WebAtoms.JSProperty> Entries
        {
            get
            {
                foreach (var (Key, Value) in this.GetAllEntries(false))
                {
                    yield return new WebAtoms.JSProperty(Key.ToString(), Value);
                }
            }
        }

        public string DebugView => this.ToDetailString();

        public IJSValue CreateNewInstance(params IJSValue[] args)
        {
            var a = new Arguments(args);
            return this.CreateInstance(a);
        }

        public void DefineProperty(string name, JSPropertyDescriptor pd)
        {
            JSFunction pget = null;
            JSFunction pset = null;
            JSValue pvalue = null;
            var value = pd.Value as JSValue;
            var get = pd.Get as JSFunction;
            var set = pd.Set as JSFunction;
            var pt = JSPropertyAttributes.Empty;
            if (pd.Configurable ?? false)
                pt |= JSPropertyAttributes.Configurable;
            if (pd.Enumerable ?? false)
                pt |= JSPropertyAttributes.Enumerable;
            if (!pd.Writable ?? false)
                pt |= JSPropertyAttributes.Readonly;
            if (get != null)
            {
                pt |= JSPropertyAttributes.Property;
                pget = get;
            }
            if (set != null)
            {
                pt |= JSPropertyAttributes.Property;
                pset = set;
            }
            if (get == null && set == null)
            {
                pt |= JSPropertyAttributes.Value;
                pvalue = value;
                pget = value as JSFunction;
            }
            var pAttributes = pt;
            var target = this as JSObject;
            ref var ownProperties = ref target.GetOwnProperties();
            KeyString key = name;
            ownProperties[key.Key] = new JSProperty(key, pget, pset, pvalue, pAttributes);
        }

        public bool DeleteProperty(string name)
        {
            return this.Delete(name).BooleanValue;
        }

        public bool HasProperty(string name)
        {
            if (!(this is JSObject @object))
                return false;
            ref var ownProperties = ref @object.GetOwnProperties();
            KeyString key = name;
            return ownProperties.HasKey(key.Key);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, params IJSValue[] args)
        {
            var a = new Arguments(thisValue ?? JSUndefined.Value, args);
            return InvokeFunction(a);
        }

        public IJSValue InvokeMethod(string name, params IJSValue[] args)
        {
            var fx = GetMethod(name);
            var a = new Arguments(this, args);
            return fx(a);
        }

        public IList<IJSValue> ToArray()
        {
            return new AtomEnumerable(this);
        }

        public T Unwrap<T>()
        {
            return (T)(this as ClrProxy).value;
        }

        bool IJSValue.InstanceOf(IJSValue jsClass)
        {
            return this.InstanceOf(jsClass.ToJSValue()).BooleanValue;
        }
    }
}
