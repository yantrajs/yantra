using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Extensions;

namespace YantraJS.Core.Objects
{
    public class JSReflect: JSObject
    {

        [Static("apply", Length = 3)]
        public static JSValue Apply(in Arguments a)
        {
            var (target, thisArgument, arguments) = a.Get3();
            var fx = target as JSFunction;
            return fx.InvokeFunction(Arguments.ForApply(thisArgument, arguments));
        }

        [Static("construct", Length = 2)]
        public static JSValue Construct(in Arguments a)
        {
            var (target, arguments, newTarget) = a.Get3();
            newTarget = newTarget.IsUndefined ? target : newTarget;
            var fx = target as JSFunction;
            return fx.CreateInstance(Arguments.ForApply(new JSObject(), arguments));
        }

        [Static("defineProperty", Length = 3)]
        public static JSValue DefineProperty(in Arguments a)
        {
            var (target, propertyKey, attributes) = a.Get3();
            if (!(target is JSObject targetObject))
                return JSBoolean.False;
            if (!targetObject.IsExtensible())
                return JSBoolean.False;
            if (!(attributes is JSObject pd))
                return JSBoolean.False;
            var k = propertyKey.ToKey();
            if (k.IsSymbol)
            {
                JSObject.InternalAddProperty(targetObject, k.JSValue as JSSymbol, pd);
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
            return JSBoolean.True;
        }

        [Static("deleteProperty", Length = 2)]
        public static JSValue DeleteProperty(in Arguments a)
        {
            var (target, propertyKey) = a.Get2();
            if (!(target is JSObject @object))
                return JSBoolean.False;
            var key = propertyKey.ToKey();
            if (key.IsSymbol)
            {
                ref var symbols = ref @object.GetSymbols();
                return symbols.RemoveAt(key.Key) ? JSBoolean.True : JSBoolean.False;
            }
            if (key.IsUInt)
            {
                ref var elements = ref @object.GetElements();
                return elements.RemoveAt(key.Key) ? JSBoolean.True : JSBoolean.False;
            }
            ref var properties = ref @object.GetOwnProperties();
            return properties.RemoveAt(key.Key) ? JSBoolean.True : JSBoolean.False;
        }

        [Static("get", Length = 2)]
        public static JSValue Get(in Arguments a)
        {
            var (target, propertyKey, receiver) = a.Get3();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            receiver = receiver.IsUndefined ? target : receiver;
            var key = propertyKey.ToKey();
            ref var p = ref JSProperty.Empty;
            if (key.IsSymbol)
            {
                p = @object.GetInternalProperty(key.ToJSValue() as JSSymbol);
            } else
            {
                if (key.IsUInt)
                {
                    p = @object.GetInternalProperty(key.Key);
                } else
                {
                    p = @object.GetInternalProperty(in key);
                }
            }
            if (p.IsEmpty)
                return JSUndefined.Value;
            if (p.IsValue)
                return p.value;
            return p.get.InvokeFunction(new Arguments(receiver));
        }

        [Static("getOwnPropertyDescriptor", Length = 2)]
        public static JSValue GetOwnPropertyDescriptor(in Arguments a)
        {
            var (target, propertyKey) = a.Get2();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            var key = propertyKey.ToKey();
            ref var p = ref JSProperty.Empty;
            if (key.IsSymbol)
            {
                p = @object.GetInternalProperty(key.ToJSValue() as JSSymbol);
            }
            else
            {
                if (key.IsUInt)
                {
                    p = @object.GetInternalProperty(key.Key);
                }
                else
                {
                    p = @object.GetInternalProperty(in key);
                }
            }
            if (p.IsEmpty)
                return JSUndefined.Value;
            return p.ToJSValue();
        }

        [Static("getPrototypeOf")]
        public static JSValue GetPrototypeOf(in Arguments a)
        {
            var target = a.Get1();
            if(!(target is JSObject))
                throw JSContext.Current.NewTypeError($"Not an object");
            var p = target.prototypeChain?.@object;
            if (p == target || p == null)
                return JSNull.Value;
            return p;
        }

        [Static("has", Length = 2)]
        public static JSValue Has(in Arguments a)
        {
            var (target, propertyKey, receiver) = a.Get3();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            var key = propertyKey.ToKey();
            ref var p = ref JSProperty.Empty;
            if (key.IsSymbol)
            {
                p = @object.GetInternalProperty(key.ToJSValue() as JSSymbol);
            }
            else
            {
                if (key.IsUInt)
                {
                    p = @object.GetInternalProperty(key.Key);
                }
                else
                {
                    p = @object.GetInternalProperty(in key);
                }
            }
            if (p.IsEmpty)
                return JSBoolean.False;
            return JSBoolean.True;
        }

        [Static("isExtensible", Length = 1)]
        public static JSValue IsExtensible(in Arguments a)
        {
            var target = a.Get1();
            if(!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            return @object.IsExtensible() ? JSBoolean.True : JSBoolean.False;
        }

        [Static("ownKeys", Length = 1)]
        public static JSValue OwnKeys(in Arguments a)
        {
            var target = a.Get1();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            // return @object.IsExtensible() ? JSBoolean.True : JSBoolean.False;
            var r = new JSArray();
            var een = @object.GetElementEnumerator();
            while(een.MoveNext(out var hasValue, out var value, out var index))
            {
                if (hasValue)
                    r.Add(new JSNumber(index));
            }
            var en = new PropertySequence.Enumerator(@object.GetOwnProperties(false));
            while (en.MoveNext())
            {
                r.Add(en.Current.ToJSValue());
            }
            ref var symbols = ref @object.GetSymbols();
            foreach(var e in symbols.AllValues())
            {
                r.Add(e.Value.ToJSValue());
            }
            return r;
        }

        [Static("preventExtensions", Length = 1)]
        public static JSValue PreventExtensions(in Arguments a)
        {
            var target = a.Get1();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            @object.status |= ObjectStatus.NonExtensible;
            return JSBoolean.True;
        }

        [Static("set", Length = 2)]
        public static JSValue Set(in Arguments a)
        {
            var (target, propertyKey, value, receiver) = a.Get4();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            receiver = receiver.IsUndefined ? target : receiver;
            var key = propertyKey.ToKey();
            ref var p = ref JSProperty.Empty;
            if (key.IsSymbol)
            {
                var symbol = key.ToJSValue() as JSSymbol;
                p = @object.GetInternalProperty(symbol, false);
                if (p.IsProperty)
                {
                    p.set.InvokeFunction(new Arguments(receiver, value));
                    return JSBoolean.True;
                }
                ref var symbols = ref @object.GetSymbols();
                symbols.Save(symbol.Key.Key, JSProperty.Property(key, value));
                return JSBoolean.True;
            }
            else
            {
                if (key.IsUInt)
                {
                    p = @object.GetInternalProperty(key.Key, false);
                    if (p.IsProperty)
                    {
                        p.set.InvokeFunction(new Arguments(receiver, value));
                        return JSBoolean.True;
                    }
                    ref var elements = ref @object.GetElements(true);
                    elements[key.Key] = JSProperty.Property(key, value);
                    return JSBoolean.True;
                }
                else
                {
                    p = @object.GetInternalProperty(in key, false);
                    if (p.IsProperty)
                    {
                        p.set.InvokeFunction(new Arguments(receiver, value));
                        return JSBoolean.True;
                    }
                    ref var properties = ref @object.GetOwnProperties(true);
                    properties[key.Key] = JSProperty.Property(key, value);
                    return JSBoolean.True;
                }
            }
        }

        [Static("setPrototypeOf")]
        public static JSValue SetPrototypeOf(in Arguments a)
        {
            var (target,p) = a.Get2();
            if (!(target is JSObject))
                throw JSContext.Current.NewTypeError($"Not an object");
            if(!(p is JSObject prototype))
                throw JSContext.Current.NewTypeError($"Not an object");
            target.prototypeChain = prototype?.PrototypeObject;
            return p;
        }
    }

}
