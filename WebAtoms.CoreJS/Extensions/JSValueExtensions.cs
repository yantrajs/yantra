using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Extensions
{
    public static class JSValueExtensions
    {

        //internal static IEnumerable<KeyValuePair<string, JSValue>> GetEntries(this JSValue value)
        //{
        //    if (!(value is JSObject @object))
        //        yield break;

        //    // first enumerate numeric values..

        //    var ownProperties = @object.ownProperties;
        //    if (ownProperties == null)
        //        yield break;
        //    foreach (var p in ownProperties.AllValues())
        //    {
        //        if (!p.Value.IsEnumerable)
        //            continue;
        //        yield return new KeyValuePair<string, JSValue>(p.Value.key.ToString(), value.GetValue(p.Value));
        //    }
        //}

        internal static IEnumerable<JSValue> GetAllKeys(this JSValue value, bool showEnumerableOnly = true)
        {
            if (!(value is JSObject @object))
                yield break;
            var elements = @object.elements;
            if (elements != null)
            {
                foreach (var p in elements.AllValues())
                {
                    if (showEnumerableOnly)
                    {
                        if (!p.Value.IsEnumerable)
                            continue;
                    }
                    yield return new JSNumber(p.Key);
                }
            }

            var ownProperties = @object.ownProperties;
            if (ownProperties != null)
            {
                foreach (var p in ownProperties.AllValues())
                {
                    if (showEnumerableOnly)
                    {
                        if (!p.Value.IsEnumerable)
                            continue;
                    }
                    yield return p.Value.ToJSValue();
                }
            }

            var @base = value.prototypeChain;
            if (@base != value && @base != null)
            {
                foreach (var i in @base.GetAllKeys(showEnumerableOnly))
                    yield return i;
            }
        }

        internal static IEnumerable<(JSValue Key, JSValue Value)> 
            GetAllEntries(this JSValue value, bool showEnumerableOnly = true)
        {
            if (!(value is JSObject @object))
                yield break;
            var elements = @object.elements;
            if (elements != null)
            {
                foreach (var p in elements.AllValues())
                {
                    if (showEnumerableOnly)
                    {
                        if (!p.Value.IsEnumerable)
                            continue;
                    }
                    yield return ( new JSNumber(p.Key), value.GetValue(p.Value));
                }
            }

            var ownProperties = @object.ownProperties;
            if (ownProperties != null)
            {
                foreach (var p in ownProperties.AllValues())
                {
                    if (showEnumerableOnly)
                    {
                        if (!p.Value.IsEnumerable)
                            continue;
                    }
                    yield return (p.Value.ToJSValue(), value.GetValue(p.Value));
                }
            }

            var @base = value.prototypeChain;
            if (@base != value && @base != null) {
                foreach(var bp in @base.GetAllEntries(showEnumerableOnly))
                {
                    yield return bp;
                }
            }
        }

        internal static IEnumerable<(int index, KeyString key, JSValue value)> GetOwnEntries(this JSValue value)
        {
            if (!(value is JSObject @object))
                yield break;
            var elements = @object.elements;
            if (elements != null)
            {
                foreach (var p in elements.AllValues())
                {
                    yield return ((int)p.Key, KeyString.Empty, value.GetValue(p.Value));
                }
            }

            var ownProperties = @object.ownProperties;
            if (ownProperties != null)
            {
                foreach (var p in ownProperties.AllValues())
                {
                    yield return (-1, p.Value.key, value.GetValue(p.Value));
                }
            }
        }

        internal static JSProperty GetInternalProperty(this JSValue value, KeyString key, bool inherited = true)
        {
            if (!(value is JSObject @object))
            {
                if (!inherited || value.prototypeChain == null)
                    return new JSProperty();
                return value.prototypeChain.GetInternalProperty(key, inherited);
            }
            var ownProperties = @object.ownProperties;
            if (ownProperties != null && ownProperties.TryGetValue(key.Key, out var r))
            {
                return r;
            }
            if (inherited && value.prototypeChain != null)
                return value.prototypeChain.GetInternalProperty(key, inherited);
            return new JSProperty();
        }



        public static JSValue GetProperty(this JSValue value, KeyString name)
        {
            switch (value)
            {
                case JSUndefined _:
                    throw JSContext.Current.TypeError($"Unable to get {name} of undefined");
                case JSNull __:
                    throw JSContext.Current.TypeError($"Unable to get {name} of null");
                
                    // speed improvement for Array.length
                case JSArray a
                    when name.Key == KeyStrings.length.Key:
                    return new JSNumber(a._length);
            }
            var p = value.GetInternalProperty(name);
            if (p.IsEmpty)
            {
                return JSUndefined.Value;
            }
            return p.IsValue ? p.value : p.get.InvokeFunction(value, JSArguments.Empty);
        }

        public static JSValue SetProperty(this JSValue target, KeyString name, JSValue value)
        {
            if (target is JSUndefined)
            {
                throw JSContext.Current.TypeError($"Unable to set {name} of undefined");
            }
            if (target is JSNull)
            {
                throw JSContext.Current.TypeError($"Unable to set {name} of null");
            }
            if (!(target is JSObject @object))
                return value;
            var ownProperties = @object.ownProperties ?? (@object.ownProperties = new PropertySequence());
            var p = target.GetInternalProperty(name);
            if (p.IsEmpty)
            {
                p = JSProperty.Property(value, JSPropertyAttributes.Value | JSPropertyAttributes.Enumerable | JSPropertyAttributes.Configurable);
                p.key = name;
                ownProperties[name.Key] = p;
                return value;
            }
            if (!p.IsValue && p.set != null)
            {
                p.set.InvokeFunction(target, value);
            }
            else
            {
                p.value = value;
                ownProperties[name.Key] = p;
            }
            return value;
        }

        public static JSValue GetProperty(this JSValue target, JSValue key)
        {
            string keyText = null;
            double d = double.NaN;
            switch (key)
            {
                case JSString @string:
                    keyText = @string.value;
                    d = NumberParser.CoerceToNumber(keyText);
                    break;
                case JSNumber number:
                    d = number.value;
                    break;
                case JSNull @null:
                    keyText = "null";
                    break;
                case JSUndefined @null:
                    keyText = "undefined";
                    break;
                case JSBoolean b:
                    keyText = b._value ? "true" : "false";
                    break;
                default:
                    keyText = key.ToString();
                    d = NumberParser.CoerceToNumber(keyText);
                    break;
            }
            int n = -1;
            if (!double.IsNaN(d) && d >= 0 && (d % 1) == 0)
            {
                n = (int)d;
            }
            if (n >= 0)
            {
                return GetProperty(target, (uint)n);
            }
            return GetProperty(target, keyText);
        }

        public static JSValue SetProperty(this JSValue target, JSValue key, JSValue value)
        {
            string keyText = null;
            double d = double.NaN;
            switch (key)
            {
                case JSString @string:
                    keyText = @string.value;
                    d = NumberParser.CoerceToNumber(keyText);
                    break;
                case JSNumber number:
                    d = number.value;
                    break;
                case JSNull @null:
                    keyText = "null";
                    break;
                case JSUndefined @null:
                    keyText = "undefined";
                    break;
                case JSBoolean b:
                    keyText = b._value ? "true" : "false";
                    break;
                default:
                    keyText = key.ToString();
                    d = NumberParser.CoerceToNumber(keyText);
                    break;
            }
            int n = -1;
            if (!double.IsNaN(d) && d > 0 && (d % 1) == 0)
            {
                n = (int)d;
            }
            if (n >= 0)
            {
                return SetProperty(target, (uint)n, value);
            }
            return SetProperty(target, keyText, value);

        }

        public static JSValue GetProperty(this JSValue target, uint key)
        {
            if (target is JSUndefined)
            {
                throw JSContext.Current.TypeError($"Unable to set {key} of undefined");
            }
            if (target is JSNull)
            {
                throw JSContext.Current.TypeError($"Unable to set {key} of null");
            }
            if (target is JSString @string)
            {
                var sv = @string.value;
                if (sv.Length > key)
                {
                    return new JSString(new String(sv[(int)key],1));
                }
                return JSUndefined.Value;
            }
            if (!(target is JSObject @object))
                return JSUndefined.Value;
            var elements = @object.elements;
            if (elements == null)
                return JSUndefined.Value;
            if (elements.TryGetValue(key, out var p))
                return p.value;
            return JSUndefined.Value;
        }

        public static JSValue SetProperty(this JSValue target, uint key, JSValue value)
        {
            if (target is JSUndefined)
            {
                throw JSContext.Current.TypeError($"Unable to set {key} of undefined");
            }
            if (target is JSNull)
            {
                throw JSContext.Current.TypeError($"Unable to set {key} of null");
            }
            if (!(target is JSObject @object))
                return JSUndefined.Value;
            var elements = @object.elements ?? (@object.elements = new BinaryUInt32Map<JSProperty>());
            elements.Save(key, JSProperty.Property(value));
            if (target is JSArray array)
            {
                if (array._length <= key)
                    array._length = key + 1;
            }
            return JSUndefined.Value;
        }

        public static JSValue Delete(this JSValue target, JSValue key)
        {
            if (target is JSUndefined)
            {
                throw JSContext.Current.TypeError($"Unable to set {key} of undefined");
            }
            if (target is JSNull)
            {
                throw JSContext.Current.TypeError($"Unable to set {key} of null");
            }
            if (!(target is JSObject @object))
                return JSContext.Current.False;
            var ownProperties = @object.ownProperties;

            // return true if property was deleted successfully... 
            KeyString ks;
            switch (key)
            {
                case JSString @string:
                    ks = KeyStrings.GetOrCreate(@string.value);
                    break;
                case JSNumber number:
                    ks = KeyStrings.GetOrCreate(number.value.ToString());
                    break;
                case JSSymbol symbol:
                    ks = symbol.Key;
                    break;
                default:
                    throw JSContext.Current.TypeError("not supported");
            }
            var px = target.GetInternalProperty(ks, false);
            if (px.IsEmpty)
                return JSContext.Current.False;
            // only in strict mode...
            if (!px.IsConfigurable)
                throw JSContext.Current.TypeError("Cannot delete property of sealed object");
            ownProperties.RemoveAt(ks.Key);
            return JSContext.Current.True;
        }



        public static JSBoolean InstanceOf(this JSValue target, JSValue value)
        {
            //var target = this;
            //while(target != null)
            //{
            //    target.prototypeChain 
            //}
            throw new NotImplementedException();
        }

        public static JSBoolean IsIn(this JSValue target, JSValue value)
        {
            //var target = this;
            //while(target != null)
            //{
            //    target.prototypeChain 
            //}
            throw new NotImplementedException();
        }

        public static JSValue InvokeMethod(this JSValue target, KeyString key, JSValue[] args)
        {
            var property = target.GetProperty(key);
            if (property is JSUndefined)
                throw new NotImplementedException($"Cannot invoke {key}, it is undefined");
            if (!(property is JSFunction function))
                throw new NotImplementedException($"Cannot invoke {key}, {property} is not a function");
            return function.f(target, args);
        }

        public static JSValue InvokeMethod(this JSValue target, uint key, JSValue[] args)
        {
            var property = target.GetProperty(key);
            if (property is JSUndefined)
                throw new NotImplementedException($"Cannot invoke {key}, it is undefined");
            if (!(property is JSFunction function))
                throw new NotImplementedException($"Cannot invoke {key}, {property} is not a function");
            return function.f(target, args);
        }

        public static JSValue InvokeMethod(this JSValue target, JSValue key, JSValue[] args)
        {
            var property = target.GetProperty(key);
            if (property is JSUndefined)
                throw new NotImplementedException($"Cannot invoke {key}, it is undefined");
            if (!(property is JSFunction function))
                throw new NotImplementedException($"Cannot invoke {key}, {property} is not a function");
            return function.f(target, args);
        }

        internal static JSValue Delete(this JSValue target, KeyString ks)
        {
            if (target is JSUndefined)
            {
                throw JSContext.Current.TypeError($"Unable to set {ks} of undefined");
            }
            if (target is JSNull)
            {
                throw JSContext.Current.TypeError($"Unable to set {ks} of null");
            }
            if (!(target is JSObject @object))
                return JSContext.Current.False;
            var ownProperties = @object.ownProperties;
            if (ownProperties == null)
                return JSContext.Current.False;
            var px = ownProperties[ks.Key];
            if (px.IsEmpty)
                return JSContext.Current.False;
            // only in strict mode...
            if (!px.IsConfigurable)
                throw JSContext.Current.TypeError("Cannot delete property of sealed object");
            ownProperties.RemoveAt(ks.Key);
            return JSContext.Current.True;
        }

        internal static JSValue Delete(this JSValue target, uint ks)
        {
            if (target is JSUndefined)
            {
                throw JSContext.Current.TypeError($"Unable to set {ks} of undefined");
            }
            if (target is JSNull)
            {
                throw JSContext.Current.TypeError($"Unable to set {ks} of null");
            }
            if (!(target is JSObject @object))
                return JSContext.Current.False;
            var ownProperties = @object.elements;
            if (ownProperties == null)
                return JSContext.Current.False;
            var px = ownProperties[ks];
            if (px.IsEmpty)
                return JSContext.Current.False;
            // only in strict mode...
            if (!px.IsConfigurable)
                throw JSContext.Current.TypeError("Cannot delete property of sealed object");
            ownProperties.RemoveAt(ks);
            return JSContext.Current.True;
        }
    }
}
