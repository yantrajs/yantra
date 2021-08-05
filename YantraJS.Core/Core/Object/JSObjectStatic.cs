using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS;
using YantraJS.Core.Typed;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{
    public static class JSObjectStatic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSObject ToJSObject(this JSValue value)
        {
            if (value is JSObject @object)
                return @object;
            if (value.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
            throw JSContext.Current.NewTypeError(JSError.Parameter_is_not_an_object);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryAsObjectThrowIfNullOrUndefined(this JSValue value, out JSObject @object)
        {
            if (value.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
            @object = value as JSObject;
            return @object != null;
        }

        [Static("create")]
        internal static JSValue StaticCreate(in Arguments a)
        {
            var p = a.Get1();
            if (!(p is JSObject proto))
            {
                if (!p.IsNull)
                    throw JSContext.Current.NewTypeError("Object prototype may only be an Object or null");
                proto = JSContext.Current.ObjectPrototype;
            }
            return new JSObject(proto);
        }

        [Static("assign")]
        internal static JSValue Assign(in Arguments a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject firstObject))
                return first;
            ref var firstOwnProperties = ref firstObject.GetOwnProperties();
            for (var i = 1; i < a.Length; i++)
            {
                var ai = a.GetAt(i);
                if (!(ai is JSObject @object))
                    continue;
                ref var props = ref @object.GetOwnProperties(false);
                if (props.IsEmpty)
                    continue;
                var en = props.GetEnumerator();
                while (en.MoveNext(out var keyString, out var value))
                {
                    firstOwnProperties.Put(keyString.Key, @object.GetValue(value));
                }
            }
            return first;
        }

        [Static("entries")]
        internal static JSValue StaticEntries(in Arguments a)
        {
            var target = a.Get1();
            if (target.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
            if (!target.IsObject)
                return new JSArray();
            var r = new JSArray();
            ref var rElements = ref r.CreateElements();
            var ownEntries = target.GetElementEnumerator();
            while(ownEntries.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                    continue;
                rElements.Put(r._length++,
                        new JSArray(new JSString(index.ToString()), item)
                    ); 
            }
            var en = (target as JSObject).GetOwnProperties(false).GetEnumerator();
            while (en.MoveNext(out var key, out var property))
            {
                rElements.Put(r._length++,
                        new JSArray(key.ToJSValue(), target.GetValue(property))
                    );
            }
            return r;
        }

        [Static("freeze")]
        internal static JSValue Freeze(in Arguments a)
        {
            throw new NotImplementedException();
        }

        [Static("defineProperties")]
        internal static JSValue DefineProperties(in Arguments a)
        {
            var (a0, a1) = a.Get2();
            if (!(a0 is JSObject target))
                throw JSContext.Current.NewTypeError("Object.defineProperty called on non-object");
            var pds = a1;
            if (pds.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(pds is JSObject pdObject))
                return target;
            if (!target.IsExtensible())
                throw JSContext.Current.NewTypeError("Object is not extensible");

            var ownElements = pdObject.GetElementEnumerator();
            while (ownElements.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                    continue;
                JSObject.InternalAddProperty(target, index, item);
            }

            var properties = pdObject.GetOwnProperties(false).GetEnumerator();
            while (properties.MoveNext(out var keyString, out var property))
            {
                JSObject.InternalAddProperty(target, keyString, target.GetValue(property));
            }

            return target;
        }

        [Static("defineProperty")]
        internal static JSValue DefineProperty(in Arguments a)
        {
            var (target, key, desc) = a.Get3();
            if (!(target is JSObject targetObject))
                throw JSContext.Current.NewTypeError("Object.defineProperty called on non-object");
            if (!targetObject.IsExtensible())
                throw JSContext.Current.NewTypeError("Object is not extensible");
            if (!(desc is JSObject pd))
                throw JSContext.Current.NewTypeError("Property Description must be an object");
            var k = key.ToKey();
            if (k.IsSymbol)
            {
                JSObject.InternalAddProperty(targetObject, k.Symbol, pd);
            }
            else
            {
                if (!k.IsUInt)
                {
                    JSObject.InternalAddProperty(targetObject, k.KeyString, pd);
                }
                else
                {
                    JSObject.InternalAddProperty(targetObject, k.Index, pd);
                }
            }
            return target;
        }

        [Static("entries")]
        internal static JSValue Entries(in  Arguments a)
        {
            var @this = a.This;
            if(@this.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.NotIterable("undefined"));
            var obj = @this as JSObject;
            var r = new JSArray();

            var vp = new PropertySequence.ValueEnumerator(obj, false);
            while(vp.MoveNext(out var value, out var key))
            {
                r[r._length++] = new JSArray(value, key.ToJSValue());
            }
            return r;
        }


        [Static("fromEntries")]

        internal static JSValue FromEntries(in Arguments a)
        {
            var v = a.Get1();
            if (v.IsNullOrUndefined)
            {
                throw JSContext.Current.NewTypeError(JSTypeError.NotIterable("undefined"));
            }
            var r = new JSObject();
            if ((v is JSArray va))
            {

                ref var vaElements = ref va.GetElements();
                for (uint i = 0; i < va._length; i++)
                {
                    var vi = vaElements[i];
                    if (!(vi.value is JSArray ia))
                        throw JSContext.Current.NewTypeError(JSTypeError.NotEntry(vi));
                    var first = ia[0];
                    var second = ia[1];
                    var key = first.ToKey();
                    if (key.IsSymbol)
                    {
                        r.DefineProperty(key.Symbol, JSProperty.Property(second,
                            JSPropertyAttributes.EnumerableConfigurableValue));
                    }
                    else
                    {
                        if (key.IsUInt)
                        {
                            r.DefineProperty(key.Index, JSProperty.Property(second,
                                JSPropertyAttributes.EnumerableConfigurableValue));
                        }
                        else
                        {
                            r.DefineProperty(key.KeyString, JSProperty.Property(key.KeyString, second,
                                JSPropertyAttributes.EnumerableConfigurableValue));
                        }
                    }
                }
            }
            return r;
        }

        [Static("is")]
        internal static JSValue Is(in Arguments a)
        {
            var (first, second) = a.Get2();
            return first.Is(second);
        }

        [Static("isExtensible")]
        internal static JSValue IsExtensible(in Arguments a)
        {
            if (a.Get1() is JSObject @object && @object.IsExtensible())
            {
                return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Static("isFrozen")]
        internal static JSValue IsFrozen(in Arguments a)
        {
            if ((a.Get1() is JSObject @object) && @object.IsFrozen())
                return JSBoolean.True;
            return JSBoolean.False;
        }

        [Static("isSealed")]
        internal static JSValue IsSealed(in Arguments a)
        {
            if ((a.Get1() is JSObject @object) && @object.IsSealed())
                return JSBoolean.True;
            return JSBoolean.False;
        }

        [Static("keys")]
        internal static JSValue Keys(in Arguments a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            var en = jobj.GetAllKeys(true, false);
            var r = new JSArray();
            ref var e = ref r.GetElements();
            while (en.MoveNext(out var hasValue, out var value, out var index)) {
                if (hasValue)
                {
                    e.Put(r._length++, value);
                }
            }
            return r;
        }

        [Static("preventExtensions")]
        internal static JSValue PreventExtensions(in Arguments a)
        {
            var first = a.Get1();
            if (!(first is JSObject @object))
                return first;
            @object.status |= ObjectStatus.NonExtensible;
            return @object;
        }

        [Static("seal")]

        internal static JSValue Seal(in Arguments a)
        {
            var first = a.Get1();
            if (!(first is JSObject @object))
                return first;
            @object.status |= ObjectStatus.Sealed;
            @object.GetOwnProperties().Update((x, v) =>
            {
                // v.Attributes &= ~(JSPropertyAttributes.Configurable);
                v = new JSProperty(v.key, v.get, v.set, v.value, v.Attributes & (~JSPropertyAttributes.Configurable));
                return (true, v);
            });
            return first;
        }

        [Static("setPrototypeOf")]
        internal static JSValue SetPrototypeOf(in Arguments a)
        {
            var (first, second) = a.Get2();
            if (!(first is JSObject @object))
                return first;
            if (!@object.IsExtensible())
                throw JSContext.Current.NewTypeError("Object is not extensible");
            if (second is JSObject proto)
                first.BasePrototypeObject = proto;
            return first;
        }

        [Static("values")]
        internal static JSValue Values(in Arguments a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject target))
                return new JSArray();
            var r = new JSArray();
            ref var rElements = ref r.CreateElements();
            var ownEntries = target.GetElementEnumerator();
            while (ownEntries.MoveNext(out var hasValue, out var item, out var index))
            {
                if(!hasValue)
                {
                    continue;
                }
                rElements.Put(r._length++,
                        item
                    );
            }
            var en = target.GetOwnProperties(false).GetEnumerator();
            while (en.MoveNext(out  var property))
            {
                rElements.Put(r._length++,
                        target.GetValue(property)
                    );
            }
            return r;
        }

        [Static("getOwnPropertyDescriptor")]
        internal static JSValue GetOwnPropertyDescriptor(in Arguments a)
        {
            var (first, name) = a.Get2();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return JSUndefined.Value;
            var key = name.ToKey(false);
            JSProperty p;
            if (key.IsUInt)
            {
                // check for typedArray..
                if (first is TypedArray ta)
                {
                    var v = ta[key.Index];
                    if (v.IsUndefined)
                        return JSUndefined.Value;

                    p = JSProperty.Property(v, JSPropertyAttributes.Enumerable | JSPropertyAttributes.Value);
                }
                else
                {
                    p = jobj.GetInternalProperty(key.Index, false);
                }
            } else {
                p = jobj.GetInternalProperty(key.KeyString, false);
            }
            if (!p.IsEmpty)
                return p.ToJSValue();
            return JSUndefined.Value;
        }

        [Static("getOwnPropertyDescriptors")]
        internal static JSValue GetOwnPropertyDescriptors(in Arguments a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            var r = new JSObject();
            ref var p = ref r.GetOwnProperties(true);
            var en = jobj.GetOwnProperties(false).GetEnumerator();
            while(en.MoveNext(out var key, out var property))
            {
                p.Put(key.Key) = property;    
            }
            return r;
        }

        /// <summary>
        /// The Object.getOwnPropertyNames() method returns an array of all properties 
        /// (including non-enumerable properties except for those which use Symbol) 
        /// found directly in a given object.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [Static("getOwnPropertyNames")]
        internal static JSValue GetOwnPropertyNames(in Arguments a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            var r = new JSArray(jobj.GetAllKeys(false, false));
            return r;
        }

        [Static("getOwnPropertySymbols")]
        internal static JSValue GetOwnPropertySymbols(in Arguments a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            ref var symbols = ref jobj.GetSymbols();
            var keys = symbols.AllValues().Select(x => KeyStrings.GetJSString( x.Value.key));
            return new JSArray(keys);
        }

        [Static("getPrototypeOf")]
        internal static JSValue GetPrototypeOf(in Arguments a)
        {
            var target = a.Get1();
            if (target is JSPrimitive primitive)
                primitive.ResolvePrototype();
            var p = target.prototypeChain?.@object ?? JSNull.Value;
            return p;
        }

    }
}
