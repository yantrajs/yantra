using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
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
            var (first,second) = a.Get2();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject firstObject))
                return first;
            if (!(second is JSObject @object))
                return first;
            if (@object.ownProperties != null)
            {
                var en = new PropertySequence.Enumerator(@object.ownProperties);
                while(en.MoveNext())
                {
                    var item = en.Current;
                    firstObject.ownProperties[item.key.Key] = item;
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
            var ownEntries = target.GetElementEnumerator();
            while(ownEntries.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                    continue;
                r.elements[r._length++] = JSProperty.Property(
                        new JSArray(new JSString(index.ToString()), item)
                    ); 
            }
            var en = new PropertySequence.Enumerator((target as JSObject).ownProperties);
            while (en.MoveNext())
            {
                r.elements[r._length++] = JSProperty.Property(
                        new JSArray(en.Current.key.ToJSValue(), target.GetValue(en.Current))
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

            var properties = new PropertySequence.Enumerator(pdObject.ownProperties);
            while (properties.MoveNext())
            {
                JSObject.InternalAddProperty(target, properties.Current.key, target.GetValue(properties.Current));
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
                    JSObject.InternalAddProperty(targetObject, k, pd);
                }
                else
                {
                    JSObject.InternalAddProperty(targetObject, k.Key, pd);
                }
            }
            return target;
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
                for (uint i = 0; i < va._length; i++)
                {
                    var vi = va.elements[i];
                    if (!(vi.value is JSArray ia))
                        throw JSContext.Current.NewTypeError(JSTypeError.NotEntry(vi));
                    var first = ia[0];
                    var second = ia[1];
                    if (first is JSSymbol symbol)
                    {
                        r.DefineProperty(symbol, JSProperty.Property(symbol.Key, second,
                            JSPropertyAttributes.EnumerableConfigurableValue));
                    }
                    else
                    {
                        var key = first.ToKey();
                        r.DefineProperty(key, JSProperty.Property(key, second,
                            JSPropertyAttributes.EnumerableConfigurableValue));
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
            return new JSArray(jobj.GetAllKeys(true, false));
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
            @object.ownProperties.Update((x, v) =>
            {
                v.Attributes &= ~(JSPropertyAttributes.Configurable);
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
            first.prototypeChain = second as JSObject ?? first.prototypeChain;
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
            var ownEntries = target.GetElementEnumerator();
            while (ownEntries.MoveNext(out var hasValue, out var item, out var index))
            {
                if(!hasValue)
                {
                    continue;
                }
                r.elements[r._length++] = JSProperty.Property(
                        item
                    );
            }
            var en = new PropertySequence.Enumerator(target.ownProperties);
            while (en.MoveNext())
            {
                r.elements[r._length++] = JSProperty.Property(
                        target.GetValue(en.Current)
                    );
            }
            return r;
        }

        [Static("getOwnPropertyDescriptor")]
        internal static JSValue GetOwnPropertyDescriptor(in Arguments a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            throw new NotImplementedException();
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
            var en = new PropertySequence.Enumerator(jobj.ownProperties);
            while(en.MoveNext())
            {
                var x = en.Current;
                var p = JSProperty.Property(x.key, x.ToJSValue());
                r.elements[x.key.Key] = p;
            }
            return r;
        }

        [Static("getOwnPropertyNames")]
        internal static JSValue GetOwnPropertyNames(in Arguments a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            var en = new PropertySequence.Enumerator(jobj.ownProperties);
            var r = new JSArray();
            while (en.MoveNext())
            {
                r.Add(en.Current.ToJSValue());
            }
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
            var keys = jobj.symbols.AllValues.Select(x => x.Value.key.ToJSValue());
            return new JSArray(keys);
        }

        [Static("getPrototypeOf")]
        internal static JSValue GetPrototypeOf(in Arguments a)
        {
            var target = a.Get1();
            var p = target.prototypeChain;
            return p;
        }

    }
}
