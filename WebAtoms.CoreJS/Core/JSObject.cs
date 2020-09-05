using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public class PropertySequence
    {

        private BinaryUInt32Map<int> map = new BinaryUInt32Map<int>();
        private List<JSProperty> properties = new List<JSProperty>();

        public IEnumerable<(uint Key,JSProperty Value)> AllValues()
        {
            foreach(var p in properties)
            {
                if (p.Attributes != JSPropertyAttributes.Deleted)
                    yield return (p.key.Key, p);
            }
        }
        public void Update(Func<uint, JSProperty, (bool update, JSProperty v)> func)
        {
            int i = 0;
            foreach(var p in properties.ToList())
            {
                var update = func((p.key.Key), p);
                if (update.update)
                {
                    properties[i] = update.v;
                }
                i++;
            }
        }
        public bool RemoveAt(uint key)
        {
            if(map.TryGetValue(key, out var pkey))
            {
                // move all properties up...
                properties[pkey] = new JSProperty { Attributes = JSPropertyAttributes.Deleted };
            }
            return false;
        }
        public bool TryGetValue(uint key, out JSProperty obj)
        {
            if(map.TryGetValue(key, out var pkey))
            {
                obj = properties[pkey];
                return obj.Attributes != JSPropertyAttributes.Deleted;
            }
            obj = new JSProperty();
            return false;
        }

        public JSProperty this [uint key]
        {
            get
            {
                if(map.TryGetValue(key, out var pkey))
                {
                    return properties[pkey];
                }
                return new JSProperty();
            }
            set { 
                if(map.TryGetValue(key,out var pkey))
                {
                    properties[pkey] = value;
                    return;
                }
                pkey = properties.Count;
                map[key] = pkey;
                properties.Add(value);
            }
        }

    }

    public partial class JSObject : JSValue
    {
        public static readonly KeyString KeyToJSON = "toJSON";

        protected JSObject(JSValue prototype) : base(prototype)
        {
        }

        public JSObject() : this(JSContext.Current?.ObjectPrototype)
        {
            
        }

        public JSObject(params JSProperty[] entries) : this(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new PropertySequence();
            foreach (var p in entries)
            {
                ownProperties[p.key.Key] = p;
            }
        }

        public JSObject(IEnumerable<JSProperty> entries) : this(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new PropertySequence();
            foreach (var p in entries)
            {
                ownProperties[p.key.Key] = p;
            }
        }

        internal BinaryUInt32Map<JSProperty> elements;
        internal PropertySequence ownProperties;


        public JSValue DefineProperty(KeyString name, JSProperty p)
        {
            var key = name.Key;
            var ownProperties = this.ownProperties ?? (this.ownProperties = new PropertySequence());
            var old = ownProperties[key];
            if (!old.IsEmpty)
            {
                if (!old.IsConfigurable)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            p.key = name;
            ownProperties[key] = p;
            return JSUndefined.Value;
        }

        public void DefineProperties(params JSProperty[] list)
        {
            var ownProperties = this.ownProperties ?? (this.ownProperties = new PropertySequence());
            foreach (var p in list)
            {
                var key = p.key.Key;
                var old = ownProperties[key];
                if (!old.IsEmpty)
                {
                    if (!old.IsConfigurable)
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                ownProperties[key] = p;
            }
        }


        public override string ToDetailString()
        {
            var all = this.GetAllEntries(false).Select((e) => $"{e.Key}: {e.Value.ToDetailString()}");
            return $"{{ {string.Join(", ", all)} }}";
        }

        public override double DoubleValue{
            get {
                var fx = this[KeyStrings.valueOf];
                if (fx is JSUndefined)
                    return NumberParser.CoerceToNumber(this.ToString());
                var v = fx.InvokeFunction(this, JSArguments.Empty);
                return v.DoubleValue;
            }
        }

        private static void InternalAddProperty(JSObject target, uint key, JSValue pd)
        {
            var p = new JSProperty();
            var value = pd[KeyStrings.value];
            var get = pd[KeyStrings.get] as JSFunction;
            var set = pd[KeyStrings.set] as JSFunction;
            var pt = JSPropertyAttributes.Empty;
            if (pd[KeyStrings.configurable].BooleanValue)
                pt |= JSPropertyAttributes.Configurable;
            if (pd[KeyStrings.enumerable].BooleanValue)
                pt |= JSPropertyAttributes.Enumerable;
            if (pd[KeyStrings.@readonly].BooleanValue)
                pt |= JSPropertyAttributes.Readonly;
            if (get != null)
            {
                pt |= JSPropertyAttributes.Property;
                p.get = get;
            }
            if (set != null)
            {
                pt |= JSPropertyAttributes.Property;
                p.set = set;
            }
            if (get == null && set == null)
            {
                pt |= JSPropertyAttributes.Value;
                p.value = value;
            }
            p.Attributes = pt;
            var elements = target.elements ?? (target.elements = new BinaryUInt32Map<JSProperty>());
            elements[key] = p;
            if (target is JSArray array)
            {
                if (array._length <= key)
                    array._length = key + 1;
            }
        }

        private static void InternalAddProperty(JSObject target, KeyString key, JSValue pd)
        {
            var p = new JSProperty
            {
                key = key
            };
            var value = pd[KeyStrings.value];
            var get = pd[KeyStrings.get] as JSFunction;
            var set = pd[KeyStrings.set] as JSFunction;
            var pt = JSPropertyAttributes.Empty;
            if (pd[KeyStrings.configurable].BooleanValue)
                pt |= JSPropertyAttributes.Configurable;
            if (pd[KeyStrings.enumerable].BooleanValue)
                pt |= JSPropertyAttributes.Enumerable;
            if (pd[KeyStrings.@readonly].BooleanValue)
                pt |= JSPropertyAttributes.Readonly;
            if (get != null)
            {
                pt |= JSPropertyAttributes.Property;
                p.get = get;
            }
            if (set != null)
            {
                pt |= JSPropertyAttributes.Property;
                p.set = set;
            }
            if (get == null && set == null)
            {
                pt |= JSPropertyAttributes.Value;
                p.value = value;
            }
            p.Attributes = pt;
            var ownProperties = target.ownProperties ?? (target.ownProperties = new PropertySequence());
            ownProperties[p.key.Key] = p;
        }


        public override JSValue AddValue(JSValue value)
        {
            return new JSString(this.ToString() + value.ToString());
        }

        public override JSValue AddValue(double value)
        {
            return new JSString(this.ToString() + value.ToString());
        }

        public override JSValue AddValue(string value)
        {
            return new JSString(this.ToString() + value);
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            if (value is JSString str)
                if (this.ToString() == str.value)
                    return JSContext.Current.True;
            if (DoubleValue == value.DoubleValue)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw new NotImplementedException("object is not a function");
        }

        internal override JSBoolean Less(JSValue value)
        {
            switch(value)
            {
                case JSString strValue:
                    if (this.ToString().CompareTo(strValue.value) < 0)
                        return JSContext.Current.True;
                    break;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean LessOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) <= 0):
                        return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean Greater(JSValue value)
        {
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) > 0):
                        return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean GreaterOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) >= 0):
                        return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }
    }
}
