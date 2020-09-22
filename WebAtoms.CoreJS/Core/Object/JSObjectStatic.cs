using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
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
        internal static JSValue StaticCreate(JSValue t,params JSValue[] a)
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
        internal static JSValue Assign(JSValue t,params JSValue[] a)
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
                foreach (var item in @object.ownProperties.AllValues())
                {
                    firstObject.ownProperties[item.Key] = item.Value;
                }
            }
            return first;
        }

        [Static("entries")]
        internal static JSValue StaticEntries(JSValue t,params JSValue[] a)
        {
            var target = a[0];
            if (target.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
            if (!target.IsObject)
                return new JSArray();
            var r = new JSArray();
            foreach(var (index, key, property) in target.GetOwnEntries())
            {
                if (index != -1)
                {
                    r.elements[r._length++] = JSProperty.Property(
                        new JSArray(new JSNumber(index), property));
                } else
                {
                    r.elements[r._length++] = JSProperty.Property(
                        new JSArray(key.ToJSValue(), property));
                }
            }
            return r;
        }

        [Static("freeze")]
        internal static JSValue Freeze(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("defineProperties")]
        internal static JSValue _DefineProperties(JSValue t,params JSValue[] a)
        {
            if (!(a[0] is JSObject target))
                throw JSContext.Current.NewTypeError("Object.defineProperty called on non-object");
            var pds = a[1];
            if (pds.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(pds is JSObject pdObject))
                return target;

            foreach(var (index, key, property) in pdObject.GetOwnEntries())
            {
                if (index != -1)
                {
                    JSObject.InternalAddProperty(target, (uint)index, property);
                }
                else
                {
                    JSObject.InternalAddProperty(target, key, property);
                }
            }

            return target;
        }

        [Static("defineProperty")]
        internal static JSValue _DefineProperty(JSValue t,params JSValue[] a)
        {
            var (target, key, desc) = a.Get3();
            if (!(target is JSObject targetObject))
                throw new JSException("Object.defineProperty called on non-object");
            if (!(desc is JSObject pd))
                throw new JSException("Property Description must be an object");
            var k = key.ToKey();
            if (!k.IsUInt)
            {
                JSObject.InternalAddProperty(targetObject, k, pd);
            } else
            {
                JSObject.InternalAddProperty(targetObject, k.Key, pd);
            }
            return target;
        }


        [Static("fromEntries")]

        internal static JSValue _FromEntries(JSValue t,params JSValue[] a)
        {
            var v = a.Get1();
            if (v.IsNullOrUndefined)
            {
                throw JSContext.Current.NewTypeError(JSTypeError.NotIterable("undefined"));
            }
            var r = new JSObject();
            if ((v is JSArray va))
            {
                foreach(var item in va.elements.AllValues)
                {
                    var vi = item.Value;
                    if (!(vi.value is JSArray ia))
                        throw JSContext.Current.NewTypeError(JSTypeError.NotEntry(vi));
                    var first = ia[0].ToString();
                    var second = ia[1];
                    r.DefineProperty(first, JSProperty.Property(first, second,
                        JSPropertyAttributes.EnumerableConfigurableValue));
                }
            }
            return r;
        }

        [Static("is")]

        internal static JSValue _Is(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("isExtensible")]
        internal static JSValue _IsExtensible(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("isFrozen")]
        internal static JSValue _IsFrozen(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("isSealed")]
        internal static JSValue _IsSealed(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("keys")]
        internal static JSValue _Keys(JSValue t,params JSValue[] a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            return new JSArray(jobj.GetAllKeys(true, false));
        }

        [Static("preventExtensions")]
        internal static JSValue _PreventExtensions(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("seal")]

        internal static JSValue _Seal(JSValue t,params JSValue[] a)
        {
            var first = a.Get1();
            if (!(first is JSObject @object))
                return first;
            @object.ownProperties.Update((x, v) =>
            {
                v.Attributes &= ~(JSPropertyAttributes.Configurable);
                return (true, v);
            });
            return first;
        }

        [Static("setPrototypeOf")]
        internal static JSValue _SetPrototypeOf(JSValue t,params JSValue[] a)
        {
            var first = a.Get1();
            if (!(first is JSObject))
                return first;
            first.prototypeChain = a[1] as JSObject;
            return first;
        }

        [Static("values")]
        internal static JSValue _Values(JSValue t,params JSValue[] a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            return new JSArray(jobj.GetOwnEntries().Select(x => x.value));
        }

        [Static("getOwnPropertyDescriptor")]
        internal static JSValue _GetOwnPropertyDescriptor(JSValue t,params JSValue[] a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            return t;
        }

        [Static("getOwnPropertyDescriptors")]
        internal static JSValue _GetOwnPropertyDescriptors(JSValue t,params JSValue[] a)
        {
            var first = a.Get1();
            if (first.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            return new JSObject(jobj.ownProperties.AllValues().Select(x => 
                JSProperty.Property(x.Value.key, x.Value.ToJSValue())
            ));
        }

        [Static("getOwnPropertyNames")]
        internal static JSValue _GetOwnPropertyNames(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("getOwnPropertySymbols")]
        internal static JSValue _GetOwnPropertySymbols(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("getPrototypeOf")]
        internal static JSValue _GetPrototypeOf(JSValue t,params JSValue[] a)
        {
            var target = a.Get1();
            var p = target.prototypeChain;
            return p;
        }

    }
}
